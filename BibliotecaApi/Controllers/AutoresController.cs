using System.IO.Compression;
using AutoMapper;
using Azure;
using BibliotecaApi.Datos;
using BibliotecaApi.DTOs;
using BibliotecaApi.Entidades;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Controllers;

//Atributo para indicar que se refiere a un controlador de un web api
[ApiController]
//Ruta del controlador (Url a la que debe ser enviada la peticion HTTP para poder llamar a las acciones del controlador)
[Route("api/autores")]
public class AutoresController : ControllerBase
{
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper1;
    public AutoresController(ApplicationDbContext context, IMapper mapper)
    {
        dbContext = context;
        mapper1 = mapper;
    }

    //Lissta de valores
    [HttpGet]
    public async Task<IEnumerable<AutorDTO>> Get()
    {
        //Se obtiene la lista completa de entidades Autor desde la base de datos usando EF Core.
        var autores = await dbContext.Autores.ToListAsync();
        /*
        AutoMapper convierte la lista de entidades Autor en una lista de AutorDTO.
        Esto asegura que el cliente reciba solo los campos definidos en el DTO (ej. Id, Nombre), ocultando propiedades internas de la entidad.
        */
        var autoresDTO = mapper1.Map<IEnumerable<AutorDTO>>(autores);
        //Devuelve la lista de DTOs directamente al cliente en formato JSON.
        return autoresDTO;
    }
    
    //Obtener un solo valor basado en el id
    [HttpGet("{id:int}", Name = "ObtenerAutor")] //api/autores/id
    public async Task<ActionResult<AutorConLibrosDTO>> Get(int id)
    {
         var autor = await dbContext.Autores
         .Include(x => x.Libros)
            .ThenInclude(x => x.Libro)
         .FirstOrDefaultAsync(x => x.Id == id);

        if(autor is null)
        {
            return NotFound();
        }

        var autorDTO = mapper1.Map<AutorConLibrosDTO>(autor);
        return autorDTO;
    }

    //Registrar nuevo valor
    [HttpPost]
    public async Task<ActionResult> Post(AutorCreacionDTO autorCreacionDTO)
    {
        //AutoMapper convierte el DTO en la entidad Autor que entiende EF Core.
        var autor = mapper1.Map<Autor>(autorCreacionDTO);
        //Se guarda el nuevo autor en la base de datos.
        dbContext.Add(autor);
        await dbContext.SaveChangesAsync();   
        //Se convierte la entidad guardada en un AutorDTO (el DTO de salida), que puede tener campos distintos a los de creación (ej. incluir el Id).
        var autorDTO = mapper1.Map<AutorDTO>(autor);
        //Devuelve un 201 Created con la ruta para obtener el recurso recién creado y el DTO de salida.
        return CreatedAtRoute("ObtenerAutor", new {id = autor.Id}, autorDTO);
    }

    //Actualizar un valor existente
    [HttpPut("{id:int}")] // api/autores/id
    public async Task<ActionResult> Put(int id, AutorCreacionDTO autorCreacionDTO)
    {
        //Se convierte el DTO en la entidad Autor.
        var autor = mapper1.Map<Autor>(autorCreacionDTO);
        //Se asigna el Id recibido en la URL para indicar qué registro actualizar.
        autor.Id = id;
        //EF Core marca la entidad como modificada y actualiza el registro en la base de datos.
        dbContext.Update(autor);
        await dbContext.SaveChangesAsync();
        //Devuelve un 200 OK indicando que la operación fue exitosa.
        return NoContent();
    }

    //Accion Patch, actualizacion de datos especificos
    [HttpPatch("{id:int}")]
    public async Task<ActionResult> Patch(int id, JsonPatchDocument<AutorPatchDTO> patchDocument)
    {
        if(patchDocument is null)
        {
            return BadRequest();
        }

        var autorDB = await dbContext.Autores.FirstOrDefaultAsync(x => x.Id == id);
        if(autorDB is null)
        {
            return NotFound();
        }

        var autorPatchDTO = mapper1.Map<AutorPatchDTO>(autorDB);
        patchDocument.ApplyTo(autorPatchDTO, ModelState);

        var esValido = TryValidateModel(autorPatchDTO);

        if(!esValido)
        {
            return ValidationProblem();
        }

        mapper1.Map(autorPatchDTO, autorDB);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    //Eliminar valor existente
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var registrosBorrados = await dbContext.Autores.Where(x => x.Id == id).ExecuteDeleteAsync();
        if(registrosBorrados == 0)
        {
            return NotFound();
        }

        return NoContent();
    }
}