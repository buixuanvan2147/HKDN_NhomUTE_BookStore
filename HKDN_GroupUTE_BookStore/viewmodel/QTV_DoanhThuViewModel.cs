using System.ComponentModel.DataAnnotations;

namespace HKDN_GroupUTE_BookStore.ViewModel
{
    public class QTV_DoanhThuViewModel
    {
        // ===== CHỜ XÁC NHẬN =====
        [Display(Name = "Số lượng đơn hàng chờ xác nhận")]
        public int SoLuongChoXacNhan { get; set; }

        [Display(Name = "Tổng tiền đơn hàng chờ xác nhận")]
        public decimal TongTienChoXacNhan { get; set; }

        // ===== ĐANG XỬ LÝ =====
        [Display(Name = "Số lượng đơn hàng đang xử lý")]
        public int SoLuongDangXuLy { get; set; }

        [Display(Name = "Tổng tiền đơn hàng đang xử lý")]
        public decimal TongTienDangXuLy { get; set; }

        // ===== ĐANG GIAO =====
        [Display(Name = "Số lượng đơn hàng đang giao")]
        public int SoLuongDangGiao { get; set; }

        [Display(Name = "Tổng tiền đơn hàng đang giao")]
        public decimal TongTienDangGiao { get; set; }

        // ===== ĐÃ GIAO =====
        [Display(Name = "Số lượng đơn hàng đã giao")]
        public int SoLuongDaGiao { get; set; }

        [Display(Name = "Tổng tiền đơn hàng đã giao")]
        public decimal TongTienDaGiao { get; set; }

        // ===== ĐÃ HỦY =====
        [Display(Name = "Số lượng đơn hàng đã hủy")]
        public int SoLuongDaHuy { get; set; }

        [Display(Name = "Tổng tiền đơn hàng đã hủy")]
        public decimal TongTienDaHuy { get; set; }

        public QTV_DoanhThuViewModel()
        {
            SoLuongChoXacNhan = 0;
            TongTienChoXacNhan = 0;

            SoLuongDangXuLy = 0;
            TongTienDangXuLy = 0;

            SoLuongDangGiao = 0;
            TongTienDangGiao = 0;

            SoLuongDaGiao = 0;
            TongTienDaGiao = 0;

            SoLuongDaHuy = 0;
            TongTienDaHuy = 0;
        }
    }
}
