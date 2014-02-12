var currentId;
var currentRateTableId;
var currentRateTableChangeId;
var currentChangeId;
var currentExceptionId;
var currentMode;
var editor;
var steppedInto = false;
var detailLevel = 0;
var readOnlyMode = false;

$(function () {
    initApp();

});
// Initialize the app
function initApp() {
    initSidebar();
    initCodeEditor();
    loadRules(rulesMode);
    initToolbar();
    $('#middle-center').click(function () { outerLayout.close('south'); });


    //$('.toolbar-button').button();

}
function initToolbar() {

    $('#toolbar-button-code').click(function () { loadingScreen(); showCode(function (d) { editor.setValue(d); $.unblockUI(); }); });

}
function lightbarToToolbar() {
    $('.light-bar').children().appendTo('#toolbar');
    $('.light-bar').remove();
}
function HandleReadOnly() {
    var hideShowSelector = '#add-rate-table-row, .edit-rate-table-row, .delete-rate-table-row, #add-rule, #edit-rule, #delete-rule, #toolbar-button-build, #save-code,#add-exception,#add-rate-table, #import-rate-table';
    if (readOnlyMode) {
        $(hideShowSelector).hide();
    } else {

    }

}
function LookupTableKey(id, f) { if (readOnlyMode) return; $.get('./Rating/LookupTableKey', { ajax: true, id: id, factorTableId: currentRateTableId, factorTableChangeId: currentRateTableChangeId }, f); }
function importRatingTable(id, f) { if (readOnlyMode) return; $.get('./Rating/ImportRatingTable', { ajax: true }, f); }
function importRatingTableData(form, f) { if (readOnlyMode) return; $.post('./Rating/ImportRatingTableData?ajax=true', $(form).serialize(), f); }
function showCode(f) { $.get('./Rating/ShowCode', { ajax: true }, f); }
function publishAssembly(effectiveDate, f) { $.get('./Rating/PublishAssembly', { ajax: true, effectiveDate: effectiveDate }, f); }
function stepIntoRule(id, f) { $.get('./Rating/StepIntoRule', { ajax: true, id: id }, f); steppedInto = id != ''; }

function rateTableColumn(id, f) { $.get('./Rating/RateTableColumn', { ajax: true, id: id, factorTableId: currentRateTableId, factorTableChangeId: currentRateTableChangeId }, f); }
function rateTableRow(id, f) { $.get('./Rating/RateTableRow', { ajax: true, id: id, factorTableId: currentRateTableId, factorTableChangeId: currentRateTableChangeId }, f); }
function rateTableProperties(id, f) { $.get('./Rating/RateTableProperties', { ajax: true, id: id }, f); }


function getRule(id, f) { $.get('./Rating/Rule', { ajax: true, parentId: currentChangeId, ruleId: id }, f); }
function getRatingRuleException(id, f) { $.get('./Rating/RatingRuleException', { ajax: true, id: id }, f); }

function setBaseKeys(id, f) { $.get('./Rating/SetBaseKeys', { ajax: true, key: id }, f);  }
function assignParent(ruleId, parentId, f) { if (readOnlyMode) return; $.get('./Rating/AssignRuleParent', { ajax: true, parentId: parentId, ruleId: ruleId }, f); }
function deleteRule(ruleId, f) { if (readOnlyMode) return; $.get('./Rating/DeleteRule', { ajax: true, ruleId: ruleId }, f); }
function deleteRatingRuleException(id, f) { if (readOnlyMode) return; $.get('./Rating/DeleteRatingRuleException', { ajax: true, id: id }, f); }

function deleteLookupTableKey(id, f) { if (readOnlyMode) return; $.get('./Rating/DeleteLookupTableKey', { ajax: true, id: id }, function (d) { f(d); loadRatingTable(currentRateTableId, ratingTableMode); }); }
function deleteRateTableColumn(id, f) { if (readOnlyMode) return; $.get('./Rating/DeleteRateTableColumn', { ajax: true, id: id }, function (d) { f(d); loadRatingTable(currentRateTableId, ratingTableMode); }); }
function deleteRateTableRow(id, f) { if (readOnlyMode) return; $.get('./Rating/DeleteRateTableRow', { ajax: true, factorTableId: currentRateTableId, rowId: id }, function (d) { f(d); loadRatingTable(currentRateTableId, ratingTableMode); }); }

