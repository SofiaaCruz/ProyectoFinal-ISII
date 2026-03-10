using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinal_ISII.Models;
using ProyectoFinal_ISII.Repositories;
using ProyectoFinal_ISII.ViewModels;

namespace ProyectoFinal_ISII.Controllers;

public class UsuarioController : Controller
{
    private readonly ILogger<UsuarioController> _logger;
    private readonly IUsuarioRepository usuarioRepository;

    public UsuarioController(ILogger<UsuarioController> logger, IUsuarioRepository _usuarioRepository)
    {
        _logger = logger;
        usuarioRepository = _usuarioRepository;
    }

    // Página principal - lista de usuarios
    public IActionResult Index()
    {   
        try
        {
            if(notLoggedUser()) return redirectToLogin();
            if(!isAdmin()) return redirectOperatorUser();

            _logger.LogInformation("Administrador accedió a la lista de usuarios.");

            var users = usuarioRepository.GetAll();
            var usersVM = new ListarUsuariosViewModel(users);

            return View(usersVM);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al mostrar la lista de usuarios");
            return BadRequest();
        }
    }

    // Vista para crear usuario
    public IActionResult Create()
    {
        try
        {
            if(notLoggedUser()) return redirectToLogin();
            if(!isAdmin()) return redirectOperatorUser();

            _logger.LogInformation("Administrador accedió a la pantalla de creación de usuario.");

            return View(new CrearUsuarioViewModel());
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la vista de creación de usuario");
            return BadRequest();
        }
    }

    [HttpPost]
    public IActionResult Create(CrearUsuarioViewModel usuarioVM)
    {
        try
        {
            _logger.LogInformation("Intento de crear usuario");

            if(!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido al intentar crear usuario");
                return RedirectToAction("Index");
            }

            if(!isAdmin())
            {
                _logger.LogWarning("Usuario no autorizado intentó crear usuario");
                return redirectOperatorUser();
            }

            var usuario = new Usuario(usuarioVM);
            usuarioRepository.Create(usuario);

            _logger.LogInformation($"Se creó el usuario '{usuario.Nombre}' con rol {usuario.Rol}");

            return RedirectToAction("Index");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario");
            return BadRequest();
    }
}

    // Modificar usuario
    public IActionResult Update(int id)
    {
        try
        {
            if(notLoggedUser()) return redirectToLogin();
            if(!isAdmin()) return redirectOperatorUser();

            _logger.LogInformation($"Administrador accedió a modificar el usuario ID {id}");

            var usuario = usuarioRepository.GetById(id);
            var usuarioVM = new ModificarUsuarioViewModel(usuario);

            return View(usuarioVM);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al cargar modificación de usuario");
            return BadRequest();
        }
    }

    [HttpPost]
    public IActionResult Update(int id, ModificarUsuarioViewModel usuarioVM)
    {
        try
        {
            if(!ModelState.IsValid) return RedirectToAction("Index");
            if(!isAdmin()) return redirectOperatorUser();

            var usuario = new Usuario(usuarioVM);
            usuarioRepository.Update(id, usuario);

            _logger.LogInformation($"Se modificó el usuario con ID {id}");

            return RedirectToAction("Index");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al modificar usuario");
            return BadRequest();
        }
    }

    // Eliminar usuario
    public IActionResult Delete(int id) 
    {
        try
        {
            if(notLoggedUser()) return redirectToLogin();
            if(!isAdmin()) return redirectOperatorUser();

            _logger.LogInformation($"Administrador accedió a eliminar el usuario ID {id}");

            return View(usuarioRepository.GetById(id));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al cargar eliminación de usuario");
            return BadRequest();
        }
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            if(usuarioRepository.Delete(id) > 0)      
            {
                _logger.LogInformation($"Se eliminó el usuario con ID {id}");
                return RedirectToAction("Index");
            }
            else
            {
                _logger.LogWarning($"Intento fallido de eliminar usuario ID {id}");
                return RedirectToAction("Error");
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar usuario");
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
        _logger.LogWarning("Intento de acceso a sección de usuarios por un operador.");

        TempData["ErrorMessage"] = "No puedes acceder a este sitio porque no eres administrador";
        return RedirectToRoute(new { controller = "Tablero", action = "Index" });
    }

    private IActionResult redirectToLogin()
    {
        _logger.LogWarning("Intento de acceso sin sesión iniciada.");

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