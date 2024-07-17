using BankSystem_MVC_.Data;
using BankSystem_MVC_.Dto;
using BankSystem_MVC_.Models;
using BankSystem_MVC_.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BankSystem_MVC_.Controllers
{
    public class OtpController : Controller
    {
        private readonly BankDbContext _bankDbContext;
        private readonly IEmailRepository _emailRepository;

        public OtpController(BankDbContext bankDbContext, IEmailRepository emailRepository)
        {
            _bankDbContext = bankDbContext;
            _emailRepository = emailRepository;
        }

        public IActionResult Add()
        {
            try
            {
                var accountList = _bankDbContext.Otp.ToList();
                return View(accountList);
            }
            catch (Exception ex)
            {
                // Log the exception here
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public IActionResult AddOtp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateOtp(int accountId)
        {
            try
            {
                
                var account = await _bankDbContext.Account.FirstOrDefaultAsync(e => e.Id == accountId);

                if (account == null)
                {
                    return BadRequest($"Account with ID {accountId} does not exist.");
                }

                var existingOtp = await _bankDbContext.Otp
                    .Where(e => e.AccountId == accountId && e.ExpiryDate > DateTime.Now)
                    .FirstOrDefaultAsync();

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
                await _bankDbContext.SaveChangesAsync();

                var mailRequest = new MailRequest
                {
                    ToEmail = account.Email,
                    Subject = "OTP for Transfer Confirmation",
                    Body = $"Your OTP for transfer is: {otpCode}"
                };

                await _emailRepository.SendEmailAsync(mailRequest);
                // return Ok(otpCode);
                return Ok("Generating the Otp succesfully");
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(500, "Internal server error.");
            }
        }

        private string GenerateRandomOtp()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public IActionResult VerifyOtp()
        {
            return View();
        }

        [HttpPost]
        public async Task<bool> Verify(int accountId,string Code)
       // public async Task<IActionResult> Verify(int accountId, string otpCode)
        {
            try
            {
                var account = await _bankDbContext.Account.Where(e => e.Id == accountId).FirstOrDefaultAsync();


                // var account = await _bankDbContext.Account.FirstOrDefaultAsync(e => e.Id == accountId);

                if (account == null)
                {
                   // ViewData["Message"] = "AccountId Does Not Exist.";
                    throw new ArgumentException($"Account with ID {accountId} does not exist.");
                }
                var otp = await _bankDbContext.Otp
                    .Where(e => e.AccountId == accountId && e.Code == Code && e.ExpiryDate > DateTime.Now)
                    .FirstOrDefaultAsync();

                if (otp != null)
                {
                    otp.IsUsed = true;
                    _bankDbContext.Otp.Update(otp);
                    await _bankDbContext.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error verifying OTP", ex);
            }

        }
    }
}













