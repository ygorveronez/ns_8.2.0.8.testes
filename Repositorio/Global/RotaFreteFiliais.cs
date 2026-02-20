using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class RotaFreteFiliais : RepositorioBase<Dominio.Entidades.RotaFreteFiliais>
    {
        public RotaFreteFiliais(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.RotaFreteFiliais BuscarPorCodigo(int codigo)
        {
            var consultaRotaFreteFilial = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteFiliais>()
                .Where(o => o.Codigo == codigo);

            return consultaRotaFreteFilial.FirstOrDefault();
        }

        public List<Dominio.Entidades.RotaFreteFiliais> BuscarPorRotaFrete(int codigoRotaFrete)
        {
            var consultaRotaFreteFilial = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteFiliais>()
                .Where(o => o.RotaFrete.Codigo == codigoRotaFrete);

            return consultaRotaFreteFilial.ToList();
        }

        public Dominio.Entidades.RotaFreteFiliais BuscarPorRotaFreteEFilial(int codigoRotaFrete, int codigoFilial)
        {
            var consultaRotaFreteFilial = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteFiliais>()
                .Where(o => o.RotaFrete.Codigo == codigoRotaFrete && o.Filial.Codigo == codigoFilial);

            return consultaRotaFreteFilial.FirstOrDefault();
        }

        public void DeletarPorRotaFrete(int codigoRotaFrete)
        {
            try
            {

                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM RotaFreteFiliais c WHERE c.RotaFrete.Codigo = :codigoRotaFrete").SetInt32("codigoRotaFrete", codigoRotaFrete).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM RotaFreteFiliais c WHERE c.RotaFrete.Codigo = :codigoRotaFrete").SetInt32("codigoRotaFrete", codigoRotaFrete).ExecuteUpdate();

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
