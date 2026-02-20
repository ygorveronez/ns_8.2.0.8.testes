using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class RotaFreteDestinatarios : RepositorioBase<Dominio.Entidades.RotaFreteDestinatarios>
    {
        public RotaFreteDestinatarios(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.RotaFreteDestinatarios BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteDestinatarios>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();

        }
        public List<Dominio.Entidades.RotaFreteDestinatarios> BuscarPorRocartaFrete(int codigoRotaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteDestinatarios>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.RotaFrete.Codigo == codigoRotaFrete);

            return result.ToList();

        }

        public List<Dominio.Entidades.RotaFreteDestinatarios> BuscarPorCodigoIntegracaoRotaFrete(int codigoRotaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteDestinatarios>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.RotaFrete.Codigo == codigoRotaFrete);

            return result.ToList();

        }

        public Dominio.Entidades.RotaFreteDestinatarios BuscarPorCPFCNPJCodigoRotaFrete(double cpfcnpj, int codigoRotaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteDestinatarios>();

            var result = from obj in query where obj.Cliente.CPF_CNPJ.Equals(cpfcnpj) && obj.RotaFrete.Codigo == codigoRotaFrete select obj;

            return result.FirstOrDefault();
        }

        public void DeletarPorRotaFrete(int codigoRotaFrete)
        {
            try
            {

                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM RotaFreteDestinatarios c WHERE c.RotaFrete.Codigo = :codigoRotaFrete").SetInt32("codigoRotaFrete", codigoRotaFrete).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM RotaFreteDestinatarios c WHERE c.RotaFrete.Codigo = :codigoRotaFrete").SetInt32("codigoRotaFrete", codigoRotaFrete).ExecuteUpdate();

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

