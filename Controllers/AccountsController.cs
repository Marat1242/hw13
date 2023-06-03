using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
// using Services.Extension;
using Supermarket.ModelViews;
// using Services.Helpper;
using Business;
using Business.Data;
using Services.Helpper;
using Services.Extension;
using Domain;

namespace Supermarket.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {

        private readonly DbMarketsContext _context;
        public INotyfService _notyfService { get; }
		private readonly ILogger<AccountsController> _logger;

		public AccountsController(ILogger<AccountsController> logger , DbMarketsContext context, INotyfService notyfService)
        {
			_logger = logger;
			_context = context;
            _notyfService = notyfService;
        }


		[HttpGet]
        [AllowAnonymous]
        public IActionResult ValidatePhone(string Phone)
        {
            try
            {
                var cust = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Phone.ToLower() == Phone.ToLower());
                if (cust != null)
                    return Json(data: "Phone number : " + Phone + "has been used");

                return Json(data: true);

            }
            catch
            {
                return Json(data: true);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateEmail(string Email)
        {
            try
            {
                var cust = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email.ToLower() == Email.ToLower());
                if (cust != null)
                    return Json(data: "Email : " + Email + " has been used");
                return Json(data: true);
            }
            catch
            {
                return Json(data: true);
            }
        }
        [Route("my-account.html", Name = "Dashboard")]
        public IActionResult Dashboard()
        {
            var accountID = HttpContext.Session.GetString("CustomerId");
            if (accountID != null)
            {
                var cust = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(accountID));
                if (cust != null)
                {
                    var lsOrder = _context.Orders
                        .Include(x => x.TransactStatus)
                        .AsNoTracking()
                        .Where(x => x.CustomerId == cust.CustomerId)
                        .OrderByDescending(x => x.OrderDate)
                        .ToList();
                    ViewBag.DonHang = lsOrder;
                    return View(cust);
                }

            }
            return RedirectToAction("Login");
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("register.html", Name = "Register")]
        public IActionResult RegisterAccount()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register.html", Name = "Register")]
        public async Task<IActionResult> RegisterAccount(RegisterVM account)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string salt = Utilities.GetRandomKey();
                    Customer cust = new Customer
                    {
                        FullName = account.FullName,
                        Phone = account.Phone.Trim().ToLower(),
                        Email = account.Email.Trim().ToLower(),
                        Address = account.Address.Trim().ToLower(),
                        Password = (account.Password + salt.Trim()).ToMD5(),
                        Active = true,
                        Salt = salt,
                        CreateDate = DateTime.Now
                    };
                    try
                    {
                        _context.Add(cust);
                        await _context.SaveChangesAsync();
                        //Lưu Session MaKh
                        HttpContext.Session.SetString("CustomerId", cust.CustomerId.ToString());
                        var accountID = HttpContext.Session.GetString("CustomerId");

                        //Identity
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name,cust.FullName),
                            new Claim("CustomerId", cust.CustomerId.ToString())
                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);
                        _notyfService.Success("Sign Up Success");
                        return RedirectToAction("Dashboard", "Accounts");
                    }
                    catch
                    {
                        return RedirectToAction("RegisterAccount", "Accounts");
                    }
                }
                else
                {
                    return View(account);
                }
            }
            catch
            {
                return View(account);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("login.html", Name = "Login")]
        public IActionResult Login(string returnUrl = null)
        {
            var accountID = HttpContext.Session.GetString("CustomerId");
            if (accountID != null)
            {
                return RedirectToAction("Dashboard", "Accounts");
            }
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("login.html", Name = "Login")]
        public async Task<IActionResult> Login(LoginVM customer, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isEmail = Utilities.IsValidEmail(customer.UserName);
                    if (!isEmail) return View(customer);

                    var cust = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email.Trim() == customer.UserName);

                    if (cust == null) return RedirectToAction("RegisterAccount");
                    string pass = (customer.Password + cust.Salt.Trim()).ToMD5();
                    if (cust.Password != pass)
                    {
                        _notyfService.Success("Login information is incorrect");
                        return View(customer);
                    }
                    //kiem tra xem account co bi disable hay khong

                    if (cust.Active == false)
                    {
                        return RedirectToAction("Notification", "Accounts");
                    }

                    //Luu Session MaKh
                    HttpContext.Session.SetString("CustomerId", cust.CustomerId.ToString());
                    var accountID = HttpContext.Session.GetString("CustomerId");

                    //Identity
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, cust.FullName),
                        new Claim("CustomerId", cust.CustomerId.ToString())
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    _notyfService.Success("Logged in successfully");
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToAction("Dashboard", "Accounts");
                    }           
                }


				else
				{
					return Redirect(returnUrl);
				//	return View(customer);
				}
			}

			//catch (Exception ex)
			//{
			//	// Log the exception
			//	_logger.LogError(ex, "Error occurred while logging in");

			//	// Redirect to an error page or show a generic error message
			//	return RedirectToAction("Error", "Home");
			//}


            catch
            {
                return RedirectToAction("RegisterAccount", "Accounts");
            }

            return View(customer);
        } 

		

		    [HttpGet]
            [Route("log-out.html", Name = "LogOut")]
            public IActionResult Logout()
            {
                HttpContext.SignOutAsync();
                HttpContext.Session.Remove("CustomerId");
                return RedirectToAction("Index", "Home");
            }

            [HttpPost]
            public IActionResult ChangePassword(ChangePasswordViewModel model)
            {
                try
                {
                    var accountID = HttpContext.Session.GetString("CustomerId");
                    if (accountID == null)
                    {
                        return RedirectToAction("Login", "Accounts");
                    }
                    if (ModelState.IsValid)
                    {
                        var account = _context.Customers.Find(Convert.ToInt32(accountID));
                        if (account == null) return RedirectToAction("Login", "Accounts");
                        var pass = (model.PasswordNow.Trim() + account.Salt.Trim()).ToMD5();
                        {
                            string passnew = (model.Password.Trim() + account.Salt.Trim()).ToMD5();
                            account.Password = passnew;
                            _context.Update(account);
                            _context.SaveChanges();
                            _notyfService.Success("Change password successfully");
                            return RedirectToAction("Dashboard", "Accounts");
                        }
                    }
                }
                catch
                {
                    _notyfService.Success("Password change failed");
                    return RedirectToAction("Dashboard", "Accounts");
                }
                _notyfService.Success("Password change failed");
                return RedirectToAction("Dashboard", "Accounts");
            }
        
    }
}
