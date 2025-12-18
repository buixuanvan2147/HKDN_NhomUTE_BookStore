using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HKDN_GroupUTE_BookStore.Models
{
    [Table("LienHe")]
    public class LienHe
    {
        [Key]
        public int MaLienHe { get; set; }

        [StringLength(10)]
        public string? MaNguoiDung { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; }

        [Required]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        [StringLength(200)]
        public string TieuDe { get; set; }

        [Required]
        public string NoiDung { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "ChuaXuLy";

        public string? PhanHoiAdmin { get; set; }

        public DateTime NgayGui { get; set; }

        public DateTime? NgayPhanHoi { get; set; }

        // Navigation (không bắt buộc)
        [ForeignKey("MaNguoiDung")]
        public virtual Nguoidung? NguoiDung { get; set; }
    }
}
