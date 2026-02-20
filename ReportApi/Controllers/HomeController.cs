using System.Web.Mvc;

namespace ReportApi.Controllers;

public class HomeController : Controller
{
    public ActionResult Index()
    {
        ViewBag.Title = "MultiSoftware - API Report";

        return View();
    }
}
