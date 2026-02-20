using System.Globalization;

namespace ReportApi
{
    public static class CultureConfig
    {
        public static void RegisterCulture()
        {
            string invariantCulture = System.Configuration.ConfigurationManager.AppSettings["DefaultCulture"];

            if (string.IsNullOrWhiteSpace(invariantCulture))
                invariantCulture = "pt-BR";

            CultureInfo currentCulture = new CultureInfo(invariantCulture);
            System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = currentCulture;

            CultureInfo.DefaultThreadCurrentCulture = currentCulture;
            CultureInfo.DefaultThreadCurrentUICulture = currentCulture;
        }
    }
}