using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Terceiros
{
    public class ContratoFreteAcrescimoDesconto : RepositorioBase<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto>
    {
        public ContratoFreteAcrescimoDesconto(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ContratoFreteAcrescimoDesconto(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> Consultar(Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaContratoFreteAcrescimoDesconto filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> result = Consultar(filtrosPesquisa);

            result = result.Fetch(o => o.Justificativa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaContratoFreteAcrescimoDesconto filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public bool ContratoEstaQuitado(int codigoContratoFrete)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            var resut = from obj in query
                        where obj.ContratoFrete.Codigo == codigoContratoFrete && obj.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada
                            && (obj.ContratoFrete.ValoresAdicionais.Any(o => o.AplicacaoValor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoAdiantamento) ? obj.Adiantado : !obj.Adiantado)
                        select obj;

            return resut.Any();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> BuscarPorSituacaoEContratoSemPagamento(int codigoContrato, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteAcrescimoDesconto situacao)
        {
            var pesagemIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto>();
            IQueryable<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto> queryPagamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto>();

            queryPagamento = queryPagamento.Where(c => c.PagamentoAgregado.Situacao == SituacaoPagamentoAgregado.Finalizado || c.PagamentoAgregado.Situacao == SituacaoPagamentoAgregado.Iniciada || c.PagamentoAgregado.Situacao == SituacaoPagamentoAgregado.AgAprovacao);
            pesagemIntegracao = pesagemIntegracao.Where(o => o.Situacao == situacao && o.ContratoFrete.Codigo == codigoContrato);

            pesagemIntegracao = pesagemIntegracao.Where(c => !queryPagamento.Any(p => p.ContratoFreteAcrescimoDesconto == c));

            return pesagemIntegracao.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> BuscarPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteAcrescimoDesconto situacao)
        {
            var pesagemIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto>()
                .Where(o => o.Situacao == situacao && !o.DisponibilizarFechamentoDeAgregado)
                .ToList();

            return pesagemIntegracao;
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> BuscarPorSituacaoEContrato(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteAcrescimoDesconto situacao, int codigoContratoFrete)
        {
            IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto>();

            query = query.Where(obj => obj.Situacao == situacao && obj.ContratoFrete.Codigo == codigoContratoFrete);

            return query.ToList();
        }

        public bool AcrescimoDescontoContratoEmAberto(int codigoBaixaTitulo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto>();

            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> queryBaixa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();
            var resultBaixa = from obj in queryBaixa where obj.TituloBaixa.Codigo == codigoBaixaTitulo select obj;

            return query.Where(o => resultBaixa.Where(b => b.Titulo.ContratoFrete.Codigo == o.ContratoFrete.Codigo).Any() && o.Situacao != SituacaoContratoFreteAcrescimoDesconto.Cancelado &&
                               o.Situacao != SituacaoContratoFreteAcrescimoDesconto.Rejeitado && o.Situacao != SituacaoContratoFreteAcrescimoDesconto.Finalizado).Any();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> ConsultarParaCIOT(int ciot, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = ConsultarParaCIOT(ciot);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsultaParaCIOT(int ciot)
        {
            var result = ConsultarParaCIOT(ciot);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> ConsultarParaFechamentoAgregado(int codigoCIOT, TipoJustificativa? tipoJustificativa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarParaFechamentoAgregado(codigoCIOT, tipoJustificativa);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarParaFechamentoAgregado(int codigoCIOT, TipoJustificativa? tipoJustificativa)
        {
            var result = _ConsultarParaFechamentoAgregado(codigoCIOT, tipoJustificativa);

            return result.Count();
        }

        public decimal SomarValorParaFechamentoAgregado(int codigoCIOT, TipoJustificativa? tipoJustificativa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto>();

            var result = from obj in query where obj.DisponibilizarFechamentoDeAgregado select obj;

            // Filtros
            if (codigoCIOT > 0)
                result = result.Where(obj => obj.CIOT.Codigo == codigoCIOT);

            if (tipoJustificativa.HasValue)
                result = result.Where(obj => obj.Justificativa.TipoJustificativa == tipoJustificativa);

            if (result.Count() > 0)
                return result.Sum(obj => obj.Valor);
            else
                return 0;
        }

        public decimal SomarValorParaFechamentoAgregadoPorCarga(int codigoCarga, TipoJustificativa? tipoJustificativa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto>();

            var result = from obj in query where obj.DisponibilizarFechamentoDeAgregado select obj;

            // Filtros
            if (codigoCarga > 0)
                result = result.Where(obj => obj.ContratoFrete.Carga.Codigo == codigoCarga);

            if (tipoJustificativa.HasValue)
                result = result.Where(obj => obj.Justificativa.TipoJustificativa == tipoJustificativa);

            if (result.Count() > 0)
                return result.Sum(obj => obj.Valor);
            else
                return 0;
        }


        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> ConsultarParaContratoFrete(int codigoContratoFrete, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto>();

            var result = from obj in query where obj.ContratoFrete.Codigo == codigoContratoFrete && obj.Situacao != SituacaoContratoFreteAcrescimoDesconto.Cancelado && obj.Situacao != SituacaoContratoFreteAcrescimoDesconto.Rejeitado select obj;

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsultaParaContratoFrete(int codigoContratoFrete)
        {
            IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto>();

            var result = from obj in query where obj.ContratoFrete.Codigo == codigoContratoFrete && obj.Situacao != SituacaoContratoFreteAcrescimoDesconto.Cancelado && obj.Situacao != SituacaoContratoFreteAcrescimoDesconto.Rejeitado select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> BuscarPorCIOT(int codigoCIOT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto>();
            query = query.Where(obj => obj.CIOT.Codigo == codigoCIOT);
            return query.ToList();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> Consultar(Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaContratoFreteAcrescimoDesconto filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.ContratoFrete > 0)
                result = result.Where(obj => obj.ContratoFrete.Codigo == filtrosPesquisa.ContratoFrete);

            if (filtrosPesquisa.Justificativa > 0)
                result = result.Where(obj => obj.Justificativa.Codigo == filtrosPesquisa.Justificativa);

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteAcrescimoDesconto.Todos)
                result = result.Where(obj => obj.Situacao == filtrosPesquisa.Situacao);

            if (filtrosPesquisa.CodigosCarga != null && filtrosPesquisa.CodigosCarga.Count > 0)
                result = result.Where(o => filtrosPesquisa.CodigosCarga.Contains(o.ContratoFrete.Carga.Codigo));

            if (filtrosPesquisa.NomeSubcontratado != null && filtrosPesquisa.NomeSubcontratado.Count > 0)
                result = result.Where(o => filtrosPesquisa.NomeSubcontratado.Contains(o.ContratoFrete.TransportadorTerceiro.CPF_CNPJ));

            if (filtrosPesquisa.NomeMotorista != null && filtrosPesquisa.NomeMotorista.Count > 0)
                result = result.Where(o => filtrosPesquisa.NomeMotorista.Contains(o.ContratoFrete.Carga.CodigoPrimeiroMotorista));
            
            if (filtrosPesquisa.CodigosCIOT?.Count > 0)
                result = result.Where(o => o.ContratoFrete.Carga.CargaCIOTs.Any(p => filtrosPesquisa.CodigosCIOT.Contains(p.CIOT.Codigo)));

            if (filtrosPesquisa.TipoJustificativa != TipoJustificativa.Todos)
                result = result.Where(obj => obj.Justificativa.TipoJustificativa == filtrosPesquisa.TipoJustificativa);

            if (filtrosPesquisa.CodigoTransportadorContratoFreteOrigem > 0)
                result = result.Where(obj => obj.ContratoFreteOriginal.TransportadorTerceiro.CPF_CNPJ == filtrosPesquisa.CodigoTransportadorContratoFreteOrigem && obj.ContratoFrete == null && obj.DisponibilizarFechamentoDeAgregado);

            return result;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> ConsultarParaCIOT(int ciot)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto>();
            var result = from obj in query where obj.Situacao != SituacaoContratoFreteAcrescimoDesconto.Cancelado && obj.Situacao != SituacaoContratoFreteAcrescimoDesconto.Rejeitado select obj;

            var queryIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao>();
            var resultIntegracao = from obj in queryIntegracao select obj;

            result = result.Where(o => resultIntegracao.Where(i => i.CIOT.Codigo == ciot && i.ContratoFreteAcrescimoDesconto.Codigo == o.Codigo).Any());

            return result;
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ContratoFreteAcrescimoDesconto> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Frete.ContratoFreteAcrescimoDesconto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaRegraICMS = new Frete.ConsultaContratoFreteAcrescimoDesconto().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);



            consultaRegraICMS.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.ContratoFreteAcrescimoDesconto)));



            return consultaRegraICMS.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ContratoFreteAcrescimoDesconto>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ContratoFreteAcrescimoDesconto>> ConsultarRelatorioAsync(Dominio.ObjetosDeValor.Embarcador.Frete.ContratoFreteAcrescimoDesconto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaRegraICMS = new Frete.ConsultaContratoFreteAcrescimoDesconto().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);
            consultaRegraICMS.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.ContratoFreteAcrescimoDesconto)));
            return await consultaRegraICMS.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Fretes.ContratoFreteAcrescimoDesconto>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Frete.ContratoFreteAcrescimoDesconto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var query = new Frete.ConsultaContratoFreteAcrescimoDesconto().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);



            return query.SetTimeout(600).UniqueResult<int>();
        }

        public Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto BuscarPorcCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto>();
            query = query.Where(obj => obj.ContratoFrete.Carga.Codigo == codigoCarga);
            return query.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> _ConsultarParaFechamentoAgregado(int codigoCIOT, TipoJustificativa? tipoJustificativa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto>();

            var result = from obj in query where obj.DisponibilizarFechamentoDeAgregado select obj;

            // Filtros
            if (codigoCIOT > 0)
                result = result.Where(obj => obj.CIOT.Codigo == codigoCIOT);

            if (tipoJustificativa.HasValue)
                result = result.Where(obj => obj.Justificativa.TipoJustificativa == tipoJustificativa);

            return result;
        }

        #endregion
    }
}
