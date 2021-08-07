namespace XFiler.SDK
{
    public static class SpecialUrls
    {
        public static XFilerUrl MyComputer = new("��� ���������", "xfiler://mycomputer");

        public static XFilerUrl Settings = new("���������", "xfiler://settings");


        public static XFilerUrl? GetSpecialUrl(string fullName)
        {
            if (MyComputer.FullName == fullName)
                return MyComputer;
            if (Settings.FullName == fullName)
                return Settings;    

            return null;
        }
    }
}