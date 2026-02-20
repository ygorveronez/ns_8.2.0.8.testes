using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota
{
    public class RegraPlanejamentoFrotaCEPDestino : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPDestino>
    {
        public RegraPlanejamentoFrotaCEPDestino(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPDestino> BuscarPorRegraPlanejamentoFrota(int codigoRegraPlanejamentoFrota)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPDestino>();

            query = query.Where(o => o.RegraPlanejamentoFrota.Codigo == codigoRegraPlanejamentoFrota);

            return query.ToList();
        }

        public void DeletarPorRegraPlanejamentoFrota(int codigoRegraPlanejamentoFrota)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM RegraPlanejamentoFrotaCEPDestino c WHERE c.RegraPlanejamentoFrota.Codigo = :codigoRegraPlanejamentoFrota").SetInt32("codigoRegraPlanejamentoFrota", codigoRegraPlanejamentoFrota).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM RegraPlanejamentoFrotaCEPDestino c WHERE c.RegraPlanejamentoFrota.Codigo = :codigoRegraPlanejamentoFrota").SetInt32("codigoRegraPlanejamentoFrota", codigoRegraPlanejamentoFrota).ExecuteUpdate();

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException ex)
            {
                if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                    if (excecao.Number == 547)
                    {
                        throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                    }
                }
                throw;
            }
        }
    }
}