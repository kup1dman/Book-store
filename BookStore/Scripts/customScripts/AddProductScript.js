$(function () {

    /*Скрипт для AddProduct.cshtml*/
    /*Предпросмотр загруженной картинки*/

    function readURL(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $("img#imgPreview")
                    .attr("src", e.target.result)
                    .width(200)
                    .height(200);
            }

            reader.readAsDataURL(input.files[0]);
        }
    }

    $("#imageUpload").change(function () {
        readURL(this);
    });

    /*-----------------------------------------------------------*/

});