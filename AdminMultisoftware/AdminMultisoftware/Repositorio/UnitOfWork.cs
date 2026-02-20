using AdminMultisoftware.Dominio.Enumeradores;
using System;
using System.Data;
using System.Data.SqlClient;

namespace AdminMultisoftware.Repositorio
{
    public sealed class UnitOfWork : IDisposable
    {
        #region Atributos

        private bool _disposed;
        private NHibernate.ITransaction _transacao;

        #endregion Atributos

        #region Propriedades

        internal NHibernate.ISession Sessao { get; private set; }

        public string StringConexao { get; private set; }

        #endregion Propriedades

        #region Construtores

        public UnitOfWork(string stringConexao) : this(stringConexao, tipoSessaoBancoDados: TipoSessaoBancoDados.Atual) { }

        public UnitOfWork(string stringConexao, TipoSessaoBancoDados tipoSessaoBancoDados)
        {
            StringConexao = stringConexao;

            if (tipoSessaoBancoDados == TipoSessaoBancoDados.Nova)
                Sessao = SessionHelper.OpenSession(stringConexao);
            else
                Sessao = NHibernateHttpModule.CurrentSession(stringConexao);
        }

        [Obsolete("Utilizar apenas para testes de performance das threads")]
        public UnitOfWork(string stringConexao, SqlConnection sqlConnection)
        {
            StringConexao = stringConexao;
            Sessao = SessionHelper.OpenSession(stringConexao, sqlConnection);
        }

        #endregion

        #region Métodos Privados

        private void GravarConteudoNoArquivo(string mensagem, string prefixo)
        {
            try
            {
                DateTime dateTime = DateTime.Now;
                string arquivo = (string.IsNullOrWhiteSpace(prefixo) ? "" : prefixo + "-") + dateTime.Day + "-" + dateTime.Month + "-" + dateTime.Year + ".txt";
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
                string file = System.IO.Path.Combine(path, arquivo);

                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                System.IO.StreamWriter strw = new System.IO.StreamWriter(file, true);

                try
                {
                    strw.WriteLine(DateTime.Now.ToLongTimeString());
                    strw.WriteLine(mensagem);
                    strw.WriteLine();
                }
                finally
                {
                    strw.Close();
                }
            }
            catch
            {
            }
        }

        private bool IsTransacaoPermiteRollback()
        {
            return ((_transacao != null) && _transacao.IsActive && !_transacao.WasRolledBack & !_transacao.WasCommitted);
        }

        #endregion

        #region Métodos Públicos

        public void CommitChanges()
        {
            if (_transacao == null)
                throw new InvalidOperationException("Não foi aberta transação.");

            if (!_transacao.WasRolledBack & !_transacao.WasCommitted)
                _transacao.Commit();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                GC.SuppressFinalize(this);
                return;
            }

            if ((_transacao != null) && !_transacao.WasRolledBack & !_transacao.WasCommitted)
            {
                try
                {
                    _transacao.Rollback();
                    _transacao.Dispose();
                }
                catch (Exception excecao)
                {
                    GravarConteudoNoArquivo(excecao.ToString(), "Rollback_Dispose_Ex");
                }
            }

            if (Sessao != null)
            {
                //Sessao.Flush(); //todo: quando inclui o flush qualquer informação inconsistente no banco vai gerar uma excessão aqui, ver como tratar
                if (Sessao.IsConnected && (Sessao.Connection.State != ConnectionState.Closed))
                    Sessao.Connection.Close();

                if (Sessao.IsOpen) //handle "Session is closed" error when accessing connection
                {
                    Sessao.Connection.Dispose();
                    Sessao.Close();
                }

                Sessao.Dispose();
            }

            _disposed = true;

            GC.SuppressFinalize(this);
        }

        public bool IsActiveTransaction()
        {
            return (_transacao != null) && _transacao.IsActive;
        }

        public bool IsOpenSession()
        {
            return Sessao.IsOpen;
        }

        public void Rollback()
        {
            if (!IsTransacaoPermiteRollback())
                return;

            try
            {
                _transacao.Rollback();
                Sessao.Clear();
            }
            catch (Exception excecao)
            {
                GravarConteudoNoArquivo(excecao.ToString(), "Rollback_Ex");
            }
        }

        public void Start()
        {
            _transacao = Sessao.BeginTransaction();
        }

        public void Start(IsolationLevel isolation)
        {
            _transacao = Sessao.BeginTransaction(isolation);
        }

        #endregion
    }
}
