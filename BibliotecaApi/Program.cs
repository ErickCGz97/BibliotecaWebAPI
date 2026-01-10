using System.Text.Json.Serialization;
using BibliotecaApi.Datos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

//Inicio Area de servicios

//AutoMapper version 13.0.1
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddDbContext<ApplicationDbContext>(opciones => opciones.UseSqlServer("name=DefaultConnection"));


//Fin area de servicios

var app = builder.Build();

//Area de middlewares
app.MapControllers();

app.Run();


//Comandos de instalcion de paquetes requerios en VS Code
/*

dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson --version 9.0.1

Migracion de tabla Comentarios
dotnet ef migrations add TablaComentarios
dotnet ef database update

Migracion tabla AutoresLibros (Relacion Muchos a Muchos)
dotnet ef migrations add TablaAutoresLibros
*/
  