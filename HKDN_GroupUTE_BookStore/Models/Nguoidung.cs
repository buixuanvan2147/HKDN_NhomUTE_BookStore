using System;
using System.Collections.Generic;

namespace HKDN_GroupUTE_BookStore.Models;

public partial class Nguoidung
{
    public string MaNguoiDung { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string? SoDienThoai { get; set; }

    public string? DiaChi { get; set; }

    public string? VaiTro { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual ICollection<Danhgium> Danhgia { get; set; } = new List<Danhgium>();

    public virtual ICollection<Danhsachyeuthich> Danhsachyeuthiches { get; set; } = new List<Danhsachyeuthich>();

    public virtual ICollection<Donhang> Donhangs { get; set; } = new List<Donhang>();
}
