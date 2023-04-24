using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CustomAuthentication.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace CustomAuthentication.Handlers
{
    public class CustomSecurityTokenHandler
    {

        // To generate encrypted string of this token
        public string GetEncryptedString(CustomSecurityToken token, string key)
        {
            var jsonSerializedToken = JsonSerializer.Serialize(token);
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(jsonSerializedToken);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }


        public CustomSecurityToken GetDecryptedToken(string tokenString, string key)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(tokenString);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            var jsonTokenString = streamReader.ReadToEnd();
                            var tokenParams = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(jsonTokenString);
                            var issuer = tokenParams["Issuer"].ToString();
                            var audience = tokenParams["Audience"].ToString();
                            var validTo = JsonSerializer.Deserialize<DateTime>(tokenParams["ValidTo"]);
                            var claims = JsonSerializer.Deserialize<Dictionary<string, string>>(tokenParams["Claims"]);
                            var token = new CustomSecurityToken(issuer, audience, claims, validTo);
                            return token;
                        }
                    }
                }
            }
        }

        public ClaimsPrincipal GetPrincipalFromToken(string tokenString, IConfiguration config)
        {
            var key = config["token:key"];
           
            var token = GetDecryptedToken(tokenString, key);
            // create claims array from the model
            var claims = token.Claims.Select(c => new Claim(c.Key, c.Value)).ToList();

            // generate claimsIdentity on the name of the class
            var claimsIdentity = new ClaimsIdentity(claims,
                        nameof(CustomSecurityTokenHandler));

            var principal = new ClaimsPrincipal(claimsIdentity);

            return principal;
        }


        public CustomSecurityToken GetAccessToken(User user, IConfiguration config)
        {
            //create claims details based on the user information
            var claims = new[] {
                        new Claim("Subject", config["token:subject"]),
                        new Claim("Id", Guid.NewGuid().ToString()),
                        new Claim("Iat", DateTime.UtcNow.ToString()),
                        new Claim("UserId", user.Id),
                        new Claim("UserName", user.UserName),
                        new Claim("Email", user.Email)
                    };
            var claimsDictionary = claims.ToDictionary(c => c.Type, c => c.Value);
            var key = config["token:key"];
            var token = new CustomSecurityToken(
                config["token:issuer"],
                config["token:audience"],
                claimsDictionary,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(config["token:accessTokenExpiryMinutes"])));

            return token;
        }

        public string GetRefreshToken(UserManager<User> userManager)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            //ensure token is unique by checking against db
            var tokenIsUnique = !userManager.Users.Any(u => u.RefreshToken == token);

            if (!tokenIsUnique)
                return GetRefreshToken(userManager);  //recursive call

            return token;
        }
    }
}