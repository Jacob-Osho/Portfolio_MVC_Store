using MVC_Store.Areas.Admin.Models.ViewModels.Shop;
using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Shop;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace MVC_Store.Areas.Admin.Controllers
{
    [Authorize(Roles ="Admin")]
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
        // GET: Admin/Shop/DeleteCategory/id
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
            using (Db db = new Db())
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
        // POST: Admin/Shop/ReorderCategories
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
        //10
        // POST: Admin/Shop/RenameCategory/id
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            //проверить имя на уникальность
            using (Db db = new Db())
            {
                if (db.Categories.Any(x => x.Name == newCatName))
                {
                    return "titletaken";
                }
                //получаем класс ДТО с бд
                CategoryDTO dto = db.Categories.Find(id);
                //редактируем модель ДТО
                dto.Name = newCatName;
                dto.Slag = newCatName.Replace(" ", "-").ToLower();

                //сохранить изменения
                db.SaveChanges();
            }
            //возвращаем слово
            return "ok";
        }
        //11
        //создаем метод добавления товаров
        // GET: Admin/Shop/AddProduct
        [HttpGet]
        public ActionResult AddProduct()
        {
            //обьявляем модель данных
            ProductVM model = new ProductVM();
            // добавляем Список категорий из базы в модель
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");

            }
            // возвращаем модель
            return View(model);
        }
        //12
        //создаем метод добавления товаров
        // POST: Admin/Shop/AddProduct
        [HttpPost]
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            //проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
                    return View(model);
                }
            }
            // проверяем имя продукта на уникальность
            using (Db db = new Db())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "That product name is taken");
                    model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
                    return View(model);
                }
            }
            // обьявляем прееменную продукт айди
            int id;
            // инициализируем и сохраняем в базу модель на основе продукт дто
            using (Db db = new Db())
            {
                ProductDTO product = new ProductDTO();
                product.Name = model.Name;
                product.Slug = model.Name.Replace(" ", "-").ToLower();
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;


                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                product.CategoryName = catDTO.Name;
                db.Products.Add(product);
                db.SaveChanges();
                id = product.Id;
            }
            // выводим сообщение пользователю Темп Дейта
            TempData["SM"] = "You have added product ";

            // Метод создания директории по сохранению  картинок
            #region Upload Image
            // создаем ссылки директории
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            // проверяем наличие директорий (если нет то создаем)
            if (!Directory.Exists(pathString1))
            {
                Directory.CreateDirectory(pathString1);
            }
            if (!Directory.Exists(pathString2))
            {
                Directory.CreateDirectory(pathString2);
            }
            if (!Directory.Exists(pathString3))
            {
                Directory.CreateDirectory(pathString3);
            }
            if (!Directory.Exists(pathString4))
            {
                Directory.CreateDirectory(pathString4);
            }
            if (!Directory.Exists(pathString5))
            {
                Directory.CreateDirectory(pathString5);
            }

            // проверяем был ли файл згружен 
            if (file != null && file.ContentLength > 0)
            {
                //Получаем расширение файла
                string ext = file.ContentType.ToLower();
                // проверяем  расширение файла
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpg" &&
                    ext != "image/gif" &&
                    ext != "image/x-jpg" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "The image was not uploaded! Wrong image extention");
                        return View(model);
                    }
                }


                // обьявляем переменную с именем изображения
                string imgName = file.FileName;

                // сохроняем изображение в модель ДТО
                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imgName;
                    db.SaveChanges();
                }

                //  назначаем пути к оригинальному и  уменьшеному изображению

                string path = string.Format($"{pathString2}\\{imgName}");
                string path2 = string.Format($"{pathString3}\\{imgName}");

                //сохроняем оригинальное изображение
                file.SaveAs(path);

                // создаем и сохроняем уменьшенную копию
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200).Crop(1,1);
                img.Save(path2);
            }
            #endregion

            // переодресовать пользователя
            return RedirectToAction("AddProduct");

        }

        //13(PageList.MVC)
        //создаем   методсписка товаров
        // GET: Admin/Shop/Products 
        [HttpGet]
        public ActionResult Products(int? page, int? catId)
        {
            int numberOfGoodsOnPage = 15;
            //обьявить модель продакт ВМ (лист)
            List<ProductVM> listOfProductVM;

            //устанавливаем номер страницы
            var pageNumber = page ?? 1;
            using (Db db = new Db())
            {
                // инициализируем лист заполняем данными
                listOfProductVM = db.Products.ToArray()
                        .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                        .Select(x => new ProductVM(x))
                        .ToList();
                //  запонляем категории данными
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                // устанавливаем выбранную категорию
                ViewBag.SelectedCat = catId.ToString();
            }
            //устанавливаем постраничную навигацию
            var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, numberOfGoodsOnPage);
            ViewBag.onePageOfProducts = onePageOfProducts;

            // возвращаем все это в представление
            return View(listOfProductVM);
        }

        //14
        //создаем редактирование товаров
        // GET: Admin/Shop/EditProduct
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            //обьявляем модель продактВМ
            ProductVM model;
            using (Db db = new Db())
            {
                // получаем все поля продукта
                ProductDTO dto = db.Products.Find(id);
                // проверяем доступен ли продукт
                if (dto == null)
                {
                    return Content("That product does not exist");
                }
                // инициализируем модель данных
                model = new ProductVM(dto);

                // создаем список категорий
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                // получаем все изороброжения 
                model.GalleryImages = Directory
                    .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                    .Select(fn => Path.GetFileName(fn));
            }
            // возвращаем модель в представление
            return View(model);
        }
        //14
        //создаем редактирование товаров
        // POST: Admin/Shop/EditProduct
        [HttpPost]
        public ActionResult EditProduct(ProductVM model, HttpPostedFileBase file)
        {
            //получаем айди продукта
            int id = model.Id;
            //заполняем список категориями 
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            //заполняем список  изображеними
            model.GalleryImages = Directory
                   .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                   .Select(fn => Path.GetFileName(fn));

            // проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //проверяем имя продукта на уникальность
            using (Db db = new Db())
            {
                if (db.Products.Where(x => x.Id != id)
                    .Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "That product name is already taken");
                    return View(model);
                }
            }
            //обновляем продукт
            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);
                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-").ToLower();
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.CategoryId = model.CategoryId;
                dto.ImageName = model.ImageName;
                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = catDTO.Name;
                db.SaveChanges();
            }
            //установить сообщение в тэмп дейта
            TempData["SM"] = "You have edited  the product ";
            #region ImageUpload
            //15
            // проверить загружен ли файл
            if (file != null && file.ContentLength > 0)
            {
                // получить расширение файла
                string ext = file.ContentType.ToLower();
                // проверряем расширение
                if (ext != "image/jpg" &&
                   ext != "image/jpeg" &&
                   ext != "image/pjpg" &&
                   ext != "image/gif" &&
                   ext != "image/x-jpg" &&
                   ext != "image/png")
                {
                    using (Db db = new Db())
                    {

                        ModelState.AddModelError("", "The image was not uploaded! Wrong image extention");
                        return View(model);
                    }
                }

                // устанавливаем пути для загрузки
                var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                var pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
                var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");


                //удаляем существующие файлы в дерикториях и папки
                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);
                foreach (var file2 in di1.GetFiles())
                {
                    file2.Delete();

                }
                foreach (var file3 in di2.GetFiles())
                {
                    file3.Delete();

                }
                // сахроняем имя изоброжение
                string imgName = file.FileName;


                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imgName;
                    db.SaveChanges();
                }
                // сохраняем оригинал и привью версии
                string path = string.Format($"{pathString1}\\{imgName}");
                string path2 = string.Format($"{pathString2}\\{imgName}");
                //сохроняем оригинальное изображение
                file.SaveAs(path);

                // создаем и сохроняем уменьшенную копию
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200).Crop(1,1);
                img.Save(path2);

            }


            #endregion
            //переадресовать пользователя
            return RedirectToAction("EditProduct");
        }

        //15
        //создаем метод удаления товаров
        // POST: Admin/Shop/DeleteProduct/id
        public ActionResult DeleteProduct(int id)
        {
            using (Db db = new Db())
            {
                //получаем данные страницы по айди
                ProductDTO dto = db.Products.Find(id);

                //удаляем  страницу
                db.Products.Remove(dto);

                // сохраняем изменения в базе
                db.SaveChanges();
            }
            //удаляем директории товара (изоражения )
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
            var deleteDirPath = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            try
            {
                if (Directory.Exists(deleteDirPath))
                    Directory.Delete(deleteDirPath, true);
            }
            catch (Exception ex)
            {
                
                TempData["SM"] = $" not fully deleted product {ex.Message}";
                return RedirectToAction("Products");
            }
            
            
         
            //Оповесщаем пользователя о упешном сохронении  
            TempData["SM"] = "You have succesfully deleted product";
            return RedirectToAction("Products");
        }
        //16
        //создаем метод Добовления  изорожений в галереб
        // POST: Admin/Shop/SaveGalleryImages/id

        [HttpPost]
        public void SaveGalleryImages(int id)
        {
            // перебираем все полученные файлы из представления
            foreach (string fileName in Request.Files)
            {
                //инициализируем файлы
                HttpPostedFileBase file = Request.Files[fileName];
                // проверяем файлы на null
                if (file != null && file.ContentLength > 0)
                {
                    // Назначаем пути к директориям
                    var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
                    var pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
                    var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

                    // Назначаем пути самих изоброжений
                    var path1 = string.Format($"{pathString1}\\{file.FileName}");
                    var path2 = string.Format($"{pathString2}\\{file.FileName}");
              
                    file.SaveAs(path1);
                    // Сохроняем оригинальные и уменьшенные копии
                    WebImage img = new WebImage(file.InputStream);
                 
                    img.Resize(200,200).Crop(1,1);
                    img.Save(path2);
                }
            }
        }

        //метод удаления изобродений из галереи 16
        // POST: Admin/Shop/DeleteImage/id/imageName
        public void DeleteImage(int id , string imageName)
        {
            var fullPath1 = Request.MapPath("~/Images/Uploads/Products/"+id.ToString()+"/Gallery/"+ imageName);
            var fullPath2 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/Thumbs/" + imageName);
            if(System.IO.File.Exists(fullPath1))
            {
                System.IO.File.Delete(fullPath1);
            }
            if (System.IO.File.Exists(fullPath2))
            {
                System.IO.File.Delete(fullPath2);
             }

        }
        //27
        // Создаем метод вывода всех товаров
        // GET: Admin/Shop/Orders
        public ActionResult Orders()
        {
            //  инициализируем модель ордерсфорадминВМ
           List< OrdersForAdminVM> ordersForAdmin= new List<OrdersForAdminVM>();
            using(Db db = new Db())
            {
                // инициализируем модель ордерВМ
                List<OrderVM> orders = db.Orders.ToArray().Select(x => new OrderVM(x)).ToList();
              
                // Перебрать весь список товаров и заполнить нашу модель оредВМ
                foreach(var order in orders)
                {
                    //  Инициализируем Словаь товаро
                    Dictionary<string,int> productsAndQty = new Dictionary<string,int>();
                    // создаем переменную для общей суммы
                    decimal total = 0m;
                    // Инициализируем лист ордерсДетейлс
                    List<OrderDetailsDTO> orderDetailsList = db.OrderDetails.Where(x => x.OrderId == order.OrderId).ToList();
                    //  получаем имя пользователя
                    UserDTO user = db.Users.FirstOrDefault(x=>x.Id == order.UserId);
                    string userName = user.UserName;
                    // перебираем илст товаров ордердетейлсДТО
                    foreach(var orderDetails in orderDetailsList)
                    {
                        // Получаем товар
                        ProductDTO product = db.Products.FirstOrDefault(x => x.Id == orderDetails.ProductId);
                        // получаем цену товара 
                        decimal price = product.Price;
                        //  получаем название товар
                        string productName = product.Name;
                        // добовляем товар в словарь
                        productsAndQty.Add(productName, orderDetails.Quantity);
                        //  Получаем полную стоимость товаров
                        total += orderDetails.Quantity * price;
                    }
                    // добовляем данные в ордерсфорадминВМ
                    ordersForAdmin.Add(new OrdersForAdminVM()
                    {
                        OrderNumber = order.OrderId,
                        UserName = userName,
                        Total = total,
                        ProductsAndQty = productsAndQty,
                        CreatedAt = order.CreatedAt
                    }); 
                }
            }
            // возвращаем предстовление с моделью
            return View(ordersForAdmin);
        }
    }
    
}