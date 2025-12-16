using HKDN_GroupUTE_BookStore.Models;
using HKDN_GroupUTE_BookStore.ViewModel; // Nếu dùng ViewModel QTV_QLSP_TheLoai_List
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HKDN_GroupUTE_BookStore.Controllers
{
    public class QTV_QLSP_TheLoaiController : Controller
    {
        private readonly CsharpBookShopContext _shopContext;

        public QTV_QLSP_TheLoaiController(CsharpBookShopContext shopContext)
        {
            _shopContext = shopContext;
        }

        // GET: QTV_QLSP_TheLoai/Index
        public async Task<IActionResult> Index()
        {
            var theloaiList = await _shopContext.Theloais
                .AsNoTracking()
                .Select(tl => new QTV_QLSP_TheLoai_List
                {
                    MaTheLoai = tl.MaTheLoai,
                    TenTheLoai = tl.TenTheLoai,
                    SoLoaiSach = tl.Saches.Count(), // Số đầu sách thuộc thể loại này
                    TongSachTheoTheLoai = tl.Saches.Sum(s => s.SoLuongTon ?? 0) // Tổng tồn kho
                })
                .ToListAsync();

            return View(theloaiList);
        }

        // GET: QTV_QLSP_TheLoai/Details/TL001
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var theLoai = await _shopContext.Theloais
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.MaTheLoai == id);

            if (theLoai == null)
                return NotFound();

            return View(theLoai);
        }

        // GET: QTV_QLSP_TheLoai/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: QTV_QLSP_TheLoai/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Theloai theLoai)
        {
            if (ModelState.IsValid)
            {
                _shopContext.Theloais.Add(theLoai);
                await _shopContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(theLoai);
        }

        // GET: QTV_QLSP_TheLoai/Edit/TL001
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var theLoai = await _shopContext.Theloais.FindAsync(id);
            if (theLoai == null)
                return NotFound();

            return View(theLoai);
        }

        // POST: QTV_QLSP_TheLoai/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Theloai theLoai)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _shopContext.Update(theLoai);
                    await _shopContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _shopContext.Theloais.AnyAsync(t => t.MaTheLoai == theLoai.MaTheLoai))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(theLoai);
        }

        // GET: QTV_QLSP_TheLoai/Delete/TL001
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var theLoai = await _shopContext.Theloais
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.MaTheLoai == id);

            if (theLoai == null)
                return NotFound();

            return View(theLoai);
        }

        // POST: QTV_QLSP_TheLoai/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var theLoai = await _shopContext.Theloais.FindAsync(id);
            if (theLoai != null)
            {
                _shopContext.Theloais.Remove(theLoai);
                await _shopContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}