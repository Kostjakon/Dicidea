using System;
using System.Collections.Generic;
using Dicidea.Core.Helper;
using Newtonsoft.Json;

namespace Dicidea.Core.Models
{
    public class RollEmSpace : NotifyDataErrorInfo<RollEmSpace>
    {
        private string _name;
        private string _description;
        private List<Dice> _dices;
        private List<string> _diceIds;
        public RollEmSpace()
        {
            Rules.Add(new DelegateRule<RollEmSpace>(nameof(Name), "The roll'em space has to have a name.", r => !string.IsNullOrWhiteSpace(r?.Name)));
            Id ??= Guid.NewGuid().ToString("N");
        }
        [JsonProperty(PropertyName = "RollEmSpaceId", Required = Required.Always)]
        public string Id { get; }
        [JsonProperty(Required = Required.Always)]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        [JsonIgnore]
        public virtual List<Dice> Dices
        {
            get => _dices;
            set => SetProperty(ref _dices, value);
        }

        public virtual List<string> DiceIds
        {
            get => _diceIds;
            set => SetProperty(ref _diceIds, value);
        }
    }
}
