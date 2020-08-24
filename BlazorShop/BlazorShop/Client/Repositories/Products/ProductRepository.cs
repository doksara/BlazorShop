﻿using BlazorShop.Client.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorShop.Shared.Models;
using BlazorShop.Shared.Http;
using BlazorShop.Shared.DTOs;

namespace BlazorShop.Client.Repositories.Products
{
    class ProductRepository : IProductRepository
    {
        private readonly IHttpService _httpService;
        private readonly string baseURL = "api/product";

        public ProductRepository(IHttpService httpService)
        {
            this._httpService = httpService;
        }

        public async Task<List<ProductViewModel>> GetProducts()
        {
            var response = await _httpService.Get<List<ProductViewModel>>(baseURL);

            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }

            return response.Data;
        }
    }
}