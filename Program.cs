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
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Enter your Supabase access token. Example: \"Bearer eyJhbGci...\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

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

