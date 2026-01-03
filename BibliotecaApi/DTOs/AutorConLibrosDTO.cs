namespace BibliotecaApi.DTOs;

//Herencia 
public class AutorConLibrosDTO : AutorDTO
{
    public List<LibroDTO> Libros {get;set;} = [];
}