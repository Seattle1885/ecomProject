using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace Client.Models
{
    public class User 
    {
        [Key]
        public int UserId {get; set; }

        [Required]
        [StringLength(20, MinimumLength=5)]
        public string FirstName {get; set; }

        
        [Required]
        [StringLength(20, MinimumLength=2)]
        public string LastName {get; set; }

        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage= "InValid E-mail")]
        public string Email {get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8,ErrorMessage="Password must be 8 Characters long")]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        public string Password{get; set;}

        public DateTime CreatedAt {get; set; } = DateTime.Now;
        
        public DateTime UpdatedAt {get; set; } = DateTime.Now;

        [NotMapped]
        [Display(Name="Confirm Password")]
        [Compare("Password")]
        [DataType(DataType.Password,ErrorMessage="Invalid")]
        public string PWC {get; set;}
        public List<Product> UserProducts {get; set; } 
        
    }
}