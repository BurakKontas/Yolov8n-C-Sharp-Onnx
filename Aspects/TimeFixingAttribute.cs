using System.Reflection;
using Cat_or_Dog.Helpers;
using MethodDecorator.Fody.Interfaces;

namespace Cat_or_Dog.Aspects
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TimeFixingAttribute : Attribute, IMethodDecorator
    {
        private readonly int _time;
        private readonly HighResolutionStopwatch _stopwatch;
        private readonly HighResolutionWait _waiter;

        public TimeFixingAttribute(int time)
        {
            _stopwatch = new HighResolutionStopwatch();
            _time = time;
            _waiter = new HighResolutionWait();
        }


        public void OnEntry()
        {
            _stopwatch.Start();
        }

        public async void OnExit()
        {
            _stopwatch.Stop();
            int elapsedTime = (int)_stopwatch.ElapsedMilliseconds;

            int sleepTime = _time - elapsedTime;

            if (sleepTime < 0) return;

            _waiter.Wait(sleepTime);
            
        }

        #region Didnt Use
        public void Init(object instance, MethodBase method, object[] args)
        {
            
        }
        public void OnException(Exception exception)
        {
            Console.WriteLine(exception);
        }
        #endregion
    }
}