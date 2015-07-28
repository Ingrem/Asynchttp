using System;
using System.Threading;
using Timer = System.Threading.Timer;

namespace WebService.Controllers
{
    public class TimeoutLongPolling
    {
        public int GetTimeout(string url)
        {
            int i = 10;
            while (url[i] != '/')
                i++;
            int j = i + 1;
            while (url[j] != '/')
                j++;
            url = url.Substring(i + 1, j - i - 1);
            return Convert.ToInt32(url);
        }
        public void CreateTimeout(int timeout)
        {
            TimerCallback tm = Count;
            new Timer(tm, null, timeout, timeout);
        }
        private void Count(object obj)
        {
            //request and close polling!!!!!!!!!!!!!!!!!!
        }
    }
}