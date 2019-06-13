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

			_currentImage = "test.jpg";
			_previousImage = new Command(ExecutePreviousImage);
			_nextImage = new Command(ExecuteNextImage);
			_openImageInFolder = new Command(ExecuteOpenImageInFolder);
			_likeImage = new Command(ExecuteLikeImage);
			_dislikeImage = new Command(ExecuteDislikeImage);
			_openMenu = new Command(ExecuteOpenMenu);
		}

		~MainViewModel()
		{
			Properties.Settings.Default.Save();
		}

		private System.Threading.Timer SaveTimer;
		private DirectoryInfo _picturesDir;
		private string _currentImage;
		private Command _previousImage;
		private Command _nextImage;
		private Command _openImageInFolder;
		private Command _likeImage;
		private Command _dislikeImage;
		private Command _openMenu;

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

		public Command PreviousImage
		{
			get { return _previousImage; }
		}

		public Command NextImage
		{
			get { return _nextImage; }
		}

		public Command OpenImageInFolder
		{
			get { return _openImageInFolder; }
		}

		public Command LikeImage
		{
			get { return _likeImage; }
		}

		public Command DislikeImage
		{
			get { return _dislikeImage; }
		}

		public Command OpenMenu
		{
			get { return _openMenu; }
		}

		public void ExecutePreviousImage()
		{

		}
		public void ExecuteNextImage()
		{

		}
		public void ExecuteOpenImageInFolder()
		{

		}
		public void ExecuteLikeImage()
		{

		}
		public void ExecuteDislikeImage()
		{

		}
		public void ExecuteOpenMenu()
		{

		}
	}
}
