using AutoMapper;
using BankSystem_MVC_.Data;
using BankSystem_MVC_.Dto;
using BankSystem_MVC_.Models;
using BankSystem_MVC_.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace BankSystem_MVC_.Controllers
{
    public class TransferController : Controller
    {
        private readonly BankDbContext _bankDbContext;
        private readonly IEmailRepository _emailRepository;
        private readonly IMapper _mapper;

        public TransferController(BankDbContext bankDbContext, IMapper mapper,IEmailRepository emailRepository)
        {
            _bankDbContext = bankDbContext;
            _emailRepository = emailRepository;
            _mapper = mapper;
        }

        public IActionResult TransferAccount()
        {
            try
            {
                var accountList = _bankDbContext.Transfer.ToList();
                return View(accountList);
            }
            catch (Exception ex)
            {
                // Log exception here
                return View(); // Optionally pass an error message to the view
            }
        }

        public IActionResult AddTransfer()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateTransfer(TransferDto transferDto)
        {
            try
            {
               
                if (transferDto.TransferAmount <= 0)
                {
                    return BadRequest("Transfer amount must be greater than zero.");
                }

                using (var transaction = _bankDbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var senderAccount = _bankDbContext.Account.FirstOrDefault(e => e.Id == transferDto.SenderId);
                        if (senderAccount == null)
                        {
                            return NotFound("Sender account not found.");
                        }

                        var receiverAccount = _bankDbContext.Account.FirstOrDefault(e => e.Id == transferDto.ReceiverId);
                        if (receiverAccount == null)
                        {
                            return NotFound("Receiver account not found.");
                        }

                        if (senderAccount.CurrentBalance < transferDto.TransferAmount)
                        {
                            return BadRequest("Insufficient balance.");
                        }

                        senderAccount.CurrentBalance -= transferDto.TransferAmount;
                        receiverAccount.CurrentBalance += transferDto.TransferAmount;

                        _bankDbContext.Account.Update(senderAccount);
                        _bankDbContext.Account.Update(receiverAccount);

                        var transfer = _mapper.Map<Transfer>(transferDto);
                        _bankDbContext.Transfer.Add(transfer);
                       // _bankDbContext.Otp.Remove(otp);

                        _bankDbContext.SaveChanges();
                        transaction.Commit();
                        var senderEmailRequest = new MailRequest
                        {
                            ToEmail = senderAccount.Email,
                            Subject = "Transfer Confirmation",
                            Body = $"{senderAccount.FirstName}, you have successfully transferred {transferDto.TransferAmount} to {receiverAccount.FirstName}. Your new balance is {senderAccount.CurrentBalance}."
                        };
                        _emailRepository.SendEmailAsync(senderEmailRequest);

                        var receiverEmailRequest = new MailRequest
                        {
                            ToEmail = receiverAccount.Email,

                            Subject = "Transfer Confirmation",
                            Body = $"{receiverAccount.FirstName}, you have received {transferDto.TransferAmount} from {senderAccount.FirstName}. Your new balance is {receiverAccount.CurrentBalance}."
                        };
                         _emailRepository.SendEmailAsync(receiverEmailRequest);

                        return Ok(_mapper.Map<TransferDto>(transfer));
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        // Log exception here
                        return StatusCode(500, "Internal server error.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception here
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}




