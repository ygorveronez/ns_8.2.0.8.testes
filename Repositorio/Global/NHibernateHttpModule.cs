using NHibernate;
using System;
using System.Data;
using System.Threading;
using System.Web;

namespace Repositorio
{
    public class NHibernateHttpModule : System.Web.IHttpModule
    {
        public const string KEY = "_TheSession_";

        public NHibernateHttpModule()
        {
        }

        #region "IHttpModule Members"

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
        }

        #endregion

        public static void CloseSession(string stringConexao)
        {
            if (HttpContext.Current == null)
                return;

            HttpContext currentContext = HttpContext.Current;
            ISession session = currentContext.Items[stringConexao] as ISession;

            if (session == null)
                return;

            if (session.IsConnected)
            {
                //session.Flush(); //todo: quando inclui o flush qualquer informação inconsistente no banco vai gerar uma excessão aqui, ver como tratar
                if (session.Connection.State != ConnectionState.Closed)
                    session.Connection.Close();

                session.Connection.Dispose();
                session.Close();
            }

            session.Dispose();

            currentContext.Items[stringConexao] = null;
        }

        private static void CloseSession(ISession session)
        {
            if (session == null)
                return;

            if (session.IsConnected)
            {
                //session.Flush(); //todo: quando inclui o flush qualquer informação inconsistente no banco vai gerar uma excessão aqui, ver como tratar
                if (session.Connection.State != ConnectionState.Closed)
                    session.Connection.Close();

                session.Connection.Dispose();
                session.Close();
            }

            session.Dispose();
        }

        internal static ISession CurrentSession(string stringConexao, bool forcarAbrirNovaConexao)
        {
            if (HttpContext.Current == null || forcarAbrirNovaConexao)
            {
                ISession session = Thread.GetData(Thread.GetNamedDataSlot(stringConexao)) as ISession;
                if (session == null || !session.IsOpen)
                {

                    try
                    {
                        CloseSession(session);
                    }
                    catch (Exception ex)
                    {
                        Infrastructure.Services.Logging.Logger.Current.Error($"[Arquitetura-CatchNoAction] Erro ao fechar sessão NHibernate durante abertura de nova sessão: {ex}", "CatchNoAction");
                    }

                    session = SessionHelper.OpenSession(stringConexao);

                    Thread.SetData(Thread.GetNamedDataSlot(stringConexao), session);
                }
                return session;
            }
            else
            {
                HttpContext currentContext = HttpContext.Current;
                ISession session = currentContext.Items[stringConexao] as ISession;
                if (session == null || !session.IsOpen)
                {
                    try
                    {
                        CloseSession(session);
                    }
                    catch (Exception ex)
                    {
                        Infrastructure.Services.Logging.Logger.Current.Error($"[Arquitetura-CatchNoAction] Erro ao fechar sessão NHibernate (HttpContext): {ex}", "CatchNoAction");
                    }

                    session = SessionHelper.OpenSession(stringConexao);

                    currentContext.Items[stringConexao] = session;
                }
                return session;
            }
        }
    }
}

