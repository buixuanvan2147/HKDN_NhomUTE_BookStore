using System;
using System.Collections.Generic;

namespace HKDN_GroupUTE_BookStore.Models;

public partial class Danhsachyeuthich
{
    public int MaYeuThich { get; set; }

    public string? MaNguoiDung { get; set; }

    public string? MaSach { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual Nguoidung? MaNguoiDungNavigation { get; set; }

    public virtual Sach? MaSachNavigation { get; set; }
}
