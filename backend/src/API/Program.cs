using NationalClothingStore.Infrastructure.Data;
using NationalClothingStore.Infrastructure.Extensions;
using NationalClothingStore.API;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnet/swashbuckle
builder.Services.AddEndpointsApiExplorer();
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

// Initialize the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DatabaseInitializer.InitializeDatabaseAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "National Clothing Store API v1");
        c.DocumentTitle = "National Clothing Store API";
        c.DefaultModelsExpandDepth(-1); // Collapse models by default
        c.DisplayRequestDuration();
        c.EnablePersistAuthorization();
    });
}

app.UseHttpsRedirection();

app.Run();
