using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Controllers
{
    public class PagesController : Controller
    {
        //17
        // GET: Index/{page}
        public ActionResult Index(string page ="")
        {
            // Получаем/утсанавливаем краткий заголовок(СЛАГ)
            if (page == "")
                page = "home";

            // Обьявляем модель и классс ДТО
            PageVM model;
            PagesDTO dto;
            //Проверка на доступность текущей страницы
            using(Db db = new Db())
            {
                if(!db.Pages.Any(x=>x.Slug.Equals(page)))
                {
                    return RedirectToAction("Index", new { page = "" });
                }
            }
            // ПОлучаем Дто Страницы
            using(Db db =new Db())
            {
                dto = db.Pages.Where(x=>x.Slug.Equals(page)).FirstOrDefault();
            }

            //Устанавливаем  заголовок страницы (тайтл)
            ViewBag.PageTitle = dto.Title;

            //проверяем боковую панель
            if (dto.HasSidebar == true)
            {
                ViewBag.Sidebar = "Yes";
            }
            else
            {
                ViewBag.Sidebar = "No";
            }

            //заполняем модель данными
            model =new PageVM(dto);

            //возвращаем представление с моделью
            return View(model); 
        }
        
        public ActionResult PagesMenuPartial()
        {
            // иницаилизируем лист пейдж вм
            List<PageVM> pagesVMList;
            // получаем все страницы кроме Хоум
            using(Db db = new Db())
            {
                pagesVMList=db.Pages.ToArray()
                    .OrderBy(x => x.Sorting)
                    .Where(x=>x.Slug!="home")
                    .Select(x=>new PageVM(x))
                    .ToList();
            }
            // возвращаем частичное представление с листом данными

            return PartialView("_PagesMenuPartial", pagesVMList);
        }
        public ActionResult SidebarPartial()
        {
            //обьявляем модлель
            SideBarVM model;
            //инициализируем модель данными
            using (Db db = new Db())
            {
                SideBarDTO dto = db.Sidebras.Find(1);
                model = new SideBarVM(dto);
            }
            // возвращаем модель в частичное представлении
            return PartialView("_SidebarPartial",model);
        }
    }
}