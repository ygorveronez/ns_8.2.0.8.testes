namespace Localization.Service
{
    public class Common
    {
        #region Properties

        private System.Reflection.Assembly ExecutingAssembly = null;

        #endregion

        public Common()
        {
            ExecutingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
        }

        public string GetTranslationByResourcePath(string path, string defaultText, bool translate = true)
        {
            if (!translate || string.IsNullOrWhiteSpace(path) || !path.Contains("."))
                return defaultText;

            try
            {
                int index = path.LastIndexOf(".");

                string resourcePath = path.Remove(index, path.Length - index);
                string resourceName = path.Substring(index + 1, path.Length - (index + 1));

                System.Resources.ResourceManager rm = new System.Resources.ResourceManager(resourcePath, ExecutingAssembly);

                System.Resources.ResourceSet resourceSet = rm.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, true);

                object valor = resourceSet.GetObject(resourceName);

                if (valor != null)
                    return (string)valor;
            }
            catch
            {
#if DEBUG
                throw;
#endif
            }

            return defaultText;
        }
    }
}
