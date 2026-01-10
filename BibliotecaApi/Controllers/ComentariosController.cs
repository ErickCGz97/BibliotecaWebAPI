using System.IO.Compression;
using AutoMapper;
using BibliotecaApi.Datos;
using BibliotecaApi.DTOs;
using BibliotecaApi.Entidades;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/libros/{libroId:int}/comentarios")]
public class ComentariosController : ControllerBase
{
    private readonly ApplicationDbContext applicationDbContext1;
    private readonly IMapper mapper1;
    public ComentariosController(ApplicationDbContext applicationDbContext, IMapper mapper)
    {
        this.applicationDbContext1 = applicationDbContext;
        this.mapper1 = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId)
    {
        var existeLibro = await applicationDbContext1.Libros.AnyAsync(x => x.Id == libroId);
        if (!existeLibro)
        {
            return NotFound();
        }

        var comentarios = await applicationDbContext1.Comentarios
        .Where(x => x.LibroId == libroId)
        .OrderByDescending(x => x.FechaPublicacion)
        .ToListAsync();

        return mapper1.Map<List<ComentarioDTO>>(comentarios);
    }

    [HttpGet("{id}", Name = "ObtenerComentario")]
    public async Task<ActionResult<ComentarioDTO>> Get(Guid id)
    {
        var comentario = await applicationDbContext1.Comentarios.FirstOrDefaultAsync(x => x.Id == id);

        if (comentario is null)
        {
            return NotFound();
        }
        return mapper1.Map<ComentarioDTO>(comentario);
    }

    [HttpPost]
    public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO)
    {
        var existeLibro = await applicationDbContext1.Libros.AnyAsync(x => x.Id == libroId);

        if (!existeLibro)
        {
            return NotFound();
        }

        var comentario = mapper1.Map<Comentario>(comentarioCreacionDTO);
        comentario.LibroId = libroId;
        comentario.FechaPublicacion = DateTime.UtcNow;
        applicationDbContext1.Add(comentario);
        await applicationDbContext1.SaveChangesAsync();

        var comentarioDTO = mapper1.Map<ComentarioDTO>(comentario);
        return CreatedAtRoute("ObtenerComentario", new { id = comentario.Id, libroId }, comentarioDTO);
    }

    //Accion Patch, actualizacion de datos especificos
    [HttpPatch("{id}")]
    public async Task<ActionResult> Patch(Guid id, int libroId, JsonPatchDocument<ComentarioPatchDTO> patchDocument)
    {
        if (patchDocument is null)
        {
            return BadRequest();
        }

        var existeLibro = await applicationDbContext1.Libros.AnyAsync(x => x.Id == libroId);

        if (!existeLibro)
        {
            return NotFound();
        }

        var comentarioDB = await applicationDbContext1.Comentarios.FirstOrDefaultAsync(x => x.Id == id);
        if (comentarioDB is null)
        {
            return NotFound();
        }

        var comentarioPatchDTO = mapper1.Map<ComentarioPatchDTO>(comentarioDB);
        patchDocument.ApplyTo(comentarioPatchDTO, ModelState);

        var esValido = TryValidateModel(comentarioPatchDTO);

        if (!esValido)
        {
            return ValidationProblem();
        }

        mapper1.Map(comentarioPatchDTO, comentarioDB);
        await applicationDbContext1.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id, int libroId)
    {
        var existeLibro = await applicationDbContext1.Libros.AnyAsync(x => x.Id == libroId);

        if (!existeLibro)
        {
            return NotFound();
        }

        var registrosBorrados = await applicationDbContext1.Comentarios.Where(x => x.Id == id).ExecuteDeleteAsync();

        if(registrosBorrados == 0)
        {
            return NotFound();
        }

        return NoContent();
    }

}