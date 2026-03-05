using ProyectoFinal_ISII.Models;

namespace ProyectoFinal_ISII.Repositories;

public interface IUsuarioRepository 
{
    void Create(Usuario usuario);
    void Update(int id, Usuario usuario);
    List<Usuario> GetAll();
    Usuario GetById(int id);
    int Delete(int id);
    Usuario GetLoggedUser(string nombre, string password);
}
