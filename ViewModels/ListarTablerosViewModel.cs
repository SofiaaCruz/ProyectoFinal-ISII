using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ProyectoFinal_ISII.Models;

namespace ProyectoFinal_ISII.ViewModels;

public class ListarTablerosViewModel 
{
    public int IdUsuario { get; set; }

    public List<Tablero> Tableros { get; set; }

    public List<Tablero> TablerosPropios { get; set; }

    public List<Tablero> TablerosConTareasAsignadas { get; set; }

    public List<Usuario> UsuariosRegistrados { get; set; }

    public ListarTablerosViewModel() 
    {
        Tableros = new();
        TablerosPropios = new();
        TablerosConTareasAsignadas = new();
    }

    public ListarTablerosViewModel(List<Tablero> tableros, int idUsuario, List<Usuario> usuarios)
    {
        Tableros = tableros;
        IdUsuario = idUsuario;
        UsuariosRegistrados = usuarios;
    }

    public ListarTablerosViewModel(List<Tablero> tableros1, List<Tablero> tableros2, int idUsuario, List<Usuario> usuarios)
    {
        TablerosPropios = tableros1;
        TablerosConTareasAsignadas = tableros2;
        IdUsuario = idUsuario;
        UsuariosRegistrados = usuarios;
    }
}