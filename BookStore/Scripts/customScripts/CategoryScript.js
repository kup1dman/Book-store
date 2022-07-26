$(function () {

    // Добавление новой категории(Код скопирован из файла ютуб урока)
    /* Объявляем и инициализируем нужные переменные */

    var newCatA = $("a#newcata"); /*Класс линка добавления*/
    var newCatTextInput = $("#new-cat-name"); /*Класс текстового поля ввода*/
    var ajaxText = $("span.ajax-text"); /*Класс картинки загрузки*/
    var table = $("table#pages tbody"); /*Класс таблицы вывода*/

    /* Пишем функцию на отлов нажатия Enter */
    newCatTextInput.keyup(function (e) {
        if (e.keyCode == 13) {
            newCatA.click();
            location.reload(true);
        }
    });

    /* Пишем функцию Click */
    newCatA.click(function (e) {
        e.preventDefault();

        var catName = newCatTextInput.val();

        if (catName.length < 3) {
            alert("Имя категории должно быть как минимум из 3 букв");
            return false;
        }

        ajaxText.show();

        var url = "/Admin/Shop/AddNewCategory";

        $.post(url, { catName: catName }, function (data) {
            var response = data.trim();

            if (response == "titletaken") {
                ajaxText.html("<span class='alert alert-danger'>Такой заголовок уже имеется!</span>");
                setTimeout(function () {
                    ajaxText.fadeOut("fast", function () {
                        ajaxText.html("<img src='/Content/img/ajax-loader.gif' height='80' />");
                    });
                }, 2000);
                return false;
            }
            else {
                if (!$("table#pages").length) {
                    location.reload();
                }
                else {
                    ajaxText.html("<span class='alert alert-success'>Категория добавлена</span>");
                    setTimeout(function () {
                        ajaxText.fadeOut("fast", function () {
                            ajaxText.html("<img src='/Source files/loading.gif' height='80' />");
                        });
                    }, 2000);

                    newCatTextInput.val("");

                    var toAppend = $("table#pages tbody tr:last").clone();
                    toAppend.attr("id", "id_" + data);
                    toAppend.find("#item_Name").val(catName);
                    toAppend.find("a.delete").attr("href", "/Admin/Shop/DeleteCategory/" + data);
                    table.append(toAppend);
                    table.sortable("refresh");
                }
            }
        });
    });

    // Подтверждение удаление категории
    $("body").on("click", "a.delete", function () {
        if (!confirm("Подтвердить удаление категории")) {
            return false;
        }
    });

    // Изменение названия категории
    var originalTextBoxValue;

    $("table#pages input.text-box").dblclick(function () {
        originalTextBoxValue = $(this).val();
        $(this).attr("readonly", false);
    });

    $("table#pages input.text-box").keyup(function (e) {
        if (e.keyCode == 13) {
            $(this).blur();
        }
    });

    $("table#pages input.text-box").blur(function () {
        var $this = $(this);
        var ajaxdiv = $this.parent().parent().parent().find(".ajaxdivtd");
        var newCatName = $this.val();
        var id = $this.parent().parent().parent().parent().parent().attr("id").substring(3);
        var url = "/admin/shop/RenameCategory";

        if (newCatName.length < 3) {
            alert("Имя категории должно быть длинее 3 букв.");
            $this.attr("readonly", true);
            return false;
        }

        $.post(url, { newCatName: newCatName, id: id }, function (data) {
            var response = data.trim();

            if (response == "titletaken") {
                $this.val(originalTextBoxValue);
                ajaxdiv.html("<div class='alert alert-danger'>Такой заголовок уже есть!</div>").show();
            }
            else {
                ajaxdiv.html("<div class='alert alert-success'>Имя категории изменено!</div>").show();
            }

            setTimeout(function () {
                ajaxdiv.fadeOut("fast", function () {
                    ajaxdiv.html("");
                });
            }, 3000);
        }).done(function () {
            $this.attr("readonly", true);

        });
    });


    // Скрипт сортировки страниц
    $("table#pages tbody").sortable({
        items: "tr:not(.home)",
        placeholder: "ui-state-highlight",
        update: function () {
            var ids = $("table#pages tbody").sortable("serialize");
            var url = "/Admin/Shop/ReorderCategories";

            $.post(url, ids, function (data) {

            });
        }
    });

});