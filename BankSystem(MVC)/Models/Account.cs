using System.ComponentModel.DataAnnotations;
using static System.Net.WebRequestMethods;
using System.Security.Cryptography.Xml;

namespace BankSystem_MVC_.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public double CurrentBalance { get; set; }
        [Required]
        public int AccountNumber { get; set; }

        // Navigation 
        public virtual ICollection<Deposite> Deposites { get; set; }
        public virtual ICollection<Transfer> Transfers { get; set; }
        public virtual ICollection<Withdraw> Withdraws { get; set; }
        public virtual ICollection<Otp> Otps { get; set; }

    }
}
