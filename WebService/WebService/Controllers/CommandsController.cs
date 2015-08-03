using System.Threading.Tasks;
using System.Web.Mvc;
using WebService.Service;

namespace WebService.Controllers
{
    public class CommandsController : Controller
    {
        [HttpGet]
        public async Task<ActionResult> commands(string deviceId, int timeout)
        {
            LongPolling longPolling = new LongPolling();

            await Task.FromResult(longPolling.CommandWaitPolling(deviceId, timeout));

            string jsonStringtmp = LongPolling.JsonStrings[deviceId];
            LongPolling.JsonStrings.Remove(deviceId);

            if (jsonStringtmp == "timeout")
                return new HttpStatusCodeResult(408, "Request Timeout");
            if (jsonStringtmp == "Database error")
                return new HttpStatusCodeResult(503, "Database error");
            if (jsonStringtmp == "RabbitMQ error")
                return new HttpStatusCodeResult(503, "RabbitMQ error");
            
            return Json(jsonStringtmp, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Index(string deviceId, string command)
        {
            RabbitQueue rabbit = new RabbitQueue();
            CommandValidation validation = new CommandValidation();

            if (validation.Validation(command))
            {
                rabbit.Producer(deviceId, command);
                rabbit.CreateTimeout(deviceId);
                if (LongPolling.Connections.Contains(deviceId))
                    LongPolling.JsonStrings[deviceId] = (rabbit.Consumer(deviceId));
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
