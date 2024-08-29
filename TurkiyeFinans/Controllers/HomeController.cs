using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TurkiyeFinans.Models;

namespace TurkiyeFinans.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewModel viewModel = new ViewModel();
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Index(ViewModel model)
        {
            ServiceKPSPublic service = new ServiceKPSPublic();
            Response response = new Response();

            response._mernisserviceparametters.TCKimlikNo = model._mernisserviceparametters.TCKimlikNo;
            response._mernisserviceparametters.Ad=model._mernisserviceparametters.Ad;
            response._mernisserviceparametters.Soyad = model._mernisserviceparametters.Soyad;
            response._mernisserviceparametters.DogumYili = model._mernisserviceparametters.DogumYili;

            var resault = service.OnGetService(response._mernisserviceparametters);


            return View(model);
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
    }
}
