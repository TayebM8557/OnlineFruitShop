using System.ComponentModel.DataAnnotations;

namespace OnlineFruitShop.Model
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
