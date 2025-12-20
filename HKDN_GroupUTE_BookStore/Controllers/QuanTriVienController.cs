using HKDN_GroupUTE_BookStore.Models;
using HKDN_GroupUTE_BookStore.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HKDN_GroupUTE_BookStore.Controllers
{
    public class QuanTriVienController : Controller
    {
        private readonly CsharpBookShopContext _shopContext;

        public QuanTriVienController(CsharpBookShopContext shopContext)
        {
            _shopContext = shopContext;
        }

        // GET: QuanTriVien/TrangChu_QuanTriVien
        public IActionResult TrangChu_QuanTriVien()
        {
            // ===== NGƯỜI DÙNG =====
            var soKhachHang = _shopContext.Nguoidungs.Count(x => x.VaiTro == "KhachHang");
            var soQuanTriVien = _shopContext.Nguoidungs.Count(x => x.VaiTro == "Admin");

            // ===== ĐƠN HÀNG =====
            var tongDonHang = _shopContext.Donhangs.Count();
            var daGiao = _shopContext.Donhangs.Count(x => x.TrangThaiDonHang == "DaGiao");
            var daHuy = _shopContext.Donhangs.Count(x => x.TrangThaiDonHang == "DaHuy");
            var dangXuLy = _shopContext.Donhangs.Count(x => x.TrangThaiDonHang == "DangXuLy");

            // ===== SÁCH =====
            var tongSachTonKho = _shopContext.Saches.Sum(x => (int?)x.SoLuongTon) ?? 0;
            var tongSachDaBan = _shopContext.Saches.Sum(x => (int?)x.SoLuongDaBan) ?? 0;
            var tongTheLoai = _shopContext.Theloais.Count();

            // ===== BIỂU ĐỒ THEO NĂM HIỆN TẠI =====
            int namHienTai = DateTime.Now.Year;

            var dataTheoThang = _shopContext.Donhangs
                .Where(x => x.NgayTao.HasValue && x.NgayTao.Value.Year == namHienTai)
                .GroupBy(x => new
                {
                    Thang = x.NgayTao.Value.Month,
                    x.TrangThaiDonHang
                })
                .Select(g => new
                {
                    Thang = g.Key.Thang,
                    TrangThai = g.Key.TrangThaiDonHang,
                    SoLuong = g.Count()
                })
                .ToList();

            var thang = Enumerable.Range(1, 12)
                .Select(t => $"Tháng {t}")
                .ToList();

            var donDangXuLy = new List<int>();
            var donDaGiao = new List<int>();
            var donDaHuy = new List<int>();

            for (int i = 1; i <= 12; i++)
            {
                donDangXuLy.Add(dataTheoThang
                    .Where(x => x.Thang == i && x.TrangThai == "DangXuLy")
                    .Sum(x => x.SoLuong));

                donDaGiao.Add(dataTheoThang
                    .Where(x => x.Thang == i && x.TrangThai == "DaGiao")
                    .Sum(x => x.SoLuong));

                donDaHuy.Add(dataTheoThang
                    .Where(x => x.Thang == i && x.TrangThai == "DaHuy")
                    .Sum(x => x.SoLuong));
            }

            var model = new QTV_TrangChu_LoadDuLieu_VM
            {
                // Người dùng
                TongSoNguoiDung = soKhachHang + soQuanTriVien,
                TongSoKhachHang = soKhachHang,
                TongSoQuanTriVien = soQuanTriVien,

                // Đơn hàng
                TongDonHang = tongDonHang,
                DaHoanThanh = daGiao, // CHỈ ĐÃ GIAO
                ChuaHoanThanh = dangXuLy,
                DaGiao = daGiao,
                DaHuy = daHuy,

                // Sách
                TongSach = tongSachTonKho + tongSachDaBan,
                TongSachTonKho = tongSachTonKho,
                TongSachDaBan = tongSachDaBan,
                TongTheLoai = tongTheLoai,

                // Chart
                Thang = thang,
                DonDangXuLy = donDangXuLy,
                DonDaGiao = donDaGiao,
                DonDaHuy = donDaHuy
            };

            return View(model);
        }



        // GET: QuanTriVien/QuanLySanPham_QuanTriVien
        public IActionResult QuanLySanPham_QuanTriVien(string searchString, string categoryId)
        {
            ViewBag.TheLoais = new SelectList(
                _shopContext.Theloais,
                "MaTheLoai",
                "TenTheLoai",
                categoryId
            );

            var query = _shopContext.Saches.Include(s => s.MaTheLoaiNavigation).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                query = query.Where(s => s.TenSach.ToLower().Contains(searchString)
                                      || s.TacGia.ToLower().Contains(searchString));
            }

            if (!string.IsNullOrEmpty(categoryId))
            {
                query = query.Where(s => s.MaTheLoai == categoryId);
            }

            var books = query.Select(s => new QTV_QuanLySanPham_LoadDuLieu_VM
            {
                MaSach = s.MaSach,
                AnhBia = string.IsNullOrEmpty(s.UrlanhBia) ? "sach_default.jpg" : s.UrlanhBia,
                TenSach = s.TenSach,
                TheLoai = s.MaTheLoaiNavigation != null ? s.MaTheLoaiNavigation.TenTheLoai : "Không xác định",
                MaTheLoai = s.MaTheLoai,
                TacGia = s.TacGia,
                Gia = s.Gia,
                DaBan = s.SoLuongDaBan ?? 0,
                TonKho = s.SoLuongTon ?? 0
            }).ToList();

            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentCategory = categoryId;
            ViewBag.Books = books;

            return View();
        }

    }
}
