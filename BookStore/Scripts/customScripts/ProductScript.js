$(function () {

    /* Скрипт скачан с ютуб-ролика (урока 13) */
    /* Выбор товаров по определенной категории*/
    /* Скрипт для Products.cshtml*/

    $("#SelectCategory").on("change", function () {
        var url = $(this).val();

        if (url) {
            window.location = "/Admin/Shop/Products?categoryId=" + url;
        }
        return false;
    });

    /*-----------------------------------------------------------*/

    /* Подтверждение удаление продукта */

    $("a.delete").click(function () {
        if (!confirm("Подтверждение удаление продукта"))
        {
            return false;
        } 
    });

    /*-----------------------------------------------------------*/
});