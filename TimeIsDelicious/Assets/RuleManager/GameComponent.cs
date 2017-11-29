using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RuleManager
{
    class GameComponent : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public Guid GUID { get; } = Guid.NewGuid();
    }
}
