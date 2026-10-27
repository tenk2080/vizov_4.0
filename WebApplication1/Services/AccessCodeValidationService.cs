namespace WebApplication1  
{
    public interface IAccessCodeValidationService
    {
        ValidationResult Validate(string? code);
    }

    public class AccessCodeValidationService : IAccessCodeValidationService
    {
        private const string SpecialChars = "!@#$%";

        public ValidationResult Validate(string? code)
        {
            var result = new ValidationResult();

            if (string.IsNullOrWhiteSpace(code))
            {
                result.Errors.Add("Код доступа отсутствует или пустой.");
                result.IsValid = false;
                result.Message = "Код отклонён.";
                return result;
            }

            // Длина — от 8 до 16 символов включительно
            if (code.Length < 8 || code.Length > 16)
            {
                result.Errors.Add("Длина кода должна быть от 8 до 16 символов.");
            }

            // Нет пробелов
            if (code.Contains(' '))
            {
                result.Errors.Add("Код не должен содержать пробелов.");
            }

            // Первый символ — буква
            if (!char.IsLetter(code[0]))
            {
                result.Errors.Add("Первый символ должен быть буквой.");
            }

            // Последний символ — цифра или разрешённый специальный символ
            char last = code[^1];
            if (!char.IsDigit(last) && !SpecialChars.Contains(last))
            {
                result.Errors.Add("Последний символ должен быть цифрой или одним из символов: !, @, #, $, %.");
            }

            bool hasUpper = false;
            bool hasLower = false;
            bool hasDigit = false;
            bool hasSpecial = false;

            int sameCount = 1;
            char prev = code[0];
            int digitSeqLen = 0;
            char? prevDigit = null;

            for (int i = 0; i < code.Length; i++)
            {
                char c = code[i];

                if (c >= 'A' && c <= 'Z') hasUpper = true;
                if (c >= 'a' && c <= 'z') hasLower = true;

                if (char.IsDigit(c))
                {
                    hasDigit = true;
                    if (prevDigit.HasValue && c == prevDigit + 1)
                    {
                        digitSeqLen++;
                    }
                    else
                    {
                        digitSeqLen = 1;
                    }
                    prevDigit = c;

                    if (digitSeqLen >= 3)
                    {
                        string msg = "Обнаружена последовательность из трёх или более подряд идущих цифр.";
                        if (!result.Errors.Contains(msg))
                            result.Errors.Add(msg);
                    }
                }
                else
                {
                    prevDigit = null;
                    digitSeqLen = 0;
                }

                if (SpecialChars.Contains(c)) hasSpecial = true;

                if (i > 0 && c == prev)
                {
                    sameCount++;
                }
                else
                {
                    sameCount = 1;
                }
                prev = c;

                if (sameCount >= 3)
                {
                    string msg = "Обнаружено три или более одинаковых символа подряд.";
                    if (!result.Errors.Contains(msg))
                        result.Errors.Add(msg);
                }
            }

            if (!hasUpper) result.Errors.Add("Нет заглавной латинской буквы.");
            if (!hasLower) result.Errors.Add("Нет строчной латинской буквы.");
            if (!hasDigit) result.Errors.Add("Нет цифры.");
            if (!hasSpecial) result.Errors.Add("Нет специального символа.");

            result.IsValid = result.Errors.Count == 0;
            result.Message = result.IsValid
                ? "Код принят. Ворота открываются."
                : "Код отклонён.";

            return result;
        }
    }
}
