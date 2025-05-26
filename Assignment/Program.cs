using Assignment.Helpers;
using Assignment.Middleware;
using Assignment.Repository;
using Assignment.Repository.Interfaces;
using Assignment.Repository.MongoBase;
using Assignment.Services;
using Assignment.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));


builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IColumnService, ColumnService>();

builder.Services.AddSingleton<ITaskRepository, TaskRepository>();
builder.Services.AddSingleton<IColumnRepository, ColumnRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var columnRepo = scope.ServiceProvider.GetRequiredService<IColumnRepository>();
    await DefaultColumns.SeedDefaultColumnsAsync(columnRepo);
}

app.Run();
