using HealthCareMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HealthCareMVC.Controllers
{
    public class AppointmentBookingController : Controller
    {
        private readonly IConfiguration _configuration;
        public AppointmentBookingController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<AppointmentBookingViewModel> appointments = new();
            using (var client = new HttpClient())
            {
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));

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
            AppointmentBookingViewModel appointment = null;
            using (var client = new HttpClient())
            {
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));

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
        [Route("AppointmentBooking/Create")]
        public async Task<IActionResult> Create()
        {
            AppointmentBookingViewModel viewModel = new AppointmentBookingViewModel
            {
                Specializations = await this.GetSpecializations()
            };
            return View(viewModel);
          
        }

        [HttpPost("AppointmentBooking/Create")]
        public async Task<IActionResult> Create(AppointmentBookingViewModel appointment)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                    client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);

                    var result = await client.PostAsJsonAsync("AppointmentBooking/CreateAppointment", appointment);
                    if (result.StatusCode == System.Net.HttpStatusCode.Created)
                    {

                        return RedirectToAction("index", "AppointmentBooking");
                    }
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
                    //  client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                    client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);
                    var result = await client.GetAsync($"AppointmentBooking/GetAppointmentById/{id}");
                    if (result.IsSuccessStatusCode)
                    {
                        appointment = await result.Content.ReadAsAsync<AppointmentBookingViewModel>();
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
        public async Task<IActionResult> Delete(AppointmentBookingViewModel appointment)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.DeleteAsync($"AppointmentBooking/DeleteAppointment/{appointment.Id}");
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
    }
}
