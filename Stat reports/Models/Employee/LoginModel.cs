using System.ComponentModel.DataAnnotations;

namespace Web.Models.Employee
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Введите рабочий Email.")]
        [EmailAddress(ErrorMessage = "Некорректный Email.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Длина адреса от 6 до 100 символов.")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный Email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [StringLength(50, MinimumLength = 7, ErrorMessage = "Длина может быть от 7 до 50 символов включительно.")]
        public string Password { get; set; }
    }
}
