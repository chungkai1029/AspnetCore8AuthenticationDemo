using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AspnetCore8AuthenticationDemo.Data;
using AspnetCore8AuthenticationDemo.Models;
using AspnetCore8AuthenticationDemo.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration["MySQL:AspnetCore8AuthenticationDemo"] ?? throw new InvalidOperationException("Connection string 'AccountDbContextConnection' not found.");

// Add services to the container.
builder.Services.AddControllersWithViews();

// To configure a connection with MySQL by EF Core context. 
builder.Services.AddDbContext<AccountDbContext>(
    options => options.UseMySQL(connectionString)
);

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<AccountDbContext>();

// To add Identity services to the container.
builder.Services.AddAuthorization();

// To confirm the email address and enables the user to log in.
builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
});

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<Email>(builder.Configuration.GetSection("Email"));

builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    })
    .AddMicrosoftAccount(microsoftOptions =>
    {
        microsoftOptions.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
        microsoftOptions.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Map identity route.
app.MapIdentityApi<IdentityUser>();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
