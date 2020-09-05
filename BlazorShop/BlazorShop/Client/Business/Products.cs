﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorShop.Shared.Models;

namespace BlazorShop.Client.Business 
{
    class Products 
    {
        public static double CalculatePromotionValue(Product product) 
        {
            double adjustment = Math.Pow(10, 1);
            double value = product.Value - (product.Value * product.PromotionPercentage / 100);

            return Math.Floor(value * adjustment) / adjustment;
        }
    }
}
