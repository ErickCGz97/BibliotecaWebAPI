using System.Text.Json.Serialization;
using BibliotecaApi.Datos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

//Inicio Area de servicios

//AutoMapper version 13.0.1
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers().AddJsonOptions(opciones => opciones.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddDbContext<ApplicationDbContext>(opciones => opciones.UseSqlServer("name=DefaultConnection"));


//Fin area de servicios

var app = builder.Build();

//Area de middlewares
app.MapControllers();

app.Run();
