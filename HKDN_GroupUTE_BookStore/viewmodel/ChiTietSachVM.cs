using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKDN_GroupUTE_BookStore.ViewModel
{
    public class ChiTietSachVM
    {
        public string MaSach { get; set; }
        public string TenSach { get; set; }
        public string TacGia { get; set; }
        public string TenTheLoai { get; set; }
        public decimal DonGia { get; set; }
        public string MoTa { get; set; }
        public string Hinh { get; set; } // Tương ứng với URLAnhBia
        public int SoLuongTon { get; set; }
    }
}