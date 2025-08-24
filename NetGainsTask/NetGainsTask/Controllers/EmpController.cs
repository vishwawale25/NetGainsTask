using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NetGainsTask.EmpDb;
using NetGainsTask.Models;
using Newtonsoft.Json;

namespace NetGainsTask.Controllers
{
    public class EmpController : Controller
    {
        private readonly DbContextFile _context;
        private readonly string apiKey = "VWJXSzVWb0xpUUxPMldIZ2Z3OU5iZ1B5WmJ4Y0pOQzg3YTFESUFCNA==";

        public EmpController(DbContextFile context)
        {
            _context = context;
        }

        // Index - Async logic but same name
        public async Task<IActionResult> Index(string searchString)
        {
            var employees = _context.Employees.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(e =>
                    e.FirstName.Contains(searchString) ||
                    e.LastName.Contains(searchString) ||
                    e.Email.Contains(searchString) ||
                    e.PhoneNo.Contains(searchString) ||
                    e.State.Contains(searchString) ||
                    e.Country.Contains(searchString) ||
                    e.Qualification.Contains(searchString));
            }

            ViewData["filter"] = searchString;

            var list = await employees.OrderBy(x => x.Id).ToListAsync();
            return View(list);
        }

        // Create (GET)
        public async Task<IActionResult> Create()
        {
            var countries = await GetCountriesAsync();
            ViewBag.Countries = new SelectList(countries, "Iso2", "Name");


            return View();
        }

        // Create (POST)
        [HttpPost]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                var countries = await GetCountriesAsync();
                ViewBag.Countries = new SelectList(countries, "Iso2", "Name");
                return View(employee);
            }

            Console.WriteLine("Received State: " + employee.State);

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Get States (AJAX)
        [HttpGet]
        public async Task<JsonResult> GetStates(string countryCode)
        {
            var states = await GetStatesFromApiAsync(countryCode);
            return Json(states);
        }

        // API - Countries
        private async Task<List<Country>> GetCountriesAsync()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("X-CSCAPI-KEY", apiKey);

            var response = await client.GetAsync("https://api.countrystatecity.in/v1/countries");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {error}");
            }

            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Country>>(body);
        }

        // API - States
        private async Task<List<State>> GetStatesFromApiAsync(string countryCode)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("X-CSCAPI-KEY", apiKey);

            var response = await client.GetAsync($"https://api.countrystatecity.in/v1/countries/{countryCode}/states");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {error}");
            }

            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<State>>(body);
        }

        // Edit (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // Load countries from API
            var countries = await GetCountriesAsync();
            ViewBag.Countries = new SelectList(countries, "Iso2", "Name", employee.Country);

            ViewData["empid"] = employee.Id;
            return View(employee);
        }


        // Edit (POST)
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Delete
        public async Task<IActionResult> Delete(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null)
            {
                return RedirectToAction(nameof(Index));
            }

            _context.Employees.Remove(emp);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }


}
