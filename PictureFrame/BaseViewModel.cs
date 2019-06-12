using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace PictureFrame
{
	public class BaseViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public virtual void NotifyPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected bool SetField<T>(ref T currentValue, T newValue, [CallerMemberName]string propertyName = null)
		{
			bool fieldChanged = false;

			if (!EqualityComparer<T>.Default.Equals(currentValue, newValue))
			{
				currentValue = newValue;

				NotifyPropertyChanged(propertyName);

				fieldChanged = true;
			}

			return fieldChanged;
		}
	}
}
