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
            if(string.IsNullOrEmpty(HttpContext.Session.GetString("rol"))) 
            {
                TempData["ErrorMessage"] = "Inicie sesión antes de acceder a este sitio";
                return RedirectToAction("Index", "Login");
            }
            
            if(HttpContext.Session.GetString("rol") == "administrador") 
            {
                return View("IndexAdministratorUser");
            }
            return View("IndexOperatorUser");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.ToString());
            return BadRequest();
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}