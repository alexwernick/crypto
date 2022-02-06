using System.Threading;
using System.Threading.Tasks;

namespace Crypto.Threading
{
    public class ThreadSafeSingleShotGuard
    {
        private readonly int? _resetDelaySeconds;
        public ThreadSafeSingleShotGuard(int? resetDelaySeconds = null)
        {
            _resetDelaySeconds = resetDelaySeconds;
        }

        private static int NotCalled = 0;
        private static int Called = 1;
        private int _state = NotCalled;

        public bool CheckAndSetFirstCall
        {
            get
            {
                bool firstCall = Interlocked.Exchange(ref _state, Called) == NotCalled;

                if (firstCall && _resetDelaySeconds.HasValue)
                {
                    var _ = Task.Run(() => DelayReset(_resetDelaySeconds.Value));
                }

                return firstCall;
            }
        }

        public void Reset()
        {
            Interlocked.Exchange(ref _state, NotCalled);
        }

        private void DelayReset(int resetDelaySeconds)
        {
            Thread.Sleep(resetDelaySeconds * 1000);
            Reset();
        }
    }
}
