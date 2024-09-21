using AutoMapper;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Implements;
using Repository.Interfaces;
using Service;
using Service.Implements;
using Service.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IRankService, RankService>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IMessageService, MessageService>();

// AutoMapper configuration
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
                     .AllowAnyHeader()
                     .AllowAnyMethod();
    });
});

//DBContext
builder.Services.AddDbContext<bef4qvhxkgrn0oa7ipg0Context>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
