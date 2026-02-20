using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class ConfiguracaoRotaFreteTipoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteTipoCarga>
    {
        #region Construtores

        public ConfiguracaoRotaFreteTipoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteTipoCarga> BuscarPorConfiguracaoRotaFrete(int codigoConfiguracaoRotaFrete)
        {
            var consultaConfiguracaoRotaFreteTipoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteTipoCarga>()
                .Where(o => o.ConfiguracaoRotaFrete.Codigo == codigoConfiguracaoRotaFrete);

            consultaConfiguracaoRotaFreteTipoCarga = consultaConfiguracaoRotaFreteTipoCarga
                .Fetch(o => o.TipoCarga);

            return consultaConfiguracaoRotaFreteTipoCarga.ToList();
        }

        public void DeletarPorConfiguracaoRotaFrete(int codigoConfiguracaoRotaFrete)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao
                        .CreateQuery($"delete from ConfiguracaoRotaFreteTipoCarga where ConfiguracaoRotaFrete.Codigo = :codigoConfiguracaoRotaFrete ")
                        .SetInt32("codigoConfiguracaoRotaFrete", codigoConfiguracaoRotaFrete)
                        .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao
                            .CreateQuery($"delete from ConfiguracaoRotaFreteTipoCarga where ConfiguracaoRotaFrete.Codigo = :codigoConfiguracaoRotaFrete ")
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
