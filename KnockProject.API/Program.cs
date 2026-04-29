using KnockProject.Core.Interfaces;
using KnockProject.Infrastructure.Data;
using KnockProject.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabanı (pgvector)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.UseVector()
    ));

// 2. HttpClient — tek Pollinations istemcisi (HuggingFace kaldırıldı)
builder.Services.AddHttpClient("Pollinations", client =>
{
    client.Timeout = TimeSpan.FromSeconds(120); // Flux model 1024x1024 için yeterli
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// 3. AI Servisleri (Pollinations.ai — ücretsiz, key gerektirmez)
builder.Services.AddScoped<IEmbeddingService, MockEmbeddingService>(); // Bypass Python for live test
builder.Services.AddScoped<ILlmService, PollinationsLlmService>();
builder.Services.AddScoped<IImageService, PollinationsImageService>();

// 4. Controllers + OpenAPI / Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors();
// builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger UI → http://localhost:5101
// app.UseSwagger();
// app.UseSwaggerUI(c =>
// {
//     c.SwaggerEndpoint("/swagger/v1/swagger.json", "KnockProject API V1");
//     c.RoutePrefix = string.Empty;
// });

// app.UseHttpsRedirection(); // Kapalı — lokal geliştirme
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthorization();
app.MapControllers();

// Data Seeding
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated(); // Ensure tables are created
    var seedPath = Path.Combine(Directory.GetCurrentDirectory(), "seed.json");
    await DataSeeder.SeedAsync(db, seedPath);
}

app.Run();