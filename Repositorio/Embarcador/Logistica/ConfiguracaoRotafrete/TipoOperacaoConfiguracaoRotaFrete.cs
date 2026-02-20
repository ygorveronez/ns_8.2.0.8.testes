using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class TipoOperacaoConfiguracaoRotaFrete : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TipoOperacaoConfiguracaoRotaFrete>
    {
        #region Construtores

        public TipoOperacaoConfiguracaoRotaFrete(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        public Task<List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TipoOperacaoConfiguracaoRotaFrete>> BuscarPorConfiguracaoRotaFrete(int codigoConfiguracaoRotaFrete)
        {
            var consultaTipoOperacaoConfiguracaoRotaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TipoOperacaoConfiguracaoRotaFrete>()
                .Where(o => o.ConfiguracaoRotaFrete.Codigo == codigoConfiguracaoRotaFrete);

            consultaTipoOperacaoConfiguracaoRotaFrete = consultaTipoOperacaoConfiguracaoRotaFrete
                .Fetch(o => o.TipoOperacao);

            return consultaTipoOperacaoConfiguracaoRotaFrete.ToListAsync(CancellationToken);
        }

        public void DeletarPorConfiguracaoRotaFrete(int codigoConfiguracaoRotaFrete)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao
                        .CreateQuery($"delete from TipoOperacaoConfiguracaoRotaFrete where ConfiguracaoRotaFrete.Codigo = :codigoConfiguracaoRotaFrete ")
                        .SetInt32("codigoConfiguracaoRotaFrete", codigoConfiguracaoRotaFrete)
                        .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao
                            .CreateQuery($"delete from TipoOperacaoConfiguracaoRotaFrete where ConfiguracaoRotaFrete.Codigo = :codigoConfiguracaoRotaFrete ")
                            .SetInt32("codigoConfiguracaoRotaFrete", codigoConfiguracaoRotaFrete)
                            .ExecuteUpdate();

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException excecao)
            {
                if (excecao.InnerException != null && object.ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;

                    if (excecaoSql.Number == 547)
                        throw new Exception("O registro possui dependências e não pode ser excluido.", excecao);
                }

                throw;
            }
        }

        #endregion
    }
}
