namespace Denxorz.Latch;

public class FullLatch
{
    public bool IsLatched { get; private set; }

    public void LatchAndRun(Action action)
    {
        try
        {
            IsLatched = true;
            action();
        }
        finally
        {
            IsLatched = false;
        }
    }

    public void RunIfNotLatched(Action action)
    {
        if (IsLatched)
        {
            return;
        }
        action();
    }
}
