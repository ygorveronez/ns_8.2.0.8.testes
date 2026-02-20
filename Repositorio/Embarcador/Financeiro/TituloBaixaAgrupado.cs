using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class TituloBaixaAgrupado : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>
    {
        public TituloBaixaAgrupado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();

            query = query.Where(o => o.Codigo == codigo);

            return query.Fetch(o => o.Titulo).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado BuscarPorTituloEBaixa(int codigoTituloBaixa, int codigoTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();
            var result = from obj in query where obj.Titulo.Codigo == codigoTitulo && obj.TituloBaixa.Codigo == codigoTituloBaixa select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> BuscarPorBaixaTitulo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();

            query = query.Where(obj => obj.TituloBaixa.Codigo == codigo);

            return query.Fetch(o => o.Titulo).ThenFetch(o => o.Pessoa)
                        .Fetch(o => o.Titulo).ThenFetch(o => o.GrupoPessoas)
                        .Fetch(o => o.Titulo).ThenFetch(o => o.TipoMovimento).ThenFetch(o => o.PlanoDeContaDebito)
                        .Fetch(o => o.Titulo).ThenFetch(o => o.FaturaParcela).ThenFetch(o => o.Fatura)
                        .ToList();
        }

        public int ContarTipoMovimentoTituloPagamentoParcialPorBaixa(int codigoTituloBaixa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();

            query = query.Where(obj => obj.TituloBaixa.Codigo == codigoTituloBaixa && obj.ValorPago != obj.ValorTotalAPagar && obj.Titulo.TipoMovimento != null);

            return query.Select(obj => obj.Titulo.TipoMovimento.Codigo).Distinct().Count();
        }

        public List<int> BuscarCodigosTitulosComDataEmissaoSuperiorDataBaixa(int codigoTituloBaixa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();

            query = query.Where(obj => obj.TituloBaixa.Codigo == codigoTituloBaixa && obj.Titulo.DataEmissao.Value.Date > obj.DataBaixa.Date);

            return query.Select(obj => obj.Titulo.Codigo).Distinct().ToList();
        }

        public List<int> BuscarCodigosTitulosSemTipoMovimento(int codigoTituloBaixa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();

            query = query.Where(obj => obj.TituloBaixa.Codigo == codigoTituloBaixa && obj.Titulo.TipoMovimento == null);

            return query.Select(obj => obj.Titulo.Codigo).Distinct().ToList();
        }

        public DateTime? BuscarMaiorDataEmissaoPorTituloBaixa(int codigoTituloBaixa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();

            query = query.Where(o => o.TituloBaixa.Codigo == codigoTituloBaixa);

            return query.Max(o => o.Titulo.DataEmissao);
        }

        public Dominio.ObjetosDeValor.Embarcador.Financeiro.ValoresSumarizadosBaixaTitulo ObterValoresSumarizadosPorTituloBaixa(int codigoTituloBaixa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();

            query = query.Where(o => o.TituloBaixa.Codigo == codigoTituloBaixa);

            return query.GroupBy(o => o.TituloBaixa).Select(o => new Dominio.ObjetosDeValor.Embarcador.Financeiro.ValoresSumarizadosBaixaTitulo()
            {
                Valor = o.Sum(t => (decimal?)t.Titulo.ValorOriginal) ?? 0m,
                ValorAcrescimo = o.Sum(t => (decimal?)t.ValorAcrescimo) ?? 0m,
                ValorAcrescimoMoeda = o.Sum(t => (decimal?)t.ValorAcrescimoMoeda) ?? 0m,
                ValorDesconto = o.Sum(t => (decimal?)t.ValorDesconto) ?? 0m,
                ValorDescontoMoeda = o.Sum(t => (decimal?)t.ValorDescontoMoeda) ?? 0m,
                ValorPago = o.Sum(t => (decimal?)t.ValorPago) ?? 0m,
                ValorPagoMoeda = o.Sum(t => (decimal?)t.ValorPagoMoeda) ?? 0m,
                ValorTotalAPagar = o.Sum(t => (decimal?)t.ValorTotalAPagar) ?? 0m,
                ValorTotalAPagarMoeda = o.Sum(t => (decimal?)t.ValorTotalAPagarMoeda) ?? 0m,
                ValorMoeda = o.Sum(t => (decimal?)t.Titulo.ValorOriginalMoedaEstrangeira) ?? 0m
            }).FirstOrDefault();
        }

        public decimal ObterValorPagoPorTituloBaixa(int codigoTituloBaixa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();

            query = query.Where(o => o.TituloBaixa.Codigo == codigoTituloBaixa);

            return query.Sum(o => (decimal?)o.ValorPago) ?? 0m;
        }

        public decimal ObterValorAcrescimoPorTituloBaixa(int codigoTituloBaixa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();

            query = query.Where(o => o.TituloBaixa.Codigo == codigoTituloBaixa);

            return query.Sum(o => (decimal?)o.ValorAcrescimo) ?? 0m;
        }

        public decimal ObterValorDescontoPorTituloBaixa(int codigoTituloBaixa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();

            query = query.Where(o => o.TituloBaixa.Codigo == codigoTituloBaixa);

            return query.Sum(o => (decimal?)o.ValorDesconto) ?? 0m;
        }

        public decimal ObterValorTotalAPagarPorTituloBaixa(int codigoTituloBaixa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();

            query = query.Where(o => o.TituloBaixa.Codigo == codigoTituloBaixa);

            return query.Sum(o => (decimal?)o.ValorTotalAPagar) ?? 0m;
        }

        public List<int> BuscarCodigoFaturasPorTituloBaixa(int codigoTituloBaixa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();

            query = query.Where(o => o.TituloBaixa.Codigo == codigoTituloBaixa && o.Titulo.FaturaParcela != null);

            return query.Select(o => o.Titulo.FaturaParcela.Fatura.Codigo).Distinct().ToList();
        }

        public List<double> BuscarTomadoresPorTituloBaixa(int codigoTituloBaixa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();

            query = query.Where(o => o.TituloBaixa.Codigo == codigoTituloBaixa && o.Titulo.Pessoa != null);

            return query.Select(o => o.Titulo.Pessoa.CPF_CNPJ).Distinct().ToList();
        }

        public List<int> BuscarGrupoPessoasPorTituloBaixa(int codigoTituloBaixa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();

            query = query.Where(o => o.TituloBaixa.Codigo == codigoTituloBaixa && o.Titulo.GrupoPessoas != null);

            return query.Select(o => o.Titulo.GrupoPessoas.Codigo).Distinct().ToList();
        }

        public List<string> BuscarNumeroDocumentoPorTituloBaixa(int codigoTituloBaixa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();

            query = query.Where(o => o.TituloBaixa.Codigo == codigoTituloBaixa);

            return query.Select(o => o.Titulo.NumeroDocumentos).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> ConsultarTituloAReceberQuitadoPendentesIntegracao(string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();

            query = query.Where(obj => ((bool?)obj.Titulo.IntegradoQuitacao ?? false) == false && obj.Titulo.TipoTitulo == TipoTitulo.Receber && obj.Titulo.StatusTitulo == StatusTitulo.Quitada &&
                                obj.TituloBaixa.TipoBaixaTitulo == TipoTitulo.Receber && obj.TituloBaixa.SituacaoBaixaTitulo == SituacaoBaixaTitulo.Finalizada);

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaTituloAReceberQuitadoPendentesIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();

            query = query.Where(obj => ((bool?)obj.Titulo.IntegradoQuitacao ?? false) == false && obj.Titulo.TipoTitulo == TipoTitulo.Receber && obj.Titulo.StatusTitulo == StatusTitulo.Quitada &&
                                obj.TituloBaixa.TipoBaixaTitulo == TipoTitulo.Receber && obj.TituloBaixa.SituacaoBaixaTitulo == SituacaoBaixaTitulo.Finalizada);

            return query.Count();
        }

        #endregion
    }
}
