using Microsoft.AspNetCore.Mvc;

namespace Supermarket.Controllers
{
	public class AjaxContentController : Controller
	{
		public IActionResult HeaderCart()
		{
			return ViewComponent("HeaderCart");
		}
		public IActionResult HeaderFavourites()
		{
			return ViewComponent("NumberCart");
		}
	}
}
