using BCCK_CSharp_BookShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCCK_CSharp_BookShop.ViewModel
{
    public class Index_Home_ListSach
    {
        public List<Sach> TatCaSach { get; set; }
        public List<Sach> SachHot { get; set; }
        public List<Sach> SachXuHuong { get; set; }
    }
}