using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.WebService.Carga;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPedido : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPedido>
    {
        private CancellationToken _cancellationToken;
        public CargaPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaPedido(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos - Agendamento Entrega Pedido

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoPesquisaAgendamentoEntrega> ConsultaAgendamentoEntrega(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoEntrega filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string sqlQuery = $@"SELECT {ObterSelectCamposAgendamentoEntregaPedido(filtrosPesquisa)}
                                 FROM {ObterFromAgendamentoEntregaPedido()}
                                 WHERE {ObterWhereAgendamentoEntregaPedido(filtrosPesquisa)}
                                 ORDER BY JanelaCarregamento.CJC_INICIO_CARREGAMENTO
                                 OFFSET {parametrosConsulta.InicioRegistros} ROWS
                                 FETCH NEXT {parametrosConsulta.LimiteRegistros} ROWS ONLY;";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoPesquisaAgendamentoEntrega)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoPesquisaAgendamentoEntrega>();
        }

        public int ContarConsultaAgendamentoEntrega(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoEntrega filtrosPesquisa)
        {
            string sqlQuery = $@"SELECT COUNT(*) FROM {ObterFromAgendamentoEntregaPedido()} WHERE {ObterWhereAgendamentoEntregaPedido(filtrosPesquisa)}";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        #endregion

        #region Métodos Públicos - Agendamento Entrega Pedido Agrupado

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoPesquisaAgendamentoEntrega> ConsultaAgendamentoEntregaAgrupado(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoEntrega filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string sqlQuery = $@"{ObterCommonTableExpressionAgendamentoEntregaPedidoAgrupado()}
                                 SELECT {ObterSelectCamposAgendamentoEntregaPedidoAgrupado(filtrosPesquisa)}
                                 FROM {ObterFromAgendamentoEntregaPedidoAgrupado()}
                                 WHERE {ObterWhereAgendamentoEntregaPedidoAgrupado(filtrosPesquisa)}
                                 ORDER BY JanelaCarregamento.CJC_INICIO_CARREGAMENTO
                                 OFFSET {parametrosConsulta.InicioRegistros} ROWS
                                 FETCH NEXT {parametrosConsulta.LimiteRegistros} ROWS ONLY;";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoPesquisaAgendamentoEntrega)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoPesquisaAgendamentoEntrega>();
        }

        public int ContarConsultaAgendamentoEntregaAgrupado(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoEntrega filtrosPesquisa)
        {
            string sqlQuery = $@"{ObterCommonTableExpressionAgendamentoEntregaPedidoAgrupado()}
                                 SELECT COUNT(*)
                                 FROM {ObterFromAgendamentoEntregaPedidoAgrupado()}
                                 WHERE {ObterWhereAgendamentoEntregaPedidoAgrupado(filtrosPesquisa)}";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        #endregion

        #region Métodos Públicos - Agendamento Entrega Pedido Consulta

        public int ContarConsultaAgendamentoEntregaPedidoConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoEntregaPedidoConsulta filtrosPesquisa)
        {
            StringBuilder sql = new StringBuilder(ObterSqlContarConsulta());
            sql.Append(ObterFiltrosPesquisa(filtrosPesquisa));

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql.ToString());
            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoPesquisaAgendamentoEntregaConsulta> ConsultaAgendamentoEntregaPedidoConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoEntregaPedidoConsulta filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            StringBuilder sql = new StringBuilder(ObterSqlConsulta());
            sql.Append(ObterSqlFrom());
            sql.Append(ObterFiltrosPesquisa(filtrosPesquisa));
            sql.Append(ObterSqlGroupByOrderBy());

            if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                sql.Append($" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;");

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql.ToString());
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoPesquisaAgendamentoEntregaConsulta)));
            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoPesquisaAgendamentoEntregaConsulta>();
        }

        #endregion

        #region Métodos Privados - Agendamento Entrega Pedido Consulta

        private string ObterSqlConsulta()
        {
            return @"SELECT  CargaPedido.CPE_CODIGO Codigo,
                             Pedido.PED_CODIGO CodigoPedido,
								   SUBSTRING((
										SELECT DISTINCT ', ' + _NotasFiscais.NumeroNotaFiscal
										  FROM (
												   SELECT CAST(_NotaFiscalParcial.CNP_NUMERO AS VARCHAR) NumeroNotaFiscal
													 FROM T_PEDIDO_NOTA_FISCAL_PARCIAL _NotaFiscalParcial
													WHERE _NotaFiscalParcial.PED_CODIGO = Pedido.PED_CODIGO
													UNION
												   SELECT CAST(_NotaFiscal.NF_NUMERO AS VARCHAR) NumeroNotaFiscal
													 FROM T_XML_NOTA_FISCAL _NotaFiscal
													 JOIN T_PEDIDO_XML_NOTA_FISCAL _PedidoXMLNotaFiscal ON _NotaFiscal.NFX_CODIGO = _PedidoXMLNotaFiscal.NFX_CODIGO
													WHERE _PedidoXMLNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO AND _NotaFiscal.NF_ATIVA = 1
											   ) _NotasFiscais
										   FOR XML PATH('')
									), 3, 1000) NotaFiscal,
								   SUBSTRING((SELECT DISTINCT ', ' + CAST(_CTe.CON_NUM as VARCHAR)
														FROM T_CTE _CTe
														JOIN T_CARGA_CTE _CargaCTe ON _CTe.CON_CODIGO = _CargaCTe.CON_CODIGO
                                                       WHERE _CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000) as CTe,
								   TipoOperacao.TOP_DESCRICAO TipoOperacao,
								   Pedido.PED_OBSERVACAO_ADICIONAL ObservacaoAgendamento,
								   Pedido.PED_OBSERVACAO_SOLICITACAO_REAGENDAMENTO ObservacaoReagendamento,
								   Pedido.PED_SITUACAO_AGENDAMENTO_ENTREGA_PEDIDO SituacaoAgendamento,
								   Destinatario.CLI_NOME + ' (' + CONVERT(NVARCHAR(18), CONVERT(BIGINT, Destinatario.CLI_CGCCPF)) +')' Destinatario,
                                   Destinatario.CLI_EXIGE_QUE_ENTREGAS_SEJAM_AGENDADAS AS ExigeAgendamento,
								   Destino.LOC_DESCRICAO Destino,
								   Estado.UF_SIGLA UFDestino,
								   Pedido.PED_DATA_AGENDAMENTO DataAgendamento,
								   Pedido.PED_QUANTIDADE_VOLUMES Volumes,
								   Pedido.PED_CUBAGEM_TOTAL MetrosCubicos,
                                    tiposcargas.TCG_DESCRICAO TipoCarga,
                                   Pedido.PED_PREVISAO_ENTREGA DataPrevisaoEntrega";
        }

        private string ObterSqlGroupByOrderBy()
        {
            return @" GROUP BY Pedido.PED_CODIGO,
											TipoOperacao.TOP_DESCRICAO,
											Pedido.PED_OBSERVACAO_ADICIONAL,
											Pedido.PED_SITUACAO_AGENDAMENTO_ENTREGA_PEDIDO,
											Destinatario.CLI_NOME,
											Destino.LOC_DESCRICAO,
											Estado.UF_SIGLA,
											Pedido.PED_DATA_AGENDAMENTO,
											Pedido.PED_QUANTIDADE_VOLUMES,
											Pedido.PED_CUBAGEM_TOTAL,
											CargaPedido.CAR_CODIGO,
											CargaPedido.CPE_CODIGO,
											Destinatario.CLI_CGCCPF,
                                            Destinatario.CLI_EXIGE_QUE_ENTREGAS_SEJAM_AGENDADAS,
											Pedido.CAR_DATA_CARREGAMENTO_PEDIDO,
                                            Carga.CAR_CODIGO,
                                            Pedido.PED_OBSERVACAO_SOLICITACAO_REAGENDAMENTO,
                                            tiposcargas.TCG_DESCRICAO,
                                            Pedido.PED_PREVISAO_ENTREGA
								   ORDER BY Pedido.CAR_DATA_CARREGAMENTO_PEDIDO";
        }

        private string ObterSqlContarConsulta()
        {
            StringBuilder SQLContarConsulta = new StringBuilder("SELECT COUNT (*) ");
            SQLContarConsulta.Append(ObterSqlFrom());
            return SQLContarConsulta.ToString();
        }

        private string ObterSqlFrom()
        {
            return @" FROM T_CARGA_PEDIDO CargaPedido
								   JOIN T_CARGA Carga on Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO
								   JOIN T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
								   LEFT JOIN T_TIPO_OPERACAO TipoOperacao on Carga.TOP_CODIGO = TipoOperacao.TOP_CODIGO
								   LEFT JOIN T_LOCALIDADES Destino on Pedido.LOC_CODIGO_DESTINO = Destino.LOC_CODIGO
								   LEFT JOIN T_CLIENTE Destinatario on Pedido.CLI_CODIGO = Destinatario.CLI_CGCCPF 
                                   left join T_TIPO_DE_CARGA tiposcargas ON tiposcargas.TCG_CODIGO = carga.TCG_CODIGO
								   LEFT JOIN T_UF Estado on Estado.UF_SIGLA = Destino.UF_SIGLA ";
        }

        private string ObterFiltrosPesquisa(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoEntregaPedidoConsulta filtrosPesquisa)
        {
            string dataPattern = "yyyyMMdd HH:mm:ss";

            StringBuilder where = new StringBuilder(" WHERE Pedido.PED_SITUACAO_AGENDAMENTO_ENTREGA_PEDIDO IS NOT NULL AND Carga.CAR_SITUACAO NOT IN (13, 18)");

            if (filtrosPesquisa.CodigoTransportador > 0)
                where.Append($" AND Carga.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador}");

            if (filtrosPesquisa.DataAgendamentoInicial.HasValue)
                where.Append($" AND Pedido.PED_DATA_AGENDAMENTO >= '{filtrosPesquisa.DataAgendamentoInicial.Value.Date.ToString(dataPattern)}'");

            if (filtrosPesquisa.DataAgendamentoFinal.HasValue)
                where.Append($" AND Pedido.PED_DATA_AGENDAMENTO <= '{filtrosPesquisa.DataAgendamentoFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay).ToString(dataPattern)}'");

            if (filtrosPesquisa.NotaFiscal > 0)
                where.Append($@" AND EXISTS  (
							   SELECT _NotaFiscalParcial.CNP_NUMERO
								 FROM T_PEDIDO_NOTA_FISCAL_PARCIAL _NotaFiscalParcial
								WHERE _NotaFiscalParcial.PED_CODIGO = Pedido.PED_CODIGO
								  AND _NotaFiscalParcial.CNP_NUMERO = {filtrosPesquisa.NotaFiscal}
								UNION
							   SELECT _NotaFiscal.NFX_CODIGO
								 FROM T_XML_NOTA_FISCAL _NotaFiscal
								 JOIN T_PEDIDO_XML_NOTA_FISCAL _PedidoXMLNotaFiscal ON _NotaFiscal.NFX_CODIGO = _PedidoXMLNotaFiscal.NFX_CODIGO
								WHERE _PedidoXMLNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
								  AND _NotaFiscal.NF_NUMERO = {filtrosPesquisa.NotaFiscal}
							)");

            if (filtrosPesquisa.CTe > 0)
                where.Append($@" AND EXISTS (
									 SELECT TOP(1) _CTe.CON_CODIGO
                                       FROM T_CTE _CTe
								       JOIN T_CARGA_CTE _CargaCTe ON _CTe.CON_CODIGO = _CargaCTe.CON_CODIGO
                                      WHERE _CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO
									    AND _CTe.CON_NUM = {filtrosPesquisa.CTe}
                                 )");

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                where.Append($" AND TipoOperacao.TOP_CODIGO = {filtrosPesquisa.CodigoTipoOperacao}");

            if (filtrosPesquisa.Situacao.HasValue)
                where.Append($" AND Pedido.PED_SITUACAO_AGENDAMENTO_ENTREGA_PEDIDO = {(int)filtrosPesquisa.Situacao.Value}");

            if (filtrosPesquisa.CpfCnpjCliente > 0)
                where.Append($" AND Destinatario.CLI_CGCCPF = {filtrosPesquisa.CpfCnpjCliente}");

            if (filtrosPesquisa.CodigoDestino > 0)
                where.Append($" AND Destino.LOC_CODIGO = {filtrosPesquisa.CodigoDestino}");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Estado) && !filtrosPesquisa.Estado.Contains("0"))
                where.Append($" AND Estado.UF_SIGLA = '{filtrosPesquisa.Estado}'");

            return where.ToString();
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPrimeiroPedidoSemNotasDePallet(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            int quantidade = 1000;

            for (int i = 0; i < cargaPedidos.Count; i += quantidade)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = query.Where(o => cargaPedidos.Skip(i).Take(quantidade).Contains(o) && !o.NotasFiscais.Any(nf => nf.XMLNotaFiscal.TipoNotaFiscalIntegrada == TipoNotaFiscalIntegrada.RemessaPallet)).Select(o => o.Pedido).FirstOrDefault();

                if (pedido != null)
                    return pedido;
            }

            return null;
        }

        public int BuscarQuantidadeCargaPedidoPorVeiculoMotorista(int codigoVeiculo, List<int> codigosMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.Carga.SituacaoCarga != SituacaoCarga.Anulada && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Encerrada && obj.Carga.Veiculo.Codigo == codigoVeiculo && obj.Carga.Motoristas.Any(c => codigosMotorista.Contains(c.Codigo)));

            return query.Count();
        }

        public Task<int> BuscarQuantidadeCargaPedidoPorVeiculoMotoristaAsync(int codigoVeiculo, List<int> codigosMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.Carga.SituacaoCarga != SituacaoCarga.Anulada && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Encerrada);

            if (codigoVeiculo > 0)
                query.Where(obj => obj.Carga.Veiculo.Codigo == codigoVeiculo);

            if (codigosMotorista?.Count > 0)
                query.Where(obj => obj.Carga.Motoristas.Any(c => codigosMotorista.Contains(c.Codigo)));

            return query.CountAsync();
        }

        public int BuscarQuantidadeCargaPedidoPorVeiculoMotorista(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.Carga.SituacaoCarga != SituacaoCarga.Anulada && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Encerrada && obj.Carga.Veiculo.Codigo == codigoVeiculo);

            return query.Count();
        }

        public int BuscarQuantidadeCargaPedidoPorVeiculoMotorista(List<int> codigosMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.Carga.SituacaoCarga != SituacaoCarga.Anulada && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Encerrada && obj.Carga.Motoristas.Any(c => codigosMotorista.Contains(c.Codigo)));

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarUltimaCargaPedidoPorVeiculoMotorista(int codigoVeiculo, List<int> codigosMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.Carga.SituacaoCarga != SituacaoCarga.Anulada && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.Veiculo.Codigo == codigoVeiculo && obj.Carga.Motoristas.Any(c => codigosMotorista.Contains(c.Codigo)));

            return query.OrderByDescending(obj => obj.Codigo).FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarUltimaCargaPedidoPorVeiculoMotoristaAsync(int codigoVeiculo, List<int> codigosMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.Carga.SituacaoCarga != SituacaoCarga.Anulada && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada);

            if (codigoVeiculo > 0)
                query.Where(obj => obj.Carga.Veiculo.Codigo == codigoVeiculo);

            if (codigosMotorista?.Count > 0)
                query.Where(obj => obj.Carga.Motoristas.Any(c => codigosMotorista.Contains(c.Codigo)));

            return query.OrderByDescending(obj => obj.Codigo).FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarUltimaCargaPedidoPorVeiculoMotorista(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.Carga.SituacaoCarga != SituacaoCarga.Anulada && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.Veiculo.Codigo == codigoVeiculo);

            return query.OrderByDescending(obj => obj.Codigo).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarUltimaCargaPedidoPorVeiculoMotorista(List<int> codigosMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.Carga.SituacaoCarga != SituacaoCarga.Anulada && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.Motoristas.Any(c => codigosMotorista.Contains(c.Codigo)));

            return query.OrderByDescending(obj => obj.Codigo).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarNaoEncaixadaPorPedido(int pedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.Pedido.Codigo == pedido && !obj.PedidoEncaixado);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCargasEPedido(IList<int> codigosCargas, int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.Pedido.Codigo == codigoPedido && codigosCargas.Contains(obj.Carga.Codigo));

            return query.ToList();
        }

        public Task<DateTime?> BuscarDataBaseCRTPedidoAsync(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Codigo == codigoCargaPedido);

            return query.Select(o => o.Pedido.DataBaseCRT).FirstOrDefaultAsync(CancellationToken);
        }

        public Task<DateTime?> BuscarDataBaseCRTCargaAsync(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.Select(o => o.Pedido.DataBaseCRT).FirstOrDefaultAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega> BuscarCanaisEntregaPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            var result = from obj in query where obj.Pedido.CanalEntrega != null && obj.Carga.Codigo == carga select obj.Pedido.CanalEntrega;

            return result.Distinct().ToList();
        }

        public decimal BuscarValorTotalNotasFiscais(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.Sum(o => (decimal?)o.Pedido.ValorTotalNotasFiscais) ?? 0m;
        }

        public decimal BuscarValorFreteNegociadoCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.Sum(o => (decimal?)o.Pedido.ValorFreteNegociado) ?? 0m;
        }

        public decimal BuscarValorFreteToneladaNegociadoCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.Sum(o => (decimal?)o.Pedido.ValorFreteToneladaNegociado) ?? 0m;
        }

        public List<int> ObterEmpresasCargaFilialEmissora(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == carga && o.CargaOrigem.EmpresaFilialEmissora != null);

            return query.Select(obj => obj.CargaOrigem.EmpresaFilialEmissora.Codigo).Distinct().ToList();
        }

        public DateTime? BuscarPrimeiraPrevisaoEntrega(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.Pedido.PrevisaoEntrega.HasValue);
            return query.Select(o => o.Pedido.PrevisaoEntrega)?.FirstOrDefault() ?? null;
        }

        public Task<DateTime?> BuscarMaiorPrevisaoEntregaAsync(int codigoCarga, double codigoCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.Pedido.Destinatario.CPF_CNPJ == codigoCliente && obj.Pedido.PrevisaoEntrega.HasValue);
            return query.MaxAsync(obj => obj.Pedido.PrevisaoEntrega, CancellationToken) ?? null;
        }

        public DateTime? BuscarMaiorPrevisaoEntrega(int codigoCarga, double codigoCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.Pedido.Destinatario.CPF_CNPJ == codigoCliente && obj.Pedido.PrevisaoEntrega.HasValue);
            return query.Max(obj => obj.Pedido.PrevisaoEntrega) ?? null;
        }

        public bool CargaTipoFeeder(int codigoCarga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga && o.TipoPropostaMultimodal == TipoPropostaMultimodal.Feeder
                );

            return consultaCargaPedido.Any();
        }

        public bool ContemDataPrevisaoInferior(int codigoCarga, DateTime dataReagendamento)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga && o.Pedido.DataPrevisaoSaida.HasValue &&
                    dataReagendamento < o.Pedido.DataPrevisaoSaida
                );

            return consultaCargaPedido.Any();
        }

        public DateTime? BuscarDataPrevisaoChegadaDestinatario(int codigoCarga, double cpfCnpjDestinatario)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga &&
                    (
                        (o.Recebedor != null && o.Recebedor.CPF_CNPJ == cpfCnpjDestinatario) ||
                        (o.Recebedor == null && o.Pedido.Destinatario.CPF_CNPJ == cpfCnpjDestinatario)
                    ) &&
                    o.Pedido.DataPrevisaoChegadaDestinatario.HasValue
                );

            return consultaCargaPedido.Select(o => o.Pedido.DataPrevisaoChegadaDestinatario)?.FirstOrDefault() ?? null;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorNumeroControleSemCodigoXMLNotaFiscal(string numeroControle, int codigoXMLNotaFiscal)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            var result = from obj in query
                         where
                             obj.Pedido.NumeroControle == numeroControle
                             && obj.Pedido.SituacaoPedido != SituacaoPedido.Cancelado
                             && obj.Pedido.SituacaoPedido != SituacaoPedido.Finalizado
                             && !obj.NotasFiscais.Any(x => x.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal)
                         select obj;

            return result.ToList();

        }

        public bool ExistePorNumeroControleECarga(string numeroControle, int codigoCarga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            var result = from obj in query
                         where
                             obj.Pedido.NumeroControle == numeroControle
                             && obj.Carga.Codigo == codigoCarga
                         select obj;

            return result.Count() > 0;
        }

        public Task<bool> ExistePorNumeroControleECargaAsync(string numeroControle, int codigoCarga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            var result = from obj in query
                         where
                             obj.Pedido.NumeroControle == numeroControle
                             && obj.Carga.Codigo == codigoCarga
                         select obj;

            return result.AnyAsync();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorCargaENumeroEXP(int codigoCarga, string numeroEXP)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.Pedido.NumeroEXP == numeroEXP);

            return consultaCargaPedido.FirstOrDefault();
        }

        public bool ExisteNumeroEXPPorCarga(int codigoCarga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.Pedido.NumeroEXP != null && o.Pedido.NumeroEXP != string.Empty);

            return consultaCargaPedido.Count() > 0;
        }

        public bool ExisteOperacaoIntermunicipalNaCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == carga && o.Origem.Codigo != o.Destino.Codigo);

            return query.Any();
        }

        public List<int> ObterEmpresasCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == carga && o.CargaOrigem.Empresa != null);

            return query.Select(obj => obj.CargaOrigem.Empresa.Codigo).Distinct().ToList();
        }

        public List<int> ObterEmpresasCargaOriginal(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.CargaOrigem.Codigo == carga && o.Carga.Empresa != null);

            return query.Select(obj => obj.CargaOrigem.Empresa.Codigo).Distinct().ToList();
        }

        public bool GerarAverbacaoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == codigoCarga);
            return query.Select(o => o.Pedido.NecessitaAverbacaoAutomatica)?.FirstOrDefault() ?? false;
        }

        public bool ContemProvedorOS(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == codigoCarga);
            return query.Select(o => o.Pedido.ProvedorOS != null)?.FirstOrDefault() ?? false;
        }

        public bool ContemProdutoSemNCM(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == codigoCarga);
            return query.Any(o => o.Pedido.Produtos.Any(p => p.Produto == null || p.Produto.CodigoNCM == "" || p.Produto.CodigoNCM == null));
        }

        public bool ContemProdutoEmbarcador(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == codigoCarga);

            var queryProduto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            queryProduto = queryProduto.Where(obj => query.Any(p => p.Pedido == obj.Pedido));

            return queryProduto.Count() > 0;
        }

        public List<string> BuscarNumeroCargaRedespachoNaoCancelada(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga &&
                                     o.CargaPedidoProximoTrecho != null &&
                                     !o.CargaPedidoFilialEmissora &&
                                     o.CargaPedidoProximoTrecho.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada
                                     && o.CargaPedidoProximoTrecho.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada
                                     );

            return query.Select(o => o.CargaPedidoProximoTrecho.Carga.CodigoCargaEmbarcador).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarCargasRedespachoNaoCancelada(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga &&
                                     o.CargaPedidoProximoTrecho != null &&
                                     !o.CargaPedidoFilialEmissora &&
                                     o.CargaPedidoProximoTrecho.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada
                                     && o.CargaPedidoProximoTrecho.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada
                                     );

            return query.Select(o => o.CargaPedidoProximoTrecho.Carga).Distinct().ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> ObterDadosResumidosPorCargas(List<int> cargas)
        {
            return ConsultarDadosResumidosPorCargas(cargas).Select(obj => new Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido()
            {
                Codigo = obj.Codigo,
                CodigoCarga = obj.Carga.Codigo,
                NumeroPedidoEmbarcador = obj.Pedido.NumeroPedidoEmbarcador,
                CodigoPedido = obj.Pedido.Codigo,
                DataPrevisaoEntrega = obj.Pedido.PrevisaoEntrega,
                Ordem = obj.Pedido.Ordem,
                Armador = obj.Pedido.ClienteDonoContainer != null ? new Dominio.ObjetosDeValor.Cliente()
                {
                    Codigo = obj.Pedido.ClienteDonoContainer.CPF_CNPJ,
                    CodigoIntegracao = obj.Pedido.ClienteDonoContainer.CodigoIntegracao,
                    Nome = obj.Pedido.ClienteDonoContainer.Nome,
                    NomeFantasia = obj.Pedido.ClienteDonoContainer.NomeFantasia,
                    PontoTransbordo = obj.Pedido.ClienteDonoContainer.PontoTransbordo,
                    Tipo = obj.Pedido.ClienteDonoContainer.Tipo
                } : null,
                Remetente = obj.Pedido.Remetente != null ? new Dominio.ObjetosDeValor.Cliente()
                {
                    Codigo = obj.Pedido.Remetente.CPF_CNPJ,
                    Nome = obj.Pedido.Remetente.Nome
                } : null,
                Destinatario = obj.Pedido.Destinatario != null ? new Dominio.ObjetosDeValor.Cliente()
                {
                    Codigo = obj.Pedido.Destinatario.CPF_CNPJ,
                    Nome = obj.Pedido.Destinatario.Nome,
                    ClassificacaoPessoaCor = (obj.Pedido.Destinatario.GrupoPessoas != null && obj.Pedido.Destinatario.GrupoPessoas.Classificacao != null) ? obj.Pedido.Destinatario.GrupoPessoas.Classificacao.Cor : string.Empty,
                    Endereco = obj.Pedido.Destinatario.Endereco,
                    Bairro = obj.Pedido.Destinatario.Bairro,
                    Numero = obj.Pedido.Destinatario.Numero
                } : null,
                Origem = obj.Pedido.Origem != null ? new Dominio.ObjetosDeValor.Localidade()
                {
                    Codigo = obj.Pedido.Origem.Codigo,
                    Descricao = obj.Pedido.Origem.Descricao
                } : null,
                Destino = obj.Pedido.Destino != null ? new Dominio.ObjetosDeValor.Localidade()
                {
                    Codigo = obj.Pedido.Destino.Codigo,
                    Descricao = obj.Pedido.Destino.Descricao
                } : null
            }).ToList();
        }

        public int ContarDadosResumidosPorCargas(List<int> cargas)
        {
            return ConsultarDadosResumidosPorCargas(cargas).Count();
        }

        public IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> ConsultarDadosResumidosPorCargas(List<int> cargas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => cargas.Contains(o.Carga.Codigo));

            return query;
        }

        public bool PossuiIntegracaoPedido(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == carga && o.Pedido.IDLoteTrizy > 0);

            return query.Any();
        }

        public bool PossuiIntegracaoDocumentoAnterior(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == carga && o.Carga.TipoOperacao != null && (o.Carga.TipoOperacao.EnviarCTesPorWebService == true || o.Carga.TipoOperacao.EnviarSeguroAverbacaoPorWebService == true) && o.Pedido.ProvedorOS != null && o.Pedido.ProvedorOS.EnviarDocumentacaoCTeAverbacaoSegundaInstancia == true);

            return query.Any();
        }

        public bool PossuiIntegracaoCTeAnterior(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == carga && o.Carga.TipoOperacao != null && o.Carga.TipoOperacao.EnviarCTesPorWebService == true && o.Pedido.ProvedorOS != null && o.Pedido.ProvedorOS.EnviarDocumentacaoCTeAverbacaoSegundaInstancia == true);

            return query.Any();
        }

        public bool PossuiIntegracaoSeguroAnterior(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == carga && o.Carga.TipoOperacao != null && o.Carga.TipoOperacao.EnviarSeguroAverbacaoPorWebService == true && o.Pedido.ProvedorOS != null && o.Pedido.ProvedorOS.EnviarDocumentacaoCTeAverbacaoSegundaInstancia == true);

            return query.Any();
        }

        public bool PedidosNaoPossuiProcImportacao(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            return query.Any(o => o.Carga.Codigo == carga && (o.Pedido.Adicional1 == "" || o.Pedido.Adicional1 == null));
        }
        public async Task<bool> PedidosNaoPossuiProcImportacaoAsync(int carga, CancellationToken cancellation)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            return await query.AnyAsync(o => o.Carga.Codigo == carga && (o.Pedido.Adicional1 == "" || o.Pedido.Adicional1 == null), cancellation);
        }

        public bool ExisteDiferenteSubcontratacaoPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == carga && o.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada);

            return query.Any();
        }

        public bool ExisteCTeEmitidoNoEmbarcador(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == carga && o.CTeEmitidoNoEmbarcador);

            return query.Select(o => o.Codigo).Any();
        }

        public List<Dominio.Entidades.Cliente> BuscarExpedidoresPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.Expedidor != null);

            return consultaCargaPedido.Select(obj => obj.Expedidor).Distinct().ToList();
        }

        public List<Dominio.Entidades.Cliente> BuscarExpedidoresPorCargaOrigem(int codigoCargaOrigem)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.CargaOrigem.Codigo == codigoCargaOrigem && o.Expedidor != null);

            return consultaCargaPedido.Select(obj => obj.Expedidor).Distinct().ToList();
        }

        public bool ExisteNotaFiscalVinculada(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == carga);

            return query.Any();
        }

        public bool ExisteNotaFiscalVinculada(int carga, string chave)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == carga && o.XMLNotaFiscal.Chave == chave);

            return query.Any();
        }

        public bool ExisteInLand(int carga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == carga && o.Pedido.CodigoInLand != null && o.Pedido.CodigoInLand != "");

            return consultaCargaPedido.Any();
        }

        public bool ExistePedidoQueGerouCargaAutomaticamente(int carga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == carga && o.Pedido.AdicionadaManualmente == true && o.Pedido.GerarAutomaticamenteCargaDoPedido == true);

            return consultaCargaPedido.Any();
        }

        public bool ExisteCTeAnteriorVinculada(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == carga);

            return query.Any();
        }

        public bool ExisteCargaDePreCarga(int codigoPedido)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o =>
                    o.Carga.CargaDePreCarga == true &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                    o.Pedido.Codigo == codigoPedido
                );

            return consultaCargaPedido.Count() > 0;
        }

        public List<string> BuscarNumerosPedidos(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == carga);

            return query.Select(obj => obj.Pedido.NumeroPedidoEmbarcador).ToList();
        }

        public List<string> BuscarNumerosCargasPorPedidos(List<int> codigosPedidos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => codigosPedidos.Contains(o.Pedido.Codigo));

            return query.Select(obj => obj.Carga.CodigoCargaEmbarcador).Distinct().ToList();
        }

        public string BuscarNumeroBooking(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == carga);

            return query.Select(obj => obj.Pedido.NumeroBooking).FirstOrDefault();
        }

        public string BuscarNumeroOS(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == carga);

            return query.Select(obj => obj.Pedido.NumeroOS).FirstOrDefault();
        }

        public bool ContainerADefinir(List<int> codigoCargaPedidos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            if (codigoCargaPedidos.Count == 1)
                query = query.Where(o => o.Codigo == codigoCargaPedidos.FirstOrDefault() && o.Pedido.ContainerADefinir == true);
            else
                query = query.Where(o => codigoCargaPedidos.Contains(o.Codigo) && o.Pedido.ContainerADefinir == true);

            query = query.Where(o => o.Pedido.ContainerADefinir == true);

            return query.Any();
        }

        public bool ContainerADefinirCarga(int codigoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoPedido && o.Pedido.ContainerADefinir == true);

            return query.Any();
        }

        public bool ContemPedidoSemContainer(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.Pedido.Container == null);

            return query.Any();
        }

        public bool ExistePorPedido(int codigoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Pedido.Codigo == codigoPedido && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada);

            return query.Any();
        }

        public Task<bool> ExistePorPedidoAsync(int codigoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Pedido.Codigo == codigoPedido && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada);

            return query.AnyAsync(CancellationToken);
        }

        public bool ExistePorPedidoPorProtocolo(int protocoloPedido, bool cargasAtivas, string codigoCargaEmbarcador = "")
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Pedido.Protocolo == protocoloPedido);

            if (cargasAtivas)
                query = query.Where(o => o.Carga.SituacaoCarga != SituacaoCarga.Anulada && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada);

            if (!string.IsNullOrEmpty(codigoCargaEmbarcador))
                query = query.Where(o => o.Carga.CodigoCargaEmbarcador != codigoCargaEmbarcador);

            return query.Any();
        }

        public List<int> BuscarProtocoloPedidoPorProtocoloDeferenteDeCarga(List<int> protocolosPedido, int codigoCarga)
        {
            string sql = $@"select ped.PED_PROTOCOLO as Protocolo from t_carga_pedido carPed
                            left outer join t_pedido ped on carPed.PED_CODIGO = ped.PED_CODIGO
                            inner join T_CARGA carga on carPed.CAR_CODIGO = carga.CAR_CODIGO
                            where ped.PED_PROTOCOLO in ({string.Join(",", protocolosPedido.ToList())}) and (carga.CAR_SITUACAO <> 18 or carga.CAR_SITUACAO is null)
                            and (carga.CAR_SITUACAO <> 13 or carga.CAR_SITUACAO is null) and carga.CAR_CARGA_GERADA_VIA_DOCUMENTO_TRANSPORTE = 1
                            and not exists (select top 1 ped.PED_PROTOCOLO FROM T_CARGA_PEDIDO_RECUSA_CTE recusa where recusa.PED_CODIGO = ped.PED_CODIGO)";

            if (codigoCarga > 0)
                sql += " and carga.CAR_CODIGO = " + codigoCarga;

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Pedido.PedidosProtocolo)));
            var lista = consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidosProtocolo>();

            return lista.Select(x => x.Protocolo).ToList();

        }

        public List<int> BuscarProtocolosPedidosVinculados(List<int> protocolosPedido, int protocoloCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(cargaPedido => protocolosPedido.Contains(cargaPedido.Pedido.Protocolo) &&
                                     cargaPedido.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                                     cargaPedido.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                                     !cargaPedido.Pedido.ReentregaSolicitada &&
                                     cargaPedido.Carga.Codigo != protocoloCarga);

            return query.Select(cargaPedido => cargaPedido.Pedido.Protocolo).ToList();
        }

        public bool ExisteCTeEmitidoNoEmbarcadorPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTeEmitidoNoEmbarcador);

            return query.Any();
        }

        public string BuscarCSTICMSdaCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.Select(o => o.CST).Distinct().Count() > 0 ? string.Join(", ", query.Select(o => o.CST).Distinct().ToList()) : "";
        }

        public decimal BuscarMediaAliquotaICMSdaCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && !o.PedidoSemNFe);

            decimal? retorno = query.Average(o => (decimal?)o.PercentualAliquota);

            return retorno.HasValue ? (retorno.Value) : 0;
        }

        public decimal BuscarMediaAliquotaISSdaCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && !o.PedidoSemNFe);

            decimal? retorno = query.Average(o => (decimal?)o.PercentualAliquotaISS);

            return retorno.HasValue ? (retorno.Value) : 0;
        }

        public Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS BuscarImpostoIBSCBSPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.OutrasAliquotas != null);

            return query.Select(cargaPedido => new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS
            {
                ClassificacaoTributaria = cargaPedido.ClassificacaoTributariaIBSCBS,

                AliquotaIBSEstadual = cargaPedido.AliquotaIBSEstadual,
                PercentualReducaoIBSEstadual = cargaPedido.PercentualReducaoIBSEstadual,

                AliquotaIBSMunicipal = cargaPedido.AliquotaIBSMunicipal,
                PercentualReducaoIBSMunicipal = cargaPedido.PercentualReducaoIBSMunicipal,

                AliquotaCBS = cargaPedido.AliquotaCBS,
                PercentualReducaoCBS = cargaPedido.PercentualReducaoCBS
            }).FirstOrDefault() ?? new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarCargaPendentesDistribuidor()
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.Carga> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();

            var result = query.Where(obj => obj.PendenteGerarCargaDistribuidor &&
                                            ((obj.CargaFechada && !obj.CargaAgrupada) ||
                                             (!obj.CargaFechada && obj.CargaAgrupamento != null && !obj.CargaAgrupamento.CargaDePreCarga)) &&
                                            obj.SituacaoCarga != SituacaoCarga.Cancelada &&
                                            !obj.CargaOrigemPedidos.Any(pe => pe.AgInformarRecebedor) &&
                                            (obj.TipoOperacao == null || (((bool?)obj.TipoOperacao.GerarRedespachoAutomaticamentePorPedidoAposEmissaoDocumentos ?? false) == false &&
                                            ((bool?)obj.TipoOperacao.ConfiguracaoCarga.GerarRedespachoAutomaticamenteAposEmissaoDocumentos ?? false) == false))
                                            ).ToList();
            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarCargaPendentesGeracaoCargaSegundoTechoPorPedido()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>()
                .Where(obj => obj.PendenteGerarCargaDistribuidor &&
                              obj.SituacaoCarga != SituacaoCarga.Cancelada &&
                              obj.SituacaoCarga != SituacaoCarga.Anulada &&
                              (obj.SituacaoCarga == SituacaoCarga.EmTransporte ||
                              obj.SituacaoCarga == SituacaoCarga.AgImpressaoDocumentos ||
                              obj.SituacaoCarga == SituacaoCarga.AgIntegracao) &&
                              ((bool?)obj.TipoOperacao.GerarRedespachoAutomaticamentePorPedidoAposEmissaoDocumentos ?? false))
                .Where(obj => (obj.CargaFechada && !obj.CargaAgrupada) ||
                              (!obj.CargaFechada && (obj.CargaAgrupamento == null || !obj.CargaAgrupamento.CargaDePreCarga)) &&
                              (!obj.CargaOrigemPedidos.Any() || !obj.CargaOrigemPedidos.Any(pe => pe.AgInformarRecebedor)));

            return query.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarCargaPendentesGeracaoCargaSegundoTrechoAposEmissao(int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.Carga> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>()
                .Where(obj => obj.PendenteGerarCargaDistribuidor &&
                              obj.SituacaoCarga != SituacaoCarga.Cancelada &&
                              obj.SituacaoCarga != SituacaoCarga.Anulada &&
                              (obj.SituacaoCarga == SituacaoCarga.EmTransporte ||
                              obj.SituacaoCarga == SituacaoCarga.AgImpressaoDocumentos ||
                              obj.SituacaoCarga == SituacaoCarga.AgIntegracao) &&
                              ((bool?)obj.TipoOperacao.ConfiguracaoCarga.GerarRedespachoAutomaticamenteAposEmissaoDocumentos ?? false))
                .Where(obj => ((obj.CargaFechada && !obj.CargaAgrupada) ||
                              !obj.CargaFechada && (obj.CargaAgrupamento == null || !obj.CargaAgrupamento.CargaDePreCarga)) &&
                              !obj.CargaOrigemPedidos.Any(pe => pe.AgInformarRecebedor));

            return query.Take(limite).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargasPedidoDeColetaPorPedidos(List<int> codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where codigoPedido.Contains(obj.Pedido.Codigo) && obj.Carga.CargaColeta == true select obj;
            return
                result.
                Fetch(obj => obj.Pedido).
                ToList();
        }
        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarCargaPedidoPorProtocolo(int protocoloPedido, int protocoloCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> consultaCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            consultaCarga = consultaCarga.Where(cargaPedido => cargaPedido.Carga.Protocolo == protocoloCarga && cargaPedido.Pedido.Protocolo == protocoloPedido);

            return consultaCarga.FirstOrDefault();
        }

        public void DisponibilizarPedidosComRecebedorParaSeparacao(int codigoCarga)
        {
            string sql = @"
                update Pedido
                   set Pedido.CLI_CODIGO_LOCAL_EXPEDICAO = CargaPedido.CLI_CODIGO_RECEBEDOR,
                       Pedido.PED_DISPONIVEL_PARA_SEPARACAO = 1
                  from T_CARGA_PEDIDO CargaPedido
                  join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                 where CargaPedido.CAR_CODIGO = :codigoCarga
                   and CargaPedido.CLI_CODIGO_RECEBEDOR is not null";

            this.SessionNHiBernate.CreateSQLQuery(sql)
                .SetInt32("codigoCarga", codigoCarga)
                .ExecuteUpdate();
        }

        public void DisponibilizarPedidosEncaixadosComRecebedorParaSeparacao(int codigoCarga)
        {
            string sql = @"
                update Pedido
                   set Pedido.CLI_CODIGO_LOCAL_EXPEDICAO = CargaPedido.CLI_CODIGO_RECEBEDOR,
                       Pedido.PED_DISPONIVEL_PARA_SEPARACAO = 1
                  from T_CARGA_PEDIDO CargaPedido
                  join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                 where CargaPedido.CAR_CODIGO = :codigoCarga
                   and CargaPedido.CLI_CODIGO_RECEBEDOR is not null
                   and CargaPedido.PED_PEDIDO_ENXAIXADO = 1";

            this.SessionNHiBernate.CreateSQLQuery(sql)
                .SetInt32("codigoCarga", codigoCarga)
                .ExecuteUpdate();
        }

        /// <summary>
        /// após confirmar a entrega automaticamente disponibiliza todos os pedidos que não foram entregas no cliente final e controlam separação de pedido para uma nova separação.
        /// </summary>
        /// <param name="codigoEntrega"></param>
        /// <param name="cpfCnpjResponsavelRedespacho"></param>
        public void DisponibilizarPedidosParaNovaSeparacao(int codigoEntrega, double cpfCnpjResponsavelRedespacho)
        {
            UnitOfWork.Sessao.CreateSQLQuery("" +
                @"update pedido
                     set pedido.PED_DISPONIVEL_PARA_SEPARACAO = 1,
                         pedido.CLI_CODIGO_RESPONSAVEL_REDESPACHO = :cpfCnpjResponsavelRedespacho
                    from T_CARGA_ENTREGA_PEDIDO pedidoEntrega 
                   inner join t_carga_pedido cargaPedido on cargaPedido.CPE_CODIGO = pedidoEntrega.CPE_CODIGO 
                   inner join t_pedido pedido on pedido.ped_codigo = cargaPedido.PED_CODIGO 
                   inner join T_CARGA_ENTREGA cargaEntrega on cargaEntrega.CEN_CODIGO = pedidoEntrega.CEN_CODIGO
                   where pedido.PED_CONTROLA_SEPARACAO_PEDIDO = 1
                     and pedidoEntrega.CEN_CODIGO = :codigoEntrega
                     and cargaEntrega.CLI_CODIGO_ENTREGA <> pedido.CLI_CODIGO;"
            )
                .SetInt32("codigoEntrega", codigoEntrega)
                .SetDouble("cpfCnpjResponsavelRedespacho", cpfCnpjResponsavelRedespacho)
                .ExecuteUpdate();
        }

        public void DisponibilizarPedidosParaReentregaComSeparacao(int codigoEntrega)
        {
            string sql = @"
                update Pedido
                   set Pedido.CLI_CODIGO_LOCAL_EXPEDICAO = isnull(CargaPedido.CLI_CODIGO_EXPEDIDOR, Pedido.CLI_CODIGO_REMETENTE),
                       Pedido.PED_DISPONIVEL_PARA_SEPARACAO = 1,
                       Pedido.PED_REENTREGA_SOLICITADA = 1
                  from T_CARGA_ENTREGA_PEDIDO EntregaPedido
                  join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = EntregaPedido.CPE_CODIGO
                  join T_PEDIDO Pedido on Pedido.ped_codigo = CargaPedido.PED_CODIGO
                 where EntregaPedido.CEN_CODIGO = :codigoEntrega";

            UnitOfWork.Sessao.CreateSQLQuery(sql)
                .SetInt32("codigoEntrega", codigoEntrega)
                .ExecuteUpdate();
        }

        public void DisponibilizarPedidosParaSeparacao(int codigoCarga)
        {
            string sql = @"
                update Pedido
                   set Pedido.PED_DISPONIVEL_PARA_SEPARACAO = 1,
                       Pedido.PED_CONTROLA_SEPARACAO_PEDIDO = 1
                  from T_CARGA_PEDIDO CargaPedido
                  join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                 where CargaPedido.CAR_CODIGO = :codigoCarga";

            this.SessionNHiBernate.CreateSQLQuery(sql)
                .SetInt32("codigoCarga", codigoCarga)
                .ExecuteUpdate();
        }

        public int SetarTipoRatioDocumentosPorCarga(int carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos tipoEmissaoCTeDocumentos)
        {
            string hql = "update CargaPedido set TipoRateio = :tipoEmissaoCTeDocumentos where Codigo in (select obj.Codigo from CargaPedido obj where obj.Carga.Codigo = :Carga)";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Carga", carga);
            query.SetEnum("tipoEmissaoCTeDocumentos", tipoEmissaoCTeDocumentos);
            return query.ExecuteUpdate();
        }

        public int SetarOrdemColeta(int carga, int ordem, double cnpj)
        {
            //UnitOfWork.Sessao.CreateQuery("DELETE FROM RateioDespesaVeiculoLancamentoDia WHERE Codigo IN (SELECT c.Codigo FROM RateioDespesaVeiculoLancamentoDia c WHERE c.Lancamento.RateioDespesa.Codigo = :codigoRateioDespesaVeiculo)").SetInt64("codigoRateioDespesaVeiculo", codigoRateioDespesaVeiculo).ExecuteUpdate();
            string hql = "update CargaPedido set OrdemColeta = :Ordem where Codigo in (select obj.Codigo from CargaPedido obj where obj.Carga.Codigo = :Carga and (obj.Expedidor.CPF_CNPJ = :CNPJ or obj.Pedido.Remetente.CPF_CNPJ = :CNPJ))";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Ordem", ordem);
            query.SetInt32("Carga", carga);
            query.SetDouble("CNPJ", cnpj);
            return query.ExecuteUpdate();
        }
        public async Task<int> SetarOrdemColetaAsync(int carga, int ordem, double cnpj)
        {
            string hql = "update CargaPedido set OrdemColeta = :Ordem where Codigo in (select obj.Codigo from CargaPedido obj where obj.Carga.Codigo = :Carga and (obj.Expedidor.CPF_CNPJ = :CNPJ or obj.Pedido.Remetente.CPF_CNPJ = :CNPJ))";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Ordem", ordem);
            query.SetInt32("Carga", carga);
            query.SetDouble("CNPJ", cnpj);
            return await query.ExecuteUpdateAsync();
        }


        public int SetarOrdemColetaOutroendereco(int carga, int ordem, int outroEndereco)
        {
            string hql = "update cargaPedido set cargaPedido.PED_ORDEM_COLETA = " + ordem + " from t_carga_pedido cargaPedido inner join t_pedido pedido on pedido.PED_CODIGO = cargaPedido.PED_CODIGO left join T_PEDIDO_ENDERECO pedidoEndereco on  pedidoEndereco.PEN_CODIGO = pedido.PEN_CODIGO_ORIGEM where cargaPedido.CAR_CODIGO = " + carga + " and pedidoEndereco.COE_CODIGO = " + outroEndereco; // SQL-INJECTION-SAFE
            var query = this.SessionNHiBernate.CreateSQLQuery(hql);

            return query.ExecuteUpdate();
        }
        public async Task<int> SetarOrdemColetaOutroenderecoAsync(int carga, int ordem, int outroEndereco)
        {
            string hql = "update cargaPedido set cargaPedido.PED_ORDEM_COLETA = " + ordem + " from t_carga_pedido cargaPedido inner join t_pedido pedido on pedido.PED_CODIGO = cargaPedido.PED_CODIGO left join T_PEDIDO_ENDERECO pedidoEndereco on  pedidoEndereco.PEN_CODIGO = pedido.PEN_CODIGO_ORIGEM where cargaPedido.CAR_CODIGO = " + carga + " and pedidoEndereco.COE_CODIGO = " + outroEndereco; // SQL-INJECTION-SAFE
            var query = this.SessionNHiBernate.CreateSQLQuery(hql);

            return await query.ExecuteUpdateAsync();
        }

        public int removerOrdemColeta(int carga)
        {
            string hql = "update CargaPedido set OrdemColeta = 0 where Codigo in (select obj.Codigo from CargaPedido obj where obj.Carga.Codigo = :Carga)";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Carga", carga);
            return query.ExecuteUpdate();
        }

        //public int removerRecebedorConsolidado(int carga)
        //{
        //    string hql = "update CargaPedido set Recebedor = null where Codigo in (select obj.Codigo from CargaPedido obj where obj.Carga.Codigo = :Carga)";
        //    var query = this.SessionNHiBernate.CreateQuery(hql);
        //    query.SetInt32("Carga", carga);
        //    return query.ExecuteUpdate();
        //}

        public int SetarCargaOrigem(int carga, int cargaOrigem)
        {
            string hql = "update CargaPedido set Carga = :Carga where CargaOrigem = :CargaOrigem;";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("CargaOrigem", cargaOrigem);
            query.SetInt32("Carga", carga);
            return query.ExecuteUpdate();
        }

        public int SetarCienciaDoEnvioInformado(int carga)
        {
            string hql = "update CargaPedido set CienciaDoEnvioDaNotaInformado = :CienciaDoEnvioDaNotaInformado where Carga = :Carga;";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetBoolean("CienciaDoEnvioDaNotaInformado", true);
            query.SetInt32("Carga", carga);
            return query.ExecuteUpdate();
        }

        public int SetarOrdemEntrega(int carga, int ordem, double cnpj)
        {
            string hql = "update CargaPedido set OrdemEntrega = :Ordem where Codigo in (select obj.Codigo from CargaPedido obj where obj.Carga.Codigo = :Carga and (obj.Recebedor.CPF_CNPJ = :CNPJ or obj.Pedido.Destinatario.CPF_CNPJ = :CNPJ))";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Ordem", ordem);
            query.SetInt32("Carga", carga);
            query.SetDouble("CNPJ", cnpj);
            return query.ExecuteUpdate();
        }
        public async Task<int> SetarOrdemEntregaAsync(int carga, int ordem, double cnpj)
        {
            string hql = "update CargaPedido set OrdemEntrega = :Ordem where Codigo in (select obj.Codigo from CargaPedido obj where obj.Carga.Codigo = :Carga and (obj.Recebedor.CPF_CNPJ = :CNPJ or obj.Pedido.Destinatario.CPF_CNPJ = :CNPJ))";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Ordem", ordem);
            query.SetInt32("Carga", carga);
            query.SetDouble("CNPJ", cnpj);
            return await query.ExecuteUpdateAsync();
        }

        public int SetarOrdemEntregaPorEndereco(int carga, int ordem, double cnpj, int codigoDestino)
        {
            string hql = "update CargaPedido set OrdemEntrega = :Ordem where Codigo in (select obj.Codigo from CargaPedido obj where obj.Carga.Codigo = :Carga and (obj.Recebedor.CPF_CNPJ = :CNPJ or obj.Pedido.Destinatario.CPF_CNPJ = :CNPJ) and obj.Destino.Codigo = :CodigoDestino)";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Ordem", ordem);
            query.SetInt32("Carga", carga);
            query.SetDouble("CNPJ", cnpj);
            query.SetInt32("CodigoDestino", codigoDestino);
            return query.ExecuteUpdate();
        }

        public async Task<int> SetarOrdemEntregaPorEnderecoAsync(int carga, int ordem, double cnpj, int codigoDestino)
        {
            string hql = "update CargaPedido set OrdemEntrega = :Ordem where Codigo in (select obj.Codigo from CargaPedido obj where obj.Carga.Codigo = :Carga and (obj.Recebedor.CPF_CNPJ = :CNPJ or obj.Pedido.Destinatario.CPF_CNPJ = :CNPJ) and obj.Destino.Codigo = :CodigoDestino)";

            var query = this.SessionNHiBernate.CreateQuery(hql);

            query.SetInt32("Ordem", ordem);
            query.SetInt32("Carga", carga);
            query.SetDouble("CNPJ", cnpj);
            query.SetInt32("CodigoDestino", codigoDestino);

            return await query.ExecuteUpdateAsync();
        }

        public int removerStageRelevanteCargaPedido(int CodigoStage)
        {
            string hql = "update CargaPedido set StageRelevanteCusto = null where StageRelevanteCusto = :stage";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("stage", CodigoStage);
            return query.ExecuteUpdate();
        }

        public int SetarOrdemEntrega(int carga, int ordem, double cnpj, DateTime? dataPrevisao, DateTime? dataInicioJanelaDescarga, DateTime? dataFimJanelaDescarga)
        {
            string hqlDataInicioJanela = dataInicioJanelaDescarga.HasValue ? ", InicioJanelaDescarga = :InicioJanelaDescarga " : "";
            string hqlDataFimJanela = dataFimJanelaDescarga.HasValue ? ", FimJanelaDescarga = :FimJanelaDescarga " : "";
            string hqlData = dataPrevisao.HasValue ? ", PrevisaoEntrega = :PrevisaoEntrega " : "";

            string hql = "update CargaPedido set OrdemEntrega = :Ordem " + hqlData + hqlDataInicioJanela + hqlDataFimJanela + "where Codigo in (select obj.Codigo from CargaPedido obj where obj.Carga.Codigo = :Carga and (obj.Recebedor.CPF_CNPJ = :CNPJ or obj.Pedido.Destinatario.CPF_CNPJ = :CNPJ))"; // SQL-INJECTION-SAFE
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Ordem", ordem);
            query.SetInt32("Carga", carga);
            query.SetDouble("CNPJ", cnpj);
            if (dataPrevisao.HasValue)
                query.SetDateTime("PrevisaoEntrega", dataPrevisao.Value);
            if (dataInicioJanelaDescarga.HasValue)
                query.SetDateTime("InicioJanelaDescarga", dataInicioJanelaDescarga.Value);
            if (dataFimJanelaDescarga.HasValue)
                query.SetDateTime("FimJanelaDescarga", dataFimJanelaDescarga.Value);

            return query.ExecuteUpdate();
        }

        public int SetarOrdemEntregaOutroEndereco(int carga, int ordem, int outroEndereco)
        {
            string hql = "update cargaPedido set cargaPedido.PED_ORDEM_ENTREGA = " + ordem + " from t_carga_pedido cargaPedido inner join t_pedido pedido on pedido.PED_CODIGO = cargaPedido.PED_CODIGO left join T_PEDIDO_ENDERECO pedidoEndereco on  pedidoEndereco.PEN_CODIGO = pedido.PEN_CODIGO_DESTINO where cargaPedido.CAR_CODIGO = " + carga + " and pedidoEndereco.COE_CODIGO = " + outroEndereco; // SQL-INJECTION-SAFE
            var query = this.SessionNHiBernate.CreateSQLQuery(hql);

            return query.ExecuteUpdate();
        }
        public async Task<int> SetarOrdemEntregaOutroEnderecoAsync(int carga, int ordem, int outroEndereco)
        {
            string hql = "update cargaPedido set cargaPedido.PED_ORDEM_ENTREGA = " + ordem + " from t_carga_pedido cargaPedido inner join t_pedido pedido on pedido.PED_CODIGO = cargaPedido.PED_CODIGO left join T_PEDIDO_ENDERECO pedidoEndereco on  pedidoEndereco.PEN_CODIGO = pedido.PEN_CODIGO_DESTINO where cargaPedido.CAR_CODIGO = " + carga + " and pedidoEndereco.COE_CODIGO = " + outroEndereco; // SQL-INJECTION-SAFE
            var query = this.SessionNHiBernate.CreateSQLQuery(hql);

            return await query.ExecuteUpdateAsync();
        }

        public int RemoverOrdemEntrega(int carga)
        {
            string hql = "update CargaPedido set OrdemEntrega = 0 where Codigo in (select obj.Codigo from CargaPedido obj where obj.Carga.Codigo = :Carga)";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Carga", carga);
            return query.ExecuteUpdate();
        }

        public int RemoverReferenciasPedidoTrecho(int CodigoCargaPedido)
        {
            string hql = "update t_carga_pedido set PED_CODIGO_TRECHO_ANTERIOR = null where PED_CODIGO_TRECHO_ANTERIOR = " + CodigoCargaPedido; // SQL-INJECTION-SAFE
            var query = this.SessionNHiBernate.CreateSQLQuery(hql);
            query.ExecuteUpdate();

            string hql2 = "update t_carga_pedido set PED_CODIGO_PROXIMO_TRECHO = null where PED_CODIGO_PROXIMO_TRECHO = " + CodigoCargaPedido; // SQL-INJECTION-SAFE
            var query2 = this.SessionNHiBernate.CreateSQLQuery(hql2);

            return query2.ExecuteUpdate();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarEntregasFinalizadas(Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.DataChegada != null && obj.DataSaida != null select obj;

            return ObterLista(result, parametrosConsulta);
        }

        public int ContarEntregasFinalizadas(Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.DataChegada != null && obj.DataSaida != null select obj;

            return result.Count();
        }

        public string ContemCargaEmAberto(string container, int codigoViagem, int codigoPortoOrigem, TipoPropostaMultimodal tipoPropostaMultimodal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query
                         where obj.Pedido.Container.Numero == container
                         && obj.Pedido.PedidoViagemNavio.Codigo == codigoViagem
                         && obj.Pedido.Porto.Codigo == codigoPortoOrigem
                         && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada
                         && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada
                         && obj.TipoPropostaMultimodal == tipoPropostaMultimodal
                         select obj;

            if (result.Any())
                return result.FirstOrDefault()?.Pedido?.NumeroBooking ?? "";
            else
                return string.Empty;
        }

        public bool ContemPedidoEmCargaEmAberto(int pedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Pedido.Codigo == pedido && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada select obj;

            return result.Any();
        }

        public bool ContemPedidoEmCarga(int pedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Pedido.Codigo == pedido select obj;

            return result.Any();
        }

        public bool ContemDestinatariosParaBloquear(int carga, List<double> cnpjDestinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == carga && cnpjDestinatario.Contains(obj.Pedido.Destinatario.CPF_CNPJ) select obj;

            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarCargasTrechoAnterior(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query
                         where obj.Carga.Codigo == carga && obj.Carga.SituacaoCarga == SituacaoCarga.CalculoFrete
       && !obj.CargaPedidoTrechoAnterior.Carga.DataEnvioUltimaNFe.HasValue
       && !obj.CargaPedidoTrechoAnterior.Carga.CalculandoFrete
       && obj.CargaPedidoTrechoAnterior.CargaPedidoFilialEmissora
                         select obj.CargaPedidoTrechoAnterior.Carga;
            return result.Distinct().ToList();
        }

        public bool VerificarCargasPendentesDistribuidor(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            //var result = from obj in query where obj.PendenteGerarCargaDistribuidor && obj.Carga.CargaFechada && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && !obj.AgInformarRecebedor select obj.Carga;
            var result = (from obj in query where obj.PendenteGerarCargaDistribuidor && obj.Carga.Codigo == carga select obj);
            return result.Any();
        }

        public void SetarPedidoEncaixeParaNulo(int carga)
        {
            string hql = "update CargaPedido cargaPedido set cargaPedido.CargaPedidoEncaixe = null, cargaPedido.ValorFreteAntesAlteracaoManual = 0, cargaPedido.ValorFreteFilialEmissoraAntesAlteracaoManual= 0 , cargaPedido.ValorFreteResidual = 0 where cargaPedido.Carga= :Carga ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Carga", carga);
            query.ExecuteUpdate();
        }

        public void ZerarValorFreteNegociado(int carga)
        {
            string hql = "UPDATE Pedido SET ValorFreteNegociado = 0 WHERE Codigo IN (SELECT c.Pedido.Codigo FROM CargaPedido c WHERE c.Carga.Codigo = :codigoCarga)";

            IQuery query = this.SessionNHiBernate.CreateQuery(hql);

            query.SetInt32("codigoCarga", carga);
            query.ExecuteUpdate();
        }

        public void ZerarValorFreteSemNota(int carga)
        {
            string hql = @"update CargaPedido cargaPedido set cargaPedido.ValorFrete = 0, cargaPedido.ValorFreteAPagar = 0, cargaPedido.ValorISS = 0, cargaPedido.ValorICMS = 0, cargaPedido.ValorICMSIncluso = 0, cargaPedido.ValorFreteResidual = 0, 
                                                            cargaPedido.ValorRetencaoISS = 0, cargaPedido.ValorFreteFilialEmissora = 0, cargaPedido.ValorICMSFilialEmissora = 0, cargaPedido.ValorFreteAPagarFilialEmissora = 0, 
                                                            cargaPedido.AliquotaIR = 0, cargaPedido.BaseCalculoIR = 0, cargaPedido.ValorIR = 0, 
                                                            cargaPedido.BaseCalculoIBSCBS = 0, cargaPedido.ValorIBSEstadual = 0, cargaPedido.ValorIBSMunicipal = 0, cargaPedido.ValorCBS = 0 
                                                            where cargaPedido.Carga = :Carga and cargaPedido.PedidoSemNFe = :PedidoSemNFe";

            IQuery query = this.SessionNHiBernate.CreateQuery(hql);

            query.SetInt32("Carga", carga);
            query.SetBoolean("PedidoSemNFe", true);
            query.ExecuteUpdate();
        }

        public void SetarAguardandoNotas(int carga)
        {
            string hql = "update CargaPedido cargaPedido set cargaPedido.SituacaoEmissao = :SituacaoEmissao where cargaPedido.Carga= :Carga ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Carga", carga);
            query.SetEnum("SituacaoEmissao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.AgEnvioNF);
            query.ExecuteUpdate();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarUltimaEntregaCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == carga);
            return query.OrderByDescending(obj => obj.OrdemEntrega).FirstOrDefault();
        }

        public List<int> BuscarCodigosOrigem(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == carga && obj.Origem != null);
            return query.Select(obj => obj.Origem.Codigo).Distinct().ToList();
        }

        public List<int> BuscarCodigosDestino(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == carga && obj.Destino != null);
            return query.Select(obj => obj.Destino.Codigo).Distinct().ToList();
        }

        public List<double> BuscarCNPJsClienteOrigem(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == carga && obj.Pedido.Remetente != null);
            return query.Select(obj => obj.Pedido.Remetente.CPF_CNPJ).Distinct().ToList();
        }

        public List<(string, double)> BuscarNomeECNPJsClienteOrigem(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == carga && obj.Pedido.Remetente != null);
            var result = query
                .Select(obj => new { obj.Pedido.Remetente.Nome, obj.Pedido.Remetente.CPF_CNPJ })
                .Distinct()
                .ToList();

            return result.Select(obj => (obj.Nome, obj.CPF_CNPJ)).ToList();
        }

        public List<double> BuscarCNPJsClienteDestino(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == carga && obj.Pedido.Destinatario != null);
            return query.Select(obj => obj.Pedido.Destinatario.CPF_CNPJ).Distinct().ToList();
        }

        public List<string> BuscarUFsOrigem(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == carga && obj.Origem != null);
            return query.Select(obj => obj.Origem.Estado.Sigla).Distinct().ToList();
        }

        public List<string> BuscarUFsDestino(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == carga && obj.Destino != null);
            return query.Select(obj => obj.Destino.Estado.Sigla).Distinct().ToList();
        }

        public List<int> BuscarCodigosRegiaoOrigem(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == carga && obj.Origem != null && obj.Origem.Regiao != null);
            return query.Select(obj => obj.Origem.Regiao.Codigo).Distinct().ToList();
        }

        public List<int> BuscarCodigosRegiaoDestino(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == carga && obj.Destino != null && obj.Destino.Regiao != null);
            return query.Select(obj => obj.Destino.Regiao.Codigo).Distinct().ToList();
        }

        public List<int> BuscarCodigosRotaFrete(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == carga && obj.Pedido.RotaFrete != null);
            return query.Select(obj => obj.Pedido.RotaFrete.Codigo).Distinct().ToList();
        }

        public List<string> BuscarCEPsClienteOrigem(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == carga && obj.Pedido.Remetente != null);
            return query.Select(obj => obj.Pedido.Remetente.CEP).Distinct().ToList();
        }

        public List<string> BuscarCEPsClienteDestino(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == carga && obj.Pedido.Destinatario != null);
            return query.Select(obj => obj.Pedido.Destinatario.CEP).Distinct().ToList();
        }

        public List<string> BuscarProvedorPedidoPorPagamentoProvedor(int codigoPagamentoProvedor)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga> queryPagamentoProvedorCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga>();
            IQueryable<int> cargas = queryPagamentoProvedorCarga.Where(obj => obj.PagamentoProvedor.Codigo == codigoPagamentoProvedor).Select(obj => obj.Carga.Codigo);
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            IQueryable<string> provedor = query.Where(cp => cargas.Contains(cp.Carga.Codigo)).Select(cp => cp.Pedido.ProvedorOS.Nome);

            return provedor.ToList();
        }

        public List<int> BuscarCodigosPaisOrigem(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == carga && obj.Origem != null && obj.Origem.Pais != null);
            return query.Select(obj => obj.Origem.Pais.Codigo).Distinct().ToList();
        }

        public List<int> BuscarCodigosPaisDestino(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == carga && obj.Destino != null && obj.Destino.Pais != null);
            return query.Select(obj => obj.Destino.Pais.Codigo).Distinct().ToList();
        }

        public List<int> BuscarCodigosCentroResultado(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == carga && obj.Pedido.CentroResultado != null);
            return query.Select(obj => obj.Pedido.CentroResultado.Codigo).Distinct().ToList();
        }
        public List<int> BuscarCodigosModeloVeicularCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == carga && obj.Pedido.ModeloVeicularCarga != null);
            return query.Select(obj => obj.Pedido.ModeloVeicularCarga.Codigo).Distinct().ToList();
        }
        public List<int> BuscarCodigosNivelCooperado(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.Carga.Codigo == carga && obj.Carga.Veiculo.Proprietario.Modalidades.Any(x => x.ModalidadesTransportadora.Any(p => p.TipoTerceiro != null)));

            return query.Select(obj => obj.Carga.Veiculo.Proprietario.Modalidades.Select(x => x.ModalidadesTransportadora.Select(d => d.TipoTerceiro.Codigo).FirstOrDefault()).FirstOrDefault()).ToList();
        }
        public int BuscarCargasPeriodoLiberacaoGR(DateTime dataInicial, DateTime dataFinal, Dominio.Entidades.Veiculo veiculo)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesPermitidas = new List<SituacaoCarga>()
            {
                 SituacaoCarga.Encerrada,
                 SituacaoCarga.EmTransporte,
            };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();
            var result = from obj in query
                         where obj.DataCriacaoCarga >= dataInicial && obj.DataCriacaoCarga < dataFinal && obj.Veiculo.Codigo == veiculo.Codigo && situacoesPermitidas.Contains(obj.SituacaoCarga)
                         select obj.DataCriacaoCarga;

            return result.Distinct().Count();
        }
        public int BuscarCargasPeriodoLiberacaoGRMotorista(DateTime dataInicial, DateTime dataFinal, Dominio.Entidades.Usuario motorista)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesPermitidas = new List<SituacaoCarga>()
            {
                 SituacaoCarga.Encerrada,
                 SituacaoCarga.EmTransporte,
            };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();
            var result = from obj in query
                         where obj.DataCriacaoCarga >= dataInicial && obj.DataCriacaoCarga < dataFinal && obj.Motoristas.Any(mot => mot.Codigo == motorista.Codigo) && situacoesPermitidas.Contains(obj.SituacaoCarga)
                         select obj.DataCriacaoCarga;

            return result.Distinct().Count();
        }
        public List<string> BuscarUFsDestinoPorLocalColeta(int carga, int localColeta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == carga && obj.Origem.Codigo == localColeta);
            return query.Select(obj => obj.Destino.Estado.Sigla).Distinct().ToList();
        }

        public List<Dominio.Entidades.Localidade> BuscarLocalidadesEmissaoNFsPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.PossuiNFS && obj.Carga.Codigo == carga select obj.Destino;

            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidosPendentesDistribuidor(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.PendenteGerarCargaDistribuidor && obj.CargaOrigem.Codigo == carga && !obj.AgInformarRecebedor && obj.Recebedor != null select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidosPendentesDistribuidor(List<int> codigosCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.PendenteGerarCargaDistribuidor && codigosCarga.Contains(obj.CargaOrigem.Codigo) && !obj.AgInformarRecebedor && obj.Recebedor != null select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidosPendentesGeracaoCargaSegundoTechoPorPedido(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.PendenteGerarCargaDistribuidor && obj.CargaOrigem.Codigo == carga && !obj.AgInformarRecebedor && obj.Recebedor != null select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCarregamento(int carregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Carregamento.Codigo == carregamento select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCarregamentos(List<int> carregamentos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(obj => carregamentos.Contains(obj.Carga.Carregamento.Codigo));

            return query
                .Fetch(obj => obj.Carga)
                .Fetch(obj => obj.Pedido)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCodigoDiferenteECarga(List<int> codigos, int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            var result = from obj in query where obj.Carga.Codigo == carga && !codigos.Contains(obj.Pedido.Codigo) select obj;

            return result
                .ToList();
        }

        public bool BuscarPorCargaENumerPedido(int carga, string numeroPedidoEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == carga && obj.Pedido.NumeroPedidoEmbarcador == numeroPedidoEmbarcador select obj;
            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorCargaUnica(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;
            return result.FirstOrDefault();
        }

        public int ContarCargasPedidoPorNumeroBooking(string numeroBooking)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Pedido.NumeroBooking == numeroBooking select obj;
            return result.Count();
        }

        public bool ExistePorNumeroBooking(string numeroBooking)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Pedido.NumeroBooking == numeroBooking && obj.Pedido.SituacaoPedido != SituacaoPedido.Cancelado select obj;
            return result.Any();
        }

        public bool ExistePorNumeroBookingContainer(string numeroBooking, string numeroContainer)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesPermitidas = new List<SituacaoCarga>()
            {
                 SituacaoCarga.Encerrada,
                 SituacaoCarga.EmTransporte,
                 SituacaoCarga.AgNFe,
                 SituacaoCarga.Nova,
            };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => situacoesPermitidas.Contains(obj.Carga.SituacaoCarga) &&
                                        obj.Pedido.NumeroBooking == numeroBooking &&
                                        obj.Pedido.Container.Numero == numeroContainer);
            return query.Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorNumeroCargaNumeroContainer(string numeroCarga, string numeroContainer)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesPermitidas = new List<SituacaoCarga>()
            {
                 SituacaoCarga.Encerrada,
                 SituacaoCarga.EmTransporte,
                 SituacaoCarga.AgIntegracao
            };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => situacoesPermitidas.Contains(obj.Carga.SituacaoCarga) &&
                                        obj.Carga.CodigoCargaEmbarcador == numeroCarga &&
                                        obj.Pedido.Container.Numero == numeroContainer);
            return query.FirstOrDefault();
        }

        public bool ExisteCargaPorBookingAntigo(string numeroBooking, string numeroContainer)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesPermitidas = new List<SituacaoCarga>()
            {
                 SituacaoCarga.Encerrada,
                 SituacaoCarga.EmTransporte,
            };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => situacoesPermitidas.Contains(obj.Carga.SituacaoCarga) &&
                                        obj.Pedido.NumeroBooking == numeroBooking &&
                                        obj.Pedido.Container.Numero == numeroContainer);
            return query.Any();
        }

        public bool ExisteCargaPorBookingNovo(string numeroBooking)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesPermitidas = new List<SituacaoCarga>()
            {
                 SituacaoCarga.Nova,
                 SituacaoCarga.AgNFe,
            };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => situacoesPermitidas.Contains(obj.Carga.SituacaoCarga) &&
                                        obj.Pedido.NumeroBooking == numeroBooking);
            return query.Any();
        }

        public int QtdeCargasPorBookingNovo(string numeroBooking)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesPermitidas = new List<SituacaoCarga>()
            {
                 SituacaoCarga.AgNFe,
                 SituacaoCarga.Nova,
            };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => situacoesPermitidas.Contains(obj.Carga.SituacaoCarga) &&
                                        obj.Pedido.NumeroBooking == numeroBooking);
            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorNumeroBooking(string numeroBooking, List<int> cargasUtilizadas)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesPermitidas = new List<SituacaoCarga>()
            {
                 SituacaoCarga.AgNFe,
            };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => situacoesPermitidas.Contains(obj.Carga.SituacaoCarga) &&
                                        obj.Pedido.NumeroBooking == numeroBooking &&
                                        !cargasUtilizadas.Contains(obj.Carga.Codigo));

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorNumeroBookingNumeroContainer(string numeroBooking, string numeroContainer, List<int> cargasUtilizadas)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesPermitidas = new List<SituacaoCarga>()
            {
                 SituacaoCarga.Encerrada,
                 SituacaoCarga.EmTransporte,
                 SituacaoCarga.AgNFe,
                 SituacaoCarga.Nova,
            };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => situacoesPermitidas.Contains(obj.Carga.SituacaoCarga) &&
                                        obj.Pedido.NumeroBooking == numeroBooking &&
                                        obj.Pedido.Container.Numero == numeroContainer &&
                                        !cargasUtilizadas.Contains(obj.Carga.Codigo));

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargasSVMBookingContainerEncerradaTransporte(string numeroBooking, string numeroContainer)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesPermitidas = new List<SituacaoCarga>()
            {
                 SituacaoCarga.Encerrada,
                 SituacaoCarga.EmTransporte,
                 SituacaoCarga.AgIntegracao,
            };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.Pedido.NumeroBooking == numeroBooking &&
                                        obj.Carga.CargaSVM &&
                                        obj.Pedido.Container.Numero == numeroContainer &&
                                        situacoesPermitidas.Contains(obj.Carga.SituacaoCarga));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargasSVMPorContainerEBooking(string numeroContainer, string numeroBooking)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.Pedido.Container.Numero == numeroContainer && obj.Pedido.NumeroBooking == numeroBooking &&
                                        obj.Carga.CargaSVM == true && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada
                                        && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada);

            return query.ToList();
        }

        public bool ExisteMesmoNumeroBooking(string numeroBooking)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Pedido.CodigoCargaEmbarcador.Equals(numeroBooking) && obj.Pedido.SituacaoPedido != SituacaoPedido.Cancelado select obj;
            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidosPorNumeroBookingOrdenadosDesc(string numeroBooking)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Pedido.NumeroBooking == numeroBooking select obj;
            return result.OrderByDescending(o => o.Codigo).ToList();
        }

        public bool ExistePorNumeroCargaENumeroPedido(string numeroCarga, string numeroPedidoEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.CodigoCargaEmbarcador == numeroCarga && obj.Pedido.NumeroPedidoEmbarcador == numeroPedidoEmbarcador && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada select obj;
            return result.Any();
        }

        public bool ExistePorNumeroPedido(string numeroPedidoEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Pedido.NumeroPedidoEmbarcador == numeroPedidoEmbarcador && obj.Pedido.SituacaoPedido != SituacaoPedido.Cancelado select obj;
            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorNumeroPedidoEmbarcador(string numeroPedidoEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Pedido.NumeroPedidoEmbarcador == numeroPedidoEmbarcador && obj.Pedido.SituacaoPedido != SituacaoPedido.Cancelado select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorEntidadeContainer(int codigoCarga, int codigoContainer, double cnpjRemetente, double cnpjDestinatario, int codigoTipoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Pedido != null && obj.Carga != null && obj.Carga.Codigo == codigoCarga select obj;
            if (codigoContainer > 0)
                result = result.Where(obj => obj.Pedido.Container != null && obj.Pedido.Container.Codigo == codigoContainer);
            else
                result = result.Where(obj => obj.Pedido.Container == null);

            if (cnpjRemetente > 0)
                result = result.Where(obj => obj.Pedido.Remetente != null && obj.Pedido.Remetente.CPF_CNPJ == cnpjRemetente);
            else
                result = result.Where(obj => obj.Pedido.Remetente == null);

            if (cnpjDestinatario > 0)
                result = result.Where(obj => obj.Pedido.Destinatario != null && obj.Pedido.Destinatario.CPF_CNPJ == cnpjDestinatario);
            else
                result = result.Where(obj => obj.Pedido.Destinatario == null);

            if (codigoTipoCarga > 0)
                result = result.Where(obj => obj.Pedido.TipoDeCarga == null || obj.Pedido.TipoDeCarga.Codigo == codigoTipoCarga);

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorEntidadeContainerAsync(int codigoCarga, int codigoContainer, double cnpjRemetente, double cnpjDestinatario, int codigoTipoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Pedido != null && obj.Carga != null && obj.Carga.Codigo == codigoCarga select obj;
            if (codigoContainer > 0)
                result = result.Where(obj => obj.Pedido.Container != null && obj.Pedido.Container.Codigo == codigoContainer);
            else
                result = result.Where(obj => obj.Pedido.Container == null);

            if (cnpjRemetente > 0)
                result = result.Where(obj => obj.Pedido.Remetente != null && obj.Pedido.Remetente.CPF_CNPJ == cnpjRemetente);
            else
                result = result.Where(obj => obj.Pedido.Remetente == null);

            if (cnpjDestinatario > 0)
                result = result.Where(obj => obj.Pedido.Destinatario != null && obj.Pedido.Destinatario.CPF_CNPJ == cnpjDestinatario);
            else
                result = result.Where(obj => obj.Pedido.Destinatario == null);

            if (codigoTipoCarga > 0)
                result = result.Where(obj => obj.Pedido.TipoDeCarga == null || obj.Pedido.TipoDeCarga.Codigo == codigoTipoCarga);

            return result.FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.WithOptions(o => o.SetTimeout(120)).FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCodigoAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return await result.WithOptions(o => o.SetTimeout(120)).FirstOrDefaultAsync();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = query.Where(obj => codigos.Contains(obj.Codigo));
            return result.ToList();
        }

        public async Task<List<(int ProtocoloCargaOrigem, int ProtocoloPedido)>> BuscarProtocolosPorCodigosAsync(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            var result = await query
                .Where(obj => codigos.Contains(obj.Codigo))
                .Select(obj => new
                {
                    ProtocoloCargaOrigem = obj.CargaOrigem.Protocolo,
                    ProtocoloPedido = obj.Pedido.Protocolo
                })
                .ToListAsync(CancellationToken);

            return result
                .Select(o => (o.ProtocoloCargaOrigem, o.ProtocoloPedido))
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorCodigosRemetenteDestinatario(List<int> codigos, double remetente, double destinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where codigos.Contains(obj.Codigo) && obj.Pedido.Remetente.CPF_CNPJ == remetente && obj.Pedido.Destinatario.CPF_CNPJ == destinatario select obj;
            return
                result.
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Origem).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Destino).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Remetente).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Destinatario).
                Fetch(obj => obj.Origem).
                Fetch(obj => obj.Destino).
                Fetch(obj => obj.Expedidor).
                Fetch(obj => obj.Recebedor).
                Fetch(obj => obj.CargaOrigem).
                ThenFetch(obj => obj.Empresa).
                ThenFetch(obj => obj.Localidade).
                Fetch(obj => obj.CFOP).
                Fetch(obj => obj.CFOPFilialEmissora).
                Fetch(obj => obj.FormulaRateio).
                FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCodigoComFetch(List<int> codigos)
        {
            var query1 = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query1 = query1.Where(obj => codigos.Contains(obj.Codigo));
            query1
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.TipoOperacao)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Remetente)
                .Fetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.EnderecoOrigem)
                .ThenFetch(obj => obj.ClienteOutroEndereco)
                .ThenFetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Estado)
                .ThenFetch(obj => obj.Pais)
                .ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> c1 = query1.ToList();

            var query2 = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query2 = query2.Where(obj => codigos.Contains(obj.Codigo));
            query2
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.EnderecoDestino)
                .ThenFetch(obj => obj.ClienteOutroEndereco)
                .ThenFetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Estado)
                .ThenFetch(obj => obj.Pais)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Regiao)
                .Fetch(obj => obj.Tomador)
                .Fetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.CFOP)
                .Fetch(obj => obj.Origem)
                .Fetch(obj => obj.Destino)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.RotaFrete)
                .ToList();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> c2 = query2.ToList();
            foreach (var cc1 in c1)
            {
                foreach (var cc2 in c2.Where(p => p.Pedido.Codigo == cc1.Pedido.Codigo))
                {
                    cc1.Pedido.Destinatario = cc2.Pedido.Destinatario;
                    cc1.Pedido.EnderecoDestino = cc2.Pedido.EnderecoDestino;
                    cc1.Pedido.Tomador = cc2.Pedido.Tomador;

                    if (cc1.Pedido.Tomador != null)
                        cc1.Pedido.Tomador.ModeloDocumentoFiscal = cc2.Pedido.Tomador.ModeloDocumentoFiscal;
                    cc1.CFOP = cc2.CFOP;
                    cc1.Pedido.Origem = cc2.Pedido.Origem;
                    cc1.Pedido.Destino = cc2.Pedido.Destino;
                    cc1.Pedido.RotaFrete = cc2.Pedido.RotaFrete;
                }
            }
            return c1;
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorCodigoComFetch(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = from obj in query where obj.Codigo == codigo select obj;

            return
                query.
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Origem).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Destino).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Remetente).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Destinatario).
                Fetch(obj => obj.Origem).
                Fetch(obj => obj.Destino).
                Fetch(obj => obj.Expedidor).
                Fetch(obj => obj.Recebedor).
                Fetch(obj => obj.CargaOrigem).
                ThenFetch(obj => obj.Empresa).
                Fetch(obj => obj.CFOP).
                Fetch(obj => obj.CFOPFilialEmissora).
                Fetch(obj => obj.FormulaRateio).
                FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorCodigoComFetchEmpresa(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = from obj in query where obj.Codigo == codigo select obj;

            return
                query
                .Fetch(obj => obj.CargaOrigem)
                    .ThenFetch(obj => obj.Empresa)
                    .ThenFetch(obj => obj.Configuracao)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorCodigoDetalhes(int codigo)
        {
            var iQueryable = this.SessionNHiBernate
                .Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(obj => obj.Codigo == codigo)
                .Fetch(obj => obj.Pedido)
                    .ThenFetch(p => p.Origem)
                .Fetch(obj => obj.Pedido)
                    .ThenFetch(p => p.Destino)
                .Fetch(obj => obj.Pedido)
                    .ThenFetch(p => p.Remetente)
                    .ThenFetch(r => r.Localidade)
                .Fetch(obj => obj.Pedido)
                    .ThenFetch(p => p.Destinatario)
                    .ThenFetch(d => d.Localidade)
                .Fetch(obj => obj.Origem)
                .Fetch(obj => obj.Destino)
                .Fetch(obj => obj.Expedidor)
                    .ThenFetch(e => e.Localidade)
                .Fetch(obj => obj.Recebedor)
                    .ThenFetch(r => r.Localidade)
                .Fetch(obj => obj.Carga)
                    .ThenFetch(c => c.TipoOperacao)
                .Fetch(obj => obj.Pedido)
                    .ThenFetch(p => p.Filial)
                .Fetch(obj => obj.Tomador)
                    .ThenFetch(t => t.Localidade)
                .Fetch(obj => obj.FormulaRateio)
                .Fetch(obj => obj.Carga)
                    .ThenFetch(c => c.Empresa)
                    .ThenFetch(e => e.Localidade)
                .Fetch(obj => obj.Pedido)
                    .ThenFetch(p => p.Container)
                .Fetch(obj => obj.Pedido)
                    .ThenFetch(p => p.ContainerTipoReserva)
                .Fetch(obj => obj.Pedido)
                    .ThenFetch(p => p.EnderecoDestino);

            var result = iQueryable.FirstOrDefault();

            if (result?.Pedido?.TipoOperacao?.ConsiderarTomadorPedido == true && result.Pedido.Tomador != null)
            {
                result.Tomador = result.Pedido.Tomador;
            }

            return result;
        }

        public decimal BuscarValorTotalPedidos(int carga)
        {
            //var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            //var result = from obj in query where obj.Carga.Codigo == carga select obj;
            //return result.Sum(obj => obj.Pedido.ValorTotalNotasFiscais);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(o => o.Carga.Codigo == carga);
            return query.Sum(o => (decimal?)o.Pedido.ValorTotalNotasFiscais) ?? 0m;
        }

        public decimal BuscarValorTotalISSRetidoPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == carga select obj;
            return result.Sum(obj => obj.ValorRetencaoISS);
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarCargaPedidoProximoTrecho(int pedido, int cargaPedidoDifere)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query
                         where
                            obj.Pedido.Codigo == pedido
                            && obj.Codigo != cargaPedidoDifere
                            && obj.Expedidor == obj.Pedido.Recebedor
                         select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidosProximoTrecho(List<int> pedidos)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> result = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            int take = 1000;
            int start = 0;
            while (start < pedidos?.Count)
            {
                List<int> tmp = pedidos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                var filter = from obj in query
                             where tmp.Contains(obj.Pedido.Codigo) &&
                                   obj.Expedidor != null &&
                                   obj.Expedidor == obj.Pedido.Recebedor
                             select obj;

                result.AddRange(filter.Fetch(x => x.Pedido)
                                      .ToList());

                start += take;
            }

            return result;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>> BuscarCargaPedidosProximoTrechoAsync(List<int> pedidos)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> result = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            int take = 1000;
            int start = 0;
            while (start < pedidos?.Count)
            {
                List<int> tmp = pedidos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                var filter = from obj in query
                             where tmp.Contains(obj.Pedido.Codigo) &&
                                   obj.Expedidor != null &&
                                   obj.Expedidor == obj.Pedido.Recebedor
                             select obj;

                result.AddRange(await filter.Fetch(x => x.Pedido)
                                      .ToListAsync());

                start += take;
            }

            return result;
        }

        public bool VerificarCargasAgValorRedespacho(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == carga select obj;
            return result.Any(obj => obj.AgValorRedespacho);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargasPedidoAgIntegracao(List<double> destinatarios, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query
                         where obj.Carga.Protocolo > 0 && !obj.CargaPedidoIntegrada &&
                                   (obj.Carga.SituacaoCarga == SituacaoCarga.AgImpressaoDocumentos ||
                                   obj.Carga.SituacaoCarga == SituacaoCarga.EmTransporte ||
                                   obj.Carga.SituacaoCarga == SituacaoCarga.Encerrada ||
                                   obj.Carga.SituacaoCarga == SituacaoCarga.LiberadoPagamento)
                         select obj;

            if (destinatarios.Count > 0)
                result = result.Where(obj => destinatarios.Contains(obj.Recebedor.CPF_CNPJ));

            return result
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Remetente)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Tomador)
                .Fetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.CFOP)
                .Fetch(obj => obj.Origem)
                .Fetch(obj => obj.Destino)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.RotaFrete)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.ClienteDeslocamento)
                .Fetch(obj => obj.CargaOrigem)
                .ThenFetch(obj => obj.Empresa)
                .Fetch(obj => obj.CargaOrigem)
                .ThenFetch(obj => obj.EmpresaFilialEmissora)
                .Fetch(obj => obj.CargaPedidoProximoTrecho)
                .Fetch(obj => obj.CargaPedidoTrechoAnterior)
                .Skip(inicio).Take(limite)
                .ToList();
        }

        public int ContarCargasPedidoAgIntegracao(List<double> destinatarios)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query
                         where obj.Carga.Protocolo > 0 && !obj.CargaPedidoIntegrada &&
                                   (obj.Carga.SituacaoCarga == SituacaoCarga.AgImpressaoDocumentos ||
                                   obj.Carga.SituacaoCarga == SituacaoCarga.EmTransporte ||
                                   obj.Carga.SituacaoCarga == SituacaoCarga.Encerrada ||
                                   obj.Carga.SituacaoCarga == SituacaoCarga.LiberadoPagamento)
                         select obj;

            if (destinatarios.Count > 0)
                result = result.Where(obj => (destinatarios.Contains(obj.Pedido.Destinatario.CPF_CNPJ) && obj.Recebedor == null) || destinatarios.Contains(obj.Recebedor.CPF_CNPJ));

            return result.Count();
        }

        public Task<List<int>> BuscarCargasAgIntegracaoEmbarcadorAsync(bool validarIntegradoraNFe, DateTime dataFinalizacaoEmissao, int integradora, int codigoGrupoPessoas, int codigoEmpresa, int inicio, int limite, string codigoIntegracaoTipoOperacao, string codigoIntegracaoFilial, bool filtrarPorSituacao = true)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = ConsultaCargasAgIntegracaoEmbarcador(validarIntegradoraNFe, dataFinalizacaoEmissao, integradora, codigoGrupoPessoas, codigoEmpresa, codigoIntegracaoTipoOperacao, codigoIntegracaoFilial, filtrarPorSituacao);

            return query.Select(obj => obj.Codigo).OrderByDescending(x => x).Skip(inicio).Take(limite).ToListAsync(CancellationToken);
        }

        public Task<int> ContarCargasAgIntegracaoEmbarcadorAsync(bool validarIntegradoraNFe, DateTime dataFinalizacaoEmissao, int integradora, int codigoGrupoPessoas, int codigoEmpresa, string codigoIntegracaoTipoOperacao, string codigoIntegracaoFilial, bool filtrarPorSituacao = true)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = ConsultaCargasAgIntegracaoEmbarcador(validarIntegradoraNFe, dataFinalizacaoEmissao, integradora, codigoGrupoPessoas, codigoEmpresa, codigoIntegracaoTipoOperacao, codigoIntegracaoFilial, filtrarPorSituacao);

            return query.CountAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargasRedespachoAgIntegracaoEmbarcador(bool validarIntegradoraNFe, DateTime dataFinalizacaoEmissao, int integradora, int codigoGrupoPessoas, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query
                         where obj.Carga.Protocolo > 0 && obj.Carga.Redespacho != null &&
                                   obj.Carga.DataFinalizacaoEmissao <= dataFinalizacaoEmissao &&
                                   (obj.Carga.SituacaoCarga == SituacaoCarga.AgImpressaoDocumentos ||
                                   obj.Carga.SituacaoCarga == SituacaoCarga.EmTransporte ||
                                   obj.Carga.SituacaoCarga == SituacaoCarga.Encerrada ||
                                   obj.Carga.SituacaoCarga == SituacaoCarga.LiberadoPagamento)
                         select obj;

            result = result.Where(obj => !obj.Carga.CargaIntegradaEmbarcador);

            if (validarIntegradoraNFe)
                result = result.Where(obj => obj.Carga.IntegradoraNFe.Codigo == integradora);

            if (codigoGrupoPessoas > 0)
                result = result.Where(o => o.Carga.GrupoPessoaPrincipal.Codigo == codigoGrupoPessoas);

            return result
                .Fetch(obj => obj.Carga)
                .Fetch(obj => obj.Pedido)
                .Skip(inicio).Take(limite).ToList();
        }

        public int ContarCargasRedespachoAgIntegracaoEmbarcador(bool validarIntegradoraNFe, DateTime dataFinalizacaoEmissao, int integradora, int codigoGrupoPessoas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query
                         where obj.Carga.Protocolo > 0 && obj.Carga.Redespacho != null &&
                                   obj.Carga.DataFinalizacaoEmissao <= dataFinalizacaoEmissao &&
                                   (obj.Carga.SituacaoCarga == SituacaoCarga.AgImpressaoDocumentos ||
                                   obj.Carga.SituacaoCarga == SituacaoCarga.EmTransporte ||
                                   obj.Carga.SituacaoCarga == SituacaoCarga.Encerrada ||
                                   obj.Carga.SituacaoCarga == SituacaoCarga.LiberadoPagamento)
                         select obj;


            result = result.Where(obj => !obj.Carga.CargaIntegradaEmbarcador);

            if (validarIntegradoraNFe)
                result = result.Where(obj => obj.Carga.IntegradoraNFe.Codigo == integradora);

            if (codigoGrupoPessoas > 0)
                result = result.Where(o => o.Carga.GrupoPessoaPrincipal.Codigo == codigoGrupoPessoas);

            return result.Count();
        }

        public Task<List<int>> BuscarCargasPedidoAgIntegracaoEmbarcadorAsync(bool validarIntegradoraNFe, DateTime dataFinalizacaoEmissao, int integradora, int codigoGrupoPessoas, int codigoEmpresa, List<double> CPFCNPJClientes, int inicio, int limite, string codigoIntegracaoTipoOperacao, string codigoIntegracaoFilial, bool filtrarPorSituacao = true)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> query = ConsultaCargasPedidoAgIntegracaoEmbarcador(validarIntegradoraNFe, dataFinalizacaoEmissao, integradora, codigoGrupoPessoas, codigoEmpresa, CPFCNPJClientes, codigoIntegracaoTipoOperacao, codigoIntegracaoFilial, filtrarPorSituacao);

            return query.Select(obj => obj.PedidoXMLNotaFiscal.CargaPedido.Codigo).OrderByDescending(x => x).Skip(inicio).Take(limite).ToListAsync(CancellationToken);
        }

        public Task<int> ContarCargasPedidoAgIntegracaoEmbarcadorAsync(bool validarIntegradoraNFe, DateTime dataFinalizacaoEmissao, int integradora, int codigoGrupoPessoas, int codigoEmpresa, List<double> CPFCNPJClientes, string codigoIntegracaoTipoOperacao, string codigoIntegracaoFilial, bool filtrarPorSituacao = true)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> query = ConsultaCargasPedidoAgIntegracaoEmbarcador(validarIntegradoraNFe, dataFinalizacaoEmissao, integradora, codigoGrupoPessoas, codigoEmpresa, CPFCNPJClientes, codigoIntegracaoTipoOperacao, codigoIntegracaoFilial, filtrarPorSituacao);

            return query.Select(obj => obj.PedidoXMLNotaFiscal.CargaPedido).Distinct().CountAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargasPedidoRedespachoAgIntegracaoEmbarcador(bool validarIntegradoraNFe, DateTime dataFinalizacaoEmissao, int integradora, int codigoGrupoPessoas, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            var result = from obj in query
                         where obj.PedidoXMLNotaFiscal.CargaPedido.Carga.Protocolo > 0 && obj.CargaCTe.Carga.Redespacho != null &&
                                   obj.CargaCTe.CTe != null && obj.CargaCTe.CTe.Status == "A" &&
                                   obj.CargaCTe.CTe.DataEmissao <= dataFinalizacaoEmissao &&
                                   (obj.PedidoXMLNotaFiscal.CargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgImpressaoDocumentos ||
                                   obj.PedidoXMLNotaFiscal.CargaPedido.Carga.SituacaoCarga == SituacaoCarga.EmTransporte ||
                                   obj.PedidoXMLNotaFiscal.CargaPedido.Carga.SituacaoCarga == SituacaoCarga.Encerrada ||
                                   obj.PedidoXMLNotaFiscal.CargaPedido.Carga.SituacaoCarga == SituacaoCarga.LiberadoPagamento ||
                                   (obj.PedidoXMLNotaFiscal.CargaPedido.Carga.SituacaoCarga == SituacaoCarga.PendeciaDocumentos && obj.PedidoXMLNotaFiscal.CargaPedido.Carga.AgImportacaoCTe))
                         select obj;

            result = result.Where(obj => !obj.PedidoXMLNotaFiscal.CargaPedido.CargaPedidoIntegrada);

            if (validarIntegradoraNFe)
                result = result.Where(obj => obj.PedidoXMLNotaFiscal.CargaPedido.Carga.IntegradoraNFe.Codigo == integradora);

            if (codigoGrupoPessoas > 0)
                result = result.Where(o => o.PedidoXMLNotaFiscal.CargaPedido.Carga.GrupoPessoaPrincipal.Codigo == codigoGrupoPessoas);

            return result.Select(obj => obj.PedidoXMLNotaFiscal.CargaPedido)
                .OrderBy(obj => obj.Codigo)
                .Fetch(obj => obj.Carga)
                .Fetch(obj => obj.Pedido)
                .Distinct()
                .Skip(inicio).Take(limite).ToList();
        }

        public int ContarCargasPedidoRedespachoAgIntegracaoEmbarcador(bool validarIntegradoraNFe, DateTime dataFinalizacaoEmissao, int integradora, int codigoGrupoPessoas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            var result = from obj in query
                         where obj.PedidoXMLNotaFiscal.CargaPedido.Carga.Protocolo > 0 && obj.CargaCTe.Carga.Redespacho != null &&
                                   obj.CargaCTe.CTe != null && obj.CargaCTe.CTe.Status == "A" &&
                                   obj.CargaCTe.CTe.DataEmissao <= dataFinalizacaoEmissao &&
                                   (obj.PedidoXMLNotaFiscal.CargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgImpressaoDocumentos ||
                                   obj.PedidoXMLNotaFiscal.CargaPedido.Carga.SituacaoCarga == SituacaoCarga.EmTransporte ||
                                   obj.PedidoXMLNotaFiscal.CargaPedido.Carga.SituacaoCarga == SituacaoCarga.Encerrada ||
                                   obj.PedidoXMLNotaFiscal.CargaPedido.Carga.SituacaoCarga == SituacaoCarga.LiberadoPagamento ||
                                   (obj.PedidoXMLNotaFiscal.CargaPedido.Carga.SituacaoCarga == SituacaoCarga.PendeciaDocumentos && obj.PedidoXMLNotaFiscal.CargaPedido.Carga.AgImportacaoCTe))
                         select obj;

            result = result.Where(obj => !obj.PedidoXMLNotaFiscal.CargaPedido.CargaPedidoIntegrada);

            if (validarIntegradoraNFe)
                result = result.Where(obj => obj.PedidoXMLNotaFiscal.CargaPedido.Carga.IntegradoraNFe.Codigo == integradora);

            if (codigoGrupoPessoas > 0)
                result = result.Where(o => o.PedidoXMLNotaFiscal.CargaPedido.Carga.GrupoPessoaPrincipal.Codigo == codigoGrupoPessoas);

            return result.Select(obj => obj.PedidoXMLNotaFiscal.CargaPedido).Distinct().Count();
        }

        public List<Dominio.Entidades.Cliente> ConsultarDestinatarioCarga(bool destinatariosDaCarga, int carga, string nome, double cpfCnpj, string tipo, Dominio.Entidades.Localidade localidade, string telefone, int codigoGrupoPessoas, bool buscarPorCargaOrigem, int inicioRegistros, int maximoRegistros, string propOrdenacao = "", string dirOrdenacao = "asc")
        {
            var consultaCliente = ConsultarDestinatarioCarga(destinatariosDaCarga, carga, nome, cpfCnpj, tipo, localidade, telefone, codigoGrupoPessoas, buscarPorCargaOrigem);

            return ObterLista(consultaCliente, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsultarDestinatarioCarga(bool destinatariosDaCarga, int carga, string nome, double cpfCnpj, string tipo, Dominio.Entidades.Localidade localidade, string telefone, int codigoGrupoPessoas, bool buscarPorCargaOrigem)
        {
            var consultaCliente = ConsultarDestinatarioCarga(destinatariosDaCarga, carga, nome, cpfCnpj, tipo, localidade, telefone, codigoGrupoPessoas, buscarPorCargaOrigem);

            return consultaCliente.Count();
        }

        public List<Dominio.Entidades.Cliente> ConsultarTomadorCarga(bool destinatariosDaCarga, int carga, string nome, double cpfCnpj, string tipo, Dominio.Entidades.Localidade localidade, string telefone, int codigoGrupoPessoas, int inicioRegistros, int maximoRegistros, string propOrdenacao = "", string dirOrdenacao = "asc")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var queryPessoa = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            query = query.Where(obj => obj.CargaOrigem.Codigo == carga);

            if (!string.IsNullOrWhiteSpace(nome))
                queryPessoa = queryPessoa.Where(o => o.Nome.Contains(nome));

            if (!string.IsNullOrWhiteSpace(tipo))
                queryPessoa = queryPessoa.Where(o => o.Tipo.Equals(tipo));

            if (localidade != null)
                queryPessoa = queryPessoa.Where(o => o.Localidade == localidade);

            if (!string.IsNullOrWhiteSpace(telefone))
                queryPessoa = queryPessoa.Where(o => o.Telefone1.Equals(telefone));

            if (cpfCnpj > 0)
                queryPessoa = queryPessoa.Where(o => o.CPF_CNPJ == cpfCnpj);

            if (codigoGrupoPessoas > 0)
                queryPessoa = queryPessoa.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas);

            var queryTomadores = query.Select(o => o.Pedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario ? o.Pedido.Destinatario :
                                      o.Pedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor ? o.Pedido.Expedidor :
                                      o.Pedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros ? o.Pedido.Tomador :
                                      o.Pedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor ? o.Pedido.Recebedor :
                                      o.Pedido.Remetente).ToList();

            queryPessoa = queryPessoa.Where(o => queryTomadores.Contains(o));

            return queryPessoa
                    .OrderBy((propOrdenacao ?? "Nome") + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                    .Skip(inicioRegistros)
                    .Take(maximoRegistros)
                    .Timeout(120)
                    .Distinct().ToList();
        }

        public int ContarConsultarTomadorCarga(bool destinatariosDaCarga, int carga, string nome, double cpfCnpj, string tipo, Dominio.Entidades.Localidade localidade, string telefone, int codigoGrupoPessoas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var queryPessoa = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            query = query.Where(obj => obj.CargaOrigem.Codigo == carga);

            if (!string.IsNullOrWhiteSpace(nome))
                queryPessoa = queryPessoa.Where(o => o.Nome.Contains(nome));

            if (!string.IsNullOrWhiteSpace(tipo))
                queryPessoa = queryPessoa.Where(o => o.Tipo.Equals(tipo));

            if (localidade != null)
                queryPessoa = queryPessoa.Where(o => o.Localidade == localidade);

            if (!string.IsNullOrWhiteSpace(telefone))
                queryPessoa = queryPessoa.Where(o => o.Telefone1.Equals(telefone));

            if (cpfCnpj > 0)
                queryPessoa = queryPessoa.Where(o => o.CPF_CNPJ == cpfCnpj);

            if (codigoGrupoPessoas > 0)
                queryPessoa = queryPessoa.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas);

            var queryTomadores = query.Select(o => o.Pedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario ? o.Pedido.Destinatario :
                                     o.Pedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor ? o.Pedido.Expedidor :
                                     o.Pedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros ? o.Pedido.Tomador :
                                     o.Pedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor ? o.Pedido.Recebedor :
                                     o.Pedido.Remetente).ToList();

            queryPessoa = queryPessoa.Where(o => queryTomadores.Contains(o));

            return queryPessoa.Distinct().Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCodigo(int[] codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where codigo.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidoTrechoAnteriorCargaDiferente(int codigoCargaPedido, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.CargaPedidoTrechoAnterior.Codigo == codigoCargaPedido && obj.Carga.Codigo != codigoCarga select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidoProximoTrechoCargaDiferente(int codigoCargaPedido, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.CargaPedidoProximoTrecho.Codigo == codigoCargaPedido && obj.Carga.Codigo != codigoCarga select obj;
            return result.ToList();
        }

        public bool VerificarCargaPedidoTrechoAnterior(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.CargaPedidoTrechoAnterior.Codigo == codigo && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada select obj;
            int cont = result.Count();
            if (cont > 0)
                return true;
            else
                return false;
        }

        public bool VerificarCargaPedidoProximoTrecho(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.CargaPedidoProximoTrecho.Codigo == codigo && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada select obj;
            int cont = result.Count();
            if (cont > 0)
                return true;
            else
                return false;
        }

        public bool VerificarSeTodosPedidosEstaoAutorizadosPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == carga && obj.SituacaoEmissao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada && !obj.PedidoSemNFe && obj.Pedido.TipoPedido == TipoPedido.Entrega select obj;
            int cont = result.Count();
            if (cont > 0)
                return false;
            else
                return true;
        }

        public bool VerificarSeTodosPedidosEstaoAutorizadosPorCargaExpedidor(int carga, double expedidor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == carga && obj.Expedidor.CPF_CNPJ == expedidor && obj.SituacaoEmissao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada && !obj.PedidoSemNFe && obj.Pedido.TipoPedido == TipoPedido.Entrega select obj;
            int cont = result.Count();
            if (cont > 0)
                return false;
            else
                return true;
        }

        public bool VerificarSeTodosPedidosEstaoAutorizadosPorCargaRemetente(int carga, double remetente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query
                         where obj.Carga.Codigo == carga && obj.Expedidor == null
&& obj.Pedido.Remetente.CPF_CNPJ == remetente
&& obj.SituacaoEmissao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada
&& obj.Pedido.TipoPedido == TipoPedido.Entrega
                         select obj;
            int cont = result.Count();
            if (cont > 0)
                return false;
            else
                return true;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> BuscarCodigosCargaPedidoPorCarga(int codigoCarga, bool desconsiderarPesoNotasPallet)
        {
            return BuscarCodigosCargaPedidoPorCargas(new List<int>() { codigoCarga }, desconsiderarPesoNotasPallet);
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> BuscarCodigosCargaPedidoPorCargas(List<int> codigosCarga, bool desconsiderarPesoNotasPallet)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(cargaPedido => codigosCarga.Contains(cargaPedido.Carga.Codigo));

            var consultaNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(notaFiscal => codigosCarga.Contains(notaFiscal.CargaPedido.Carga.Codigo) && notaFiscal.XMLNotaFiscal.nfAtiva);

            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidos = consultaCargaPedido.Select(cargaPedido => new Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido()
            {
                Codigo = cargaPedido.Codigo,
                CodigoCarga = cargaPedido.Carga.Codigo,
                NumeroControlePedido = !string.IsNullOrWhiteSpace(cargaPedido.Pedido.NumeroControle) ? cargaPedido.Pedido.NumeroControle : string.Empty,
                CodigoPedidoCliente = cargaPedido.Pedido != null ? cargaPedido.Pedido.CodigoPedidoCliente : string.Empty,
                NumeroPedidoEmbarcador = cargaPedido.Pedido.NumeroPedidoEmbarcador,
                ReentregaSolicitada = (bool?)cargaPedido.ReentregaSolicitada ?? false,
                CienciaDoEnvioDaNotaInformado = (bool?)cargaPedido.CienciaDoEnvioDaNotaInformado ?? false,
                CTeEmitidoNoEmbarcador = (bool?)cargaPedido.CTeEmitidoNoEmbarcador ?? false,
                PedidoTransbordo = (bool?)cargaPedido.Pedido.PedidoTransbordo ?? false,
                AgInformarRecebedor = (bool?)cargaPedido.AgInformarRecebedor ?? false,
                TipoContratacaoCarga = (TipoContratacaoCarga?)cargaPedido.TipoContratacaoCarga ?? TipoContratacaoCarga.Normal,
                AdicionadaManualmente = (bool?)cargaPedido.Pedido.AdicionadaManualmente ?? false,
                Ordem = cargaPedido.Pedido.Ordem,
                NumeroBooking = cargaPedido.Pedido.NumeroBooking,
                NumeroContainer = cargaPedido.Pedido.Container != null ? (cargaPedido.Pedido.Container.Numero + (string.IsNullOrWhiteSpace(cargaPedido.Pedido.TaraContainer) ? "" : " Tr.: " + cargaPedido.Pedido.TaraContainer) + " (" + cargaPedido.Pedido.LacreContainerUm + " " + cargaPedido.Pedido.LacreContainerDois + " " + cargaPedido.Pedido.LacreContainerTres + ")") : "",
                NumeroPedido = cargaPedido.Pedido.Numero.ToString("D"),
                PossuiGenset = (bool?)cargaPedido.Pedido.PossuiGenset ?? false,
                ValorCustoFrete = (decimal?)cargaPedido.Pedido.ValorCustoFrete ?? 0,
                PrevisaoEntregaTransportador = (DateTime?)cargaPedido.Pedido.PrevisaoEntregaTransportador ?? null,
                Cubagem = cargaPedido.Pedido.CubagemTotal,
                CanceladoAposVinculoCarga = (bool?)cargaPedido.Pedido.CanceladoAposVinculoCarga ?? false,
                IdMontagemContainer = cargaPedido.Pedido.MontagemContainer != null ? cargaPedido.Pedido.MontagemContainer.Id.ToString() : "",
                NumeroContainerCarga = cargaPedido.Pedido.Container != null ? cargaPedido.Pedido.Container.Numero : "",
                DataBaseCRT = cargaPedido.Pedido.DataBaseCRT,
                Remetente = cargaPedido.Pedido.Remetente == null ? null : new Dominio.ObjetosDeValor.Cliente()
                {
                    Codigo = cargaPedido.Pedido.Remetente.CPF_CNPJ,
                    Nome = cargaPedido.Pedido.Remetente.Nome,
                    CPFCNPJ = cargaPedido.Pedido.Remetente.CPF_CNPJ.ToString()
                },
                Destinatario = cargaPedido.Pedido.Destinatario == null ? null : new Dominio.ObjetosDeValor.Cliente()
                {
                    Codigo = cargaPedido.Pedido.Destinatario.CPF_CNPJ,
                    Nome = cargaPedido.Pedido.Destinatario.Nome,
                    CPFCNPJ = cargaPedido.Pedido.Destinatario.CPF_CNPJ.ToString(),
                    Regiao = cargaPedido.Pedido.Destinatario.Regiao.Descricao,
                    Mesorregiao = cargaPedido.Pedido.Destinatario.MesoRegiao.Descricao
                },
                TipoPropostaMultimodal = (TipoPropostaMultimodal?)cargaPedido.TipoPropostaMultimodal ?? TipoPropostaMultimodal.Nenhum,
                ModalPropostaMultimodal = (ModalPropostaMultimodal?)cargaPedido.ModalPropostaMultimodal ?? ModalPropostaMultimodal.Nenhum,
                TipoServicoMultimodal = (TipoServicoMultimodal?)cargaPedido.TipoServicoMultimodal ?? TipoServicoMultimodal.Nenhum,
                TipoCobrancaMultimodal = (TipoCobrancaMultimodal?)cargaPedido.TipoCobrancaMultimodal ?? TipoCobrancaMultimodal.Nenhum,
                IndicadorRemessaVenda = (bool?)cargaPedido.IndicadorRemessaVenda ?? false,
                PesoPallet = (cargaPedido.PesoPallet.HasValue ? cargaPedido.PesoPallet.Value : 0)
            }).ToList();

            var notasFiscais = consultaNotaFiscal
                .Select(notaFiscal =>
                    new Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoXMLNotaFiscalConsulta()
                    {
                        Ativa = notaFiscal.XMLNotaFiscal.nfAtiva,
                        Codigo = notaFiscal.XMLNotaFiscal.Codigo,
                        CodigoCargaPedido = notaFiscal.CargaPedido.Codigo,
                        PesoCubado = (decimal?)notaFiscal.XMLNotaFiscal.PesoCubado ?? 0,
                        Peso = notaFiscal.XMLNotaFiscal.Peso,
                        TipoNotaFiscalIntegrada = notaFiscal.XMLNotaFiscal.TipoNotaFiscalIntegrada,
                        Valor = notaFiscal.XMLNotaFiscal.Valor,
                        ClassificacaoNFe = notaFiscal.XMLNotaFiscal.ClassificacaoNFe,
                        TipoFatura = (bool?)notaFiscal.XMLNotaFiscal.TipoFatura ?? false
                    }
                ).ToList();

            IEnumerable<int> codigosCargaPedidos = notasFiscais.Select(notaFiscal => notaFiscal.CodigoCargaPedido).Distinct();

            foreach (int codigoCargaPedido in codigosCargaPedidos)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido cargaPedido = cargaPedidos.Where(cargaPedidoFiltrar => cargaPedidoFiltrar.Codigo == codigoCargaPedido).FirstOrDefault();

                cargaPedido.NotasFiscais.AddRange(notasFiscais
                    .Where(notaFiscal => notaFiscal.CodigoCargaPedido == codigoCargaPedido)
                    .Select(notaFiscal => new Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoXMLNotaFiscal()
                    {
                        Ativa = notaFiscal.Ativa,
                        Codigo = notaFiscal.Codigo,
                        Peso = notaFiscal.Peso,
                        PesoCubado = (decimal?)notaFiscal.PesoCubado ?? 0,
                        Valor = ((notaFiscal.TipoNotaFiscalIntegrada != TipoNotaFiscalIntegrada.RemessaPallet) || !desconsiderarPesoNotasPallet) ? notaFiscal.Valor : 0m,
                        ClassificacaoNFe = notaFiscal.ClassificacaoNFe,
                        TipoFatura = notaFiscal.TipoFatura
                    })
                    .ToList()
                );
            }

            return cargaPedidos;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargasPedidoPorCodigosCarga(List<int> codigosCarga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(cargaPedido => codigosCarga.Contains(cargaPedido.Carga.Codigo));

            return consultaCargaPedido.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargasPedidoPorCodigosCargaFetch(List<int> codigosCarga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(cargaPedido => codigosCarga.Contains(cargaPedido.Carga.Codigo))
                .Fetch(obj => obj.Pedido).ThenFetch(obj => obj.Remetente)
                .Fetch(obj => obj.Expedidor)
                .ToList();
            return consultaCargaPedido.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.PedidoTransbordo> BuscarCodigosPedidoTransbordoPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == carga);

            var queryPedidoTransbordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>();
            queryPedidoTransbordo = queryPedidoTransbordo.Where(obj => query.Any(p => p.Pedido == obj.Pedido));

            return queryPedidoTransbordo.Select(obj => new Dominio.ObjetosDeValor.Embarcador.Carga.PedidoTransbordo()
            {
                Sequencia = obj.Sequencia,
                Navio = obj.PedidoViagemNavio != null ? obj.PedidoViagemNavio.Descricao : "",
                Porto = obj.Porto != null ? obj.Porto.Descricao : ""
            })?.Distinct().ToList() ?? null;
        }

        public List<int> BuscarCodigosPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosPedidoPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.Select(o => o.Pedido.Codigo).ToList();
        }

        /// <summary>
        /// verifica se ouve um redespacho intermedirio onde o recebedor era igual ao expedidor do proximo trecho.
        /// </summary>
        /// <param name="numeroPedido"></param>
        /// <param name="recebedor"></param>
        /// <returns></returns>
        public bool VerificarSePedidoTeveRedespachoIntermediario(string numeroPedido, double recebedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query
                         where obj.TipoContratacaoCarga == TipoContratacaoCarga.RedespachoIntermediario && obj.Pedido.NumeroPedidoEmbarcador == numeroPedido
        && (obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada) && obj.Carga.CargaFechada
                         select obj;


            if (recebedor > 0)
                result = result.Where(obj => obj.Recebedor.CPF_CNPJ == recebedor);

            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarSePedidoJaFoiEncaixadoPorExpedidor(string numeroPedido, int filial, double expedidor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query
                         where (obj.TipoContratacaoCarga == TipoContratacaoCarga.SVMTerceiro ||
                         obj.TipoContratacaoCarga == TipoContratacaoCarga.SubContratada ||
                         obj.TipoContratacaoCarga == TipoContratacaoCarga.Redespacho ||
                         obj.TipoContratacaoCarga == TipoContratacaoCarga.RedespachoIntermediario)
        && (obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada) && obj.Carga.CargaFechada
                         select obj;

            if (!string.IsNullOrWhiteSpace(numeroPedido))
                result = result.Where(obj => obj.Pedido.NumeroPedidoEmbarcador == numeroPedido);

            if (filial > 0)
                result = result.Where(obj => obj.Carga.Filial.Codigo == filial);

            if (expedidor > 0)
                result = result.Where(obj => obj.Expedidor.CPF_CNPJ == expedidor);

            return result.FirstOrDefault();
        }

        public bool BuscarSePedidoJaFoiEncaixadoNaCarga(string numeroPedido, int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query
                         where obj.Carga.Codigo == carga && obj.PedidoEncaixado && obj.Pedido.NumeroPedidoEmbarcador == numeroPedido
                         select obj;


            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorMDFeAquaviarioGeradoMasNaoVinculado(int codigoViagem, int codigoPortoOrigem, int codigoPortoDestino, int codigoTerminalOrigem, int codigoTerminalDestino)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query
                         where obj.Carga.MDFeAquaviarioGeradoMasNaoVinculado == true && obj.Pedido.PedidoViagemNavio.Codigo == codigoViagem
                            && obj.Pedido.Porto.Codigo == codigoPortoOrigem
                            && obj.Pedido.PortoDestino.Codigo == codigoPortoDestino
                            && obj.Pedido.TerminalOrigem.Codigo == codigoTerminalOrigem
                            && obj.Pedido.TerminalDestino.Codigo == codigoTerminalDestino
                         select obj;

            return result
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCargaOrigem(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.CargaOrigem.Codigo == codigoCarga select obj;
            return result
                .Fetch(obj => obj.Pedido)
                .Fetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.CFOP)
                .Fetch(obj => obj.Origem)
                .Fetch(obj => obj.Destino)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorPedidos(List<int> pedidos)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> result = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            int take = 100;
            int start = 0;
            while (start < pedidos?.Count)
            {
                List<int> tmp = pedidos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                var filter = from obj in query
                             where tmp.Contains(obj.Pedido.Codigo) && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada
                             select obj;

                var filter2 = from obj in query
                              where tmp.Contains(obj.Pedido.Codigo)
                              where tmp.Contains(obj.Pedido.Codigo) && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada
                              select obj;

                filter.Fetch(obj => obj.Pedido)
                    .ThenFetch(obj => obj.Remetente)
                    .Fetch(obj => obj.Pedido)
                    .ThenFetch(obj => obj.Destinatario)
                    .ThenFetch(obj => obj.Localidade)
                    .Fetch(obj => obj.Expedidor)
                    .ThenFetch(obj => obj.Localidade)
                    .Fetch(obj => obj.Recebedor)
                    .ThenFetch(obj => obj.Localidade)
                    .Fetch(obj => obj.Tomador)
                    .ToList();

                filter2.Fetch(obj => obj.ModeloDocumentoFiscal)
                    .Fetch(obj => obj.CFOP)
                    .Fetch(obj => obj.Origem)
                    .Fetch(obj => obj.Destino)
                    .Fetch(obj => obj.CargaOrigem)
                    .ThenFetch(obj => obj.Empresa)
                    .Fetch(obj => obj.CargaOrigem)
                    .ThenFetch(obj => obj.EmpresaFilialEmissora)
                    .Fetch(obj => obj.CargaPedidoProximoTrecho)
                    .Fetch(obj => obj.CargaPedidoTrechoAnterior)
                    .ToList();

                foreach (var q1 in filter)
                {
                    foreach (var q2 in filter2.Where(obj => obj.Codigo == q1.Codigo))
                    {
                        if (q2.CFOP != null)
                            q1.CFOP = q2.CFOP;

                        if (q2.Origem != null)
                            q1.Origem = q2.Origem;

                        if (q2.Destino != null)
                            q1.Destino = q2.Destino;

                        if (q2.CargaOrigem != null)
                            q1.CargaOrigem = q2.CargaOrigem;

                        if (q2.CargaPedidoProximoTrecho != null)
                            q1.CargaPedidoProximoTrecho = q2.CargaPedidoProximoTrecho;

                        if (q2.CargaPedidoTrechoAnterior != null)
                            q1.CargaPedidoTrechoAnterior = q2.CargaPedidoTrechoAnterior;
                    }
                }
                result.AddRange(filter);

                start += take;
            }

            return result;

        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCargas(List<int> cargas)
        {
            var query1 = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query1 = query1.Where(obj => cargas.Contains(obj.Carga.Codigo));
            query1
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.TipoOperacao)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Remetente)
                .Fetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.EnderecoOrigem)
                .ThenFetch(obj => obj.ClienteOutroEndereco)
                .ThenFetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Estado)
                .ThenFetch(obj => obj.Pais)
                .ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> c1 = query1.ToList();


            var query2 = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query2 = query2.Where(obj => cargas.Contains(obj.Carga.Codigo));
            query2
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.EnderecoDestino)
                .ThenFetch(obj => obj.ClienteOutroEndereco)
                .ThenFetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Estado)
                .ThenFetch(obj => obj.Pais)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Regiao)
                .Fetch(obj => obj.Tomador)
                .Fetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.CFOP)
                .Fetch(obj => obj.Origem)
                .Fetch(obj => obj.Destino)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.RotaFrete)
                .ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> c2 = query2.ToList();
            foreach (var cc1 in c1)
            {
                foreach (var cc2 in c2.Where(p => p.Pedido.Codigo == cc1.Pedido.Codigo))
                {
                    cc1.Pedido.Destinatario = cc2.Pedido.Destinatario;
                    cc1.Pedido.EnderecoDestino = cc2.Pedido.EnderecoDestino;
                    cc1.Pedido.Tomador = cc2.Pedido.Tomador;

                    if (cc1.Pedido.Tomador != null)
                        cc1.Pedido.Tomador.ModeloDocumentoFiscal = cc2.Pedido.Tomador.ModeloDocumentoFiscal;
                    cc1.CFOP = cc2.CFOP;
                    cc1.Pedido.Origem = cc2.Pedido.Origem;
                    cc1.Pedido.Destino = cc2.Pedido.Destino;
                    cc1.Pedido.RotaFrete = cc2.Pedido.RotaFrete;
                }
            }
            return c1;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCargasFetchBasico(List<int> cargas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => cargas.Contains(obj.Carga.Codigo));

            return query
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Remetente)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Destinatario)
                .Fetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.Tomador)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCargasSemFetch(List<int> cargas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => cargas.Contains(obj.Carga.Codigo));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCargasSemFetch(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => carga == obj.Carga.Codigo);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorCargaAtiva(string codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.CodigoCargaEmbarcador == codigoCarga && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada select obj;
            return result
                .Fetch(obj => obj.Carga)
                .Fetch(obj => obj.Pedido)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPedidosPreCargaPorCarga(int codigoCarga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.Pedido.PedidoDePreCarga);

            return consultaCargaPedido
                .Fetch(o => o.Carga)
                .Fetch(o => o.Pedido)
                .ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>> BuscarPedidosPreCargaPorCargaAsync(int codigoCarga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.Pedido.PedidoDePreCarga);

            return consultaCargaPedido
                .Fetch(o => o.Carga)
                .Fetch(o => o.Pedido)
                .ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCargaResumo(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            return result
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.TipoOperacao)
                .Fetch(obj => obj.Pedido)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCargaSemNfsManual(int codigoCarga, TipoPedido? tipoPedido = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(obj => !((bool?)obj.PossuiNFSManual).HasValue || obj.PossuiNFSManual == false);

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga);

            if (tipoPedido.HasValue)
                query = query.Where(obj => obj.Pedido.TipoPedido == tipoPedido);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorLancamentoNfsManual(int codigoLancamentoNfsManual)
        {
            var queryNfsManual = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>()
                .Where(obj => obj.LancamentoNFSManual.Codigo == codigoLancamentoNfsManual);

            return queryNfsManual
                .Select(obj => obj.PedidoXMLNotaFiscal.CargaPedido)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarCargasAnteriores(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            var result = (from obj in query where obj.Carga.Codigo == codigoCarga && obj.CargaPedidoTrechoAnterior != null select obj.CargaPedidoTrechoAnterior.Carga).Distinct();

            return result.ToList();

        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarCargasProximoTrecho(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            var result = (from obj in query where obj.Carga.Codigo == codigoCarga && obj.CargaPedidoProximoTrecho != null select obj.CargaPedidoProximoTrecho.Carga).Distinct();

            return result.ToList();

        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarCargasDeEntregasRedespachoPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            var result = (from obj in query where obj.Pedido.Codigo == codigoPedido && obj.Carga.TipoOperacao != null && obj.Carga.TipoOperacao.OperacaoDeRedespacho == true select obj.Carga).Distinct();

            return result.ToList();

        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorUnicaCarga(int codigoCarga)
        {
            var consultaCargaIntegracaoValePedagio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return consultaCargaIntegracaoValePedagio.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido Busca(int codigoCarga)
        {
            var consultaCargaIntegracaoValePedagio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return consultaCargaIntegracaoValePedagio.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga == carga);
            return query.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>> BuscarPorCodigoCargaAsync(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);
            return query.ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCarga(int codigoCarga, TipoPedido? tipoPedido = null, bool retornarPedidoPallet = true, bool naoRetornarPedidosSemNFe = false, bool apenasPedidosSemNfe = false)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            if (tipoPedido != null)
                query = query.Where(o => o.Pedido.TipoPedido == tipoPedido);
            if (!retornarPedidoPallet)
                query = query.Where(o => !o.PedidoPallet);
            if (naoRetornarPedidosSemNFe)
                query = query.Where(o => !o.PedidoSemNFe);
            if (apenasPedidosSemNfe)
                query = query.Where(o => o.PedidoSemNFe);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosPrimeiroFetch = AplicarFetchOrigem(query).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosSegundoFetch = AplicarFetchDestino(query).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosTerceiroFetch = AplicarFetchOutros(query).ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cp1 in cargaPedidosPrimeiroFetch)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cp2 = cargaPedidosSegundoFetch.First(p => p.Pedido.Codigo == cp1.Pedido.Codigo);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cp3 = cargaPedidosTerceiroFetch.First(p => p.Pedido.Codigo == cp1.Pedido.Codigo);

                //Fetch CP2
                cp1.Destino = cp2.Destino;
                cp1.Pedido.Destinatario = cp2.Pedido.Destinatario;
                cp1.Pedido.EnderecoDestino = cp2.Pedido.EnderecoDestino;
                cp1.Recebedor = cp2.Recebedor;

                //Fetch CP3
                cp1.Pedido.Tomador = cp3.Pedido.Tomador;
                cp1.CFOP = cp3.CFOP;
                cp1.Pedido.RotaFrete = cp3.Pedido.RotaFrete;
                cp1.StageRelevanteCusto = cp3.StageRelevanteCusto;
                cp1.Pedido.TipoOperacao = cp3.Pedido.TipoOperacao;

                if (cp1.Pedido.Tomador != null)
                    cp1.Pedido.Tomador.ModeloDocumentoFiscal = cp3.Pedido.Tomador.ModeloDocumentoFiscal;
            }

            return cargaPedidosPrimeiroFetch;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>> BuscarPorCargaAsync(int codigoCarga, TipoPedido? tipoPedido = null, bool retornarPedidoPallet = true, bool naoRetornarPedidosSemNFe = false, bool apenasPedidosSemNfe = false)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            if (tipoPedido != null)
                query = query.Where(o => o.Pedido.TipoPedido == tipoPedido);
            if (!retornarPedidoPallet)
                query = query.Where(o => !o.PedidoPallet);
            if (naoRetornarPedidosSemNFe)
                query = query.Where(o => !o.PedidoSemNFe);
            if (apenasPedidosSemNfe)
                query = query.Where(o => o.PedidoSemNFe);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosPrimeiroFetch = AplicarFetchOrigem(query).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosSegundoFetch = AplicarFetchDestino(query).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosTerceiroFetch = AplicarFetchOutros(query).ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cp1 in cargaPedidosPrimeiroFetch)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cp2 = cargaPedidosSegundoFetch.First(p => p.Pedido.Codigo == cp1.Pedido.Codigo);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cp3 = cargaPedidosTerceiroFetch.First(p => p.Pedido.Codigo == cp1.Pedido.Codigo);

                //Fetch CP2
                cp1.Destino = cp2.Destino;
                cp1.Pedido.Destinatario = cp2.Pedido.Destinatario;
                cp1.Pedido.EnderecoDestino = cp2.Pedido.EnderecoDestino;
                cp1.Recebedor = cp2.Recebedor;

                //Fetch CP3
                cp1.Pedido.Tomador = cp3.Pedido.Tomador;
                cp1.CFOP = cp3.CFOP;
                cp1.Pedido.RotaFrete = cp3.Pedido.RotaFrete;
                cp1.StageRelevanteCusto = cp3.StageRelevanteCusto;
                cp1.Pedido.TipoOperacao = cp3.Pedido.TipoOperacao;

                if (cp1.Pedido.Tomador != null)
                    cp1.Pedido.Tomador.ModeloDocumentoFiscal = cp3.Pedido.Tomador.ModeloDocumentoFiscal;
            }

            return cargaPedidosPrimeiroFetch;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCargaDuplicarCargaPedidos(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query
                .Fetch(obj => obj.Pedido).ThenFetch(obj => obj.Veiculos)
                .Fetch(obj => obj.Pedido).ThenFetch(obj => obj.Motoristas)
                .Fetch(obj => obj.Pedido).ThenFetch(obj => obj.NotasFiscais)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCarga_antiga(int codigoCarga, TipoPedido? tipoPedido = null, bool retornarPedidoPallet = true, bool naoRetornarPedidosSemNFe = false)
        {
            int registrosTotais = 0;
            List<int> codigosCargasPedidos = null;
            int limite = 0; //Criado para situação de erros de max row size devido a quantidade de pedidos. Na Cobasi publiquei com 500

            if (limite > 0)
            {
                codigosCargasPedidos = BuscarCodigosPorCarga(codigoCarga);
                registrosTotais = codigosCargasPedidos.Count();
            }

            if (registrosTotais > limite)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> result = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

                int start = 0;
                while (start < codigosCargasPedidos?.Count)
                {
                    List<int> tmp = codigosCargasPedidos.Skip(start).Take(limite).ToList();

                    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                    var filter = from obj in query
                                 where tmp.Contains(obj.Codigo)
                                 select obj;

                    result.AddRange(filter.Fetch(obj => obj.Pedido)
                    .ThenFetch(obj => obj.TipoOperacao)
                    .Fetch(obj => obj.Pedido)
                    .ThenFetch(obj => obj.Remetente)
                    .Fetch(obj => obj.Pedido)
                    .ThenFetch(obj => obj.Destinatario)
                    .ThenFetch(obj => obj.Localidade)
                    .ThenFetch(obj => obj.Regiao)
                    .Fetch(obj => obj.Expedidor)
                    .ThenFetch(obj => obj.Localidade)
                    .Fetch(obj => obj.Recebedor)
                    .ThenFetch(obj => obj.Localidade)
                    .Fetch(obj => obj.Pedido)
                    .ThenFetch(obj => obj.EnderecoOrigem)
                    .ThenFetch(obj => obj.ClienteOutroEndereco)
                    .ThenFetch(obj => obj.Localidade)
                    .ThenFetch(obj => obj.Estado)
                    .ThenFetch(obj => obj.Pais)
                    .Fetch(obj => obj.Pedido)
                    .ThenFetch(obj => obj.EnderecoDestino)
                    .ThenFetch(obj => obj.ClienteOutroEndereco)
                    .ThenFetch(obj => obj.Localidade)
                    .ThenFetch(obj => obj.Estado)
                    .ThenFetch(obj => obj.Pais)
                    .Fetch(obj => obj.Tomador)
                    .Fetch(obj => obj.ModeloDocumentoFiscal)
                    .Fetch(obj => obj.CFOP)
                    .Fetch(obj => obj.Origem)
                    .Fetch(obj => obj.Destino)
                    //.Fetch(obj => obj.Pedido)
                    //.ThenFetch(obj => obj.RotaFrete)
                    //.Fetch(obj => obj.Pedido)
                    //.ThenFetch(obj => obj.ClienteDeslocamento)
                    //.Fetch(obj => obj.CargaOrigem)
                    //.ThenFetch(obj => obj.Empresa)
                    //.ThenFetch(obj => obj.Localidade)
                    //.Fetch(obj => obj.CargaOrigem)
                    //.ThenFetch(obj => obj.EmpresaFilialEmissora)
                    //.ThenFetch(obj => obj.Localidade)
                    //.Fetch(obj => obj.CargaPedidoProximoTrecho)
                    //.Fetch(obj => obj.CargaPedidoTrechoAnterior)
                    .ToList());

                    start += limite;
                }

                return result;

            }
            else
            {
                var result = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

                result = result.Where(o => o.Carga.Codigo == codigoCarga);

                if (tipoPedido != null)
                    result = result.Where(o => o.Pedido.TipoPedido == tipoPedido);

                if (!retornarPedidoPallet)
                    result = result.Where(o => o.PedidoPallet == false);

                if (naoRetornarPedidosSemNFe)
                    result = result.Where(o => o.PedidoSemNFe == false);

                return result
                    .Fetch(obj => obj.Pedido)
                    .ThenFetch(obj => obj.TipoOperacao)
                    .Fetch(obj => obj.Pedido)
                    .ThenFetch(obj => obj.Remetente)
                    .Fetch(obj => obj.Pedido)
                    .ThenFetch(obj => obj.Destinatario)
                    .ThenFetch(obj => obj.Localidade)
                    .ThenFetch(obj => obj.Regiao)
                    .Fetch(obj => obj.Expedidor)
                    .ThenFetch(obj => obj.Localidade)
                    .Fetch(obj => obj.Recebedor)
                    .ThenFetch(obj => obj.Localidade)
                    .Fetch(obj => obj.Pedido)
                    .ThenFetch(obj => obj.EnderecoOrigem)
                    .ThenFetch(obj => obj.ClienteOutroEndereco)
                    .ThenFetch(obj => obj.Localidade)
                    .ThenFetch(obj => obj.Estado)
                    .ThenFetch(obj => obj.Pais)
                    .Fetch(obj => obj.Pedido)
                    .ThenFetch(obj => obj.EnderecoDestino)
                    .ThenFetch(obj => obj.ClienteOutroEndereco)
                    .ThenFetch(obj => obj.Localidade)
                    .ThenFetch(obj => obj.Estado)
                    .ThenFetch(obj => obj.Pais)
                    .Fetch(obj => obj.Tomador)
                    .Fetch(obj => obj.ModeloDocumentoFiscal)
                    .Fetch(obj => obj.CFOP)
                    .Fetch(obj => obj.Origem)
                    .Fetch(obj => obj.Destino)
                    //.Fetch(obj => obj.Pedido)
                    //.ThenFetch(obj => obj.RotaFrete)
                    //.Fetch(obj => obj.Pedido)
                    //.ThenFetch(obj => obj.ClienteDeslocamento)
                    //.Fetch(obj => obj.CargaOrigem)
                    //.ThenFetch(obj => obj.Empresa)
                    //.ThenFetch(obj => obj.Localidade)
                    //.Fetch(obj => obj.CargaOrigem)
                    //.ThenFetch(obj => obj.EmpresaFilialEmissora)
                    //.ThenFetch(obj => obj.Localidade)
                    //.Fetch(obj => obj.CargaPedidoProximoTrecho)
                    //.Fetch(obj => obj.CargaPedidoTrechoAnterior)
                    .ToList();
            }
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCargaECte(int codigoCarga, int codigoCte)
        {
            var consultaNotaFiscalPorCte = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>()
                .Where(notaFiscalCte => notaFiscalCte.CargaCTe.Carga.Codigo == codigoCarga && notaFiscalCte.CargaCTe.CTe.Codigo == codigoCte);

            var consultaPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(notaFiscal => notaFiscal.CargaPedido.Carga.Codigo == codigoCarga && consultaNotaFiscalPorCte.Any(notaFiscalCte => notaFiscalCte.PedidoXMLNotaFiscal.Codigo == notaFiscal.Codigo));

            return consultaPedidoXMLNotaFiscal.Select(notaFiscal => notaFiscal.CargaPedido).ToList();
        }

        public int QuantidadeCargaPedido(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == carga);

            return query.Count();
        }

        public decimal BuscarQuantidadePalletsPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == carga);

            return query.Sum(obj => (decimal?)(obj.Pedido.NumeroPaletes + obj.Pedido.NumeroPaletesFracionado)) ?? 0;
        }

        public decimal BuscarNumeroPaletesPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == carga);

            return query.Sum(obj => (int?)obj.Pedido.NumeroPaletes) ?? 0m;
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.QuantidadeDePaletesPorCarga> BuscarNumeroPaletesPorCarga(List<int> cargas)
        {
            string sql = $@" SELECT CAR_CODIGO CodigoCarga, SUM(PED_NUMERO_PALETES)QuantidadePaletes FROM T_CARGA_PEDIDO CP
                             INNER JOIN T_PEDIDO P ON P.PED_CODIGO = CP.PED_CODIGO
                             WHERE CAR_CODIGO in ({string.Join(",", cargas.ToList())}) 
                             GROUP BY CAR_CODIGO ";
            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.QuantidadeDePaletesPorCarga)));
            return consulta.List<Dominio.ObjetosDeValor.Embarcador.Logistica.QuantidadeDePaletesPorCarga>();
        }

        public int BuscarQuantidadeDisponibilizadas(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == carga);

            return query.Sum(obj => obj.Pedido.QtdDisponibilizada);
        }

        public int BuscarQuantidadeNaoEmbarcadas(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == carga);

            return query.Sum(obj => obj.Pedido.QtdNaoEmbarcadas);
        }

        public decimal BuscarDistanciaPorCarga(int codigoCarga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return consultaCargaPedido.Sum(o => (decimal?)o.Pedido.Distancia) ?? 0m;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorProtocoloCarga(int protocoloCarga, bool retornarCargasAgrupadasCarregamento = false)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            if (retornarCargasAgrupadasCarregamento)
            {
                var queryCargaOrigem = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                query = query.Where(o => (from obj in queryCargaOrigem where obj.CargaOrigem.Protocolo == protocoloCarga || (obj.Carga.CargaAgrupada && obj.Carga.Codigo == protocoloCarga) select obj.Codigo).Contains(o.Codigo));

            }
            else
                query = query.Where(o => (o.CargaOrigem.Protocolo == protocoloCarga));

            var result = query.Fetch(obj => obj.Pedido)
                        .Fetch(obj => obj.ModeloDocumentoFiscal)
                        .Fetch(obj => obj.CFOP)
                        .Fetch(obj => obj.Origem)
                        .Fetch(obj => obj.Destino)
                        .Fetch(obj => obj.Pedido)
                        .ThenFetch(obj => obj.Destinatario)
                        .ThenFetch(obj => obj.Localidade)
                        .ThenFetch(obj => obj.Pais)
                        .Fetch(obj => obj.Pedido)
                        .ThenFetch(obj => obj.Remetente)
                        .ThenFetch(obj => obj.Localidade)
                        .Fetch(obj => obj.Pedido)
                        .ThenFetch(obj => obj.Tomador)
                        .ThenFetch(obj => obj.Localidade)
                        .Fetch(obj => obj.Recebedor)
                        .ThenFetch(obj => obj.Localidade)
                        .Fetch(obj => obj.Expedidor)
                        .ThenFetch(obj => obj.Localidade)
                        .Fetch(obj => obj.Carga)
                        .Timeout(60)
                        .ToList();

            if (result.Any())
            {
                var codigosCarga = result.Select(x => x.Carga.Codigo).Distinct();

                var cargasCarregadas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>()
                    .Where(c => codigosCarga.Contains(c.Codigo))
                    .Fetch(c => c.Veiculo)
                    .ThenFetch(v => v.ModeloVeicularCarga)
                    .ThenFetch(m => m.DivisoesCapacidade)
                    .ToList()
                    .ToDictionary(c => c.Codigo);

                foreach (var item in result)
                {
                    if (cargasCarregadas.TryGetValue(item.Carga.Codigo, out var carga))
                    {
                        item.Carga = carga;
                    }
                }
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCargaSemPedidoPallet(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && !obj.PedidoPallet select obj;
            return result
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Remetente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Remetente)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.Tomador)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.CFOP)
                .Fetch(obj => obj.Origem)
                .Fetch(obj => obj.Destino)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCargaParaRateio(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query
                         where
                            obj.Carga.Codigo == codigoCarga
                            && obj.PedidoSemNFe == false
                         select obj;
            return result
                .Fetch(obj => obj.Pedido)
                .Fetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.CFOP)
                .Fetch(obj => obj.Origem)
                .Fetch(obj => obj.Destino)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> ObterTodasCargasTrechosAnteriores(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.CargaPedidoProximoTrecho.Carga.Codigo == carga select obj;
            return result.Select(obj => obj.Carga).Distinct().ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPrimeiraPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;
            return result.Fetch(obj => obj.Pedido).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCargaOrdenadoPorEntrega(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Destinatario)
                .OrderBy(o => o.OrdemEntrega)
                .ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>> BuscarPorCargaOrdenadoPorEntregaAsync(int codigoCarga, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return query
                .Fetch(obj => obj.Pedido)
                    .ThenFetch(obj => obj.Remetente)
                .Fetch(obj => obj.Pedido)
                    .ThenFetch(obj => obj.Destinatario)
                .Fetch(obj => obj.Expedidor)
                .Fetch(obj => obj.Recebedor)
                .OrderBy(o => o.OrdemEntrega)
                .ToListAsync(cancellationToken);
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPrimeiraPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;
            return result
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Remetente)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Tomador)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.Origem)
                .Fetch(obj => obj.Destino)
                .FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPrimeiraPorCargaAsync(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;
            return await result
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Remetente)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Tomador)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.Origem)
                .Fetch(obj => obj.Destino)
                .FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPrimeiroPorCargaSemFetch(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Cliente BuscarClienteDaPrimeiraPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            return query
                .Where(obj => obj.Carga.Codigo == codigoCarga)
                .Fetch(obj => obj.Pedido).ThenFetch(obj => obj.Remetente)
                .Fetch(obj => obj.Expedidor)
                .Select(obj => obj.Expedidor != null ? obj.Expedidor : obj.Pedido.Remetente) // Prioriza o expedidor ao remetente
                .FirstOrDefault();
        }

        public double? BuscarCodigoClienteDaPrimeiraPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            return query
                .Where(obj => obj.Carga.Codigo == codigoCarga && (obj.Pedido.Remetente != null || obj.Pedido.Expedidor != null))
                .Fetch(obj => obj.Pedido).ThenFetch(obj => obj.Remetente)
                .Fetch(obj => obj.Expedidor)
                .Select(obj => obj.Expedidor != null ? obj.Expedidor.CPF_CNPJ : obj.Pedido.Remetente.CPF_CNPJ) // Prioriza o expedidor ao remetente
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPrimeiraPorCargaOrigem(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.CargaOrigem.Codigo == codigoCarga select obj;
            return result
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Remetente)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Tomador)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.Origem)
                .Fetch(obj => obj.Destino)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var resultCargaPedido = from obj in queryCargaPedido select obj;
            resultCargaPedido = resultCargaPedido.Where(obj => result.Select(o => o.Carga).Contains(obj.Carga));

            return resultCargaPedido.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.CargaCTes.Any(cte => cte.CTe.Codigo == codigoCTe));
            return query.FirstOrDefault();
        }

        public Task<IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoDadosParaCanhotoIntegracao>> BuscarDadosParaCanhotoIntegracao(int codigoCTe, List<int> codigosNotaFiscal, CancellationToken cancellationToken)
        {
            string sql = @"select
                    carped.CAR_CODIGO as CodigoCarga,
                    ped.PED_CODIGO as CodigoPedido,
                    car.CAR_CODIGO_CARGA_EMBARCADOR as NumeroCargaEmbarcador,
                    ped.PED_NUMERO_PEDIDO_EMBARCADOR as NumeroPedidoEmbarcador,
                    nfx.NF_NUMERO as NumeroNotaFiscal,
                    MIN(cen.CEN_DATA_INICIO_ENTREGA) as DataDeEntregaMaisAntiga
                    from T_CARGA_PEDIDO carped
                    inner join T_CARGA_CTE carcte on carped.CAR_CODIGO = carcte.CAR_CODIGO
                    inner join T_PEDIDO_XML_NOTA_FISCAL pnf on carped.CPE_CODIGO = pnf.CPE_CODIGO
                    inner join T_CARGA car on carped.CAR_CODIGO = car.CAR_CODIGO 
                    inner join T_PEDIDO ped on carped.PED_CODIGO = ped.PED_CODIGO
                    inner join T_XML_NOTA_FISCAL nfx on pnf.NFX_CODIGO = nfx.NFX_CODIGO 
                    left join T_CARGA_ENTREGA cen on cen.CAR_CODIGO = carped.CAR_CODIGO
                    where carcte.CON_CODIGO = :codigoCTe
                    and pnf.NFX_CODIGO in (:codigosNotaFiscal)
                    group by carped.CAR_CODIGO, ped.PED_CODIGO, car.CAR_CODIGO_CARGA_EMBARCADOR, ped.PED_NUMERO_PEDIDO_EMBARCADOR, nfx.NF_NUMERO";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetParameter("codigoCTe", codigoCTe);
            consulta.SetParameterList("codigosNotaFiscal", codigosNotaFiscal);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoDadosParaCanhotoIntegracao)));

            return consulta.ListAsync<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoDadosParaCanhotoIntegracao>(cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarlistaPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.CargaCTes.Any(cte => cte.CTe.Codigo == codigoCTe));
            return query.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga> BuscarTipoContratosPorCarregamento(int codigoCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Carregamento.Codigo == codigoCarregamento select obj;
            return result.Select(obj => obj.TipoContratacaoCarga).Distinct().ToList();
        }

        public bool PossuiTipoContratoNormalPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal select obj;
            return result.Any();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga> BuscarTipoContratosPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;
            return result.Select(obj => obj.TipoContratacaoCarga).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCargaParaRateioDescarga(int codigoCarga, int exceto, List<double> clientes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Codigo != exceto && clientes.Contains(obj.Pedido.Destinatario.CPF_CNPJ) select obj;
            return result
                .Fetch(obj => obj.Pedido)
                .Fetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.CFOP)
                .Fetch(obj => obj.Origem)
                .Fetch(obj => obj.Destino)
                .ToList();
        }

        public List<string> BuscarNumeroPedidosPorCarga(int codigoCarga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return consultaCargaPedido.Select(o => o.Pedido.NumeroPedidoEmbarcador).Distinct().ToList();
        }

        public List<string> BuscarNumeroPedidosClientePorCarga(int codigoCarga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return consultaCargaPedido.Select(o => o.Pedido.CodigoPedidoCliente).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargasPedidosPorCargas(List<int> codigosCarga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => codigosCarga.Contains(o.Carga.Codigo))
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Destinatario)
                .Fetch(obj => obj.Destino);
            return consultaCargaPedido.ToList();
        }

        public Task<List<Dominio.ObjetosDeValor.Embarcador.Pessoas.ClienteComplementar>> BuscarClientesComplementaresPorCargas(List<int> codigosCargas)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>().Where(o => codigosCargas.Contains(o.Carga.Codigo));

            var consultaClienteComplementar = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar>().Where(obj => consultaCargaPedido
                .Select(o => o.Pedido.Destinatario.CPF_CNPJ).Distinct().Contains(obj.Cliente.CPF_CNPJ))
                .Select(obj => new Dominio.ObjetosDeValor.Embarcador.Pessoas.ClienteComplementar
                {
                    Codigo = obj.Codigo,
                    Matriz = obj.Matriz,
                    EscritorioVendas = obj.EscritorioVendas,
                    CpfCnpjCliente = obj.Cliente.CPF_CNPJ
                });

            return consultaClienteComplementar.ToListAsync();
        }

        public List<string> BuscarVendedoresPorCarga(int codigoCarga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return consultaCargaPedido.Select(o => o.Pedido.Vendedor).Distinct().ToList();
        }

        public List<string> BuscarSupervisoresPorCarga(int codigoCarga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return consultaCargaPedido.Select(o => o.Pedido.FuncionarioSupervisor.Nome).Distinct().ToList();
        }

        public List<string> BuscarGerentesPorCarga(int codigoCarga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return consultaCargaPedido.Select(o => o.Pedido.FuncionarioGerente.Nome).Distinct().ToList();
        }

        public List<string> BuscarNomeFuncionariosVendedoresPorCarga(int codigoCarga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.Pedido.FuncionarioVendedor != null);

            return consultaCargaPedido.Select(o => o.Pedido.FuncionarioVendedor.Nome).Distinct().ToList();
        }

        public List<string> BuscarReservaPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.Select(o => o.Pedido.Reserva).Distinct().ToList();
        }

        public List<string> BuscarNumeroCargasPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Pedido.Codigo == codigoPedido && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada);

            return query.Select(o => o.Carga.CodigoCargaEmbarcador).ToList();
        }

        public List<string> BuscarPlacasVeiculosCargasPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Pedido.Codigo == codigoPedido && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada);

            return query.Select(o => o.Carga.PlacasVeiculos).ToList();
        }

        public List<string> BuscarNumeroFrotasVeiculosCargasPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Pedido.Codigo == codigoPedido && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada);

            return query.Select(o => o.Carga.NumeroFrotasVeiculos).ToList();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Pedido.TipoCarga> BuscarNumeroCargasPorPedidos(List<int> codigosPedidos)
        {
            string sqlQuery = @"
select PED.PED_CODIGO as Codigo
     , CAR.CAR_CODIGO_CARGA_EMBARCADOR as CodigoEmbarcador
	 , NULL as Descricao
  from T_CARGA_PEDIDO  CPE
     , T_PEDIDO        PED
	 , T_CARGA         CAR
 where CAR.CAR_CODIGO  = CPE.CAR_CODIGO
   AND CPE.PED_CODIGO  = PED.PED_CODIGO
   AND CAR.CAR_SITUACAO <> :situacao 
   AND PED.PED_CODIGO in ( :codigos ) 
 ORDER BY 1; ";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            query.SetParameter("situacao", SituacaoCarga.Cancelada);
            query.SetParameterList("codigos", codigosPedidos);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Pedido.TipoCarga)));

            return query.List<Dominio.ObjetosDeValor.Embarcador.Pedido.TipoCarga>();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga> BuscarTipoContratosSubContratacaoFilialEmissoraPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;
            return result.Select(obj => obj.TipoContratacaoCargaSubContratacaoFilialEmissora).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCargaCodigoIntegracao(string numeroCarga, string filial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.CargaOrigem.CodigoCargaEmbarcador == numeroCarga select obj;

            if (!string.IsNullOrWhiteSpace(filial))
                result = result.Where(obj => obj.CargaOrigem.Filial.CodigoFilialEmbarcador == filial || obj.CargaOrigem.Filial.OutrosCodigosIntegracao.Contains(filial));

            return result.ToList();
        }

        public decimal BuscarValorICMSParaComponenteCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.IncluirICMSBaseCalculo && obj.CST != "60" select obj;

            return result.Sum(o => (decimal?)o.ValorICMS) ?? 0m;
        }

        public decimal BuscarValorICMSInclusoParaComponenteCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.IncluirICMSBaseCalculo && obj.CST != "60" select obj;

            return result.Sum(o => (decimal?)o.ValorICMSIncluso) ?? 0m;
        }

        public decimal BuscarValorISSParaComponenteCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.IncluirISSBaseCalculo select obj;
            return result.Sum(o => (decimal?)o.ValorISS) ?? 0m;
        }

        public decimal BuscarValorPISCONFINSParaComponenteCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.IncluirISSBaseCalculo select obj;
            return result.Sum(o => (decimal?)(o.ValorPis + o.ValorCofins)) ?? 0m;
        }

        public bool VerificarSePossuiPedidoSemRecebedor(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Recebedor == null select obj.Codigo;
            return result.Any();
        }

        public string BuscarObservacaoPedidoPorCarga(int codigoCarga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga &&
                    o.Pedido.Observacao != null &&
                    o.Pedido.Observacao != ""
                );

            List<string> listaObservacao = consultaCargaPedido
                .OrderBy(o => o.Pedido.Observacao)
                .Select(o => o.Pedido.Observacao)
                .Distinct()
                .ToList();

            return string.Join(" | ", listaObservacao).Left(1000) ?? "";
        }

        public decimal BuscarPesoTotalPorCargaOrigem(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.CargaOrigem.Codigo == codigoCarga select obj;
            return result.Sum(obj => obj.Peso);
        }

        public decimal BuscarPesoTotalPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;
            return result.Sum(obj => obj.Peso);
        }

        public decimal BuscarPesoTotalPedidoPorCargaEDestinatario(int codigoCarga, double cpfCnpjDestinatario, bool utilizaPesoLiquido = false)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga && ((double?)o.Recebedor.CPF_CNPJ ?? o.Pedido.Destinatario.CPF_CNPJ) == cpfCnpjDestinatario && o.TipoCarregamentoPedido != TipoCarregamentoPedido.TrocaNota);

            if (utilizaPesoLiquido)
                return consultaCargaPedido.Sum(o => o.PesoLiquido > 0 ? (decimal?)o.PesoLiquido : (decimal?)o.Peso) ?? 0m;
            else
                return consultaCargaPedido.Sum(o => (decimal?)o.Peso) ?? 0m;

        }

        public bool ExistePorCargaEDestinatario(int codigoCarga, double cpfCnpjDestinatario)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga && ((double?)o.Recebedor.CPF_CNPJ ?? o.Pedido.Destinatario.CPF_CNPJ) == cpfCnpjDestinatario && o.TipoCarregamentoPedido != TipoCarregamentoPedido.TrocaNota);

            return consultaCargaPedido.Any();
        }

        public int BuscarQuantidadeVolumesPorCargaEDestinatarioRecebedor(int codigoCarga, double cpfCnpjDestino)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga && (o.Recebedor.CPF_CNPJ == cpfCnpjDestino || o.Pedido.Destinatario.CPF_CNPJ == cpfCnpjDestino))
                .Select(o => o.Pedido.Produtos.Count()).ToList();

            return consultaCargaPedido.Sum();
        }

        public decimal BuscarPesoLiquidoTotalPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga);

            return query.Sum(obj => obj.PesoLiquido);
        }

        public decimal BuscarPesoTotalPorCarregamento(int codigoCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Carregamento.Codigo == codigoCarregamento select obj;
            return result.Sum(obj => obj.Peso);
        }

        public decimal BuscarPesoLiquidoTotalPorCarregamento(int codigoCarregamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.Carga.Carregamento.Codigo == codigoCarregamento);

            return query.Sum(obj => obj.PesoLiquido);
        }

        public decimal BuscarCubagemTotalPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Codigo == codigoCargaPedido select obj;
            return result.Sum(obj => obj.Pedido.CubagemTotal);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCargasQueTeraoRedespacho(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query
                         where obj.Carga.Codigo == codigoCarga &&
           obj.Pedido.TipoOperacaoEmissao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao.VendaComRedespacho
                         select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorVeiculoTipoIntegracao(string placa, SituacaoCarga situacaoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => ((o.Carga.TipoOperacao != null && o.Carga.TipoOperacao.TipoIntegracao != null && o.Carga.TipoOperacao.TipoIntegracao.Tipo == tipoIntegracao) || (o.Carga.GrupoPessoaPrincipal != null && o.Carga.GrupoPessoaPrincipal.TipoIntegracao != null && o.Carga.GrupoPessoaPrincipal.TipoIntegracao.Tipo == tipoIntegracao))
                && (o.Carga.Veiculo.Placa == placa || o.Carga.VeiculosVinculados.Any(vv => vv.Placa == placa))
                && o.Carga.SituacaoCarga == situacaoCarga);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorVeiculoOrigemSituacao(string placa, int[] codigoOrigens, SituacaoCarga situacaoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.CTeEmitidoNoEmbarcador && codigoOrigens.Contains(o.Origem.Codigo) && (o.Carga.Veiculo.Placa == placa || o.Carga.VeiculosVinculados.Any(vv => vv.Placa == placa)) && o.Carga.SituacaoCarga == situacaoCarga);

            return query.ToList();
        }

        public void InformarCTesEmitidos(int carga)
        {
            string hql = "update CargaPedido cargaPedido set cargaPedido.CTesEmitidos = :CTesEmitidos where cargaPedido.Carga = :Carga and cargaPedido.SituacaoEmissao = :SituacaoEmissao and cargaPedido.CTesEmitidos = :CTesEmitidosAtual";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetBoolean("CTesEmitidos", true);
            query.SetBoolean("CTesEmitidosAtual", false);
            query.SetEnum("SituacaoEmissao", SituacaoNF.NFEnviada);
            query.SetInt32("Carga", carga);
            query.ExecuteUpdate();
        }

        //processo contrario quando emitido CRT, volta para etapa da nota para emitir o CTe
        public void InformarCTesNaoEmitidos(int carga)
        {
            string hql = "update CargaPedido cargaPedido set cargaPedido.CTesEmitidos = :CTesEmitidos where cargaPedido.Carga = :Carga and cargaPedido.SituacaoEmissao = :SituacaoEmissao and cargaPedido.CTesEmitidos = :CTesEmitidosAtual";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetBoolean("CTesEmitidos", false);
            query.SetBoolean("CTesEmitidosAtual", true);
            query.SetEnum("SituacaoEmissao", SituacaoNF.NFEnviada);
            query.SetInt32("Carga", carga);
            query.ExecuteUpdate();
        }

        public void InformarCTesEmitidosPedido(int carga)
        {
            //string hql = @"UPDATE pedido
            //                  set pedido.SituacaoPlanejamentoPedidoTMS = :SituacaoPlanejamentoPedidoTMS
            //                 FROM Pedido pedido
            //                 JOIN CargaPedido cargaPedido on pedido.Pedido = cargaPedido.Pedido
            //                where cargaPedido.Carga = :Carga";
            string hql = @"UPDATE CargaPedido cargaPedido
                              set cargaPedido.Pedido.SituacaoPlanejamentoPedidoTMS = :SituacaoPlanejamentoPedidoTMS                           
                            where cargaPedido.Carga = :Carga";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetEnum("SituacaoPlanejamentoPedidoTMS", SituacaoPlanejamentoPedidoTMS.CargaGerouDocumentacao);
            query.SetInt32("Carga", carga);
            query.ExecuteUpdate();
        }

        public void InformarCTesFilialEmissoraEmitidos(int carga)
        {
            string hql = "update CargaPedido cargaPedido set cargaPedido.CTesFilialEmissoraEmitidos = :CTesEmitidos where cargaPedido.Carga = :Carga and cargaPedido.SituacaoEmissao = :SituacaoEmissao and cargaPedido.CTesFilialEmissoraEmitidos = :CTesEmitidosAtual";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetBoolean("CTesEmitidos", true);
            query.SetBoolean("CTesEmitidosAtual", false);
            query.SetEnum("SituacaoEmissao", SituacaoNF.NFEnviada);
            query.SetInt32("Carga", carga);
            query.ExecuteUpdate();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> Consultar(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoes, int carga, bool somentePendentes, int numeroNF, int numeroCTe, string codigoPedidoEmbarcador, string codigoCargaEmbarcador, int origem, int destino, int filial, double remetente, double destinatario, double expedidorRecebedor, int redespacho, bool apenasEmpresaPermiteEncaixe, bool buscarPorCargaOrigem, string estadoDestino, bool liberadosParaRedespacho, List<int> codigosFiliais, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, List<int> cargasUtilizadas = null)
        {
            var result = MontarPesquisa(situacoes, carga, somentePendentes, numeroNF, numeroCTe, codigoPedidoEmbarcador, codigoCargaEmbarcador, origem, destino, filial, remetente, destinatario, expedidorRecebedor, redespacho, apenasEmpresaPermiteEncaixe, buscarPorCargaOrigem, estadoDestino, liberadosParaRedespacho, codigosFiliais, cargasUtilizadas);

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(obj => obj.Carga)
                .Fetch(obj => obj.Destino)
                .Fetch(obj => obj.Recebedor)
                .Fetch(obj => obj.Pedido).ThenFetch(obj => obj.Destinatario)
                .Fetch(obj => obj.Pedido).ThenFetch(obj => obj.CanalEntrega)
                .ToList();
        }

        public int ContarConsulta(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoes, int carga, bool somentePendentes, int numeroNF, int numeroCTe, string codigoPedidoEmbarcador, string codigoCargaEmbarcador, int origem, int destino, int filial, double remetente, double destinatario, double expedidorRecebedor, int redespacho, bool apenasEmpresaPermiteEncaixe, bool buscarPorCargaOrigem, string estadoDestino, bool liberadosParaRedespacho, List<int> codigosFiliais, List<int> cargasUtilizadas = null)
        {
            var result = MontarPesquisa(situacoes, carga, somentePendentes, numeroNF, numeroCTe, codigoPedidoEmbarcador, codigoCargaEmbarcador, origem, destino, filial, remetente, destinatario, expedidorRecebedor, redespacho, apenasEmpresaPermiteEncaixe, buscarPorCargaOrigem, estadoDestino, liberadosParaRedespacho, codigosFiliais, cargasUtilizadas);
            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidosPorSituacaoCarga(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga situacaoCarga, string codigoPedidoEmbarcador, int filial, double remetente, double destinatario, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query
                         where obj.Carga.SituacaoCarga == situacaoCarga
                         select obj;

            if (filial > 0)
                result = result.Where(obj => obj.Pedido.Filial.Codigo == filial);

            if (remetente > 0)
                result = result.Where(obj => obj.Pedido.Remetente.CPF_CNPJ == remetente);

            if (destinatario > 0)
                result = result.Where(obj => obj.Pedido.Destinatario.CPF_CNPJ == destinatario);

            if (!string.IsNullOrWhiteSpace(codigoPedidoEmbarcador))
                result = result.Where(obj => obj.Pedido.NumeroPedidoEmbarcador == codigoPedidoEmbarcador);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarCargaPedidosPorSituacaoCarga(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga situacaoCarga, string codigoPedidoEmbarcador, int filial, double remetente, double destinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query
                         where obj.Carga.SituacaoCarga == situacaoCarga
                         select obj;

            if (filial > 0)
                result = result.Where(obj => obj.Pedido.Filial.Codigo == filial);

            if (remetente > 0)
                result = result.Where(obj => obj.Pedido.Remetente.CPF_CNPJ == remetente);

            if (destinatario > 0)
                result = result.Where(obj => obj.Pedido.Destinatario.CPF_CNPJ == destinatario);

            if (!string.IsNullOrWhiteSpace(codigoPedidoEmbarcador))
                result = result.Where(obj => obj.Pedido.NumeroPedidoEmbarcador == codigoPedidoEmbarcador);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorNumeroCargaSituacao(string numeroCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga situacaoCarga, bool comTipoDeOperacaoAtivaParaRecebimento = false, bool cTeEmitidoNoEmbarcador = true)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.CodigoCargaEmbarcador == numeroCarga && obj.Carga.SituacaoCarga == situacaoCarga);

            if (cTeEmitidoNoEmbarcador)
                query = query.Where(obj => obj.CTeEmitidoNoEmbarcador == true);

            if (comTipoDeOperacaoAtivaParaRecebimento)
                query = query.Where(obj => obj.Carga.TipoOperacao != null && obj.Carga.TipoOperacao.ReceberCTesAverbacaoPorWebService == true);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorNumeroBookingSituacao(string numeroBooking, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga situacaoCarga, bool comTipoDeOperacaoAtivaParaRecebimento = false, bool cTeEmitidoNoEmbarcador = true)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Pedido.NumeroBooking == numeroBooking && obj.Carga.SituacaoCarga == situacaoCarga);

            if (cTeEmitidoNoEmbarcador)
                query = query.Where(obj => obj.CTeEmitidoNoEmbarcador == true);

            if (comTipoDeOperacaoAtivaParaRecebimento)
                query = query.Where(obj => obj.Carga.TipoOperacao != null && obj.Carga.TipoOperacao.ReceberCTesAverbacaoPorWebService == true);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorNumeroOSSituacao(string numeroOS, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga situacaoCarga, bool comTipoDeOperacaoAtivaParaRecebimento = false, bool cTeEmitidoNoEmbarcador = true)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Pedido.NumeroOS == numeroOS && obj.Carga.SituacaoCarga == situacaoCarga);

            if (cTeEmitidoNoEmbarcador)
                query = query.Where(obj => obj.CTeEmitidoNoEmbarcador == true);

            if (comTipoDeOperacaoAtivaParaRecebimento)
                query = query.Where(obj => obj.Carga.TipoOperacao != null && obj.Carga.TipoOperacao.ReceberCTesAverbacaoPorWebService == true);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorNumeroOSMaeSituacao(string numeroOS, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga situacaoCarga, bool comTipoDeOperacaoAtivaParaRecebimento = false, bool cTeEmitidoNoEmbarcador = true)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var queryPedidoAdicional = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional>();
            queryPedidoAdicional = queryPedidoAdicional.Where(obj => obj.NumeroOSMae == numeroOS);

            query = query.Where(obj => queryPedidoAdicional.Any(a => a.Pedido == obj.Pedido) && obj.Carga.SituacaoCarga == situacaoCarga);

            if (cTeEmitidoNoEmbarcador)
                query = query.Where(obj => obj.CTeEmitidoNoEmbarcador == true);

            if (comTipoDeOperacaoAtivaParaRecebimento)
                query = query.Where(obj => obj.Carga.TipoOperacao != null && obj.Carga.TipoOperacao.ReceberCTesAverbacaoPorWebService == true);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorVeiculoOrigemSituacao(int veiculo, int origem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga situacaoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.CTeEmitidoNoEmbarcador && obj.Origem.Codigo == origem && (obj.Carga.Veiculo.Codigo == veiculo || obj.Carga.VeiculosVinculados.Any(o => o.Codigo == veiculo)) && obj.Carga.SituacaoCarga == situacaoCarga select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorNumeroPedidoEmbarcador(string numeroPedidoEmbarcador, bool cteEmitidoNoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga situacaoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.CTeEmitidoNoEmbarcador == cteEmitidoNoEmbarcador && o.Pedido.NumeroPedidoEmbarcador == numeroPedidoEmbarcador && o.Carga.SituacaoCarga == situacaoCarga);

            return query.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>> BuscarPorNumeroPedidoEmbarcadorAsync(string numeroPedidoEmbarcador, bool cteEmitidoNoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga situacaoCarga, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.CTeEmitidoNoEmbarcador == cteEmitidoNoEmbarcador && o.Pedido.NumeroPedidoEmbarcador == numeroPedidoEmbarcador && o.Carga.SituacaoCarga == situacaoCarga);

            return query.ToListAsync(cancellationToken);
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPrimeiroPorNumeroPedidoEmbarcador(string numeroPedidoEmbarcador, bool cteEmitidoNoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga situacaoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.CTeEmitidoNoEmbarcador == cteEmitidoNoEmbarcador && o.Pedido.NumeroPedidoEmbarcador == numeroPedidoEmbarcador && o.Carga.SituacaoCarga == situacaoCarga);

            return query.OrderBy(o => o.Carga.DataCriacaoCarga).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPrimeiroPorNumeroPedidoEmbarcadorEGrupoPessoas(string numeroPedidoEmbarcador, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, bool cteEmitidoNoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga situacaoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.CTeEmitidoNoEmbarcador == cteEmitidoNoEmbarcador && o.Pedido.NumeroPedidoEmbarcador == numeroPedidoEmbarcador && o.Carga.SituacaoCarga == situacaoCarga && (o.Carga.GrupoPessoaPrincipal == grupoPessoas || o.Pedido.GrupoPessoas == grupoPessoas || o.Pedido.Remetente.GrupoPessoas == grupoPessoas));

            return query.OrderBy(o => o.Carga.DataCriacaoCarga).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPrimeiroPorGrupoPessoasEVeiculo(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Dominio.Entidades.Veiculo veiculo, bool cteEmitidoNoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga situacaoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.CTeEmitidoNoEmbarcador == cteEmitidoNoEmbarcador && o.Carga.SituacaoCarga == situacaoCarga && o.Carga.Veiculo == veiculo && (o.Carga.GrupoPessoaPrincipal == grupoPessoas || o.Pedido.GrupoPessoas == grupoPessoas || o.Pedido.Remetente.GrupoPessoas == grupoPessoas));

            return query.OrderBy(o => o.Carga.DataCriacaoCarga).FirstOrDefault();
        }

        public bool VerificarSeOperacaoTeraEmissaoFilialEmissoraPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == carga select obj;
            return result.Any(obj => obj.CargaPedidoFilialEmissora);
        }

        public Task<bool> VerificarSeOperacaoTeraEmissaoFilialEmissoraPorCargaAsync(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == carga select obj;
            return result.AnyAsync(obj => obj.CargaPedidoFilialEmissora);
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarCargaPedidoPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;
            return result.FirstOrDefault();
        }

        public int VerificarPorCargaSePossuiValorZerado(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == carga && obj.ValorFreteAPagar <= 0 && obj.ValorFrete <= 0 select obj;
            return result.Count();
        }

        public int VerificarSePossuiValorZerado(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == carga && obj.Carga.ValorFrete > 0 && obj.ValorFrete == 0 && obj.ValorFreteAPagar > 0.01m && !obj.PedidoSemNFe select obj;
            return result.Count();
        }

        public int VerificarSeNaoPossuiPedidoLiberadoSemNFe(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == carga && obj.PedidoSemNFe == false select obj;
            return result.Count();
        }

        public bool VerificarPorCargaSePossuiComplementoAEmitirPornota(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == carga && obj.EmitirComplementarFilialEmissora == true && (obj.TipoRateio == TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada || obj.TipoRateio == TipoEmissaoCTeDocumentos.EmitePorNotaFiscalIndividual || obj.TipoRateio == TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupadaEntrePedidos) select obj;
            return result.Any();
        }

        public bool VerificarPorCargaSePendenteDadosRecebedor(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == carga && obj.AgInformarRecebedor select obj;
            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarCargaPedidoPorCargaePedidoCodigoIntegracao(string numeroCarga, string numeroPedido, string filial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query
                         where obj.Pedido.CodigoCargaEmbarcador == numeroPedido && obj.CargaOrigem.CodigoCargaEmbarcador == numeroCarga
        && obj.CargaOrigem.SituacaoCarga != SituacaoCarga.Cancelada && obj.CargaOrigem.SituacaoCarga != SituacaoCarga.Anulada
                         select obj;

            if (!string.IsNullOrWhiteSpace(filial))
                result = result.Where(obj => obj.Carga.Filial.CodigoFilialEmbarcador == filial);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidoPorPedidoCodigoIntegracao(string numeroPedido, string filial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Pedido.NumeroPedidoEmbarcador == numeroPedido select obj;

            if (!string.IsNullOrWhiteSpace(filial))
                result = result.Where(obj => obj.CargaOrigem.Filial.CodigoFilialEmbarcador == filial || obj.CargaOrigem.Filial.OutrosCodigosIntegracao.Contains(filial));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidoPorNumeroPedidoEmbarcador(string numeroPedido, string filial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Pedido.NumeroPedidoEmbarcador == numeroPedido select obj;

            if (!string.IsNullOrWhiteSpace(filial))
                result = result.Where(obj => obj.CargaOrigem.Filial.CodigoFilialEmbarcador == filial);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidoParaVincularNotasPorNumeroPedido(string numeroPedido)
        {
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesCarga = new SituacaoCarga[] { SituacaoCarga.Nova, SituacaoCarga.AgNFe };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => (o.Pedido.NumeroPedidoEmbarcador == numeroPedido || o.Pedido.NumeroPedidoEmbarcador.StartsWith(numeroPedido + "_")) && situacoesCarga.Contains(o.Carga.SituacaoCarga));

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarCargaPedidoParaVincularCTesPorNumeroBooking(string numeroBooking, string numerocontainer, double remetente, double tomador)
        {
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesCarga = new SituacaoCarga[] { SituacaoCarga.Nova, SituacaoCarga.AgNFe };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Pedido.NumeroBooking == numeroBooking && o.Pedido.Container.Numero == numerocontainer && situacoesCarga.Contains(o.Carga.SituacaoCarga) && o.Pedido.Remetente.CPF_CNPJ == remetente && o.Pedido.Tomador.CPF_CNPJ == tomador);
            query = query.Where(o => o.Carga.TipoOperacao != null && (o.Carga.TipoOperacao.ExpressaoRegularNumeroBookingObservacaoCTe == true || o.Carga.TipoOperacao.ExpressaoRegularNumeroContainerObservacaoCTe == true));

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarCargaPedidoParaVincularCTesPorNumeroBooking(string numeroBooking, string numerocontainer)
        {
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesCarga = new SituacaoCarga[] { SituacaoCarga.Nova, SituacaoCarga.AgNFe };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Pedido.NumeroBooking == numeroBooking && o.Pedido.Container.Numero == numerocontainer && situacoesCarga.Contains(o.Carga.SituacaoCarga));
            query = query.Where(o => o.Carga.TipoOperacao != null && (o.Carga.TipoOperacao.ExpressaoRegularNumeroBookingObservacaoCTe == true || o.Carga.TipoOperacao.ExpressaoRegularNumeroContainerObservacaoCTe == true));

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarCargaPedidoParaVincularCTesPorNumeroBooking(string numeroBooking)
        {
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesCarga = new SituacaoCarga[] { SituacaoCarga.Nova, SituacaoCarga.AgNFe };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Pedido.NumeroBooking == numeroBooking && situacoesCarga.Contains(o.Carga.SituacaoCarga));
            query = query.Where(o => o.Carga.TipoOperacao != null && (o.Carga.TipoOperacao.ExpressaoRegularNumeroBookingObservacaoCTe == true || o.Carga.TipoOperacao.ExpressaoRegularNumeroContainerObservacaoCTe == true));

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidoParaVincularNotasPorNumeroBooking(string numeroBooking, string numerocontainer)
        {
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesCarga = new SituacaoCarga[] { SituacaoCarga.Nova, SituacaoCarga.AgNFe };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Pedido.NumeroBooking == numeroBooking && o.Pedido.Container.Numero == numerocontainer && situacoesCarga.Contains(o.Carga.SituacaoCarga));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidoParaVincularNotasPorNumeroBooking(string numeroBooking, bool semContainer)
        {
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesCarga = new SituacaoCarga[] { SituacaoCarga.Nova, SituacaoCarga.AgNFe };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Pedido.NumeroBooking == numeroBooking && situacoesCarga.Contains(o.Carga.SituacaoCarga));
            if (semContainer)
                query = query.Where(o => o.Pedido.Container == null);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidoParaVincularNotasPorOrigemDataEVeiculo(double cpfCnpjRemetente, DateTime dataCarregamento, string placaVeiculo)
        {
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesCarga = new SituacaoCarga[] { SituacaoCarga.Nova, SituacaoCarga.AgNFe };

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Pedido.Remetente.CPF_CNPJ == cpfCnpjRemetente && o.Pedido.DataCarregamentoPedido.Value.Date == dataCarregamento && (o.Carga.Veiculo.Placa == placaVeiculo || o.Carga.VeiculosVinculados.Any(v => v.Placa == placaVeiculo)) && situacoesCarga.Contains(o.Carga.SituacaoCarga));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidoParaVincularNotasPorOrigemDestinoDataEVeiculo(double cpfCnpjRemetente, double cpfCnpjDestinatario, DateTime dataCarregamento, string placaVeiculo)
        {
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesCarga = new SituacaoCarga[] { SituacaoCarga.Nova, SituacaoCarga.AgNFe };

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Pedido.Remetente.CPF_CNPJ == cpfCnpjRemetente && o.Pedido.Destinatario.CPF_CNPJ == cpfCnpjDestinatario && o.Pedido.DataCarregamentoPedido.Value.Date == dataCarregamento && (o.Carga.Veiculo.Placa == placaVeiculo || o.Carga.VeiculosVinculados.Any(v => v.Placa == placaVeiculo)) && situacoesCarga.Contains(o.Carga.SituacaoCarga));

            return query.ToList();
        }

        public bool PossuiCargaInaptaAAlteracao(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Pedido.Codigo == codigoPedido);

            return query.Any(o => o.Carga.SituacaoCarga != SituacaoCarga.Nova && o.Carga.SituacaoCarga != SituacaoCarga.AgNFe && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;
            return result.ToList();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorPedidoETomadorAsync(int codigoPedido, double codigoEmitente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(obj => obj.Pedido.Codigo == codigoPedido && obj.ObterTomador().CPF_CNPJ == codigoEmitente);

            return query.FirstOrDefaultAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorPedidoNaCarga(int codigoPedido)
        {
            var consultaCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(cargaPedido => cargaPedido.Pedido.Codigo == codigoPedido)
                .Select(cargaPedido => cargaPedido.Carga.Codigo);

            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(cargaPedido => consultaCarga.Contains(cargaPedido.Carga.Codigo));

            return consultaCargaPedido.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPrimeiroPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;
            return result.Fetch(obj => obj.Carga).ThenFetch(obj => obj.Empresa).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorPedidoComCargaAtiva(int codigoPedido)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o =>
                    o.Pedido.Codigo == codigoPedido &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Anulada
                );

            return consultaCargaPedido
                .Fetch(o => o.Carga)
                .ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>> BuscarPorPedidoComCargaAtivaAsync(int codigoPedido)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o =>
                    o.Pedido.Codigo == codigoPedido &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Anulada
                );

            return consultaCargaPedido
                .Fetch(o => o.Carga)
                .ToListAsync(CancellationToken);
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPrimeiroPorPedidoComCargaAtivaAsync(int codigoPedido)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o =>
                    o.Pedido.Codigo == codigoPedido &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Anulada
                );

            return consultaCargaPedido
                .Fetch(o => o.Carga)
                .FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Cargas.Carga BuscarPreCargaPorNumeroCargaVincularPreCarga(string numeroPedidoEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query
                         where obj.Pedido.NumeroPedidoEmbarcador == numeroPedidoEmbarcador
       && obj.Carga.CargaDePreCarga && (obj.Carga.CargaFechada || (!obj.Carga.CargaFechada && obj.Carga.CargaAgrupamento != null)) && obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada
       && obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada
                         select obj.Carga;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarCargaAtualPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada orderby obj.Carga.Codigo descending select obj;
            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaAtualPorPedidoAsync(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada orderby obj.Carga.Codigo descending select obj;
            return result.FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarCargaAtualPorProtocoloPedido(int protocoloPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Pedido.Protocolo == protocoloPedido && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada orderby obj.Carga.Codigo descending select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarCargaAtualPorProtocoloCarga(int protocoloCarga, bool pegarSemContainer = false)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Protocolo == protocoloCarga && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada);

            if (pegarSemContainer)
                query = query.Where(obj => obj.Pedido.Container == null);

            return query
               .OrderByDescending(o => o.Carga.Codigo)
               .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.Carga BuscarCargaPorProtocoloPedido(int protocoloPedido)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Pedido.Protocolo == protocoloPedido && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada
                            && (o.Carga.DadosSumarizados == null || o.Carga.DadosSumarizados.CargaTrecho == null || o.Carga.DadosSumarizados.CargaTrecho == CargaTrechoSumarizada.Agrupadora));

            return consultaCargaPedido
                .Select(o => o.Carga)
                .OrderByDescending(o => o.Codigo)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorPedidoDiferenteCargaPedido(int codigoPedido, int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Pedido.Protocolo == codigoPedido && obj.Codigo != cargaPedido select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorProtocoloPedido(int protocoloPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Pedido.Protocolo == protocoloPedido);

            return query.Fetch(obj => obj.Pedido)
                        .Fetch(obj => obj.Carga)
                        .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPrimeiroPorProtocoloPedido(int protocoloPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Pedido.Protocolo == protocoloPedido);

            return query.Fetch(obj => obj.Pedido)
                        .Fetch(obj => obj.Carga)
                        .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPrimeiroPorProtocoloCarga(int protocoloCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Protocolo == protocoloCarga);

            return query.Fetch(obj => obj.Pedido)
                        .Fetch(obj => obj.Carga)
                        .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorContainerECarga(int codigoContainer, int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Pedido.Container.Codigo == codigoContainer && o.Carga.Codigo == codigoCarga);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorNumeroControlePedido(string numeroControle)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Pedido.NumeroControle == numeroControle);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidoNaoPossuiNumeroControlePedido(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => (o.Pedido.NumeroControle == null || o.Pedido.NumeroControle == "") && o.Carga.Codigo == codigoCarga);

            return query.ToList();
        }

        public bool PossuiCargaPedidoPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return consultaCargaPedido.Any();
        }

        public int ContarPorCarga(int codigoCarga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return consultaCargaPedido.Count();
        }

        public int ContarPorCargaESituacao(int codigoCarga, SituacaoPedido situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Pedido.SituacaoPedido == situacao select obj.Codigo;
            return result.Count();
        }

        public List<int> BuscarCodigoCargaPorPedidosComTipoOperacao(List<int> codigoPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(cp => codigoPedidos.Contains(cp.Pedido.Codigo)
            && cp.Carga.SituacaoCarga != SituacaoCarga.Cancelada
            && cp.Carga.Redespacho != null);

            return query.Select(p => p.Carga.Codigo).Distinct().ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.Carga BuscarCargaComComTipoOperacaoDisponivelParaCarrementoDiferenteDaAtual(int codigoPedido, int codigoCargaDiferente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(cp => cp.Pedido.Codigo == codigoPedido && cp.Carga.Codigo != codigoCargaDiferente
            && cp.Carga.TipoOperacao != null
            && cp.Carga.TipoOperacao.ConfiguracaoCarga != null
            && cp.Carga.TipoOperacao.ConfiguracaoCarga.DeixarPedidosDisponiveisParaMontegemCarga == true);

            return query.Select(cp => cp.Carga).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarCargaComComTipoOperacaoDisponivelParaCarrementoDiferenteDaAtual(List<int> codigosPedido, int codigoCargaDiferente)
        {
            List<Dominio.Entidades.Embarcador.Cargas.Carga> result = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

            if (codigosPedido?.Count == 0)
                return result;

            int take = 1000;
            int start = 0;

            while (start < codigosPedido.Count)
            {
                List<int> tmp = codigosPedido.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                var filter = from obj in query
                             where tmp.Contains(obj.Pedido.Codigo) && obj.Carga.Codigo != codigoCargaDiferente
                             && obj.Carga.TipoOperacao != null
                             && obj.Carga.TipoOperacao.ConfiguracaoCarga != null
                             && obj.Carga.TipoOperacao.ConfiguracaoCarga.DeixarPedidosDisponiveisParaMontegemCarga
                             select obj.Carga;

                result.AddRange(filter.Fetch(o => o.Pedidos).ToList());

                start += take;
            }

            return result;
        }

        public async Task<List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOrigemPedidos>> BuscarCargaComComTipoOperacaoDisponivelParaCarrementoDiferenteDaAtualAsync(List<int> codigosPedido, int codigoCargaDiferente)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOrigemPedidos> result = new List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOrigemPedidos>();

            if (codigosPedido?.Count == 0)
                return result;

            int take = 2000;
            int start = 0;

            while (start < codigosPedido.Count)
            {
                List<int> tmp = codigosPedido.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                var filter = from obj in query
                             where tmp.Contains(obj.Pedido.Codigo) && obj.Carga.Codigo != codigoCargaDiferente
                             && obj.Carga.TipoOperacao != null
                             && obj.Carga.TipoOperacao.ConfiguracaoCarga != null
                             && obj.Carga.TipoOperacao.ConfiguracaoCarga.DeixarPedidosDisponiveisParaMontegemCarga
                             select obj;

                result.AddRange(await filter
                    .Select(x => new Dominio.ObjetosDeValor.Embarcador.Carga.CargaOrigemPedidos()
                    {
                        Codigo = x.Carga.Codigo,
                        CodigoPedido = x.Pedido.Codigo,
                    }).ToListAsync());

                start += take;
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCargaEstado(int codigoCarga, string siglaEstado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Destino.Estado.Sigla == siglaEstado select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPendetesNotasFiscais(bool exigeCienciaParaEmissao, int codigoGrupoPessoas, int inicio, int limite, string codigoTipoOperacao, bool naoRetornarCargasCanceladas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.Pedido.TipoPedido == TipoPedido.Entrega &&
                                       (obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe ||
                                       (exigeCienciaParaEmissao && obj.Carga.SituacaoCarga == SituacaoCarga.CalculoFrete && obj.Carga.ExigeNotaFiscalParaCalcularFrete && obj.SituacaoEmissao == SituacaoNF.NFEnviada)) &&
                                       !obj.CienciaDoEnvioDaNotaInformado);

            if (exigeCienciaParaEmissao)
                query = query.Where(obj => obj.SituacaoEmissao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.AgEnvioNF || (obj.SituacaoEmissao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada && !obj.CienciaDoEnvioDaNotaInformado));
            else
                query = query.Where(obj => obj.SituacaoEmissao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.AgEnvioNF);

            if (codigoGrupoPessoas > 0)
                query = query.Where(o => o.Carga.GrupoPessoaPrincipal.Codigo == codigoGrupoPessoas);

            if (naoRetornarCargasCanceladas)
                query = query.Where(obj => obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada);

            if (!string.IsNullOrWhiteSpace(codigoTipoOperacao))
                query = query.Where(o => o.Carga.TipoOperacao.CodigoIntegracao == codigoTipoOperacao);

            if (inicio > 0)
                query = query.Skip(inicio);

            if (limite > 0)
                query = query.Take(limite);

            return query.ToList();
        }

        public int ContarPendetesNotasFiscais(int codigoGrupoPessoas, string codigoTipoOperacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.SituacaoEmissao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.AgEnvioNF &&
                                       obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe &&
                                       !obj.CienciaDoEnvioDaNotaInformado);

            if (codigoGrupoPessoas > 0)
                query = query.Where(o => o.Carga.GrupoPessoaPrincipal.Codigo == codigoGrupoPessoas);

            if (!string.IsNullOrWhiteSpace(codigoTipoOperacao))
                query = query.Where(o => o.Carga.TipoOperacao.CodigoIntegracao == codigoTipoOperacao);

            return query.Count();
        }

        public bool ExisteCargaPedidoSemNota(int protocolaCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.SituacaoEmissao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.AgEnvioNF &&
                                       obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe &&
                                       obj.Carga.Protocolo == protocolaCarga);
            return query.Any();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPedidoPorCargaENumeroPedidoEmbarcador(int codigoCarga, string numeroPedidoEmbarcador)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.Pedido.NumeroPedidoEmbarcador == numeroPedidoEmbarcador);

            return consultaCargaPedido.Select(o => o.Pedido).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPedidoPorCargaEContainer(int codigoCarga, int codigoContainer)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.Pedido.Container.Codigo == codigoContainer);

            return consultaCargaPedido.Select(o => o.Pedido).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorCargaEPedido(int codigoCarga, int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Pedido.Codigo == codigoPedido select obj;
            return result.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCargaEPedidoAsync(int codigoCarga, int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Pedido.Codigo == codigoPedido select obj;
            return await result.FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorCargaEPedidoFetch(int codigoCarga, int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Pedido.Codigo == codigoPedido select obj;
            return result.Fetch(obj => obj.Pedido).Fetch(obj => obj.Carga).Fetch(obj => obj.CargaPedidoProximoTrecho).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorPedidoECargaDiferente(int codigoCarga, int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(o => o.Carga.SituacaoCarga != SituacaoCarga.Anulada && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada);

            var result = from obj in query where obj.Carga.Codigo != codigoCarga && obj.Pedido.Codigo == codigoPedido select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCargaEPedidos(int codigoCarga, List<int> codigosPedidos)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga && codigosPedidos.Contains(o.Pedido.Codigo));

            return consultaCargaPedido.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorCargaEProtocoloPedido(int codigoCarga, int protocoloPedido)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.Pedido.Protocolo == protocoloPedido);

            return consultaCargaPedido.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPrimeiroPorProtocoloCargaEProtocoloPedido(int protocoloCarga, int protocoloPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.CargaOrigem.Protocolo == protocoloCarga && o.Pedido.Protocolo == protocoloPedido);

            return query.OrderByDescending(o => o.Codigo)
                .Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Origem).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Destino).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Remetente).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Destinatario).
                Fetch(obj => obj.Origem).
                Fetch(obj => obj.Destino).
                Fetch(obj => obj.Expedidor).
                Fetch(obj => obj.Recebedor).
                Fetch(obj => obj.Carga).
                FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorProtocoloCargaEProtocoloPedido(int protocoloCarga, int protocoloPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Protocolo == protocoloCarga && o.Pedido.Protocolo == protocoloPedido);

            return query
                .OrderByDescending(o => o.Codigo)
                .Fetch(obj => obj.Pedido).ThenFetch(obj => obj.Origem)
                .Fetch(obj => obj.Pedido).ThenFetch(obj => obj.Destino)
                .Fetch(obj => obj.Pedido).ThenFetch(obj => obj.Remetente)
                .Fetch(obj => obj.Pedido).ThenFetch(obj => obj.Destinatario)
                .Fetch(obj => obj.Origem).Fetch(obj => obj.Destino)
                .Fetch(obj => obj.Expedidor)
                .Fetch(obj => obj.Recebedor)
                .Fetch(obj => obj.Carga).ThenFetch(obj => obj.TipoOperacao)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarListaPorProtocoloCargaOrigemEProtocoloPedido(int protocoloCarga, int protocoloPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.CargaOrigem.Protocolo == protocoloCarga && o.Pedido.Protocolo == protocoloPedido);

            return query.OrderByDescending(o => o.Codigo)
                .Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Origem).
                /*
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Destino).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Remetente).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Destinatario).
                */
                Fetch(obj => obj.Origem).
                Fetch(obj => obj.Destino).
                Fetch(obj => obj.Expedidor).
                Fetch(obj => obj.Recebedor).
                Fetch(obj => obj.Carga).
                ThenFetch(obj => obj.TipoOperacao).
                ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarListaPorProtocoloCargaOrigemEProtocoloPedido(List<int> protocolosCarga, int protocoloPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => protocolosCarga.Contains(o.CargaOrigem.Protocolo) && o.Pedido.Protocolo == protocoloPedido);

            return query.OrderByDescending(o => o.Codigo)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Origem)
                .Fetch(obj => obj.Origem)
                .Fetch(obj => obj.Destino)
                .Fetch(obj => obj.Expedidor)
                .Fetch(obj => obj.Recebedor)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.TipoOperacao)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarListaPorDataCarregamento(DateTime dataDe, DateTime dataAte, int codigoEmpresa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.DataCarregamentoCarga >= dataDe && o.Carga.DataCarregamentoCarga <= dataAte);

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Carga.Empresa.Codigo == codigoEmpresa);

            return query.OrderByDescending(o => o.Codigo)
                .Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Origem).
                /*
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Destino).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Remetente).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Destinatario).
                */
                Fetch(obj => obj.Origem).
                Fetch(obj => obj.Destino).
                Fetch(obj => obj.Expedidor).
                Fetch(obj => obj.Recebedor).
                Fetch(obj => obj.Carga).
                ThenFetch(obj => obj.TipoOperacao).
                ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarListaPorPeriodo(Periodo periodo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            if (periodo.DataCriacaoInicial != DateTime.MinValue && periodo.DataCriacaoFinal != DateTime.MinValue)
                query = query.Where(o => o.Carga.DataCriacaoCarga >= periodo.DataCriacaoInicial && o.Carga.DataCriacaoCarga <= periodo.DataCriacaoFinal);

            if (periodo.DataCarregamentoInicial != DateTime.MinValue && periodo.DataCarregamentoFinal != DateTime.MinValue)
                query = query.Where(o => o.Carga.DataCarregamentoCarga >= periodo.DataCarregamentoInicial && o.Carga.DataCarregamentoCarga <= periodo.DataCarregamentoFinal);

            return query.OrderByDescending(o => o.Codigo).Fetch(obj => obj.Carga).ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorProtocoloCargaOrigemEContainerPedido(int protocoloCarga, string numeroContainer)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.CargaOrigem.Protocolo == protocoloCarga && o.Pedido.Container.Numero == numeroContainer);

            return query.OrderByDescending(o => o.Codigo)
                .Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Origem).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Destino).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Remetente).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Destinatario).
                Fetch(obj => obj.Origem).
                Fetch(obj => obj.Destino).
                Fetch(obj => obj.Expedidor).
                Fetch(obj => obj.Recebedor).
                Fetch(obj => obj.Carga).
                ThenFetch(obj => obj.TipoOperacao).Timeout(6000).
                FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorProtocoloCargaOrigemEProtocoloPedido(int protocoloCarga, int protocoloPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.CargaOrigem.Protocolo == protocoloCarga && o.Pedido.Protocolo == protocoloPedido);

            return query.OrderByDescending(o => o.Codigo)
                .Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Origem).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Destino).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Remetente).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Destinatario).
                Fetch(obj => obj.Origem).
                Fetch(obj => obj.Destino).
                Fetch(obj => obj.Expedidor).
                Fetch(obj => obj.Recebedor).
                Fetch(obj => obj.Carga).
                ThenFetch(obj => obj.TipoOperacao).Timeout(6000).
                FirstOrDefault();
        }

        public List<string> BuscarPorCargaEClienteDestinatario(int codigoCarga, double cliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            if (cliente > 0)
                result = result.Where(o => o.Pedido.Destinatario.CPF_CNPJ == cliente);

            return result
                .Select(s => s.Pedido.NumeroPedidoEmbarcador)
                .Distinct()
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidoPorCargaEClienteDestinatario(int codigoCarga, double cliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            if (cliente > 0)
                result = result.Where(o => o.Pedido.Destinatario.CPF_CNPJ == cliente);

            return result.Fetch(o => o.Pedido).ThenFetch(o => o.Remetente).ToList();
        }

        public Task<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPedidoPorCargaEClienteDestinatarioAsync(int codigoCarga, double cliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(obj => obj.Carga.Codigo == codigoCarga);

            if (cliente > 0)
                query = query.Where(o => o.Pedido.Destinatario.CPF_CNPJ == cliente);

            return query.Select(obj => obj.Pedido).FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorCargaEPedidoCodigoIntegracao(string numeroCarga, string numeroPedido, string filial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.CargaOrigem.CodigoCargaEmbarcador == numeroCarga && obj.Pedido.NumeroPedidoEmbarcador == numeroPedido select obj;

            if (!string.IsNullOrWhiteSpace(filial))
                result = result.Where(obj => obj.CargaOrigem.Filial.CodigoFilialEmbarcador == filial);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorProtocoloCargaEProtocoloPedidoAutorizados(int protocoloCarga, int protocoloPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.CargaOrigem.Protocolo == protocoloCarga && o.Pedido.Protocolo == protocoloPedido);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorProtocoloCargaEProtocoloPedidoAutorizados(List<int> protocolosCarga, int protocoloPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => protocolosCarga.Contains(o.CargaOrigem.Protocolo) && o.Pedido.Protocolo == protocoloPedido);

            return query.ToList();
        }

        public List<int> BuscarCodigosPorCargaEncaixar(string numeroCarga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Pedido.NumeroCargaEncaixar == numeroCarga && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada);

            return consultaCargaPedido.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos> BuscarTipoRateioPorCarga(int codigoCarga, bool ignorarCargaPedidoSimplificado = false)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            if (ignorarCargaPedidoSimplificado)
                query = query.Where(o => o.Carga.Codigo == codigoCarga && !o.IndicadorCTeSimplificado);
            else
                query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.Select(o => o.TipoRateio).Distinct().ToList();
        }

        public bool NaoContemDestinatarioPedido(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.Any(o => o.Pedido.Destinatario == null);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPedidosPorPedidoEncaixado(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.CargaPedidoEncaixe.Codigo == codigoPedido select obj;

            return result.ToList();
        }

        public bool PossuiMontagemContainerNaCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            return query.Any(c => c.Pedido.MontagemContainer != null);
        }

        public async Task<Dominio.ObjetosDeValor.Embarcador.Integracao.SkyMark.Percurso> BuscarPercursoPorCargaAsync(int codigoCarga, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(cp => cp.Carga.Codigo == codigoCarga)
                    .SelectMany(cp =>
                        from lo in this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>()
                            .Where(lo => lo.Codigo == cp.Origem.Codigo).DefaultIfEmpty()
                        from ld in this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>()
                            .Where(ld => ld.Codigo == cp.Destino.Codigo).DefaultIfEmpty()
                        select new Dominio.ObjetosDeValor.Embarcador.Integracao.SkyMark.Percurso
                        {
                            CidadeOrigem = lo != null ? lo.Descricao : null,
                            UfOrigem = lo != null ? lo.Estado.Sigla : null,
                            CidadeDestino = ld != null ? ld.Descricao : null,
                            UfDestino = ld != null ? ld.Estado.Sigla : null
                        });
            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPedidosPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> queryPedido = result.Select(obj => obj.Pedido);

            return queryPedido
                .Fetch(obj => obj.Remetente)
                .Fetch(obj => obj.Destinatario).ThenFetch(x => x.MesoRegiao)
                .Fetch(obj => obj.Empresa).ThenFetch(obj => obj.Localidade).ThenFetch(obj => obj.Estado)
                .ToList();
        }

        public Dominio.Entidades.ModeloDocumentoFiscal ObterModeloDocumentoFiscal(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == codigoCarga);
            return query.FirstOrDefault()?.ModeloDocumentoFiscal ?? null;
        }

        public Task<List<Dominio.Entidades.Embarcador.Pedidos.Pedido>> BuscarPedidosPorCargaAsync(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> queryPedido = result.Select(obj => obj.Pedido);

            return queryPedido
                .Fetch(obj => obj.Remetente)
                .Fetch(obj => obj.Destinatario).ThenFetch(x => x.MesoRegiao)
                .Fetch(obj => obj.Empresa).ThenFetch(obj => obj.Localidade).ThenFetch(obj => obj.Estado)
                .ToListAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPrimeiroPedidoPorCarga(int codigoCarga)
        {
            var cargaPedido1 = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
            .Where(obj => obj.Carga.Codigo == codigoCarga)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Remetente)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .ThenFetch(o => o.Pais)
                .Fetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.EnderecoOrigem)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.EnderecoDestino)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.RotaFrete)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.ClienteDeslocamento)
                .FirstOrDefault();

            var cargaPedido2 = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
            .Where(obj => obj.Carga.Codigo == codigoCarga)
                .Fetch(obj => obj.FormulaRateio)
                .Fetch(obj => obj.Tomador)
                .Fetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.ModeloDocumentoFiscalIntramunicipal)
                .Fetch(obj => obj.CFOP)
                .Fetch(obj => obj.Origem)
                .Fetch(obj => obj.Destino)
                .Fetch(obj => obj.CargaOrigem)
                .ThenFetch(obj => obj.Empresa)
                .Fetch(obj => obj.CargaOrigem)
                .Fetch(obj => obj.CargaPedidoProximoTrecho)
                .Fetch(obj => obj.CargaPedidoTrechoAnterior)
                .FirstOrDefault();

            if (cargaPedido1 != null)
            {
                cargaPedido1.FormulaRateio = cargaPedido2.FormulaRateio;
                cargaPedido1.Tomador = cargaPedido2.Tomador;
                cargaPedido1.ModeloDocumentoFiscal = cargaPedido2.ModeloDocumentoFiscal;
                cargaPedido1.ModeloDocumentoFiscalIntramunicipal = cargaPedido2.ModeloDocumentoFiscalIntramunicipal;
                cargaPedido1.CFOP = cargaPedido2.CFOP;
                cargaPedido1.Origem = cargaPedido2.Origem;
                cargaPedido1.Destino = cargaPedido2.Destino;
                cargaPedido1.CargaOrigem = cargaPedido2.CargaOrigem;
                cargaPedido1.CargaPedidoProximoTrecho = cargaPedido2.CargaPedidoProximoTrecho;
                cargaPedido1.CargaPedidoTrechoAnterior = cargaPedido2.CargaPedidoTrechoAnterior;
            }

            return cargaPedido1;
        }

        public async Task<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPrimeiroPedidoPorCargaAsync(int codigoCarga)
        {
            var cargaPedido1 = await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
            .Where(obj => obj.Carga.Codigo == codigoCarga)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Remetente)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .ThenFetch(o => o.Pais)
                .Fetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.EnderecoOrigem)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.EnderecoDestino)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.RotaFrete)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.ClienteDeslocamento)
                .FirstOrDefaultAsync();

            var cargaPedido2 = await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
            .Where(obj => obj.Carga.Codigo == codigoCarga)
                .Fetch(obj => obj.FormulaRateio)
                .Fetch(obj => obj.Tomador)
                .Fetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.ModeloDocumentoFiscalIntramunicipal)
                .Fetch(obj => obj.CFOP)
                .Fetch(obj => obj.Origem)
                .Fetch(obj => obj.Destino)
                .Fetch(obj => obj.CargaOrigem)
                .ThenFetch(obj => obj.Empresa)
                .Fetch(obj => obj.CargaOrigem)
                .Fetch(obj => obj.CargaPedidoProximoTrecho)
                .Fetch(obj => obj.CargaPedidoTrechoAnterior)
                .FirstOrDefaultAsync();

            if (cargaPedido1 != null)
            {
                cargaPedido1.FormulaRateio = cargaPedido2.FormulaRateio;
                cargaPedido1.Tomador = cargaPedido2.Tomador;
                cargaPedido1.ModeloDocumentoFiscal = cargaPedido2.ModeloDocumentoFiscal;
                cargaPedido1.ModeloDocumentoFiscalIntramunicipal = cargaPedido2.ModeloDocumentoFiscalIntramunicipal;
                cargaPedido1.CFOP = cargaPedido2.CFOP;
                cargaPedido1.Origem = cargaPedido2.Origem;
                cargaPedido1.Destino = cargaPedido2.Destino;
                cargaPedido1.CargaOrigem = cargaPedido2.CargaOrigem;
                cargaPedido1.CargaPedidoProximoTrecho = cargaPedido2.CargaPedidoProximoTrecho;
                cargaPedido1.CargaPedidoTrechoAnterior = cargaPedido2.CargaPedidoTrechoAnterior;
            }

            return cargaPedido1;
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPrimeiroPedidoPorProtocoloCarga(int protocoloCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Protocolo == protocoloCarga);
            return query
                .Fetch(obj => obj.Pedido).ThenFetch(obj => obj.Remetente)
                .Fetch(obj => obj.Pedido).ThenFetch(obj => obj.Destinatario).ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Expedidor).ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Recebedor).ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Pedido).ThenFetch(obj => obj.EnderecoOrigem)
                .Fetch(obj => obj.Pedido).ThenFetch(obj => obj.EnderecoDestino)
                .Fetch(obj => obj.Tomador)
                .Fetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.CFOP)
                .Fetch(obj => obj.Origem)
                .Fetch(obj => obj.Destino)
                .Fetch(obj => obj.Pedido).ThenFetch(obj => obj.RotaFrete)
                .Fetch(obj => obj.CargaOrigem).ThenFetch(obj => obj.Empresa)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Cliente BuscarPrimeiroDestinatarioDePedidoPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Carga.Codigo == codigoCarga);
            return query.Select(o => o.Pedido.Destinatario).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidoPorCargaEDestinatario(int codigoCarga, double cpfCnpjDestinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            var result = query.Where(obj => obj.Carga.Codigo == codigoCarga && ((double?)obj.Recebedor.CPF_CNPJ ?? obj.Pedido.Destinatario.CPF_CNPJ) == cpfCnpjDestinatario);

            return result.ToList();
        }

        public Dominio.Entidades.RotaFrete BuscarPrimeiraRotaDoPedidoPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.Pedido.RotaFrete != null);

            return query.Select(o => o.Pedido.RotaFrete).FirstOrDefault();
        }

        public async Task<Dominio.Entidades.RotaFrete> BuscarPrimeiraRotaDoPedidoPorCargaAsync(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.Pedido.RotaFrete != null);

            return await query.Select(o => o.Pedido.RotaFrete).FirstOrDefaultAsync();
        }

        public bool ExistePedidoComPontoPartidaPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.PontoPartida != null);

            return query.Select(o => o.Codigo).Any();
        }

        public Dominio.Entidades.Embarcador.Financeiro.CentroResultado BuscarCentroResultadoPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.Pedido.CentroResultado != null);

            return query.Select(o => o.Pedido.CentroResultado).FirstOrDefault();
        }

        public bool PossuiPedidoSubstituicao(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && (obj.Pedido.Substituicao ?? false));

            return query.Any();
        }

        public List<int> BuscarCodigoCargasOriginais(int cargaAgrupadora)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == cargaAgrupadora);

            return query.Select(obj => obj.CargaOrigem.Codigo).Distinct().ToList();
        }

        public decimal BuscarAliquotaICMSPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.Select(o => o.PercentualAliquota).FirstOrDefault();
        }

        public decimal BuscarValorFreteTransportadorTerceiroPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.Sum(o => (decimal?)o.Pedido.ValorFreteTransportadorTerceiro) ?? 0m;
        }

        public decimal BuscarValorFreteToneladaTerceiroPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.Sum(o => (decimal?)o.Pedido.ValorFreteToneladaTerceiro) ?? 0m;
        }

        public bool ProximaEtapaFluxoGestaoPatioLiberadaPorCarga(int codigoCarga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga && (((bool?)o.ProximaEtapaFluxoGestaoPatioLiberada).HasValue == false || o.ProximaEtapaFluxoGestaoPatioLiberada == false));

            return consultaCargaPedido.Count() == 0;
        }

        public void BloquearLiberacaoProximaEtapaFluxoGestaoPatioPorCarga(int codigoCarga)
        {
            UnitOfWork.Sessao
                .CreateQuery($"update CargaPedido set ProximaEtapaFluxoGestaoPatioLiberada = 0 where Carga.Codigo = :codigoCarga ")
                .SetInt32("codigoCarga", codigoCarga)
                .ExecuteUpdate();
        }

        public bool BuscarPedidoSemDataPrevisaoEntrega(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && !obj.Pedido.PrevisaoEntrega.HasValue);

            return query.Count() > 0;
        }
        public async Task<bool> BuscarPedidoSemDataPrevisaoEntregaAsync(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && !obj.Pedido.PrevisaoEntrega.HasValue);

            return await query.CountAsync() > 0;
        }

        public bool PossuiPedidoComVolumesZerado(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.Pedido.QtVolumes == 0);

            return query.Any();
        }
        public async Task<bool> PossuiPedidoComVolumesZeradoAsync(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.Pedido.QtVolumes == 0);

            return await query.AnyAsync();
        }

        public bool PossuiPedidoComClienteSemLocalidade(int codigoCarga, int codigoLocalidadeNaoCadastrada)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga
                                       && (obj.Pedido.Remetente.Localidade.Codigo == codigoLocalidadeNaoCadastrada ||
                                           obj.Pedido.Destinatario.Localidade.Codigo == codigoLocalidadeNaoCadastrada ||
                                           obj.Pedido.Recebedor.Localidade.Codigo == codigoLocalidadeNaoCadastrada ||
                                           obj.Pedido.Expedidor.Localidade.Codigo == codigoLocalidadeNaoCadastrada));

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Filiais.Filial> BuscarFiliaisPorDestinatariosDaCarga(int codigoCarga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            var consultaFilial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>()
                .Where(filial => filial.Ativo == true && consultaCargaPedido.Any(cargaPedido => cargaPedido.Pedido.Destinatario.CPF_CNPJ == double.Parse(filial.CNPJ)));

            return consultaFilial.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Filiais.Filial> BuscarFiliaisPorDestinatariosDaCargaOrigem(int codigoCarga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.CargaOrigem.Codigo == codigoCarga);

            var consultaFilial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>()
                .Where(filial => filial.Ativo == true && consultaCargaPedido.Any(cargaPedido => cargaPedido.Pedido.Destinatario.CPF_CNPJ == double.Parse(filial.CNPJ)));

            return consultaFilial.ToList();
        }

        public Dominio.Entidades.Embarcador.Filiais.Filial BuscarPrimeiraFilialPorExpedidoresDaCarga(int codigoCarga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.Expedidor != null);

            var consultaFilial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>()
                .Where(filial => filial.Ativo == true && consultaCargaPedido.Any(cargaPedido => cargaPedido.Pedido.Expedidor.CPF_CNPJ == double.Parse(filial.CNPJ)));

            return consultaFilial.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Filiais.Filial> BuscarPrimeiraFilialPorExpedidoresDaCargaAsync(int codigoCarga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.Expedidor != null);

            var consultaFilial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>()
                .Where(filial => filial.Ativo == true && consultaCargaPedido.Any(cargaPedido => cargaPedido.Pedido.Expedidor.CPF_CNPJ == double.Parse(filial.CNPJ)));

            return consultaFilial.FirstOrDefaultAsync();
        }

        public decimal BuscarPesoPorDestinatariosDaCarga(int codigoCarga, double cpfCnpjDestinatario)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.Pedido.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);

            return consultaCargaPedido.Sum(o => (decimal?)o.Peso) ?? 0m;
        }

        public decimal BuscarVolumesPorDestinatariosDaCarga(int codigoCarga, double cpfCnpjDestinatario)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.Pedido.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);

            return consultaCargaPedido.Sum(o => (decimal?)o.QtVolumes) ?? 0m;
        }

        public int BuscarCodigoCanalVendaPorDestinatarioDaCarga(int codigoCarga, double cpfCnpjDestinatario)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(cargaPedido =>
                    cargaPedido.Carga.Codigo == codigoCarga &&
                    ((double?)cargaPedido.Recebedor.CPF_CNPJ ?? cargaPedido.Pedido.Destinatario.CPF_CNPJ) == cpfCnpjDestinatario &&
                    cargaPedido.TipoCarregamentoPedido != TipoCarregamentoPedido.TrocaNota &&
                    cargaPedido.Pedido.TipoPedido != TipoPedido.Coleta &&
                    cargaPedido.PedidoPallet == false &&
                    cargaPedido.CanalVenda != null
                );

            return consultaCargaPedido
                .Select(o => o.CanalVenda.Codigo)
                .FirstOrDefault();
        }

        public IList<(int CodigoCarga, int CodigoCanalVenda)> BuscarDadosCanalVendaPorDestinatarioDasCargas(List<int> codigosCargas, double cpfCnpjDestinatario)
        {
            var sql = $@"
                Select Carga.CAR_CODIGO CodigoCarga, isnull(CanalVenda.CNV_CODIGO, 0) CodigoCanalVenda
                  from T_CARGA Carga
                  left join (
                           Select CargaPedido.CAR_CODIGO,
                                  CargaPedido.CNV_CODIGO,
                                  row_number() over (partition by CargaPedido.CAR_CODIGO order by Sum(CargaPedido.CPE_CODIGO) desc) Ordem
                             from T_CARGA_PEDIDO CargaPedido
                             Join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                            where isnull(CargaPedido.CLI_CODIGO_RECEBEDOR, Pedido.CLI_CODIGO) = {cpfCnpjDestinatario}
                              and CargaPedido.CNV_CODIGO is not null
                              and CargaPedido.PED_TIPO_CARREGAMENTO_PEDIDO <> {(int)TipoCarregamentoPedido.TrocaNota}
                              and CargaPedido.PED_PEDIDO_PALLET = 0
                              and Pedido.PED_TIPO_PEDIDO <> {(int)TipoPedido.Coleta}
                            group by CargaPedido.CAR_CODIGO, CargaPedido.CNV_CODIGO
                       ) CanalVenda on CanalVenda.CAR_CODIGO = Carga.CAR_CODIGO and CanalVenda.Ordem = 1
                 where Carga.CAR_CODIGO in ({string.Join(", ", codigosCargas)})";

            var consultaCanalVenda = this.SessionNHiBernate.CreateSQLQuery(sql);

            consultaCanalVenda.SetResultTransformer(new NHibernate.Transform.AliasToBeanConstructorResultTransformer(typeof((int CodigoCarga, int CodigoCanalVenda)).GetConstructors().FirstOrDefault()));

            return consultaCanalVenda.SetTimeout(600).List<(int CodigoCarga, int CodigoCanalVenda)>();
        }

        public void ZerarValorValePedagioPorCarga(int carga)
        {
            string hql = "update CargaPedido cargaPedido set cargaPedido.ValorPedagio = 0 where cargaPedido.Carga= :Carga";

            IQuery query = this.SessionNHiBernate.CreateQuery(hql);

            query.SetInt32("Carga", carga);
            query.ExecuteUpdate();
        }

        public List<(int CodigoCarga, string NumeroEXP)> BuscarNumerosEXPPorCarga(List<int> codigosCarga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => codigosCarga.Contains(o.Carga.Codigo) && (o.Pedido.NumeroEXP ?? "") != "");

            return consultaCargaPedido
                .Select(o => ValueTuple.Create(o.Carga.Codigo, o.Pedido.NumeroEXP))
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorNumeroCargaEPedido(string numeroCarga, int codigoPedido)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(cargaPedido =>
                    cargaPedido.Pedido.Codigo == codigoPedido &&
                    cargaPedido.CargaOrigem.CodigoCargaEmbarcador == numeroCarga &&
                    cargaPedido.CargaOrigem.SituacaoCarga != SituacaoCarga.Cancelada &&
                    cargaPedido.CargaOrigem.SituacaoCarga != SituacaoCarga.Anulada
                );

            return consultaCargaPedido
                .OrderByDescending(o => o.Codigo)
                .FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorNumeroCargaEPedidoAsync(string numeroCarga, int codigoPedido)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(cargaPedido =>
                    cargaPedido.Pedido.Codigo == codigoPedido &&
                    cargaPedido.CargaOrigem.CodigoCargaEmbarcador == numeroCarga &&
                    cargaPedido.CargaOrigem.SituacaoCarga != SituacaoCarga.Cancelada &&
                    cargaPedido.CargaOrigem.SituacaoCarga != SituacaoCarga.Anulada
                );

            return consultaCargaPedido
                .OrderByDescending(o => o.Codigo)
                .FirstOrDefaultAsync();
        }

        public bool PossuiMesmaOrigemEDestino(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.PossuiCTe && o.Carga.Codigo == codigoCarga && o.Origem == o.Destino);

            return query.Select(o => o.Codigo).Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorPedido(List<int> codigosPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => codigosPedido.Contains(o.Pedido.Codigo));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarCargaPedidoEXMLNotaFiscal(List<int> codigosCargasPedidos)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(o => codigosCargasPedidos.Contains(o.CargaPedido.Codigo));

            return consulta
                .Fetch(o => o.CargaPedido).ThenFetch(o => o.Pedido)
                .Fetch(o => o.XMLNotaFiscal)
                .ToList();
        }

        public void AjustarValoresCargaComImpostoIncluso(int codigoCarga)
        {
            UnitOfWork.Sessao.CreateSQLQuery("UPDATE cargaPedido SET cargaPedido.PED_VALOR_FRETE_COM_ICMS_INCLUSO = ROUND(cargaPedido.PED_VALOR_FRETE / (1 - (cargaPedido.PED_PERCENTUAL_ALICOTA / 100)), 2) FROM T_CARGA_PEDIDO cargaPedido WHERE cargaPedido.CAR_CODIGO = :codigoCarga").SetInt32("codigoCarga", codigoCarga).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("UPDATE cargaPedidoComponentesFrete SET cargaPedidoComponentesFrete.CCF_VALOR_COMPONENTE_COM_ICMS_INCLUSO = ROUND(cargaPedidoComponentesFrete.CCF_VALOR_COMPONENTE / (1 - (cargaPedido.PED_PERCENTUAL_ALICOTA / 100)), 2) FROM T_CARGA_PEDIDO_COMPONENTES_FRETE cargaPedidoComponentesFrete INNER JOIN T_CARGA_PEDIDO cargaPedido ON cargaPedidoComponentesFrete.CPE_CODIGO = cargaPedido.CPE_CODIGO WHERE cargaPedido.CAR_CODIGO = :codigoCarga").SetInt32("codigoCarga", codigoCarga).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("UPDATE pedidoXMLNotaFiscal SET pedidoXMLNotaFiscal.PNF_VALOR_FRETE_COM_ICMS_INCLUSO = ROUND(pedidoXMLNotaFiscal.PNF_VALOR_FRETE / (1 - (pedidoXMLNotaFiscal.PNF_PERCENTUAL_ALICOTA / 100)), 2)  FROM T_PEDIDO_XML_NOTA_FISCAL pedidoXMLNotaFiscal INNER JOIN T_CARGA_PEDIDO cargaPedido on cargaPedido.CPE_CODIGO = pedidoXMLNotaFiscal.CPE_CODIGO WHERE cargaPedido.CAR_CODIGO = :codigoCarga").SetInt32("codigoCarga", codigoCarga).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("UPDATE pedidoXMLNotafiscalComponentesFrete SET pedidoXMLNotafiscalComponentesFrete.NFC_VALOR_COMPONENTE_COM_ICMS_INCLUSO = ROUND(pedidoXMLNotafiscalComponentesFrete.NFC_VALOR_COMPONENTE / (1 - (pedidoXMLNotaFiscal.PNF_PERCENTUAL_ALICOTA / 100)), 2) FROM T_PEDIDO_XML_NOTA_FISCAL_COMPONENTES_FRETE pedidoXMLNotafiscalComponentesFrete INNER JOIN T_PEDIDO_XML_NOTA_FISCAL pedidoXMLNotaFiscal ON pedidoXMLNotaFiscal.PNF_CODIGO = pedidoXMLNotafiscalComponentesFrete.PNF_CODIGO INNER JOIN T_CARGA_PEDIDO cargaPedido ON cargaPedido.CPE_CODIGO = pedidoXMLNotaFiscal.CPE_CODIGO WHERE cargaPedido.CAR_CODIGO = :codigoCarga").SetInt32("codigoCarga", codigoCarga).ExecuteUpdate();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidoCompativelComPacoteAsync(double cpfCnpjRemetente, double cpfCnpjDestinatario, double cpfCnpjContratante, CancellationToken cancellationToken)
        {
            List<SituacaoCarga> situacaoCargas = new List<SituacaoCarga>()
            {
                SituacaoCarga.Nova,
                SituacaoCarga.AgNFe
            };

            List<double> listaCnpj = new List<double>()
            {
                cpfCnpjRemetente,
                cpfCnpjContratante
            };

            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o =>
                    situacaoCargas.Contains(o.Carga.SituacaoCarga) && o.Carga.CargaFechada == true &&
                    listaCnpj.Contains(o.Pedido.Remetente.CPF_CNPJ) && o.Pedido.Destinatario.CPF_CNPJ == cpfCnpjDestinatario &&
                    o.Carga.ProcessandoDocumentosFiscais == false && o.Carga.DataFinalizacaoProcessamentoDocumentosFiscais.HasValue == false &&
                    o.Carga.TipoOperacao.ConfiguracaoCarga.PermitirIntegrarPacotes == true
                );

            return consultaCargaPedido.OrderByDescending(o => o.Carga.Codigo).Fetch(x => x.Carga).FirstOrDefaultAsync(cancellationToken);
        }

        public List<int> BuscarProtocoloPedidoPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada select obj.Pedido.Protocolo;
            return result.ToList();
        }

        public Dominio.ObjetosDeValor.Embarcador.Localidade.DadosPaisOrigemDestino BuscarDadosPaisOrigemDestinoPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga == carga);

            return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Localidade.DadosPaisOrigemDestino()
            {
                AbreviacaoOrigem = o.Origem.Pais.Abreviacao,
                LicencaTNTIOrigem = o.Origem.Pais.LicencaTNTI,
                AbreviacaoDestino = o.Destino.Pais.Abreviacao,
                LicencaTNTIDestino = o.Destino.Pais.LicencaTNTI
            }).FirstOrDefault();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoDadosRateioFrete> BuscarDadosRateioPorCargaEmitidaParcialmente(int codigoCarga)
        {
            string sql = $@"
                select row_number() over(order by isnull(DataEmissaoCte, getdate()), DataPedidoAdicionado asc) Ordem, *
                  from (
                           select CargaPedido.PED_VALOR_FRETE_PAGAR ValorFrete,
                                  CargaPedido.PED_PESO Peso,
                                  CargaPedido.PED_PESO_LIQUIDO PesoLiquido,
                                  CargaPedido.PED_DATA_CRIACAO DataPedidoAdicionado,
                                  Stage.STA_NUMERO_STAGE NumeroStage,
                                  Pedido.PED_NUMERO_PEDIDO_EMBARCADOR NumeroPedido,
                                  Pedido.PED_PESO_TOTAL_CARGA PesoPedido,
                                  Pedido.PED_PESO_LIQUIDO_TOTAL_CARGA PesoLiquidoPedido,
                                  isnull(FormulaRateio.RFO_PARAMETRO_RATEIO_FORMULA, {(int)ParametroRateioFormula.peso}) ParametroRateio,
                           	      (
                                      select min(Cte.CON_DATAHORAEMISSAO)
                                        from T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFiscal
                                        left join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE NotaFiscalCte on NotaFiscalCte.PNF_CODIGO = PedidoNotaFiscal.PNF_CODIGO
                                        left join T_CARGA_CTE CargaCte on CargaCte.CCT_CODIGO = NotaFiscalCte.CCT_CODIGO
                                        left join T_CTE Cte on Cte.CON_CODIGO = CargaCte.CON_CODIGO
                                       where PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
                                  ) DataEmissaoCte,
                           	      (
                                      select min(Cte.CON_NUM)
                                        from T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFiscal
                                        left join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE NotaFiscalCte on NotaFiscalCte.PNF_CODIGO = PedidoNotaFiscal.PNF_CODIGO
                                        left join T_CARGA_CTE CargaCte on CargaCte.CCT_CODIGO = NotaFiscalCte.CCT_CODIGO
                                        left join T_CTE Cte on Cte.CON_CODIGO = CargaCte.CON_CODIGO
                                       where PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
                                  ) NumeroCte,
                           	      (
                                      select min(Cte.CON_VALOR_RECEBER)
                                        from T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFiscal
                                        left join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE NotaFiscalCte on NotaFiscalCte.PNF_CODIGO = PedidoNotaFiscal.PNF_CODIGO
                                        left join T_CARGA_CTE CargaCte on CargaCte.CCT_CODIGO = NotaFiscalCte.CCT_CODIGO
                                        left join T_CTE Cte on Cte.CON_CODIGO = CargaCte.CON_CODIGO
                                       where PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
                                  ) ValorCte
                             from T_CARGA_PEDIDO CargaPedido
                             join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                             left join T_STAGE Stage on Stage.STA_CODIGO = CargaPedido.STA_CODIGO_RELEVANTE_CUSTO
                             left join T_RATEIO_FORMULA FormulaRateio on FormulaRateio.RFO_CODIGO = CargaPedido.RFO_CODIGO
                            where CargaPedido.CAR_CODIGO = {codigoCarga}
                       ) DadosRateioPorCargaEmitidaParcialmente";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoDadosRateioFrete)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoDadosRateioFrete>();
        }
        public async Task<List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoDadosRateioFrete>> BuscarDadosRateioPorCargaEmitidaParcialmenteAsync(int codigoCarga, CancellationToken cancellationToken)
        {
            string sql = $@"
                select row_number() over(order by isnull(DataEmissaoCte, getdate()), DataPedidoAdicionado asc) Ordem, *
                  from (
                           select CargaPedido.PED_VALOR_FRETE_PAGAR ValorFrete,
                                  CargaPedido.PED_PESO Peso,
                                  CargaPedido.PED_PESO_LIQUIDO PesoLiquido,
                                  CargaPedido.PED_DATA_CRIACAO DataPedidoAdicionado,
                                  Stage.STA_NUMERO_STAGE NumeroStage,
                                  Pedido.PED_NUMERO_PEDIDO_EMBARCADOR NumeroPedido,
                                  Pedido.PED_PESO_TOTAL_CARGA PesoPedido,
                                  Pedido.PED_PESO_LIQUIDO_TOTAL_CARGA PesoLiquidoPedido,
                                  isnull(FormulaRateio.RFO_PARAMETRO_RATEIO_FORMULA, {(int)ParametroRateioFormula.peso}) ParametroRateio,
                           	      (
                                      select min(Cte.CON_DATAHORAEMISSAO)
                                        from T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFiscal
                                        left join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE NotaFiscalCte on NotaFiscalCte.PNF_CODIGO = PedidoNotaFiscal.PNF_CODIGO
                                        left join T_CARGA_CTE CargaCte on CargaCte.CCT_CODIGO = NotaFiscalCte.CCT_CODIGO
                                        left join T_CTE Cte on Cte.CON_CODIGO = CargaCte.CON_CODIGO
                                       where PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
                                  ) DataEmissaoCte,
                           	      (
                                      select min(Cte.CON_NUM)
                                        from T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFiscal
                                        left join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE NotaFiscalCte on NotaFiscalCte.PNF_CODIGO = PedidoNotaFiscal.PNF_CODIGO
                                        left join T_CARGA_CTE CargaCte on CargaCte.CCT_CODIGO = NotaFiscalCte.CCT_CODIGO
                                        left join T_CTE Cte on Cte.CON_CODIGO = CargaCte.CON_CODIGO
                                       where PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
                                  ) NumeroCte,
                           	      (
                                      select min(Cte.CON_VALOR_RECEBER)
                                        from T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFiscal
                                        left join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE NotaFiscalCte on NotaFiscalCte.PNF_CODIGO = PedidoNotaFiscal.PNF_CODIGO
                                        left join T_CARGA_CTE CargaCte on CargaCte.CCT_CODIGO = NotaFiscalCte.CCT_CODIGO
                                        left join T_CTE Cte on Cte.CON_CODIGO = CargaCte.CON_CODIGO
                                       where PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
                                  ) ValorCte
                             from T_CARGA_PEDIDO CargaPedido
                             join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                             left join T_STAGE Stage on Stage.STA_CODIGO = CargaPedido.STA_CODIGO_RELEVANTE_CUSTO
                             left join T_RATEIO_FORMULA FormulaRateio on FormulaRateio.RFO_CODIGO = CargaPedido.RFO_CODIGO
                            where CargaPedido.CAR_CODIGO = {codigoCarga}
                       ) DadosRateioPorCargaEmitidaParcialmente";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoDadosRateioFrete)));

            return (List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoDadosRateioFrete>)await consulta.SetTimeout(600).ListAsync<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoDadosRateioFrete>(cancellationToken);
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoComposicaoRateioFrete> BuscarDadosComposicaoRateioFrete(int codigoCarga)
        {
            string sql = $@"select CargaPedido.PED_PESO Peso,
                                   CargaPedido.CPE_CODIGO CodigoPedido,
                                   CargaPedido.PED_VALOR_FRETE_PAGAR ValorPedido,
                                   CargaPedido.PED_VALOR_FRETE ValorCalculado,
                                   Pedido.PED_NUMERO_PEDIDO_EMBARCADOR NumeroPedido,
                                   Pedido.PED_PESO_TOTAL_CARGA PesoPedido,
                                   LocalidadeOrigem.LOC_DESCRICAO Origem,
                                   LocalidadeDestino.LOC_DESCRICAO Destino,
                                   TabelaFrete.TBF_CODIGO_INTEGRACAO CodigoTabela,
                                   FormulaRateio.RFO_PERCENTUAL_ACRESCENTAR_PESO_TOTAL_CARGA TaxaElemento,
                                   FormulaRateio.RFO_DESCRICAO DescricaoRateio
                              from T_CARGA_PEDIDO CargaPedido
                              join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                         left join T_TABELA_FRETE TabelaFrete on TabelaFrete.TBF_CODIGO = CargaPedido.TBF_CODIGO
                         left join T_LOCALIDADES LocalidadeOrigem on LocalidadeOrigem.LOC_CODIGO = CargaPedido.LOC_CODIGO_ORIGEM
                         left join T_LOCALIDADES LocalidadeDestino on LocalidadeDestino.LOC_CODIGO = CargaPedido.LOC_CODIGO_DESTINO
                         left join T_RATEIO_FORMULA FormulaRateio on FormulaRateio.RFO_CODIGO = CargaPedido.RFO_CODIGO
                             where CargaPedido.CAR_CODIGO = {codigoCarga}";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoComposicaoRateioFrete)));

            return consulta.List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoComposicaoRateioFrete>();
        }
        public async Task<List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoComposicaoRateioFrete>> BuscarDadosComposicaoRateioFreteAsync(int codigoCarga)
        {
            string sql = $@"select CargaPedido.PED_PESO Peso,
                                   CargaPedido.CPE_CODIGO CodigoPedido,
                                   CargaPedido.PED_VALOR_FRETE_PAGAR ValorPedido,
                                   CargaPedido.PED_VALOR_FRETE ValorCalculado,
                                   Pedido.PED_NUMERO_PEDIDO_EMBARCADOR NumeroPedido,
                                   Pedido.PED_PESO_TOTAL_CARGA PesoPedido,
                                   LocalidadeOrigem.LOC_DESCRICAO Origem,
                                   LocalidadeDestino.LOC_DESCRICAO Destino,
                                   TabelaFrete.TBF_CODIGO_INTEGRACAO CodigoTabela,
                                   FormulaRateio.RFO_PERCENTUAL_ACRESCENTAR_PESO_TOTAL_CARGA TaxaElemento,
                                   FormulaRateio.RFO_DESCRICAO DescricaoRateio
                              from T_CARGA_PEDIDO CargaPedido
                              join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                         left join T_TABELA_FRETE TabelaFrete on TabelaFrete.TBF_CODIGO = CargaPedido.TBF_CODIGO
                         left join T_LOCALIDADES LocalidadeOrigem on LocalidadeOrigem.LOC_CODIGO = CargaPedido.LOC_CODIGO_ORIGEM
                         left join T_LOCALIDADES LocalidadeDestino on LocalidadeDestino.LOC_CODIGO = CargaPedido.LOC_CODIGO_DESTINO
                         left join T_RATEIO_FORMULA FormulaRateio on FormulaRateio.RFO_CODIGO = CargaPedido.RFO_CODIGO
                             where CargaPedido.CAR_CODIGO = {codigoCarga}";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoComposicaoRateioFrete)));

            return (List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoComposicaoRateioFrete>)await consulta.ListAsync<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoComposicaoRateioFrete>();
        }

        public Dominio.Entidades.Cliente BuscarPrimeiroClientePorCarga(int codigoCarga, bool destinatariosDaCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga && (destinatariosDaCarga ? o.Pedido.Destinatario != null : o.Pedido.Remetente != null));

            return query.Select(o => destinatariosDaCarga ? o.Pedido.Destinatario : o.Pedido.Remetente).FirstOrDefault();
        }

        public Task<Dominio.Entidades.Cliente> BuscarPrimeiroClientePorCargaAsync(int codigoCarga, bool destinatariosDaCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga && (destinatariosDaCarga ? o.Pedido.Destinatario != null : o.Pedido.Remetente != null));

            return query.Select(o => destinatariosDaCarga ? o.Pedido.Destinatario : o.Pedido.Remetente).FirstOrDefaultAsync(CancellationToken);
        }

        public string BuscarEmailPrimeiroClientePorCarga(int codigoCarga, bool destinatariosDaCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga && (destinatariosDaCarga ? o.Pedido.Destinatario != null : o.Pedido.Remetente != null));

            return query.Select(o => destinatariosDaCarga ? o.Pedido.Destinatario.Email : o.Pedido.Remetente.Email).FirstOrDefault();
        }

        public bool ExistePorNumeroOS(string numeroOS)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Pedido.NumeroOS == numeroOS &&
                        o.Pedido.Container == null &&
                        o.Carga.SituacaoCarga == SituacaoCarga.AgNFe);

            return query.Any();
        }

        public IList<(int carga, string notaFiscal)> BuscarNFsPorCargas(List<int> codigosCargas)
        {
            var sql = $@"select C.CAR_CODIGO carga, CAST(NF_NUMERO AS VARCHAR(25)) notaFiscal from T_CARGA C (NOLOCK) 
                INNER JOIN T_CARGA_PEDIDO (NOLOCK) CP ON CP.CAR_CODIGO = C.CAR_CODIGO
                INNER JOIN T_PEDIDO_XML_NOTA_FISCAL (NOLOCK) NFP ON NFP.CPE_CODIGO = CP.CPE_CODIGO
                INNER JOIN T_XML_NOTA_FISCAL (NOLOCK) NF ON NF.NFX_CODIGO = NFP.NFX_CODIGO
                WHERE C.CAR_CODIGO IN ({string.Join(", ", codigosCargas)})";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanConstructorResultTransformer(typeof((int carga, string notaFiscal)).GetConstructors().FirstOrDefault()));
            return consulta.SetTimeout(7000).List<(int carga, string notaFiscal)>();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorNumeroOSComFetch(string numeroOS)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Pedido.NumeroOS == numeroOS &&
                        o.Pedido.Container == null &&
                        o.Carga.SituacaoCarga == SituacaoCarga.AgNFe);

            return query.Fetch(o => o.Pedido)
                        .Fetch(o => o.Carga)
                        .FirstOrDefault();
        }

        public bool ExisteCargaMaePorNumeroOS(int codigo, string numeroOS)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Codigo != codigo &&
                        o.Pedido.NumeroOS == numeroOS &&
                        o.Pedido.Container != null);

            return query.Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarCargaMaePorNumeroOSComFetch(int codigo, string numeroOS)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Codigo != codigo &&
                        o.Pedido.NumeroOS == numeroOS &&
                        o.Pedido.Container != null);

            return query.Fetch(o => o.Pedido)
                        .ThenFetch(o => o.Container)
                        .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPrimeiraPorCargaOrdenadoPelaDataMenorDataCarregamento(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            return result
                .Fetch(obj => obj.Pedido)
                .OrderBy(a => a.Pedido.DataCarregamentoPedido)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorProtocoloCargaSemFetchDesnecessarios(int protocoloCarga, bool retornarCargasAgrupadasCarregamento = false)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            if (retornarCargasAgrupadasCarregamento)
            {
                var queryCargaOrigem = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                query = query.Where(o => (from obj in queryCargaOrigem where obj.CargaOrigem.Protocolo == protocoloCarga || (obj.Carga.CargaAgrupada && obj.Carga.Codigo == protocoloCarga) select obj.Codigo).Contains(o.Codigo));
            }
            else
                query = query.Where(o => (o.CargaOrigem.Protocolo == protocoloCarga));

            return query.Fetch(obj => obj.Carga)
                        .Fetch(obj => obj.CargaOrigem)
                        .Timeout(60)
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorIdentificadorRota(string identificadorRota)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(obj => obj.CargaOrigem.IdentificadorDeRota == identificadorRota);

            return query.Fetch(obj => obj.Carga)
                        .Fetch(obj => obj.CargaOrigem)
                        .Fetch(obj => obj.Pedido)
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPedidosDevolucaoPacotesSemPacote(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>().Where(cp => cp.Carga.Codigo == codigoCarga);
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> queryCargaPedidoPacote = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>();

            queryCargaPedido = queryCargaPedido.Where(cp => cp.Pedido.DevolucaoPacote && queryCargaPedidoPacote.Where(p => p.CargaPedido.Pedido.Codigo == cp.Pedido.Codigo).Count() == 0);

            return queryCargaPedido.Select(obj => obj.Pedido).ToList();
        }

        public bool ExisteNotaRemessaPallet(int codigoCargaPedido)
        {
            var notasFiscais = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Codigo == codigoCargaPedido)
                .SelectMany(o => o.NotasFiscais)
                .Select(x => x.XMLNotaFiscal);

            return notasFiscais.Any(o => o.TipoNotaFiscalIntegrada == TipoNotaFiscalIntegrada.RemessaPallet);
        }

        public List<int> BuscarCodigosCanalEntregaPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga == carga && o.CanalEntrega != null);

            return query.Select(o => o.CanalEntrega.Codigo).Distinct().ToList();
        }

        public string BuscarDescricaoTipoDeCargaPorPrioridade(List<int> codigoCargaPedido)
        {
            if (codigoCargaPedido == null)
                return null;

            var result = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(obj => codigoCargaPedido.Contains(obj.Codigo) && obj.Pedido.TipoDeCarga != null && obj.Pedido.TipoDeCarga.PrioridadeCarga > 0)
                .OrderBy(x => x.Pedido.TipoDeCarga.PrioridadeCarga)
                .Select(x => x.Pedido.TipoDeCarga.Descricao)
                .FirstOrDefault();

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorTrechosAnteriores(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> queryCargaPedido = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(cargaPedido => cargaPedido.CargaPedidoTrechoAnterior.Codigo == codigoCargaPedido);

            return queryCargaPedido.ToList();
        }

        public int ContarPorTrechosAnteriores(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> queryCargaPedido = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(cargaPedido => cargaPedido.CargaPedidoTrechoAnterior.Codigo == codigoCargaPedido);

            return queryCargaPedido.Count();
        }

        public bool NaoComprarValePedagioConfiguradoRemetentePedidoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from cargaPedido in query
                         where
                               cargaPedido.Carga.Codigo == codigoCarga
                            && cargaPedido.Pedido.Remetente.NaoComprarValePedagio
                         select cargaPedido;

            return result.Any();
        }

        public bool ExisteLiberarPedidoRecebedorNaCargaAnterior(List<int> codigosPedidos, int codigoCargaIgnorar)
        {
            List<SituacaoCarga> situacoesCargaCancelada = SituacaoCargaHelper.ObterSituacoesCargaCancelada();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            var result = query.Where(
                obj => obj.Carga.CargaFechada &&
                !situacoesCargaCancelada.Contains(obj.Carga.SituacaoCarga) &&
                obj.Carga.Codigo != codigoCargaIgnorar &&
                codigosPedidos.Contains(obj.Pedido.Codigo)
            );

            result = result.OrderBy(obj => obj.Codigo).ThenBy(obj => obj.Carga.TipoOperacao.ConfiguracaoCarga.LiberarPedidoComRecebedorParaMontagemCarga);

            return result.Select(obj => obj.Carga.TipoOperacao.ConfiguracaoCarga.LiberarPedidoComRecebedorParaMontagemCarga).FirstOrDefault();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoTrechoAnterior> BuscarCargaPedidoTrechoAnteriorPorPedido(List<int> codigosPedidos, double cnpjEmpresa, int codigoCargaIgnorar)
        {
            string query = @$"  select
                                pedido.PED_CODIGO as CodigoPedido,
                                (select top 1 CPE_CODIGO
                                    from T_CARGA_PEDIDO cargaPedido
                                    join T_CARGA carga on carga.CAR_CODIGO = cargaPedido.CAR_CODIGO
                                    where cargaPedido.PED_CODIGO = pedido.PED_CODIGO and carga.CAR_CARGA_FECHADA = 1
                                    and carga.CAR_SITUACAO not in (13, 18) and carga.CAR_CODIGO <> {codigoCargaIgnorar}
                                    and cargaPedido.CLI_CODIGO_EXPEDIDOR = {cnpjEmpresa}
                                    order by cargaPedido.CPE_CODIGO desc
                                ) as CodigoCargaPedido
                                from T_PEDIDO pedido
                                where pedido.PED_CODIGO in ({string.Join(",", codigosPedidos)})
                                and exists (select NFX_CODIGO from T_PEDIDO_NOTAS_FISCAIS notas where notas.PED_CODIGO = pedido.PED_CODIGO)"; // SQL-INJECTION-SAFE

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoTrechoAnterior)));

            return nhQuery.List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoTrechoAnterior>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoTrechoAnterior> BuscarCargaPedidoTrechoAnteriorPorPedidoRecebedor(List<int> codigosPedidos, double cnpjExpedidor, int codigoCargaIgnorar)
        {
            string query = @$"  select
                                pedido.PED_CODIGO as CodigoPedido,
                                (select top 1 CPE_CODIGO
                                    from T_CARGA_PEDIDO cargaPedido
                                    join T_CARGA carga on carga.CAR_CODIGO = cargaPedido.CAR_CODIGO
                                    where cargaPedido.PED_CODIGO = pedido.PED_CODIGO and carga.CAR_CARGA_FECHADA = 1
                                    and carga.CAR_SITUACAO not in (13, 18) and carga.CAR_CODIGO <> {codigoCargaIgnorar}
                                    and cargaPedido.CLI_CODIGO_RECEBEDOR = {cnpjExpedidor}
                                    order by cargaPedido.CPE_CODIGO desc
                                ) as CodigoCargaPedido
                                from T_PEDIDO pedido
                                where pedido.PED_CODIGO in ({string.Join(",", codigosPedidos)})
                                and exists (select NFX_CODIGO from T_PEDIDO_NOTAS_FISCAIS notas where notas.PED_CODIGO = pedido.PED_CODIGO)"; // SQL-INJECTION-SAFE

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoTrechoAnterior)));

            return nhQuery.List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoTrechoAnterior>();
        }

        public bool FoiDisponibilizadoPedidoParaNovaMontagemCarga(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(cargaPedido => cargaPedido.Codigo == codigoCargaPedido && cargaPedido.Expedidor != null && cargaPedido.Carga.TipoOperacao.ConfiguracaoCarga.DisponibilizarNotaFiscalNoPedidoAoFinalizarCarga);

            return query.Any();
        }

        public TipoCobrancaMultimodal RetonrarCobrancaMultiModalCarga(int codigoCarga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
               .Where(o => o.Carga.Codigo == codigoCarga);

            return consultaCargaPedido.Select(o => o.TipoCobrancaMultimodal).FirstOrDefault();
        }

        public List<int> BuscarListaPedidoCargasPorProtocolo(int codigoProtocolo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            var result = query
                  .Where(obj => obj.Carga.Codigo == codigoProtocolo)
                  .SelectMany(obj => query
                      .Where(innerObj => innerObj.Pedido.Codigo == obj.Pedido.Codigo)
                      .Select(innerObj => innerObj.CargaOrigem.Codigo))
                  .ToList();

            return result;
        }

        public List<Tuple<int, DateTime>> BuscarDataInicialColetaPedidoPorCarga(List<int> codigosCargas)
        {
            List<Tuple<int, DateTime>> listaTuplas = new();
            int skip = 0;
            int take = 500;
            int total = codigosCargas.Count;
            while (skip < total)
            {
                List<int> tempCodigosCargas = codigosCargas.Skip(skip).Take(take).ToList();
                string sql = @$"select
                            CAR_CODIGO Codigo,
                            (select top 1 MIN(PED_DATA_INICIAL_COLETA) from t_pedido where ped_codigo in (select ped_codigo from t_carga_pedido cp where cp.car_codigo = carga.Car_codigo)) DataInicialColeta
                            from T_CARGA carga
                            where carga.CAR_CODIGO IN ({string.Join(",", tempCodigosCargas)})"; // SQL-INJECTION-SAFE

                var nhQuery = this.SessionNHiBernate.CreateSQLQuery(sql);

                nhQuery.AddScalar("Codigo", NHibernateUtil.Int32);
                nhQuery.AddScalar("DataInicialColeta", NHibernateUtil.DateTime);

                var resultados = nhQuery.List<object[]>();
                var tuplas = resultados
                    .Select(resultado => new Tuple<int, DateTime>((int)resultado[0], (DateTime)resultado[1]))
                    .ToList();

                listaTuplas.AddRange(tuplas);

                skip += take;
            }

            return listaTuplas;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCotacaoCargaFinalizada(long codigoCotacaoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Pedido.CotacaoPedido.Codigo == codigoCotacaoPedido && obj.Carga.SituacaoCarga == SituacaoCarga.Encerrada select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCotacaoCarga(long codigoCotacaoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Pedido.CotacaoPedido.Codigo == codigoCotacaoPedido select obj;
            return result.ToList();
        }

        #endregion

        #region Métodos Públicos - Confirmar Documentos Em lote

        public int ContarConsultaCargasPedidos(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaEmissaoCTePortoLote filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(ConsultarCargasPedidosPagamentoProvedor(filtrosPesquisa, parametroConsulta, true));

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.EmissaoCTePortoLoteCargaPedido> BuscarCargasPedidosPagamentoProvedor(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaEmissaoCTePortoLote filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(ConsultarCargasPedidosPagamentoProvedor(filtrosPesquisa, parametroConsulta, false));

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.EmissaoCTePortoLoteCargaPedido)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Carga.EmissaoCTePortoLoteCargaPedido>();
        }

        #endregion

        #region Métodos Privados - Confirmar Documentos Em lote

        private string ConsultarCargasPedidosPagamentoProvedor(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaEmissaoCTePortoLote filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta, bool somenteContarNumeroRegistros)
        {
            string sql;

            if (somenteContarNumeroRegistros)
            {
                sql = "select count (distinct Concat(Carga.CAR_CODIGO, Pedido.PED_NUMERO_BOOKING)) ";
            }
            else
            {
                sql = $@"SELECT distinct
                    Carga.CAR_CODIGO Codigo,
                    Carga.CAR_CODIGO_CARGA_EMBARCADOR Carga,
					Pedido.PED_NUMERO_BOOKING NumeroBooking,
					PedidoViagemNavio.PVN_DESCRICAO VVD,
					PortoOrigem.POT_DESCRICAO PortoOrigem,
					PortoDestino.POT_DESCRICAO PortoDestino,
					DadosSumarizados.CDS_REMETENTES Remetente,
					DadosSumarizados.CDS_DESTINATARIOS Destinatario,
					Carga.CAR_SITUACAO SituacaoCarga,
                    Carga.CAR_MENSAGEM_RETORNO_ETAPA_DOCUMENTO MensagemRetorno ";
            }

            sql += @" FROM T_CARGA_PEDIDO CargaPedido
					JOIN T_CARGA Carga on Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO
					JOIN T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
					LEFT JOIN T_PEDIDO_VIAGEM_NAVIO PedidoViagemNavio on PedidoViagemNavio.PVN_CODIGO = Pedido.PVN_CODIGO
					LEFT JOIN T_PORTO PortoOrigem on PortoOrigem.POT_CODIGO = Pedido.POT_CODIGO_ORIGEM
					LEFT JOIN T_PORTO PortoDestino on PortoDestino.POT_CODIGO = Pedido.POT_CODIGO_DESTINO
					LEFT JOIN T_CARGA_DADOS_SUMARIZADOS DadosSumarizados on DadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO";

            sql += ObterWhereConsultaDirecionamentoPorOperador(filtrosPesquisa);

            if (parametroConsulta != null && !somenteContarNumeroRegistros)
            {
                sql += $" order by {parametroConsulta.PropriedadeOrdenar} {parametroConsulta.DirecaoOrdenar}";

                if ((parametroConsulta.InicioRegistros > 0) || (parametroConsulta.LimiteRegistros > 0))
                    sql += $" offset {parametroConsulta.InicioRegistros} rows fetch next {parametroConsulta.LimiteRegistros} rows only;";
            }

            return sql;
        }

        private string ObterWhereConsultaDirecionamentoPorOperador(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaEmissaoCTePortoLote filtrosPesquisa)
        {
            StringBuilder where = new StringBuilder(@" WHERE CargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL IN (1, 4) 
                                                      AND CargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL IN (1, 2)
                                                      AND Carga.CAR_SITUACAO = 5
                                                      AND Carga.CAR_CODIGO in (
                                                       SELECT 
															Carga.CAR_CODIGO
														FROM 
															T_CARGA Carga
														WHERE 
															NOT EXISTS (
																SELECT 1 
																FROM T_CARGA_PEDIDO CargaPedido    
																LEFT JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal        
																	ON CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO    
																WHERE
																	CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO      
																	AND PedidoXMLNotaFiscal.NFX_CODIGO IS NULL
                                                            ))
                                                        and Carga.CAR_PROCESSANDO_DOCUMENTOS_FISCAIS = 0 ");

            if (filtrosPesquisa.CodigoNavioViagemDirecao > 0)
                where.Append($" and PedidoViagemNavio.PVN_CODIGO = {filtrosPesquisa.CodigoNavioViagemDirecao}");

            if (filtrosPesquisa.CodigoPortoDestino > 0)
                where.Append($" and PortoDestino.POT_CODIGO = {filtrosPesquisa.CodigoPortoDestino}");

            if (filtrosPesquisa.CodigoPortoOrigem > 0)
                where.Append($" and PortoOrigem.POT_CODIGO = {filtrosPesquisa.CodigoPortoOrigem}");

            return where.ToString();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> ConsultaCargasPedidoAgIntegracaoEmbarcador(bool validarIntegradoraNFe, DateTime dataFinalizacaoEmissao, int integradora, int codigoGrupoPessoas, int codigoEmpresa, List<double> CPFCNPJClientes, string codigoIntegracaoTipoOperacao, string codigoIntegracaoFilial, bool filtrarPorSituacao = true)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            query = query.Where(obj => obj.PedidoXMLNotaFiscal.CargaPedido.CargaOrigem.Protocolo > 0 &&
                                       obj.CargaCTe.CTe != null && obj.CargaCTe.CTe.Status == "A" &&
                                       !obj.PedidoXMLNotaFiscal.CargaPedido.CargaPedidoIntegrada);

            if (filtrarPorSituacao)
            {
                query = query.Where(obj => (obj.PedidoXMLNotaFiscal.CargaPedido.CargaOrigem.SituacaoCarga == SituacaoCarga.AgImpressaoDocumentos ||
                                           obj.PedidoXMLNotaFiscal.CargaPedido.CargaOrigem.SituacaoCarga == SituacaoCarga.EmTransporte ||
                                           obj.PedidoXMLNotaFiscal.CargaPedido.CargaOrigem.SituacaoCarga == SituacaoCarga.Encerrada ||
                                           obj.PedidoXMLNotaFiscal.CargaPedido.CargaOrigem.SituacaoCarga == SituacaoCarga.LiberadoPagamento ||
                                           (obj.PedidoXMLNotaFiscal.CargaPedido.CargaOrigem.SituacaoCarga == SituacaoCarga.PendeciaDocumentos && obj.PedidoXMLNotaFiscal.CargaPedido.Carga.AgImportacaoCTe)));
            }

            if (dataFinalizacaoEmissao > DateTime.MinValue)
                query = query.Where(obj => obj.CargaCTe.CTe.DataEmissao <= dataFinalizacaoEmissao);

            if (validarIntegradoraNFe)
                query = query.Where(obj => obj.PedidoXMLNotaFiscal.CargaPedido.CargaOrigem.IntegradoraNFe.Codigo == integradora);

            if (codigoGrupoPessoas > 0)
                query = query.Where(o => o.PedidoXMLNotaFiscal.CargaPedido.CargaOrigem.GrupoPessoaPrincipal.Codigo == codigoGrupoPessoas);

            if (codigoEmpresa > 0)
                query = query.Where(o => o.PedidoXMLNotaFiscal.CargaPedido.CargaOrigem.Empresa.Codigo == codigoEmpresa);

            if (!string.IsNullOrWhiteSpace(codigoIntegracaoTipoOperacao))
                query = query.Where(o => o.PedidoXMLNotaFiscal.CargaPedido.CargaOrigem.TipoOperacao.CodigoIntegracao == codigoIntegracaoTipoOperacao);

            if (!string.IsNullOrWhiteSpace(codigoIntegracaoFilial))
                query = query.Where(o => o.PedidoXMLNotaFiscal.CargaPedido.CargaOrigem.Filial.CodigoFilialEmbarcador == codigoIntegracaoFilial || o.PedidoXMLNotaFiscal.CargaPedido.CargaOrigem.Filial.OutrosCodigosIntegracao.Contains(codigoIntegracaoFilial));

            if (CPFCNPJClientes.Count > 0)
                query = query.Where(o => CPFCNPJClientes.Contains(o.PedidoXMLNotaFiscal.CargaPedido.Recebedor.CPF_CNPJ));

            return query;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> ConsultaCargasAgIntegracaoEmbarcador(bool validarIntegradoraNFe, DateTime dataFinalizacaoEmissao, int integradora, int codigoGrupoPessoas, int codigoEmpresa, string codigoIntegracaoTipoOperacao, string codigoIntegracaoFilial, bool filtrarPorSituacao = true)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.CargaOrigem.Protocolo > 0);

            if (filtrarPorSituacao)
            {
                query = query.Where(obj => (obj.CargaOrigem.SituacaoCarga == SituacaoCarga.AgImpressaoDocumentos ||
                                       obj.CargaOrigem.SituacaoCarga == SituacaoCarga.EmTransporte ||
                                       obj.CargaOrigem.SituacaoCarga == SituacaoCarga.Encerrada ||
                                       obj.CargaOrigem.SituacaoCarga == SituacaoCarga.LiberadoPagamento));
            }

            query = query.Where(obj => !obj.CargaOrigem.CargaIntegradaEmbarcador);

            if (dataFinalizacaoEmissao > DateTime.MinValue)
                query = query.Where(obj => obj.CargaOrigem.DataFinalizacaoEmissao <= dataFinalizacaoEmissao);

            if (validarIntegradoraNFe)
                query = query.Where(obj => obj.CargaOrigem.IntegradoraNFe.Codigo == integradora);

            if (codigoGrupoPessoas > 0)
                query = query.Where(o => o.CargaOrigem.GrupoPessoaPrincipal.Codigo == codigoGrupoPessoas);

            if (codigoEmpresa > 0)
                query = query.Where(o => o.CargaOrigem.Empresa.Codigo == codigoEmpresa);

            if (!string.IsNullOrWhiteSpace(codigoIntegracaoTipoOperacao))
                query = query.Where(o => o.CargaOrigem.TipoOperacao.CodigoIntegracao == codigoIntegracaoTipoOperacao);

            if (!string.IsNullOrWhiteSpace(codigoIntegracaoFilial))
                query = query.Where(o => o.CargaOrigem.Filial.CodigoFilialEmbarcador == codigoIntegracaoFilial || o.CargaOrigem.Filial.OutrosCodigosIntegracao.Contains(codigoIntegracaoFilial));

            return query;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> AplicarFetchOrigem(IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query)
        {
            return query
                .Fetch(x => x.Pedido)
                    .ThenFetch(x => x.Remetente)
                    .ThenFetch(x => x.Localidade)
                    .ThenFetch(x => x.Estado)
                    .ThenFetch(x => x.Pais)
                .Fetch(x => x.Expedidor)
                    .ThenFetch(x => x.Localidade)
                    .ThenFetch(x => x.Estado)
                    .ThenFetch(x => x.Pais)
                .Fetch(x => x.Pedido)
                    .ThenFetch(x => x.EnderecoOrigem)
                    .ThenFetch(x => x.ClienteOutroEndereco)
                    .ThenFetch(x => x.Localidade)
                    .ThenFetch(x => x.Estado)
                    .ThenFetch(x => x.Pais)
                .Fetch(x => x.Origem);
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> AplicarFetchDestino(IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query)
        {
            return query
                .Fetch(x => x.Pedido)
                    .ThenFetch(x => x.EnderecoDestino)
                    .ThenFetch(x => x.ClienteOutroEndereco)
                    .ThenFetch(x => x.Localidade)
                    .ThenFetch(x => x.Estado)
                    .ThenFetch(x => x.Pais)
                .Fetch(x => x.Pedido)
                    .ThenFetch(x => x.Destinatario)
                    .ThenFetch(x => x.Localidade)
                    .ThenFetch(x => x.Regiao)
                .Fetch(x => x.Pedido)
                    .ThenFetch(x => x.Destinatario)
                    .ThenFetch(x => x.Localidade)
                    .ThenFetch(x => x.Estado)
                    .ThenFetch(x => x.Pais)
                .Fetch(x => x.Recebedor)
                    .ThenFetch(x => x.Localidade)
                    .ThenFetch(x => x.Estado)
                    .ThenFetch(x => x.Pais)
                .Fetch(x => x.Destino);
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> AplicarFetchOutros(IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query)
        {
            return query
                .Fetch(x => x.Pedido)
                    .ThenFetch(x => x.TipoOperacao)
                .Fetch(x => x.ModeloDocumentoFiscal)
                .Fetch(x => x.CFOP)
                .Fetch(x => x.StageRelevanteCusto)
                .Fetch(x => x.Pedido)
                    .ThenFetch(x => x.RotaFrete)
                .Fetch(x => x.Tomador);
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> MontarPesquisa(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoes, int carga, bool somentePendentes, int numeroNF, int numeroCTe, string codigoPedidoEmbarcador, string codigoCargaEmbarcador, int origem, int destino, int filial, double remetente, double destinatario, double expedidorRecebedor, int redespacho, bool apenasEmpresaPermiteEncaixe, bool buscarPorCargaOrigem, string estadoDestino, bool liberadosParaRedespacho, List<int> codigosFiliais, List<int> cargasUtilizadas = null)
        {

            //Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga RepConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(this.UnitOfWork);
            //Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = RepConfiguracaoGeralCarga.BuscarPrimeiroRegistro();



            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            var result = from obj in query select obj;

            if (situacoes != null && situacoes.Count > 0)
                query = query.Where(o => situacoes.Contains(o.Carga.SituacaoCarga));

            if (filial > 0)
                result = result.Where(obj => obj.Pedido.Filial.Codigo == filial);

            if (remetente > 0)
                result = result.Where(obj => obj.Pedido.Remetente.CPF_CNPJ == remetente);

            if (destinatario > 0)
                result = result.Where(obj => obj.Pedido.Destinatario.CPF_CNPJ == destinatario);

            if (redespacho > 0)
                result = result.Where(obj => obj.CargaRedespacho.Codigo == redespacho);
            else if (liberadosParaRedespacho)
            {
                result = result.Where(obj => obj.CargaRedespacho == null || obj.CargaRedespacho.CargaGerada.SituacaoCarga == SituacaoCarga.Cancelada || obj.CargaRedespacho.CargaGerada.SituacaoCarga == SituacaoCarga.Anulada || obj.CargaRedespacho.Expedidor.CPF_CNPJ != expedidorRecebedor);
            }

            if (carga > 0)
            {
                if (buscarPorCargaOrigem)
                    result = result.Where(obj => obj.CargaOrigem.Codigo == carga);
                else
                    result = result.Where(obj => obj.Carga.Codigo == carga);
            }

            if (codigosFiliais.Count > 0)
            {
                //result = result.Where(obj => obj.Recebedor == null || obj.Recebedor.ClienteDescargas.Any(x => x.FilialResponsavelRedespacho == null));
                //result = result.Where(obj => obj.Recebedor.ClienteDescargas.Any(x => x.FilialResponsavelRedespacho.Codigo == obj.CargaOrigem.Filial.Codigo || x.FilialResponsavelRedespacho.Codigo == obj.Carga.Filial.Codigo) || obj.Recebedor == null);
            }

            if (expedidorRecebedor > 0)
                result = result.Where(obj => obj.Recebedor.CPF_CNPJ == expedidorRecebedor || obj.Recebedor == null);

            if (somentePendentes)
                result = result.Where(obj => obj.AgInformarRecebedor);

            if (!string.IsNullOrWhiteSpace(codigoPedidoEmbarcador))
                result = result.Where(obj => obj.Pedido.NumeroPedidoEmbarcador == codigoPedidoEmbarcador);

            if (!string.IsNullOrWhiteSpace(codigoCargaEmbarcador))
                result = result.Where(obj => obj.Carga.CodigoCargaEmbarcador == codigoCargaEmbarcador);

            if (origem > 0)
                result = result.Where(obj => obj.Origem.Codigo == origem);

            if (destino > 0)
                result = result.Where(obj => obj.Destino.Codigo == destino);

            if (apenasEmpresaPermiteEncaixe)
                result = result.Where(obj => obj.Carga.Empresa.PermiteEmitirSubcontratacao);
            //result = result.Where(obj => obj.Carga.Empresa.PermiteEmitirSubcontratacao || configuracaoGeralCarga.PermitirEncaixarPedidosComReentregaSolicitada ? obj.ReentregaSolicitada : false);

            if (numeroNF > 0)
                result = result.Where(obj => obj.NotasFiscais.Any(nf => nf.XMLNotaFiscal.Numero == numeroNF));

            if (numeroCTe > 0)
                result = result.Where(obj => obj.Carga.CargaCTes.Any(cte => cte.CTe.Numero == numeroCTe));

            if (!string.IsNullOrWhiteSpace(estadoDestino) && estadoDestino != "0")
                result = result.Where(obj => obj.Destino.Estado.Sigla == estadoDestino);

            if (cargasUtilizadas != null && cargasUtilizadas.Count > 0)
            {
                if (buscarPorCargaOrigem)
                    result = result.Where(obj => cargasUtilizadas.Contains(obj.CargaOrigem.Codigo));
                else
                    result = result.Where(obj => cargasUtilizadas.Contains(obj.Carga.Codigo));
            }


            return result;
        }

        private IQueryable<Dominio.Entidades.Cliente> ConsultarDestinatarioCarga(bool destinatariosDaCarga, int carga, string nome, double cpfCnpj, string tipo, Dominio.Entidades.Localidade localidade, string telefone, int codigoGrupoPessoas, bool buscarPorCargaOrigem)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            if (buscarPorCargaOrigem)
                consultaCargaPedido = consultaCargaPedido.Where(cargaPedido => cargaPedido.CargaOrigem.Codigo == carga);
            else
                consultaCargaPedido = consultaCargaPedido.Where(cargaPedido => cargaPedido.Carga.Codigo == carga);

            var consultaCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>()
                .Where(cliente => consultaCargaPedido.Any(cargaPedido => cargaPedido.Pedido.Destinatario.CPF_CNPJ == cliente.CPF_CNPJ));

            if (!string.IsNullOrWhiteSpace(nome))
                consultaCliente = consultaCliente.Where(cliente => cliente.Nome.Contains(nome));

            if (!string.IsNullOrWhiteSpace(tipo))
                consultaCliente = consultaCliente.Where(cliente => cliente.Tipo.Equals(tipo));

            if (localidade != null)
                consultaCliente = consultaCliente.Where(cliente => cliente.Localidade == localidade);

            if (!string.IsNullOrWhiteSpace(telefone))
                consultaCliente = consultaCliente.Where(cliente => cliente.Telefone1.Equals(telefone));

            if (cpfCnpj > 0)
                consultaCliente = consultaCliente.Where(cliente => cliente.CPF_CNPJ == cpfCnpj);

            if (codigoGrupoPessoas > 0)
                consultaCliente = consultaCliente.Where(cliente => cliente.GrupoPessoas.Codigo == codigoGrupoPessoas || cliente.GrupoPessoas.Codigo == codigoGrupoPessoas);

            return consultaCliente;
        }

        #endregion

        #region Métodos Privados - Agendamento Entrega Pedido

        private string ObterSelectCamposAgendamentoEntregaPedido(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoEntrega filtrosPesquisa)
        {
            return $@"     CargaPedido.CPE_CODIGO AS CodigoCargaPedido,
                           Pedido.PED_CODIGO AS CodigoPedido,
                           CargaEntrega.CEN_DISTANCIA AS Distancia,                                                           
                           CargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA AS DataEntregaReprogramada,
                           Carga.CAR_DATA_INICIO_VIAGEM AS DataInicioViagem,
                           CargaEntrega.CEN_DATA_ENTREGA_PREVISTA AS DataEntregaPrevista,
                           ISNULL( CargaEntrega.CEN_SENHA_ENTREGA,Pedido.PED_SENHA_AGENDAMENTO) AS SenhaEntregaAgendamento,
                           Carga.CAR_DATA_TERMINO_CARGA AS DataTerminoCarga,
                           Carga.CAR_DATA_CRIACAO AS DataCriacaoCarga,
                           Carga.CAR_DATA_CARREGAMENTO AS DataCarregamento,
                           Carga.CAR_DATA_INICIO_VIAGEM_PREVISTA AS DataInicioViagemPrevista,
                           configuracaoGeral.CEM_PREVISAO_ENTREGA_VELOCIDADE_MEDIA_CARREGADO PrevisaoEntregaVelocidadeMedia,
                           configuracaoGeral.CEM_DATA_BASE_PARA_CALCULO_PREVISAO_CONTROLE_ENTREGA DataBaseCalculoPrevisaoControleEntrega,
                           CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO AS InicioCarregamento,
                           Carga.CAR_CODIGO AS CodigoCarga,
                           Carga.CAR_CODIGO_CARGA_EMBARCADOR AS Carga,
                           Transportador.EMP_CODIGO AS CodigoTransportador,
                           TipoOperacao.TOP_DESCRICAO AS TipoOperacao,
                           Pedido.PED_SITUACAO_AGENDAMENTO_ENTREGA_PEDIDO AS SituacaoAgendamento,
                           Destinatario.CLI_NOME AS Cliente,
                           Destinatario.CLI_CGCCPF AS CPFCNPJCliente, 
                           Destinatario.CLI_CODIGO_INTEGRACAO AS CodigoIntegracaoCliente,
                           Destinatario.CLI_EXIGE_QUE_ENTREGAS_SEJAM_AGENDADAS AS ExigeAgendamento,
                           Destino.LOC_DESCRICAO AS Destino,
                           Destino.UF_SIGLA AS UFDestino,
                           Transportador.EMP_FANTASIA AS Transportador,
                           Carga.CAR_DATA_SUGESTAO_ENTREGA DataSugestaoEntrega,
                           Pedido.PED_TIPO_AGENDAMENTO_ENTREGA TipoAgendamentoEntrega,
                           Pedido.CAR_DATA_CARREGAMENTO_PEDIDO DataCarregamentoInicial,
                           Pedido.PED_DATA_TERMINO_CARREGAMENTO DataCarregamentoFinal,
                           Pedido.PED_DATA_AGENDAMENTO DataAgendamento,
                           Carga.CAR_SITUACAO SituacaoViagem,
                           Pedido.PED_PREVISAO_ENTREGA DataPrevisaoEntrega,
                           TipoOperacao.TOP_PERMITE_AGENDAR_ENTREGA_SOMENTE_APOS_INICIO_VIAGEM_CARGA PermiteAgendarEntregaSomenteAposInicioViagem,
                           Destinatario.CLI_EXIGE_QUE_ENTREGAS_SEJAM_AGENDADAS ExigeAgendarEntregas,
                           Destinatario.CLI_PERMITE_AGENDAR_COM_VIAGEM_INICIADA PermiteAgendarComViagemIniciada,
                           Carga.CAR_SITUACAO SituacaoCarga,
                           Pedido.PED_DATA_CRIACAO DataCriacaoPedido,
                           TipoOperacao.TOP_PERMITIR_AGENDAR_DESCARGA_APOS_DATA_ENTREGA_SUGERIDA PermitirAgendarDescargaAposDataEntregaSugerida,
                           Pedido.PED_OBSERVACAO_SOLICITACAO_REAGENDAMENTO ObservacaoReagendamento,
                           Pedido.PED_OBSERVACAO ObservacaoPedido,
                           Pedido.PED_NUMERO_PEDIDO_EMBARCADOR NumeroPedidoEmbarcador,
                           Pedido.PED_NUMERO_ORDEM NumeroOrdemPedido,
                           Pedido.PED_DATA_PREVISAO_SAIDA DataPrevisaoSaida,
                           ClienteDescarga.CLD_TEMPO_AGENDAMENTO TempoAgendamento,
                           (
                            SELECT TOP 1 _auditoriaConsulta.APA_DATA 
                              FROM T_AGENDAMENTO_ENTREGA_PEDIDO_CONSULTA_AUDITORIA _auditoriaConsulta 
                             WHERE _auditoriaConsulta.PED_CODIGO = Pedido.PED_CODIGO 
                             ORDER BY APA_DATA DESC
                           ) UltimaConsultaTransportador,
                           Pedido.PED_QUANTIDADE_VOLUMES QuantidadeVolumes,
                           Pedido.PED_CUBAGEM_TOTAL QuantidadeMetrosCubicos,
                           (
                            SELECT COUNT(1) 
                              FROM T_PEDIDO_XML_NOTA_FISCAL _pedidoXmlNotaFiscal
                               WHERE _pedidoXmlNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
                           ) NumeroNotasFiscaisDoPedido,
                            SUBSTRING((
                            					SELECT DISTINCT ', ' + _NotasFiscais.NumeroNotaFiscal
                            					  FROM (
                            							   SELECT CAST(_NotaFiscal.NF_NUMERO AS VARCHAR) NumeroNotaFiscal
                            								 FROM T_XML_NOTA_FISCAL _NotaFiscal
                            								 JOIN T_PEDIDO_NOTA_FISCAL_PARCIAL _NotaFiscalParcial ON _NotaFiscalParcial.CNP_NUMERO = _NotaFiscal.NF_NUMERO
                            								WHERE _NotaFiscalParcial.PED_CODIGO = Pedido.PED_CODIGO
                            								UNION
                            							   SELECT CAST(_NotaFiscal.NF_NUMERO AS VARCHAR) NumeroNotaFiscal
                            								 FROM T_XML_NOTA_FISCAL _NotaFiscal
                            								 JOIN T_PEDIDO_XML_NOTA_FISCAL _PedidoXMLNotaFiscal ON _NotaFiscal.NFX_CODIGO = _PedidoXMLNotaFiscal.NFX_CODIGO
                            								WHERE _PedidoXMLNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
                            						   ) _NotasFiscais
                            					   FOR XML PATH('')
                            				), 3, 1000) NFe,
                            (
                             SELECT TOP 1 _nfSituacao.NFS_DESCRICAO 
                               FROM T_NOTA_FISCAL_SITUACAO _nfSituacao 
                               JOIN T_XML_NOTA_FISCAL _xmlNotaFiscal 
                                 ON _nfSituacao.NFS_CODIGO = _xmlNotaFiscal.NFS_CODIGO
                               JOIN T_PEDIDO_XML_NOTA_FISCAL _pedidoXmlNotaFiscal 
                                 ON _pedidoXmlNotaFiscal.NFX_CODIGO = _xmlNotaFiscal.NFX_CODIGO
                               WHERE _pedidoXmlNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
                            ) SituacaoNotaFiscal,
                            Pedido.PED_DATA_AGENDAMENTO DataLiberadoAgendamento,
                            Coalesce((Select top 1 FUN_NOME from T_FUNCIONARIO WHERE FUN_CODIGO = Pedido.FUN_CODIGO_RESPONSAVEL_AGENDAMENTO),'') UsuarioAgendamento,
                            Pedido.PED_DATA_ASSUMIU_AGENDAMENTO DataUsuarioAssumiuAgendamento,
                            ClienteDescarga.CLD_FORMA_AGENDAMENTO FormaAgendamento,
                            Destinatario.CLI_FONE TelefoneCliente,
                            ClienteDescarga.CLD_LINK_PARA_AGENDAMENTO LinkAgendamento,
                            ClienteDescarga.CLD_EXIGE_SENHA_NO_AGENDAMENTO ExigeSenhaAgendamento,
                            Coalesce((
                                SELECT STRING_AGG(COE_EMAIL, ', ')
	                            FROM T_CLIENTE_OUTRO_EMAIL EMAILS WHERE EMAILS.CLI_CGCCPF = Destinatario.CLI_CGCCPF and COE_EMAIL_STATUS = 'A' AND COE_EMAIL_TIPO = 8
                                ),'') EmailAgendamento,
                            Case When {(filtrosPesquisa?.ObrigaCarga ?? false ? "1" : "0")} = 0 and Coalesce(ClienteDescarga.CLD_EXIGE_AGENDAMENTO,0) = 1 and Coalesce(ClienteDescarga.CLD_AGENDAMENTO_EXIGE_NOTA_FISCAL,0) = 1 Then
                                    Coalesce((
                                        SELECT NF_DATA_EMISSAO FROM T_PEDIDO_XML_NOTA_FISCAL _pedidoNotaFiscal 
                                        INNER JOIN T_XML_NOTA_FISCAL _nf ON _nf.NFX_CODIGO = _pedidoNotaFiscal.NFX_CODIGO
                                        WHERE _pedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO)
                                    ,'')
                                 When {(filtrosPesquisa?.ObrigaCarga ?? false ? "1" : "0")} = 0 and Coalesce(ClienteDescarga.CLD_EXIGE_AGENDAMENTO,0) = 1 and Coalesce(ClienteDescarga.CLD_AGENDAMENTO_EXIGE_NOTA_FISCAL,0) = 0 Then
                                    Coalesce(Carga.CAR_DATA_CRIACAO,'')
                                 Else
                                    ''
                            End DataTelaAgendamento,
                            CAST({(filtrosPesquisa?.ObrigaCarga ?? false ? "1" : "0")} as bit) ObrigaCarga,
                            Pedido.PED_DATA_E_HORA_ENVIO_EMAIL_AGENDAMENTO DataeHoraEnvioEmailAgendamento, 
                            Pedido.PED_DATA_SUGESTAO_REAGENDAMENTO_DESCARGA DataHoraSugestaoReagendamento ";
        }

        private string ObterFromAgendamentoEntregaPedido()
        {
            return @" T_CARGA_PEDIDO CargaPedido
		                 JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
		                 LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO
		                 LEFT JOIN T_CARGA_JANELA_CARREGAMENTO JanelaCarregamento ON CargaPedido.CAR_CODIGO = JanelaCarregamento.CAR_CODIGO
		                 LEFT JOIN T_EMPRESA Transportador ON Transportador.EMP_CODIGO = Carga.EMP_cODIGO
		                 LEFT JOIN T_TIPO_OPERACAO TipoOperacao ON TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO
		                 JOIN T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = Pedido.CLI_CODIGO
                         LEFT JOIN T_CLIENTE_DESCARGA ClienteDescarga on ClienteDescarga.CLI_CGCCPF = Destinatario.CLI_CGCCPF
		                 LEFT JOIN T_LOCALIDADES Destino on Destino.LOC_CODIGO = Destinatario.LOC_CODIGO
                         LEFT JOIN T_CARGA_ENTREGA_PEDIDO CargaEntregaPedido on CargaEntregaPedido.CPE_CODIGO = CargaPedido.CPE_CODIGO
                         LEFT JOIN T_CARGA_ENTREGA CargaEntrega on CargaEntrega.CEN_CODIGO = CargaEntregaPedido.CEN_CODIGO And CargaEntrega.CEN_COLETA = 0
                         LEFT JOIN T_CARGA_JANELA_CARREGAMENTO CargaJanelaCarregamento on CargaJanelaCarregamento.CAR_CODIGO = Carga.CAR_CODIGO
                         , T_CONFIGURACAO_EMBARCADOR configuracaoGeral";
        }

        private string ObterWhereAgendamentoEntregaPedido(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoEntrega filtrosPesquisa)
        {
            string pattern = "yyyy-MM-dd";

            StringBuilder where = new StringBuilder(@" Pedido.PED_SITUACAO_AGENDAMENTO_ENTREGA_PEDIDO IS NOT NULL 
                                                         AND NOT EXISTS (
                                                                         SELECT TOP 1 1 FROM T_PEDIDO_XML_NOTA_FISCAL _pedidoXmlNotaFiscal 
                                                                           JOIN T_XML_NOTA_FISCAL _xmlNotaFiscal on _xmlNotaFiscal.NFX_CODIGO = _pedidoXmlNotaFiscal.NFX_CODIGO
                                                                           JOIN T_NOTA_FISCAL_SITUACAO _notaFiscalSituacao on _notaFiscalSituacao.NFS_CODIGO = _xmlNotaFiscal.NFS_CODIGO
                                                                           WHERE _notaFiscalSituacao.NFS_BLOQUEAR_VISUALIZACAO_AGENDAMENTO_ENTREGA_PEDIDO = 1 
                                                                             AND CargaPedido.CPE_CODIGO = _pedidoXmlNotaFiscal.CPE_CODIGO
                                                                        ) ");

            if (filtrosPesquisa.SomenteCargasFinalizadas)
                where.Append("AND Carga.CAR_SITUACAO = 11 ");
            else
                where.Append("AND Carga.CAR_SITUACAO NOT IN (13, 18, 11) ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Carga))
                where.Append($" AND Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.Carga}'");

            if (filtrosPesquisa.SituacoesAgendamento.Count > 0)
                where.Append($" AND Pedido.PED_SITUACAO_AGENDAMENTO_ENTREGA_PEDIDO IN ({string.Join(", ", filtrosPesquisa.SituacoesAgendamento.Select(x => (int)x).ToList())})");

            if (filtrosPesquisa.CodigosClientes.Count > 0)
                where.Append($" AND Pedido.CLI_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosClientes)})");

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                where.Append($" AND Carga.TOP_CODIGO = {filtrosPesquisa.CodigoTipoOperacao}");

            if (filtrosPesquisa.CodigosTransportadores.Count > 0)
                where.Append($" AND Carga.EMP_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosTransportadores)})");

            if (filtrosPesquisa.DataAgendamentoInicial.HasValue)
                where.Append($" AND Pedido.PED_DATA_AGENDAMENTO >= '{filtrosPesquisa.DataAgendamentoInicial.Value.ToString(pattern)} 00:00:00'");

            if (filtrosPesquisa.DataAgendamentoFinal.HasValue)
                where.Append($" AND Pedido.PED_DATA_AGENDAMENTO <= '{filtrosPesquisa.DataAgendamentoFinal.Value.ToString(pattern)} 23:59:59'");

            if (filtrosPesquisa.DataCarregamentoInicial.HasValue)
                where.Append($" AND JanelaCarregamento.CJC_INICIO_CARREGAMENTO >= '{filtrosPesquisa.DataCarregamentoInicial.Value.ToString(pattern)} 00:00:00'");

            if (filtrosPesquisa.DataCarregamentoFinal.HasValue)
                where.Append($" AND JanelaCarregamento.CJC_INICIO_CARREGAMENTO <= '{filtrosPesquisa.DataCarregamentoFinal.Value.ToString(pattern)} 23:59:59'");

            if (filtrosPesquisa.DataPrevisaoEntregaInicial.HasValue)
                where.Append($" AND Pedido.PED_PREVISAO_ENTREGA >= '{filtrosPesquisa.DataPrevisaoEntregaInicial.Value.ToString(pattern)} 00:00:00'");

            if (filtrosPesquisa.DataPrevisaoEntregaFinal.HasValue)
                where.Append($" AND Pedido.PED_PREVISAO_ENTREGA <= '{filtrosPesquisa.DataPrevisaoEntregaFinal.Value.ToString(pattern)} 23:59:59'");

            if (filtrosPesquisa.DataCriacaoPedidoInicial.HasValue)
                where.Append($" AND Pedido.PED_DATA_CRIACAO >= '{filtrosPesquisa.DataCriacaoPedidoInicial.Value.ToString(pattern)} 00:00:00'");

            if (filtrosPesquisa.DataCriacaoPedidoFinal.HasValue)
                where.Append($" AND Pedido.PED_DATA_CRIACAO <= '{filtrosPesquisa.DataCriacaoPedidoFinal.Value.ToString(pattern)} 23:59:59'");

            if (filtrosPesquisa.NFe > 0)
                where.Append($@" AND EXISTS  (
							   SELECT _NotaFiscal.NFX_CODIGO
								 FROM T_XML_NOTA_FISCAL _NotaFiscal
								 JOIN T_PEDIDO_NOTA_FISCAL_PARCIAL _NotaFiscalParcial ON _NotaFiscalParcial.CNP_NUMERO = _NotaFiscal.NF_NUMERO
								WHERE _NotaFiscalParcial.PED_CODIGO = Pedido.PED_CODIGO
								  AND _NotaFiscalParcial.CNP_NUMERO = {filtrosPesquisa.NFe}
								UNION
							   SELECT _NotaFiscal.NFX_CODIGO
								 FROM T_XML_NOTA_FISCAL _NotaFiscal
								 JOIN T_PEDIDO_XML_NOTA_FISCAL _PedidoXMLNotaFiscal ON _NotaFiscal.NFX_CODIGO = _PedidoXMLNotaFiscal.NFX_CODIGO
								WHERE _PedidoXMLNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
								  AND _NotaFiscal.NF_NUMERO = {filtrosPesquisa.NFe}
							)");

            if (filtrosPesquisa.PossuiDataSugestaoEntrega.HasValue)
            {
                if (filtrosPesquisa.PossuiDataSugestaoEntrega.Value)
                    where.Append($" AND Carga.CAR_DATA_SUGESTAO_ENTREGA IS NOT NULL");
                else if (!filtrosPesquisa.PossuiDataSugestaoEntrega.Value)
                    where.Append($" AND Carga.CAR_DATA_SUGESTAO_ENTREGA IS NULL");
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOrdem))
            {
                if (filtrosPesquisa.NumeroOrdem.Contains(","))
                {
                    List<string> NumeroOrdemFiltro = filtrosPesquisa.NumeroOrdem.Split(',').Select(s => s.Trim()).Distinct().ToList();
                    where.Append($" AND Pedido.PED_NUMERO_ORDEM IN ('{string.Join("','", NumeroOrdemFiltro)}')");
                }
                else
                    where.Append($" AND Pedido.PED_NUMERO_ORDEM = '{filtrosPesquisa.NumeroOrdem}'");
            }

            if (filtrosPesquisa.PossuiDataTerminoCarregamento.HasValue)
            {
                if (filtrosPesquisa.PossuiDataTerminoCarregamento.Value)
                    where.Append($" AND Pedido.PED_DATA_TERMINO_CARREGAMENTO IS NOT NULL");
                else if (!filtrosPesquisa.PossuiDataTerminoCarregamento.Value)
                    where.Append($" AND Pedido.PED_DATA_TERMINO_CARREGAMENTO IS NULL");
            }

            if (filtrosPesquisa.PossuiNotaFiscalVinculada.HasValue)
            {
                if (filtrosPesquisa.PossuiNotaFiscalVinculada.Value == SimNao.Sim)
                    where.Append($" AND EXISTS (SELECT TOP 1 1 FROM T_PEDIDO_XML_NOTA_FISCAL _pedidoNotaFiscal WHERE _pedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO)");
                else
                    where.Append($" AND NOT EXISTS (SELECT TOP 1 1 FROM T_PEDIDO_XML_NOTA_FISCAL _pedidoNotaFiscal WHERE _pedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO)");
            }

            if (filtrosPesquisa.DataInicialSugestaoEntrega.HasValue)
                where.Append($" AND Carga.CAR_DATA_SUGESTAO_ENTREGA >= '{filtrosPesquisa.DataInicialSugestaoEntrega.Value.ToString(pattern)} 00:00:00'");

            if (filtrosPesquisa.DataFinalSugestaoEntrega.HasValue)
                where.Append($" AND Carga.CAR_DATA_SUGESTAO_ENTREGA <= '{filtrosPesquisa.DataFinalSugestaoEntrega.Value.ToString(pattern)} 23:59:59'");

            if (filtrosPesquisa.CodigosSituacaoNotaFiscal.Count > 0)
                where.Append($@" AND EXISTS 
                                    (
                                     SELECT TOP 1 1 
                                       FROM T_PEDIDO_XML_NOTA_FISCAL _pedidoNotaFiscal 
                                       JOIN T_XML_NOTA_FISCAL _xmlNotaFiscal ON _xmlNotaFiscal.NFX_CODIGO = _pedidoNotaFiscal.NFX_CODIGO 
                                       JOIN T_NOTA_FISCAL_SITUACAO _notaFiscalSituacao ON _notaFiscalSituacao.NFS_CODIGO = _xmlNotaFiscal.NFS_CODIGO
                                      WHERE _pedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO AND _notaFiscalSituacao.NFS_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosSituacaoNotaFiscal)})
                                     )");

            if (filtrosPesquisa.CanalEntrega != null && filtrosPesquisa.CanalEntrega.Count > 0)
                where.Append($@" AND Pedido.CNE_CODIGO in ({string.Join(", ", filtrosPesquisa.CanalEntrega)})");

            if (filtrosPesquisa?.ObrigaCarga ?? false)
                where.Append($@" AND Carga.Car_CODIGO is not null");
            else
                where.Append($@" AND ((ClienteDescarga.CLD_EXIGE_AGENDAMENTO = 1 or Destinatario.CLI_EXIGE_QUE_ENTREGAS_SEJAM_AGENDADAS = 1) and (((ClienteDescarga.CLD_AGENDAMENTO_EXIGE_NOTA_FISCAL = 0 or ClienteDescarga.CLD_AGENDAMENTO_EXIGE_NOTA_FISCAL is null) and Carga.CAR_CODIGO is not null) or (ClienteDescarga.CLD_AGENDAMENTO_EXIGE_NOTA_FISCAL = 1 AND Carga.CAR_CODIGO is not null AND Exists (SELECT TOP 1 1 FROM T_PEDIDO_XML_NOTA_FISCAL _notafiscalpedidofiltro WHERE _notafiscalpedidofiltro.CPE_CODIGO = CargaPedido.CPE_CODIGO))))");

            if (filtrosPesquisa.DataInicialCriacaoDaCarga.HasValue)
                where.Append($" AND Carga.CAR_DATA_CRIACAO >= '{filtrosPesquisa.DataInicialCriacaoDaCarga.Value.ToString(pattern)} 00:00:00'");

            if (filtrosPesquisa.DataFinalCriacaoDaCarga.HasValue)
                where.Append($" AND Carga.CAR_DATA_CRIACAO <= '{filtrosPesquisa.DataFinalCriacaoDaCarga.Value.ToString(pattern)} 23:59:59'");

            if (!string.IsNullOrEmpty(filtrosPesquisa.SenhaEntregaAgendamento))
                where.Append($" AND CargaEntrega.CEN_SENHA_ENTREGA = '{filtrosPesquisa.SenhaEntregaAgendamento}'");

            if (filtrosPesquisa.EntegasComSenhaDeAgendamento.HasValue)
            {
                if (filtrosPesquisa.EntegasComSenhaDeAgendamento.Value)
                    where.Append($" AND (CargaEntrega.CEN_SENHA_ENTREGA IS NOT NULL AND CargaEntrega.CEN_SENHA_ENTREGA != '') ");
                else
                    where.Append($" AND (CargaEntrega.CEN_SENHA_ENTREGA IS NULL OR CargaEntrega.CEN_SENHA_ENTREGA = '') ");
            }

            if (filtrosPesquisa.SiglasUFDestino.Count > 0)
                where.Append($" And Destino.UF_SIGLA IN ({string.Join(", ", filtrosPesquisa.SiglasUFDestino.Select(x => $"'{x}'"))})");

            return where.ToString();
        }

        #endregion

        #region Métodos Privados - Agendamento Entrega Pedido Agrupado

        private string ObterCommonTableExpressionAgendamentoEntregaPedidoAgrupado()
        {
            return $@"With Pedidos As
                     (
                         Select CargaEntregaPedido.CEN_CODIGO CodigoCargaEntrega,
		                     Row_Number() Over (Partition By CargaEntregaPedido.CEN_CODIGO Order By CargaEntregaPedido.CEN_CODIGO, Pedido.PED_CODIGO Desc) RowNum,
		                     Pedido.PED_CODIGO CodigoPedido,
		                     Pedido.PED_SITUACAO_AGENDAMENTO_ENTREGA_PEDIDO SituacaoAgendamento,
		                     Pedido.PED_DATA_ASSUMIU_AGENDAMENTO DataUsuarioAssumiuAgendamento,
		                     Pedido.PED_DATA_AGENDAMENTO DataLiberadoAgendamento,
		                     Pedido.PED_TIPO_AGENDAMENTO_ENTREGA TipoAgendamentoEntrega,
		                     Pedido.CAR_DATA_CARREGAMENTO_PEDIDO DataCarregamentoInicial,
		                     Pedido.PED_DATA_TERMINO_CARREGAMENTO DataCarregamentoFinal,
		                     Pedido.PED_DATA_AGENDAMENTO DataAgendamento,
		                     Pedido.PED_PREVISAO_ENTREGA DataPrevisaoEntrega,
		                     Pedido.PED_DATA_CRIACAO DataCriacaoPedido,
		                     Pedido.PED_OBSERVACAO_SOLICITACAO_REAGENDAMENTO ObservacaoReagendamento,
		                     Pedido.PED_OBSERVACAO ObservacaoPedido,
		                     Pedido.PED_NUMERO_PEDIDO_EMBARCADOR NumeroPedidoEmbarcador,
		                     Pedido.PED_NUMERO_ORDEM NumeroOrdemPedido,
		                     Pedido.PED_DATA_PREVISAO_SAIDA DataPrevisaoSaida,
		                     Pedido.PED_QUANTIDADE_VOLUMES QuantidadeVolumes,
		                     Pedido.PED_CUBAGEM_TOTAL QuantidadeMetrosCubicos,
		                     Pedido.PED_DATA_E_HORA_ENVIO_EMAIL_AGENDAMENTO DataeHoraEnvioEmailAgendamento,
		                     Pedido.PED_DATA_SUGESTAO_REAGENDAMENTO_DESCARGA DataHoraSugestaoReagendamento,
		                     Pedido.FUN_CODIGO_RESPONSAVEL_AGENDAMENTO UsuarioAgendamento,
		                     Pedido.CNE_CODIGO CanalEntrega,
                             Pedido.PED_SENHA_AGENDAMENTO as SenhaAgendamento
	                     From T_CARGA_ENTREGA_PEDIDO CargaEntregaPedido
		                     Left Join T_CARGA_PEDIDO CargaPedido On CargaPedido.CPE_CODIGO = CargaEntregaPedido.CPE_CODIGO
		                     Left Join T_PEDIDO Pedido On Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                     ),
                     NotasFiscais As
                     (
	                     Select Distinct
		                     CargaEntregaPedido.CEN_CODIGO CodigoEntrega,
		                     _PedidoXMLNotaFiscal.CPE_CODIGO CodigoPedido,
		                     Cast(_NotaFiscal.NF_NUMERO As VarChar) NumeroNotaFiscal,
		                     _NotaFiscal.NF_DATA_EMISSAO DataEmissao,
                             _NotaFiscal.NFS_CODIGO CodigoSituacao,
		                     _nfSituacao.NFS_DESCRICAO DescricaoSituacao,
		                     _nfSituacao.NFS_BLOQUEAR_VISUALIZACAO_AGENDAMENTO_ENTREGA_PEDIDO BloquearVisualizacaoAgendamentoEntregaPedido
	                     From T_CARGA_ENTREGA_PEDIDO CargaEntregaPedido
		                     Left Join T_CARGA_PEDIDO CargaPedido On CargaPedido.CPE_CODIGO = CargaEntregaPedido.CPE_CODIGO
		                     Left Join T_PEDIDO_XML_NOTA_FISCAL _PedidoXMLNotaFiscal On _PedidoXMLNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
		                     Left Join T_XML_NOTA_FISCAL _NotaFiscal On _NotaFiscal.NFX_CODIGO = _PedidoXMLNotaFiscal.NFX_CODIGO
		                     Left Join T_NOTA_FISCAL_SITUACAO _nfSituacao On _nfSituacao.NFS_CODIGO = _NotaFiscal.NFS_CODIGO
                     )";
        }

        private string ObterSelectCamposAgendamentoEntregaPedidoAgrupado(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoEntrega filtrosPesquisa)
        {
            return $@"CargaEntrega.CEN_CODIGO CodigoCargaEntrega,
		        CargaEntrega.CEN_DISTANCIA Distancia,
		        CargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA DataEntregaReprogramada,
		        CargaEntrega.CEN_DATA_ENTREGA_PREVISTA DataEntregaPrevista,
		        ISNULL(CargaEntrega.CEN_SENHA_ENTREGA, PrimeiroPedido.SenhaAgendamento) SenhaEntregaAgendamento,
                
		        Carga.CAR_CODIGO CodigoCarga,
		        Carga.CAR_CODIGO_CARGA_EMBARCADOR Carga,
		        Carga.CAR_SITUACAO SituacaoCarga,
		        Carga.CAR_SITUACAO SituacaoViagem,
		        Carga.CAR_DATA_SUGESTAO_ENTREGA DataSugestaoEntrega,
		        Carga.CAR_DATA_INICIO_VIAGEM_PREVISTA DataInicioViagemPrevista,
		        Carga.CAR_DATA_INICIO_VIAGEM DataInicioViagem,
		        Carga.CAR_DATA_TERMINO_CARGA DataTerminoCarga,
		        Carga.CAR_DATA_CRIACAO DataCriacaoCarga,
		        Carga.CAR_DATA_CARREGAMENTO DataCarregamento,
		        
		        TipoOperacao.TOP_DESCRICAO TipoOperacao,
		        TipoOperacao.TOP_PERMITE_AGENDAR_ENTREGA_SOMENTE_APOS_INICIO_VIAGEM_CARGA PermiteAgendarEntregaSomenteAposInicioViagem,
		        TipoOperacao.TOP_PERMITIR_AGENDAR_DESCARGA_APOS_DATA_ENTREGA_SUGERIDA PermitirAgendarDescargaAposDataEntregaSugerida,
		        
		        SubString((Select Distinct ', '+Cast(CodigoPedido As VarChar) From Pedidos PedidosCargaEntrega Where PedidosCargaEntrega.CodigoCargaEntrega = CargaEntrega.CEN_CODIGO For Xml Path('')), 3, 1000) CodigoPedidoAgrupado,
		        SubString((Select Distinct ', '+Cast(NumeroPedidoEmbarcador As VarChar) From Pedidos PedidosCargaEntrega Where PedidosCargaEntrega.CodigoCargaEntrega = CargaEntrega.CEN_CODIGO For Xml Path('')), 3, 1000) NumeroPedidoEmbarcador,
		        SubString((Select Distinct ', '+Cast(NumeroOrdemPedido As VarChar) From Pedidos PedidosCargaEntrega Where PedidosCargaEntrega.CodigoCargaEntrega = CargaEntrega.CEN_CODIGO For Xml Path('')), 3, 1000) NumeroOrdemPedido,
		        (Select Sum(QuantidadeVolumes) From Pedidos PedidosCargaEntrega Where PedidosCargaEntrega.CodigoCargaEntrega = CargaEntrega.CEN_CODIGO) QuantidadeVolumes,
		        (Select Sum(QuantidadeMetrosCubicos) From Pedidos PedidosCargaEntrega Where PedidosCargaEntrega.CodigoCargaEntrega = CargaEntrega.CEN_CODIGO) QuantidadeMetrosCubicos,
		        (Select Count(1) From Pedidos PedidosCargaEntrega Where PedidosCargaEntrega.CodigoCargaEntrega = CargaEntrega.CEN_CODIGO) QuantidadePedidosDaEntrega,
		        PrimeiroPedido.CodigoPedido,
		        PrimeiroPedido.DataUsuarioAssumiuAgendamento,
		        PrimeiroPedido.DataLiberadoAgendamento,
		        PrimeiroPedido.SituacaoAgendamento,
		        PrimeiroPedido.TipoAgendamentoEntrega,
		        PrimeiroPedido.DataCarregamentoInicial,
		        PrimeiroPedido.DataCarregamentoFinal,
		        PrimeiroPedido.DataAgendamento,
		        PrimeiroPedido.DataPrevisaoEntrega,
                SubString((Select Distinct ', '+Format(DataCriacaoPedido,'yyyy-MM-dd HH:mm:ss.fff') From Pedidos PedidosCargaEntrega Where PedidosCargaEntrega.CodigoCargaEntrega = CargaEntrega.CEN_CODIGO For Xml Path('')), 3, 1000) DataCriacaoPedidoAgrupado,
		        PrimeiroPedido.DataCriacaoPedido,
		        PrimeiroPedido.ObservacaoReagendamento,
		        PrimeiroPedido.ObservacaoPedido,
                SubString((Select Distinct ', '+ObservacaoPedido From Pedidos PedidosCargaEntrega Where PedidosCargaEntrega.CodigoCargaEntrega = CargaEntrega.CEN_CODIGO For Xml Path('')), 3, 1000) ObservacaoPedidoAgrupado,
		        PrimeiroPedido.DataPrevisaoSaida,
		        PrimeiroPedido.DataeHoraEnvioEmailAgendamento,
		        PrimeiroPedido.DataHoraSugestaoReagendamento,
                
		        (Select Count(1) From NotasFiscais Where CodigoEntrega = CargaEntrega.CEN_CODIGO) NumeroNotasFiscaisDoPedido,
		        SubString((Select ', ' + NumeroNotaFiscal From NotasFiscais Where CodigoEntrega = CargaEntrega.CEN_CODIGO For Xml Path('')), 3, 1000) NFe,
		        (Select Top 1 DescricaoSituacao From NotasFiscais Where CodigoEntrega = CargaEntrega.CEN_CODIGO) SituacaoNotaFiscal,
                
		        Destinatario.CLI_NOME Cliente,
		        Destinatario.CLI_CGCCPF CPFCNPJCliente,
		        Destinatario.CLI_CODIGO_INTEGRACAO CodigoIntegracaoCliente,
		        Destinatario.CLI_EXIGE_QUE_ENTREGAS_SEJAM_AGENDADAS ExigeAgendamento,
		        Destinatario.CLI_EXIGE_QUE_ENTREGAS_SEJAM_AGENDADAS ExigeAgendarEntregas,
		        Destinatario.CLI_PERMITE_AGENDAR_COM_VIAGEM_INICIADA PermiteAgendarComViagemIniciada,
		        Destinatario.CLI_FONE TelefoneCliente,
		        Coalesce(
                        (Select String_Agg(COE_EMAIL, ', ')
                            From T_CLIENTE_OUTRO_EMAIL EMAILS
                            Where EMAILS.CLI_CGCCPF = Destinatario.CLI_CGCCPF
                                And COE_EMAIL_STATUS = 'A'
                                And COE_EMAIL_TIPO = 8), '') EmailAgendamento,
                
		        Destino.LOC_DESCRICAO Destino,
                Destino.UF_SIGLA UFDestino,
                
		        ClienteDescarga.CLD_TEMPO_AGENDAMENTO TempoAgendamento,
		        ClienteDescarga.CLD_FORMA_AGENDAMENTO FormaAgendamento,
		        ClienteDescarga.CLD_LINK_PARA_AGENDAMENTO LinkAgendamento,
		        ClienteDescarga.CLD_EXIGE_SENHA_NO_AGENDAMENTO ExigeSenhaAgendamento,
		        Case
		            When {(filtrosPesquisa?.ObrigaCarga ?? false ? "1" : "0")} = 0 And Coalesce(ClienteDescarga.CLD_EXIGE_AGENDAMENTO, 0) = 1 And Coalesce(ClienteDescarga.CLD_AGENDAMENTO_EXIGE_NOTA_FISCAL, 0) = 1
		                Then Coalesce((Select TOP 1 DataEmissao From NotasFiscais Where CodigoEntrega = CargaEntrega.CEN_CODIGO) , '')
		            When {(filtrosPesquisa?.ObrigaCarga ?? false ? "1" : "0")} = 0 And Coalesce(ClienteDescarga.CLD_EXIGE_AGENDAMENTO, 0) = 1 And Coalesce(ClienteDescarga.CLD_AGENDAMENTO_EXIGE_NOTA_FISCAL, 0) = 0
		                Then Coalesce(Carga.CAR_DATA_CRIACAO, '')
		            Else ''
		        End DataTelaAgendamento,
		        Cast({(filtrosPesquisa?.ObrigaCarga ?? false ? "1" : "0")} As Bit) ObrigaCarga,
                
		        Transportador.EMP_CODIGO CodigoTransportador,
		        Transportador.EMP_FANTASIA Transportador,
		        (Select Top 1 _auditoriaConsulta.APA_DATA
		            From T_AGENDAMENTO_ENTREGA_PEDIDO_CONSULTA_AUDITORIA _auditoriaConsulta
		            Where _auditoriaConsulta.PED_CODIGO = (Select Top 1 CodigoPedido From Pedidos Where CodigoCargaEntrega = CargaEntrega.CEN_CODIGO)
		            Order By APA_DATA DESC) UltimaConsultaTransportador,
                
		        CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO InicioCarregamento,
                
		        Coalesce((Select Top 1 FUN_NOME From T_FUNCIONARIO Where FUN_CODIGO = (Select Top 1 UsuarioAgendamento From Pedidos Where CodigoCargaEntrega = CargaEntrega.CEN_CODIGO)), '') UsuarioAgendamento,
		        
		        ConfiguracaoEmbarcador.CEM_PREVISAO_ENTREGA_VELOCIDADE_MEDIA_CARREGADO PrevisaoEntregaVelocidadeMedia,
		        ConfiguracaoEmbarcador.CEM_DATA_BASE_PARA_CALCULO_PREVISAO_CONTROLE_ENTREGA DataBaseCalculoPrevisaoControleEntrega";
        }

        private string ObterFromAgendamentoEntregaPedidoAgrupado()
        {
            return @"T_CARGA_ENTREGA CargaEntrega
                    LEFT join T_CARGA_ENTREGA_PEDIDO CargaEntregaPedido ON CargaEntrega.CEN_CODIGO = CargaEntregaPedido.CEN_CODIGO
                    Left Join T_CLIENTE Destinatario On Destinatario.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
                    Left Join T_CLIENTE_DESCARGA ClienteDescarga On ClienteDescarga.CLI_CGCCPF = Destinatario.CLI_CGCCPF
                    Left Join T_LOCALIDADES Destino On Destino.LOC_CODIGO = Destinatario.LOC_CODIGO
                    Left Join T_CARGA Carga On Carga.CAR_CODIGO = CargaEntrega.CAR_CODIGO
                    Left Join T_TIPO_OPERACAO TipoOperacao On TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO
                    Left Join T_CARGA_JANELA_CARREGAMENTO JanelaCarregamento On JanelaCarregamento.CAR_CODIGO = Carga.CAR_CODIGO
                    Left Join T_EMPRESA Transportador On Transportador.EMP_CODIGO = Carga.EMP_cODIGO 
                    Left Join T_CARGA_JANELA_CARREGAMENTO CargaJanelaCarregamento On CargaJanelaCarregamento.CAR_CODIGO = Carga.CAR_CODIGO
                    Left Join T_CARGA_PEDIDO CargaPedido On CargaPedido.CPE_CODIGO = CargaEntregaPedido.CPE_CODIGO
                    Left Join (Select * From Pedidos PedidosCargaEntrega Where PedidosCargaEntrega.RowNum = 1) PrimeiroPedido On PrimeiroPedido.CodigoCargaEntrega = CargaEntrega.CEN_CODIGO, 
                    T_CONFIGURACAO_EMBARCADOR ConfiguracaoEmbarcador";
        }

        private string ObterWhereAgendamentoEntregaPedidoAgrupado(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoEntrega filtrosPesquisa)
        {
            string pattern = "yyyy-MM-dd";

            StringBuilder where = new StringBuilder(@"CargaEntrega.CEN_COLETA = 0
                                                      And Exists
		                                                  (Select CodigoPedido
		                                                   From  Pedidos PedidosCargaEntrega
		                                                   Where PedidosCargaEntrega.CodigoCargaEntrega = CargaEntrega.CEN_CODIGO
		                                                     And PedidosCargaEntrega.SituacaoAgendamento Is Not Null)
                                                      And Not Exists
                                                          (Select Top 1 1
                                                           From  NotasFiscais NotasFiscaisCargaEntrega
                                                           Where NotasFiscaisCargaEntrega.CodigoEntrega = CargaEntrega.CEN_CODIGO
			                                                 And NotasFiscaisCargaEntrega.BloquearVisualizacaoAgendamentoEntregaPedido = 1)");

            if (filtrosPesquisa.SomenteCargasFinalizadas)
                where.Append(" And Carga.CAR_SITUACAO = 11 ");
            else
                where.Append(" And Carga.CAR_SITUACAO NOT IN (13, 18, 11) ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Carga))
                where.Append($" And Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.Carga}'");

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                where.Append($" And Carga.TOP_CODIGO = {filtrosPesquisa.CodigoTipoOperacao}");

            if (filtrosPesquisa.CodigosTransportadores.Count > 0)
                where.Append($" And Carga.EMP_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosTransportadores)})");

            if (filtrosPesquisa.PossuiDataSugestaoEntrega.HasValue)
            {
                if (filtrosPesquisa.PossuiDataSugestaoEntrega.Value)
                    where.Append($" And Carga.CAR_DATA_SUGESTAO_ENTREGA IS NOT NULL");
                else if (!filtrosPesquisa.PossuiDataSugestaoEntrega.Value)
                    where.Append($" And Carga.CAR_DATA_SUGESTAO_ENTREGA IS NULL");
            }

            if (filtrosPesquisa.DataInicialSugestaoEntrega.HasValue)
                where.Append($" And Carga.CAR_DATA_SUGESTAO_ENTREGA >= '{filtrosPesquisa.DataInicialSugestaoEntrega.Value.ToString(pattern)} 00:00:00'");

            if (filtrosPesquisa.DataFinalSugestaoEntrega.HasValue)
                where.Append($" And Carga.CAR_DATA_SUGESTAO_ENTREGA <= '{filtrosPesquisa.DataFinalSugestaoEntrega.Value.ToString(pattern)} 23:59:59'");

            if (filtrosPesquisa.ObrigaCarga ?? false)
                where.Append($@" And Carga.Car_CODIGO is not null");
            else
                where.Append($@" And ((ClienteDescarga.CLD_EXIGE_AGENDAMENTO = 1 or Destinatario.CLI_EXIGE_QUE_ENTREGAS_SEJAM_AGENDADAS = 1) And (((ClienteDescarga.CLD_AGENDAMENTO_EXIGE_NOTA_FISCAL = 0 or ClienteDescarga.CLD_AGENDAMENTO_EXIGE_NOTA_FISCAL is null) And Carga.CAR_CODIGO is not null) or (ClienteDescarga.CLD_AGENDAMENTO_EXIGE_NOTA_FISCAL = 1 And Carga.CAR_CODIGO is not null And Exists (SELECT TOP 1 1 FROM T_PEDIDO_XML_NOTA_FISCAL _notafiscalpedidofiltro WHERE _notafiscalpedidofiltro.CPE_CODIGO = CargaPedido.CPE_CODIGO))))");

            if (filtrosPesquisa.DataInicialCriacaoDaCarga.HasValue)
                where.Append($" And Carga.CAR_DATA_CRIACAO >= '{filtrosPesquisa.DataInicialCriacaoDaCarga.Value.ToString(pattern)} 00:00:00'");

            if (filtrosPesquisa.DataFinalCriacaoDaCarga.HasValue)
                where.Append($" And Carga.CAR_DATA_CRIACAO <= '{filtrosPesquisa.DataFinalCriacaoDaCarga.Value.ToString(pattern)} 23:59:59'");

            if (filtrosPesquisa.CodigosClientes.Count > 0)
                where.Append($" And CargaEntrega.CLI_CODIGO_ENTREGA IN ({string.Join(", ", filtrosPesquisa.CodigosClientes)})");

            where.Append($@"    And Exists
		                                (Select CodigoPedido
		                                 From Pedidos PedidosCargaEntrega
		                                 Where PedidosCargaEntrega.CodigoCargaEntrega = CargaEntrega.CEN_CODIGO");

            if (filtrosPesquisa.SituacoesAgendamento.Count > 0)
                where.Append($"    And PedidosCargaEntrega.SituacaoAgendamento IN ({string.Join(", ", filtrosPesquisa.SituacoesAgendamento.Select(x => (int)x).ToList())})");

            if (filtrosPesquisa.DataAgendamentoInicial.HasValue)
                where.Append($"    And PedidosCargaEntrega.DataAgendamento >= '{filtrosPesquisa.DataAgendamentoInicial.Value.ToString(pattern)} 00:00:00'");

            if (filtrosPesquisa.DataAgendamentoFinal.HasValue)
                where.Append($"    And PedidosCargaEntrega.DataAgendamento <= '{filtrosPesquisa.DataAgendamentoFinal.Value.ToString(pattern)} 23:59:59'");

            if (filtrosPesquisa.DataPrevisaoEntregaInicial.HasValue)
                where.Append($"    And PedidosCargaEntrega.DataPrevisaoEntrega >= '{filtrosPesquisa.DataPrevisaoEntregaInicial.Value.ToString(pattern)} 00:00:00'");

            if (filtrosPesquisa.DataPrevisaoEntregaFinal.HasValue)
                where.Append($"    And PedidosCargaEntrega.DataPrevisaoEntrega <= '{filtrosPesquisa.DataPrevisaoEntregaFinal.Value.ToString(pattern)} 23:59:59'");

            if (filtrosPesquisa.DataCriacaoPedidoInicial.HasValue)
                where.Append($"    And PedidosCargaEntrega.DataCriacaoPedido >= '{filtrosPesquisa.DataCriacaoPedidoInicial.Value.ToString(pattern)} 00:00:00'");

            if (filtrosPesquisa.DataCriacaoPedidoFinal.HasValue)
                where.Append($"    And PedidosCargaEntrega.DataCriacaoPedido <= '{filtrosPesquisa.DataCriacaoPedidoFinal.Value.ToString(pattern)} 23:59:59'");


            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOrdem))
            {
                if (filtrosPesquisa.NumeroOrdem.Contains(","))
                {
                    List<string> NumeroOrdemFiltro = filtrosPesquisa.NumeroOrdem.Split(',').Select(s => s.Trim()).Distinct().ToList();
                    where.Append($"    And PedidosCargaEntrega.NumeroOrdemPedido In ('{string.Join("','", filtrosPesquisa.NumeroOrdem.Split(',').Select(s => s.Trim()).Distinct().ToList())}')");
                }
                else
                    where.Append($"    And PedidosCargaEntrega.NumeroOrdemPedido = '{filtrosPesquisa.NumeroOrdem}'");
            }

            if (filtrosPesquisa.PossuiDataTerminoCarregamento.HasValue)
            {
                if (filtrosPesquisa.PossuiDataTerminoCarregamento.Value)
                    where.Append($"    And PedidosCargaEntrega.DataCarregamentoFinal IS NOT NULL");
                else if (!filtrosPesquisa.PossuiDataTerminoCarregamento.Value)
                    where.Append($"    And PedidosCargaEntrega.DataCarregamentoFinal IS NULL");
            }

            if (filtrosPesquisa.CanalEntrega != null && filtrosPesquisa.CanalEntrega.Count > 0)
                where.Append($@"    And PedidosCargaEntrega.CanalEntrega in ({string.Join(", ", filtrosPesquisa.CanalEntrega)})");

            where.Append($@"    )");

            if (filtrosPesquisa.NFe > 0)
                where.Append($@" And Exists
                                    (Select Top 1 1
                                     From NotasFiscais NotasFiscaisCargaEntrega
                                     Where  NotasFiscaisCargaEntrega.CodigoEntrega = CargaEntrega.CEN_CODIGO
			                            And NotasFiscaisCargaEntrega.NumeroNotaFiscal = {filtrosPesquisa.NFe})");

            if (filtrosPesquisa.PossuiNotaFiscalVinculada.HasValue)
            {
                where.Append(" And ");
                if (filtrosPesquisa.PossuiNotaFiscalVinculada.Value == SimNao.Nao)
                    where.Append(" Not");
                where.Append($@" Exists 
                                    (Select Top 1 1
                                    From NotasFiscais NotasFiscaisCargaEntrega
                                    Where  NotasFiscaisCargaEntrega.CodigoEntrega = CargaEntrega.CEN_CODIGO)");
            }

            if (filtrosPesquisa.CodigosSituacaoNotaFiscal.Count > 0)
                where.Append($@" Exists 
                                    (Select Top 1 1
                                    From NotasFiscais NotasFiscaisCargaEntrega
                                    Where  NotasFiscaisCargaEntrega.CodigoEntrega = CargaEntrega.CEN_CODIGO
                                        And NotasFiscaisCargaEntrega.CodigoSituacao In ({string.Join(", ", filtrosPesquisa.CodigosSituacaoNotaFiscal)}))");


            if (filtrosPesquisa.DataCarregamentoInicial.HasValue)
                where.Append($" AND JanelaCarregamento.CJC_INICIO_CARREGAMENTO >= '{filtrosPesquisa.DataCarregamentoInicial.Value.ToString(pattern)} 00:00:00'");

            if (filtrosPesquisa.DataCarregamentoFinal.HasValue)
                where.Append($" AND JanelaCarregamento.CJC_INICIO_CARREGAMENTO <= '{filtrosPesquisa.DataCarregamentoFinal.Value.ToString(pattern)} 23:59:59'");

            if (!string.IsNullOrEmpty(filtrosPesquisa.SenhaEntregaAgendamento))
                where.Append($" AND CargaEntrega.CEN_SENHA_ENTREGA = '{filtrosPesquisa.SenhaEntregaAgendamento}'");

            if (filtrosPesquisa.EntegasComSenhaDeAgendamento.HasValue)
            {
                if (filtrosPesquisa.EntegasComSenhaDeAgendamento.Value)
                    where.Append($" AND (CargaEntrega.CEN_SENHA_ENTREGA IS NOT NULL AND CargaEntrega.CEN_SENHA_ENTREGA != '') ");
                else
                    where.Append($" AND (CargaEntrega.CEN_SENHA_ENTREGA IS NULL OR CargaEntrega.CEN_SENHA_ENTREGA = '') ");
            }


            if (filtrosPesquisa.SiglasUFDestino.Count > 0)
                where.Append($" And Destino.UF_SIGLA IN ({string.Join(", ", filtrosPesquisa.SiglasUFDestino.Select(x => $"'{x}'"))})");


            return where.ToString();
        }

        #endregion

        #region Relatório de Componentes de Frete

        public IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteComponentes> BuscarRelatorioFreteComponente(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> proprieadesBuscar, DateTime dataInicial, DateTime dataFinal, double remetente, double destinatario, double tomador, double expedidor, List<double> codigosRecebedor, List<int> codigosFilial, int empresa, List<int> codigosTipoCarga, List<int> codigosTipoOperacao, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true)
        {
            string select = "";
            string groupBy = "";
            setarSelectBuscarRelatorioFreteComponente(ref select, ref groupBy, proprieadesBuscar);

            string query = "select " + select + "isNull(sum(CargaPedido.PED_VALOR_FRETE),0) as ValorFrete, isNull(CargaPedido.PED_ICMS_PAGO_POR_ST, 0) as ICMSPagoPorST from "; // SQL-INJECTION-SAFE

            setarWhereBuscarRelatorioFreteComponente(ref query, dataInicial, dataFinal, remetente, destinatario, tomador, expedidor, codigosRecebedor, codigosFilial, empresa, codigosTipoCarga, codigosTipoOperacao);

            query += " group by " + groupBy + " CargaPedido.PED_ICMS_PAGO_POR_ST ";

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";


            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteComponentes)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteComponentes>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteComponentes>> BuscarRelatorioFreteComponenteAsync(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> proprieadesBuscar, DateTime dataInicial, DateTime dataFinal, double remetente, double destinatario, double tomador, double expedidor, List<double> codigosRecebedor, List<int> codigosFilial, int empresa, List<int> codigosTipoCarga, List<int> codigosTipoOperacao, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true)
        {
            string select = "";
            string groupBy = "";
            setarSelectBuscarRelatorioFreteComponente(ref select, ref groupBy, proprieadesBuscar);

            string query = "select " + select + "isNull(sum(CargaPedido.PED_VALOR_FRETE),0) as ValorFrete, isNull(CargaPedido.PED_ICMS_PAGO_POR_ST, 0) as ICMSPagoPorST from "; // SQL-INJECTION-SAFE

            setarWhereBuscarRelatorioFreteComponente(ref query, dataInicial, dataFinal, remetente, destinatario, tomador, expedidor, codigosRecebedor, codigosFilial, empresa, codigosTipoCarga, codigosTipoOperacao);

            query += " group by " + groupBy + " CargaPedido.PED_ICMS_PAGO_POR_ST ";

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";


            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteComponentes)));

            return await nhQuery.ListAsync<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteComponentes>();
        }

        private void setarSelectBuscarRelatorioFreteComponente(ref string select, ref string groupBy, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> proprieadesBuscar)
        {
            foreach (Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento propriedade in proprieadesBuscar)
            {
                switch (propriedade.Propriedade)
                {
                    case "Transportador":
                        select += "TTransportador.EMP_RAZAO as Transportador, TTransportador.EMP_CNPJ as TransportadorCGC, TLocTransportador.LOC_DESCRICAO + ' - ' + TLocTransportador.UF_SIGLA as TransportadorLocalidade,";
                        groupBy += "TTransportador.EMP_RAZAO,TTransportador.EMP_CNPJ,TLocTransportador.LOC_DESCRICAO,TLocTransportador.UF_SIGLA,";
                        break;

                    case "Remetente":
                        select += "TRemetente.CLI_NOME as Remetente, TRemetente.CLI_CGCCPF as RemetenteCGC,TLocRemetente.LOC_DESCRICAO  + ' - ' +  TLocRemetente.UF_SIGLA as RemetenteLocalidade,";
                        if (!groupBy.Contains("TRemetente.CLI_NOME"))
                            groupBy += "TRemetente.CLI_NOME,TRemetente.CLI_CGCCPF,TLocRemetente.LOC_DESCRICAO,TLocRemetente.UF_SIGLA,";
                        break;

                    case "Destinatario":
                        select += "TDestinatario.CLI_NOME as Destinatario,TDestinatario.CLI_CGCCPF as DestinatarioCGC,TDestinatario.CLI_CGCCPF as DestinatarioCGC, TLocDestinatario.LOC_DESCRICAO + ' - ' + TLocDestinatario.UF_SIGLA as DestinatarioLocalidade,";
                        if (!groupBy.Contains("TDestinatario.CLI_NOME"))
                            groupBy += "TDestinatario.CLI_NOME,TDestinatario.CLI_CGCCPF, TLocDestinatario.LOC_DESCRICAO, TLocDestinatario.UF_SIGLA,";
                        break;

                    case "Tomador":
                        select += "ISNULL(case CargaPedido.PED_TIPO_TOMADOR when 0 then TRemetente.CLI_NOME when 1 then TExpedidor.CLI_NOME when 2 then TRecebedor.CLI_NOME when 3 then TDestinatario.CLI_NOME when 4 then TTomador.CLI_NOME end, '') as Tomador,";
                        select += "ISNULL(case CargaPedido.PED_TIPO_TOMADOR when 0 then TRemetente.CLI_CGCCPF when 1 then TExpedidor.CLI_CGCCPF when 2 then TRecebedor.CLI_CGCCPF when 3 then TDestinatario.CLI_CGCCPF when 4 then TTomador.CLI_CGCCPF end, '') as TomadorCGC,";
                        select += "ISNULL(case CargaPedido.PED_TIPO_TOMADOR when 0 then TLocRemetente.LOC_DESCRICAO  + ' - ' + TLocRemetente.UF_SIGLA when 1 then TLocExpedidor.LOC_DESCRICAO  + ' - ' + TLocExpedidor.UF_SIGLA when 2 then TLocRecebedor.LOC_DESCRICAO  + ' - ' + TLocRecebedor.UF_SIGLA when 3 then TLocDestinatario.LOC_DESCRICAO  + ' - ' + TLocDestinatario.UF_SIGLA when 4 then TLocTomador.LOC_DESCRICAO  + ' - ' + TLocTomador.UF_SIGLA end, '') as TomadorLocalidade,";

                        groupBy += "TTomador.CLI_NOME,TTomador.CLI_CGCCPF,TLocTomador.LOC_DESCRICAO,TLocTomador.UF_SIGLA,CargaPedido.PED_TIPO_TOMADOR,";
                        if (!groupBy.Contains("TRemetente.CLI_NOME"))
                            groupBy += "TRemetente.CLI_NOME,TRemetente.CLI_CGCCPF,TLocRemetente.LOC_DESCRICAO,TLocRemetente.UF_SIGLA,";
                        if (!groupBy.Contains("TDestinatario.CLI_NOME"))
                            groupBy += "TDestinatario.CLI_NOME,TDestinatario.CLI_CGCCPF, TLocDestinatario.LOC_DESCRICAO, TLocDestinatario.UF_SIGLA,";
                        if (!groupBy.Contains("TExpedidor.CLI_NOME"))
                            groupBy += "TExpedidor.CLI_NOME,TExpedidor.CLI_CGCCPF,TLocExpedidor.LOC_DESCRICAO, TLocExpedidor.UF_SIGLA,";
                        if (!groupBy.Contains("TRecebedor.CLI_NOME"))
                            groupBy += "TRecebedor.CLI_NOME,TRecebedor.CLI_CGCCPF,TLocRecebedor.LOC_DESCRICAO,TLocRecebedor.UF_SIGLA,";
                        break;

                    case "Expedidor":
                        select += "ISNULL(TExpedidor.CLI_NOME, '') as Expedidor,ISNULL(TExpedidor.CLI_CGCCPF, '') as ExpedidorCGC,ISNULL(TLocExpedidor.LOC_DESCRICAO  + ' - ' +  TLocExpedidor.UF_SIGLA, '') as ExpedidorLocalidade,";
                        if (!groupBy.Contains("TExpedidor.CLI_NOME"))
                            groupBy += "TExpedidor.CLI_NOME,TExpedidor.CLI_CGCCPF,TLocExpedidor.LOC_DESCRICAO, TLocExpedidor.UF_SIGLA,";
                        break;

                    case "Recebedor":
                        select += "ISNULL(TRecebedor.CLI_NOME, '') as Recebedor,ISNULL(TRecebedor.CLI_CGCCPF, '') as RecebedorCGC,ISNULL(TLocRecebedor.LOC_DESCRICAO  + ' - ' + TLocRecebedor.UF_SIGLA , '') as RecebedorLocalidade,";
                        if (!groupBy.Contains("TRecebedor.CLI_NOME"))
                            groupBy += "TRecebedor.CLI_NOME,TRecebedor.CLI_CGCCPF,TLocRecebedor.LOC_DESCRICAO,TLocRecebedor.UF_SIGLA,";
                        break;

                    case "Filial":
                        select += "TFilial.FIL_DESCRICAO as Filial,";
                        groupBy += "TFilial.FIL_DESCRICAO,";
                        break;

                    case "TipoPagamento":
                        select += "TPedido.PED_TIPO_PAGAMENTO as TipoPagamento,";
                        break;

                    case "Frota":
                        select += @"reverse(stuff(reverse(((select (case vei.VEI_NUMERO_FROTA when null then '' when '' then '' else vei.VEI_NUMERO_FROTA + ', ' end) from T_VEICULO vei where vei.VEI_CODIGO = Carga.CAR_VEICULO) + 
                                            ISNULL((SELECT (case veiculo1.VEI_NUMERO_FROTA when null then '' when '' then '' else veiculo1.VEI_NUMERO_FROTA + ', ' end) FROM T_CARGA_VEICULOS_VINCULADOS veiculoVinculadoCarga1 
                                            INNER JOIN T_VEICULO veiculo1 ON veiculoVinculadoCarga1.VEI_CODIGO = veiculo1.VEI_CODIGO WHERE veiculoVinculadoCarga1.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), ''))), 1, 2, '')) as Frota,";

                        if (!groupBy.Contains("Carga.CAR_CODIGO"))
                            groupBy += "Carga.CAR_CODIGO,";
                        if (!groupBy.Contains("Carga.CAR_VEICULO"))
                            groupBy += "Carga.CAR_VEICULO,";
                        break;

                    case "ValorICMS":
                        select += @"isNull((select sum(cte.CON_VAL_ICMS) from T_CTE cte
                                    LEFT JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CON_CODIGO = cte.CON_CODIGO 
                                    where CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO and cte.CON_STATUS = 'A'), 0) AS ValorICMS, ";

                        if (!groupBy.Contains("Carga.CAR_CODIGO"))
                            groupBy += "Carga.CAR_CODIGO,";
                        break;

                    case "ValorAReceber":
                        select += @"isNull((select sum(cte.CON_VALOR_RECEBER) from T_CTE cte
                                    LEFT JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CON_CODIGO = cte.CON_CODIGO 
                                    where CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO and cte.CON_STATUS = 'A'), 0) AS ValorAReceber, ";

                        if (!groupBy.Contains("Carga.CAR_CODIGO"))
                            groupBy += "Carga.CAR_CODIGO,";
                        break;

                    case "ValorDescarga":
                        select += @"isNull((select SUM(cargaCTeComponenteFrete.CCC_VALOR_COMPONENTE) from T_CARGA_CTE_COMPONENTES_FRETE cargaCTeComponenteFrete 
                                    inner join T_CARGA_CTE CargaCTe on CargaCTe.CCT_CODIGO = cargaCTeComponenteFrete.CCT_CODIGO 
                                    where CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO and cargaCTeComponenteFrete.CCC_TIPO_COMPONENTE_FRETE = 3), 0) AS ValorDescarga, ";

                        if (!groupBy.Contains("Carga.CAR_CODIGO"))
                            groupBy += "Carga.CAR_CODIGO,";
                        break;

                    case "ValorPedagio":
                        select += @"isNull((select SUM(cargaCTeComponenteFrete.CCC_VALOR_COMPONENTE) from T_CARGA_CTE_COMPONENTES_FRETE cargaCTeComponenteFrete 
                                    inner join T_CARGA_CTE CargaCTe on CargaCTe.CCT_CODIGO = cargaCTeComponenteFrete.CCT_CODIGO 
                                    where CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO and cargaCTeComponenteFrete.CCC_TIPO_COMPONENTE_FRETE = 2), 0) AS ValorPedagio, ";

                        if (!groupBy.Contains("Carga.CAR_CODIGO"))
                            groupBy += "Carga.CAR_CODIGO,";
                        break;

                    case "ValorAdValorem":
                        select += @"isNull((select SUM(cargaCTeComponenteFrete.CCC_VALOR_COMPONENTE) from T_CARGA_CTE_COMPONENTES_FRETE cargaCTeComponenteFrete 
                                    inner join T_CARGA_CTE CargaCTe on CargaCTe.CCT_CODIGO = cargaCTeComponenteFrete.CCT_CODIGO 
                                    where CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO and cargaCTeComponenteFrete.CCC_TIPO_COMPONENTE_FRETE = 5), 0) AS ValorAdValorem, ";

                        if (!groupBy.Contains("Carga.CAR_CODIGO"))
                            groupBy += "Carga.CAR_CODIGO,";
                        break;

                    case "ValorOutros":
                        select += @"isNull((select SUM(cargaCTeComponenteFrete.CCC_VALOR_COMPONENTE) from T_CARGA_CTE_COMPONENTES_FRETE cargaCTeComponenteFrete 
                                    inner join T_CARGA_CTE CargaCTe on CargaCTe.CCT_CODIGO = cargaCTeComponenteFrete.CCT_CODIGO 
                                    where CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO and cargaCTeComponenteFrete.CCC_TIPO_COMPONENTE_FRETE = 9), 0) AS ValorOutros, ";

                        if (!groupBy.Contains("Carga.CAR_CODIGO"))
                            groupBy += "Carga.CAR_CODIGO,";
                        break;

                    case "ValorComplementoFrete":
                        select += @"isNull((select SUM(cargaCTeComponenteFrete.CCC_VALOR_COMPONENTE) from T_CARGA_CTE_COMPONENTES_FRETE cargaCTeComponenteFrete 
                                    inner join T_CARGA_CTE CargaCTe on CargaCTe.CCT_CODIGO = cargaCTeComponenteFrete.CCT_CODIGO 
                                    where CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO and cargaCTeComponenteFrete.CCC_TIPO_COMPONENTE_FRETE = 4), 0) AS ValorComplementoFrete, ";

                        if (!groupBy.Contains("Carga.CAR_CODIGO"))
                            groupBy += "Carga.CAR_CODIGO,";
                        break;

                    case "ValorComplementoICMS":
                        select += @"isNull((select SUM(cargaCTeComponenteFrete.CCC_VALOR_COMPONENTE) from T_CARGA_CTE_COMPONENTES_FRETE cargaCTeComponenteFrete 
                                    inner join T_CARGA_CTE CargaCTe on CargaCTe.CCT_CODIGO = cargaCTeComponenteFrete.CCT_CODIGO 
                                    where CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO and cargaCTeComponenteFrete.CCC_TIPO_COMPONENTE_FRETE = 1), 0) AS ValorComplementoICMS, ";

                        if (!groupBy.Contains("Carga.CAR_CODIGO"))
                            groupBy += "Carga.CAR_CODIGO,";
                        break;

                    default:
                        if (propriedade.CodigoDinamico > 0)
                        {
                            select += $@"isNull((select SUM(cargaCTeComponenteFrete.CCC_VALOR_COMPONENTE) from T_CARGA_CTE_COMPONENTES_FRETE cargaCTeComponenteFrete 
                                    inner join T_CARGA_CTE CargaCTe on CargaCTe.CCT_CODIGO = cargaCTeComponenteFrete.CCT_CODIGO 
                                    where CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO and cargaCTeComponenteFrete.CFR_CODIGO = {propriedade.CodigoDinamico}), 0) AS {propriedade.Propriedade}, ";

                            if (!groupBy.Contains("Carga.CAR_CODIGO"))
                                groupBy += "Carga.CAR_CODIGO,";
                        }
                        break;
                }
            }
        }

        private void setarWhereBuscarRelatorioFreteComponente(ref string query, DateTime dataInicial, DateTime dataFinal, double remetente, double destinatario, double tomador, double expedidor, List<double> codigosRecebedor, List<int> codigosFilial, int empresa, List<int> codigosTipoCarga, List<int> codigosTipoOperacao)
        {
            string pattern = "yyyy-MM-dd";

            query += @" T_CARGA_PEDIDO CargaPedido
                        inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO 
                        inner join T_PEDIDO as TPedido on TPedido.PED_CODIGO = CargaPedido.PED_CODIGO 
                        inner join T_EMPRESA as TTransportador on Carga.EMP_CODIGO = TTransportador.EMP_CODIGO 
                        inner join T_LOCALIDADES as TLocTransportador on TLocTransportador.LOC_CODIGO = TTransportador.LOC_CODIGO 
                        inner join T_FILIAL as TFilial on TFilial.FIL_CODIGO = Carga.FIL_CODIGO 
                        inner join T_CLIENTE as TRemetente on TRemetente.CLI_CGCCPF = TPedido.CLI_CODIGO_REMETENTE 
                        inner join T_CLIENTE as TDestinatario on TDestinatario.CLI_CGCCPF = TPedido.CLI_CODIGO 
                        inner join T_LOCALIDADES as TLocRemetente on TLocRemetente.LOC_CODIGO = TRemetente.LOC_CODIGO 
                        inner join T_LOCALIDADES as TLocDestinatario on TLocDestinatario.LOC_CODIGO = TDestinatario.LOC_CODIGO 
                        left join T_CLIENTE as TTomador on TTomador.CLI_CGCCPF = CargaPedido.CLI_CODIGO_TOMADOR 
                        left join T_LOCALIDADES as TLocTomador on TLocTomador.LOC_CODIGO = TTomador.LOC_CODIGO 
                        left join T_CLIENTE as TExpedidor on TExpedidor.CLI_CGCCPF = TPedido.CLI_CODIGO_EXPEDIDOR 
                        left join T_LOCALIDADES as TLocExpedidor on TLocExpedidor.LOC_CODIGO = TExpedidor.LOC_CODIGO 
                        left join T_CLIENTE as TRecebedor on TRecebedor.CLI_CGCCPF = TPedido.CLI_CODIGO_RECEBEDOR 
                        left join T_LOCALIDADES as TLocRecebedor on TLocRecebedor.LOC_CODIGO = TRecebedor.LOC_CODIGO ";

            query += " where (Carga.CAR_SITUACAO = 9 or Carga.CAR_SITUACAO = 11 or Carga.CAR_SITUACAO = 10)";
            query += @"AND Carga.CAR_CODIGO IN (select cargaCTe.CAR_CODIGO from T_CTE cte
                           JOIN T_CARGA_CTE cargaCTe ON cargaCTe.CON_CODIGO = cte.CON_CODIGO 
                           where cargaCTe.CAR_CODIGO = Carga.CAR_CODIGO and cte.CON_STATUS = 'A')";

            if (dataInicial != DateTime.MinValue)
                query += " AND Carga.CAR_DATA_CRIACAO >= '" + dataInicial.ToString(pattern) + "'";

            if (dataFinal != DateTime.MinValue)
                query += " AND Carga.CAR_DATA_CRIACAO < '" + dataFinal.AddDays(1).ToString(pattern) + "'";

            if (empresa > 0)
                query += " AND Carga.EMP_CODIGO = " + empresa.ToString();

            if (codigosFilial?.Count > 0)
                query += $@" and (Carga.FIL_CODIGO in ({string.Join(", ", codigosFilial)}) OR EXISTS (   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", codigosFilial)})))";

            if (codigosTipoCarga?.Count > 0)
                query += $" AND (Carga.TCG_CODIGO in ({string.Join(",", codigosTipoCarga)}){(codigosTipoCarga.Contains(-1) ? " or Carga.TCG_CODIGO is null" : "")})";

            if (codigosTipoOperacao?.Count > 0)
                query += $" AND (Carga.TOP_CODIGO in ({string.Join(",", codigosTipoOperacao)}){(codigosTipoOperacao.Contains(-1) ? " or Carga.TOP_CODIGO is null" : "")})";

            if (remetente > 0)
                query += " AND TRemetente.CLI_CGCCPF = " + remetente;

            if (destinatario > 0)
                query += " AND TDestinatario.CLI_CGCCPF = " + destinatario;

            if (expedidor > 0)
                query += " AND TExpedidor.CLI_CGCCPF = " + destinatario;

            if (codigosRecebedor.Count > 0)
                query += $" AND TRecebedor.CLI_CGCCPF in ({string.Join(",", codigosRecebedor)})";


            if (tomador > 0)
                query += " AND TTomador.CLI_CGCCPF = " + destinatario;
        }

        #endregion

        #region Relatório de Encaixes

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Encaixe.RelatorioEncaixe>> ConsultarRelatorioEncaixe(DateTime dataInicial, DateTime dataFinal, int codigoTransportador, List<int> codigosFilial, int codigoOrigem, int codigoDestino, List<int> codigosTipoCarga, int codigoModeloVeiculo, double cpfCnpjRemetente, double cpfCnpjDestinatario, int codigoVeiculo, int codigoMotorista, int codigoGrupoPessoas, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoes, List<int> codigosTipoOperacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalPrestacao tipoLocalPrestacao, string cargaEncaixada, string pedidoEncaixado, int notaEncaixada, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioEncaixe(dataInicial, dataFinal, codigoTransportador, codigosFilial, codigoOrigem, codigoDestino, codigosTipoCarga, codigoModeloVeiculo, cpfCnpjRemetente, cpfCnpjDestinatario, codigoVeiculo, codigoMotorista, codigoGrupoPessoas, situacoes, codigosTipoOperacao, tipoLocalPrestacao, cargaEncaixada, pedidoEncaixado, notaEncaixada, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.Encaixe.RelatorioEncaixe)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Cargas.Encaixe.RelatorioEncaixe>();
        }

        public int ContarConsultaRelatorioEncaixe(DateTime dataInicial, DateTime dataFinal, int codigoTransportador, List<int> codigosFilial, int codigoOrigem, int codigoDestino, List<int> codigosTipoCarga, int codigoModeloVeiculo, double cpfCnpjRemetente, double cpfCnpjDestinatario, int codigoVeiculo, int codigoMotorista, int codigoGrupoPessoas, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoes, List<int> codigosTipoOperacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalPrestacao tipoLocalPrestacao, string cargaEncaixada, string pedidoEncaixado, int notaEncaixada, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioEncaixe(dataInicial, dataFinal, codigoTransportador, codigosFilial, codigoOrigem, codigoDestino, codigosTipoCarga, codigoModeloVeiculo, cpfCnpjRemetente, cpfCnpjDestinatario, codigoVeiculo, codigoMotorista, codigoGrupoPessoas, situacoes, codigosTipoOperacao, tipoLocalPrestacao, cargaEncaixada, pedidoEncaixado, notaEncaixada, true, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0));

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioEncaixe(DateTime dataInicial, DateTime dataFinal, int codigoTransportador, List<int> codigosFilial, int codigoOrigem, int codigoDestino, List<int> codigosTipoCarga, int codigoModeloVeiculo, double cpfCnpjRemetente, double cpfCnpjDestinatario, int codigoVeiculo, int codigoMotorista, int codigoGrupoPessoas, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoes, List<int> codigosTipoOperacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalPrestacao tipoLocalPrestacao, string cargaEncaixada, string pedidoEncaixado, int notaEncaixada, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaEncaixe(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaEncaixe(ref where, ref groupBy, ref joins, dataInicial, dataFinal, codigoTransportador, codigosFilial, codigoOrigem, codigoDestino, codigosTipoCarga, codigoModeloVeiculo, cpfCnpjRemetente, cpfCnpjDestinatario, codigoVeiculo, codigoMotorista, codigoGrupoPessoas, situacoes, codigosTipoOperacao, tipoLocalPrestacao, cargaEncaixada, pedidoEncaixado, notaEncaixada);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaEncaixe(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena) && propOrdena != "Codigo")
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }


            // SELECT
            string query = "SELECT ";

            if (count)
                query += "DISTINCT(COUNT(0) OVER())";
            else if (select.Length > 0)
                query += select.Substring(0, select.Length - 2);

            // FROM
            query += " FROM T_CARGA_PEDIDO CargaPedidoDeEncaixe ";

            // JOIN
            query += joins;

            // WHERE
            query += " WHERE 1 = 1" + where;

            // GROUP BY
            if (groupBy.Length > 0)
                query += " GROUP BY " + groupBy.Substring(0, groupBy.Length - 2);

            // ORDER BY
            if (orderBy.Length > 0)
                query += " ORDER BY " + orderBy;
            else if (!count)
                query += " ORDER BY 1 ASC";

            // LIMIT
            if (!count && limite > 0)
                query += " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY";

            return query;
        }

        private void SetarSelectRelatorioConsultaEncaixe(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select += "CargaPedidoDeEncaixe.CPE_CODIGO Codigo, ";
                        groupBy += "CargaPedidoDeEncaixe.CPE_CODIGO, ";
                    }
                    break;
                case "CargaDeEncaixe":
                    if (!select.Contains(" CargaDeEncaixe, "))
                    {
                        select += "CargaEncaixe.CAR_CODIGO_CARGA_EMBARCADOR CargaDeEncaixe, ";
                        groupBy += "CargaEncaixe.CAR_CODIGO_CARGA_EMBARCADOR, ";

                        if (!joins.Contains(" CargaEncaixe "))
                            joins += "INNER JOIN T_CARGA CargaEncaixe ON CargaPedidoDeEncaixe.CAR_CODIGO = CargaEncaixe.CAR_CODIGO ";
                    }
                    break;
                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select += "CargaEncaixeTransp.EMP_RAZAO Transportador, ";
                        groupBy += "CargaEncaixeTransp.EMP_RAZAO, ";

                        if (!joins.Contains(" CargaEncaixe "))
                            joins += "INNER JOIN T_CARGA CargaEncaixe ON CargaPedidoDeEncaixe.CAR_CODIGO = CargaEncaixe.CAR_CODIGO ";
                        if (!joins.Contains(" CargaEncaixeTransp "))
                            joins += "INNER JOIN T_EMPRESA CargaEncaixeTransp ON CargaEncaixe.EMP_CODIGO = CargaEncaixeTransp.EMP_CODIGO ";
                    }
                    break;
                case "Veiculo":
                    if (!select.Contains(" Veiculo, "))
                    {
                        select += "CargaEncaixeVeic.VEI_PLACA Veiculo, ";
                        groupBy += "CargaEncaixeVeic.VEI_PLACA, ";

                        if (!joins.Contains(" CargaEncaixe "))
                            joins += "INNER JOIN T_CARGA CargaEncaixe ON CargaPedidoDeEncaixe.CAR_CODIGO = CargaEncaixe.CAR_CODIGO ";
                        if (!joins.Contains(" CargaEncaixeVeic "))
                            joins += "INNER JOIN T_VEICULO CargaEncaixeVeic ON CargaEncaixe.CAR_VEICULO = CargaEncaixeVeic.VEI_CODIGO ";
                    }
                    break;
                case "Motoristas":
                    if (!select.Contains(" Motoristas,"))
                    {
                        select += @"SUBSTRING((
		                                SELECT ', ' + _motorista.FUN_NOME
		                                FROM T_FUNCIONARIO _motorista INNER JOIN T_CARGA_MOTORISTA _carga ON _carga.CAR_MOTORISTA = _motorista.FUN_CODIGO 
		                                WHERE _carga.CAR_CODIGO = CargaPedidoDeEncaixe.CAR_CODIGO
	                                FOR XML PATH('')), 3, 1000) Motoristas, ";
                        groupBy += " CargaPedidoDeEncaixe.CAR_CODIGO, ";
                    }
                    break;
                case "CargaEncaixada":
                    if (!select.Contains(" CargaEncaixada, "))
                    {
                        select += "CargaEncaixada.CAR_CODIGO_CARGA_EMBARCADOR CargaEncaixada, ";
                        groupBy += "CargaEncaixada.CAR_CODIGO_CARGA_EMBARCADOR, ";

                        if (!joins.Contains(" CargaPedidoEncaixada "))
                            joins += "INNER JOIN T_CARGA_PEDIDO CargaPedidoEncaixada ON CargaPedidoDeEncaixe.PED_CODIGO_PEDIDO_ENCAIXADO = CargaPedidoEncaixada.CPE_CODIGO ";
                        if (!joins.Contains(" CargaEncaixada "))
                            joins += "INNER JOIN T_CARGA CargaEncaixada ON CargaPedidoEncaixada.CAR_CODIGO = CargaEncaixada.CAR_CODIGO ";
                    }
                    break;
                case "PedidoEncaixado":
                    if (!select.Contains(" PedidoEncaixado, "))
                    {
                        select += "PedidoEncaixado.PED_NUMERO_PEDIDO_EMBARCADOR PedidoEncaixado, ";
                        groupBy += "PedidoEncaixado.PED_NUMERO_PEDIDO_EMBARCADOR, ";

                        if (!joins.Contains(" CargaPedidoEncaixada "))
                            joins += "INNER JOIN T_CARGA_PEDIDO CargaPedidoEncaixada ON CargaPedidoDeEncaixe.PED_CODIGO_PEDIDO_ENCAIXADO = CargaPedidoEncaixada.CPE_CODIGO ";
                        if (!joins.Contains(" PedidoEncaixado "))
                            joins += "INNER JOIN T_PEDIDO PedidoEncaixado ON CargaPedidoEncaixada.PED_CODIGO = PedidoEncaixado.PED_CODIGO ";
                    }
                    break;
                case "CTesDoEncaixe":
                    if (!select.Contains(" CTesDoEncaixe,"))
                    {
                        select += @"SUBSTRING((
		                                SELECT ', ' + CONVERT(NVARCHAR(50), _cte.CON_NUM) 
		                                FROM T_CARGA_PEDIDO _cargapedido
		                                LEFT JOIN T_CARGA _carga on _carga.CAR_CODIGO = _cargapedido.CAR_CODIGO
		                                LEFT JOIN T_PEDIDO_XML_NOTA_FISCAL _cargapedidoxmlnotafiscal on _cargapedidoxmlnotafiscal.CPE_CODIGO = _cargapedido.CPE_CODIGO
		                                LEFT JOIN T_XML_NOTA_FISCAL xmlnotafiscal on xmlnotafiscal.NFX_CODIGO = _cargapedidoxmlnotafiscal.NFX_CODIGO
		                                LEFT JOIN T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE _cargapedidoxmlnotafiscalcte on _cargapedidoxmlnotafiscalcte.PNF_CODIGO = _cargapedidoxmlnotafiscal.PNF_CODIGO
		                                LEFT JOIN T_CARGA_CTE _cargacte on _cargacte.CCT_CODIGO = _cargapedidoxmlnotafiscalcte.CCT_CODIGO
		                                LEFT JOIN T_CTE _cte on _cte.CON_CODIGO = _cargacte.CON_CODIGO
		                                WHERE _cargapedido.PED_CODIGO = PedidoEncaixe.PED_CODIGO
		                                GROUP BY _cte.CON_NUM
	                                FOR XML PATH('')), 3, 1000) CTesDoEncaixe, ";
                        groupBy += " PedidoEncaixe.PED_CODIGO, ";

                        if (!joins.Contains(" PedidoEncaixe "))
                            joins += "INNER JOIN T_PEDIDO PedidoEncaixe ON CargaPedidoDeEncaixe.PED_CODIGO = PedidoEncaixe.PED_CODIGO ";
                    }
                    break;
                case "CTesEncaixados":
                    if (!select.Contains(" CTesEncaixados,"))
                    {
                        select += @"SUBSTRING((
		                                SELECT ', ' + CONVERT(NVARCHAR(50), _cte.CON_NUM) 
		                                FROM T_CARGA_PEDIDO _cargapedido
		                                LEFT JOIN T_CARGA _carga on _carga.CAR_CODIGO = _cargapedido.CAR_CODIGO
		                                LEFT JOIN T_PEDIDO_XML_NOTA_FISCAL _cargapedidoxmlnotafiscal on _cargapedidoxmlnotafiscal.CPE_CODIGO = _cargapedido.CPE_CODIGO
		                                LEFT JOIN T_XML_NOTA_FISCAL xmlnotafiscal on xmlnotafiscal.NFX_CODIGO = _cargapedidoxmlnotafiscal.NFX_CODIGO
		                                LEFT JOIN T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE _cargapedidoxmlnotafiscalcte on _cargapedidoxmlnotafiscalcte.PNF_CODIGO = _cargapedidoxmlnotafiscal.PNF_CODIGO
		                                LEFT JOIN T_CARGA_CTE _cargacte on _cargacte.CCT_CODIGO = _cargapedidoxmlnotafiscalcte.CCT_CODIGO
		                                LEFT JOIN T_CTE _cte on _cte.CON_CODIGO = _cargacte.CON_CODIGO
		                                WHERE _cargapedido.PED_CODIGO = PedidoEncaixado.PED_CODIGO
		                                GROUP BY _cte.CON_NUM
	                                FOR XML PATH('')), 3, 1000) CTesEncaixados, ";
                        groupBy += " PedidoEncaixado.PED_CODIGO, ";

                        if (!joins.Contains(" CargaPedidoEncaixada "))
                            joins += "INNER JOIN T_CARGA_PEDIDO CargaPedidoEncaixada ON CargaPedidoDeEncaixe.PED_CODIGO_PEDIDO_ENCAIXADO = CargaPedidoEncaixada.CPE_CODIGO ";
                        if (!joins.Contains(" PedidoEncaixado "))
                            joins += "INNER JOIN T_PEDIDO PedidoEncaixado ON CargaPedidoEncaixada.PED_CODIGO = PedidoEncaixado.PED_CODIGO ";
                    }
                    break;
                case "NotasEncaixadas":
                    if (!select.Contains(" NotasEncaixadas,"))
                    {
                        select += @"SUBSTRING((
		                                SELECT ', ' + CONVERT(NVARCHAR(50), _xmlnotafiscal.NF_NUMERO) 
		                                FROM T_PEDIDO_XML_NOTA_FISCAL _cargapedidoxmlnotafiscal INNER JOIN T_XML_NOTA_FISCAL _xmlnotafiscal ON _xmlnotafiscal.NFX_CODIGO = _cargapedidoxmlnotafiscal.NFX_CODIGO 
		                                WHERE _cargapedidoxmlnotafiscal.CPE_CODIGO = CargaPedidoEncaixada.CPE_CODIGO FOR XML PATH('')), 3, 1000
	                                ) NotasEncaixadas, ";
                        groupBy += "CargaPedidoEncaixada.CPE_CODIGO, ";

                        if (!joins.Contains(" CargaPedidoEncaixada "))
                            joins += "INNER JOIN T_CARGA_PEDIDO CargaPedidoEncaixada ON CargaPedidoDeEncaixe.PED_CODIGO_PEDIDO_ENCAIXADO = CargaPedidoEncaixada.CPE_CODIGO ";
                    }
                    break;
                case "CodigoClienteEncaixado":
                    if (!select.Contains(" CodigoClienteEncaixado, "))
                    {
                        select += "ClienteEncaixado.CLI_CODIGO_INTEGRACAO CodigoClienteEncaixado, ";
                        groupBy += "ClienteEncaixado.CLI_CODIGO_INTEGRACAO, ";

                        if (!joins.Contains(" CargaPedidoEncaixada "))
                            joins += "INNER JOIN T_CARGA_PEDIDO CargaPedidoEncaixada ON CargaPedidoDeEncaixe.PED_CODIGO_PEDIDO_ENCAIXADO = CargaPedidoEncaixada.CPE_CODIGO ";
                        if (!joins.Contains(" PedidoEncaixado "))
                            joins += "INNER JOIN T_PEDIDO PedidoEncaixado ON CargaPedidoEncaixada.PED_CODIGO = PedidoEncaixado.PED_CODIGO ";
                        if (!joins.Contains(" ClienteEncaixado "))
                            joins += "INNER JOIN T_CLIENTE ClienteEncaixado ON ClienteEncaixado.CLI_CGCCPF = PedidoEncaixado.CLI_CODIGO ";
                    }
                    break;
                case "CNPJClienteEncaixado":
                    if (!select.Contains(" CNPJClienteEncaixado, "))
                    {
                        select += "CAST(CAST(ClienteEncaixado.CLI_CGCCPF AS BIGINT) AS VARCHAR) CNPJClienteEncaixado, ";
                        select += "ClienteEncaixado.CLI_FISJUR TipoCliente, ";
                        groupBy += "ClienteEncaixado.CLI_CGCCPF, ";
                        groupBy += "ClienteEncaixado.CLI_FISJUR, ";

                        if (!joins.Contains(" CargaPedidoEncaixada "))
                            joins += "INNER JOIN T_CARGA_PEDIDO CargaPedidoEncaixada ON CargaPedidoDeEncaixe.PED_CODIGO_PEDIDO_ENCAIXADO = CargaPedidoEncaixada.CPE_CODIGO ";
                        if (!joins.Contains(" PedidoEncaixado "))
                            joins += "INNER JOIN T_PEDIDO PedidoEncaixado ON CargaPedidoEncaixada.PED_CODIGO = PedidoEncaixado.PED_CODIGO ";
                        if (!joins.Contains(" ClienteEncaixado "))
                            joins += "INNER JOIN T_CLIENTE ClienteEncaixado ON ClienteEncaixado.CLI_CGCCPF = PedidoEncaixado.CLI_CODIGO ";
                    }
                    break;
                case "ClienteEncaixado":
                    if (!select.Contains(" ClienteEncaixado, "))
                    {
                        select += "ClienteEncaixado.CLI_NOME ClienteEncaixado, ";
                        groupBy += "ClienteEncaixado.CLI_NOME, ";

                        if (!joins.Contains(" CargaPedidoEncaixada "))
                            joins += "INNER JOIN T_CARGA_PEDIDO CargaPedidoEncaixada ON CargaPedidoDeEncaixe.PED_CODIGO_PEDIDO_ENCAIXADO = CargaPedidoEncaixada.CPE_CODIGO ";
                        if (!joins.Contains(" PedidoEncaixado "))
                            joins += "INNER JOIN T_PEDIDO PedidoEncaixado ON CargaPedidoEncaixada.PED_CODIGO = PedidoEncaixado.PED_CODIGO ";
                        if (!joins.Contains(" ClienteEncaixado "))
                            joins += "INNER JOIN T_CLIENTE ClienteEncaixado ON ClienteEncaixado.CLI_CGCCPF = PedidoEncaixado.CLI_CODIGO ";
                    }
                    break;
                case "LocalidadeClienteEncaixado":
                    if (!select.Contains(" LocalidadeClienteEncaixado, "))
                    {
                        select += "LocalidadeClienteEncaixado.LOC_DESCRICAO + ' - ' + LocalidadeClienteEncaixado.UF_SIGLA LocalidadeClienteEncaixado, ";
                        groupBy += "LocalidadeClienteEncaixado.LOC_DESCRICAO, LocalidadeClienteEncaixado.UF_SIGLA, ";

                        if (!joins.Contains(" CargaPedidoEncaixada "))
                            joins += "INNER JOIN T_CARGA_PEDIDO CargaPedidoEncaixada ON CargaPedidoDeEncaixe.PED_CODIGO_PEDIDO_ENCAIXADO = CargaPedidoEncaixada.CPE_CODIGO ";
                        if (!joins.Contains(" PedidoEncaixado "))
                            joins += "INNER JOIN T_PEDIDO PedidoEncaixado ON CargaPedidoEncaixada.PED_CODIGO = PedidoEncaixado.PED_CODIGO ";
                        if (!joins.Contains(" ClienteEncaixado "))
                            joins += "INNER JOIN T_CLIENTE ClienteEncaixado ON ClienteEncaixado.CLI_CGCCPF = PedidoEncaixado.CLI_CODIGO ";
                        if (!joins.Contains(" LocalidadeClienteEncaixado "))
                            joins += "INNER JOIN T_LOCALIDADES LocalidadeClienteEncaixado ON LocalidadeClienteEncaixado.LOC_CODIGO = ClienteEncaixado.LOC_CODIGO ";
                    }
                    break;
                case "ValorPrestacaoEncaixe":
                    if (!select.Contains(" ValorPrestacaoEncaixe, "))
                    {
                        select += "CargaPedidoDeEncaixe.PED_VALOR_FRETE_PAGAR ValorPrestacaoEncaixe, ";
                        groupBy += "CargaPedidoDeEncaixe.PED_VALOR_FRETE_PAGAR, ";
                    }
                    break;
                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaEncaixe(ref string where, ref string groupBy, ref string joins, DateTime dataInicial, DateTime dataFinal, int codigoTransportador, List<int> codigosFilial, int codigoOrigem, int codigoDestino, List<int> codigosTipoCarga, int codigoModeloVeiculo, double cpfCnpjRemetente, double cpfCnpjDestinatario, int codigoVeiculo, int codigoMotorista, int codigoGrupoPessoas, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoes, List<int> codigosTipoOperacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalPrestacao tipoLocalPrestacao, string cargaEncaixada, string pedidoEncaixado, int notaEncaixada)
        {
            where += " AND CargaPedidoDeEncaixe.PED_CODIGO_PEDIDO_ENCAIXADO IS NOT NULL";

            if (dataInicial != DateTime.MinValue || dataFinal != DateTime.MinValue)
            {
                string pattern = "yyyy-MM-dd";

                if (!joins.Contains(" CargaEncaixe "))
                    joins += "INNER JOIN T_CARGA CargaEncaixe ON CargaPedidoDeEncaixe.CAR_CODIGO = CargaEncaixe.CAR_CODIGO ";

                if (dataInicial != DateTime.MinValue)
                    where += " AND CargaEncaixe.CAR_DATA_CRIACAO > '" + dataInicial.ToString(pattern) + "'";
                if (dataFinal != DateTime.MinValue)
                    where += " AND CargaEncaixe.CAR_DATA_CRIACAO < '" + dataFinal.AddDays(1).ToString(pattern) + "'";
            }

            if (codigoTransportador > 0)
            {
                if (!joins.Contains(" CargaEncaixe "))
                    joins += "INNER JOIN T_CARGA CargaEncaixe ON CargaPedidoDeEncaixe.CAR_CODIGO = CargaEncaixe.CAR_CODIGO ";
                if (!joins.Contains(" CargaEncaixeTransp "))
                    joins += "INNER JOIN T_EMPRESA CargaEncaixeTransp ON CargaEncaixe.EMP_CODIGO = CargaEncaixeTransp.EMP_CODIGO ";

                where += " AND CargaEncaixeTransp.EMP_CODIGO = " + codigoTransportador.ToString();
            }

            if (codigosFilial?.Count > 0)
            {
                if (!joins.Contains(" CargaEncaixe "))
                    joins += "INNER JOIN T_CARGA CargaEncaixe ON CargaPedidoDeEncaixe.CAR_CODIGO = CargaEncaixe.CAR_CODIGO ";

                where += $" AND CargaEncaixe.FIL_CODIGO in ({string.Join(", ", codigosFilial)})";
            }

            if (codigoOrigem > 0)
            {
                where += " AND CargaPedidoDeEncaixe.LOC_CODIGO_ORIGEM = " + codigoOrigem.ToString();
            }

            if (codigoDestino > 0)
            {
                where += " AND CargaPedidoDeEncaixe.LOC_CODIGO_DESTINO = " + codigoDestino.ToString();
            }

            if (codigosTipoCarga?.Count > 0)
            {
                if (!joins.Contains(" CargaEncaixe "))
                    joins += "INNER JOIN T_CARGA CargaEncaixe ON CargaPedidoDeEncaixe.CAR_CODIGO = CargaEncaixe.CAR_CODIGO ";

                where += $" AND (CargaEncaixe.TCG_CODIGO in ({string.Join(", ", codigosTipoCarga)}){(codigosTipoCarga.Contains(-1) ? " or CargaEncaixe.TCG_CODIGO is null" : "")})";
            }

            if (codigoModeloVeiculo > 0)
            {
                if (!joins.Contains(" CargaEncaixe "))
                    joins += "INNER JOIN T_CARGA CargaEncaixe ON CargaPedidoDeEncaixe.CAR_CODIGO = CargaEncaixe.CAR_CODIGO ";

                where += " AND CargaEncaixe.MVC_CODIGO = " + codigoModeloVeiculo.ToString();
            }

            if (cpfCnpjRemetente > 0)
            {
                if (!joins.Contains(" PedidoEncaixe "))
                    joins += "INNER JOIN T_PEDIDO PedidoEncaixe ON CargaPedidoDeEncaixe.PED_CODIGO = PedidoEncaixe.PED_CODIGO ";

                where += " AND PedidoEncaixe.CLI_CODIGO_REMETENTE = " + cpfCnpjRemetente.ToString();
            }

            if (cpfCnpjDestinatario > 0)
            {
                if (!joins.Contains(" PedidoEncaixe "))
                    joins += "INNER JOIN T_PEDIDO PedidoEncaixe ON CargaPedidoDeEncaixe.PED_CODIGO = PedidoEncaixe.PED_CODIGO ";

                where += " AND PedidoEncaixe.CLI_CODIGO = " + cpfCnpjDestinatario.ToString();
            }

            if (codigoVeiculo > 0)
            {
                if (!joins.Contains(" CargaEncaixe "))
                    joins += "INNER JOIN T_CARGA CargaEncaixe ON CargaPedidoDeEncaixe.CAR_CODIGO = CargaEncaixe.CAR_CODIGO ";

                where += " AND CargaEncaixe.CAR_VEICULO = " + codigoVeiculo.ToString();
            }

            if (codigoMotorista > 0)
            {
                if (!joins.Contains(" CargaEncaixe "))
                    joins += "INNER JOIN T_CARGA CargaEncaixe ON CargaPedidoDeEncaixe.CAR_CODIGO = CargaEncaixe.CAR_CODIGO ";

                where += " AND CargaEncaixe.CAR_CODIGO IN(" + @"
                                SELECT _cargamotorista.CAR_CODIGO
                                FROM T_CARGA_MOTORISTA _cargamotorista
                                INNER JOIN T_FUNCIONARIO _motoristas ON _cargamotorista.CAR_MOTORISTA = _motoristas.FUN_CODIGO 
                                WHERE _motoristas.FUN_CODIGO = " + codigoMotorista.ToString()
                        + ")";
            }

            if (codigoGrupoPessoas > 0)
            {
                if (!joins.Contains(" CargaEncaixe "))
                    joins += "INNER JOIN T_CARGA CargaEncaixe ON CargaPedidoDeEncaixe.CAR_CODIGO = CargaEncaixe.CAR_CODIGO ";

                where += " AND CargaEncaixe.GRP_CODIGO = " + codigoGrupoPessoas.ToString();
            }

            if (situacoes.Count() > 0)
            {
                string inSituacoes = String.Join(", ", (from o in situacoes select (int)o).ToArray());

                if (!joins.Contains(" CargaEncaixe "))
                    joins += "INNER JOIN T_CARGA CargaEncaixe ON CargaPedidoDeEncaixe.CAR_CODIGO = CargaEncaixe.CAR_CODIGO ";

                where += " AND CargaEncaixe.CAR_SITUACAO IN (" + inSituacoes + ")";
            }

            if (codigosTipoOperacao?.Count > 0)
            {
                if (!joins.Contains(" CargaEncaixe "))
                    joins += "INNER JOIN T_CARGA CargaEncaixe ON CargaPedidoDeEncaixe.CAR_CODIGO = CargaEncaixe.CAR_CODIGO ";

                where += $" AND (CargaEncaixe.TOP_CODIGO in ({string.Join(", ", codigosTipoOperacao)}){(codigosTipoOperacao.Contains(-1) ? " or CargaEncaixe.TOP_CODIGO is null" : "")})";
            }

            if (tipoLocalPrestacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalPrestacao.todos)
            {
                if (!joins.Contains(" CargaPedidoEncaixada "))
                    joins += "INNER JOIN T_CARGA_PEDIDO CargaPedidoEncaixada ON CargaPedidoDeEncaixe.PED_CODIGO_PEDIDO_ENCAIXADO = CargaPedidoEncaixada.CPE_CODIGO ";
                if (!joins.Contains(" PedidoEncaixado "))
                    joins += "INNER JOIN T_PEDIDO PedidoEncaixado ON CargaPedidoEncaixada.PED_CODIGO = PedidoEncaixado.PED_CODIGO ";

                if (tipoLocalPrestacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalPrestacao.intraMunicipal)
                    where += " AND PedidoEncaixado.LOC_CODIGO_ORIGEM = PedidoEncaixado.LOC_CODIGO_DESTINO";
                else if (tipoLocalPrestacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalPrestacao.interMunicipal)
                    where += " AND PedidoEncaixado.LOC_CODIGO_ORIGEM <> PedidoEncaixado.LOC_CODIGO_DESTINO";
            }

            if (!string.IsNullOrWhiteSpace(cargaEncaixada))
            {
                if (!joins.Contains(" CargaPedidoEncaixada "))
                    joins += "INNER JOIN T_CARGA_PEDIDO CargaPedidoEncaixada ON CargaPedidoDeEncaixe.PED_CODIGO_PEDIDO_ENCAIXADO = CargaPedidoEncaixada.CPE_CODIGO ";
                if (!joins.Contains(" CargaEncaixada "))
                    joins += "INNER JOIN T_CARGA CargaEncaixada ON CargaPedidoEncaixada.CAR_CODIGO = CargaEncaixada.CAR_CODIGO ";

                where += " AND CargaEncaixada.CAR_CODIGO_CARGA_EMBARCADOR = '" + cargaEncaixada + "'";
            }

            if (!string.IsNullOrWhiteSpace(pedidoEncaixado))
            {
                if (!joins.Contains(" CargaPedidoEncaixada "))
                    joins += "INNER JOIN T_CARGA_PEDIDO CargaPedidoEncaixada ON CargaPedidoDeEncaixe.PED_CODIGO_PEDIDO_ENCAIXADO = CargaPedidoEncaixada.CPE_CODIGO ";
                if (!joins.Contains(" PedidoEncaixado "))
                    joins += "INNER JOIN T_PEDIDO PedidoEncaixado ON CargaPedidoEncaixada.PED_CODIGO = PedidoEncaixado.PED_CODIGO ";

                where += " AND PedidoEncaixado.PED_NUMERO_PEDIDO_EMBARCADOR LIKE '%" + pedidoEncaixado + "%'";
            }

            if (notaEncaixada > 0)
            {
                if (!joins.Contains(" CargaPedidoEncaixada "))
                    joins += "INNER JOIN T_CARGA_PEDIDO CargaPedidoEncaixada ON CargaPedidoDeEncaixe.PED_CODIGO_PEDIDO_ENCAIXADO = CargaPedidoEncaixada.CPE_CODIGO ";

                where += " AND CargaPedidoEncaixada.CPE_CODIGO IN(" + @"
                                SELECT _cargapedidoxmlnotafiscal.CPE_CODIGO
		                        FROM T_PEDIDO_XML_NOTA_FISCAL _cargapedidoxmlnotafiscal INNER JOIN T_XML_NOTA_FISCAL _xmlnotafiscal ON _xmlnotafiscal.NFX_CODIGO = _cargapedidoxmlnotafiscal.NFX_CODIGO 
		                        WHERE _xmlnotafiscal.NF_NUMERO = " + notaEncaixada.ToString()
                        + ")";
            }
        }

        #endregion

        #region Relatorio Agendamento Entrega Pedido

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.AgendamentoEntregaPedido> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioAgendamentoEntrega filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = new ConsultaAgendamentoEntregaPedido().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.AgendamentoEntregaPedido)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.AgendamentoEntregaPedido>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioAgendamentoEntrega filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consulta = new ConsultaAgendamentoEntregaPedido().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoValoresDeFrete> BuscaCargaPedidoValoresDeFrete(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            return query.Where(x => x.Carga.Codigo == codigoCarga).Select(x => new Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoValoresDeFrete
            {
                CodigoCargaPedido = x.Codigo,
                CodigoPedido = x.Pedido.Codigo,
                NomeRemetente = x.Pedido.Remetente.Nome ?? "",
                NomeDestinatario = x.Pedido.Destinatario.Nome ?? "",
                NumeroPedido = x.Pedido.Numero.ToString(), //cargaPedido.Pedido.CodigoPedidoCliente
                ValorFrete = x.ValorFrete,
                ValorFreteFilialEmissora = x.ValorFreteFilialEmissora,
                ValorFreteAntesDaAlteracaoManual = x.ValorFreteAntesAlteracaoManual == null ? 0.0m : x.ValorFreteAntesAlteracaoManual,
                ValorFreteFilialEmissoraAntesDaAlteracaoManual = x.ValorFreteFilialEmissoraAntesAlteracaoManual == null ? 0.0m : x.ValorFreteFilialEmissoraAntesAlteracaoManual,
                ValorFreteDatabase = x.ValorFrete,
                ValorFreteFilialEmissoraDatabase = x.ValorFreteFilialEmissora,
            }).ToList();
        }

        public async Task<List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoValoresDeFrete>> BuscaCargaPedidoValoresDeFreteAsync(int codigoCarga, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            return await query.Where(x => x.Carga.Codigo == codigoCarga).Select(x => new Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoValoresDeFrete
            {
                CodigoCargaPedido = x.Codigo,
                CodigoPedido = x.Pedido.Codigo,
                NomeRemetente = x.Pedido.Remetente.Nome ?? "",
                NomeDestinatario = x.Pedido.Destinatario.Nome ?? "",
                NumeroPedido = x.Pedido.Numero.ToString(),
                ValorFrete = x.ValorFrete,
                ValorFreteFilialEmissora = x.ValorFreteFilialEmissora,
                ValorFreteAntesDaAlteracaoManual = x.ValorFreteAntesAlteracaoManual == null ? 0.0m : x.ValorFreteAntesAlteracaoManual,
                ValorFreteFilialEmissoraAntesDaAlteracaoManual = x.ValorFreteFilialEmissoraAntesAlteracaoManual == null ? 0.0m : x.ValorFreteFilialEmissoraAntesAlteracaoManual,
                ValorFreteDatabase = x.ValorFrete,
                ValorFreteFilialEmissoraDatabase = x.ValorFreteFilialEmissora,
            }).ToListAsync(cancellationToken);
        }

        #endregion
    }
}
