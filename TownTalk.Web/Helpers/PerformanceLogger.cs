using System.Diagnostics;

namespace TownTalk.Helpers;
public class PerformanceLogger
{
    private Stopwatch _stopwatch;

    public PerformanceLogger()
    {
        _stopwatch = new Stopwatch();
    }

    public void Start()
    {
        _stopwatch.Start();
    }

    public void Stop(string label = "Execution Time")
    {
        _stopwatch.Stop();
        Logger.Log($"{label}: {_stopwatch.ElapsedMilliseconds} ms");
    }
}