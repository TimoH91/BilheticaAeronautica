using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BilheticaAeronautica.Mobile.Validations
{
    public class Validator : IValidator
    {
        public string NameError { get; set; }
        public string EmailError { get; set; }
        public string TelephoneError { get; set; }
        public string PasswordError { get; set; }

        private const string EmptyNameErrorMessage = "Please, enter your name.";
        private const string InvalidNameErrorMessage = "Please, enter a valid name.";
        private const string EmailEmptyErrorMessage = "Please, enter your email.";
        private const string EmailInvalidErrorMessage = "Please, enter a valid email.";
        private const string TelephoneEmptyErrorMessage = "Please, enter your telephone number.";
        private const string TelephoneInvalidErorrMessage = "Please, enter a valid telephone number.";
        private const string PasswordEmptyErrorMessage = "Please, enter a password.";
        private const string PasswordInvalidErrorMessage = "The password must contain at least 8 caracteres, including letters and numbers.";

        public Task<bool> Validar(string name, string email, string telephone, string password)
        {
            var isNameValid = ValidateName(name);
            var isEmailValid = ValidateEmail(email);
            var isTelephoneValid = ValidateTelephone(telephone);
            var isPasswordValid = ValidatePassword(password);

            return Task.FromResult(isNameValid && isEmailValid && isTelephoneValid && isPasswordValid);
        }

        private bool ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                NameError = EmptyNameErrorMessage;
                return false;
            }

            if (name.Length < 3)
            {
                NameError = InvalidNameErrorMessage;
                return false;
            }

            NameError = "";
            return true;
        }

        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                EmailError = EmailEmptyErrorMessage;
                return false;
            }

            if (!Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                EmailError = EmailInvalidErrorMessage;
                return false;
            }

            EmailError = "";
            return true;
        }

        private bool ValidateTelephone(string telephone)
        {
            if (string.IsNullOrEmpty(telephone))
            {
                TelephoneError = TelephoneEmptyErrorMessage;
                return false;
            }

            if (telephone.Length < 12)
            {
                TelephoneError = TelephoneInvalidErorrMessage;
                return false;
            }

            TelephoneError = "";
            return true;
        }

        private bool ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                PasswordError = PasswordEmptyErrorMessage;
                return false;
            }

            if (password.Length < 8 || !Regex.IsMatch(password, @"[a-zA-Z]") || !Regex.IsMatch(password, @"\d"))
            {
                PasswordError = PasswordInvalidErrorMessage;
                return false;
            }

            PasswordError = "";
            return true;
        }

    }
}
