using System.Globalization;

namespace SGT.WebAdmin.Extensions
{
	public static class WebApplicationBuilderExtensions
	{
		public static void ConfigureCulture(this WebApplicationBuilder builder)
		{
			string culture = builder.Configuration.GetValue<string>("Globalization:Culture");

			if (string.IsNullOrWhiteSpace(culture))
				culture = "pt-BR";

			CultureInfo cultureInfo = CultureInfo.GetCultureInfo(culture);

			CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
			CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
		}
	}
}
