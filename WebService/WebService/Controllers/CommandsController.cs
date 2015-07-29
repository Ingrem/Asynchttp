using System.Web.Mvc;

namespace WebService.Controllers
{
    public class CommandsController : Controller
    {
        readonly TimeoutLongPolling _timeout = new TimeoutLongPolling();
        readonly RabbitQueue _rabbit = new RabbitQueue();

        [HttpGet]
        public ActionResult Commands(string deviceId, int timeout)
        {
            // open polling!!!!!!!!!!!!!!!!!!!
            _timeout.CreateTimeout(timeout);
            var json = _rabbit.Consumer(deviceId);
            Response.Write(json);
            return View();
        }
    }
}
