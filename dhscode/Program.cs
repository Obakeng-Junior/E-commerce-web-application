using CologneStore.ImageService;
using DHSOnlineStore.Data;
using DHSOnlineStore.Email;
using DHSOnlineStore.EmailService;
using DHSOnlineStore.ImageService;
using DHSOnlineStore.Repositories.Class;
using DHSOnlineStore.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using DHSOnlineStore.Repositories;
using CologneStore.Data;

var builder = WebApplication.CreateBuilder(args);

// Retrieve the connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection")
    ?? throw new InvalidOperationException("Connection string 'DatabaseConnection' not found.");

// Configure DbContext with RetryOnFailure
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5, // Number of retries before failing
            maxRetryDelay: TimeSpan.FromSeconds(10), // Delay between retries
            errorNumbersToAdd: null // Retry on common transient errors by default
        )));


// Identity configuration
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


#region Repository Injection
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<IStockRepository, StockRepository>();
builder.Services.AddTransient<ICartRepository, CartRepository>();
builder.Services.AddTransient<IOrderRepository, OrderRepository>();
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddTransient<ICustomerServiceBooking, CustomerServiceBookingRepository>();
builder.Services.AddScoped<ICustomerInquiryRepository, CustomerInquiryRepository>();
builder.Services.AddScoped<IContactFormRepository, ContactFormRepository>();
#endregion

// Configure email service and inject SmtpSettings from appsettings.json
//builder.Services.AddSingleton<IEmailService, EmailService>();
//builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    await DbSeeder.SeedDefaultData(scope.ServiceProvider);
//}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication(); // Make sure to add this for Identity support
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Cart}/{action=Checkout}");
    endpoints.MapControllerRoute(
        name: "payfast",
        pattern: "payfast/{action=Pay}",
        defaults: new { controller = "PayFast" });
});

app.Run();
