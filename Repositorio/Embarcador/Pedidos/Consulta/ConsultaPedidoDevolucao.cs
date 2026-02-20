using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Pedido;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Pedidos
{
    sealed class ConsultaPedidoDevolucao : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoDevolucao>
    {
        #region Construtores
        public ConsultaPedidoDevolucao() : base(tabela: "T_CHAMADOS Chamados") { }

        #endregion

        #region Métodos Privados

		private void SetarJoinsCargaEntrega(StringBuilder joins)
        {
			if (!joins.Contains(" CargaEntrega "))
				joins.Append(" LEFT JOIN T_CARGA_ENTREGA CargaEntrega ON CargaEntrega.CEN_CODIGO = Chamados.CEN_CODIGO ");
        }

		private void SetarJoinsCargaEntregaNotaFiscal(StringBuilder joins)
        {
			SetarJoinsCargaEntrega(joins);

			if (!joins.Contains(" CargaEntregaNotaFiscal "))
				joins.Append(" LEFT JOIN T_CARGA_ENTREGA_NOTA_FISCAL CargaEntregaNotaFiscal ON CargaEntregaNotaFiscal.CEN_CODIGO = CargaEntrega.CEN_CODIGO ");
        }

		private void SetarJoinsPedidoNotaFiscal(StringBuilder joins)
        {
			SetarJoinsCargaEntregaNotaFiscal(joins);

			if (!joins.Contains(" PedidoNotaFiscal "))
				joins.Append(" LEFT JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFiscal ON PedidoNotaFiscal.PNF_CODIGO = CargaEntregaNotaFiscal.PNF_CODIGO ");
        }

		private void SetarJoinsNotaFiscal(StringBuilder joins)
        {
			SetarJoinsPedidoNotaFiscal(joins);

			if (!joins.Contains(" NotaFiscal "))
				joins.Append(" LEFT JOIN T_XML_NOTA_FISCAL NotaFiscal ON NotaFiscal.NFX_CODIGO = PedidoNotaFiscal.NFX_CODIGO ");
        }

		private void SetarJoinsCargaPedido(StringBuilder joins)
        {
			SetarJoinsPedidoNotaFiscal(joins);

			if (!joins.Contains(" CargaPedido "))
				joins.Append(" LEFT JOIN T_CARGA_PEDIDO CargaPedido ON CargaPedido.CPE_CODIGO = PedidoNotaFiscal.CPE_CODIGO ");
        }

		private void SetarJoinsPedido(StringBuilder joins)
        {
			SetarJoinsCargaPedido(joins);

			if (!joins.Contains(" Pedido "))
				joins.Append(" LEFT JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
        }

		private void SetarJoinsPedidoAdicional(StringBuilder joins)
        {
			SetarJoinsPedido(joins);

			if (!joins.Contains(" PedidoAdicional "))
				joins.Append(" LEFT JOIN T_PEDIDO_ADICIONAL PedidoAdicional ON PedidoAdicional.PED_CODIGO = Pedido.PED_CODIGO ");
        }

		private void SetarJoinsCarga(StringBuilder joins)
        {
			if (!joins.Contains(" Carga "))
				joins.Append(" LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamados.CAR_CODIGO ");
        }

		private void SetarJoinsDestinatario(StringBuilder joins)
        {
			SetarJoinsPedido(joins);

			if (!joins.Contains(" Destinatario "))
				joins.Append(" LEFT JOIN T_CLIENTE Destinatario ON Destinatario.CLI_CGCCPF = Pedido.CLI_CODIGO ");
        }

		private void SetarJoinsDestino(StringBuilder joins)
        {
			SetarJoinsPedido(joins);

			if (!joins.Contains(" Destino "))
				joins.Append(" LEFT JOIN T_LOCALIDADES Destino ON Destino.LOC_CODIGO = Pedido.LOC_CODIGO_DESTINO ");
        }

		private void SetarJoinsTransportador(StringBuilder joins)
        {
			SetarJoinsCarga(joins);

			if (!joins.Contains(" Transportador "))
				joins.Append(" LEFT JOIN T_EMPRESA Transportador ON Transportador.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

		private void SetarJoinsMotivo(StringBuilder joins)
        {
			SetarJoinsCargaEntrega(joins);

			if (!joins.Contains(" Motivo "))
				joins.Append(" LEFT JOIN T_OCORRENCIA Motivo ON Motivo.OCO_CODIGO = CargaEntrega.OCO_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, FiltroPesquisaRelatorioPedidoDevolucao filtrosPesquisa)
        {
            switch (propriedade)
            {
				case "NumeroNotaFiscal":
					if (!select.Contains(" NumeroNotaFiscal, "))
                    {
						select.Append("NotaFiscal.NF_NUMERO NumeroNotaFiscal, ");
						groupBy.Append("NotaFiscal.NF_NUMERO, ");

						SetarJoinsNotaFiscal(joins);
                    }
					break;

				case "DataEmissaoNotaFiscalFormatada":
					if (!select.Contains(" DataEmissaoNotaFiscal, "))
                    {
						select.Append("NotaFiscal.NF_DATA_EMISSAO DataEmissaoNotaFiscal, ");
						groupBy.Append("NotaFiscal.NF_DATA_EMISSAO, ");

						SetarJoinsNotaFiscal(joins);
					}
					break;

				case "ValorTotalNotaFiscal":
					if (!select.Contains(" ValorTotalNotaFiscal, "))
                    {
						select.Append("NotaFiscal.NF_VALOR ValorTotalNotaFiscal, ");
						groupBy.Append("NotaFiscal.NF_VALOR, ");

						SetarJoinsNotaFiscal(joins);
                    }
					break;

				case "NomeDestinatario":
					if (!select.Contains(" NomeDestinatario, "))
                    {
						select.Append("Destinatario.CLI_NOME NomeDestinatario, ");
						groupBy.Append("Destinatario.CLI_NOME, ");

						SetarJoinsDestinatario(joins);
                    }
					break;

				case "CNPJDestinatario":
					if (!select.Contains(" CNPJDestinatario, "))
                    {
						select.Append("Destinatario.CLI_CGCCPF CNPJDestinatario, ");
						groupBy.Append("Destinatario.CLI_CGCCPF, ");

						SetarJoinsDestinatario(joins);
                    }
					break;

				case "Destinatario":
					SetarSelect("NomeDestinatario", 0, select, joins, groupBy, false, filtrosPesquisa);
					SetarSelect("CNPJDestinatario", 0, select, joins, groupBy, false, filtrosPesquisa);
					break;

				case "CidadeDestino":
					if (!select.Contains(" CidadeDestino, "))
                    {
						select.Append("Destino.LOC_DESCRICAO CidadeDestino, ");
						groupBy.Append("Destino.LOC_DESCRICAO, ");

						SetarJoinsDestino(joins);
                    }
					break;

				case "UFDestino":
					if (!select.Contains(" UFDestino, "))
                    {
						select.Append("Destino.UF_SIGLA UFDestino, ");
						groupBy.Append("Destino.UF_SIGLA, ");

						SetarJoinsDestino(joins);
                    }
					break;

				case "Destino":
					SetarSelect("CidadeDestino", 0, select, joins, groupBy, false, filtrosPesquisa);
					SetarSelect("UFDestino", 0, select, joins, groupBy, false, filtrosPesquisa);
					break;

				case "RazaoTransportador":
					if (!select.Contains(" RazaoTransportador, "))
                    {
						select.Append("Transportador.EMP_RAZAO RazaoTransportador, ");
						groupBy.Append("Transportador.EMP_RAZAO, ");

						SetarJoinsTransportador(joins);
                    }
					break;

				case "CNPJTransportador":
					if (!select.Contains(" CNPJTransportador, "))
                    {
						select.Append("CAST(Transportador.EMP_CNPJ AS VARCHAR(20)) CNPJTransportador, ");
						groupBy.Append("Transportador.EMP_CNPJ, ");

						SetarJoinsTransportador(joins);
                    }
					break;

				case "Transportador":
					SetarSelect("RazaoTransportador", 0, select, joins, groupBy, false, filtrosPesquisa);
					SetarSelect("CNPJTransportador", 0, select, joins, groupBy, false, filtrosPesquisa);
					break;

				case "TipoDevolucao":
					if (!select.Contains(" TipoDevolucao, "))
                    {
						select.Append(@"CASE ");
						select.Append(@"	WHEN CargaEntrega.CEN_DEVOLUCAO_PARCIAL = 1 THEN 'Devolução Parcial' ");
						select.Append(@"	ELSE 'Devolução Total' ");
						select.Append(@"END TipoDevolucao, ");

						groupBy.Append("CargaEntrega.CEN_DEVOLUCAO_PARCIAL, ");

						SetarJoinsCargaEntrega(joins);
					}
					break;

				case "Motivo":
					if (!select.Contains(" Motivo, "))
                    {
						select.Append("Motivo.OCO_DESCRICAO Motivo, ");
						groupBy.Append("Motivo.OCO_DESCRICAO, ");

						SetarJoinsMotivo(joins);
                    }
					break;

				case "ISISReturn":
					if (!select.Contains(" ISISReturn, "))
                    {
						select.Append(" COALESCE(PedidoAdicional.PAD_ISIS_RETURN, ");
						select.Append("		SUBSTRING(( ");
						select.Append("			SELECT ', ' + ");
						select.Append("				CAST(PedidoAdicional.PAD_ISIS_RETURN AS NVARCHAR) ");
						select.Append("			FROM T_PEDIDO PedidoDevolucao ");
						select.Append("				INNER JOIN T_CARGA_PEDIDO CargaPedidoDevolucao ON CargaPedidoDevolucao.PED_CODIGO = PedidoDevolucao.PED_CODIGO ");		
						select.Append("				INNER JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoDevolucaoNotaFiscal ON PedidoDevolucaoNotaFiscal.CPE_CODIGO = CargaPedidoDevolucao.CPE_CODIGO ");
						select.Append("				INNER JOIN T_XML_NOTA_FISCAL NotaFiscalDevolucao ON NotaFiscalDevolucao.NFX_CODIGO = PedidoDevolucaoNotaFiscal.NFX_CODIGO ");
						select.Append("				INNER JOIN T_PEDIDO_ADICIONAL PedidoAdicional ON PedidoAdicional.PED_CODIGO = PedidoDevolucao.PED_CODIGO ");
						select.Append("			WHERE PedidoDevolucao.PED_NUMERO_PEDIDO_EMBARCADOR = Pedido.PED_NUMERO_PEDIDO_EMBARCADOR ");
						select.Append("				AND PedidoDevolucao.PED_CODIGO <> Pedido.PED_CODIGO  ");
						select.Append("				AND PedidoDevolucao.PED_SITUACAO <> 2 ");
						select.Append("				AND NotaFiscalDevolucao.NF_SITUACAO_ENTREGA IN(3,4) ");
						select.Append("				AND PedidoDevolucao.TOP_CODIGO IN (SELECT DISTINCT TOP_CODIGO_PEDIDO_DEVOLUCAO FROM T_OCORRENCIA WHERE TOP_CODIGO_PEDIDO_DEVOLUCAO IS NOT NULL) ");
						select.Append("			FOR XML PATH('')), ");
						select.Append("		3, 1000), ");
						select.Append("	'') ISISReturn, ");

						if (!groupBy.Contains("PedidoAdicional.PAD_ISIS_RETURN"))
							groupBy.Append("PedidoAdicional.PAD_ISIS_RETURN, ");

						if (!groupBy.Contains("Pedido.PED_CODIGO"))
							groupBy.Append("Pedido.PED_CODIGO, ");

						if (!groupBy.Contains("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR"))
							groupBy.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR, ");

						SetarJoinsPedidoAdicional(joins);
                    }
					break;

				case "DataEntregaFormatada":
					if (!select.Contains(" DataEntrega, "))
                    {
						select.Append("Pedido.PED_DATA_ENTREGA DataEntrega, ");
						groupBy.Append("Pedido.PED_DATA_ENTREGA, ");

						SetarJoinsPedido(joins);
                    }
					break;

				case "QuantidadeVolumes":
					if (!select.Contains(" QuantidadeVolumes, "))
                    {
						select.Append("NotaFiscal.NF_VOLUMES QuantidadeVolumes, ");
						groupBy.Append("NotaFiscal.NF_VOLUMES, ");

						SetarJoinsNotaFiscal(joins);
                    }
					break;

				case "NFD":
					break;

				case "ValorNFD":
					break;

			}
        }

        protected override void SetarWhere(FiltroPesquisaRelatorioPedidoDevolucao filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
			where.Append("AND ((CargaEntrega.CEN_DEVOLUCAO_PARCIAL = 1 AND NotaFiscal.NF_SITUACAO_ENTREGA in(3, 4)) OR (CargaEntrega.CEN_DEVOLUCAO_PARCIAL = 0 OR CargaEntrega.CEN_DEVOLUCAO_PARCIAL IS NULL)) ");

			if (filtrosPesquisa.CodigoPedido > 0)
            {
				where.Append($" AND CargaPedido.PED_CODIGO = {filtrosPesquisa.CodigoPedido} ");
				SetarJoinsCargaPedido(joins);
            }

			if (filtrosPesquisa.CodigoCarga > 0)
				where.Append($" AND Chamados.CAR_CODIGO = {filtrosPesquisa.CodigoCarga} ");

			if (filtrosPesquisa.TipoDevolucao == TipoColetaEntregaDevolucao.Parcial)
            {
				where.Append(" AND CargaEntrega.CEN_DEVOLUCAO_PARCIAL = 1 ");
				SetarJoinsCargaEntrega(joins);
            }
			else if (filtrosPesquisa.TipoDevolucao == TipoColetaEntregaDevolucao.Total)
            {
				where.Append(" AND (CargaEntrega.CEN_DEVOLUCAO_PARCIAL = 0 OR CargaEntrega.CEN_DEVOLUCAO_PARCIAL IS NULL) ");
				SetarJoinsCargaEntrega(joins);
            }

			if (filtrosPesquisa.NumeroNF > 0)
            {
				where.Append($" AND NotaFiscal.NF_NUMERO = {filtrosPesquisa.NumeroNF} ");
				SetarJoinsNotaFiscal(joins);
            }

			if (filtrosPesquisa.DataEmissaoNFInicial != DateTime.MinValue)
            {
				where.Append($" AND NotaFiscal.NF_DATA_EMISSAO >= '{filtrosPesquisa.DataEmissaoNFInicial.ToString("yyyy-MM-dd")}' ");
				SetarJoinsNotaFiscal(joins);
            }

			if (filtrosPesquisa.DataEmissaoNFFinal != DateTime.MinValue)
			{
				where.Append($" AND NotaFiscal.NF_DATA_EMISSAO < '{filtrosPesquisa.DataEmissaoNFFinal.AddDays(1).ToString("yyyy-MM-dd")}' ");
				SetarJoinsNotaFiscal(joins);
			}

			if (filtrosPesquisa.CodigoTransportador > 0)
            {
				where.Append($" AND Carga.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador} ");
				SetarJoinsCarga(joins);
            }

			if (filtrosPesquisa.CodigoCliente > 0)
            {
				where.Append($" AND Destinatario.CLI_CGCCPF = {filtrosPesquisa.CodigoCliente} ");
				SetarJoinsDestinatario(joins);
            }

			if (filtrosPesquisa.CodigoMotivo > 0)
            {
				where.Append($" AND CargaEntrega.OCO_CODIGO = {filtrosPesquisa.CodigoMotivo} ");
				SetarJoinsCargaEntrega(joins);
            }
		}

        #endregion
    }
}
