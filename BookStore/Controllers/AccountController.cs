using BookStore.Models.Data;
using BookStore.Models.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BookStore.Controllers
{
    public class AccountController : Controller
    {
        public object FormAuthentication { get; private set; }

        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        // GET: Account/create-account
        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        // POST: Account/create-account
        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserViewModel model)
        {
            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }

            // Проверить соответствие пароля
            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Пароли не совпадают");
                return View("CreateAccount", model);
            }

            using(Db db = new Db())
            {
                // Проверяем имя на уникальность
                if(db.Users.Any(x => x.Username.Equals(model.Username)))
                {
                    ModelState.AddModelError("", $"{model.Username} уже занято");
                    model.Username = "";
                    return View("CreateAccount", model);
                }

                // Создаем экземпляр класса UserDTO
                UserDTO userDTO = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAddress = model.EmailAddress,
                    Username = model.Username,
                    Password = model.Password
                };

                // Добавляем данные в модель
                db.Users.Add(userDTO);

                // Сохраняем данные
                db.SaveChanges();

                // Добавляем роль пользователю
                int id = userDTO.Id;
                UserRoleDTO userRoleDTO = new UserRoleDTO()
                {
                    UserId = id,
                    RoleId = 2
                };
                db.UserRoles.Add(userRoleDTO);
                db.SaveChanges();
            }

            // Записываем сообщение в TempData
            TempData["SuccesfulMessage"] = "Регистрация прошла успешно";

            // Переадресовываем пользователя
            return Redirect("/Home/Index");
        }

        // GET: Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            // Подтверждаем что пользователь не авторизован
            string userName = User.Identity.Name;

            if (!string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("user-profile");
            }

            // Возвращаем представление
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(LoginUserViewModel model)
        {
            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Проверяем пользователя на валидность
            bool isValid = false;

            using(Db db = new Db())
            {
                if(db.Users.Any(x => x.Username.Equals(model.Username) && x.Password.Equals(model.Password)))
                {
                    isValid = true;
                }

                if (!isValid)
                {
                    ModelState.AddModelError("", "Неправильное имя пользователя или пароль!");
                    return View(model);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                    return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, model.RememberMe));
                }
            }
        }


        // GET: Account/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("/Home/Index");
        }


        public ActionResult UserNavPartial()
        {
            // Получаем имя пользователя
            string userName = User.Identity.Name;

            // Объявляем модель UserNavPartialVM
            UserNavPartialViewModel model;

            using(Db db = new Db())
            {
                // Получаем пользователя
                UserDTO userDTO = db.Users.FirstOrDefault(x => x.Username == userName);

                // Заполняем модель данными из контекста (DTO)
                model = new UserNavPartialViewModel()
                {
                    FirstName = userDTO.FirstName,
                    LastName = userDTO.LastName
                };
            }

            // Возвращаем частичное представление из модели
            return PartialView(model);
        }

        // GET: Account/UserProfile
        [HttpGet]
        [ActionName("user-profile")]
        public ActionResult UserProfile()
        {
            // Получаем имя пользователя
            string userName = User.Identity.Name;

            // Объявляем модель UserProfileViewModel
            UserProfileViewModel model;

            using(Db db = new Db())
            {
                // Получаем пользователя
                UserDTO userDTO = db.Users.FirstOrDefault(x => x.Username == userName);

                // Инициализируем модель данными
                model = new UserProfileViewModel(userDTO);
            }

            // Возвращаем модель в представление
            return View("UserProfile", model);
        }

        // POST: Account/UserProfile
        [HttpPost]
        [ActionName("user-profile")]
        public ActionResult UserProfile(UserProfileViewModel model)
        {
            bool IsUserNameChanged = false;

            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View("UserProfile", model);
            }

            // Проверяем пароль (если пользователь его меняет)
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Пароли не совпадают");
                    return View("UserProfile", model);
                }
            }

            // Получаем имя пользователя
            string userName = User.Identity.Name;

            using (Db db = new Db())
            {
                // Проверяем, сменился ли username
                if(userName != model.Username)
                {
                    userName = model.Username;
                    IsUserNameChanged = true;
                }
                
                // Проверяем имя на уникальность
                if (db.Users.Where(x => x.Id != model.Id).Any(x => x.Username == userName))
                {
                    ModelState.AddModelError("", $"Имя {model.Username} уже занято!");
                    model.Username = "";
                    return View("UserProfile", model);
                }

                // Изменяем модель контекста данных (DTO)
                UserDTO userDTO = db.Users.Find(model.Id);

                userDTO.FirstName = model.FirstName;
                userDTO.LastName = model.LastName;
                userDTO.EmailAddress = model.EmailAddress;
                userDTO.Username = model.Username;

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    userDTO.Password = model.Password;
                }

                // Сохраняем изменения
                db.SaveChanges();
            }

            // Устанавливаем сообщение в TempData
            TempData["SuccessfulMessage"] = "Данные профиля успешно изменены";


            // Возвращаем представление с моделью
            if (!IsUserNameChanged)
            {
                return View("UserProfile", model);
            }
            else
            {
                return RedirectToAction("Logout");
            }
            
        }
    }
}