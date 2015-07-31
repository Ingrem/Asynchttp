using System.Threading;
using Timer = System.Threading.Timer;

namespace WebService.Controllers
{
    public class TimeoutLongPolling
    {
        public void CreateTimeout(int timeout)
        {
            TimerCallback tm = Count;
            new Timer(tm, null, timeout, timeout);
        }
        private void Count(object obj)
        {
            //request and close polling!!!!!!!!!!!!!!!!!!
            // code 408 Request Timeout
        }
    }
}