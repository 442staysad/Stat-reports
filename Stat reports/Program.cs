using Core.Entities;
using Core.Interfaces;
using Core.Services;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Infrastructure.Data;
using Infrastructure.Repository;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Stat_reports.Filters;
using Stat_reportsnt.Filters;

var builder = WebApplication.CreateBuilder(args);

// ����������� � �� (������ ������ ����������� �� ����)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ����������� ����������� (Generic Repository)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// ��������� ������� (������� �� �����)
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IExcelSplitterService, ExcelSplitterService>();

//ilder.Services.AddHostedService<ReportDeadlineCheckerHostedService>();
builder.Services.AddSingleton<AdminAuthFilter>();
builder.Services.AddSingleton<AuthorizeBranchAndUserAttribute>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISummaryReportService,SummaryReportService>();

// ��������� MVC
builder.Services.AddScoped<IFileService,FileService>();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPasswordHasher<Branch>, PasswordHasher<Branch>>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
var app = builder.Build();

// ��������� Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
//builder.Services.AddSession();
//app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ReportMvc}/{action=Index}/{id?}");
app.Run();


