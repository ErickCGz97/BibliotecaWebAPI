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
        CreateMap<Autor, AutorPatchDTO>().ReverseMap();

        CreateMap<AutorLibro, LibroDTO>()
            .ForMember(dto => dto.Id, config => config.MapFrom(ent => ent.LibroId))
            .ForMember(dto => dto.Titulo, config => config.MapFrom(ent => ent.Libro!.Titulo));

        CreateMap<Libro,LibroDTO>();
        CreateMap<LibroCreacionDTO, Libro>()
            .ForMember(ent => ent.Autores, config => config.MapFrom(dto => dto.AutoresIds.Select(id => new AutorLibro {AutorId = id})));

        CreateMap<Libro,LibroConAutoresDTO>();

        CreateMap<AutorLibro,AutorDTO>()
            .ForMember(dto => dto.Id, config => config.MapFrom(ent => ent.AutorId))
            .ForMember(dto => dto.NombreCompleto, config => config.MapFrom(ent => MapearNombreApellidoAutor(ent.Autor!)));

        //Mapeo para la creacion de Libros en el Post de crecaion de Autores
        CreateMap<LibroCreacionDTO, AutorLibro>()
            .ForMember(ent => ent.Libro,
                config => config.MapFrom(dto => new Libro {Titulo = dto.Titulo}));

        CreateMap<ComentarioCreacionDTO, Comentario>();
        CreateMap<Comentario, ComentarioDTO>();  
        CreateMap<ComentarioPatchDTO, Comentario>().ReverseMap();
    }

    //private string MapearNombreApellidoAutor(Autor autor) => $"{autor.Nombres} {autor.Apellidos}";

    private string MapearNombreApellidoAutor(Autor autor)
    {
        return ($"{autor.Nombres} {autor.Apellidos}");
    }

}