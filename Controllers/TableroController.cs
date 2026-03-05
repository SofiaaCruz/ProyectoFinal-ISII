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

    // Endpoints

    // Mostrar tableros

    public IActionResult Index() 
    {
        try
        {
            if(notLoggedUser()) return redirectToLogin();

            int idUsuario = int.Parse(HttpContext.Session.GetString("id"));

            List<Tarea> tareas = new();
            List<Tablero> tablerosPropios = new();
            List<Tablero> tablerosConTareasAsignadas = new();
            List<Usuario> usuariosRegistrados = usuarioRepository.GetAll();

            if(isAdmin())
            {
                tablerosPropios = tableroRepository.GetAll();
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

                
                tablerosConTareasAsignadas = tablerosConTareasAsignadas.GroupBy(tablero => tablero.Id).Select(group => group.First()).ToList();

                return View("IndexOperatorUser", new ListarTablerosViewModel(tablerosPropios, tablerosConTareasAsignadas, idUsuario, usuariosRegistrados));
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.ToString());
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

            return View(new CrearTableroViewModel(idUsuario, usuarios));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.ToString());
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
            return RedirectToAction("Index");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.ToString());
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

            return View(new ModificarTableroViewModel(tablero, idUsuario, usuarios));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.ToString());
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
            return RedirectToAction("Index");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.ToString());
            return BadRequest();
        }
    }

    
    // Eliminar tablero

    public IActionResult Delete(int idTablero) 
    {
        try
        {
            if(notLoggedUser()) return redirectToLogin();
            return View(tableroRepository.GetById(idTablero));
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
            if(tableroRepository.Delete(id) > 0)
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