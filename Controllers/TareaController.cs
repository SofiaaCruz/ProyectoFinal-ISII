using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinal_ISII.Models;
using ProyectoFinal_ISII.Repositories;
using ProyectoFinal_ISII.ViewModels;

namespace ProyectoFinal_ISII.Controllers;

public class TareaController : Controller
{
    private readonly ILogger<TareaController> _logger;
    private readonly ITareaRepository tareaRepository;
    private readonly IUsuarioRepository usuarioRepository;
    private readonly ITableroRepository tableroRepository;

    public TareaController(ILogger<TareaController> logger, ITareaRepository _tareaRepository, IUsuarioRepository _usuarioRepository, ITableroRepository _tableroRepository)
    {
        _logger = logger;
        tareaRepository = _tareaRepository;
        usuarioRepository = _usuarioRepository;
        tableroRepository = _tableroRepository;
    }

    // Mostrar tareas
    public IActionResult Index()
    {
        try
        {
            if(notLoggedUser()) return redirectToLogin();
            
            int idUsuario = int.Parse(HttpContext.Session.GetString("id"));

            _logger.LogInformation($"Usuario {idUsuario} accedió a la lista de tareas.");

            List<Tarea> tareas = new();
            List<Tarea> tareasAsignadas = new();
            List<Tarea> tareasCreadas = new();

            List<Tablero> tableros = tableroRepository.GetAll();
            List<Usuario> usuarios = usuarioRepository.GetAll();
 
            if(isAdmin())
            {
                tareas = tareaRepository.GetAll();
                _logger.LogInformation("Administrador visualizó todas las tareas.");
                return View("IndexAdministratorUser", new ListarTareasViewModel(tareas, idUsuario, usuarios, tableros));
            }
            else
            {
                tareas = tareaRepository.GetAll();
                tareasAsignadas = tareaRepository.GetByUsuarioId(idUsuario);
                List<Tablero> tablerosDeUsuario = tableroRepository.GetByUserId(idUsuario);

                tareasCreadas.AddRange(tareas.Where(task => tablerosDeUsuario.Any(board => board.Id == task.IdTablero)));

                _logger.LogInformation($"Usuario {idUsuario} visualizó sus tareas.");

                return View("IndexOperatorUser", new ListarTareasViewModel(tareasAsignadas, tareasCreadas, idUsuario, usuarios, tableros));
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al mostrar tareas");
            return BadRequest();
        } 
    }

    public IActionResult ShowSingleTask(int idTarea, int idUsuario)
    {
        if(notLoggedUser()) return redirectToLogin();

        _logger.LogInformation($"Usuario {idUsuario} visualizó la tarea {idTarea}");

        var tarea = tareaRepository.GetById(idTarea);
        var usuarios = usuarioRepository.GetAll();
        var tableros = tableroRepository.GetAll();

        return View("SingleTaskView", new ListarTareasViewModel(tarea, idUsuario, usuarios, tableros));
    }

    public IActionResult ShowTasksOnBoard(int idTablero, int idUsuario)
    {
        try
        {
            if(notLoggedUser()) return redirectToLogin();

            _logger.LogInformation($"Usuario {idUsuario} visualizó las tareas del tablero {idTablero}");

            List<Tarea> tareasDeTablero = tareaRepository.GetByTableroId(idTablero);
            List<Tablero> tablerosDeUsuario = tableroRepository.GetByUserId(idUsuario);
            List<Tarea> tareasCreadas = new();

            tareasCreadas.AddRange(tareasDeTablero.Where(task => tablerosDeUsuario.Any(board => board.Id == task.IdTablero)));

            var tableros = tableroRepository.GetAll();
            var usuarios = usuarioRepository.GetAll();
            
            if(isAdmin())
            {
                return View("TasksOnBoardAdministratorUser", new ListarTareasViewModel(tareasDeTablero, idUsuario, usuarios, tableros, idTablero));
            }
            else
            {
                return View("TasksOnBoardOperatorUser", new ListarTareasViewModel(tareasDeTablero, tareasCreadas, idUsuario, usuarios, tableros, idTablero));
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al mostrar tareas del tablero");
            return BadRequest();
        } 
    }

    // Crear tarea
    public IActionResult Create(int idUsuario, int idTablero = -999) 
    {
        try
        {
            if(notLoggedUser()) return redirectToLogin();

            _logger.LogInformation($"Usuario {idUsuario} accedió a la pantalla de creación de tareas.");
            
            List<Usuario> usuarios = usuarioRepository.GetAll();
            List<Tablero> tableros = new();

            if(isAdmin()) 
                tableros = tableroRepository.GetAll();
            else 
                tableros = tableroRepository.GetByUserId(idUsuario);

            if(idTablero != -999)
                return View("CreateOnBoard", new CrearTareaViewModel(usuarios, tableros, idTablero));
            else 
                return View(new CrearTareaViewModel(usuarios, tableros));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al cargar creación de tarea");
            return BadRequest();
        }
    }

    [HttpPost]
    public IActionResult Create(CrearTareaViewModel tareaVM)
    {
        try
        {
            if(!ModelState.IsValid) return RedirectToAction("Index");

            var tarea = new Tarea(tareaVM);
            tareaRepository.Create(tarea.IdTablero, tarea);

            _logger.LogInformation($"Se creó la tarea '{tarea.Nombre}' en el tablero {tarea.IdTablero}");

            return RedirectToAction("Index");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al crear tarea");
            return BadRequest();
        }
    }

    // Modificar tarea
    public IActionResult Update(int idTarea, int idUsuario, int taskType = 0)
    {
        try
        {
            if(notLoggedUser()) return redirectToLogin();

            _logger.LogInformation($"Usuario {idUsuario} accedió a modificar la tarea {idTarea}");

            List<Usuario> usuarios = usuarioRepository.GetAll();
            List<Tablero> tableros = new();

            if(isAdmin())
                tableros = tableroRepository.GetAll();
            else 
                tableros = tableroRepository.GetByUserId(idUsuario);

            var tarea = tareaRepository.GetById(idTarea);
            var tareaVM = new ModificarTareaViewModel(tarea, usuarios, tableros);

            switch (taskType)
            {
                case 0:
                case 1:
                    return View("Update", tareaVM);
                case 2:
                    return View("UpdateAssignedTask", tareaVM);
                default:
                    return BadRequest();
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al cargar modificación de tarea");
            return BadRequest();
        }
    }

    [HttpPost]
    public IActionResult Update(ModificarTareaViewModel tareaVM)
    {
        try
        {
            if(!ModelState.IsValid) return RedirectToAction("Index");

            var tarea = new Tarea(tareaVM);
            tareaRepository.Update(tarea.Id, tarea);

            _logger.LogInformation($"Se modificó la tarea con ID {tarea.Id}");

            return RedirectToAction("Index");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al modificar tarea");
            return BadRequest();
        }
    }

    // Eliminar tarea
    public IActionResult Delete(int idTarea)
    {
        try
        {
            if(notLoggedUser()) return redirectToLogin();

            _logger.LogInformation($"Se accedió a eliminar la tarea {idTarea}");

            return View(tareaRepository.GetById(idTarea));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al cargar eliminación de tarea");
            return BadRequest();
        }
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            if(tareaRepository.Delete(id) > 0)
            {
                _logger.LogInformation($"Se eliminó la tarea con ID {id}");
                return RedirectToAction("Index");
            }
            else
            {
                _logger.LogWarning($"Intento fallido de eliminar tarea ID {id}");
                return RedirectToAction("Error");
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar tarea");
            return BadRequest();
        }
    }

    private bool notLoggedUser()
    {
        return (HttpContext.Session.GetString("rol") != "administrador" && HttpContext.Session.GetString("rol") != "operador");
    }

    private bool isAdmin()
    {
        return (HttpContext.Session != null && HttpContext.Session.GetString("rol") == "administrador");
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
        _logger.LogError("Se accedió a la página de error.");
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}