using DataAccess.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Models.ViewModels;
using System.Diagnostics.Metrics;
using System.Security.Claims;
using Utility;

namespace Ma5zonyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private ProductIRepo _product;
        private UserManager<ApplicationUser> _userManager;
        private ProductLogIRepo _log;

        public ProductController(ProductIRepo product, UserManager<ApplicationUser> userManager, ProductLogIRepo log)
        {
            _product = product;
            _userManager = userManager;
            _log = log;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageSize = 5, int pageNumber = 1, string? name = null, double? sellingPrice = null, double? purchasePrice = null)
        {
            var res = new Result<List<ProductsVM>>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted == true)
            {
                res.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(res);
            }
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
                Quantity=p.Quantity
            }).ToList();
            res.IsSuccess=true;
            res.Total = products.Total;
            res.PageNumber = pageNumber;
            res.PageSize = pageSize;
            return Ok(res);
        }
        [HttpPost("crete")]
        public async Task<IActionResult> Create(ProductVM productVM)
        {
            var res = new Result<ProductVM>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted == true)
            {
                res.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(res);
            }
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
            _log.CreateOperationLog(null, product, StaticData.AddOperationType, userId);
            return Ok(res);
        }
        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var res = new Result<ProductDetailsVM>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted == true)
            {
                res.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(res);
            }
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
        public async Task<IActionResult> Edit(ProductEditVM productVM)
        {
            var res = new Result<ProductDetailsVM>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted == true)
            {
                res.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(res);
            }
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
            var chekIfNameExists=_product.GetOne(e=>e.Name==productVM.Name && e.ProductId != productVM.ProductId);
            if (chekIfNameExists != null)
            {
                res.Meesage = "اسم المنتج مستخدم من قبل";
                return BadRequest(res);
            }
            var product = _product.GetOne(e => e.ProductId == productVM.ProductId);
            if (product == null || product.IsDeleted == true)
            {
                res.Meesage = "لم يتم العثور على هذا المنتج";
                return BadRequest(res);
            }
            var oldProduct = new Product
            {
                Name = product.Name,
                MinLimit = product.MinLimit,
                PurchasePrice = product.PurchasePrice,
                SellingPrice = product.SellingPrice,
            };
            product.Name = productVM.Name;
            product.PurchasePrice = productVM.PurchasePrice;
            product.SellingPrice = productVM.SellingPrice;
            product.MinLimit = productVM.MinLimit;
            _product.Edit(product);
            _product.commit();
            res.IsSuccess = true;
            _log.CreateOperationLog(oldProduct, product, StaticData.EditOperationType, userId);
            return Ok(res);
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var res = new Result<ProductVM>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted == true)
            {
                res.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(res);
            }
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
            _log.CreateOperationLog(product, null, StaticData.DeleteOperationType, userId);
            return Ok(res);
        }
        [HttpGet("get-products-for-import-operation")]
        public async Task<IActionResult> GetProductsForImportOperations()
        {
            var res = new Result<List<ProductForOperation>>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted == true)
            {
                res.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(res);
            }
            res.Data = _product.GetProductsForOperations(1);
            res.IsSuccess = true;
            return Ok(res);
        }
        [HttpGet("get-products-for-export-operation")]
        public async Task<IActionResult> GetProductsForExportOperations()
        {
            var res = new Result<List<ProductForOperation>>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted == true)
            {
                res.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(res);
            }
            res.Data = _product.GetProductsForOperations(2);
            res.IsSuccess = true;
            return Ok(res);
        }
    }
}