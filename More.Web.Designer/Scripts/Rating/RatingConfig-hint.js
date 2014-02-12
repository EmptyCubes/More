var variables = null;
var methods = null;
var updateVariables = function (id) {
    $.getJSON('./Intellisense/Variables', { id: id }, function (data) {
        variables = data;
    });
};
    
(function () {
    updateVariables(null);
    
    $.getJSON('./Intellisense/Methods', null, function (data) {
        methods = data;
    });

    function forEach(arr, f) {
        for (var i = 0, e = arr.length; i < e; ++i) f(arr[i]);
    }

    function arrayContains(arr, item) {
        if (!Array.prototype.indexOf) {
            var i = arr.length;
            while (i--) {
                if (arr[i] === item) {
                    return true;
                }
            }
            return false;
        }
        return arr.indexOf(item) != -1;
    }

    function scriptHint(editor, keywords, getToken) {
        // Find the token at the cursor
        var cur = editor.getCursor(), token = getToken(editor, cur), tprop = token;
        // If it's not a 'word-style' token, ignore the token.
        //        if (!/^[\w$_]*$/.test(token.string)) {
        //            token = tprop = { start: cur.ch, end: cur.ch, string: "", state: token.state,
        //                className: token.string == "." ? "property" : null
        //            };
        //        }


     
        return {
            list: getCompletions(token, keywords),
            from: { line: cur.line, ch: token.start },
            to: { line: cur.line, ch: token.end }
        };
    }

    CodeMirror.javascriptHint = function (editor) {
        return scriptHint(editor, null,
            function (e, cur) { return e.getTokenAt(cur); });
    };
    var startsWith = function (strA,strB){
        if (strA == null || strB == null) return false;
        if (strB.length > strA.length) return false;
            return strA.slice(0, strB.length) == strB;
  };
    function getNames(arr,filterBy) {
        var result = [];
        var filter = $.trim(filterBy);
        for(x in arr) {
            var val = arr[x].description;
            if ((filter != null && filter != '') && !startsWith(val, filter)) {
                continue;
            }
            result[result.length] = arr[x];
        }
        return result;
    }
    window.getNames = getNames;
    function getCompletions(token, keywords) {
        return getNames(variables.concat(methods),token.string);
    }
})();
