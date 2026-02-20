using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.CTe
{
    sealed class ConsultaTakeOrPay : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioTakeOrPay>
    {
        #region Construtores

        public ConsultaTakeOrPay() : base(tabela: "T_CTE as CTe") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCargaCTe(StringBuilder joins)
        {
            if (!joins.Contains(" CargaCTe "))
                joins.Append(" left join T_CARGA_CTE CargaCTe on CargaCTe.CON_CODIGO = CTe.CON_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsCargaCTe(joins);

            if (!joins.Contains(" Carga "))
                joins.Append(" left join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO ");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            SetarJoinsCargaCTe(joins);

            if (!joins.Contains(" CargaPedido "))
                joins.Append(" left join T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = CargaCTe.CAR_CODIGO ");
        }

        private void SetarJoinsPedido(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Pedido "))
                joins.Append(" left join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
        }

        private void SetarJoinsTomador(StringBuilder joins)
        {
            if (!joins.Contains(" TomadorPagadorCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE TomadorPagadorCTe on CTe.CON_TOMADOR_PAGADOR_CTE = TomadorPagadorCTe.PCT_CODIGO ");
        }

        private void SetarJoinsTomadorCliente(StringBuilder joins)
        {
            SetarJoinsTomador(joins);

            if (!joins.Contains(" ClienteTomador "))
                joins.Append(" left join T_CLIENTE ClienteTomador on ClienteTomador.CLI_CGCCPF = TomadorPagadorCTe.CLI_CODIGO ");
        }

        private void SetarJoinsTomadorGrupo(StringBuilder joins)
        {
            SetarJoinsTomador(joins);

            if (!joins.Contains(" GrupoTomadorPagadorCTe "))
                joins.Append(" left join T_GRUPO_PESSOAS GrupoTomadorPagadorCTe on GrupoTomadorPagadorCTe.GRP_CODIGO = TomadorPagadorCTe.GRP_CODIGO ");
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

        private void SetarJoinsViagem(StringBuilder joins)
        {
            if (!joins.Contains(" Viagem "))
                joins.Append(" left join T_PEDIDO_VIAGEM_NAVIO Viagem on Viagem.PVN_CODIGO = CTe.CON_VIAGEM ");
        }

        private void SetarJoinsNavio(StringBuilder joins)
        {
            SetarJoinsViagem(joins);

            if (!joins.Contains(" Navio "))
                joins.Append(" left join T_NAVIO Navio on Navio.NAV_CODIGO = Viagem.NAV_CODIGO ");
        }

        private void SetarJoinsDocumentoFaturamento(StringBuilder joins)
        {
            if (!joins.Contains(" DocumentoFaturamentoCTe "))
                joins.Append(" left join T_DOCUMENTO_FATURAMENTO DocumentoFaturamentoCTe on CTe.CON_CODIGO = DocumentoFaturamentoCTe.CON_CODIGO");
        }

        private void SetarJoinsFaturaDocumento(StringBuilder joins)
        {
            SetarJoinsDocumentoFaturamento(joins);

            if (!joins.Contains(" FaturaDocumento "))
                joins.Append(" left join T_FATURA_DOCUMENTO FaturaDocumento on FaturaDocumento.DFA_CODIGO = DocumentoFaturamentoCTe.DFA_CODIGO");
        }

        private void SetarJoinsFatura(StringBuilder joins)
        {
            SetarJoinsFaturaDocumento(joins);

            if (!joins.Contains(" Fatura "))
                joins.Append(" left join T_FATURA Fatura on Fatura.FAT_CODIGO = FaturaDocumento.FAT_CODIGO");
        }

        private void SetarJoinsFaturaParcela(StringBuilder joins)
        {
            SetarJoinsFatura(joins);

            if (!joins.Contains(" FaturaParcela "))
                joins.Append(" left join T_FATURA_PARCELA FaturaParcela on FaturaParcela.FAT_CODIGO = Fatura.FAT_CODIGO");
        }

        private void SetarJoinsTituloParcela(StringBuilder joins)
        {
            SetarJoinsFaturaParcela(joins);

            if (!joins.Contains(" TituloParcela "))
                joins.Append(" left join T_TITULO TituloParcela on TituloParcela.FAP_CODIGO = FaturaParcela.FAP_CODIGO");
        }

        private void SetarJoinsTituloDocumentoCTe(StringBuilder joins)
        {
            if (!joins.Contains(" TituloDocumentoCTe "))
                joins.Append(" left join T_TITULO_DOCUMENTO TituloDocumentoCTe on TituloDocumentoCTe.CON_CODIGO = CTe.CON_CODIGO");
        }

        private void SetarJoinsFaturaIntegracao(StringBuilder joins)
        {
            SetarJoinsFaturaDocumento(joins);

            if (!joins.Contains(" FaturaIntegracao "))
                joins.Append(" left join T_FATURA_INTEGRACAO FaturaIntegracao on FaturaIntegracao.FAT_CODIGO = FaturaDocumento.FAT_CODIGO");
        }

        private void SetarJoinsFaturaIntegracaoIntegracaoArquivo(StringBuilder joins)
        {
            SetarJoinsFaturaIntegracao(joins);

            if (!joins.Contains(" FaturaIntegracaoIntegracaoArquivo "))
                joins.Append(" left join T_FATURA_INTEGRACAO_INTEGRACAO_ARQUIVO FaturaIntegracaoIntegracaoArquivo on FaturaIntegracaoIntegracaoArquivo.FAI_CODIGO = FaturaIntegracao.FAI_CODIGO");
        }

        private void SetarJoinsFaturaIntegracaoArquivo(StringBuilder joins)
        {
            SetarJoinsFaturaIntegracaoIntegracaoArquivo(joins);

            if (!joins.Contains(" FaturaIntegracaoArquivo "))
                joins.Append(" left join T_FATURA_INTEGRACAO_ARQUIVO FaturaIntegracaoArquivo on FaturaIntegracaoArquivo.FIA_CODIGO = FaturaIntegracaoIntegracaoArquivo.FIA_CODIGO");
        }

        private void SetarJoinsAcordoFaturamentoCliente(StringBuilder joins)
        {
            SetarJoinsTomador(joins);

            if (!joins.Contains(" AcordoFaturamentoCliente "))
                joins.Append(" left join T_ACORDO_FATURAMENTO_CLIENTE AcordoFaturamentoCliente on AcordoFaturamentoCliente.GRP_CODIGO = TomadorPagadorCTe.GRP_CODIGO");
        }

        private void SetarJoinsViagemScheduleDestino(StringBuilder joins)
        {
            if (!joins.Contains(" ViagemScheduleDestino "))
                joins.Append(" left join T_PEDIDO_VIAGEM_NAVIO_SCHEDULE ViagemScheduleDestino on ViagemScheduleDestino.PVN_CODIGO = CTe.CON_VIAGEM AND ViagemScheduleDestino.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_ORIGEM ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" TransportadorCTe "))
                joins.Append(" inner join T_EMPRESA TransportadorCTe on CTe.EMP_CODIGO = TransportadorCTe.EMP_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioTakeOrPay filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO_CARGA_EMBARCADOR"))
                            groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "CPFCNPJTomador":
                    if (!select.Contains(" CPFCNPJTomador, "))
                    {
                        select.Append("TomadorPagadorCTe.PCT_CPF_CNPJ CPFCNPJTomador, ");

                        if (!groupBy.Contains("TomadorPagadorCTe.PCT_CPF_CNPJ"))
                            groupBy.Append("TomadorPagadorCTe.PCT_CPF_CNPJ, ");

                        SetarJoinsTomador(joins);
                    }
                    break;

                case "Tomador":
                    if (!select.Contains(" Tomador, "))
                    {
                        select.Append("TomadorPagadorCTe.PCT_NOME Tomador, ");

                        if (!groupBy.Contains("CTe.CON_TOMADOR_PAGADOR_CTE"))
                            groupBy.Append("CTe.CON_TOMADOR_PAGADOR_CTE, ");

                        if (!groupBy.Contains("TomadorPagadorCTe.PCT_NOME"))
                            groupBy.Append("TomadorPagadorCTe.PCT_NOME, ");

                        SetarJoinsTomador(joins);
                    }
                    break;

                case "GrupoTomador":
                    if (!select.Contains(" GrupoTomador, "))
                    {
                        select.Append("GrupoTomadorPagadorCTe.GRP_DESCRICAO GrupoTomador, ");

                        if (!groupBy.Contains("GrupoTomadorPagadorCTe.GRP_DESCRICAO"))
                            groupBy.Append("GrupoTomadorPagadorCTe.GRP_DESCRICAO, ");

                        SetarJoinsTomadorGrupo(joins);
                    }
                    break;

                case "CodigoDocumentoTomador":
                    if (!select.Contains(" CodigoDocumentoTomador, "))
                    {
                        select.Append("ClienteTomador.CLI_CODIGO_DOCUMENTO CodigoDocumentoTomador, ");
                        groupBy.Append("ClienteTomador.CLI_CODIGO_DOCUMENTO, ");

                        SetarJoinsTomadorCliente(joins);
                    }
                    break;

                case "ValorFrete":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorFrete"))
                        select.Append("SUM(CTe.CON_VALOR_FRETE) ValorFrete, ");
                    break;

                case "SituacaoFatura":
                    if (!select.Contains(" SituacaoFatura, "))
                    {
                        select.Append("CASE WHEN Fatura.FAT_SITUACAO = 1 THEN 'Em Andamento' WHEN Fatura.FAT_SITUACAO = 2 THEN 'Fechado' WHEN Fatura.FAT_SITUACAO = 3 THEN 'Cancelado' ELSE 'Em Andamento' END SituacaoFatura, ");
                        groupBy.Append("Fatura.FAT_SITUACAO, ");

                        SetarJoinsFatura(joins);
                    }
                    break;

                case "NumeroFatura":
                    if (!select.Contains(" NumeroFatura, "))
                    {
                        select.Append("Fatura.FAT_NUMERO as NumeroFatura, ");
                        groupBy.Append("Fatura.FAT_NUMERO, ");

                        SetarJoinsFatura(joins);
                    }
                    break;

                case "DataFaturaFormatada":
                    if (!select.Contains(" DataFatura, "))
                    {
                        select.Append("Fatura.FAT_DATA_FATURA as DataFatura, ");
                        groupBy.Append("Fatura.FAT_DATA_FATURA, ");

                        SetarJoinsFatura(joins);
                    }
                    break;

                case "NumeroBoleto":
                    if (!select.Contains(" NumeroBoleto, "))
                    {
                        select.Append("TituloParcela.TIT_NOSSO_NUMERO as NumeroBoleto, ");

                        if (!groupBy.Contains("TituloParcela.TIT_NOSSO_NUMERO"))
                            groupBy.Append("TituloParcela.TIT_NOSSO_NUMERO, ");

                        SetarJoinsTituloParcela(joins);
                    }
                    break;

                case "DataVencimentoBoletoFormatada":
                    if (!select.Contains(" DataVencimentoBoleto, "))
                    {
                        select.Append("TituloParcela.TIT_DATA_VENCIMENTO DataVencimentoBoleto, ");
                        groupBy.Append("TituloParcela.TIT_DATA_VENCIMENTO, ");

                        SetarJoinsTituloParcela(joins);
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

                case "PortoOrigem":
                    if (!select.Contains(" PortoOrigem, "))
                    {
                        select.Append("PortoOrigem.POT_DESCRICAO PortoOrigem, ");
                        groupBy.Append("PortoOrigem.POT_DESCRICAO, ");

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

                case "PortoDestino":
                    if (!select.Contains(" PortoDestino, "))
                    {
                        select.Append("PortoDestino.POT_DESCRICAO PortoDestino, ");
                        groupBy.Append("PortoDestino.POT_DESCRICAO, ");

                        SetarJoinsPortoDestino(joins);
                    }
                    break;

                case "Viagem":
                    if (!select.Contains(" Viagem, "))
                    {
                        select.Append("Viagem.PVN_DESCRICAO Viagem, ");
                        groupBy.Append("Viagem.PVN_DESCRICAO, ");

                        SetarJoinsViagem(joins);
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

                case "EmailsFaturaIntegracao":
                    if (!select.Contains(" EmailsFaturaIntegracao, "))
                    {
                        select.Append("REPLACE(FaturaIntegracaoArquivo.FIA_MENSAGEM, 'Email(s): ', '') as EmailsFaturaIntegracao, ");
                        groupBy.Append("FaturaIntegracaoArquivo.FIA_MENSAGEM, ");

                        SetarJoinsFaturaIntegracaoArquivo(joins);
                    }
                    break;

                case "EmailFatura":
                    SetarSelect("EmailsFaturaIntegracao", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("NumeroBoleto", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "DiasPrazoFaturamento":
                    if (!select.Contains(" DiasPrazoFaturamento, "))
                    {
                        select.Append("AcordoFaturamentoCliente.AFC_TAKE_OR_PAY_DIA_DE_PRAZO_FATURA DiasPrazoFaturamento, ");
                        groupBy.Append("AcordoFaturamentoCliente.AFC_TAKE_OR_PAY_DIA_DE_PRAZO_FATURA, ");

                        SetarJoinsAcordoFaturamentoCliente(joins);
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

                case "DescricaoTipoProposta":
                    if (!select.Contains(" TipoProposta, "))
                    {
                        select.Append("CargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL TipoProposta, ");
                        groupBy.Append("CargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "QtdDisponibilizada":
                    if (!select.Contains(" QtdDisponibilizada, "))
                    {
                        select.Append("Pedido.PED_QTD_DISPONIBILIZADA QtdDisponibilizada, ");
                        groupBy.Append("Pedido.PED_QTD_DISPONIBILIZADA, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "QtdNaoEmbarcadas":
                    if (!select.Contains(" QtdNaoEmbarcadas, "))
                    {
                        select.Append("Pedido.PED_QTD_NAO_EMBARCADA QtdNaoEmbarcadas, ");
                        groupBy.Append("Pedido.PED_QTD_NAO_EMBARCADA, ");

                        SetarJoinsPedido(joins);
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

                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select.Append("TransportadorCTe.EMP_RAZAO Transportador, ");
                        groupBy.Append("TransportadorCTe.EMP_RAZAO, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "ObservacaoFatura":
                    if (!select.Contains(" ObservacaoFatura, "))
                    {
                        select.Append("Fatura.FAT_OBSERVACAO_FATURA ObservacaoFatura, ");
                        groupBy.Append("Fatura.FAT_OBSERVACAO_FATURA, ");

                        SetarJoinsFatura(joins);
                    }
                    break;

                case "UnidadesContabilidade":
                    if (!select.Contains(" UnidadesContabilidade, "))
                    {
                        select.Append("Pedido.PED_UNIDADES_CONTABILIDADE UnidadesContabilidade, ");
                        groupBy.Append("Pedido.PED_UNIDADES_CONTABILIDADE, ");

                        SetarJoinsPedido(joins);
                    }
                    break;
                case "AliquotaICMSContabilidade":
                    if (!select.Contains(" AliquotaICMSContabilidade, "))
                    {
                        select.Append("Pedido.PED_ALIQUOTA_ICMS_CONTABILIDADE AliquotaICMSContabilidade, ");
                        groupBy.Append("Pedido.PED_ALIQUOTA_ICMS_CONTABILIDADE, ");

                        SetarJoinsPedido(joins);
                    }
                    break;
                case "ValorICMSContabilidade":
                    if (!select.Contains(" ValorICMSContabilidade, "))
                    {
                        select.Append("Pedido.PED_VALOR_ICMS_CONTABILIDADE ValorICMSContabilidade, ");
                        groupBy.Append("Pedido.PED_VALOR_ICMS_CONTABILIDADE, ");

                        SetarJoinsPedido(joins);
                    }
                    break;
                case "PTAXContabilidade":
                    if (!select.Contains(" PTAXContabilidade, "))
                    {
                        select.Append("Pedido.PED_PTAX_CONTABILIDADE PTAXContabilidade, ");
                        groupBy.Append("Pedido.PED_PTAX_CONTABILIDADE, ");

                        SetarJoinsPedido(joins);
                    }
                    break;
                case "ValorUSDContabilidade":
                    if (!select.Contains(" ValorUSDContabilidade, "))
                    {
                        select.Append("Pedido.PED_VALOR_USD_CONTABILIDADE ValorUSDContabilidade, ");
                        groupBy.Append("Pedido.PED_VALOR_USD_CONTABILIDADE, ");

                        SetarJoinsPedido(joins);
                    }
                    break;
                case "PONumberContabilidade":
                    if (!select.Contains(" PONumberContabilidade, "))
                    {
                        select.Append("Pedido.PED_PO_NUMBER_CONTABILIDADE PONumberContabilidade, ");
                        groupBy.Append("Pedido.PED_PO_NUMBER_CONTABILIDADE, ");

                        SetarJoinsPedido(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioTakeOrPay filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            SetarJoinsFaturaParcela(joins);
            SetarJoinsCarga(joins);

            where.Append(" and Carga.CAR_CARGA_TAKE_OR_PAY = 1");

            if (filtrosPesquisa.DataInicialFatura != DateTime.MinValue)
                where.Append("  and CAST(Fatura.FAT_DATA_FATURA AS DATE) >= '" + filtrosPesquisa.DataInicialFatura.ToString(pattern) + "'");

            if (filtrosPesquisa.DataFinalFatura != DateTime.MinValue)
                where.Append("  and CAST(Fatura.FAT_DATA_FATURA AS DATE) <= '" + filtrosPesquisa.DataFinalFatura.ToString(pattern) + "'");

            if (filtrosPesquisa.CodigoCarga > 0)
                where.Append("  and Carga.CAR_CODIGO = " + filtrosPesquisa.CodigoCarga);

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
            {
                where.Append($"  and GrupoTomadorPagadorCTe.GRP_CODIGO = {filtrosPesquisa.CodigoGrupoPessoas}");

                SetarJoinsTomadorGrupo(joins);
            }

            if (filtrosPesquisa.DataInicialFatura != DateTime.MinValue)
                where.Append("  and CAST(Fatura.FAT_DATA_FATURA AS DATE) >= '" + filtrosPesquisa.DataInicialFatura.ToString(pattern) + "'");

            if (filtrosPesquisa.DataFinalFatura != DateTime.MinValue)
                where.Append("  and CAST(Fatura.FAT_DATA_FATURA AS DATE) <= '" + filtrosPesquisa.DataFinalFatura.ToString(pattern) + "'");

            if (filtrosPesquisa.SituacaoFatura.HasValue)
                where.Append("  and Fatura.FAT_SITUACAO = " + filtrosPesquisa.SituacaoFatura.Value.ToString("D"));

            if (filtrosPesquisa.NumeroFatura > 0)
                where.Append($"  and Fatura.FAT_NUMERO = {filtrosPesquisa.NumeroFatura}");

            if (filtrosPesquisa.CodigoViagem > 0)
                where.Append($" and CTe.CON_VIAGEM = {filtrosPesquisa.CodigoViagem}");

            if (filtrosPesquisa.CodigoPortoOrigem > 0)
                where.Append($" and CTe.POT_CODIGO_ORIGEM = {filtrosPesquisa.CodigoPortoOrigem}");

            if (filtrosPesquisa.CodigoPortoDestino > 0)
                where.Append($" and CTe.POT_CODIGO_DESTINO = {filtrosPesquisa.CodigoPortoDestino}");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBoleto))
            {
                where.Append($" and TituloParcela.TIT_NOSSO_NUMERO = '{filtrosPesquisa.NumeroBoleto}'");

                SetarJoinsTituloParcela(joins);
            }

            if (filtrosPesquisa.TipoProposta.Count > 0)
            {
                where.Append($" and CargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL in ({ string.Join(", ", filtrosPesquisa.TipoProposta.Select(o => o.ToString("D"))) })");

                SetarJoinsCargaPedido(joins);
            }

            if (filtrosPesquisa.DataInicialPrevisaoSaidaNavio != DateTime.MinValue)
            {
                where.Append(" and CAST(ViagemScheduleDestino.PVS_DATA_PREVISAO_SAIDA_NAVIO AS DATE) >= '" + filtrosPesquisa.DataInicialPrevisaoSaidaNavio.ToString(pattern) + "'");

                SetarJoinsViagemScheduleDestino(joins);
            }

            if (filtrosPesquisa.DataFinalPrevisaoSaidaNavio != DateTime.MinValue)
            {
                where.Append(" and CAST(ViagemScheduleDestino.PVS_DATA_PREVISAO_SAIDA_NAVIO AS DATE) <= '" + filtrosPesquisa.DataFinalPrevisaoSaidaNavio.ToString(pattern) + "'");

                SetarJoinsViagemScheduleDestino(joins);
            }
        }

        #endregion
    }
}
