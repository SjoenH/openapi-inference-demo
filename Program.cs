using OpenApiInferenceDemo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add Swashbuckle services for OpenAPI documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Map minimal API endpoints for comparison
app.MapDemoMinimalApiEndpoints();

// Map minimal API endpoints WITHOUT explicit Results<> types to test inference
app.MapDemoMinimalApiEndpointsWithoutExplicitTypes();

app.Run();
