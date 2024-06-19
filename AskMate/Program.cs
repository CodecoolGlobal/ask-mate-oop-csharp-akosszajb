using AskMate.Middleware;

// var builder = WebApplication.CreateBuilder(args);
//
// // Add services to the container.
//
// builder.Services.AddControllers();
// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
//
// var app = builder.Build();
//
// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
//
// app.UseHttpsRedirection();
//
// app.UseCorsMiddleware();
//
// app.UseAuthorization();
//
// app.MapControllers();
//
// app.Run();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add session services with configuration
builder.Services.AddDistributedMemoryCache(); // Uses a memory cache for storage of session data
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Set how long the session lasts
    options.Cookie.HttpOnly = true;  // Enhances security by preventing client-side scripts from accessing the cookie
    options.Cookie.IsEssential = true;  // Marks the session cookie as essential for the application to function
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCorsMiddleware();  // Ensure your custom CORS middleware is still appropriately placed

app.UseSession();  // Activate session handling

app.UseAuthMiddleware();

app.UseAuthorization();

app.MapControllers();

app.Run();
