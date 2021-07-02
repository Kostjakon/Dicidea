using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dicidea.Core.Models;
using Newtonsoft.Json;
using Prism.Services.Dialogs;

namespace Dicidea.Core.Services
{
    public class IdeaDataServiceJson : IIdeaDataService
    {

        private readonly List<Idea> _allIdeas;
        private readonly IDialogService _dialogService;

        public IdeaDataServiceJson(IDialogService dialogService)
        {
            _dialogService = dialogService;
            _allIdeas = LoadIdeasAsync().Result;
            //SaveDiceAsync(_allDice);
        }

        private string FileName => Path.Combine(FolderName, "idea.json");

        private string FolderName =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dicidea");

        public virtual async Task<List<Idea>> GetAllIdeasAsync()
        {
            await Task.CompletedTask;
            return _allIdeas;
        }

        public virtual async Task AddIdeaAsync(Idea idea)
        {
            await Task.CompletedTask;
            _allIdeas.Add(idea);
        }
        public virtual async Task AddIdeasAsync(List<Idea> ideas)
        {
            await Task.CompletedTask;
            IEnumerable<Idea> tmp = ideas.Where(i => i.Save);
            foreach (Idea idea in tmp)
            {
                _allIdeas.Add(idea);
            }
        }

        public virtual async Task DeleteIdeaAsync(Idea idea)
        {
            await Task.CompletedTask;
            _allIdeas.Remove(idea);
        }

        public virtual async Task SaveIdeasAsync()
        {
            await Task.CompletedTask;
            await SaveIdeaAsync(_allIdeas.Where(i => i.Save));
        }

        public virtual async Task<Idea> GetLastRolledIdeaAsync()
        {
            await Task.CompletedTask;
            return _allIdeas.OrderByDescending(i => i.RolledDate).First();
        }

        public virtual async Task DeleteIdeaCategoryAsync(Idea idea, IdeaCategory ideaCategory)
        {
            await Task.CompletedTask;
            idea.IdeaCategories.Remove(ideaCategory);
        }

        public virtual async Task DeleteIdeaElementAsync(IdeaCategory ideaCategory, IdeaElement ideaElement)
        {
            await Task.CompletedTask;
            ideaCategory.IdeaElements.Remove(ideaElement);
        }

        public virtual async Task DeleteIdeaValueAsync(IdeaElement ideaElement, IdeaValue ideaValue)
        {
            await Task.CompletedTask;
            ideaElement.IdeaValues.Remove(ideaValue);
        }

        private async Task<List<Idea>> LoadIdeasAsync()
        {
            await Task.Delay(0);

            try
            {

                if (!File.Exists(FileName))
                {
                    Debug.WriteLine("Create new Ideas List");
                    return new List<Idea>();
                }

                string data = File.ReadAllText(FileName);
                List<Idea> allIdeas = JsonConvert.DeserializeObject<List<Idea>>(data);
                return allIdeas;
            }
            catch (Exception e)
            {
                _dialogService.ShowDialog("ErrorDialog",
                    new DialogParameters
                    {
                        { "title", "Error" },
                        { "message", $"Loading of file '{FileName}' failed!\nError: '{e.Message}" }
                    },
                    r =>
                    {
                        if (r.Result == ButtonResult.None) return;
                        if (r.Result == ButtonResult.OK) return;
                        if (r.Result == ButtonResult.Cancel) { }
                    });
                throw;
            }
        }
        private async Task SaveIdeaAsync(IEnumerable<Idea> ideas)
        {
            foreach (Idea idea in ideas)
            {
                Debug.WriteLine(idea.Name);
            }
            await Task.Delay(0);
            try
            {
                if (!Directory.Exists(FolderName))
                {
                    Directory.CreateDirectory(FolderName);
                }
                string data = JsonConvert.SerializeObject(ideas);
                File.WriteAllText(FileName, data);
            }
            catch (Exception e)
            {
                _dialogService.ShowDialog("ErrorDialog",
                    new DialogParameters
                    {
                        { "title", "Error" },
                        { "message", $"Saving in path '{FolderName}' failed!\nError: '{e.Message}" }
                    },
                    r =>
                    {
                        if (r.Result == ButtonResult.None) return;
                        if (r.Result == ButtonResult.OK) return;
                        if (r.Result == ButtonResult.Cancel) { }
                    });
                throw;
            }
        }
    }
}
