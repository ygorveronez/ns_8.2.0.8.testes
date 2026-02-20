using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pessoas
{
    public class GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga>
    {
        public GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga> BuscarPorGrupoPessoas(int codigoGrupoPessoas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga>();

            query = query.Where(o => o.ModeloVeicularEmbarcador.GrupoPessoas.Codigo == codigoGrupoPessoas);

            return query.ToList();
        }

        public bool ExistePorGrupoPessoasEModeloVeicular(int codigoGrupoPessoas, int codigoModeloVeicular, int codigo = 0)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga>();

            query = query.Where(o => o.ModeloVeicularEmbarcador.GrupoPessoas.Codigo == codigoGrupoPessoas && o.ModeloVeicular.Codigo == codigoModeloVeicular);

            if (codigo > 0)
                query = query.Where(o => o.Codigo != codigo);

            return query.Select(o => o.Codigo).Any();
        }

        public Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga BuscarPorGrupoPessoasEModeloVeicular(int codigoGrupoPessoas, int codigoModeloVeicular)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga>();

            query = query.Where(o => o.ModeloVeicularEmbarcador.GrupoPessoas.Codigo == codigoGrupoPessoas && o.ModeloVeicular.Codigo == codigoModeloVeicular);
            
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga> BuscarPorModeloVeicularEmbarcador(int codigoModeloVeicularEmbarcador)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga>();

            query = query.Where(o => o.ModeloVeicularEmbarcador.Codigo == codigoModeloVeicularEmbarcador);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga> Consultar(int codigoGrupoPessoas, string propOrdenar, string dirOrdenar, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga>();

            query = query.Where(o => o.ModeloVeicularEmbarcador.GrupoPessoas.Codigo == codigoGrupoPessoas);

            return query.Fetch(o => o.ModeloVeicularEmbarcador).Fetch(o => o.ModeloVeicular).OrderBy(propOrdenar + " " + dirOrdenar).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(int codigoGrupoPessoas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga>();

            query = query.Where(o => o.ModeloVeicularEmbarcador.GrupoPessoas.Codigo == codigoGrupoPessoas);

            return query.Count();
        }

        public void DeletarPorModeloVeicularEmbarcador(int codigoModeloVeicularEmbarcador)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga WHERE Codigo IN (SELECT c.Codigo FROM GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga c WHERE c.ModeloVeicularEmbarcador.Codigo = :codigoModeloVeicularEmbarcador)").SetInt32("codigoModeloVeicularEmbarcador", codigoModeloVeicularEmbarcador).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga WHERE Codigo IN (SELECT c.Codigo FROM GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga c WHERE c.ModeloVeicularEmbarcador.Codigo = :codigoModeloVeicularEmbarcador)").SetInt32("codigoModeloVeicularEmbarcador", codigoModeloVeicularEmbarcador).ExecuteUpdate();

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
