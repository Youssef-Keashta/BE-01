using BE_01.Security;
using Microsoft.AspNetCore.Authentication;

DotNetEnv.Env.Load();


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<SupabaseAuthService>(client =>
{
    var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL");
    client.BaseAddress = new Uri(supabaseUrl);
    client.DefaultRequestHeaders.Add("apikey", Environment.GetEnvironmentVariable("SUPABASE_KEY"));
});

builder.Services.AddAuthentication("Supabase")
    .AddScheme<AuthenticationSchemeOptions, SupabaseAuthHandler>("Supabase", options => { });

builder.Services.AddAuthorization();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Initialize the database
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
BE_01.Data.TaskDatabase.Initialize(connectionString);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new
{
    name = "Task API",
    version = "1.0",
    endpoints = new[] { "/tasks" }
}));

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapControllers();

app.Run();

