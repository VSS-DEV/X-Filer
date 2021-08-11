﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Dragablz;
using Prism.Commands;
using XFiler.SDK;

namespace XFiler
{
    public class TabsViewModel : BaseViewModel, ITabsViewModel
    {
        #region Private Fields

        private readonly ITabFactory _tabFactory;
        private readonly IWindowFactory _windowFactory;

        #endregion

        #region Public Properties

        public IInterTabClient InterTabClient { get; }

        public ObservableCollection<ITabItemModel> TabItems { get; }

        public ItemActionCallback ClosingTabItemHandler { get; }

        public ITabItemModel? CurrentTabItem { get; set; }

        public IReadOnlyCollection<IMenuItemViewModel> Bookmarks { get; }

        public Func<ITabItemModel> Factory { get; }

        #endregion

        #region Commands

        public DelegateCommand<object> CreateNewTabItemCommand { get; }
        public DelegateCommand<object> OpenTabItemInNewWindowCommand { get; }
        public DelegateCommand<object> DuplicateTabCommand { get; }
        public DelegateCommand<object> CloseOtherTabsCommand { get; }
        public DelegateCommand CreateSettingsTabCommand { get; }
        public DelegateCommand CloseAllTabsCommand { get; }

        #endregion

        #region Constructor

        public TabsViewModel(IInterTabClient tabClient,
            ITabFactory tabFactory,
            IWindowFactory windowFactory,
            IBookmarksManager bookmarksManager,
            IEnumerable<ITabItemModel> init)
        {
            _tabFactory = tabFactory;
            _windowFactory = windowFactory;

            InterTabClient = tabClient;
            Bookmarks = bookmarksManager.Bookmarks;
            ClosingTabItemHandler = ClosingTabItemHandlerImpl;
            CreateNewTabItemCommand = new DelegateCommand<object>(OnCreateNewTabItem);
            OpenTabItemInNewWindowCommand =
                new DelegateCommand<object>(OnOpenTabItemInNewWindow, OnCanOpenTabItemInNewWindow);
            DuplicateTabCommand = new DelegateCommand<object>(OnDuplicate);
            CloseOtherTabsCommand = new DelegateCommand<object>(OnCloseOtherTabs, CanCloseAllTabs);

            CreateSettingsTabCommand = new DelegateCommand(OnOpenSettings);
            CloseAllTabsCommand = new DelegateCommand(OnCloseAllTabs);


            TabItems = new ObservableCollection<ITabItemModel>(init);

            Factory = CreateTabVm;

            TabItems.CollectionChanged += TabItemsOnCollectionChanged;
        }

        #endregion

        #region Public Methods

        public void OnOpenNewTab(FileEntityViewModel fileEntityViewModel, bool isSelectNewTab = false)
        {
            if (fileEntityViewModel is DirectoryViewModel directoryViewModel)
            {
                var tab = _tabFactory.CreateExplorerTab(directoryViewModel.DirectoryInfo);
                TabItems.Add(tab);

                if (isSelectNewTab)
                    CurrentTabItem = tab;
            }
        }

        #endregion

        #region Private Methods

        private void OnCreateNewTabItem(object? obj)
        {
            TabItems.Add(_tabFactory.CreateMyComputerTab());
        }

        private bool OnCanOpenTabItemInNewWindow(object? obj) => TabItems.Count > 1;

        private void OnOpenTabItemInNewWindow(object? obj)
        {
            if (obj is not ITabItemModel directoryTabItem)
                return;

            TabItems.Remove(directoryTabItem);

            _windowFactory.OpenTabInNewWindow(directoryTabItem);
        }

        private void OnDuplicate(object? obj)
        {
            if (obj is not ITabItemModel directoryTabItem)
                return;

            TabItems.Add(_tabFactory
                .CreateTab(directoryTabItem.Route));
        }

        private bool CanCloseAllTabs(object? obj) => TabItems.Count > 1;

        private void OnCloseOtherTabs(object? obj)
        {
            if (obj is not ITabItemModel tabItem)
                return;

            var removedItems = TabItems.Where(i => i != tabItem).ToList();

            foreach (var item in removedItems)
                TabablzControl.CloseItem(item);
        }

        private void OnCloseAllTabs()
        {
            var removedItems = TabItems.ToList();

            foreach (var item in removedItems)
                TabablzControl.CloseItem(item);
        }

        private void OnOpenSettings()
        {
            var tab = _tabFactory.CreateTab(SpecialRoutes.Settings);
            TabItems.Add(tab);
            CurrentTabItem = tab;
        }

        private ITabItemModel CreateTabVm() => _tabFactory.CreateMyComputerTab();

        private void TabItemsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OpenTabItemInNewWindowCommand.RaiseCanExecuteChanged();
            CloseOtherTabsCommand.RaiseCanExecuteChanged();
        }

        private void ClosingTabItemHandlerImpl(ItemActionCallbackArgs<TabablzControl> args)
        {
            //in here you can dispose stuff or cancel the close

            //here's your view model:
            var viewModel = args.DragablzItem.DataContext as ITabItemModel;
            viewModel?.Dispose();
            //here's how you can cancel stuff:
            //args.Cancel(); 
        }

        #endregion
    }
}