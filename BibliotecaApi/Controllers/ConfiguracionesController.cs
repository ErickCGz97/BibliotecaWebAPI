using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApi.Controllers;

/*
TEORIA
Clase ConfiguracionesController.cs
Creacion: 19/1/2026
Tema: Aprendizaje de Configuraciones en Web API
*/

[ApiController]
[Route("api/configuraciones")]
public class ConfiguracionesController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly IConfigurationSection seccion_01;
    private readonly IConfigurationSection seccion_02;

    public ConfiguracionesController(IConfiguration configuration)
    {
        this.configuration = configuration;
        seccion_01 = configuration.GetSection("seccion_1");
        seccion_02 = configuration.GetSection("seccion_2");

    }

    [HttpGet]
    public ActionResult<string> Get()
    {
        var opcion1 = configuration["apellido"];

        var opcion2 = configuration.GetValue<string>("apellido")!;

        return opcion2;
    }

    [HttpGet("obtenertodos")]
    public ActionResult GetObtenerTodos()
    {
        var hijos = seccion_02.GetChildren().Select(x => $"{x.Key}: {x.Value}");
        return Ok(new {hijos});
    }

    [HttpGet("seccion_01")]
    public ActionResult GetSeccion01()
    {
        var nombre = seccion_01.GetValue<string>("nombre");
        var edad = seccion_01.GetValue<int>("edad");

        return Ok(new {nombre, edad});
    }

        [HttpGet("seccion_02")]
    public ActionResult GetSeccion02()
    {
        var nombre = seccion_02.GetValue<string>("nombre");
        var edad = seccion_02.GetValue<int>("edad");

        return Ok(new {nombre, edad});
    }

    [HttpGet("secciones")]
    public ActionResult<string> GetSeccion()
    {
        var opcion1 = configuration["ConnectionStrings:DefaultConnection"];

        var opcion2 = configuration.GetValue<string>("ConnectionStrings:DefaultConnection");

        var seccion = configuration.GetSection("ConnectionStrings");
        var opcion3 = seccion["DefaultConnection"];

        return opcion3!;
    }

    [HttpGet("proveedor")]
    public ActionResult GetProveedor()
    {
        var valor = configuration.GetValue<string>("quien-soy");
        return Ok(new {valor});
    }
}