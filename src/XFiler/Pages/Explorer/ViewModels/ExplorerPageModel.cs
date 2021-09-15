﻿using Prism.Commands;
using System.ComponentModel;
using System.IO;
using System.Linq;
using XFiler.Commands;

namespace XFiler
{
    public class ExplorerPageModel : BasePageModel, IExplorerPageModel
    {
        #region Private Fields

        private DirectoryInfo _directory;

        #endregion

        #region Public Properties

        public IReadOnlyList<IFilesPresenterFactory> FilesPresenters { get; private set; }
        public IFilesPresenterFactory CurrentPresenter { get; set; }

        #endregion

        public DelegateCommand<object> PasteCommand { get; private set; }
        public DelegateCommand<IFileSystemModel> CreateFolderCommand { get; private set; }
        public DelegateCommand<IFileSystemModel> CreateTextCommand { get; private set; }
        public DelegateCommand<IFileSystemModel> OpenInNativeExplorerCommand { get; private set; }

        #region Constructor

        public ExplorerPageModel(
            IReadOnlyList<IFilesPresenterFactory> filesPresenters,
            IClipboardService clipboardService,
            IMainCommands mainCommands,
            DirectoryInfo directory) : base(typeof(ExplorerPage), new XFilerRoute(directory))
        {
            _directory = directory;

            FilesPresenters = filesPresenters;
            PasteCommand = clipboardService.PasteCommand;

            CreateFolderCommand = mainCommands.CreateFolderCommand;
            CreateTextCommand = mainCommands.CreateTextCommand;
            OpenInNativeExplorerCommand = mainCommands.OpenInNativeExplorerCommand;

            PropertyChanged += DirectoryTabItemViewModelOnPropertyChanged;

            foreach (var factory in filesPresenters)
                factory.DirectoryOrFileOpened += FilePresenterOnDirectoryOrFileOpened;

            CurrentPresenter = FilesPresenters.First();
        }

        #endregion

        #region Public Methods

        public override void Dispose()
        {
            base.Dispose();

            PropertyChanged -= DirectoryTabItemViewModelOnPropertyChanged;

            foreach (var factory in FilesPresenters)
            {
                factory.DirectoryOrFileOpened -= FilePresenterOnDirectoryOrFileOpened;

                factory.Dispose();
            }

            _directory = null!;
            FilesPresenters = null!;
            CurrentPresenter = null!;

            PasteCommand = null!;
            CreateFolderCommand = null!;
            CreateTextCommand = null!;
            OpenInNativeExplorerCommand = null!;
        }

        #endregion

        #region Private Methods

        private void OpenDirectory()
        {
            CurrentPresenter.UpdatePresenter(_directory);
        }

        private void FilePresenterOnDirectoryOrFileOpened(object? sender, OpenDirectoryEventArgs e)
        {
            XFilerRoute route = e.FileEntityViewModel switch
            {
                DirectoryViewModel directoryViewModel => new XFilerRoute(directoryViewModel.DirectoryInfo),
                FileViewModel fileViewModel => new XFilerRoute(fileViewModel.FileInfo),
                _ => SpecialRoutes.MyComputer
            };

            GoTo(route);
        }
        
        private void DirectoryTabItemViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            PropertyChanged -= DirectoryTabItemViewModelOnPropertyChanged;

            switch (e.PropertyName)
            {
                case nameof(CurrentPresenter):

                    OpenDirectory();

                    break;
            }

            PropertyChanged += DirectoryTabItemViewModelOnPropertyChanged;
        }

        #endregion
    }
}