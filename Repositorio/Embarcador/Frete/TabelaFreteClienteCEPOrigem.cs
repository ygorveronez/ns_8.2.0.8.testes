using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class TabelaFreteClienteCEPOrigem : RepositorioBase<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem>
    {
        public TabelaFreteClienteCEPOrigem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem> BuscarPorTabelaFrete(int codigoTabelaFreteCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem>();

            query = query.Where(o => o.TabelaFreteCliente.Codigo == codigoTabelaFreteCliente);

            return query.ToList();
        }

        public void DeletarPorTabelaFreteCliente(int codigoTabelaFretecliente)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM TabelaFreteClienteCEPOrigem c WHERE c.TabelaFreteCliente.Codigo = :codigoTabelaFretecliente").SetInt32("codigoTabelaFretecliente", codigoTabelaFretecliente).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM TabelaFreteClienteCEPOrigem c WHERE c.TabelaFreteCliente.Codigo = :codigoTabelaFretecliente").SetInt32("codigoTabelaFretecliente", codigoTabelaFretecliente).ExecuteUpdate();

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
