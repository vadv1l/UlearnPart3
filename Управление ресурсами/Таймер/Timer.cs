using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Memory.Timers
{
    public class Timer : IDisposable
    {
        private bool _disposed = false;

        private static StringWriter s_writer = new StringWriter();

        public readonly Stopwatch Stopwatch = new Stopwatch();

        public readonly List<Timer> ChildList = new List<Timer>();

        public long Duration { get; private set; }

        public long Rest { get; private set; }

        public int Level { get; }

        public string Name { get; }

        public Timer(string name) : this(name, 0) { }

        public Timer(string name, int level)
        {
            Name = name;
            Level = level;
        }

        ~Timer() => Dispose(false);

        public static Timer Start(StringWriter writer, string name = "*")
        {
            s_writer = writer;
            var timer = new Timer(name);
            timer.Stopwatch.Start();
            return timer;
        }

        public Timer StartChildTimer(string name)
        {
            var childTimer = new Timer(name, Level + 1);
            ChildList.Add(childTimer);
            childTimer.Stopwatch.Start();
            return childTimer;
        }

        private static string FormatReportLine(string timerName, int level, long value)
        {
            var intro = new string(' ', level * 4) + timerName;
            return $"{intro,-20}: {value}\n";
        }

        public void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (!disposing) return;
            Stopwatch.Stop();
            Duration = Stopwatch.Elapsed.Milliseconds;
            Rest = Duration - ChildList.Sum(child =>
            {
                child.Stopwatch.Stop();
                return child.Stopwatch.Elapsed.Milliseconds;
            });
            if (Level == 0) WriteReport(this);
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private static void WriteReport(Timer timer)
        {
            s_writer.Write(FormatReportLine(timer.Name, timer.Level, timer.Duration));
            if (!timer.ChildList.Any()) return;
            timer.ChildList.ForEach(WriteReport);
            s_writer.Write(FormatReportLine(nameof(Rest), timer.Level + 1, timer.Rest));
        }
    }
}