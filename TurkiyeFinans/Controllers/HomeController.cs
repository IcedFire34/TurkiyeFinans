using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;
using TurkiyeFinans.Models;

namespace TurkiyeFinans.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CustomerOperations _customerOperations;

        public HomeController(ILogger<HomeController> logger, CustomerOperations customerOperations)
        {
            _logger = logger;
            _customerOperations = customerOperations;
        }

        public IActionResult Index()
        {
            ViewModel viewModel = new ViewModel();
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> KayitOl(ViewModel model)
        {
            if (model._mernisserviceparametters.TCKimlikNo == 0 || model._mernisserviceparametters.Ad == null || model._mernisserviceparametters.Soyad == null || 
                model._mernisserviceparametters.DogumTarihi == null || model._mernisserviceparametters.Adres == null || model._mernisserviceparametters.Telefon == null || 
                model._mernisserviceparametters.Email == null ||
                model._mernisserviceparametters.Email.IndexOf("@") == -1 || 
                model._mernisserviceparametters.TCKimlikNo.ToString().Length != 11 || 
                model._mernisserviceparametters.Telefon.ToString().Length != 11 )
            {
                _logger.LogError("<<<<< Girilen bilgiler kurallara uygun degil >>>>>");
                return View("Index");
            }
            ServiceKPSPublic service = new ServiceKPSPublic();
            Response response = new Response();
            response._mernisserviceparametters.TCKimlikNo = model._mernisserviceparametters.TCKimlikNo;
            response._mernisserviceparametters.Ad=model._mernisserviceparametters.Ad;
            response._mernisserviceparametters.Soyad = model._mernisserviceparametters.Soyad;
            response._mernisserviceparametters.DogumYili = Convert.ToInt16(model._mernisserviceparametters.DogumTarihi.Substring(0,4));            
            var resault = service.OnGetService(response._mernisserviceparametters);
            if (resault.Result == true)
            {
                _logger.LogInformation("<<<<< TC: dogru >>>>>");
                bool isAdded = await _customerOperations.AddCustomerAsync(model._mernisserviceparametters);
                if (isAdded) {
                    _logger.LogInformation("<<<<< Musteri eklendi >>>>>");
                    return View("AnaEkran");
                }
                else
                {
                    _logger.LogError("<<<<< HATA: Musteri eklenemedi >>>>>");
                    return View("Index");
                }
            }
            else
            {
                _logger.LogError("<<<<< TC: yanlis >>>>>");
                return View("Index");
            }           
        }
        [HttpPost]
        public async Task<IActionResult> Sil(ViewModel model)
        {            
            bool isDeleted = await _customerOperations.DelCustomerAsync(model._mernisserviceparametters);
            if (isDeleted)
            {
                _logger.LogInformation("<<<<< Musteri silindi >>>>>");
            }
            else
            {
                _logger.LogError("<<<<< HATA: Musteri silinemedi >>>>>");
            }
            return View("Index");
            
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
