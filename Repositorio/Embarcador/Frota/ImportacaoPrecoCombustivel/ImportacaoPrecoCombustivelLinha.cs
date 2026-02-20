using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Frota.ImportacaoPrecoCombustivel
{
    public class ImportacaoPrecoCombustivelLinha : RepositorioBase<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha>
    {
        public ImportacaoPrecoCombustivelLinha(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha> BuscarPorImportacao(int codigoImportacaoPrecoCombustivel)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha>();

            query = query.Where(o => o.ImportacaoPrecoCombustivel.Codigo == codigoImportacaoPrecoCombustivel);

            return query.ToList();
        }

        public List<int> BuscarCodigosLinhasPendentesGeracaoPedido(int codigoImportacaoPrecoCombustivel)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha>();

            query = query.Where(o => o.ImportacaoPrecoCombustivel.Codigo == codigoImportacaoPrecoCombustivel && o.PostoCombustivelTabelaValores == null);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha> BuscarPorImportacaoPrecoCombustivel(int codigoImportacaoPrecoCombustivel)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha>();

            query = query.Where(o => o.ImportacaoPrecoCombustivel.Codigo == codigoImportacaoPrecoCombustivel);

            return query.Fetch(o => o.PostoCombustivelTabelaValores).OrderBy(o => o.Numero).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha> BuscarSemCargaPorImportacaoPrecoCombustivel(int codigoImportacaoPrecoCombustivel)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha>();

            query = query.Where(o => o.ImportacaoPrecoCombustivel.Codigo == codigoImportacaoPrecoCombustivel && o.PostoCombustivelTabelaValores == null);

            return query.Fetch(o => o.PostoCombustivelTabelaValores)
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha> BuscarPorPostoCombustivelTabelaValores(int codigoPostoCombustivelTabelaValores)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha>();
            query = query.Where(o => o.PostoCombustivelTabelaValores.Codigo == codigoPostoCombustivelTabelaValores);
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha> BuscarPorPostoCombustivelTabelaValores(List<int> codigosPostoCombustivelTabelaValores)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha>();
            query = query.Where(o => codigosPostoCombustivelTabelaValores.Contains(o.PostoCombustivelTabelaValores.Codigo));
            return query.ToList();
        }

        public int ContarPostoCombustivelTabelaValoresPorImportacaoPrecoCombustivel(int codigoImportacaoPrecoCombustivel)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha>();

            query = query.Where(o => o.ImportacaoPrecoCombustivel.Codigo == codigoImportacaoPrecoCombustivel && o.PostoCombustivelTabelaValores != null);

            return query.Select(o => o.PostoCombustivelTabelaValores.Codigo).Distinct().Count();
        }

        public void SetarCargaLinhas(int codigoLinha, int codigoPostoCombustivelTabelaValores)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("UPDATE ImportacaoPrecoCombustivelLinha linha SET PostoCombustivelTabelaValores = :codigoPostoCombustivelTabelaValores WHERE Codigo = :codigoLinha").SetParameter("codigoLinha", codigoLinha).SetParameter("codigoPostoCombustivelTabelaValores", codigoPostoCombustivelTabelaValores).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("UPDATE ImportacaoPrecoCombustivelLinha linha SET PostoCombustivelTabelaValores = :codigoPostoCombustivelTabelaValores WHERE Codigo = :codigoLinha").SetParameter("codigoLinha", codigoLinha).SetParameter("codigoPostoCombustivelTabelaValores", codigoPostoCombustivelTabelaValores).ExecuteUpdate();

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
