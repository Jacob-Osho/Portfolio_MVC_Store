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
                //получаем данные категории по айди
                CategoryDTO dto = db.Categories.Find(id);

                //удаляем  категорию
                db.Categories.Remove(dto);

                // сохраняем изменения в базе
                db.SaveChanges();
            }
          

            //Оповесщаем пользователя о упешном сохронении  
            TempData["SM"] = "You have succesfully deleted page";
            //переадресовываем пользователя  на страницу категории
            return RedirectToAction("Categories");
        }
      
        //9
        // POST: Admin/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            //обьявляем строковую прееменную айди
            string id;
            using(Db db =new Db())
            {
                //провекра имя категории на уникальность
                 if (db.Categories.Any(x => x.Name == catName))
                 {
                    return "titletaken";
                 }
                //инициализируем модель ДТО
                    CategoryDTO dto = new CategoryDTO();
                //заполняем данными модель
                dto.Name = catName;
                dto.Slag = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;
                //сохраняем
                db.Categories.Add(dto);
                db.SaveChanges();
                //получаем айди что бы вернуть его представление
                id = dto.Id.ToString();
            }
            //Возвращаем айди 
            return id;
        }

        //9
        // Post: Admin/Shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (Db db = new Db())
            {
                //счетчик
                int count = 1;//еденица потому что хоум мы не будем сортировать

                //иннициализируем модель данных
                CategoryDTO dto;

                //устанавливаем сортировку для каждой страницы
                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;
                    db.SaveChanges();
                    count++;
                }
            }
        }
    } 
}