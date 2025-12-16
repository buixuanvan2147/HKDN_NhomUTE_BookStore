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
        public async Task<IActionResult> Index()
        {
            var donHangs = _shopContext.Donhangs
                .Include(d => d.MaNguoiDungNavigation) // Tương đương Include("NguoiDung")
                .AsNoTracking();

            var viewModel = new QTV_DoanhThuViewModel
            {
                SoLuongDonHangDangXuLy = await donHangs.CountAsync(d => d.TrangThaiDonHang == "DangXuLy"),
                TongTienDangXuLy = await donHangs.Where(d => d.TrangThaiDonHang == "DangXuLy").SumAsync(d => d.TongTien),
                SoLuongDonHangDaGiao = await donHangs.CountAsync(d => d.TrangThaiDonHang == "DaGiao"),
                TongTienDaGiao = await donHangs.Where(d => d.TrangThaiDonHang == "DaGiao").SumAsync(d => d.TongTien),
                SoLuongDonHangDaHuy = await donHangs.CountAsync(d => d.TrangThaiDonHang == "DaHuy"),
                TongTienDaHuy = await donHangs.Where(d => d.TrangThaiDonHang == "DaHuy").SumAsync(d => d.TongTien)
            };

            ViewBag.DoanhThuThongKe = viewModel;

            var listDonHang = await donHangs.ToListAsync();
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

        // GET: QTV_QLDH/Create
        public IActionResult Create()
        {
            ViewBag.MaNguoiDung = new SelectList(_shopContext.Nguoidungs, "MaNguoiDung", "HoTen");
            return View();
        }

        // POST: QTV_QLDH/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Donhang donHang)
        {
            if (ModelState.IsValid)
            {
                _shopContext.Donhangs.Add(donHang);
                await _shopContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaNguoiDung = new SelectList(_shopContext.Nguoidungs, "MaNguoiDung", "HoTen", donHang.MaNguoiDung);
            return View(donHang);
        }

        // GET: QTV_QLDH/Edit/DH001
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var donHang = await _shopContext.Donhangs.FindAsync(id);
            if (donHang == null)
                return NotFound();

            ViewBag.MaNguoiDung = new SelectList(_shopContext.Nguoidungs, "MaNguoiDung", "HoTen", donHang.MaNguoiDung);
            return View(donHang);
        }

        // POST: QTV_QLDH/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Donhang donHang)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _shopContext.Update(donHang);
                    await _shopContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _shopContext.Donhangs.AnyAsync(d => d.MaDonHang == donHang.MaDonHang))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaNguoiDung = new SelectList(_shopContext.Nguoidungs, "MaNguoiDung", "HoTen", donHang.MaNguoiDung);
            return View(donHang);
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