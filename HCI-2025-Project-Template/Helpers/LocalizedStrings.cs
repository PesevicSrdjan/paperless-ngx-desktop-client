using System.ComponentModel;
using System.Resources;
using System.Threading;

namespace HCI_2025_Project_Template.Helpers
{
    public class LocalizedStrings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string this[string key]
        {
            get
            {
                return Resources.Strings.ResourceManager.GetString(key, Thread.CurrentThread.CurrentUICulture) ?? $"!{key}!";
            }
        }
        public void RaiseAllPropertiesChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}