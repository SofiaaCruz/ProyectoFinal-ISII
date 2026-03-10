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

    // Endpoint de vista de login
    public IActionResult Index()
    {
        _logger.LogInformation("Acceso a la página de login");
        return View();
    }

    // Endpoint de control de inicio de sesión
    [HttpPost]     
    public IActionResult Login(LoginViewModel usuario)
    {
        try 
        {
            _logger.LogInformation($"Intento de inicio de sesión del usuario: {usuario.NombreUsuario}");

            var usuarioLogueado = usuarioRepository.GetLoggedUser(usuario.NombreUsuario.Trim(), usuario.Password);

            if(usuarioLogueado.Nombre == null) 
            {
                // Acceso rechazado
                _logger.LogWarning($"Intento de acceso inválido - Usuario: {usuario.NombreUsuario.Trim()}");

                TempData["ErrorMessage"] = "Ha ingresado credenciales incorrectas, o el usuario no existe";
                return RedirectToAction("Index");
            }
            else 
            {
                // Acceso exitoso
                _logger.LogInformation($"El usuario {usuario.NombreUsuario.Trim()} ingresó correctamente.");

                LoguearUsuario(usuarioLogueado);

                return RedirectToRoute(new { controller = "Home", action = "Index" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el inicio de sesión");
            return BadRequest();
        }
    }

    public IActionResult Logout() 
    {
        _logger.LogInformation("Usuario accedió a la página de cierre de sesión");
        return View();
    }

    [HttpPost]
    public IActionResult LogoutConfirmed()
    {
        try
        {
            HttpContext.Session.Clear();

            _logger.LogInformation("Usuario cerró sesión correctamente.");

            return RedirectToAction("Index", "Login");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el cierre de sesión");
            return BadRequest();
        }
    }

    private void LoguearUsuario(Usuario usuario)
    {
        HttpContext.Session.SetString("id", usuario.Id.ToString());
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