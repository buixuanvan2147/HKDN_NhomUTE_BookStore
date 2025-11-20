using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using BCCK_CSharp_BookShop.Models;
using BCCK_CSharp_BookShop.ViewModel;

namespace BCCK_CSharp_BookShop.Controllers
{
    public class QuanTriVienController : Controller
    {
        private readonly CSharp_BookShopEntities db = new CSharp_BookShopEntities();

        // GET: QuanTriVien/TrangChu_QuanTriVien
        public ActionResult TrangChu_QuanTriVien()
        {
            // Tính toán số lượng khách hàng và quản trị viên
            var soKhachHang = db.NguoiDungs.Count(nd => nd.VaiTro == "KhachHang");
            var soQuanTriVien = db.NguoiDungs.Count(nd => nd.VaiTro == "Admin");

            // Tính tổng số đơn hàng và các trạng thái của đơn hàng
            var tongDonHang = db.DonHangs.Count();
            var daGiao = db.DonHangs.Count(dh => dh.TrangThaiDonHang == "DaGiao");
            var daHuy = db.DonHangs.Count(dh => dh.TrangThaiDonHang == "DaHuy");
            var chuaHoanThanh = db.DonHangs.Count(dh => dh.TrangThaiDonHang == "DangXuLy");

            // Tính tổng sách tồn kho và đã bán, với giá trị null được xử lý về 0
            var sachTonKho = db.Saches.Sum(x => (int?)x.SoLuongTon) ?? 0;
            var sachDaBan = db.Saches.Sum(x => (int?)x.SoLuongDaBan) ?? 0;
            var tongtheloai = db.TheLoais.Count();
            // Tạo đối tượng ViewModel để truyền lên View
            var model = new QTV_TrangChu_LoadDuLieu_VM
            {
                TongSoNguoiDung = soKhachHang + soQuanTriVien,
                TongSoKhachHang = soKhachHang,
                TongSoQuanTriVien = soQuanTriVien,
                TongDonHang = tongDonHang,
                DaHoanThanh = daGiao + daHuy,
                ChuaHoanThanh = chuaHoanThanh,
                DaGiao = daGiao,
                DaHuy = daHuy,
                TongSach = sachDaBan + sachTonKho,
                TongSachTonKho = sachTonKho,
                TongSachDaBan = sachDaBan,
                TongTheLoai = tongtheloai,
            }; 

            return View(model);
        }

        // GET: QuanTriVien/QuanLySanPham_QuanTriVien
        public ActionResult QuanLySanPham_QuanTriVien()
        {
            try
            {
                ViewBag.TheLoais = new SelectList(db.TheLoais, "MaTheLoai", "TenTheLoai");

                var books = db.Saches
                .Include("TheLoai")
                .Select(s => new QTV_QuanLySanPham_LoadDuLieu_VM
                {
                    MaSach = s.MaSach,
                    AnhBia = string.IsNullOrEmpty(s.URLAnhBia) ? "sach_default.jpg" : s.URLAnhBia,
                    TenSach = s.TenSach,
                    TheLoai = s.TheLoai != null ? s.TheLoai.TenTheLoai : "Không xác định",
                    MaTheLoai = s.MaTheLoai,
                    TacGia = s.TacGia,
                    Gia = s.Gia,
                    DaBan = s.SoLuongDaBan ?? 0,
                    TonKho = s.SoLuongTon ?? 0
                }).ToList();

                // Lưu danh sách sách vào ViewBag
                ViewBag.Books = books;

                return View();
            }
            catch (Exception ex)
            {
                // Ghi log lỗi (nếu cần)
                System.Diagnostics.Debug.WriteLine($"Lỗi trong QuanLySanPham_QuanTriVien: {ex.Message}");
                // Trả về view với thông báo lỗi
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải dữ liệu. Vui lòng thử lại.";
                return View(new QTV_QuanLySanPham_LoadDuLieu_VM());
            }
        }

        public ActionResult QuanLyNguoiDung_QuanTriVien()
        {
            return View();
        }

        public ActionResult QuanLyDonHang_QuanTriVien()
        {
            return View();
        }

        public ActionResult ThongKe_QuanTriVien()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}