function loadRules(f) { $.get('./Rating/Rules', { ajax: true }, f); }
function loadAssemblies(f) { $.get('./Rating/Assemblies', { ajax: true }, f); }
function loadExceptions(f) { $.get('./Rating/BaseKeys', { ajax: true }, f); }
function loadRatingTables(f) { $.get('./Rating/RatingTables', { ajax: true }, f); }


function loadRatingTable(id, f) { $.get('./Rating/RateTable', { ajax: true, id: id }, f); currentRateTableId = id; }
function testRatingForm(f) { $.get('./Rating/TestRatingForm', { ajax: true }, f); }
function testRating(form, f) { $.get('./Rating/TestRating?ajax=true', $(form).serialize(), f); outerLayout.hide('south'); }
function compileRating(effectiveDate, f) {
    $.blockUI({ message: '<b><img src="./../../Content/ajax-loader.gif" /> Compiling this may take a moment...</b>' });
    $.get('./Rating/CompileRating', { ajax: true, effectiveDate: effectiveDate }, f);
}
function getRuleExpression(ruleId, f) { $.get('./Rating/RuleExpression', { ruleId: ruleId }, f); }
function getRatingRuleExceptionExpression(id, f) { $.get('./Rating/RatingRuleExceptionExpression', { id: id }, f); }

function saveRuleExpression(ruleId, expression, f) { if (readOnlyMode) return; $.get('./Rating/SaveRuleExpression', { ruleId: ruleId, expression: expression, ajax: true }, f); outerLayout.close('south'); }
function saveRatingRuleExceptionExpression(id, expression, f) { if (readOnlyMode) return; $.get('./Rating/SaveRatingRuleExceptionExpression', { id: id, expression: expression, ajax: true }, f); outerLayout.close('south'); }
function saveRatingRuleException(form, f) { if (readOnlyMode) return; $.get('./Rating/SaveRatingRuleException?ajax=true', $(form).serialize(), f); }
function saveRule(f) { if (readOnlyMode) return; $.get('./Rating/SaveRule?ajax=true', $('#mainform').serialize(), f); }
function saveRateTable(form, f) { if (readOnlyMode) return; $.get('./Rating/SaveRateTable?ajax=true', $(form).serialize(), f); }
function saveLookupTableKey(form, f) { if (readOnlyMode) return; $.get('./Rating/SaveLookupTableKey?ajax=true', $(form).serialize(), f); }
function saveRateTableColumn(form, f) { if (readOnlyMode) return; $.get('./Rating/SaveRateTableColumn?ajax=true', $(form).serialize(), f); }
function saveRateTableRow(form, f) { if (readOnlyMode) return; $.get('./Rating/SaveRateTableRow?ajax=true', $(form).serialize(), function () { f(); loadRatingTable(currentRateTableId, ratingTableMode); }); }
function showDialog(f) { return function (d) { var a = $(d); a.find('input[type="submit"]').first().click(function () { f(this); a.dialog('close'); return false; }); a.dialog({ close: function (ev, ui) { $(this).remove(); } }); }; }
// Import 
function loadRatingImport(f) { $.get('./Import/ChooseFile', { ajax: true }, f); }
function loadImportFiles(f) { $.get('./Import/ImportFiles', { ajax: true }, f); }
function selectImportFile(filename, f) { $.get('./Import/SelectFile', { filename: filename,  ajax: true }, f); }
function completeImport(f) { if (readOnlyMode) return; $.post('./Import/CompleteImport?ajax=true', $('#importform').serialize(), f); }

