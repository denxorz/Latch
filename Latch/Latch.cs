using System;

namespace Denxorz.Latch
{
    public class Latch
    {
        private readonly FullLatch latch = new FullLatch();

        public bool IsLatched => latch.IsLatched;

        public void RunLatched(Action action)
        {
            latch.RunIfNotLatched(() => latch.LatchAndRun(action));
        }
    }
}