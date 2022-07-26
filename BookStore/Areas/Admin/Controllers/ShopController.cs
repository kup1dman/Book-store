using BookStore.Models.Data;
using BookStore.Models.ViewModels.Shop;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace BookStore.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop/Categories
        public ActionResult Categories()
        {
            // Объвляем модель типа List
            List<CategoryViewModel> categoryVMList = new List<CategoryViewModel>();

            using (Db db = new Db())
            {
                // Инициализируем модель данными
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryViewModel(x)).ToList();
            }

            // Возвращаем List в  представлением
            return View(categoryVMList);
        }

        // POST: Admin/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            // Объявляем string переменную Id
            string id;

            using (Db db = new Db())
            {
                // Проверяем имя категории на уникальность
                if (db.Categories.Any(x => x.Name == catName))
                    return "titletaken";

                // Инициализируем модель DTO
                CategoryDTO dto = new CategoryDTO();

                // Добавляем данные в модель
                dto.Name = catName;
                dto.Description = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;

                // Сохраняем в БД
                db.Categories.Add(dto);
                db.SaveChanges();

                // Получаем Id для возврата в представление
                id = dto.Id.ToString();
            }

            // Возвращаем Id в представление
            return id;
        }

        // POST: Admin/Shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (Db db = new Db())
            {
                // Реализуем начальный счетчик
                int counter = 1;

                // Инициализируем модель данных
                CategoryDTO dto;

                // Устанавливаем сортировку для каждой страницы
                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = counter;

                    db.SaveChanges();

                    counter++;
                }
            }
        }

        // GET: Admin/Shop/DeleteCategory/id
        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                // Получаем категорию
                CategoryDTO dto = db.Categories.Find(id);

                // Удаляем категорию
                db.Categories.Remove(dto);

                // Сохранение изменение в БД
                db.SaveChanges();
            }

            //Оповещаем пользователя
            TempData["SuccessfulMessage"] = "Категория удалена.";

            // Переадресация пользователя 
            return RedirectToAction("Categories");
        }

        // POST: Admin/Shop/RenameCategory/id
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (Db db = new Db())
            {
                // Проверить имя на уникальность
                if (db.Categories.Any(x => x.Name == newCatName))
                {
                    return "titletaken";
                }

                // Получаем модель DTO
                CategoryDTO dto = db.Categories.Find(id);

                // Редактируем модель DTO
                dto.Name = newCatName;
                dto.Description = newCatName.Replace(" ", "-").ToLower();

                // Сохраняем изменения
                db.SaveChanges();
            }

            // Возвращаем слово
            return "ok";
        }

        //GET: Admin/Shop/AddProduct
        [HttpGet]
        public ActionResult AddProduct()
        {
            // Объявляем модель данных
            ProductViewModel model = new ProductViewModel();

            // Добавляем список категорий из базы в модель
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), dataValueField: "id", dataTextField: "Name");
            }

            // Вернуть модель в представление
            return View(model);

        }

        //POST: Admin/Shop/AddProduct
        // Очень сложный метод
        [HttpPost]
        public ActionResult AddProduct(ProductViewModel model, HttpPostedFileBase file)
        {
            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }
            }

            // Проверяем имя продукта на уникальность
            using (Db db = new Db())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "Такое имя уже существует!");
                    return View(model);
                }
            }

            // Инициализируем productId
            int productId;

            // Инициализируем и сохраняем модель на основе ProductDTO
            using (Db db = new Db())
            {
                ProductDTO productDTO = new ProductDTO();
                productDTO.Name = model.Name;
                productDTO.Slug = model.Name.Replace(" ", "-").ToLower();
                productDTO.Description = model.Description;
                productDTO.Price = model.Price;
                productDTO.CategoryId = model.CategoryId;

                CategoryDTO categoryDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                productDTO.CategoryName = categoryDTO.Name;

                db.Products.Add(productDTO);
                db.SaveChanges();

                productId = productDTO.Id;
            }

            // Добавляем сообщение для пользователя
            TempData["SuccessfulMessage"] = "Продукт успешно добавлен";

            // Штука полезная, но опасная, использовал только в учебных целях

            #region Upload Image

            // Создаем необходимые ссылки на директории
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");

            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + productId.ToString());

            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + productId.ToString() + "\\Thumbs");

            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + productId.ToString() + "\\Gallery");

            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + productId.ToString() + "\\Gallery\\Thumbs");

            // Проверяем наличие директорий (если нет, создаем)
            IsDirectoryExist(pathString1);
            IsDirectoryExist(pathString2);
            IsDirectoryExist(pathString3);
            IsDirectoryExist(pathString4);
            IsDirectoryExist(pathString5);

            // Проверяем, был ли файл загружен
            if (file != null && file.ContentLength > 0)
            {
                // Получаем расширение файла
                string extenstion = file.ContentType.ToLower();

                // Проверяем расширение файла
                if (extenstion != "image/jpq" &&
                    extenstion != "image/jpeg" &&
                    extenstion != "image/pjpeg" &&
                    extenstion != "image/gif" &&
                    extenstion != "image/png" &&
                    extenstion != "image/x-png"
                    )
                {
                    using (Db db = new Db())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "Такое расширение не поддерживается!");
                        return View(model);
                    }
                }

                // Объявляем переменную с именем изображения
                string imageName = file.FileName;

                // Сохраняем имя изображение в модель DTO
                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(productId);
                    dto.ImageName = imageName;
                    db.SaveChanges();
                }

                // Назначаем пути к оригинальному и уменьшенному изображению
                var path = string.Format($"{pathString2}\\{imageName}");
                var path2 = string.Format($"{pathString3}\\{imageName}");

                // Сохраняем ориг изображение
                file.SaveAs(path);

                // Создаем и сохраняем уменьшенную копию
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200).Crop(1,1);
                img.Save(path2);
            }
            #endregion


            // Переадресоваем пользователя
            return RedirectToAction("AddProduct");
        }

        private void IsDirectoryExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        //GET: Admin/Shop/Products
        [HttpGet]
        public ActionResult Products(int? page, int? categoryId)
        {
            // Объявляем ProductViewModel типа List
            List<ProductViewModel> productList;

            // Устанавливаем номер страницы
            var pageNumber = page ?? 1;

            using(Db db = new Db())
            {
                // Инициализируем list и заполняем данными
                productList = db.Products.ToArray()
                    .Where(x => categoryId == null || categoryId == 0 || x.CategoryId == categoryId)
                    .Select(x => new ProductViewModel(x))
                    .ToList();

                // Заполняем категории данными
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                // Устанавливаем выбранную категорию
                ViewBag.SelectedCategory = categoryId.ToString();
            }

            // Устанавливаем постраничную навигацию
            var onePageOfProducts = productList.ToPagedList(pageNumber, 3);
            ViewBag.onePageOfProducts = onePageOfProducts;

            // Возвращаем представление с данными
            return View(productList);
        }

        //GET: Admin/Shop/EditProduct
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            // Объявляем модель ProductViewModel
            ProductViewModel model;

            using(Db db = new Db())
            {
                // Получаем продукт
                ProductDTO dto = db.Products.Find(id);

                // Проверяем существует ли продукт
                if (dto == null)
                {
                    return Content("Такого товара не существует!");
                }

                // Инициализируем модель данными
                model = new ProductViewModel(dto);

                // Создаем спиок категорий 
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                // Получаем все изображения из галереи
                try
                {
                    model.GalleryImages = Directory
                    .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                    .Select(x => Path.GetFileName(x));
                }
                catch
                {
                    //model.GalleryImages = null;
                }
                    
            }

            // Возвращаем модель в представление 
            return View(model);
        }

        //POST: Admin/Shop/EditProduct
        [HttpPost]
        public ActionResult EditProduct(ProductViewModel model, HttpPostedFileBase file)
        {
            // Получаем Id продукта
            int productId = model.Id;

            
            using(Db db = new Db())
            {
                // Заполняем список категориями
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            // Заполняем список изображениями
            model.GalleryImages = Directory
                    .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + productId + "/Gallery/Thumbs"))
                    .Select(x => Path.GetFileName(x));

            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Проверяем имя продукта на уникальность
            using(Db db = new Db())
            {
                if(db.Products.Where(x => x.Id != productId).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "Такое имя уже существует!");
                    return View(model);
                }
            }

            // Обновляем продукт в БД
            using(Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(productId);
                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-").ToLower();
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.CategoryId = model.CategoryId;
                dto.ImageName = model.ImageName;

                CategoryDTO categoryDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = categoryDTO.Name;

                db.SaveChanges();
            }

            // Устанавливаем сообщение в TempData
            TempData["SuccessfulMessage"] = "Продукт обновлен";

            // Логика обработки изображений
            #region Image Upload

            //Проверяем загрузку файла
            if (file != null && file.ContentLength > 0)
            {
                // Получаем расширение файла
                string extenstion = file.ContentType.ToLower();

                // Проверяем расширение
                if (extenstion != "image/jpq" &&
                    extenstion != "image/jpeg" &&
                    extenstion != "image/pjpeg" &&
                    extenstion != "image/gif" &&
                    extenstion != "image/png" &&
                    extenstion != "image/x-png"
                    )
                {
                    using (Db db = new Db())
                    {
                        ModelState.AddModelError("", "Такое расширение не поддерживается!");
                        return View(model);
                    }
                }

                // Устанавливаем пути загрузки
                var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                var pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + productId.ToString());

                var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + productId.ToString() + "\\Thumbs");

                // Удаляем существующие файлы и директории
                DirectoryInfo directory1 = new DirectoryInfo(pathString1);
                DirectoryInfo directory2 = new DirectoryInfo(pathString2);

                if (directory1 != null)
                {
                    foreach (var fileItem in directory1.GetFiles())
                    {
                        fileItem.Delete();
                    }
                }

                if (directory2 != null)
                {
                    foreach (var fileThumbItem in directory2.GetFiles())
                    {
                        fileThumbItem.Delete();
                    }
                }

                // Сохраняем изображение
                string imageName = file.FileName;
                using(Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(productId);
                    dto.ImageName = imageName;
                    db.SaveChanges();
                }

                // Сохраняем оригинал и превью версии
                var path1 = string.Format($"{pathString1}\\{imageName}");
                var path2 = string.Format($"{pathString2}\\{imageName}");

                // Сохраняем ориг изображение
                file.SaveAs(path1);

                // Создаем и сохраняем уменьшенную копию
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200).Crop(1, 1);
                img.Save(path2);
            }

            #endregion

            // Переадресовывем пользователя
            return RedirectToAction("EditProduct");
        }

        //GET: Admin/Shop/DeleteProduct
        public ActionResult DeleteProduct(int id)
        {
            using (Db db = new Db())
            {
                // Получаем продукт
                ProductDTO dto = db.Products.Find(id);

                // Удаляем продукт
                db.Products.Remove(dto);

                // Сохранение изменение в БД
                db.SaveChanges();
            }

            // Удаляем директории (изображения)
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")} Images\\Uploads"));

            var pathString = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());

            if (Directory.Exists(pathString)) 
            {
                Directory.Delete(pathString, true);
            }

            // Переадресация пользователя 
            return RedirectToAction("Products");
        }

        //POST: Admin/Shop/SaveGalleryImages
        [HttpPost]
        public void SaveGalleryImages(int id)
        {
            // Перебираем все полученные файлы
            foreach (string fileName in Request.Files)
            {

                // Инициализируем файлы
                HttpPostedFileBase file = Request.Files[fileName];

                // Проверяем на null
                if (file != null && file.ContentLength > 0)
                {

                    // Назначаем пути к дерикториям
                    var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                    string pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
                    string pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");


                    // Назначем пути изображений
                    var path1 = string.Format($"{pathString1}\\{file.FileName}");
                    var path2 = string.Format($"{pathString2}\\{file.FileName}");

                    // Сохраняем ориг изображения и их уменьш. копии
                    file.SaveAs(path1);

                    WebImage image = new WebImage(file.InputStream);
                    image.Resize(200, 200).Crop(1, 1);
                    image.Save(path2);
                }
            }
        }

        //POST: Admin/Shop/DeleteImage/id/imageName
        [HttpPost]
        public void DeleteImage(int id, string imageName)
        {
            // Задаем полный путь к ориг изображениям и уменьш. изображениям
            string fullPath1 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/" + imageName);
            string fullPath2 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/Thumbs/" + imageName);

            // Удаляем пути
            if (System.IO.File.Exists(fullPath1))
            {
                System.IO.File.Delete(fullPath1);
            }

            if (System.IO.File.Exists(fullPath2))
            {
                System.IO.File.Delete(fullPath2);
            }
        }
    }
}