using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net;
using TinyMCEApp.Models;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace TinyMCEApp.Controllers
{
    public class BaseController : Controller
    {
        private readonly LakeFsConfigModel _lakeFsConfigModel;
        private readonly IConfiguration _configuration;
        public BaseController(IConfiguration configuration)
        {
            _lakeFsConfigModel = new LakeFsConfigModel(configuration);
            _configuration = configuration;
        }

        [NonAction]
        public string UploadFile(IFormFile file, string folder)
        {
            var fileName = $"{Guid.NewGuid()}.{Path.GetExtension(file.FileName)}";
            var filePathVirtual = "/Storage" + "/" + folder + "/" + fileName;
            var filePath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Storage", folder);

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
            using (
                var stream = System.IO.File.Create(Path.Combine(filePath, fileName)))
            {
                file.CopyTo(stream);
            }
            return filePathVirtual;
        }
        [NonAction]
        public string UploadFileToLakeFS(byte[] bytesData, string extension, string repo)
        {
            try
            {

                if (bytesData == null)
                    throw new ArgumentNullException(nameof(bytesData), "Precisa incluir o arquivo");

                using var client = new HttpClient();
                using var formData = new MultipartFormDataContent();
                //;

                var path = $"{Guid.NewGuid():N}{extension}";
                formData.Add(new StreamContent(new MemoryStream(bytesData)), "content", path);

                var authenticationString = $"{_lakeFsConfigModel.ClientId}:{_lakeFsConfigModel.SecretKey}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));

                client.DefaultRequestHeaders.Authorization
                              = new AuthenticationHeaderValue("BASIC", base64EncodedAuthenticationString);
                var response = client
                .PostAsync($"{_lakeFsConfigModel.Server}/repositories/{repo}/branches/{_lakeFsConfigModel.Branch}/objects?path={path}", formData).Result;

                if (response.StatusCode == HttpStatusCode.Created)
                    return path;
                else
                    throw new HttpRequestException("Erro ao carregar conteudo");
            }
            catch (Exception)
            {

                throw new HttpRequestException("Erro ao carregar conteudo");
            }
        }
        [NonAction]
        public TemplateViewModel ToModel(TemplateModel model)
        {
            return new TemplateViewModel
            {
                title = model.Name,
                description = model.Name,
                content = GetContent(model.Path,"templates"),
            };
        }
        [NonAction]
        public string GetContent(string path,string repo)
        {
            try
            {
                var _lakeFsConfigModel = new LakeFsConfigModel(_configuration);
                using var client = new HttpClient();
                var authenticationString = $"{_lakeFsConfigModel.ClientId}:{_lakeFsConfigModel.SecretKey}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));

                client.DefaultRequestHeaders.Authorization
                              = new AuthenticationHeaderValue("BASIC", base64EncodedAuthenticationString);
                var response =
                    client.GetAsync($"{_lakeFsConfigModel.Server}/repositories/{repo}/refs/{_lakeFsConfigModel.Branch}/objects?path={path}").Result;
                var data = response.Content.ReadAsByteArrayAsync();

                var html = Encoding.ASCII.GetString(data.Result);

                return html;
            }
            catch (Exception)
            {

                return "<p>Erro ao carregar conteúdo do template</p>";
            }

        }
        [NonAction]
        public string UploadFile(IFormFile file, string folder, string id)
        {
            try
            {
                var fileName = id.ToString() + Path.GetExtension(file.FileName);
                var filePathVirtual = "/Storage" + "/" + folder + "/" + fileName;
                var filePath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Storage", folder);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);
                using (
                    var stream = System.IO.File.Create(Path.Combine(filePath, fileName)))
                {
                    file.CopyTo(stream);
                }
                return filePathVirtual;

            }
            catch (Exception)
            {
                return null;
            }
        }
        [NonAction]
        public string DeleteFile(string UrlFile)
        {
            List<string> list = new() { Environment.CurrentDirectory, "wwwroot" };
            list.AddRange(UrlFile.Split("/").ToList());
            var filePath = Path.Combine(list.ToArray());
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
            return filePath;

        }
    }
}
