using Microsoft.AspNetCore.Mvc;
using SimpleDN8WebWithAuth.Models;
using System.Diagnostics;

namespace SimpleDN8WebWithAuth.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        var common = _configuration["DemoSettingsFeed:CommonSharedSetting"];
        var secret = _configuration["DemoSettingsFeed:SecretSharedSetting"];

        var model = new HomeIndexViewModel
        {
            Common = common,
            Secret = secret
        };
        
        return View(model);
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
