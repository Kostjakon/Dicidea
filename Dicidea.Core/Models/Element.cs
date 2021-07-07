using System;
using System.Collections.Generic;
using Dicidea.Core.Helper;
using Newtonsoft.Json;

namespace Dicidea.Core.Models
{
    /// <summary>
    /// Model Klasse eines Elements eines Würfels
    /// </summary>
    public sealed class Element : NotifyDataErrorInfo<Element>
    {
        private string _name;
        private int _amount;
        private bool _active;
        private int _valueAmount;
        private bool _onlyUnique;
        private bool _canBeUnique;

        public Element(bool newElement)
        {
            Values = new List<Value>
            {
                new Value(newElement)
            };
            Rules.Add(new DelegateRule<Element>(nameof(Name), "The element has to have a name.", e => !string.IsNullOrWhiteSpace(e?.Name)));
            Id = Guid.NewGuid().ToString("N"); 
            Name = "";
            Amount = 1;
            CanBeUnique = false;
            ValueAmount = 1;
            Active = true;
            OnlyUnique = true;
        }
        public Element()
        {
            Rules.Add(new DelegateRule<Element>(nameof(Name), "The element has to have a name.", e => !string.IsNullOrWhiteSpace(e?.Name)));
        }

        [JsonProperty(PropertyName = "ElementId", Required = Required.Always)]
        public string Id { get; set; }

        public string Name { get => _name; set => SetProperty(ref _name, value); }

        public List<Value> Values { get; set; }

        /// <summary>
        /// Funktion die die Anzahl der aktiven Werte der Werte Liste zurückgibt
        /// </summary>
        /// <returns>Anzahl aktiver Werte</returns>
        public int GetActiveValueCount()
        {
            int activeCount = 0;
            if (Values != null)
            {
                foreach (Value value in Values)
                {
                    if (value.Active)
                    {
                        activeCount++;
                    }
                }
            }

            return activeCount;
        }
        public int Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }
        public int ValueAmount
        {
            get => _valueAmount;
            set {
                if(Values != null) CanBeUnique = (GetActiveValueCount() >= value);
                SetProperty(ref _valueAmount, value);
            }
        }

        public bool Active
        {
            get => _active;
            set => SetProperty(ref _active, value);
        }

        public bool OnlyUnique
        {
            get => _onlyUnique;
            set
            {
                if (CanBeUnique)
                {
                    SetProperty(ref _onlyUnique, value);
                }
                else
                {
                    SetProperty(ref _onlyUnique, false);
                }
            }
        }

        /// <summary>
        /// Funktion um die Aktualisierung von CanBeUnique manuell anzustoßen
        /// </summary>
        public void UpdateCanBeUnique()
        {
            CanBeUnique = (GetActiveValueCount() >= ValueAmount);
        }

        public bool CanBeUnique
        {
            get => _canBeUnique;
            set
            {
                if (!value)
                {
                    OnlyUnique = false;
                }
                SetProperty(ref _canBeUnique, value);
            }
        }
    }
}
