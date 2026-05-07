var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<VerticalSlice_Backend.Features.Admin.AdminRepository>();
builder.Services.AddScoped<VerticalSlice_Backend.Features.CartAndFavorites.CartAndFavoritesRepository>();
builder.Services.AddScoped<VerticalSlice_Backend.Features.Identity.IdentityRepository>();
builder.Services.AddScoped<VerticalSlice_Backend.Features.Products.ProductsRepository>();
builder.Services.AddScoped<VerticalSlice_Backend.Features.Checkout.CheckoutRepository>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