function visitRule(id) {
    rulesMode(); $('.expression-tree-node[ruleId="' + id + '"]').trigger("click");
}
function exceptionsMode(d) {
    currentMode = 0;
    if (d != null) {
        $('#rating-exceptions-content').html(d);
        initExceptions();
    }
    $('.selected').removeClass("selected");
    $('#rating-exceptions-link').addClass("selected");
    $('.section').hide();
    $('#rating-exceptions-content').show();
    window.outerLayout.hide('south');
    //window.outerLayout.hide('west');
    HandleReadOnly();
}
function rulesMode(d) {

    currentMode = 1;
    if (d != null) {
        $('#section-rules').html(d);
        initRatingRules();
    }
    $('.section').hide();
    $('#rating-rules-content').show();
    $('#section-rules').show();
    $('.selected').removeClass("selected");
    $('#rating-rules-link').addClass("selected");
    window.outerLayout.show('south');
    filter();
    HandleReadOnly();

}
function ratingAssembliesMode(d) {
    currentMode = 3;
    if (d != null) {
        $('#rating-assemblies-content').html(d);

    }
    $('.section').hide();
    $('#rating-assemblies-content').show();
    $('.selected').removeClass("selected");
    $('#rating-assemblies-link').addClass("selected");
    HandleReadOnly();
}
function ratingTableMode(d) {
    currentMode = 2;
    tablesMode();
    if (d != null) {
        $('#rating-table-form').html(d);

    }
    HandleReadOnly();
}
function tablesMode(d) {
    currentMode = 5;
    if (d != null) {
        $('#rating-tables-list').html(d);
        initRatingRules();
    }
    $('.section').hide();
    $('#rating-tables-content').show();
    $('.selected').removeClass("selected");
    $('#rating-tables-link').addClass("selected");
    window.outerLayout.hide('south');
    HandleReadOnly();
}

function importFilesMode(d) {
    if (d != null) {
        $('#rating-file-list').html(d);
    }
    $('.section').hide();
    $('#rating-tables-import-content').show();
    $('.selected').removeClass("selected");
    $('#rating-tables-import-link').addClass("selected");
    window.outerLayout.hide('south');
    HandleReadOnly();
}
function importOptionsMode(d) {
    if (d != null) {
        $('#rating-tables-import-options').html(d);
    }
    $('.section').hide();
    $('#rating-tables-import-content').show();
    $('.selected').removeClass("selected");
    $('#rating-tables-import-link').addClass("selected");
    window.outerLayout.hide('south');
    HandleReadOnly();
}

function propertiesMode() {

    $('.section').hide();
    $('#rating-rules-content').show();
    $('#section-rule-properties').show();
    HandleReadOnly();
}
function testMode(d) {

    if (d != null) {
        $('#rating-test-content').html(d);

    }
    $('.section').hide();
    $('#rating-test-content').show();
}
function initExceptions() {
    $(".exception-item").dblclick(function () {
        $(".ui-selected").removeClass("ui-selected");
        $(this).toggleClass("ui-selected");

        currentExceptionId = $(this).attr('id');

        getRatingRuleException(currentExceptionId, showDialog(function (d) { saveRatingRuleException('#ratingRuleExceptionForm', exceptionsMode); }));
        outerLayout.show('south');
        return false;
    });
    $(".exception-item").click(function () {
        $(".ui-selected").removeClass("ui-selected");
        $(this).toggleClass("ui-selected");

        currentExceptionId = $(this).attr('id');

        getRatingRuleExceptionExpression(currentExceptionId, function (d) { editor.setValue(d); });
        outerLayout.show('south');
        return false;
    });

}
function initRatingRules() {
    $('#toolbar-button-run').click(function () { testRatingForm(showDialog(function () { testRating('#testRatingForm', testMode); })); });
    $('#toolbar-button-build').click(function () { compileRating('', function (d) { $.unblockUI(); $(d).dialog(); }); });
    $('#detail-level-normal').click(function () { detailLevel = 0; filter(); });
    $('#detail-level-full').click(function () { detailLevel = 1; filter(); });
    if (detailLevel == 0) {
        $('#detail-level-normal').attr('checked', true);
        $('#detail-level-full').attr('checked', false);
    } else {
        $('#detail-level-full').attr('checked', true);
        $('#detail-level-normal').attr('checked', false);
    }
    //$('#detail-level').buttonset();
    $('#edit-rule').click(function () {
        getRule(currentId, function (d) {
            var html = $(d);
            $('#section-rule-properties').html('');
            html.appendTo($('#section-rule-properties'));
            $('input[type="submit"]').first().click(function () { saveRule(rulesMode); return false; });
            propertiesMode();
        });
    });
    // Clickable
    $(".expression-tree-node").dblclick(function () {
        $(".ui-selected").removeClass("ui-selected");
        $(this).toggleClass("ui-selected");

        currentId = $(this).attr('id');
        currentChangeId = $(this).attr('changeId');

        stepIntoRule(currentId, rulesMode);
        return false;
    });
    $(".expression-tree-node").click(function () {
        $(".ui-selected").removeClass("ui-selected");
        $(this).toggleClass("ui-selected");

        currentId = $(this).attr('id');
        currentChangeId = $(this).attr('changeId');
        updateVariables(currentId);
        getRuleExpression(currentId, function (d) { editor.setValue(d); });
        outerLayout.open('south');
        return false;
    });
    // Dragging
    $(".expression-tree-node").draggable({
        revert: true,
        helper: 'clone',
        containment: $('#middle'),
        start: function (event, ui) { $('.expression-tree-node-drop-here').show(); },
        stop: function (event, ui) { $('.expression-tree-node-drop-here').hide(); }
    });

    $(".expression-tree-node").droppable({
        accept: '.expression-tree-node',
        drop: function (event, ui) {
            $(this).children('.rule-set-drop').removeClass('ui-droppable-over');
            $(this).removeClass('ui-droppable-over');

            var ruleId = $(ui.draggable).attr('id');
            var parentId = $(this).attr('changeId');
            assignParent(ruleId, parentId, rulesMode);


        },
        over: function (event, ui) {
            $('#insert-here').remove();
            if ($(ui.draggable).attr('ruleSetId') != null) {
                $(this).children('.rule-set-drop').addClass('ui-droppable-over');
                $(this).children('.rule-set-drop').show();
            } else {
                $(this).addClass('ui-droppable-over');
            }
        },
        out: function (event, ui) {
            $(this).removeClass('ui-droppable-over');
            $(this).children('.rule-set-drop').removeClass('ui-droppable-over');
            $(this).children('.rule-set-drop').hide();
        }
    });

    filter();
}

