namespace XFiler.SDK
{
    public static class SpecialUrls
    {
        public static XFilerRoute MyComputer = new("��� ���������", "xfiler://mycomputer", RouteType.Special);

        public static XFilerRoute Settings = new("���������", "xfiler://settings", RouteType.Special);


        public static XFilerRoute? GetSpecialUrl(string fullName)
        {
            if (MyComputer.FullName == fullName)
                return MyComputer;
            if (Settings.FullName == fullName)
                return Settings;

            return null;
        }
    }
}