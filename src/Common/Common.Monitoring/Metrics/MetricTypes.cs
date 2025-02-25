using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Common.Monitoring.Metrics;

public abstract class Metric<T>(string name, string? unit, string? description) where T : struct
{
    public string Name { get; } = name;
    public string? Unit { get; } = unit;
    public string? Description { get; } = description;
}

public class CounterMetric<T>(Counter<T> counter)
    : Metric<T>(counter.Name, counter.Unit, counter.Description) where T : struct
{
    public Counter<T> Counter { get; } = counter;

    public void Inc(T amount = default, params KeyValuePair<string, object?>[] tags)
    {
        if (tags.Length > 0)
            Counter.Add(amount, tags);
        else
            Counter.Add(amount);
    }
}

public class GaugeMetric<T>(Gauge<T> gauge)
    : Metric<T>(gauge.Name, gauge.Unit, gauge.Description) where T : struct
{
    private readonly Gauge<T> _gauge = gauge;

    public T Value { get; private set; }

    public void Set(T value)
    {
        Value = value;
    }

    public void Inc(T amount)
    {
        dynamic currentValue = Value;
        dynamic incrementAmount = amount;
        Value = (T)(currentValue + incrementAmount);
    }

    public void Dec(T amount)
    {
        dynamic currentValue = Value;
        dynamic decrementAmount = amount;
        Value = (T)(currentValue - decrementAmount);
    }
}

public class HistogramMetric<T>(Histogram<T> histogram)
    : Metric<T>(histogram.Name, histogram.Unit, histogram.Description) where T : struct
{
    public Histogram<T> Histogram { get; } = histogram;

    public void Record(T value, params KeyValuePair<string, object?>[] tags)
    {
        if (tags.Length > 0)
            Histogram.Record(value, tags);
        else
            Histogram.Record(value);
    }

    public IDisposable StartTimer(params KeyValuePair<string, object?>[] tags)
    {
        return new HistogramTimer<T>(Histogram, tags);
    }
}

internal class HistogramTimer<T>(Histogram<T> histogram, KeyValuePair<string, object?>[] tags)
    : IDisposable where T : struct
{
    private readonly Histogram<T> _histogram = histogram;
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private readonly KeyValuePair<string, object?>[] _tags = tags;

    public void Dispose()
    {
        _stopwatch.Stop();
        dynamic elapsed = _stopwatch.Elapsed.TotalMilliseconds;
        _histogram.Record((T)elapsed, _tags);
    }
}