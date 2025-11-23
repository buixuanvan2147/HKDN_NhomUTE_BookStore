using System;
using System.Collections.Generic;

namespace HKDN_GroupUTE_BookStore.Models;

public partial class Danhgium
{
    public int MaDanhGia { get; set; }

    public string? MaSach { get; set; }

    public string? MaNguoiDung { get; set; }

    public int? DiemDanhGia { get; set; }

    public string? BinhLuan { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual Nguoidung? MaNguoiDungNavigation { get; set; }

    public virtual Sach? MaSachNavigation { get; set; }
}
