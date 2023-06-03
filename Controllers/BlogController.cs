using Microsoft.AspNetCore.Mvc;
using PagedList.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Business.Data;
using Domain;

namespace Supermarket.Controllers
{
    public class BlogController : Controller
    {
        private readonly DbMarketsContext _context;
        public BlogController(DbMarketsContext context)
        {
            _context = context;
        }
        // GET: /<controller>/
        [Route("blogs.html", Name = ("Blog"))]
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;
            var lsTinDangs = _context.TinDangs
                .AsNoTracking()
                .OrderBy(x => x.PostId);
            PagedList<TinDang> models = new PagedList<TinDang>((IQueryable<TinDang>)lsTinDangs, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }
        [Route("/news/{Alias}-{id}.html", Name = "TinChiTiet")]
        public IActionResult Details(int id)
        {
            var tindang = _context.TinDangs.AsNoTracking().SingleOrDefault(x => x.PostId == id);
            if (tindang == null)
            {
                return RedirectToAction("Index");
            }
            var lsBaivietlienquan = _context.TinDangs
                .AsNoTracking()
                .Where(x => x.Published == true && x.PostId != id)
                .Take(3)
                .OrderByDescending(x => x.CreatedDate).ToList();
            ViewBag.Baivietlienquan = lsBaivietlienquan;
            return View(tindang);
        }
    }
}
