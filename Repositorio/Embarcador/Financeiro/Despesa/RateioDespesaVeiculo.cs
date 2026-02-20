using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro.Despesa
{
    public class RateioDespesaVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo>
    {
        public RateioDespesaVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo BuscarPorMovimentoFinanceiro(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo>();
            var result = from obj in query where obj.MovimentoFinanceiro.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo BuscarPorTitulo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo>();
            var result = from obj in query where obj.Titulo.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo BuscarPorContratoFinanciamento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo>();
            var result = from obj in query where obj.ContratoFinanciamento.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo BuscarPorInfracao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo>();
            var result = from obj in query where obj.Infracao.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo> Consultar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRateioDespesaVeiculo filtros, string propOrdenar, string dirOrdenar, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo> query = ObterConsulta(filtros);

            query = query.OrderBy(propOrdenar + " " + dirOrdenar);

            if (inicio > 0 || limite > 0)
                query = query.Skip(inicio).Take(limite);

            return query.ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRateioDespesaVeiculo filtros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo> query = ObterConsulta(filtros);

            return query.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo> ObterConsulta(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRateioDespesaVeiculo filtros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo>();

            if (filtros.ValorInicial > 0m)
                query = query.Where(o => o.Valor >= filtros.ValorInicial.Value);

            if (filtros.ValorFinal > 0m)
                query = query.Where(o => o.Valor <= filtros.ValorFinal.Value);

            if (filtros.DataInicial.HasValue && filtros.DataFinal.HasValue)
                query = query.Where(o => (o.DataInicial <= filtros.DataFinal.Value.Date && o.DataFinal >= filtros.DataFinal.Value.Date) ||
                                         (o.DataInicial <= filtros.DataInicial.Value.Date && o.DataFinal >= filtros.DataInicial.Value.Date));
            else if (filtros.DataInicial.HasValue)
                query = query.Where(o => (o.DataInicial <= filtros.DataInicial.Value.Date && o.DataFinal >= filtros.DataInicial.Value.Date));
            else if (filtros.DataFinal.HasValue)
                query = query.Where(o => (o.DataInicial <= filtros.DataFinal.Value.Date && o.DataFinal >= filtros.DataFinal.Value.Date));

            if (filtros.CodigoTipoDespesa > 0)
                query = query.Where(o => o.TipoDespesa.Codigo == filtros.CodigoTipoDespesa);

            if (filtros.CodigoVeiculo > 0)
                query = query.Where(o => o.Veiculos.Any(v => v.Veiculo.Codigo == filtros.CodigoVeiculo));

            if (filtros.CodigoSegmentoVeiculo > 0)
                query = query.Where(o => o.SegmentosVeiculos.Any(s => s.Codigo == filtros.CodigoSegmentoVeiculo));

            if (filtros.CodigoCentroResultado > 0)
                query = query.Where(o => o.CentroResultados.Any(c => c.CentroResultado.Codigo == filtros.CodigoCentroResultado));

            if (!string.IsNullOrWhiteSpace(filtros.NumeroDocumento))
                query = query.Where(o => o.NumeroDocumento.Contains(filtros.NumeroDocumento));

            if (!string.IsNullOrWhiteSpace(filtros.TipoDocumento))
                query = query.Where(o => o.TipoDocumento.Contains(filtros.TipoDocumento));

            if (filtros.RatearPeloPercentualFaturadoDoVeiculoNoPeriodo.HasValue)
                query = query.Where(o => o.RatearPeloPercentualFaturadoDoVeiculoNoPeriodo == filtros.RatearPeloPercentualFaturadoDoVeiculoNoPeriodo.Value);
            
            if (filtros.CodigoColaborador > 0)
                query = query.Where(o => o.Colaborador.Codigo == filtros.CodigoColaborador);

            if (filtros.CodigoPessoa > 0)
                query = query.Where(o => o.Pessoa.CPF_CNPJ == filtros.CodigoPessoa);

            return query;
        }

        #endregion

        #region Relatórios
        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RateioDespesaVeiculo> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioRateioDespesaVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = new ConsultaRateioDespesaVeiculo().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.RateioDespesaVeiculo)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RateioDespesaVeiculo>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioRateioDespesaVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consulta = new ConsultaRateioDespesaVeiculo().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}
