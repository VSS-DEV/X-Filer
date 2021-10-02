﻿using System.ComponentModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace XFiler;

public sealed class ExplorerPageModel : BasePageModel, IExplorerPageModel
{
    #region Private Fields

    private IDirectorySettings _directorySettings;
    private IReactiveOptions _reactiveOptions;
    private IStorage _storage;
    private DirectoryInfo _directory = null!;

    #endregion

    #region Public Properties

    public IReadOnlyList<IFilesPresenterFactory> FilesPresenters { get; private set; }
    public IFilesPresenterFactory CurrentPresenter { get; set; }

    public IReadOnlyList<IFilesGroup> FilesGroups { get; private set; }
    public IFilesGroup CurrentGroup { get; set; }

    public ImageSource? BackgroundImage { get; private set; }

    #endregion

    #region Commands

    public DelegateCommand<object> PasteCommand { get; private set; }
    public DelegateCommand<IFileSystemModel> CreateFolderCommand { get; private set; }
    public DelegateCommand<IFileSystemModel> CreateTextCommand { get; private set; }
    public DelegateCommand<IFileSystemModel> OpenInNativeExplorerCommand { get; private set; }

    #endregion

    #region Constructor

    public ExplorerPageModel(
        IReadOnlyList<IFilesPresenterFactory> filesPresenters,
        IReadOnlyList<IFilesGroup> groups,
        IClipboardService clipboardService,
        IMainCommands mainCommands,
        IDirectorySettings directorySettings,
        IReactiveOptions reactiveOptions,
        IStorage storage)
    {
        _directorySettings = directorySettings;
        _reactiveOptions = reactiveOptions;
        _storage = storage;
        
        FilesPresenters = filesPresenters;
        FilesGroups = groups;
        PasteCommand = clipboardService.PasteCommand;

        CreateFolderCommand = mainCommands.CreateFolderCommand;
        CreateTextCommand = mainCommands.CreateTextCommand;
        OpenInNativeExplorerCommand = mainCommands.OpenInNativeExplorerCommand;
    }

    #endregion

    #region Public Methods

    public void Init(DirectoryInfo directory)
    {
        Init(typeof(ExplorerPage), new DirectoryRoute(directory));

        _directory = directory;

        var dirSettings = _directorySettings.GetSettings(directory.FullName);

        CurrentGroup = FilesGroups.FirstOrDefault(g => g.Id == dirSettings.GroupId) ??
                       FilesGroups.First();

        PropertyChanged += DirectoryTabItemViewModelOnPropertyChanged;

        foreach (var factory in FilesPresenters)
            factory.DirectoryOrFileOpened += FilePresenterOnDirectoryOrFileOpened;

        CurrentPresenter = SelectInitPresenter(dirSettings, _reactiveOptions);

        BackgroundImage = CreateImageSource(_reactiveOptions.ExplorerBackgroundImagePath);

        _reactiveOptions.PropertyChanged += ReactiveOptionsOnPropertyChanged;
    }

    public override void Dispose()
    {
        base.Dispose();

        PropertyChanged -= DirectoryTabItemViewModelOnPropertyChanged;

        _reactiveOptions.PropertyChanged -= ReactiveOptionsOnPropertyChanged;

        foreach (var factory in FilesPresenters)
        {
            factory.DirectoryOrFileOpened -= FilePresenterOnDirectoryOrFileOpened;

            factory.Dispose();
        }

        _directory = null!;
        _directorySettings = null!;
        _storage = null!;
        _reactiveOptions = null!;
        FilesPresenters = null!;
        CurrentPresenter = null!;
        FilesGroups = null!;
        CurrentGroup = null!;

        PasteCommand = null!;
        CreateFolderCommand = null!;
        CreateTextCommand = null!;
        OpenInNativeExplorerCommand = null!;
    }

    #endregion

    #region Private Methods

    private void OpenDirectory() => CurrentPresenter.UpdatePresenter(_directory, CurrentGroup);

    private void FilePresenterOnDirectoryOrFileOpened(object? sender, OpenDirectoryEventArgs e)
    {
        var route = e.FileEntityViewModel switch
        {
            DirectoryViewModel directoryViewModel => new DirectoryRoute(directoryViewModel.DirectoryInfo),
            FileViewModel fileViewModel => new FileRoute(fileViewModel.FileInfo),
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
            case nameof(CurrentGroup):
                OpenDirectory();
                _directorySettings.SetSettings(_directory.FullName,
                    new DirectorySettingsInfo(CurrentGroup.Id, CurrentPresenter.Id));
                break;
        }

        PropertyChanged += DirectoryTabItemViewModelOnPropertyChanged;
    }

    private IFilesPresenterFactory SelectInitPresenter(DirectorySettingsInfo dirSettings,
        IReactiveOptions options)
    {
        var presenterId = options.DefaultPresenterId;

        if (!options.AlwaysOpenDirectoryInDefaultPresenter)
            presenterId = dirSettings.PresenterId ?? presenterId;

        return FilesPresenters.FirstOrDefault(p => p.Id == presenterId) ??
               FilesPresenters.First();
    }

    private void ReactiveOptionsOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IReactiveOptions.ExplorerBackgroundImagePath))
            BackgroundImage = CreateImageSource(_reactiveOptions.ExplorerBackgroundImagePath);
    }

    private ImageSource? CreateImageSource(string? imagePath)
    {
        if (imagePath != null)
        {
            var fullPath = Path.Combine(_storage.ExplorerWallpapersDirectory, imagePath);

            if (File.Exists(fullPath))
                return new BitmapImage(new Uri(fullPath));
        }

        return null;
    }

    #endregion
}