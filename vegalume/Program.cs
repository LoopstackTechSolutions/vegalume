using vegalume.Repositorio;

var builder = WebApplication.CreateBuilder(args);

// Explicitly set URL from environment variable (set via launchSettings.json profiles)
var url = builder.Configuration["applicationUrl"];
if (!string.IsNullOrEmpty(url))
{
    builder.WebHost.UseUrls(url);
}

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ClienteRepositorio>();
builder.Services.AddScoped<FuncionarioRepositorio>();
builder.Services.AddScoped<PedidoRepositorio>();
builder.Services.AddScoped<PratoRepositorio>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
