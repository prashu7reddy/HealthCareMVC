using HealthCareMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HealthCareMVC.Controllers
{
    public class DoctorController : Controller
    {

        private readonly IConfiguration _configuration;
        public DoctorController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        //private readonly string ApiUrl = "https://localhost:44350/api/";

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<DoctorViewModel> doctors = new ();
            using (var client = new HttpClient())
            {
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));

                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync("Doctors/GetAllDoctors");
                if (result.IsSuccessStatusCode)
                {
                    doctors = await result.Content.ReadAsAsync<List<DoctorViewModel>>();
                }
            }
                return View(doctors);
        }
        [HttpGet]
        public async Task<IActionResult>Details(int id)
        {
            DoctorViewModel doctor = null;
            using(var client = new HttpClient())
            {
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));

                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync($"Doctors/GetDoctorById/{id}");
                if (result.IsSuccessStatusCode)
                {
                    doctor = await result.Content.ReadAsAsync<DoctorViewModel>();
                }
            }
            return View(doctor);
        }
        [HttpGet]
        [Route("Doctors/Create")]
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost("Doctors/Create")]
        public async Task<IActionResult> Create(DoctorViewModel doctor)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                   //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                    client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);

                    var result = await client.PostAsJsonAsync("Doctors/CreateDoctor", doctor);
                    if (result.StatusCode == System.Net.HttpStatusCode.Created)
                    {

                        return RedirectToAction("index", "Doctor");
                    }
                }
            }

            return View(doctor);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {
                DoctorViewModel doctor = null;
                using (var client = new HttpClient())
                {
                  //  client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                    client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);
                    var result = await client.GetAsync($"doctors/GetDoctorById/{id}");
                    if (result.IsSuccessStatusCode)
                    {
                        doctor = await result.Content.ReadAsAsync<DoctorViewModel>();
                        return View(doctor);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Doctor doesn't exist");
                    }

                }
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Edit( DoctorViewModel doctor)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);
                    var result = await client.PutAsJsonAsync($"Doctors/UpdateDoctor/{doctor.Id}", doctor);
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Server Error, Please try later");
                    }

                }
            }
            return View();
        }

        [HttpGet]

        public async Task<IActionResult> Delete(int id)
        {
            DoctorViewModel doctor = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync($"Doctors/GetDoctorById/{id}");
                if (result.IsSuccessStatusCode)
                {
                    doctor = await result.Content.ReadAsAsync<DoctorViewModel>();
                    return View(doctor);
                }
                else
                {
                    ModelState.AddModelError("", "Server Error.Please try later");
                }
            }
            return View(doctor);
        }
        
       [HttpPost("Doctors/Delete/{DoctorId}")]
        public async Task<IActionResult> Delete(DoctorViewModel doctor)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.DeleteAsync($"Doctors/DeleteDoctor/{doctor.Id}");
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");

                }
                else
                {
                    ModelState.AddModelError("", "Server Error.Please try later");
                }
            }
            return View();

        }

    }
}
