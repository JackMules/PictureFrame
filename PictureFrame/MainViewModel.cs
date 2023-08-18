using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Forms;
using System.Windows.Media;
using System.Diagnostics;

namespace PictureFrame
{
	public class MainViewModel : BaseViewModel
	{
		public MainViewModel()
		{
			_previousImage = new Command(ExecutePreviousImage);
			_nextImage = new Command(ExecuteNextImage);
			_openImageInFolder = new Command(ExecuteOpenImageInFolder);
			_likeImage = new Command(ExecuteLikeImage);
			_dislikeImage = new Command(ExecuteDislikeImage);
			_changeImagesDir = new Command(ExecuteChangeImagesDir);
			_exit = new Command(ExecuteExit);

			_period = 60;
			_random = new Random();

			_randomise = Properties.Settings.Default.Randomise;
			_stretch = Properties.Settings.Default.Stretch;

			_saveTimer = new System.Threading.Timer(new TimerCallback(DoChangeRefresh), null, Timeout.Infinite, Timeout.Infinite);

			_pictureTimer = new System.Threading.Timer(new TimerCallback(DoNextImage), null, 0, _period * 1000);

			try
			{
				_imagesDir = new DirectoryInfo(Properties.Settings.Default.ImagesDir);
			}
			catch (NullReferenceException e)
			{
				MessageBox.Show("Couldn't initialise:" + e.ToString(), "Initialisation error");
			}

			AddImagesToList();

			CurrentIndex = Properties.Settings.Default.CurrentIndex;
		}

		~MainViewModel()
		{
			Properties.Settings.Default.Save();
		}

		private Random _random;
		private System.Threading.Timer _pictureTimer;
		private System.Threading.Timer _saveTimer;
		private DirectoryInfo _imagesDir;
		private int _currentIndex;
		private string _currentImage;
		private string _currentImageName;
		private List<FileInfo> _images;
		private bool _randomise;
		private int _period;
		private Stretch _stretch;
		private Command _previousImage;
		private Command _nextImage;
		private Command _openImageInFolder;
		private Command _likeImage;
		private Command _dislikeImage;
		private Command _changeImagesDir;
		private Command _exit;
		private Queue<int> _prevImages;

		public DirectoryInfo ImagesDir
		{
			get { return _imagesDir; }
			set
			{
				SetField(ref _imagesDir, value);
				Properties.Settings.Default.ImagesDir = value.FullName;
			}
		}

		public int CurrentIndex
		{
			get { return _currentIndex; }
			set
			{
				if (Images.Count > 0)
				{
					if (value < 0)
					{
						value = Images.Count - 1;
					}
					else if (value >= Images.Count)
					{
						value = 0;
					}
					SetField(ref _currentIndex, value);

					CurrentImage = Images[_currentIndex].FullName;
					CurrentImageName = Images[_currentIndex].Name;
					Properties.Settings.Default.CurrentIndex = value;
				}
			}
		}

		public string CurrentImage
		{
			get { return _currentImage; }
			set
			{
				SetField(ref _currentImage, value);
				Properties.Settings.Default.CurrentImage = value;
			}
		}

		public string CurrentImageName
		{
			get { return _currentImageName; }
			set { SetField(ref _currentImageName, value); }
		}

		public bool Fill
		{
			get { return Stretch == Stretch.UniformToFill; }
			set
			{
				if (value)
				{
					Stretch = Stretch.UniformToFill;
				}
				else
				{
					Stretch = Stretch.Uniform;
				}
			}
		}

		public Stretch Stretch
		{
			get { return _stretch; }
			set
			{
				SetField(ref _stretch, value);
				Properties.Settings.Default.Stretch = value;
			}
		}

		public List<FileInfo> Images
		{
			get { return _images; }
			set { SetField(ref _images, value); }
		}

		public bool Randomise
		{
			get { return _randomise; }
			set
			{
				SetField(ref _randomise, value);
				Properties.Settings.Default.Randomise = value;
			}
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
					RestartSaveTimer();
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
					RestartSaveTimer();
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
					RestartSaveTimer();
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
					RestartSaveTimer();
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

		void RestartSaveTimer()
		{
			_saveTimer.Change(200, Timeout.Infinite);
		}

		void DoNextImage(object state)
		{
			System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
			{
				ExecuteNextImage();
			}));
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

		public Command ChangeImagesDir
		{
			get { return _changeImagesDir; }
		}

		public Command Exit
		{
			get { return _exit; }
		}

		public void ExecutePreviousImage()
		{
			CurrentIndex = _prevImages.Last();
		}

		public void ExecuteNextImage()
		{
			if (Randomise)
			{
				_prevImages.Enqueue(CurrentIndex);
				if (_prevImages.Count > 100)
				{
					_prevImages.Dequeue();
				}
				CurrentIndex = _random.Next(Images.Count - 1);
			}
			else
			{
				++CurrentIndex;
			}
		}

		public void ExecuteOpenImageInFolder()
		{
			if (Images.Count > 0 &&
				Images[CurrentIndex].Exists)
			{
				string arg = "/select, \"" + Images[CurrentIndex].FullName + "\"";
				Process.Start("explorer.exe", arg);
			}
		}

		public void ExecuteLikeImage()
		{

		}

		public void ExecuteDislikeImage()
		{

		}

		public void ExecuteChangeImagesDir()
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			folderBrowserDialog.Description = "Select the directory containing your images.";
			folderBrowserDialog.ShowNewFolderButton = false;

			DialogResult result = folderBrowserDialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				ImagesDir = new DirectoryInfo(folderBrowserDialog.SelectedPath);
			}

			AddImagesToList();

			CurrentIndex = 0;
		}

		public void AddImagesToList()
		{
			Images = new List<FileInfo>();

			foreach (FileInfo file in ImagesDir.GetFiles())
			{
				if (file.Extension == ".jpg" ||
						file.Extension == ".jpeg" ||
						file.Extension == ".png" ||
						file.Extension == ".gif" ||
						file.Extension == ".bmp")
				{
					Images.Add(file);
				}
			}
		}

		public void ExecuteExit()
		{
			System.Windows.Application.Current.Shutdown();
		}
	}
}
