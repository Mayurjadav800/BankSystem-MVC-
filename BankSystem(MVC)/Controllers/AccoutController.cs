using BankSystem_MVC_.Data;
using BankSystem_MVC_.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem_MVC_.Controllers
{
    public class AccoutController : Controller
    {
        private readonly BankDbContext _bankDbContext;

        public AccoutController(BankDbContext bankDbContext)
        {
            _bankDbContext = bankDbContext;
        }
        public IActionResult Add()
        {
            try
            {
                var accountlist = _bankDbContext.Account.ToList();
                return View(accountlist);

            }
            catch (Exception ex)
            {
                return View();
            }

        }
        public IActionResult AddAccount()
        {
            return View();
        } // Handle the account creation form submission

        [HttpPost]
        public ActionResult Create(Account account)
        {
            _bankDbContext.Account.Add(account);
            _bankDbContext.SaveChanges();
            return RedirectToAction("Add");
        }
    }
}
