namespace HKDN_GroupUTE_BookStore.ViewModel
{
    public class QTV_TrangChu_LoadDuLieu_VM
    {
        // ===== NGƯỜI DÙNG =====
        public int TongSoNguoiDung { get; set; }
        public int TongSoKhachHang { get; set; }
        public int TongSoQuanTriVien { get; set; }

        // ===== ĐƠN HÀNG =====
        public int TongDonHang { get; set; }
        public int DaHoanThanh { get; set; } // ĐÃ GIAO
        public int ChuaHoanThanh { get; set; } // ĐANG XỬ LÝ
        public int DaGiao { get; set; }
        public int DaHuy { get; set; }

        // ===== SÁCH =====
        public int TongSach { get; set; }
        public int TongSachTonKho { get; set; }
        public int TongSachDaBan { get; set; }
        public int TongTheLoai { get; set; }

        // ===== BIỂU ĐỒ =====
        public List<string> Thang { get; set; }
        public List<int> DonDangXuLy { get; set; }
        public List<int> DonDaGiao { get; set; }
        public List<int> DonDaHuy { get; set; }
    }
}
