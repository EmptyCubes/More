function ExecuteMoreRules(url, effectiveDate, ruleBook, inputs, selectItems, handleResult) {
    $.support.cors = true;
    $.ajax(
            {
                type: "POST",
                url: url + "/api/values",
                data: //JSON.stringify(
                    {
                    effectiveDate: effectiveDate,
                    rulebook: ruleBook,
                    items: selectItems,
                    data: JSON.stringify(inputs)
                }
                //)
                ,
                success: function (d) { handleResult(jQuery.parseJSON(d)); },
                dataType: "json"
            }
    );
}