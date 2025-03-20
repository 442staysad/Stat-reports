using Core.Interfaces;
using Core.Services;
using Infrastructure.Data;
using Infrastructure.Repository;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// ����������� � �� (������ ������ ����������� �� ����)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ����������� ����������� (Generic Repository)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// ��������� ������� (������� �� �����)
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IExcelReportMerger, ExcelReportMerger>();
builder.Services.AddHostedService<ReportDeadlineCheckerHostedService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
// ��������� MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ��������� Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads")),
    RequestPath = "/uploads"
});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Report}/{action=Index}/{id?}");

app.Run();


