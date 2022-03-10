$(function () {
    $("#pdfKanJi").click(function () {
        console.log("ok");
        var a = document.createElement("a");
        document.body.appendChild(a);
        a.style = "display: none";
        $.ajax({
            
            url: "/KanJi/GenerateAndDownloadPDF",
            type: "post",
           // data: objectData,
            //contentType: "application/json; charset=utf-8",
            xhrFields: {
                responseType: 'blob'
            },
            success: function (data) {
                console.log(data);
                var file = new Blob([data], { type: 'application/pdf' });
                var fileURL = URL.createObjectURL(file);
                a.href = url;
                a.download = "TestKanJi";
                a.click();
                window.URL.revokeObjectURL(fileURL);
            }
        });
    })
})
