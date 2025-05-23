﻿using DataAccess.IRepos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Models.ViewModels;
using System.Security.Claims;
using Utility;

namespace Ma5zonyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private CustomerSupplierIRepo _supplier;
        private UserManager<ApplicationUser> _userManager;
        private CustomerSupplierLogIRepo _log;
        public SupplierController(CustomerSupplierIRepo supplier, CustomerSupplierLogIRepo log, UserManager<ApplicationUser> userManager)
        {
            _supplier = supplier;
            _log = log;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 5, string? name = null, int? age = null, string? address = null, int? numOfDeal = null, bool? isReliable = null, string? phoneNum = null, string? email = null)
        {
            var res = new Result<List<CustomersSuppliersVM>>();
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
            var suppliers = _supplier.GetAll(pageNumber: pageNumber, pageSize: pageSize, filters: filters, expression: s => s.IsDeleted == false && s.LookupCustomerSupplierTypeId == 1);
            res.Data = suppliers.Data?.Select(s => new CustomersSuppliersVM
            {
                CustomerSupplierId = s.CustomerSupplierId,
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
        public async Task<IActionResult> Create(CustomerSupplierCreateVM supplierVM)
        {
            var res = new Result<CustomerSupplierCreateVM>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted == true)
            {
                res.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(res);
            }
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
            var newSupplier = new CustomerSupplier()
            {
                Name = supplierVM.Name,
                Email = supplierVM.Email,
                Age = supplierVM.Age,
                NumOfDeal = 0,
                Address = supplierVM.Address,
                PhoneNumber = supplierVM.PhoneNumber,
                IsReliable = supplierVM.IsReliable,
                LookupCustomerSupplierTypeId = 1
            };
            _supplier.Create(newSupplier);
            _supplier.commit();
            _log.CreateOperationLog(null, newSupplier, StaticData.AddOperationType, userId);
            res.IsSuccess = true;
            return Ok(res);
        }
        [HttpPut("edit")]
        public async Task<IActionResult> Edit(CustomerSupplierEditVM supplierVM)
        {
            var res = new Result<CustomerSupplierCreateVM>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted == true)
            {
                res.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(res);
            }
            if (!ModelState.IsValid)
            {
                res.Meesage = "يرجى ادخال بيانات المنتج بشكل صحيح";
                return BadRequest(res);
            }
            var supplier = _supplier.GetOne(e => e.CustomerSupplierId == supplierVM.CustomerSupplierId && e.IsDeleted == false && e.LookupCustomerSupplierTypeId == 1);
            if (supplier == null)
            {
                res.Meesage = "لم يتم العثور على هذا المورد";
                return BadRequest(res);
            }
            if (supplierVM.Age < 18)
            {
                res.Meesage = "عمر المورد يجب ان يكون اكبر من 17 سنه";
                return BadRequest(res);
            }
            var chekIfNameExists = _supplier.GetOne(e => e.Name == supplierVM.Name && e.CustomerSupplierId != supplierVM.CustomerSupplierId && e.LookupCustomerSupplierTypeId == 1);
            if (chekIfNameExists != null)
            {
                res.Meesage = "اسم المورد موجود بالفعل";
                return BadRequest(res);
            }
            var chekIfEmailExists = _supplier.GetOne(e => e.Email == supplierVM.Email && e.CustomerSupplierId != supplierVM.CustomerSupplierId && e.LookupCustomerSupplierTypeId == 1);
            if (chekIfEmailExists != null)
            {
                res.Meesage = "البريد الالكترونى موجود بالفعل";
                return BadRequest(res);
            }
            var oldSupplier = new CustomerSupplier
            {
                Address = supplier.Address,
                Age = supplier.Age,
                Email = supplier.Email,
                IsReliable = supplier.IsReliable,
                LookupCustomerSupplierTypeId = supplier.LookupCustomerSupplierTypeId,
                Name = supplier.Name,
                PhoneNumber = supplier.PhoneNumber,
                CustomerSupplierId = supplier.CustomerSupplierId,
            };
            supplier.Name = supplierVM.Name;
            supplier.Email = supplierVM.Email;
            supplier.Address = supplierVM.Address;
            supplier.PhoneNumber = supplierVM.PhoneNumber;
            supplier.Age = supplierVM.Age;
            supplier.IsReliable = supplierVM.IsReliable;
            _supplier.Edit(supplier);
            _supplier.commit();
            res.IsSuccess = true;
            _log.CreateOperationLog(oldSupplier, supplier, StaticData.EditOperationType, userId);
            return Ok(res);
        }
        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var res = new Result<CustomerSupplierVM>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted == true)
            {
                res.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(res);
            }
            var supplier = _supplier.GetOne(e => e.CustomerSupplierId == id && e.IsDeleted == false && e.LookupCustomerSupplierTypeId == 1);
            if (supplier == null)
            {
                res.Meesage = "لم يتم العثور على هذا المورد";
                return BadRequest(res);
            }
            var supplierVM = new CustomerSupplierVM()
            {
                CustomerSupplierId = supplier.CustomerSupplierId,
                Name = supplier.Name,
                Email = supplier.Email,
                Address = supplier.Address,
                Age = supplier.Age,
                IsReliable = supplier.IsReliable,
                PhoneNumber = supplier.PhoneNumber,
                NumOfDeal = supplier.NumOfDeal
            };
            res.Data = supplierVM;
            res.IsSuccess = true;
            return Ok(res);
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var res = new Result<CustomerSupplierVM>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted == true)
            {
                res.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(res);
            }
            var supplier = _supplier.GetOne(e => e.CustomerSupplierId == id && e.IsDeleted == false && e.LookupCustomerSupplierTypeId == 1);
            if (supplier == null)
            {
                res.Meesage = "لم يتم العثور على هذا المورد";
                return BadRequest(res);
            }
            supplier.IsDeleted = true;
            _supplier.Edit(supplier);
            _supplier.commit();
            res.IsSuccess = true;
            _log.CreateOperationLog(supplier, null, StaticData.DeleteOperationType, userId);
            return Ok(res);
        }
        [HttpGet("get-suppliers-for-operation")]
        public async Task<IActionResult> GetSuppliersForOperation()
        {
            var res = new Result<List<SupplierOrCustomerForOperation>>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted == true)
            {
                res.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(res);
            }
            var suppliers = _supplier.GetSuppliersOrCustomersForOperation(1);
            res.Data = suppliers;
            res.IsSuccess = true;
            return Ok(res);
        }
    }
}