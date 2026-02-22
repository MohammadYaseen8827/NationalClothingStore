using NationalClothingStore.Infrastructure.Data;
using NationalClothingStore.Infrastructure.Extensions;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Swagger UI for API documentation
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "National Clothing Store API",
        Description = "E-commerce API for National Clothing Store",
        Version = "v1",
        Contact = new()
        {
            Name = "API Support",
            Email = "support@nationalclothingstore.com"
        }
    });
    
    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    // Enable Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "National Clothing Store API v1");
        c.DocumentTitle = "National Clothing Store API";
        c.DefaultModelsExpandDepth(-1); // Collapse models by default
        c.DisplayRequestDuration();
        c.EnablePersistAuthorization();
    });
}

app.UseHttpsRedirection();

app.Run();
