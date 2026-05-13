using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<VerticalSlice_Backend.Common.DbConnectionFactory>();

builder.Services.AddScoped<VerticalSlice_Backend.Features.CartAndFavorites.CartAndFavoritesRepository>();
builder.Services.AddScoped<VerticalSlice_Backend.Features.Identity.IdentityRepository>();
builder.Services.AddScoped<VerticalSlice_Backend.Features.Products.ProductsRepository>();
builder.Services.AddScoped<VerticalSlice_Backend.Features.Checkout.CheckoutRepository>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
           policy.WithOrigins("http://localhost:4200")
                 .AllowAnyHeader()
                 .AllowAnyMethod());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
