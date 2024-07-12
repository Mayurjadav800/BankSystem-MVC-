using AutoMapper;
using BankSystem_MVC_.Data;
using BankSystem_MVC_.Dto;
using BankSystem_MVC_.Models;
using BankSystem_MVC_.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankSystem_MVC_.Controllers
{
    public class DepositeController : Controller
    {
        private readonly BankDbContext _bankDbContext;
        private readonly IMapper _mapper;
        private readonly IEmailRepository _emailRepository;

        public DepositeController(BankDbContext bankDbContext, IMapper mapper,IEmailRepository emailRepository)
        {
            _bankDbContext = bankDbContext;
            _mapper = mapper;
            _emailRepository = emailRepository;
        }
        public IActionResult DepositeAccount()
        {
            try
            {
                var accountlist = _bankDbContext.Deposite.ToList();
                return View(accountlist);

            }
            catch (Exception ex)
            {
                return View();
            }

        }
        public IActionResult AddDeposit()
        {
            return View();
        }
       

        [HttpPost]
        public  IActionResult CreateDeposite(DepositeDto depositeDto)
        {
            if (depositeDto.DepositeAmount <= 0)
            {
                throw new ArgumentException("Deposite amount must be greater then Zero");
            }
            using (var transcation = _bankDbContext.Database.BeginTransaction())
            {
                {
                    try
                    {
                        var account = _bankDbContext.Account.FirstOrDefault(e => e.Id == depositeDto.AccountId);
                        if (account == null)
                        {
                            throw new ArgumentException("Account Not Found");
                        }
                        var deposite = _mapper.Map<Deposite>(depositeDto);
                        _bankDbContext.Deposite.Add(deposite);
                        _bankDbContext.SaveChanges();

                        var users = _mapper.Map<DepositeDto>(deposite);
                        account.CurrentBalance = account.CurrentBalance + depositeDto.DepositeAmount;
                        _bankDbContext.Account.Update(account);
                        _bankDbContext.SaveChanges();
                        transcation.Commit();
                        var mailRequest = new MailRequest()
                        {
                            ToEmail = account.Email,
                            Subject = "Deposite Confirmation",
                            Body = $"{account.FirstName},your deposite of {depositeDto.DepositeAmount} has succesfully processed."

                        };
                         _emailRepository.SendEmailAsync(mailRequest);
                        return RedirectToAction("DepositeAccount");


                    }
                    catch (Exception ex)
                    {
                        transcation.RollbackAsync();

                        return RedirectToAction("DepositeAccount");
                    }
                }


            }
        }
    }
}
