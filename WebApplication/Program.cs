using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Services;
using System.Text.Json.Serialization;
using WebApplication.Services.Interfaces;
using WebApplication.Services.ModelServices;
using System.Security.Claims;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddHttpClient<IScraperService, ScraperService>();
//builder.Services.AddScoped<TimetableCrawler>();
builder.Services.AddScoped<IAcademicYearService, AcademicYearService>();
builder.Services.AddScoped<IBuildingsService, BuildingsService>();
builder.Services.AddScoped<IFacultiesService, FacultiesService>();
builder.Services.AddScoped<IFieldsOfStudiesService, FieldsOfStudiesService>();
builder.Services.AddScoped<IGroupsService, GroupsService>();
builder.Services.AddScoped<IRoomsService, RoomsService>();
builder.Services.AddScoped<IScheduleChangesService, ScheduleChangesService>();
builder.Services.AddScoped<ISchedulerService, SchedulerService>();
builder.Services.AddScoped<ISemestersService, SemestersService>();
builder.Services.AddScoped<ISpecializationsService, SpecializationsService>();
builder.Services.AddScoped<IStudentGroupsService, StudentGroupsService>();
builder.Services.AddScoped<IStudentsService, StudentsService>();
builder.Services.AddScoped<ISubjectsService, SubjectsService>();
builder.Services.AddScoped<ITeachersService, TeachersService>();
builder.Services.AddScoped<ITimetablesService, TimetablesService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAdminService, AdminService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
})
.AddGoogle(options =>
{
    options.ClientId = "597549856575-hnmt5tg33ajlnhbuv937tiktpn3u2gfi.apps.googleusercontent.com";
    options.ClientSecret = "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";

    options.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents
    {
        OnTicketReceived = async context =>
        {
            var googleEmail = context.Principal?.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(googleEmail))
            {
                context.Response.Redirect("/Account/Login?error=BladGoogle");
                context.HandleResponse();
                return;
            }

            var dbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
            var dbUser = await dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == googleEmail);

            if (dbUser != null)
            {
                var lokalneClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, dbUser.Id.ToString()),
                    new Claim(ClaimTypes.Name, dbUser.Username),
                    new Claim(ClaimTypes.Email, dbUser.Email),
                    new Claim(ClaimTypes.Role, dbUser.Role.Name)
                };

                var identity = new ClaimsIdentity(lokalneClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                context.Principal = new ClaimsPrincipal(identity);
            }
            else
            {
                context.Principal = null;
                
                context.Response.Redirect("/Account/Login?error=BrakKontaWSystemie");
                context.HandleResponse(); 
            }
        }
    };
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

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Scheduler}/{action=Index}/{id?}");

app.Run();