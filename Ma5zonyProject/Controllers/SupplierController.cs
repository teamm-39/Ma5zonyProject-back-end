using DataAccess.IRepos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Models.ViewModels;
using Utility;

namespace Ma5zonyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private SupplierIRepo _supplier;

        public SupplierController(SupplierIRepo supplier)
        {
            _supplier = supplier;
        }
        [HttpGet]
        public IActionResult GetAll(int pageNumber = 1, int pageSize = 5, string? name = null, int? age = null, string? address = null, int? numOfDeal = null, bool? isReliable = null, string? phoneNum = null, string? email = null)
        {
            var res = new Result<List<SuppliersVM>>();
            if (pageSize <= 0 || pageNumber <= 0)
            {
                res.Meesage = "رقم الصفحة وعدد العناصر يجب أن يكونا أكبر من الصفر";
                return BadRequest(res);
            }
            if (age.HasValue && age <= 0)
            {
                res.Meesage = "العمر يجب ان يكون اكبر من الصفر";
                return BadRequest(res);
            }
            if (numOfDeal.HasValue && numOfDeal < 0)
            {
                res.Meesage = "عدد المعاملات يجب أن يكون أكبر من أو يساوي الصفر";
                return BadRequest(res);
            }
            var filters = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(name)) filters.Add("Name", name);
            if (!string.IsNullOrWhiteSpace(address)) filters.Add("Address", address);
            if (!string.IsNullOrWhiteSpace(email)) filters.Add("Email", email);
            if (!string.IsNullOrWhiteSpace(phoneNum)) filters.Add("PhoneNumber", phoneNum);
            if (isReliable.HasValue) filters.Add("IsReliable", isReliable.Value);
            if (age.HasValue && age > 0) filters.Add("Age", age);
            if (numOfDeal.HasValue && numOfDeal >= 0) filters.Add("NumOfDeal", numOfDeal);
            var suppliers = _supplier.GetAll(pageNumber: pageNumber, pageSize: pageSize, filters: filters, expression: s => s.IsDeleted == false);
            res.Data = suppliers.Data?.Select(s => new SuppliersVM
            {
                SupplierId = s.SupplierId,
                Name = s.Name,
                Address = s.Address,
                Age = s.Age,
                NumOfDeal = s.NumOfDeal,
                PhoneNumber = s.PhoneNumber,
                IsReliable = s.IsReliable,
                Email = s.Email,
            }).ToList();
            res.IsSuccess = true;
            res.Total = suppliers.Total;
            res.PageNumber = pageNumber;
            res.PageSize = pageSize;
            return Ok(res);
        }
        [HttpPost("create")]
        public IActionResult Create(SupplierCreateVM supplierVM)
        {
            var res = new Result<SupplierCreateVM>();
            if (!ModelState.IsValid)
            {
                res.Meesage = "يرجى ادخال بيانات المنتج بشكل صحيح";
                return BadRequest(res);
            }
            if (supplierVM.Age < 18)
            {
                res.Meesage = "عمر المورد يجب ان يكون اكبر من 17 سنه";
                return BadRequest(res);
            }
            var chekIfNameExists = _supplier.GetOne(e => e.Name == supplierVM.Name);
            if (chekIfNameExists != null)
            {
                res.Meesage = "اسم المورد موجود بالفعل";
                return BadRequest(res);
            }
            var chekIfEmailExists = _supplier.GetOne(e => e.Email == supplierVM.Email);
            if (chekIfEmailExists != null)
            {
                res.Meesage = "البريد الالكترونى موجود بالفعل";
                return BadRequest(res);
            }
            var newSupplier = new Supplier()
            {
                Name = supplierVM.Name,
                Email = supplierVM.Email,
                Age = supplierVM.Age,
                NumOfDeal = 0,
                Address = supplierVM.Address,
                PhoneNumber = supplierVM.PhoneNumber,
                IsReliable = supplierVM.IsReliable
            };
            _supplier.Create(newSupplier);
            _supplier.commit();
            res.IsSuccess = true;
            return Ok(res);
        }
        [HttpPut("edit")]
        public IActionResult Edit(SupplierEditVM supplierVM)
        {
            var res = new Result<SupplierCreateVM>();
            if (!ModelState.IsValid)
            {
                res.Meesage = "يرجى ادخال بيانات المنتج بشكل صحيح";
                return BadRequest(res);
            }
            var oldSupplier = _supplier.GetOne(e => e.SupplierId == supplierVM.SupplierId && e.IsDeleted == false);
            if (oldSupplier == null)
            {
                res.Meesage = "لم يتم العثور على هذا المورد";
                return BadRequest(res);
            }
            if (supplierVM.Age < 18)
            {
                res.Meesage = "عمر المورد يجب ان يكون اكبر من 17 سنه";
                return BadRequest(res);
            }
            var chekIfNameExists = _supplier.GetOne(e => e.Name == supplierVM.Name && e.SupplierId != supplierVM.SupplierId);
            if (chekIfNameExists != null)
            {
                res.Meesage = "اسم المورد موجود بالفعل";
                return BadRequest(res);
            }
            var chekIfEmailExists = _supplier.GetOne(e => e.Email == supplierVM.Email && e.SupplierId != supplierVM.SupplierId);
            if (chekIfEmailExists != null)
            {
                res.Meesage = "البريد الالكترونى موجود بالفعل";
                return BadRequest(res);
            }
            oldSupplier.Name = supplierVM.Name;
            oldSupplier.Email = supplierVM.Email;
            oldSupplier.Address = supplierVM.Address;
            oldSupplier.PhoneNumber = supplierVM.PhoneNumber;
            oldSupplier.Age = supplierVM.Age;
            oldSupplier.IsReliable = supplierVM.IsReliable;
            _supplier.Edit(oldSupplier);
            _supplier.commit();
            res.IsSuccess = true;
            return Ok(res);
        }
        [HttpGet("details/{id}")]
        public IActionResult GetOne(int id)
        {
            var res = new Result<SupplierVM>();
            var supplier = _supplier.GetOne(e => e.SupplierId == id && e.IsDeleted == false);
            if (supplier == null)
            {
                res.Meesage = "لم يتم العثور على هذا المورد";
                return BadRequest(res);
            }
            var supplierVM = new SupplierVM() { 
                SupplierId = supplier.SupplierId,
                Name = supplier.Name,
                Email = supplier.Email,
                Address = supplier.Address,
                Age = supplier.Age,
                IsReliable = supplier.IsReliable,
                PhoneNumber = supplier.PhoneNumber,
                NumOfDeal = supplier.NumOfDeal
            };
            res.Data = supplierVM;
            res.IsSuccess= true;
            return Ok(res);
        }
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id) { 
        var res=new Result<SupplierVM>();
            var supplier=_supplier.GetOne(e=>e.SupplierId==id&&e.IsDeleted== false);
            if (supplier == null) {
                res.Meesage = "لم يتم العثور على هذا المورد";
                return BadRequest(res);
            }
            _supplier.Delete(id);
            _supplier.commit();
            res.IsSuccess= true;
            return Ok(res);
        }
        [HttpGet("get-suppliers-for-operation")]
        public IActionResult GetSuppliersForOperation()
        {
            var res=new Result<List<SupplierForOperation>>();
            var suppliers = _supplier.GetSuppliersForOperation();
            res.Data = suppliers;
            res.IsSuccess = true;
            return Ok(res);
        }
    }
}