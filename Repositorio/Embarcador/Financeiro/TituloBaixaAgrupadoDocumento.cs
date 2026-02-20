using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class TituloBaixaAgrupadoDocumento : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento>
    {
        public TituloBaixaAgrupadoDocumento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.TituloBaixa BuscarTituloBaixaPorTituloBaixaAgrupadoDocumento(int codigoTituloBaixaAgrupadoDocumento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento>();

            query = query.Where(o => o.Codigo == codigoTituloBaixaAgrupadoDocumento);

            return query.Select(o => o.TituloBaixaAgrupado.TituloBaixa).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> BuscarPorBaixa(int codigoBaixa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento>();

            query = query.Where(obj => obj.TituloBaixaAgrupado.TituloBaixa.Codigo == codigoBaixa);

            return query.OrderBy(o => o.ValorPago).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> BuscarPorTituloBaixaAgrupado(int codigoTituloBaixaAgrupado)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento>();

            query = query.Where(obj => obj.TituloBaixaAgrupado.Codigo == codigoTituloBaixaAgrupado);

            return query.Fetch(o => o.TituloDocumento).ThenFetch(o => o.Carga)
                        .Fetch(o => o.TituloDocumento).ThenFetch(o => o.CTe).ThenFetch(o => o.Serie)
                        .OrderBy(o => o.ValorTotalAPagar - o.ValorPago)
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> BuscarPorTituloBaixa(int codigoTituloBaixa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento>();

            query = query.Where(obj => obj.TituloBaixaAgrupado.TituloBaixa.Codigo == codigoTituloBaixa);

            return query.Fetch(o => o.TituloBaixaAgrupado)
                        .Fetch(o => o.TituloDocumento).ThenFetch(o => o.Carga)
                        .Fetch(o => o.TituloDocumento).ThenFetch(o => o.CTe)
                        .ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento BuscarPorTituloBaixaAgrupadoETituloDocumento(int codigoTituloBaixaAgrupado, int codigoTituloDocumento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento>();

            query = query.Where(obj => obj.TituloBaixaAgrupado.Codigo == codigoTituloBaixaAgrupado && obj.TituloDocumento.Codigo == codigoTituloDocumento);

            return query.FirstOrDefault();
        }

        public Dominio.ObjetosDeValor.Embarcador.Financeiro.ValoresSumarizadosBaixaTitulo ObterValoresSumarizadosPorTituloBaixaAgrupado(int codigoTituloBaixaAgrupado)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento>();

            query = query.Where(o => o.TituloBaixaAgrupado.Codigo == codigoTituloBaixaAgrupado);

            return query.GroupBy(o => o.TituloBaixaAgrupado).Select(o => new Dominio.ObjetosDeValor.Embarcador.Financeiro.ValoresSumarizadosBaixaTitulo()
            {
                ValorAcrescimo = o.Sum(t => (decimal?)t.ValorAcrescimo) ?? 0m,
                ValorAcrescimoMoeda = o.Sum(t => (decimal?)t.ValorAcrescimoMoeda) ?? 0m,
                ValorDesconto = o.Sum(t => (decimal?)t.ValorDesconto) ?? 0m,
                ValorDescontoMoeda = o.Sum(t => (decimal?)t.ValorDescontoMoeda) ?? 0m,
                ValorPago = o.Sum(t => (decimal?)t.ValorPago) ?? 0m,
                ValorPagoMoeda = o.Sum(t => (decimal?)t.ValorPagoMoeda) ?? 0m,
                ValorTotalAPagar = o.Sum(t => (decimal?)t.ValorTotalAPagar) ?? 0m,
                ValorTotalAPagarMoeda = o.Sum(t => (decimal?)t.ValorTotalAPagarMoeda) ?? 0m
            }).FirstOrDefault();
        }

        public decimal ObterValorPagoPorTituloBaixaAgrupado(int codigoTituloBaixaAgrupado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento>();

            query = query.Where(o => o.TituloBaixaAgrupado.Codigo == codigoTituloBaixaAgrupado);

            return query.Sum(o => (decimal?)o.ValorPago) ?? 0m;
        }

        public decimal ObterValorAcrescimoPorTituloBaixaAgrupado(int codigoTituloBaixaAgrupado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento>();

            query = query.Where(o => o.TituloBaixaAgrupado.Codigo == codigoTituloBaixaAgrupado);

            return query.Sum(o => (decimal?)o.ValorAcrescimo) ?? 0m;
        }

        public decimal ObterValorDescontoPorTituloBaixaAgrupado(int codigoTituloBaixaAgrupado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento>();

            query = query.Where(o => o.TituloBaixaAgrupado.Codigo == codigoTituloBaixaAgrupado);

            return query.Sum(o => (decimal?)o.ValorDesconto) ?? 0m;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> ConsultarPorTituloBaixa(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloBaixaAgrupadoDocumento filtrosPesquisa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento>();

            query = query.Where(o => o.TituloBaixaAgrupado.TituloBaixa.Codigo == filtrosPesquisa.CodigoTituloBaixa);

            if (filtrosPesquisa.NumeroCTe > 0)
                query = query.Where(o => o.TituloDocumento.CTe.Numero == filtrosPesquisa.NumeroCTe);

            if (filtrosPesquisa.CodigoDocumento > 0)
                query = query.Where(o => o.TituloDocumento.CTe.Codigo == filtrosPesquisa.CodigoDocumento);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                query = query.Where(o => o.TituloDocumento.Carga.CodigoCargaEmbarcador == filtrosPesquisa.NumeroCarga);

            if (filtrosPesquisa.NumeroTitulo > 0)
                query = query.Where(o => o.TituloBaixaAgrupado.Titulo.Codigo == filtrosPesquisa.NumeroTitulo);

            if (filtrosPesquisa.CodigoTomador > 0)
                query = query.Where(o => o.TituloBaixaAgrupado.Titulo.Pessoa.CPF_CNPJ == filtrosPesquisa.CodigoTomador);

            return query.Fetch(o => o.TituloBaixaAgrupado)
                        .ThenFetch(o => o.Titulo)
                        .ThenFetch(o => o.GrupoPessoas)
                        .Fetch(o => o.TituloBaixaAgrupado)
                        .ThenFetch(o => o.Titulo)
                        .ThenFetch(o => o.Pessoa)
                        .Fetch(o => o.TituloBaixaAgrupado)
                        .ThenFetch(o => o.TituloBaixa)
                        .Fetch(o => o.TituloDocumento)
                        .ThenFetch(o => o.CTe)
                        .ThenFetch(o => o.Serie)
                        .Fetch(o => o.TituloDocumento)
                        .ThenFetch(o => o.Carga)
                        .OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarPorTituloBaixa(int codigoTituloBaixa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento>();

            query = query.Where(o => o.TituloBaixaAgrupado.TituloBaixa.Codigo == codigoTituloBaixa);

            return query.Count();
        }

        public int ContarPorTituloBaixaAgrupado(int codigoTituloBaixaAgrupado)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento>();

            query = query.Where(o => o.TituloBaixaAgrupado.Codigo == codigoTituloBaixaAgrupado);

            return query.Count();
        }

        public int ContarConsultaPorTituloBaixa(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloBaixaAgrupadoDocumento filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento>();

            query = query.Where(o => o.TituloBaixaAgrupado.TituloBaixa.Codigo == filtrosPesquisa.CodigoTituloBaixa);

            if (filtrosPesquisa.NumeroCTe > 0)
                query = query.Where(o => o.TituloDocumento.CTe.Numero == filtrosPesquisa.NumeroCTe);

            if (filtrosPesquisa.CodigoDocumento > 0)
                query = query.Where(o => o.TituloDocumento.CTe.Codigo == filtrosPesquisa.CodigoDocumento);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                query = query.Where(o => o.TituloDocumento.Carga.CodigoCargaEmbarcador == filtrosPesquisa.NumeroCarga);

            if (filtrosPesquisa.NumeroTitulo > 0)
                query = query.Where(o => o.TituloBaixaAgrupado.Titulo.Codigo == filtrosPesquisa.NumeroTitulo);

            if (filtrosPesquisa.CodigoTomador > 0)
                query = query.Where(o => o.TituloBaixaAgrupado.Titulo.Pessoa.CPF_CNPJ == filtrosPesquisa.CodigoTomador);

            return query.Count();
        }

        public decimal ObterValorTotalAPagarPorTituloBaixaAgrupado(int codigoTituloBaixaAgrupado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento>();

            query = query.Where(o => o.TituloBaixaAgrupado.Codigo == codigoTituloBaixaAgrupado);

            return query.Sum(o => (decimal?)o.ValorTotalAPagar) ?? 0m;
        }

        public void DeletarPorTituloBaixaAgrupado(int codigoTituloBaixaAgrupado)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM TituloBaixaAgrupadoDocumentoAcrescimoDesconto WHERE Codigo IN (SELECT c.Codigo FROM TituloBaixaAgrupadoDocumentoAcrescimoDesconto c WHERE c.TituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.Codigo = :codigoTituloBaixaAgrupado)").SetInt32("codigoTituloBaixaAgrupado", codigoTituloBaixaAgrupado).ExecuteUpdate();
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM TituloBaixaAgrupadoDocumento c WHERE c.TituloBaixaAgrupado.Codigo = :codigoTituloBaixaAgrupado").SetInt32("codigoTituloBaixaAgrupado", codigoTituloBaixaAgrupado).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM TituloBaixaAgrupadoDocumentoAcrescimoDesconto WHERE Codigo IN (SELECT c.Codigo FROM TituloBaixaAgrupadoDocumentoAcrescimoDesconto c WHERE c.TituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.Codigo = :codigoTituloBaixaAgrupado)").SetInt32("codigoTituloBaixaAgrupado", codigoTituloBaixaAgrupado).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM TituloBaixaAgrupadoDocumento c WHERE c.TituloBaixaAgrupado.Codigo = :codigoTituloBaixaAgrupado").SetInt32("codigoTituloBaixaAgrupado", codigoTituloBaixaAgrupado).ExecuteUpdate();

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

        public void SetarBaixaFinalizada(int codigoTituloBaixaAgrupadoDocumento, bool baixaFinalizada)
        {
            string hql = "UPDATE TituloBaixaAgrupadoDocumento tituloBaixaAgrupadoDocumento SET tituloBaixaAgrupadoDocumento.BaixaFinalizada = :baixaFinalizada WHERE tituloBaixaAgrupadoDocumento.Codigo = :codigoTituloBaixaAgrupadoDocumento";

            var query = this.SessionNHiBernate.CreateQuery(hql);

            query.SetInt32("codigoTituloBaixaAgrupadoDocumento", codigoTituloBaixaAgrupadoDocumento);
            query.SetBoolean("baixaFinalizada", baixaFinalizada);

            query.ExecuteUpdate();
        }
    }
}
