using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Northwind.Mvc.Models;
using System.Diagnostics;
using Packt.Shared; // NorthwindContext

namespace Northwind.Mvc.Controllers
{
    public class QueryController : Controller
    {
        private readonly ILogger<QueryController> _logger;
        private readonly NorthwindContext db;
        private readonly IHttpClientFactory clientFactory;

        public QueryController(ILogger<QueryController> logger, NorthwindContext injectedContext, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            db = injectedContext;
            clientFactory = httpClientFactory;
        }

        [Route("query")]                       //route alias -- simplifies route address
        [Authorize(Roles = "Administrators")]  //temporary -- to illustrate 'authorization' decorator
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Index()
        {
            _logger.LogError("This is a query page error (test!)");
            _logger.LogWarning("This is your first query page warning!");
            _logger.LogWarning("Second query pagewarning!");
            _logger.LogInformation("I am LogInfo in the Index method of the QueryController.");

            QueryIndexViewModel model = new
            (
                VisitorCount: (new Random()).Next(1, 1001),
                Categories: await db.Categories.ToListAsync(),
                Products: await db.Products.ToListAsync()
            );
            return View(model); // pass model to view
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> ProductDetail(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest("You must pass a product ID in the route, for example, /Query/ProductDetail/21");
            }

            Product? model = await db.Products
              .SingleOrDefaultAsync(p => p.ProductId == id);

            if (model == null)
            {
                return NotFound($"ProductId {id} not found.");
            }

            return View(model); // pass model to view and then return result
        }


        public IActionResult ProductsThatCostMoreThan(decimal? price)
        {
            if (!price.HasValue)
            {
                return BadRequest("You must pass a product price in the query string, for example, /Query/ProductsThatCostMoreThan?price=50");
            }

            IEnumerable<Product> model = db.Products
              .Include(p => p.Category)
              .Include(p => p.Supplier)
              .Where(p => p.UnitPrice > price);

            if (!model.Any())
            {
                return NotFound(
                  $"No products cost more than {price:C}.");
            }

            ViewData["MaxPrice"] = price.Value.ToString("C");
            return View(model); // pass model to view
        }

        public async Task<IActionResult> Customers(string country)
        {
            string uri;
            if (string.IsNullOrEmpty(country))
            {
                ViewData["Title"] = "All Customers Worldwide";
                uri = "api/customers/";
            }
            else
            {
                ViewData["Title"] = $"Customers in {country}";
                uri = $"api/customers/?country={country}";
            }

            HttpClient client = clientFactory.CreateClient(name: "Northwind.WebApi");
            HttpRequestMessage request = new(method: HttpMethod.Get, requestUri: uri);
            HttpResponseMessage response = await client.SendAsync(request);

            IEnumerable<Customer>? model = await response.Content.ReadFromJsonAsync<IEnumerable<Customer>>();

            return View(model);
        }
    }
}

