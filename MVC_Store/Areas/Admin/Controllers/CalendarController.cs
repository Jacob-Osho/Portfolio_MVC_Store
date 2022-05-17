using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Areas.Admin.Controllers
{
    public class CalendarController : Controller
    {
        // GET: Admin/Calendar
		[HttpGet]
        public ActionResult BookDate()
        {
            return View(new EventVM());
        }
        public JsonResult GetEvents()
        {
            using(Db db = new Db())
            {
                var events = db.Events.ToList();
                return new JsonResult { Data = events, JsonRequestBehavior= JsonRequestBehavior.AllowGet };
            }

           
        }
        //POST Admin/Calendar/SaveEvent
        [HttpPost]
        public JsonResult SaveEvent(EventVM model)
        {
            var status = false;
            EventDTO dto = new EventDTO
            {
                Subject = model.Subject,
                Start = model.Start,
                End = model.End,
                Description = model.Description,
                IsFullDay = model.IsFullDay,
                ThemeColor = model.ThemeColor
            };
            using (Db db = new Db())
            {
                if (model.EventID > 0)
                {
                    //Update event
                    var newEvent = db.Events.Where(x =>x.EventID == model.EventID).FirstOrDefault();
                    if (newEvent != null)
                    {
                        newEvent.Subject = model.Subject;
                        newEvent.Start = model.Start;
                        newEvent.End = model.End;
                        newEvent.Description = model.Description;
                        newEvent.IsFullDay = model.IsFullDay;
                        newEvent.ThemeColor = model.ThemeColor;
                    }
                }
                else
                {
                    //Create New Event
                    db.Events.Add(dto);
                }
                db.SaveChanges();
                status = true;
            }
            return new JsonResult { Data = new { status = status } };
        }

        //POST Admin/Calendar/DeleteEvent
        [HttpPost] 
        public JsonResult DeleteEvent( int id)
        {
            var status = false;
            using(Db db = new Db())
            {
                var events = db.Events.Where(x => x.EventID == id).FirstOrDefault();
                if(events != null)
                {
                    db.Events.Remove(events);
                    db.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data= new { status = status } };
        }
        //?
        // GET: Admin/Calendar
        //[HttpGet]
        //public ActionResult GetEvents()
        //      {
        //	return View();
        //      }
    }
}