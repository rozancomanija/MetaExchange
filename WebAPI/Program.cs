using Contract.Service;
using ContractContract.Service;
using Services.Repositories;
using Services.Repositories.Implementation;
using Services.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI
builder.Services.AddScoped<IOrderBooksRepository, OrderBooksRepository>();
builder.Services.AddScoped<IOrderBooksService, OrderBooksService>();
builder.Services.AddScoped<IMetaExchangeRepository, MetaExchangeRepository>();
builder.Services.AddScoped<IMetaExchangeService, MetaExchangeService>();

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

app.Run();
