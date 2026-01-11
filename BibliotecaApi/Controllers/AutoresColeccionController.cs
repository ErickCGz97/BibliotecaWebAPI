using AutoMapper;
using BibliotecaApi.Datos;
using BibliotecaApi.DTOs;
using BibliotecaApi.Entidades;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/autores-coleccion")]
public class AutoresColeccionController: ControllerBase
{
    private readonly ApplicationDbContext applicationDbContext;
    private readonly IMapper mapper1;
    public AutoresColeccionController(ApplicationDbContext context, IMapper mapper)
    {
        applicationDbContext = context;
        mapper1 = mapper;
    }

    [HttpGet("{ids}", Name = "ObtenerAutoresPorIds")] // api/autores-coleccion/1,2,3
    public async Task<ActionResult<List<AutorConLibrosDTO>>> Get(string ids)
    {
        //Variable para agregar coleccion de ids 
        var idsColeccion = new List<int>();
        //recorrido de ids 
        foreach (var id in ids.Split(","))
        {
            //Conversion de ids string a int para guardar en coleccion 
            if(int.TryParse(id, out int idInt))
            {
                idsColeccion.Add(idInt);
            }
        }

        if(!idsColeccion.Any())
        {
            ModelState.AddModelError(nameof(ids), "Ningun Id fue encontrado");
            return ValidationProblem();
        }

        var autores = await applicationDbContext.Autores.Include(x => x.Libros)
            .ThenInclude(x => x.Libro)
            .Where(x => idsColeccion.Contains(x.Id))
            .ToListAsync();

        if(autores.Count != idsColeccion.Count)
        {
            return NotFound();
        }        

        var autoresDTO = mapper1.Map<List<AutorConLibrosDTO>>(autores);
        return autoresDTO;
    }


    //Creacion de coleccion de autores en un metodo POST
    [HttpPost]
    public async Task<ActionResult> Post(IEnumerable<AutorCreacionDTO> autorCreacionDTOs)
    {
        var autores = mapper1.Map<IEnumerable<Autor>>(autorCreacionDTOs);
        applicationDbContext.AddRange(autores);
        await applicationDbContext.SaveChangesAsync();

        var autoresDTO = mapper1.Map<IEnumerable<AutorDTO>>(autores);
        var ids = autores.Select(x => x.Id);
        var idsString = string.Join(",", ids);
        return CreatedAtRoute("ObtenerAutoresPorIds", new {ids = idsString}, autoresDTO);
    }
}