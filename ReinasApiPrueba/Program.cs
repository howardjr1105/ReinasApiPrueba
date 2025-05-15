using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ReinasApiPrueba;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Reinas API", Version = "v1" });

    // Forzar application/json como tipo por defecto
    c.SupportNonNullableReferenceTypes();

    // Elimina o evita application/json-patch+json
    c.MapType<object>(() => new OpenApiSchema { Type = "object" });
});


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // Cambia esto por el origen permitido
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

// Add swagger for API documentation (optional)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Reinas API v1");
    });

}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.UseRouting(); // Asegúrate de que esté antes de mapear los endpoints

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<NotificationHub>("/notificationHub"); // Aquí mapeas el Hub de SignalR
});



app.Run();