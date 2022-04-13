using MVC_Store.Models.Cart;
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
              var cart = Session["cart"] as List<CartVM> ?? new List<CartVM> ();

            // проверяем пуста ли карзина
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Your cart is empty";
                return View();
            }
            // если не пуста складываем сумму и передаем через вьюбег
            decimal total = 0m;
            foreach(var item in cart)
            {
                total += item.Tottal;
               
            }
            ViewBag.GrandTotal = total;

            
            //возвращаем лист в представление
            return View(cart);
        }

        //18
        public ActionResult CartPartial()
        {
            //обьявляем модель  Карт ВМ
            CartVM model = new CartVM();
            //Обьявляем переменную количества
            int qty = 0;
            // обьявляем переменную цені
            decimal price = 0m;
            //Проверяем сессию
            if(Session["cart"]!=null)
            {
                //Получаем общее количесто товаров и цену 
                var list = (List<CartVM>)Session["cart"];
                foreach(var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;

                }
            }
            else
            {
                // если карзина пустая  присваиваем все о 0  что бы не было эксепшнов
                model.Quantity = 0;
                model.Price = 0m;
            }

            // возвращаем частичное представление с моделью
            return PartialView("_CartPartial",model);
        }
    }
}