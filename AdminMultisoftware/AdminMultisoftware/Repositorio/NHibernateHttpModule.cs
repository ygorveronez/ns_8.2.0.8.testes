using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
//using System.Web;
using NHibernate;
using System.Threading;

namespace AdminMultisoftware.Repositorio
{
    public class NHibernateHttpModule//: System.Web.IHttpModule
    {
        public const string KEY = "_TheSession_";

        public NHibernateHttpModule()
        {
        }

        #region "IHttpModule Members"

        public void Dispose()
        {
        }


        //public void Init(HttpApplication context)
        //{
        //}

        #endregion

        public static void CloseSession(string stringConexao)
        {
            //if (HttpContext.Current == null)
            //    return;

            //HttpContext currentContext = HttpContext.Current;
            //ISession session = currentContext.Items[stringConexao] as ISession;

            ISession session = Thread.GetData(Thread.GetNamedDataSlot(stringConexao)) as ISession;

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
            Thread.FreeNamedDataSlot(stringConexao);
            //currentContext.Items[stringConexao] = null;
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

        internal static ISession CurrentSession(string stringConexao)
        {
            //if (HttpContext.Current == null)
            //{
            ISession session = Thread.GetData(Thread.GetNamedDataSlot(stringConexao)) as ISession;
            if (session == null || !session.IsOpen)
            {
                CloseSession(session);

                session = SessionHelper.OpenSession(stringConexao);

                Thread.SetData(Thread.GetNamedDataSlot(stringConexao), session);
            }
            return session;
            //}
            //else
            //{
            //    HttpContext currentContext = HttpContext.Current;
            //    ISession session = currentContext.Items[stringConexao] as ISession;
            //    if (session == null || !session.IsOpen)
            //    {
            //        CloseSession(session);

            //        session = SessionHelper.OpenSession(stringConexao);

            //        currentContext.Items[stringConexao] = session;
            //    }
            //    return session;
            //}
        }

    }
}
