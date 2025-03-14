using Microsoft.AspNetCore.Mvc.Rendering;
using Core.DTO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models.Employee
{
    public class EmployeeModel
    {
        public int? Id { get; set; }
        [Required(ErrorMessage ="Введите имя")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я,.'-]+$", 
                        ErrorMessage ="Некорректное имя")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Введите имя")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я,.'-]+$",
                ErrorMessage = "Некорректная фамилия")]
        public string LastName { get; set; }
        [RegularExpression(@"^[a-zA-Zа-яА-Я,.'-]+$",
        ErrorMessage = "Некорректное отчество")]
        public string Patronymic { get; set; }

        [Required(ErrorMessage = "Назначьте роль")]
        public int RoleId { get; set; }
        [Required(ErrorMessage = "Введите рабочий Email.")]
        [EmailAddress(ErrorMessage = "Некорректный Email.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Длина адреса от 6 до 100 символов.")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный Email.")]
        public string WorkEmailAddress { get; set; }

        [RegularExpression(@"^([\+]?(?:00)?[0-9]{1,3}[\s.-]?[0-9]{1,12})([\s.-]?[0-9]{1,4}?)$", ErrorMessage = "Некорректный номер")]
        public string WorkPhoneNumber { get; set; }
        [EmailAddress(ErrorMessage = "Некорректный Email.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Длина адреса от 6 до 100 символов.")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный Email.")]
        public string PrivateEmailAddress { get; set; }

        [RegularExpression(@"^([\+]?(?:00)?[0-9]{1,3}[\s.-]?[0-9]{1,12})([\s.-]?[0-9]{1,4}?)$", ErrorMessage = "Некорректный номер")]
        public string PrivatePhoneNumber { get; set; }

        [StringLength(50, MinimumLength = 7, ErrorMessage = "Длина может быть от 7 до 50 символов включительно.")]
        public string Password { get; set; }
    }
}
