using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinal_ISII.Models;
using ProyectoFinal_ISII.Repositories;
using ProyectoFinal_ISII.ViewModels;

namespace ProyectoFinal_ISII.Controllers;

public class LoginController : Controller
{

    private readonly ILogger<LoginController> _logger;
    private readonly IUsuarioRepository usuarioRepository;

    public LoginController(ILogger<LoginController> logger, IUsuarioRepository _usuarioRepository)
    {
        _logger = logger;
        usuarioRepository = _usuarioRepository;
    }

    // Endpoints

    public IActionResult Index()
    {
        return View();
    }

    // Endpoint de control de inicio de sesión

    [HttpPost]     
    public IActionResult Login(LoginViewModel usuario)
    {
        try 
        {
            var usuarioLogueado = usuarioRepository.GetLoggedUser(usuario.NombreUsuario.Trim(), usuario.Password);

            if(usuarioLogueado.Nombre == null) 
            {
                // Acceso rechazado
                _logger.LogWarning($"Intento de acceso inválido - Usuario: {usuario.NombreUsuario.Trim()} - Clave ingresada: {usuario.Password}");

                TempData["ErrorMessage"] = "Ha ingresado credenciales incorrectas, o el usuario no existe";
                return RedirectToAction("Index");       // Si el usuario no existe, retorna a 'Index'
            }
            else 
            {
                // Acceso exitoso
                _logger.LogInformation($"El usuario {usuario.NombreUsuario.Trim()} ingresó correctamente.");

                LoguearUsuario(usuarioLogueado);        // Registrar el usuario

                if (HttpContext.Session.GetString("rol") == Rol.administrador.ToString())
                {
                    return RedirectToRoute(new { controller = "Home", action = "Index" });
                }
                else
                {
                    return RedirectToRoute(new { controller = "Home", action = "Index" });
                }
            }
        }
        catch (Exception ex)
        {
            // Loggeo de errores
            _logger.LogError($"Error durante el inicio de sesión: {ex.ToString()}");
            return BadRequest();
        }
    }

    public IActionResult Logout() 
    {
        return View();
    }

    [HttpPost]
    public IActionResult LogoutConfirmed()
    {
        try
        {
            HttpContext.Session.Clear();    // Elimina todas las variables de sesión al cerrar sesión
            _logger.LogInformation("Usuario cerró sesión correctamente.");
            return RedirectToAction("Index", "Login");      // Redirige a la página de inicio de sesión
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error durante el cierre de sesión: {ex.ToString()}");
            return BadRequest();
        }
    }

    private void LoguearUsuario(Usuario usuario)
    {
        HttpContext.Session.SetString("id", usuario.Id.ToString());         // Establece una variable de sesión, "id", con el valor 'usuario.Id'
        HttpContext.Session.SetString("nombreUsuario", usuario.Nombre);
        HttpContext.Session.SetString("rol", usuario.Rol.ToString());
        HttpContext.Session.SetString("password", usuario.Password);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
