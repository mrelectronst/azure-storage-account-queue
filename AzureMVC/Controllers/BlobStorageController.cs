using AzureStorageLib;
using AzureTableStorageMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzureTableStorageMVC.Controllers
{
    public class BlobStorageController : Controller
    {
        private readonly IBlobStorage _blobStorage;

        public BlobStorageController(IBlobStorage blobStorage)
        {
            this._blobStorage = blobStorage;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var names = _blobStorage.GetNames(EnumContainerName.azurecontainerpics);

            string blobUrl = $"{_blobStorage.BlobUrl}/{EnumContainerName.azurecontainerpics.ToString()}";

            ViewBag.blobs = names.Select(x => new BlobFile { Name = x, Url = $"{blobUrl}/{x}" }).ToList();

            ViewBag.logs = await _blobStorage.GetLogAsync("controller.txt");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile picture)
        {
            await _blobStorage.SetLogAsync("Entered Upload Method", "controller.txt");

            var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(picture.FileName);
            await _blobStorage.UploadAsync(picture.OpenReadStream(), newFileName, EnumContainerName.azurecontainerpics);

            await _blobStorage.SetLogAsync("Exit Upload Method", "controller.txt");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Download(string fileName)
        {
            var stream = await _blobStorage.DownloadAsync(fileName, EnumContainerName.azurecontainerpics);

            return File(stream, "application/octet-stream", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string fileName)
        {
            await _blobStorage.DeleteAsync(fileName, EnumContainerName.azurecontainerpics);

            return RedirectToAction("Index");
        }
    }
}
