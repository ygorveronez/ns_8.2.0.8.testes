using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.CTe
{
    sealed class ConsultaApuracaoICMS : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioApuracaoICMS>
    {
        #region Construtores

        public ConsultaApuracaoICMS() : base(tabela: "T_CTE as CTe") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            if (!joins.Contains(" DestinatarioCTe "))
                joins.Append(" join T_CTE_PARTICIPANTE DestinatarioCTe on CTe.CON_DESTINATARIO_CTE = DestinatarioCTe.PCT_CODIGO ");
        }

        private void SetarJoinsExpedidor(StringBuilder joins)
        {
            if (!joins.Contains(" ExpedidorCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE ExpedidorCTe on CTe.CON_EXPEDIDOR_CTE = ExpedidorCTe.PCT_CODIGO ");
        }

        private void SetarJoinsRecebedor(StringBuilder joins)
        {
            if (!joins.Contains(" RecebedorCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE RecebedorCTe on CTe.CON_RECEBEDOR_CTE = RecebedorCTe.PCT_CODIGO ");
        }

        private void SetarJoinsRemetente(StringBuilder joins)
        {
            if (!joins.Contains(" RemetenteCTe "))
                joins.Append(" join T_CTE_PARTICIPANTE RemetenteCTe on CTe.CON_REMETENTE_CTE = RemetenteCTe.PCT_CODIGO ");
        }

        private void SetarJoinsTomador(StringBuilder joins)
        {
            if (!joins.Contains(" TomadorPagadorCTe "))
                joins.Append(" join T_CTE_PARTICIPANTE TomadorPagadorCTe on CTe.CON_TOMADOR_PAGADOR_CTE = TomadorPagadorCTe.PCT_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioApuracaoICMS filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("CTe.CON_CODIGO as Codigo, ");
                        groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "CPFCNPJRemetenteFormatado":
                    if (!select.Contains(" CPFCNPJRemetente, "))
                    {
                        select.Append("RemetenteCTe.PCT_CPF_CNPJ CPFCNPJRemetente, RemetenteCTe.PCT_TIPO TipoRemetente, ");
                        groupBy.Append("RemetenteCTe.PCT_CPF_CNPJ, RemetenteCTe.PCT_TIPO, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "IERemetente":
                    if (!select.Contains(" IERemetente, "))
                    {
                        select.Append("RemetenteCTe.PCT_IERG IERemetente, ");
                        groupBy.Append("RemetenteCTe.PCT_IERG, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "Remetente":
                    if (!select.Contains(" Remetente, "))
                    {
                        select.Append("RemetenteCTe.PCT_NOME Remetente, ");
                        groupBy.Append("RemetenteCTe.PCT_NOME, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "CPFCNPJDestinatarioFormatado":
                    if (!select.Contains(" CPFCNPJDestinatario, "))
                    {
                        select.Append("DestinatarioCTe.PCT_CPF_CNPJ CPFCNPJDestinatario, DestinatarioCTe.PCT_TIPO TipoDestinatario, ");
                        groupBy.Append("DestinatarioCTe.PCT_CPF_CNPJ, DestinatarioCTe.PCT_TIPO, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "IEDestinatario":
                    if (!select.Contains(" IEDestinatario, "))
                    {
                        select.Append("DestinatarioCTe.PCT_IERG IEDestinatario, ");
                        groupBy.Append("DestinatarioCTe.PCT_IERG, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "Destinatario":
                    if (!select.Contains(" Destinatario, "))
                    {
                        select.Append("DestinatarioCTe.PCT_NOME Destinatario, ");
                        groupBy.Append("DestinatarioCTe.PCT_NOME, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "CPFCNPJTomadorFormatado":
                    if (!select.Contains(" CPFCNPJTomador, "))
                    {
                        select.Append("TomadorPagadorCTe.PCT_CPF_CNPJ CPFCNPJTomador, TomadorPagadorCTe.PCT_TIPO TipoTomador, ");
                        groupBy.Append("TomadorPagadorCTe.PCT_CPF_CNPJ, TomadorPagadorCTe.PCT_TIPO, ");

                        SetarJoinsTomador(joins);
                    }
                    break;

                case "IETomador":
                    if (!select.Contains(" IETomador, "))
                    {
                        select.Append("TomadorPagadorCTe.PCT_IERG IETomador, ");
                        groupBy.Append("TomadorPagadorCTe.PCT_IERG, ");

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

                case "CPFCNPJExpedidorFormatado":
                    if (!select.Contains(" CPFCNPJExpedidor, "))
                    {
                        select.Append("ExpedidorCTe.PCT_CPF_CNPJ CPFCNPJExpedidor, ExpedidorCTe.PCT_TIPO TipoExpedidor, ");
                        groupBy.Append("ExpedidorCTe.PCT_CPF_CNPJ, ExpedidorCTe.PCT_TIPO, ");

                        SetarJoinsExpedidor(joins);
                    }
                    break;

                case "IEExpedidor":
                    if (!select.Contains(" IEExpedidor, "))
                    {
                        select.Append("ExpedidorCTe.PCT_IERG IEExpedidor, ");
                        groupBy.Append("ExpedidorCTe.PCT_IERG, ");

                        SetarJoinsExpedidor(joins);
                    }
                    break;

                case "Expedidor":
                    if (!select.Contains(" Expedidor, "))
                    {
                        select.Append("ExpedidorCTe.PCT_NOME Expedidor, ");
                        groupBy.Append("ExpedidorCTe.PCT_NOME, ");

                        SetarJoinsExpedidor(joins);
                    }
                    break;

                case "CPFCNPJRecebedorFormatado":
                    if (!select.Contains(" CPFCNPJRecebedor, "))
                    {
                        select.Append("RecebedorCTe.PCT_CPF_CNPJ CPFCNPJRecebedor, RecebedorCTe.PCT_TIPO TipoRecebedor, ");
                        groupBy.Append("RecebedorCTe.PCT_CPF_CNPJ, RecebedorCTe.PCT_TIPO, ");

                        SetarJoinsRecebedor(joins);
                    }
                    break;

                case "IERecebedor":
                    if (!select.Contains(" IERecebedor, "))
                    {
                        select.Append("RecebedorCTe.PCT_IERG IERecebedor, ");
                        groupBy.Append("RecebedorCTe.PCT_IERG, ");

                        SetarJoinsRecebedor(joins);
                    }
                    break;

                case "Recebedor":
                    if (!select.Contains(" Recebedor, "))
                    {
                        select.Append("RecebedorCTe.PCT_NOME Recebedor, ");
                        groupBy.Append("RecebedorCTe.PCT_NOME, ");

                        SetarJoinsRecebedor(joins);
                    }
                    break;

                case "DataEmissaoFormatada":
                    if (!select.Contains(" DataEmissao, "))
                    {
                        select.Append("CTe.CON_DATAHORAEMISSAO DataEmissao, ");
                        groupBy.Append("CTe.CON_DATAHORAEMISSAO, ");
                    }
                    break;

                case "AliquotaICMSInterna":
                    if (!somenteContarNumeroRegistros && !select.Contains("AliquotaICMSInterna"))
                        select.Append("SUM(CTe.CON_ALIQUOTA_ICMS_INTERNA) AliquotaICMSInterna, ");
                    break;

                case "PercentualICMSPartilha":
                    if (!somenteContarNumeroRegistros && !select.Contains("PercentualICMSPartilha"))
                        select.Append("AVG(CTe.CON_PERCENTUAL_ICMS_PARTILHA) PercentualICMSPartilha, ");
                    break;

                case "ValorICMSUFOrigem":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorICMSUFOrigem"))
                        select.Append("SUM(CTe.CON_VALOR_ICMS_UF_ORIGEM) ValorICMSUFOrigem, ");
                    break;

                case "ValorICMSUFDestino":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorICMSUFDestino"))
                        select.Append("SUM(CTe.CON_VALOR_ICMS_UF_DESTINO) ValorICMSUFDestino, ");
                    break;

                case "ValorICMSFCPFim":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorICMSFCPFim"))
                        select.Append("SUM(CTe.CON_VALOR_ICMS_FCP_DESTINO) ValorICMSFCPFim, ");
                    break;

                case "CaracteristicaTransporteCTe":
                    if (!select.Contains(" CaracteristicaTransporteCTe, "))
                    {
                        select.Append("CTe.CON_CARAC_TRANSP CaracteristicaTransporteCTe, ");
                        groupBy.Append("CTe.CON_CARAC_TRANSP, ");
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

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroCTe":
                    if (!select.Contains(" NumeroCTe, "))
                    {
                        select.Append("CTe.CON_NUM as NumeroCTe, ");
                        groupBy.Append("CTe.CON_NUM, ");
                    }
                    break;

                case "NumeroControle":
                    if (!select.Contains(" NumeroControle, "))
                    {
                        select.Append("CTe.CON_NUMERO_CONTROLE as NumeroControle, ");
                        groupBy.Append("CTe.CON_NUMERO_CONTROLE, ");
                    }
                    break;


            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioApuracaoICMS filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            where.Append(" and CTe.CON_STATUS = 'A' and CTe.CON_VALOR_ICMS_UF_DESTINO > 0 ");

            if (filtrosPesquisa.DataEmissaoInicial != DateTime.MinValue)
                where.Append(" and CTe.CON_DATAHORAEMISSAO >= '" + filtrosPesquisa.DataEmissaoInicial.ToString(pattern) + "'");

            if (filtrosPesquisa.DataEmissaoFinal != DateTime.MinValue)
                where.Append(" and CTe.CON_DATAHORAEMISSAO < '" + filtrosPesquisa.DataEmissaoFinal.AddDays(1).ToString(pattern) + "'");

            if (filtrosPesquisa.CnpjCpfRemetente > 0d)
            {
                where.Append(" and RemetenteCTe.CLI_CODIGO = " + filtrosPesquisa.CnpjCpfRemetente.ToString("F0"));

                SetarJoinsRemetente(joins);
            }

            if (filtrosPesquisa.CnpjCpfDestinatario > 0d)
            {
                where.Append(" and DestinatarioCTe.CLI_CODIGO = " + filtrosPesquisa.CnpjCpfDestinatario.ToString("F0"));

                SetarJoinsDestinatario(joins);
            }

            if (filtrosPesquisa.CnpjCpfTomador > 0d)
            {
                where.Append(" and TomadorPagadorCTe.CLI_CODIGO = " + filtrosPesquisa.CnpjCpfTomador.ToString("F0"));

                SetarJoinsTomador(joins);
            }

            if (filtrosPesquisa.CnpjCpfRecebedor > 0d)
            {
                where.Append(" and RecebedorCTe.CLI_CODIGO = " + filtrosPesquisa.CnpjCpfRecebedor.ToString("F0"));

                SetarJoinsRecebedor(joins);
            }

            if (filtrosPesquisa.CnpjCpfExpedidor > 0d)
            {
                where.Append(" and ExpedidorCTe.CLI_CODIGO = " + filtrosPesquisa.CnpjCpfExpedidor.ToString("F0"));

                SetarJoinsExpedidor(joins);
            }
        }

        #endregion
    }
}
