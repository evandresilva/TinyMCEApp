using Microsoft.AspNetCore.Mvc;
using TinyMCEApp.Models;
using TinyMCEApp.Context;
using Newtonsoft.Json;
using System.Text;
using Rocket.PdfGenerator;
using System.IO;
using System.Xml.Linq;

namespace TinyMCEApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly MyContext _myContext;
        //to improve
        public HomeController(MyContext myContext, IConfiguration configuration) : base(configuration)
        {
            _myContext = myContext;
        }

        public IActionResult Index(int? documentId = null, int? templateId = null)
        {
            var templates = _myContext.Templates.ToList().Select(x => new TemplateViewModel
            {
                content = GetContent(x.Path, "templates"),
                description = x.Name,
                title = x.Name
            }).ToList();
            ViewBag.Templates = JsonConvert.SerializeObject(templates);
            if (documentId.HasValue && documentId != 0)
            {
                var model = _myContext.Documents.FirstOrDefault(x => x.Id == documentId);
                return View(new DocumentViewModel
                {
                    content = model != null ? GetContent(model.Path, "documents") : "",
                    description = model?.Name ?? "",
                    title = model?.Name ?? ""
                });
            }
            if (templateId.HasValue && templateId != 0)
            {
                var model = _myContext.Templates.FirstOrDefault(x => x.Id == templateId);
                return View(new DocumentViewModel
                {
                    content = model != null ? GetContent(model.Path, "templates") : "",
                    description = model?.Name ?? "",
                    title = model?.Name ?? ""
                });
            }
            return View();
        }
        [HttpPost]
        public IActionResult Save([FromForm] string content, string name)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(content);
            string path = UploadFileToLakeFS(bytes, ".html", "documents");
            //var folder = "drafts";

            //var fileName = $"{Guid.NewGuid()}.html";
            //var filePath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Storage", folder);
            //if (!Directory.Exists(filePath))
            //    Directory.CreateDirectory(filePath);
            //System.IO.File.Create(Path.Combine(filePath, fileName)).Close();
            //System.IO.File.WriteAllText(Path.Combine(filePath, fileName), content);
            _myContext.Documents.Add(new DocumentModel
            {
                Name = name,
                Path = path
            });
            _myContext.SaveChanges();
            return Ok();
        }
        [HttpPost]
        public IActionResult SaveAsTemplate([FromForm] string content, [FromForm] string name)
        {
            //var folder = "templates";
            byte[] bytes = Encoding.ASCII.GetBytes(content);
            var path = UploadFileToLakeFS(bytes, ".html", "templates");
            //UploadFileToLakeFS(bytes, "html", "templates");
            //var fileName = $"{Guid.NewGuid()}.html";
            //var filePath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Storage", folder);
            //if (!Directory.Exists(filePath))
            //    Directory.CreateDirectory(filePath);
            //System.IO.File.Create(Path.Combine(filePath, fileName)).Close();
            //System.IO.File.WriteAllText(Path.Combine(filePath, fileName), content);

            _myContext.Templates.Add(new TemplateModel
            {
                Name = name,
                Path = path
            });
            _myContext.SaveChanges();
            return Ok();
        }
        [HttpPost]
        public IActionResult ExportToPDF([FromForm] string html)
        {
            //Initialize HTML to PDF converter
            //HtmlToPdfConverter generator = new HtmlToPdfConverter();
            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();

            //Convert HTML to PDF document
            var filePath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Storage", "export", "template.html");
            var template = System.IO.File.ReadAllText(filePath);
            string fullHTML = template.Replace("{body}", html);
            var document = htmlConverter.GeneratePdf(fullHTML, "");

            //Create memory stream
            //using MemoryStream stream = new MemoryStream();

            //Save the document
            //document.Save(stream);

            //return File(stream.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf, "HTML-to-PDF.pdf");
            return Ok(document.ToArray());
        }
        [HttpPost]
        public IActionResult UploadImage(IFormFile file)
        {
            var image = UploadFile(file, "tmp");
            var response = new
            {
                location = image
            };
            return Ok(response);
        }
        [HttpGet]
        public IActionResult GetTemplates()
        {
            var templates = _myContext.Templates.ToList()
                .Select(x => new TemplateViewModel
                {
                    content = GetContent(x.Path, "templates"),
                    description = x.Name,
                    title = x.Name
                });
            return Ok(templates.ToList());

        }
        public IActionResult Privacy()
        {
            return View();
        }

    }
}