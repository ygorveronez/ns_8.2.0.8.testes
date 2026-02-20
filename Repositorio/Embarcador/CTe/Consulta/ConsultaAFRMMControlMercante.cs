using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.CTe
{
    sealed class ConsultaAFRMMControlMercante : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAFRMMControlMercante>
    {
        #region Construtores

        public ConsultaAFRMMControlMercante() : base(tabela: "T_CTE as CTe") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsPortoOrigem(StringBuilder joins)
        {
            if (!joins.Contains(" PortoOrigem "))
                joins.Append(" left join T_PORTO PortoOrigem on PortoOrigem.POT_CODIGO = CTe.POT_CODIGO_ORIGEM ");
        }

        private void SetarJoinsLocalidadePortoOrigem(StringBuilder joins)
        {
            SetarJoinsPortoOrigem(joins);

            if (!joins.Contains(" LocalidadePortoOrigem "))
                joins.Append(" left join T_LOCALIDADES LocalidadePortoOrigem on LocalidadePortoOrigem.LOC_CODIGO = PortoOrigem.LOC_CODIGO ");
        }

        private void SetarJoinsPaisPortoOrigem(StringBuilder joins)
        {
            SetarJoinsLocalidadePortoOrigem(joins);

            if (!joins.Contains(" PaisPortoOrigem "))
                joins.Append(" left join T_PAIS PaisPortoOrigem on PaisPortoOrigem.PAI_CODIGO = LocalidadePortoOrigem.PAI_CODIGO ");
        }

        private void SetarJoinsPortoDestino(StringBuilder joins)
        {
            if (!joins.Contains(" PortoDestino "))
                joins.Append(" left join T_PORTO PortoDestino on PortoDestino.POT_CODIGO = CTe.POT_CODIGO_DESTINO ");
        }

        private void SetarJoinsLocalidadePortoDestino(StringBuilder joins)
        {
            SetarJoinsPortoDestino(joins);

            if (!joins.Contains(" LocalidadePortoDestino "))
                joins.Append(" left join T_LOCALIDADES LocalidadePortoDestino on LocalidadePortoDestino.LOC_CODIGO = PortoDestino.LOC_CODIGO ");
        }

        private void SetarJoinsPaisPortoDestino(StringBuilder joins)
        {
            SetarJoinsLocalidadePortoDestino(joins);

            if (!joins.Contains(" PaisPortoDestino "))
                joins.Append(" left join T_PAIS PaisPortoDestino on PaisPortoDestino.PAI_CODIGO = LocalidadePortoDestino.PAI_CODIGO ");
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

        private void SetarJoinsLocalidadeInicioPrestacao(StringBuilder joins)
        {
            if (!joins.Contains(" InicioPrestacaoCTe "))
                joins.Append(" left join T_LOCALIDADES InicioPrestacaoCTe on CTe.CON_LOCINICIOPRESTACAO = InicioPrestacaoCTe.LOC_CODIGO ");
        }

        private void SetarJoinsPaisInicioPrestacao(StringBuilder joins)
        {
            SetarJoinsLocalidadeInicioPrestacao(joins);

            if (!joins.Contains(" PaisInicioPrestacao "))
                joins.Append(" left join T_PAIS PaisInicioPrestacao on PaisInicioPrestacao.PAI_CODIGO = InicioPrestacaoCTe.PAI_CODIGO ");
        }

        private void SetarJoinsLocalidadeFimPrestacao(StringBuilder joins)
        {
            if (!joins.Contains(" FimPrestacaoCTe "))
                joins.Append(" left join T_LOCALIDADES FimPrestacaoCTe on CTe.CON_LOCTERMINOPRESTACAO = FimPrestacaoCTe.LOC_CODIGO ");
        }

        private void SetarJoinsPaisFimPrestacao(StringBuilder joins)
        {
            SetarJoinsLocalidadeFimPrestacao(joins);

            if (!joins.Contains(" PaisFimPrestacao "))
                joins.Append(" left join T_PAIS PaisFimPrestacao on PaisFimPrestacao.PAI_CODIGO = FimPrestacaoCTe.PAI_CODIGO ");
        }

        private void SetarJoinsViagemSchedule(StringBuilder joins)
        {
            if (!joins.Contains(" Schedule "))
                joins.Append(" join T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule on Schedule.PVS_CODIGO = CTe.PVS_CODIGO ");
        }

        private void SetarJoinsViagemScheduleDestino(StringBuilder joins)
        {
            if (!joins.Contains(" ViagemScheduleDestino "))
                joins.Append(" left join T_PEDIDO_VIAGEM_NAVIO_SCHEDULE ViagemScheduleDestino on ViagemScheduleDestino.PVN_CODIGO = CTe.CON_VIAGEM AND ViagemScheduleDestino.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO ");
        }

        private void SetarJoinsTomador(StringBuilder joins)
        {
            if (!joins.Contains(" TomadorPagadorCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE TomadorPagadorCTe on TomadorPagadorCTe.PCT_CODIGO = CTe.CON_TOMADOR_PAGADOR_CTE ");
        }

        private void SetarJoinsTomadorCliente(StringBuilder joins)
        {
            SetarJoinsTomador(joins);

            if (!joins.Contains(" ClienteTomador "))
                joins.Append(" left join T_CLIENTE ClienteTomador on ClienteTomador.CLI_CGCCPF = TomadorPagadorCTe.CLI_CODIGO ");
        }

        private void SetarJoinsRemetente(StringBuilder joins)
        {
            if (!joins.Contains(" Remetente "))
                joins.Append(" left join T_CTE_PARTICIPANTE Remetente on Remetente.PCT_CODIGO = CTe.CON_REMETENTE_CTE ");
        }

        private void SetarJoinsRemetenteCliente(StringBuilder joins)
        {
            SetarJoinsRemetente(joins);

            if (!joins.Contains(" ClienteRemetente "))
                joins.Append(" left join T_CLIENTE ClienteRemetente on ClienteRemetente.CLI_CGCCPF = Remetente.CLI_CODIGO ");
        }

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            if (!joins.Contains(" Destinatario "))
                joins.Append(" left join T_CTE_PARTICIPANTE Destinatario on Destinatario.PCT_CODIGO = CTe.CON_DESTINATARIO_CTE ");
        }

        private void SetarJoinsDestinatarioCliente(StringBuilder joins)
        {
            SetarJoinsDestinatario(joins);

            if (!joins.Contains("ClienteDestinatario"))
                joins.Append(" left join T_CLIENTE ClienteDestinatario on ClienteDestinatario.CLI_CGCCPF = Destinatario.CLI_CODIGO ");
        }

        private void SetarJoinsExpedidor(StringBuilder joins)
        {
            if (!joins.Contains(" Expedidor "))
                joins.Append(" left join T_CTE_PARTICIPANTE Expedidor on Expedidor.PCT_CODIGO = CTe.CON_EXPEDIDOR_CTE ");
        }

        private void SetarJoinsExpedidorCliente(StringBuilder joins)
        {
            SetarJoinsExpedidor(joins);

            if (!joins.Contains(" ClienteExpedidor "))
                joins.Append(" left join T_CLIENTE ClienteExpedidor on ClienteExpedidor.CLI_CGCCPF = Expedidor.CLI_CODIGO ");
        }

        private void SetarJoinsRecebedor(StringBuilder joins)
        {
            if (!joins.Contains(" Recebedor "))
                joins.Append(" left join T_CTE_PARTICIPANTE Recebedor on Recebedor.PCT_CODIGO = CTe.CON_RECEBEDOR_CTE ");
        }

        private void SetarJoinsRecebedorCliente(StringBuilder joins)
        {
            SetarJoinsRecebedor(joins);

            if (!joins.Contains(" ClienteRecebedor "))
                joins.Append(" left join T_CLIENTE ClienteRecebedor on ClienteRecebedor.CLI_CGCCPF = Recebedor.CLI_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAFRMMControlMercante filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("CTe.CON_CODIGO as Codigo, ");
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

                case "Navio":
                    if (!select.Contains(" Navio, "))
                    {
                        select.Append("Navio.NAV_DESCRICAO as Navio, ");
                        groupBy.Append("Navio.NAV_DESCRICAO, ");

                        SetarJoinsNavio(joins);
                    }
                    break;

                case "Viagem":
                    if (!select.Contains(" Viagem, "))
                    {
                        select.Append("CAST(Viagem.PVN_NUMERO_VIAGEM AS VARCHAR(20)) as Viagem, ");
                        groupBy.Append("Viagem.PVN_NUMERO_VIAGEM, ");

                        SetarJoinsViagem(joins);
                    }
                    break;

                case "Direcao":
                    if (!select.Contains(" Direcao, "))
                    {
                        select.Append(@"CASE
                                        WHEN Viagem.PVN_DIRECAO = 1 THEN 'N'
                                        WHEN Viagem.PVN_DIRECAO = 2 THEN 'S'
                                        WHEN Viagem.PVN_DIRECAO = 3 THEN 'E'
                                        WHEN Viagem.PVN_DIRECAO = 4 THEN 'W'
                                    ELSE ''
                                    END Direcao, ");
                        groupBy.Append("Viagem.PVN_DIRECAO, ");

                        SetarJoinsViagem(joins);
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

                case "InicioPrestacao":
                    if (!select.Contains(" InicioPrestacao, "))
                    {
                        select.Append("(InicioPrestacaoCTe.LOC_DESCRICAO + ' ' + InicioPrestacaoCTe.UF_SIGLA + ' ' + PaisInicioPrestacao.PAI_ABREVIACAO) InicioPrestacao, ");
                        groupBy.Append("InicioPrestacaoCTe.LOC_DESCRICAO, InicioPrestacaoCTe.UF_SIGLA, PaisInicioPrestacao.PAI_ABREVIACAO, ");

                        SetarJoinsPaisInicioPrestacao(joins);
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
                        select.Append("(PortoOrigem.POT_DESCRICAO + ' ' + LocalidadePortoOrigem.UF_SIGLA + ' ' + PaisPortoOrigem.PAI_ABREVIACAO) PortoOrigem, ");
                        groupBy.Append("PortoOrigem.POT_DESCRICAO, LocalidadePortoOrigem.UF_SIGLA, PaisPortoOrigem.PAI_ABREVIACAO, ");

                        SetarJoinsPaisPortoOrigem(joins);
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
                        select.Append("(PortoDestino.POT_DESCRICAO + ' ' + LocalidadePortoDestino.UF_SIGLA + ' ' + PaisPortoDestino.PAI_ABREVIACAO) PortoDestino, ");
                        groupBy.Append("PortoDestino.POT_DESCRICAO, LocalidadePortoDestino.UF_SIGLA, PaisPortoDestino.PAI_ABREVIACAO, ");

                        SetarJoinsPaisPortoDestino(joins);
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

                case "FimPrestacao":
                    if (!select.Contains(" FimPrestacao, "))
                    {
                        select.Append("(FimPrestacaoCTe.LOC_DESCRICAO + ' ' + FimPrestacaoCTe.UF_SIGLA + ' ' + PaisFimPrestacao.PAI_ABREVIACAO) FimPrestacao, ");
                        groupBy.Append("FimPrestacaoCTe.LOC_DESCRICAO, FimPrestacaoCTe.UF_SIGLA, PaisFimPrestacao.PAI_ABREVIACAO, ");

                        SetarJoinsPaisFimPrestacao(joins);
                    }
                    break;

                case "NumeroControle":
                    if (!select.Contains(" NumeroControle, "))
                    {
                        select.Append("CTe.CON_NUMERO_CONTROLE as NumeroControle, ");

                        if (!groupBy.Contains("CTe.CON_NUMERO_CONTROLE"))
                            groupBy.Append("CTe.CON_NUMERO_CONTROLE, ");
                    }
                    break;

                case "UBLI":
                    if (!select.Contains(" UBLI, "))
                    {
                        select.Append("CTe.CON_NUMERO_CONTROLE as UBLI, ");

                        if (!groupBy.Contains("CTe.CON_NUMERO_CONTROLE"))
                            groupBy.Append("CTe.CON_NUMERO_CONTROLE, ");
                    }
                    break;

                case "Carrier":
                    if (!select.Contains(" Carrier, "))
                    {
                        select.Append("36 as Carrier, ");
                    }
                    break;

                case "DataETAFormatada":
                    if (!select.Contains(" ETA, "))
                    {
                        select.Append("Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO ETA, ");

                        if (!groupBy.Contains("Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO"))
                            groupBy.Append("Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO, ");

                        SetarJoinsViagemSchedule(joins);

                        //select.Append(@"ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_CINCO AND Schedule.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO),
                        //                        ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_QUATRO AND Schedule.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO),
                        //                        ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_TRES AND Schedule.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO),
                        //                        ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_DOIS AND Schedule.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO),
                        //                        ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_UM AND Schedule.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO),
                        //                        (SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CTe.CON_VIAGEM AND Schedule.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO)))))) ETA,");

                        //if (!groupBy.Contains("CTe.CON_VIAGEM"))
                        //    groupBy.Append("CTe.CON_VIAGEM, ");
                        //if (!groupBy.Contains("CTe.CON_VIAGEM_PASSAGEM_CINCO"))
                        //    groupBy.Append("CTe.CON_VIAGEM_PASSAGEM_CINCO, ");
                        //if (!groupBy.Contains("CTe.CON_VIAGEM_PASSAGEM_QUATRO"))
                        //    groupBy.Append("CTe.CON_VIAGEM_PASSAGEM_QUATRO, ");
                        //if (!groupBy.Contains("CTe.CON_VIAGEM_PASSAGEM_TRES"))
                        //    groupBy.Append("CTe.CON_VIAGEM_PASSAGEM_TRES, ");
                        //if (!groupBy.Contains("CTe.CON_VIAGEM_PASSAGEM_DOIS"))
                        //    groupBy.Append("CTe.CON_VIAGEM_PASSAGEM_DOIS, ");
                        //if (!groupBy.Contains("CTe.CON_VIAGEM_PASSAGEM_UM"))
                        //    groupBy.Append("CTe.CON_VIAGEM_PASSAGEM_UM, ");                        
                        //if (!groupBy.Contains("CTe.POT_CODIGO_DESTINO"))
                        //    groupBy.Append("CTe.POT_CODIGO_DESTINO, ");
                        //if (!groupBy.Contains("CTe.CON_TERMINAL_DESTINO"))
                        //    groupBy.Append("CTe.CON_TERMINAL_DESTINO, ");                        

                        //select.Append("ViagemScheduleDestino.PVS_DATA_PREVISAO_CHEGADA_NAVIO ETA, ");

                        //if (!groupBy.Contains("ViagemScheduleDestino.PVS_DATA_PREVISAO_CHEGADA_NAVIO"))
                        //    groupBy.Append("ViagemScheduleDestino.PVS_DATA_PREVISAO_CHEGADA_NAVIO, ");

                        //SetarJoinsViagemScheduleDestino(joins);
                    }
                    break;

                case "DataOperacaoETAFormatada":
                    if (!select.Contains(" DataOperacaoETA, "))
                    {
                        select.Append("Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO DataOperacaoETA, ");

                        if (!groupBy.Contains("Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO"))
                            groupBy.Append("Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO, ");

                        SetarJoinsViagemSchedule(joins);

                        //select.Append(@"ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_CINCO AND Schedule.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO),
                        //                        ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_QUATRO AND Schedule.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO),
                        //                        ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_TRES AND Schedule.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO),
                        //                        ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_DOIS AND Schedule.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO),
                        //                        ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_UM AND Schedule.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO),
                        //                        (SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CTe.CON_VIAGEM AND Schedule.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO)))))) DataOperacaoETA,");

                        //if (!groupBy.Contains("CTe.CON_VIAGEM"))
                        //    groupBy.Append("CTe.CON_VIAGEM, ");
                        //if (!groupBy.Contains("CTe.CON_VIAGEM_PASSAGEM_CINCO"))
                        //    groupBy.Append("CTe.CON_VIAGEM_PASSAGEM_CINCO, ");
                        //if (!groupBy.Contains("CTe.CON_VIAGEM_PASSAGEM_QUATRO"))
                        //    groupBy.Append("CTe.CON_VIAGEM_PASSAGEM_QUATRO, ");
                        //if (!groupBy.Contains("CTe.CON_VIAGEM_PASSAGEM_TRES"))
                        //    groupBy.Append("CTe.CON_VIAGEM_PASSAGEM_TRES, ");
                        //if (!groupBy.Contains("CTe.CON_VIAGEM_PASSAGEM_DOIS"))
                        //    groupBy.Append("CTe.CON_VIAGEM_PASSAGEM_DOIS, ");
                        //if (!groupBy.Contains("CTe.CON_VIAGEM_PASSAGEM_UM"))
                        //    groupBy.Append("CTe.CON_VIAGEM_PASSAGEM_UM, ");                        
                        //if (!groupBy.Contains("CTe.POT_CODIGO_DESTINO"))
                        //    groupBy.Append("CTe.POT_CODIGO_DESTINO, ");
                        //if (!groupBy.Contains("CTe.CON_TERMINAL_DESTINO"))
                        //    groupBy.Append("CTe.CON_TERMINAL_DESTINO, ");

                        //select.Append("ViagemScheduleDestino.PVS_DATA_PREVISAO_CHEGADA_NAVIO DataOperacaoETA, ");

                        //if (!groupBy.Contains("ViagemScheduleDestino.PVS_DATA_PREVISAO_CHEGADA_NAVIO"))
                        //    groupBy.Append("ViagemScheduleDestino.PVS_DATA_PREVISAO_CHEGADA_NAVIO, ");

                        //SetarJoinsViagemScheduleDestino(joins);
                    }
                    break;

                case "ShipperCode":
                    if (!select.Contains(" ShipperCode, "))
                    {
                        select.Append("(case when CTe.CON_TIPO_SERVICO = 4 then ClienteExpedidor.CLI_CODIGO_DOCUMENTO else ClienteRemetente.CLI_CODIGO_DOCUMENTO end) as ShipperCode, ");

                        groupBy.Append("ClienteExpedidor.CLI_CODIGO_DOCUMENTO, ");
                        groupBy.Append("ClienteRemetente.CLI_CODIGO_DOCUMENTO, ");

                        if (!groupBy.Contains("CTe.CON_TIPO_SERVICO"))
                            groupBy.Append("CTe.CON_TIPO_SERVICO, ");

                        SetarJoinsExpedidorCliente(joins);
                        SetarJoinsRemetenteCliente(joins);
                    }
                    break;

                case "Shipper":
                    if (!select.Contains(" Shipper, "))
                    {
                        select.Append("(case when CTe.CON_TIPO_SERVICO = 4 then ClienteExpedidor.CLI_NOME else ClienteRemetente.CLI_NOME end) as Shipper, ");

                        groupBy.Append("ClienteExpedidor.CLI_NOME, ");
                        groupBy.Append("ClienteRemetente.CLI_NOME, ");

                        if (!groupBy.Contains("CTe.CON_TIPO_SERVICO"))
                            groupBy.Append("CTe.CON_TIPO_SERVICO, ");

                        SetarJoinsExpedidorCliente(joins);
                        SetarJoinsRemetenteCliente(joins);
                    }
                    break;

                //case "ConsigCode":
                //    if (!select.Contains(" ConsigCode, "))
                //    {
                //        select.Append(@"(CASE WHEN CTe.CON_TIPO_SERVICO = 4 AND CTe.CON_NUMERO_CONTROLE LIKE 'SVM%' THEN 
                //                            (SELECT TOP(1) Cliente.CLI_CODIGO_DOCUMENTO
                //                            FROM T_CTE_SUBCONTRATADO Anterior
                //                            JOIN T_CTE MTL ON MTL.CON_CHAVECTE = Anterior.CSU_CHAVE
                //                            JOIN T_CTE_PARTICIPANTE Tomador ON Tomador.PCT_CODIGO = MTL.CON_TOMADOR_PAGADOR_CTE
                //                            JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = Tomador.CLI_CODIGO
                //                            WHERE ANTERIOR.CON_CODIGO = CTe.CON_CODIGO)
                //                         ELSE ClienteTomador.CLI_CODIGO_DOCUMENTO END) as ConsigCode, ");

                //        if (!groupBy.Contains("ClienteTomador.CLI_CODIGO_DOCUMENTO"))
                //            groupBy.Append("ClienteTomador.CLI_CODIGO_DOCUMENTO, ");

                //        if (!groupBy.Contains("CTe.CON_TIPO_SERVICO"))
                //            groupBy.Append("CTe.CON_TIPO_SERVICO, ");

                //        if (!groupBy.Contains("CTe.CON_NUMERO_CONTROLE"))
                //            groupBy.Append("CTe.CON_NUMERO_CONTROLE, ");

                //        SetarJoinsTomadorCliente(joins);
                //    }
                //    break;

                //case "Consig":
                //    if (!select.Contains(" Consig, "))
                //    {
                //        select.Append(@"(CASE WHEN CTe.CON_TIPO_SERVICO = 4 AND CTe.CON_NUMERO_CONTROLE LIKE 'SVM%' THEN 
                //                            (SELECT TOP(1) Cliente.CLI_NOME
                //                            FROM T_CTE_SUBCONTRATADO Anterior
                //                            JOIN T_CTE MTL ON MTL.CON_CHAVECTE = Anterior.CSU_CHAVE
                //                            JOIN T_CTE_PARTICIPANTE Tomador ON Tomador.PCT_CODIGO = MTL.CON_TOMADOR_PAGADOR_CTE
                //                            JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = Tomador.CLI_CODIGO
                //                            WHERE ANTERIOR.CON_CODIGO = CTe.CON_CODIGO)
                //                         ELSE ClienteTomador.CLI_NOME END) as Consig, ");

                //        if (!groupBy.Contains("ClienteTomador.CLI_NOME"))
                //            groupBy.Append("ClienteTomador.CLI_NOME, ");

                //        if (!groupBy.Contains("CTe.CON_TIPO_SERVICO"))
                //            groupBy.Append("CTe.CON_TIPO_SERVICO, ");

                //        if (!groupBy.Contains("CTe.CON_NUMERO_CONTROLE"))
                //            groupBy.Append("CTe.CON_NUMERO_CONTROLE, ");

                //        SetarJoinsTomadorCliente(joins);
                //    }
                //    break;

                case "Notify1Code":
                    if (!select.Contains(" Notify1Code, "))
                    {
                        select.Append("(case when CTe.CON_TIPO_SERVICO = 4 then ClienteRecebedor.CLI_CODIGO_DOCUMENTO else ClienteDestinatario.CLI_CODIGO_DOCUMENTO end) as Notify1Code, ");

                        groupBy.Append("ClienteRecebedor.CLI_CODIGO_DOCUMENTO, ");
                        groupBy.Append("ClienteDestinatario.CLI_CODIGO_DOCUMENTO, ");

                        if (!groupBy.Contains("CTe.CON_TIPO_SERVICO"))
                            groupBy.Append("CTe.CON_TIPO_SERVICO, ");

                        SetarJoinsRecebedorCliente(joins);
                        SetarJoinsDestinatarioCliente(joins);
                    }
                    break;

                case "Notify1":
                    if (!select.Contains(" Notify1, "))
                    {
                        select.Append("(case when CTe.CON_TIPO_SERVICO = 4 then ClienteRecebedor.CLI_NOME else ClienteDestinatario.CLI_NOME end) as Notify1, ");

                        groupBy.Append("ClienteRecebedor.CLI_NOME, ");
                        groupBy.Append("ClienteDestinatario.CLI_NOME, ");

                        if (!groupBy.Contains("CTe.CON_TIPO_SERVICO"))
                            groupBy.Append("CTe.CON_TIPO_SERVICO, ");

                        SetarJoinsRecebedorCliente(joins);
                        SetarJoinsDestinatarioCliente(joins);
                    }
                    break;

                case "FreightCode":
                case "ConsigCode":
                    if (!select.Contains(" FreightCode, "))
                    {
                        select.Append(@"(CASE
                            WHEN CTe.CON_TIPO_SERVICO = 4
                                 AND CTe.CON_NUMERO_CONTROLE LIKE 'SVM%' THEN
                                   (SELECT TOP(1) Cliente.CLI_CODIGO_DOCUMENTO
                                    FROM T_CTE_TERCEIRO Anterior
                                    JOIN T_PEDIDO_CTE_PARA_SUB_CONTRATACAO PedidoSub on Anterior.CPS_CODIGO = PedidoSub.CPS_CODIGO
                                    JOIN T_CTE_PARTICIPANTE Tomador ON Tomador.PCT_CODIGO = CASE WHEN Anterior.CPS_TOMADOR = 0 THEN Anterior.CPS_REMETENTE_CTE 
					                WHEN Anterior.CPS_TOMADOR = 1 THEN Anterior.CPS_EXPEDIDOR_CTE
					                WHEN Anterior.CPS_TOMADOR = 2 THEN Anterior.CPS_RECEBEDOR_CTE
					                WHEN Anterior.CPS_TOMADOR = 3 THEN Anterior.CPS_DESTINATARIO_CTE
					                WHEN Anterior.CPS_TOMADOR = 4 THEN Anterior.CPS_TOMADOR_CTE
					                ELSE CPS_DESTINATARIO_CTE END
                                    JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = Tomador.CLI_CODIGO
					                JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoSub.CPE_CODIGO
					                JOIN T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = CargaPedido.CAR_CODIGO
                                    WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO)
                            ELSE ClienteTomador.CLI_CODIGO_DOCUMENTO
                        END) AS FreightCode, ");

                        if (!groupBy.Contains("ClienteTomador.CLI_CODIGO_DOCUMENTO"))
                            groupBy.Append("ClienteTomador.CLI_CODIGO_DOCUMENTO, ");

                        if (!groupBy.Contains("CTe.CON_TIPO_SERVICO"))
                            groupBy.Append("CTe.CON_TIPO_SERVICO, ");

                        if (!groupBy.Contains("CTe.CON_NUMERO_CONTROLE"))
                            groupBy.Append("CTe.CON_NUMERO_CONTROLE, ");

                        SetarJoinsTomadorCliente(joins);
                    }
                    break;
                case "Consig":
                case "Freight":
                    if (!select.Contains(" Freight, "))
                    {
                        select.Append(@"(CASE
                            WHEN CTe.CON_TIPO_SERVICO = 4
                                 AND CTe.CON_NUMERO_CONTROLE LIKE 'SVM%' THEN
                                   (SELECT TOP(1) Cliente.CLI_NOME
                                    FROM T_CTE_TERCEIRO Anterior
                                    JOIN T_PEDIDO_CTE_PARA_SUB_CONTRATACAO PedidoSub on Anterior.CPS_CODIGO = PedidoSub.CPS_CODIGO
                                    JOIN T_CTE_PARTICIPANTE Tomador ON Tomador.PCT_CODIGO = CASE WHEN Anterior.CPS_TOMADOR = 0 THEN Anterior.CPS_REMETENTE_CTE 
					                WHEN Anterior.CPS_TOMADOR = 1 THEN Anterior.CPS_EXPEDIDOR_CTE
					                WHEN Anterior.CPS_TOMADOR = 2 THEN Anterior.CPS_RECEBEDOR_CTE
					                WHEN Anterior.CPS_TOMADOR = 3 THEN Anterior.CPS_DESTINATARIO_CTE
					                WHEN Anterior.CPS_TOMADOR = 4 THEN Anterior.CPS_TOMADOR_CTE
					                ELSE CPS_DESTINATARIO_CTE END
                                    JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = Tomador.CLI_CODIGO
					                JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoSub.CPE_CODIGO
					                JOIN T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = CargaPedido.CAR_CODIGO
                                    WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO)
                            ELSE ClienteTomador.CLI_NOME
                        END) AS Freight, ");

                        if (!groupBy.Contains("ClienteTomador.CLI_NOME"))
                            groupBy.Append("ClienteTomador.CLI_NOME, ");

                        if (!groupBy.Contains("CTe.CON_TIPO_SERVICO"))
                            groupBy.Append("CTe.CON_TIPO_SERVICO, ");

                        if (!groupBy.Contains("CTe.CON_NUMERO_CONTROLE"))
                            groupBy.Append("CTe.CON_NUMERO_CONTROLE, ");

                        SetarJoinsTomadorCliente(joins);
                    }
                    break;

                case "CommodityText":
                    if (!select.Contains(" CommodityText, "))
                    {
                        select.Append("'CARGA DE CABOTAGEM' as CommodityText, ");
                    }
                    break;

                case "Service":
                    if (!select.Contains(" Service, "))
                    {
                        select.Append("'ALCT' as Service, ");
                    }
                    break;

                case "NumeroBooking":
                    if (!select.Contains(" NumeroBooking, "))
                    {
                        select.Append("CTe.CON_NUMERO_BOOKING as NumeroBooking, ");
                        groupBy.Append("CTe.CON_NUMERO_BOOKING, ");
                    }
                    break;

                case "Copysgn":
                    if (!select.Contains(" Copysgn, "))
                    {
                        select.Append("(case when CTe.POT_CODIGO_PASSAGEM_UM is not null then 'T' else '' end) as Copysgn, ");

                        if (!groupBy.Contains("CTe.POT_CODIGO_PASSAGEM_UM"))
                            groupBy.Append("CTe.POT_CODIGO_PASSAGEM_UM, ");
                    }
                    break;

                case "NumeroManifesto":
                    if (!select.Contains(" NumeroManifesto, "))
                    {
                        select.Append("(case when CTe.POT_CODIGO_PASSAGEM_UM is not null and (CTe.CON_NUMERO_MANIFESTO_TRANSBORDO <> '' or CTe.CON_NUMERO_MANIFESTO_TRANSBORDO is not null) " +
                                           " then CTe.CON_NUMERO_MANIFESTO_TRANSBORDO else CTe.CON_NUMERO_MANIFESTO end) as NumeroManifesto, ");
                        groupBy.Append("CTe.CON_NUMERO_MANIFESTO, CTe.CON_NUMERO_MANIFESTO_TRANSBORDO, ");

                        if (!groupBy.Contains("CTe.POT_CODIGO_PASSAGEM_UM"))
                            groupBy.Append("CTe.POT_CODIGO_PASSAGEM_UM, ");
                    }
                    break;

                case "NumeroCEMercante":
                    if (!select.Contains(" NumeroCEMercante, "))
                    {
                        select.Append("CTe.CON_NUMERO_CE_MERCANTE NumeroCEMercante, ");
                        groupBy.Append("CTe.CON_NUMERO_CE_MERCANTE, ");
                    }
                    break;

                case "ValorPrestacao":
                    if (!somenteContarNumeroRegistros && !select.Contains(" ValorPrestacaoServico, "))
                    {
                        select.Append("CTe.CON_VALOR_PREST_SERVICO ValorPrestacaoServico, ");
                        groupBy.Append("CTe.CON_VALOR_PREST_SERVICO, ");

                        SetarSelect("TipoPropostaMultimodal", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    }
                    break;

                case "FrtCurr":
                    if (!select.Contains(" FrtCurr, "))
                    {
                        select.Append("'BRL' as FrtCurr, ");
                    }
                    break;

                case "FrtPaymode":
                    if (!select.Contains(" FrtPaymode, "))
                    {
                        select.Append("'P' as FrtPaymode, ");
                    }
                    break;

                case "ValorICMS":
                    if (!somenteContarNumeroRegistros && !select.Contains(" ValorICMS, "))
                    {
                        select.Append("CTe.CON_VAL_ICMS ValorICMS, ");

                        if (!groupBy.Contains("CTe.CON_VAL_ICMS"))
                            groupBy.Append("CTe.CON_VAL_ICMS, ");
                    }
                    break;

                case "Curr5":
                    if (!select.Contains(" Curr5, "))
                    {
                        select.Append("'BRL' as Curr5, ");
                    }
                    break;

                case "PayM5":
                    if (!select.Contains(" PayM5, "))
                    {
                        select.Append("'P' as PayM5, ");
                    }
                    break;

                case "ValorICMSST":
                    if (!somenteContarNumeroRegistros && !select.Contains(" ValorICMSST, "))
                    {
                        select.Append("(case when CON_CST = '60' then CTe.CON_VAL_ICMS else 0.0 end) ValorICMSST, ");

                        if (!groupBy.Contains("CTe.CON_CST"))
                            groupBy.Append("CTe.CON_CST, ");
                        if (!groupBy.Contains("CTe.CON_VAL_ICMS"))
                            groupBy.Append("CTe.CON_VAL_ICMS, ");
                    }
                    break;

                case "Curr6":
                    if (!select.Contains(" Curr6, "))
                    {
                        select.Append("'BRL' as Curr6, ");
                    }
                    break;

                case "PayM6":
                    if (!select.Contains(" PayM6, "))
                    {
                        select.Append("'P' as PayM6, ");
                    }
                    break;

                case "Irin":
                    if (!select.Contains(" Irin, "))
                    {
                        select.Append("Navio.NAV_IRIN as Irin, ");
                        groupBy.Append("Navio.NAV_IRIN, ");

                        SetarJoinsNavio(joins);
                    }
                    break;

                case "CTeSign":
                    if (!select.Contains(" CTeSign, "))
                    {
                        select.Append("(case when CTe.CON_TIPO_SERVICO = 4 then 'A' else 'P' end) as CTeSign, ");

                        if (!groupBy.Contains("CTe.CON_TIPO_SERVICO"))
                            groupBy.Append("CTe.CON_TIPO_SERVICO, ");
                    }
                    break;

                case "Station":
                    if (!select.Contains(" Station, "))
                    {
                        select.Append("6 as Station, ");
                    }
                    break;

                case "Amount9":
                    SetarSelect("ValorPrestacao", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("TipoPropostaMultimodal", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    break;

                case "Curr9":
                    SetarSelect("TipoPropostaMultimodal", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    break;

                case "PayM9":
                    SetarSelect("TipoPropostaMultimodal", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    break;

                case "TipoPropostaMultimodal":
                    if (!select.Contains(" TipoPropostaMultimodal, "))
                    {
                        select.Append(@"ISNULL((SELECT top 1 cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL
                                                        from T_CARGA_PEDIDO cargaPedido 
                                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = cargaPedido.CAR_CODIGO 
                                                 WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO), 0) TipoPropostaMultimodal, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAFRMMControlMercante filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string datePattern = "yyyy-MM-dd";

            groupBy.Append(" CTe.CON_CODIGO, ");

            where.Append(" and CTe.CON_TIPO_MODAL = 3 and CTe.CON_STATUS = 'A'");

            if (filtrosPesquisa.DataEmissaoInicial != DateTime.MinValue)
            {
                where.Append($" and Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO > '{filtrosPesquisa.DataEmissaoInicial.AddDays(-1).ToString(datePattern)}'");
                SetarJoinsViagemSchedule(joins);
                //where.Append($@" and ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_CINCO AND Schedule.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO),
                //                                ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_QUATRO AND Schedule.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO),
                //                                ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_TRES AND Schedule.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO),
                //                                ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_DOIS AND Schedule.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO),
                //                                ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_UM AND Schedule.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO),
                //                                (SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CTe.CON_VIAGEM AND Schedule.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO)))))) > '{filtrosPesquisa.DataEmissaoInicial.AddDays(-1).ToString(datePattern)}'");

                //where.Append($" and CAST(ViagemScheduleDestino.PVS_DATA_PREVISAO_CHEGADA_NAVIO AS DATE) >= '{filtrosPesquisa.DataEmissaoInicial.ToString(datePattern)}'");
                //SetarJoinsViagemScheduleDestino(joins);
            }

            if (filtrosPesquisa.DataEmissaoFinal != DateTime.MinValue)
            {
                where.Append($" and Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO < '{filtrosPesquisa.DataEmissaoFinal.AddDays(1).ToString(datePattern)}'");
                SetarJoinsViagemSchedule(joins);
                //where.Append($@" and ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_CINCO AND Schedule.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO),
                //                                ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_QUATRO AND Schedule.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO),
                //                                ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_TRES AND Schedule.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO),
                //                                ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_DOIS AND Schedule.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO),
                //                                ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_UM AND Schedule.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO),
                //                                (SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CTe.CON_VIAGEM AND Schedule.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_DESTINO)))))) < '{filtrosPesquisa.DataEmissaoFinal.AddDays(1).ToString(datePattern)}'");

                //where.Append($" and CAST(ViagemScheduleDestino.PVS_DATA_PREVISAO_CHEGADA_NAVIO AS DATE) <= '{filtrosPesquisa.DataEmissaoFinal.ToString(datePattern)}'");
                //SetarJoinsViagemScheduleDestino(joins);
            }

            if (filtrosPesquisa.CodigoViagem > 0)
                where.Append($" and CTe.CON_VIAGEM = {filtrosPesquisa.CodigoViagem}");

            if (filtrosPesquisa.CodigoPortoOrigem > 0)
                where.Append($" and CTe.POT_CODIGO_ORIGEM = {filtrosPesquisa.CodigoPortoOrigem}");

            if (filtrosPesquisa.CodigoPortoDestino > 0)
                where.Append($" and CTe.POT_CODIGO_DESTINO = {filtrosPesquisa.CodigoPortoDestino}");
        }

        #endregion
    }
}
