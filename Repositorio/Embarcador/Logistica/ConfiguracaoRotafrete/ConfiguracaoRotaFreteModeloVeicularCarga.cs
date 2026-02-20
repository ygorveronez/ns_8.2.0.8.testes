using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class ConfiguracaoRotaFreteModeloVeicularCarga : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteModeloVeicularCarga>
    {
        #region Construtores

        public ConfiguracaoRotaFreteModeloVeicularCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteModeloVeicularCarga> BuscarPorConfiguracaoRotaFrete(int codigoConfiguracaoRotaFrete)
        {
            var consultaConfiguracaoRotaFreteModeloVeicularCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteModeloVeicularCarga>()
                .Where(o => o.ConfiguracaoRotaFrete.Codigo == codigoConfiguracaoRotaFrete);

            consultaConfiguracaoRotaFreteModeloVeicularCarga = consultaConfiguracaoRotaFreteModeloVeicularCarga
                .Fetch(o => o.ModeloVeicularCarga);

            return consultaConfiguracaoRotaFreteModeloVeicularCarga.ToList();
        }

        public void DeletarPorConfiguracaoRotaFrete(int codigoConfiguracaoRotaFrete)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao
                        .CreateQuery($"delete from ConfiguracaoRotaFreteModeloVeicularCarga where ConfiguracaoRotaFrete.Codigo = :codigoConfiguracaoRotaFrete ")
                        .SetInt32("codigoConfiguracaoRotaFrete", codigoConfiguracaoRotaFrete)
                        .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao
                            .CreateQuery($"delete from ConfiguracaoRotaFreteModeloVeicularCarga where ConfiguracaoRotaFrete.Codigo = :codigoConfiguracaoRotaFrete ")
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
