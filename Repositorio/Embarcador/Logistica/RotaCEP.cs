using System;
using System.Linq;
using System.Linq.Dynamic.Core;
namespace Repositorio.Embarcador.Logistica
{
    public class RotaCEP : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.RotaCEP>
    {
        public RotaCEP(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.RotaCEP BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.RotaCEP>();

            var result = from obj in query select obj;
            result = result.Where(rot => rot.Codigo == codigo);

            return result.FirstOrDefault();

        }

        public Dominio.Entidades.Embarcador.Logistica.RotaCEP BuscarPorCEP(int cep)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.RotaCEP>();

            var result = from obj in query select obj;
            result = result.Where(rot => cep >= int.Parse(rot.CEPInicial) && cep <= int.Parse(rot.CEPFinal));

            return result.FirstOrDefault();
        }

        public void DeletarPorRota(int codigoRota)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE RotaCEP obj WHERE obj.Rota.Codigo = :codigoRota")
                                     .SetInt32("codigoRota", codigoRota)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE RotaCEP obj WHERE obj.Rota.Codigo = :codigoRota")
                                .SetInt32("codigoRota", codigoRota)
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
