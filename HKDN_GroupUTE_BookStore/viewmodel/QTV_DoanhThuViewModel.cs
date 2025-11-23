using System.ComponentModel.DataAnnotations;

namespace HKDN_GroupUTE_BookStore.ViewModel
{
    public class QTV_DoanhThuViewModel
    {
        [Display(Name = "Số lượng đơn hàng đang xử lý")]
        public int SoLuongDonHangDangXuLy { get; set; }

        [Display(Name = "Tổng tiền đơn hàng đang xử lý")]
        public decimal TongTienDangXuLy { get; set; }

        [Display(Name = "Số lượng đơn hàng đã giao")]
        public int SoLuongDonHangDaGiao { get; set; }

        [Display(Name = "Tổng tiền đơn hàng đã giao")]
        public decimal TongTienDaGiao { get; set; }

        [Display(Name = "Số lượng đơn hàng đã hủy")]
        public int SoLuongDonHangDaHuy { get; set; }

        [Display(Name = "Tổng tiền đơn hàng đã hủy")]
        public decimal TongTienDaHuy { get; set; }

        public QTV_DoanhThuViewModel()
        {
            SoLuongDonHangDangXuLy = 0;
            TongTienDangXuLy = 0;
            SoLuongDonHangDaGiao = 0;
            TongTienDaGiao = 0;
            SoLuongDonHangDaHuy = 0;
            TongTienDaHuy = 0;
        }
    }
}
