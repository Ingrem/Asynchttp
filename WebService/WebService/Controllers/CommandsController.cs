using System.Web.Mvc;

namespace WebService.Controllers
{
    public class CommandsController : Controller
    {
        readonly RabbitQueue _rabbit = new RabbitQueue();
        readonly CommandValidation _validation = new CommandValidation();
        readonly GetIdFromRequest _getId = new GetIdFromRequest();

        [HttpGet]
        public ActionResult commands(string deviceId, int timeout)
        {
            // open polling!!!!!!!!!!!!!!!!!!!
           // _timeout.CreateTimeout(timeout);
           // _rabbit.Consumer(deviceId);
            return View();
        }

        [HttpPost]
        public ActionResult Index(string deviceId, string command)
        {
            if (_validation.Validation(command))
            {
                string id = _getId.FromPost(command);
                _rabbit.Producer(id, command);
                _rabbit.CreateTimeout(id);
                // ask all open pollings!!!!!!!!!!!!!!!!!!!!!
            }
            else
            {
                return new HttpStatusCodeResult(400, "Validation error");
            }
            return new HttpStatusCodeResult(202, "Accepted");
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult LogsView()
        {
            return View();
        }
    }
}
