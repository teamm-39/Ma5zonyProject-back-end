using DataAccess.IRepos;
using Microsoft.AspNetCore.Authorization;
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
    public class CustomerController : ControllerBase
    {
        private CustomerSupplierIRepo _customer;
        private UserManager<ApplicationUser> _userManager;
        private CustomerSupplierLogIRepo _log;
        public CustomerController(CustomerSupplierIRepo customer, CustomerSupplierLogIRepo log, UserManager<ApplicationUser> userManager)
        {
            _customer = customer;
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
            var customers = _customer.GetAll(pageNumber: pageNumber, pageSize: pageSize, filters: filters, expression: s => s.IsDeleted == false && s.LookupCustomerSupplierTypeId == 2);
            res.Data = customers.Data?.Select(s => new CustomersSuppliersVM
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
            res.Total = customers.Total;
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
                res.Meesage = "عمر العميل يجب ان يكون اكبر من 17 سنه";
                return BadRequest(res);
            }
            var chekIfNameExists = _customer.GetOne(e => e.Name == supplierVM.Name);
            if (chekIfNameExists != null)
            {
                res.Meesage = "اسم العميل موجود بالفعل";
                return BadRequest(res);
            }
            var chekIfEmailExists = _customer.GetOne(e => e.Email == supplierVM.Email);
            if (chekIfEmailExists != null)
            {
                res.Meesage = "البريد الالكترونى موجود بالفعل";
                return BadRequest(res);
            }
            var newCustomer = new CustomerSupplier()
            {
                Name = supplierVM.Name,
                Email = supplierVM.Email,
                Age = supplierVM.Age,
                NumOfDeal = 0,
                Address = supplierVM.Address,
                PhoneNumber = supplierVM.PhoneNumber,
                IsReliable = supplierVM.IsReliable,
                LookupCustomerSupplierTypeId = 2
            };
            _customer.Create(newCustomer);
            _customer.commit();
            res.IsSuccess = true;
            _log.CreateOperationLog(null, newCustomer, StaticData.AddOperationType, userId);
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
            var supplier = _customer.GetOne(e => e.CustomerSupplierId == id && e.IsDeleted == false && e.LookupCustomerSupplierTypeId == 2);
            if (supplier == null)
            {
                res.Meesage = "لم يتم العثور على هذا العميل";
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
        [HttpPut("edit")]
        public async Task<IActionResult> Edit(CustomerSupplierEditVM customerVM)
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
            var customer = _customer.GetOne(e => e.CustomerSupplierId == customerVM.CustomerSupplierId && e.IsDeleted == false && e.LookupCustomerSupplierTypeId == 2);
            if (customer == null)
            {
                res.Meesage = "لم يتم العثور على هذا العميل";
                return BadRequest(res);
            }
            if (customerVM.Age < 18)
            {
                res.Meesage = "عمر العميل يجب ان يكون اكبر من 17 سنه";
                return BadRequest(res);
            }
            var chekIfNameExists = _customer.GetOne(e => e.Name == customerVM.Name && e.CustomerSupplierId != customerVM.CustomerSupplierId);
            if (chekIfNameExists != null)
            {
                res.Meesage = "اسم العميل موجود بالفعل";
                return BadRequest(res);
            }
            var chekIfEmailExists = _customer.GetOne(e => e.Email == customerVM.Email && e.CustomerSupplierId != customerVM.CustomerSupplierId);
            if (chekIfEmailExists != null)
            {
                res.Meesage = "البريد الالكترونى موجود بالفعل";
                return BadRequest(res);
            }
            var oldCustomer = new CustomerSupplier
            {
                Age = customer.Age,
                Address = customer.Address,
                CustomerSupplierId = customer.CustomerSupplierId,
                Email = customer.Email,
                IsReliable = customer.IsReliable,
                LookupCustomerSupplierTypeId = customer.LookupCustomerSupplierTypeId,
                Name = customer.Name,
                PhoneNumber = customer.PhoneNumber
            };
            customer.Name = customerVM.Name;
            customer.Email = customerVM.Email;
            customer.Address = customerVM.Address;
            customer.PhoneNumber = customerVM.PhoneNumber;
            customer.Age = customerVM.Age;
            customer.IsReliable = customerVM.IsReliable;
            _customer.Edit(customer);
            _customer.commit();
            res.IsSuccess = true;
            _log.CreateOperationLog(oldCustomer, customer, StaticData.EditOperationType, userId);
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
            var customer = _customer.GetOne(e => e.CustomerSupplierId == id && e.IsDeleted == false && e.LookupCustomerSupplierTypeId == 2);
            if (customer == null)
            {
                res.Meesage = "لم يتم العثور على هذا العميل";
                return BadRequest(res);
            }
            customer.IsDeleted = true;
            _customer.Edit(customer);
            _customer.commit();
            res.IsSuccess = true;
            _log.CreateOperationLog(customer, null, StaticData.DeleteOperationType, userId);
            return Ok(res);
        }
        [HttpGet("get-customers-for-operation")]
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
            var customers = _customer.GetSuppliersOrCustomersForOperation(2);
            res.Data = customers;
            res.IsSuccess = true;
            return Ok(res);
        }
    }
}
