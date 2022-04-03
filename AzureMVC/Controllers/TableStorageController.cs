using AzureMVC.Models;
using Microsoft.AspNetCore.Mvc;
using NoSqlDomain;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AzureMVC.Controllers
{
    public class TableStorageController : Controller
    {
        private readonly ITableStorage<Product> _product;

        public TableStorageController(ITableStorage<Product> product)
        {
            this._product = product;
        }

        public IActionResult Index()
        {
            ViewBag.products = _product.GetAll().ToList();
            ViewBag.IsUpdate = false;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Post(Product product)
        {
            product.RowKey = Guid.NewGuid().ToString();
            product.PartitionKey = "Categories";

            await _product.Post(product);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Put(string RowKey, string PartitionKey)
        {
            var product = await _product.GetByRowAndPartitionKeys(RowKey, PartitionKey);

            ViewBag.products = _product.GetAll().ToList();
            ViewBag.IsUpdate = true;

            return View("Index", product);
        }

        [HttpPost]
        public async Task<IActionResult> Put(Product product)
        {
            product.ETag = "*";
            ViewBag.IsUpdate = true;
            await _product.Put(product);

            return RedirectToAction("Index");
        }
        
        public async Task<IActionResult> Delete(string RowKey, string PartitionKey)
        {
            await _product.Delete(RowKey, PartitionKey);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Query(int stock)
        {
            ViewBag.IsUpdate = false;
            ViewBag.products = _product.Query(x => x.Stock > stock).ToList();

            return View("Index");
        }
    }
}
