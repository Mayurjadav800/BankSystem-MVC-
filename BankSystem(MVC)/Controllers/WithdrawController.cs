using AutoMapper;
using BankSystem_MVC_.Data;
using BankSystem_MVC_.Dto;
using BankSystem_MVC_.Models;
using BankSystem_MVC_.Repository;
using Microsoft.AspNetCore.Mvc;
using System.CodeDom;

namespace BankSystem_MVC_.Controllers
{
    public class WithdrawController : Controller
    {
        private readonly BankDbContext _bankDbContext;
        private readonly IMapper _mapper;
        private readonly IEmailRepository _emailRepository;

        public WithdrawController(BankDbContext bankDbContext,IMapper mapper,IEmailRepository emailRepository)
        {
            _bankDbContext = bankDbContext;
            _mapper = mapper;
            _emailRepository = emailRepository;
        }
        public IActionResult WithdrawAccount()
        {
            try
            {
                var accountlist = _bankDbContext.Withdraw.ToList();
                return View(accountlist);

            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public IActionResult AddWithdraw()
        {
            return View();
        }
        [HttpPost]

        public IActionResult CreateWithdraw(WithdrawDto withdrawDto)
        {
            if (withdrawDto.WithdrawAmount < 0)
            {
                throw new ArgumentException("Withdraw amount must be greater then Zero");
            }
            using (var transcation = _bankDbContext.Database.BeginTransaction())
            {
                try
                {
                    var account = _bankDbContext.Account
                        .FirstOrDefault(e => e.Id == withdrawDto.AccountId);
                    if (account == null)
                    {
                        throw new ArgumentException("Withdrawal  amount greater than Zero");
                    }
                    var withdraw = _mapper.Map<Withdraw>(withdrawDto);
                    _bankDbContext.Withdraw.Add(withdraw);
                    _bankDbContext.SaveChanges();

                    var user = _mapper.Map<WithdrawDto>(withdraw);
                    account.CurrentBalance = account.CurrentBalance - withdrawDto.WithdrawAmount;
                    _bankDbContext.Account.Update(account);

                    _bankDbContext.SaveChanges();
                    transcation.Commit(); 
                    var mailRequest = new MailRequest()
                    {
                        ToEmail = account.Email,
                        Subject = "Withdrawal Confirmation",
                        Body = $"{account.FirstName}, your withdrawal of {withdrawDto.WithdrawAmount} has been successfully processed. Your new balance is {account.CurrentBalance}."
                    };

                     _emailRepository.SendEmailAsync(mailRequest);
                    return RedirectToAction("WithdrawAccount");
                }
                catch (Exception ex)
                {


                    transcation.RollbackAsync();
                    return RedirectToAction("WithdrawAccount");
                }
            }
        }
    }
}
