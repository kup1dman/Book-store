using BookStore.Models.Data;
using BookStore.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebGrease;
using log4net;
using Stripe;
using Stripe.Checkout;

namespace BookStore.Controllers
{
    public class CartController : Controller
    {
        private ILog log = log4net.LogManager.GetLogger("CartController");
        // GET: Cart
        public ActionResult Index()
        {
            // Объявляем list типа CartViewModel
            var cartList = Session["cart"] as List<CartViewModel> ?? new List<CartViewModel>();

            // Проверяем пуста ли корзина
            if (cartList.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Ваша корзина пуста!";
                return View();
            }

            // Складываем сумму и записываем во ViewBag
            decimal total = 0m;
            foreach (var item in cartList)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            // Возвращаем list в представление
            return View(cartList);
        }

        public ActionResult CartPartial()
        {
            // Объявляем модель CartViewModel
            CartViewModel model = new CartViewModel();

            // Объявляем переменную количества
            int quantity = 0;

            // Объявляем переменную цены
            decimal price = 0m;

            // Проверяем сессию корзины
            if (Session["cart"] != null)
            {
                // Получаем общее количество товаров и цену
                var list = (List<CartViewModel>)Session["cart"];

                foreach (var item in list)
                {
                    quantity += item.Quantity;
                    price += item.Quantity * item.Price;
                }

                model.Quantity = quantity;
                model.Price = price;
            }
            else
            {
                // Или устанавливаем количество и цену 0
                model.Quantity = 0;
                model.Price = 0m;
            }

            // Возвращаем частичное представление
            return PartialView("_CartPartial", model);
        }

        public void AddToCartPartial(int id)
        {
            // Объявляем list типа CartViewModel
            List<CartViewModel> cartList = Session["cart"] as List<CartViewModel> ?? new List<CartViewModel>();

            // Объявляем модель CartViewModel
            CartViewModel model = new CartViewModel();

            // Объявляем переменную для категории присланного товара
            string categoryName = null;

            using (Db db = new Db())
            {
                // Получаем продукт по id
                ProductDTO product = db.Products.Find(id);

                // Получаем категорию продукта
                categoryName = product.CategoryName.Trim().ToLower().Replace(" ", "-");

                // Проверяем есть ли такой уже продукт
                var productInCart = cartList.FirstOrDefault(x => x.ProductId == id);

                // Если нет, то добавляем новый товар
                if (productInCart == null)
                {
                    cartList.Add(new CartViewModel()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }
                else
                {
                    // Если да, то добавляем +1 товара
                    productInCart.Quantity++;
                }

            }

            // Получаем общее количество и цену
            int quantity = 0;
            decimal price = 0m;

            foreach (var item in cartList)
            {
                quantity += item.Quantity;
                price += item.Quantity * item.Price;
            }

            // Добавляем общее количество и цену в модель
            model.Quantity = quantity;
            model.Price = price;

            // Сохраняем состояние корзины в сессию
            Session["cart"] = cartList;

            //return Redirect("/Home/Category/" + categoryName);
        }

        // GET: /Cart/IncrementProduct
        public JsonResult IncrementProduct(int productId)
        {
            // Объявляем list типа CartViewModel
            List<CartViewModel> cartList = Session["cart"] as List<CartViewModel>;

            using (Db db = new Db())
            {
                // Получаем модель CartViewModel из листа
                CartViewModel model = cartList.FirstOrDefault(x => x.ProductId == productId);

                // Добавляем количество
                model.Quantity++;

                // Сохраняем необходимые данные
                var result = new { quantity = model.Quantity, price = model.Price };

                // Возвращаем JSON ответ с данными
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        // GET: /Cart/DecrementProduct
        public JsonResult DecrementProduct(int productId)
        {
            // Объявляем list типа CartViewModel
            List<CartViewModel> cartList = Session["cart"] as List<CartViewModel>;

            using (Db db = new Db())
            {
                // Получаем модель CartViewModel из листа
                CartViewModel model = cartList.FirstOrDefault(x => x.ProductId == productId);

                // Отнимаем количество
                if (model.Quantity > 1)
                {
                    model.Quantity--;
                }
                else
                {
                    model.Quantity = 0;
                    cartList.Remove(model);
                }

                // Сохраняем необходимые данные
                var result = new { quantity = model.Quantity, price = model.Price };

                // Возвращаем JSON ответ с данными
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: /Cart/RemoveProduct
        public void RemoveProduct(int productId)
        {
            // Объявляем list типа CartViewModel
            List<CartViewModel> cartList = Session["cart"] as List<CartViewModel>;

            using (Db db = new Db())
            {
                // Получаем модель CartViewModel из листа
                CartViewModel model = cartList.FirstOrDefault(x => x.ProductId == productId);

                cartList.Remove(model);
            }
        }

        // GET: /Cart/Checkout
        [HttpGet]
        public ActionResult Checkout()
        {
            // Получаем общее количество товаров и цену
            var list = (List<CartViewModel>)Session["cart"];
            decimal total = 0m;
            foreach (var item in list)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            return View(list);

        }
    }
}