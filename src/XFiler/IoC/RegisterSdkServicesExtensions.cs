﻿using Autofac;
using XFiler.Commands;
using XFiler.Resize;

namespace XFiler
{
    internal static class RegisterSdkServicesExtensions
    {
        public static void RegisterSdkServices(this ContainerBuilder services)
        {
            services.RegisterType<Storage>().As<IStorage>().SingleInstance();

            services.RegisterBookmarksServices();

            services.RegisterType<MainCommands>().As<IMainCommands>().SingleInstance();

            services.RegisterType<ClipboardService>().As<IClipboardService>().SingleInstance();

            services.RegisterType<FileOperations>().As<IFileOperations>().SingleInstance();
           
            services.RegisterType<FileTypeResolver>().As<IFileTypeResolver>().SingleInstance();

            services.RegisterIconServices();

            services.RegisterType<RenameService>().As<IRenameService>().SingleInstance();

            services.RegisterType<ExplorerOptions>().As<IExplorerOptions>().SingleInstance();

            services.RegisterType<DirectorySettings>().As<IDirectorySettings>().SingleInstance();
        }

        private static void RegisterBookmarksServices(this ContainerBuilder services)
        {
            services.RegisterType<MenuItemFactory>().As<IMenuItemFactory>().SingleInstance();
            services.RegisterType<BookmarksManager>().As<IBookmarksManager>().SingleInstance();
        }

        private static void RegisterIconServices(this ContainerBuilder services)
        {
            services.RegisterType<FastResizeImageService>().As<IResizeImageService>().SingleInstance();
            //services.RegisterType<SixLaborsResizeImageService>().As<IResizeImageService>().SingleInstance();

            // Image icon pipeline:
            services.RegisterType<NativeExeIconProvider>().As<IIconProvider>().SingleInstance();
            services.RegisterType<IconProviderForImages>().As<IIconProvider>().SingleInstance();
            services.RegisterType<BaseIconProvider>().As<IIconProvider>().SingleInstance();
            services.RegisterType<NativeFileIconProvider>().As<IIconProvider>().SingleInstance();
            services.RegisterType<BlankIconProvider>().As<IIconProvider>().SingleInstance();

            services.RegisterType<IconLoader>().As<IIconLoader>().SingleInstance();
        }
    }
}