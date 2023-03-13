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
      
        public IActionResult Doctor_Home()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Index()
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
        [Route("Doctors/Create")]
        public async Task<IActionResult> Create()
        {
            DoctorViewModel viewModel = new DoctorViewModel
            {
                Specializations = await this.GetSpecializations()
            };
            return View(viewModel);
        }

        [HttpPost("Doctors/Create")]
        public async Task<IActionResult> Create(DoctorViewModel doctor)
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

                        return RedirectToAction("index", "Doctor");
                    }
                }
            }
            DoctorViewModel viewModel = new DoctorViewModel
            {
                Specializations = await this.GetSpecializations()
            };
            return View(viewModel);
           
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {
                DoctorViewModel doctor = null;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                    client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);
                    var result = await client.GetAsync($"doctors/GetDoctorById/{id}");
                    if (result.IsSuccessStatusCode)
                    {
                        doctor = await result.Content.ReadAsAsync<DoctorViewModel>();
                        doctor.Specializations = await this.GetSpecializations();
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
        public async Task<IActionResult> Edit(DoctorViewModel doctor)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);
                    var result = await client.PutAsJsonAsync($"Doctors/UpdateDoctor/{doctor.Id}", doctor);
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Doctors","Admin");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Server Error, Please try later");
                    }

                }
            }
            DoctorViewModel viewModel = new DoctorViewModel
            {
                Specializations = await this.GetSpecializations()
            };
            return View(viewModel);
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

        [HttpPost]
        public async Task<IActionResult> Delete(DoctorViewModel doctor)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.DeleteAsync($"Doctors/DeleteDoctor/{doctor.Id}");
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Doctors","Admin");

                }
                else
                {
                    ModelState.AddModelError("", "Server Error.Please try later");
                }
            }
            return View();

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
        public async Task<IActionResult> Appointment()
        {
            string userName = HttpContext.Session.GetString("DoctorName");
            List<AppointmentBookingViewModel> appointments = new();
            using (var client = new HttpClient())
            {
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));

                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                // var result = await client.GetAsync("AppointmentBooking/GetAllBookings");
                var result = await client.GetAsync($"AppointmentBooking/GetAllAppointmentsByDoctorName/{userName}");
                if (result.IsSuccessStatusCode)
                {
                    appointments = await result.Content.ReadAsAsync<List<AppointmentBookingViewModel>>();
                }
            }
            return View(appointments);
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

        public async Task<IActionResult> AppointmentDelete(int id)
        {
            AppointmentBookingViewModel appointment = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync($"AppointmentBooking/GetAppointmentById/{id}");
                if (result.IsSuccessStatusCode)
                {
                    appointment = await result.Content.ReadAsAsync<AppointmentBookingViewModel>();
                    return View(appointment);
                }
                else
                {
                    ModelState.AddModelError("", "Server Error.Please try later");
                }
            }
            return View(appointment);
        }
        [HttpPost]
        // [HttpPost("AppointmentBooking/Delete/{AppointmentBookingId}")]
        public async Task<IActionResult> AppointmentDelete(AppointmentBookingViewModel appointment)
        {

            using (var client = new HttpClient())
            {

                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.DeleteAsync($"AppointmentBooking/DeleteAppointment/{appointment.Id}");
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Appointment","Doctor");

                }
                else
                {
                    return RedirectToAction("Appointment","Doctor");
                    //ModelState.AddModelError("", "Server Error.Please try later");
                }
            }

        }

    }
}
