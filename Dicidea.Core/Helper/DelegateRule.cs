using System;
using System.Collections.Generic;
using System.Text;

namespace Dicidea.Core.Helper
{
    public sealed class DelegateRule<T> : Rule<T>
    {
        private readonly Func<T, bool> _rule;

        #region Constructors

        public DelegateRule(string propertyName, object error, Func<T, bool> rule)
            : base(propertyName, error)
        {
            _rule = rule ?? throw new ArgumentNullException(nameof(rule));
        }

        #endregion

        #region Rule<T> Members

        public override bool Apply(T obj)
        {
            return _rule(obj);
        }

        #endregion
    }
}
