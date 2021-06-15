using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Client.Models
{
    public class Buyer
    {
        [Key]
        public int BuyerId {get; set; }

        public int UserId {get; set; }

        public int ProductId {get; set; }

        public User Users {get; set;}

        public Product Products {get; set; }         
    }
}