﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductShop.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string ProductComposition { get; set; }
        public string Manufacturer { get; set; }

        public bool IsDeleted { get; set; }
        public int Count { get; set; } 
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public bool HaveDiscount { get; set; }
    }
}
