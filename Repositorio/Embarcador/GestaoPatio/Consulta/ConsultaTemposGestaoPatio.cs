using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.GestaoPatio
{
    sealed class ConsultaTemposGestaoPatio : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioTemposGestaoPatio>
    {
        #region Construtores

        public ConsultaTemposGestaoPatio() : base(tabela: "T_FLUXO_GESTAO_PATIO as FluxoGestaoPatio ") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsFilial(StringBuilder joins)
        {
            if (!joins.Contains(" Filial "))
                joins.Append(" left join T_FILIAL Filial on Filial.FIL_CODIGO = FluxoGestaoPatio.FIL_CODIGO ");
        }

        private void SetarJoinsModeloVeicular(StringBuilder joins)
        {
            if (!joins.Contains(" ModeloVeicular "))
                joins.Append(" left join T_MODELO_VEICULAR_CARGA ModeloVeicular on ModeloVeicular.MVC_CODIGO = ISNULL(Carga.MVC_CODIGO, PreCarga.MVC_CODIGO) ");
        }

        private void SetarJoinsCargaDadosSumarizados(StringBuilder joins)
        {
            if (!joins.Contains(" CargaDadosSumarizados "))
                joins.Append(" left join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on CargaDadosSumarizados.CDS_CODIGO = ISNULL(Carga.CDS_CODIGO, PreCarga.CDS_CODIGO) ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins.Append(" left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = ISNULL(Carga.CAR_VEICULO, PreCarga.CAR_VEICULO) ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" Transportador "))
                joins.Append(" left join T_EMPRESA Transportador on Transportador.EMP_CODIGO = ISNULL(Carga.EMP_CODIGO, PreCarga.EMP_CODIGO) ");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            if (!joins.Contains(" CargaPedido "))
                joins.Append(" left join T_CARGA_PEDIDO CargaPedido ON CargaPedido.CAR_CODIGO = FluxoGestaoPatio.CAR_CODIGO ");
        }

        private void SetarJoinsNotaFiscal(StringBuilder joins)
        {
            if (!joins.Contains(" NotaFiscal "))
                joins.Append("  left join T_XML_NOTA_FISCAL NotaFiscal ON NotaFiscal.NFX_CODIGO = PedidoNotaFisca.NFX_CODIGO ");
        }

        private void SetarJoinsPedidoNotaFiscal(StringBuilder joins)
        {
            if (!joins.Contains(" PedidoNotaFisca "))
                joins.Append(" left join T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFisca ON PedidoNotaFisca.CPE_CODIGO = CargaPedido.CPE_CODIGO ");
        }


        private void SetarJoinsTipoCarga(StringBuilder joins)
        {
            if (!joins.Contains(" TipoCarga "))
                joins.Append(" left join T_TIPO_DE_CARGA TipoCarga on TipoCarga.TCG_CODIGO = ISNULL(Carga.TCG_CODIGO, PreCarga.TCG_CODIGO) ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = ISNULL(Carga.TOP_CODIGO, PreCarga.TOP_CODIGO) ");
        }

        private void SetarJoinsRota(StringBuilder joins)
        {
            if (!joins.Contains(" Rota "))
                joins.Append(" left join T_ROTA_FRETE Rota on Rota.ROF_CODIGO = ISNULL(Carga.ROF_CODIGO, PreCarga.ROF_CODIGO) ");
        }

        private void SetarJoinsAreaVeiculoPosicao(StringBuilder joins)
        {
            if (!joins.Contains(" AreaVeiculoPosicao "))
                joins.Append(" left join T_AREA_VEICULO_POSICAO AreaVeiculoPosicao on AreaVeiculoPosicao.AVP_CODIGO = PreCarga.AVP_CODIGO ");
        }

        private void SetarJoinsAreaVeiculo(StringBuilder joins)
        {
            SetarJoinsAreaVeiculoPosicao(joins);

            if (!joins.Contains(" AreaVeiculo "))
                joins.Append(" left join T_AREA_VEICULO AreaVeiculo on AreaVeiculo.ARV_CODIGO = AreaVeiculoPosicao.ARV_CODIGO ");
        }

        private void SetarJoinsCargaJanelaCarregamentoGuarita(StringBuilder joins)
        {
            if (!joins.Contains(" CargaJanelaCarregamentoGuarita "))
                joins.Append(" left join T_CARGA_JANELA_CARREGAMENTO_GUARITA CargaJanelaCarregamentoGuarita on CargaJanelaCarregamentoGuarita.FGP_CODIGO = FluxoGestaoPatio.FGP_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioTemposGestaoPatio filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "Filial":
                    if (!select.Contains(" Filial, "))
                    {
                        select.Append("Filial.FIL_DESCRICAO Filial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "ModeloVeiculo":
                    if (!select.Contains(" ModeloVeiculo, "))
                    {
                        select.Append("ModeloVeicular.MVC_DESCRICAO ModeloVeiculo, ");
                        groupBy.Append("ModeloVeicular.MVC_DESCRICAO, ");

                        SetarJoinsModeloVeicular(joins);
                    }
                    break;

                case "Motorista":
                    if (!select.Contains(" Motorista, "))
                    {
                        select.Append(@"(CASE WHEN Carga.CAR_CODIGO IS NOT NULL 
                                        THEN
                                            (SUBSTRING((SELECT ', ' + motorista.FUN_NOME
		                                    FROM T_CARGA_MOTORISTA cargaMotorista
			                                JOIN T_FUNCIONARIO motorista on cargaMotorista.CAR_MOTORISTA = motorista.FUN_CODIGO 
		                                    WHERE cargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000))
                                        ELSE
                                            (SUBSTRING((SELECT ', ' + motorista.FUN_NOME
		                                    FROM T_PRE_CARGA_MOTORISTA preCargaMotorista
			                                JOIN T_FUNCIONARIO motorista on preCargaMotorista.PED_CODIGO = motorista.FUN_CODIGO 
		                                    WHERE preCargaMotorista.PCA_CODIGO = PreCarga.PCA_CODIGO FOR XML PATH('')), 3, 1000))
                                        END) Motorista, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        if (!groupBy.Contains("PreCarga.PCA_CODIGO,"))
                            groupBy.Append("PreCarga.PCA_CODIGO, ");
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga, "))
                    {
                        select.Append("ISNULL(Carga.CAR_CODIGO_CARGA_EMBARCADOR, PreCarga.PCA_NUMERO_CARGA) NumeroCarga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, PreCarga.PCA_NUMERO_CARGA, ");
                    }
                    break;

                case "NumeroDoca":
                    if (!select.Contains(" NumeroDoca, "))
                    {
                        select.Append(@"(CASE WHEN Carga.CAR_CODIGO IS NOT NULL
                                        THEN
                                            (CASE WHEN Carga.CAR_NUMERO_DOCA is not null and Carga.CAR_NUMERO_DOCA <> '' AND Carga.CAR_NUMERO_DOCA_ENCOSTA is not null and Carga.CAR_NUMERO_DOCA_ENCOSTA <> '' AND Carga.CAR_NUMERO_DOCA <> Carga.CAR_NUMERO_DOCA_ENCOSTA
                                                THEN Carga.CAR_NUMERO_DOCA + ' / ' + Carga.CAR_NUMERO_DOCA_ENCOSTA
                                            WHEN Carga.CAR_NUMERO_DOCA is not null and Carga.CAR_NUMERO_DOCA <> '' AND Carga.CAR_NUMERO_DOCA_ENCOSTA is not null and Carga.CAR_NUMERO_DOCA_ENCOSTA <> '' AND Carga.CAR_NUMERO_DOCA = Carga.CAR_NUMERO_DOCA_ENCOSTA
                                                THEN Carga.CAR_NUMERO_DOCA
                                            WHEN (Carga.CAR_NUMERO_DOCA is null or Carga.CAR_NUMERO_DOCA = '') AND Carga.CAR_NUMERO_DOCA_ENCOSTA is not null and Carga.CAR_NUMERO_DOCA_ENCOSTA <> ''
                                                THEN Carga.CAR_NUMERO_DOCA_ENCOSTA
                                            ELSE Carga.CAR_NUMERO_DOCA END)
                                        ELSE
                                        (AreaVeiculo.ARV_DESCRICAO + ' - ' + AreaVeiculoPosicao.AVP_DESCRICAO)
                                        END) NumeroDoca, ");

                        groupBy.Append("Carga.CAR_NUMERO_DOCA, Carga.CAR_NUMERO_DOCA_ENCOSTA, AreaVeiculo.ARV_DESCRICAO, AreaVeiculoPosicao.AVP_DESCRICAO, ");
                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsAreaVeiculo(joins);
                    }
                    break;

                case "SituacaoCargaFormatada":
                    if (!select.Contains(" SituacaoCarga, "))
                    {
                        select.Append("ISNULL(Carga.CAR_SITUACAO, -1) SituacaoCarga, ");
                        groupBy.Append("Carga.CAR_SITUACAO, ");
                    }
                    break;

                case "Remetente":
                    if (!select.Contains(" Remetente, "))
                    {
                        select.Append("CargaDadosSumarizados.CDS_CODIGO_INTEGRACAO_REMETENTES Remetente, ");
                        groupBy.Append("CargaDadosSumarizados.CDS_CODIGO_INTEGRACAO_REMETENTES, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "Destinatario":
                    if (!select.Contains(" Destinatario, "))
                    {
                        select.Append("CargaDadosSumarizados.CDS_CODIGO_INTEGRACAO_DESTINATARIOS Destinatario, ");
                        groupBy.Append("CargaDadosSumarizados.CDS_CODIGO_INTEGRACAO_DESTINATARIOS, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "Veiculo":
                    if (!select.Contains(" Veiculo, "))
                    {
                        select.Append("Veiculo.VEI_PLACA Veiculo, ");
                        groupBy.Append("Veiculo.VEI_PLACA, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "VeiculosVinculados":
                    if (!select.Contains(" VeiculosVinculados, "))
                    {
                        select.Append(@"(CASE WHEN Carga.CAR_CODIGO IS NOT NULL 
                                        THEN
                                            (SUBSTRING((SELECT ', ' + veiculo.VEI_PLACA
		                                    FROM T_CARGA_VEICULOS_VINCULADOS cargaReboque
			                                JOIN T_VEICULO veiculo on cargaReboque.VEI_CODIGO = veiculo.VEI_CODIGO 
		                                    WHERE cargaReboque.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000))
                                        ELSE
                                            (SUBSTRING((SELECT ', ' + veiculo.VEI_PLACA
		                                    FROM T_PRE_CARGA_VEICULOS_VINCULADOS preCargaReboque
			                                JOIN T_VEICULO veiculo on preCargaReboque.VEI_CODIGO = veiculo.VEI_CODIGO 
		                                    WHERE preCargaReboque.PCA_CODIGO = PreCarga.PCA_CODIGO FOR XML PATH('')), 3, 1000))
                                        END) VeiculosVinculados, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        if (!groupBy.Contains("PreCarga.PCA_CODIGO,"))
                            groupBy.Append("PreCarga.PCA_CODIGO, ");
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select.Append("Transportador.EMP_RAZAO Transportador, ");
                        groupBy.Append("Transportador.EMP_RAZAO, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "TipoCarga":
                    if (!select.Contains(" TipoCarga, "))
                    {
                        select.Append("TipoCarga.TCG_DESCRICAO TipoCarga, ");
                        groupBy.Append("TipoCarga.TCG_DESCRICAO, ");

                        SetarJoinsTipoCarga(joins);
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

                case "DataCarregamentoFormatada":
                    if (!select.Contains(" DataCarregamento, "))
                    {
                        select.Append("ISNULL(Carga.CAR_DATA_CARREGAMENTO, PreCarga.CAR_DATA_PREVISAO_ENTREGA) DataCarregamento, ");
                        groupBy.Append("Carga.CAR_DATA_CARREGAMENTO, PreCarga.CAR_DATA_PREVISAO_ENTREGA, ");
                    }
                    break;

                case "Peso":
                    if (!select.Contains(" Peso, "))
                    {
                        select.Append("CargaDadosSumarizados.CDS_PESO_TOTAL Peso, ");
                        groupBy.Append("CargaDadosSumarizados.CDS_PESO_TOTAL, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "AreaVeiculo":
                    if (!select.Contains(" AreaVeiculo, "))
                    {
                        select.Append(@"SUBSTRING((SELECT ', ' + areaVeiculo.ARV_DESCRICAO
		                                    FROM T_AREA_VEICULO areaVeiculo
			                                JOIN T_CARGA_AREA_VEICULO cargaAreaVeiculo on cargaAreaVeiculo.ARV_CODIGO = areaVeiculo.ARV_CODIGO 
		                                    WHERE cargaAreaVeiculo.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000) AreaVeiculo, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "Rota":
                    if (!select.Contains(" Rota, "))
                    {
                        select.Append("Rota.ROF_DESCRICAO Rota, ");
                        groupBy.Append("Rota.ROF_DESCRICAO, ");

                        SetarJoinsRota(joins);
                    }
                    break;

                case "ObservacaoFluxoPatio":
                    if (!select.Contains(" ObservacaoFluxoPatio, "))
                    {
                        select.Append(@"(SELECT TOP 1 cargaJanelaCarregamento.CJC_OBSERVACAO_FLUXO_PATIO 
                                        FROM T_CARGA_JANELA_CARREGAMENTO cargaJanelaCarregamento
                                        WHERE cargaJanelaCarregamento.CAR_CODIGO = Carga.CAR_CODIGO OR cargaJanelaCarregamento.PCA_CODIGO = PreCarga.PCA_CODIGO) ObservacaoFluxoPatio, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        if (!groupBy.Contains("PreCarga.PCA_CODIGO,"))
                            groupBy.Append("PreCarga.PCA_CODIGO, ");
                    }
                    break;

                case "DataDocaInformadaFormatada":
                    if (!select.Contains(" DataDocaInformada, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_DOCA_INFORMADA DataDocaInformada, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_DOCA_INFORMADA, ");
                    }
                    break;

                case "TempoAgInformarDocaDescricao":
                    SetarSelect("TempoAgInformarDoca", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "TempoAgInformarDoca":
                    if (!select.Contains(" TempoAgInformarDoca, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TEMPO_AG_INFORMAR_DOCA TempoAgInformarDoca, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TEMPO_AG_INFORMAR_DOCA, ");
                    }
                    break;

                case "DataChegadaVeiculoFormatada":
                    SetarSelect("DataChegadaVeiculo", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "PrevistoRealizadoChegadaVeiculoDescricao":
                    SetarSelect("DataChegadaVeiculo", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("DataChegadaVeiculoPrevista", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "TempoAgChegadaVeiculoDescricao":
                    SetarSelect("TempoAgChegadaVeiculo", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "TempoAgChegadaVeiculo":
                    if (!select.Contains(" TempoAgChegadaVeiculo, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TEMPO_AG_CHEGADA_VEICULO TempoAgChegadaVeiculo, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TEMPO_AG_CHEGADA_VEICULO, ");
                    }
                    break;

                case "DataChegadaVeiculo":
                    if (!select.Contains(" DataChegadaVeiculo, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_CHEGADA_VEICULO DataChegadaVeiculo, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_CHEGADA_VEICULO, ");
                    }
                    break;

                case "DataChegadaVeiculoPrevista":
                    if (!select.Contains(" DataChegadaVeiculoPrevista, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_CHEGADA_VEICULO_PREVISTA DataChegadaVeiculoPrevista, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_CHEGADA_VEICULO_PREVISTA, ");
                    }
                    break;

                case "DataEntregaGuaritaFormatada":
                    if (!select.Contains(" DataEntregaGuarita, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_DATA_ENTRADA_GUARITA DataEntregaGuarita, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_DATA_ENTRADA_GUARITA, ");
                    }
                    break;

                case "TempoAgEntradaGuaritaDescricao":
                    SetarSelect("TempoAgEntradaGuarita", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "TempoAgEntradaGuarita":
                    if (!select.Contains(" TempoAgEntradaGuarita, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TEMPO_AG_ENTRADA_GUARITA TempoAgEntradaGuarita, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TEMPO_AG_ENTRADA_GUARITA, ");
                    }
                    break;

                case "DataFimCheckListFormatada":
                    if (!select.Contains(" DataFimCheckList, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_DATA_FIM_CHECKLIST DataFimCheckList, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_DATA_FIM_CHECKLIST, ");
                    }
                    break;

                case "TempoAgChecklistDescricao":
                    SetarSelect("TempoAgChecklist", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "TempoAgChecklist":
                    if (!select.Contains(" TempoAgChecklist, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TEMPO_AG_CHECKLIST TempoAgChecklist, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TEMPO_AG_CHECKLIST, ");
                    }
                    break;

                case "DataTravaChaveFormatada":
                    if (!select.Contains(" DataTravaChave, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TRAVA_CHAVE DataTravaChave, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TRAVA_CHAVE, ");
                    }
                    break;

                case "TempoAgTravaChaveDescricao":
                    SetarSelect("TempoAgTravaChave", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "TempoAgTravaChave":
                    if (!select.Contains(" TempoAgTravaChave, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TEMPO_AG_TRAVA_CHAVE TempoAgTravaChave, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TEMPO_AG_TRAVA_CHAVE, ");
                    }
                    break;

                case "DataInicioCarregamentoFormatada":
                    if (!select.Contains(" DataInicioCarregamento, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_DATA_INICIO_CARREGAMENTO DataInicioCarregamento, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_DATA_INICIO_CARREGAMENTO, ");
                    }
                    break;

                case "TempoAgIncioCarregamentoDescricao":
                    SetarSelect("TempoAgInicioCarregamento", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "TempoAgInicioCarregamento":
                    if (!select.Contains(" TempoAgInicioCarregamento, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TEMPO_AG_INICIO_CARREGAMENTO TempoAgInicioCarregamento, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TEMPO_AG_INICIO_CARREGAMENTO, ");
                    }
                    break;

                case "DataFimCarregamentoFormatada":
                    if (!select.Contains(" DataFimCarregamento, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_DATA_FIM_CARREGAMENTO DataFimCarregamento, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_DATA_FIM_CARREGAMENTO, ");
                    }
                    break;

                case "TempoAgFimCarregamentoDescricao":
                    SetarSelect("TempoAgFimCarregamento", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "TempoAgFimCarregamento":
                    if (!select.Contains(" TempoAgFimCarregamento, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TEMPO_AG_FIM_CARREGAMENTO TempoAgFimCarregamento, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TEMPO_AG_FIM_CARREGAMENTO, ");
                    }
                    break;

                case "DataLiberacaoChaveFormatada":
                    if (!select.Contains(" DataLiberacaoChave, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_LIBERACAO_CHAVE DataLiberacaoChave, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_LIBERACAO_CHAVE, ");
                    }
                    break;

                case "TempoAgLiberacaoChaveDescricao":
                    SetarSelect("TempoAgLiberacaoChave", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "TempoAgLiberacaoChave":
                    if (!select.Contains(" TempoAgLiberacaoChave, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TEMPO_AG_LIBERACAO_CHAVE TempoAgLiberacaoChave, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TEMPO_AG_LIBERACAO_CHAVE, ");
                    }
                    break;

                case "DataFaturamentoFormatada":
                    if (!select.Contains(" DataFaturamento, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_FATURAMENTO DataFaturamento, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_FATURAMENTO, ");
                    }
                    break;

                case "TempoAgFaturamentoDescricao":
                    SetarSelect("TempoAgFaturamento", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "TempoAgFaturamento":
                    if (!select.Contains(" TempoAgFaturamento, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TEMPO_AG_FATURAMENTO TempoAgFaturamento, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TEMPO_AG_FATURAMENTO, ");
                    }
                    break;

                case "DataInicioViagemFormatada":
                    if (!select.Contains(" DataInicioViagem, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_DATA_SAIDA_GUARITA DataInicioViagem, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_DATA_SAIDA_GUARITA, ");
                    }
                    break;

                case "TempoAgInicioViagemDescricao":
                    SetarSelect("TempoAgInicioViagem", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "TempoAgInicioViagem":
                    if (!select.Contains(" TempoAgInicioViagem, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TEMPO_AG_INICIO_VIAGEM TempoAgInicioViagem, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TEMPO_AG_INICIO_VIAGEM, ");
                    }
                    break;

                case "DataPosicaoFormatada":
                    if (!select.Contains(" DataPosicao, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_POSICAO DataPosicao, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_POSICAO, ");
                    }
                    break;

                case "TempoAgPosicaoDescricao":
                    SetarSelect("TempoAgPosicao", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "TempoAgPosicao":
                    if (!select.Contains(" TempoAgPosicao, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TEMPO_AG_POSICAO TempoAgPosicao, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TEMPO_AG_POSICAO, ");
                    }
                    break;

                case "DataChegadaLojaFormatada":
                    if (!select.Contains(" DataChegadaLoja, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_CHEGADA_LOJA DataChegadaLoja, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_CHEGADA_LOJA, ");
                    }
                    break;

                case "TempoAgChegadaLojaDescricao":
                    SetarSelect("TempoAgChegadaLoja", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "TempoAgChegadaLoja":
                    if (!select.Contains(" TempoAgChegadaLoja, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TEMPO_AG_CHEGADA_LOJA TempoAgChegadaLoja, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TEMPO_AG_CHEGADA_LOJA, ");
                    }
                    break;

                case "DataDeslocamentoPatioFormatada":
                    if (!select.Contains(" DataDeslocamentoPatio, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_DESLOCAMENTO_PATIO DataDeslocamentoPatio, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_DESLOCAMENTO_PATIO, ");
                    }
                    break;

                case "TempoAgDeslocamentoPatioDescricao":
                    SetarSelect("TempoAgDeslocamentoPatio", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "TempoAgDeslocamentoPatio":
                    if (!select.Contains(" TempoAgDeslocamentoPatio, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TEMPO_AG_DESLOCAMENTO_PATIO TempoAgDeslocamentoPatio, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TEMPO_AG_DESLOCAMENTO_PATIO, ");
                    }
                    break;

                case "DataSaidaLojaFormatada":
                    if (!select.Contains(" DataSaidaLoja, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_SAIDA_LOJA DataSaidaLoja, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_SAIDA_LOJA, ");
                    }
                    break;

                case "TempoAgSaidaLojaDescricao":
                    SetarSelect("TempoAgSaidaLoja", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "TempoAgSaidaLoja":
                    if (!select.Contains(" TempoAgSaidaLoja, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TEMPO_AG_SAIDA_LOJA TempoAgSaidaLoja, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TEMPO_AG_SAIDA_LOJA, ");
                    }
                    break;

                case "DataFimViagemFormatada":
                    if (!select.Contains(" DataFimViagem, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_FIM_VIAGEM DataFimViagem, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_FIM_VIAGEM, ");
                    }
                    break;

                case "TempoAgFimViagemDescricao":
                    SetarSelect("TempoAgFimViagem", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "TempoAgFimViagem":
                    if (!select.Contains(" TempoAgFimViagem, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TEMPO_AG_FIM_VIAGEM TempoAgFimViagem, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TEMPO_AG_FIM_VIAGEM, ");
                    }
                    break;

                case "DataInicioHigienizacaoFormatada":
                    if (!select.Contains(" DataInicioHigienizacao, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_DATA_INICIO_HIGIENIZACAO DataInicioHigienizacao, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_DATA_INICIO_HIGIENIZACAO, ");
                    }
                    break;

                case "TempoAgInicioHigienizacaoDescricao":
                    SetarSelect("TempoAgInicioHigienizacao", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "TempoAgInicioHigienizacao":
                    if (!select.Contains(" TempoAgInicioHigienizacao, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TEMPO_AGUARDANDO_INICIO_HIGIENIZACAO TempoAgInicioHigienizacao, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TEMPO_AGUARDANDO_INICIO_HIGIENIZACAO, ");
                    }
                    break;

                case "DataFimHigienizacaoFormatada":
                    if (!select.Contains(" DataFimHigienizacao, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_DATA_FIM_HIGIENIZACAO DataFimHigienizacao, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_DATA_FIM_HIGIENIZACAO, ");
                    }
                    break;

                case "TempoAgFimHigienizacaoDescricao":
                    SetarSelect("TempoAgFimHigienizacao", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "TempoAgFimHigienizacao":
                    if (!select.Contains(" TempoAgFimHigienizacao, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TEMPO_AGUARDANDO_FIM_HIGIENIZACAO TempoAgFimHigienizacao, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TEMPO_AGUARDANDO_FIM_HIGIENIZACAO, ");
                    }
                    break;

                case "DataSolicitacaoVeiculoFormatada":
                    if (!select.Contains(" DataSolicitacaoVeiculo, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_DATA_SOLICITACAO_VEICULO DataSolicitacaoVeiculo, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_DATA_SOLICITACAO_VEICULO, ");
                    }
                    break;

                case "TempoAgSolicitacaoVeiculoDescricao":
                    SetarSelect("TempoAgSolicitacaoVeiculo", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "TempoAgSolicitacaoVeiculo":
                    if (!select.Contains(" TempoAgSolicitacaoVeiculo, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TEMPO_AGUARDANDO_SOLICITACAO_VEICULO TempoAgSolicitacaoVeiculo, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TEMPO_AGUARDANDO_SOLICITACAO_VEICULO, ");
                    }
                    break;

                case "DataInicioDescarregamentoFormatada":
                    if (!select.Contains(" DataInicioDescarregamento, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_DATA_INICIO_DESCARREGAMENTO DataInicioDescarregamento, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_DATA_INICIO_DESCARREGAMENTO, ");
                    }
                    break;

                case "TempoAgInicioDescarregamentoDescricao":
                    SetarSelect("TempoAgInicioDescarregamento", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "TempoAgInicioDescarregamento":
                    if (!select.Contains(" TempoAgInicioDescarregamento, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TEMPO_AGUARDANDO_INICIO_DESCARREGAMENTO TempoAgInicioDescarregamento, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TEMPO_AGUARDANDO_INICIO_DESCARREGAMENTO, ");
                    }
                    break;

                case "DataFimDescarregamentoFormatada":
                    if (!select.Contains(" DataFimDescarregamento, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_DATA_FIM_DESCARREGAMENTO DataFimDescarregamento, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_DATA_FIM_DESCARREGAMENTO, ");
                    }
                    break;

                case "TempoAgFimDescarregamentoDescricao":
                    SetarSelect("TempoAgFimDescarregamento", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "TempoAgFimDescarregamento":
                    if (!select.Contains(" TempoAgFimDescarregamento, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TEMPO_AGUARDANDO_FIM_DESCARREGAMENTO TempoAgFimDescarregamento, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TEMPO_AGUARDANDO_FIM_DESCARREGAMENTO, ");
                    }
                    break;

                case "DataDocumentoFiscalFormatada":
                    if (!select.Contains(" DataDocumentoFiscal, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_DATA_DOCUMENTO_FISCAL DataDocumentoFiscal, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_DATA_DOCUMENTO_FISCAL, ");
                    }
                    break;

                case "TempoAgDocumentoFiscalDescricao":
                    SetarSelect("TempoAgDocumentoFiscal", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "TempoAgDocumentoFiscal":
                    if (!select.Contains(" TempoAgDocumentoFiscal, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TEMPO_AGUARDANDO_DOCUMENTO_FISCAL TempoAgDocumentoFiscal, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TEMPO_AGUARDANDO_DOCUMENTO_FISCAL, ");
                    }
                    break;

                case "DataDocumentosTransporteFormatada":
                    if (!select.Contains(" DataDocumentosTransporte, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_DATA_DOCUMENTOS_TRANSPORTE DataDocumentosTransporte, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_DATA_DOCUMENTOS_TRANSPORTE, ");
                    }
                    break;

                case "TempoAgDocumentosTransporteDescricao":
                    SetarSelect("TempoAgDocumentosTransporte", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "TempoAgDocumentosTransporte":
                    if (!select.Contains(" TempoAgDocumentosTransporte, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TEMPO_AGUARDANDO_DOCUMENTOS_TRANSPORTE TempoAgDocumentosTransporte, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TEMPO_AGUARDANDO_DOCUMENTOS_TRANSPORTE, ");
                    }
                    break;

                case "DataMontagemCargaFormatada":
                    if (!select.Contains(" DataMontagemCarga, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_DATA_MONTAGEM_CARGA DataMontagemCarga, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_DATA_MONTAGEM_CARGA, ");
                    }
                    break;

                case "TempoAgMontagemCargaDescricao":
                    SetarSelect("TempoAgMontagemCarga", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "TempoAgMontagemCarga":
                    if (!select.Contains(" TempoAgMontagemCarga, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TEMPO_AGUARDANDO_MONTAGEM_CARGA TempoAgMontagemCarga, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TEMPO_AGUARDANDO_MONTAGEM_CARGA, ");
                    }
                    break;

                case "DataSeparacaoMercadoriaFormatada":
                    if (!select.Contains(" DataSeparacaoMercadoria, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_DATA_SEPARACAO_MERCADORIA DataSeparacaoMercadoria, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_DATA_SEPARACAO_MERCADORIA, ");
                    }
                    break;

                case "TempoAgSeparacaoMercadoriaDescricao":
                    SetarSelect("TempoAgSeparacaoMercadoria", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "TempoAgSeparacaoMercadoria":
                    if (!select.Contains(" TempoAgSeparacaoMercadoria, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TEMPO_AGUARDANDO_SEPARACAO_MERCADORIA TempoAgSeparacaoMercadoria, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TEMPO_AGUARDANDO_SEPARACAO_MERCADORIA, ");
                    }
                    break;

                case "SomaTotalDosTempos":
                    SetarSelect("TempoAgInformarDoca", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("TempoAgChegadaVeiculo", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("TempoAgEntradaGuarita", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("TempoAgChecklist", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("TempoAgTravaChave", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("TempoAgInicioCarregamento", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("TempoAgFimCarregamento", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("TempoAgLiberacaoChave", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("TempoAgFaturamento", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("TempoAgInicioViagem", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("TempoAgPosicao", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("TempoAgChegadaLoja", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("TempoAgDeslocamentoPatio", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("TempoAgSaidaLoja", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("TempoAgFimViagem", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("TempoAgInicioHigienizacao", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("TempoAgFimHigienizacao", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("TempoAgSolicitacaoVeiculo", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("TempoAgInicioDescarregamento", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("TempoAgFimDescarregamento", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("TempoAgDocumentoFiscal", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("TempoAgDocumentosTransporte", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("TempoAgMontagemCarga", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("TempoAgSeparacaoMercadoria", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "PesoChegadaVeiculo":
                    if (!select.Contains(" PesoChegadaVeiculo, "))
                    {
                        select.Append("CargaJanelaCarregamentoGuarita.CJC_PESAGEM_INICIAL PesoChegadaVeiculo, ");
                        groupBy.Append("CargaJanelaCarregamentoGuarita.CJC_PESAGEM_INICIAL, ");

                        SetarJoinsCargaJanelaCarregamentoGuarita(joins);
                    }
                    break;

                case "PesoSaidaVeiculo":
                    if (!select.Contains(" PesoSaidaVeiculo, "))
                    {
                        select.Append("CargaJanelaCarregamentoGuarita.CJC_PESAGEM_FINAL PesoSaidaVeiculo, ");
                        groupBy.Append("CargaJanelaCarregamentoGuarita.CJC_PESAGEM_FINAL, ");

                        SetarJoinsCargaJanelaCarregamentoGuarita(joins);
                    }
                    break;
                case "CodigoTransportador":
                    if (!select.Contains(" CodigoTransportador, "))
                    {
                        select.Append("Transportador.EMP_CODIGO_INTEGRACAO CodigoTransportador, ");
                        groupBy.Append("Transportador.EMP_CODIGO_INTEGRACAO, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;
                case "ValorTotalNotaFiscal":
                    if (!select.Contains(" ValorTotalNotaFiscal, "))
                    {
                        select.Append("SUM(NotaFiscal.NF_VALOR) ValorTotalNotaFiscal, ");
                        groupBy.Append("NotaFiscal.NF_VALOR, ");

                        SetarJoinsCargaPedido(joins);
                        SetarJoinsPedidoNotaFiscal(joins);
                        SetarJoinsNotaFiscal(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioTemposGestaoPatio filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            //string pattern = "yyyy-MM-dd";
            string patternHour = "yyyy-MM-dd HH:mm";

            joins.Insert(0, @"
                left join T_CARGA Carga on Carga.CAR_CODIGO = FluxoGestaoPatio.CAR_CODIGO
                left join T_PRE_CARGA PreCarga on PreCarga.PCA_CODIGO = FluxoGestaoPatio.PCA_CODIGO "
            );

            where.Append(" and (FluxoGestaoPatio.CAR_CODIGO is null or Carga.CAR_CARGA_FECHADA = 1)");

            if (!filtrosPesquisa.ListarCargasCanceladas)
            {
                where.Append($" and (FluxoGestaoPatio.CAR_CODIGO is null or (Carga.CAR_SITUACAO <> {(int)SituacaoCarga.Cancelada} and Carga.CAR_SITUACAO <> {(int)SituacaoCarga.Anulada}))");
                where.Append($" and (FluxoGestaoPatio.PCA_CODIGO is null or PreCarga.PCA_SITUACAO <> {(int)SituacaoPreCarga.Cancelada})");
            }

            if (filtrosPesquisa.DataInicioCarregamento != DateTime.MinValue)
                where.Append($" and (Carga.CAR_DATA_CARREGAMENTO >= '{ filtrosPesquisa.DataInicioCarregamento.ToString(patternHour) }' or PreCarga.CAR_DATA_PREVISAO_ENTREGA >= '{ filtrosPesquisa.DataInicioCarregamento.ToString(patternHour) }')");

            if (filtrosPesquisa.DataFimCarregamento != DateTime.MinValue)
                where.Append($" and (Carga.CAR_DATA_CARREGAMENTO <= '{ filtrosPesquisa.DataFimCarregamento.ToString(patternHour) }:59' or PreCarga.CAR_DATA_PREVISAO_ENTREGA <= '{ filtrosPesquisa.DataFimCarregamento.ToString(patternHour) }:59')");

            if (filtrosPesquisa.CodigosFilial.Count > 0)
                where.Append($" and FluxoGestaoPatio.FIL_CODIGO in ({string.Join(", ",filtrosPesquisa.CodigosFilial)})");

            if (filtrosPesquisa.CodigoMotorista > 0)
            {
                where.Append($@" and (
	                                    exists (
		                                    select motorista.FUN_CODIGO 
		                                    from T_CARGA_MOTORISTA cargaMotorista
			                                join T_FUNCIONARIO motorista on cargaMotorista.CAR_MOTORISTA = motorista.FUN_CODIGO 
		                                    where cargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO and motorista.FUN_CODIGO = {filtrosPesquisa.CodigoMotorista}
	                                    )
	                                    or exists (
		                                    select motorista.FUN_CODIGO 
		                                    from T_PRE_CARGA_MOTORISTA preCargaMotorista
			                                join T_FUNCIONARIO motorista on preCargaMotorista.PED_CODIGO = motorista.FUN_CODIGO 
		                                    where preCargaMotorista.PCA_CODIGO = PreCarga.PCA_CODIGO and motorista.FUN_CODIGO = {filtrosPesquisa.CodigoMotorista}
	                                    )
                                    )");
            }

            if (filtrosPesquisa.CodigoTipoCarga > 0)
                where.Append($" and (Carga.TCG_CODIGO = {filtrosPesquisa.CodigoTipoCarga} or PreCarga.TCG_CODIGO = {filtrosPesquisa.CodigoTipoCarga})");

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                where.Append($" and (Carga.TOP_CODIGO = {filtrosPesquisa.CodigoTipoOperacao} or PreCarga.TOP_CODIGO = {filtrosPesquisa.CodigoTipoOperacao})");

            if (filtrosPesquisa.CodigoRota > 0)
                where.Append($" and (Carga.ROF_CODIGO = {filtrosPesquisa.CodigoRota} or PreCarga.ROF_CODIGO = {filtrosPesquisa.CodigoRota})");

            if (filtrosPesquisa.CodigoTransportador > 0)
                where.Append($" and (Carga.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador} or PreCarga.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador})");

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                where.Append($@" and (  Carga.CAR_VEICULO = {filtrosPesquisa.CodigoVeiculo} or PreCarga.CAR_VEICULO = {filtrosPesquisa.CodigoVeiculo}
	                                    or exists (
		                                    select veiculo.VEI_CODIGO 
		                                    from T_CARGA_VEICULOS_VINCULADOS cargaReboque
			                                join T_VEICULO veiculo on cargaReboque.VEI_CODIGO = veiculo.VEI_CODIGO 
		                                    where cargaReboque.CAR_CODIGO = Carga.CAR_CODIGO and veiculo.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo}
	                                    )
	                                    or exists (
		                                    select veiculo.VEI_CODIGO 
		                                    from T_PRE_CARGA_VEICULOS_VINCULADOS preCargaReboque
			                                join T_VEICULO veiculo on preCargaReboque.VEI_CODIGO = veiculo.VEI_CODIGO 
		                                    where preCargaReboque.PCA_CODIGO = PreCarga.PCA_CODIGO and veiculo.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo}
	                                    )
                                    )");
            }

            if (filtrosPesquisa.EtapaFluxoGestaoPatio != EtapaFluxoGestaoPatio.Todas)
                where.Append($" and FluxoGestaoPatio.FGE_ETAPA_FLUXO_GESTAO_ATUAL = {filtrosPesquisa.EtapaFluxoGestaoPatio.ToString("d")}");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}'");

            if (filtrosPesquisa.Situacao.HasValue)
                where.Append($" and FluxoGestaoPatio.FGP_SITUACAO_ETAPA_FLUXO_GESTAO = {filtrosPesquisa.Situacao.Value.ToString("d")}");
        }

        #endregion
    }
}
