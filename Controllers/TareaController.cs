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


    // Endpoints

    // Mostrar tareas

    public IActionResult Index()
    {
        try
        {
            if(notLoggedUser()) return redirectToLogin();
            
            int idUsuario = int.Parse(HttpContext.Session.GetString("id"));

            List<Tarea> tareas = new();
            List<Tarea> tareasAsignadas = new();
            List<Tarea> tareasCreadas = new();

            List<Tablero> tableros = tableroRepository.GetAll();
            List<Usuario> usuarios = usuarioRepository.GetAll();
 
            if(isAdmin())
            {
                tareas = tareaRepository.GetAll();
                return View("IndexAdministratorUser", new ListarTareasViewModel(tareas, idUsuario, usuarios, tableros));
            }
            else
            {
                tareas = tareaRepository.GetAll();
                tareasAsignadas = tareaRepository.GetByUsuarioId(idUsuario);
                List<Tablero> tablerosDeUsuario = tableroRepository.GetByUserId(idUsuario);

                /* foreach (Tarea task in tareas)
                {
                    int idTablero = task.IdTablero;
                    foreach (Tablero board in tableros)
                    {
                        if(board.Id == idTablero)
                        {
                            tareasCreadas.Add(task);
                        }
                    }
                } */

                tareasCreadas.AddRange(tareas.Where(task => tablerosDeUsuario.Any(board => board.Id == task.IdTablero)));
                return View("IndexOperatorUser", new ListarTareasViewModel(tareasAsignadas, tareasCreadas, idUsuario, usuarios, tableros));
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.ToString());
            return BadRequest();
        } 
    }

    public IActionResult ShowSingleTask(int idTarea, int idUsuario)
    {
        if(notLoggedUser()) return redirectToLogin();
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
            _logger.LogError(ex.ToString());
            return BadRequest();
        } 
    }

    // Crear tarea

    public IActionResult Create(int idUsuario, int idTablero = -999) 
    {
        try
        {
            if(notLoggedUser()) return redirectToLogin();
            
            List<Usuario> usuarios = usuarioRepository.GetAll();
            List<Tablero> tableros = new();

            if(isAdmin()) 
            {
                tableros = tableroRepository.GetAll();
            }
            else 
            {
                tableros = tableroRepository.GetByUserId(idUsuario);
            }

            if(idTablero != -999)
            {
                return View("CreateOnBoard", new CrearTareaViewModel(usuarios, tableros, idTablero));
            }
            else 
            {
                return View(new CrearTareaViewModel(usuarios, tableros));
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.ToString());
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

            return RedirectToAction("Index");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.ToString());
            return BadRequest();
        }
    }

    // Modificar tarea

    public IActionResult Update(int idTarea, int idUsuario, int taskType = 0)
    {
        try
        {
            if(notLoggedUser()) return redirectToLogin();

            List<Usuario> usuarios = usuarioRepository.GetAll();
            List<Tablero> tableros = new();

            if(isAdmin())
            {
                tableros = tableroRepository.GetAll();
            }
            else 
            {
                tableros = tableroRepository.GetByUserId(idUsuario);
            }

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
            _logger.LogError(ex.ToString());
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

            return RedirectToAction("Index");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.ToString());
            return BadRequest();
        }
    }

    // Eliminar tarea

    public IActionResult Delete(int idTarea)
    {
        try
        {
            if(notLoggedUser()) return redirectToLogin();
            
            return View(tareaRepository.GetById(idTarea));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.ToString());
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
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.ToString());
            return BadRequest();
        }
    }

    // Método que verifica que haya una sesión existente 

    private bool notLoggedUser()
    {
        return (HttpContext.Session.GetString("rol") != "administrador" && HttpContext.Session.GetString("rol") != "operador");
    }

    // Método que verifica que el usuario logueado sea 'administrador'

    private bool isAdmin()
    {
        return (HttpContext.Session != null && HttpContext.Session.GetString("rol") == "administrador");
    }

    // Método que redirecciona a la pantalla de inicio mostrando un mensaje de error correspondiente

    private IActionResult redirectOperatorUser()
    {
        TempData["ErrorMessage"] = "No puedes acceder a este sitio porque no eres administrador";        // Almacena el mensaje en TempData (se utiliza para pasar datos entre acciones durante redirecciones) para mostrarlo en la página de inicio
        return RedirectToRoute(new { controller = "Tablero", action = "Index" });
    }

    // Método que redirecciona a la pantalla de inicio de sesión

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
