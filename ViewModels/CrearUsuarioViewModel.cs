using System.ComponentModel;
using System.ComponentModel.DataAnnotations;        // Dependencias de validación. Validaciones del lado del servidor (Back-End)
using ProyectoFinal_ISII.Models;

namespace ProyectoFinal_ISII.ViewModels;

public class CrearUsuarioViewModel 
{
    [Display(Name = "ID")]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Este campo es requerido")]
    [Display(Name = "Nombre de usuario")]
    [StringLength(30, ErrorMessage = "El nombre de usuario no puede tener mas de 30 caracteres")]
    public string Nombre { get; set; }
    
    [Required(ErrorMessage = "Este campo es requerido")]
    [Display(Name = "Contraseña")]
    [StringLength(30, MinimumLength = 4, ErrorMessage = "La contraseña debe tener entre 8 y 30 caracteres")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Este campo es requerido")]
    [Display(Name = "Rol del usuario")]
    public Rol Rol { get; set; }

    public CrearUsuarioViewModel() {}

    public CrearUsuarioViewModel(Usuario usuario) 
    {
        Id = usuario.Id;
        Nombre = usuario.Nombre;
        Password = usuario.Password;
        Rol = usuario.Rol;
    }
}