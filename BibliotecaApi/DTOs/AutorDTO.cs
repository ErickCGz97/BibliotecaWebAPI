namespace BibliotecaApi.DTOs;

/*
Los DTOs (Data Transfer Objects) son clases diseñadas para transportar datos entre capas de la aplicación sin exponer directamente las entidades del dominio o la base de datos. 
*/
public class AutorDTO
{
    public int Id {get;set;}
    public required string NombreCompleto {get;set;}

}