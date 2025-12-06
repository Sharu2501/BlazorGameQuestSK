using Microsoft.EntityFrameworkCore;
using BlazorGameAPI.Data;
using BlazorGameAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext avec PostgreSQL (Supabase)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Services
builder.Services.AddScoped<PlayerService>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<MonsterService>();
builder.Services.AddScoped<CombatService>();
builder.Services.AddScoped<DungeonService>();
builder.Services.AddScoped<ArtifactService>();
builder.Services.AddScoped<GameSessionService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<GameHistoryService>();
builder.Services.AddScoped<HighScoreService>();
builder.Services.AddScoped<GameSessionService>();

// Add Controllers
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();