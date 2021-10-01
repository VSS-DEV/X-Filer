﻿using System.IO;

namespace XFiler;

internal sealed class Storage : IStorage
{
    #region Public Properties

    public string BaseDirectory { get; }

    public string ExplorerWallpapersDirectory { get; }

    public string LogDirectory { get; }

    public string ConfigDirectory { get; }

    public string DbDirectory { get; }

    public string Bookmarks { get; }    

    #endregion

    #region Constructors

    private Storage(string directory)
    {
        BaseDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), directory);
        Directory.CreateDirectory(BaseDirectory);

        ExplorerWallpapersDirectory = Path.Combine(BaseDirectory, "Explorer", "Wallpapers");
        Directory.CreateDirectory(ExplorerWallpapersDirectory);

        LogDirectory = Path.Combine(BaseDirectory, "Logs");
        Directory.CreateDirectory(LogDirectory);

        ConfigDirectory = Path.Combine(BaseDirectory, "Config");
        Directory.CreateDirectory(ConfigDirectory);

        DbDirectory = Path.Combine(BaseDirectory, "Data");
        Directory.CreateDirectory(DbDirectory);

        Bookmarks = Path.Combine(BaseDirectory, "bookmarks.json");
    }

    public Storage() : this("X-Filer")
    {
    }

    #endregion
}