﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorShop.Shared.DTOs.Order
{
    public class UpdateOrderStatusDTO
    {
        public int Id { get; set; }
        public string Status { get; set; }
    }
}
