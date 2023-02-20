using HealthCareMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Threading.Tasks;


namespace HealthCareMVC.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IConfiguration _configuration;
        public AccountsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [NonAction]
        public async Task<string> ExtractRole()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                //  var result = await client.GetAsync("Accounts/GetName");                
                var roleResult = await client.GetAsync("Accounts/GetRole");
                if (roleResult.IsSuccessStatusCode)
                {
                    // var name = await result.Content.ReadAsAsync<string>();
                    // ViewBag.Name = name;                    
                    var role = await roleResult.Content.ReadAsAsync<string>();
                    // ViewBag.Role = role; 
                    return role;
                }
                return null;
            }
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                    var result = await client.PostAsJsonAsync("Accounts/Login", login);
                    if (result.IsSuccessStatusCode)
                    {
                        string token = await result.Content.ReadAsAsync<string>();
                        HttpContext.Session.SetString("token", token);

                        //string userName = login.Username;
                        //HttpContext.Session.SetString("Patient", userName);

                        // TempData["UserName"] = login.Username;
                        string role = await ExtractRole();
                        if (role == "Patient")
                        {

                            return RedirectToAction("Patient_Home", "Patient");
                        }
                        else if (role == "admin")
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        else if (role == "Doctor")
                        {
                            return RedirectToAction("Doctor_Home", "Doctor");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                       
                    }
                    ModelState.AddModelError("", "Invalid Username or Password");
                }
            }
            return View(login);
        }
      



        [HttpGet]
        public IActionResult SignUp()
        {
            RolesSignUpViewModel model = new RolesSignUpViewModel
            {
                Values = new List<SelectListItem>
                         {
                             new SelectListItem { Value = "Patient", Text = "Patient" },
                             new SelectListItem { Value = "Doctor", Text = "Doctor" }
                         }


            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SignUp([FromForm] RolesSignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();

                    client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);


                    SignUpViewModel user = new SignUpViewModel
                    {
                        UserName = model.SignUpRoles.UserName,
                        FullName = model.SignUpRoles.FullName,
                        Password = model.SignUpRoles.Password,
                        ConfirmPassword = model.SignUpRoles.ConfirmPassword,
                        Email = model.SignUpRoles.Email,
                        PhoneNumber = model.SignUpRoles.PhoneNumber,
                        Role = model.SelectedValue


                    };

                    var result = await client.PostAsJsonAsync("Accounts/Register", user);
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Login");
                    }

                }
            }
            SignUpViewModel user1 = new SignUpViewModel
            {
                UserName = model.SignUpRoles.UserName,
                FullName = model.SignUpRoles.FullName,
                Password = model.SignUpRoles.Password,
                Email = model.SignUpRoles.Email,
                PhoneNumber = model.SignUpRoles.PhoneNumber,
                Role = model.SelectedValue


            };
            RolesSignUpViewModel model1 = new RolesSignUpViewModel
            {
                SignUpRoles = user1,
                Values = new List<SelectListItem>
                         {
                             new SelectListItem { Value = "Patient", Text = "Patient" },
                             new SelectListItem { Value = "Doctor", Text = "Doctor" }
                         }


            };
            return View(model1);
        }


        [HttpPost]
        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("token");
            return RedirectToAction("Index", "Home");
        }



    }
}
