    /* Increment product */
$(function () {

    $("a.incproduct").click(function (e) {
        e.preventDefault();

        var productId = $(this).data("id");
        var url = "/cart/IncrementProduct";

        $.getJSON(url, { productId: productId }, function (data) {
            $("td.quantity" + productId).html(data.quantity);

            var price = data.quantity * data.price;
            var priceHtml = (price.toFixed(2) + " руб.").toString();

            $("td.total" + productId).html(priceHtml);

            var gt = parseFloat($("td.grandtotal span").text())
            var grandtotal = (gt + data.price).toFixed(2);

            $("td.grandtotal span").text(grandtotal);
        });
    });
});
    /*-----------------------------------------------------------*/


    /* Decrement product */
$(function () {

    $("a.decproduct").click(function (e) {
        e.preventDefault();

        var $this = $(this);
        var productId = $(this).data("id");
        var url = "/cart/DecrementProduct";

        $.getJSON(url, { productId: productId }, function (data) {

            if (data.quantity == 0) {
                $this.parent().fadeOut("fast", function () {
                    location.reload();
                });
            }
            else {
                $("td.quantity" + productId).html(data.quantity);

                var price = data.quantity * data.price;
                var priceHtml = (price.toFixed(2) + " руб.").toString();

                $("td.total" + productId).html(priceHtml);

                var gt = parseFloat($("td.grandtotal span").text());
                var grandtotal = (gt - data.price).toFixed(2);

                $("td.grandtotal span").text(grandtotal);
            }
        });
    });
});
    /*-----------------------------------------------------------*/


    /* Remove product */
$(function () {

    $("a.removeproduct").click(function (e) {
        e.preventDefault();

        var $this = $(this);
        var productId = $(this).data("id");
        var url = "/cart/RemoveProduct";

        $.get(url, { productId: productId }, function (data) {
            location.reload();
        });
    });
});
    /*-----------------------------------------------------------*/