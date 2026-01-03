using AutoMapper;
using BibliotecaApi.Datos;
using BibliotecaApi.DTOs;
using BibliotecaApi.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/libros")]
public class LibrosController: ControllerBase
{
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper1;


    public LibrosController(ApplicationDbContext context, IMapper mapper)
    {
        dbContext = context;
        mapper1 = mapper;
    }

    //Lista de elementos Libros
    [HttpGet]
    public async Task<IEnumerable<LibroDTO>> Get()
    {
        var libros = await dbContext.Libros.ToListAsync();
        var librosDTO = mapper1.Map<IEnumerable<LibroDTO>>(libros);
        return librosDTO;
    }

    //Obtener un elemento Libro por ID
    [HttpGet("{id:int}", Name = "ObtenerLibro")]
    public async Task<ActionResult<LibroConAutorDTO>> Get(int id)
    {
        var libro = await dbContext.Libros
        .Include(x => x.Autor)
        .FirstOrDefaultAsync(x => x.Id == id);
        
        if(libro is null)
        {
            return NotFound();
        }

        var libroDTO = mapper1.Map<LibroConAutorDTO>(libro);
        return libroDTO;
    }

    //Registrar nuevo Libro
    [HttpPost]
    public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
    {
        var libro = mapper1.Map<Libro>(libroCreacionDTO);

        dbContext.Add(libro);
        await dbContext.SaveChangesAsync();

        var libroDTO = mapper1.Map<LibroDTO>(libro);
        return CreatedAtRoute("ObtenerLibro", new {id = libro.Id}, libroDTO);
    }

    //Actualizar un valor existente
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
    {
        var libro = mapper1.Map<Libro>(libroCreacionDTO);

        libro.Id = id;

        dbContext.Update(libro);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    //Eliminar valor existente
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var registrosBorrados = await dbContext.Libros.Where(x => x.Id == id).ExecuteDeleteAsync();
        if(registrosBorrados == 0)
        {
            return NotFound();
        }

        return NoContent();
    }

}