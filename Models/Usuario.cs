namespace ProyectoFinal_ISII.Models;
using ProyectoFinal_ISII.ViewModels;

public enum Rol 
{
    administrador,
    operador
}

public class Usuario 
{

    private int id;
    private string nombre;
    private string password;
    private Rol rol;

    public int Id { get => id; set => id = value; }
    public string Nombre { get => nombre; set => nombre = value; }
    public string Password { get => password; set => password = value;}
    public Rol Rol { get => rol; set => rol = value;}

    public Usuario() {}

    public Usuario(CrearUsuarioViewModel usuarioVM)
    {
        Id = usuarioVM.Id;
        Nombre = usuarioVM.Nombre;
        Password = usuarioVM.Password;
        Rol = usuarioVM.Rol;
    }

    public Usuario(ModificarUsuarioViewModel usuarioVM)
    {
        Id = usuarioVM.Id;
        Nombre = usuarioVM.Nombre;
        Password = usuarioVM.Password;
        Rol = usuarioVM.Rol;
    }

}