using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinal_ISII.Models;
using ProyectoFinal_ISII.Repositories;
using ProyectoFinal_ISII.ViewModels;

namespace ProyectoFinal_ISII.Controllers;

public class TableroController : Controller
{
    private readonly ILogger<TableroController> _logger;
    private readonly ITableroRepository tableroRepository;
    private readonly IUsuarioRepository usuarioRepository;
    private readonly ITareaRepository tareaRepository;

    public TableroController(ILogger<TableroController> logger, ITableroRepository _tableroRepository, IUsuarioRepository _usuarioRepository, ITareaRepository _tareaRepository)
    {
        _logger = logger;
        tableroRepository = _tableroRepository;
        usuarioRepository = _usuarioRepository;
        tareaRepository = _tareaRepository;
    }

    // Mostrar tableros
    public IActionResult Index() 
    {
        try
        {
            if(notLoggedUser()) return redirectToLogin();

            int idUsuario = int.Parse(HttpContext.Session.GetString("id"));

            _logger.LogInformation($"Usuario {idUsuario} accedió a la lista de tableros.");

            List<Tarea> tareas = new();
            List<Tablero> tablerosPropios = new();
            List<Tablero> tablerosConTareasAsignadas = new();
            List<Usuario> usuariosRegistrados = usuarioRepository.GetAll();

            if(isAdmin())
            {
                tablerosPropios = tableroRepository.GetAll();

                _logger.LogInformation("Administrador visualizó todos los tableros.");

                return View("IndexAdministratorUser", new ListarTablerosViewModel(tablerosPropios, idUsuario, usuariosRegistrados));
            }
            else
            {
                tareas = tareaRepository.GetByUsuarioId(idUsuario);
                tablerosPropios = tableroRepository.GetByUserId(idUsuario);

                foreach(Tarea task in tareas)
                {
                    int idTablero = task.IdTablero;
                    var tableroBuscado = tableroRepository.GetById(idTablero);
                    tablerosConTareasAsignadas.Add(tableroBuscado);
                }

                tablerosConTareasAsignadas = tablerosConTareasAsignadas
                    .GroupBy(tablero => tablero.Id)
                    .Select(group => group.First())
                    .ToList();

                _logger.LogInformation($"Usuario {idUsuario} visualizó sus tableros.");

                return View("IndexOperatorUser", new ListarTablerosViewModel(tablerosPropios, tablerosConTareasAsignadas, idUsuario, usuariosRegistrados));
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al mostrar los tableros");
            return BadRequest();
        }
    }

    // Crear tablero
    public IActionResult Create(int idUsuario) 
    {
        try
        {
            if(notLoggedUser()) return redirectToLogin();

            List<Usuario> usuarios = usuarioRepository.GetAll();

            _logger.LogInformation($"Usuario {idUsuario} accedió a la pantalla de creación de tableros.");

            return View(new CrearTableroViewModel(idUsuario, usuarios));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la vista de creación de tablero");
            return BadRequest();
        }
    }

    [HttpPost]
    public IActionResult Create(CrearTableroViewModel tableroVM)
    {
        try
        {
            if(!ModelState.IsValid) return RedirectToAction("Index");

            var tablero = new Tablero(tableroVM);
            tableroRepository.Create(tablero);

            _logger.LogInformation($"Se creó el tablero '{tablero.Nombre}' por el usuario {tablero.IdUsuarioPropietario}");

            return RedirectToAction("Index");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al crear un tablero");
            return BadRequest();
        }
    }

    // Modificar tablero
    public IActionResult Update(int idTablero, int idUsuario) 
    {
        try
        {
            if(notLoggedUser()) return redirectToLogin();

            var tablero = tableroRepository.GetById(idTablero);
            var usuarios = usuarioRepository.GetAll();

            _logger.LogInformation($"Usuario {idUsuario} accedió a la modificación del tablero {idTablero}");

            return View(new ModificarTableroViewModel(tablero, idUsuario, usuarios));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la vista de modificación de tablero");
            return BadRequest();
        }
    }

    [HttpPost]
    public IActionResult Update(ModificarTableroViewModel tableroVM)
    {
        try
        {
            if(!ModelState.IsValid) return RedirectToAction("Index");

            var tablero = new Tablero(tableroVM);
            tableroRepository.Update(tablero.Id, tablero);

            _logger.LogInformation($"Se modificó el tablero ID {tablero.Id}");

            return RedirectToAction("Index");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al modificar un tablero");
            return BadRequest();
        }
    }

    // Eliminar tablero
    public IActionResult Delete(int idTablero) 
    {
        try
        {
            if(notLoggedUser()) return redirectToLogin();

            _logger.LogInformation($"Se accedió a la pantalla de eliminación del tablero {idTablero}");

            return View(tableroRepository.GetById(idTablero));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la vista de eliminación");
            return BadRequest();
        }
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            if(tableroRepository.Delete(id) > 0)
            {
                _logger.LogInformation($"Se eliminó el tablero con ID {id}");
                return RedirectToAction("Index");
            }
            else 
            {
                _logger.LogWarning($"Intento fallido de eliminar el tablero ID {id}");
                return RedirectToAction("Error");
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar tablero");
            return BadRequest();
        }
    }

    private bool notLoggedUser()
    {
        return (HttpContext.Session.GetString("rol") != "administrador" &&
                HttpContext.Session.GetString("rol") != "operador");
    }

    private bool isAdmin()
    {
        return (HttpContext.Session != null &&
                HttpContext.Session.GetString("rol") == "administrador");
    }

    private IActionResult redirectOperatorUser()
    {
        TempData["ErrorMessage"] = "No puedes acceder a este sitio porque no eres administrador";
        return RedirectToRoute(new { controller = "Tablero", action = "Index" });
    }

    private IActionResult redirectToLogin()
    {
        TempData["ErrorMessage"] = "Inicie sesión antes de acceder a este sitio";
        return RedirectToRoute(new { controller = "Login", action = "Index" });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}