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
    public class ExportOperationController : ControllerBase
    {
        private OperationIRepo _operation;
        private StoreIRepo _store;
        private ProductIRepo _product;
        private OperationStoreProductIRepo _operationStoreProduct;
        private CustomerSupplierIRepo _customer;
        private StoreProductIRepo _storeProduct;
        public ExportOperationController(OperationIRepo operation, OperationStoreProductIRepo operationStoreProduct, StoreIRepo store, ProductIRepo product, StoreProductIRepo storeProduct, CustomerSupplierIRepo customer)
        {
            _operation = operation;
            _operationStoreProduct = operationStoreProduct;
            _store = store;
            _product = product;
            _storeProduct = storeProduct;
            _customer = customer;
        }

        [HttpGet]
        public IActionResult GetAll(int pageNumber = 1, int pageSize = 5, DateTime? dateTime = null, string? userName = null, string? customerName = null)
        {
            var res = new Result<List<ExportsVM>>();
            if (pageSize <= 0 || pageNumber <= 0)
            {
                res.Meesage = "رقم الصفحة وعدد العناصر يجب أن يكونا أكبر من الصفر";
                return BadRequest(res);
            }
            var filters = new Dictionary<string, object>
            {
                { "LookupOperationTypeId", StaticData.ExportOperation }
            };
            if (!string.IsNullOrWhiteSpace(userName))
                filters.Add("ApplicationUser.Name", userName);

            if (dateTime.HasValue)
                filters.Add("DateTime", dateTime.Value);

            if (!string.IsNullOrWhiteSpace(customerName))
                filters.Add("CustomerSupplier.Name", customerName);

            var data = _operation.GetAll(
                pageSize: pageSize, pageNumber: pageNumber,
                includes: [e => e.CustomerSupplier, e => e.ApplicationUser],
                filters: filters, expression: e => e.IsDeleted == false);

            var ExportsOperations = data?.Data?.Select(op => new ExportsVM
            {
                OperationId = op.OperationId,
                DateTime = op.DateTime,
                TotalPrice = op.TotalPrice ?? 0,
                UserName = op.ApplicationUser.Name,
                CustomerName = op.CustomerSupplier.Name
            }).ToList();
            res.Data = ExportsOperations;
            res.IsSuccess = true;
            res.Total = data.Total;
            res.PageNumber = pageNumber;
            res.PageSize = pageSize;
            return Ok(res);
        }

        [HttpPost("create")]
        public IActionResult Create(ExportOperationCreateVM exportOperation)
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
            var getCustomer = _customer.GetOne(e => e.CustomerSupplierId == exportOperation.CustomerId && e.IsDeleted == false && e.LookupCustomerSupplierTypeId == 2);
            if (getCustomer == null)
            {
                res.Meesage = "يرجى ادخال العميل بشكل صحيح";
                return BadRequest(res);
            }
            double totalPrice = 0;
            foreach (var sp in exportOperation.SP)
            {
                var getStore = _store.GetOne(e => e.StoreId == sp.FromStoreId && e.IsDeleted == false);
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
                var getStoreProduct = _storeProduct.GetOne(e => e.ProductId == sp.ProductId && e.StoreId == sp.FromStoreId);
                if (getStoreProduct == null)
                {
                    res.Meesage = $"لا يوجد منتجات من {getProduct.Name} فى مخزن {getStore.Name}";
                    return BadRequest(res);
                }
                if (getStoreProduct.Quantity < sp.Quantity)
                {
                    res.Meesage = $"لا يوجد كمية كافيه من منتج {getProduct.Name} فى مخزن {getStore.Name}";
                    return BadRequest(res);
                }
                totalPrice = (sp.Quantity * getProduct.SellingPrice) + totalPrice;
            }

            var operationId = _operation.CreateOperation(StaticData.ExportOperation, userId, exportOperation.CustomerId, totalPrice);
            foreach (var sp in exportOperation.SP)
            {
                var operationStoreProduct = new OperationStoreProduct() { FromStoreId = sp.FromStoreId, ProductId = sp.ProductId, OperationId = operationId, Quantity = sp.Quantity };
                _operationStoreProduct.Create(operationStoreProduct);
                _operationStoreProduct.commit();
                var getProduct = _product.GetOne(e => e.ProductId == sp.ProductId && e.IsDeleted == false);
                getProduct.Quantity = getProduct.Quantity - sp.Quantity;
                _product.Edit(getProduct);
                _product.commit();
                var storeProduct = _storeProduct.GetOne(e => e.ProductId == getProduct.ProductId && e.StoreId == sp.FromStoreId);
                storeProduct.Quantity = storeProduct.Quantity - sp.Quantity;
                _storeProduct.Edit(storeProduct);
                _storeProduct.commit();
            }
            getCustomer.NumOfDeal++;
            _customer.Edit(getCustomer);
            _customer.commit();
            res.IsSuccess = true;
            return Ok(res);
        }
        [HttpGet("details/{id}")]
        public IActionResult GetOPeration(int id)
        {
            var res = new Result<OperationVM>();
            var operation = _operation.GetOne(e => e.OperationId == id && e.IsDeleted == false && e.LookupOperationTypeId == StaticData.ExportOperation, includes: [e => e.ApplicationUser, e => e.CustomerSupplier]);
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
                CustomerName = operation.CustomerSupplier.Name,
                CustomerId = operation.CustomerSupplierId,
                TotalPrice = operation.TotalPrice
            };
            res.Data = operationVM;
            return Ok(res);
        }
        [HttpGet("details/get-products-from-stores/{id}")]
        public IActionResult GetProductsStoreForExportOperationDetails(int id, int pageSize = 5, int pageNumber = 1)
        {
            var res = new Result<List<StoreProductForGetOperationVM>>();
            if (pageSize <= 0 || pageNumber <= 0)
            {
                res.Meesage = "رقم الصفحة وعدد العناصر يجب أن يكونا أكبر من الصفر";
                return BadRequest(res);
            }
            var products = _operationStoreProduct.GetAll
                (
                expression: e => e.OperationId == id && e.IsDeleted == false,
                includes: [e => e.Product, e => e.FromStore],
                pageSize: pageSize, pageNumber: pageNumber
                );
            if (products.Data == null)
            {
                res.Meesage = "لم يتم العثور على منتجات لهذه العمليه";
                return BadRequest(res);
            }
            var data = products.Data.Select(e => new StoreProductForGetOperationVM
            {
                Id = e.OperationStoreProductId,
                ProductId = e.ProductId,
                ProductName = e.Product.Name,
                FromStoreId = e.FromStoreId,
                StoreName = e.FromStore.Name,
                Quantity = e.Quantity,
                Price = e.Product.SellingPrice
            }).ToList();
            res.Data = data;
            res.IsSuccess = true;
            res.PageSize = pageSize;
            res.Total = products.Total;
            res.PageNumber = pageNumber;
            return Ok(res);
        }
        [HttpGet("details/get-products-from-stores-for-edit/{id}")]
        public IActionResult GetProductsStoreForExportOperationEdit(int id)
        {
            var res = new Result<List<StoreProductForGetOperationVM>>();
            var products = _operationStoreProduct.GetAllWithoutPagination
                (
                expression: e => e.OperationId == id && e.IsDeleted == false,
                includes: [e => e.Product, e => e.FromStore]
                );
            if (products == null)
            {
                res.Meesage = "لم يتم العثور على منتجات لهذه العمليه";
                return BadRequest(res);
            }
            var data = products.Select(e => new StoreProductForGetOperationVM
            {
                Id = e.OperationStoreProductId,
                ProductId = e.ProductId,
                ProductName = e.Product.Name,
                FromStoreId = e.FromStoreId,
                StoreName = e.FromStore.Name,
                Quantity = e.Quantity,
                Price = e.Product.SellingPrice
            }).ToList();
            res.Data = data;
            res.IsSuccess = true;
            res.Total = products.Count();
            return Ok(res);
        }
        [HttpPut("edit/{id}")]
        public IActionResult Edit(int id, ExportOperationCreateVM operationVM)
        {
            var res = new Result<ExportOperationCreateVM>();
            var operation = _operation.GetOne(e => e.OperationId == id && e.LookupOperationTypeId == StaticData.ExportOperation && e.IsDeleted == false);
            if (operation == null)
            {
                res.Meesage = "لم يتم العثور على العمليه";
                return BadRequest(res);
            }
            var customer = _customer.GetOne(e => e.CustomerSupplierId == operationVM.CustomerId && e.IsDeleted == false && e.LookupCustomerSupplierTypeId == 2);
            if (customer == null)
            {
                res.Meesage = "لم يتم العثور على العميل";
                return BadRequest(res);
            }
            foreach (var sp in operationVM.SP)
            {
                var s = _store.GetOne(e => e.StoreId == sp.FromStoreId && e.IsDeleted == false);
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
                var SP = _storeProduct.GetOne(e => e.ProductId == i.ProductId && e.StoreId == i.FromStoreId);
                var s = _store.GetOne(e => e.StoreId == i.FromStoreId);
                var p = _product.GetOne(e => e.ProductId == i.ProductId);
                SP.Quantity = SP.Quantity + i.Quantity;
                p.Quantity = p.Quantity + i.Quantity;
                _operationStoreProduct.Delete(i.OperationStoreProductId);
                _storeProduct.Edit(SP);
                _product.Edit(p);
            }
            var oldCustomer= _customer.GetOne(e=>e.CustomerSupplierId==operation.CustomerSupplierId);
            oldCustomer.NumOfDeal--;
            _customer.Edit(oldCustomer);
            _customer.commit();
            operation.CustomerSupplierId = operationVM.CustomerId;
            _storeProduct.commit();
            _product.commit();
            _operationStoreProduct.commit();
            operation.TotalPrice = 0;
            foreach (var sp in operationVM.SP)
            {
                var product = _product.GetOne(e => e.ProductId == sp.ProductId && e.IsDeleted == false);
                var store = _store.GetOne(e => e.StoreId == sp.FromStoreId && e.IsDeleted == false);
                var storeHasProduct = _storeProduct.GetOne(e => e.StoreId == sp.FromStoreId && e.ProductId == sp.ProductId);
                if (storeHasProduct != null && storeHasProduct.Quantity >= sp.Quantity)
                {
                    storeHasProduct.Quantity -= sp.Quantity;
                }
                else if (storeHasProduct.Quantity < sp.Quantity)
                {
                    res.Meesage = $"كمية المنتج {product.Name} غير كافيه فى مخزن {store.Name}";
                    return BadRequest(res);
                }
                else
                {
                    res.Meesage = $"لا يوجد منتجات من نوع {product.Name} فى مخزن {store.Name}";
                    return BadRequest(res);
                }
                product.Quantity = product.Quantity - sp.Quantity;
                var OSP = new OperationStoreProduct() { OperationId = id, FromStoreId = sp.FromStoreId, ProductId = sp.ProductId, Quantity = sp.Quantity };
                _operationStoreProduct.Create(OSP);
                _product.Edit(product);
                operation.TotalPrice = operation.TotalPrice + (sp.Quantity * product.SellingPrice);
            }
            customer.NumOfDeal++;
            _customer.Edit(customer);
            _customer.commit();
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
            var operation = _operation.GetOne(e => e.OperationId == id && e.IsDeleted == false && e.LookupOperationTypeId == StaticData.ExportOperation);
            if (operation == null)
            {
                res.Meesage = "لم يتم العثور على هذه العمليه";
                return BadRequest(res);
            }
            var customer = _customer.GetOne(e => e.CustomerSupplierId == operation.CustomerSupplierId && e.IsDeleted == false && e.LookupCustomerSupplierTypeId == 2);
            if (customer == null)
            {
                res.Meesage = "لم يتم العثور على عميل هذه العمليه";
                return BadRequest(res);
            }
            var operationStoreProduct = _operationStoreProduct.GetAllIds(id);
            foreach (var osp in operationStoreProduct)
            {
                var store = _store.GetOne(e => e.StoreId == osp.ToStoreId && e.IsDeleted == false);
                var product = _product.GetOne(e => e.ProductId == osp.ProductId && e.IsDeleted == false);
                var storeProduct = _storeProduct.GetOne(e => e.ProductId == osp.ProductId && e.StoreId == osp.FromStoreId);
                storeProduct.Quantity = storeProduct.Quantity + osp.Quantity;
                product.Quantity = product.Quantity + osp.Quantity;
                osp.IsDeleted = true;
                _storeProduct.Edit(storeProduct);
                _product.Edit(product);
                _operationStoreProduct.Edit(osp);
            }
            customer.NumOfDeal--;
            _customer.Edit(customer);
            _customer.commit();
            operation.IsDeleted = true;
            _operation.Edit(operation);
            _storeProduct.commit();
            _product.commit();
            _operationStoreProduct.commit();
            _operation.commit();
            res.IsSuccess = true;
            return Ok(res);
        }
    }
}
