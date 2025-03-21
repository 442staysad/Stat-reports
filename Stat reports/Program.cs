using Core.Entities;
using Core.Interfaces;
using Core.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Repository;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Подключение к БД (замени строку подключения на свою)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрация репозитория (Generic Repository)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Добавляем сервисы (добавим их позже)
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IExcelReportMerger, ExcelReportMerger>();
//ilder.Services.AddHostedService<ReportDeadlineCheckerHostedService>();
builder.Services.AddScoped<IBranchAuthService, BranchAuthService>();
builder.Services.AddScoped<IUserAuthService, UserAuthService>();
builder.Services.AddScoped<ISummaryReportService,SummaryReportService>();
builder.Services.AddScoped<ISummaryReportRepository, SummaryReportRepository>();
// Настройка MVC
builder.Services.AddScoped<IFileService,FileService>();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPasswordHasher<Branch>, PasswordHasher<Branch>>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
var app = builder.Build();

// Настройка Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ReportMvc}/{action=Index}/{id?}");
app.Run();


