using System.Collections.Generic;

namespace HKDN_GroupUTE_BookStore.ViewModel
{
    public class QTV_TrangChu_LoadDuLieu_VM
    {
        // ===== NGƯỜI DÙNG =====
        public int TongSoNguoiDung { get; set; }
        public int TongSoKhachHang { get; set; }
        public int TongSoQuanTriVien { get; set; }

        // ===== ĐƠN HÀNG TỔNG QUAN =====
        public int TongDonHang { get; set; }

        // ===== 5 TRẠNG THÁI RIÊNG BIỆT (SỐ LƯỢNG) =====
        public int SlChoXacNhan { get; set; } 
        public int SlDangXuLy { get; set; }
        public int SlDangGiao { get; set; }  
        public int SlDaGiao { get; set; }
        public int SlDaHuy { get; set; }

        // ===== SÁCH =====
        public int TongSach { get; set; }
        public int TongSachTonKho { get; set; }
        public int TongSachDaBan { get; set; }
        public int TongTheLoai { get; set; }

        // ===== BIỂU ĐỒ (LIST DATA CHO 5 TRẠNG THÁI) =====
        public List<string> Thang { get; set; }

        public List<int> DonChoXacNhan { get; set; } 
        public List<int> DonDangXuLy { get; set; }
        public List<int> DonDangGiao { get; set; } 
        public List<int> DonDaGiao { get; set; }
        public List<int> DonDaHuy { get; set; }

        // ===== DỮ LIỆU DOANH THU =====
        public List<decimal> DoanhThuTheoThang { get; set; }
    }
}