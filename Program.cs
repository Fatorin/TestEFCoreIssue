using Microsoft.EntityFrameworkCore;
using Npgsql;
using TestEFCoreIssue.ApplicationContext;
using TestEFCoreIssue.Model;

var builder = WebApplication.CreateBuilder(args);

var connectstring = "Host=localhost:5432;Database=postgres;Username=postgres;Password=postgres;Keepalive=30;ConnectionLifetime=15;";
var npgsqlBuild = new NpgsqlDataSourceBuilder(connectstring).EnableDynamicJson().Build();
builder.Services.AddDbContext<MyDbContext>(options => options.UseNpgsql(npgsqlBuild));
builder.Services.AddDbContext<MyDbContext>(opt => opt.UseNpgsql("TodoList"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/mapinfos/{id}", async (int id, MyDbContext dbContext) =>
{
    var mapInfo = await dbContext.MapInfo.FindAsync(id);
    return mapInfo != null ? Results.Ok(mapInfo) : Results.NotFound();
})
.WithOpenApi();

app.MapGet("/mapinfos/tags", async (MapTag[] tags, MyDbContext dbContext) =>
{
    var mapInfo = await dbContext.MapInfo
        .Where(map => map.Tags != null && tags.All(tag => map.Tags.Contains(tag)))
        .ToListAsync();

    return mapInfo != null ? Results.Ok(mapInfo) : Results.NotFound();
})
.WithOpenApi();

app.MapPost("/mapinfos", async (MapInfo mapInfo, MyDbContext dbContext) =>
{
    await dbContext.MapInfo.AddAsync(mapInfo);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/mapinfos/{mapInfo.Id}", mapInfo);
})
.WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
