using Microsoft.AspNetCore.Mvc;

namespace FastFood.Controllers;

public class BaseController : Controller
{
    protected void AddErrors(IEnumerable<string> errors)
    {
        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }
    }
}
