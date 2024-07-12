
using BankSystem_MVC_.Data;
using BankSystem_MVC_.Dto;
using BankSystem_MVC_.Models;
using Microsoft.AspNetCore.Mvc;
namespace BankSystem_MVC_.Controllers
{
    public class OtpController : Controller
    {
        private readonly BankDbContext _bankDbContext;

        public OtpController(BankDbContext bankDbContext)
        {
            _bankDbContext = bankDbContext;
        }
        public IActionResult Add()
        {
            try
            {
                var accountlist = _bankDbContext.Otp.ToList();
                return View(accountlist);

            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public IActionResult AddOtp()
        {
            return View();
        }
        public IActionResult CreateOtp(int accounId)
        {
            try
            {
                var account = _bankDbContext.Otp.FirstOrDefault(e => e.Id == accounId);
                if(account == null)
                {
                    throw new ArgumentException("AccountId is  not null excepts");
                }
                var existingOtp = _bankDbContext.Otp
               .Where(e => e.AccountId == accounId && e.ExpiryDate > DateTime.Now)
               .FirstOrDefault();
                if (existingOtp != null)
                {
                    return Ok(existingOtp.Code);
                }
                var otpCode = GenerateRandomOtp();
                var otp = new Otp
                {
                    AccountId = account.Id,
                    Code = otpCode,
                    ExpiryDate = DateTime.Now.AddMinutes(2),
                    IsUsed = false
                };
                _bankDbContext.Otp.Add(otp);
                 _bankDbContext.SaveChanges();
                return Ok(otpCode);
            }
            catch(Exception ex)
            {
                return StatusCode(500, "Internal server error.");
            }
        }
        private string GenerateRandomOtp()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
        [HttpPost]
        public IActionResult Verify(int accountId, string otpCode)
        {
            try
            {
                var account = _bankDbContext.Account.Find(accountId);
                if (account == null)
                {
                    return BadRequest($"Account with ID {accountId} does not exist.");
                }

                var otp = _bankDbContext.Otp
                    .Where(e => e.AccountId == accountId && e.Code == otpCode && e.ExpiryDate > DateTime.Now)
                    .FirstOrDefault();

                if (otp != null)
                {
                    otp.IsUsed = true;
                    _bankDbContext.Otp.Update(otp);
                     _bankDbContext.SaveChanges();
                    return Ok("OTP verified successfully.");
                }

                return BadRequest("Invalid or expired OTP.");
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(500, "Internal server error.");
            }
        }







    }
}
