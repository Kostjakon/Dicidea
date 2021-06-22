using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

namespace Dicidea.Core.Helper
{
    public abstract class Disposable : IDisposable
    {
        #region Fields
        private bool isDisposed;
        private Subject<Unit> whenDisposedSubject;
        #endregion

        #region Desctructors
        ~Disposable()
        {
            this.Dispose(false);
        }
        #endregion

        #region Properties
        public IObservable<Unit> WhenDisposed
        {
            get
            {
                if (this.IsDisposed)
                {
                    return Observable.Return(Unit.Default);
                }
                else
                {
                    if (this.whenDisposedSubject == null)
                    {
                        this.whenDisposedSubject = new Subject<Unit>();
                    }
                    return this.whenDisposedSubject.AsObservable();
                }
            }
        }
        public bool IsDisposed
        {
            get { return this.isDisposed; }
        }
        #endregion

        #region Public Methods
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Protected Methods
        protected virtual void DisposeManaged()
        {
        }
        protected virtual void DisposeUnmanaged()
        {
        }
        protected void ThrowIfDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }
        #endregion

        #region Private Methods
        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.DisposeManaged();
                }
                this.DisposeUnmanaged();
                this.isDisposed = true;
                if (this.whenDisposedSubject != null)
                {
                    this.whenDisposedSubject.OnNext(Unit.Default);
                    this.whenDisposedSubject.OnCompleted();
                    this.whenDisposedSubject.Dispose();
                }
            }
        }
        #endregion
    }
}
