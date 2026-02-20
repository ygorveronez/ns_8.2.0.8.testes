using Dominio.Interfaces.Database;
using Dominio.ObjetosDeValor.Enumerador;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public sealed class UnitOfWork : IUnitOfWork, IDisposable, IAsyncDisposable
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
            else if (tipoSessaoBancoDados == TipoSessaoBancoDados.AtualizarAtual)
                Sessao = NHibernateHttpModule.CurrentSession(stringConexao, forcarAbrirNovaConexao: true);
            else
                Sessao = NHibernateHttpModule.CurrentSession(stringConexao, forcarAbrirNovaConexao: false);
        }

        [Obsolete("Utilizar apenas para testes de performance das threads")]
        public UnitOfWork(string stringConexao, SqlConnection sqlConnection)
        {
            StringConexao = stringConexao;
            Sessao = SessionHelper.OpenSession(stringConexao, sqlConnection);
        }

        #endregion

        #region Métodos Privados

        private bool IsTransacaoPermiteRollback()
        {
            return ((_transacao != null) && _transacao.IsActive && !_transacao.WasRolledBack & !_transacao.WasCommitted);
        }

        #endregion

        #region Métodos Públicos

        public void Clear()
        {
            Sessao.Clear();
        }

        public void Clear(object entidade)
        {
            Sessao.Evict(entidade);
        }

        public void CommitChanges()
        {
            if (_transacao == null)
                throw new InvalidOperationException("Não foi aberta transação.");

            if (!_transacao.WasRolledBack & !_transacao.WasCommitted)
                _transacao.Commit();
        }

        public async Task CommitChangesAsync(CancellationToken cancellationToken = default)
        {
            if (_transacao == null)
                throw new InvalidOperationException("Não foi aberta transação.");

            if (!_transacao.WasRolledBack & !_transacao.WasCommitted)
                await _transacao.CommitAsync(cancellationToken);
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
                catch (Exception ex)
                {
                    Infrastructure.Services.Logging.Logger.Current.Error($"Rollback error: {ex}", "Rollback_Ex");
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

        public async ValueTask DisposeAsync()
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
                    await _transacao.RollbackAsync();
                    _transacao.Dispose();
                }
                catch (Exception ex)
                {
                    Infrastructure.Services.Logging.Logger.Current.Error($"Rollback error: {ex}", "Rollback_Ex");
                }
            }

            if (Sessao != null)
            {
                if (Sessao.IsConnected && (Sessao.Connection.State != ConnectionState.Closed))
                    Sessao.Connection.Close();

                if (Sessao.IsOpen)
                {
                    Sessao.Connection.Dispose();
                    Sessao.Close();
                }

                Sessao.Dispose();
            }

            _disposed = true;

            GC.SuppressFinalize(this);
        }

        public void Flush()
        {
            Sessao.Flush();
        }

        public void FlushAndClear()
        {
            Sessao.Flush();
            Sessao.Clear();
        }

        public System.Data.Common.DbConnection GetConnection()
        {
            return Sessao.Connection;
        }

        public bool IsActiveTransaction()
        {
            return (_transacao != null) && _transacao.IsActive;
        }

        public bool IsTransacaoAtiva()
        {
            return IsActiveTransaction();
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
            catch (Exception ex)
            {
                Infrastructure.Services.Logging.Logger.Current.Error($"Rollback error: {ex}", "Rollback_Ex");
            }
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!IsTransacaoPermiteRollback())
                return;

            try
            {
                await _transacao.RollbackAsync(cancellationToken);
                Sessao.Clear();
            }
            catch (Exception ex)
            {
                Infrastructure.Services.Logging.Logger.Current.Error($"Rollback error: {ex}", "Rollback_Ex");
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

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            _transacao = Sessao.BeginTransaction();
        }

        public async Task StartAsync(IsolationLevel isolation, CancellationToken cancellationToken = default)
        {
            _transacao = Sessao.BeginTransaction(isolation);
        }

        #endregion
    }
}