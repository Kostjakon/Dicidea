using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using DicePage.ViewModels;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Dicidea.Core.Services;
using IdeaPage.ViewModels;
using Prism.Commands;

namespace RollEmSpacePage.ViewModels
{
    class RollEmSpaceViewModel : NotifyPropertyChanges
    {
        private bool _isEditEnabled;
        private bool _isEditDisabled = true;
        private IdeaListViewModel _ideaListViewModel;
        private ListCollectionView _groupedIdeaView;

        private Dice _selectedDice;
        //private readonly object _lock = new object();
        public RollEmSpaceViewModel(Dice dice, Idea idea, IIdeaDataService ideaDataService)
        {
            _selectedDice = dice;
            //SendMailCommand = new DelegateCommand(SendMailCommand, CanSendMailExecute);
            if (GroupedIdeaView != null)
            {
                Debug.WriteLine("Binding of GroupedCategoriesView in DiceViewModel");
                //System.Windows.Data.BindingOperations.EnableCollectionSynchronization(GroupedCategoriesView, _lock);
            }
            Idea = idea;

            RollEmSpaceViewModel self = this;
            CreateGroupedView();
            AddCommand = new DelegateCommand(AddExecute);
            EditCommand = new DelegateCommand(EditExecute);
        }

        public bool IsEditEnabled
        {
            get => _isEditEnabled;
            set => SetProperty(ref _isEditEnabled, value);
        }
        public bool IsEditDisabled
        {
            get => _isEditDisabled;
            set => SetProperty(ref _isEditDisabled, value);
        }


        public ListCollectionView GroupedIdeaView
        {
            get => _groupedIdeaView;
            set => SetProperty(ref _groupedIdeaView, value);
        }
        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand EditCommand { get; set; }
        private async void AddExecute()
        {
            Debug.WriteLine("Add Idea");
            //await Application.Current.Dispatcher.BeginInvoke(() => Task.Run(_categoryListViewModel.AddCategoryAsync));
            /*
            Thread thread = new Thread(delegate()
            {
                AddCategory();
            });
            thread.IsBackground = true;
            thread.Start();
            */
            await _ideaListViewModel.AddIdeaAsync();
            //GroupedCategoriesView.Refresh();

        }

        /*
        public void AddCategory()
        {
            Dispatcher.BeginInvoke((Action) (async () =>
            {
                await Task.Run(_categoryListViewModel.AddCategoryAsync);
            }));
        }
        */

        public void EditExecute()
        {
            Debug.WriteLine("Edit Dice");
            IsEditEnabled = !IsEditEnabled;
            IsEditDisabled = !IsEditDisabled;

        }

        public IdeaViewModel SelectedIdea
        {
            get
            {
                if (GroupedIdeaView != null) return GroupedIdeaView.CurrentItem as IdeaViewModel;
                else return null;
            }
            set => GroupedIdeaView.MoveCurrentTo(value);
        }

        private void OnNext(string propertyName)
        {
            if (propertyName == nameof(Idea.Name))
            {
                //GroupedCategoriesView.Refresh(); <- Hier ist das Problem
            }
        }

        private void CreateGroupedView()
        {
            ObservableCollection<IdeaViewModel> ideaViewModels = _ideaListViewModel.AllIdeas;
            //foreach (var categoryViewModel in categoryViewModels)
            //{
            //    categoryViewModel.Category.WhenPropertyChanged.Subscribe(OnNext);
            //}

            var propertyName = "Idea.Name";
            GroupedIdeaView = new ListCollectionView(ideaViewModels)
            {
                IsLiveSorting = true,
                SortDescriptions = { new SortDescription(propertyName, ListSortDirection.Ascending) }
            };
            //GroupedCategoriesView.GroupDescriptions.Add(new PropertyGroupDescription
            //{
            //    PropertyName = propertyName,
            //    Converter = new NameToInitialConverter()
            //});

            GroupedIdeaView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedIdea));
        }

        public async Task AddIdeaAsync()
        {
            Debug.WriteLine("Add Idea");
            await _ideaListViewModel.AddIdeaAsync();
            //GroupedCategoriesView.Refresh();
        }
        public async Task DeleteIdeaAsync()
        {
            Debug.WriteLine("Delete Idea");
            await _ideaListViewModel.DeleteIdeaAsync(SelectedIdea);
            GroupedIdeaView.Refresh();
        }


        public Idea Idea { get; }
    }
}
