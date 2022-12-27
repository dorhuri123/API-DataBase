
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<MySqlConnector.MySqlConnection>
    (_ => new MySqlConnector.MySqlConnection(builder.Configuration["ConnectionStrings:Default"]));

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
            .AllowAnyMethod()
            .SetIsOriginAllowed(origin => true)
            .AllowAnyHeader()
            .AllowCredentials();
        });
});



var app = builder.Build();

app.UseRouting();

app.UseCors("AllowAll");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
