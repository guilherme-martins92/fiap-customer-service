using System.Text.RegularExpressions;

namespace Fiap.CustomerService.Application.Common
{
    public static class FormatUtils
    {
        // Document (CPF/CNPJ)
        public static string FormatDocumentNumber(string document)
        {
            var digits = Unformat(document);
            if (digits.Length == 11)
                return Convert.ToUInt64(digits).ToString(@"000\.000\.000\-00"); // CPF
            if (digits.Length == 14)
                return Convert.ToUInt64(digits).ToString(@"00\.000\.000\/0000\-00"); // CNPJ
            return document;
        }

        public static string UnformatDocumentNumber(string document)
        {
            return Unformat(document);
        }

        // CEP
        public static string FormatPostalCode(string cep)
        {
            var digits = Unformat(cep);
            if (digits.Length == 8)
                return Convert.ToUInt32(digits).ToString(@"00000\-000");
            return cep;
        }

        public static string UnformatPostalCode(string cep)
        {
            return Unformat(cep);
        }

        // Phone
        public static string FormatPhoneNumber(string phone)
        {
            var digits = Unformat(phone);
            if (digits.Length == 10)
                return Convert.ToUInt64(digits).ToString(@"\(00\) 0000\-0000");
            if (digits.Length == 11)
                return Convert.ToUInt64(digits).ToString(@"\(00\) 00000\-0000");
            return phone;
        }

        public static string UnformatPhoneNumber(string phone)
        {
            return Unformat(phone);
        }

        // Helper: Remove all non-digit characters
        private static string Unformat(string value)
        {
            return Regex.Replace(value ?? string.Empty, @"\D", "");
        }
    }
}