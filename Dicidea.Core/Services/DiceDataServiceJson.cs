using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dicidea.Core.Models;
using Dicidea.Core.SampleDice;
using Newtonsoft.Json;
using Prism.Services.Dialogs;

namespace Dicidea.Core.Services
{
    /// <summary>
    /// Simpler Service zum Verwalten, Laden und Speichern von Würfeln in einer Json-Datei.
    /// </summary>
    public class DiceDataServiceJson : IDiceDataService
    {
        private readonly List<Dice> _allDice;
        private readonly IDialogService _dialogService;
        public DiceDataServiceJson(IDialogService dialogService)
        {
            _dialogService = dialogService;
            _allDice = LoadDiceAsync().Result;
        }
        private string FileName => Path.Combine(FolderName, "dice.json");
        private string FolderName => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dicidea");

        /// <summary>
        /// Funktion um die Liste der Würfel zu bekommen.
        /// </summary>
        /// <returns>Liste der Würfel</returns>
        public virtual async Task<List<Dice>> GetAllDiceAsync()
        {
            await Task.CompletedTask;
            return _allDice;
        }

        /// <summary>
        /// Funktion um der Würfel Liste einen Würfel hinzuzufügen
        /// </summary>
        /// <param name="dice">Würfel der hinzugefügt werden soll</param>
        /// <returns></returns>
        public virtual async Task AddDiceAsync(Dice dice)
        {
            await Task.CompletedTask;
            _allDice.Add(dice);
        }

        /// <summary>
        /// Funktion zum Löschen eines Würfels
        /// </summary>
        /// <param name="dice">Würfel der gelöscht werden soll</param>
        /// <returns></returns>
        public virtual async Task DeleteDiceAsync(Dice dice)
        {
            await Task.CompletedTask;
            _allDice.Remove(dice);
        }

        /// <summary>
        /// Funktion um den Würfel zu erhalten mit dem zuletzt gewürfelt oder der zuletzt hinzugefügt worden ist zu erhalten, gibt es mehrere mit dem gleichen Datum wird der erste davon zurückgegeben
        /// </summary>
        /// <returns>Zuletzt erstellter/zuletzt verwendeter Würfel</returns>
        public virtual async Task<Dice> GetLastRolledDiceAsync()
        {
            await Task.CompletedTask;
            if (_allDice != null && _allDice.Count > 0) return _allDice.OrderByDescending(d => d.LastUsed).First();
            return null;
        }

        /// <summary>
        /// Funktion zum Speichern der Würfel.
        /// </summary>
        /// <returns></returns>
        public virtual async Task SaveRolledDiceAsync()
        {
            await Task.CompletedTask;
            await SaveDiceAsync(GetDiceToSave());
        }

