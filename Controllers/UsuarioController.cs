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

    // Endpoints

    // Página principal - muestra todos los usuarios en una tabla

    public IActionResult Index()
    {   
        try
        {
            // Se verifica que el usuario esté logueado correctamente

            if(notLoggedUser()) return redirectToLogin();

            // El acceso a todos los usuarios esta permitido sólo para el usuario 'administrador'

            if(!isAdmin()) return redirectOperatorUser();

            var users = usuarioRepository.GetAll();
            var usersVM = new ListarUsuariosViewModel(users);

            return View(usersVM);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.ToString());
            return BadRequest();
        }
    }

    // Creación de usuario. Recibe los datos de un usuario desde un formulario y los carga en la BD

    // Para obtener la vista (No es necesario escribir [HttpGet], pues se asume que ese es el verbo por defecto)

    public IActionResult Create()
    {
        try
        {
            if(notLoggedUser()) return redirectToLogin();
            if(!isAdmin()) return redirectOperatorUser();
            return View(new CrearUsuarioViewModel());
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.ToString());
            return BadRequest();
        }
    }

    [HttpPost]      // Para añadir el usuario en la BD
    public IActionResult Create(CrearUsuarioViewModel usuarioVM)        // Recibe el objeto view model desde el formulario
    {
        try
        {
            if(!ModelState.IsValid) return RedirectToAction("Index");   // Si el modelo no está en su estado válido, se redirecciona a la página de inicio (verifica que no haya errores de validación)   
            if(!isAdmin()) return redirectOperatorUser();  // Si el usuario no es administrador, es redirigido a la página de inicio       

            var usuario = new Usuario(usuarioVM);
            usuarioRepository.Create(usuario);
            
            return RedirectToAction("Index");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.ToString());
            return BadRequest();
        }
    }

    // Modificación de un usuario

    public IActionResult Update(int id)
    {
        try
        {
            if(notLoggedUser()) return redirectToLogin();
            if(!isAdmin()) return redirectOperatorUser();

            var usuario = usuarioRepository.GetById(id);
            var usuarioVM = new ModificarUsuarioViewModel(usuario);

            return View(usuarioVM);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.ToString());
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

            return RedirectToAction("Index");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.ToString());
            return BadRequest();
        }
    }

    // Eliminación de un usuario

    public IActionResult Delete(int id) 
    {
        try
        {
            if(notLoggedUser()) return redirectToLogin();
            if(!isAdmin()) return redirectOperatorUser();
            return View(usuarioRepository.GetById(id));
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
            if(usuarioRepository.Delete(id) > 0)      
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