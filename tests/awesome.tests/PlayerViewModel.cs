using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TOTOllyGeek.Awesome.Tests;

public class PlayerViewModel : INotifyPropertyChanged
{
    private string _nickName;
    
    public string NickName
    {
        get => _nickName;
        set => SetField(ref _nickName, value);
    }

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
    #endregion
}