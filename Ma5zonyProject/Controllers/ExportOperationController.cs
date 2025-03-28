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
            var opsIds = _operationStoreProduct.GetAllIds(id);
            foreach (var i in opsIds)
            {
                var p = _product.GetOne(e => e.ProductId == i.ProductId && e.IsDeleted == false);
                var sp = _storeProduct.GetOne(e => e.ProductId == i.ProductId && e.StoreId == i.FromStoreId);
                p.Quantity = p.Quantity + i.Quantity;
                sp.Quantity = sp.Quantity + i.Quantity;
                _product.Edit(p);
                _storeProduct.Edit(sp);
                _operationStoreProduct.Delete(i.OperationStoreProductId);
            }
            operation.TotalPrice = 0;
            foreach (var i in operationVM.SP)
            {
                var p = _product.GetOne(e => e.ProductId == i.ProductId && e.IsDeleted == false);
                if (p == null)
                {
                    res.Meesage ="يرجى ادخال المنتجات بشكل صحيح";
                    return BadRequest(res);
                }
                var s = _store.GetOne(e => e.StoreId == i.FromStoreId && e.IsDeleted == false);
                if (s == null)
                {
                    res.Meesage = "يرجى ادخال المخازن بشكل صحيح";
                    return BadRequest(res);
                }
                var sp = _storeProduct.GetOne(e => e.ProductId == i.ProductId && e.StoreId == i.FromStoreId);
                if (sp == null)
                {
                    res.Meesage = $"المخزن {s.Name} لا يحتوى على منتج من نوع {p.Name}";
                    return BadRequest(res);
                }
                else if (sp.Quantity < i.Quantity)
                {
                    res.Meesage = $"كمية المنتج {p.Name} غير كافيه فى مخزن {s.Name}";
                    return BadRequest(res);
                }
                var ops=new OperationStoreProduct { OperationId=operation.OperationId, ProductId=i.ProductId,FromStoreId=i.FromStoreId,Quantity=i.Quantity};
                p.Quantity = p.Quantity - i.Quantity;
                sp.Quantity = sp.Quantity - i.Quantity;
                _operationStoreProduct.Create(ops);
                _product.Edit(p);
                _storeProduct.Edit(sp);
                operation.TotalPrice = (i.Quantity * p.SellingPrice) + operation.TotalPrice;
            }
            var oldCustomer=_customer.GetOne(e=>e.CustomerSupplierId==operation.CustomerSupplierId);
            operation.CustomerSupplierId = operationVM.CustomerId;
            oldCustomer.NumOfDeal--;
            _customer.Edit(oldCustomer);
            customer.NumOfDeal++;
            _customer.Edit(customer);
            _operation.Edit(operation);
            _operation.commit();
            _product.commit();
            _storeProduct.commit();
            _operationStoreProduct.commit();
            _customer.commit();
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
            var customer = _customer.GetOne(e => e.CustomerSupplierId == operation.CustomerSupplierId && e.LookupCustomerSupplierTypeId == 2);
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
