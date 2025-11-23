using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HKDN_GroupUTE_BookStore.ViewModel
{
    public class QTV_TrangChu_LoadDuLieu_VM
    {
        public int TongSoNguoiDung { get; set; }
        public int TongSoKhachHang { get; set; }
        public int TongSoQuanTriVien { get; set; }

        public int TongDonHang { get; set; }
        public int DaHoanThanh { get; set; } //Đơn hàng đã giao + đã hủy
        public int ChuaHoanThanh { get; set; } //Đơn hàng đang xử lý
        public int DaGiao { get; set; } // Đơn hàng đã giao
        public int DaHuy { get; set; } // Đơn hàng đã hủy

        public int TongSach { get; set; }
        public int TongSachTonKho { get; set; }
        public int TongSachDaBan { get; set; }
        public int TongTheLoai {  get; set; } // tổng số thể loại sách
    }
}