        /// <summary>
        /// Funktion die überprüft welche Würfel Fehler haben.
        /// </summary>
        /// <returns>Liste mit Würfeln ohne Fehler</returns>
        private List<Dice> GetDiceToSave()
        {

            List<Dice> diceToSave = new List<Dice>();
            foreach (Dice dice in _allDice)
            {
                if (!dice.HasErrors)
                {
                    bool error = false;
                    foreach (Category category in dice.Categories)
                    {
                        if (!category.HasErrors)
                        {
                            foreach (Element element in category.Elements)
                            {
                                if (!element.HasErrors)
                                {
                                    foreach (Value value in element.Values)
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
                        diceToSave.Add(dice);
                    }
                }
            }

            return diceToSave;
        }

        /// <summary>
        /// Funktion zum Speichern der Würfelliste als Json-Datei
        /// </summary>
        /// <returns></returns>
        public virtual async Task SaveDiceAsync()
        {
            await Task.CompletedTask;

            List<Dice> diceToSave = GetDiceToSave();

            await SaveDiceAsync(diceToSave);

            _dialogService.ShowDialog("SavedDiceDialog",
                new DialogParameters
                {
                    { "title", "Saved dice" },
                    { "allDice", diceToSave }
                },
                r =>
                {
                    if (r.Result == ButtonResult.None) return;
                    if (r.Result == ButtonResult.OK) return;
                    if (r.Result == ButtonResult.Cancel) { }
                });
        }

        /// <summary>
        /// Funktion zum Hinzufügen einer Kategorie zu einem Würfel
        /// </summary>
        /// <param name="dice">Würfel zu dem die Kategorie hinzugefügt werden soll</param>
        /// <param name="category">Kategorie die hinzugefügt werden soll</param>
        /// <returns></returns>
        public virtual async Task AddCategoryAsync(Dice dice, Category category)
        {
            await Task.CompletedTask;
            dice.Categories.Add(category);
        }

        /// <summary>
        /// Funktion zum Entfernen einer Kategorie aus einem Würfel
        /// </summary>
        /// <param name="dice">Würfel aus dem die Kategorie entfernt werden soll</param>
        /// <param name="category">Kategorie die entfernt werden soll</param>
        /// <returns></returns>
        public virtual async Task DeleteCategoryAsync(Dice dice, Category category)
        {
            await Task.CompletedTask;
            dice.Categories.Remove(category);
        }

        /// <summary>
        /// Funktion zum Hinzufügen eines Elements zu einer Kategorie
        /// </summary>
        /// <param name="category"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public virtual async Task AddElementAsync(Category category, Element element)
        {
            await Task.CompletedTask;
            category.Elements.Add(element);
        }

        /// <summary>
        /// Funktion zum Entfernen eines Elements von einer Kategorie
        /// </summary>
        /// <param name="category">Kategorie aus der das Element entfernt werden soll</param>
        /// <param name="element">Element das entfernt werden soll</param>
        /// <returns></returns>
        public virtual async Task DeleteElementAsync(Category category, Element element)
        {
            await Task.CompletedTask;
            category.Elements.Remove(element);
        }

        /// <summary>
        /// Funktion zum hinzufügen eines Wertes zu einem Element
        /// </summary>
        /// <param name="element">Element zu dem der Wert hinzugefügt werden soll</param>
        /// <param name="value">Wert der hinzugefügt werden soll</param>
        /// <returns></returns>
        public virtual async Task AddValueAsync(Element element, Value value)
        {
            await Task.CompletedTask;
            element.Values.Add(value);
        }

        /// <summary>
        /// Funktion zum Entfernen eines Wertes aus einem Element
        /// </summary>
        /// <param name="element">Element aus dem der Wert entfernt werden soll</param>
        /// <param name="value">Wert der entfernt werden soll</param>
        /// <returns></returns>
        public virtual async Task DeleteValueAsync(Element element, Value value)
        {
            await Task.CompletedTask;
            element.Values.Remove(value);
        }

        /// <summary>
        /// Funktion zum Laden der Würfel aus einer Json-Datei. Gibt es noch keine Datei mit Würfeln oder ist die Liste leer werden die Beispielwürfel geladen.
        /// </summary>
        /// <returns></returns>
        private async Task<List<Dice>> LoadDiceAsync()
        {
            await Task.Delay(0);

            try
            {

                if (!File.Exists(FileName)) return LoadSampleDice();
                string data = File.ReadAllText(FileName);
                List<Dice> allDice = JsonConvert.DeserializeObject<List<Dice>>(data);
                return allDice is {Count: > 0} ? allDice : LoadSampleDice();
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

        /// <summary>
        /// Funktion zum Laden aller Beispiel Würfel
        /// </summary>
        /// <returns>Liste aller Beispiel Würfel</returns>
        private List<Dice> LoadSampleDice()
        {
            Dice sims = SimsDice.GetSimsDice();
            List<Dice> tmp = new List<Dice>()
            {
                sims
            };
            return tmp;
        }

        /// <summary>
        /// Funktion zum speichern der übergebenen Würfelliste. Hier wird keine Überprüfung mehr auf Fehler in den Würfeln gemacht.
        /// </summary>
        /// <param name="dice">Liste der Würfel die gespeichert werden soll</param>
        /// <returns></returns>
        private async Task SaveDiceAsync(List<Dice> dice)
        {
            await Task.Delay(0);
            try
            {
                if (!Directory.Exists(FolderName))Directory.CreateDirectory(FolderName);
                string data = JsonConvert.SerializeObject(dice);
                File.WriteAllText(FileName, data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
