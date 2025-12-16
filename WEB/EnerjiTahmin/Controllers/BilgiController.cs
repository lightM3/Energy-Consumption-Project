using Microsoft.AspNetCore.Mvc;

public class BilgiController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}