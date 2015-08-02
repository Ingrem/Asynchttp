using System.Web.Mvc;
using WebService.Service;

namespace WebService.Controllers
{
    public class CommandsController : Controller
    {
        readonly RabbitQueue _rabbit = new RabbitQueue();
        readonly CommandValidation _validation = new CommandValidation();
        readonly LongPolling _polling = new LongPolling();

        [HttpGet]
        public ActionResult commands(string deviceId, int timeout)
        {
            object[] str = {deviceId, timeout};
            return View(str);
        }

        [HttpPost]
        public ActionResult Index(string deviceId, string command)
        {
            if (_validation.Validation(command))
            {
                _rabbit.Producer(deviceId, command);
                _rabbit.CreateTimeout(deviceId);
                _polling.AskAll(deviceId);
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
