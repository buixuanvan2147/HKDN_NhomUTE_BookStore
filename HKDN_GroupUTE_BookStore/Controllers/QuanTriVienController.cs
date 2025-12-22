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
            // ===== 1. NGƯỜI DÙNG =====
            var soKhachHang = _shopContext.Nguoidungs.Count(x => x.VaiTro == "KhachHang");
            var soQuanTriVien = _shopContext.Nguoidungs.Count(x => x.VaiTro == "Admin");

            // ===== 2. ĐƠN HÀNG (ĐẾM SỐ LƯỢNG HIỆN TẠI) =====
            var tongDonHang = _shopContext.Donhangs.Count();

            // Đếm đủ 5 trạng thái
            var slChoXacNhan = _shopContext.Donhangs.Count(x => x.TrangThaiDonHang == "ChoXacNhan");
            var slDangXuLy = _shopContext.Donhangs.Count(x => x.TrangThaiDonHang == "DangXuLy");
            var slDangGiao = _shopContext.Donhangs.Count(x => x.TrangThaiDonHang == "DangGiao");
            var slDaGiao = _shopContext.Donhangs.Count(x => x.TrangThaiDonHang == "DaGiao");
            var slDaHuy = _shopContext.Donhangs.Count(x => x.TrangThaiDonHang == "DaHuy");

            // ===== 3. SÁCH =====
            var tongSachTonKho = _shopContext.Saches.Sum(x => (int?)x.SoLuongTon) ?? 0;
            var tongSachDaBan = _shopContext.Saches.Sum(x => (int?)x.SoLuongDaBan) ?? 0;
            var tongTheLoai = _shopContext.Theloais.Count();

            // ===== 4. XỬ LÝ DỮ LIỆU BIỂU ĐỒ (12 THÁNG) =====
            int namHienTai = DateTime.Now.Year;

            // Lấy dữ liệu thô (Raw Data)
            var dataTheoThang = _shopContext.Donhangs
                .Where(x => x.NgayTao.HasValue && x.NgayTao.Value.Year == namHienTai)
                .Select(x => new
                {
                    Thang = x.NgayTao.Value.Month,
                    TrangThai = x.TrangThaiDonHang,
                    TongTien = x.TongTien
                })
                .ToList();

            var thang = new List<string>();

            // 5 List cho 5 đường biểu đồ
            var donChoXacNhan = new List<int>();
            var donDangXuLy = new List<int>();
            var donDangGiao = new List<int>();
            var donDaGiao = new List<int>();
            var donDaHuy = new List<int>();

            var doanhThuTheoThang = new List<decimal>();

            for (int i = 1; i <= 12; i++)
            {
                thang.Add($"Tháng {i}");

                // Lọc data của tháng i
                var dataThang = dataTheoThang.Where(x => x.Thang == i).ToList();

                donChoXacNhan.Add(dataThang.Count(x => x.TrangThai == "ChoXacNhan"));
                donDangXuLy.Add(dataThang.Count(x => x.TrangThai == "DangXuLy"));
                donDangGiao.Add(dataThang.Count(x => x.TrangThai == "DangGiao"));
                donDaGiao.Add(dataThang.Count(x => x.TrangThai == "DaGiao"));
                donDaHuy.Add(dataThang.Count(x => x.TrangThai == "DaHuy"));

                // Doanh thu (chỉ tính đơn đã giao)
                doanhThuTheoThang.Add(dataThang
                    .Where(x => x.TrangThai == "DaGiao")
                    .Sum(x => x.TongTien));
            }

            var model = new QTV_TrangChu_LoadDuLieu_VM
            {
                // User
                TongSoNguoiDung = soKhachHang + soQuanTriVien,
                TongSoKhachHang = soKhachHang,
                TongSoQuanTriVien = soQuanTriVien,

                // Đơn hàng tổng quan
                TongDonHang = tongDonHang,
                SlChoXacNhan = slChoXacNhan,
                SlDangXuLy = slDangXuLy,
                SlDangGiao = slDangGiao,
                SlDaGiao = slDaGiao,
                SlDaHuy = slDaHuy,

                // Sách
                TongSach = tongSachTonKho + tongSachDaBan,
                TongSachTonKho = tongSachTonKho,
                TongSachDaBan = tongSachDaBan,
                TongTheLoai = tongTheLoai,

                // Chart Data
                Thang = thang,
                DonChoXacNhan = donChoXacNhan,
                DonDangXuLy = donDangXuLy,
                DonDangGiao = donDangGiao,
                DonDaGiao = donDaGiao,
                DonDaHuy = donDaHuy,
                DoanhThuTheoThang = doanhThuTheoThang
            };

            return View(model);
        }

        // GET: QuanTriVien/QuanLySanPham_QuanTriVien
        public IActionResult QuanLySanPham_QuanTriVien(string searchString, string categoryId)
        {
            var categoriesWithBooks = _shopContext.Theloais
                    .Where(tl => _shopContext.Saches.Any(s => s.MaTheLoai == tl.MaTheLoai))
                    .OrderBy(tl => tl.TenTheLoai)
                    .ToList();

            ViewBag.TheLoais = new SelectList(
                categoriesWithBooks,
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
