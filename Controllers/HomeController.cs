using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinal_ISII.Models; 

namespace ProyectoFinal_ISII.Controllers; 

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        try
        {
            _logger.LogInformation("Acceso al Home del sistema");

            if(string.IsNullOrEmpty(HttpContext.Session.GetString("rol"))) 
            {
                _logger.LogWarning("Intento de acceso sin iniciar sesión");

                TempData["ErrorMessage"] = "Inicie sesión antes de acceder a este sitio";
                return RedirectToAction("Index", "Login");
            }
            
            if(HttpContext.Session.GetString("rol") == "administrador") 
            {
                _logger.LogInformation("Usuario administrador accedió al panel principal");

                return View("IndexAdministratorUser");
            }

            _logger.LogInformation("Usuario operador accedió al panel principal");
            return View("IndexOperatorUser");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Ocurrió un error en el HomeController");
            return BadRequest();
        }
    }

    public IActionResult Privacy()
    {
        _logger.LogInformation("Acceso a la página de privacidad");
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        _logger.LogError("Se produjo un error en la aplicación");

        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}