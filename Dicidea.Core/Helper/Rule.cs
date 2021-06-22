using System;
using System.Collections.Generic;
using System.Text;

namespace Dicidea.Core.Helper
{
    public abstract class Rule<T>
    {
        #region Constructors

        protected Rule(string propertyName, object error)
        {
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            Error = error ?? throw new ArgumentNullException(nameof(error));
        }

        #endregion

        #region Apply

        public abstract bool Apply(T obj);

        #endregion

        #region Properties

        public string PropertyName { get; }

        public object Error { get; }

        #endregion
    }
}
