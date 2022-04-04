using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            //обявьяем списокдля преставления(PageVM)

            List<PageVM> pageList;
            //запонлнить список с базы данных(инициализировать ДБ)
            using (Db db = new Db())
            {
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }
            //Вернуть все это в представление
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
        public ActionResult AddPage(PageVM model)
        {
            int sortingAmount = 100;
            //проверка модели на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {
                //обьявляем переменную для краткого описания(slug)

                string slug;

                //инициализируем класс  pageDTO

                PagesDTO dto = new PagesDTO();

                //приписываем заголовок нашей модели
                dto.Title = model.Title.ToUpper();

                //проверяем есть ли краткое описание если нет присваимваем

                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                //проверяем заголовок и краткое описание на уникальность

                if (db.Pages.Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title  already exist");
                    return View(model);
                }
                else if (db.Pages.Any(x => x.Slug == model.Slug))
                {
                    ModelState.AddModelError("", "That slug  already exist");
                    return View(model);
                }

                //присваеваем оставшееся значение нашей модели 
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = sortingAmount;

                //сохроняем модель в  базу данных
                db.Pages.Add(dto);
                db.SaveChanges();
            }
            //Передаем сообщение через TempData

            TempData["SM"] = "You have added new page";

            // переадресовываем пользователя на метод индекса

            return RedirectToAction("Index");
        }
        // Get: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            //обьявим модель    ПэджВМ
            PageVM model;
            using (Db db = new Db())
            {
                //получаем данные страницы по айди
                PagesDTO dto = db.Pages.Find(id);

                //проверяем доступность этой страницы
                if (dto == null)
                {
                    return Content("The page does not exists");
                }

                //инициализируем модель данными из дто
                model = new PageVM(dto);
            }
            //возвращаем представление с моделью

            return View(model);
        }

        // POST: Admin/Pages/EditPage/id
        [HttpPost]
        public ActionResult EditPage(PageVM model)  
        {

            //проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {
                // getting Page Id
                int id = model.Id;

                //обьявляем прееменную для слага
                string slug = "home";

                // получаем страницу по айди
                PagesDTO dto = db.Pages.Find(id);

                //присваиваем в ДТО название модели
                dto.Title = model.Title;

                //Проверяем слаг на присутствие и если его нет то присвоить
                if(model.Slug != "home")
                {
                    if(string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ","-").ToLower();
                    }
                    else
                    {
                        slug=model.Slug.Replace(" ","-").ToLower();
                    }
                }
                //Проверка заголовка и слаг на уникальность

                if (db.Pages.Where(x => x.Id !=id).Any(x =>x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title  already exist");
                    return View(model);
                }
                else if (db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "That slug  already exist");
                    return View(model);
                }


                //присваеваем оставшееся значение нашей модели 
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                //сохроняем changes в  базу данных
                
                db.SaveChanges();
            }
            //Оповесщаем пользователя о упешном сохронении  
            TempData["SM"] = "You have edited the page";

            //Переодресовать пользователя на страницу которую он редоктировал
            return RedirectToAction("EditPage");
        }

        // GET: Admin/Pages/PageDetails/id
        public ActionResult PageDetails(int id)
        {
            //обявляем модель Пээейдж ВМ
            PageVM model;
            using (Db db = new Db())
            {
                //получаем данные страницы по айди
                PagesDTO dto = db.Pages.Find(id);

                // убеждаемся что страница доступна
                if (dto == null)
                {
                    return Content("The page does not exists");
                }
                
                // присвоиваем из базы данных все поля
                model = new PageVM(dto);
            }
            //вернуть модель представления
            return View(model);
        }

        //Метод Удаления страниц
        // GET: Admin/Pages/DeletePage/id
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                //получаем данные страницы по айди
                PagesDTO dto = db.Pages.Find(id);

                //удаляем  страницу
                db.Pages.Remove(dto);

                // сохраняем изменения в базе
                db.SaveChanges();
            }
            //переадресовываем пользователя  на страницу индекс

            //Оповесщаем пользователя о упешном сохронении  
            TempData["SM"] = "You have succesfully deleted page";
            return RedirectToAction("Index");
        }

        
    }

    
}