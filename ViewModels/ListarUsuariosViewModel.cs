using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ProyectoFinal_ISII.Models;

namespace ProyectoFinal_ISII.ViewModels;

public class ListarUsuariosViewModel 
{
    public List<Usuario> Usuarios { get; set; }

    public ListarUsuariosViewModel() {}

    public ListarUsuariosViewModel(List<Usuario> usuarios)
    {
        Usuarios = usuarios;
    }
    
}