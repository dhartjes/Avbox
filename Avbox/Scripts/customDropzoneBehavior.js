$(document).ready(function () {
    Dropzone.autoDiscover = false;

    //Simple Dropzonejs
    $("#dzUpload").dropzone({
        url: "SaveUploadedFile",
        addRemoveLinks: true,
        success: function (file, response) {
            var imgName = response;
            file.previewElement.classList.add("dz-success");
            console.log("Successfully uploaded: " + imgName);
        },
        error: function (file, response) {
            file.previewElement.classList.add("dz-error");
        }
    })
});

//File upload response from the server. Await click submit to upload files.
//Dropzone.options.dropzoneForm = {

//    autoProcessQueue: false,

//    init: function () {
//        var submitButton = $("#submit");
//        var myDropzone = this;

//        submitButton.on("click", function () {
//            myDropzone.processQueue();
//        })
//    }
//}

//Begins processing the file immediately. Upon completion, creates var res which does nothing?
//Dropzone.options.dropzoneForm = {

//    init: function () {
//        var submitButton = $("#submit");
//        var myDropzone = this;

//        this.on("complete", function (data) {
//            var res = JSON.parse(data.xhr.responseText);
//        });
//    }
//}