using System.Linq;

namespace WpfApp.Services
{
    public class CpfValidator
    {
        public static bool IsValid(string cpf)
        {
            if (cpf == null) return false;

            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            if (cpf.Length != 11) return false;

            var invalid = new[] {
                "00000000000",
                "11111111111",
                "22222222222",
                "33333333333",
                "44444444444",
                "55555555555",
                "66666666666",
                "77777777777",
                "88888888888",
                "99999999999"};

            if (invalid.Contains(cpf)) return false;

            int Calc(int len)
            {
                int sum = 0; for (int i = 0; i < len; i++) sum += (cpf[i] - '0') * (len + 1 - i);
                int r = sum % 11; return r < 2 ? 0 : 11 - r;
            }

            return Calc(9) == (cpf[9] - '0') && Calc(10) == (cpf[10] - '0');
        }
    }
}