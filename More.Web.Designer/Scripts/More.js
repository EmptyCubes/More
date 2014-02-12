var currentTableGroup = null;
function filterTables(txt) {
    if (currentTableGroup == null) return;
    var val = txt.val();
    var reg = new RegExp(val, "gi");
    console.log(val, currentTableGroup.children('.table-group-tables').first().children().count);
    currentTableGroup.children('.table-group-tables').first().children().each(function (index, obj) {
        var name = $(obj).attr('table');
        console.log(name);
        if (reg.test(name)) {
            $(obj).show();
        } else {
            $(obj).hide();
        }
    });
}
function viewTableGroup(item) {
    currentTableGroup = item;
    $('#table-groups-bar').removeClass('hide');
    $('.table-group').addClass('hide');
    item.removeClass('hide');
    item.children('.table-group-tables').first().removeClass('hide');
}
function tableGroupsBack() {
    currentTableGroup = null;
    $('#table-groups-bar').addClass('hide');

    $('.table-group').removeClass('hide');
    $('.table-group-tables').addClass('hide');
}

function initializeTree() {
    $('.rule-tooltip').hide();
    //    $(".expression-tree-node").mouseenter(function () { $(this).children('.rule-tooltip').first().show(); });
    //    $(".expression-tree-node").mouseleave(function () { $('.rule-tooltip').hide(); });
    $(".expression-tree-node").click(function (e) {
        $(".ui-selected").removeClass("ui-selected");
        $(this).toggleClass("ui-selected");
        if (e.button == 0)
            RuleSelected($(this), $(this).attr('ruleId'), $(this).attr('changeId'));
        return false;
    });
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

            var ruleId = $(ui.draggable).attr('ruleId');
            var parentId = $(this).attr('changeId');
            $.get($(ui.draggable).attr('assignParentUrl') + '&parentId=' + parentId, null, function (result) { $('#middle-center').html(result); RuleBookLoaded(); });
        },
        over: function (event, ui) {
            $('#insert-here').remove();
            if ($(ui.draggable).attr('ruleId') != null) {
                $(this).addClass('ui-droppable-over');
                $(this).show();
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
    $('.expression-tree-node').contextPopup({
        title: 'Rule Menu',
        items: [
            {
                label: 'Delete', icon: 'icons/shopping-basket.png', action: function (item) {
                    var deleteUrl = $(item).closest('a').attr('deleteUrl');
                    if (deleteUrl == null || deleteUrl == '') {
                        alert("Can't delete this item.");
                        return;
                    }
                    $("<DIV></DIV>").dialog({
                        resizable: false,
                        height: 140,
                        title: "Are you sure want to delete this item?",
                        modal: true,
                        buttons: {
                            "Delete": function () {
                                $.get(deleteUrl, null, function (d) { $('#middle-center').html(d); RuleDeleted(); });
                                $(this).dialog("close");
                            },
                            Cancel: function () {
                                $(this).dialog("close");
                            }
                        }
                    });
                }
            },
            {
                label: 'Properties', icon: 'icons/receipt-text.png', action: function (item) {
                    var propertiesUrl = $(item).closest('a').attr('propertiesUrl');
                    $.get(propertiesUrl, null, function (d) { $("#modal-dialog").html(d); ShowModalDialog("Rule Properties"); });
                }
            },
            {
                label: 'Step Into', icon: 'icons/book-open-list.png', action: function (item) {
                    var stepIntoUrl = $(item).closest('a').attr('stepIntoUrl');
                    $.get(stepIntoUrl, null, function (d) { $("#middle-center").html(d); RuleBookLoaded(); });
                }
            },
            {
                label: 'Info',
                icon: 'icons/receipt-text.png',
                action: function (item) {
                    $(item).closest('a').find('table').first().dialog();
                }
            },
            {
                label: 'Clear Parent',
                icon: 'icons/receipt-text.png',
                action: function (item) {
                    var assignParentUrl = $(item).closest('a').attr('assignparenturl');
                    $.get(assignParentUrl + '&parentId=', null, function (result) { $('#middle-center').html(result); RuleBookLoaded(); });
                }
            }
        ]
    });
}

//-------------------------------
// Events
//-------------------------------

function ShowModalDialog(title) {
    $('#modal-dialog input[type="submit"]').first().hide();
    $('#modal-dialog').dialog({
        modal: true,
        title: title,
        buttons: {
            "Save": function () {
                $('#modal-dialog form').first().submit();
                $(this).dialog("close");
            }
        }
    }
    );
}

function RuleDeleted() { initializeTree(); }
// On loading success of pages
function RuleBookLoaded() { initializeTree(); }
function LookupTableLoaded() { }
function ImportLookupFormLoaded() { }
function TestRuleBookFormLoaded() { ShowModalDialog("Test Rule Book"); }
function AddLookupFormLoaded() { ShowModalDialog("Add Lookup Table"); }

function AddRuleBookFormLoaded() {
    // Hide any submit button to the form
    $('#modal-dialog input[type="submit"]').first().hide();
    $('#modal-dialog').dialog({
        modal: true,
        title: "Add RuleBook",
        buttons: {
            "Save": function () {
                $('#modal-dialog form').first().submit();
                $(this).dialog("close");
            }
        }
    }
    );
}
function AddLookupRowFormLoaded() {
    ShowModalDialog("Add Lookup Row");
}
function AddRuleFormLoaded() {
    // Hide any submit button to the form
    $('#modal-dialog input[type="submit"]').first().hide();
    $('#modal-dialog').dialog({
        modal: true,
        title: "Add Rule",
        buttons: {
            "Save": function () {
                $('#modal-dialog form').first().submit();
                $(this).dialog("close");
            }
        }
    }
    );
}
function AddAssemblyFormLoaded() { ShowModalDialog("Add Assembly"); }
// After things have been added
function TestRuleBookCompleted() { }
function AssemblyAdded() { }
function LookupAdded() { }
function LookupRowAdded() { }
function RuleBookAdded() { }
function RuleAdded() { initializeTree(); }

function RuleSelected(item, ruleId, ruleChangeId) {
    // Set the rule ids for other items
    currentRuleId = ruleId;
    currentRuleChangeId = ruleChangeId;

    var saveExpressionUrl = $(item).attr('saveExpressionUrl');
    $.get($(item).attr('getCodeUrl'), null, function (result) {
        editor.setValue(result);
        $('#code-editor').dialog(
            {
                title: "Edit Expression: " + item.attr('name'),
                width: 400,
                height: 168,
                position: { my: "center top", at: "center bottom", of: item },
                buttons: {
                    "Save": function () {
                        $.post(saveExpressionUrl, { expression: editor.getValue() }, function (d) {
                            $("#middle-center").html(d);
                            RuleBookLoaded();
                        });
                        $(this).dialog("close");
                    }
                }
            });
        editor.refresh();
    });
}

$(function () {
    initCodeEditor();
    // Initialize left tabs
    $('.nav-button').click(function () { $('.nav-selected').removeClass('nav-selected'); $(this).addClass('nav-selected'); });
});

var editor = null;

var currentRuleId = 0;
var currentRuleChangeId = 0;

function initCodeEditor() {
    //editor = CodeMirror.fromTextArea(document.getElementById("code"), { lineNumbers: true });
    CodeMirror.commands.autocomplete = function (cm) {
        CodeMirror.simpleHint(cm, CodeMirror.javascriptHint);
    };
    editor = CodeMirror.fromTextArea(document.getElementById("code"),
                      {
                          lineNumbers: true,
                          extraKeys: {
                              "Ctrl-Space": "autocomplete"
                          }
                      });
}