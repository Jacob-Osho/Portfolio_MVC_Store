using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Controllers
{
    public class ShopController : Controller
    {
        //18
        // GET: Shop
        public ActionResult Index()
        {

            return RedirectToAction("Index", "Pages");//переадресация на другой метод в другом контроллере
        }

        public ActionResult СategoryMenuPartial()
        {
            //обьявляем модель типа лист категори вм
            List<CategoryVM> categoriesVmList;
            //инициализируем модель данными
            using (Db db = new Db())
            {
                categoriesVmList = db.Categories.ToArray()
                    .OrderBy(x => x.Sorting)
                    .Select(x => new CategoryVM(x))
                    .ToList();
            }
            //возвращаем частичное представление
            return PartialView("_СategoryMenuPartial", categoriesVmList);
        }
    }
}