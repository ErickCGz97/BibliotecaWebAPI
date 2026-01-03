using System.ComponentModel.DataAnnotations;
using BibliotecaApi.Validaciones;

namespace BibliotecaApi.Entidades;

public class Autor
{
    public int Id {get;set;}

    [Required(ErrorMessage = "El campo {0} es requerido")]
    [StringLength(150, ErrorMessage = "El campo {0} debe tener {1} caracteres como maximo")]
    [PrimeraLetraMayuscula]
    public required string Nombres {get;set;}

    [Required(ErrorMessage = "El campo {0} es requerido")]
    [StringLength(150, ErrorMessage = "El campo {0} debe tener {1} caracteres como maximo")]
    [PrimeraLetraMayuscula]
    public required string Apellidos {get;set;}    

    [StringLength(20, ErrorMessage = "El campo {0} debe tener {1} caracteres como maximo")]
    public required string? Identificacion {get;set;}
    public List<Libro> Libros {get;set;} = new List<Libro>();
}