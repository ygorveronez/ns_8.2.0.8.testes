using System;
using System.Linq;

namespace Repositorio.Embarcador.Transportadores
{
    public class TransportadorFilial : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.TransportadorFilial>
    {
        public TransportadorFilial(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Transportadores.TransportadorFilial BuscarMatrizPorTransportadorFilial(string cnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorFilial>();

            var result = from obj in query where obj.CNPJ == cnpj select obj;

            return result.FirstOrDefault();
        }

        public void DeletarPorEmpresa(int codigoEmpresa)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE TransportadorFilial obj WHERE obj.Empresa.Codigo = :codigoEmpresa")
                                     .SetInt32("codigoEmpresa", codigoEmpresa)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE TransportadorFilial obj WHERE obj.Empresa.Codigo = :codigoEmpresa")
                                .SetInt32("codigoEmpresa", codigoEmpresa)
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