using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
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
            List<CategoryVM> categoriesVMList;
            //инициализируем модель данными
            using (Db db = new Db())
            {
                categoriesVMList = db.Categories.ToArray()
                    .OrderBy(x => x.Sorting)
                    .Select(x => new CategoryVM(x))
                    .ToList();
            }
            //возвращаем частичное представление
            return PartialView("_СategoryMenuPartial", categoriesVMList);
        }
        //19
        // GET: Shop/Сategory/name
        public ActionResult Сategory(string name)
        {
            //обьявляем список типа лист
            List<ProductVM> productVMModel;
            
            using(Db db = new Db())
            {
                //получаем айди категори
                CategoryDTO categoryDTO = db.Categories.Where(x => x.Slag == name).FirstOrDefault();
                int catId = categoryDTO.Id;
                // инициализируем спиоск данными
                productVMModel = db.Products.ToArray()
                    .Where(x => x.CategoryId == catId)
                    .Select(x => new ProductVM(x))
                    .ToList();
                //получаем имя категории
                var productCat = db.Products.Where(x=>x.CategoryId == catId).FirstOrDefault();
                // проверяем на налл
                if(productCat == null)
                {
                    var catName = db.Categories
                        .Where(x=>x.Slag == name)
                        .Select(x=>x.Name)
                        .FirstOrDefault();  
                    ViewBag.CategoryName = catName;
                }
                else
                {
                    ViewBag.CategoryName = productCat.CategoryName;
                }
            }

            // возвращаем представление с моделью
            return View(productVMModel);
        }
        //19
        // GET: Shop/product-details/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            // Обьявляем модели ДТО И ВМ
            ProductDTO dto;
            ProductVM model;
            // Инициализируем айди  продукта
            int id = 0;
            using (Db db =new Db())
            {
                
                
                
                // Проверяем доступен ли продукт
                if(!db.Products.Any(x => x.Slug.Equals(name)))
                {
                    return RedirectToAction("Index","Shop");
                }
                // инициализируем  модель продукт дто Данными
                dto = db.Products.Where(x=>x.Slug == name).FirstOrDefault();
                // Получаем АЙДИ
                id = dto.Id;
                // Инициализируем Модель (ВМ) данными
                model = new ProductVM(dto);
            }
            //получаем изоброжение из голереи
            model.GalleryImages = Directory
                .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/"+id+"/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));
            
            // возвращаем модель в представление
            return View("ProductDetails", model);
        }
    }
}