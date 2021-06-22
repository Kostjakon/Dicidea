using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Dicidea.Core.Helper
{
    public abstract class NotifyPropertyChanges : Disposable, INotifyPropertyChanged //, INotifyPropertyChanging
    {
        #region Public Events
        
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add => PropertyChanged += value;
            remove => PropertyChanged -= value;
        }
        
        private event PropertyChangedEventHandler PropertyChanged;
        
        public IObservable<string> WhenPropertyChanged
        {
            get
            {
                ThrowIfDisposed();
                return Observable
                    .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                        h => PropertyChanged += h,
                        h => PropertyChanged -= h)
                    .Select(x => x.EventArgs.PropertyName);
            }
        }
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Debug.Assert(
                string.IsNullOrEmpty(propertyName) ||
                GetType().GetRuntimeProperty(propertyName) != null,
                "Check that the property name exists for this instance.");
            var eventHandler = PropertyChanged;
            eventHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        protected void OnPropertyChanged(params string[] propertyNames)
        {
            if (propertyNames == null)
                throw new ArgumentNullException(nameof(propertyNames));
            foreach (var propertyName in propertyNames)
                OnPropertyChanged(propertyName);
        }
        
        protected virtual void OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            Debug.Assert(
                string.IsNullOrEmpty(propertyName) ||
                GetType().GetRuntimeProperty(propertyName) != null,
                "Check that the property name exists for this instance.");
            //PropertyChangingEventHandler eventHandler = this.PropertyChanging;
            
        }
        
        protected void OnPropertyChanging(params string[] propertyNames)
        {
            if (propertyNames == null)
                throw new ArgumentNullException(nameof(propertyNames));
            foreach (var propertyName in propertyNames)
                OnPropertyChanging(propertyName);
        }
        
        protected bool SetProperty<TProp>(
            ref TProp currentValue,
            TProp newValue,
            [CallerMemberName] string propertyName = null)
        {
            ThrowIfDisposed();
            if (!Equals(currentValue, newValue))
            {
                OnPropertyChanging(propertyName);
                currentValue = newValue;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }
        
        protected bool SetProperty<TProp>(
            ref TProp currentValue,
            TProp newValue,
            params string[] propertyNames)
        {
            ThrowIfDisposed();
            if (!Equals(currentValue, newValue))
            {
                OnPropertyChanging(propertyNames);
                currentValue = newValue;
                OnPropertyChanged(propertyNames);
                return true;
            }
            return false;
        }
        
        protected bool SetProperty(
            Func<bool> equal,
            Action action,
            [CallerMemberName] string propertyName = null)
        {
            ThrowIfDisposed();
            if (equal())
                return false;
            OnPropertyChanging(propertyName);
            action();
            OnPropertyChanged(propertyName);
            return true;
        }

        protected bool SetProperty(
            Func<bool> equal,
            Action action,
            params string[] propertyNames)
        {
            ThrowIfDisposed();
            if (equal())
                return false;
            OnPropertyChanging(propertyNames);
            action();
            OnPropertyChanged(propertyNames);
            return true;
        }

        #endregion
    }
}
