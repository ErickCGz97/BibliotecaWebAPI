using AutoMapper;
using BibliotecaApi.DTOs;
using BibliotecaApi.Entidades;

namespace BibliotecaApi.Utilidades;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Autor,AutorDTO>()
        .ForMember(dto => dto.NombreCompleto, 
            config => config.MapFrom(autor => MapearNombreApellidoAutor(autor)));

        CreateMap<Autor,AutorConLibrosDTO>()
        .ForMember(dto => dto.NombreCompleto, 
            config => config.MapFrom(autor => MapearNombreApellidoAutor(autor)));

        CreateMap<AutorCreacionDTO , Autor>();

        //Mapper de Libros
        CreateMap<Libro,LibroConAutorDTO>()
        .ForMember(dto => dto.AutorNombre, 
            config => config.MapFrom(ent => MapearNombreApellidoAutor(ent.Autor!)));


        CreateMap<Libro,LibroDTO>();
        CreateMap<LibroCreacionDTO, Libro>();
    }

    //private string MapearNombreApellidoAutor(Autor autor) => $"{autor.Nombres} {autor.Apellidos}";

    private string MapearNombreApellidoAutor(Autor autor)
    {
        return ($"{autor.Nombres} {autor.Apellidos}");
    }

}