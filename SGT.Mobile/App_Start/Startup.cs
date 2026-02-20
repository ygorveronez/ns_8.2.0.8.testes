using Owin;
using System;
using Microsoft.Extensions.Configuration;

namespace SGT.Mobile
{
    public class Startup
    {
        private static IConfigurationRoot _appSettingsAD;

        public static IConfigurationRoot appSettingsAD
        {
            get
            {
                if (_appSettingsAD == null)
                {
                    // Cria o arquivo appsettingsad.json se não existir
                    string diretorio = AppDomain.CurrentDomain.BaseDirectory;
                    Utilidades.File.CreateAppSettingsJsonIfNotExist(diretorio);

                    // Carrega a configuração do arquivo appsettingsad.json
                    _appSettingsAD = new ConfigurationBuilder()
                                        .SetBasePath(diretorio)
                                        .AddJsonFile("appsettings.json").Build();
                }
                return _appSettingsAD;
            }
        }

        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}