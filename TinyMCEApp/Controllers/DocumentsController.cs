using Microsoft.AspNetCore.Mvc;
using TinyMCEApp.Context;

namespace TinyMCEApp.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly MyContext _context;
        public DocumentsController(MyContext myContext)
        {
            _context = myContext;
        }
        public IActionResult Index()
        {
            var documents = _context.Documents.ToList();
            return View(documents);
        }
    }
}
