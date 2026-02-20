using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Embarcador.Documentos
{
    sealed class ConsultaDadosDocsys : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroDadosDocsys>
    {
        #region Construtores

        public ConsultaDadosDocsys() : base(tabela: "T_DADOS_DOCSYS Docs") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCTe(StringBuilder joins)
        {
            if (!joins.Contains(" CTe "))
                joins.Append(" JOIN T_CTE CTe on Docs.CON_CODIGO = CTe.CON_CODIGO ");
        }

        private void SetarJoinsOrigem(StringBuilder joins)
        {
            if (!joins.Contains(" Origem "))
                joins.Append(" JOIN T_LOCALIDADES Origem on Origem.LOC_CODIGO = CTe.CON_LOCINICIOPRESTACAO ");
        }

        private void SetarJoinsDestino(StringBuilder joins)
        {
            if (!joins.Contains(" Destino "))
                joins.Append(" JOIN T_LOCALIDADES Destino on Destino.LOC_CODIGO = CTe.CON_LOCTERMINOPRESTACAO ");
        }

        private void SetarJoinsTomador(StringBuilder joins)
        {
            if (!joins.Contains(" Tomador "))
                joins.Append(" JOIN T_CTE_PARTICIPANTE Tomador on Tomador.PCT_CODIGO = CTe.CON_TOMADOR_PAGADOR_CTE ");
        }

        private void SetarJoinsSchedule(StringBuilder joins)
        {
            if (!joins.Contains(" Schedule "))
                joins.Append(" LEFT OUTER JOIN T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule on Schedule.PVN_CODIGO = CTe.CON_VIAGEM and Schedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_ORIGEM ");
        }

        private void SetarJoinsViagem(StringBuilder joins)
        {
            if (!joins.Contains(" Viagem "))
                joins.Append(" JOIN T_PEDIDO_VIAGEM_NAVIO Viagem on Viagem.PVN_CODIGO = CTe.CON_VIAGEM ");
        }

        private void SetarJoinsPortoOrigem(StringBuilder joins)
        {
            if (!joins.Contains(" PortoOrigem "))
                joins.Append(" JOIN T_PORTO PortoOrigem on PortoOrigem.POT_CODIGO = CTe.POT_CODIGO_ORIGEM ");
        }

        private void SetarJoinsPortoDestino(StringBuilder joins)
        {
            if (!joins.Contains(" PortoDestino "))
                joins.Append(" JOIN T_PORTO PortoDestino on PortoDestino.POT_CODIGO = CTe.POT_CODIGO_DESTINO ");
        }

        private void SetarJoinsCargaCTe(StringBuilder joins)
        {
            if (!joins.Contains(" CargaCTe "))
                joins.Append(" JOIN T_CARGA_CTE CargaCTe on CargaCTe.CON_CODIGO = CTe.CON_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append(" JOIN T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" LEFT OUTER JOIN T_TIPO_OPERACAO TipoOperacao on Carga.TOP_CODIGO = TipoOperacao.TOP_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroDadosDocsys filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Viagem":
                case "ViagemBookingCTe":
                    if (!select.Contains(" Viagem,"))
                    {
                        select.Append("Viagem.PVN_DESCRICAO Viagem, ");
                        groupBy.Append("Viagem.PVN_DESCRICAO, ");

                        SetarJoinsCTe(joins);
                        SetarJoinsViagem(joins);
                    }
                    break;
                case "PortoOrigem":
                    if (!select.Contains(" PortoOrigem,"))
                    {
                        select.Append("PortoOrigem.POT_DESCRICAO PortoOrigem, ");
                        groupBy.Append("PortoOrigem.POT_DESCRICAO, ");

                        SetarJoinsCTe(joins);
                        SetarJoinsPortoOrigem(joins);
                    }
                    break;
                case "PortoDestino":
                    if (!select.Contains(" PortoDestino,"))
                    {
                        select.Append("PortoDestino.POT_DESCRICAO PortoDestino, ");
                        groupBy.Append("PortoDestino.POT_DESCRICAO, ");

                        SetarJoinsCTe(joins);
                        SetarJoinsPortoDestino(joins);
                    }
                    break;
                case "Booking":
                    if (!select.Contains(" Booking,"))
                    {
                        select.Append("CTe.CON_NUMERO_BOOKING Booking, ");
                        groupBy.Append("CTe.CON_NUMERO_BOOKING, ");

                        SetarJoinsCTe(joins);
                    }
                    break;
                case "NumeroControle":
                    if (!select.Contains(" NumeroControle,"))
                    {
                        select.Append("CTe.CON_NUMERO_CONTROLE NumeroControle, ");
                        groupBy.Append("CTe.CON_NUMERO_CONTROLE, ");

                        SetarJoinsCTe(joins);
                    }
                    break;
                case "TipoCTE":
                case "TIPO_CTE":                    
                case "TipoCTeFormatado":
                    if (!select.Contains(" TipoCTE,"))
                    {
                        select.Append("CTe.CON_TIPO_CTE TipoCTE, ");
                        groupBy.Append("CTe.CON_TIPO_CTE, ");

                        SetarJoinsCTe(joins);
                    }
                    break;
                case "ValorReceber":
                    if (!select.Contains(" ValorReceber,"))
                    {
                        select.Append("CTe.CON_VALOR_RECEBER ValorReceber, ");
                        groupBy.Append("CTe.CON_VALOR_RECEBER, ");

                        SetarJoinsCTe(joins);
                    }
                    break;
                case "ValorPrestacao":
                    if (!select.Contains(" ValorPrestacao,"))
                    {
                        select.Append("CTe.CON_VALOR_PREST_SERVICO ValorPrestacao, ");
                        groupBy.Append("CTe.CON_VALOR_PREST_SERVICO, ");

                        SetarJoinsCTe(joins);
                    }
                    break;
                case "PrevisaoSaida":
                case "PrevisaoSaidaFormatado":
                case "CortePrevisaoSaida":
                    if (!select.Contains(" PrevisaoSaida,"))
                    {
                        select.Append("Schedule.PVS_DATA_PREVISAO_SAIDA_NAVIO PrevisaoSaida, ");
                        groupBy.Append("Schedule.PVS_DATA_PREVISAO_SAIDA_NAVIO, ");

                        SetarJoinsCTe(joins);
                        SetarJoinsSchedule(joins);
                    }
                    break;
                case "Emissao":
                case "EmissaoFormatado":
                case "DiaEmissao":
                case "AnoEmissao":
                case "CorteEmissao":
                    if (!select.Contains(" Emissao,"))
                    {
                        select.Append("CTe.CON_DATAHORAEMISSAO Emissao, ");
                        groupBy.Append("CTe.CON_DATAHORAEMISSAO, ");

                        SetarJoinsCTe(joins);
                    }
                    break;
                case "VoucherNO":
                    if (!select.Contains(" VoucherNO,"))
                    {
                        select.Append("Docs.DAD_VOUCHER_NO VoucherNO, ");
                        groupBy.Append("Docs.DAD_VOUCHER_NO, ");

                    }
                    break;
                case "VoucherDate":
                case "VoucherDateFormatado":
                    if (!select.Contains(" VoucherDate,"))
                    {
                        select.Append("Docs.TDO_VOUCHER_DATE VoucherDate, ");
                        groupBy.Append("Docs.TDO_VOUCHER_DATE, ");

                    }
                    break;
                case "DACS_transf":
                case "DACS_transfFormatado":
                case "StatusDocsys":
                case "MesContabil":
                    if (!select.Contains(" DACS_transf,"))
                    {
                        select.Append("Docs.TDO_DACS_TRANSF DACS_transf, ");
                        groupBy.Append("Docs.TDO_DACS_TRANSF, ");

                    }
                    break;
                case "UBLI":
                    if (!select.Contains(" UBLI,"))
                    {
                        select.Append("Docs.DAD_UBLI UBLI, ");
                        groupBy.Append("Docs.DAD_UBLI, ");

                    }
                    break;
                case "DataInclusao":
                case "DataInclusaoFormatado":
                    if (!select.Contains(" DataInclusao,"))
                    {
                        select.Append("Docs.TDO_DATA_INCLUSAO DataInclusao, ");
                        groupBy.Append("Docs.TDO_DATA_INCLUSAO, ");

                    }
                    break;
                case "CorrCode":
                case "CanceladosAnulados":
                    if (!select.Contains(" CorrCode,"))
                    {
                        select.Append("Docs.DAD_CORR_CODE CorrCode, ");
                        groupBy.Append("Docs.DAD_CORR_CODE, ");

                    }
                    break;
                case "BLVersion":
                    if (!select.Contains(" BLVersion,"))
                    {
                        select.Append("Docs.DAD_BL_VERSION BLVersion, ");
                        groupBy.Append("Docs.DAD_BL_VERSION, ");

                    }
                    break;
                case "TipoOperacao":
                case "TipoEmissao":
                    if (!select.Contains(" TipoOperacao,"))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO TipoOperacao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinsCTe(joins);
                        SetarJoinsCargaCTe(joins);
                        SetarJoinsCarga(joins);
                        SetarJoinsTipoOperacao(joins);
                    }
                    break;
                case "Status":
                case "StatusFormatado":
                case "ConsiderarDesconsiderar":
                    if (!select.Contains(" Status,"))
                    {
                        select.Append("CTe.CON_STATUS Status, ");
                        groupBy.Append("CTe.CON_STATUS, ");

                        SetarJoinsCTe(joins);
                    }
                    break;
                case "Tomador":
                    if (!select.Contains(" Tomador,"))
                    {
                        select.Append("Tomador.PCT_NOME Tomador, ");
                        groupBy.Append("Tomador.PCT_NOME, ");

                        SetarJoinsCTe(joins);
                        SetarJoinsTomador(joins);
                    }
                    break;
                case "PossuiCartaCorrecao":
                case "PossuiCartaCorrecaoFormatado":
                    if (!select.Contains(" PossuiCartaCorrecao,"))
                    {
                        select.Append("CTe.CON_POSSUI_CARTA_CORRECAO PossuiCartaCorrecao, ");
                        groupBy.Append("CTe.CON_POSSUI_CARTA_CORRECAO, ");

                        SetarJoinsCTe(joins);
                    }
                    break;
                case "FoiAnulado":
                case "FoiAnuladoFormatado":
                    if (!select.Contains(" FoiAnulado,"))
                    {
                        select.Append("CTe.CON_POSSUI_ANULACAO_SUBSTITUICAO FoiAnulado, ");
                        groupBy.Append("CTe.CON_POSSUI_ANULACAO_SUBSTITUICAO, ");

                        SetarJoinsCTe(joins);
                    }
                    break;
                case "PossuiComplemento":
                case "PossuiComplementoFormatado":
                    if (!select.Contains(" PossuiComplemento,"))
                    {
                        select.Append("CTe.CON_POSSUI_CTE_COMPLEMENTAR PossuiComplemento, ");
                        groupBy.Append("CTe.CON_POSSUI_CTE_COMPLEMENTAR, ");

                        SetarJoinsCTe(joins);
                    }
                    break;
                case "FoiSubstituido":
                case "FoiSubstituidoFormatado":
                    if (!select.Contains(" FoiSubstituido,"))
                    {
                        select.Append("(select top(1) count(1) from t_cte _cte where _cte.CON_CHAVE_CTE_SUB_COMP = CTe.CON_CHAVECTE) FoiSubstituido, ");
                        groupBy.Append("CTe.CON_CHAVECTE, ");

                        SetarJoinsCTe(joins);
                    }
                    break;
                case "TrackingData":
                case "TrackingDataFormatado":
                    if (!select.Contains(" TrackingData,"))
                    {
                        select.Append(@"(SELECT top(1) Tracking.TDO_DATA_GERACAO FROM T_TRACKING_DOCUMENTACAO Tracking where Tracking.PVN_CODIGO = CTe.CON_VIAGEM and Tracking.POT_CODIGO_ORIGEM = CTe.POT_CODIGO_ORIGEM order by Tracking.TDO_DATA_GERACAO desc) TrackingData, ");
                        groupBy.Append("CTe.CON_VIAGEM, CTe.POT_CODIGO_ORIGEM, ");

                        SetarJoinsCTe(joins);
                    }
                    break;                    
                case "DataEmissaoFatura":
                case "DataEmissaoFaturaFormatado":
                    if (!select.Contains(" DataEmissaoFatura,"))
                    {
                        select.Append(@"ISNULL((SELECT top(1) Titulo.TIT_DATA_VENCIMENTO FROM T_TITULO_DOCUMENTO TitDoc JOIN T_TITULO Titulo on Titulo.TIT_CODIGO = TitDoc.TIT_CODIGO where Titulo.TIT_STATUS <> 4 AND TitDoc.CON_CODIGO = CTe.CON_CODIGO),
                                (SELECT top(1) Fat.FAT_DATA_FATURA FROM T_DOCUMENTO_FATURAMENTO DocFat JOIN T_FATURA_DOCUMENTO FatDoc on FatDoc.DFA_CODIGO = DocFat.DFA_CODIGO JOIN T_FATURA Fat on Fat.FAT_CODIGO = FatDoc.FAT_CODIGO and Fat.FAT_SITUACAO = 2 where DocFat.CON_CODIGO = CTe.CON_CODIGO) ) DataEmissaoFatura, ");
                        groupBy.Append("CTe.CON_CODIGO, ");

                        SetarJoinsCTe(joins);
                    }
                    break;
                case "FoiFaturado":
                case "FoiFaturadoFormatado":
                    if (!select.Contains(" FoiFaturado,"))
                    {
                        select.Append(@"ISNULL((SELECT top(1) 1 FROM T_TITULO_DOCUMENTO TitDoc JOIN T_TITULO Titulo on Titulo.TIT_CODIGO = TitDoc.TIT_CODIGO where Titulo.TIT_STATUS <> 4 AND TitDoc.CON_CODIGO = CTe.CON_CODIGO),
                                ISNULL((SELECT top(1) 1 FROM T_DOCUMENTO_FATURAMENTO DocFat JOIN T_FATURA_DOCUMENTO FatDoc on FatDoc.DFA_CODIGO = DocFat.DFA_CODIGO JOIN T_FATURA Fat on Fat.FAT_CODIGO = FatDoc.FAT_CODIGO and Fat.FAT_SITUACAO = 2 where DocFat.CON_CODIGO = CTe.CON_CODIGO), 0))FoiFaturado, ");
                        groupBy.Append("CTe.CON_CODIGO, ");

                        SetarJoinsCTe(joins);
                    }
                    break;
                case "IBGEOrigem":
                case "IBGEDestino":                
                    if (!select.Contains(" IBGEOrigem,"))
                    {
                        select.Append("Origem.LOC_IBGE IBGEOrigem, ");
                        groupBy.Append("Origem.LOC_IBGE, ");

                        SetarJoinsCTe(joins);
                        SetarJoinsOrigem(joins);
                    }
                    if (!select.Contains(" IBGEDestino,"))
                    {
                        select.Append("Destino.LOC_IBGE IBGEDestino, ");
                        groupBy.Append("Destino.LOC_IBGE, ");

                        SetarJoinsCTe(joins);
                        SetarJoinsDestino(joins);
                    }
                    break;
                case "EstadoOrigem":
                case "EstadoDestino":
                case "EstadosIguais":
                    if (!select.Contains(" EstadoOrigem,"))
                    {
                        select.Append("Origem.UF_SIGLA EstadoOrigem, ");
                        groupBy.Append("Origem.UF_SIGLA, ");

                        SetarJoinsCTe(joins);
                        SetarJoinsOrigem(joins);
                    }
                    if (!select.Contains(" EstadoDestino,"))
                    {
                        select.Append("Destino.UF_SIGLA EstadoDestino, ");
                        groupBy.Append("Destino.UF_SIGLA, ");

                        SetarJoinsCTe(joins);
                        SetarJoinsDestino(joins);
                    }
                    if (!select.Contains(" IBGEOrigem,"))
                    {
                        select.Append("Origem.LOC_IBGE IBGEOrigem, ");
                        groupBy.Append("Origem.LOC_IBGE, ");

                        SetarJoinsCTe(joins);
                        SetarJoinsOrigem(joins);
                    }
                    if (!select.Contains(" IBGEDestino,"))
                    {
                        select.Append("Destino.LOC_IBGE IBGEDestino, ");
                        groupBy.Append("Destino.LOC_IBGE, ");

                        SetarJoinsCTe(joins);
                        SetarJoinsDestino(joins);
                    }
                    break;
                case "Duplicado":
                case "DuplicadoFormatado":                    
                    if (!select.Contains(" Duplicado,"))
                    {
                        select.Append("Docs.DAD_DUPLICADO Duplicado, ");
                        groupBy.Append("Docs.DAD_DUPLICADO, ");

                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroDadosDocsys filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (filtrosPesquisa.DataEmissaoInicial != DateTime.MinValue || filtrosPesquisa.DataEmissaoFinal != DateTime.MinValue)
            {
                if (filtrosPesquisa.DataEmissaoInicial != DateTime.MinValue)
                    where.Append(" AND CTe.CON_DATAHORAEMISSAO >= '" + filtrosPesquisa.DataEmissaoInicial.ToString("yyyy-MM-dd") + "'");

                if (filtrosPesquisa.DataEmissaoFinal != DateTime.MinValue)
                    where.Append(" AND CTe.CON_DATAHORAEMISSAO <= '" + filtrosPesquisa.DataEmissaoFinal.ToString("yyyy-MM-dd") + " 23:59:59'");

                SetarJoinsCTe(joins);
            }


            if (filtrosPesquisa.PedidoViagemNavio > 0)
            {
                where.Append(" AND CTe.CON_VIAGEM = " + filtrosPesquisa.PedidoViagemNavio.ToString());

                SetarJoinsCTe(joins);
            }
        }

        #endregion
    }
}
