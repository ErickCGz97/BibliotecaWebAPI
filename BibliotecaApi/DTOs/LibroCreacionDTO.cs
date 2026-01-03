using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.DTOs;

public class LibroCreacionDTO
{
    [Required]
    [StringLength(250, ErrorMessage ="EL campo {0} deve tener {1} caracteres o menos")]
    public required string Titulo {get;set;}

    public int AutorId {get;set;}
}