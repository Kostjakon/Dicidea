using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dicidea.Core.Models;
using Newtonsoft.Json;
using Prism.Services.Dialogs;

namespace Dicidea.Core.Services
{
    /// <summary>
    ///  Simpler Service zum Verwalten, Laden und Speichern von Ideen in einer Json-Datei.
    /// </summary>
    public class IdeaDataServiceJson : IIdeaDataService
    {

        private readonly List<Idea> _allIdeas;
        private readonly IDialogService _dialogService;

        public IdeaDataServiceJson(IDialogService dialogService)
        {
            _dialogService = dialogService;
            _allIdeas = LoadIdeasAsync().Result;
        }
        private string FileName => Path.Combine(FolderName, "idea.json");
        private string FolderName => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dicidea");

        /// <summary>
        /// Funktion um die Liste der Ideen zu bekommen.
        /// </summary>
        /// <returns>Liste der Ideen</returns>
        public virtual async Task<List<Idea>> GetAllIdeasAsync()
        {
            await Task.CompletedTask;
            return _allIdeas;
        }

        /// <summary>
        /// Funktion um der Ideen Liste eine Liste von Ideen hinzuzufügen
        /// </summary>
        /// <param name="ideas">Die Liste von Ideen die hinzugefügt werden soll</param>
        /// <returns></returns>
        public virtual async Task AddIdeasAsync(List<Idea> ideas)
        {
            await Task.CompletedTask;
            IEnumerable<Idea> tmp = ideas.Where(i => i.Save);
            foreach (Idea idea in tmp)
            {
                _allIdeas.Add(idea);
            }
        }

        /// <summary>
        /// Funktion zum löschen einer Idee
        /// </summary>
        /// <param name="idea">Idee die gelöscht werden soll</param>
        /// <returns></returns>
        public virtual async Task DeleteIdeaAsync(Idea idea)
        {
            await Task.CompletedTask;
            _allIdeas.Remove(idea);
        }

        /// <summary>
        /// Funktion zum Speichern der Ideen. Hier wird überprüft welche Ideen Fehler haben und nur die ohne Fehler zum Speichern weitergegeben
        /// </summary>
        /// <returns></returns>
        public virtual async Task SaveIdeasAsync()
        {
            await Task.CompletedTask;
            List<Idea> ideasToSave = new List<Idea>();

            // Überprüfung welche Ideen Error haben
            // Nur Ideen die in keiner Tiefe Error haben werden der Liste an zu speichernden Ideen hinzugefügt
            foreach (Idea idea in _allIdeas)
            {
                if (!idea.HasErrors)
                {
                    bool error = false;
                    foreach (IdeaCategory category in idea.IdeaCategories)
                    {
                        if (!category.HasErrors)
                        {
                            foreach (IdeaElement element in category.IdeaElements)
                            {
                                if (!element.HasErrors)
                                {
                                    foreach (IdeaValue value in element.IdeaValues)
                                    {
                                        if (value.HasErrors)
                                        {
                                            error = true;
                                        }
                                    }
                                }
                                else
                                {
                                    error = true;
                                }
                            }
                        }
                        else
                        {
                            error = true;
                        }
                    }

                    if (!error)
                    {
                        ideasToSave.Add(idea);
                    }
                }
            }
            await SaveIdeaAsync(ideasToSave);
        }

        /// <summary>
        /// Funktion um die letzte gerollte Idee zu erhalten, gibt es mehrere mit dem gleichen Datum wird die erste davon zurückgegeben
        /// </summary>
        /// <returns>Idea die zuletzt gerollt wurde</returns>
        public virtual async Task<Idea> GetLastRolledIdeaAsync()
        {
            await Task.CompletedTask;
            if(_allIdeas != null && _allIdeas.Count > 0) return _allIdeas.OrderByDescending(i => i.RolledDate).First();
            return null;
        }

        /// <summary>
        /// Funktion zum löschen einer Kategorie aus einer Idee
        /// </summary>
        /// <param name="idea">Idee aus der die Kategorie entfernt werden soll</param>
        /// <param name="ideaCategory">Kategorie die entfernt werden soll</param>
        /// <returns></returns>
        public virtual async Task DeleteIdeaCategoryAsync(Idea idea, IdeaCategory ideaCategory)
        {
            await Task.CompletedTask;
            idea.IdeaCategories.Remove(ideaCategory);
        }

        /// <summary>
        /// Funktion zum löschen eines Elements aus einer Kategorie
        /// </summary>
        /// <param name="ideaCategory">Kategorie aus der das Element entfernt werden soll</param>
        /// <param name="ideaElement">Element das entfernt werden soll</param>
        /// <returns></returns>
        public virtual async Task DeleteIdeaElementAsync(IdeaCategory ideaCategory, IdeaElement ideaElement)
        {
            await Task.CompletedTask;
            ideaCategory.IdeaElements.Remove(ideaElement);
        }

        /// <summary>
        /// Funktion zum löschen eines Wertes aus einem Element.
        /// </summary>
        /// <param name="ideaElement">Element aus dem der Wert entfernt werden soll</param>
        /// <param name="ideaValue">Wert der entfernt werden soll</param>
        /// <returns></returns>
        public virtual async Task DeleteIdeaValueAsync(IdeaElement ideaElement, IdeaValue ideaValue)
        {
            await Task.CompletedTask;
            ideaElement.IdeaValues.Remove(ideaValue);
        }

        /// <summary>
        /// Lädt die Ideen. Falls keine Datei existiert wird eine neue leere Liste erstellt und zurückgegeben.
        /// </summary>
        /// <returns>Liste mit Ideen</returns>
        private async Task<List<Idea>> LoadIdeasAsync()
        {
            await Task.Delay(0);

            try
            {

                if (!File.Exists(FileName))return new List<Idea>();
                string data = File.ReadAllText(FileName);
                List<Idea> allIdeas = JsonConvert.DeserializeObject<List<Idea>>(data);
                return allIdeas;
            }
            catch (Exception e)
            {
                // Wenn das Laden der Datei aus irgendwelchen Gründen nicht funktioniert wird ein Dialog mit dem Fehler angezeigt
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
                return null;
            }
        }

        /// <summary>
        /// Speichert die Liste der übergebenen Ideen als Json-Datei, hier wird keine Überprüfung auf Fehler innerhalb der Ideen mehr gemacht!
        /// </summary>
        /// <param name="ideas">Eine Liste aller Ideen die gespeichert werden soll</param>
        /// <returns></returns>
        private async Task SaveIdeaAsync(List<Idea> ideas)
        {
            await Task.Delay(0);
            try
            {
                if (!Directory.Exists(FolderName))Directory.CreateDirectory(FolderName);
                string data = JsonConvert.SerializeObject(ideas);
                File.WriteAllText(FileName, data);
            }
            catch (Exception e)
            {
                // Wenn das Speichern der Datei aus irgendwelchen Gründen nicht funktioniert wird ein Dialog mit dem Fehler angezeigt
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
