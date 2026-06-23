using System.Text.RegularExpressions;

namespace Social_Network.Helpers
{
    public static class ValidationHelper
    {
        private static readonly Regex EmailRegex = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool IsValidEmail(string? email)
            => !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email.Trim());

        // Базовая проверка надёжности пароля: минимум 6 символов,
        // хотя бы одна буква и одна цифра.
        public static bool IsValidPassword(string? password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                return false;

            bool hasLetter = false, hasDigit = false;
            foreach (var c in password)
            {
                if (char.IsLetter(c)) hasLetter = true;
                else if (char.IsDigit(c)) hasDigit = true;
            }
            return hasLetter && hasDigit;
        }

        // Логин: 3–30 символов, буквы/цифры/._
        public static bool IsValidLogin(string? login)
            => !string.IsNullOrWhiteSpace(login)
               && Regex.IsMatch(login.Trim(), @"^[A-Za-z0-9._]{3,30}$");
    }
}
