﻿using System.Collections.Generic;
using System.Linq;
using Dragablz;
using XFiler.SDK;

namespace XFiler
{
    public class TabsFactory : ITabsFactory
    {
        private readonly IInterTabClient _tabClient;
        private readonly ITabFactory _tabFactory;
        private readonly IWindowFactory _windowFactory;
        private readonly IBookmarksManager _bookmarksManager;
        private readonly ISettingsTabFactory _settingsFactory;

        public TabsFactory(IInterTabClient tabClient, ITabFactory tabFactory,
            IWindowFactory windowFactory, IBookmarksManager bookmarksManager, ISettingsTabFactory settingsFactory)
        {
            _tabClient = tabClient;
            _tabFactory = tabFactory;
            _windowFactory = windowFactory;
            _bookmarksManager = bookmarksManager;
            _settingsFactory = settingsFactory;
        }

        public ITabsViewModel CreateTabsViewModel(IEnumerable<ITabItemModel> initItems)
            => new TabsViewModel(_tabClient, _tabFactory, _windowFactory,
                _bookmarksManager, initItems, _settingsFactory);

        public ITabsViewModel CreateTabsViewModel()
            => new TabsViewModel(_tabClient, _tabFactory, _windowFactory, _bookmarksManager,
                Enumerable.Empty<ITabItemModel>(), _settingsFactory);
    }
}