using System.Data.SQLite;
using ProyectoFinal_ISII.Models;
using ProyectoFinal_ISII.Exceptions;

namespace ProyectoFinal_ISII.Repositories;

public class UsuarioRepository : IUsuarioRepository 
{
    private readonly string _connectionString;
    private readonly ITareaRepository _tareaRepository;
    private readonly ITableroRepository _tableroRepository;

    public UsuarioRepository(string connectionString, ITareaRepository tareaRepository, ITableroRepository tableroRepository)
    {
        _connectionString = connectionString;
        _tareaRepository = tareaRepository;
        _tableroRepository = tableroRepository;
    }

    public void Create(Usuario usuario) 
    {
        try
        {    
            var query = @"INSERT INTO Usuario (nombre_de_usuario, password, rol) VALUES (@nombre_de_usuario, @password, @rol);";      // Consulta SQL

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString)) 
            {
                connection.Open();      // Inicio de conexión con la BD
                
                var command = new SQLiteCommand(query, connection);

                command.Parameters.Add(new SQLiteParameter("@nombre_de_usuario", usuario.Nombre));
                command.Parameters.Add(new SQLiteParameter("@password", usuario.Password));
                command.Parameters.Add(new SQLiteParameter("@rol", usuario.Rol));
                
                command.ExecuteNonQuery();      // Ejecuta comandos como las instrucciones INSERT, DELETE, UPDATE.

                connection.Close();     // Fin de conexión con la BD
            }
        }
        catch(Exception ex)
        {
            throw new ElementNotCreatedException("Error al crear el usuario en la base de datos", ex);
        }
    }

    public void Update(int id, Usuario usuario) 
    {
        try
        {    
            var query = @"UPDATE Usuario SET nombre_de_usuario = @nuevo_nombre, password = @password, rol = @rol WHERE id = @id_buscado;";

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString)) 
            {
                connection.Open();

                var command = new SQLiteCommand(query, connection);

                command.Parameters.Add(new SQLiteParameter("@id_buscado", id));
                command.Parameters.Add(new SQLiteParameter("@nuevo_nombre", usuario.Nombre));
                command.Parameters.Add(new SQLiteParameter("@password", usuario.Password));
                command.Parameters.Add(new SQLiteParameter("@rol", usuario.Rol));

                command.ExecuteNonQuery();
                
                connection.Close();
            }
        }
        catch(Exception ex)
        {
            throw new ElementNotCreatedException($"Error al actualizar el usuario (ID: {id}) en la base de datos. Detalles: {ex.Message}", ex);
        }
    }

    public List<Usuario> GetAll() 
    {
        List<Usuario> usuarios = new();
        
        try
        {    
            var query = @"SELECT * FROM Usuario;";
            
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString)) 
            {

                connection.Open();

                var command = new SQLiteCommand(query, connection);

                using (var reader = command.ExecuteReader()) 
                {      // Lectura de tuplas

                    while (reader.Read()) 
                    {

                        var usuario = new Usuario();

                        usuario.Id = Convert.ToInt32(reader["id"]);
                        usuario.Nombre = reader["nombre_de_usuario"].ToString();
                        usuario.Password = reader["password"].ToString();
                        
                        var read = reader["rol"].ToString();
                        Rol rolUsuario;
                        if(Enum.TryParse(read, out rolUsuario))
                        {
                            usuario.Rol = rolUsuario;
                        }

                        usuarios.Add(usuario);
                        
                    }
                }

                connection.Close();
            }
        }
        catch(Exception ex)
        {
            throw new ElementNotFoundException($"Error al obtener los usuarios registrados en la base de datos.", ex);
        }

        return usuarios;        // Colocar un 'return' dentro de un 'finally' podría generar errores, por eso es conveniente separar estas sentencias
    }

    public Usuario GetById(int id) 
    {
        Usuario usuarioBuscado = new Usuario();

        try
        {
            var query = @"SELECT * FROM Usuario WHERE id = @id_buscado;";

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var command = new SQLiteCommand(query, connection);
                command.Parameters.Add(new SQLiteParameter("@id_buscado", id));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        usuarioBuscado.Id = Convert.ToInt32(reader["id"]);
                        usuarioBuscado.Nombre = reader["nombre_de_usuario"].ToString();
                        usuarioBuscado.Password = reader["password"].ToString();
                        string read = reader["rol"].ToString();
                        Rol rolUsuario;
                        if (Enum.TryParse(read, out rolUsuario))
                        {
                            usuarioBuscado.Rol = rolUsuario;
                        }
                    }
                }

                connection.Close();
            }
        }
        catch(Exception ex)
        {
            throw new ElementNotFoundException($"Error al buscar el usuario (ID: {id}) en la base de datos.", ex);
        }

        return usuarioBuscado;
    }

    public int Delete(int id) 
    {
        int result = 0;

        try
        {    
            _tareaRepository.SetDefaultUsuarioId(id);       // Se setean los ID's del usuario asignado en -9999 a las tareas que éste haya tenido
            _tableroRepository.SetDefaultUsuarioId(id);      // Se setean los ID's del usuario propietario en -9999 a los tableros de los que haya sido dueño

            var query = @"DELETE FROM Usuario WHERE Usuario.id = (@id_buscado);";

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString)) 
            {

                connection.Open();

                var command = new SQLiteCommand(query, connection);

                command.Parameters.Add(new SQLiteParameter("@id_buscado", id));
                
                result = command.ExecuteNonQuery();     // Indica el número de tuplas afectadas en la BD. Si se elimina efectivamente una tupla, entonces retorna un número distinto de cero

                connection.Close();
                
            }
        }
        catch(Exception ex)
        {
            throw new OperationFailedException($"Error al eliminar el usuario (ID: {id}) de la base de datos", ex);
        }

        return result;
    }

    public Usuario GetLoggedUser(string nombre, string password) 
    {
        var usuarioBuscado = new Usuario();

        try
        {    
            var query = @"SELECT * FROM Usuario WHERE (nombre_de_usuario = @nombreUsuario AND password = @password);";

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var command = new SQLiteCommand(query, connection);

                command.Parameters.Add(new SQLiteParameter("@nombreUsuario", nombre));
                command.Parameters.Add(new SQLiteParameter("@password", password));


                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        usuarioBuscado.Id = Convert.ToInt32(reader["id"]);
                        usuarioBuscado.Nombre = reader["nombre_de_usuario"].ToString();
                        usuarioBuscado.Password = reader["password"].ToString();
                        string read = reader["rol"].ToString();
                        Rol rolUsuario;
                        if (Enum.TryParse(read, out rolUsuario))
                        {
                            usuarioBuscado.Rol = rolUsuario;
                        }
                    }
                }

                connection.Close();
            }
        }
        catch(Exception ex)
        {
            throw new ElementNotFoundException($"Error al buscar el usuario (Nombre: {nombre}, Contraseña: {password}) en la base de datos.", ex);
        }

        return usuarioBuscado;
    }
}