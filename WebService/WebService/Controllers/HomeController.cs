using System.Web.Mvc;

namespace WebService.Controllers
{
    public class HomeController : Controller
    {
        readonly TimeoutLongPolling _timeout = new TimeoutLongPolling();
        readonly RabbitQueue _rabbit = new RabbitQueue();
        readonly CommandValidation _validation = new CommandValidation();
        readonly GetIdFromRequest _getId = new GetIdFromRequest();

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string command)
        {
            if (_validation.Validation(command))
            {
                string id = _getId.FromPost(command);
                _rabbit.Producer(id, command);
                _rabbit.CreateTimeout(id);
                // ask all open pollings!!!!!!!!!!!!!!!!!!!!!
            }
            return View();
        }

        [HttpGet]
        public ActionResult Commands()
        {
            // need to get url !!!!!!!!!!!!!!!!!!!!!!!!!!!
            _timeout.CreateTimeout(_timeout.GetTimeout(Request.Url.ToString()));
            _rabbit.Consumer(_getId.FromGet(Request.Url.ToString()));
            return View();
        }

        [HttpGet]
        public ActionResult LogsView()
        {
            return View();
        }
    }
}
