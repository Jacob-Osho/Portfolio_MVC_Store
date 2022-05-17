using MVC_Store.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_Store.Models.ViewModels.Pages
{
    public class EventVM
    {
        public EventVM()
        {

        }
        public EventVM(EventDTO row)
        {
            EventID = row.EventID;
            Subject = row.Subject;
            Description = row.Description;
            Start = row.Start;
            End = row.End;
            ThemeColor = row.ThemeColor;
            IsFullDay = row.IsFullDay;

        }
        public int EventID { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int ThemeColor { get; set; }
        public bool IsFullDay { get; set; }
    }
}