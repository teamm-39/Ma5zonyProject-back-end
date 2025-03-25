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
        private CustomerSupplierIRepo _supplier;
        public ImortOperationController(OperationIRepo operation, OperationStoreProductIRepo operationStoreProduct, StoreIRepo store, ProductIRepo product, StoreProductIRepo storeProduct, CustomerSupplierIRepo supplier)
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
                filters.Add("CustomerSupplier.Name", supplierName);

            var data = _operation.GetAll(
                pageSize: pageSize, pageNumber: pageNumber,
                includes: [e => e.CustomerSupplier, e => e.ApplicationUser],
                filters: filters ,expression:e=>e.IsDeleted==false);

            var importsOperations = data?.Data?.Select(op => new ImportsVM
            {
                OperationId = op.OperationId,
                DateTime = op.DateTime,
                TotalPrice = op.TotalPrice ?? 0,
                UserName = op.ApplicationUser.Name,
                SupplierName = op.CustomerSupplier.Name
            }).ToList();
            res.Data = importsOperations;
            res.IsSuccess = true;
            res.Total = data.Total;
            res.PageNumber = pageNumber;
            res.PageSize = pageSize;
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
            var getSupplier = _supplier.GetOne(e => e.CustomerSupplierId == importOperation.SupplierId && e.IsDeleted == false&&e.LookupCustomerSupplierTypeId==1);
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
                var getProduct = _product.GetOne(e => e.ProductId == sp.ProductId && e.IsDeleted == false);
                getProduct.Quantity = sp.Quantity + getProduct.Quantity;
                _product.Edit(getProduct);
                _product.commit();
                var getStore = _store.GetOne(e => e.StoreId == sp.ToStoreId && e.IsDeleted == false);
                var storeProduct = _storeProduct.GetOne(e => e.ProductId == getProduct.ProductId && e.StoreId == sp.ToStoreId);
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
            getSupplier.NumOfDeal++;
            _supplier.Edit(getSupplier);
            _supplier.commit();
            res.IsSuccess = true;
            return Ok(res);
        }
        [HttpGet("details/{id}")]
        public IActionResult GetOPeration(int id)
        {
            var res = new Result<OperationVM>();
            var operation = _operation.GetOne(e => e.OperationId == id&& e.IsDeleted==false && e.LookupOperationTypeId == StaticData.ImportOperation, includes: [e => e.ApplicationUser, e => e.CustomerSupplier]);
            if (operation == null)
            {
                res.Meesage = "لم يتم العثور على هذه العمليه ";
                return BadRequest(res);
            }
            var operationVM = new OperationVM()
            {
                OperationId = operation.OperationId,
                DateTime = operation.DateTime,
                UserName = operation.ApplicationUser.Name,
                SupplierName = operation.CustomerSupplier.Name,
                SupplierId=operation.CustomerSupplierId,
                TotalPrice = operation.TotalPrice
            };
            res.Data = operationVM;
            return Ok(res);
        }
        [HttpGet("details/get-products-to-stores/{id}")]
        public IActionResult GetProductsStoreForImportOperationDetails(int id, int pageSize = 5, int pageNumber = 1)
        {
            var res = new Result<List<StoreProductForGetImportOperationVM>>();
            if (pageSize <= 0 || pageNumber <= 0)
            {
                res.Meesage = "رقم الصفحة وعدد العناصر يجب أن يكونا أكبر من الصفر";
                return BadRequest(res);
            }
            var products = _operationStoreProduct.GetAll
                (
                expression: e => e.OperationId == id && e.IsDeleted==false ,
                includes: [e => e.Product, e => e.ToStore],
                pageSize: pageSize, pageNumber: pageNumber
                );
            if (products == null)
            {
                res.Meesage = "لم يتم العثور على منتجات لهذه العمليه";
                return BadRequest(res);
            }
            var data = products.Data.Select(e => new StoreProductForGetImportOperationVM
            {
                Id = e.OperationStoreProductId,
                ProductId = e.ProductId,
                ProductName = e.Product.Name,
                ToStoreId = e.ToStoreId,
                StoreName = e.ToStore.Name,
                Quantity = e.Quantity,
                Price = e.Product.PurchasePrice
            }).ToList();
            res.Data = data;
            res.IsSuccess = true;
            res.PageSize = pageSize;
            res.Total = products.Total;
            res.PageNumber = pageNumber;
            return Ok(res);
        }
        [HttpGet("details/get-products-to-stores-for-edit/{id}")]
        public IActionResult GetProductsStoreForImportOperationEdit(int id)
        {
            var res = new Result<List<StoreProductForGetImportOperationVM>>();
            var products = _operationStoreProduct.GetAllWithoutPagination
                (
                expression: e => e.OperationId == id && e.IsDeleted == false,
                includes: [e => e.Product, e => e.ToStore]
                );
            if (products == null)
            {
                res.Meesage = "لم يتم العثور على منتجات لهذه العمليه";
                return BadRequest(res);
            }
            var data = products.Select(e => new StoreProductForGetImportOperationVM
            {
                Id = e.OperationStoreProductId,
                ProductId = e.ProductId,
                ProductName = e.Product.Name,
                ToStoreId = e.ToStoreId,
                StoreName = e.ToStore.Name,
                Quantity = e.Quantity,
                Price = e.Product.PurchasePrice
            }).ToList();
            res.Data = data;
            res.IsSuccess = true;
            res.Total = products.Count();
            return Ok(res);
        }
        [HttpPut("edit/{id}")]
        public IActionResult Edit(int id, ImportOperationCreateVM operationVM)
        {
            var res = new Result<ImportOperationCreateVM>();
            var operation = _operation.GetOne(e => e.OperationId == id && e.LookupOperationTypeId == StaticData.ImportOperation && e.IsDeleted==false);
            if (operation == null)
            {
                res.Meesage = "لم يتم العثور على العمليه";
                return BadRequest(res);
            }
            var supplier = _supplier.GetOne(e => e.CustomerSupplierId == operationVM.SupplierId && e.IsDeleted == false && e.LookupCustomerSupplierTypeId == 1);
            if (supplier == null)
            {
                res.Meesage = "لم يتم العثور على المورد";
                return BadRequest(res);
            }
            foreach (var sp in operationVM.SP)
            {
                var s = _store.GetOne(e => e.StoreId == sp.ToStoreId && e.IsDeleted == false);
                if (s == null)
                {
                    res.Meesage = "يرجى ادخال المخازن بشكل صحيح";
                    return BadRequest(res);
                }
                var p = _product.GetOne(e => e.ProductId == sp.ProductId && e.IsDeleted == false);
                if (p == null)
                {
                    res.Meesage = "يرجى ادخال المنتجات بشكل صحيح";
                    return BadRequest(res);
                }
            }
            foreach (var i in _operationStoreProduct.GetAllIds(id))
            {
                var SP = _storeProduct.GetOne(e => e.ProductId == i.ProductId && e.StoreId == i.ToStoreId);
                var s = _store.GetOne(e => e.StoreId == i.ToStoreId);
                var p = _product.GetOne(e => e.ProductId == i.ProductId);
                SP.Quantity = SP.Quantity - i.Quantity;
                p.Quantity = p.Quantity - i.Quantity;
                _operationStoreProduct.Delete(i.OperationStoreProductId);
                _storeProduct.Edit(SP);
                _product.Edit(p);
            }
            operation.CustomerSupplierId = operationVM.SupplierId;
            _storeProduct.commit();
            _product.commit();
            _operationStoreProduct.commit();
            operation.TotalPrice = 0;
            foreach (var sp in operationVM.SP)
            {
                var product = _product.GetOne(e => e.ProductId == sp.ProductId && e.IsDeleted == false);
                var store = _store.GetOne(e => e.StoreId == sp.ToStoreId && e.IsDeleted == false);
                var storeHasProduct = _storeProduct.GetOne(e => e.StoreId == sp.ToStoreId && e.ProductId == sp.ProductId);
                if (storeHasProduct != null)
                {
                    storeHasProduct.Quantity += sp.Quantity;
                }
                else
                {
                    var createStoreProduct = new StoreProducts() { ProductId = sp.ProductId, StoreId = sp.ToStoreId, Quantity = sp.Quantity };
                    _storeProduct.Create(createStoreProduct);
                }
                product.Quantity = product.Quantity + sp.Quantity;
                var OSP = new OperationStoreProduct() { OperationId = id, ToStoreId = sp.ToStoreId, ProductId = sp.ProductId, Quantity = sp.Quantity };
                _operationStoreProduct.Create(OSP);
                _product.Edit(product);
                operation.TotalPrice = operation.TotalPrice + (sp.Quantity * product.PurchasePrice);
            }
            _operation.Edit(operation);
            _operation.commit();
            _product.commit();
            _operationStoreProduct.commit();
            _storeProduct.commit();
            res.IsSuccess = true;
            return Ok(res);
        }
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var res = new Result<OperationVM>();
            var operation = _operation.GetOne(e => e.OperationId == id && e.IsDeleted == false &&e.LookupOperationTypeId==StaticData.ImportOperation);
            if (operation == null)
            {
                res.Meesage = "لم يتم العثور على هذه العمليه";
                return BadRequest(res);
            }
            var supplier = _supplier.GetOne(e => e.CustomerSupplierId == operation.CustomerSupplierId && e.IsDeleted == false && e.LookupCustomerSupplierTypeId == 1);
            if (supplier == null) {
                res.Meesage = "لم يتم العثور على مورد هذه العمليه";
                return BadRequest(res);
            }
            var operationStoreProduct = _operationStoreProduct.GetAllIds(id);
            foreach (var osp in operationStoreProduct)
            {
                var store = _store.GetOne(e => e.StoreId == osp.ToStoreId && e.IsDeleted == false);
                var product = _product.GetOne(e => e.ProductId == osp.ProductId && e.IsDeleted == false);
                var storeProduct = _storeProduct.GetOne(e => e.ProductId == osp.ProductId && e.StoreId == osp.ToStoreId);
                if (storeProduct.Quantity < osp.Quantity)
                {
                    res.Meesage = $"لا يمكن حذف هذه العمليه لان كمية المنتج {product.Name} فى مخزن {store.Name} غير كافيه";
                    return BadRequest(res);
                }
                storeProduct.Quantity = storeProduct.Quantity - osp.Quantity;
                product.Quantity = product.Quantity - osp.Quantity;
                osp.IsDeleted = true;
                _storeProduct.Edit(storeProduct);
                _product.Edit(product);
                _operationStoreProduct.Edit(osp);
            }
            supplier.NumOfDeal--;
            _supplier.Edit(supplier);
            _supplier.commit();
            operation.IsDeleted = true;
            _operation.Edit(operation);
            _storeProduct.commit();
            _product.commit();
            _operationStoreProduct.commit();
            _operation.commit();
            res.IsSuccess=true;
            return Ok(res);
        }
    }
}
