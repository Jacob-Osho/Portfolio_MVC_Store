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
                    TempData["SM"] = "Succes";
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
        public ActionResult Inde6x()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login ");
        }
        [DisplayName("user-profile")]
        public ActionResult In4dex()
        {
            //
            //
            //
            //
            //
            //
            return View();
        }
   
        public ActionResult Inde1x()
        {
            //
            //
            //
            //
            //
            //
            return View();
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