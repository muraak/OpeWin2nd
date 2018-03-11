
// Copyright (c) 2018 muraak
// 
// This software is released under the MIT License.
// https://opensource.org/licenses/MIT


$(function()
{
    var md = readFileAsText("./md/readme.md");
    $("#markdown").html(marked(md));
});

function readFileAsText(filePath)
{
    $.ajaxSetup({
        xhr: function(){
                if("ActiveXObject" in window) 
                    return new ActiveXObject("Microsoft.XMLHTTP");
                else 
                    return new XMLHttpRequest();
            },
        async: false,
    });

    var txt;
    $.ajax({
        type: 'get',
        url: filePath,
    }).done(function(data){
        txt = data
    }).error(function(data){
        // for Firefox
        if(data.readyState == 4 & data.status == 200)
            txt = data.responseText;
        else
            txt = "This content failed to load.";
    });

    return txt;
}