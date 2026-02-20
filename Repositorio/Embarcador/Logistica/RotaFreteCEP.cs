using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class RotaFreteCEP : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP>
    {
        public RotaFreteCEP(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP>();

            query = query.Where(rot => rot.Codigo == codigo);

            return query.FirstOrDefault();

        }

        public List<Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP> BuscarPorRota(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP>();

            query = query.Where(rot => rot.RotaFrete.Codigo == codigo);

            return query.ToList();

        }

        public Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP BuscarPorCEP(int cep)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP>();

            query = query.Where(rot => cep >= rot.CEPInicial && cep <= rot.CEPFinal);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP> BuscarPorCEP(double cnpf, string ufOrigem, int cep)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP>();

            query = query.Where(rot => rot.RotaFrete.Ativo && (rot.RotaFrete.EstadosOrigem.Any(o => o.Sigla == ufOrigem) || rot.RotaFrete.ClientesOrigem.Any(o => o.CPF_CNPJ == cnpf)) && cep >= rot.CEPInicial && cep <= rot.CEPFinal);

            return query.Fetch(o => o.RotaFrete).ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP BuscarPorCEP(Dominio.Entidades.RotaFrete rotaFrete, int cepInicial, int cepFinal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP>();

            query = query.Where(rot => rot.RotaFrete == rotaFrete && rot.CEPInicial == cepInicial && rot.CEPFinal == cepFinal);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP> Consultar(int codigoRotaFrete, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP>();

            query = query.Where(o => o.RotaFrete.Codigo == codigoRotaFrete);

            return query.OrderBy(propOrdena + " " + dirOrdena).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoRotaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP>();

            query = query.Where(o => o.RotaFrete.Codigo == codigoRotaFrete);

            return query.Count();
        }

        public void DeletarPorRota(int codigoRota)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE RotaFreteCEP obj WHERE obj.RotaFrete.Codigo = :codigoRota")
                                     .SetInt32("codigoRota", codigoRota)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE RotaFreteCEP obj WHERE obj.RotaFrete.Codigo = :codigoRota")
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
