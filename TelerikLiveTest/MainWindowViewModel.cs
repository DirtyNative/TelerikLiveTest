using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

namespace TelerikLiveTest
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		private readonly Random _random = new();

		private DateTime _visibleMinimum = new(2021, 11, 11);
		private DateTime _visibleMaximum = new(2021, 11, 11, 2, 0, 0);

		private ObservableCollection<DataPoint> _dataPoints = new();

		public const double DataFps = 100;
		public const double RefreshFps = 60;

		public MainWindowViewModel()
		{
			var samplePointTimer = new Timer();
			samplePointTimer.Elapsed += SamplePointTimerTick;
			samplePointTimer.Interval = DataFps;
			samplePointTimer.Start();

			var updateTimer = new DispatcherTimer();
			updateTimer.Tick += UpdateTimerOnElapsed;
			updateTimer.Interval = TimeSpan.FromMilliseconds(1000d / RefreshFps);
			updateTimer.Start();
		}


		public ObservableCollection<DataPoint> DataPoints
		{
			get => _dataPoints;
			set
			{
				_dataPoints = value;
				OnPropertyChanged(nameof(DataPoints));
			}
		}

		public DateTime VisibleMinimum
		{
			get => _visibleMinimum;
			set
			{
				_visibleMinimum = value;
				OnPropertyChanged(nameof(VisibleMinimum));
			}
		}

		public DateTime VisibleMaximum
		{
			get => _visibleMaximum;
			set
			{
				_visibleMaximum = value;
				OnPropertyChanged(nameof(VisibleMaximum));
			}
		}

		public Size MaxZoom => new(3, 1);

		public double VisibleSeconds => 30;

		private void SamplePointTimerTick(object? sender, EventArgs e)
		{
			GenerateSamplePoint();
		}

		private void UpdateTimerOnElapsed(object? sender, EventArgs e)
		{
			lock (((ICollection) DataPoints).SyncRoot)
			{
				VisibleMinimum = DateTime.Now.AddSeconds(-(VisibleSeconds + 1));
				VisibleMaximum = DateTime.Now.AddSeconds(-1);
			}
		}

		private void GenerateSamplePoint()
		{
			var x = DateTime.Now;
			var y = _random.Next(0, 1000);

			var point = new DataPoint(x, y);

			Application.Current.Dispatcher.InvokeAsync(() =>
			{
				DataPoints.Add(point);
				DataPoints.Remove((e) => e.X < VisibleMinimum);
			}, DispatcherPriority.ApplicationIdle);
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}