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
    public class PatientController : Controller
    {

        private readonly IConfiguration _configuration;
        public PatientController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<PatientViewModel> patients = new();
            using (var client = new HttpClient())
            {
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));

                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync("Patients/GetAllPatients");
                if (result.IsSuccessStatusCode)
                {
                    patients = await result.Content.ReadAsAsync<List<PatientViewModel>>();
                }
            }
            return View(patients);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            PatientViewModel patient = null;
            using (var client = new HttpClient())
            {
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));

                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync($"Patients/GetPatientById/{id}");
                if (result.IsSuccessStatusCode)
                {
                    patient = await result.Content.ReadAsAsync<PatientViewModel>();
                }
            }
            return View(patient);
        }
        [HttpGet]
        [Route("Patients/Create")]
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost("Patients/Create")]
        public async Task<IActionResult> Create(PatientViewModel patient)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                    client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);

                    var result = await client.PostAsJsonAsync("Patients/CreatePatient", patient);
                    if (result.StatusCode == System.Net.HttpStatusCode.Created)
                    {

                        return RedirectToAction("index", "Patient");
                    }
                }
            }

            return View(patient);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {
                PatientViewModel patient = null;
                using (var client = new HttpClient())
                {
                    //  client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                    client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);
                    var result = await client.GetAsync($"patients/GetPatientById/{id}");
                    if (result.IsSuccessStatusCode)
                    {
                        patient = await result.Content.ReadAsAsync<PatientViewModel>();
                        return View(patient);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Patient doesn't exist");
                    }

                }
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(PatientViewModel patient)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);
                    var result = await client.PutAsJsonAsync($"Patients/UpdatePatient/{patient.Id}", patient);
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
            PatientViewModel patient = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync($"Patients/GetPatientById/{id}");
                if (result.IsSuccessStatusCode)
                {
                    patient = await result.Content.ReadAsAsync<PatientViewModel>();
                    return View(patient);
                }
                else
                {
                    ModelState.AddModelError("", "Server Error.Please try later");
                }
            }
            return View(patient);
        }
        [HttpPost("Patienrs/Delete/{PatientId}")]
        public async Task<IActionResult> Delete(PatientViewModel patient)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.DeleteAsync($"Patients/DeletePatient/{patient.Id}");
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
