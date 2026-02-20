using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class TabelaFreteClienteModeloVeicularCarga : RepositorioBase<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga>
    {
        #region Construtores

        public TabelaFreteClienteModeloVeicularCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga> BuscarPorTabelaFreteCliente(int codigoTabelaFreteCliente)
        {
            var consultaModeloVeicularCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga>()
                .Where(o => o.TabelaFreteCliente.Codigo == codigoTabelaFreteCliente);

            return consultaModeloVeicularCarga.ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga BuscarPorTabelaFreteClienteEModeloVeicularCarga(int codigoTabelaFreteCliente, int codigoModeloVeicularCarga)
        {
            var consultaModeloVeicularCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga>()
                .Where(o => o.TabelaFreteCliente.Codigo == codigoTabelaFreteCliente && o.ModeloVeicularCarga.Codigo == codigoModeloVeicularCarga);

            return consultaModeloVeicularCarga.FirstOrDefault();
        }

        public void DeletarPorTabelaFreteCliente(int codigoTabelaFretecliente)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                    UnitOfWork.Sessao.CreateQuery("delete from TabelaFreteClienteModeloVeicularCarga ModeloVeicularCarga where ModeloVeicularCarga.TabelaFreteCliente.Codigo = :codigoTabelaFretecliente").SetInt32("codigoTabelaFretecliente", codigoTabelaFretecliente).ExecuteUpdate();
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("delete from TabelaFreteClienteModeloVeicularCarga ModeloVeicularCarga where ModeloVeicularCarga.TabelaFreteCliente.Codigo = :codigoTabelaFretecliente").SetInt32("codigoTabelaFretecliente", codigoTabelaFretecliente).ExecuteUpdate();

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
                if ((excecao.InnerException != null) && object.ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;

                    if (excecaoSql.Number == 547)
                        throw new Exception("O registro possui dependências e não pode ser excluido.", excecao);
                }

                throw;
            }
        }

        public void RemoverPendenciaIntegracao(int codigoTabelaFreteCliente, int codigoModeloVeicularCarga)
        {
            UnitOfWork.Sessao
                .CreateQuery(@"
                    update TabelaFreteClienteModeloVeicularCarga
                       set PendenteIntegracao = :pendenteIntegracao
                     where TabelaFreteCliente.Codigo = :codigoTabelaFreteCliente
                       and ModeloVeicularCarga.Codigo = :codigoModeloVeicularCarga"
                )
                .SetBoolean("pendenteIntegracao", false)
                .SetInt32("codigoTabelaFreteCliente", codigoTabelaFreteCliente)
                .SetInt32("codigoModeloVeicularCarga", codigoModeloVeicularCarga)
                .ExecuteUpdate();
        }

        #endregion Métodos Públicos
    }
}
