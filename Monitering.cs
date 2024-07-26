using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.ObjectModel;

namespace GORAE_REF_SYSTEM
{
    public class MainViewModel
    {
        public PlotModel AccumInTemp { get; private set; }
        public ObservableCollection<DataPoint> AccumInTempData { get; private set; }

        public PlotModel AccumInPress { get; private set; }
        public ObservableCollection<DataPoint> AccumInPressData { get; private set; }

        public PlotModel AccumOutTemp { get; private set; }
        public ObservableCollection<DataPoint> AccumOutTempData { get; private set; }

        public PlotModel AccumOutPress { get; private set; }
        public ObservableCollection<DataPoint> AccumOutPressData { get; private set; }

        public PlotModel BoosterInTemp { get; private set; }
        public ObservableCollection<DataPoint> BoosterInTempData { get; private set; }

        public PlotModel BoosterInPress { get; private set; }
        public ObservableCollection<DataPoint> BoosterInPressData { get; private set; }

        public PlotModel BoosterOutTemp { get; private set; }
        public ObservableCollection<DataPoint> BoosterOutTempData { get; private set; }

        public PlotModel BoosterOutPress { get; private set; }
        public ObservableCollection<DataPoint> BoosterOutPressData { get; private set; }

        public PlotModel Vacuum { get; private set; }
        public ObservableCollection<DataPoint> VacuumData { get; private set; }

        public MainViewModel()
        {
            AccumInTemp = CreatePlotModel("Accum In Temp", 0, 50);
            AccumInTempData = new ObservableCollection<DataPoint>();
            AccumInTemp.Series.Add(new LineSeries { ItemsSource = AccumInTempData });

            AccumInPress = CreatePlotModel("Accum In Press", 0, 50);
            AccumInPressData = new ObservableCollection<DataPoint>();
            AccumInPress.Series.Add(new LineSeries { ItemsSource = AccumInPressData });

            AccumOutTemp = CreatePlotModel("Accum Out Temp", 0, 50);
            AccumOutTempData = new ObservableCollection<DataPoint>();
            AccumOutTemp.Series.Add(new LineSeries { ItemsSource = AccumOutTempData });

            AccumOutPress = CreatePlotModel("Accum Out Press", 0, 50);
            AccumOutPressData = new ObservableCollection<DataPoint>();
            AccumOutPress.Series.Add(new LineSeries { ItemsSource = AccumOutPressData });

            BoosterInTemp = CreatePlotModel("Booster In Temp", 0, 50);
            BoosterInTempData = new ObservableCollection<DataPoint>();
            BoosterInTemp.Series.Add(new LineSeries { ItemsSource = BoosterInTempData });

            BoosterInPress = CreatePlotModel("Booster In Press", 0, 50);
            BoosterInPressData = new ObservableCollection<DataPoint>();
            BoosterInPress.Series.Add(new LineSeries { ItemsSource = BoosterInPressData });

            BoosterOutTemp = CreatePlotModel("Booster Out Temp", 0, 50);
            BoosterOutTempData = new ObservableCollection<DataPoint>();
            BoosterOutTemp.Series.Add(new LineSeries { ItemsSource = BoosterOutTempData });

            BoosterOutPress = CreatePlotModel("Booster Out Press", 0, 50);
            BoosterOutPressData = new ObservableCollection<DataPoint>();
            BoosterOutPress.Series.Add(new LineSeries { ItemsSource = BoosterOutPressData });

            Vacuum = CreatePlotModel("Vacuum", 0, 100);
            VacuumData = new ObservableCollection<DataPoint>();
            Vacuum.Series.Add(new LineSeries { ItemsSource = VacuumData });
        }

        private PlotModel CreatePlotModel(string title, double yMin, double yMax)
        {
            var plotModel = new PlotModel { Title = title };
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = yMin, Maximum = yMax });
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
            return plotModel;
        }

        public void UpdatePlotModel(ObservableCollection<DataPoint> dataPoints, double xValue, double yValue)
        {
            dataPoints.Add(new DataPoint(xValue, yValue));
        }

        public void RefreshPlots()
        {
            AccumInTemp.InvalidatePlot(true);
            AccumInPress.InvalidatePlot(true);
            AccumOutTemp.InvalidatePlot(true);
            AccumOutPress.InvalidatePlot(true);
            BoosterInTemp.InvalidatePlot(true);
            BoosterInPress.InvalidatePlot(true);
            BoosterOutTemp.InvalidatePlot(true);
            BoosterOutPress.InvalidatePlot(true);
            Vacuum.InvalidatePlot(true);
        }
    }
}