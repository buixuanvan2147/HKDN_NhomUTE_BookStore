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
            // ====== THỐNG KÊ CHUNG ======
            var soKhachHang = _shopContext.Nguoidungs.Count(x => x.VaiTro == "KhachHang");
            var soQuanTriVien = _shopContext.Nguoidungs.Count(x => x.VaiTro == "Admin");

            var tongDonHang = _shopContext.Donhangs.Count();
            var daGiao = _shopContext.Donhangs.Count(x => x.TrangThaiDonHang == "DaGiao");
            var daHuy = _shopContext.Donhangs.Count(x => x.TrangThaiDonHang == "DaHuy");
            var chuaHoanThanh = _shopContext.Donhangs.Count(x => x.TrangThaiDonHang == "DangXuLy");

            var sachTonKho = _shopContext.Saches.Sum(x => (int?)x.SoLuongTon) ?? 0;
            var sachDaBan = _shopContext.Saches.Sum(x => (int?)x.SoLuongDaBan) ?? 0;
            var tongTheLoai = _shopContext.Theloais.Count();

            // ====== BIỂU ĐỒ THEO THÁNG (DÙNG NgayTao) ======
            var dataTheoThang = _shopContext.Donhangs
                .GroupBy(x => new
                {
                    Thang = x.NgayTao.Value.Month,
                    Nam = x.NgayTao.Value.Year,
                    x.TrangThaiDonHang
                })
                .Select(g => new
                {
                    Thang = g.Key.Thang,
                    Nam = g.Key.Nam,
                    TrangThai = g.Key.TrangThaiDonHang,
                    SoLuong = g.Count()
                })
                .ToList();

            var thang = Enumerable.Range(1, 12).Select(t => "Tháng " + t).ToList();

            var dangXuLy = new List<int>();
            var daGiaoThang = new List<int>();
            var daHuyThang = new List<int>();

            for (int i = 1; i <= 12; i++)
            {
                dangXuLy.Add(dataTheoThang
                    .Where(x => x.Thang == i && x.TrangThai == "DangXuLy")
                    .Sum(x => x.SoLuong));

                daGiaoThang.Add(dataTheoThang
                    .Where(x => x.Thang == i && x.TrangThai == "DaGiao")
                    .Sum(x => x.SoLuong));

                daHuyThang.Add(dataTheoThang
                    .Where(x => x.Thang == i && x.TrangThai == "DaHuy")
                    .Sum(x => x.SoLuong));
            }

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
                TongTheLoai = tongTheLoai,

                // Chart
                Thang = thang,
                DonDangXuLy = dangXuLy,
                DonDaGiao = daGiaoThang,
                DonDaHuy = daHuyThang
            };

            return View(model);
        }


        // GET: QuanTriVien/QuanLySanPham_QuanTriVien
        public IActionResult QuanLySanPham_QuanTriVien()
        {
            ViewBag.TheLoais = new SelectList(
                _shopContext.Theloais,
                "MaTheLoai",
                "TenTheLoai"
            );

            var books = _shopContext.Saches
                .Include(s => s.MaTheLoaiNavigation)
                .Select(s => new QTV_QuanLySanPham_LoadDuLieu_VM
                {
                    MaSach = s.MaSach,
                    AnhBia = string.IsNullOrEmpty(s.UrlanhBia)
                        ? "sach_default.jpg"
                        : s.UrlanhBia,
                    TenSach = s.TenSach,
                    TheLoai = s.MaTheLoaiNavigation != null
                        ? s.MaTheLoaiNavigation.TenTheLoai
                        : "Không xác định",
                    MaTheLoai = s.MaTheLoai,
                    TacGia = s.TacGia,
                    Gia = s.Gia,
                    DaBan = s.SoLuongDaBan ?? 0,
                    TonKho = s.SoLuongTon ?? 0
                })
                .ToList();

            ViewBag.Books = books;
            return View();
        }

    }
}
