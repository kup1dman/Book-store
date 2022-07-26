using BookStore.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Models.ViewModels.Pages
{
    public class SidebarViewModel
    {
        public SidebarViewModel() { }

        public SidebarViewModel(SidebarsDTO row)
        {
            Id = row.Id;
            Body = row.Body;
        }

        public int Id { get; set; }

        [AllowHtml]
        public string Body { get; set; }
    }
}