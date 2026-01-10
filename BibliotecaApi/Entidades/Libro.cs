using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.Entidades;

public class Libro
{
    public int Id {get; set;}

    [Required]
    [StringLength(250, ErrorMessage ="EL campo {0} deve tener {1} caracteres o menos")]
    public required string Titulo {get;set;}

    /*Propiedad navigacional o Propiedad de navegacion
        Permite traer data de una entidad relacionada
    */
    public List<AutorLibro> Autores {get;set;} = [];

    public List<Comentario> Comentarios { get; set; }   = [];
}