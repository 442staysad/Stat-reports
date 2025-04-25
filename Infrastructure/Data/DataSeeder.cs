using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Получаем password hasher
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

            if (!context.SystemRoles.Any())
            {
                var roles = new List<SystemRole>
            {
                new() { RoleName = "Admin" },
                new() { RoleName = "PEB" },
                new() { RoleName = "OBUnF" },
                new() { RoleName = "User" }
            };
                await context.SystemRoles.AddRangeAsync(roles);
                await context.SaveChangesAsync();
            }

            if (!context.Branches.Any())
            {
                var branches = new List<Branch>
            {
                new() { Name = "Филиал Минск", UNP = "100001" },
                new() { Name = "Филиал Гомель", UNP = "100002" },
                new() { Name = "Филиал Витебск", UNP = "100003" },
                new() { Name = "Филиал Брест", UNP = "100004" }
            };

                // Установка паролей филиалов
                foreach (var branch in branches)
                {
                    // Временно создаём юзера, чтобы воспользоваться hasher
                    var tempUser = new User();
                    branch.PasswordHash = hasher.HashPassword(tempUser, branch.UNP switch
                    {
                        "100001" => "minsk123",
                        "100002" => "gomel123",
                        "100003" => "vitebsk123",
                        "100004" => "brest123",
                        _ => "default"
                    });
                }

                await context.Branches.AddRangeAsync(branches);
                await context.SaveChangesAsync();
            }

            if (!context.Users.Any())
            {
                var roles = context.SystemRoles.ToDictionary(r => r.RoleName, r => r.Id);
                var branches = context.Branches.ToList();

                var users = new List<User>
            {
                new()
                {
                    UserName = "admin",
                    FullName = "Системный администратор",
                    Email = "admin@domain.by",
                    RoleId = roles["Admin"],
                    BranchId = branches.First(b => b.UNP == "100001").Id // Привязываем к филиалу Минск
                },
                new()
                {
                    UserName = "peb",
                    FullName = "Пользователь ПЭБ",
                    Email = "peb@domain.by",
                    RoleId = roles["PEB"],
                    BranchId = branches.First(b => b.UNP == "100002").Id 
                },
                new()
                {
                    UserName = "obunf",
                    FullName = "Пользователь ОБУиФ",
                    Email = "obunf@domain.by",
                    RoleId = roles["OBUnF"],
                    BranchId = branches.First(b => b.UNP == "100003").Id
                },
                new()
                {
                    UserName = "user1",
                    FullName = "Пользователь филиала Минск",
                    Email = "user1@domain.by",
                    RoleId = roles["User"],
                    BranchId = branches.First(b => b.UNP == "100004").Id
                }
            };

                // Установка паролей
                users[0].PasswordHash = hasher.HashPassword(users[0], "admin123");
                users[1].PasswordHash = hasher.HashPassword(users[1], "peb123");
                users[2].PasswordHash = hasher.HashPassword(users[2], "obunf123");
                users[3].PasswordHash = hasher.HashPassword(users[3], "user123");

                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
            }
        }
    }
}
