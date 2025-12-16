using HKDN_GroupUTE_BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HKDN_GroupUTE_BookStore.Controllers
{
    public class QTV_QLNDController : Controller
    {
        private readonly CsharpBookShopContext _shopContext;

        public QTV_QLNDController(CsharpBookShopContext shopContext)
        {
            _shopContext = shopContext;
        }

        // GET: QTV_QLND/Index
        public async Task<IActionResult> Index()
        {
            var nguoiDungs = await _shopContext.Nguoidungs
                .AsNoTracking()
                .ToListAsync();

            return View(nguoiDungs);
        }

        // GET: QTV_QLND/Details/ND001
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var nguoiDung = await _shopContext.Nguoidungs
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.MaNguoiDung == id);

            if (nguoiDung == null)
                return NotFound();

            return View(nguoiDung);
        }

        // GET: QTV_QLND/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: QTV_QLND/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Nguoidung nguoiDung)
        {
            if (ModelState.IsValid)
            {
                _shopContext.Nguoidungs.Add(nguoiDung);
                await _shopContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(nguoiDung);
        }

        // GET: QTV_QLND/Edit/ND001
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var nguoiDung = await _shopContext.Nguoidungs.FindAsync(id);
            if (nguoiDung == null)
                return NotFound();

            return View(nguoiDung);
        }

        // POST: QTV_QLND/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Nguoidung nguoiDung)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _shopContext.Update(nguoiDung);
                    await _shopContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _shopContext.Nguoidungs.AnyAsync(n => n.MaNguoiDung == nguoiDung.MaNguoiDung))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(nguoiDung);
        }

        // GET: QTV_QLND/Delete/ND001
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var nguoiDung = await _shopContext.Nguoidungs
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.MaNguoiDung == id);

            if (nguoiDung == null)
                return NotFound();

            return View(nguoiDung);
        }

        // POST: QTV_QLND/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var nguoiDung = await _shopContext.Nguoidungs.FindAsync(id);
            if (nguoiDung != null)
            {
                _shopContext.Nguoidungs.Remove(nguoiDung);
                await _shopContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}