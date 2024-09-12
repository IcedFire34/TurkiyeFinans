using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
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
        private readonly TransferOperations _transferOperations;
        private readonly TransactionOperations _transactionOperations;
        public HomeController(ILogger<HomeController> logger, CustomerOperations customerOperations,
            TurkiyeFinansDbContext context, AccountOperations accountOperations,
            CurrencyOperations currencyOperations, TransferOperations transferOperations, 
            TransactionOperations transactionOperations)
        {
            _logger = logger;
            _customerOperations = customerOperations;
            _context = context;
            _accountOperations = accountOperations;
            _currencyOperations = currencyOperations;
            _transferOperations = transferOperations;
            _transactionOperations = transactionOperations;
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Index()
        {
            ViewModel viewModel = new ViewModel();
            return View(viewModel);
        }
        
        // Kayýt Ol
        // Kayit formu tarafindan cagriliyor.
        [HttpPost]
        public async Task<IActionResult> KayitOlGonder(long TCKimlikNo, string Ad, string Soyad, string DogumTarihi, string Adres, string Telefon, string Email, string Pass)
        {            
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
                Customer customer = new Customer
                {
                    FirstName = Ad,
                    LastName = Soyad,
                    DateOfBirth = DogumTarihi,
                    Address = Adres,
                    PhoneNumber = Telefon,
                    Email = Email,
                    IdentificationNumber = TCKimlikNo.ToString(),
                    Pass = Pass,
                };
                _logger.LogInformation("<<<<< TC: dogru >>>>>");
                bool isAdded = await _customerOperations.AddCustomerAsync(customer);
                if (isAdded) {                    
                    ViewModel viewModel = new ViewModel
                    {
                        _Customer = customer
                    };
                    _logger.LogInformation("<<<<< Musteri eklendi >>>>>");
                    return View("AnaEkran",viewModel);
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
            Customer customer = await _customerOperations.GetCustomerAsync(SilTCKimlikNo.ToString());
            bool isDeleted = await _customerOperations.DelCustomerAsync(customer);
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
            var viewData = new ViewModel
            {
                _Customer = await _customerOperations.GetCustomerAsync(GirTCKimlikNo.ToString()),                
            };
            bool isVerify = await _customerOperations.VerifyCustomerAsync(viewData._Customer);
            if (isVerify)
            {
                _logger.LogInformation("<<<<< Bilgiler dogru. Giris yapiliyor. >>>>>");                
                TempData["UserTC"]=GirTCKimlikNo.ToString();

                viewData._Accounts = await _accountOperations.ListAccountAsync(viewData._Customer.CustomerId);
                if (viewData._Customer.IdentificationNumber == "11111111111")
                {
                    return View("Admin",viewData);
                }
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
                _Customers = _context.Customers.ToList(),                
            };
            return View("Admin",viewModel);
        }

        public IActionResult AnaEkran()
        {
            ViewModel viewModel = new ViewModel();
            return View(viewModel);
        }
        public IActionResult Transfer()
        {
            string userTC = TempData["UserTC"].ToString();
            TempData["UserTC"] = userTC;
            ViewModel viewModel = new ViewModel
            {
                _Customer = _customerOperations.GetCustomerAsync(userTC).Result,
                
            };
            viewModel._Accounts = _accountOperations.ListAccountAsync(viewModel._Customer.CustomerId).Result;
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
            string userTC = TempData["UserTC"].ToString();
            Customer user = await _customerOperations.GetCustomerAsync(userTC);            
            bool result = await _accountOperations.AddAccountAsync(user.CustomerId,AccountType,Currency,Deposit); 
            TempData["UserTC"]=userTC;

            ViewModel viewModel = new ViewModel
            {
                _Customer = user,
                _Accounts = await _accountOperations.ListAccountAsync(user.CustomerId),
            };
            return View("AnaEkran",viewModel);
        }        

        [HttpPost]
        public async Task<IActionResult> HesapKapat(int CloseAccountID)
        {
            string userTC = TempData["UserTC"].ToString();
            TempData["UserTC"] = userTC;

            Customer user = await _customerOperations.GetCustomerAsync(userTC);
            bool result = await _accountOperations.CloseAccountAsync(CloseAccountID);
            ViewModel viewModel = new ViewModel
            {
                _Customer = user,
                _Accounts = await _accountOperations.ListAccountAsync(user.CustomerId),
            };
            return View("AnaEkran", viewModel);
        }
        
        public async Task<IActionResult> HesapListele()
        {            
            string userTC = TempData["UserTC"].ToString();
            Customer user = await _customerOperations.GetCustomerAsync(userTC);
            List<Account> accounts = await _accountOperations.ListAccountAsync(user.CustomerId);
            TempData["UserTC"] = userTC;
            ViewModel viewModel = new ViewModel
            {
                _Customer = user,
                _Accounts= accounts
            };
            return View("AnaEkran",viewModel);
        }

        public async Task<IActionResult> Test()
        {
            _logger.LogInformation("<<<<< Bu bir testtir >>>>>");
            Console.WriteLine(_accountOperations.GetAccountWithAccountId(4051).Result.Iban);
            return RedirectToAction("AnaEkran");
        }
        public async Task<IActionResult> ParaYatir()
        {
            _logger.LogInformation("<<<<< Bu bir testtir >>>>>");
            Console.WriteLine(_accountOperations.Deposit(4050,1000,"Para yatir test"));
            return RedirectToAction("AnaEkran");
        }
        public async Task<IActionResult> ParaCek()
        {
            _logger.LogInformation("<<<<< Bu bir testtir >>>>>");
            Console.WriteLine(_accountOperations.Withdraw(4050, 70, "Para Cek test"));
            return RedirectToAction("AnaEkran");
        }
        public async Task<IActionResult> HavaleYap(decimal senderAccount, string recipientIBAN, string recipientName, double recipientAmount)
        {           
            bool result = await _transferOperations.Havale(senderAccount,recipientIBAN,recipientName,recipientAmount);
            Console.WriteLine(result);
            return RedirectToAction("AnaEkran");
        }
        public IActionResult MevduatHesapla(float tutar,float vade,float faizOrani=40f,float stopajOrani=7.5f)
        {
            ViewBag.Vade = vade;
            ViewBag.FaizOrani = faizOrani;            
            ViewBag.BürütFaizTutari =tutar * faizOrani * (vade/36600);
            ViewBag.StopajOrani = stopajOrani;
            float StopajTutari = ViewBag.BürütFaizTutari * (stopajOrani / 100);
            ViewBag.StopajTutari = (StopajTutari).ToString("N2");
            float NetFaizTutari = ViewBag.BürütFaizTutari - StopajTutari;
            ViewBag.NetFaizTutari = NetFaizTutari.ToString("N2");
            ViewBag.VadeSonuTutari = (tutar + NetFaizTutari).ToString("N2");
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
