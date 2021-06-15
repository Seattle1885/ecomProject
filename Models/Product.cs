using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography.X509Certificates;

namespace Client.Models
{
    public class Product 
    {
        [Key]
        public int ProductId {get; set; }

        [Required]
        public string ProductName {get; set; }

        [Required]
        [StringLength(1000)]
        public string Description {get; set; }

        [Required]
        public int Quantity {get; set; }

        [Display(Name = "Profile Picture")]  

        public DateTime CreatedAt {get; set; } = DateTime.Now;
        
        public DateTime UpdatedAt {get; set; } = DateTime.Now;
        public int UserId {get; set; }

        public User Creator {get; set; }

        public List<Buyer> UserBuyers {get; set;}
    }
}