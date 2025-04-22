using DataAccess.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Models.ViewModels;
using Utility;

namespace Ma5zonyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreProductController : ControllerBase
    {
        private StoreProductIRepo _storeProduct;
        private StoreIRepo _store;
        private ProductIRepo _product;
        public StoreProductController(StoreProductIRepo storeProductI,StoreIRepo store,ProductIRepo product)
        {
            _storeProduct = storeProductI;
            _store = store;
            _product = product;
        }
        [HttpGet("get-products-for-store/{storeId}")]
        public IActionResult GetProductsForStore(int storeId, int pageNumber = 1, int pageSize = 5)
        {
            var res=new Result<List<ProductsVM>>();
            var store=_store.GetOne(e=>e.StoreId==storeId&&e.IsDeleted==false);
            if (store == null)
            {
                res.Meesage = "لم يتم العثور على هذا المخزن";
                return BadRequest(res);
            }
            var sp = _storeProduct.GetAll(expression: e => e.StoreId == storeId, pageSize: pageSize, pageNumber: pageNumber, includes: [e => e.Product]);
            var products = sp.Data.Select(
                e => new ProductsVM
                {
                    ProductId = e.StoreId,
                    Name = e.Product.Name,
                    SellingPrice = e.Product.SellingPrice,
                    PurchasePrice = e.Product.PurchasePrice,
                    MinLimit = e.Product.MinLimit,
                    Quantity = e.Product.Quantity
                }).ToList();
            res.IsSuccess = true;
            res.Data = products;
            res.PageSize = pageSize;
            res.PageNumber=pageNumber;
            res.Total = sp.Total;
            return Ok(res);
        }
        [HttpGet("get-Stores-for-product/{productId}")]
        public IActionResult GetStoresForProduct(int productId, int pageNumber = 1, int pageSize = 5)
        {
            var res = new Result<List<StoreForProductVM>>();
            var product = _product.GetOne(e => e.ProductId == productId && e.IsDeleted == false);
            if (product == null) {
                res.Meesage = "لم يتم العثور على هذا المنتج";
                return BadRequest(res);
            }
            var sp = _storeProduct.GetAll(expression: e => e.ProductId == productId, pageNumber: pageNumber, pageSize: pageSize, includes: [e=>e.Store,e=>e.Product]);
            var stores = sp.Data.Select(e => new StoreForProductVM { StoreId = e.StoreId, Name = e.Store.Name, Country = e.Store.Country, City = e.Store.City,ProductQuantity=e.Quantity }).ToList();
            res.IsSuccess = true;
            res.Data = stores;
            res.PageSize = pageSize;
            res.PageNumber = pageNumber;
            res.Total = sp.Total;
            return Ok(res);
        }

    }
}