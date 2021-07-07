using System;
using System.Collections.Generic;
using Dicidea.Core.Helper;
using Newtonsoft.Json;

namespace Dicidea.Core.Models
{
    /// <summary>
    /// Model Klasse einer Idee
    /// </summary>
    public sealed class Idea : NotifyDataErrorInfo<Idea>
    {
        private string _name;
        private string _sectionName;
        private string _description;
        private List<IdeaCategory> _ideaCategories;
        private DateTime _rolledDate;
        private bool _save;
        public Idea(string name, string sectionName, string description)
        {
            IdeaCategories = new List<IdeaCategory>();
            SectionName = sectionName;
            Name = name;
            Description = description;
            Id = Guid.NewGuid().ToString("N");
            Save = false;
            Rules.Add(new DelegateRule<Idea>(nameof(Name), "The element has to have a name.", e => !string.IsNullOrWhiteSpace(e?.Name)));
            RolledDate = DateTime.Now;
        }

        public Idea()
        {
            Rules.Add(new DelegateRule<Idea>(nameof(Name), "The element has to have a name.", e => !string.IsNullOrWhiteSpace(e?.Name)));
        }

        [JsonProperty(PropertyName = "IdeaId", Required = Required.Always)]
        public string Id
        {
            get; 
            set;
        }

        public bool Save
        {
            get => _save;
            set => SetProperty(ref _save, value);
        }

        public string Name
        {
            get => _name; 
            set => SetProperty(ref _name, value);
        }
        public string SectionName
        {
            get => _sectionName; 
            set => SetProperty(ref _sectionName, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public DateTime RolledDate
        {
            get => _rolledDate;
            set => SetProperty(ref _rolledDate, value);
        }

        public List<IdeaCategory> IdeaCategories
        {
            get => _ideaCategories; 
            set => SetProperty(ref _ideaCategories, value);
        }
    }
}
