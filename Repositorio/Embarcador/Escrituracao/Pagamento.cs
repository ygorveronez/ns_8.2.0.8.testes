using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Escrituracao
{
    public class Pagamento : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.Pagamento>
    {
        public Pagamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Pagamento(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Escrituracao.Pagamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.Pagamento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento> BuscarPorCodigo(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.Pagamento>();
            query = query.Where(obj => codigos.Contains(obj.Codigo));

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.Pagamento BuscarSeExistePagamentoEmFechamento()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.Pagamento>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.EmFechamento || obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.PendenciaFechamento select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento> BuscarPagamentoEmFechamento(int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.Pagamento>();
            var result = from obj in query
                         where
                             obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.EmFechamento
                             && obj.GerandoMovimentoFinanceiro
                             && obj.UltimaCargaEmCancelamento == null
                         select obj;

            return result
                .Fetch(o => o.Tomador).ThenFetch(o => o.GrupoPessoas)
                .Take(limite).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento> BuscarPagamentosEmIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.Pagamento>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.AguardandoIntegracao select obj;

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento>> BuscarPagamentoAgIntegracaoAsync(CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.Pagamento>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.AguardandoIntegracao select obj;

            return result.ToListAsync(cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento> BuscarPagamentosFalhaIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.Pagamento>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.FalhaIntegracao select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento> BuscarPagamentosAutomaticosAguardandoFechamento(int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.Pagamento>();
            var result = from obj in query
                         where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.EmFechamento && obj.GeradoAutomaticamente == true
                         && obj.GerandoMovimentoFinanceiro == false
                         select obj;

            return result.Take(limite).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento> BuscarSemRegraAprovacaoPagamentoPorCodigos(List<int> codigosPagamento)
        {
            var consultaPagamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.Pagamento>()
                .Where(o => codigosPagamento.Contains(o.Codigo) && o.Situacao == SituacaoPagamento.SemRegraAprovacao);

            return consultaPagamento.ToList();
        }

        public int ObterProximoNumero()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.Pagamento>();

            var result = from obj in query select obj;

            int? retorno = result.Max(o => (int?)o.Numero);
            return retorno.HasValue ? (retorno.Value + 1) : 1;

        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaPagamento filtroPesquisa, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(filtroPesquisa);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Tomador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Filial)
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaPagamento filtroPesquisa)
        {
            var result = _Consultar(filtroPesquisa);
            return result.Count();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Escrituracao.IntegracaoLotePagamento> ConsultarRelatorioIntegracaoLotePagamento(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaIntegracaoLotePagamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaQuantidade = new ConsultaIntegracaoLotePagamento().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaQuantidade.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Escrituracao.IntegracaoLotePagamento)));

            return consultaQuantidade.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Escrituracao.IntegracaoLotePagamento>();
        }

        public int ContarConsultaRelatorioIntegracaoLotePagamento(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaIntegracaoLotePagamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaQuantidade = new ConsultaIntegracaoLotePagamento().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaQuantidade.SetTimeout(600).UniqueResult<int>();
        }

        public IList<int> BuscarCodigosCargasLiberadasPorPagamento(int codigoPagamento)
        {
            var sqlQuery = $"select CAR_CODIGO from T_PAGAMENTO_CARGA_LIBERADA where PAG_CODIGO = {codigoPagamento}"; // SQL-INJECTION-SAFE
            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            return query.SetTimeout(300).List<int>();
        }

        public IList<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.Pagamento> BuscarParaAPIDePagamentos(List<int> codigosCarga)
        {
            StringBuilder sql = new();
            sql.Append($@"SELECT    
                            COALESCE(PAG.CAR_CODIGO, DFA.CAR_CODIGO_PAGAMENTO) AS CodigoCarga, 
                            PAG.PAG_CODIGO AS CodigoPagamento, 
                            PAG_NUMERO AS NumeroPagamento, 
                            PAG_VALOR_PAGAMENTO AS ValorPagamento, 
                            PAG_SITUACAO AS SituacaoPagamentoInt 
                        FROM 
                          T_PAGAMENTO PAG LEFT JOIN T_DOCUMENTO_FATURAMENTO DFA ON PAG.PAG_CODIGO = DFA.PAG_CODIGO"
                        );

            if (!codigosCarga.IsNullOrEmpty())
                sql.Append($@" WHERE 
                                  PAG.CAR_CODIGO IN ({string.Join(", ", codigosCarga)}) 
                                  OR PAG.PAG_CODIGO IN (
                                    SELECT 
                                      DFA.PAG_CODIGO 
                                    FROM 
                                      T_DOCUMENTO_FATURAMENTO DFA 
                                      LEFT JOIN T_CARGA_CTE CCT ON CCT.CON_CODIGO = DFA.CON_CODIGO 
                                    WHERE 
                                      CCT.CAR_CODIGO IN ({string.Join(", ", codigosCarga)}) AND PAG_CODIGO IS NOT NULL)");
            else
                sql.Append($@" WHERE PAG.CAR_CODIGO = 0");

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql.ToString());

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.Pagamento)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.Pagamento>();
        }

        public IList<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.Pagamento> BuscarPorDocumentoParaAPI(List<int> codigosCarga, Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.RequisicaoBuscarDadosPagamento request)
        {
            StringBuilder sql = new();
            sql.Append($@"SELECT DFA.CAR_CODIGO_PAGAMENTO AS CodigoCarga,
                         PAG.PAG_CODIGO AS CodigoPagamento,
                         PAG_NUMERO AS NumeroPagamento,
                         PAG_VALOR_PAGAMENTO AS ValorPagamento,
                         PAG_SITUACAO AS SituacaoPagamentoInt
						 FROM T_PAGAMENTO PAG INNER JOIN T_DOCUMENTO_FATURAMENTO DFA ON PAG.PAG_CODIGO = DFA.PAG_CODIGO
                         LEFT JOIN T_CTE CON ON DFA.CON_CODIGO = CON.CON_CODIGO"
                        );

            sql.Append(@" WHERE 1=1");

            if (!codigosCarga.IsNullOrEmpty())
                sql.Append($@" AND DFA.CAR_CODIGO_PAGAMENTO IN ({string.Join(", ", codigosCarga)})");

            if (request.ProtocoloDocumento.HasValue)
                sql.Append($@" AND CON.CON_CODIGO = {request.ProtocoloDocumento}");
            else if (request.ChaveDocumento != null)
                sql.Append($@" AND CON.CON_CHAVECTE = '{request.ChaveDocumento}'");

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql.ToString());

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.Pagamento)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.Pagamento>();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Escrituracao.Pagamento> _Consultar(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaPagamento filtroPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.Pagamento>();

            var result = from obj in query select obj;

            result = result.Where(o => o.LotePagamentoLiberado == filtroPesquisa.PagamentoLiberado);

            if (filtroPesquisa.DataInicialEmissao != DateTime.MinValue)
                result = result.Where(obj => obj.DocumentosFaturamento.Any(doc => doc.DataEmissao.Date >= filtroPesquisa.DataInicialEmissao));

            if (filtroPesquisa.DataFinalEmissao != DateTime.MinValue)
                result = result.Where(obj => obj.DocumentosFaturamento.Any(doc => doc.DataEmissao.Date <= filtroPesquisa.DataFinalEmissao));

            if (filtroPesquisa.Empresa > 0)
                result = result.Where(o => o.Empresa.Codigo == filtroPesquisa.Empresa);

            if (filtroPesquisa.Carga > 0)
            {
                if (!filtroPesquisa.PagamentoLiberado)
                    result = result.Where(o => o.DocumentosFaturamento.Any(doc => doc.CargaPagamento.Codigo == filtroPesquisa.Carga));
                else
                    result = result.Where(o => o.DocumentosFaturamentoLiberado.Any(doc => doc.CargaPagamento.Codigo == filtroPesquisa.Carga));
            }

            if (filtroPesquisa.NumeroDOC > 0)
            {
                if (!filtroPesquisa.PagamentoLiberado)
                    result = result.Where(o => o.DocumentosFaturamento.Any(doc => doc.Numero == filtroPesquisa.NumeroDOC.ToString()));
                else
                    result = result.Where(o => o.DocumentosFaturamentoLiberado.Any(doc => doc.Numero == filtroPesquisa.NumeroDOC.ToString()));
            }

            if (filtroPesquisa.Ocorrencia > 0)
            {
                if (!filtroPesquisa.PagamentoLiberado)
                    result = result.Where(o => o.DocumentosFaturamento.Any(doc => doc.CargaOcorrenciaPagamento.Codigo == filtroPesquisa.Ocorrencia));
                else
                    result = result.Where(o => o.DocumentosFaturamentoLiberado.Any(doc => doc.CargaOcorrenciaPagamento.Codigo == filtroPesquisa.Ocorrencia));
            }

            if (filtroPesquisa.Filial > 0)
                result = result.Where(o => o.Filial.Codigo == filtroPesquisa.Filial);

            if (filtroPesquisa.Numero > 0)
                result = result.Where(o => o.Numero == filtroPesquisa.Numero);

            if (filtroPesquisa.SituacaoPagamento.HasValue)
                result = result.Where(o => o.Situacao == filtroPesquisa.SituacaoPagamento.Value);

            if (filtroPesquisa.TipoOperacao > 0)
                result = result.Where(o => o.Carga.TipoOperacao.Codigo == filtroPesquisa.TipoOperacao);

            if (filtroPesquisa.Tomador > 0)
                result = result.Where(o => o.Tomador.CPF_CNPJ == filtroPesquisa.Tomador);


            return result;
        }

        #endregion
    }
}
