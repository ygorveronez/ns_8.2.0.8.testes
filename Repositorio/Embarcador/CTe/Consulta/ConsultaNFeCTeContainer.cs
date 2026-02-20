using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.CTe
{
    sealed class ConsultaNFeCTeContainer : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioNFeCTeContainer>
    {
        #region Construtores

        public ConsultaNFeCTeContainer() : base(tabela: "T_PEDIDO_XML_NOTA_FISCAL as PedidoNotaFiscal ") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            if (!joins.Contains(" CargaPedido "))
                joins.Append(" inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoNotaFiscal.CPE_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Carga "))
                joins.Append(" inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO and Carga.CAR_CARGA_FECHADA = 1 ");
        }

        private void SetarJoinsNFe(StringBuilder joins)
        {
            if (!joins.Contains(" NFe "))
                joins.Append(" inner join T_XML_NOTA_FISCAL NFe on NFe.NFX_CODIGO = PedidoNotaFiscal.NFX_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }

        private void SetarJoinsTipoCarga(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" TipoCarga "))
                joins.Append(" left join T_TIPO_DE_CARGA TipoCarga on TipoCarga.TCG_CODIGO = Carga.TCG_CODIGO ");
        }

        private void SetarJoinsPedido(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Pedido "))
                joins.Append(" left join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
        }

        private void SetarJoinsNCM(StringBuilder joins)
        {
            if (!joins.Contains(" FNNCM "))
                joins.Append(" left join T_XML_NOTA_FISCAL FNNCM on FNNCM.NFX_CODIGO = PedidoNotaFiscal.NFX_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioNFeCTeContainer filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("PedidoNotaFiscal.PNF_CODIGO as Codigo, ");
                        groupBy.Append("PedidoNotaFiscal.PNF_CODIGO, ");
                    }
                    break;

                case "ContainerDescricao":
                    if (!select.Contains(" ContainerDescricao, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + Container.CTR_DESCRICAO
		                                            FROM T_CONTAINER Container
                                                    join T_CTE_CONTAINER CTeContainer on Container.CTR_CODIGO = CTeContainer.CTR_CODIGO
                                                    join T_CTE_CONTAINER_DOCUMENTO documento on documento.CER_CODIGO = CTeContainer.CER_CODIGO
		                                            {SetarWhereSubselectContainer(filtroPesquisa)} FOR XML PATH('')), 3, 1000) ContainerDescricao, ");

                        if (!groupBy.Contains("NFe.NFX_CODIGO,"))
                            groupBy.Append("NFe.NFX_CODIGO, ");

                        SetarJoinsNFe(joins);
                    }
                    break;

                case "TipoContainer":
                    if (!select.Contains(" TipoContainer, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + ContainerTipo.CTI_DESCRICAO
		                                            FROM T_CONTAINER_TIPO ContainerTipo
                                                    join T_CONTAINER Container on ContainerTipo.CTI_CODIGO = Container.CTI_CODIGO
                                                    join T_CTE_CONTAINER CTeContainer on Container.CTR_CODIGO = CTeContainer.CTR_CODIGO
                                                    join T_CTE_CONTAINER_DOCUMENTO documento on documento.CER_CODIGO = CTeContainer.CER_CODIGO
		                                            {SetarWhereSubselectContainer(filtroPesquisa)} FOR XML PATH('')), 3, 1000) TipoContainer, ");

                        if (!groupBy.Contains("NFe.NFX_CODIGO,"))
                            groupBy.Append("NFe.NFX_CODIGO, ");

                        SetarJoinsNFe(joins);
                    }
                    break;

                case "NumeroBooking":
                    if (!select.Contains(" NumeroBooking, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + CTe.CON_NUMERO_BOOKING
		                                            FROM T_CTE CTe
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) NumeroBooking, ");
                    }
                    break;

                case "NumeroOS":
                    if (!select.Contains(" NumeroOS, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + CTe.CON_NUMERO_OS
		                                            FROM T_CTE CTe
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) NumeroOS, ");
                    }
                    break;

                case "NumeroControle":
                    if (!select.Contains(" NumeroControle, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + CTe.CON_NUMERO_CONTROLE
		                                            FROM T_CTE CTe
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) NumeroControle, ");
                    }
                    break;

                case "NumeroCTe":
                    if (!select.Contains(" NumeroCTe, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + CAST(CTe.CON_NUM  AS NVARCHAR(10))
		                                            FROM T_CTE CTe
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) NumeroCTe, ");
                    }
                    break;

                case "StatusCTe":
                    if (!select.Contains(" StatusCTe, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + CASE CTe.CON_STATUS 
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
                                                                        END
		                                            FROM T_CTE CTe
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) StatusCTe, ");
                    }
                    break;

                case "Viagem":
                    if (!select.Contains(" Viagem, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + Viagem.PVN_DESCRICAO
		                                            FROM T_CTE CTe
                                                    join T_PEDIDO_VIAGEM_NAVIO Viagem on Viagem.PVN_CODIGO = CTe.CON_VIAGEM
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) Viagem, ");
                    }
                    break;

                case "NavioTransbordo":
                    if (!select.Contains(" NavioTransbordo, "))
                    {
                        select.Append(
                            $@"SUBSTRING((
                                SELECT DISTINCT ', ' + navio.PVN_DESCRICAO
                                        from T_PEDIDO_VIAGEM_NAVIO navio
                                        inner join T_PEDIDO_TRANSBORDO pedidoTransbordo on pedidoTransbordo.PVN_CODIGO = navio.PVN_CODIGO 
                                        inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = pedidoTransbordo.PED_CODIGO 
                                        inner join T_CARGA_CTE _cargaCTe on _cargaCTe.CAR_CODIGO = CargaPedido.CAR_CODIGO 
                                        join T_CTE CTe on _cargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                        join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
                                        {SetarWhereSubselectCTe(filtroPesquisa)} for xml path('')), 3, 1000) NavioTransbordo, "
                        );
                    }
                    break;

                case "PortoOrigem":
                    if (!select.Contains(" PortoOrigem, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + PortoOrigem.POT_DESCRICAO
		                                            FROM T_CTE CTe
                                                    join T_PORTO PortoOrigem on PortoOrigem.POT_CODIGO = CTe.POT_CODIGO_ORIGEM
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) PortoOrigem, ");
                    }
                    break;

                case "PortoDestino":
                    if (!select.Contains(" PortoDestino, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + PortoDestino.POT_DESCRICAO
		                                            FROM T_CTE CTe
                                                    join T_PORTO PortoDestino on PortoDestino.POT_CODIGO = CTe.POT_CODIGO_DESTINO
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) PortoDestino, ");
                    }
                    break;

                case "PortoTransbordo":
                    if (!select.Contains(" PortoTransbordo, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + concat(PortoPassagemUm.POT_DESCRICAO
                                                                    , CASE WHEN PortoPassagemUm.POT_CODIGO IS NOT NULL THEN '; ' + PortoPassagemDois.POT_DESCRICAO ELSE '' END
                                                                    , CASE WHEN PortoPassagemDois.POT_CODIGO IS NOT NULL THEN '; ' + PortoPassagemTres.POT_DESCRICAO ELSE '' END
                                                                    , CASE WHEN PortoPassagemTres.POT_CODIGO IS NOT NULL THEN '; ' + PortoPassagemQuatro.POT_DESCRICAO ELSE '' END
                                                                    , CASE WHEN PortoPassagemQuatro.POT_CODIGO IS NOT NULL THEN '; ' + PortoPassagemCinco.POT_DESCRICAO ELSE '' END )
		                                            FROM T_CTE CTe
                                                    join T_PORTO PortoPassagemUm on PortoPassagemUm.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_UM
                                                    left join T_PORTO PortoPassagemDois on PortoPassagemDois.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_DOIS
                                                    left join T_PORTO PortoPassagemTres on PortoPassagemTres.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_TRES
                                                    left join T_PORTO PortoPassagemQuatro on PortoPassagemQuatro.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_QUATRO
                                                    left join T_PORTO PortoPassagemCinco on PortoPassagemCinco.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_CINCO
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) PortoTransbordo, ");
                    }
                    break;

                case "TerminalOrigem":
                    if (!select.Contains(" TerminalOrigem, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + TerminalOrigem.TTI_DESCRICAO
		                                            FROM T_CTE CTe
                                                    join T_TIPO_TERMINAL_IMPORTACAO TerminalOrigem on TerminalOrigem.TTI_CODIGO = CTe.CON_TERMINAL_ORIGEM
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) TerminalOrigem, ");
                    }
                    break;

                case "TerminalDestino":
                    if (!select.Contains(" TerminalDestino, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + TerminalDestino.TTI_DESCRICAO
		                                            FROM T_CTE CTe
                                                    join T_TIPO_TERMINAL_IMPORTACAO TerminalDestino on TerminalDestino.TTI_CODIGO = CTe.CON_TERMINAL_DESTINO
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) TerminalDestino, ");
                    }
                    break;

                case "TerminalTransbordo":
                    if (!select.Contains(" TerminalTransbordo, "))
                    {
                        select.Append(
                            $@"SUBSTRING((
                                SELECT DISTINCT '; ' + terminal.TTI_DESCRICAO
                                        from T_TIPO_TERMINAL_IMPORTACAO terminal
                                        inner join T_PEDIDO_TRANSBORDO pedidoTransbordo on pedidoTransbordo.TTI_CODIGO = terminal.TTI_CODIGO 
                                        inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = pedidoTransbordo.PED_CODIGO 
                                        inner join T_CARGA_CTE _cargaCTe on _cargaCTe.CAR_CODIGO = CargaPedido.CAR_CODIGO 
                                        join T_CTE CTe on _cargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                        join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
                                 {SetarWhereSubselectCTe(filtroPesquisa)} for xml path('')), 3, 1000) TerminalTransbordo, "
                        );
                    }
                    break;

                case "Remetente":
                    if (!select.Contains(" Remetente, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + Remetente.PCT_NOME
		                                            FROM T_CTE CTe
                                                    join T_CTE_PARTICIPANTE Remetente on Remetente.PCT_CODIGO = CTe.CON_REMETENTE_CTE
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) Remetente, ");
                    }
                    break;
                case "CNPJRemetente":
                    if (!select.Contains(" CNPJRemetente, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + CNPJRemetente.PCT_CPF_CNPJ
		                                            FROM T_CTE CTe
                                                    join T_CTE_PARTICIPANTE CNPJRemetente on CNPJRemetente.PCT_CODIGO = CTe.CON_REMETENTE_CTE
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) CNPJRemetente, ");
                    }
                    break;
                case "Destinatario":
                    if (!select.Contains(" Destinatario, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + Destinatario.PCT_NOME
		                                            FROM T_CTE CTe
                                                    join T_CTE_PARTICIPANTE Destinatario on Destinatario.PCT_CODIGO = CTe.CON_DESTINATARIO_CTE
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) Destinatario, ");
                    }
                    break;
                case "CNPJDestinatario":
                    if (!select.Contains(" CNPJDestinatario, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + CNPJDestinatario.PCT_CPF_CNPJ
		                                            FROM T_CTE CTe
                                                    join T_CTE_PARTICIPANTE CNPJDestinatario on CNPJDestinatario.PCT_CODIGO = CTe.CON_DESTINATARIO_CTE
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) CNPJDestinatario, ");
                    }
                    break;
                case "Expedidor":
                    if (!select.Contains(" Expedidor, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + Expedidor.PCT_NOME
		                                            FROM T_CTE CTe
                                                    join T_CTE_PARTICIPANTE Expedidor on Expedidor.PCT_CODIGO = CTe.CON_EXPEDIDOR_CTE
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) Expedidor, ");
                    }
                    break;
                case "CNPJExpedidor":
                    if (!select.Contains(" CNPJExpedidor, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + CNPJExpedidor.PCT_CPF_CNPJ
		                                            FROM T_CTE CTe
                                                    join T_CTE_PARTICIPANTE CNPJExpedidor on CNPJExpedidor.PCT_CODIGO = CTe.CON_EXPEDIDOR_CTE
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) CNPJExpedidor, ");
                    }
                    break;
                case "Recebedor":
                    if (!select.Contains(" Recebedor, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + Recebedor.PCT_NOME
		                                            FROM T_CTE CTe
                                                    join T_CTE_PARTICIPANTE Recebedor on Recebedor.PCT_CODIGO = CTe.CON_RECEBEDOR_CTE
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) Recebedor, ");
                    }
                    break;
                case "CNPJRecebedor":
                    if (!select.Contains(" CNPJRecebedor, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + CNPJRecebedor.PCT_CPF_CNPJ
		                                            FROM T_CTE CTe
                                                    join T_CTE_PARTICIPANTE CNPJRecebedor on CNPJRecebedor.PCT_CODIGO = CTe.CON_RECEBEDOR_CTE
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) CNPJRecebedor, ");
                    }
                    break;
                case "Tomador":
                    if (!select.Contains(" Tomador, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + TomadorPagadorCTe.PCT_NOME
		                                            FROM T_CTE CTe
                                                    join T_CTE_PARTICIPANTE TomadorPagadorCTe on TomadorPagadorCTe.PCT_CODIGO = CTe.CON_TOMADOR_PAGADOR_CTE
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) Tomador, ");
                    }
                    break;
                case "CNPJTomador":
                    if (!select.Contains(" CNPJTomador, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + CNPJTomador.PCT_CPF_CNPJ
		                                            FROM T_CTE CTe
                                                    join T_CTE_PARTICIPANTE CNPJTomador on CNPJTomador.PCT_CODIGO = CTe.CON_TOMADOR_PAGADOR_CTE
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) CNPJTomador, ");
                    }
                    break;
                case "Tara":
                    if (!select.Contains(" Tara, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + FORMAT(container.CTR_TARA, 'N2', 'pt-BR')
		                                            FROM T_CONTAINER Container
                                                    join T_CTE_CONTAINER CTeContainer on Container.CTR_CODIGO = CTeContainer.CTR_CODIGO
                                                    join T_CTE_CONTAINER_DOCUMENTO documento on documento.CER_CODIGO = CTeContainer.CER_CODIGO
		                                            {SetarWhereSubselectContainer(filtroPesquisa)} FOR XML PATH('')), 3, 1000) Tara, ");

                        if (!groupBy.Contains("NFe.NFX_CODIGO,"))
                            groupBy.Append("NFe.NFX_CODIGO, ");

                        SetarJoinsNFe(joins);
                    }
                    break;

                case "NumeroLacre":
                    if (!select.Contains(" NumeroLacre, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + (CTeContainer.CER_LACRE1
                                                                        + CASE WHEN RTRIM(CTeContainer.CER_LACRE1) <> '' THEN ', ' + CTeContainer.CER_LACRE2 ELSE '' END
                                                                        + CASE WHEN RTRIM(CTeContainer.CER_LACRE2) <> '' THEN ', ' + CTeContainer.CER_LACRE3 ELSE '' END )
		                                            FROM T_CTE_CONTAINER CTeContainer 
                                                    join T_CTE_CONTAINER_DOCUMENTO documento on documento.CER_CODIGO = CTeContainer.CER_CODIGO
		                                            {SetarWhereSubselectContainer(filtroPesquisa)} FOR XML PATH('')), 3, 1000) NumeroLacre, ");

                        if (!groupBy.Contains("NFe.NFX_CODIGO,"))
                            groupBy.Append("NFe.NFX_CODIGO, ");

                        SetarJoinsNFe(joins);
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO TipoOperacao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");


                        SetarJoinsTipoOperacao(joins);
                    }
                    break;

                case "UFInicioPrestacao":
                    if (!select.Contains(" UFInicioPrestacao, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + InicioPrestacaoCTe.UF_SIGLA
		                                            FROM T_CTE CTe
                                                    join T_LOCALIDADES InicioPrestacaoCTe on CTe.CON_LOCINICIOPRESTACAO = InicioPrestacaoCTe.LOC_CODIGO
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) UFInicioPrestacao, ");
                    }
                    break;

                case "UFFimPrestacao":
                    if (!select.Contains(" UFFimPrestacao, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + FimPrestacaoCTe.UF_SIGLA
		                                            FROM T_CTE CTe
                                                    join T_LOCALIDADES FimPrestacaoCTe on CTe.CON_LOCTERMINOPRESTACAO = FimPrestacaoCTe.LOC_CODIGO
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) UFFimPrestacao, ");
                    }
                    break;

                case "CargaIMO":
                    if (!select.Contains(" CargaIMO, "))
                    {
                        select.Append("(CASE WHEN TipoCarga.TCG_POSSUI_CARGA_PERIGOSA = 1 THEN 'Sim' ELSE 'Não' END) CargaIMO, ");
                        groupBy.Append("TipoCarga.TCG_POSSUI_CARGA_PERIGOSA, ");

                        SetarJoinsTipoCarga(joins);
                    }
                    break;

                case "SituacaoCargaFormatada":
                    if (!select.Contains(" SituacaoCarga, "))
                    {
                        select.Append("Carga.CAR_SITUACAO SituacaoCarga, ");
                        groupBy.Append("Carga.CAR_SITUACAO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "ProdutoPredominante":
                    if (!select.Contains(" ProdutoPredominante, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + CTe.CON_PRODUTO_PRED
		                                            FROM T_CTE CTe
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) ProdutoPredominante, ");
                    }
                    break;

                case "PossuiCCe":
                    if (!select.Contains(" PossuiCCe, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + (CASE WHEN CTe.CON_POSSUI_CARTA_CORRECAO = 1 THEN 'Sim' ELSE 'Não' END)
		                                            FROM T_CTE CTe
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) PossuiCCe, ");
                    }
                    break;

                case "NumeroNota":
                    if (!select.Contains(" NumeroNota, "))
                    {
                        select.Append("NFe.NF_NUMERO as NumeroNota, ");
                        groupBy.Append("NFe.NF_NUMERO, ");

                        SetarJoinsNFe(joins);
                    }
                    break;

                case "SerieNota":
                    if (!select.Contains(" SerieNota, "))
                    {
                        select.Append("NFe.NF_SERIE as SerieNota, ");
                        groupBy.Append("NFe.NF_SERIE, ");

                        SetarJoinsNFe(joins);
                    }
                    break;

                case "DataEmissaoNotaFormatada":
                    if (!select.Contains(" DataEmissaoNota, "))
                    {
                        select.Append("NFe.NF_DATA_EMISSAO as DataEmissaoNota, ");
                        groupBy.Append("NFe.NF_DATA_EMISSAO, ");

                        SetarJoinsNFe(joins);
                    }
                    break;

                case "ValorNota":
                    if (!select.Contains(" ValorNota, "))
                    {
                        select.Append("NFe.NF_VALOR as ValorNota, ");
                        groupBy.Append("NFe.NF_VALOR, ");

                        SetarJoinsNFe(joins);
                    }
                    break;

                case "PesoNota":
                    if (!select.Contains(" PesoNota, "))
                    {
                        select.Append("NFe.NF_PESO as PesoNota, ");
                        groupBy.Append("NFe.NF_PESO, ");

                        SetarJoinsNFe(joins);
                    }
                    break;

                case "ChaveNota":
                    if (!select.Contains(" ChaveNota, "))
                    {
                        select.Append("NFe.NF_CHAVE as ChaveNota, ");

                        if (!groupBy.Contains("NFe.NF_CHAVE,"))
                            groupBy.Append("NFe.NF_CHAVE, ");

                        SetarJoinsNFe(joins);
                    }
                    break;

                case "ChaveCTeAquaviario":
                    if (!select.Contains(" ChaveCTeAquaviario, "))
                    {
                        select.Append(@"(
                                (SUBSTRING((
                                SELECT DISTINCT ', ' + cte.CON_CHAVECTE
                                        from T_CTE cte
                                        inner join T_CTE_DOCS cteDocs on cteDocs.CON_CODIGO = cte.CON_CODIGO 
                                 WHERE cte.CON_TIPO_MODAL = 3 and cte.CON_TIPO_SERVICO <> 4 and cte.CON_STATUS = 'A' and cteDocs.NFX_CODIGO_CORRETA = NFe.NFX_CODIGO for xml path('')), 3, 1000)) ) ChaveCTeAquaviario, "
                        );

                        if (!groupBy.Contains("NFe.NFX_CODIGO,"))
                            groupBy.Append("NFe.NFX_CODIGO, ");

                        SetarJoinsNFe(joins);
                    }
                    break;

                case "ChaveCTeMultimodal":
                    if (!select.Contains(" ChaveCTeMultimodal, "))
                    {
                        select.Append(@"(
                                (SUBSTRING((
                                SELECT DISTINCT ', ' + cte.CON_CHAVECTE
                                        from T_CTE cte
                                        inner join T_CTE_DOCS cteDocs on cteDocs.CON_CODIGO = cte.CON_CODIGO 
                                 WHERE cte.CON_TIPO_MODAL = 6 and cte.CON_TIPO_SERVICO <> 4 and cte.CON_STATUS = 'A' and cteDocs.NFX_CODIGO_CORRETA = NFe.NFX_CODIGO for xml path('')), 3, 1000)) ) ChaveCTeMultimodal, "
                        );

                        if (!groupBy.Contains("NFe.NFX_CODIGO,"))
                            groupBy.Append("NFe.NFX_CODIGO, ");

                        SetarJoinsNFe(joins);
                    }
                    break;

                case "ChaveCTeSVM":
                    if (!select.Contains(" ChaveCTeSVM, "))
                    {
                        select.Append(@"(
                                (SUBSTRING((
                                SELECT DISTINCT ', ' + cte.CON_CHAVECTE
                                        from T_CTE cte
                                        inner join T_CTE_DOCS cteDocs on cteDocs.CON_CODIGO = cte.CON_CODIGO 
                                 WHERE cte.CON_TIPO_SERVICO = 4 and cte.CON_STATUS = 'A' and cteDocs.NFX_CODIGO_CORRETA = NFe.NFX_CODIGO for xml path('')), 3, 1000)) ) ChaveCTeSVM, "
                        );

                        if (!groupBy.Contains("NFe.NFX_CODIGO,"))
                            groupBy.Append("NFe.NFX_CODIGO, ");

                        SetarJoinsNFe(joins);
                    }
                    break;

                case "NumeroPedidoEmbarcador":
                    if (!select.Contains(" NumeroPedidoEmbarcador, "))
                    {
                        select.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR as NumeroPedidoEmbarcador, ");
                        groupBy.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "ChaveCTE":
                    if (!select.Contains(" ChaveCTE, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + CTe.CON_CHAVECTE
		                                            FROM T_CTE CTe
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) ChaveCTE, ");
                    }
                    break;

                case "PropostaComercial":
                    if (!select.Contains(" PropostaComercial, "))
                    {
                        select.Append("Pedido.PED_CODIGO_PROPOSTA as PropostaComercial, ");
                        groupBy.Append("Pedido.PED_CODIGO_PROPOSTA, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "MasterBL":
                    if (!select.Contains(" MasterBL, "))
                    {
                        select.Append("NFe.NF_MASTER_BL as MasterBL, ");
                        groupBy.Append("NFe.NF_MASTER_BL, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "Embarque":
                    if (!select.Contains(" Embarque, "))
                    {
                        select.Append("NFe.NF_EMBARQUE as Embarque, ");
                        groupBy.Append("NFe.NF_EMBARQUE, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "NumeroDI":
                    if (!select.Contains(" NumeroDI, "))
                    {
                        select.Append("NFe.NF_NUMERO_DI as NumeroDI, ");
                        groupBy.Append("NFe.NF_NUMERO_DI, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "NumeroControleCliente":
                    if (!select.Contains(" NumeroControleCliente, "))
                    {
                        select.Append("NFe.NF_NUMERO_CONTROLE_CLIENTE as NumeroControleCliente, ");
                        groupBy.Append("NFe.NF_NUMERO_CONTROLE_CLIENTE, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "Reefer":
                    if (!select.Contains(" Reefer, "))
                    {
                        select.Append("(CASE WHEN Pedido.PED_CONTEM_CARGA_REFRIGERADA = 1 THEN 'Sim' ELSE 'Não' END) Reefer, ");
                        groupBy.Append("Pedido.PED_CONTEM_CARGA_REFRIGERADA, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "NumeroReferenciaEDI":
                    if (!select.Contains(" NumeroReferenciaEDI, "))
                    {
                        select.Append("NFe.NF_NUMERO_REFERENCIA_EDI as NumeroReferenciaEDI, ");

                        if (!groupBy.Contains("NFe.NF_NUMERO_REFERENCIA_EDI,"))
                            groupBy.Append("NFe.NF_NUMERO_REFERENCIA_EDI, ");

                        SetarJoinsNFe(joins);
                    }
                    break;

                case "NCM":
                    if (!select.Contains(" FNNCM, "))
                    {
                        select.Append("FNNCM.NF_NCM as NCM, ");
                        if (!groupBy.Contains("FNNCM.NF_NCM,"))
                            groupBy.Append("FNNCM.NF_NCM, ");

                        SetarJoinsNCM(joins);
                    }
                    break;

                case "BookingReferenceFeeder":
                    if (!select.Contains(" BookingReferenceFeeder, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + CTe.CON_BOOKING_REFERENCE
		                                            FROM T_CTE CTe
                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
		                                            {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) BookingReferenceFeeder, ");
                    }
                    break;

                case "AliquotaISS":
                    if (!select.ToString().Contains(" AliquotaISS, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + CAST(CTe.CON_ALIQUOTA_ISS AS VARCHAR)
                                     FROM T_CTE CTe
                                     JOIN T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal ON CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
                                    {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) AS AliquotaISS, ");
                    }
                    break;

                case "ValorISS":
                    if (!select.ToString().Contains(" ValorISS, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + CAST(CTe.CON_VALOR_ISS AS VARCHAR)
                                     FROM T_CTE CTe
                                     JOIN T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal ON CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
                                    {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) AS ValorISS, ");
                    }
                    break;

                case "ValorISSRetido":
                    if (!select.ToString().Contains(" ValorISSRetido, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + CAST(CTe.CON_VALOR_ISS_RETIDO AS VARCHAR)
                                     FROM T_CTE CTe
                                     JOIN T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal ON CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
                                    {SetarWhereSubselectCTe(filtroPesquisa)} FOR XML PATH('')), 3, 1000) AS ValorISSRetido, ");
                    }
                    break;



            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioNFeCTeContainer filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            SetarJoinsCarga(joins);
            SetarJoinsNFe(joins);

            groupBy.Insert(0, "NFe.NFX_CODIGO, ");

            where.Append(" and Carga.CAR_SITUACAO NOT IN (13, 18) and (Carga.CAR_CARGA_TRANSBORDO is null or Carga.CAR_CARGA_TRANSBORDO = 0)");

            if (filtrosPesquisa.NumeroNota > 0)
                where.Append($" and NFe.NF_NUMERO = {filtrosPesquisa.NumeroNota}");

            if (filtrosPesquisa.SituacaoCarga.Count > 0)
                where.Append($" and Carga.CAR_SITUACAO in ({string.Join(", ", filtrosPesquisa.SituacaoCarga.Select(o => o.ToString("D")))})");

            if (filtrosPesquisa.SituacoesCargaMercante != null && filtrosPesquisa.SituacoesCargaMercante.Count > 0)
            {
                where.Append($" and (1 = 0 ");
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
                where.Append($" )");
            }

            if (filtrosPesquisa.TipoProposta.Count > 0)
                where.Append($" and CargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL in ({string.Join(", ", filtrosPesquisa.TipoProposta.Select(o => o.ToString("D")))})");

            if (filtrosPesquisa.TipoServico.Count > 0)
                where.Append($" and CargaPedido.TBF_TIPO_SERVICO_MULTIMODAL in ({string.Join(", ", filtrosPesquisa.TipoServico.Select(o => o.ToString("D")))})");

            if (filtrosPesquisa.CodigoContainer > 0)
            {
                where.Append($@" and NFe.NFX_CODIGO in (SELECT documento.NFX_CODIGO
		                                                FROM T_CONTAINER Container
                                                        join T_CTE_CONTAINER CTeContainer on Container.CTR_CODIGO = CTeContainer.CTR_CODIGO
                                                        join T_CTE_CONTAINER_DOCUMENTO documento on documento.CER_CODIGO = CTeContainer.CER_CODIGO
		                                                WHERE documento.CCD_TIPO_DOCUMENTO = 0 and Container.CTR_CODIGO = {filtrosPesquisa.CodigoContainer})");
            }

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                where.Append($" and Carga.TOP_CODIGO = ({filtrosPesquisa.CodigoTipoOperacao})");

            //Filtros relacionados a ctes
            StringBuilder whereCTe = new StringBuilder();
            SetarWhereCTe(filtrosPesquisa, whereCTe);

            if (!string.IsNullOrWhiteSpace(whereCTe.ToString().Trim()))
            {
                where.Append($@" and NFe.NFX_CODIGO in (SELECT CTeXMLNotaFiscal.NFX_CODIGO
                                                        FROM T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal
                                                        join T_CTE CTe on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
                                                        join T_CARGA_CTE CargaCTe ON CargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                                        left join T_EMPRESA_SERIE Serie on CTe.CON_SERIE = Serie.ESE_CODIGO
                                                        left join T_CTE_PARTICIPANTE TomadorPagadorCTe on TomadorPagadorCTe.PCT_CODIGO = CTe.CON_TOMADOR_PAGADOR_CTE
                                                        where CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO {whereCTe.ToString().Trim()})");
            }
        }

        #endregion

        #region Métodos Privados - Subselects

        private string SetarWhereCTe(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioNFeCTeContainer filtrosPesquisa, StringBuilder whereCTe)
        {
            string datePattern = "yyyy-MM-dd";

            if (filtrosPesquisa.DataEmissaoInicial != DateTime.MinValue)
                whereCTe.Append($" and CTe.CON_DATAHORAEMISSAO >= '{filtrosPesquisa.DataEmissaoInicial.ToString(datePattern)}'");

            if (filtrosPesquisa.DataEmissaoFinal != DateTime.MinValue)
                whereCTe.Append($" and CTe.CON_DATAHORAEMISSAO < '{filtrosPesquisa.DataEmissaoFinal.AddDays(1).ToString(datePattern)}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
                whereCTe.Append($" and CTe.CON_NUMERO_BOOKING = '{filtrosPesquisa.NumeroBooking}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOS))
                whereCTe.Append($" and CTe.CON_NUMERO_OS = '{filtrosPesquisa.NumeroOS}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroControle))
                whereCTe.Append($" and CTe.CON_NUMERO_CONTROLE = '{filtrosPesquisa.NumeroControle}'");

            if (filtrosPesquisa.NumeroCTe > 0)
                whereCTe.Append($" and CTe.CON_NUM = {filtrosPesquisa.NumeroCTe}");

            if (filtrosPesquisa.NumeroSerie > 0)
                whereCTe.Append($" and Serie.ESE_NUMERO = {filtrosPesquisa.NumeroSerie}");

            if (filtrosPesquisa.TipoModal.Count > 0)
                whereCTe.Append($" and CTe.CON_TIPO_MODAL in ({string.Join(", ", filtrosPesquisa.TipoModal.Select(o => o.ToString("D")))})");

            if (filtrosPesquisa.SituacaoCTe.Count > 0)
                whereCTe.Append($" and CTe.CON_STATUS in ('{string.Join("', '", filtrosPesquisa.SituacaoCTe.Select(o => o))}')");

            if (filtrosPesquisa.CodigoViagem > 0)
                whereCTe.Append($" and CTe.CON_VIAGEM = {filtrosPesquisa.CodigoViagem}");

            if (filtrosPesquisa.CodigoPortoOrigem > 0)
                whereCTe.Append($" and CTe.POT_CODIGO_ORIGEM = {filtrosPesquisa.CodigoPortoOrigem}");

            if (filtrosPesquisa.CodigoPortoDestino > 0)
                whereCTe.Append($" and CTe.POT_CODIGO_DESTINO = {filtrosPesquisa.CodigoPortoDestino}");

            if (filtrosPesquisa.CodigoTerminalOrigem > 0)
                whereCTe.Append($" and CTe.CON_TERMINAL_ORIGEM = {filtrosPesquisa.CodigoTerminalOrigem}");

            if (filtrosPesquisa.CodigoTerminalDestino > 0)
                whereCTe.Append($" and CTe.CON_TERMINAL_DESTINO = {filtrosPesquisa.CodigoTerminalDestino}");

            if (filtrosPesquisa.CodigoGrupoPessoa > 0)
                whereCTe.Append($" and TomadorPagadorCTe.GRP_CODIGO = {filtrosPesquisa.CodigoGrupoPessoa}");

            if (filtrosPesquisa.TiposCTe.Count > 0)
                whereCTe.Append($" and CTe.CON_TIPO_CTE in ({string.Join(", ", filtrosPesquisa.TiposCTe.Select(o => o.ToString("D")))})");

            if (filtrosPesquisa.FoiAnulado != Dominio.Enumeradores.OpcaoSimNaoPesquisa.Todos)
            {
                if (filtrosPesquisa.FoiAnulado == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                    whereCTe.Append(" and CTe.CON_POSSUI_ANULACAO_SUBSTITUICAO = 1");
                else
                    whereCTe.Append(" and (CTe.CON_POSSUI_ANULACAO_SUBSTITUICAO = 0 or CTe.CON_POSSUI_ANULACAO_SUBSTITUICAO is null)");
            }

            if (filtrosPesquisa.FoiSubstituido != Dominio.Enumeradores.OpcaoSimNaoPesquisa.Todos)
            {
                if (filtrosPesquisa.FoiSubstituido == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                    whereCTe.Append(" and exists (select _cte.CON_CODIGO from T_CTE _cte where _cte.CON_CHAVE_CTE_SUB_COMP = CTe.CON_CHAVECTE)");
                else
                    whereCTe.Append(" and not exists (select _cte.CON_CODIGO from T_CTE _cte where _cte.CON_CHAVE_CTE_SUB_COMP = CTe.CON_CHAVECTE)");
            }

            return whereCTe.ToString().Trim();
        }

        private string SetarWhereSubselectCTe(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioNFeCTeContainer filtrosPesquisa)
        {
            StringBuilder joins = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            StringBuilder where = new StringBuilder();

            where.Append(" WHERE CTeXMLNotaFiscal.NFX_CODIGO = NFe.NFX_CODIGO");
            SetarWhereCTe(filtrosPesquisa, where);

            if (filtrosPesquisa.NumeroSerie > 0)
                joins.Append(" join T_EMPRESA_SERIE Serie on CTe.CON_SERIE = Serie.ESE_CODIGO ");

            if (filtrosPesquisa.CodigoGrupoPessoa > 0)
                joins.Append(" join T_CTE_PARTICIPANTE TomadorPagadorCTe on TomadorPagadorCTe.PCT_CODIGO = CTe.CON_TOMADOR_PAGADOR_CTE ");

            if (filtrosPesquisa.TipoProposta.Count > 0)
            {
                if (!joins.Contains(" CargaCTe "))
                    joins.Append(" join T_CARGA_CTE CargaCTe ON CargaCTe.CON_CODIGO = CTe.CON_CODIGO ");
                if (!joins.Contains(" CargaPedido "))
                    joins.Append(" join T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = CargaCTe.CAR_CODIGO ");

                where.Append($" and CargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL in ({string.Join(", ", filtrosPesquisa.TipoProposta.Select(o => o.ToString("D")))})");
            }

            if (filtrosPesquisa.TipoServico.Count > 0)
            {
                if (!joins.Contains(" CargaCTe "))
                    joins.Append(" join T_CARGA_CTE CargaCTe ON CargaCTe.CON_CODIGO = CTe.CON_CODIGO ");
                if (!joins.Contains(" CargaPedido "))
                    joins.Append(" join T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = CargaCTe.CAR_CODIGO ");

                where.Append($" and CargaPedido.TBF_TIPO_SERVICO_MULTIMODAL in ({string.Join(", ", filtrosPesquisa.TipoServico.Select(o => o.ToString("D")))})");
            }

            sql.Append(joins.ToString());
            sql.Append(where.ToString().Trim());

            return sql.ToString();
        }

        private string SetarWhereSubselectContainer(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioNFeCTeContainer filtrosPesquisa)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder where = new StringBuilder();

            where.Append(" WHERE documento.CCD_TIPO_DOCUMENTO = 0 and documento.NFX_CODIGO = NFe.NFX_CODIGO");

            if (filtrosPesquisa.CodigoContainer > 0)
                where.Append($" and Container.CTR_CODIGO = {filtrosPesquisa.CodigoContainer}");

            sql.Append(where.ToString().Trim());

            return sql.ToString();
        }

        #endregion
    }
}
