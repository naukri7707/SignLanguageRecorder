using System.Diagnostics;

namespace SignLanguageRecorder.Utilities;

public static class Awaiter
{
    public static int Interval { get; set; } = 100;

    public static async Task WaitUntil(Func<bool> predicate)
    {
        while (!predicate())
        {
            await Task.Delay(Interval);
        }
    }

    public static async Task<bool> WaitUntil(Func<bool> predicate, TimeSpan timeout)
    {
        var stopWatch = Stopwatch.StartNew();

        while (!predicate())
        {
            if (stopWatch.Elapsed > timeout)
            {
                return false;
            }

            await Task.Delay(Interval);
        }

        return true;
    }

    public static async Task WaitWhile(Func<bool> predicate)
    {
        while (predicate())
        {
            await Task.Delay(Interval);
        }
    }

    public static async Task<bool> WaitWhile(Func<bool> predicate, TimeSpan timeout)
    {
        var stopWatch = Stopwatch.StartNew();

        while (predicate())
        {
            if (stopWatch.Elapsed > timeout)
            {
                return false;
            }

            await Task.Delay(Interval);
        }

        return true;
    }
}
