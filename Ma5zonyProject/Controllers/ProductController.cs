using DataAccess.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Models.ViewModels;
using System.Diagnostics.Metrics;
using Utility;

namespace Ma5zonyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private ProductIRepo _product;

        public ProductController(ProductIRepo product)
        {
            _product = product;
        }
        [HttpGet]
        public IActionResult GetAll(int pageSize = 5, int pageNumber = 1, string? name = null, double? sellingPrice = null, double? purchasePrice = null)
        {
            var res = new Result<List<ProductsVM>>();
            if (pageSize <= 0 || pageNumber <= 0)
            {
                res.Meesage = "رقم الصفحة وعدد العناصر يجب أن يكونا أكبر من الصفر";
                return BadRequest(res);
            }
            if (sellingPrice<=0 || purchasePrice<= 0)
            {
                res.Meesage = "سعر الشراء وسعر البيع يجب ان يكونوا اكبر من الصفر";
                return BadRequest(res);
            }
            var filters = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(name)) filters.Add("Name", name);
            if (sellingPrice.HasValue && sellingPrice > 0) filters.Add("SellingPrice", sellingPrice);
            if (purchasePrice.HasValue && purchasePrice > 0) filters.Add("PurchasePrice", purchasePrice);
            var products = _product.GetAll(pageNumber: pageNumber, pageSize: pageSize, filters: filters, expression: e => e.IsDeleted == false);
            res.Data = products.Data?.Select(p => new ProductsVM
            {
                ProductId = p.ProductId,
                Name = p.Name,
                SellingPrice = p.SellingPrice,
                PurchasePrice = p.PurchasePrice,
                MinLimit = p.MinLimit,
            }).ToList();
            res.IsSuccess=true;
            res.Total = products.Total;
            res.PageNumber = pageNumber;
            res.PageSize = pageSize;
            return Ok(res);
        }
        [HttpPost("crete")]
        public IActionResult Create(ProductVM productVM)
        {
            var res = new Result<ProductVM>();
            if (!ModelState.IsValid || productVM.SellingPrice <= 0 || productVM.PurchasePrice <= 0 || productVM.MinLimit <= 0)
            {
                res.Meesage = "يرجى ادخال بيانات المنتج بشكل صحيح";
                return BadRequest(res);
            }
            var chekIfNameIsNotHere = _product.GetOne(e => e.Name == productVM.Name);
            if (chekIfNameIsNotHere != null)
            {
                res.Meesage = "اسم المنتج مستخدم من قبل";
                return BadRequest(res);
            }
            Product product = new Product
            {
                Name = productVM.Name,
                SellingPrice = productVM.SellingPrice,
                MinLimit = productVM.MinLimit,
                PurchasePrice = productVM.PurchasePrice,
                Quantity = 0
            };
            _product.Create(product);
            _product.commit();
            res.IsSuccess = true;
            res.Meesage = "تم انشاء المنتج بنجاح";
            return Ok(res);
        }
        [HttpGet("details/{id}")]
        public IActionResult GetOne(int id)
        {
            var res = new Result<ProductDetailsVM>();
            var product = _product.GetOne(e => e.ProductId == id);
            if (product == null || product.IsDeleted == true)
            {
                res.Meesage = "لم يتم العثور على هذا المنتج";
                return BadRequest(res);
            }
            var productVM = new ProductDetailsVM
            {
                ProductId = id,
                MinLimit = product.MinLimit,
                SellingPrice = product.SellingPrice,
                PurchasePrice = product.PurchasePrice,
                Name = product.Name,
                Quantity = product.Quantity
            };
            res.Data = productVM;
            res.IsSuccess = true;
            return Ok(res);
        }
        [HttpPut("edit")]
        public IActionResult Edit(ProductEditVM productVM)
        {
            var res = new Result<ProductDetailsVM>();
            if (!ModelState.IsValid || productVM.MinLimit <= 0 || productVM.SellingPrice <= 0 || productVM.PurchasePrice <= 0)
            {
                res.Meesage = "يرجى ادخال بيانات المنتج بشكل صحيح";
                return BadRequest(res);
            }
            if (productVM.SellingPrice <= 0)
            {
                res.Meesage = "يجب ان يكون سعر البيع اكبر من الصفر";
                return BadRequest(res);
            }
            if (productVM.PurchasePrice <= 0)
            {
                res.Meesage = "يجب ان يكون سعر الشراء اكبر من الصفر";
                return BadRequest(res);
            }
            if (productVM.MinLimit <= 0)
            {
                res.Meesage = "يجب ان يكون الحد الادنى اكبر من الصفر";
                return BadRequest(res);
            }
            var chekIfNameExists=_product.GetOne(e=>e.Name==productVM.Name);
            if (chekIfNameExists != null)
            {
                res.Meesage = "اسم المنتج مستخدم من قبل";
                return BadRequest(res);
            }
            var oldProduct = _product.GetOne(e => e.ProductId == productVM.ProductId);
            if (oldProduct == null || oldProduct.IsDeleted == true)
            {
                res.Meesage = "لم يتم العثور على هذا المنتج";
                return BadRequest(res);
            }
            oldProduct.Name = productVM.Name;
            oldProduct.PurchasePrice = productVM.PurchasePrice;
            oldProduct.SellingPrice = productVM.SellingPrice;
            oldProduct.MinLimit = productVM.MinLimit;
            _product.Edit(oldProduct);
            _product.commit();
            res.IsSuccess = true;
            return Ok(res);
        }
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var res = new Result<ProductVM>();
            var product = _product.GetOne(e => e.ProductId == id);
            if (product == null || product.IsDeleted == true)
            {
                res.Meesage = "لم يتم العثور على هذا المنتج";
                return BadRequest(res);
            }
            if (product.Quantity > 0)
            {
                res.Meesage = "لا يمكن حذف المنتج لوجود كميه متبقيه منه فى المخازن";
                return BadRequest(res);
            }
            _product.Delete(id);
            _product.commit();
            res.IsSuccess = true;
            res.Meesage = "تم حذف المنتج بنجاح";
            return Ok(res);
        }
    }
}