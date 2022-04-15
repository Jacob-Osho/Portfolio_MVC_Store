﻿using MVC_Store.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MVC_Store
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        //25
        //метод обработки запросов аутентефикации
        protected void Application_AuthenticateRequest()
        {
            // Проверяем авторизированн ли пользователь
            if (User == null)
                return;
            // получаем имя ползователя
            string userName = Context.User.Identity.Name;

            // обьявляем массив ролей
            string[] roles = null;

           
            using(Db db = new Db())
            {
                // заполняем роли
                UserDTO dto =db.Users.FirstOrDefault(x =>x.UserName == userName);
                if (dto == null)
                    return;
                else
                {
                    roles = db.UserRoles.Where(x => x.UserId == dto.Id)
                                            .Select(x => x.Role.Name)
                                            .ToArray();
                }
                    
            }
            // создаем обьект интерфейса Iprincipal
            IIdentity userIdenity = new GenericIdentity(userName);
            IPrincipal newUserObj = new GenericPrincipal(userIdenity, roles);

            // обьявляем  и инициализируем  данными  Context.user
            Context.User = newUserObj;
        }
    }
}
