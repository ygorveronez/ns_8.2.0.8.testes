using Microsoft.Extensions.DependencyInjection;
using System;

namespace SGT.WebService
{
    public class Global : System.Web.HttpApplication
    {
        private static bool _isFirstRequestExecuted = false;
        private static readonly object _lock = new object();
        private static IServiceProvider _serviceProvider;

        protected void Application_Start(object sender, EventArgs e)
        {
            Infrastructure.Services.Logging.Logger.Configure(new Servicos.Logging.ServicosLogger());
#if DEBUG
            //log4net.Config.XmlConfigurator.Configure();
#endif
            var serviceCollection = new ServiceCollection();
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            ExecuteOnFirstRequestOnly();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            unitOfWork.Dispose();
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            Repositorio.NHibernateHttpModule.CloseSession(Conexao.createInstance(_serviceProvider).StringConexao);
        }

        private static void ExecuteOnFirstRequestOnly()
        {
            if (!_isFirstRequestExecuted)
            {
                lock (_lock)
                {
                    if (!_isFirstRequestExecuted)
                    {
                        Conexao.createInstance(_serviceProvider).ConfigureFileStorage();

                        _isFirstRequestExecuted = true;
                    }
                }
            }
        }
    }
}