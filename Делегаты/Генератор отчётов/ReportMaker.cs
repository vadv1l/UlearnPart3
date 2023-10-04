using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Delegates.Reports
{
    public static class StatisticsCounter
    {
        public static object GetMeanAndStdOfStatistic(IEnumerable<double> data)
        {
            var dataArray = data as double[] ?? data.ToArray();
            var mean = dataArray.Average();
            var std = Math.Sqrt(dataArray.Select(z => Math.Pow(z - mean, 2)).Sum() / (dataArray.Length - 1));

            return new MeanAndStd
            {
                Mean = mean,
                Std = std
            };
        }

        public static object GetMedianOfStatistic(IEnumerable<double> data)
        {
            var sortedArray = data.OrderBy(item => item).ToArray();
            if (sortedArray.Length % 2 == 0)
                return (sortedArray[sortedArray.Length / 2] + sortedArray[sortedArray.Length / 2 - 1]) / 2;

            return sortedArray[sortedArray.Length / 2];
        }
    }

    public static class ReportMaker
    {
        private static IEnumerable<(string, object)> GetItems(IEnumerable<Measurement> measurements,
            Func<IEnumerable<double>, object> statisticFunction)
        {
            var dataArray = measurements as Measurement[] ?? measurements.ToArray();
            var temperature = statisticFunction(dataArray.Select(measurement => measurement.Temperature));
            var humidity = statisticFunction(dataArray.Select(measurement => measurement.Humidity));
            return new[]
            {
                ("Temperature", temperature),
                ("Humidity", humidity)
            };
        }

        public static string GetMarkdownString(IEnumerable<Measurement> measurements,
            Func<IEnumerable<double>, object> statisticFunction, string caption)
        {
            var dataArray = measurements as Measurement[] ?? measurements.ToArray();
            var reportStringBuilder = new StringBuilder();
            reportStringBuilder.Append($"## {caption}\n\n");
			foreach(var itemInfo in GetItems(dataArray, statisticFunction))
				reportStringBuilder.Append($" * **{itemInfo.Item1}**: {itemInfo.Item2}\n\n");
            return reportStringBuilder.ToString();
        }

        public static string GetHtmlString(IEnumerable<Measurement> measurements,
            Func<IEnumerable<double>, object> statisticFunction, string caption)
        {
            var dataArray = measurements as Measurement[] ?? measurements.ToArray();
            var reportStringBuilder = new StringBuilder();
            reportStringBuilder.Append($"<h1>{caption}</h1><ul>");
            foreach (var itemInfo in GetItems(dataArray, statisticFunction))
                reportStringBuilder.Append($"<li><b>{itemInfo.Item1}</b>: {itemInfo.Item2}");
            reportStringBuilder.Append($"</ul>");
            return reportStringBuilder.ToString();
        }
    }

    public static class ReportMakerHelper
	{
		public static string MeanAndStdHtmlReport(IEnumerable<Measurement> data)
		{
			return ReportMaker.GetHtmlString(data, StatisticsCounter.GetMeanAndStdOfStatistic,
                "Mean and Std");
		}

		public static string MedianMarkdownReport(IEnumerable<Measurement> data)
		{
            return ReportMaker.GetMarkdownString(data, StatisticsCounter.GetMedianOfStatistic,
                "Median");
        }

		public static string MeanAndStdMarkdownReport(IEnumerable<Measurement> measurements)
		{
            return ReportMaker.GetMarkdownString(measurements, StatisticsCounter.GetMeanAndStdOfStatistic,
                "Mean and Std");
        }

		public static string MedianHtmlReport(IEnumerable<Measurement> measurements)
		{
            return ReportMaker.GetHtmlString(measurements, StatisticsCounter.GetMedianOfStatistic,
                "Median");
        }
	}
}