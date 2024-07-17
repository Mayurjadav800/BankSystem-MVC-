using AutoMapper;
using BankSystem_MVC_.Data;
using BankSystem_MVC_.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;

namespace BankSystem_MVC_.Repository
{
   
   
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly IMapper _mapper;
        private readonly BankDbContext _bankDbContext;
        private readonly IConfiguration _configuration;

        public AuthenticationRepository(IMapper mapper, BankDbContext bankDbContext, IConfiguration configuration)
        {
            _mapper = mapper;
            _bankDbContext = bankDbContext;
            _configuration = configuration;
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}
       

        public async Task<string> CreateAuthentication(LogginDto logginDto)
        {
            try
            {
                var user = _bankDbContext.Account.Where(e => e.Email == logginDto.Email && e.Password == logginDto.Password);
                if (user == null)
                {
                    throw new UnauthorizedAccessException("Invalid Credentials");
                }
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier,logginDto.Email),

                };
                var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.Now.AddMinutes(15),
                    signingCredentials: credentials);
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    } 
}
