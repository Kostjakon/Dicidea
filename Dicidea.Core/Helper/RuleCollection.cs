using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;

namespace Dicidea.Core.Helper
{
    public sealed class RuleCollection<T> : Collection<Rule<T>>
    {
        #region Public Methods

        public void Add(string propertyName, object error, Func<T, bool> rule)
        {
            Add(new DelegateRule<T>(propertyName, error, rule));
        }

        public IEnumerable<object> Apply(T obj, string propertyName)
        {
            var errors = new List<object>();
            foreach (Rule<T> rule in this)
                if (string.IsNullOrEmpty(propertyName) || rule.PropertyName.Equals(propertyName))
                    if (!rule.Apply(obj) && !errors.Contains(rule.Error))
                        errors.Add(rule.Error);
            return errors;
        }

        #endregion
    }
}
