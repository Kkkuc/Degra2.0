using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Services;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);
/*
builder.Services.Scan(scan => scan
    .FromApplicationDependencies()
    .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Service")))
    .AsMatchingInterface()
    .WithScopedLifetime());
*/

builder.Services.AddControllersWithViews();
//builder.Services.AddScoped<TimetableCrawler>();
builder.Services.AddScoped<IAcademicYearService, AcademicYearService>();
builder.Services.AddScoped<IBuildingsService, BuildingsService>();
builder.Services.AddScoped<IFacultiesService, FacultiesService>();
builder.Services.AddScoped<IFieldsOfStudiesService, FieldsOfStudiesService>();
builder.Services.AddScoped<IGroupsService, GroupsService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath =
            "/Account/Login"; //ciastka jescze do ogarnieca - na te strony bedzie mozna wchodzic jak sie je zrobi
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();