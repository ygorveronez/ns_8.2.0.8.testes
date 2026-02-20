using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.TorreControle
{
    public class ConsultaPorNotaFiscal : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>
    {
        public ConsultaPorNotaFiscal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public IList<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaPorNotaFiscal> Consultar(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaConsultaPorNotaFiscal filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            StringBuilder queryString = new StringBuilder(ObterCamposConsulta())
                                            .Append(ObterFromConsulta())
                                            .Append(ObterWhereConsulta(filtrosPesquisa))
                                            .Append(ObterGroupByConsulta())
                                            .Append($" ORDER BY {parametroConsulta.PropriedadeOrdenar} {parametroConsulta.DirecaoOrdenar}");

            queryString.Append($" OFFSET {parametroConsulta.InicioRegistros} ROWS");

            if (parametroConsulta.LimiteRegistros > 0)
                queryString.Append($" FETCH NEXT {parametroConsulta.LimiteRegistros} ROWS ONLY");

            var consulta = this.SessionNHiBernate.CreateSQLQuery(queryString.ToString())
                                                 .SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaPorNotaFiscal)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaPorNotaFiscal>();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaConsultaPorNotaFiscal filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            StringBuilder queryString = new StringBuilder("SELECT DISTINCT(COUNT(0) OVER ())   ")
                                      .Append(ObterFromConsulta())
                                      .Append(ObterWhereConsulta(filtrosPesquisa))
                                      .Append(ObterGroupByConsulta());

            var consulta = this.SessionNHiBernate.CreateSQLQuery(queryString.ToString());

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        #endregion

        #region Métodos Privados

        private string ObterCamposConsulta()
        {
            return @"SELECT NotaFiscal.NFX_CODIGO AS Codigo,
							NotaFiscal.NF_NUMERO AS Numero,
							Carga.CAR_CODIGO_CARGA_EMBARCADOR AS Carga,
						    (SELECT TOP 1 _notaFiscalSituacao.NFS_DESCRICAO
						 		  FROM T_NOTA_FISCAL_SITUACAO _notaFiscalSituacao
						 		 WHERE NotaFiscal.NFS_CODIGO = _notaFiscalSituacao.NFS_CODIGO
						    ) AS SituacaoNotaFiscal,
						    TipoOperacao.TOP_DESCRICAO AS TipoOperacao,
						    Pedido.PED_SITUACAO_AGENDAMENTO_ENTREGA_PEDIDO AS SituacaoAgendamento,
						    Pedido.PED_OBSERVACAO_SOLICITACAO_REAGENDAMENTO AS ObservacaoReagendamento,
						    Destinatario.CLI_NOME AS Cliente,
						    Destinatario.CLI_CGCCPF AS ClienteCnpj,
						    Destino.LOC_DESCRICAO AS Destino,
						    Transportador.EMP_FANTASIA AS Transportador,
						    Transportador.EMP_CNPJ AS TransportadorCnpj,
						    Carga.CAR_DATA_SUGESTAO_ENTREGA AS SugestaoDataEntrega,
						    (
								CASE WHEN Pedido.PED_DATA_AGENDAMENTO <> Pedido.PED_DATA_PRIMEIRO_AGENDAMENTO 
									 THEN Pedido.PED_DATA_AGENDAMENTO
								ELSE NULL
								END
							) AS DataReagendamento,
							Pedido.PED_DATA_PRIMEIRO_AGENDAMENTO AS DataAgendamento,
						    Carga.CAR_DATA_INICIO_VIAGEM AS DataTerminoCarregamento,
							CargaEntrega.CEN_DATA_INICIO_ENTREGA AS DataInicioEntrega,
							CargaEntrega.CEN_DATA_ENTREGA AS DataFimEntrega,
						    ViewTorreControle.STATUS AS SituacaoViagem,
						    NotaFiscal.NF_SITUACAO_ENTREGA AS SituacaoEntregaNotaFiscal,
						 	SUBSTRING(
						 				(
						 					SELECT ', ' + _contatoCliente.ANX_DESCRICAO
						 					  FROM T_AGENDAMENTO_ENTREGA_PEDIDO_CLIENTE_ANEXOS AS _contatoCliente
						 					 WHERE _contatoCliente.PED_CODIGO = Pedido.PED_CODIGO
						 					   FOR XML PATH('')
						 			     ), 3, 1000
						 			  ) AS ContatoCliente,
						 	SUBSTRING(
						 				(
						 					SELECT ', ' + _contatoTransportador.ANX_DESCRICAO
						 					  FROM T_AGENDAMENTO_ENTREGA_PEDIDO_TRANSPORTADOR_ANEXOS AS _contatoTransportador
						 					 WHERE _contatoTransportador.PED_CODIGO = Pedido.PED_CODIGO
						 					   FOR XML PATH('')
						 			     ), 3, 1000
						 			  ) AS ContatoTransportador,
						 	SUBSTRING(
						 				(
						 					SELECT ', ' + _tipoOcorrencia.OCO_DESCRICAO
						 					  FROM T_OCORRENCIA AS _tipoOcorrencia 
						 					  JOIN T_PEDIDO_OCORRENCIA_COLETA_ENTREGA AS _ocorrencia
						 						ON _tipoOcorrencia.OCO_CODIGO = _ocorrencia.OCO_CODIGO
						 					 WHERE _ocorrencia.PED_CODIGO = Pedido.PED_CODIGO
						 					   FOR XML PATH('')
						 			     ), 3, 1000
						 			  ) AS Ocorrencia,
						 	(
						 		CASE WHEN Pedido.PED_DATA_AGENDAMENTO IS NULL OR CargaEntrega.CEN_DATA_INICIO_ENTREGA IS NULL AND MotivoReagendamento.MRE_CONSIDERAR_ON_TIME IS NULL
						 			 THEN 0 
						 			 ELSE
						 		 CASE 
									WHEN Pedido.PED_DATA_AGENDAMENTO >= CargaEntrega.CEN_DATA_INICIO_ENTREGA 
						 			THEN 1
                                    WHEN CargaEntrega.CEN_DATA_INICIO_ENTREGA > Pedido.PED_DATA_AGENDAMENTO 
						 			THEN 2
                                    WHEN Pedido.PED_DATA_AGENDAMENTO IS NULL
                                    THEN 1
									WHEN Pedido.PED_DATA_AGENDAMENTO >= CargaEntrega.CEN_DATA_INICIO_ENTREGA AND MotivoReagendamento.MRE_CONSIDERAR_ON_TIME = 1
                                    THEN 1
                                    WHEN CargaEntrega.CEN_DATA_INICIO_ENTREGA > Pedido.PED_DATA_AGENDAMENTO AND MotivoReagendamento.MRE_CONSIDERAR_ON_TIME = 1
						 		    THEN 2
						 		END END
						 	) AS SituacaoEntrega
					";
        }

        private string ObterFromConsulta()
        {
			return @"FROM T_XML_NOTA_FISCAL AS NotaFiscal
					 JOIN T_PEDIDO_XML_NOTA_FISCAL AS PedidoNotaFiscal ON NotaFiscal.NFX_CODIGO = PedidoNotaFiscal.NFX_CODIGO
					 JOIN T_CARGA_PEDIDO AS CargaPedido ON CargaPedido.CPE_CODIGO = PedidoNotaFiscal.CPE_CODIGO
					 JOIN T_CARGA AS Carga ON Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO
					 LEFT JOIN T_CARGA_JANELA_CARREGAMENTO AS CargaJanelaCarregamento ON Carga.CAR_CODIGO = CargaJanelaCarregamento.CAR_CODIGO
					 LEFT JOIN T_TIPO_OPERACAO AS TipoOperacao ON TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO
					 JOIN T_PEDIDO AS Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
					 LEFT JOIN T_CLIENTE AS Destinatario ON Destinatario.CLI_CGCCPF = NotaFiscal.CLI_CODIGO_DESTINATARIO
					 LEFT JOIN T_LOCALIDADES AS Destino ON Destino.LOC_CODIGO = Destinatario.LOC_CODIGO
					 LEFT JOIN T_EMPRESA AS Transportador ON Transportador.EMP_CODIGO = Carga.EMP_CODIGO
					 LEFT JOIN T_CARGA_ENTREGA_PEDIDO CargaEntregaPedido ON CargaEntregaPedido.CEP_CODIGO = (select top 1 CEP_CODIGO from T_CARGA_ENTREGA_PEDIDO where CPE_CODIGO = CargaPedido.CPE_CODIGO order by CEP_CODIGO desc)
					 LEFT JOIN T_CARGA_ENTREGA CargaEntrega ON CargaEntrega.CEN_CODIGO = CargaEntregaPedido.CEN_CODIGO
					 LEFT JOIN V_TORRE_CONTROLE_CARGAS ViewTorreControle ON ViewTorreControle.CEN_CODIGO = CargaEntrega.CEN_CODIGO
					 LEFT JOIN T_MOTIVO_REAGENDAMENTO AS MotivoReagendamento ON MotivoReagendamento.MRE_CODIGO = Pedido.MRE_CODIGO";
        }

        private string ObterGroupByConsulta()
        {
            return @"GROUP BY NotaFiscal.NFX_CODIGO,
							  NotaFiscal.NF_NUMERO,
							  Carga.CAR_CODIGO_CARGA_EMBARCADOR,
						      NotaFiscal.NFS_CODIGO,
							  Carga.CAR_CODIGO,
						      TipoOperacao.TOP_DESCRICAO,
						      Pedido.PED_SITUACAO_AGENDAMENTO_ENTREGA_PEDIDO,
							  Pedido.PED_OBSERVACAO_SOLICITACAO_REAGENDAMENTO,
						      Destinatario.CLI_NOME,
						      Destinatario.CLI_CGCCPF,
						      Destino.LOC_DESCRICAO,
						      Transportador.EMP_FANTASIA,
						      Transportador.EMP_CNPJ,
						      Carga.CAR_DATA_SUGESTAO_ENTREGA,
							  Pedido.PED_DATA_AGENDAMENTO,
							  Pedido.PED_DATA_PRIMEIRO_AGENDAMENTO,
							  Pedido.PED_DATA_PRIMEIRO_AGENDAMENTO ,
						      Carga.CAR_DATA_INICIO_VIAGEM,
							  Pedido.PED_CODIGO,
							  ViewTorreControle.STATUS,
						      NotaFiscal.NF_SITUACAO_ENTREGA,
							  CargaEntrega.CEN_DATA_ENTREGA,
							  CargaEntrega.CEN_DATA_INICIO_ENTREGA,
							MotivoReagendamento.MRE_CONSIDERAR_ON_TIME  ";
        }

        private string ObterWhereConsulta(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaConsultaPorNotaFiscal filtrosPesquisa)
        {
            string pattern = "yyyy-MM-dd";

            StringBuilder where = new StringBuilder(" WHERE NotaFiscal.NF_ATIVA = 1 AND Carga.CAR_SITUACAO NOT IN (13, 18) AND Pedido.PED_SITUACAO IN (1, 3) ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                where.Append($" AND Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroCarga}' ");

            if (filtrosPesquisa.Cliente > 0)
                where.Append($" AND Destinatario.CLI_CGCCPF = {filtrosPesquisa.Cliente} ");

            if (filtrosPesquisa.NumeroNota > 0)
                where.Append($" AND NotaFiscal.NF_NUMERO = {filtrosPesquisa.NumeroNota} ");

            if (filtrosPesquisa.SituacaoAgendamento.HasValue)
                where.Append($" AND Pedido.PED_SITUACAO_AGENDAMENTO_ENTREGA_PEDIDO = {(int)filtrosPesquisa.SituacaoAgendamento.Value} ");

            if (filtrosPesquisa.TipoOperacao > 0)
                where.Append($" AND Carga.TOP_CODIGO = {filtrosPesquisa.TipoOperacao} ");

            if (filtrosPesquisa.Transportador > 0)
                where.Append($" AND Carga.EMP_CODIGO = {filtrosPesquisa.Transportador} ");

            if (filtrosPesquisa.DataAgendamentoInicial.HasValue)
                where.Append($" AND Pedido.PED_DATA_AGENDAMENTO >= '{filtrosPesquisa.DataAgendamentoInicial.Value.ToString(pattern)} 00:00:00' ");

            if (filtrosPesquisa.DataAgendamentoFinal.HasValue)
                where.Append($" AND Pedido.PED_DATA_AGENDAMENTO <= '{filtrosPesquisa.DataAgendamentoFinal.Value.ToString(pattern)} 23:59:59' ");

            if (filtrosPesquisa.DataCarregamentoInicial.HasValue)
                where.Append($" AND CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO >= '{filtrosPesquisa.DataCarregamentoInicial.Value.ToString(pattern)} 00:00:00' ");

            if (filtrosPesquisa.DataCarregamentoFinal.HasValue)
                where.Append($" AND CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO <= '{filtrosPesquisa.DataCarregamentoFinal.Value.ToString(pattern)} 23:59:59' ");

            if (filtrosPesquisa.DataPrevisaoEntregaInicial.HasValue)
                where.Append($" AND CargaEntrega.CEN_DATA_ENTREGA_PREVISTA >= '{filtrosPesquisa.DataPrevisaoEntregaInicial.Value.ToString(pattern)} 00:00:00' ");

            if (filtrosPesquisa.DataPrevisaoEntregaFinal.HasValue)
                where.Append($" AND CargaEntrega.CEN_DATA_ENTREGA_PREVISTA <= '{filtrosPesquisa.DataPrevisaoEntregaFinal.Value.ToString(pattern)} 23:59:59' ");

            return where.ToString();
        }

        #endregion

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.TorreControle.RelatorioConsultaPorNotaFiscal> ConsultarRelatorioConsultaPorNotaFiscal(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsultaPorNotaFiscal filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
			var query = new Consulta.ConsultaPorNotaFiscal().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.TorreControle.RelatorioConsultaPorNotaFiscal)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.TorreControle.RelatorioConsultaPorNotaFiscal>();
        }

        public int ContarConsultaRelatorioConsultaPorNotaFiscal(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsultaPorNotaFiscal filtrosPesquisa, List<PropriedadeAgrupamento> propriedades)
        {
            var query = new Consulta.ConsultaPorNotaFiscal().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}
