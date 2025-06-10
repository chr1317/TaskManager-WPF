using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;

namespace TaskManagerWpfApp.ViewModels
{
    public class StatisticsViewModel : ObservableObject
    {
        public ISeries[] TaskCompletionSeries { get; set; }

        public StatisticsViewModel(IEnumerable<ToDoTasks> tasks)
        {
            int completed = tasks.Count(t => t.Status == TaskStatus.Complete);
            int notCompleted = tasks.Count() - completed;


            TaskCompletionSeries = new ISeries[]
            {
                new PieSeries<int>
                {
                    Values = new[] { completed },
                    Name = "Ukończone",
                    TooltipLabelFormatter = (chartPoint) =>
                        $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue} ({chartPoint.StackedValue.Share:P0})",
                    Fill = new SolidColorPaint(SKColors.SeaGreen)
                },
                new PieSeries<int>
                {
                    Values = new[] { notCompleted },
                    Name = "Nieukończone",
                    TooltipLabelFormatter = (chartPoint) =>
                        $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue} ({chartPoint.StackedValue.Share:P0})",
                    Fill = new SolidColorPaint(SKColors.IndianRed)
                }
            };

        }
    }
}
