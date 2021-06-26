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
        private RollEmSpaceListViewModel _rollEmSpaceListViewModel;
        private ListCollectionView _groupedIdeaView;
        //private readonly object _lock = new object();
        public RollEmSpaceViewModel(RollEmSpaceListViewModel rollEmSpaceListViewModel, Idea idea, IIdeaDataService ideaDataService)
        {
            _rollEmSpaceListViewModel = rollEmSpaceListViewModel;
            //SendMailCommand = new DelegateCommand(SendMailCommand, CanSendMailExecute);
            if (GroupedIdeaView != null)
            {
                Debug.WriteLine("Binding of GroupedCategoriesView in DiceViewModel");
                //System.Windows.Data.BindingOperations.EnableCollectionSynchronization(GroupedCategoriesView, _lock);
            }
            Idea = idea;

            RollEmSpaceViewModel self = this;
            CreateGroupedView();
            RollCommand = new DelegateCommand(RollExecute);
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
        public DelegateCommand RollCommand { get; set; }
        public DelegateCommand EditCommand { get; set; }
        private async void RollExecute()
        {
            Debug.WriteLine("Add Idea");
            // TODO: Roll Ideas

            // Hier Schleife mit selectedDice

            await _ideaListViewModel.AddIdeaAsync();
            GroupedIdeaView.Refresh();

        }

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
