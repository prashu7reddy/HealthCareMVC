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
    public class AdminController : Controller
    {
        private readonly IConfiguration _configuration;
        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Doctors()
        {
            List<DoctorViewModel> doctors = new();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));

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
        public async Task<IActionResult> Appointment()
        {
            List<AppointmentBookingViewModel> appointments = new();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));

                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync("AppointmentBooking/GetAllBookings");
                if (result.IsSuccessStatusCode)
                {
                    appointments = await result.Content.ReadAsAsync<List<AppointmentBookingViewModel>>();
                }
            }
            return View(appointments);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            DoctorViewModel doctor = null;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));

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
        [Route("Admin/DocCreate")]
        public async Task<IActionResult> DocCreate()
        {
            DoctorViewModel viewModel = new DoctorViewModel
            {
                Specializations = await this.GetSpecializations()
            };
            return View(viewModel);
        }

        //private Task<List<DocSpecializationModel>> GetSpecializations()
        //{
        //    throw new NotImplementedException();
        //}

        [HttpPost("Admin/DocCreate")]
        public async Task<IActionResult> DocCreate(DoctorViewModel doctor)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                    client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);

                    var result = await client.PostAsJsonAsync("Doctors/CreateDoctor", doctor);
                    if (result.StatusCode == System.Net.HttpStatusCode.Created)
                    {

                        return RedirectToAction("Doctors", "Admin");
                    }
                }
            }
            DoctorViewModel viewModel = new DoctorViewModel
            {
                Specializations = await this.GetSpecializations()
            };
            return View(viewModel);

        }
        [NonAction]
        public async Task<List<DocSpecializationModel>> GetSpecializations()
        {
            List<DocSpecializationModel> specializations = new();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync("Doctors/GetSpecializations");
                if (result.IsSuccessStatusCode)
                {
                    specializations = await result.Content.ReadAsAsync<List<DocSpecializationModel>>();
                }
            }
            return specializations;
        }
        [HttpGet]
        public async Task<IActionResult> AppointmentDetails(int id)
        {
            AppointmentBookingViewModel appointment = null;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));

                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync($"AppointmentBooking/GetAppointmentById/{id}");
                if (result.IsSuccessStatusCode)
                {
                    appointment = await result.Content.ReadAsAsync<AppointmentBookingViewModel>();
                }
            }
            return View(appointment);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {
                AppointmentBookingViewModel appointment = null;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                    client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);
                    var result = await client.GetAsync($"AppointmentBooking/GetAppointmentById/{id}");
                    if (result.IsSuccessStatusCode)
                    {
                        appointment = await result.Content.ReadAsAsync<AppointmentBookingViewModel>();
                        //appointment.Specializations = await this.GetSpecializations();
                        return View(appointment);
                    }
                    else
                    {
                        ModelState.AddModelError("", "appointment doesn't exist");
                    }

                }
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(AppointmentBookingViewModel appointment)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);
                    var result = await client.PutAsJsonAsync($"AppointmentBooking/UpdateAppointment/{appointment.Id}", appointment);
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Appointment","Admin");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Server Error, Please try later");
                    }

                }
            }
            return View();
        }

    }
}
