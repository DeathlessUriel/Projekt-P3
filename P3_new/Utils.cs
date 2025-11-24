using System;
using System.Text.RegularExpressions;

namespace TextRpg
{
    public static class Utils
    {
        public static readonly Random RNG = new Random();

        public static bool ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            var re = new Regex(@"^[A-Za-zĄĘÓŁŻŹĆŃąćęłóźż\- ]{2,20}$");
            return re.IsMatch(name);
        }
    }
}
