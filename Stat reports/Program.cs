using Core.Entities;
using Core.Interfaces;
using Core.Services;

using Infrastructure.Data;
using Infrastructure.Repository;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Stat_reports.Filters;
using Stat_reportsnt.Filters;

var builder = WebApplication.CreateBuilder(args);
// Подключение к БД (замени строку подключения на свою)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрация репозитория (Generic Repository)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Добавляем сервисы (добавим их позже)
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReportTemplateService, ReportTemplateService>();
builder.Services.AddScoped<IExcelSplitterService, ExcelSplitterService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISummaryReportService, SummaryReportService>();
builder.Services.AddScoped<IDeadlineService, DeadlineService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddHostedService<DeadlineNotificationHostedService>();

builder.Services.AddSingleton<AdminAuthFilter>();
builder.Services.AddSingleton<AuthorizeBranchAndUserAttribute>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Настройка MVC
builder.Services.AddScoped<IFileService,FileService>();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPasswordHasher<Branch>, PasswordHasher<Branch>>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/BranchLogin"; // при попытке неавторизованного доступа
        options.AccessDeniedPath = "/Auth/AccessDenied"; // при запрете по ролям
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Если БД ещё не создана — создаём с миграциями
    if (dbContext.Database.GetPendingMigrations().Any())
    {
        dbContext.Database.Migrate();
    }

    // Проверка: есть ли данные в ключевых таблицах
    var hasRoles = await dbContext.SystemRoles.AnyAsync();
    var hasUsers = await dbContext.Users.AnyAsync();
    var hasBranches = await dbContext.Branches.AnyAsync();

    if (!hasRoles || !hasUsers || !hasBranches)
    {
        await DbSeeder.SeedAsync(scope.ServiceProvider);
    }
}

// Настройка Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
//builder.Services.AddSession();
//app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();




app.UseEndpoints(endpoints =>
{
    // Указываем маршруты для областей
    endpoints.MapAreaControllerRoute(
        name: "admin",
        areaName: "Admin",
        pattern: "Admin/{controller=Admin}/{action=Index}/{id?}");
});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ReportMvc}/{action=WorkingReports}/{id?}");
 app.Run();


