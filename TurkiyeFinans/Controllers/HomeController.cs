using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TurkiyeFinans.Models;

namespace TurkiyeFinans.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CustomerOperations _customerOperations;
        private readonly AccountOperations _accountOperations;
        private readonly TurkiyeFinansDbContext _context;
        private readonly CurrencyOperations _currencyOperations;

        public HomeController(ILogger<HomeController> logger, CustomerOperations customerOperations, TurkiyeFinansDbContext context, AccountOperations accountOperations, CurrencyOperations currencyOperations)
        {
            _logger = logger;
            _customerOperations = customerOperations;
            _context = context;
            _accountOperations = accountOperations;
            _currencyOperations = currencyOperations;
        }

        public IActionResult Index()
        {
            ViewModel viewModel = new ViewModel();
            return View(viewModel);
        }
        // Kayýt Ol
        // Kayit formu tarafindan cagriliyor.
        [HttpPost]
        public async Task<IActionResult> KayitOl(long TCKimlikNo, string Ad, string Soyad, string DogumTarihi, string Adres, string Telefon, string Email, string Pass)
        {
            var param = new MernisServiceParametters
            {
                TCKimlikNo = TCKimlikNo,
                Ad = Ad,
                Soyad = Soyad,
                DogumTarihi = DogumTarihi,
                Adres = Adres,
                Telefon = Telefon,
                Email = Email,
                Pass = Pass
            };
            if (TCKimlikNo == 0 || Ad == null ||  Soyad == null || DogumTarihi == null || Adres == null || Telefon == null || Email == null || Email.IndexOf("@") == -1 || TCKimlikNo.ToString().Length != 11 || Telefon.ToString().Length != 11 )
            {
                _logger.LogError("<<<<< Girilen bilgiler kurallara uygun degil >>>>>");

                return View("Index");
            }
            
            ServiceKPSPublic service = new ServiceKPSPublic();
            Response response = new Response();
            response._mernisserviceparametters.TCKimlikNo = TCKimlikNo;
            response._mernisserviceparametters.Ad=Ad;
            response._mernisserviceparametters.Soyad = Soyad;
            response._mernisserviceparametters.DogumYili = Convert.ToInt16(DogumTarihi.Substring(0,4));            
            var resault = service.OnGetService(response._mernisserviceparametters);
            if (resault.Result == true)
            {
                _logger.LogInformation("<<<<< TC: dogru >>>>>");
                bool isAdded = await _customerOperations.AddCustomerAsync(param);
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
        // Customer siliyor
        [HttpPost]
        public async Task<IActionResult> Sil(long SilTCKimlikNo)
        {
            var param = new MernisServiceParametters
            {
                TCKimlikNo = SilTCKimlikNo
            };
            bool isDeleted = await _customerOperations.DelCustomerAsync(param);
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
        
        // Giris yapiyor
        [HttpPost]
        public async Task<IActionResult> GirisYap(long GirTCKimlikNo, string GirPass)
        {
            var param = new MernisServiceParametters
            {
                TCKimlikNo = GirTCKimlikNo,
                Pass = GirPass
            };
            var viewData = new ViewModel
            {
                _Customer = _context.Customers.Where(c => c.IdentificationNumber == GirTCKimlikNo.ToString()).ToList(),
            };
            bool isVerify = await _customerOperations.VerifyCustomerAsync(param);
            if (isVerify)
            {
                _logger.LogInformation("<<<<< Bilgiler dogru. Giris yapiliyor. >>>>>");
                TempData["UserTC"]=GirTCKimlikNo.ToString();
                return View("AnaEkran",viewData);
            }
            else
            {
                _logger.LogError("<<<<< HATA: Giris yapilamadi. Girilen bilgiler yanlis >>>>>");
                return View("Index");
            }            
        }

        public IActionResult CustomerListele()
        {
            var viewModel = new ViewModel
            {
                _Customer = _context.Customers.ToList(),
                
            };
            return View("Index", viewModel);
        }

        public IActionResult AnaEkran()
        {
            ViewModel viewModel = new ViewModel();
            return View(viewModel);
        }

        public IActionResult HesapOlustur()
        {
            ViewModel viewModel = new ViewModel();
            viewModel._Currency = _currencyOperations.GetAllCurrencyAsync().Result;  // Döviz listesini alýyoruz           
            ViewBag.Currencies = viewModel._Currency;  // Dövizleri ViewBag ile sayfaya taþýyoruz
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> HesapAc(string AccountType,int Deposit,string Currency)
        {
            List<Customer> customerList = new List<Customer>(); 

            string userTC = TempData["UserTC"].ToString();
            Customer user = await _customerOperations.GetCustomerAsync(userTC);            
            bool result = await _accountOperations.AddAccountAsync(user.CustomerId,AccountType,Currency,Deposit); 
            TempData["UserTC"]=userTC;

            customerList.Add(user);
            ViewModel viewModel = new ViewModel
            {
                _Customer= customerList,
            };
            return View("AnaEkran",viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> HesapListele()
        {
            List<Customer> customerList = new List<Customer>();
            string userTC = TempData["UserTC"].ToString();
            Customer user = await _customerOperations.GetCustomerAsync(userTC);

            List<Account> result = await _accountOperations.ListAccountAsync(user.CustomerId);
            TempData["UserTC"] = userTC;

            customerList.Add(user);
            ViewModel viewModel = new ViewModel
            {
                _Customer = customerList,
                _Accounts= result
            };
            return View("AnaEkran",viewModel);
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
