using RumataShop.Context;
using RumataShop.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// DapperContext'i sisteme tanýtma faslý
builder.Services.AddSingleton<DapperContext>();

builder.Services.AddScoped<IUrunRepository, UrunRepository>();

builder.Services.AddScoped<IKategoriRepository, KategoriRepository>();

builder.Services.AddScoped<IMusteriRepository, MusteriRepository>();

builder.Services.AddScoped<ISiparisRepository, SiparisRepository>();
// kimlik doðrulama servisi
builder.Services.AddAuthentication("CookieAuth")
       .AddCookie("CookieAuth", config =>
       {
           config.LoginPath = "/Musteri/Giris"; // Giriþ yapmamýþ kiþi buraya atýlýr
           config.ExpireTimeSpan = TimeSpan.FromDays(7); // Beni 7 gün hatýrla
       });

var app = builder.Build();


// --- KESÝN TÜRKÇE AYARI ---
var defaultCulture = new System.Globalization.CultureInfo("tr-TR");
// Virgül ve Nokta karmaþasýný önlemek için kesin kurallar:
defaultCulture.NumberFormat.NumberDecimalSeparator = ",";
defaultCulture.NumberFormat.CurrencyDecimalSeparator = ",";
defaultCulture.NumberFormat.CurrencyGroupSeparator = ".";
defaultCulture.NumberFormat.NumberGroupSeparator = ".";

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(defaultCulture),
    SupportedCultures = new List<System.Globalization.CultureInfo> { defaultCulture },
    SupportedUICultures = new List<System.Globalization.CultureInfo> { defaultCulture }
};

app.UseRequestLocalization(localizationOptions);
// ---------------------------
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication(); // Kimlik Kontrolü (kimsin)
app.UseAuthorization(); // Yetki Kontrolü (Girebilir misin?)

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
    );

app.Run();
