$(function () {

    // Подтверждение удаление страницы
    $("a.delete").click(function () {
        if (!confirm("Подтвердить удаление страницы")) {
            return false;
        }
    });

    // Скрипт сортировки страниц
    $("table#pages tbody").sortable({
        items: "tr:not(.home)",
        placeholder: "ui-state-highlight",
        update: function () {
            var ids = $("table#pages tbody").sortable("serialize");
            var url = "/Admin/Pages/ReorderPages";

            $.post(url, ids, function (data) {

            });
        }
    });

});