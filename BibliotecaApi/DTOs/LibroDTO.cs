namespace BibliotecaApi.DTOs;

public class LibroDTO
{
    public int Id {get;set;}
    public required string Titulo {get;set;}
    public int AutorId {get;set;}

    //POsible insercion y llamado de propiedad nombre de autor 
}