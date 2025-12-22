using HKDN_GroupUTE_BookStore.Models;
using HKDN_GroupUTE_BookStore.ViewModel; // Nếu có ViewModel QTV_DoanhThuViewModel
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HKDN_GroupUTE_BookStore.Controllers
{
    public class QTV_QLDHController : Controller
    {
        private readonly CsharpBookShopContext _shopContext;

        public QTV_QLDHController(CsharpBookShopContext shopContext)
        {
            _shopContext = shopContext;
        }

        // GET: QTV_QLDH/Index
        public async Task<IActionResult> Index(string searchString, string statusFilter)
        {
            var query = _shopContext.Donhangs
                .Include(d => d.MaNguoiDungNavigation)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                query = query.Where(d =>
                    d.MaDonHang.ToLower().Contains(searchString) ||
                    d.MaNguoiDungNavigation.HoTen.ToLower().Contains(searchString));
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(d => d.TrangThaiDonHang == statusFilter);
            }

            var viewModel = new QTV_DoanhThuViewModel
            {
                // ===== CHỜ XÁC NHẬN =====
                SoLuongChoXacNhan = await query
                    .CountAsync(d => d.TrangThaiDonHang == "ChoXacNhan"),

                TongTienChoXacNhan = await query
                    .Where(d => d.TrangThaiDonHang == "ChoXacNhan")
                    .SumAsync(d => (decimal?)d.TongTien) ?? 0,

                // ===== ĐANG XỬ LÝ =====
                SoLuongDangXuLy = await query
                    .CountAsync(d => d.TrangThaiDonHang == "DangXuLy"),

                TongTienDangXuLy = await query
                    .Where(d => d.TrangThaiDonHang == "DangXuLy")
                    .SumAsync(d => (decimal?)d.TongTien) ?? 0,

                // ===== ĐANG GIAO =====
                SoLuongDangGiao = await query
                    .CountAsync(d => d.TrangThaiDonHang == "DangGiao"),

                TongTienDangGiao = await query
                    .Where(d => d.TrangThaiDonHang == "DangGiao")
                    .SumAsync(d => (decimal?)d.TongTien) ?? 0,

                // ===== ĐÃ GIAO =====
                SoLuongDaGiao = await query
                    .CountAsync(d => d.TrangThaiDonHang == "DaGiao"),

                TongTienDaGiao = await query
                    .Where(d => d.TrangThaiDonHang == "DaGiao")
                    .SumAsync(d => (decimal?)d.TongTien) ?? 0,

                // ===== ĐÃ HỦY =====
                SoLuongDaHuy = await query
                    .CountAsync(d => d.TrangThaiDonHang == "DaHuy"),

                TongTienDaHuy = await query
                    .Where(d => d.TrangThaiDonHang == "DaHuy")
                    .SumAsync(d => (decimal?)d.TongTien) ?? 0
            };

            ViewBag.DoanhThuThongKe = viewModel;
            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentStatus = statusFilter;

            var listDonHang = await query
                .OrderBy(d =>
                    d.TrangThaiDonHang == "ChoXacNhan" ? 1 :
                    d.TrangThaiDonHang == "DangXuLy" ? 2 :
                    d.TrangThaiDonHang == "DangGiao" ? 3 :
                    d.TrangThaiDonHang == "DaGiao" ? 4 :
                    d.TrangThaiDonHang == "DaHuy" ? 5 : 6)
                .ThenByDescending(d => d.NgayTao)
                .AsNoTracking()
                .ToListAsync();

            return View(listDonHang);
        }


        // GET: QTV_QLDH/Details/DH001
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var donHang = await _shopContext.Donhangs
                .Include(d => d.MaNguoiDungNavigation)
                .Include(d => d.Chitietdonhangs)
                    .ThenInclude(ct => ct.MaSachNavigation) // Tải thông tin sách trong chi tiết đơn hàng
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.MaDonHang == id);

            if (donHang == null)
                return NotFound();

            return View(donHang);
        }


        [HttpPost]
        [HttpPost]
        public JsonResult CapNhatTrangThaiAjax(string id, string trangThaiMoi)
        {
            var donHang = _shopContext.Donhangs.Find(id);

            if (donHang == null)
                return Json(new { success = false, msg = "Không tìm thấy đơn hàng!" });

            // Có thể chặn nghiệp vụ tại đây nếu muốn
            donHang.TrangThaiDonHang = trangThaiMoi;
            _shopContext.SaveChanges();

            return Json(new { success = true, msg = "Cập nhật trạng thái thành công!" });
        }


        // GET: QTV_QLDH/Delete/DH001
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var donHang = await _shopContext.Donhangs
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.MaDonHang == id);

            if (donHang == null)
                return NotFound();

            return View(donHang);
        }

        // POST: QTV_QLDH/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var donHang = await _shopContext.Donhangs.FindAsync(id);
            if (donHang != null)
            {
                _shopContext.Donhangs.Remove(donHang);
                await _shopContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}