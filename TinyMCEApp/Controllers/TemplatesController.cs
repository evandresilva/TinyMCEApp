using Microsoft.AspNetCore.Mvc;
using TinyMCEApp.Context;

namespace TinyMCEApp.Controllers
{
    public class TemplatesController : Controller
    {
        private readonly MyContext _context;
        public TemplatesController(MyContext myContext)
        {
            _context = myContext;
        }
        public IActionResult Index()
        {
            var Templates = _context.Templates.ToList();
            return View(Templates);
        }
    }
}
