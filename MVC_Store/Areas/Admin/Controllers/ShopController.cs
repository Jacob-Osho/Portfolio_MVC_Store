using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop
        public ActionResult Categories()
        {
            //  обьявляем модель типа лист
            List<CategoryVM> categoryVMList;
            //инициализируем модель  данніми
            using (Db db = new Db())
            {
                categoryVMList = db.Categories.ToArray()
                    .OrderBy(x => x.Sorting)
                    .Select(x => new CategoryVM(x))
                    .ToList();
            }
            // возвращаем лист с представлениями
       
            return View(categoryVMList);
        }
        //Метод Удаления страниц
        // GET: Admin/Pages/DeleteCategory/id
        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                //получаем данные страницы по айди
                CategoryDTO dto = db.Categories.Find(id);

                //удаляем  страницу
                db.Categories.Remove(dto);

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