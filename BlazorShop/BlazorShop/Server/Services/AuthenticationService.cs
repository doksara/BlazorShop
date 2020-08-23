﻿using BlazorShop.Server.Data;
using BlazorShop.Shared.DTOs;
using BlazorShop.Shared.Http;
using BlazorShop.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlazorShop.Server.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly BlazorShopContext _context;
        private readonly IConfiguration _configuration;

        public AuthenticationService(BlazorShopContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        public async Task<HttpResponse<UserToken>> Login(string username, string password)
        {
            HttpResponse<UserToken> response = new HttpResponse<UserToken>();
            User user = await _context.User.FirstOrDefaultAsync(x => x.Username.ToLower().Equals(username.ToLower()));
            
            if (user == null)
            {
                response.Success = false;
            }
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
            }
            else
            {
                response.Data = CreateToken(user);
            }

            return response;
        }

        public async Task<HttpResponse<int>> Register(UserRegisterDTO userInfo)
        {
            HttpResponse<int> response = new HttpResponse<int>();
            
            User newUser = new User
            {
                Username = userInfo.Username,
                Name = userInfo.Name,
                Enabled = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Email = userInfo.Email,
                Phone = userInfo.Phone,
                Document = userInfo.Document,
                Discriminator = "Client",
                Address = userInfo.Address
            };


            if (await UserExists(userInfo.Username))
            {
                response.Success = false;
                // response.Message = "User already exists!";
                return response;
            }

            CreatePasswordHash(userInfo.Password, out byte[] passwordHash, out byte[] passwordSalt);

            newUser.PasswordSalt = passwordSalt;
            newUser.PasswordHash = passwordHash;

            await _context.User.AddAsync(newUser);
            await _context.SaveChangesAsync();
            
            response.Data = newUser.Id;
            
            return response;
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.User.AnyAsync(x => x.Username.ToLower() == username.ToLower()))
            {
                return true;
            }
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private UserToken CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Discriminator)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var expiration = DateTime.UtcNow.AddYears(1);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: creds);

            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}
