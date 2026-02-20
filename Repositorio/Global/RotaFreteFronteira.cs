using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class RotaFreteFronteira : RepositorioBase<Dominio.Entidades.RotaFreteFronteira>
    {
        public RotaFreteFronteira(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.RotaFreteFronteira BuscarPorCodigo(int codigo)
        {
            var consultaRotaFreteFronteira = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteFronteira>()
                .Where(o => o.Codigo == codigo);

            return consultaRotaFreteFronteira.FirstOrDefault();
        }

        public List<Dominio.Entidades.RotaFreteFronteira> BuscarPorRotaFrete(int codigoRotaFrete)
        {
            var consultaRotaFreteFronteira = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteFronteira>()
                .Where(o => o.RotaFrete.Codigo == codigoRotaFrete);

            return consultaRotaFreteFronteira.ToList();
        }

        public Dominio.Entidades.RotaFreteFronteira BuscarPorRotaFreteECliente(int codigoRotaFrete, double cnpjFronteira)
        {
            var consultaRotaFreteFronteira = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteFronteira>()
                .Where(o => o.RotaFrete.Codigo == codigoRotaFrete && o.Cliente.CPF_CNPJ == cnpjFronteira);

            return consultaRotaFreteFronteira.FirstOrDefault();
        }

        public void DeletarPorRotaFrete(int codigoRotaFrete)
        {
            try
            {

                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM RotaFreteFronteira c WHERE c.RotaFrete.Codigo = :codigoRotaFrete").SetInt32("codigoRotaFrete", codigoRotaFrete).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM RotaFreteFronteira c WHERE c.RotaFrete.Codigo = :codigoRotaFrete").SetInt32("codigoRotaFrete", codigoRotaFrete).ExecuteUpdate();

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
