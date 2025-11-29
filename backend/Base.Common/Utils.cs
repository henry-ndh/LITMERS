using AutoMapper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using TFU.Common.Extension;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Base.Common
{
    public static class Utils
    {
       

        /// <summary>
        /// tính thời gian trước
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string TimeAgo(DateTime dt)
        {
            var span = DateTime.Now - Convert.ToDateTime(dt);
            if (span.Days > 365)
            {
                var years = (span.Days / 365);
                if (span.Days % 365 != 0)
                    years += 1;
                return String.Format(" {0} {1} trước",
                years, "năm");
            }
            if (span.Days > 30)
            {
                var months = (span.Days / 30);
                if (span.Days % 31 != 0)
                    months += 1;
                return String.Format(" {0} {1} trước",
                months, "tháng");
            }
            if (span.Days > 0)
                return String.Format(" {0} {1} trước",
                span.Days, "ngày");
            if (span.Hours > 0)
                return String.Format(" {0} {1} trước",
                span.Hours, "giờ");
            if (span.Minutes > 0)
                return String.Format(" {0} {1} trước",
                span.Minutes, "phút");
            if (span.Seconds > 5)
                return String.Format(" {0} giây trước", span.Seconds);
            return span.Seconds <= 5 ? "vừa xong" : string.Empty;
        }

        public static DateTime ConvertToVietnamTime(DateTime utcDateTime)
        {
            TimeZoneInfo vietNamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, vietNamTimeZone);
        }


        public static DateTime GetCurrentVNTime()
        {
            DateTime utcNow = DateTime.UtcNow;
            DateTime vnTime = ConvertToVietnamTime(utcNow);
            return vnTime;
        }

        /// <summary>
        /// Get current date time in Vietnam timezone
        /// </summary>
        /// <returns></returns>
        public static DateTime GetCurrentDateTime()
        {
            DateTime utcNow = DateTime.UtcNow.AddHours(7);
            return utcNow;
        }

        /// <summary>
        /// Get current date time as string with format yyyy-MM-dd HH:mm
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetCurrentDateTimeAsString(string format = "yyyy-MM-dd HH:mm")
        {
            DateTime vietnamTime = GetCurrentDateTime();
            return vietnamTime.ToString(format);
        }

        /// <summary>
        /// Get current date as string with format yyyy-MM-dd
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetCurrentDateAsString(string format = "yyyy-MM-dd")
        {
            DateTime vietnamTime = GetCurrentDateTime();
            return vietnamTime.ToString(format);
        }

        /// <summary>
        /// Serialize the object to json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeObjectToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        /// <summary>
        /// Deserialize the json to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T DeserializeJsonToObject<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Generate a token for email verification
        /// </summary>
        /// <param name="_configuration"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string GenerateVerifyEmailToken(IConfiguration _configuration, string email)
        {
            var claim = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),

                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            if (_configuration == null)
            {
                Console.WriteLine("Configuration is null");
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claim,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        
        public static string GenerateRefreshToken(int length = 64)
        {
            var randomNumber = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            return Convert.ToBase64String(randomNumber);
        }



        public static string DecodeVerifyEmailToken(string token, IConfiguration configuration)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = configuration["JwtSettings:Secret"]!;
            var key = Encoding.UTF8.GetBytes(secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;

                return jwtToken.Subject;
            }
            catch
            {
                return null;
            }
        }



        /// <summary>
        /// Get the first name and last name from full name
        /// 
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static (string FirstName, string LastName) SplitName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return (string.Empty, string.Empty);

            var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
                return (parts[0], string.Empty); 

            var firstName = parts[0];
            var lastName = string.Join(' ', parts.Skip(1));

            return (firstName, lastName);
        }

    }
}
