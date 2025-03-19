using DataAccess.IRepos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Models.ViewModels;
using System.Security.Claims;
using Utility;
namespace Ma5zonyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImortOperationController : ControllerBase
    {
        private OperationIRepo _operation;
        private StoreIRepo _store;
        private ProductIRepo _product;
        private OperationStoreProductIRepo _operationStoreProduct;
        private StoreProductIRepo _storeProduct;
        private SupplierIRepo _supplier;
        public ImortOperationController(OperationIRepo operation, OperationStoreProductIRepo operationStoreProduct, StoreIRepo store, ProductIRepo product, StoreProductIRepo storeProduct, SupplierIRepo supplier)
        {
            _operation = operation;
            _operationStoreProduct = operationStoreProduct;
            _store = store;
            _product = product;
            _storeProduct = storeProduct;
            _supplier = supplier;
        }
        [HttpGet]
        public IActionResult GetAll(int pageNumber = 1, int pageSize = 5, DateTime? dateTime = null, string? userName = null, string? supplierName = null)
        {
            var res = new Result<List<ImportsVM>>();
            if (pageSize <= 0 || pageNumber <= 0)
            {
                res.Meesage = "رقم الصفحة وعدد العناصر يجب أن يكونا أكبر من الصفر";
                return BadRequest(res);
            }
            var filters = new Dictionary<string, object>
            {
                { "LookupOperationTypeId", StaticData.ImportOperation }
            };
            if (!string.IsNullOrWhiteSpace(userName))
                filters.Add("ApplicationUser.Name", userName);

            if (dateTime.HasValue)
                filters.Add("DateTime", dateTime.Value);

            if (!string.IsNullOrWhiteSpace(supplierName))
                filters.Add("Supplier.Name", supplierName);

            var data = _operation.GetAll(
                pageSize: pageSize, pageNumber: pageNumber,
                includes: [e => e.Supplier, e => e.ApplicationUser],
                filters:filters);

            var importsOperations = data?.Data?.Select(op => new ImportsVM
            {
                OperationId = op.OperationId,
                DateTime = op.DateTime,
                TotalPrice = op.TotalPrice ?? 0,
                UserName = op.ApplicationUser.Name,
                SupplierName = op.Supplier.Name
            }).ToList();
            res.Data = importsOperations;
            res.IsSuccess = true;
            res.Total = data.Total;
            res.PageNumber = pageNumber;
            res.PageSize=pageSize;
            return Ok(res);
        }
        [HttpPost("create")]
        public IActionResult Create(ImportOperationCreateVM importOperation)
        {
            var res = new Result<Operation>();
            if (!ModelState.IsValid)
            {
                res.Meesage = "يرجى ادخال البيانات بشكل صحيح";
                return BadRequest(res);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                res.Meesage = "يرجى تسجيل الدخول اولا";
                return BadRequest(res);
            }
            var getSupplier = _supplier.GetOne(e => e.SupplierId == importOperation.SupplierId && e.IsDeleted == false);
            if (getSupplier == null)
            {
                res.Meesage = "يرجى ادخال المورد بشكل صحيح";
                return BadRequest(res);
            }
            double totalPrice = 0;
            foreach (var sp in importOperation.SP)
            {
                var getStore = _store.GetOne(e => e.StoreId == sp.ToStoreId && e.IsDeleted == false);
                if (getStore == null)
                {
                    res.Meesage = "يرجى ادخال المخازن بشكل صحيح";
                    return BadRequest(res);
                }
                var getProduct = _product.GetOne(e => e.ProductId == sp.ProductId && e.IsDeleted == false);
                if (getProduct == null)
                {
                    res.Meesage = "يرجى ادخال المنتجات بشكل صحيح";
                    return BadRequest(res);
                }
                if (sp.Quantity <= 0)
                {
                    res.Meesage = "الكميه يجب ان تكون رقم صحيح";
                }
                totalPrice = (sp.Quantity * getProduct.PurchasePrice) + totalPrice;
            }

            var operationId = _operation.CreateOperation(StaticData.ImportOperation, userId, importOperation.SupplierId, totalPrice);
            foreach (var sp in importOperation.SP)
            {
                var operationStoreProduct = new OperationStoreProduct() { ToStoreId = sp.ToStoreId, ProductId = sp.ProductId, OperationId = operationId, Quantity = sp.Quantity };
                _operationStoreProduct.Create(operationStoreProduct);
                _operationStoreProduct.commit();
                getSupplier.NumOfDeal++;
                _supplier.Edit(getSupplier);
                _supplier.commit();
                var getProduct = _product.GetOne(e => e.ProductId == sp.ProductId && e.IsDeleted == false);
                getProduct.Quantity += sp.Quantity;
                _product.Edit(getProduct);
                _product.commit();
                var getStore = _store.GetOne(e => e.StoreId == sp.ToStoreId && e.IsDeleted == false);
                var storeProduct = _storeProduct.GetOne(e => e.ProductId == getProduct.ProductId && e.StoreId == sp.ToStoreId && e.IsDeleted == false);
                if (storeProduct != null)
                {
                    storeProduct.Quantity += sp.Quantity;
                }
                else
                {
                    var createStoreProduct = new StoreProducts() { ProductId = getProduct.ProductId, StoreId = sp.ToStoreId, Quantity = sp.Quantity };
                    _storeProduct.Create(createStoreProduct);
                    _storeProduct.commit();
                }
            }
            res.IsSuccess = true;
            return Ok(res);
        }
    }
}
