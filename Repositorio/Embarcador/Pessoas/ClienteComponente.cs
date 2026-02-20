using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pessoas
{
    public class ClienteComponente : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ClienteComponente>
    {
        public ClienteComponente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public void DeletarPorCliente(double codigoCliente)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE ClienteComponente obj WHERE obj.Cliente.CPF_CNPJ = :codigoCliente")
                                     .SetDouble("codigoCliente", codigoCliente)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE ClienteComponente obj WHERE obj.Cliente.CPF_CNPJ = :codigoCliente")
                                            .SetDouble("codigoCliente", codigoCliente)
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

        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteComponente> BuscarComponentesPorClienteFilialEmpresa(List<double> cnpjs, int codigoFilial, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteComponente>();

            var result = from obj in query where cnpjs.Contains(obj.Cliente.CPF_CNPJ) && (obj.Filial == null || obj.Filial.Codigo == codigoFilial) select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => (o.Empresa == null || o.Empresa.Codigo == codigoEmpresa));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteComponente> BuscarComponentesPorCliente(double cnpjs)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteComponente>();

            var result = from obj in query where cnpjs == obj.Cliente.CPF_CNPJ select obj;

            return result.ToList();
        }
    }
}
