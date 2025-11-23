using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HKDN_GroupUTE_BookStore.ViewModel
{
    public class QTV_QLSP_TheLoai_List
    {
        public string MaTheLoai { get; set; }
        public string TenTheLoai { get; set; }
        public int SoLoaiSach { get; set; } // Số đầu sách khác nhau
        public int TongSachTheoTheLoai { get; set; }// Tổng số sách (tính theo tồn kho)
    }
}