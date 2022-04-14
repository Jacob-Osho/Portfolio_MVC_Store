using MVC_Store.Models.Cart;
using MVC_Store.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            // обьявляем лист типа карт  вью модел
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // проверяем пуста ли карзина
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Your cart is empty";
                return View();
            }
            // если не пуста складываем сумму и передаем через вьюбег
            decimal total = 0m;
            foreach (var item in cart)
            {
                total += item.Total;

            }
            ViewBag.GrandTotal = total;


            //возвращаем лист в представление
            return View(cart);
        }

        //20
        public ActionResult CartPartial()
        {
            //обьявляем модель  Карт ВМ
            CartVM model = new CartVM();
            //Обьявляем переменную количества
            int qty = 0;
            // обьявляем переменную цені
            decimal price = 0m;
            //Проверяем сессию
            if (Session["cart"] != null)
            {
                //Получаем общее количесто товаров и цену 
                var list = (List<CartVM>)Session["cart"];
                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;

                }

                model.Quantity = qty;
                model.Price = price;
            }
            else
            {
                // если карзина пустая  присваиваем все о 0  что бы не было эксепшнов
                model.Quantity = 0;
                model.Price = 0m;
            }

            // возвращаем частичное представление с моделью
            return PartialView("_CartPartial", model);
        }
        //21
        public ActionResult AddToCartPartial(int id)
        {
            // обьявляем лист карт вм
            List<CartVM> cart =Session["cart"] as List<CartVM> ?? new List<CartVM>();
            //обьявляем панель картвм
            CartVM model = new CartVM();
           
            using(Db db = new Db())
            {
                //получить продукт по айди
                ProductDTO product = db.Products.Find(id);
                // проводим проверку есть ли такой товар уже  в карзине 
                var productInCart = cart.FirstOrDefault(x => x.PorductId == id);
                // если нет, то добовляем товар
                if(productInCart == null)
                {
                    cart.Add(new CartVM()
                    {
                        PorductId = product.Id,
                        ProductName = product.Name,
                        Quantity =1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }
                else
                {// если  да , то добовляем еще 1
                    productInCart.Quantity++;
                }
                
            }
            // получаем общее количество,цену и  доволяем  в модель
            int qty = 0;
            decimal price = 0;
            foreach(var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }
            model.Quantity = qty;
            model.Price = price;

            //сохраняем состояние карзины в сессию
            Session["cart"] = cart;
            //возвращаем частичное представление
            return PartialView("_AddToCartPartial", model);
        }
        //21
        // GET: /cart/IncrementProduct
        public JsonResult IncrementProduct(int productId)
        {
            // обьявляем лист картVM
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                //получаем модель cartVM с ли ста картвм
                CartVM model = cart.FirstOrDefault(x => x.PorductId == productId);
                // добовляем кол-во
                model.Quantity++;

            //сохраняем необходимые данные
            var result = new {qty = model.Quantity, price = model.Price};
                //Вернуть  джейсон ответ с данными

                return Json(result, JsonRequestBehavior.AllowGet); 
            }
        }


        public ActionResult DecrementProduct(int productId)
        {
            // обьявляем лист картVM
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                //получаем модель cartVM с ли ста картвм
                CartVM model = cart.FirstOrDefault(x => x.PorductId == productId);
                // отнимаем кол-во
                if (model.Quantity > 1)
                    model.Quantity--;
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }


                //сохраняем необходимые данные
                var result = new { qty = model.Quantity, price = model.Price };
                //Вернуть  джейсон ответ с данными

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        public void RemoveProduct(int productId)
        {
            // обьявляем лист картVM
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                //получаем модель cartVM с ли ста картвм
                CartVM model = cart.FirstOrDefault(x => x.PorductId == productId);
                // удаляем товар
                cart.Remove(model);

            }
        }
    }
}