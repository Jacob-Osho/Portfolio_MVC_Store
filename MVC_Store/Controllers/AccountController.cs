using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MVC_Store.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }
        // GET: account/create-account
        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        // POST: account/create-account
        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserVM model)
        {
            // проверяем модель на валидность
            if (!ModelState.IsValid)
                return View("CreateAccount", model);

            // проверяем соответсвие пароля
            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Password  do not match");
                return View("CreateAccount", model);
            }


            using (Db db = new Db())
            {
                // проверяем имя на уникальность
                if (db.Users.Any(x => x.UserName.Equals(model.UserName)))
                {
                    ModelState.AddModelError("", $"Username {model.UserName} is already taken. ");
                    model.UserName = "";
                    return View("CreateAccount", model);
                }

                // создаем экземпляр класса ЮзерДТО
                UserDTO userDTO = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAdress = model.EmailAdress,
                    UserName = model.UserName,
                    Password = model.Password
                };
                // Добовляем данные в модель
                db.Users.Add(userDTO);
                //  Сохроняем данные
                db.SaveChanges();

                // Добовляем роль пользователю
                int id = userDTO.Id;
                UserRoleDTO userRoleDTO = new UserRoleDTO()
                {
                    UserId = id,
                    RoleId = 2//user
                };
                db.UserRoles.Add(userRoleDTO);
                db.SaveChanges();
            }
            // записываем сообщение в ТЭмп Дата
            TempData["SM"] = "You are now registred and can login ";
            // переадресовываем пользователя
            return RedirectToAction("Login");
        }
        // GET: Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            // Подтверждаем что пользователь не авторизованн
            string userName= User.Identity.Name;
            if (!string.IsNullOrEmpty(userName))
                return RedirectToAction("user-profile");
            // возвращаем представление
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            // Проверяем модель на валидность
            if(!ModelState.IsValid)
                return View(model);

            // Проверяем пользователя на валидность
            bool isValid = false;
            using(Db db = new Db())
            {
                if(db.Users.Any(x=>x.UserName.Equals(model.Username) && x.Password.Equals(model.Password)))
                {
                    isValid = true;
                   
                }
                if(!isValid)
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    return View(model);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                    return Redirect(FormsAuthentication.GetRedirectUrl(model.Username,model.RememberMe));
                }
            }
        }

        
        //GET: /account/logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        public ActionResult UserNavPartial()
        {
            //  получаем имя пользователя
            string userName = User.Identity.Name;
            // обьявляем модель
            UserNavPartialVM model;
            using (Db db = new Db())
            {
                // получаем пользователя (User)
                UserDTO dto = db.Users.FirstOrDefault(x => x.UserName == userName);

                // заполняем модель данными с(контекста) дто
                model = new UserNavPartialVM()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };
            }
            // возвращаем частичное представление с моделью

            return PartialView("_UserNavPartial", model);
        }

        //GET: /account/user-profile
        [HttpGet]
        [ActionName("user-profile")]
        public ActionResult UserProfile()
        {
            // Получаем имя пользователя
            string userName =User.Identity.Name;
            // Обьявляем модель
            UserProfileVM model;
            using(Db db =new Db())
            {
                // Получаем пользователя
                UserDTO dto = db.Users.FirstOrDefault(x => x.UserName == userName);
                // инициализируем модель данными
                model = new UserProfileVM(dto);
            }
            //возвращаем модель представления
            return View("UserProfile",model);
        }
   
       
        //GET: /account/user-profile
        [HttpPost]
        [ActionName("user-profile")]
        public ActionResult UserProfile(UserProfileVM model)
        {
            bool userNameIsChanged = false;
            // Проверяем модель на валидность
            if(!ModelState.IsValid)
            {
                return View ("UserProfile",model);
            }

            // Прверяем пороль если пользователь его меняет  
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Passwords does not match");
                    return View("UserProfile", model);
                }
            }
            using (Db db=new Db())
            {
                
                // получаем имя пользователя
                string userName = User.Identity.Name;
                // проверяем сменилось ли имя пользователя 
                if(userName != model.UserName)
                {
                    userName = model.UserName;
                    userNameIsChanged = true;
                }
                // Проверяем имя на уникальность
                if (db.Users.Where(x => x.Id != model.Id).Any(x => x.UserName == userName))
                {
                    ModelState.AddModelError("", $"Username {model.UserName} alreay taken");
                    model.UserName = "";
                    return View("UserProfile", model);
                }
                // изменяем контектст данніх ( дто)
                UserDTO dto = db.Users.Find(model.Id);
                dto.FirstName=model.FirstName;
                dto.LastName=model.LastName;
                dto.EmailAdress = model.EmailAdress;
                dto.UserName = model.UserName;
                if(!string.IsNullOrWhiteSpace(model.Password))
                {
                    dto.Password = model.Password;
                }
                
                // сохроняем изменения
                db.SaveChanges();
            }

            // устанавливаем сообщение  в темп дата
            TempData["SM"] = "your profile has been successfully updated";

            if (!userNameIsChanged)
            {
                //  Возвращаем представление с моделью
                return View("UserProfile", model);
            }
            else
                return RedirectToAction("Logout");
            
        }
        public ActionResult Inde62x()
        {
            //
            //
            //
            //
            //
            //
            return View();
        }
    }
}