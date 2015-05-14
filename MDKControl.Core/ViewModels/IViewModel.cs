using System;
using System.ComponentModel;

namespace MDKControl.ViewModels
{
    public interface IViewModel : INotifyPropertyChanged
    {
        string Title { get; set; }
        void SetState<T>(Action<T> action) where T : class, IViewModel;
    }
}

