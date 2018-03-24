using System;
using System.ComponentModel;

namespace BoardGameLeagueLib.DbClasses
{
    // Adapted from http://paulstovell.com/blog/dynamic-datagrid
    public class GenericProperty : INotifyPropertyChanged
    {
        public GenericProperty(string a_Name, object a_Value)
        {
            Name = a_Name;
            Value = a_Value;
        }

        public string Name { get; private set; }
        public object Value { get; set; }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged(String a_PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(a_PropertyName));
        }

        #endregion
    }
}
