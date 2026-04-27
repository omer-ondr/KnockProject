using KnockProject.Core.Interfaces;
using KnockProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using KnockProject.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabanı ve Vector eklentisi bağlantısı (Eksik olan UseVector buraya eklendi)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.UseVector() 
    ));

builder.Services.AddScoped<IEmbeddingService, MockEmbeddingService>();
builder.Services.AddScoped<ILlmService, MockLlmService>();
builder.Services.AddScoped<IImageService, MockImageService>();

// 2. Projemizde Controller kullanacağımızı sisteme söylüyoruz
builder.Services.AddControllers();

// OpenAPI/Swagger desteği
builder.Services.AddOpenApi();

var app = builder.Build();

// HTTP request pipeline yapılandırması
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// 3. İstekleri Controller sınıflarına yönlendiriyoruz
app.MapControllers();

app.Run();