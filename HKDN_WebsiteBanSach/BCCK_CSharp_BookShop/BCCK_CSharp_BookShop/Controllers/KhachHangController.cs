using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCCK_CSharp_BookShop.Models;

namespace BCCK_CSharp_BookShop.Controllers
{
    public class KhachHangController : Controller
    {
        CSharp_BookShopEntities db = new CSharp_BookShopEntities();

        // Model đơn giản cho mục trong giỏ hàng
        public class CartItem
        {
            public string MaSach { get; set; }
            public string TenSach { get; set; }
            public decimal Gia { get; set; }
            public int SoLuong { get; set; }
            public string URLAnhBia { get; set; }
        }

        // Model cho view User
        public class UserViewModel
        {
            public string MaNguoiDung { get; set; }
            public string HoTen { get; set; }
            public string Email { get; set; }
            public string SoDienThoai { get; set; }
            public string DiaChi { get; set; }
            public List<DonHangInfo> DonHangs { get; set; }
        }

        public class DonHangInfo
        {
            public string MaDonHang { get; set; }
            public DateTime NgayTao { get; set; }
            public string TrangThaiDonHang { get; set; }
            public decimal TongTien { get; set; }
            public List<ChiTietDonHangInfo> ChiTiets { get; set; }
        }

        public class ChiTietDonHangInfo
        {
            public string MaSach { get; set; }
            public string TenSach { get; set; }
            public string URLAnhBia { get; set; }
            public decimal GiaBan { get; set; }
            public int SoLuong { get; set; }
        }

        // Lấy key session dựa trên MaNguoiDung
        private string GetCartSessionKey()
        {
            string maNguoiDung = Session["UserMaNguoiDung"]?.ToString();
            return string.IsNullOrEmpty(maNguoiDung) ? "GioHang_Temp" : "GioHang_" + maNguoiDung;
        }

        // Action để kiểm tra trạng thái đăng nhập
        [HttpPost]
        public JsonResult KiemTraDangNhap()
        {
            bool isLoggedIn = Session["UserName"] != null;
            return Json(new { loggedIn = isLoggedIn });
        }

        // Action để thêm, giảm, hoặc xóa sản phẩm trong giỏ hàng
        [HttpPost]
        public JsonResult ThemVaoGioHang(string maSach, bool giamSoLuong = false, bool xoaSanPham = false)
        {
            try
            {
                string sessionKey = GetCartSessionKey();
                List<CartItem> gioHang = Session[sessionKey] as List<CartItem> ?? new List<CartItem>();

                if (xoaSanPham)
                {
                    gioHang.RemoveAll(x => x.MaSach == maSach);
                    Session[sessionKey] = gioHang;
                    return Json(new { success = true, message = "Xóa sản phẩm thành công!" });
                }

                var sach = db.Saches.FirstOrDefault(s => s.MaSach == maSach);
                if (sach == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại." });
                }

                var item = gioHang.FirstOrDefault(x => x.MaSach == maSach);
                if (giamSoLuong && item != null)
                {
                    if (item.SoLuong > 1)
                    {
                        item.SoLuong--;
                    }
                    else
                    {
                        gioHang.Remove(item);
                    }
                    Session[sessionKey] = gioHang;
                    return Json(new { success = true, message = "Giảm số lượng thành công!" });
                }

                if (item != null)
                {
                    item.SoLuong++;
                }
                else
                {
                    gioHang.Add(new CartItem
                    {
                        MaSach = sach.MaSach,
                        TenSach = sach.TenSach,
                        Gia = sach.Gia,
                        SoLuong = 1,
                        URLAnhBia = sach.URLAnhBia
                    });
                }

                Session[sessionKey] = gioHang;
                return Json(new { success = true, message = "Thêm thành công vào giỏ hàng!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã có lỗi xảy ra: " + ex.Message });
            }
        }

        // Action để đặt hàng
        [HttpPost]
        public JsonResult DatHang(string diaChiGiao)
        {
            try
            {
                if (Session["UserMaNguoiDung"] == null)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để đặt hàng." });
                }

                string sessionKey = GetCartSessionKey();
                List<CartItem> gioHang = Session[sessionKey] as List<CartItem>;
                if (gioHang == null || !gioHang.Any())
                {
                    return Json(new { success = false, message = "Giỏ hàng trống. Vui lòng thêm sản phẩm." });
                }

                string maDonHang = "DH" + (db.DonHangs.Count() + 1).ToString("D3");
                while (db.DonHangs.Any(d => d.MaDonHang == maDonHang))
                {
                    maDonHang = "DH" + (int.Parse(maDonHang.Substring(2)) + 1).ToString("D3");
                }

                var donHang = new DonHang
                {
                    MaDonHang = maDonHang,
                    MaNguoiDung = Session["UserMaNguoiDung"].ToString(),
                    TongTien = gioHang.Sum(x => x.Gia * x.SoLuong),
                    TrangThaiDonHang = "DangXuLy",
                    DiaChiGiao = diaChiGiao,
                    NgayTao = DateTime.Now
                };

                db.DonHangs.Add(donHang);

                foreach (var item in gioHang)
                {
                    var sach = db.Saches.FirstOrDefault(s => s.MaSach == item.MaSach);
                    if (sach == null || sach.SoLuongTon < item.SoLuong)
                    {
                        return Json(new { success = false, message = $"Sản phẩm {item.TenSach} không đủ hàng." });
                    }

                    sach.SoLuongTon -= item.SoLuong;
                    sach.SoLuongDaBan += item.SoLuong;

                    var chiTietDonHang = new ChiTietDonHang
                    {
                        MaDonHang = donHang.MaDonHang,
                        MaSach = item.MaSach,
                        SoLuong = item.SoLuong,
                        GiaBan = item.Gia
                    };
                    db.ChiTietDonHangs.Add(chiTietDonHang);
                }

                db.SaveChanges();
                Session[sessionKey] = null;
                return Json(new { success = true, message = "Đặt hàng thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã có lỗi xảy ra: " + ex.Message });
            }
        }

