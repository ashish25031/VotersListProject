using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace VotersListProject.Models
{
    public class VoterModel : IValidatableObject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string VoterId { get; set; }

        [Required]
        public string VoterName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public DateTime DoB { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 12)]
        public string Aadhaar { get; set; }

        [Required]
        public string Role { get; set; }

        // ============================
        // 🔐 Email OTP Verification
        // ============================

        public bool IsEmailVerified { get; set; } = false;

        public string? EmailOTP { get; set; }

        public DateTime? OTPExpiry { get; set; }

        // ============================
        // 🔥 Custom Age Validation
        // ============================
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var age = DateTime.Today.Year - DoB.Year;

            if (DoB > DateTime.Today.AddYears(-age))
            {
                age--;
            }

            if (age < 18)
            {
                yield return new ValidationResult(
                    "Age should be greater than 18",
                    new[] { nameof(DoB) });
            }
        }
    }
}