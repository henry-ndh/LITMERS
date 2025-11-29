
using System;
using System.Linq;
namespace Base.Helper
{
    public class AKVHHelper
    {
        private static readonly Random _random = new Random();

        public static string GenerateAffiliateCode()
        {
            string numbers = _random.Next(10000000, 99999999).ToString();

            string letters = new string(Enumerable.Range(0, 2)
                .Select(_ => (char)_random.Next('A', 'Z' + 1))
                .ToArray());

            return numbers + letters;
        }

        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }

}
