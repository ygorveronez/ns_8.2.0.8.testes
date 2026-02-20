using System;

namespace SGT.Mobile
{
    public class Global : System.Web.HttpApplication
    {
        private static bool _isFirstRequestExecuted = false;
        private static readonly object _lock = new object();

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            ExecuteOnFirstRequestOnly();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            AdminMultisoftware.Repositorio.NHibernateHttpModule.CloseSession(Conexao.AdminStringConexao);
        }

        private static void ExecuteOnFirstRequestOnly()
        {
            if (!_isFirstRequestExecuted)
            {
                lock (_lock)
                {
                    if (!_isFirstRequestExecuted)
                    {
                        Conexao.ConfigureFileStorage();

                        _isFirstRequestExecuted = true;
                    }
                }
            }
        }
    }
}