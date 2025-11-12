using System;
using System.Text.RegularExpressions;

namespace TextRpg
{
    public static class Utils
    {
        // Poprawka: używamy stringa verbatim (@) aby nie mieć problemów z escape'ami
        // oraz unikamy błędu CS1009 (unrecognized escape sequence).
        public static bool ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            var re = new Regex(@"^[A-Za-zĄĘÓŁŻŹĆŃąćęłóźż\- ]{2,20}$");
            return re.IsMatch(name);
        }
    }
}