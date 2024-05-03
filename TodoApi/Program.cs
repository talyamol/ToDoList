
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ToDoList", Version = "v1" });
});
builder.Services.AddSwaggerGenNewtonsoftSupport();

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"), 
                      Microsoft.EntityFrameworkCore.ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ToDoDB"))));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:3000")  // מוסיף מקור הלקוח שלך
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    c.RoutePrefix = string.Empty;
});

// הפעל את CORS רק על מסלולי ה-API
app.UseCors(builder => 
    builder.WithOrigins("http://localhost:3000")
           .AllowAnyMethod()
           .AllowAnyHeader()
);

app.MapGet("/tasks", async (ToDoDbContext dbContext) =>
{
    var tasks = await dbContext.Items.ToListAsync(); 
    return Results.Ok(tasks); 
});

// Endpoint for adding a new task
app.MapPost("/tasks", async (ToDoDbContext dbContext, Item item) =>
{
    dbContext.Items.Add(item);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/tasks/{item.Id}", item);
});

// Endpoint for updating a task
app.MapPut("/tasks/{id}", async (ToDoDbContext dbContext, int id, Item updatedItem) =>
{
    var existingItem = await dbContext.Items.FindAsync(id);
    if (existingItem == null)
        return Results.NotFound();

    existingItem.Name = updatedItem.Name;
    existingItem.IsComplete = updatedItem.IsComplete;

    await dbContext.SaveChangesAsync();
    return Results.Ok(existingItem);
});

// Endpoint for deleting a task
app.MapDelete("/tasks/{id}", async (ToDoDbContext dbContext, int id) =>
{
    var existingItem = await dbContext.Items.FindAsync(id);
    if (existingItem == null)
        return Results.NotFound();

    dbContext.Items.Remove(existingItem);
    await dbContext.SaveChangesAsync();
    
    return Results.NoContent();
});
app.MapGet("/", () => "ToDoList API is running");

app.Run();
