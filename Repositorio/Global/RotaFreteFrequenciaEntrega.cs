using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class RotaFreteFrequenciaEntrega : RepositorioBase<Dominio.Entidades.RotaFreteFrequenciaEntrega>
    {
        public RotaFreteFrequenciaEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.RotaFreteFrequenciaEntrega> BuscarPorRotasFrete(List<int> rotas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteFrequenciaEntrega>();

            var result = from obj in query where rotas.Contains(obj.RotaFrete.Codigo) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.RotaFreteFrequenciaEntrega> BuscarPorRotaFrete(int codigoRotaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteFrequenciaEntrega>();

            var result = from obj in query where obj.RotaFrete.Codigo == codigoRotaFrete select obj;

            return result.ToList();
        }

        public void DeletarPorRotaFrete(int codigoRotaFrete)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM RotaFreteFrequenciaEntrega c WHERE c.RotaFrete.Codigo = :codigoRotaFrete").SetInt32("codigoRotaFrete", codigoRotaFrete).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM RotaFreteFrequenciaEntrega c WHERE c.RotaFrete.Codigo = :codigoRotaFrete").SetInt32("codigoRotaFrete", codigoRotaFrete).ExecuteUpdate();

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
