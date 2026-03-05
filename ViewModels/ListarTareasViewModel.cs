using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ProyectoFinal_ISII.Models;

namespace ProyectoFinal_ISII.ViewModels;

public class ListarTareasViewModel 
{
    public List<Tarea> Tareas { get; set; }

    public List<Tarea> TareasCreadas { get; set; }

    public int IdUsuario { get; set; }

    public Tarea TareaSeleccionada { get; set; }

    public List<Tablero> TablerosRegistrados { get; set; }

    public List<Usuario> UsuariosRegistrados { get; set; }

    public int IdTablero { get; set; }

    public ListarTareasViewModel() 
    {
        Tareas = new();
        TareasCreadas = new();
    }

    public ListarTareasViewModel(List<Tarea> tareas, List<Tarea> tareasCreadas, int idUsuario) 
    {
        Tareas = tareas;
        TareasCreadas = tareasCreadas;
        IdUsuario = idUsuario;
    }

    public ListarTareasViewModel(List<Tarea> tareas, List<Tarea> tareasCreadas, int idUsuario, List<Usuario> usuarios, List<Tablero> tableros) 
    {
        Tareas = tareas;
        TareasCreadas = tareasCreadas;
        IdUsuario = idUsuario;
        UsuariosRegistrados = usuarios;
        TablerosRegistrados = tableros;
    }

    public ListarTareasViewModel(List<Tarea> tareas, List<Tarea> tareasCreadas, int idUsuario, List<Usuario> usuarios, List<Tablero> tableros, int idTablero) 
    {
        Tareas = tareas;
        TareasCreadas = tareasCreadas;
        IdUsuario = idUsuario;
        UsuariosRegistrados = usuarios;
        TablerosRegistrados = tableros;
        IdTablero = idTablero;
    }

    public ListarTareasViewModel(List<Tarea> tareas, int idUsuario)
    {
        Tareas = tareas;
        IdUsuario = idUsuario;
    }

    public ListarTareasViewModel(List<Tarea> tareas, int idUsuario, List<Usuario> usuarios, List<Tablero> tableros)
    {
        Tareas = tareas;
        IdUsuario = idUsuario;
        UsuariosRegistrados = usuarios;
        TablerosRegistrados = tableros;
    }

    public ListarTareasViewModel(List<Tarea> tareas, int idUsuario, List<Usuario> usuarios, List<Tablero> tableros, int idTablero)
    {
        Tareas = tareas;
        IdUsuario = idUsuario;
        UsuariosRegistrados = usuarios;
        TablerosRegistrados = tableros;
        IdTablero = idTablero;
    }

    public ListarTareasViewModel(Tarea tarea, int idUsuario)
    {
        TareaSeleccionada = tarea;
        IdUsuario = idUsuario;
    }

    public ListarTareasViewModel(Tarea tarea, int idUsuario, List<Usuario> usuarios, List<Tablero> tableros)
    {
        TareaSeleccionada = tarea;
        IdUsuario = idUsuario;
        UsuariosRegistrados = usuarios;
        TablerosRegistrados = tableros;
    }

}