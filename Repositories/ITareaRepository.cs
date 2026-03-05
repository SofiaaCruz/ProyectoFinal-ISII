using ProyectoFinal_ISII.Models;

namespace ProyectoFinal_ISII.Repositories;

public interface ITareaRepository 
{
    Tarea Create(int idTablero, Tarea tarea);
    void Update(int id, Tarea tarea);
    List<Tarea> GetAll();
    Tarea GetById(int id);
    List<Tarea> GetByUsuarioId(int idUsuario);
    List<Tarea> GetByTableroId(int idTablero);
    int Delete(int idTarea);
    void DeleteByTableroId(int idTablero);
    void SetDefaultUsuarioId(int idUsuario);
    void Assign(int idUsuario, int idTarea);
}