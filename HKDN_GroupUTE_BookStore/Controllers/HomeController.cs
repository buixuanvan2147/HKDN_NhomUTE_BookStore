using System.Diagnostics;
using HKDN_GroupUTE_BookStore.ViewModel;
using HKDN_GroupUTE_BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace HKDN_GroupUTE_BookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CsharpBookShopContext _shopContext;
        
        public HomeController(ILogger<HomeController> logger, CsharpBookShopContext shopContext)
        {
            _logger = logger;
            _shopContext = shopContext;
        }

        public IActionResult Index_Home()
        {
            var allBooks = _shopContext.Saches.ToList();
            var hotBooks = _shopContext.Saches.Where(s => s.SoLuongTon > 50).ToList();
            var trendingBooks = _shopContext.Saches.OrderByDescending(s => s.NgayTao).Take(10).ToList();

            var viewModel = new Index_Home_ListSach
            {
                TatCaSach = allBooks,
                SachHot = hotBooks,
                SachXuHuong = trendingBooks
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
