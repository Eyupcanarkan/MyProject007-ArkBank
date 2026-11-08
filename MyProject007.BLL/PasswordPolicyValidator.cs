using System;
using System.Collections.Generic;
using System.Linq;

namespace MyProject007.BLL
{
    public class PasswordValidationResult
    {
        public bool IsValid { get; }
        public List<string> Errors { get; }

        public PasswordValidationResult(bool isValid, List<string> errors)
        {
            IsValid = isValid;
            Errors = errors ?? new List<string>();
        }
    }

    public static class PasswordPolicyValidator
    {
        public static PasswordValidationResult Validate(
            string password, string firstName, string lastName, DateTime? birthDate)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(password))
            {
                errors.Add("Parola alanı boş bırakılamaz.");
                return new PasswordValidationResult(false, errors);
            }

          
            if (password.Length < 6)
                errors.Add("Parola en az 6 karakter uzunluğunda olmalıdır.");

       
            if (!password.Any(char.IsUpper))
                errors.Add("Parola en az bir büyük harf içermelidir.");

            if (!password.Any(char.IsLower))
                errors.Add("Parola en az bir küçük harf içermelidir.");

            if (!password.Any(char.IsDigit))
                errors.Add("Parola en az bir rakam içermelidir.");

            string pLower = password.ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(firstName) && firstName.Length >= 2 && pLower.Contains(firstName.Trim().ToLowerInvariant()))
                errors.Add("Parola adınızı içeremez.");

            if (!string.IsNullOrWhiteSpace(lastName) && lastName.Length >= 2 && pLower.Contains(lastName.Trim().ToLowerInvariant()))
                errors.Add("Parola soyadınızı içeremez.");

            
            if (birthDate.HasValue && pLower.Contains(birthDate.Value.Year.ToString()))
                errors.Add("Parola doğum yılınızı içeremez.");

       
            if (ContainsSequentialDigits(password))
                errors.Add("Parola en az üç haneli ardışık rakam dizisi (örn: 123 veya 321) içeremez.");

            return new PasswordValidationResult(errors.Count == 0, errors);
        }

        private static bool ContainsSequentialDigits(string password)
        {
            for (int i = 0; i < password.Length - 2; i++)
            {
                if (char.IsDigit(password[i]) && char.IsDigit(password[i + 1]) && char.IsDigit(password[i + 2]))
                {
                    int d1 = password[i] - '0';
                    int d2 = password[i + 1] - '0';
                    int d3 = password[i + 2] - '0';

                   
                    if ((d2 == d1 + 1 && d3 == d2 + 1) || (d2 == d1 - 1 && d3 == d2 - 1))
                        return true;
                }
            }
            return false;
        }
    }
}