using System.Web.Mvc;
using WebService.Models;

namespace WebService.Controllers
{
    public class HomeController : Controller
    {
        readonly RabbitQueue _rabbit = new RabbitQueue();
        readonly CommandValidation _validation = new CommandValidation();
        readonly GetIdFromRequest _getId = new GetIdFromRequest();

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(JsonSerialization.SendCommandDto commandDto)
        {
            string command = JsonSerialization.Serialization(commandDto);
            if (_validation.Validation(command) && ModelState.IsValid)
            {
                string id = _getId.FromPost(command);
                _rabbit.Producer(id, command);
                _rabbit.CreateTimeout(id);
                // ask all open pollings!!!!!!!!!!!!!!!!!!!!!
            }
                            
            return View();
        }
        
        [HttpGet]
        public ActionResult LogsView()
        {
            return View();
        }
    }
}
