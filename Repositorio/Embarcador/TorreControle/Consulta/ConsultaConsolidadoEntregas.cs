using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.TorreControle.Consulta
{
    sealed class ConsultaConsolidadoEntregas : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsolidadoEntregas>
    {
        #region Construtores

        public ConsultaConsolidadoEntregas() : base(tabela: "T_CARGA_ENTREGA as cargaEntrega") { }

        #endregion

        #region Métodos Privados
        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" carga "))
                joins.Append(" inner join T_CARGA carga on carga.CAR_CODIGO = cargaEntrega.CAR_CODIGO ");
        }

        private void SetarJoinsCargaEntregaProduto(StringBuilder joins)
        {
            if (!joins.Contains(" cargaEntregaProduto "))
                joins.Append(" left join T_CARGA_ENTREGA_PRODUTO cargaEntregaProduto on cargaEntregaProduto.CEN_CODIGO = cargaEntrega.CEN_CODIGO ");
        }

        private void SetarJoinsCargaEntregaPedido(StringBuilder joins)
        {
            if (!joins.Contains(" cargaEntregaPedido "))
                joins.Append(" join T_CARGA_ENTREGA_PEDIDO cargaEntregaPedido on cargaEntregaPedido.CEN_CODIGO = cargaEntrega.CEN_CODIGO ");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            SetarJoinsCargaEntregaPedido(joins);

            if (!joins.Contains(" cargaPedido "))
                joins.Append(" join T_CARGA_PEDIDO cargaPedido on cargaPedido.CPE_CODIGO = cargaEntregaPedido.CPE_CODIGO ");
        }

        private void SetarJoinsPedido(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" pedido "))
                joins.Append(" join T_PEDIDO pedido on pedido.PED_CODIGO = cargaPedido.PED_CODIGO ");
        }

        private void SetarJoinsCanalVenda(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" canalvenda "))
                joins.Append(" left join T_CANAL_VENDA canalvenda on canalvenda.CNV_CODIGO = pedido.CNV_CODIGO ");
        }

        private void SetarJoinsCanalEntrega(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" canalentrega "))
                joins.Append(" left join T_CANAL_ENTREGA canalentrega on canalentrega.CNE_CODIGO = pedido.CNE_CODIGO ");
        }

        private void SetarJoinsPedidoXMLNotaFiscal(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" pedidoXMLNotaFiscal "))
                joins.Append(" left join T_PEDIDO_XML_NOTA_FISCAL pedidoXMLNotaFiscal on pedidoXMLNotaFiscal.CPE_CODIGO = cargaPedido.CPE_CODIGO ");
        }

        private void SetarJoinsXMLNotaFiscal(StringBuilder joins)
        {
            SetarJoinsPedidoXMLNotaFiscal(joins);

            if (!joins.Contains(" XMLNotaFiscal "))
                joins.Append(" left join T_XML_NOTA_FISCAL XMLNotaFiscal on XMLNotaFiscal.NFX_CODIGO = pedidoXMLNotaFiscal.NFX_CODIGO ");
        }

        private void SetarJoinsCanhotoNotaFiscal(StringBuilder joins)
        {
            SetarJoinsXMLNotaFiscal(joins);

            if (!joins.Contains(" canhotoNotaFiscal "))
                joins.Append(" left join T_CANHOTO_NOTA_FISCAL canhotoNotaFiscal on canhotoNotaFiscal.NFX_CODIGO = XMLNotaFiscal.NFX_CODIGO and canhotoNotaFiscal.CNF_SITUACAO_DIGITALIZACAO_CANHOTO in (2,3) ");
        }

        private void SetarJoinsUsuarioDigitalizacaoCanhoto(StringBuilder joins)
        {
            SetarJoinsCanhotoNotaFiscal(joins);

            if (!joins.Contains(" usuarioDigitalaizacaoCanhoto "))
                joins.Append(" left join T_FUNCIONARIO usuarioDigitalaizacaoCanhoto on usuarioDigitalaizacaoCanhoto.FUN_CODIGO = canhotoNotaFiscal.UDC_CODIGO ");
        }

        private void SetarJoinsPosicao(StringBuilder joins)
        {
            SetarJoinsMonitoramento(joins);

            if (!joins.Contains(" posicao "))
                joins.Append(" left join T_POSICAO posicao on posicao.POS_CODIGO = monitoramento.POS_ULTIMA_POSICAO ");
        }

        private void SetarJoinsPosicaoAtual(StringBuilder joins)
        {
            SetarJoinsPosicao(joins);

            if (!joins.Contains(" posicaoAtual "))
                joins.Append(" left join T_POSICAO_ATUAL posicaoAtual on posicaoAtual.POS_CODIGO = posicao.POS_CODIGO ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" transportador "))
                joins.Append(" left join t_empresa transportador on transportador.emp_codigo = carga.emp_codigo ");
        }

        private void SetarJoinsMonitoramento(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" monitoramento "))
                joins.Append(" left join T_MONITORAMENTO monitoramento on monitoramento.CAR_CODIGO = carga.CAR_CODIGO ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" filial "))
                joins.Append(" left join T_FILIAL filial on filial.FIL_CODIGO = carga.FIL_CODIGO ");
        }
        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" tipoOperacao "))
                joins.Append(" left join T_TIPO_OPERACAO tipoOperacao on tipoOperacao.TOP_CODIGO = carga.TOP_CODIGO ");
        }

        private void SetarJoinsModeloVeicular(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" modeloVeicular "))
                joins.Append(" left join T_MODELO_VEICULAR_CARGA modeloVeicular on modeloVeicular.MVC_CODIGO = carga.MVC_CODIGO ");
        }

        private void SetarJoinsProduto(StringBuilder joins)
        {
            SetarJoinsCargaEntregaProduto(joins);

            if (!joins.Contains(" produto "))
                joins.Append(" left join T_PRODUTO_EMBARCADOR produto on produto.PRO_CODIGO = cargaEntregaProduto.PRO_CODIGO ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" veiculo "))
                joins.Append(" left join T_VEICULO veiculo on veiculo.VEI_CODIGO = carga.CAR_VEICULO ");
        }

        private void SetarJoinsExpedidor(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" expedidor "))
                joins.Append(" left join T_CLIENTE expedidor on cargaPedido.CLI_CODIGO_EXPEDIDOR = expedidor.CLI_CGCCPF ");
        }

        private void SetarJoinsRecebedor(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" recebedor "))
                joins.Append(" left join T_CLIENTE recebedor on cargaPedido.CLI_CODIGO_RECEBEDOR = recebedor.CLI_CGCCPF ");
        }

        private void SetarJoinsRemetentePedido(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" remetentePedido "))
                joins.Append(" left join T_CLIENTE remetentePedido on pedido.CLI_CODIGO_REMETENTE = remetentePedido.CLI_CGCCPF ");
        }

        private void SetarJoinsDestinatarioPedido(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" destinatarioPedido "))
                joins.Append(" left join T_CLIENTE destinatarioPedido on pedido.CLI_CODIGO = destinatarioPedido.CLI_CGCCPF ");
        }

        private void SetarJoinsLocalidadeRemetentePedido(StringBuilder joins)
        {
            SetarJoinsRemetentePedido(joins);

            if (!joins.Contains(" localidadeRemetentePedido "))
                joins.Append(" left join T_LOCALIDADES localidadeRemetentePedido on localidadeRemetentePedido.LOC_CODIGO = remetentePedido.LOC_CODIGO ");
        }

        private void SetarJoinsLocalidadeDestinatarioPedido(StringBuilder joins)
        {
            SetarJoinsDestinatarioPedido(joins);

            if (!joins.Contains(" localidadeDestinatarioPedido "))
                joins.Append(" left join T_LOCALIDADES localidadeDestinatarioPedido on localidadeDestinatarioPedido.LOC_CODIGO = destinatarioPedido.LOC_CODIGO ");
        }

        private void SetarJoinsLocalidadeExpedidor(StringBuilder joins)
        {
            SetarJoinsExpedidor(joins);

            if (!joins.Contains(" localidadeExpedidor "))
                joins.Append(" left join T_LOCALIDADES localidadeExpedidor on localidadeExpedidor.LOC_CODIGO = expedidor.LOC_CODIGO ");
        }

        private void SetarJoinsLocalidadeRecebedor(StringBuilder joins)
        {
            SetarJoinsRecebedor(joins);

            if (!joins.Contains(" localidadeRecebedor "))
                joins.Append(" left join T_LOCALIDADES localidadeRecebedor on localidadeRecebedor.LOC_CODIGO = recebedor.LOC_CODIGO ");
        }

        private void SetarJoinsOcorrenciaColetaEntrega(StringBuilder joins)
        {

            if (!joins.Contains(" Ocorrencia "))
                joins.Append(" left join T_OCORRENCIA Ocorrencia on Ocorrencia.OCO_CODIGO = cargaEntrega.OCO_CODIGO ");
        }

        #endregion

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsolidadoEntregas filtroPesquisa)
        {

            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo,"))
                    {
                        select.Append("cargaEntrega.CEN_CODIGO as Codigo, ");

                        if (!groupBy.Contains("cargaEntrega.CEN_CODIGO"))
                            groupBy.Append("cargaEntrega.CEN_CODIGO, ");
                    }
                    break;

                case "CodigoCargaEmbarcador":
                    if (!select.Contains(" CodigoCargaEmbarcador,"))
                    {
                        select.Append("carga.CAR_CODIGO_CARGA_EMBARCADOR as CodigoCargaEmbarcador, ");
                        groupBy.Append("carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "NumeroPedido":
                    if (!select.Contains(" NumeroPedido,"))
                    {
                        select.Append("pedido.PED_NUMERO_PEDIDO_EMBARCADOR NumeroPedido, ");

                        if (!groupBy.Contains("pedido.PED_NUMERO_PEDIDO_EMBARCADOR"))
                            groupBy.Append("pedido.PED_NUMERO_PEDIDO_EMBARCADOR, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "EscritorioVenda":
                    if (!select.Contains(" EscritorioVenda,"))
                    {
                        select.Append("pedido.PED_ESCRITORIO_VENDA EscritorioVenda, ");

                        if (!groupBy.Contains("pedido.PED_ESCRITORIO_VENDA"))
                            groupBy.Append("pedido.PED_ESCRITORIO_VENDA, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "EquipeVendas":
                    if (!select.Contains(" EquipeVendas,"))
                    {
                        select.Append("pedido.PED_EQUIPE_VENDAS EquipeVendas, ");

                        if (!groupBy.Contains("pedido.PED_EQUIPE_VENDAS"))
                            groupBy.Append("pedido.PED_EQUIPE_VENDAS, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "TipoMercado":
                    if (!select.Contains(" TipoMercado,"))
                    {
                        select.Append("pedido.PED_TIPO_MERCADORIA TipoMercado, ");

                        if (!groupBy.Contains("pedido.PED_TIPO_MERCADORIA"))
                            groupBy.Append("pedido.PED_TIPO_MERCADORIA, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "CanalEntrega":
                    if (!select.Contains(" CanalEntrega,"))
                    {
                        select.Append("canalentrega.CNE_DESCRICAO CanalEntrega, ");

                        if (!groupBy.Contains("canalentrega.CNE_DESCRICAO"))
                            groupBy.Append("canalentrega.CNE_DESCRICAO, ");

                        SetarJoinsCanalEntrega(joins);
                    }
                    break;


                case "CanalVenda":
                    if (!select.Contains(" CanalVenda,"))
                    {
                        select.Append("canalvenda.CNV_DESCRICAO CanalVenda, ");

                        if (!groupBy.Contains("canalvenda.CNV_DESCRICAO"))
                            groupBy.Append("canalvenda.CNV_DESCRICAO, ");

                        SetarJoinsCanalVenda(joins);
                    }
                    break;

                case "CargaCriticaFormatado":
                    if (!select.Contains(" CargaCritica,"))
                    {
                        select.Append("CAST(carga.CAR_CARGA_CRITICA AS INT) AS CargaCritica, ");

                        if (!groupBy.Contains("carga.CAR_CARGA_CRITICA"))
                            groupBy.Append("carga.CAR_CARGA_CRITICA, ");

                        SetarJoinsCarga(joins);
                    }
                    break;


                case "NumeroNotaFiscal":
                    if (!select.Contains(" NumeroNotaFiscal,"))
                    {
                        select.Append("XMLNotaFiscal.NF_NUMERO NumeroNotaFiscal, ");

                        if (!groupBy.Contains("XMLNotaFiscal.NF_NUMERO"))
                            groupBy.Append("XMLNotaFiscal.NF_NUMERO, ");

                        SetarJoinsXMLNotaFiscal(joins);
                    }
                    break;

                case "PesoTotalPedido":
                    if (!select.Contains(" PesoTotalPedido,"))
                    {
                        select.Append("cargaPedido.PED_PESO PesoTotalPedido, ");

                        if (!groupBy.Contains("cargaPedido.PED_PESO"))
                            groupBy.Append("cargaPedido.PED_PESO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "DataCriacaoCarga":
                    if (!select.Contains(" DataCriacaoCarga,"))
                    {
                        select.Append("FORMAT(carga.CAR_DATA_CRIACAO, 'dd/MM/yyyy HH:mm') DataCriacaoCarga, ");

                        if (!groupBy.Contains("carga.CAR_DATA_CRIACAO"))
                            groupBy.Append("carga.CAR_DATA_CRIACAO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DataInicioViagemPrevista":
                    if (!select.Contains(" DataInicioViagemPrevista,"))
                    {
                        select.Append("FORMAT(carga.CAR_DATA_INICIO_VIAGEM_PREVISTA, 'dd/MM/yyyy HH:mm') DataInicioViagemPrevista, ");

                        if (!groupBy.Contains("carga.CAR_DATA_INICIO_VIAGEM_PREVISTA"))
                            groupBy.Append("carga.CAR_DATA_INICIO_VIAGEM_PREVISTA, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DataInicioViagemRealizada":
                    if (!select.Contains(" DataInicioViagemRealizada,"))
                    {
                        select.Append("FORMAT(carga.CAR_DATA_INICIO_VIAGEM, 'dd/MM/yyyy HH:mm') DataInicioViagemRealizada, ");

                        if (!groupBy.Contains("carga.CAR_DATA_INICIO_VIAGEM,"))
                            groupBy.Append("carga.CAR_DATA_INICIO_VIAGEM, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DataInicioViagem":
                    if (!select.Contains(" DataInicioViagem,"))
                    {
                        select.Append("FORMAT(carga.CAR_DATA_INICIO_VIAGEM, 'dd/MM/yyyy HH:mm') DataInicioViagem, ");

                        if (!groupBy.Contains("carga.CAR_DATA_INICIO_VIAGEM,"))
                            groupBy.Append("carga.CAR_DATA_INICIO_VIAGEM, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DataPrevisaoEntrega":
                    if (!select.Contains(" DataPrevisaoEntrega,"))
                    {
                        select.Append("FORMAT(DATEADD(MINUTE, ISNULL(cargaEntrega.CEN_TEMPO_EXTRA_ENTREGA, 0), cargaEntrega.CEN_DATA_ENTREGA_PREVISTA), 'dd/MM/yyyy HH:mm') DataPrevisaoEntrega, ");

                        if (!groupBy.Contains("cargaEntrega.CEN_DATA_ENTREGA_PREVISTA"))
                            groupBy.Append("cargaEntrega.CEN_DATA_ENTREGA_PREVISTA, ");

                        if (!groupBy.Contains("cargaEntrega.CEN_TEMPO_EXTRA_ENTREGA"))
                            groupBy.Append("cargaEntrega.CEN_TEMPO_EXTRA_ENTREGA, ");
                    }
                    break;

                case "DataEntregaReprogramada":
                    if (!select.Contains(" DataEntregaReprogramada,"))
                    {
                        select.Append("FORMAT(cargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA, 'dd/MM/yyyy HH:mm') DataEntregaReprogramada, ");

                        if (!groupBy.Contains("cargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA"))
                            groupBy.Append("cargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA, ");
                    }
                    break;

                case "DataEntradaRaio":
                    if (!select.Contains(" DataEntradaRaio,"))
                    {
                        select.Append("FORMAT(cargaEntrega.CEN_DATA_ENTRADA_RAIO, 'dd/MM/yyyy HH:mm') DataEntradaRaio, ");

                        if (!groupBy.Contains("cargaEntrega.CEN_DATA_ENTRADA_RAIO"))
                            groupBy.Append("cargaEntrega.CEN_DATA_ENTRADA_RAIO, ");
                    }
                    break;

                case "DataSaidaRaio":
                    if (!select.Contains(" DataSaidaRaio,"))
                    {
                        select.Append("FORMAT(cargaEntrega.CEN_DATA_SAIDA_RAIO, 'dd/MM/yyyy HH:mm') DataSaidaRaio, ");

                        if (!groupBy.Contains("cargaEntrega.CEN_DATA_SAIDA_RAIO"))
                            groupBy.Append("cargaEntrega.CEN_DATA_SAIDA_RAIO, ");
                    }
                    break;

                case "DataConfirmacao":
                    if (!select.Contains(" DataConfirmacao,"))
                    {
                        select.Append("FORMAT(cargaEntrega.CEN_DATA_ENTREGA, 'dd/MM/yyyy HH:mm') DataConfirmacao, ");

                        if (!groupBy.Contains("cargaEntrega.CEN_DATA_ENTREGA,"))
                            groupBy.Append("cargaEntrega.CEN_DATA_ENTREGA, ");
                    }
                    break;

                case "DataPrevisaoEntregaAjustada":
                    if (!select.Contains(" DataPrevisaoEntregaAjustada,"))
                    {
                        select.Append("FORMAT(cargaEntrega.CEN_DATA_PREVISAO_ENTREGA_AJUSTADA, 'dd/MM/yyyy HH:mm') DataPrevisaoEntregaAjustada, ");

                        if (!groupBy.Contains("cargaEntrega.CEN_DATA_PREVISAO_ENTREGA_AJUSTADA,"))
                            groupBy.Append("cargaEntrega.CEN_DATA_PREVISAO_ENTREGA_AJUSTADA, ");
                    }
                    break;

                case "StatusPrazoEntrega":
                case "StatusPrazoEntregaFormatado":
                    if (!select.Contains(" StatusPrazoEntrega,"))
                    {
                        select.Append(@"  case 
	                                        when cargaEntrega.CEN_DATA_ENTREGA < cargaEntrega.CEN_DATA_ENTREGA_PREVISTA 
		                                        then 1
	                                        when cargaEntrega.CEN_DATA_ENTREGA > cargaEntrega.CEN_DATA_ENTREGA_PREVISTA
		                                        then 2
	                                        else 0
                                          end StatusPrazoEntrega, ");

                        if (!groupBy.Contains("cargaEntrega.CEN_STATUS_PRAZO_ENTREGA,"))
                            groupBy.Append("cargaEntrega.CEN_STATUS_PRAZO_ENTREGA, ");

                        if (!groupBy.Contains("cargaEntrega.CEN_DATA_ENTREGA,"))
                            groupBy.Append("cargaEntrega.CEN_DATA_ENTREGA, ");

                        if (!groupBy.Contains("cargaEntrega.CEN_DATA_ENTREGA_PREVISTA,"))
                            groupBy.Append("cargaEntrega.CEN_DATA_ENTREGA_PREVISTA, ");
                    }
                    break;

                case "TempoAtraso":
                    if (!select.Contains(" TempoAtraso,"))
                    {
                        select.Append("DATEPART(hour, cargaEntrega.CEN_DATA_ENTREGA_PREVISTA - cargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA) TempoAtraso, ");

                        if (!groupBy.Contains("cargaEntrega.CEN_DATA_ENTREGA_PREVISTA"))
                            groupBy.Append("cargaEntrega.CEN_DATA_ENTREGA_PREVISTA, ");

                        if (!groupBy.Contains("cargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA"))
                            groupBy.Append("cargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA, ");
                    }
                    break;

                case "MotivoUltimaOcorrencia":
                    if (!select.Contains(" MotivoUltimaOcorrencia,"))
                    {
                        select.Append(@"(
                                            select 
                                              top 1 m.MCH_DESCRICAO 
                                            from 
                                              T_CHAMADOS c
											  join T_MOTIVO_CHAMADA m on c.MCH_CODIGO = m.MCH_CODIGO 
                                            where 
                                              c.CEN_CODIGO = cargaEntrega.CEN_CODIGO
                                            order by 
                                              c.CHA_CODIGO desc
                                          ) MotivoUltimaOcorrencia, ");

                        if (!groupBy.Contains("cargaEntrega.CEN_CODIGO"))
                            groupBy.Append("cargaEntrega.CEN_CODIGO, ");

                        SetarJoinsXMLNotaFiscal(joins);
                    }
                    break;

                case "DataUltimaOcorrencia":
                    if (!select.Contains(" DataUltimaOcorrencia,"))
                    {
                        select.Append(@"(
                                            select 
                                              top 1 FORMAT(c.CHA_DATA_CRICAO, 'dd/MM/yyyy HH:mm') 
                                            from 
                                              T_CHAMADOS c
                                            where 
                                              c.CEN_CODIGO = cargaEntrega.CEN_CODIGO
                                            order by 
                                              c.CHA_CODIGO desc
                                          ) DataUltimaOcorrencia, ");

                        if (!groupBy.Contains("cargaEntrega.CEN_CODIGO"))
                            groupBy.Append("cargaEntrega.CEN_CODIGO, ");

                        SetarJoinsXMLNotaFiscal(joins);
                    }
                    break;

                case "DataInicioEntrega":
                    if (!select.Contains(" DataInicioEntrega,"))
                    {
                        select.Append("FORMAT(cargaEntrega.CEN_DATA_INICIO_ENTREGA, 'dd/MM/yyyy HH:mm') DataInicioEntrega, ");

                        if (!groupBy.Contains("cargaEntrega.CEN_DATA_INICIO_ENTREGA"))
                            groupBy.Append("cargaEntrega.CEN_DATA_INICIO_ENTREGA, ");
                    }
                    break;

                case "DataFimEntrega":
                    if (!select.Contains(" DataFimEntrega,"))
                    {
                        select.Append("FORMAT(cargaEntrega.CEN_DATA_FIM_ENTREGA, 'dd/MM/yyyy HH:mm') DataFimEntrega, ");

                        if (!groupBy.Contains("cargaEntrega.CEN_DATA_FIM_ENTREGA"))
                            groupBy.Append("cargaEntrega.CEN_DATA_FIM_ENTREGA, ");
                    }
                    break;

                case "PossuiCanhoto":
                case "PossuiCanhotoFormatado":
                    if (!select.Contains(" PossuiCanhoto,"))
                    {
                        select.Append(@"CASE
	                                        WHEN canhotoNotaFiscal.CNF_CODIGO is null THEN 'Não'
	                                        WHEN canhotoNotaFiscal.CNF_SITUACAO_DIGITALIZACAO_CANHOTO = 3  THEN 'Sim'
	                                        WHEN canhotoNotaFiscal.CNF_SITUACAO_DIGITALIZACAO_CANHOTO = 2 THEN 'Sim - Pendente'
	                                        ELSE 
		                                        ''
	                                        END PossuiCanhoto, ");

                        if (!groupBy.Contains("canhotoNotaFiscal.CNF_CODIGO"))
                            groupBy.Append("canhotoNotaFiscal.CNF_CODIGO, ");
                        groupBy.Append(" canhotoNotaFiscal.CNF_SITUACAO_DIGITALIZACAO_CANHOTO, ");

                        SetarJoinsCanhotoNotaFiscal(joins);
                    }
                    break;

                case "UsuarioCanhoto":
                    if (!select.Contains(" UsuarioCanhoto,"))
                    {
                        select.Append("usuarioDigitalaizacaoCanhoto.FUN_NOME UsuarioCanhoto, ");

                        if (!groupBy.Contains("usuarioDigitalaizacaoCanhoto.FUN_NOME"))
                            groupBy.Append("usuarioDigitalaizacaoCanhoto.FUN_NOME, ");

                        SetarJoinsUsuarioDigitalizacaoCanhoto(joins);
                    }
                    break;

                case "DataCanhoto":
                    if (!select.Contains(" DataCanhoto,"))
                    {
                        select.Append("FORMAT(canhotoNotaFiscal.CNF_DATA_ENVIO_CANHOTO, 'dd/MM/yyyy HH:mm') DataCanhoto, ");

                        if (!groupBy.Contains("canhotoNotaFiscal.CNF_DATA_ENVIO_CANHOTO"))
                            groupBy.Append("canhotoNotaFiscal.CNF_DATA_ENVIO_CANHOTO, ");

                        SetarJoinsCanhotoNotaFiscal(joins);
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador,"))
                    {
                        select.Append("transportador.EMP_RAZAO Transportador, ");

                        if (!groupBy.Contains("transportador.EMP_RAZAO"))
                            groupBy.Append("transportador.EMP_RAZAO, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "Motoristas":
                    if (!select.Contains(" Motoristas,"))
                    {
                        select.Append(@"SUBSTRING((SELECT ', ' + motorista1.FUN_NOME				
				                                    FROM T_CARGA_MOTORISTA motoristaCarga1 
					                                    INNER JOIN T_FUNCIONARIO motorista1 ON motoristaCarga1.CAR_MOTORISTA = motorista1.FUN_CODIGO 
				                                    WHERE motoristaCarga1.CAR_CODIGO = carga.CAR_CODIGO FOR XML PATH('')), 3, 1000) Motoristas, ");

                        if (!groupBy.Contains("carga.CAR_CODIGO,"))
                            groupBy.Append("carga.CAR_CODIGO, ");
                    }
                    break;

                case "PlacaTracao":
                case "PlacaTracaoFormatada":
                    if (!select.Contains(" PlacaTracao,"))
                    {
                        select.Append("veiculo.VEI_PLACA PlacaTracao, ");

                        if (!groupBy.Contains("veiculo.VEI_PLACA"))
                            groupBy.Append("veiculo.VEI_PLACA, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "PlacasReboques":
                    if (!select.Contains(" PlacasReboques,"))
                    {
                        select.Append(@"SUBSTRING((select ', ' + reboque.VEI_PLACA
		                                            from T_CARGA_VEICULOS_VINCULADOS cargaVeiculosVinculados
		                                            join T_VEICULO reboque on reboque.VEI_CODIGO = cargaVeiculosVinculados.VEI_CODIGO
		                                            where cargaVeiculosVinculados.CAR_CODIGO = carga.CAR_CODIGO FOR XML PATH('')), 3, 100) PlacasReboques, ");

                        if (!groupBy.Contains("carga.CAR_CODIGO,"))
                            groupBy.Append("carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DataUltimaPosicao":
                    if (!select.Contains(" DataUltimaPosicao,"))
                    {
                        select.Append("FORMAT(posicao.POS_DATA, 'dd/MM/yyyy HH:mm') DataUltimaPosicao, ");

                        if (!groupBy.Contains("posicao.POS_DATA"))
                            groupBy.Append("posicao.POS_DATA, ");

                        SetarJoinsPosicao(joins);
                    }
                    break;

                case "StatusPosicaoVeiculo":
                case "StatusPosicaoVeiculoFormatado":
                    if (!select.Contains(" StatusPosicaoVeiculo,"))
                    {
                        select.Append("posicaoAtual.POA_STATUS StatusPosicaoVeiculo, ");

                        if (!groupBy.Contains("posicaoAtual.POA_STATUS"))
                            groupBy.Append("posicaoAtual.POA_STATUS, ");

                        SetarJoinsPosicaoAtual(joins);
                    }
                    break;

                case "CoordenadasLocalizacaoAtual":
                    if (!select.Contains(" CoordenadasLocalizacaoAtual,"))
                    {
                        select.Append("CONCAT('Lat. ' + CAST(posicaoAtual.POA_LATITUDE as varchar), + ', ' + 'Long. ' + CAST(posicaoAtual.POA_LONGITUDE as varchar)) CoordenadasLocalizacaoAtual, ");

                        if (!groupBy.Contains("posicaoAtual.POA_LATITUDE"))
                            groupBy.Append("posicaoAtual.POA_LATITUDE, ");

                        if (!groupBy.Contains("posicaoAtual.POA_LONGITUDE"))
                            groupBy.Append("posicaoAtual.POA_LONGITUDE, ");

                        SetarJoinsPosicaoAtual(joins);
                    }
                    break;

                case "TipoInteracaoInicioViagem":
                case "TipoInteracaoInicioViagemFormatado":
                    if (!select.Contains(" TipoInteracaoInicioViagem,"))
                    {
                        select.Append(@"CASE 
	                                        WHEN carga.CAR_ORIGEM_SITUACAO = 3
		                                        THEN 1
	                                        WHEN carga.CAR_ORIGEM_SITUACAO != 3
		                                        THEN 2
	                                        ELSE null
                                          END TipoInteracaoInicioViagem, ");

                        if (!groupBy.Contains("carga.CAR_ORIGEM_SITUACAO"))
                            groupBy.Append("carga.CAR_ORIGEM_SITUACAO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "TipoInteracaoChegadaViagem":
                case "TipoInteracaoChegadaViagemFormatado":
                    if (!select.Contains(" TipoInteracaoChegadaViagem,"))
                    {
                        select.Append(@"CASE 
	                                        WHEN cargaEntrega.CEN_ORIGEM_SITUACAO = 3
		                                        THEN 1
	                                        WHEN cargaEntrega.CEN_ORIGEM_SITUACAO != 3
		                                        THEN 2
	                                        ELSE null
                                          END TipoInteracaoChegadaViagem, ");

                        if (!groupBy.Contains("cargaEntrega.CEN_ORIGEM_SITUACAO"))
                            groupBy.Append("cargaEntrega.CEN_ORIGEM_SITUACAO, ");
                    }
                    break;

                case "TipoInteracaoFimEntrega":
                case "TipoInteracaoFimEntregaFormatado":
                    if (!select.Contains(" TipoInteracaoFimEntrega,"))
                    {
                        select.Append(@"CASE 
	                                        WHEN cargaEntrega.CEN_ORIGEM_SITUACAO = 3
		                                        THEN 1
	                                        WHEN cargaEntrega.CEN_ORIGEM_SITUACAO != 3
		                                        THEN 2
	                                        ELSE null
                                          END TipoInteracaoFimEntrega, ");

                        if (!groupBy.Contains("carga.CAR_ORIGEM_SITUACAO"))
                            groupBy.Append("carga.CAR_ORIGEM_SITUACAO, ");

                        if (!groupBy.Contains("cargaEntrega.CEN_ORIGEM_SITUACAO"))
                            groupBy.Append("cargaEntrega.CEN_ORIGEM_SITUACAO, ");
                    }
                    break;

                case "PercentualViagem":
                    if (!select.Contains(" PercentualViagem,"))
                    {
                        select.Append("monitoramento.MON_PERCENTUAL_VIAGEM PercentualViagem, ");

                        if (!groupBy.Contains("monitoramento.MON_PERCENTUAL_VIAGEM"))
                            groupBy.Append("monitoramento.MON_PERCENTUAL_VIAGEM, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "CodigoFilial":
                    if (!select.Contains(" CodigoFilial,"))
                    {
                        select.Append("filial.FIL_CODIGO_FILIAL_EMBARCADOR CodigoFilial, ");

                        if (!groupBy.Contains("filial.FIL_CODIGO_FILIAL_EMBARCADOR"))
                            groupBy.Append("filial.FIL_CODIGO_FILIAL_EMBARCADOR, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "NomeFilial":
                    if (!select.Contains(" NomeFilial,"))
                    {
                        select.Append("filial.FIL_DESCRICAO NomeFilial, ");

                        if (!groupBy.Contains("filial.FIL_DESCRICAO"))
                            groupBy.Append("filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "CodigoOrigem":
                    if (!select.Contains(" CodigoOrigem,"))
                    {
                        select.Append("ISNULL(expedidor.CLI_CODIGO_INTEGRACAO, remetentePedido.CLI_CODIGO_INTEGRACAO) CodigoOrigem, ");

                        if (!groupBy.Contains("expedidor.CLI_CODIGO_INTEGRACAO"))
                            groupBy.Append("expedidor.CLI_CODIGO_INTEGRACAO, ");

                        if (!groupBy.Contains("remetentePedido.CLI_CODIGO_INTEGRACAO"))
                            groupBy.Append("remetentePedido.CLI_CODIGO_INTEGRACAO, ");


                        SetarJoinsExpedidor(joins);
                        SetarJoinsRemetentePedido(joins);
                    }
                    break;

                case "EnderecoOrigem":
                    if (!select.Contains(" EnderecoOrigem,"))
                    {
                        select.Append("ISNULL(expedidor.CLI_ENDERECO, remetentePedido.CLI_ENDERECO) EnderecoOrigem, ");

                        if (!groupBy.Contains("expedidor.CLI_ENDERECO"))
                            groupBy.Append("expedidor.CLI_ENDERECO, ");

                        if (!groupBy.Contains("remetentePedido.CLI_ENDERECO"))
                            groupBy.Append("remetentePedido.CLI_ENDERECO, ");


                        SetarJoinsExpedidor(joins);
                        SetarJoinsRemetentePedido(joins);
                    }
                    break;

                case "CidadeUFOrigem":
                    if (!select.Contains(" CidadeUFOrigem,"))
                    {
                        select.Append("CONCAT(ISNULL(localidadeExpedidor.LOC_DESCRICAO, localidadeRemetentePedido.LOC_DESCRICAO), ' - ',ISNULL(localidadeExpedidor.UF_SIGLA, localidadeRemetentePedido.UF_SIGLA)) CidadeUFOrigem, ");

                        if (!groupBy.Contains("localidadeExpedidor.LOC_DESCRICAO"))
                            groupBy.Append("localidadeExpedidor.LOC_DESCRICAO, ");

                        if (!groupBy.Contains("localidadeExpedidor.UF_SIGLA"))
                            groupBy.Append("localidadeExpedidor.UF_SIGLA, ");

                        if (!groupBy.Contains("localidadeRemetentePedido.LOC_DESCRICAO"))
                            groupBy.Append("localidadeRemetentePedido.LOC_DESCRICAO, ");

                        if (!groupBy.Contains("localidadeRemetentePedido.UF_SIGLA"))
                            groupBy.Append("localidadeRemetentePedido.UF_SIGLA, ");


                        SetarJoinsLocalidadeExpedidor(joins);
                        SetarJoinsLocalidadeRemetentePedido(joins);
                    }
                    break;

                case "Origem":
                    if (!select.Contains(" Origem,"))
                    {
                        select.Append(@"ISNULL(expedidor.CLI_NOME + ' (' + 
									                                        CASE 
										                                        WHEN expedidor.CLI_FISJUR in ('J', 'E') 
											                                        THEN FORMAT(expedidor.CLI_CGCCPF, '00\.000\.000\/0000\-00') 
									                                        ELSE FORMAT(expedidor.CLI_CGCCPF, '000\.000\.000\-00') 
									                                        END + 
								                                        ')', 
	                                        remetentePedido.CLI_NOME + ' (' + 
										                                        CASE 
											                                        WHEN remetentePedido.CLI_FISJUR in ('J', 'E') 
												                                        THEN FORMAT(remetentePedido.CLI_CGCCPF, '00\.000\.000\/0000\-00') 
										                                        ELSE FORMAT(remetentePedido.CLI_CGCCPF, '000\.000\.000\-00') 
										                                        END + ')') Origem, ");

                        if (!groupBy.Contains("expedidor.CLI_NOME"))
                            groupBy.Append("expedidor.CLI_NOME, ");

                        if (!groupBy.Contains("expedidor.CLI_FISJUR"))
                            groupBy.Append("expedidor.CLI_FISJUR, ");

                        if (!groupBy.Contains("expedidor.CLI_CGCCPF"))
                            groupBy.Append("expedidor.CLI_CGCCPF, ");

                        if (!groupBy.Contains("remetentePedido.CLI_NOME"))
                            groupBy.Append("remetentePedido.CLI_NOME, ");

                        if (!groupBy.Contains("remetentePedido.CLI_FISJUR"))
                            groupBy.Append("remetentePedido.CLI_FISJUR, ");

                        if (!groupBy.Contains("remetentePedido.CLI_CGCCPF"))
                            groupBy.Append("remetentePedido.CLI_CGCCPF, ");

                        SetarJoinsExpedidor(joins);
                        SetarJoinsRemetentePedido(joins);
                    }
                    break;

                case "CoordenadasOrigem":
                    if (!select.Contains(" CoordenadasOrigem,"))
                    {
                        select.Append("CONCAT('Lat. ' + cast(ISNULL(expedidor.CLI_LATIDUDE, remetentePedido.CLI_LATIDUDE) as varchar), + ', Long. ' + cast(ISNULL(expedidor.CLI_LONGITUDE, remetentePedido.CLI_LONGITUDE) as varchar)) CoordenadasOrigem, ");

                        if (!groupBy.Contains("expedidor.CLI_LATIDUDE"))
                            groupBy.Append("expedidor.CLI_LATIDUDE, ");

                        if (!groupBy.Contains("remetentePedido.CLI_LATIDUDE"))
                            groupBy.Append("remetentePedido.CLI_LATIDUDE, ");

                        if (!groupBy.Contains("expedidor.CLI_LONGITUDE"))
                            groupBy.Append("expedidor.CLI_LONGITUDE, ");

                        if (!groupBy.Contains("remetentePedido.CLI_LONGITUDE"))
                            groupBy.Append("remetentePedido.CLI_LONGITUDE, ");

                        SetarJoinsExpedidor(joins);
                        SetarJoinsRemetentePedido(joins);
                    }
                    break;

                case "EnderecoCliente":
                    if (!select.Contains(" EnderecoCliente,"))
                    {
                        select.Append("ISNULL(recebedor.CLI_ENDERECO, destinatarioPedido.CLI_ENDERECO) EnderecoCliente, ");

                        if (!groupBy.Contains("recebedor.CLI_ENDERECO"))
                            groupBy.Append("recebedor.CLI_ENDERECO, ");

                        if (!groupBy.Contains("destinatarioPedido.CLI_ENDERECO"))
                            groupBy.Append("destinatarioPedido.CLI_ENDERECO, ");


                        SetarJoinsRecebedor(joins);
                        SetarJoinsDestinatarioPedido(joins);
                    }
                    break;

                case "CidadeUFCliente":
                    if (!select.Contains(" CidadeUFCliente,"))
                    {
                        select.Append("CONCAT(ISNULL(localidadeRecebedor.LOC_DESCRICAO, localidadeDestinatarioPedido.LOC_DESCRICAO), ' - ',ISNULL(localidadeRecebedor.UF_SIGLA, localidadeDestinatarioPedido.UF_SIGLA)) CidadeUFCliente, ");

                        if (!groupBy.Contains("localidadeRecebedor.LOC_DESCRICAO"))
                            groupBy.Append("localidadeRecebedor.LOC_DESCRICAO, ");

                        if (!groupBy.Contains("localidadeRecebedor.UF_SIGLA"))
                            groupBy.Append("localidadeRecebedor.UF_SIGLA, ");

                        if (!groupBy.Contains("localidadeDestinatarioPedido.LOC_DESCRICAO"))
                            groupBy.Append("localidadeDestinatarioPedido.LOC_DESCRICAO, ");

                        if (!groupBy.Contains("localidadeDestinatarioPedido.UF_SIGLA"))
                            groupBy.Append("localidadeDestinatarioPedido.UF_SIGLA, ");

                        SetarJoinsLocalidadeRecebedor(joins);
                        SetarJoinsLocalidadeDestinatarioPedido(joins);
                    }
                    break;

                case "CoordenadasCliente":
                    if (!select.Contains(" CoordenadasCliente,"))
                    {
                        select.Append("CONCAT('Lat. ' + cast(ISNULL(recebedor.CLI_LATIDUDE, destinatarioPedido.CLI_LATIDUDE) as varchar), + ', Long. ' + cast(ISNULL(recebedor.CLI_LONGITUDE, destinatarioPedido.CLI_LONGITUDE) as varchar)) CoordenadasCliente, ");

                        if (!groupBy.Contains("recebedor.CLI_LATIDUDE"))
                            groupBy.Append("recebedor.CLI_LATIDUDE, ");

                        if (!groupBy.Contains("destinatarioPedido.CLI_LATIDUDE"))
                            groupBy.Append("destinatarioPedido.CLI_LATIDUDE, ");

                        if (!groupBy.Contains("recebedor.CLI_LONGITUDE"))
                            groupBy.Append("recebedor.CLI_LONGITUDE, ");

                        if (!groupBy.Contains("destinatarioPedido.CLI_LONGITUDE"))
                            groupBy.Append("destinatarioPedido.CLI_LONGITUDE, ");

                        SetarJoinsRecebedor(joins);
                        SetarJoinsDestinatarioPedido(joins);
                    }
                    break;

                case "Cliente":
                    if (!select.Contains(" Cliente,"))
                    {
                        select.Append(@"ISNULL(recebedor.CLI_NOME + ' (' + 
									                                        CASE 
										                                        WHEN recebedor.CLI_FISJUR in ('J', 'E') 
											                                        THEN FORMAT(recebedor.CLI_CGCCPF, '00\.000\.000\/0000\-00') 
									                                        ELSE FORMAT(recebedor.CLI_CGCCPF, '000\.000\.000\-00') 
									                                        END + 
								                                        ')', 
	                                        destinatarioPedido.CLI_NOME + ' (' + 
										                                        CASE 
											                                        WHEN destinatarioPedido.CLI_FISJUR in ('J', 'E') 
												                                        THEN FORMAT(destinatarioPedido.CLI_CGCCPF, '00\.000\.000\/0000\-00') 
										                                        ELSE FORMAT(destinatarioPedido.CLI_CGCCPF, '000\.000\.000\-00') 
										                                        END + ')') Cliente, ");

                        if (!groupBy.Contains("recebedor.CLI_NOME"))
                            groupBy.Append("recebedor.CLI_NOME, ");

                        if (!groupBy.Contains("recebedor.CLI_FISJUR"))
                            groupBy.Append("recebedor.CLI_FISJUR, ");

                        if (!groupBy.Contains("recebedor.CLI_CGCCPF"))
                            groupBy.Append("recebedor.CLI_CGCCPF, ");

                        if (!groupBy.Contains("destinatarioPedido.CLI_NOME"))
                            groupBy.Append("destinatarioPedido.CLI_NOME, ");

                        if (!groupBy.Contains("destinatarioPedido.CLI_FISJUR"))
                            groupBy.Append("destinatarioPedido.CLI_FISJUR, ");

                        if (!groupBy.Contains("destinatarioPedido.CLI_CGCCPF"))
                            groupBy.Append("destinatarioPedido.CLI_CGCCPF, ");

                        SetarJoinsRecebedor(joins);
                        SetarJoinsDestinatarioPedido(joins);
                    }
                    break;

                case "OrdemEntregaPrevista":
                    if (!select.Contains(" OrdemEntregaPrevista,"))
                    {
                        select.Append("cargaEntrega.CEN_ORDEM OrdemEntregaPrevista, ");

                        if (!groupBy.Contains("cargaEntrega.CEN_ORDEM"))
                            groupBy.Append("cargaEntrega.CEN_ORDEM, ");
                    }
                    break;

                case "OrdemEntregaRealizada":
                    if (!select.Contains(" OrdemEntregaRealizada,"))
                    {
                        select.Append("cargaEntrega.CEN_ORDEM_REALIZADA OrdemEntregaRealizada, ");

                        if (!groupBy.Contains("cargaEntrega.CEN_ORDEM_REALIZADA"))
                            groupBy.Append("cargaEntrega.CEN_ORDEM_REALIZADA, ");
                    }
                    break;

                case "DistanciaPrevista":
                    if (!select.Contains(" DistanciaPrevista,"))
                    {
                        select.Append("monitoramento.MON_DISTANCIA_PREVISTA DistanciaPrevista, ");

                        if (!groupBy.Contains("monitoramento.MON_DISTANCIA_PREVISTA"))
                            groupBy.Append("monitoramento.MON_DISTANCIA_PREVISTA, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "DistanciaAteDestino":
                    if (!select.Contains(" DistanciaAteDestino,"))
                    {
                        select.Append("cargaEntrega.CEN_DISTANCIA_ATE_DESTINO DistanciaAteDestino, ");

                        if (!groupBy.Contains("cargaEntrega.CEN_DISTANCIA_ATE_DESTINO"))
                            groupBy.Append("cargaEntrega.CEN_DISTANCIA_ATE_DESTINO, ");
                    }
                    break;

                case "PercentualCargaEntregue":
                    if (!select.Contains(" PercentualCargaEntregue,"))
                    {
                        select.Append(@"(select 
					                                    ((select count(_cargaEntrega.CEN_CODIGO) 
					                                    from T_CARGA_ENTREGA _cargaEntrega
					                                    where (_cargaEntrega.CEN_SITUACAO = 2 or _cargaEntrega.CEN_SITUACAO = 5) and
					                                    _cargaEntrega.CAR_CODIGO = carga.CAR_CODIGO) * 100) / COALESCE((select count(__cargaEntrega.CEN_CODIGO) 
																			                                    from T_CARGA_ENTREGA __cargaEntrega 
																			                                    where __cargaEntrega.CAR_CODIGO = carga.CAR_CODIGO), 1)
	                                    ) PercentualCargaEntregue, ");

                        if (!groupBy.Contains("carga.CAR_CODIGO"))
                            groupBy.Append("carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "CodigoTipoOperacao":
                    if (!select.Contains(" CodigoTipoOperacao,"))
                    {
                        select.Append("tipoOperacao.TOP_CODIGO_INTEGRACAO CodigoTipoOperacao, ");

                        if (!groupBy.Contains("tipoOperacao.TOP_CODIGO_INTEGRACAO"))
                            groupBy.Append("tipoOperacao.TOP_CODIGO_INTEGRACAO, ");

                        SetarJoinsTipoOperacao(joins);
                    }
                    break;

                case "CodigoProduto":
                    if (!select.Contains(" CodigoProduto,"))
                    {
                        select.Append("produto.PRO_CODIGO_PRODUTO_EMBARCADOR CodigoProduto, ");

                        if (!groupBy.Contains("produto.PRO_CODIGO_PRODUTO_EMBARCADOR"))
                            groupBy.Append("produto.PRO_CODIGO_PRODUTO_EMBARCADOR, ");

                        SetarJoinsProduto(joins);
                    }
                    break;

                case "DescricaoProduto":
                    if (!select.Contains(" DescricaoProduto,"))
                    {
                        select.Append("produto.GRP_DESCRICAO DescricaoProduto, ");

                        if (!groupBy.Contains("produto.GRP_DESCRICAO"))
                            groupBy.Append("produto.GRP_DESCRICAO, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "CodigoModeloVeicular":
                    if (!select.Contains(" CodigoModeloVeicular,"))
                    {
                        select.Append("modeloVeicular.MVC_CODIGO_MODELO_VEICULAR_DE_CARGA_EMBARCADOR CodigoModeloVeicular, ");

                        if (!groupBy.Contains("modeloVeicular.MVC_CODIGO_MODELO_VEICULAR_DE_CARGA_EMBARCADOR"))
                            groupBy.Append("modeloVeicular.MVC_CODIGO_MODELO_VEICULAR_DE_CARGA_EMBARCADOR, ");

                        SetarJoinsModeloVeicular(joins);
                    }
                    break;

                case "DescricaoModeloVeicular":
                    if (!select.Contains(" DescricaoModeloVeicular,"))
                    {
                        select.Append("modeloVeicular.MVC_DESCRICAO DescricaoModeloVeicular, ");

                        if (!groupBy.Contains("modeloVeicular.MVC_DESCRICAO"))
                            groupBy.Append("modeloVeicular.MVC_DESCRICAO, ");

                        SetarJoinsModeloVeicular(joins);
                    }
                    break;

                case "TipoOcorrencia":
                    if (!select.Contains(" TipoOcorrencia,"))
                    {
                        select.Append(@"(
                                            select 
                                              top 1 oco.OCO_DESCRICAO
                                            from 
                                              T_OCORRENCIA_COLETA_ENTREGA coleta join T_OCORRENCIA oco on coleta.OCO_CODIGO = oco.OCO_CODIGO
                                            where 
                                              coleta.CEN_CODIGO = cargaEntrega.CEN_CODIGO
                                            order by 
                                              coleta.OCE_CODIGO desc
                                          ) TipoOcorrencia, ");

                        if (!groupBy.Contains("cargaEntrega.CEN_CODIGO"))
                            groupBy.Append("cargaEntrega.CEN_CODIGO, ");

                        SetarJoinsOcorrenciaColetaEntrega(joins);
                    }
                    break;

                case "SituacaoViagem":
                case "SituacaoViagemFormatada":
                    if (!select.Contains(" SituacaoViagem,"))
                    {
                        select.Append(@"case when carga.CAR_DATA_INICIO_VIAGEM is not null and carga.CAR_DATA_FIM_VIAGEM is null
                                                then 5
                                             when carga.CAR_DATA_INICIO_VIAGEM is not null and carga.CAR_DATA_FIM_VIAGEM is not null
                                                then 2
                                             when carga.CAR_DATA_INICIO_VIAGEM is null
                                                then 3
                                            else null
                                        end SituacaoViagem, ");

                        if (!groupBy.Contains("carga.CAR_DATA_INICIO_VIAGEM"))
                            groupBy.Append("carga.CAR_DATA_INICIO_VIAGEM, ");

                        if (!groupBy.Contains("carga.CAR_DATA_FIM_VIAGEM"))
                            groupBy.Append("carga.CAR_DATA_FIM_VIAGEM, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "PedidoCriticoFormatado":
                    if (!select.Contains(" PedidoCritico,"))
                    {
                        select.Append(@"CAST(pedido.PED_PEDIDO_CRITICO AS INT) AS PedidoCritico, ");

                        if (!groupBy.Contains("pedido.PED_PEDIDO_CRITICO"))
                            groupBy.Append("pedido.PED_PEDIDO_CRITICO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsolidadoEntregas filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {

            where.Append($"and carga.CAR_SITUACAO <> 13");

            if (filtrosPesquisa.DataInicioViagemPrevistaInicial.HasValue)
            {
                SetarJoinsCarga(joins);

                where.Append($" and CAST(carga.CAR_DATA_INICIO_VIAGEM_PREVISTA AS DATE) >= '{filtrosPesquisa.DataInicioViagemPrevistaInicial.Value.ToString("yyyy-MM-dd")}'");
            }

            if (filtrosPesquisa.DataInicioViagemPrevistaFinal.HasValue)
            {
                SetarJoinsCarga(joins);

                where.Append($" and CAST(carga.CAR_DATA_INICIO_VIAGEM_PREVISTA AS DATE) <= '{filtrosPesquisa.DataInicioViagemPrevistaFinal.Value.ToString("yyyy-MM-dd")}'");
            }

            if (filtrosPesquisa.DataInicioViagemRealizadaInicial.HasValue)
            {
                SetarJoinsCarga(joins);

                where.Append($" and CAST(carga.CAR_DATA_INICIO_VIAGEM AS DATE) >= '{filtrosPesquisa.DataInicioViagemRealizadaInicial.Value.ToString("yyyy-MM-dd")}'");
            }

            if (filtrosPesquisa.DataInicioViagemRealizadaFinal.HasValue)
            {
                SetarJoinsCarga(joins);

                where.Append($" and CAST(carga.CAR_DATA_INICIO_VIAGEM AS DATE) <= '{filtrosPesquisa.DataInicioViagemRealizadaFinal.Value.ToString("yyyy-MM-dd")}'");
            }

            if (filtrosPesquisa.DataConfirmacaoInicial.HasValue)
            {
                where.Append($" and CAST(cargaEntrega.CEN_DATA_ENTREGA AS DATE) >= '{filtrosPesquisa.DataConfirmacaoInicial.Value.ToString("yyyy-MM-dd")}'");
            }

            if (filtrosPesquisa.DataConfirmacaoFinal.HasValue)
            {
                where.Append($" and CAST(cargaEntrega.CEN_DATA_ENTREGA AS DATE) <= '{filtrosPesquisa.DataConfirmacaoFinal.Value.ToString("yyyy-MM-dd")}'");
            }

            if (filtrosPesquisa.CodigoPedido > 0)
            {
                where.Append($" and pedido.PED_CODIGO = {filtrosPesquisa.CodigoPedido}");

                SetarJoinsPedido(joins);
            }

            if (filtrosPesquisa.CodigoNotaFiscal > 0)
            {
                where.Append($" and XMLNotaFiscal.NFX_CODIGO = {filtrosPesquisa.CodigoNotaFiscal}");

                SetarJoinsXMLNotaFiscal(joins);
            }

            if (filtrosPesquisa.CodigoTransportador.Count > 0)
            {
                SetarJoinsTransportador(joins);

                where.Append($" and transportador.emp_codigo in ({string.Join(",", filtrosPesquisa.CodigoTransportador)})");
            }

            if (filtrosPesquisa.CodigoMotorista > 0)
            {
                where.Append($@" and exists (SELECT top 1 1				
				                                    FROM T_CARGA_MOTORISTA motoristaCarga1 
					                                    INNER JOIN T_FUNCIONARIO motorista1 ON motoristaCarga1.CAR_MOTORISTA = motorista1.FUN_CODIGO 
				                                    WHERE motoristaCarga1.CAR_CODIGO = carga.CAR_CODIGO and motorista1.FUN_CODIGO = {filtrosPesquisa.CodigoMotorista})");
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                SetarJoinsVeiculo(joins);

                where.Append($" and veiculo.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo}");
            }

            if (filtrosPesquisa.CpfCnpjClienteDestino > 0d)
            {
                where.Append($" and ( recebedor.CLI_CGCCPF = {filtrosPesquisa.CpfCnpjClienteDestino} or pedido.CLI_CODIGO = {filtrosPesquisa.CpfCnpjClienteDestino} )");

                SetarJoinsRecebedor(joins);
                SetarJoinsPedido(joins);
            }

            if (filtrosPesquisa.CpfCnpjClienteOrigem > 0d)
            {
                where.Append($" and ( expedidor.CLI_CGCCPF = {filtrosPesquisa.CpfCnpjClienteOrigem} or pedido.CLI_CODIGO_REMETENTE = {filtrosPesquisa.CpfCnpjClienteOrigem} )");

                SetarJoinsExpedidor(joins);
                SetarJoinsPedido(joins);
            }


            if (filtrosPesquisa.CodigoCidadeOrigem > 0)
            {
                where.Append($"and ( localidadeExpedidor.LOC_CODIGO = {filtrosPesquisa.CodigoCidadeOrigem} or localidadeRemetentePedido.LOC_CODIGO = {filtrosPesquisa.CodigoCidadeOrigem} )");

                SetarJoinsLocalidadeExpedidor(joins);
                SetarJoinsLocalidadeRemetentePedido(joins);
            }

            if (filtrosPesquisa.CodigoCidadeDestino > 0)
            {
                where.Append($"and ( localidadeRecebedor.LOC_CODIGO = {filtrosPesquisa.CodigoCidadeDestino} or localidadeDestinatarioPedido.LOC_CODIGO = {filtrosPesquisa.CodigoCidadeDestino} )");

                SetarJoinsLocalidadeRecebedor(joins);
                SetarJoinsLocalidadeDestinatarioPedido(joins);
            }

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
            {
                SetarJoinsTipoOperacao(joins);

                where.Append($" and tipoOperacao.TOP_CODIGO = {filtrosPesquisa.CodigoTipoOperacao}");
            }

            if (filtrosPesquisa.TipoInteracaoInicioViagem.HasValue)
            {
                if (filtrosPesquisa.TipoInteracaoInicioViagem.Value == TipoInteracaoEntrega.Mobile)
                    where.Append(" and carga.CAR_ORIGEM_SITUACAO = 3");

                else if (filtrosPesquisa.TipoInteracaoInicioViagem.Value == TipoInteracaoEntrega.Manual)
                    where.Append(" and carga.CAR_ORIGEM_SITUACAO != 3");

                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.TipoInteracaoChegadaViagem.HasValue)
            {
                if (filtrosPesquisa.TipoInteracaoChegadaViagem.Value == TipoInteracaoEntrega.Mobile)
                    where.Append(" and cargaEntrega.CEN_ORIGEM_SITUACAO = 3");

                else if (filtrosPesquisa.TipoInteracaoChegadaViagem.Value == TipoInteracaoEntrega.Manual)
                    where.Append(" and cargaEntrega.CEN_ORIGEM_SITUACAO != 3");
            }

            if (filtrosPesquisa.CodigoCarga.Count > 0)
            {
                where.Append($" and carga.CAR_CODIGO in ({string.Join(",", filtrosPesquisa.CodigoCarga)})");

                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.StatusViagem.HasValue)
            {
                switch (filtrosPesquisa.StatusViagem.Value)
                {
                    case StatusViagemControleEntrega.EmAndamento:
                        where.Append($" and carga.CAR_DATA_INICIO_VIAGEM is not null and carga.CAR_DATA_FIM_VIAGEM is null");
                        break;

                    case StatusViagemControleEntrega.Finalizada:
                        where.Append($" and carga.CAR_DATA_INICIO_VIAGEM is not null and carga.CAR_DATA_FIM_VIAGEM is not null");
                        break;

                    case StatusViagemControleEntrega.NaoIniciada:
                        where.Append($" and carga.CAR_DATA_INICIO_VIAGEM is null");
                        break;
                }

                SetarJoinsCarga(joins);
            }
        }

    }

}
