using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;


namespace Dominio.Interfaces.Repositorios
{
    public interface IRepositorioGenerico<T> where T : class
    {
        #region Leitura

        Task<List<T>> ObterAsync(CancellationToken cancellationToken);

        Task<List<string>> ObterDistintosAsync(FieldDefinition<T, string> campo, FilterDefinition<T> filtro, CancellationToken cancellationToken);

        Task<List<T>> ObterPorExpressaoAsync(Expression<Func<T, bool>> condicao, CancellationToken cancellationToken, int? take = null);

        Task<T> ObterUmPorExpressaoAsync(Expression<Func<T, bool>> condicao, CancellationToken cancellationToken);

        #endregion

        #region Inserção

        Task InserirUmAsync(T entidade, CancellationToken cancellationToken);

        Task InserirMuitosAsync(List<T> entidades, CancellationToken cancellationToken);

        Task<List<T>> InserirMuitosERetornarAsync(List<T> entidades, CancellationToken cancellationToken);

        Task<T> InserirOuAtualizarAsync(Expression<Func<T, bool>> condicao, T entidade, CancellationToken cancellationToken);

        #endregion

        #region Atualização

        Task AtualizarUmAsync(FilterDefinition<T> filtro, UpdateDefinition<T> entidade, CancellationToken cancellationToken);

        Task AtualizarAsync(T entidade, CancellationToken cancellationToken);

        #endregion

        #region Exclusão

        Task DeletarAsync(FilterDefinition<T> filtro, CancellationToken cancellationToken);

        Task DeletarUmAsync(Expression<Func<T, bool>> filtro, CancellationToken cancellationToken);

        Task DeletarMuitosAsync(Expression<Func<T, bool>> filtro, CancellationToken cancellationToken);

        #endregion
    }
}
