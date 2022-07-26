using BookStore.Models.Data;
using BookStore.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages/PagesList
        public ActionResult PagesList()
        {
            // Объявляем список для представления (PageViewModel)
            List<PageViewModel> pageList;

            // Инициализируем список (Db)
            using (Db db = new Db())
            {
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageViewModel(x)).ToList();
            }

            // Возвращаем список в представление
            return View(pageList);
        }

        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageViewModel model)
        {
            // Проверка модели на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                // Объявляем переменную для краткого описания (description)
                string description;

                // Инициализируем класс PagesDTO
                PagesDTO dto = new PagesDTO();

                // Присваиваем заголовок модели
                dto.Title = model.Title.ToUpper();

                // Проверяем есть ли описание, если нет - присваиваем
                if (string.IsNullOrWhiteSpace(model.Description))
                {
                    description = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    description = model.Description.Replace(" ", "-").ToLower();
                }

                // Убеждаемся, что заголовок и описание - уникальны
                if (db.Pages.Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "Такой заголовок уже имеется.");
                    return View(model);
                }
                else if (db.Pages.Any(x => x.Description == model.Description))
                {
                    ModelState.AddModelError("", "Такое описание уже имеется.");
                    return View(model);
                }

                // Присваиваем оставшиеся значения модели
                dto.Description = description;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                // Сохраняем модель в базу данных
                db.Pages.Add(dto);
                db.SaveChanges();
            }

            // Передаем сообщение через TempData
            TempData["SuccessfulMessage"] = "Новая страница добавлена.";

            // Переадресовываем пользователя на метод Index
            return RedirectToAction("Index");
        }

        // GET: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            // Объявляем модель PageViewModel
            PageViewModel model;

            using (Db db = new Db())
            {
                // Получаем страницу
                PagesDTO dto = db.Pages.Find(id);

                // Проверяем, доступна ли страница
                if (dto == null)
                {
                    return Content("Эта страница не доступна.");
                }

                // Инициализируем модель данными
                model = new PageViewModel(dto);
            }

            // Возвращаем представление с моделью
            return View(model);
        }

        // POST: Admin/Pages/EditPage
        [HttpPost]
        public ActionResult EditPage(PageViewModel model)
        {
            // Проверить модель на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                // Получаем ID страницы
                int id = model.Id;

                // Объявляем переменную для Description
                string description = "home";

                // Получаем страницу (по ID)
                PagesDTO dto = db.Pages.Find(id);

                // Присваиваем название из полученной модели в DTO
                dto.Title = model.Title;

                // Проверяем Description и присваиваем его, если это необходимо
                if (model.Description != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Description))
                    {
                        description = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        description = model.Description.Replace(" ", "-").ToLower();
                    }
                }

                // Проверяем Description и Title на уникальность
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "Такой заголовок уже существует");
                    return View(model);
                }
                else if (db.Pages.Where(x => x.Id != id).Any(x => x.Description == model.Description))
                {
                    ModelState.AddModelError("", "Такое описание уже существует");
                    return View(model);
                }

                // Записываем остальные значения в класс DTO
                dto.Description = description;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                // Сохраняем изменения в базу
                db.SaveChanges();
            }

            // Оповещаем пользователя
            TempData["SuccessfulMessage"] = "Страница изменена";

            // Переадресовываем пользователя
            return RedirectToAction("EditPage");
        }

        // GET: Admin/Pages/PageDetails/id
        [HttpGet]
        public ActionResult PageDetails(int id)
        {
            // Объявляем модель PageViewModel
            PageViewModel model;

            using (Db db = new Db())
            {
                // Получаем страницу
                PagesDTO dto = db.Pages.Find(id);

                // Убеждаемся что страница доступна
                if (dto == null)
                {
                    return Content("Такой страницы не существует.");
                }

                // Присваиваем модели информацию из БД
                model = new PageViewModel(dto);
            }

            // вернуть модель в представление
            return View(model);
        }

        // GET: Admin/Pages/DeletePage/id
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                // Получаем страницу
                PagesDTO dto = db.Pages.Find(id);

                // Удаляем страницу
                db.Pages.Remove(dto);

                // Сохранение изменение в БД
                db.SaveChanges();
            }

            //Оповещаем пользователя
            TempData["SuccessfulMessage"] = "Страница удалена.";

            // Переадресация пользователя 
            return RedirectToAction("PagesList");
        }

        // POST: Admin/Pages/ReorderPages
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using (Db db = new Db())
            {
                // Реализуем начальный счетчик
                int counter = 1;

                // Инициализируем модель данных
                PagesDTO dto;

                // Устанавливаем сортировку для каждой страницы
                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = counter;

                    db.SaveChanges();

                    counter++;
                }
            }
        }

        // GET: Admin/Pages/EditSideBar
        [HttpGet]
        public ActionResult EditSideBar()
        {
            // Объявляем модель
            SidebarViewModel model;

            using (Db db = new Db())
            {
                // Получаем данные из DTO
                SidebarsDTO dto = db.SideBars.Find(1); // говнокод. Жесткие значения

                // Заполняем модель
                model = new SidebarViewModel(dto);
            }

            // Возвращаем представление с модели
            return View(model);
        }

        // POST: Admin/Pages/EditSideBar
        [HttpPost]
        public ActionResult EditSideBar(SidebarViewModel model)
        {
            
            using(Db db = new Db())
            {
                // Получаем данные из DTO
                SidebarsDTO dto = db.SideBars.Find(1); // говнкод. Жесткие значения

                // Присваиваем данные в свойство body
                dto.Body = model.Body;

                // Сохранить в БД
                db.SaveChanges();
            }

            // Присваиваем сообщение в TempData
            TempData["SuccessfulMessage"] = "Sidebar изменен";

            // Передресация
            return RedirectToAction("EditSideBar");
        }
    }
}