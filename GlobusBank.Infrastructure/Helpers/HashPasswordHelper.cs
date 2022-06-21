using GlobusBank.Infrastructure.Interfaces;
using GlobusBank.Persistence.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GlobusBank.Infrastructure.Helpers
{
    public class HashPasswordHelper: IHashHelper
    {

        public HashPasswordsDto HashPassword(string password)
        {
            using var hmac = new HMACSHA512();
            var hashedPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            var passwordSalt = hmac.Key;
            return new HashPasswordsDto
            {
                PasswordHash = hashedPassword,
                PasswordSalt = passwordSalt
            };

        }
    }
}
