using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pessoas
{
    public class GrupoPessoasTipoCargaEmbarcadorTipoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga>
    {
        public GrupoPessoasTipoCargaEmbarcadorTipoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga> BuscarPorGrupoPessoas(int codigoGrupoPessoas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga>();

            query = query.Where(o => o.TipoCargaEmbarcador.GrupoPessoas.Codigo == codigoGrupoPessoas);

            return query.ToList();
        }

        public bool ExistePorGrupoPessoasETipoCarga(int codigoGrupoPessoas, int codigoTipoCargaEmbarcador, int codigo = 0)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga>();

            query = query.Where(o => o.TipoCargaEmbarcador.GrupoPessoas.Codigo == codigoGrupoPessoas && o.TipoCargaEmbarcador.Codigo == codigoTipoCargaEmbarcador);

            if (codigo > 0)
                query = query.Where(o => o.Codigo != codigo);

            return query.Select(o => o.Codigo).Any();
        }

        public Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga BuscarPorGrupoPessoasETipoCarga(int codigoGrupoPessoas, int codigoTipoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga>();

            query = query.Where(o => o.TipoCargaEmbarcador.GrupoPessoas.Codigo == codigoGrupoPessoas && o.TipoCarga.Codigo == codigoTipoCarga);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga BuscarPorTipoCargaEmbarcador(int codigoTipoCargaEmbarcador)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga>();

            query = query.Where(o => o.TipoCargaEmbarcador.Codigo == codigoTipoCargaEmbarcador);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga> Consultar(int codigoGrupoPessoas, string propOrdenar, string dirOrdenar, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga>();

            query = query.Where(o => o.TipoCargaEmbarcador.GrupoPessoas.Codigo == codigoGrupoPessoas);

            return query.Fetch(o => o.TipoCargaEmbarcador).Fetch(o => o.TipoCarga).OrderBy(propOrdenar + " " + dirOrdenar).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(int codigoGrupoPessoas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga>();

            query = query.Where(o => o.TipoCargaEmbarcador.GrupoPessoas.Codigo == codigoGrupoPessoas);

            return query.Count();
        }

        public void DeletarPorTipoCargaEmbarcador(int codigoTipoCargaEmbarcador)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM GrupoPessoasTipoCargaEmbarcadorTipoCarga WHERE Codigo IN (SELECT c.Codigo FROM GrupoPessoasTipoCargaEmbarcadorTipoCarga c WHERE c.TipoCargaEmbarcador.Codigo = :codigoTipoCargaEmbarcador)").SetInt32("codigoTipoCargaEmbarcador", codigoTipoCargaEmbarcador).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM GrupoPessoasTipoCargaEmbarcadorTipoCarga WHERE Codigo IN (SELECT c.Codigo FROM GrupoPessoasTipoCargaEmbarcadorTipoCarga c WHERE c.TipoCargaEmbarcador.Codigo = :codigoTipoCargaEmbarcador)").SetInt32("codigoTipoCargaEmbarcador", codigoTipoCargaEmbarcador).ExecuteUpdate();

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
