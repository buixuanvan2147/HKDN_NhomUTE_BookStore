using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCCK_CSharp_BookShop.ViewModel
{
    public class QTV_QuanLySanPham_LoadDuLieu_VM
    {
        public string MaSach { get; set; }
        public string AnhBia { get; set; }
        public string TenSach { get; set; }
        public string TheLoai { get; set; }
        public string MaTheLoai { get; set; }
        public string TacGia { get; set; }
        public decimal Gia { get; set; }
        public int DaBan { get; set; }
        public int TonKho { get; set; }
        public List<string> DanhSachTenTheLoai { get; set; }
    }
}