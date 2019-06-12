using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Forms;

namespace PictureFrame
{
	public class MainViewModel : BaseViewModel
	{
		public MainViewModel()
		{
			SaveTimer = new System.Threading.Timer(new TimerCallback(DoChangeRefresh), null, Timeout.Infinite, Timeout.Infinite);

			try
			{
				_picturesDir = new DirectoryInfo(Properties.Settings.Default.PicturesDir);
			}
			catch (NullReferenceException e)
			{
				MessageBox.Show("Couldn't initialise:" + e.ToString(), "Initialisation error");
			}
		}

		~MainViewModel()
		{
			Properties.Settings.Default.Save();
		}

		private System.Threading.Timer SaveTimer;
		private DirectoryInfo _picturesDir;
		private string _currentImage;

		public DirectoryInfo PicturesDir
		{
			get { return _picturesDir; }
			set { SetField(ref _picturesDir, value); }
		}

		public string CurrentImage
		{
			get { return _currentImage; }
			set { SetField(ref _currentImage, value); }
		}

		public double WindowWidth
		{
			get
			{
				return Properties.Settings.Default.Width;
			}
			set
			{
				if (Properties.Settings.Default.Width != value)
				{
					Properties.Settings.Default.Width = value;
					NotifyPropertyChanged("WindowWidth");
					RestartTimer();
				}
			}
		}

		public double WindowHeight
		{
			get
			{
				return Properties.Settings.Default.Height;
			}
			set
			{
				if (Properties.Settings.Default.Height != value)
				{
					Properties.Settings.Default.Height = value;
					NotifyPropertyChanged("WindowHeight");
					RestartTimer();
				}
			}
		}

		public double WindowTop
		{
			get
			{
				return Properties.Settings.Default.Top;
			}
			set
			{
				if (Properties.Settings.Default.Top != value)
				{
					Properties.Settings.Default.Top = value;
					NotifyPropertyChanged("WindowTop");
					RestartTimer();
				}
			}
		}

		public double WindowLeft
		{
			get
			{
				return Properties.Settings.Default.Left;
			}
			set
			{
				if (Properties.Settings.Default.Left != value)
				{
					Properties.Settings.Default.Left = value;
					NotifyPropertyChanged("WindowLeft");
					RestartTimer();
				}
			}
		}

		void DoChangeRefresh(object state)
		{
			System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
			{
				Properties.Settings.Default.Save();
			}));
		}

		void RestartTimer()
		{
			SaveTimer.Change(200, Timeout.Infinite);
		}
	}
}
