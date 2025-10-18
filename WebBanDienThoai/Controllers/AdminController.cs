using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

//[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult ManageProducts()
    {
        return View();
    }

    public IActionResult ManageOrders()
    {
        return View();
    }

    public IActionResult ManageUsers()
    {
        return View();
    }

    public IActionResult Statistics()
    {
        return View();
    }
}
