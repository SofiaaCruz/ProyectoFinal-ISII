using System.Data.SQLite;
using ProyectoFinal_ISII.Models;
using ProyectoFinal_ISII.Exceptions;

namespace ProyectoFinal_ISII.Repositories;

public class TableroRepository : ITableroRepository
{
    private readonly string _connectionString;
    private readonly ITareaRepository _tareaRepository;

    public TableroRepository(string connectionString, ITareaRepository tareaRepository)
    {
        _connectionString = connectionString;
        _tareaRepository = tareaRepository;
    }

    public Tablero Create(Tablero tablero)
    {
        try
        {
            var query = @"INSERT INTO Tablero (id_usuario_propietario, nombre, descripcion) VALUES (@id_usuario, @nombre_tablero, @descripcion_tablero);";

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var command = new SQLiteCommand(query, connection);

                command.Parameters.Add(new SQLiteParameter("@id_usuario", tablero.IdUsuarioPropietario));
                command.Parameters.Add(new SQLiteParameter("@nombre_tablero", tablero.Nombre));
                command.Parameters.Add(new SQLiteParameter("@descripcion_tablero", tablero.Descripcion));

                command.ExecuteNonQuery();

                connection.Close();
            }
        }
        catch(Exception ex)
        {
            throw new ElementNotCreatedException($"Error al crear el tablero (ID: {tablero.Id}) en la base de datos.", ex);
        }

        return tablero;
    }

    public List<Tablero> GetAll()
    {
        List<Tablero> tableros = new();
        
        try
        {
            var query = @"SELECT * FROM Tablero;";

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var command = new SQLiteCommand(query, connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var tablero = new Tablero();

                        tablero.Id = Convert.ToInt32(reader["id"]);
                        tablero.IdUsuarioPropietario = Convert.ToInt32(reader["id_usuario_propietario"]);
                        tablero.Nombre = reader["nombre"].ToString();
                        tablero.Descripcion = reader["descripcion"].ToString();

                        tableros.Add(tablero);
                    }
                }
                connection.Close();
            }
        }
        catch(Exception ex)
        {
            throw new ElementNotFoundException("Error al obtener los tableros registrados en la base de datos.", ex);
        }

        return tableros;
    }

    public Tablero GetById(int id)
    {
        try
        {
            var query = "SELECT * FROM Tablero WHERE id = @id_buscado;";

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var command = new SQLiteCommand(query, connection);
                command.Parameters.Add(new SQLiteParameter("@id_buscado", id));

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Tablero
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            IdUsuarioPropietario = Convert.ToInt32(reader["id_usuario_propietario"]),
                            Nombre = reader["nombre"].ToString(),
                            Descripcion = reader["descripcion"].ToString()
                        };
                    }
                }

                connection.Close();
            }
        }
        catch (Exception ex)
        {
            throw new ElementNotFoundException($"Error al encontrar el tablero (ID: {id}) en la base de datos.", ex);
        }

        return null;    // Devuelve null si no se encuentra el tablero
    }

    public List<Tablero> GetByUserId(int idUsuario)
    {
        List<Tablero> tableros = new List<Tablero>();

        try
        {
            var query = "SELECT * FROM Tablero WHERE id_usuario_propietario = @idUsuario;";

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var command = new SQLiteCommand(query, connection);
                command.Parameters.Add(new SQLiteParameter("@idUsuario", idUsuario));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var tablero = new Tablero
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            IdUsuarioPropietario = Convert.ToInt32(reader["id_usuario_propietario"]),
                            Nombre = reader["nombre"].ToString(),
                            Descripcion = reader["descripcion"].ToString()
                        };

                        tableros.Add(tablero);
                    }
                }

                connection.Close();
            }
        }
        catch (Exception ex)
        {
            throw new ElementNotFoundException($"Error al encontrar los tableros asociados al usuario (ID: {idUsuario}) en la base de datos.", ex);
        }

        return tableros;
    }

    public int Delete(int id)
    {
        int result = 0;
        
        try
        {
            _tareaRepository.DeleteByTableroId(id);

            // Se procede con la eliminación del tablero

            var query = @"DELETE FROM Tablero WHERE Tablero.id = (@id_buscado);";

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var command = new SQLiteCommand(query, connection);

                command.Parameters.Add(new SQLiteParameter("@id_buscado", id));

                result = command.ExecuteNonQuery();

                connection.Close();
            }
        }
        catch(Exception ex)
        { 
            throw new OperationFailedException($"Error al eliminar el tablero (ID: {id}) de la base de datos.", ex);
        }

        return result;
    }

    public void Update(int id, Tablero tablero)
    {
        try
        {
            var query = @"UPDATE Tablero SET id_usuario_propietario = @nuevo_id_usuario, nombre = @nuevo_nombre, descripcion = @nueva_descripcion WHERE id = @id_buscado;";

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var command = new SQLiteCommand(query, connection);

                command.Parameters.Add(new SQLiteParameter("@nuevo_id_usuario", tablero.IdUsuarioPropietario));
                command.Parameters.Add(new SQLiteParameter("@nuevo_nombre", tablero.Nombre));
                command.Parameters.Add(new SQLiteParameter("@nueva_descripcion", tablero.Descripcion));
                command.Parameters.Add(new SQLiteParameter("@id_buscado", id));

                command.ExecuteNonQuery();

                connection.Close();
            }
        }
        catch(Exception ex)
        {  
            throw new ElementNotCreatedException($"Error al actualizar el tablero (ID: {id}) en la base de datos.", ex);
        }
    }

    public void SetDefaultUsuarioId(int idUsuario)
    {
        try
        {
            var query = "UPDATE Tablero SET id_usuario_propietario = -9999 WHERE id_usuario_propietario = @idUsuario;";

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
             {
                connection.Open();

                var command = new SQLiteCommand(query, connection);
                command.Parameters.Add(new SQLiteParameter("@idUsuario", idUsuario));

                command.ExecuteNonQuery();

                connection.Close();
            }
        }
        catch (Exception ex)
        {
            throw new OperationFailedException($"Error al resetear el usuario propietario del tablero en la base de datos.", ex);
        }
    }
}
