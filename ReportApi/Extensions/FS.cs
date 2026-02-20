using System;

namespace ReportApi.Extensions
{
    public class FS
    {
        private static readonly Lazy<FS> instance = new Lazy<FS>(() => new FS());

        private bool IsContainer { get; set; }
        private string Dir { get; set; }

        private FS()
        {

        }
        public static FS Instance => instance.Value;

        public static string GetPath(string path)
        {
            bool FlagMustangON = false;

            if (FlagMustangON && Instance.IsContainer && path.Length > 2 && path[1] == ':')
                path = $"{Instance.Dir}\\{path.Substring(3).TrimStart('\\')}";

            return path;
        }

        public static FS Start(string dir, bool isContainer)
        {
            Instance.Dir = dir;
            Instance.IsContainer = isContainer;
            return Instance;
        }
    }
}