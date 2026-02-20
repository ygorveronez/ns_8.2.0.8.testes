using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.CTe
{
    sealed class ConsultaFaturamentoPorCTe : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioFaturamentoPorCTe>
    {
        #region Construtores

        public ConsultaFaturamentoPorCTe() : base(tabela: "T_CTE as CTe") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            if (!joins.Contains(" DestinatarioCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE DestinatarioCTe on CTe.CON_DESTINATARIO_CTE = DestinatarioCTe.PCT_CODIGO ");
        }

        private void SetarJoinsExpedidor(StringBuilder joins)
        {
            if (!joins.Contains(" ExpedidorCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE ExpedidorCTe on CTe.CON_EXPEDIDOR_CTE = ExpedidorCTe.PCT_CODIGO ");
        }

        private void SetarJoinsLocalidadeFimPrestacao(StringBuilder joins)
        {
            if (!joins.Contains(" FimPrestacaoCTe "))
                joins.Append(" left join T_LOCALIDADES FimPrestacaoCTe on CTe.CON_LOCTERMINOPRESTACAO = FimPrestacaoCTe.LOC_CODIGO ");
        }

        private void SetarJoinsLocalidadeInicioPrestacao(StringBuilder joins)
        {
            if (!joins.Contains(" InicioPrestacaoCTe "))
                joins.Append(" left join T_LOCALIDADES InicioPrestacaoCTe on CTe.CON_LOCINICIOPRESTACAO = InicioPrestacaoCTe.LOC_CODIGO ");
        }

        private void SetarJoinsRecebedor(StringBuilder joins)
        {
            if (!joins.Contains(" RecebedorCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE RecebedorCTe on CTe.CON_RECEBEDOR_CTE = RecebedorCTe.PCT_CODIGO ");
        }

        private void SetarJoinsRemetente(StringBuilder joins)
        {
            if (!joins.Contains(" RemetenteCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE RemetenteCTe on CTe.CON_REMETENTE_CTE = RemetenteCTe.PCT_CODIGO ");
        }

        private void SetarJoinsSerie(StringBuilder joins)
        {
            if (!joins.Contains(" Serie "))
                joins.Append(" left join T_EMPRESA_SERIE Serie on CTe.CON_SERIE = Serie.ESE_CODIGO ");
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

        private void SetarJoinsCTeContainer(StringBuilder joins)
        {
            if (!joins.Contains(" CTeContainer "))
                joins.Append(" left join T_CTE_CONTAINER CTeContainer on CTe.CON_CODIGO = CTeContainer.CON_CODIGO ");
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

        private void SetarJoinsViagem(StringBuilder joins)
        {
            if (!joins.Contains(" Viagem "))
                joins.Append(" left join T_PEDIDO_VIAGEM_NAVIO Viagem on Viagem.PVN_CODIGO = CTe.CON_VIAGEM ");
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

        private void SetarJoinsTituloAVista(StringBuilder joins)
        {
            SetarJoinsTituloDocumentoCTe(joins);

            if (!joins.Contains(" TituloAVista "))
                joins.Append(" left join T_TITULO TituloAVista on TituloAVista.TIT_CODIGO = TituloDocumentoCTe.TIT_CODIGO");
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

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioFaturamentoPorCTe filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "NumeroCTe":
                    if (!select.Contains("NumeroCTe"))
                    {
                        select.Append("CTe.CON_NUM NumeroCTe, ");
                        groupBy.Append("CTe.CON_NUM, ");
                    }
                    break;

                case "ChaveCTe":
                    if (!select.Contains("ChaveCTe"))
                    {
                        select.Append("CTe.CON_CHAVECTE ChaveCTe, ");

                        if (!groupBy.Contains("CTe.CON_CHAVECTE,"))
                            groupBy.Append("CTe.CON_CHAVECTE, ");
                    }
                    break;

                case "SerieCTe":
                    if (!select.Contains("SerieCTe"))
                    {
                        select.Append("Serie.ESE_NUMERO SerieCTe, ");
                        groupBy.Append("Serie.ESE_NUMERO, ");

                        SetarJoinsSerie(joins);
                    }
                    break;

                case "DescricaoTipoCTe":
                    if (!select.Contains("TipoCTe"))
                    {
                        select.Append("CTe.CON_TIPO_CTE TipoCTe, ");
                        groupBy.Append("CTe.CON_TIPO_CTE, ");
                    }
                    break;

                case "StatusCTe":
                    if (!select.Contains("StatusCTe"))
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

                case "NumeroCarga":
                    if (!select.Contains("NumeroCarga"))
                    {
                        select.Append("substring((select distinct ', ' + Carga.CAR_CODIGO_CARGA_EMBARCADOR from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO_ORIGEM where CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) NumeroCarga, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "SituacaoCargaFormatada":
                    if (!select.Contains(" SituacaoCarga,"))
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

                        if (!groupBy.Contains(" CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "DataEmissaoFormatada":
                    if (!select.Contains("DataEmissao"))
                    {
                        select.Append("CTe.CON_DATAHORAEMISSAO DataEmissao, ");
                        groupBy.Append("CTe.CON_DATAHORAEMISSAO, ");
                    }
                    break;

                case "CPFCNPJRemetente":
                    if (!select.Contains(" CPFCNPJRemetente, "))
                    {
                        select.Append("RemetenteCTe.PCT_CPF_CNPJ CPFCNPJRemetente, ");

                        if (!groupBy.Contains("RemetenteCTe.PCT_CPF_CNPJ"))
                            groupBy.Append("RemetenteCTe.PCT_CPF_CNPJ, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "Remetente":
                    if (!select.Contains(" Remetente, "))
                    {
                        select.Append("RemetenteCTe.PCT_NOME Remetente, ");

                        if (!groupBy.Contains("RemetenteCTe.PCT_NOME"))
                            groupBy.Append("RemetenteCTe.PCT_NOME, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "CPFCNPJExpedidor":
                    if (!select.Contains(" CPFCNPJExpedidor, "))
                    {
                        select.Append("ExpedidorCTe.PCT_CPF_CNPJ CPFCNPJExpedidor, ");

                        if (!groupBy.Contains("ExpedidorCTe.PCT_CPF_CNPJ"))
                            groupBy.Append("ExpedidorCTe.PCT_CPF_CNPJ, ");

                        SetarJoinsExpedidor(joins);
                    }
                    break;

                case "Expedidor":
                    if (!select.Contains(" Expedidor, "))
                    {
                        select.Append("ExpedidorCTe.PCT_NOME Expedidor, ");

                        if (!groupBy.Contains("ExpedidorCTe.PCT_NOME"))
                            groupBy.Append("ExpedidorCTe.PCT_NOME, ");

                        SetarJoinsExpedidor(joins);
                    }
                    break;

                case "CPFCNPJRecebedor":
                    if (!select.Contains(" CPFCNPJRecebedor, "))
                    {
                        select.Append("RecebedorCTe.PCT_CPF_CNPJ CPFCNPJRecebedor, ");

                        if (!groupBy.Contains("RecebedorCTe.PCT_CPF_CNPJ"))
                            groupBy.Append("RecebedorCTe.PCT_CPF_CNPJ, ");

                        SetarJoinsRecebedor(joins);
                    }
                    break;

                case "Recebedor":
                    if (!select.Contains(" Recebedor, "))
                    {
                        select.Append("RecebedorCTe.PCT_NOME Recebedor, ");

                        if (!groupBy.Contains("RecebedorCTe.PCT_NOME"))
                            groupBy.Append("RecebedorCTe.PCT_NOME, ");

                        SetarJoinsRecebedor(joins);
                    }
                    break;

                case "CPFCNPJDestinatario":
                    if (!select.Contains(" CPFCNPJDestinatario, "))
                    {
                        select.Append("DestinatarioCTe.PCT_CPF_CNPJ CPFCNPJDestinatario, ");

                        if (!groupBy.Contains("DestinatarioCTe.PCT_CPF_CNPJ"))
                            groupBy.Append("DestinatarioCTe.PCT_CPF_CNPJ, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "Destinatario":
                    if (!select.Contains(" Destinatario, "))
                    {
                        select.Append("DestinatarioCTe.PCT_NOME Destinatario, ");

                        if (!groupBy.Contains("DestinatarioCTe.PCT_NOME"))
                            groupBy.Append("DestinatarioCTe.PCT_NOME, ");

                        SetarJoinsDestinatario(joins);
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

                case "InicioPrestacao":
                    if (!select.Contains(" InicioPrestacao,"))
                    {
                        select.Append(" InicioPrestacaoCTe.LOC_DESCRICAO + '-' + InicioPrestacaoCTe.UF_SIGLA InicioPrestacao, ");
                        groupBy.Append("InicioPrestacaoCTe.LOC_DESCRICAO, ");

                        if (!groupBy.Contains("InicioPrestacaoCTe.UF_SIGLA"))
                            groupBy.Append("InicioPrestacaoCTe.UF_SIGLA, ");

                        SetarJoinsLocalidadeInicioPrestacao(joins);
                    }
                    break;

                case "UFInicioPrestacao":
                    if (!select.Contains("UFInicioPrestacao"))
                    {
                        select.Append(" InicioPrestacaoCTe.UF_SIGLA UFInicioPrestacao, ");

                        if (!groupBy.Contains("InicioPrestacaoCTe.UF_SIGLA"))
                            groupBy.Append("InicioPrestacaoCTe.UF_SIGLA, ");

                        SetarJoinsLocalidadeInicioPrestacao(joins);
                    }
                    break;

                case "FimPrestacao":
                    if (!select.Contains(" FimPrestacao,"))
                    {
                        select.Append(" FimPrestacaoCTe.LOC_DESCRICAO + '-' + FimPrestacaoCTe.UF_SIGLA FimPrestacao, ");
                        groupBy.Append("FimPrestacaoCTe.LOC_DESCRICAO, ");

                        if (!groupBy.Contains("FimPrestacaoCTe.UF_SIGLA"))
                            groupBy.Append("FimPrestacaoCTe.UF_SIGLA, ");

                        SetarJoinsLocalidadeFimPrestacao(joins);
                    }
                    break;

                case "UFFimPrestacao":
                    if (!select.Contains("UFFimPrestacao"))
                    {
                        select.Append(" FimPrestacaoCTe.UF_SIGLA UFFimPrestacao, ");

                        if (!groupBy.Contains("FimPrestacaoCTe.UF_SIGLA"))
                            groupBy.Append(" FimPrestacaoCTe.UF_SIGLA, ");

                        SetarJoinsLocalidadeFimPrestacao(joins);
                    }
                    break;

                case "AliquotaICMS":
                    if (!somenteContarNumeroRegistros && !select.Contains("AliquotaICMS"))
                    {
                        select.Append("CTe.CON_ALIQ_ICMS AliquotaICMS, ");

                        if (!groupBy.Contains("CTe.CON_ALIQ_ICMS"))
                            groupBy.Append("CTe.CON_ALIQ_ICMS, ");
                    }
                    break;

                case "ValorICMS":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorICMS"))
                        select.Append("SUM(CTe.CON_VAL_ICMS) ValorICMS, ");
                    break;

                case "ValorFrete":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorFrete"))
                        select.Append("SUM(CTe.CON_VALOR_FRETE) ValorFrete, ");
                    break;

                case "ValorPrestacao":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorPrestacao"))
                        select.Append("SUM(CTe.CON_VALOR_PREST_SERVICO) ValorPrestacao, ");
                    break;

                case "NumeroNotaFiscal":
                    if (!select.Contains("NumeroNotaFiscal"))
                    {
                        select.Append("substring((select ', ' + notaFiscal1.NFC_NUMERO from T_CTE_DOCS notaFiscal1 where notaFiscal1.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 100000) NumeroNotaFiscal, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroBooking":
                    if (!select.Contains(" NumeroBooking, "))
                    {
                        select.Append("CTe.CON_NUMERO_BOOKING NumeroBooking, ");
                        groupBy.Append("CTe.CON_NUMERO_BOOKING, ");
                    }
                    break;

                case "NumeroOS":
                    if (!select.Contains(" NumeroOS, "))
                    {
                        select.Append("CTe.CON_NUMERO_OS NumeroOS, ");
                        groupBy.Append("CTe.CON_NUMERO_OS, ");
                    }
                    break;

                case "NumeroControle":
                    if (!select.Contains(" NumeroControle, "))
                    {
                        select.Append("CTe.CON_NUMERO_CONTROLE NumeroControle, ");
                        groupBy.Append("CTe.CON_NUMERO_CONTROLE, ");
                    }
                    break;

                case "QuantidadeNF":
                    if (!select.Contains(" QuantidadeNF, "))
                    {
                        select.Append(
                            @"  (SELECT COUNT (1) from T_CTE_DOCS cteDocs 
                                 WHERE cteDocs.CON_CODIGO = CTe.CON_CODIGO) QuantidadeNF, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroLacre":
                    if (!select.Contains(" NumeroLacre, "))
                    {
                        select.Append("(     CASE WHEN (CTeContainer.CER_LACRE1 IS NOT NULL and RTRIM(CTeContainer.CER_LACRE1) <> '') THEN CTeContainer.CER_LACRE1 ELSE '' END ");
                        select.Append("    + CASE WHEN (CTeContainer.CER_LACRE2 IS NOT NULL and RTRIM(CTeContainer.CER_LACRE2) <> '') THEN ', ' + CTeContainer.CER_LACRE2 ELSE '' END ");
                        select.Append("    + CASE WHEN (CTeContainer.CER_LACRE3 IS NOT NULL and RTRIM(CTeContainer.CER_LACRE3) <> '') THEN ', ' + CTeContainer.CER_LACRE3 ELSE '' END ");
                        select.Append(") NumeroLacre, ");
                        groupBy.Append("CTeContainer.CER_LACRE1, CTeContainer.CER_LACRE2, CTeContainer.CER_LACRE3, ");

                        SetarJoinsCTeContainer(joins);
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

                case "Container":
                    if (!select.Contains(" Container, "))
                    {
                        select.Append("Container.CTR_DESCRICAO as Container, ");
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

                case "NumeroFatura":
                    if (!select.Contains(" NumeroFatura, "))
                    {
                        select.Append("(CASE WHEN Fatura.FAT_NUMERO_FATURA_INTEGRACAO IS NULL OR Fatura.FAT_NUMERO_FATURA_INTEGRACAO = 0 THEN Fatura.FAT_NUMERO ELSE Fatura.FAT_NUMERO_FATURA_INTEGRACAO END) as NumeroFatura, ");
                        groupBy.Append("Fatura.FAT_NUMERO, Fatura.FAT_NUMERO_FATURA_INTEGRACAO, ");

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

                case "DataBoletoFormatada":
                    if (!select.Contains(" DataBoleto, "))
                    {
                        select.Append("TituloParcela.TIT_DATA_EMISSAO as DataBoleto, ");
                        groupBy.Append("TituloParcela.TIT_DATA_EMISSAO, ");

                        SetarJoinsTituloParcela(joins);
                    }
                    break;

                case "DescricaoStatusTitulo":
                    if (!select.Contains(" StatusTitulo, "))
                    {
                        select.Append("TituloParcela.TIT_STATUS StatusTitulo, ");
                        groupBy.Append("TituloParcela.TIT_STATUS, ");

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

                case "NavioTransbordo":
                    if (!select.Contains(" NavioTransbordo, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + navio.PVN_DESCRICAO
                                        from T_PEDIDO_VIAGEM_NAVIO navio
                                        inner join T_PEDIDO_TRANSBORDO pedidoTransbordo on pedidoTransbordo.PVN_CODIGO = navio.PVN_CODIGO 
                                        inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = pedidoTransbordo.PED_CODIGO 
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = CargaPedido.CAR_CODIGO 
                                 WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) NavioTransbordo, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
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

                case "NumeroTitulo":
                    if (!select.Contains(" NumeroTitulo, "))
                    {
                        select.Append("SUBSTRING((SELECT DISTINCT ', ' + CONVERT(NVARCHAR(15), Titulo.TIT_CODIGO) FROM T_TITULO Titulo INNER JOIN T_TITULO_DOCUMENTO TituloDocumento ON TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO WHERE TituloDocumento.CON_CODIGO = CTe.CON_CODIGO AND Titulo.TIT_STATUS <> 4 FOR XML PATH('')), 3, 200) NumeroTitulo, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "PesoNotaFiscal":
                    if (!select.Contains(" PesoNotaFiscal, "))
                    {
                        select.Append(
                            @"  (SELECT SUM(NFC_PESO) from T_CTE_DOCS cteDocs 
                                 WHERE cteDocs.CON_CODIGO = CTe.CON_CODIGO) PesoNotaFiscal, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "PesoBrutoNotaFiscal":
                    SetarSelect("PesoNotaFiscal", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("Tara", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "FaturamentoAVista":
                    if (!select.Contains(" FaturamentoAVista, "))
                    {
                        select.Append("(CASE WHEN CAST(TituloAVista.TIT_DATA_EMISSAO AS DATE) = CAST(TituloAVista.TIT_DATA_VENCIMENTO AS DATE) THEN 'Sim' ELSE 'Não' END) as FaturamentoAVista, ");
                        groupBy.Append("TituloAVista.TIT_DATA_EMISSAO, TituloAVista.TIT_DATA_VENCIMENTO, ");

                        SetarJoinsTituloAVista(joins);
                    }
                    break;

                case "DataVencimentoFaturaFormatada":
                    if (!select.Contains(" DataVencimentoFatura, "))
                    {
                        select.Append("FaturaParcela.FAP_DATA_VENCIMENTO as DataVencimentoFatura, ");
                        groupBy.Append("FaturaParcela.FAP_DATA_VENCIMENTO, ");

                        SetarJoinsFaturaParcela(joins);
                    }
                    break;

                case "DataEnvioEmailFaturaIntegracao":
                    if (!select.Contains(" DataEnvioEmailFaturaIntegracao, "))
                    {
                        select.Append("FaturaIntegracao.FAI_DATA_ENVIO as DataEnvioEmailFaturaIntegracao, ");
                        groupBy.Append("FaturaIntegracao.FAI_DATA_ENVIO, ");

                        SetarJoinsFaturaIntegracao(joins);
                    }
                    break;

                case "SituacaoEmailFaturaIntegracao":
                    if (!select.Contains(" SituacaoEmailFaturaIntegracao, "))
                    {
                        select.Append("FaturaIntegracao.FAI_MENSAGEM_RETORNO as SituacaoEmailFaturaIntegracao, ");
                        groupBy.Append("FaturaIntegracao.FAI_MENSAGEM_RETORNO, ");

                        SetarJoinsFaturaIntegracao(joins);
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

                case "DataEnvioFaturaFormatada":
                    SetarSelect("DataEnvioEmailFaturaIntegracao", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("NumeroBoleto", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "SituacaoEmailFatura":
                    SetarSelect("SituacaoEmailFaturaIntegracao", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("NumeroBoleto", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "EmailFatura":
                    SetarSelect("EmailsFaturaIntegracao", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("NumeroBoleto", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "DataEnvioBoletoFormatada":
                    SetarSelect("DataEnvioEmailFaturaIntegracao", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("NumeroBoleto", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "SituacaoEmailBoleto":
                    SetarSelect("SituacaoEmailFaturaIntegracao", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("NumeroBoleto", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "EmailBoleto":
                    SetarSelect("EmailsFaturaIntegracao", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("NumeroBoleto", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);

                    //Alterado para pegar os mesmos e-mails da fatura integração, já que direto do cadastro não fica de acordo
                    /*if (!select.Contains(" EmailBoleto, "))
                    {
                        select.Append(@"(CASE WHEN TituloParcela.TIT_NOSSO_NUMERO IS NOT NULL AND TituloParcela.TIT_NOSSO_NUMERO <> '' THEN
                                        ((coalesce(NULLIF(AcordoFaturamentoCliente.AFC_CABOTAGEM_EMAIL,'') + ';', '') +
                                        coalesce(NULLIF(AcordoFaturamentoCliente.AFC_LONGO_CURSO_EMAIL,'') + ';', '') +
                                        coalesce(NULLIF(AcordoFaturamentoCliente.AFC_CUSTO_EXTRA_EMAIL,'') + ';', '')) +

                                        substring((select distinct '; ' + T.EmailFatura from (

                                                select Cabotagem.AEC_EMAIL EmailFatura
                                                from T_ACORDO_FATURAMENTO_CLIENTE_EMAIL_CABOTAGEM Cabotagem
                                                where Cabotagem.AFC_CODIGO = AcordoFaturamentoCliente.AFC_CODIGO

                                                union

                                                select LongoCurso.ALC_EMAIL  EmailFatura
                                                from T_ACORDO_FATURAMENTO_CLIENTE_EMAIL_LONGO_CURSO LongoCurso
                                                where LongoCurso.AFC_CODIGO = AcordoFaturamentoCliente.AFC_CODIGO

                                                union

                                                select CustoExtra.ALE_EMAIL EmailFatura 
                                                from T_ACORDO_FATURAMENTO_CLIENTE_EMAIL_CUSTO_EXTRA CustoExtra
                                                where CustoExtra.AFC_CODIGO = AcordoFaturamentoCliente.AFC_CODIGO
                                              ) as T

                                           for xml path('')), 3, 10000)

                                    ) ELSE '' END) EmailBoleto, ");

                        groupBy.Append("AcordoFaturamentoCliente.AFC_CABOTAGEM_EMAIL, AcordoFaturamentoCliente.AFC_LONGO_CURSO_EMAIL, AcordoFaturamentoCliente.AFC_CUSTO_EXTRA_EMAIL, ");

                        if (!groupBy.Contains("AcordoFaturamentoCliente.AFC_CODIGO"))
                            groupBy.Append("AcordoFaturamentoCliente.AFC_CODIGO, ");

                        if (!groupBy.Contains("TituloParcela.TIT_NOSSO_NUMERO"))
                            groupBy.Append("TituloParcela.TIT_NOSSO_NUMERO, ");

                        SetarJoinsAcordoFaturamentoCliente(joins);
                        SetarJoinsTituloParcela(joins);
                    }*/
                    break;

                case "DiaSemana":
                    if (!select.Contains(" DiaSemana, "))
                    {
                        select.Append(
                            @"substring((
                                select distinct ', ' + CASE T.DiaSemanaFatura 
                                                            WHEN 1 THEN 'Domingo'
                                                            WHEN 2 THEN 'Segunda'
                                                            WHEN 3 THEN 'Terça'
                                                            WHEN 4 THEN 'Quarta'
                                                            WHEN 5 THEN 'Quinta'
                                                            WHEN 6 THEN 'Sexta'
                                                            WHEN 7 THEN 'Sábado'
                                                        ELSE '' END from (

                                        select Cabotagem.AFC_CABOTAGEM_DIA_SEMANA_FATURA DiaSemanaFatura
                                        from T_ACORDO_FATURAMENTO_CLIENTE_CABOTAGEM_DIA_SEMANA_FATURA Cabotagem
                                        where Cabotagem.AFC_CODIGO = AcordoFaturamentoCliente.AFC_CODIGO

                                        union

                                        select LongoCurso.AFC_LONGO_CURSO_DIA_SEMANA_FATURA  DiaSemanaFatura
                                        from T_ACORDO_FATURAMENTO_CLIENTE_LONGO_CURSO_DIA_SEMANA_FATURA LongoCurso
                                        where LongoCurso.AFC_CODIGO = AcordoFaturamentoCliente.AFC_CODIGO

                                        union

                                        select CustoExtra.AFC_CUSTO_EXTRA_DIA_SEMANA_FATURA DiaSemanaFatura 
                                        from T_ACORDO_FATURAMENTO_CLIENTE_CUSTO_EXTRA_DIA_SEMANA_FATURA CustoExtra
                                        where CustoExtra.AFC_CODIGO = AcordoFaturamentoCliente.AFC_CODIGO
                                      ) as T

                                   for xml path('')), 3, 1000) DiaSemana, "
                        );

                        if (!groupBy.Contains("AcordoFaturamentoCliente.AFC_CODIGO"))
                            groupBy.Append("AcordoFaturamentoCliente.AFC_CODIGO, ");

                        SetarJoinsAcordoFaturamentoCliente(joins);
                    }
                    break;

                case "DiaMes":
                    if (!select.Contains(" DiaMes, "))
                    {
                        select.Append(
                            @"substring((
                                select distinct ', ' + cast(T.DiaMesFatura as varchar(10)) from (

                                        select Cabotagem.AFC_CABOTAGEM_DIA_MES_FATURA DiaMesFatura
                                        from T_ACORDO_FATURAMENTO_CLIENTE_CABOTAGEM_DIA_MES_FATURA Cabotagem
                                        where Cabotagem.AFC_CODIGO = AcordoFaturamentoCliente.AFC_CODIGO

                                        union

                                        select LongoCurso.AFC_LONGO_CURSO_DIA_MES_FATURA  DiaMesFatura
                                        from T_ACORDO_FATURAMENTO_CLIENTE_LONGO_CURSO_DIA_MES_FATURA LongoCurso
                                        where LongoCurso.AFC_CODIGO = AcordoFaturamentoCliente.AFC_CODIGO

                                        union

                                        select CustoExtra.AFC_CUSTO_EXTRA_DIA_MES_FATURA DiaMesFatura 
                                        from T_ACORDO_FATURAMENTO_CLIENTE_CUSTO_EXTRA_DIA_MES_FATURA CustoExtra
                                        where CustoExtra.AFC_CODIGO = AcordoFaturamentoCliente.AFC_CODIGO
                                      ) as T

                                   for xml path('')), 3, 1000) DiaMes, "
                        );

                        if (!groupBy.Contains("AcordoFaturamentoCliente.AFC_CODIGO"))
                            groupBy.Append("AcordoFaturamentoCliente.AFC_CODIGO, ");

                        SetarJoinsAcordoFaturamentoCliente(joins);
                    }
                    break;

                case "TipoPrazoFaturamentoFormatado":
                    if (!select.Contains(" TipoPrazoFaturamento, "))
                    {
                        select.Append("(cast(AcordoFaturamentoCliente.AFC_CABOTAGEM_TIPO_PRAZO_FATURAMENTO as varchar(1)) + ',' + cast(AcordoFaturamentoCliente.AFC_LONGO_CURSO_TIPO_PRAZO_FATURAMENTO as varchar(1)) + ',' +" +
                            " cast(AcordoFaturamentoCliente.AFC_CUSTO_EXTRA_TIPO_PRAZO_FATURAMENTO as varchar(1))) as TipoPrazoFaturamento, ");
                        groupBy.Append("AcordoFaturamentoCliente.AFC_CABOTAGEM_TIPO_PRAZO_FATURAMENTO, AcordoFaturamentoCliente.AFC_LONGO_CURSO_TIPO_PRAZO_FATURAMENTO, AcordoFaturamentoCliente.AFC_CUSTO_EXTRA_TIPO_PRAZO_FATURAMENTO, ");

                        SetarJoinsAcordoFaturamentoCliente(joins);
                    }
                    break;

                case "ValorComponenteBAF":
                    if (!select.Contains(" ValorComponenteBAF, "))
                    {
                        select.Append(@"(select SUM(CargaCTeComponenteFrete.CCC_VALOR_COMPONENTE) from T_CARGA_CTE_COMPONENTES_FRETE CargaCTeComponenteFrete 
                                            inner join T_CARGA_CTE CargaCTe on CargaCTe.CCT_CODIGO = CargaCTeComponenteFrete.CCT_CODIGO 
                                            inner join T_COMPONENTE_FRETE ComponenteFrete on ComponenteFrete.CFR_CODIGO = CargaCTeComponenteFrete.CFR_CODIGO 
                                            where CargaCTe.CON_CODIGO = CTe.CON_CODIGO and ComponenteFrete.CFR_DESCRICAO = 'BAF') ValorComponenteBAF, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "ValorComponenteAdValorem":
                    if (!select.Contains(" ValorComponenteAdValorem, "))
                    {
                        select.Append(@"(select SUM(CargaCTeComponenteFrete.CCC_VALOR_COMPONENTE) from T_CARGA_CTE_COMPONENTES_FRETE CargaCTeComponenteFrete 
                                            inner join T_CARGA_CTE CargaCTe on CargaCTe.CCT_CODIGO = CargaCTeComponenteFrete.CCT_CODIGO 
                                            inner join T_COMPONENTE_FRETE ComponenteFrete on ComponenteFrete.CFR_CODIGO = CargaCTeComponenteFrete.CFR_CODIGO 
                                            where CargaCTe.CON_CODIGO = CTe.CON_CODIGO and ComponenteFrete.CFR_DESCRICAO = 'AD VALOREM') ValorComponenteAdValorem, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        select.Append(@"substring((select distinct ', ' + TipoOperacao.TOP_DESCRICAO from T_CARGA_CTE CargaCTe 
                                        inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO_ORIGEM 
                                        inner join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO 
                                        where CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) TipoOperacao, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
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

                case "SomenteCTeSubstituido":
                    if (!select.Contains(" SomenteCTeSubstituido, "))
                    {
                        select.Append(@"CASE WHEN (select count(1) from t_cte _cte where _cte.CON_CHAVE_CTE_SUB_COMP = CTe.CON_CHAVECTE) > 0 THEN 'Sim' 
                                        ELSE 'Não' END SomenteCTeSubstituido, ");

                        if (!groupBy.Contains("CTe.CON_CHAVECTE"))
                            groupBy.Append(" CTe.CON_CHAVECTE, ");
                    }
                    break;

                case "CTeAnulado":
                    if (!select.Contains(" CTeAnulado, "))
                    {
                        select.Append("CASE WHEN CTe.CON_POSSUI_ANULACAO_SUBSTITUICAO = 1 THEN 'Sim' ELSE 'Não' END CTeAnulado, ");
                        groupBy.Append("CTe.CON_POSSUI_ANULACAO_SUBSTITUICAO, ");
                    }
                    break;

                case "DescricaoTipoServico":
                    if (!select.Contains(" TipoServico, "))
                    {
                        select.Append(@"CTe.CON_TIPO_SERVICO TipoServico, ");

                        if (!groupBy.Contains("CTe.CON_TIPO_SERVICO"))
                            groupBy.Append(" CTe.CON_TIPO_SERVICO, ");
                    }
                    break;

                case "TipoProposta":
                    if (!select.Contains(" TipoProposta, "))
                    {
                        select.Append(@"SUBSTRING((
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
                                                WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) TipoProposta, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
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

                case "DataVencimentoTitulo":
                    if (!select.Contains("DataVencimentoTitulo,"))
                    {
                        select.Append("SUBSTRING((SELECT DISTINCT ', ' + CONVERT(NVARCHAR(10), Titulo.TIT_DATA_VENCIMENTO, 103) FROM T_TITULO Titulo INNER JOIN T_TITULO_DOCUMENTO TituloDocumento ON TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO WHERE TituloDocumento.CON_CODIGO = CTe.CON_CODIGO AND Titulo.TIT_STATUS <> 4 FOR XML PATH('')), 3, 200) DataVencimentoTitulo, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioFaturamentoPorCTe filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            SetarJoinsFaturaParcela(joins);

            if (filtrosPesquisa.NumeroInicial > 0)
                where.Append("  and CTe.CON_NUM >= " + filtrosPesquisa.NumeroInicial.ToString());

            if (filtrosPesquisa.NumeroFinal > 0)
                where.Append("  and CTe.CON_NUM <= " + filtrosPesquisa.NumeroFinal.ToString());

            if (filtrosPesquisa.DataInicialEmissao != DateTime.MinValue)
                where.Append("  and CTe.CON_DATAHORAEMISSAO >= '" + filtrosPesquisa.DataInicialEmissao.ToString(pattern) + "'");

            if (filtrosPesquisa.DataFinalEmissao != DateTime.MinValue)
                where.Append("  and CTe.CON_DATAHORAEMISSAO < '" + filtrosPesquisa.DataFinalEmissao.AddDays(1).ToString(pattern) + "'");

            if (filtrosPesquisa.DataInicialFatura != DateTime.MinValue)
                where.Append("  and CAST(Fatura.FAT_DATA_FATURA AS DATE) >= '" + filtrosPesquisa.DataInicialFatura.ToString(pattern) + "'");

            if (filtrosPesquisa.DataFinalFatura != DateTime.MinValue)
                where.Append("  and CAST(Fatura.FAT_DATA_FATURA AS DATE) <= '" + filtrosPesquisa.DataFinalFatura.ToString(pattern) + "'");

            if (filtrosPesquisa.DataInicialVencimentoFatura != DateTime.MinValue)
                where.Append("  and CAST(FaturaParcela.FAP_DATA_VENCIMENTO AS DATE) >= '" + filtrosPesquisa.DataInicialVencimentoFatura.ToString(pattern) + "'");

            if (filtrosPesquisa.DataFinalVencimentoFatura != DateTime.MinValue)
                where.Append("  and CAST(FaturaParcela.FAP_DATA_VENCIMENTO AS DATE) <= '" + filtrosPesquisa.DataFinalVencimentoFatura.ToString(pattern) + "'");

            if (filtrosPesquisa.CodigoCarga > 0)
                where.Append("  and CTe.CON_CODIGO in (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe where CargaCTe.CAR_CODIGO_ORIGEM = " + filtrosPesquisa.CodigoCarga.ToString() + " or CargaCTe.CAR_CODIGO =" + filtrosPesquisa.CodigoCarga.ToString() + ")"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CpfCnpjTomador > 0d)
            {
                where.Append("  and ClienteTomador.CLI_CGCCPF = " + filtrosPesquisa.CpfCnpjTomador.ToString("F0"));

                SetarJoinsTomadorCliente(joins);
            }

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
            {
                where.Append($"  and GrupoTomadorPagadorCTe.GRP_CODIGO = {filtrosPesquisa.CodigoGrupoPessoas}");

                SetarJoinsTomadorGrupo(joins);
            }

            if (filtrosPesquisa.StatusCTe.Count > 0)
                where.Append("  and CTe.CON_STATUS in ('" + string.Join("', '", filtrosPesquisa.StatusCTe) + "')");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NFe))
            {
                where.Append("  and CTe.CON_CODIGO IN (SELECT _notafiscal.CON_CODIGO FROM T_CTE_DOCS _notafiscal WHERE _notafiscal.NFC_NUMERO LIKE :NOTAFISCAL_NFC_NUMERO) ");
                parametros.Add(new Consulta.ParametroSQL("NOTAFISCAL_NFC_NUMERO", $"%{filtrosPesquisa.NFe}%"));
            }

            if (filtrosPesquisa.TipoServico.Count > 0)
                where.Append($@" and CTe.CON_CODIGO in (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe 
                                                    inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO 
                                                    inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO
                                                    where CargaPedido.TBF_TIPO_SERVICO_MULTIMODAL in ({ string.Join(", ", filtrosPesquisa.TipoServico.Select(o => o.ToString("D"))) }))"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.TipoTomador.Count > 0)
                where.Append("  and CTe.CON_TOMADOR in ('" + string.Join("', '", filtrosPesquisa.TipoTomador) + "')");

            if (filtrosPesquisa.DataInicialFatura != DateTime.MinValue)
                where.Append("  and CAST(Fatura.FAT_DATA_FATURA AS DATE) >= '" + filtrosPesquisa.DataInicialFatura.ToString(pattern) + "'");

            if (filtrosPesquisa.DataFinalFatura != DateTime.MinValue)
                where.Append("  and CAST(Fatura.FAT_DATA_FATURA AS DATE) <= '" + filtrosPesquisa.DataFinalFatura.ToString(pattern) + "'");

            if (filtrosPesquisa.SituacaoFatura.HasValue)
                where.Append("  and Fatura.FAT_SITUACAO = " + filtrosPesquisa.SituacaoFatura.Value.ToString("D"));

            if (filtrosPesquisa.NumeroFatura > 0)
                where.Append($"  and Fatura.FAT_NUMERO = {filtrosPesquisa.NumeroFatura}");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
                where.Append($" and CTe.CON_NUMERO_BOOKING = '{filtrosPesquisa.NumeroBooking}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOS))
                where.Append($" and CTe.CON_NUMERO_OS = '{filtrosPesquisa.NumeroOS}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroControle))
                where.Append($" and CTe.CON_NUMERO_CONTROLE = '{filtrosPesquisa.NumeroControle}'");

            if (filtrosPesquisa.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Todas)
                where.Append($@" and CTe.CON_CODIGO in (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe 
                                                    inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO 
                                                    where Carga.CAR_SITUACAO = { filtrosPesquisa.SituacaoCarga.ToString("D") })");

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

            if (filtrosPesquisa.CodigoViagem > 0)
                where.Append($" and CTe.CON_VIAGEM = {filtrosPesquisa.CodigoViagem}");

            if (filtrosPesquisa.CodigoPortoOrigem > 0)
                where.Append($" and CTe.POT_CODIGO_ORIGEM = {filtrosPesquisa.CodigoPortoOrigem}");

            if (filtrosPesquisa.CodigoPortoDestino > 0)
                where.Append($" and CTe.POT_CODIGO_DESTINO = {filtrosPesquisa.CodigoPortoDestino}");

            if (filtrosPesquisa.NumeroTitulo > 0)
            {
                where.Append($" and TituloParcela.TIT_CODIGO = {filtrosPesquisa.NumeroTitulo}");

                SetarJoinsTituloParcela(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBoleto))
            {
                where.Append($" and TituloParcela.TIT_NOSSO_NUMERO = '{filtrosPesquisa.NumeroBoleto}'");

                SetarJoinsTituloParcela(joins);
            }

            if (filtrosPesquisa.StatusTitulo.Count > 0)
            {
                where.Append($" and TituloParcela.TIT_STATUS in ({string.Join(", ", filtrosPesquisa.StatusTitulo.Select(o => o.ToString("D")))})");
                where.Append($" and TituloAVista.TIT_STATUS in ({string.Join(", ", filtrosPesquisa.StatusTitulo.Select(o => o.ToString("D")))})");

                SetarJoinsTituloParcela(joins);
                SetarJoinsTituloAVista(joins);
            }

            if (filtrosPesquisa.TiposCTe.Count > 0)
                where.Append($" and CTe.CON_TIPO_CTE in ({ string.Join(", ", filtrosPesquisa.TiposCTe.Select(o => o.ToString("D"))) })");

            if (filtrosPesquisa.SituacaoFaturamentoCTe.HasValue)
            {
                if (filtrosPesquisa.SituacaoFaturamentoCTe.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturamentoCTe.Faturado)
                {
                    where.Append($" and (CTe.CON_CODIGO in (SELECT TitDoc.CON_CODIGO FROM T_TITULO_DOCUMENTO TitDoc where TitDoc.CON_CODIGO = CTe.CON_CODIGO) OR CTe.CON_CODIGO in (SELECT DocFat.CON_CODIGO FROM T_DOCUMENTO_FATURAMENTO DocFat WHERE DocFat.CON_CODIGO = CTe.CON_CODIGO and DocFat.DFA_VALOR_A_FATURAR = 0))");
                }
                else if (filtrosPesquisa.SituacaoFaturamentoCTe.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturamentoCTe.NaoFaturado)
                {
                    where.Append($" and (CTe.CON_CODIGO not in (SELECT TitDoc.CON_CODIGO FROM T_TITULO_DOCUMENTO TitDoc where TitDoc.CON_CODIGO = CTe.CON_CODIGO) OR CTe.CON_CODIGO not in (SELECT DocFat.CON_CODIGO FROM T_DOCUMENTO_FATURAMENTO DocFat WHERE DocFat.CON_CODIGO = CTe.CON_CODIGO and DocFat.DFA_VALOR_A_FATURAR = 0))");
                }
            }

            if (filtrosPesquisa.TipoProposta.HasValue)
            {
                where.Append($" and CTe.CON_CODIGO in (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe " +
                             $"join T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = CargaCTe.CAR_CODIGO " +
                             $"and CargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = {(int)filtrosPesquisa.TipoProposta.Value})");
            }

            if (filtrosPesquisa.VeioPorImportacao == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Nao)
                where.Append($" and (CTe.CON_CTE_IMPORTADO_EMBARCADOR = 0 or CTe.CON_CTE_IMPORTADO_EMBARCADOR IS NULL)");
            else if (filtrosPesquisa.VeioPorImportacao == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                where.Append($" and CTe.CON_CTE_IMPORTADO_EMBARCADOR = 1");

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                where.Append($@" and CTe.CON_CODIGO in (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe 
                                                    inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO 
                                                    where Carga.TOP_CODIGO = { filtrosPesquisa.CodigoTipoOperacao })");

            if (filtrosPesquisa.SomenteCTeSubstituido)
                where.Append(" and exists (select _cte.CON_CODIGO from T_CTE _cte where _cte.CON_CHAVE_CTE_SUB_COMP = CTe.CON_CHAVECTE)");

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