        // Action hiển thị thông tin người dùng
        public ActionResult User()
        {
            if (Session["UserMaNguoiDung"] == null)
            {
                return RedirectToAction("DangNhap", "Home");
            }

            // Lấy giá trị MaNguoiDung từ Session trước khi truy vấn
            string maNguoiDung = Session["UserMaNguoiDung"].ToString();
            var nguoiDung = db.NguoiDungs.FirstOrDefault(n => n.MaNguoiDung == maNguoiDung);
            if (nguoiDung == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách đơn hàng dựa trên MaNguoiDung
            var donHangs = db.DonHangs
                .Where(d => d.MaNguoiDung == maNguoiDung) // Sử dụng biến maNguoiDung đã lấy trước
                .OrderByDescending(d => d.NgayTao)
                .ToList();

            var viewModel = new UserViewModel
            {
                MaNguoiDung = nguoiDung.MaNguoiDung,
                HoTen = nguoiDung.HoTen,
                Email = nguoiDung.Email,
                SoDienThoai = nguoiDung.SoDienThoai,
                DiaChi = nguoiDung.DiaChi,
                DonHangs = donHangs.Select(d => new DonHangInfo
                {
                    MaDonHang = d.MaDonHang,
                    NgayTao = (DateTime)d.NgayTao,
                    TrangThaiDonHang = d.TrangThaiDonHang,
                    TongTien = d.TongTien,
                    ChiTiets = db.ChiTietDonHangs
                        .Where(ct => ct.MaDonHang == d.MaDonHang)
                        .Select(ct => new ChiTietDonHangInfo
                        {
                            MaSach = ct.MaSach,
                            TenSach = ct.Sach.TenSach,
                            URLAnhBia = ct.Sach.URLAnhBia,
                            GiaBan = ct.GiaBan,
                            SoLuong = ct.SoLuong
                        }).ToList()
                }).ToList()
            };

            // Chỉ định rõ đường dẫn đến view trong Views/Home/ (vì bạn đã đặt file ở đây)
            return View("~/Views/Home/User.cshtml", viewModel);
        }

        // Action cập nhật thông tin người dùng
        [HttpPost]
        public JsonResult CapNhatThongTin(string hoTen, string email, string soDienThoai, string diaChi)
        {
            try
            {
                if (Session["UserMaNguoiDung"] == null)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để cập nhật thông tin." });
                }

                string maNguoiDung = Session["UserMaNguoiDung"].ToString();
                var nguoiDung = db.NguoiDungs.FirstOrDefault(n => n.MaNguoiDung == maNguoiDung);
                if (nguoiDung == null)
                {
                    return Json(new { success = false, message = "Người dùng không tồn tại." });
                }

                // Kiểm tra email trùng lặp
                if (db.NguoiDungs.Any(n => n.Email == email && n.MaNguoiDung != maNguoiDung))
                {
                    return Json(new { success = false, message = "Email đã được sử dụng." });
                }

                nguoiDung.HoTen = hoTen;
                nguoiDung.Email = email;
                nguoiDung.SoDienThoai = soDienThoai;
                nguoiDung.DiaChi = diaChi;
                db.SaveChanges();

                // Cập nhật Session
                Session["UserName"] = hoTen;

                return Json(new { success = true, message = "Cập nhật thông tin thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã có lỗi xảy ra: " + ex.Message });
            }
        }

        // Action hủy đơn hàng
        
        [HttpPost]
        public JsonResult HuyDonHang(string maDonHang)
        {
            try
            {
                if (Session["UserMaNguoiDung"] == null)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để hủy đơn hàng." });
                }

                // Lấy giá trị MaNguoiDung từ Session trước khi truy vấn
                string maNguoiDung = Session["UserMaNguoiDung"].ToString();

                // Sử dụng biến maNguoiDung trong truy vấn LINQ
                var donHang = db.DonHangs.FirstOrDefault(d => d.MaDonHang == maDonHang && d.MaNguoiDung == maNguoiDung);
                if (donHang == null)
                {
                    return Json(new { success = false, message = "Đơn hàng không tồn tại hoặc không thuộc về bạn." });
                }

                if (donHang.TrangThaiDonHang != "DangXuLy")
                {
                    return Json(new { success = false, message = "Chỉ có thể hủy đơn hàng đang xử lý." });
                }

                // Cập nhật trạng thái đơn hàng
                donHang.TrangThaiDonHang = "DaHuy";

                // Hoàn lại số lượng tồn kho và giảm số lượng đã bán
                var chiTietDonHangs = db.ChiTietDonHangs.Where(ct => ct.MaDonHang == maDonHang).ToList();
                foreach (var chiTiet in chiTietDonHangs)
                {
                    var sach = db.Saches.FirstOrDefault(s => s.MaSach == chiTiet.MaSach);
                    if (sach != null)
                    {
                        sach.SoLuongTon += chiTiet.SoLuong;
                        sach.SoLuongDaBan -= chiTiet.SoLuong;
                    }
                }

                db.SaveChanges();
                return Json(new { success = true, message = "Hủy đơn hàng thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã có lỗi xảy ra: " + ex.Message });
            }
        }
    }
}