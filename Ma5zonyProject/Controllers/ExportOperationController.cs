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
        private StoreProductIRepo _storeProduct;
        public ExportOperationController(OperationIRepo operation, OperationStoreProductIRepo operationStoreProduct, StoreIRepo store, ProductIRepo product, StoreProductIRepo storeProduct)
        {
            _operation = operation;
            _operationStoreProduct = operationStoreProduct;
            _store = store;
            _product = product;
            _storeProduct = storeProduct;
        }





        //[HttpPost("create")]
        //public IActionResult Create(OperationStoreProductCreateVM operationStoreProductCreateVM)
        //{
        //    var res = new Result<Operation>();
        //    if (!ModelState.IsValid)
        //    {
        //        res.Meesage = "يرجى ادخال البيانات بشكل صحيح";
        //        return BadRequest(res);
        //    }
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (userId == null)
        //    {
        //        res.Meesage = "يرجى تسجيل الدخول اولا";
        //        return BadRequest(res);
        //    }
        //    var getStore = _store.GetOne(e => e.StoreId == operationStoreProductCreateVM.ToStoreId && e.IsDeleted == false);
        //    if (getStore == null)
        //    {
        //        res.Meesage = "لم يتم العثور على المخزن";
        //        return BadRequest(res);
        //    }
        //    var getProduct = _product.GetOne(e => e.ProductId == operationStoreProductCreateVM.ProductId && e.IsDeleted == false);
        //    if (getProduct == null)
        //    {
        //        res.Meesage = "لم يتم العثور على المنتج";
        //        return BadRequest(res);
        //    }
        //    var operationId = _operation.CreateOperation(StaticData.ImportOperation, userId, operationStoreProductCreateVM.SupplierOrCustomerId);
        //    _operation.commit();
        //    var createOperationProductStore = new OperationStoreProduct()
        //    {
        //        OperationId = operationStoreProductCreateVM.OperationId,
        //        ProductId = operationStoreProductCreateVM.ProductId,
        //        ToStoreId = operationStoreProductCreateVM.ToStoreId,
        //        Quantity = operationStoreProductCreateVM.Quantity,
        //    };
        //    getProduct.Quantity -= operationStoreProductCreateVM.Quantity;
        //    _product.Edit(getProduct);
        //    var getStoreProduct = _storeProduct.GetOne(e => e.ProductId == operationStoreProductCreateVM.ProductId && e.StoreId == operationStoreProductCreateVM.ToStoreId && e.IsDeleted == false);
        //    if (getStoreProduct == null)
        //    {
        //        var storeProduct = new StoreProducts() { ProductId = operationStoreProductCreateVM.ProductId, StoreId = operationStoreProductCreateVM.ToStoreId, Quantity = operationStoreProductCreateVM.Quantity };
        //        _storeProduct.Create(storeProduct);
        //        _storeProduct.commit();
        //    }
        //    else
        //    {
        //        getStoreProduct.Quantity -= operationStoreProductCreateVM.Quantity;
        //        _storeProduct.Edit(getStoreProduct);
        //    }
        //    _operationStoreProduct.Create(createOperationProductStore);
        //    _operationStoreProduct.commit();
        //    res.IsSuccess = true;
        //    return Ok(res);
        //}
    }
}
