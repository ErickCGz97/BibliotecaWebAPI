using AutoMapper;
using BibliotecaApi.Datos;
using BibliotecaApi.DTOs;
using BibliotecaApi.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/libros")]
public class LibrosController : ControllerBase
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
    public async Task<ActionResult<LibroConAutoresDTO>> Get(int id)
    {
        var libro = await dbContext.Libros
        .Include(x => x.Autores)
            .ThenInclude(x => x.Autor)
        .FirstOrDefaultAsync(x => x.Id == id);

        if (libro is null)
        {
            return NotFound();
        }

        var libroDTO = mapper1.Map<LibroConAutoresDTO>(libro);
        return libroDTO;
    }

    [HttpPost]
    public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
    {
        // Se asegura que el cliente envíe al menos un autor para el libro.
        // Si no hay IDs de autores, se devuelve un 422 ValidationProblem con un mensaje claro.
        if (libroCreacionDTO.AutoresIds is null || libroCreacionDTO.AutoresIds.Count == 0)
        {
            ModelState.AddModelError(nameof(libroCreacionDTO.AutoresIds), "No se puede registrar un libro sin autores");
            return ValidationProblem();
        }

        //Se consulta en la base de datos cuáles de los IDs enviados realmente existen.
        //Esto evita que se creen relaciones con autores inexistentes.
        var autoresIdsExisten = await dbContext.Autores
            .Where(x => libroCreacionDTO.AutoresIds.Contains(x.Id))
            .Select(x => x.Id).ToListAsync();

        //Si hay IDs inválidos, se construye un mensaje indicando cuáles no existen.
        if (autoresIdsExisten.Count != libroCreacionDTO.AutoresIds.Count)
        {
            var autoresNoExisten = libroCreacionDTO.AutoresIds.Except(autoresIdsExisten);
            var autoresNoExistenString = string.Join(",", autoresNoExisten);
            var mensajeDeError = $"Los siguientes autores no existen: {autoresNoExistenString}";
            ModelState.AddModelError(nameof(libroCreacionDTO.AutoresIds), mensajeDeError);
            return ValidationProblem();
        }

        var libro = mapper1.Map<Libro>(libroCreacionDTO);
        AsignarOrdenAutores(libro);

        dbContext.Add(libro);
        await dbContext.SaveChangesAsync();

        var libroDTO = mapper1.Map<LibroDTO>(libro);

        return CreatedAtRoute("ObtenerLibro", new { id = libro.Id }, libroDTO);
    }

    private void AsignarOrdenAutores(Libro libro)
    {
        if (libro.Autores is not null)
        {
            for (int i = 0; i < libro.Autores.Count; i++)
            {
                libro.Autores[i].Orden = i;
            }
        }
    }

    //Actualizar un valor existente
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
    {

        // Se asegura que el cliente envíe al menos un autor para el libro.
        // Si no hay IDs de autores, se devuelve un 422 ValidationProblem con un mensaje claro.
        if (libroCreacionDTO.AutoresIds is null || libroCreacionDTO.AutoresIds.Count == 0)
        {
            ModelState.AddModelError(nameof(libroCreacionDTO.AutoresIds), "No se puede registrar un libro sin autores");
            return ValidationProblem();
        }

        //Se consulta en la base de datos cuáles de los IDs enviados realmente existen.
        //Esto evita que se creen relaciones con autores inexistentes.
        var autoresIdsExisten = await dbContext.Autores
            .Where(x => libroCreacionDTO.AutoresIds.Contains(x.Id))
            .Select(x => x.Id).ToListAsync();

        //Si hay IDs inválidos, se construye un mensaje indicando cuáles no existen.
        if (autoresIdsExisten.Count != libroCreacionDTO.AutoresIds.Count)
        {
            var autoresNoExisten = libroCreacionDTO.AutoresIds.Except(autoresIdsExisten);
            var autoresNoExistenString = string.Join(",", autoresNoExisten);
            var mensajeDeError = $"Los siguientes autores no existen: {autoresNoExistenString}";
            ModelState.AddModelError(nameof(libroCreacionDTO.AutoresIds), mensajeDeError);
            return ValidationProblem();
        }

        var libroDB = await dbContext.Libros
                        .Include(x => x.Autores)
                        .FirstOrDefaultAsync(x => x.Id == id);

        if(libroDB is null)
        {
            return NotFound();
        }

        libroDB = mapper1.Map(libroCreacionDTO, libroDB);
        AsignarOrdenAutores(libroDB);

        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    //Eliminar valor existente
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var registrosBorrados = await dbContext.Libros.Where(x => x.Id == id).ExecuteDeleteAsync();
        if (registrosBorrados == 0)
        {
            return NotFound();
        }

        return NoContent();
    }

}