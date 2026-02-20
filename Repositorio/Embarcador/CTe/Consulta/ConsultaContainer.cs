using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.CTe
{
    sealed class ConsultaContainer : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioContainer>
    {
        #region Construtores

        public ConsultaContainer() : base(tabela: "T_CTE as CTe ") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCTeContainer(StringBuilder joins)
        {
            if (!joins.Contains(" CTeContainer "))
                joins.Append(" left join T_CTE_CONTAINER CTeContainer on CTe.CON_CODIGO = CTeContainer.CON_CODIGO ");
        }

        private void SetarJoinsCargaCTe(StringBuilder joins)
        {
            if (!joins.Contains(" CargaCTe "))
                joins.Append(" join T_CARGA_CTE CargaCTe on CargaCTe.CON_CODIGO = CTe.CON_CODIGO ");
        }

        private void SetarJoinsCargaCTePedidoXML(StringBuilder joins)
        {
            if (!joins.Contains(" CargaPedidoXMLNotaFiscalCTe "))
                joins.Append(" JOIN T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe ON CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO ");
        }

        private void SetarJoinsPedidoXMLNotaFiscal(StringBuilder joins)
        {
            if (!joins.Contains(" PedidoXMLNotaFiscal "))
                joins.Append(" JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal ON PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO ");
        }

        private void SetarJoinsCargaPedidoXMLNotaFiscal(StringBuilder joins)
        {
            if (!joins.Contains(" CargaPedidoXMLNotaFiscal "))
                joins.Append(" JOIN T_CARGA_PEDIDO CargaPedidoXMLNotaFiscal ON CargaPedidoXMLNotaFiscal.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO ");
        }

        private void SetarJoinsPedidoCargaPedidoXMLNotaFiscal(StringBuilder joins)
        {
            if (!joins.Contains(" PedidoCargaPedidoXMLNotaFiscal "))
                joins.Append(" JOIN T_PEDIDO PedidoCargaPedidoXMLNotaFiscal ON CargaPedidoXMLNotaFiscal.PED_CODIGO = PedidoCargaPedidoXMLNotaFiscal.PED_CODIGO and PedidoCargaPedidoXMLNotaFiscal.CTR_CODIGO = CTeContainer.CTR_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append(" join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO ");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            if (!joins.Contains(" CargaPedido "))
                joins.Append(" join T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
        }

        private void SetarJoinsPedido(StringBuilder joins)
        {
            if (!joins.Contains(" Pedido "))
                joins.Append(" join T_PEDIDO Pedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO ");
        }

        private void SetarJoinsCTeDocs(StringBuilder joins)
        {
            if (!joins.Contains(" CTeDocs "))
                joins.Append(" join T_CTE_DOCS CTeDocs on CTeDocs.CON_CODIGO = CTe.CON_CODIGO ");
        }

        private void SetarJoinsContainer(StringBuilder joins)
        {
            SetarJoinsCTeContainer(joins);

            if (!joins.Contains(" Container "))
                joins.Append(" left join T_CONTAINER Container on Container.CTR_CODIGO = CTeContainer.CTR_CODIGO ");
        }

        private void SetarJoinsContainerTipo(StringBuilder joins)
        {
            SetarJoinsContainer(joins);

            if (!joins.Contains(" ContainerTipo "))
                joins.Append(" left join T_CONTAINER_TIPO ContainerTipo on ContainerTipo.CTI_CODIGO = Container.CTI_CODIGO ");
        }

        private void SetarJoinsSerie(StringBuilder joins)
        {
            if (!joins.Contains(" Serie "))
                joins.Append(" left join T_EMPRESA_SERIE Serie on CTe.CON_SERIE = Serie.ESE_CODIGO ");
        }

        private void SetarJoinsTomador(StringBuilder joins)
        {
            if (!joins.Contains(" TomadorPagadorCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE TomadorPagadorCTe on TomadorPagadorCTe.PCT_CODIGO = CTe.CON_TOMADOR_PAGADOR_CTE ");
        }

        private void SetarJoinsRemetente(StringBuilder joins)
        {
            if (!joins.Contains(" Remetente "))
                joins.Append(" left join T_CTE_PARTICIPANTE Remetente on Remetente.PCT_CODIGO = CTe.CON_REMETENTE_CTE ");
        }

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            if (!joins.Contains(" Destinatario "))
                joins.Append(" left join T_CTE_PARTICIPANTE Destinatario on Destinatario.PCT_CODIGO = CTe.CON_DESTINATARIO_CTE ");
        }

        private void SetarJoinsExpedidor(StringBuilder joins)
        {
            if (!joins.Contains(" Expedidor "))
                joins.Append(" left join T_CTE_PARTICIPANTE Expedidor on Expedidor.PCT_CODIGO = CTe.CON_EXPEDIDOR_CTE ");
        }

        private void SetarJoinsRecebedor(StringBuilder joins)
        {
            if (!joins.Contains(" Recebedor "))
                joins.Append(" left join T_CTE_PARTICIPANTE Recebedor on Recebedor.PCT_CODIGO = CTe.CON_RECEBEDOR_CTE ");
        }

        private void SetarJoinsPortoOrigem(StringBuilder joins)
        {
            if (!joins.Contains(" PortoOrigem "))
                joins.Append(" left join T_PORTO PortoOrigem on PortoOrigem.POT_CODIGO = CTe.POT_CODIGO_ORIGEM ");
        }

        private void SetarJoinsPortoDestino(StringBuilder joins)
        {
            if (!joins.Contains(" PortoDestino "))
                joins.Append(" left join T_PORTO PortoDestino on PortoDestino.POT_CODIGO = CTe.POT_CODIGO_DESTINO ");
        }

        private void SetarJoinsPortoPassagemUm(StringBuilder joins)
        {
            if (!joins.Contains(" PortoPassagemUm "))
                joins.Append(" left join T_PORTO PortoPassagemUm on PortoPassagemUm.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_UM ");
        }

        private void SetarJoinsPortoPassagemDois(StringBuilder joins)
        {
            if (!joins.Contains(" PortoPassagemDois "))
                joins.Append(" left join T_PORTO PortoPassagemDois on PortoPassagemDois.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_DOIS ");
        }

        private void SetarJoinsPortoPassagemTres(StringBuilder joins)
        {
            if (!joins.Contains(" PortoPassagemTres "))
                joins.Append(" left join T_PORTO PortoPassagemTres on PortoPassagemTres.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_TRES ");
        }

        private void SetarJoinsPortoPassagemQuatro(StringBuilder joins)
        {
            if (!joins.Contains(" PortoPassagemQuatro "))
                joins.Append(" left join T_PORTO PortoPassagemQuatro on PortoPassagemQuatro.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_QUATRO ");
        }

        private void SetarJoinsPortoPassagemCinco(StringBuilder joins)
        {
            if (!joins.Contains(" PortoPassagemCinco "))
                joins.Append(" left join T_PORTO PortoPassagemCinco on PortoPassagemCinco.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_CINCO ");
        }

        private void SetarJoinsTerminalOrigem(StringBuilder joins)
        {
            if (!joins.Contains(" TerminalOrigem "))
                joins.Append(" left join T_TIPO_TERMINAL_IMPORTACAO TerminalOrigem on TerminalOrigem.TTI_CODIGO = CTe.CON_TERMINAL_ORIGEM ");
        }

        private void SetarJoinsTerminalDestino(StringBuilder joins)
        {
            if (!joins.Contains(" TerminalDestino "))
                joins.Append(" left join T_TIPO_TERMINAL_IMPORTACAO TerminalDestino on TerminalDestino.TTI_CODIGO = CTe.CON_TERMINAL_DESTINO ");
        }

        private void SetarJoinsViagem(StringBuilder joins)
        {
            if (!joins.Contains(" Viagem "))
                joins.Append(" left join T_PEDIDO_VIAGEM_NAVIO Viagem on Viagem.PVN_CODIGO = CTe.CON_VIAGEM ");
        }

        private void SetarJoinsLocalidadeInicioPrestacao(StringBuilder joins)
        {
            if (!joins.Contains(" InicioPrestacaoCTe "))
                joins.Append(" left join T_LOCALIDADES InicioPrestacaoCTe on CTe.CON_LOCINICIOPRESTACAO = InicioPrestacaoCTe.LOC_CODIGO ");
        }

        private void SetarJoinsLocalidadeFimPrestacao(StringBuilder joins)
        {
            if (!joins.Contains(" FimPrestacaoCTe "))
                joins.Append(" left join T_LOCALIDADES FimPrestacaoCTe on CTe.CON_LOCTERMINOPRESTACAO = FimPrestacaoCTe.LOC_CODIGO ");
        }

        private void SetarJoinsModeloDocumento(StringBuilder joins)
        {
            if (!joins.Contains(" ModeloDocumento "))
                joins.Append(" left join T_MODDOCFISCAL ModeloDocumento on CTe.CON_MODELODOC = ModeloDocumento.MOD_CODIGO ");
        }

        private void SetarJoinsCfop(StringBuilder joins)
        {
            if (!joins.Contains(" CFOP "))
                joins.Append(" left join T_CFOP CFOP on CFOP.CFO_CODIGO = CTe.CFO_CODIGO ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" TransportadorCTe "))
                joins.Append(" inner join T_EMPRESA TransportadorCTe on CTe.EMP_CODIGO = TransportadorCTe.EMP_CODIGO ");
        }

        private void SetarJoinsTransportadorPai(StringBuilder joins)
        {
            SetarJoinsTransportador(joins);

            if (!joins.Contains(" TransportadorPaiCTe "))
                joins.Append(" left join T_EMPRESA TransportadorPaiCTe on TransportadorCTe.EMP_EMPRESA = TransportadorPaiCTe.EMP_CODIGO ");
        }

        private void SetarJoinsTransportadorConfiguracao(StringBuilder joins)
        {
            SetarJoinsTransportador(joins);

            if (!joins.Contains(" TransportadorConfiguracao "))
                joins.Append(" left join T_CONFIG TransportadorConfiguracao ON TransportadorConfiguracao.COF_CODIGO = TransportadorCTe.COF_CODIGO");
        }

        private void SetarJoinsTransportadorPaiConfiguracao(StringBuilder joins)
        {
            SetarJoinsTransportadorPai(joins);

            if (!joins.Contains(" TransportadorPaiConfiguracao "))
                joins.Append(" left join T_CONFIG TransportadorPaiConfiguracao ON TransportadorPaiConfiguracao.COF_CODIGO = TransportadorPaiCTe.COF_CODIGO");
        }

        private void SetarJoinsNavio(StringBuilder joins)
        {
            SetarJoinsViagem(joins);

            if (!joins.Contains(" Navio "))
                joins.Append(" left join T_NAVIO Navio on Navio.NAV_CODIGO = Viagem.NAV_CODIGO ");
        }

        private void SetarJoinsViagemSchedule(StringBuilder joins)
        {
            if (!joins.Contains(" ViagemSchedule "))
                joins.Append(" left join T_PEDIDO_VIAGEM_NAVIO_SCHEDULE ViagemSchedule on ViagemSchedule.PVN_CODIGO = CTe.CON_VIAGEM AND ViagemSchedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO ");
        }

        private void SetarJoinsViagemScheduleDestino(StringBuilder joins)
        {
            if (!joins.Contains(" ViagemScheduleDestino "))
                joins.Append(" left join T_PEDIDO_VIAGEM_NAVIO_SCHEDULE ViagemScheduleDestino on ViagemScheduleDestino.PVN_CODIGO = CTe.CON_VIAGEM AND ViagemScheduleDestino.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_ORIGEM ");
        }

        private void SetarJoinsPedidoViagemNavioSchedule(StringBuilder joins)
        {
            if (!joins.Contains(" PedidoViagemNavioSchedule "))
                joins.Append(" left join T_PEDIDO_VIAGEM_NAVIO_SCHEDULE PedidoViagemNavioSchedule on PedidoViagemNavioSchedule.PVS_CODIGO = CTe.PVS_CODIGO ");
        }

        private void SetarJoinBalsa(StringBuilder joins)
        {
            if (!joins.Contains(" Balsa "))
                joins.Append(" left join T_NAVIO Balsa on Balsa.NAV_CODIGO = CTe.NAV_CODIGO_BALSA ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioContainer filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("CTe.CON_CODIGO as Codigo, ");
                    }
                    break;

                case "ContainerDescricao":
                    if (!select.Contains(" ContainerDescricao, "))
                    {
                        select.Append("Container.CTR_DESCRICAO as ContainerDescricao, ");
                        groupBy.Append("Container.CTR_DESCRICAO, ");

                        SetarJoinsContainer(joins);
                    }
                    break;

                case "TipoContainer":
                    if (!select.Contains(" TipoContainer, "))
                    {
                        select.Append("ContainerTipo.CTI_DESCRICAO as TipoContainer, ");
                        groupBy.Append("ContainerTipo.CTI_DESCRICAO, ");

                        SetarJoinsContainerTipo(joins);
                    }
                    break;

                case "NumeroBooking":
                    if (!select.Contains(" NumeroBooking, "))
                    {
                        select.Append("CTe.CON_NUMERO_BOOKING as NumeroBooking, ");
                        groupBy.Append("CTe.CON_NUMERO_BOOKING, ");
                    }
                    break;

                case "NumeroOS":
                    if (!select.Contains(" NumeroOS, "))
                    {
                        select.Append("CTe.CON_NUMERO_OS as NumeroOS, ");
                        groupBy.Append("CTe.CON_NUMERO_OS, ");
                    }
                    break;

                case "NumeroControle":
                    if (!select.Contains(" NumeroControle, "))
                    {
                        select.Append("CTe.CON_NUMERO_CONTROLE as NumeroControle, ");
                        groupBy.Append("CTe.CON_NUMERO_CONTROLE, ");
                    }
                    break;

                case "NumeroCTe":
                    if (!select.Contains(" NumeroCTe, "))
                    {
                        select.Append("CTe.CON_NUM as NumeroCTe, ");
                        groupBy.Append("CTe.CON_NUM, ");
                    }
                    break;

                case "StatusCTe":
                    if (!select.Contains(" StatusCTe, "))
                    {
                        select.Append(
                            @"StatusCTe = CASE CTe.CON_STATUS 
		                        WHEN 'A' THEN 'Autorizado' 
		                        WHEN 'P' THEN 'Pendente' 
		                        WHEN 'E' THEN 'Enviado' 
		                        WHEN 'R' THEN 'Rejeitado' 
		                        WHEN 'C' THEN 'Cancelado' 
		                        WHEN 'I' THEN 'Inutilizado' 
		                        WHEN 'D' THEN 'Denegado' 
		                        WHEN 'S' THEN 'Em Digitação' 
		                        WHEN 'K' THEN 'Em Cancelamento' 
		                        WHEN 'L' THEN 'Em Inutilização' 
                                WHEN 'Z' THEN 'Anulado' 
		                        ELSE ''
                            END, "
                        );
                        groupBy.Append("CTe.CON_STATUS, ");
                    }
                    break;

                case "ValorFrete":
                    if (!select.Contains(" ValorFrete, "))
                    {
                        select.Append("CTe.CON_VALOR_FRETE as ValorFrete, ");
                        groupBy.Append("CTe.CON_VALOR_FRETE, ");
                    }
                    break;

                case "ValorICMS":
                    if (!select.Contains(" ValorICMS, "))
                    {
                        select.Append("CTe.CON_VAL_ICMS as ValorICMS, ");

                        if (!groupBy.Contains(" CTe.CON_VAL_ICMS, "))
                            groupBy.Append(" CTe.CON_VAL_ICMS, ");
                    }
                    break;

                case "Viagem":
                    if (!select.Contains(" Viagem, "))
                    {
                        select.Append("Viagem.PVN_DESCRICAO as Viagem, ");
                        groupBy.Append("Viagem.PVN_DESCRICAO, ");

                        SetarJoinsViagem(joins);
                    }
                    break;

                case "NavioTransbordo":
                    if (!select.Contains(" NavioTransbordo, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + navio.PVN_DESCRICAO
                                        from T_PEDIDO_VIAGEM_NAVIO navio
                                        inner join T_PEDIDO_TRANSBORDO pedidoTransbordo on pedidoTransbordo.PVN_CODIGO = navio.PVN_CODIGO 
                                        inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = pedidoTransbordo.PED_CODIGO 
                                        inner join T_CARGA_CTE _cargaCTe on _cargaCTe.CAR_CODIGO = CargaPedido.CAR_CODIGO 
                                 WHERE _cargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) NavioTransbordo, "
                        );
                    }
                    break;

                case "Balsa":
                    if (!select.Contains(" Balsa, "))
                    {
                        select.Append("Balsa.NAV_DESCRICAO Balsa, ");
                        groupBy.Append("Balsa.NAV_DESCRICAO, ");

                        SetarJoinBalsa(joins);
                    }
                    break;

                case "PortoOrigem":
                    if (!select.Contains(" PortoOrigem, "))
                    {
                        select.Append("PortoOrigem.POT_DESCRICAO PortoOrigem, ");
                        groupBy.Append("PortoOrigem.POT_DESCRICAO, ");

                        SetarJoinsPortoOrigem(joins);
                    }
                    break;

                case "PortoDestino":
                    if (!select.Contains(" PortoDestino, "))
                    {
                        select.Append("PortoDestino.POT_DESCRICAO PortoDestino, ");
                        groupBy.Append("PortoDestino.POT_DESCRICAO, ");

                        SetarJoinsPortoDestino(joins);
                    }
                    break;

                case "PortoTransbordo":
                    if (!select.Contains(" PortoTransbordo, "))
                    {
                        select.Append(@"concat(PortoPassagemUm.POT_DESCRICAO
                                           , CASE WHEN PortoPassagemUm.POT_CODIGO IS NOT NULL THEN ', ' + PortoPassagemDois.POT_DESCRICAO ELSE '' END
                                           , CASE WHEN PortoPassagemDois.POT_CODIGO IS NOT NULL THEN ', ' + PortoPassagemTres.POT_DESCRICAO ELSE '' END
                                           , CASE WHEN PortoPassagemTres.POT_CODIGO IS NOT NULL THEN ', ' + PortoPassagemQuatro.POT_DESCRICAO ELSE '' END
                                           , CASE WHEN PortoPassagemQuatro.POT_CODIGO IS NOT NULL THEN ', ' + PortoPassagemCinco.POT_DESCRICAO ELSE '' END
                        ) PortoTransbordo, ");

                        groupBy.Append("PortoPassagemUm.POT_CODIGO, ");
                        groupBy.Append("PortoPassagemDois.POT_CODIGO, ");
                        groupBy.Append("PortoPassagemTres.POT_CODIGO, ");
                        groupBy.Append("PortoPassagemQuatro.POT_CODIGO, ");
                        groupBy.Append("PortoPassagemCinco.POT_CODIGO, ");

                        groupBy.Append("PortoPassagemUm.POT_DESCRICAO, ");
                        groupBy.Append("PortoPassagemDois.POT_DESCRICAO, ");
                        groupBy.Append("PortoPassagemTres.POT_DESCRICAO, ");
                        groupBy.Append("PortoPassagemQuatro.POT_DESCRICAO, ");
                        groupBy.Append("PortoPassagemCinco.POT_DESCRICAO, ");

                        SetarJoinsPortoPassagemUm(joins);
                        SetarJoinsPortoPassagemDois(joins);
                        SetarJoinsPortoPassagemTres(joins);
                        SetarJoinsPortoPassagemQuatro(joins);
                        SetarJoinsPortoPassagemCinco(joins);
                    }
                    break;

                case "TerminalOrigem":
                    if (!select.Contains(" TerminalOrigem, "))
                    {
                        select.Append("TerminalOrigem.TTI_DESCRICAO TerminalOrigem, ");
                        groupBy.Append("TerminalOrigem.TTI_DESCRICAO, ");

                        SetarJoinsTerminalOrigem(joins);
                    }
                    break;

                case "TerminalDestino":
                    if (!select.Contains(" TerminalDestino, "))
                    {
                        select.Append("TerminalDestino.TTI_DESCRICAO TerminalDestino, ");
                        groupBy.Append("TerminalDestino.TTI_DESCRICAO, ");

                        SetarJoinsTerminalDestino(joins);
                    }
                    break;

                case "TerminalTransbordo":
                    if (!select.Contains(" TerminalTransbordo, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + terminal.TTI_DESCRICAO
                                        from T_TIPO_TERMINAL_IMPORTACAO terminal
                                        inner join T_PEDIDO_TRANSBORDO pedidoTransbordo on pedidoTransbordo.TTI_CODIGO = terminal.TTI_CODIGO 
                                        inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = pedidoTransbordo.PED_CODIGO 
                                        inner join T_CARGA_CTE _cargaCTe on _cargaCTe.CAR_CODIGO = CargaPedido.CAR_CODIGO 
                                 WHERE _cargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) TerminalTransbordo, "
                        );
                    }
                    break;

                case "PesoBruto":
                    if (!select.Contains(" PesoBruto, "))
                    {
                        select.Append(" (SELECT SUM(cteDocs.NFC_PESO) FROM T_CTE_DOCS cteDocs WHERE cteDocs.CON_CODIGO = CTe.CON_CODIGO) PesoBruto, ");
                    }
                    break;


                case "CNPJRemetente":
                    if (!select.Contains(" CNPJRemetente, "))
                    {
                        select.Append("Remetente.PCT_CPF_CNPJ CNPJRemetente, ");

                        if (!groupBy.Contains("Remetente.PCT_CPF_CNPJ"))
                            groupBy.Append("Remetente.PCT_CPF_CNPJ, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "Remetente":
                    if (!select.Contains(" Remetente, "))
                    {
                        select.Append("Remetente.PCT_NOME Remetente, ");
                        groupBy.Append("Remetente.PCT_NOME, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "CNPJDestinatario":
                    if (!select.Contains(" CNPJDestinatario, "))
                    {
                        select.Append("Destinatario.PCT_CPF_CNPJ CNPJDestinatario, ");

                        if (!groupBy.Contains("Destinatario.PCT_CPF_CNPJ"))
                            groupBy.Append("Destinatario.PCT_CPF_CNPJ, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "Destinatario":
                    if (!select.Contains(" Destinatario, "))
                    {
                        select.Append("Destinatario.PCT_NOME Destinatario, ");
                        groupBy.Append("Destinatario.PCT_NOME, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "CPFCNPJExpedidor":
                    if (!select.Contains(" CPFCNPJExpedidor, "))
                    {
                        select.Append("Expedidor.PCT_CPF_CNPJ CPFCNPJExpedidor, ");
                        groupBy.Append("Expedidor.PCT_CPF_CNPJ, ");

                        SetarJoinsExpedidor(joins);
                    }
                    break;

                case "Expedidor":
                    if (!select.Contains(" Expedidor, "))
                    {
                        select.Append("Expedidor.PCT_NOME Expedidor, ");
                        groupBy.Append("Expedidor.PCT_NOME, ");

                        SetarJoinsExpedidor(joins);
                    }
                    break;

                case "CNPJRecebedor":
                    if (!select.Contains(" CNPJRecebedor, "))
                    {
                        select.Append("Recebedor.PCT_CPF_CNPJ CNPJRecebedor, ");

                        if (!groupBy.Contains("Recebedor.PCT_CPF_CNPJ"))
                            groupBy.Append("Recebedor.PCT_CPF_CNPJ, ");

                        SetarJoinsRecebedor(joins);
                    }
                    break;

                case "Recebedor":
                    if (!select.Contains(" Recebedor, "))
                    {
                        select.Append("Recebedor.PCT_NOME Recebedor, ");
                        groupBy.Append("Recebedor.PCT_NOME, ");

                        SetarJoinsRecebedor(joins);
                    }
                    break;

                case "Tara":
                    if (!select.Contains(" Tara, "))
                    {
                        select.Append("Container.CTR_TARA as Tara, ");
                        groupBy.Append("Container.CTR_TARA, ");

                        SetarJoinsContainer(joins);
                    }
                    break;

                case "NumeroLacre":
                    if (!select.Contains(" NumeroLacre, "))
                    {
                        select.Append(@"(CTeContainer.CER_LACRE1
                                           + CASE WHEN RTRIM(CTeContainer.CER_LACRE1) <> '' THEN ', ' + CTeContainer.CER_LACRE2 ELSE '' END
                                           + CASE WHEN RTRIM(CTeContainer.CER_LACRE2) <> '' THEN ', ' + CTeContainer.CER_LACRE3 ELSE '' END
                                        ) NumeroLacre, ");

                        groupBy.Append("CTeContainer.CER_LACRE1, ");
                        groupBy.Append("CTeContainer.CER_LACRE2, ");
                        groupBy.Append("CTeContainer.CER_LACRE3, ");

                        SetarJoinsCTeContainer(joins);
                    }
                    break;

                case "NumeroNota":
                    if (!select.Contains("NumeroNota"))
                    {
                        select.Append("substring((select ', ' + cteDocs.NFC_NUMERO from T_CTE_DOCS cteDocs where cteDocs.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) NumeroNota, ");
                    }
                    break;

                case "QuantidadeNota":
                    if (!select.Contains(" QuantidadeNota, "))
                    {
                        select.Append(
                            @"  (SELECT COUNT (1) from T_CTE_DOCS cteDocs 
                                 WHERE cteDocs.CON_CODIGO = CTe.CON_CODIGO) QuantidadeNota, "
                        );
                    }
                    break;

                case "PossuiNotas":
                    if (!select.Contains(" PossuiNotas, "))
                    {
                        select.Append(
                            @" CASE WHEN (SELECT COUNT (1) from T_CTE_DOCS cteDocs 
                                 WHERE cteDocs.CON_CODIGO = CTe.CON_CODIGO) > 0 THEN 'Sim' ELSE 'Não' 
                                END PossuiNotas, "
                        );
                    }
                    break;

                case "ValorNotas":
                    if (!select.Contains(" ValorNotas, "))
                    {
                        select.Append(" (SELECT SUM(cteDocs.NFC_VALOR) FROM T_CTE_DOCS cteDocs WHERE cteDocs.CON_CODIGO = CTe.CON_CODIGO) ValorNotas, ");
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        select.Append(@"substring((select distinct ', ' + TipoOperacao.TOP_DESCRICAO from T_CARGA_CTE CargaCTe 
                                        inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO_ORIGEM 
                                        inner join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO 
                                        where CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) TipoOperacao, ");
                    }
                    break;

                case "InicioPrestacao":
                    if (!select.Contains(" InicioPrestacao, "))
                    {
                        select.Append("InicioPrestacaoCTe.LOC_DESCRICAO InicioPrestacao, ");
                        groupBy.Append("InicioPrestacaoCTe.LOC_DESCRICAO, ");

                        SetarJoinsLocalidadeInicioPrestacao(joins);
                    }
                    break;

                case "UFInicioPrestacao":
                    if (!select.Contains(" UFInicioPrestacao, "))
                    {
                        select.Append("InicioPrestacaoCTe.UF_SIGLA UFInicioPrestacao, ");
                        groupBy.Append("InicioPrestacaoCTe.UF_SIGLA, ");

                        SetarJoinsLocalidadeInicioPrestacao(joins);
                    }
                    break;

                case "FimPrestacao":
                    if (!select.Contains(" FimPrestacao, "))
                    {
                        select.Append("FimPrestacaoCTe.LOC_DESCRICAO FimPrestacao, ");
                        groupBy.Append("FimPrestacaoCTe.LOC_DESCRICAO, ");

                        SetarJoinsLocalidadeFimPrestacao(joins);
                    }
                    break;

                case "UFFimPrestacao":
                    if (!select.Contains(" UFFimPrestacao, "))
                    {
                        select.Append("FimPrestacaoCTe.UF_SIGLA UFFimPrestacao, ");
                        groupBy.Append("FimPrestacaoCTe.UF_SIGLA, ");

                        SetarJoinsLocalidadeFimPrestacao(joins);
                    }
                    break;

                case "CargaIMO":
                    if (!select.Contains(" CargaIMO, "))
                    {
                        select.Append(
                        @"(
                        CASE 
                            WHEN (SELECT COUNT(Pedido.PED_CODIGO)
                                FROM T_CARGA_CTE CargaCTe 
                                    INNER JOIN T_CARGA_PEDIDO CargaPedido ON CargaPedido.CAR_CODIGO = CargaCTe.CAR_CODIGO 
                                    INNER JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO 
                                WHERE Pedido.PED_POSSUI_CARGA_PERIGOSA = 1 AND CargaCTe.CON_CODIGO = CTe.CON_CODIGO) > 0 THEN 'Sim' 
                            ELSE 'Não'
                        END) CargaIMO, "
                        );
                    }
                    break;

                case "TipoProposta":
                    if (!select.Contains(" TipoProposta, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + case 
                                                            WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 1 THEN 'Carga Fechada'
                                                            WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 2 THEN 'Carga Fracionada'
                                                            WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 3 THEN 'Feeder'
                                                            WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 4 THEN 'VAS'
                                                            WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 5 THEN 'Embarque Certo - Feeder'
                                                            WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 6 THEN 'Embarque Certo - Cabotagem'
                                                            WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 7 THEN 'No Show - Cabotagem'
                                                            WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 8 THEN 'Faturamento - Contabilidade'
                                                            WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 9 THEN 'Demurrage - Cabotagem'
                                                            WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 10 THEN 'Detention - Cabotagem'
                                                       else '' end 
                                        from T_CARGA_PEDIDO cargaPedido 
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = cargaPedido.CAR_CODIGO 
                                 WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) TipoProposta, "
                        );
                    }
                    break;

                case "NumeroProposta":
                    if (!select.Contains(" NumeroProposta, "))
                    {
                        //select.Append(
                        //    @"SUBSTRING((
                        //        SELECT DISTINCT ', ' + Pedido.PED_CODIGO_PROPOSTA
                        //                from T_PEDIDO Pedido 
                        //                inner join T_CARGA_PEDIDO cargaPedido on cargaPedido.PED_CODIGO = Pedido.PED_CODIGO 
                        //                inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = cargaPedido.CAR_CODIGO 
                        //         WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) NumeroProposta, "
                        //);

                        select.Append("PedidoCargaPedidoXMLNotaFiscal.PED_CODIGO_PROPOSTA NumeroProposta, ");

                        SetarJoinsCargaCTe(joins);
                        SetarJoinsCargaCTePedidoXML(joins);
                        SetarJoinsPedidoXMLNotaFiscal(joins);
                        SetarJoinsCargaPedidoXMLNotaFiscal(joins);
                        SetarJoinsPedidoCargaPedidoXMLNotaFiscal(joins);

                        groupBy.Append("PedidoCargaPedidoXMLNotaFiscal.PED_CODIGO_PROPOSTA, ");
                    }
                    break;

                case "SituacaoCargaFormatada":
                    if (!select.Contains(" SituacaoCarga, "))
                    {
                        select.Append(
                            @"substring((
                                select distinct ', ' + cast(_carga.CAR_SITUACAO as varchar(10))
                                  from T_CARGA_CTE _cargaCTe 
                                  join T_CARGA _carga on _carga.CAR_CODIGO = _cargaCTe.CAR_CODIGO_ORIGEM 
                                 where _cargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                   for xml path('')), 3, 200
                            ) SituacaoCarga, "
                        );
                    }
                    break;

                case "CPFCNPJTomador":
                    if (!select.Contains(" CPFCNPJTomador, "))
                    {
                        select.Append("TomadorPagadorCTe.PCT_CPF_CNPJ CPFCNPJTomador, ");
                        groupBy.Append("TomadorPagadorCTe.PCT_CPF_CNPJ, ");

                        SetarJoinsTomador(joins);
                    }
                    break;

                case "Tomador":
                    if (!select.Contains(" Tomador, "))
                    {
                        select.Append("TomadorPagadorCTe.PCT_NOME Tomador, ");
                        groupBy.Append("TomadorPagadorCTe.PCT_NOME, ");

                        SetarJoinsTomador(joins);
                    }
                    break;

                case "SerieCTe":
                    if (!select.Contains(" SerieCTe, "))
                    {
                        select.Append("Serie.ESE_NUMERO SerieCTe, ");
                        groupBy.Append("Serie.ESE_NUMERO, ");

                        SetarJoinsSerie(joins);
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga, "))
                    {
                        select.Append(@"substring((select distinct ', ' + Carga.CAR_CODIGO_CARGA_EMBARCADOR from T_CARGA_CTE CargaCTe 
                                                    inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO_ORIGEM 
                                                    where CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) NumeroCarga, ");
                    }
                    break;

                case "AbreviacaoModeloDocumentoFiscal":
                    if (!select.Contains(" AbreviacaoModeloDocumentoFiscal, "))
                    {
                        select.Append("ModeloDocumento.MOD_ABREVIACAO AbreviacaoModeloDocumentoFiscal, ");
                        groupBy.Append("ModeloDocumento.MOD_ABREVIACAO, ");

                        SetarJoinsModeloDocumento(joins);
                    }
                    break;

                case "DescricaoTipoCTe":
                    if (!select.Contains(" TipoCTe, "))
                    {
                        select.Append("CTe.CON_TIPO_CTE TipoCTe, ");
                        groupBy.Append("CTe.CON_TIPO_CTE, ");
                    }
                    break;

                case "DescricaoTipoServico":
                    if (!select.Contains("TipoServico"))
                    {
                        select.Append("CTe.CON_TIPO_SERVICO TipoServico, ");
                        groupBy.Append("CTe.CON_TIPO_SERVICO, ");
                    }
                    break;

                case "DataEmissaoFormatada":
                    if (!select.Contains(" DataEmissao, "))
                    {
                        select.Append("CTe.CON_DATAHORAEMISSAO DataEmissao, ");
                        groupBy.Append("CTe.CON_DATAHORAEMISSAO, ");
                    }
                    break;

                case "DataAutorizacaoFormatada":
                    if (!select.Contains(" DataAutorizacao, "))
                    {
                        select.Append("CTe.CON_DATA_AUTORIZACAO DataAutorizacao, ");
                        groupBy.Append("CTe.CON_DATA_AUTORIZACAO, ");
                    }
                    break;

                case "DataVencimento":
                    if (!select.Contains(" DataVencimento, "))
                    {
                        select.Append(@"substring((select distinct ', ' + Convert(nvarchar(10), Titulo.TIT_DATA_VENCIMENTO, 103) from T_TITULO_DOCUMENTO TituloDocumento 
                                                    inner join T_TITULO Titulo on TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO 
                                                    where Titulo.TIT_STATUS <> 4 and TituloDocumento.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) DataVencimento, ");
                    }
                    break;

                case "DataEntrega":
                    if (!select.Contains(" DataEntrega, "))
                    {
                        select.Append("CONVERT(NVARCHAR(10), CTe.CON_DATA_ENTREGA, 103) + ' ' + CONVERT(NVARCHAR(5), CTe.CON_DATA_ENTREGA, 108) DataEntrega, ");
                        groupBy.Append("CTe.CON_DATA_ENTREGA, ");
                    }
                    break;

                case "CodigoInicioPrestacao":
                    if (!select.Contains(" CodigoInicioPrestacao, "))
                    {
                        select.Append("InicioPrestacaoCTe.LOC_CODIGO_DOCUMENTO CodigoInicioPrestacao, ");
                        groupBy.Append("InicioPrestacaoCTe.LOC_CODIGO_DOCUMENTO, ");

                        SetarJoinsLocalidadeInicioPrestacao(joins);
                    }
                    break;

                case "CodigoFimPrestacao":
                    if (!select.Contains(" CodigoFimPrestacao, "))
                    {
                        select.Append("FimPrestacaoCTe.LOC_CODIGO_DOCUMENTO CodigoFimPrestacao, ");
                        groupBy.Append("FimPrestacaoCTe.LOC_CODIGO_DOCUMENTO, ");

                        SetarJoinsLocalidadeFimPrestacao(joins);
                    }
                    break;

                case "CFOP":
                    if (!select.Contains(" CFOP, "))
                    {
                        select.Append("CFOP.CFO_CFOP CFOP, ");
                        groupBy.Append("CFOP.CFO_CFOP, ");

                        SetarJoinsCfop(joins);
                    }
                    break;

                case "CST":
                    if (!select.Contains(" CST,"))
                    {
                        select.Append(
                            @"(CASE CTe.CON_CST
                                WHEN '' THEN 'Simples Nacional'
                                WHEN '91' THEN '90' 
                                ELSE CTe.CON_CST
                            END) CST, "
                        );
                        groupBy.Append("CTe.CON_CST, ");
                    }
                    break;

                case "AliquotaICMS":
                    if (!somenteContarNumeroRegistros && !select.Contains("AliquotaICMS"))
                    {
                        select.Append("CTe.CON_ALIQ_ICMS AliquotaICMS, ");
                        groupBy.Append("CTe.CON_ALIQ_ICMS, ");
                    }
                    break;

                case "BaseCalculoICMS":
                    if (!somenteContarNumeroRegistros && !select.Contains("BaseCalculoICMS"))
                    {
                        select.Append("CTe.CON_BC_ICMS BaseCalculoICMS, ");
                        groupBy.Append("CTe.CON_BC_ICMS, ");
                    }
                    break;

                case "AliquotaISS":
                    if (!somenteContarNumeroRegistros && !select.Contains("AliquotaISS"))
                    {
                        select.Append("CTe.CON_ALIQUOTA_ISS AliquotaISS, ");
                        groupBy.Append("CTe.CON_ALIQUOTA_ISS, ");
                    }
                    break;

                case "ValorISS":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorISS"))
                    {
                        select.Append("CTe.CON_VALOR_ISS ValorISS, ");

                        if (!groupBy.Contains(" CTe.CON_VALOR_ISS, "))
                            groupBy.Append(" CTe.CON_VALOR_ISS, ");
                    }
                    break;

                case "ValorISSRetido":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorISSRetido"))
                    {
                        select.Append("CTe.CON_VALOR_ISS_RETIDO ValorISSRetido, ");
                        groupBy.Append("CTe.CON_VALOR_ISS_RETIDO, ");
                    }
                    break;

                case "ValorSemImposto":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorSemImposto"))
                    {
                        select.Append("(CTe.CON_VALOR_PREST_SERVICO - CTe.CON_VAL_ICMS - CTe.CON_VALOR_ISS) ValorSemImposto, ");

                        if (!groupBy.Contains(" CTe.CON_VALOR_PREST_SERVICO, "))
                            groupBy.Append(" CTe.CON_VALOR_PREST_SERVICO, ");
                        if (!groupBy.Contains(" CTe.CON_VAL_ICMS, "))
                            groupBy.Append(" CTe.CON_VAL_ICMS, ");
                        if (!groupBy.Contains(" CTe.CON_VALOR_ISS, "))
                            groupBy.Append(" CTe.CON_VALOR_ISS, ");
                    }
                    break;

                case "ValorReceber":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorReceber"))
                    {
                        select.Append("CTe.CON_VALOR_RECEBER ValorReceber, ");
                        groupBy.Append("CTe.CON_VALOR_RECEBER, ");
                    }
                    break;

                case "ValorPrestacao":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorPrestacao"))
                    {
                        select.Append("CTe.CON_VALOR_PREST_SERVICO ValorPrestacao, ");

                        if (!groupBy.Contains(" CTe.CON_VALOR_PREST_SERVICO, "))
                            groupBy.Append(" CTe.CON_VALOR_PREST_SERVICO, ");
                    }
                    break;

                case "AliquotaCOFINS":
                    if (!select.Contains(" AliquotaCOFINS, "))
                    {
                        select.Append("coalesce(TransportadorConfiguracao.COF_ALIQUOTA_COFINS, TransportadorPaiConfiguracao.COF_ALIQUOTA_COFINS, 0) AliquotaCOFINS, ");

                        groupBy.Append("TransportadorConfiguracao.COF_ALIQUOTA_COFINS, ");
                        groupBy.Append("TransportadorPaiConfiguracao.COF_ALIQUOTA_COFINS, ");

                        SetarJoinsTransportadorConfiguracao(joins);
                        SetarJoinsTransportadorPaiConfiguracao(joins);
                    }
                    break;

                case "AliquotaPIS":
                    if (!select.Contains(" AliquotaPIS, "))
                    {
                        select.Append("coalesce(TransportadorConfiguracao.COF_ALIQUOTA_PIS, TransportadorPaiConfiguracao.COF_ALIQUOTA_PIS, 0) AliquotaPIS, ");

                        groupBy.Append("TransportadorConfiguracao.COF_ALIQUOTA_PIS, ");
                        groupBy.Append("TransportadorPaiConfiguracao.COF_ALIQUOTA_PIS, ");

                        SetarJoinsTransportadorConfiguracao(joins);
                        SetarJoinsTransportadorPaiConfiguracao(joins);
                    }
                    break;

                case "DataColeta":
                    if (!select.Contains(" DataColeta, "))
                    {
                        select.Append(@"substring((select distinct ', ' + CONVERT(NVARCHAR(10), Pedido.CAR_DATA_CARREGAMENTO_PEDIDO, 103) + ' ' + CONVERT(NVARCHAR(5), Pedido.CAR_DATA_CARREGAMENTO_PEDIDO, 108) 
                                                    from T_CARGA_CTE CargaCTe 
                                                    inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                                    inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                                                    inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO 
                                                    inner join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO 
                                                    where CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) DataColeta, ");
                    }
                    break;

                case "DataPrevistaEntrega":
                    if (!select.Contains(" DataPrevistaEntrega, "))
                    {
                        select.Append("CONVERT(nvarchar(10), CTe.CON_DATAPREVISTAENTREGA, 103) + ' ' + CONVERT(nvarchar(5), CTe.CON_DATAPREVISTAENTREGA, 108) DataPrevistaEntrega, ");
                        groupBy.Append("CTe.CON_DATAPREVISTAENTREGA, ");
                    }
                    break;

                case "CodigoPortoOrigem":
                    if (!select.Contains(" CodigoPortoOrigem, "))
                    {
                        select.Append("PortoOrigem.POT_CODIGO_DOCUMENTO CodigoPortoOrigem, ");
                        groupBy.Append("PortoOrigem.POT_CODIGO_DOCUMENTO, ");

                        SetarJoinsPortoOrigem(joins);
                    }
                    break;

                case "CodigoPortoDestino":
                    if (!select.Contains(" CodigoPortoDestino, "))
                    {
                        select.Append("PortoDestino.POT_CODIGO_DOCUMENTO CodigoPortoDestino, ");
                        groupBy.Append("PortoDestino.POT_CODIGO_DOCUMENTO, ");

                        SetarJoinsPortoDestino(joins);
                    }
                    break;

                case "NumeroContainer":
                    if (!select.Contains(" NumeroContainer, "))
                    {
                        select.Append("Container.CTR_NUMERO as NumeroContainer, ");
                        groupBy.Append("Container.CTR_NUMERO, ");

                        SetarJoinsContainer(joins);
                    }
                    break;

                case "CodigoNavio":
                    if (!select.Contains(" CodigoNavio, "))
                    {
                        select.Append("Navio.NAV_CODIGO_DOCUMENTO as CodigoNavio, ");
                        groupBy.Append("Navio.NAV_CODIGO_DOCUMENTO, ");

                        SetarJoinsNavio(joins);
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select.Append("TransportadorCTe.EMP_FANTASIA Transportador, ");
                        groupBy.Append("TransportadorCTe.EMP_FANTASIA, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "CNPJTransportador":
                    if (!select.Contains(" CNPJTransportador, "))
                    {
                        select.Append("TransportadorCTe.EMP_CNPJ CNPJTransportador, ");
                        groupBy.Append("TransportadorCTe.EMP_CNPJ, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "Taxa":
                    if (!select.Contains(" Taxa, "))
                    {
                        //select.Append(@"(SELECT MAX(P.PED_VALOR_TAXA_FEEDER)
                        //                    FROM T_PEDIDO P
                        //                    JOIN T_CARGA_PEDIDO CP ON CP.PED_CODIGO = P.PED_CODIGO
                        //                    JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CAR_CODIGO = CP.CAR_CODIGO
                        //                    WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO) Taxa, ");

                        select.Append("PedidoCargaPedidoXMLNotaFiscal.PED_VALOR_TAXA_FEEDER Taxa, ");

                        SetarJoinsCargaCTe(joins);
                        SetarJoinsCargaCTePedidoXML(joins);
                        SetarJoinsPedidoXMLNotaFiscal(joins);
                        SetarJoinsCargaPedidoXMLNotaFiscal(joins);
                        SetarJoinsPedidoCargaPedidoXMLNotaFiscal(joins);

                        groupBy.Append("PedidoCargaPedidoXMLNotaFiscal.PED_VALOR_TAXA_FEEDER, ");
                    }
                    break;

                case "ETA":
                    if (!select.Contains(" ETA, "))
                    {
                        select.Append("CONVERT(NVARCHAR(10), ViagemSchedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO, 103) + ' ' + CONVERT(NVARCHAR(5), ViagemSchedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO, 108) ETA, ");
                        groupBy.Append("ViagemSchedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO, ");

                        SetarJoinsViagemSchedule(joins);
                    }
                    break;

                case "ETS":
                    if (!select.Contains(" ETS, "))
                    {
                        select.Append("CONVERT(NVARCHAR(10), ViagemScheduleDestino.PVS_DATA_PREVISAO_SAIDA_NAVIO, 103) + ' ' + CONVERT(NVARCHAR(5), ViagemScheduleDestino.PVS_DATA_PREVISAO_SAIDA_NAVIO, 108) ETS, ");
                        groupBy.Append("ViagemScheduleDestino.PVS_DATA_PREVISAO_SAIDA_NAVIO, ");

                        SetarJoinsViagemScheduleDestino(joins);
                    }
                    break;

                case "ETATransbordo1":
                    if (!select.Contains(" ETATransbordo1, "))
                    {
                        select.Append(@"(SELECT TOP (1) CONVERT(NVARCHAR(10), ViagemScheduleTransbordo.PVS_DATA_PREVISAO_CHEGADA_NAVIO, 103) + ' ' + CONVERT(NVARCHAR(5), ViagemScheduleTransbordo.PVS_DATA_PREVISAO_CHEGADA_NAVIO, 108)
                                           FROM T_PEDIDO_VIAGEM_NAVIO navio
                                           INNER JOIN T_PEDIDO_TRANSBORDO pedidoTransbordo ON pedidoTransbordo.PVN_CODIGO = navio.PVN_CODIGO
                                           INNER JOIN T_CARGA_PEDIDO CargaPedido ON CargaPedido.PED_CODIGO = pedidoTransbordo.PED_CODIGO
                                           INNER JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CAR_CODIGO = CargaPedido.CAR_CODIGO
                                           JOIN T_PEDIDO_VIAGEM_NAVIO_SCHEDULE ViagemScheduleTransbordo ON navio.PVN_CODIGO = ViagemScheduleTransbordo.PVN_CODIGO
                                           WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO AND ViagemScheduleTransbordo.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_DESTINO) ETATransbordo1, ");

                        groupBy.Append("CTe.POT_CODIGO_DESTINO, ");
                    }
                    break;

                case "ChaveCTe":
                    if (!select.Contains(" ChaveCTe, "))
                    {
                        select.Append("CTe.CON_CHAVECTE ChaveCTe, ");
                        groupBy.Append("CTe.CON_CHAVECTE, ");
                    }
                    break;

                case "ChaveCTeMultimodal":
                    if (!select.Contains(" ChaveCTeMultimodal, "))
                    {
                        select.Append(@"(SUBSTRING((
                                SELECT DISTINCT ', ' + cte.CON_CHAVECTE
                                        from T_CTE cte
                                        inner join T_CTE_DOCS cteDocs on cteDocs.CON_CODIGO = cte.CON_CODIGO 
                                        inner join T_CTE_CONTAINER_DOCUMENTO containerDocumento on containerDocumento.CCD_CHAVE = cteDocs.NFC_CHAVENFE 
                                 WHERE cte.CON_TIPO_MODAL = 6 and cte.CON_STATUS = 'A' and containerDocumento.CCD_TIPO_DOCUMENTO = 0
                                    and containerDocumento.CER_CODIGO = CTeContainer.CER_CODIGO for xml path('')), 3, 1000)) ChaveCTeMultimodal, "
                        );

                        if (!groupBy.Contains("CTeContainer.CER_CODIGO,"))
                            groupBy.Append("CTeContainer.CER_CODIGO, ");

                        SetarJoinsCTeContainer(joins);
                    }
                    break;

                case "ChaveCTeSVM":
                    if (!select.Contains(" ChaveCTeSVM, "))
                    {
                        select.Append(@"(SUBSTRING((
                                SELECT DISTINCT ', ' + cte.CON_CHAVECTE
                                        from T_CTE cte
                                        inner join T_CTE_DOCS cteDocs on cteDocs.CON_CODIGO = cte.CON_CODIGO 
                                        inner join T_CTE_CONTAINER_DOCUMENTO containerDocumento on containerDocumento.CCD_CHAVE = cteDocs.NFC_CHAVENFE 
                                 WHERE cte.CON_TIPO_SERVICO = 4 and cte.CON_STATUS = 'A' and containerDocumento.CCD_TIPO_DOCUMENTO = 0
                                    and containerDocumento.CER_CODIGO = CTeContainer.CER_CODIGO for xml path('')), 3, 1000)) ChaveCTeSVM, "
                        );

                        if (!groupBy.Contains("CTeContainer.CER_CODIGO,"))
                            groupBy.Append("CTeContainer.CER_CODIGO, ");

                        SetarJoinsCTeContainer(joins);
                    }
                    break;

                case "CTeAnulado":
                    if (!select.Contains(" CTeAnulado, "))
                    {
                        select.Append("CASE WHEN CTe.CON_POSSUI_ANULACAO_SUBSTITUICAO = 1 OR CTe.CON_STATUS = 'Z' THEN 'Sim' ELSE 'Não' END CTeAnulado, ");
                        groupBy.Append("CTe.CON_POSSUI_ANULACAO_SUBSTITUICAO, CTe.CON_STATUS, ");
                    }
                    break;

                case "SomenteCTeSubstituido":
                    if (!select.Contains(" SomenteCTeSubstituido, "))
                    {
                        select.Append(@"CASE WHEN (select count(1) from t_cte _cte where _cte.CON_CHAVE_CTE_SUB_COMP = CTe.CON_CHAVECTE) > 0 THEN 'Sim' 
                                        ELSE 'Não' END SomenteCTeSubstituido, ");

                        if (!groupBy.Contains("CTe.CON_CHAVECTE"))
                            groupBy.Append(" CTe.CON_CHAVECTE, ");
                    }
                    break;

                case "NumeroManifesto":
                    if (!select.Contains(" NumeroManifesto, "))
                    {
                        select.Append("CTe.CON_NUMERO_MANIFESTO NumeroManifesto, ");
                        groupBy.Append("CTe.CON_NUMERO_MANIFESTO, ");
                    }
                    break;

                case "NumeroCEMercante":
                    if (!select.Contains(" NumeroCEMercante, "))
                    {
                        select.Append("CTe.CON_NUMERO_CE_MERCANTE NumeroCEMercante, ");
                        groupBy.Append("CTe.CON_NUMERO_CE_MERCANTE, ");
                    }
                    break;

                case "NumeroManifestoFEEDER":
                    if (!select.Contains(" NumeroManifestoFEEDER, "))
                    {
                        //select.Append(
                        //    @"SUBSTRING((
                        //        SELECT DISTINCT ', ' + pedido.PED_NUMERO_MANIFESTO_FEEDER 
                        //                FROM T_PEDIDO pedido
                        //                inner join T_CARGA_PEDIDO cargaPedido on cargaPedido.PED_CODIGO = pedido.PED_CODIGO 
                        //                inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = cargaPedido.CAR_CODIGO 
                        //         WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) NumeroManifestoFEEDER, "
                        //);

                        select.Append("PedidoCargaPedidoXMLNotaFiscal.PED_NUMERO_MANIFESTO_FEEDER NumeroManifestoFEEDER, ");

                        SetarJoinsCargaCTe(joins);
                        SetarJoinsCargaCTePedidoXML(joins);
                        SetarJoinsPedidoXMLNotaFiscal(joins);
                        SetarJoinsCargaPedidoXMLNotaFiscal(joins);
                        SetarJoinsPedidoCargaPedidoXMLNotaFiscal(joins);

                        groupBy.Append("PedidoCargaPedidoXMLNotaFiscal.PED_NUMERO_MANIFESTO_FEEDER, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroCEFEEDER":
                    if (!select.Contains(" NumeroCEFEEDER, "))
                    {
                        //select.Append(
                        //    @"SUBSTRING((
                        //        SELECT DISTINCT ', ' + pedido.PED_NUMERO_CE_FEEDER 
                        //                FROM T_PEDIDO pedido
                        //                inner join T_CARGA_PEDIDO cargaPedido on cargaPedido.PED_CODIGO = pedido.PED_CODIGO 
                        //                inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = cargaPedido.CAR_CODIGO 
                        //         WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) NumeroCEFEEDER, "
                        //);

                        select.Append("PedidoCargaPedidoXMLNotaFiscal.PED_NUMERO_CE_FEEDER NumeroCEFEEDER, ");

                        SetarJoinsCargaCTe(joins);
                        SetarJoinsCargaCTePedidoXML(joins);
                        SetarJoinsPedidoXMLNotaFiscal(joins);
                        SetarJoinsCargaPedidoXMLNotaFiscal(joins);
                        SetarJoinsPedidoCargaPedidoXMLNotaFiscal(joins);

                        groupBy.Append("PedidoCargaPedidoXMLNotaFiscal.PED_NUMERO_CE_FEEDER, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "Commodity":
                    if (!select.Contains(" Commodity, "))
                    {
                        select.Append("CTe.CON_PRODUTO_PRED Commodity, ");

                        if (!groupBy.Contains("CTe.CON_PRODUTO_PRED"))
                            groupBy.Append("CTe.CON_PRODUTO_PRED, ");
                    }
                    break;

                case "AfretamentoDescricao":
                    if (!select.Contains(" Afretamento, "))
                    {
                        //select.Append(@"ISNULL((SELECT TOP(1) Pedido.PED_EMBARQUE_AFRETAMENTO_FEEDER FROM T_PEDIDO Pedido 
                        //                    join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO 
                        //                    join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = CargaPedido.CAR_CODIGO 
                        //                    where CargaCTe.CON_CODIGO = CTe.CON_CODIGO), 0) Afretamento, ");

                        select.Append("PedidoCargaPedidoXMLNotaFiscal.PED_EMBARQUE_AFRETAMENTO_FEEDER Afretamento, ");

                        SetarJoinsCargaCTe(joins);
                        SetarJoinsCargaCTePedidoXML(joins);
                        SetarJoinsPedidoXMLNotaFiscal(joins);
                        SetarJoinsCargaPedidoXMLNotaFiscal(joins);
                        SetarJoinsPedidoCargaPedidoXMLNotaFiscal(joins);

                        groupBy.Append("PedidoCargaPedidoXMLNotaFiscal.PED_EMBARQUE_AFRETAMENTO_FEEDER, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroProtocoloANTAQ":
                    if (!select.Contains(" NumeroProtocoloANTAQ, "))
                    {
                        //select.Append(
                        //    @"SUBSTRING((
                        //        SELECT DISTINCT ', ' + pedido.PED_PROTOCOLO_ANTAQ_FEEDER 
                        //                FROM T_PEDIDO pedido
                        //                inner join T_CARGA_PEDIDO cargaPedido on cargaPedido.PED_CODIGO = pedido.PED_CODIGO 
                        //                inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = cargaPedido.CAR_CODIGO 
                        //         WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) NumeroProtocoloANTAQ, "
                        //);

                        select.Append("PedidoCargaPedidoXMLNotaFiscal.PED_PROTOCOLO_ANTAQ_FEEDER NumeroProtocoloANTAQ, ");

                        SetarJoinsContainer(joins);
                        SetarJoinsCargaCTe(joins);
                        SetarJoinsCargaCTePedidoXML(joins);
                        SetarJoinsPedidoXMLNotaFiscal(joins);
                        SetarJoinsCargaPedidoXMLNotaFiscal(joins);
                        SetarJoinsPedidoCargaPedidoXMLNotaFiscal(joins);

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");

                        groupBy.Append("PedidoCargaPedidoXMLNotaFiscal.PED_PROTOCOLO_ANTAQ_FEEDER, ");
                    }
                    break;

                case "FFE":
                    if (!select.Contains(" FFE, "))
                    {
                        select.Append("ContainerTipo.CTI_FFE as FFE, ");
                        groupBy.Append("ContainerTipo.CTI_FFE, ");

                        SetarJoinsContainerTipo(joins);
                    }
                    break;

                case "TEU":
                    if (!select.Contains(" TEU, "))
                    {
                        select.Append("ContainerTipo.CTI_TEU as TEU, ");
                        groupBy.Append("ContainerTipo.CTI_TEU, ");

                        SetarJoinsContainerTipo(joins);
                    }
                    break;

                case "NumeroCTeAnulacao":
                    if (!select.Contains(" NumeroCTeAnulacao, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CAST(CTeGerado.CON_NUM AS NVARCHAR(20))
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 1 AND CTeRelacao.CON_CODIGO_ORIGINAL  = CTe.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroCTeAnulacao, ");

                    if (!groupBy.Contains("CTe.CON_CODIGO"))
                        groupBy.Append("CTe.CON_CODIGO, ");
                    break;

                case "NumeroControleCTeAnulacao":
                    if (!select.Contains(" NumeroControleCTeAnulacao, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CTeGerado.CON_NUMERO_CONTROLE
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 1 AND CTeRelacao.CON_CODIGO_ORIGINAL = CTe.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroControleCTeAnulacao, ");

                    if (!groupBy.Contains("CTe.CON_CODIGO"))
                        groupBy.Append("CTe.CON_CODIGO, ");
                    break;

                case "NumeroCTeComplementar":
                    if (!select.Contains(" NumeroCTeComplementar, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CAST(CTeGerado.CON_NUM AS NVARCHAR(20))
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 2 AND CTeRelacao.CON_CODIGO_ORIGINAL = CTe.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroCTeComplementar, ");

                    if (!groupBy.Contains("CTe.CON_CODIGO"))
                        groupBy.Append("CTe.CON_CODIGO, ");
                    break;

                case "NumeroControleCTeComplementar":
                    if (!select.Contains(" NumeroControleCTeComplementar, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CTeGerado.CON_NUMERO_CONTROLE
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 2 AND CTeRelacao.CON_CODIGO_ORIGINAL  = CTe.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroControleCTeComplementar, ");

                    if (!groupBy.Contains("CTe.CON_CODIGO"))
                        groupBy.Append("CTe.CON_CODIGO, ");
                    break;

                case "NumeroCTeSubstituto":
                    if (!select.Contains(" NumeroCTeSubstituto, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CAST(CTeGerado.CON_NUM AS NVARCHAR(20))
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 3 AND CTeRelacao.CON_CODIGO_ORIGINAL  = CTe.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroCTeSubstituto, ");

                    if (!groupBy.Contains("CTe.CON_CODIGO"))
                        groupBy.Append("CTe.CON_CODIGO, ");
                    break;

                case "NumeroControleCTeSubstituto":
                    if (!select.Contains(" NumeroControleCTeSubstituto, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CTeGerado.CON_NUMERO_CONTROLE
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 3 AND CTeRelacao.CON_CODIGO_ORIGINAL  = CTe.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroControleCTeSubstituto, ");

                    if (!groupBy.Contains("CTe.CON_CODIGO"))
                        groupBy.Append("CTe.CON_CODIGO, ");
                    break;

                case "NumeroCTeDuplicado":
                    if (!select.Contains(" NumeroCTeDuplicado, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CAST(CTeGerado.CON_NUM AS NVARCHAR(20))
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 4 AND CTeRelacao.CON_CODIGO_ORIGINAL  = CTe.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroCTeDuplicado, ");

                    if (!groupBy.Contains("CTe.CON_CODIGO"))
                        groupBy.Append("CTe.CON_CODIGO, ");
                    break;

                case "NumeroControleCTeDuplicado":
                    if (!select.Contains(" NumeroControleCTeDuplicado, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CTeGerado.CON_NUMERO_CONTROLE
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 4 AND CTeRelacao.CON_CODIGO_ORIGINAL  = CTe.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroControleCTeDuplicado, ");

                    if (!groupBy.Contains("CTe.CON_CODIGO"))
                        groupBy.Append("CTe.CON_CODIGO, ");
                    break;

                case "NumeroCTeOriginal":
                    if (!select.Contains(" NumeroCTeOriginal, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CAST(CTeOriginal.CON_NUM AS NVARCHAR(20))
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeOriginal on CTeOriginal.CON_CODIGO = CTeRelacao.CON_CODIGO_ORIGINAL 
                                WHERE CTeRelacao.CON_CODIGO_GERADO  = CTe.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroCTeOriginal, ");

                    if (!groupBy.Contains("CTe.CON_CODIGO"))
                        groupBy.Append("CTe.CON_CODIGO, ");
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                        select.Append("CTe.CON_OBSGERAIS Observacao, ");

                    if (!groupBy.Contains("CTe.CON_OBSGERAIS"))
                        groupBy.Append("CTe.CON_OBSGERAIS, ");
                    break;

                case "NumeroControleCTeOriginal":
                    if (!select.Contains(" NumeroControleCTeOriginal, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CTeOriginal.CON_NUMERO_CONTROLE
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeOriginal on CTeOriginal.CON_CODIGO = CTeRelacao.CON_CODIGO_ORIGINAL 
                                WHERE CTeRelacao.CON_CODIGO_GERADO  = CTe.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroControleCTeOriginal, ");

                    if (!groupBy.Contains("CTe.CON_CODIGO"))
                        groupBy.Append("CTe.CON_CODIGO, ");
                    break;

                case "ValorSemTributo":
                    if (!somenteContarNumeroRegistros && !select.Contains(" ValorSemTributo, "))
                        select.Append("SUM(CTe.CON_VALOR_PREST_SERVICO - CTe.CON_VAL_ICMS - CTe.CON_VALOR_ISS - CON_VALOR_ICMS_UF_DESTINO - CON_VALOR_ICMS_FCP_DESTINO) ValorSemTributo, ");
                    break;

                case "DataOperacaoNavioFormatada":
                    if (!select.Contains("DataOperacaoNavio"))
                    {
                        select.Append("PedidoViagemNavioSchedule.PVS_DATA_PREVISAO_SAIDA_NAVIO DataOperacaoNavio, ");
                        groupBy.Append("PedidoViagemNavioSchedule.PVS_DATA_PREVISAO_SAIDA_NAVIO, ");

                        SetarJoinsPedidoViagemNavioSchedule(joins);
                    }
                    break;

                case "BookingReferente":
                    if (!select.Contains("BookingReferente"))
                    {
                        select.Append("substring((select distinct ', ' + Pedido.PED_BOOKING_REFERENCE " +
                            "from T_CARGA_CTE CargaCTe inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe" +
                            " on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal " +
                            "on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO inner join T_CARGA_PEDIDO CargaPedido on" +
                            " CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO inner join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO" +
                            " where CargaCTe.CON_CODIGO = CTe.CON_CODIGO and Pedido.CTR_CODIGO = CTeContainer.CTR_CODIGO for xml path('')), 3, 200) BookingReferente, ");

                        if (!groupBy.Contains("CTeContainer.CTR_CODIGO,"))
                            groupBy.Append("CTeContainer.CTR_CODIGO, ");

                        SetarJoinsCTeContainer(joins);
                    }
                    break;

                default:
                    if (!somenteContarNumeroRegistros && propriedade.Contains("ValorComponente"))
                    {
                        select.Append(@"(select SUM(CargaCTeComponenteFrete.CCC_VALOR_COMPONENTE) from T_CARGA_CTE_COMPONENTES_FRETE CargaCTeComponenteFrete 
                                            inner join T_CARGA_CTE CargaCTe on CargaCTe.CCT_CODIGO = CargaCTeComponenteFrete.CCT_CODIGO 
                                            where CargaCTe.CON_CODIGO = CTe.CON_CODIGO and CargaCTeComponenteFrete.CFR_CODIGO = " + codigoDinamico + ") " + propriedade + ", ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioContainer filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string datePattern = "yyyy-MM-dd";

            SetarJoinsCTeContainer(joins);
            SetarJoinsContainer(joins);
            SetarJoinsCargaCTe(joins);

            groupBy.Append(" CTe.CON_CODIGO, ");

            if (filtrosPesquisa.DataEmissaoInicial != DateTime.MinValue)
                where.Append($" and CTe.CON_DATAHORAEMISSAO >= '{filtrosPesquisa.DataEmissaoInicial.ToString(datePattern)}'");

            if (filtrosPesquisa.DataEmissaoFinal != DateTime.MinValue)
                where.Append($" and CTe.CON_DATAHORAEMISSAO < '{filtrosPesquisa.DataEmissaoFinal.AddDays(1).ToString(datePattern)}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
                where.Append($" and CTe.CON_NUMERO_BOOKING = '{filtrosPesquisa.NumeroBooking}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOS))
                where.Append($" and CTe.CON_NUMERO_OS = '{filtrosPesquisa.NumeroOS}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroControle))
                where.Append($" and CTe.CON_NUMERO_CONTROLE = '{filtrosPesquisa.NumeroControle}'");

            if (filtrosPesquisa.NumeroCTe > 0)
                where.Append($" and CTe.CON_NUM = {filtrosPesquisa.NumeroCTe}");

            if (filtrosPesquisa.NumeroSerie > 0)
            {
                where.Append($" and Serie.ESE_NUMERO = {filtrosPesquisa.NumeroSerie}");
                SetarJoinsSerie(joins);
            }

            if (filtrosPesquisa.NumeroNota > 0)
            {
                where.Append($@" and CTeDocs.NFC_NUMERO = {filtrosPesquisa.NumeroNota}");
                SetarJoinsCTeDocs(joins);
            }

            if (filtrosPesquisa.TipoModal.Count > 0)
                where.Append($" and CTe.CON_TIPO_MODAL in ({string.Join(", ", filtrosPesquisa.TipoModal.Select(o => o.ToString("D")))})");

            if (filtrosPesquisa.SituacaoCarga.Count > 0)
            {
                where.Append($" and Carga.CAR_SITUACAO in ({string.Join(", ", filtrosPesquisa.SituacaoCarga.Select(o => o.ToString("D")))})");
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.SituacoesCargaMercante != null && filtrosPesquisa.SituacoesCargaMercante.Count > 0)
            {
                where.Append($" and exists (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO where CTe.CON_CODIGO = CargaCTe.CON_CODIGO AND (1 = 0 ");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.Cancelada))
                    where.Append($" or Carga.CAR_SITUACAO = 13");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.Anulada))
                    where.Append($" or Carga.CAR_SITUACAO = 18");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.AguardandoEmissao))
                    where.Append($" or (Carga.CAR_SITUACAO = 5 and Carga.CAR_DATA_RECEBIMENTO_ULTIMA_NFE is not null)");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteEmissaoCTe))
                    where.Append($" or (Carga.CAR_SITUACAO = 5 and Carga.CAR_DATA_RECEBIMENTO_ULTIMA_NFE is null)");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteMDFe))
                    where.Append($" or (Carga.CAR_MDFE_AQUAVIARIO_VINCULADO != 1 and Carga.CAR_SITUACAO != 13 and Carga.CAR_SITUACAO != 18 and Carga.CAR_CODIGO in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_TIPO_COBRANCA_MULTIMODAL = 5))");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteMercante))
                    where.Append($" or (Carga.CAR_TODOS_CTES_COM_MERCANTE != 1 and Carga.CAR_SITUACAO != 13 and Carga.CAR_SITUACAO != 18 and Carga.CAR_CODIGO in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 5))");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteFaturamento))
                    where.Append($" or (Carga.CAR_TODOS_CTES_FATURADOS != 1 and Carga.CAR_SITUACAO != 13 and Carga.CAR_SITUACAO != 18 and Carga.CAR_CODIGO in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 1 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 3 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 3))");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteIntegracaoCTe))
                    where.Append($" or Carga.CAR_SITUACAO = 15");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteIntegracaoFatura))
                    where.Append($" or (Carga.CAR_TODOS_CTES_FATURADOS_INTEGRADOS != 1 and Carga.CAR_SITUACAO != 13 and Carga.CAR_SITUACAO != 18 and Carga.CAR_CODIGO in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 1 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 3 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 3))");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteSVM))
                {
                    where.Append($" or (Carga.CAR_SITUACAO != 13 and Carga.CAR_SITUACAO != 18 and Carga.CAR_CODIGO in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 3) and ");
                    where.Append($@" exists (
                    select
                        cargacte5_.CCT_CODIGO 
                    from
                        T_CARGA_CTE cargactes4_,
                        T_CARGA_CTE cargacte5_ 
                    left outer join
                        T_CTE conhecimen6_ 
                            on cargacte5_.CON_CODIGO=conhecimen6_.CON_CODIGO 
                    where
                        Carga.CAR_CODIGO=cargactes4_.CAR_CODIGO 
                        and cargactes4_.CCT_CODIGO=cargacte5_.CCT_CODIGO 
                        and  not (exists (select
                            ctesvmmult7_.CSM_CODIGO 
                        from
                            T_CTE_SVM_MULTIMODAL ctesvmmult7_ 
                        inner join
                            T_CTE conhecimen8_ 
                                on ctesvmmult7_.CON_CODIGO_SVM=conhecimen8_.CON_CODIGO 
                        inner join
                            T_CTE conhecimen9_ 
                                on ctesvmmult7_.CON_CODIGO_MULTIMODAL=conhecimen9_.CON_CODIGO 
                        where
                            conhecimen8_.CON_STATUS='A'
                            and conhecimen9_.CON_TIPO_CTE=0
                            and (conhecimen9_.CON_CODIGO=conhecimen6_.CON_CODIGO 
                            or (conhecimen9_.CON_CODIGO is null) 
                            and (conhecimen6_.CON_CODIGO is null)))))
                    )");

                }
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.ComErro))
                    where.Append($" or Carga.CAR_SITUACAO = 15 or Carga.CAR_SITUACAO = 6 or Carga.CAR_PROBLEMA_CTE = 1");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.Finalizada))
                {
                    where.Append($" or (Carga.CAR_SITUACAO = 11 and ");
                    where.Append($" (Carga.CAR_TODOS_CTES_COM_MERCANTE = 1 or Carga.CAR_CODIGO not in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 4)) and ");
                    where.Append($" (Carga.CAR_TODOS_CTES_COM_MANIFESTO = 1 or Carga.CAR_CODIGO not in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 4)) and ");
                    where.Append($" (Carga.CAR_TODOS_CTES_FATURADOS = 1 or Carga.CAR_CODIGO not in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 1 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 3 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 3)) and ");
                    where.Append($" (Carga.CAR_MDFE_AQUAVIARIO_VINCULADO = 1 or Carga.CAR_CODIGO not in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_TIPO_COBRANCA_MULTIMODAL = 5))) ");
                }
                where.Append($" ))");
            }


            if (filtrosPesquisa.TipoProposta.Count > 0)
            {
                where.Append($" and CargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL in ({string.Join(", ", filtrosPesquisa.TipoProposta.Select(o => o.ToString("D")))})");

                SetarJoinsCarga(joins);
                SetarJoinsCargaPedido(joins);
            }

            if (filtrosPesquisa.TipoServico.Count > 0)
            {
                where.Append($" and CargaPedido.TBF_TIPO_SERVICO_MULTIMODAL in ({string.Join(", ", filtrosPesquisa.TipoServico.Select(o => o.ToString("D")))})");

                SetarJoinsCarga(joins);
                SetarJoinsCargaPedido(joins);
            }

            if (filtrosPesquisa.SituacaoCTe.Count > 0)
                where.Append($" and CTe.CON_STATUS in ('{string.Join("', '", filtrosPesquisa.SituacaoCTe.Select(o => o))}')");

            if (filtrosPesquisa.CodigoViagem > 0)
                where.Append($" and CTe.CON_VIAGEM = {filtrosPesquisa.CodigoViagem}");

            if (filtrosPesquisa.CodigoPortoOrigem > 0)
                where.Append($" and CTe.POT_CODIGO_ORIGEM = {filtrosPesquisa.CodigoPortoOrigem}");

            if (filtrosPesquisa.CodigoPortoDestino > 0)
                where.Append($" and CTe.POT_CODIGO_DESTINO = {filtrosPesquisa.CodigoPortoDestino}");

            if (filtrosPesquisa.CodigoContainer > 0)
                where.Append($" and CTeContainer.CTR_CODIGO = {filtrosPesquisa.CodigoContainer}");

            if (filtrosPesquisa.CodigoTerminalOrigem > 0)
                where.Append($" and CTe.CON_TERMINAL_ORIGEM = {filtrosPesquisa.CodigoTerminalOrigem}");

            if (filtrosPesquisa.CodigoTerminalDestino > 0)
                where.Append($" and CTe.CON_TERMINAL_DESTINO = {filtrosPesquisa.CodigoTerminalDestino}");

            if (filtrosPesquisa.CodigoGrupoPessoa > 0)
            {
                where.Append($" and TomadorPagadorCTe.GRP_CODIGO = {filtrosPesquisa.CodigoGrupoPessoa}");
                SetarJoinsTomador(joins);
            }

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
            {
                where.Append($@" and Carga.TOP_CODIGO = ({filtrosPesquisa.CodigoTipoOperacao})");

                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.TiposCTe.Count > 0)
                where.Append($" and CTe.CON_TIPO_CTE in ({string.Join(", ", filtrosPesquisa.TiposCTe.Select(o => o.ToString("D")))})");

            if (filtrosPesquisa.VeioPorImportacao == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Nao)
                where.Append($" and (CTe.CON_CTE_IMPORTADO_EMBARCADOR = 0 or CTe.CON_CTE_IMPORTADO_EMBARCADOR IS NULL)");
            else if (filtrosPesquisa.VeioPorImportacao == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                where.Append($" and CTe.CON_CTE_IMPORTADO_EMBARCADOR = 1");

            if (filtrosPesquisa.SomenteCTeSubstituido)
                where.Append(" and exists (select _cte.CON_CODIGO from T_CTE _cte where _cte.CON_CHAVE_CTE_SUB_COMP = CTe.CON_CHAVECTE)");

            if (filtrosPesquisa.CodigoViagemTransbordo > 0)
            {
                where.Append($" and CTe.CON_VIAGEM_PASSAGEM_UM = {filtrosPesquisa.CodigoViagemTransbordo}");

                if (!where.Contains("NavioTransbordo"))
                {
                    where.Append(@" and exists (SELECT navio.PVN_DESCRICAO as NavioTransbordo
                                  from T_PEDIDO_VIAGEM_NAVIO navio
                                  inner join T_PEDIDO_TRANSBORDO pedidoTransbordo on pedidoTransbordo.PVN_CODIGO = navio.PVN_CODIGO
                                  inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = pedidoTransbordo.PED_CODIGO
                                  inner join T_CARGA_CTE _cargaCTe on _cargaCTe.CAR_CODIGO = CargaPedido.CAR_CODIGO
                                  WHERE _cargaCTe.CON_CODIGO = CTe.CON_CODIGO)");
                }
            }

            if (filtrosPesquisa.CodigoPortoTransbordo > 0)
            {
                where.Append($" and CTe.POT_CODIGO_PASSAGEM_UM = {filtrosPesquisa.CodigoPortoTransbordo}");

                if (!where.Contains("NavioTransbordo"))
                {
                    where.Append(@" and exists (SELECT navio.PVN_DESCRICAO as NavioTransbordo
                                  from T_PEDIDO_VIAGEM_NAVIO navio
                                  inner join T_PEDIDO_TRANSBORDO pedidoTransbordo on pedidoTransbordo.PVN_CODIGO = navio.PVN_CODIGO
                                  inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = pedidoTransbordo.PED_CODIGO
                                  inner join T_CARGA_CTE _cargaCTe on _cargaCTe.CAR_CODIGO = CargaPedido.CAR_CODIGO 
                                  WHERE _cargaCTe.CON_CODIGO = CTe.CON_CODIGO)");
                }
            }

            if (filtrosPesquisa.CodigoBalsa > 0)
            {
                where.Append($" and CTe.NAV_CODIGO_BALSA = {filtrosPesquisa.CodigoBalsa}");
            }
        }

        #endregion
    }
}
