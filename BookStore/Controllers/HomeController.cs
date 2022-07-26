using BookStore.Models.Data;
using BookStore.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        // GET: Home/ProductCarouselPartial/name
        public ActionResult ProductCarouselPartial(string name)
        {
            // Объявляем список типа list
            List<ProductViewModel> productList;

            using (Db db = new Db())
            {
                // Получаем ID категории
                CategoryDTO categoryDTO = db.Categories.Where(x => x.Description == name).FirstOrDefault();

                int categoryId = categoryDTO.Id;

                // Инициализируем list данными
                productList = db.Products.ToArray().Where(x => x.CategoryId == categoryId).Select(x => new ProductViewModel(x)).Take(6).ToList();
            }

            // Возвращаем list в представление
            return PartialView("_ProductCarouselPartial", productList);
        }

        public ActionResult CategoryMenuPartial()
        {
            //Объявляем модель типа List<CategoryViewModel>
            List<CategoryViewModel> categoryVMList;

            using(Db db = new Db())
            {
                // Инициализируем модель данными
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryViewModel(x)).ToList();
            }

            // Возвращаем частичное представление с моделью
            return PartialView("_CategoryMenuPartial", categoryVMList);
        }

        // GET: Home/Category/name
        public ActionResult Category(string name)
        {
            // Объявляем список типа list
            List<ProductViewModel> productVMlist;

            using(Db db = new Db())
            {
                // Получаем ID категории
                CategoryDTO categoryDTO = db.Categories.Where(x => x.Description == name).FirstOrDefault();

                int categoryId = categoryDTO.Id;

                // Инициализируем список данными
                productVMlist = db.Products.ToArray().Where(x => x.CategoryId == categoryId).Select(x => new ProductViewModel(x)).ToList();

                // Получаем имя категории
                var productCategory = db.Products.Where(x => x.CategoryId == categoryId).FirstOrDefault();

                // Проверяем на null
                if(productCategory == null)
                {
                    var categoryName = db.Categories.Where(x => x.Description == name).Select(x => x.Name).FirstOrDefault();
                    ViewBag.CategoryName = categoryName;
                }
                else
                {
                    ViewBag.CategoryName = productCategory.CategoryName;
                }
            }

            // Возвращаем представление с моделью
            return View(productVMlist);
        }

        // GET: Home/product-details/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            // Объявляем модели DTO и ViewModel
            ProductDTO dto;
            ProductViewModel model;

            // Инициализируем id продукта
            int productId = 0;

            using(Db db = new Db())
            {
                // Проверяем доступен ли продукт
                if(!db.Products.Any(x => x.Slug.Equals(name)))
                {
                    return RedirectToAction("Index", "Home");
                }

                // Инициализируем модель DTO данными
                dto = db.Products.Where(x => x.Slug == name).FirstOrDefault();

                // Получаем id
                productId = dto.Id;

                // Инициализируем модель ViewModel
                model = new ProductViewModel(dto);

            }

            // Получаем изображение из галереи
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + productId + "/Gallery/Thumbs"))
                .Select(imageName => Path.GetFileName(imageName));

            // Возвращаем модель в представление
            return View("ProductDetails", model);
        }

        // POST: Home/Search/name
        [HttpPost]
        public ActionResult Search(string name)
        {
            // Объявляем list типа ProductDTO
            List<ProductDTO> listDTO;

            // Инициализируем list типа ProductViewModel
            List<ProductViewModel> listProduct = new List<ProductViewModel>();

            // Проводим поиск по названию продукта
            using(Db db = new Db())
            {
                listDTO = db.Products.ToArray().Where(x => x.Name.Contains(name) || name == null).ToList();
            }

            // Заполняем list моделей из контекста данных
            for (int i = 0; i < listDTO.Count; i++)
            {
                listProduct.Add(new ProductViewModel(listDTO[i]));
            }

            // Запоминаем введеный пользователем текст (для красоты)
            ViewBag.InputText = name;

            // Возвращаем list в представление
            return View(listProduct);
        }

        // POST: Home/SendMailToUser
        [HttpPost]
        public JsonResult SendMailToUser(string name)
        {
            // Копипаст из видоса индуса
            bool result = false;
            result = SendEmail(name, "test", "<p>Тестовая проверка рассылки.<br />Никаких новинок и распродаж нет.</p>");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public bool SendEmail(string toEmail, string subject, string emailBody)
        {
            try
            {
                string senderEmail = System.Configuration.ConfigurationManager.AppSettings["SenderEmail"].ToString();
                string senderPassword = System.Configuration.ConfigurationManager.AppSettings["SenderPassword"].ToString();

                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(senderEmail, senderPassword);

                MailMessage mailMessage = new MailMessage(senderEmail, toEmail, subject, emailBody);
                mailMessage.IsBodyHtml = true;
                mailMessage.BodyEncoding = UTF8Encoding.UTF8;
                client.Send(mailMessage);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

    }
}