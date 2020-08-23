﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorShop.Shared.DTOs
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Value { get; set; }
        public bool Promotion { get; set; }
        public int PromotionPercentage { get; set; }
        public int Quantity { get; set; }
        public string ImageName { get; set; }
    }
}