function switchDateVersion(effectiveDate) {
    loadDefaults(effectiveDate);
    loadRules();
}

// Link up the sidebar
function initSidebar() {
    $('#rating-rules-link').click(function () {
        loadRules(rulesMode);
    });
    $('#rating-test-link').click(function () {
        testRating(testMode);
    });
    $('#rating-tables-link').click(function () {
        loadRatingTables(tablesMode);
    });
    $('#rating-tables-import-link').click(function () {
        loadImportFiles(importFilesMode);
    });
    $('#rating-exceptions-link').click(function () {
        loadExceptions(exceptionsMode);
    });
    $('#rating-assemblies-link').click(function () {
        loadAssemblies(ratingAssembliesMode);
    });

}
function initCodeEditor() {
    //editor = CodeMirror.fromTextArea(document.getElementById("code"), { lineNumbers: true });
    CodeMirror.commands.autocomplete = function (cm) {
        CodeMirror.simpleHint(cm, CodeMirror.javascriptHint);
    };
    editor = CodeMirror.fromTextArea(document.getElementById("code"),
                      { lineNumbers: true,
                          extraKeys: {
                               "Ctrl-Space": "autocomplete"
                             
                          }
                      });
}

// Make a link an ajax link
function ajaxLink(item, selectedClass) {
    var url = $(item).attr('href');
    if ($(item).hasClass("ignore-ajax")) return;
    $(item).attr('href', '#');
    $.get(url, { ajax: true }, loadPage);
    if (selectedClass != null) {
        $("." + selectedClass).removeClass(selectedClass);
        $(item).addClass(selectedClass);
    }
}

// Loads the page in the center
function loadPage(data) {
    $('#middle-center').html(data);
    initAjax();
}

var filtering = false;
function filter() {
    if (steppedInto) return;
    if (detailLevel == 0) {
        $('.expression-tree-node').hide();
        $('.rule-container').show();
    } else {
        $('.expression-tree-node').show();
        $('.rule-container').show();
    }

    filtering = false;

}
$(function () {
    $('#filterText').keyup(filter);
    $('#hideStatic, #pageFilter').change(filter);
});