using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BankSystem_MVC_.Models
{
    public class Withdraw
    {
        [Key]
        public int Id { get; set; }

        // Foreign key
        [ForeignKey("Account")]
        public int AccountId { get; set; }

        [Required]
        public double WithdrawAmount { get; set; }
        [Required]
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;


        // Navigation property
        public virtual Account Account { get; set; }
    }
}
