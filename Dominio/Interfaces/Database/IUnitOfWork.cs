using NHibernate;
using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Dominio.Interfaces.Database
{
    /// <summary>
    /// Interface que define o contrato para a unidade de trabalho (Unit of Work pattern)
    /// </summary>
    public interface IUnitOfWork 
    {
        #region Propriedades                

        string StringConexao { get; }

        #endregion

        #region Métodos de Controle de Sessão

        /// <summary>
        /// Limpa a sessão do NHibernate, removendo todas as entidades do cache de primeiro nível
        /// </summary>
        void Clear();

        /// <summary>
        /// Remove uma entidade específica do cache de primeiro nível da sessão
        /// </summary>
        /// <param name="entidade">Entidade a ser removida do cache</param>
        void Clear(object entidade);

        /// <summary>
        /// Força a sincronização das mudanças pendentes com o banco de dados
        /// </summary>
        void Flush();

        /// <summary>
        /// Executa Flush seguido de Clear para sincronizar e limpar a sessão
        /// </summary>
        void FlushAndClear();

        /// <summary>
        /// Obtém a conexão de banco de dados da sessão atual
        /// </summary>
        /// <returns>Conexão de banco de dados</returns>
        DbConnection GetConnection();

        #endregion

        #region Métodos de Controle de Transação

        /// <summary>
        /// Inicia uma nova transação
        /// </summary>
        void Start();

        /// <summary>
        /// Inicia uma nova transação com nível de isolamento específico
        /// </summary>
        /// <param name="isolationLevel">Nível de isolamento da transação</param>
        void Start(IsolationLevel isolationLevel);

        /// <summary>
        /// Inicia uma nova transação de forma assíncrona
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Task representando a operação assíncrona</returns>
        Task StartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Inicia uma nova transação de forma assíncrona com nível de isolamento específico
        /// </summary>
        /// <param name="isolation">Nível de isolamento da transação</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Task representando a operação assíncrona</returns>
        Task StartAsync(IsolationLevel isolation, CancellationToken cancellationToken = default);

        /// <summary>
        /// Confirma as mudanças da transação atual
        /// </summary>
        void CommitChanges();

        /// <summary>
        /// Confirma as mudanças da transação atual de forma assíncrona
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Task representando a operação assíncrona</returns>
        Task CommitChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Desfaz as mudanças da transação atual
        /// </summary>
        void Rollback();

        /// <summary>
        /// Desfaz as mudanças da transação atual de forma assíncrona
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Task representando a operação assíncrona</returns>
        Task RollbackAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica se existe uma transação ativa
        /// </summary>
        /// <returns>True se existe uma transação ativa, false caso contrário</returns>
        bool IsActiveTransaction();

        /// <summary>
        /// Verifica se existe uma transação ativa (método legado)
        /// </summary>
        /// <returns>True se existe uma transação ativa, false caso contrário</returns>
        bool IsTransacaoAtiva();

        #endregion
    }
}
