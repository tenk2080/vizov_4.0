using Microsoft.OpenApi.Models;
using WebApplication1;
var builder = WebApplication.CreateBuilder(args);

// Добавляем контроллеры  
builder.Services.AddControllers();

// Добавляем Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Убираем DescribeAllParametersInQueryString — его нет в API Swashbuckle
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Gates API",
        Version = "v1",
        Description = "API для проверки кодов доступа к Вратам"
    });
});

// Регистрируем наш сервис валидации
builder.Services.AddScoped<IAccessCodeValidationService, AccessCodeValidationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
