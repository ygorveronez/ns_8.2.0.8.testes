using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas
{
    sealed class ConsultaCarga : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga>
    {
        #region Construtores

        public ConsultaCarga() : base(tabela: "T_CARGA as Carga") { }

        #endregion Construtores

        #region Métodos Privados

        private void SetarJoinsCargaAnulacao(StringBuilder joins)
        {
            if (!joins.Contains(" CargaAnulacao "))
                joins.Append(" left join T_CARGA_CANCELAMENTO CargaAnulacao on CargaAnulacao.CAR_CODIGO = isnull(Carga.CAR_CODIGO_AGRUPAMENTO, Carga.CAR_CODIGO) AND CargaAnulacao.CAC_TIPO = 1 ");
        }

        private void SetarJoinsCargaCancelamento(StringBuilder joins)
        {
            if (!joins.Contains(" CargaCancelamento "))
                joins.Append(" left join T_CARGA_CANCELAMENTO CargaCancelamento on CargaCancelamento.CAR_CODIGO = isnull(Carga.CAR_CODIGO_AGRUPAMENTO, Carga.CAR_CODIGO) AND CargaCancelamento.CAC_TIPO = 0 ");
        }

        private void SetarJoinsTerceiro(StringBuilder joins)
        {
            if (!joins.Contains(" Terceiro "))
                joins.Append(" left join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF =  Carga.CLI_CGCCPF_TERCEIRO");
        }

        private void SetarJoinsClienteModalidadeTerceiro(StringBuilder joins)
        {
            if (!joins.Contains(" ClienteModalidadeTerceiro "))
                joins.Append(" left join T_CLIENTE_MODALIDADE ClienteModalidadeTerceiro on ClienteModalidadeTerceiro.CPF_CNPJ = Terceiro.CLI_CGCCPF and ClienteModalidadeTerceiro.MOD_TIPO = 3");
        }

        private void SetarJoinsClienteModalidadeTransportadorTerceiro(StringBuilder joins)
        {
            if (!joins.Contains(" ClienteModalidadeTransportadorTerceiro "))
                joins.Append(" left join T_CLIENTE_MODALIDADE_TRANSPORTADORAS ClienteModalidadeTransportadorTerceiro on ClienteModalidadeTransportadorTerceiro.MOD_CODIGO = ClienteModalidadeTerceiro.MOD_CODIGO");
        }

        private void SetarJoinsTipoTerceiro(StringBuilder joins)
        {
            if (!joins.Contains(" TipoTerceiro "))
            {
                SetarJoinsTerceiro(joins);
                SetarJoinsClienteModalidadeTerceiro(joins);
                SetarJoinsClienteModalidadeTransportadorTerceiro(joins);
                joins.Append(" left join T_TIPO_TERCEIRO TipoTerceiro on TipoTerceiro.TPT_CODIGO =  ClienteModalidadeTransportadorTerceiro.TPT_CODIGO");
            }
        }




        private void SetarJoinsCargaDadosSumarizados(StringBuilder joins)
        {
            if (!joins.Contains(" DadosSumarizados "))
                joins.Append(" left join T_CARGA_DADOS_SUMARIZADOS DadosSumarizados ON DadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO ");
        }

        private void SetarJoinsContratoFreteTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" ContratoFreteTransportador "))
                joins.Append(" left join T_CONTRATO_FRETE_TRANSPORTADOR ContratoFreteTransportador on ContratoFreteTransportador.CFT_CODIGO = Carga.CFT_CODIGO ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            if (!joins.Contains(" Filial "))
                joins.Append(" left join T_FILIAL Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
        }

        private void SetarJoinsGrupoPessoas(StringBuilder joins)
        {
            if (!joins.Contains(" GrupoPessoas "))
                joins.Append(" left join T_GRUPO_PESSOAS GrupoPessoas on GrupoPessoas.GRP_CODIGO = Carga.GRP_CODIGO ");
        }

        private void SetarJoinsJanelaCarregamento(StringBuilder joins)
        {
            if (!joins.Contains(" JanelaCarregamento "))
                joins.Append(" left join T_CARGA_JANELA_CARREGAMENTO JanelaCarregamento on JanelaCarregamento.CAR_CODIGO = Carga.CAR_CODIGO ");
        }

        private void SetarJoinsJanelaDescarregamento(StringBuilder joins)
        {
            if (!joins.Contains(" JanelaDescarregamento "))
                joins.Append(" left join T_CARGA_JANELA_DESCARREGAMENTO JanelaDescarregamento on JanelaDescarregamento.CAR_CODIGO = Carga.CAR_CODIGO and isnull(JanelaDescarregamento.CJD_CANCELADA, 0) = 0 ");
        }

        private void SetarJoinsFluxoGestaoPatio(StringBuilder joins)
        {
            if (!joins.Contains(" FluxoGestaoPatio "))
                joins.Append($" left join T_FLUXO_GESTAO_PATIO FluxoGestaoPatio on FluxoGestaoPatio.CAR_CODIGO = Carga.CAR_CODIGO and FluxoGestaoPatio.FGE_TIPO = {(int)TipoFluxoGestaoPatio.Origem} and FluxoGestaoPatio.FGP_SITUACAO_ETAPA_FLUXO_GESTAO <> {(int)SituacaoEtapaFluxoGestaoPatio.Cancelado} ");
        }

        private void SetarJoinsSimulacaoFreteCarregamento(StringBuilder joins)
        {
            if (!joins.Contains(" SimulacaoFreteCarregamento "))
                joins.Append($" left join T_SIMULACAO_FRETE_CARREGAMENTO SimulacaoFreteCarregamento on SimulacaoFreteCarregamento.CRG_CODIGO = Carga.CRG_CODIGO ");
        }

        private void SetarJoinsCargaJanelaCarregamentoGuarita(StringBuilder joins)
        {
            SetarJoinsFluxoGestaoPatio(joins);

            if (!joins.Contains(" CargaJanelaCarregamentoGuarita "))
                joins.Append(" left join T_CARGA_JANELA_CARREGAMENTO_GUARITA CargaJanelaCarregamentoGuarita on CargaJanelaCarregamentoGuarita.FGP_CODIGO = FluxoGestaoPatio.FGP_CODIGO");
        }

        private void SetarJoinsModeloVeicularCarga(StringBuilder joins)
        {
            if (!joins.Contains(" ModeloVeiculo "))
                joins.Append(" left join T_MODELO_VEICULAR_CARGA ModeloVeiculo on ModeloVeiculo.MVC_CODIGO = Carga.MVC_CODIGO ");
        }

        private void SetarJoinsMontagemCarga(StringBuilder joins)
        {
            if (!joins.Contains(" MontagemCarga "))
                joins.Append(" left join T_CARREGAMENTO MontagemCarga on MontagemCarga.CRG_CODIGO = Carga.CRG_CODIGO ");
        }

        private void SetarJoinsOperadorAnulacao(StringBuilder joins)
        {
            SetarJoinsCargaAnulacao(joins);

            if (!joins.Contains(" OperadorAnulacao "))
                joins.Append(" left join T_FUNCIONARIO OperadorAnulacao on OperadorAnulacao.FUN_CODIGO = CargaAnulacao.FUN_CODIGO ");
        }

        private void SetarJoinsOperadorCancelamento(StringBuilder joins)
        {
            SetarJoinsCargaCancelamento(joins);

            if (!joins.Contains(" OperadorCancelamento "))
                joins.Append(" left join T_FUNCIONARIO OperadorCancelamento on OperadorCancelamento.FUN_CODIGO = CargaCancelamento.FUN_CODIGO ");
        }

        private void SetarJoinsOperadorCarga(StringBuilder joins)
        {
            if (!joins.Contains(" OperadorCarga "))
                joins.Append(" left join T_FUNCIONARIO OperadorCarga on OperadorCarga.FUN_CODIGO = Carga.CAR_OPERADOR ");
        }

        private void SetarJoinsAprovador(StringBuilder joins)
        {
            SetarJoinsAutorizacao(joins);

            if (!joins.Contains(" FuncioanrioAprovacao "))
                joins.Append(" LEFT OUTER JOIN T_FUNCIONARIO FuncioanrioAprovacao on FuncioanrioAprovacao.FUN_CODIGO = Autorizacao.FUN_CODIGO ");
        }

        private void SetarJoinsAutorizacao(StringBuilder joins)
        {
            if (!joins.Contains(" Autorizacao "))
                joins.Append(" LEFT OUTER JOIN T_AUTORIZACAO_ALCADA_CARGA Autorizacao on Carga.CAR_CODIGO = Autorizacao.CAR_CODIGO and Autorizacao.AAL_SITUACAO = 1");
        }

        private void SetarJoinsPreCarga(StringBuilder joins)
        {
            if (!joins.Contains(" PreCarga "))
                joins.Append(" left join T_PRE_CARGA PreCarga ON PreCarga.CAR_CODIGO = Carga.CAR_CODIGO ");
        }

        private void SetarJoinsRota(StringBuilder joins)
        {
            if (!joins.Contains(" Rota "))
                joins.Append(" left join T_ROTA_FRETE Rota on Rota.ROF_CODIGO = Carga.ROF_CODIGO ");
        }

        private void SetarJoinsTabelaFrete(StringBuilder joins)
        {
            if (!joins.Contains(" TabelaFrete "))
                joins.Append(" left join T_TABELA_FRETE TabelaFrete on TabelaFrete.TBF_CODIGO = Carga.TBF_CODIGO ");
        }

        private void SetarJoinsTipoCarga(StringBuilder joins)
        {
            if (!joins.Contains(" TipoCarga "))
                joins.Append(" left join T_TIPO_DE_CARGA TipoCarga on TipoCarga.TCG_CODIGO = Carga.TCG_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }
        private void SetarJoinsNumeroDtNatura(StringBuilder joins)
        {
            if (!joins.Contains(" NumeroDtNatura "))
                joins.Append(" left join T_CARGA_INTEGRACAO_NATURA IntegracaoNatura on IntegracaoNatura.CAR_CODIGO = Carga.CAR_CODIGO ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" Transportador "))
                joins.Append(" left join T_EMPRESA Transportador on Transportador.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        private void SetarJoinsViagem(StringBuilder joins)
        {
            if (!joins.Contains(" Viagem "))
                joins.Append(" left join T_PEDIDO_VIAGEM_NAVIO Viagem on Viagem.PVN_CODIGO = Carga.PVN_CODIGO ");
        }

        private void SetarJoinsViagemSchedule(StringBuilder joins)
        {
            if (!joins.Contains(" ViagemSchedule "))
                joins.Append(" left join T_PEDIDO_VIAGEM_NAVIO_SCHEDULE ViagemSchedule on ViagemSchedule.PVN_CODIGO = Carga.PVN_CODIGO AND ViagemSchedule.POT_CODIGO_ATRACACAO = Carga.POT_CODIGO_ORIGEM ");
        }

        private void SetarJoinsViagemScheduleDestino(StringBuilder joins)
        {
            if (!joins.Contains(" ViagemScheduleDestino "))
                joins.Append(" left join T_PEDIDO_VIAGEM_NAVIO_SCHEDULE ViagemScheduleDestino on ViagemScheduleDestino.PVN_CODIGO = Carga.PVN_CODIGO AND ViagemScheduleDestino.TTI_CODIGO_ATRACACAO = Carga.TTI_CODIGO_DESTINO ");
        }

        private void SetarJoinsCargaAgrupada(StringBuilder joins)
        {
            if (!joins.Contains(" CargaAgrupada "))
                joins.Append(" left join T_CARGA CargaAgrupada on CargaAgrupada.CAR_CODIGO = Carga.CAR_CODIGO_AGRUPAMENTO ");
        }

        private void SetarJoinsTipoSeparacao(StringBuilder joins)
        {
            SetarJoinsMontagemCarga(joins);

            if (!joins.Contains(" TipoSeparacao "))
                joins.Append(" left join T_TIPO_SEPARACAO TipoSeparacao on TipoSeparacao.TSE_CODIGO = MontagemCarga.TSE_CODIGO ");
        }

        private void SetarJoinsConfiguracaoTransportador(StringBuilder joins)
        {
            SetarJoinsTransportador(joins);

            if (!joins.Contains(" ConfiguracaoTransportador "))
                joins.Append(" left join T_CONFIG ConfiguracaoTransportador on ConfiguracaoTransportador.COF_CODIGO = Transportador.COF_CODIGO ");
        }

        private void SetarJoinsFaixaTemperatura(StringBuilder joins)
        {
            if (!joins.Contains(" FaixaTemperatura "))
                joins.Append(" left join T_FAIXA_TEMPERATURA FaixaTemperatura on FaixaTemperatura.FTE_CODIGO = Carga.FTE_CODIGO ");
        }

        private void SetarJoinsRetiradaContainer(StringBuilder joins)
        {
            if (!joins.Contains(" RetiradaContainer "))
                joins.Append(" left join T_RETIRADA_CONTAINER RetiradaContainer on RetiradaContainer.CAR_CODIGO = Carga.CAR_CODIGO ");
        }

        private void SetarJoinsClienteRetiradaContainer(StringBuilder joins)
        {
            SetarJoinsRetiradaContainer(joins);

            if (!joins.Contains(" ClienteRetiradaContainer "))
                joins.Append(" left join T_CLIENTE ClienteRetiradaContainer on ClienteRetiradaContainer.CLI_CGCCPF = RetiradaContainer.CLI_CODIGO_LOCAL ");
        }

        private void SetarJoinCargaVeiculoContainer(StringBuilder joins)
        {
            if (!joins.Contains(" CargaVeiculoContainer "))
                joins.Append(" left join T_CARGA_VEICULO_CONTAINER CargaVeiculoContainer on CargaVeiculoContainer.CAR_CODIGO = Carga.CAR_CODIGO AND CargaVeiculoContainer.CVC_NUMERO_CONTAINER != ''");
        }

        private void SetarJoinsCargaRotaFrete(StringBuilder joins)
        {
            if (!joins.Contains(" cargaRotaFrete "))
                joins.Append(" left join T_CARGA_ROTA_FRETE cargaRotaFrete on cargaRotaFrete.CAR_CODIGO = Carga.CAR_CODIGO");
        }

        private void SetarJoinsCargaVeiculoContainerAnexo(StringBuilder joins)
        {
            SetarJoinsCargaVeiculoContainer(joins);

            if (!joins.Contains(" CargaVeiculoContainerAnexo "))
                joins.Append(" left join T_CARGA_VEICULO_CONTAINER_ANEXOS CargaVeiculoContainerAnexo on CargaVeiculoContainerAnexo.CVC_CODIGO = CargaVeiculoContainer.CVC_CODIGO");

        }

        private void SetarJoinsCargaVeiculoContainer(StringBuilder joins)
        {
            if (!joins.Contains(" CargaVeiculoContainer "))
                joins.Append(" left join T_CARGA_VEICULO_CONTAINER CargaVeiculoContainer on CargaVeiculoContainer.CAR_CODIGO = Carga.CAR_CODIGO");
        }

        private void SetarJoinsTipoAnexoVeiculoContainer(StringBuilder joins)
        {
            if (!joins.Contains(" CargaVeiculoTipoAnexo "))
                joins.Append(" left join T_CARGA_TIPO_ANEXO CargaVeiculoTipoAnexo on CargaVeiculoTipoAnexo.CTA_CODIGO = CargaVeiculoContainerAnexo.CTA_CODIGO");

            SetarJoinsCargaVeiculoContainerAnexo(joins);
        }

        private void SetarJoinsDocumentoTransporte(StringBuilder joins)
        {
            if (!joins.Contains(" DocumentoTransporteCarga "))
                joins.Append(" left join T_CARGA_DADOS_PARA_PROCESSAMENTO_DT DocumentoTransporteCarga on DocumentoTransporteCarga.CAR_CODIGO = Carga.CAR_CODIGO");
        }

        private void SetarJoinsEncerramentoManualViagem(StringBuilder joins)
        {
            if (!joins.Contains(" EncerramentoManualViagem "))
                joins.Append(" left join T_ENCERRAMENTO_MANUAL_VIAGEM EncerramentoManualViagem on Carga.EMV_CODIGO = EncerramentoManualViagem.EMV_CODIGO");
        }

        private void SetarJoinsCargaTrechos(StringBuilder joins)
        {
            if (!joins.Contains(" StageAgrupamento "))
                joins.Append(" left join T_STAGE_AGRUPAMENTO StageAgrupamento on StageAgrupamento.CAR_CODIGO = Carga.CAR_CODIGO");
        }

        private void SetarJoinsMonitoramento(StringBuilder joins)
        {
            if (!joins.Contains(" Monitoramento "))
            {
                joins.Append($@"
                    left join T_MONITORAMENTO Monitoramento on Monitoramento.MON_CODIGO in (
                        select max(_monitoramento.MON_CODIGO)
                          from T_MONITORAMENTO _monitoramento
                         where _monitoramento.CAR_CODIGO = Carga.CAR_CODIGO
                           and _monitoramento.VEI_CODIGO = Carga.CAR_VEICULO
                           and _monitoramento.MON_STATUS in ({(int)MonitoramentoStatus.Iniciado}, {(int)MonitoramentoStatus.Finalizado})
                    ) "
                );
            }
        }

        private void SetarJoinsCargaRelacionada(StringBuilder joins)
        {
            if (!joins.Contains(" CargaRelacionada "))
                joins.Append(" LEFT JOIN T_CARGA_RELACIONADA CargaRelacionada on Carga.CAR_CODIGO = CargaRelacionada.CAR_CODIGO");
        }

        private void SetarJoinCargaCancelamentoJustificativa(StringBuilder joins)
        {
            SetarJoinsCargaCancelamento(joins);

            if (!joins.Contains(" JustificativaCancelamento "))
                joins.Append(" left join T_CARGA_CANCELAMENTO_JUSTIFICATIVA JustificativaCancelamento on JustificativaCancelamento.TCJ_CODIGO = CargaCancelamento.TCJ_CODIGO");

        }

        private void SetarJoinContratoFreteTerceiro(StringBuilder joins)
        {
            if (!joins.Contains(" ContratoFreteTerceiro "))
                joins.Append(" LEFT JOIN T_CONTRATO_FRETE_TERCEIRO ContratoFreteTerceiro on Carga.CAR_CODIGO = ContratoFreteTerceiro.CAR_CODIGO");
        }

        private void SetarJoinJustificativaAutorizacaoCarga(StringBuilder joins)
        {
            if (!joins.Contains(" JustificativaAutorizacaoCarga "))
                joins.Append(" LEFT JOIN T_JUSTIFICATIVA_AUTORIZACAO_CARGA JustificativaAutorizacaoCarga on Carga.JAC_CODIGO = JustificativaAutorizacaoCarga.JAC_CODIGO");
        }

        private void SetarJoinSetor(StringBuilder joins)
        {
            if (!joins.Contains(" Setor "))
                joins.Append(" LEFT JOIN T_SETOR Setor on Carga.SET_CODIGO = Setor.SET_CODIGO");
        }

        private void SetarJoinUsuarioAlteracaoFrete(StringBuilder joins)
        {
            if (!joins.Contains(" UsuarioAlteracaoFrete "))
                joins.Append(" LEFT OUTER JOIN T_FUNCIONARIO UsuarioAlteracaoFrete on Carga.CAR_USUARIO_AUTORIZOU_ALTERACAO_FRETE = UsuarioAlteracaoFrete.FUN_CODIGO ");
        }

        private void SetarJoinConfiguracaoTipoOperacaoCarga(StringBuilder joins)
        {
            SetarJoinsTipoOperacao(joins);

            if (!joins.Contains(" ConfiguracaoTipoOperacaoCarga "))
                joins.Append(" LEFT JOIN T_CONFIGURACAO_TIPO_OPERACAO_CARGA ConfiguracaoTipoOperacaoCarga ON TipoOperacao.CCG_CODIGO = ConfiguracaoTipoOperacaoCarga.CCG_CODIGO");
        }

        private void SetarJoinsPercentualExecucao(StringBuilder joins)
        {
            if (!joins.Contains(" PercentualPendente ")) // Inclui apenas os percentuais com valor abaixo de 100%
                joins.Append(" INNER JOIN T_COMISSAO_FUNCIONARIO_MOTORISTA_DOCUMENTO PercentualPendente ON Carga.CAR_CODIGO = PercentualPendente.CAR_CODIGO AND PercentualPendente.CMD_PERCENTUAL_EXECUCAO < 100 ");

        }

        private void SetarJoinMotivoSolicitacaoFrete(StringBuilder joins)
        {
            if (!joins.Contains(" MSF "))
                joins.Append(" LEFT JOIN T_MOTIVO_SOLICITACAO_FRETE MSF on Carga.MSF_CODIGO = MSF.MSF_CODIGO");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            if (!joins.Contains(" CargaPedido "))
                joins.Append("join T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO");
        }

        #endregion Métodos Privados

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtroPesquisa)
        {
            switch (propriedade)
            {
                case "CapacidadePesoVeiculo":
                    if (!select.Contains("CapacidadePesoVeiculo, "))
                    {
                        select.Append("SUM(ModeloVeiculo.MVC_CAPACIDADE_PESO_TRANSPORTE) CapacidadePesoVeiculo, ");

                        SetarJoinsModeloVeicularCarga(joins);
                    }
                    break;

                case "Carregamento":
                    if (!select.Contains(" Carregamento, "))
                    {
                        select.Append("MontagemCarga.CRG_NUMERO_CARREGAMENTO Carregamento, ");
                        groupBy.Append("MontagemCarga.CRG_NUMERO_CARREGAMENTO, ");

                        SetarJoinsMontagemCarga(joins);
                    }
                    break;

                case "CategoriaDestinatario":
                    if (!select.Contains("CategoriaDestinatario, "))
                    {
                        select.Append("substring((");
                        select.Append("    select distinct ', ' + categoriaPessoa.CTP_DESCRICAO ");
                        select.Append("      from T_CARGA_DADOS_SUMARIZADOS_DESTINATARIOS dadosSumarizadosDestinatarios ");
                        select.Append("      join T_CLIENTE cliente on cliente.CLI_CGCCPF = dadosSumarizadosDestinatarios.CLI_CGCCPF ");
                        select.Append("      join T_CATEGORIA_PESSOA categoriaPessoa on categoriaPessoa.CTP_CODIGO = cliente.CTP_CODIGO ");
                        select.Append("     where dadosSumarizadosDestinatarios.CDS_CODIGO = Carga.CDS_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) CategoriaDestinatario, ");

                        if (!groupBy.Contains("Carga.CDS_CODIGO"))
                            groupBy.Append("Carga.CDS_CODIGO, ");
                    }
                    break;

                case "CategoriaRemetente":
                    if (!select.Contains("CategoriaRemetente, "))
                    {
                        select.Append("substring((");
                        select.Append("    select distinct ', ' + categoriaPessoa.CTP_DESCRICAO ");
                        select.Append("      from T_CARGA_DADOS_SUMARIZADOS_REMETENTES dadosSumarizadosRemetentes ");
                        select.Append("      join T_CLIENTE cliente on cliente.CLI_CGCCPF = dadosSumarizadosRemetentes.CLI_CGCCPF ");
                        select.Append("      join T_CATEGORIA_PESSOA categoriaPessoa on categoriaPessoa.CTP_CODIGO = cliente.CTP_CODIGO ");
                        select.Append("     where dadosSumarizadosRemetentes.CDS_CODIGO = Carga.CDS_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) CategoriaRemetente, ");

                        if (!groupBy.Contains("Carga.CDS_CODIGO"))
                            groupBy.Append("Carga.CDS_CODIGO, ");
                    }
                    break;

                case "CNPJFilial":
                case "CNPJFormatado":
                    if (!select.Contains("CNPJFilial, "))
                    {
                        select.Append("Filial.FIL_CNPJ CNPJFilial, ");
                        groupBy.Append("Filial.FIL_CNPJ, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "CNPJTransportador":
                case "CNPJTransportadorFormatado":
                    if (!select.Contains("CNPJTransportador, "))
                    {
                        select.Append("Transportador.EMP_CNPJ CNPJTransportador, ");
                        groupBy.Append("Transportador.EMP_CNPJ, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "IDCarga":
                    if (!select.Contains("IDCarga, "))
                    {
                        select.Append("Carga.CAR_CODIGO IDCarga, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "Codigo":
                    if (!select.Contains("Codigo, "))
                    {
                        select.Append("Carga.CAR_CODIGO Codigo, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "CodigoIntegracaoDestinatarios":
                    if (!select.Contains("CodigoIntegracaoDestinatarios, "))
                    {
                        select.Append("DadosSumarizados.CDS_CODIGO_INTEGRACAO_DESTINATARIOS CodigoIntegracaoDestinatarios, ");
                        groupBy.Append("DadosSumarizados.CDS_CODIGO_INTEGRACAO_DESTINATARIOS, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "CodigoIntegracaoRemetentes":
                    if (!select.Contains("CodigoIntegracaoRemetentes, "))
                    {
                        select.Append("DadosSumarizados.CDS_CODIGO_INTEGRACAO_REMETENTES CodigoIntegracaoRemetentes, ");
                        groupBy.Append("DadosSumarizados.CDS_CODIGO_INTEGRACAO_REMETENTES, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "Companhia":
                    if (!select.Contains("Companhia, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + _pedido.PED_COMPANHIA ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargapedido.PED_CODIGO ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and isnull(_pedido.PED_COMPANHIA, '') <> '' ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) Companhia, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DataAnulacao":
                    if (!select.Contains("DataAnulacao, "))
                    {
                        select.Append("CONVERT(NVARCHAR(10), CargaAnulacao.CAC_DATA_CANCELAMENTO, 103) + ' ' + CONVERT(NVARCHAR(5), CargaAnulacao.CAC_DATA_CANCELAMENTO, 108) DataAnulacao, ");
                        groupBy.Append("CargaAnulacao.CAC_DATA_CANCELAMENTO, ");

                        SetarJoinsCargaAnulacao(joins);
                    }
                    break;

                case "DataCancelamento":
                    if (!select.Contains("DataCancelamento, "))
                    {
                        select.Append("CONVERT(NVARCHAR(10), CargaCancelamento.CAC_DATA_CANCELAMENTO, 103) + ' ' + CONVERT(NVARCHAR(5), CargaCancelamento.CAC_DATA_CANCELAMENTO, 108) DataCancelamento, ");
                        groupBy.Append("CargaCancelamento.CAC_DATA_CANCELAMENTO, ");

                        SetarJoinsCargaCancelamento(joins);
                    }
                    break;

                case "DataColeta":
                    if (!select.Contains("DataColeta,"))
                    {
                        select.Append("(select TOP 1 CONVERT(NVARCHAR(10), Pedido.PED_DATA_INICIAL_COLETA, 103) + ' ' + CONVERT(NVARCHAR(5), Pedido.PED_DATA_INICIAL_COLETA, 108) from T_PEDIDO Pedido inner join T_CARGA_PEDIDO CargaPedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO OR CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO_AGRUPAMENTO) DataColeta, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO_AGRUPAMENTO, "))
                            groupBy.Append("Carga.CAR_CODIGO_AGRUPAMENTO, ");
                    }
                    break;

                case "DataEmbarque":
                case "DataEmbarqueFormatada":
                case "DiaSemana":
                    if (!select.Contains("DataEmbarque,"))
                    {
                        select.Append("(case when isnull(JanelaCarregamento.CJC_EXCEDENTE, 1) = 0 then JanelaCarregamento.CJC_INICIO_CARREGAMENTO else null end) DataEmbarque, ");

                        groupBy.Append("JanelaCarregamento.CJC_EXCEDENTE, JanelaCarregamento.CJC_INICIO_CARREGAMENTO, ");

                        SetarJoinsJanelaCarregamento(joins);
                    }
                    break;

                case "DataDescarregamento":
                case "DataDescarregamentoFormatada":
                    if (!select.Contains("DataDescarregamento,"))
                    {
                        select.Append("(case when isnull(JanelaDescarregamento.CJD_EXCEDENTE, 1) = 0 then JanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO else Carga.CAR_DATA_DESCARREGAMENTO_CARGA end) DataDescarregamento, ");

                        groupBy.Append("JanelaDescarregamento.CJD_EXCEDENTE, JanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO, Carga.CAR_DATA_DESCARREGAMENTO_CARGA, ");

                        SetarJoinsJanelaDescarregamento(joins);
                    }
                    break;

                case "DeliveryTerm":
                    if (!select.Contains("DeliveryTerm, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + _pedido.PED_DELIVERY_TERM ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargapedido.PED_CODIGO ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and isnull(_pedido.PED_DELIVERY_TERM, '') <> '' ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) DeliveryTerm, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "SituacaoAverbacao":
                    if (!select.Contains("SituacaoAverbacao, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + CASE ");
                        select.Append("      WHEN A.AVE_STATUS = 0 THEN 'Pendente' ");
                        select.Append("      WHEN A.AVE_STATUS = 1 THEN 'Sucesso'  ");
                        select.Append("      WHEN A.AVE_STATUS = 2 THEN 'Cancelado' ");
                        select.Append("      WHEN A.AVE_STATUS = 3 THEN 'Enviado'  ");
                        select.Append("      WHEN A.AVE_STATUS = 4 THEN 'Ag. Emissão' ");
                        select.Append("      WHEN A.AVE_STATUS = 5 THEN 'Ag. Cancelamento' ");
                        select.Append("      WHEN A.AVE_STATUS = 9 THEN 'Rejeitado'  ");
                        select.Append("      ELSE 'Outros' END from T_CTE_AVERBACAO A ");
                        select.Append("      WHERE A.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) SituacaoAverbacao, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "NumeroAverbacao":
                    if (!select.Contains("NumeroAverbacao, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + A.AVE_AVERBACAO ");
                        select.Append("      from T_CTE_AVERBACAO A ");
                        select.Append("      WHERE A.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroAverbacao, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DataAverbacao":
                    if (!select.Contains("DataAverbacao, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + convert(varchar(10), A.AVE_DATA_RETORNO, 103) + ' ' + convert(varchar(5), A.AVE_DATA_RETORNO, 108) ");
                        select.Append("      from T_CTE_AVERBACAO A ");
                        select.Append("      WHERE A.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) DataAverbacao, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DataETA":
                    if (!select.Contains(" DataETA, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + convert(varchar(10), _pedido.PED_DATA_ETA, 103) + ' ' + convert(varchar(5), _pedido.PED_DATA_ETA, 108) ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargapedido.PED_CODIGO ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and _pedido.PED_DATA_ETA is not null ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) DataETA, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DataInclusaoBooking":
                    if (!select.Contains("DataInclusaoBooking, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + convert(varchar(10), _pedido.PED_DATA_INCLUSAO_BOOKING, 103) + ' ' + convert(varchar(5), _pedido.PED_DATA_INCLUSAO_BOOKING, 108) ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargapedido.PED_CODIGO ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and _pedido.PED_DATA_INCLUSAO_BOOKING is not null ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) DataInclusaoBooking, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DataInclusaoPCP":
                    if (!select.Contains("DataInclusaoPCP, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + convert(varchar(10), _pedido.PED_DATA_INCLUSAO_PCP, 103) + ' ' + convert(varchar(5), _pedido.PED_DATA_INCLUSAO_PCP, 108) ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargapedido.PED_CODIGO ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and _pedido.PED_DATA_INCLUSAO_PCP is not null ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) DataInclusaoPCP, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DataFimEmissaoDocumentos":
                    if (!select.Contains("DataFimEmissaoDocumentos, "))
                    {
                        select.Append("CONVERT(NVARCHAR(10), Carga.CAR_DATA_FINALIZACAO_EMISSAO, 103) + ' ' + CONVERT(NVARCHAR(5), Carga.CAR_DATA_FINALIZACAO_EMISSAO, 108) DataFimEmissaoDocumentos, ");
                        groupBy.Append("Carga.CAR_DATA_FINALIZACAO_EMISSAO, ");
                    }
                    break;

                case "DataInicioEmissaoDocumentos":
                    if (!select.Contains("DataInicioEmissaoDocumentos, "))
                    {
                        select.Append("CONVERT(NVARCHAR(10), Carga.CAR_DATA_INICIO_GERACAO_CTES, 103) + ' ' + CONVERT(NVARCHAR(5), Carga.CAR_DATA_INICIO_GERACAO_CTES, 108) DataInicioEmissaoDocumentos, ");
                        groupBy.Append("Carga.CAR_DATA_INICIO_GERACAO_CTES, ");
                    }
                    break;

                case "Protocolo":
                    if (!select.Contains("Protocolo, "))
                    {
                        select.Append("Carga.CAR_PROTOCOLO Protocolo, ");
                        groupBy.Append("Carga.CAR_PROTOCOLO, ");

                        SetarJoinsRota(joins);
                    }
                    break;

                case "DataRetiradaCtrn":
                    if (!select.Contains("DataRetiradaCtrn, "))
                    {
                        select.Append("case ");
                        select.Append("    when Carga.CAR_DATA_RETIRADA_CTRN is null then ");
                        select.Append("        substring(( ");
                        select.Append("            select distinct ', ' + convert(varchar(10), _cargaVeiculoContainer.CVC_DATA_RETIRADA_CTRN, 103) ");
                        select.Append("              from T_CARGA_VEICULO_CONTAINER _cargaVeiculoContainer ");
                        select.Append("             where _cargaVeiculoContainer.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("               and _cargaVeiculoContainer.CVC_DATA_RETIRADA_CTRN is not null ");
                        select.Append("               for xml path('') ");
                        select.Append("        ), 3, 1000) ");
                        select.Append("    else convert(varchar(10), Carga.CAR_DATA_RETIRADA_CTRN, 103) ");
                        select.Append("end DataRetiradaCtrn, ");

                        groupBy.Append("Carga.CAR_DATA_RETIRADA_CTRN, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DescontoOperador":
                    if (!select.Contains("TipoFreteEscolhido, "))
                    {
                        select.Append("Carga.CAR_TIPO_FRETE_ESCOLHIDO TipoFreteEscolhido, ");
                        groupBy.Append("Carga.CAR_TIPO_FRETE_ESCOLHIDO, ");
                    }

                    SetarSelect("ValorTabelaFrete", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("ValorFrete", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    break;

                case "DescricaoDataCarregamento":
                    if (!select.Contains("DataCarga, "))
                    {
                        select.Append("Carga.CAR_DATA_CRIACAO DataCarga, ");
                        groupBy.Append("Carga.CAR_DATA_CRIACAO, ");
                    }
                    break;

                case "DataCarregamentoFormatada":
                    if (!select.Contains("DataCarregamento, "))
                    {
                        select.Append("Carga.CAR_DATA_CARREGAMENTO DataCarregamento, ");
                        groupBy.Append("Carga.CAR_DATA_CARREGAMENTO, ");
                    }
                    break;


                case "DescricaoSituacao":
                    if (!select.Contains("Situacao, "))
                    {
                        select.Append("Carga.CAR_SITUACAO Situacao, ");
                        groupBy.Append("Carga.CAR_SITUACAO, ");
                    }
                    break;

                case "NumeroDtNatura":
                    if (!select.Contains("NumeroDtNatura, "))
                    {
                        select.Append("IntegracaoNatura.IDT_CODIGO as NumeroDtNatura, ");
                        groupBy.Append("IntegracaoNatura.IDT_CODIGO, ");

                        SetarJoinsNumeroDtNatura(joins);
                    }
                    break;

                case "Destinatario":
                    if (!select.Contains("Destinatario, "))
                    {
                        select.Append("DadosSumarizados.CDS_DESTINATARIOS Destinatario, ");
                        groupBy.Append("DadosSumarizados.CDS_DESTINATARIOS, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "Destino":
                case "UFDestino":
                    if (!select.Contains("Destino, "))
                    {
                        select.Append("DadosSumarizados.CDS_DESTINOS Destino, ");
                        groupBy.Append("DadosSumarizados.CDS_DESTINOS, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "Filial":
                    if (!select.Contains("Filial, "))
                    {
                        select.Append("Filial.FIL_DESCRICAO Filial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "FilialVenda":
                    if (!select.Contains("FilialVenda, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + CONVERT(VARCHAR, _pedido.FIL_CODIGO_VENDA) ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargapedido.PED_CODIGO ");
                        select.Append("      where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("      for xml path('') ");
                        select.Append("), 3, 1000) FilialVenda, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "GrupoDestinatario":
                    if (!select.Contains("GrupoDestinatario, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + grupoPessoa.GRP_DESCRICAO ");
                        select.Append("      from T_CARGA_DADOS_SUMARIZADOS_DESTINATARIOS dadosSumarizadosDestinatarios ");
                        select.Append("      join T_CLIENTE cliente on cliente.CLI_CGCCPF = dadosSumarizadosDestinatarios.CLI_CGCCPF ");
                        select.Append("      join T_GRUPO_PESSOAS grupoPessoa on grupoPessoa.GRP_CODIGO = cliente.GRP_CODIGO ");
                        select.Append("     where dadosSumarizadosDestinatarios.CDS_CODIGO = Carga.CDS_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) GrupoDestinatario, ");

                        if (!groupBy.Contains("Carga.CDS_CODIGO"))
                            groupBy.Append("Carga.CDS_CODIGO, ");
                    }
                    break;

                case "GrupoPessoas":
                    if (!select.Contains("GrupoPessoas, "))
                    {
                        select.Append("GrupoPessoas.GRP_DESCRICAO GrupoPessoas, ");
                        groupBy.Append("GrupoPessoas.GRP_DESCRICAO, ");

                        SetarJoinsGrupoPessoas(joins);
                    }
                    break;

                case "GrupoRemetente":
                    if (!select.Contains("GrupoRemetente, "))
                    {
                        select.Append("substring((");
                        select.Append("    select distinct ', ' + grupoPessoa.GRP_DESCRICAO ");
                        select.Append("      from T_CARGA_DADOS_SUMARIZADOS_REMETENTES dadosSumarizadosRemetentes ");
                        select.Append("      join T_CLIENTE cliente on cliente.CLI_CGCCPF = dadosSumarizadosRemetentes.CLI_CGCCPF ");
                        select.Append("      join T_GRUPO_PESSOAS grupoPessoa on grupoPessoa.GRP_CODIGO = cliente.GRP_CODIGO ");
                        select.Append("     where dadosSumarizadosRemetentes.CDS_CODIGO = Carga.CDS_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) GrupoRemetente, ");

                        if (!groupBy.Contains("Carga.CDS_CODIGO"))
                            groupBy.Append("Carga.CDS_CODIGO, ");
                    }
                    break;

                case "HoraFinal":
                    if (!select.Contains("HoraFinal,"))
                    {
                        select.Append("(select TOP 1 CONVERT(NVARCHAR(5), Pedido.PED_DATA_FINAL_VIAGEM_FATURADA, 108) from T_PEDIDO Pedido inner join T_CARGA_PEDIDO CargaPedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO or CargaPedido.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO) HoraFinal, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "HoraInicial":
                    if (!select.Contains("HoraInicial,"))
                    {
                        select.Append("(select TOP 1 CONVERT(NVARCHAR(5), Pedido.PED_DATA_INICIAL_VIAGEM_FATURADA, 108) from T_PEDIDO Pedido inner join T_CARGA_PEDIDO CargaPedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO or CargaPedido.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO) HoraInicial, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "IdAutorizacao":
                    if (!select.Contains("IdAutorizacao, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + _pedido.PED_ID_AUTORIZACAO ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargapedido.PED_CODIGO ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and isnull(_pedido.PED_ID_AUTORIZACAO, '') <> '' ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) IdAutorizacao, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "JustificativaAnulacao":
                    if (!select.Contains("JustificativaAnulacao, "))
                    {
                        select.Append("CargaAnulacao.CAC_MOTIVO_CANCELAMENTO JustificativaAnulacao, ");
                        groupBy.Append("CargaAnulacao.CAC_MOTIVO_CANCELAMENTO, ");

                        SetarJoinsCargaAnulacao(joins);
                    }
                    break;

                case "MotivoCancelamento":
                    if (!select.Contains("MotivoCancelamento, "))
                    {
                        select.Append("CargaCancelamento.CAC_MOTIVO_CANCELAMENTO MotivoCancelamento, ");
                        groupBy.Append("CargaCancelamento.CAC_MOTIVO_CANCELAMENTO, ");

                        SetarJoinsCargaCancelamento(joins);
                    }
                    break;

                case "JustificativaCancelamento":
                    if (!select.Contains("JustificativaCancelamento, "))
                    {
                        select.Append("JustificativaCancelamento.TCJ_DESCRICAO JustificativaCancelamento, ");
                        groupBy.Append("JustificativaCancelamento.TCJ_DESCRICAO, ");

                        SetarJoinCargaCancelamentoJustificativa(joins);
                    }
                    break;

                case "KMFinal":
                    if (!select.Contains("KMFinal,"))
                    {
                        select.Append("(SELECT TOP 1 Guarita.GUA_KM_ATUAL from T_GUARITA_TMS Guarita where Guarita.GUA_ENTRADA_SAIDA = 1 and Guarita.CAR_CODIGO = Carga.CAR_CODIGO) KMFinal, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "NumeroNfProdutor":
                    if (!select.Contains("NumeroNfProdutor,"))
                    {
                        select.Append("CargaJanelaCarregamentoGuarita.CJC_NUMERO_NF_PRODUTOR NumeroNfProdutor, ");
                        groupBy.Append("CargaJanelaCarregamentoGuarita.CJC_NUMERO_NF_PRODUTOR, ");

                        SetarJoinsCargaJanelaCarregamentoGuarita(joins);
                    }
                    break;

                case "PorcentagemPerda":
                    if (!select.Contains("PorcentagemPerda,"))
                    {
                        select.Append("CargaJanelaCarregamentoGuarita.CJC_PESAGEM_PORCENTAGEM_PERDA PorcentagemPerda, ");
                        groupBy.Append("CargaJanelaCarregamentoGuarita.CJC_PESAGEM_PORCENTAGEM_PERDA, ");

                        SetarJoinsCargaJanelaCarregamentoGuarita(joins);
                    }
                    break;

                case "PesagemQuantidadeCaixas":
                    if (!select.Contains("PesagemQuantidadeCaixas,"))
                    {
                        select.Append("CargaJanelaCarregamentoGuarita.CJC_PESAGEM_QUANTIDADE_CAIXAS PesagemQuantidadeCaixas, ");
                        groupBy.Append("CargaJanelaCarregamentoGuarita.CJC_PESAGEM_QUANTIDADE_CAIXAS, ");

                        SetarJoinsCargaJanelaCarregamentoGuarita(joins);
                    }
                    break;

                case "PesoLiquidoPosPerdas":
                    SetarSelect("PorcentagemPerda", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("PesagemInicial", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("PesagemFinal", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    break;

                case "ResultadoFinalProcessoCaixas":
                    SetarSelect("PorcentagemPerda", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("PesagemQuantidadeCaixas", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("PesagemInicial", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("PesagemFinal", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    break;

                case "PesoLiquidoPesagem":
                    SetarSelect("PesagemInicial", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("PesagemFinal", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    break;
                case "DiferencaFreteValorOperadorValorTabelaFrete":
                    SetarSelect("ValorFreteInformadoPeloOperador", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("ValorTabelaFrete", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    break;
                case "PesagemInicial":
                    if (!select.Contains(" PesagemInicial, "))
                    {
                        select.Append("CargaJanelaCarregamentoGuarita.CJC_PESAGEM_INICIAL PesagemInicial, ");
                        groupBy.Append("CargaJanelaCarregamentoGuarita.CJC_PESAGEM_INICIAL, ");

                        SetarJoinsCargaJanelaCarregamentoGuarita(joins);
                    }
                    break;

                case "PesagemFinal":
                    if (!select.Contains(" PesagemFinal, "))
                    {
                        select.Append("CargaJanelaCarregamentoGuarita.CJC_PESAGEM_FINAL PesagemFinal, ");
                        groupBy.Append("CargaJanelaCarregamentoGuarita.CJC_PESAGEM_FINAL, ");

                        SetarJoinsCargaJanelaCarregamentoGuarita(joins);
                    }
                    break;

                case "NumeroLacrePesagem":
                    if (!select.Contains(" NumeroLacrePesagem, "))
                    {
                        select.Append("CargaJanelaCarregamentoGuarita.CJC_NUMERO_LACRE NumeroLacrePesagem, ");
                        groupBy.Append("CargaJanelaCarregamentoGuarita.CJC_NUMERO_LACRE, ");

                        SetarJoinsCargaJanelaCarregamentoGuarita(joins);
                    }
                    break;

                case "LoteInternoPesagem":
                    if (!select.Contains(" LoteInternoPesagem, "))
                    {
                        select.Append("CargaJanelaCarregamentoGuarita.CJC_PESAGEM_LOTE_INTERNO LoteInternoPesagem, ");
                        groupBy.Append("CargaJanelaCarregamentoGuarita.CJC_PESAGEM_LOTE_INTERNO, ");

                        SetarJoinsCargaJanelaCarregamentoGuarita(joins);
                    }
                    break;

                case "KMInicial":
                    if (!select.Contains("KMInicial,"))
                    {
                        select.Append("(SELECT TOP 1 Guarita.GUA_KM_ATUAL from T_GUARITA_TMS Guarita where Guarita.GUA_ENTRADA_SAIDA = 2 and Guarita.CAR_CODIGO = Carga.CAR_CODIGO) KMInicial, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "KmRodado":
                    if (!select.Contains("KmRodado, "))
                    {
                        select.Append("SUM(DadosSumarizados.CDS_DISTANCIA) KmRodado, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "Mdfes":
                    if (!select.Contains("Mdfes, "))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("    SELECT DISTINCT ', ' + CAST(Mdfe.MDF_NUMERO AS NVARCHAR(4000)) ");
                        select.Append("      FROM T_CARGA_MDFE CargaMdfe ");
                        select.Append("      JOIN T_MDFE Mdfe ");
                        select.Append("        ON CargaMdfe.MDF_CODIGO = Mdfe.MDF_CODIGO ");
                        select.Append("     WHERE CargaMdfe.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       FOR XML PATH('') ");
                        select.Append("), 3, 4000) Mdfes, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "ModeloVeiculo":
                    if (!select.Contains("ModeloVeiculo, "))
                    {
                        select.Append("ModeloVeiculo.MVC_DESCRICAO ModeloVeiculo, ");
                        groupBy.Append("ModeloVeiculo.MVC_DESCRICAO, ");

                        SetarJoinsModeloVeicularCarga(joins);
                    }
                    break;

                case "Motoristas":
                    if (!select.Contains("Motoristas, "))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("    SELECT DISTINCT ', ' + CAST(( ");
                        select.Append("               SUBSTRING(Motorista.FUN_CPF, 1, 3) + '.' + ");
                        select.Append("               SUBSTRING(Motorista.FUN_CPF, 4, 3) + '.' + ");
                        select.Append("               SUBSTRING(Motorista.FUN_CPF, 7, 3) + '-' + ");
                        select.Append("               SUBSTRING(Motorista.FUN_CPF, 10, 3) + ' - ' + ");
                        select.Append("               Motorista.FUN_NOME ");
                        select.Append("           ) AS NVARCHAR(4000)) ");
                        select.Append("      FROM T_CARGA_MOTORISTA CargaMotorista ");
                        select.Append("      JOIN T_FUNCIONARIO Motorista ");
                        select.Append("        ON CargaMotorista.CAR_MOTORISTA = Motorista.FUN_CODIGO ");
                        select.Append("     WHERE CargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       FOR XML PATH('') ");
                        select.Append("), 3, 4000) Motoristas, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "NotasParciais":
                    if (!select.Contains("NotasParciais, "))
                    {
                        select.Append("substring((");
                        select.Append("    select ', ' + cast(_cargapedidoxmlnotafiscal.CFP_NUMERO as nvarchar(20)) ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_CARGA_PEDIDO_XML_NOTA_FISCAL_PARCIAL _cargapedidoxmlnotafiscal on _cargapedidoxmlnotafiscal.CPE_CODIGO = _cargapedido.CPE_CODIGO ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NotasParciais, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "NotasFiscais":
                    if (!select.Contains("NotasFiscais, "))
                    {
                        select.Append("substring((");
                        select.Append("    select ', ' + cast(nfx.NF_NUMERO as nvarchar(20)) ");
                        select.Append("      from T_CARGA car ");
                        select.Append("      left join T_CARGA_PEDIDO cpe on cpe.CAR_CODIGO = car.CAR_CODIGO ");
                        select.Append("      left join T_PEDIDO_XML_NOTA_FISCAL pex on pex.CPE_CODIGO = cpe.CPE_CODIGO ");
                        select.Append("      left join T_XML_NOTA_FISCAL nfx on nfx.NFX_CODIGO = pex.NFX_CODIGO ");
                        select.Append("     where car.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NotasFiscais, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "NumeroBooking":
                    if (!select.Contains("NumeroBooking, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + _pedido.PED_NUMERO_BOOKING ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargapedido.PED_CODIGO ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and isnull(_pedido.PED_NUMERO_BOOKING, '') <> '' ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroBooking, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains("NumeroCarga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");
                    }
                    break;
                case nameof(RelatorioCarga.Alocacao):
                    if (!select.Contains($"{nameof(RelatorioCarga.Alocacao)}, "))
                    {
                        select.Append($"Carga.CAR_ALOCACAO {nameof(RelatorioCarga.Alocacao)}, ");
                        groupBy.Append("Carga.CAR_ALOCACAO, ");
                    }
                    break;
                case nameof(RelatorioCarga.NumeroTransferencia):
                    if (!select.Contains($"{nameof(RelatorioCarga.NumeroTransferencia)}, "))
                    {
                        select.Append($"Carga.CAR_NUMERO_TRANSFERENCIA {nameof(RelatorioCarga.NumeroTransferencia)}, ");
                        groupBy.Append("Carga.CAR_NUMERO_TRANSFERENCIA, ");
                    }
                    break;
                case "NumeroColetas":
                    if (!select.Contains("NumeroColetas, "))
                    {
                        select.Append("SUM(DadosSumarizados.CDS_NUMERO_COLETAS) NumeroColetas, ");
                        //groupBy.Append("DadosSumarizados.CDS_NUMERO_COLETAS, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "NumeroEntregas":
                    if (!select.Contains("NumeroEntregas, "))
                    {
                        select.Append("SUM(DadosSumarizados.CDS_NUMERO_ENTREGAS) NumeroEntregas, ");
                        //groupBy.Append("DadosSumarizados.CDS_NUMERO_ENTREGAS, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "NumeroNavio":
                    if (!select.Contains(" NumeroNavio, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + _pedido.PED_NUMERO_NAVIO ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargapedido.PED_CODIGO ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and isnull(_pedido.PED_NUMERO_NAVIO, '') <> '' ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroNavio, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "NumeroPedido":
                    if (!select.Contains("NumeroPedido, "))
                    {
                        select.Append("substring((");
                        select.Append("    select ', ' + _pedido.PED_NUMERO_PEDIDO_EMBARCADOR ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_PEDIDO _pedido on _pedido.PED_CODIGO = _cargapedido.PED_CODIGO ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroPedido, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "NumeroPedidoInterno":
                    if (!select.Contains("NumeroPedidoInterno, "))
                    {
                        select.Append("substring((");
                        select.Append("    select ', ' + convert(nvarchar(20), _pedido.PED_NUMERO) ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_PEDIDO _pedido on _pedido.PED_CODIGO = _cargapedido.PED_CODIGO  ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroPedidoInterno, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "NumeroPedidoNotaFiscal":
                    if (!select.Contains("NumeroPedidoNotaFiscal, "))
                    {
                        select.Append("substring((");
                        select.Append("    select ', ' + cast(nfx.NF_NUMERO_PEDIDO_EMBARCADOR as nvarchar(20)) ");
                        select.Append("      from T_CARGA car ");
                        select.Append("      left join T_CARGA_PEDIDO cpe on cpe.CAR_CODIGO = car.CAR_CODIGO ");
                        select.Append("      left join T_PEDIDO_XML_NOTA_FISCAL pex on pex.CPE_CODIGO = cpe.CPE_CODIGO ");
                        select.Append("      left join T_XML_NOTA_FISCAL nfx on nfx.NFX_CODIGO = pex.NFX_CODIGO ");
                        select.Append("     where car.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroPedidoNotaFiscal, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "NumeroValePedagio":
                    if (!select.Contains("NumeroValePedagio,"))
                    {
                        select.Append(
                            @"COALESCE
                                    (
                                        substring((
                                            select distinct ', ' + _integracaoValePedagio.CVP_NUMERO_COMPROVANTE
                                              from  T_CARGA_VALE_PEDAGIO _integracaoValePedagio
                                             where _integracaoValePedagio.CAR_CODIGO = Carga.CAR_CODIGO
                                               for xml path('')
                                        ), 3, 1000),
                                        substring((
                                            select distinct ', ' + CargaIntegracaoValePedagio.CVP_NUMERO_VALE_PEDAGIO
                                              from  T_CARGA_INTEGRACAO_VALE_PEDAGIO CargaIntegracaoValePedagio
                                             where CargaIntegracaoValePedagio.CAR_CODIGO = Carga.CAR_CODIGO
                                               for xml path('')
                                        ), 3, 1000)
                                    ) NumeroValePedagio,"
                        );

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "OperadorAnulacao":
                    if (!select.Contains("OperadorAnulacao, "))
                    {
                        select.Append("OperadorAnulacao.FUN_NOME OperadorAnulacao, ");
                        groupBy.Append("OperadorAnulacao.FUN_NOME, ");

                        SetarJoinsOperadorAnulacao(joins);
                    }
                    break;

                case "OperadorCancelamento":
                    if (!select.Contains("OperadorCancelamento, "))
                    {
                        select.Append("OperadorCancelamento.FUN_NOME OperadorCancelamento, ");
                        groupBy.Append("OperadorCancelamento.FUN_NOME, ");

                        SetarJoinsOperadorCancelamento(joins);
                    }
                    break;

                case "OperadorCarga":
                    if (!select.Contains("OperadorCarga, "))
                    {
                        select.Append("OperadorCarga.FUN_NOME OperadorCarga, ");
                        groupBy.Append("OperadorCarga.FUN_NOME, ");

                        SetarJoinsOperadorCarga(joins);
                    }
                    break;

                case "Ordem":
                    if (!select.Contains("Ordem, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + _pedido.PED_ORDEM ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargapedido.PED_CODIGO ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and isnull(_pedido.PED_ORDEM, '') <> '' ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) Ordem, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;


                case "Genset":
                    if (!select.Contains("Genset, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + _cargaveiculocontainer.CVC_GENSET ");
                        select.Append("      from T_CARGA_VEICULO_CONTAINER _cargaveiculocontainer ");
                        select.Append("     where _cargaveiculocontainer.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and isnull(_cargaveiculocontainer.CVC_GENSET, '') <> '' ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) Genset, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "Origem":
                case "UFOrigem":
                    if (!select.Contains("Origem, "))
                    {
                        select.Append("DadosSumarizados.CDS_ORIGENS Origem, ");
                        groupBy.Append("DadosSumarizados.CDS_ORIGENS, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "PaisDestino":
                    if (!select.Contains("PaisDestino, "))
                    {
                        select.Append("DadosSumarizados.CDS_PAIS_DESTINOS PaisDestino, ");
                        groupBy.Append("DadosSumarizados.CDS_PAIS_DESTINOS, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "PaisOrigem":
                    if (!select.Contains("PaisOrigem, "))
                    {
                        select.Append("DadosSumarizados.CDS_PAIS_ORIGENS PaisOrigem, ");
                        groupBy.Append("DadosSumarizados.CDS_PAIS_ORIGENS, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "Pallets":
                    if (!select.Contains("Pallets, "))
                    {
                        select.Append("( ");
                        select.Append("    select sum(_pedido.PED_NUMERO_PALETES_FRACIONADO + _pedido.PED_NUMERO_PALETES) ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargapedido.PED_CODIGO ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append(") Pallets, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "ValorCustoFrete":
                    if (!select.Contains("ValorCustoFrete, "))
                    {
                        select.Append("( ");
                        select.Append("    select sum(_pedido.PED_VALOR_CUSTO_FRETE) ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargapedido.PED_CODIGO ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append(") ValorCustoFrete, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "PrevisaoEntregaTransportador":
                    if (!select.Contains("PrevisaoEntregaTransportador, "))
                    {
                        select.Append("( ");
                        select.Append("         SELECT CONVERT(VARCHAR(10), MAX(_pedido.PED_PREVISAO_ENTREGA_TRANSPORTADOR), 103) AS DATAMAX ");
                        select.Append("         FROM T_CARGA_PEDIDO _cargapedido");
                        select.Append("         join T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargapedido.PED_CODIGO");
                        select.Append("         where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO");
                        select.Append(") PrevisaoEntregaTransportador, ");



                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "PesoCarga":
                    if (!select.Contains("PesoCarga, "))
                    {
                        select.Append("( ");
                        select.Append("    select sum(_cargapedido.PED_PESO) ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append(") PesoCarga, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "PortoChegada":
                    if (!select.Contains("PortoChegada, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + _pedido.PED_PORTO_CHEGADA ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargapedido.PED_CODIGO ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and isnull(_pedido.PED_PORTO_CHEGADA, '') <> '' ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) PortoChegada, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "PortoSaida":
                    if (!select.Contains("PortoSaida, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + _pedido.PED_PORTO_SAIDA ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargapedido.PED_CODIGO ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and isnull(_pedido.PED_PORTO_SAIDA, '') <> '' ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) PortoSaida, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "PreCarga":
                    if (!select.Contains(" PreCarga "))
                    {
                        select.Append("PreCarga.PCA_NUMERO_CARGA PreCarga, ");
                        groupBy.Append("PreCarga.PCA_NUMERO_CARGA, ");

                        SetarJoinsPreCarga(joins);
                    }
                    break;

                case "QuantidadeHorasExcedentes":
                    if (!select.Contains("QuantidadeHorasExcedentes"))
                        select.Append("SUM(Carga.CAR_QUANTIDADE_HORAS_EXCEDENTES) QuantidadeHorasExcedentes, ");
                    break;

                case "QuantidadeHorasNormais":
                    if (!select.Contains("QuantidadeHorasNormais"))
                        select.Append("SUM(Carga.CAR_QUANTIDADE_HORAS) QuantidadeHorasNormais, ");
                    break;

                case "Remetente":
                    if (!select.Contains("Remetente, "))
                    {
                        select.Append("DadosSumarizados.CDS_REMETENTES Remetente, ");
                        groupBy.Append("DadosSumarizados.CDS_REMETENTES, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "Reserva":
                    if (!select.Contains("Reserva, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + _pedido.PED_RESERVA ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargapedido.PED_CODIGO ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and isnull(_pedido.PED_RESERVA, '') <> '' ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) Reserva, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "CentroResultado":
                    if (!select.Contains("CentroResultado, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + _centroResultado.CRE_DESCRICAO ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargapedido.PED_CODIGO ");
                        select.Append("      join T_CENTRO_RESULTADO _centroResultado ON _centroResultado.CRE_CODIGO = _pedido.CRE_CODIGO ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) CentroResultado, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "Resumo":
                    if (!select.Contains("Resumo, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + _pedido.PED_RESUMO ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargapedido.PED_CODIGO ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and isnull(_pedido.PED_RESUMO, '') <> '' ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) Resumo, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "Rota":
                    if (!select.Contains("Rota, "))
                    {
                        select.Append("Rota.ROF_DESCRICAO Rota, ");
                        groupBy.Append("Rota.ROF_DESCRICAO, ");

                        SetarJoinsRota(joins);
                    }
                    break;

                case "TabelaFrete":
                    if (!select.Contains("TabelaFrete,"))
                    {
                        select.Append("TabelaFrete.TBF_DESCRICAO TabelaFrete, ");
                        groupBy.Append("TabelaFrete.TBF_DESCRICAO, ");

                        SetarJoinsTabelaFrete(joins);
                    }
                    break;

                case "ObservacaoTransportador":
                    if (!select.Contains("ObservacaoTransportador,"))
                    {
                        select.Append("JanelaCarregamento.CJC_OBSERVACAO_TRANSPORTADOR as ObservacaoTransportadorJanela, ");
                        select.Append("Carga.CAR_OBSERVACAO_TRANSPORTADOR as ObservacaoTransportadorCarga, ");
                        groupBy.Append("JanelaCarregamento.CJC_OBSERVACAO_TRANSPORTADOR, ");
                        groupBy.Append("Carga.CAR_OBSERVACAO_TRANSPORTADOR, ");

                        SetarJoinsJanelaCarregamento(joins);
                    }
                    break;

                case "DataVigenciaTabelaFrete":
                    if (!select.Contains("DataVigenciaTabelaFrete,"))
                    {
                        select.Append(@"(SELECT TOP 1 
                                            convert(varchar, _tabelaFreteVigencia.TFV_DATA_INICIAL, 103) + ' ' +
                                            CASE
                                                WHEN _tabelaFreteVigencia.TFV_DATA_FINAL IS NOT NULL THEN 'até ' + convert(varchar, _tabelaFreteVigencia.TFV_DATA_FINAL, 103)
                                                ELSE ''
                                            END
                                            FROM T_TABELA_FRETE_VIGENCIA _tabelaFreteVigencia
                                            WHERE Carga.TBF_CODIGO = _tabelaFreteVigencia.TBF_CODIGO and
                                                  Carga.CAR_DATA_CRIACAO >= _tabelaFreteVigencia.TFV_DATA_INICIAL and (_tabelaFreteVigencia.TFV_DATA_FINAL is null or Carga.CAR_DATA_CRIACAO <= _tabelaFreteVigencia.TFV_DATA_FINAL)
                                            ORDER BY _tabelaFreteVigencia.TFV_DATA_FINAL DESC) DataVigenciaTabelaFrete, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                        if (!groupBy.Contains("Carga.TBF_CODIGO,"))
                            groupBy.Append("Carga.TBF_CODIGO, ");
                    }
                    break;

                case "PesoLiquidoCarga":
                    if (!select.Contains("PesoLiquidoCarga,"))
                    {
                        select.Append(@"DadosSumarizados.CDS_PESO_LIQUIDO_TOTAL PesoLiquidoCarga, ");

                        groupBy.Append("DadosSumarizados.CDS_PESO_LIQUIDO_TOTAL, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "TaxaIncidenciaFrete":
                    if (!select.Contains("TaxaIncidenciaFrete, "))
                    {
                        select.Append("(");
                        select.Append("    select ((100 * _carga.CAR_VALOR_FRETE_PAGAR) / (case isnull(sum(_xmlnotafiscal.NF_VALOR), 1) when 0 then 1 else sum(_xmlnotafiscal.NF_VALOR) end)) ");
                        select.Append("      from T_CARGA _carga ");
                        select.Append("      left join T_CARGA_PEDIDO _cargapedido on _cargapedido.CAR_CODIGO = _carga.CAR_CODIGO ");
                        select.Append("      left join T_PEDIDO_XML_NOTA_FISCAL _pedidoxmlnotafiscal on _pedidoxmlnotafiscal.CPE_CODIGO = _cargapedido.CPE_CODIGO ");
                        select.Append("      left join T_XML_NOTA_FISCAL _xmlnotafiscal on _xmlnotafiscal.NFX_CODIGO = _pedidoxmlnotafiscal.NFX_CODIGO ");
                        select.Append("     where _carga.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("     group by _carga.CAR_VALOR_FRETE_PAGAR, _carga.CAR_VALOR_ICMS ");
                        select.Append(") TaxaIncidenciaFrete, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "TaxaOcupacaoVeiculo":
                    if (!select.Contains("TaxaOcupacaoVeiculo, "))
                    {
                        select.Append("( ");
                        select.Append("    isnull(( ");
                        select.Append("        select sum(_notaFiscal.NF_PESO) * 100 ");
                        select.Append("          from ( ");
                        select.Append("                   select distinct _xmlnotafiscal.NF_NUMERO, _xmlnotafiscal.NF_PESO ");
                        select.Append("                     from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("                     join T_PEDIDO_XML_NOTA_FISCAL _pedidoxmlnotafiscal on _pedidoxmlnotafiscal.CPE_CODIGO = _cargapedido.CPE_CODIGO ");
                        select.Append("                     join T_XML_NOTA_FISCAL _xmlnotafiscal on _xmlnotafiscal.NFX_CODIGO = _pedidoxmlnotafiscal.NFX_CODIGO ");
                        select.Append("                    where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("               ) _notaFiscal ");
                        select.Append("    ), 0) / ");
                        select.Append("    isnull(( ");
                        select.Append("        select sum(_capacidadeModeloVeicularCarga.Capacidade) ");
                        select.Append("          from ( ");
                        select.Append("                   select _modeloveicularcarga.MVC_CODIGO, isnull(_modeloveicularcarga.MVC_CAPACIDADE_PESO_TRANSPORTE, 0.0) as Capacidade ");
                        select.Append("                     from T_VEICULO _veiculo ");
                        select.Append("                     join T_MODELO_VEICULAR_CARGA _modeloveicularcarga on _modeloveicularcarga.MVC_CODIGO = _veiculo.MVC_CODIGO ");
                        select.Append("                    where _veiculo.VEI_CODIGO = Carga.CAR_VEICULO ");
                        select.Append($"                     and _modeloveicularcarga.MVC_TIPO in ({(int)TipoModeloVeicularCarga.Geral}, {(int)TipoModeloVeicularCarga.Reboque}) ");
                        select.Append("                    union ");
                        select.Append("                   select distinct _modeloveicularcarga.MVC_CODIGO, isnull(_modeloveicularcarga.MVC_CAPACIDADE_PESO_TRANSPORTE, 0.0) as Capacidade ");
                        select.Append("                     from T_CARGA_VEICULOS_VINCULADOS _veiculoVinculado ");
                        select.Append("                     join T_VEICULO _veiculo on _veiculo.VEI_CODIGO = _veiculoVinculado.VEI_CODIGO ");
                        select.Append("                     join T_MODELO_VEICULAR_CARGA _modeloveicularcarga on _modeloveicularcarga.MVC_CODIGO = _veiculo.MVC_CODIGO ");
                        select.Append("                    where _veiculoVinculado.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append($"                     and _modeloveicularcarga.MVC_TIPO in ({(int)TipoModeloVeicularCarga.Geral}, {(int)TipoModeloVeicularCarga.Reboque}) ");
                        select.Append("               ) as _capacidadeModeloVeicularCarga ");
                        select.Append("    ), 1) ");
                        select.Append(") TaxaOcupacaoVeiculo, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        if (!groupBy.Contains("Carga.CAR_VEICULO,"))
                            groupBy.Append("Carga.CAR_VEICULO, ");
                    }
                    break;

                case "Temperatura":
                    if (!select.Contains(" Temperatura, "))
                    {
                        select.Append("FaixaTemperatura.FTE_DESCRICAO Temperatura, ");
                        groupBy.Append("FaixaTemperatura.FTE_DESCRICAO, ");

                        SetarJoinsFaixaTemperatura(joins);
                    }
                    break;

                case "TipoCarga":
                    if (!select.Contains("TipoCarga, "))
                    {
                        select.Append("TipoCarga.TCG_DESCRICAO TipoCarga, ");
                        groupBy.Append("TipoCarga.TCG_DESCRICAO, ");

                        SetarJoinsTipoCarga(joins);
                    }
                    break;

                case "TipoEmbarque":
                    if (!select.Contains("TipoEmbarque, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + _pedido.PED_TIPO_EMBARQUE ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargapedido.PED_CODIGO ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and isnull(_pedido.PED_TIPO_EMBARQUE, '') <> '' ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) TipoEmbarque, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "TipoFreteEscolhido":
                    if (!select.Contains("TipoFreteEscolhido, "))
                    {
                        select.Append("Carga.CAR_TIPO_FRETE_ESCOLHIDO TipoFreteEscolhido, ");
                        groupBy.Append("Carga.CAR_TIPO_FRETE_ESCOLHIDO, ");
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains("TipoOperacao, "))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO TipoOperacao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinsTipoOperacao(joins);
                    }
                    break;

                case "Tomador":
                    if (!select.Contains(" Tomador,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + ");
                        select.Append("           case CargaPedido.PED_TIPO_TOMADOR ");
                        select.Append("               when 0 then Remetente.CLI_NOME ");
                        select.Append("               when 1 then Expedidor.CLI_NOME ");
                        select.Append("               when 2 then Recebedor.CLI_NOME ");
                        select.Append("               when 3 then Destinatario.CLI_NOME ");
                        select.Append("               when 4 then Tomador.CLI_NOME ");
                        select.Append("               else '' ");
                        select.Append("           end ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO ");
                        select.Append("      left join T_CLIENTE Expedidor on Expedidor.CLI_CGCCPF = CargaPedido.CLI_CODIGO_EXPEDIDOR ");
                        select.Append("      left join T_CLIENTE Recebedor on Recebedor.CLI_CGCCPF = CargaPedido.CLI_CODIGO_RECEBEDOR ");
                        select.Append("      left join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = CargaPedido.CLI_CODIGO_TOMADOR ");
                        select.Append("      left join T_CLIENTE Remetente on Remetente.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE ");
                        select.Append("      left join T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = Pedido.CLI_CODIGO ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO or CargaPedido.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 500) Tomador, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "Transbordo":
                    if (!select.Contains("Transbordo"))
                    {
                        select.Append("(CASE Carga.CAR_CARGA_TRANSBORDO WHEN 1 THEN 'Sim' ELSE 'Não' END) Transbordo, ");
                        groupBy.Append("Carga.CAR_CARGA_TRANSBORDO, ");
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select.Append("ISNULL(NULLIF(DadosSumarizados.CDS_PORTAL_RETIRA_EMPRESA, ''), Transportador.EMP_RAZAO) Transportador, ");
                        groupBy.Append("Transportador.EMP_RAZAO, ");
                        if (!groupBy.Contains("DadosSumarizados.CDS_PORTAL_RETIRA_EMPRESA, "))
                            groupBy.Append("DadosSumarizados.CDS_PORTAL_RETIRA_EMPRESA, ");

                        SetarJoinsTransportador(joins);
                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "ValorFrete":
                    if (!select.Contains("ValorFrete, "))
                    {
                        if (filtroPesquisa?.VisualizarValorNFSeDescontandoISSRetido ?? false)
                            select.Append("SUM(Carga.CAR_VALOR_FRETE_PAGAR - Carga.CAR_VALOR_ISS) ValorFrete, ");
                        else
                            select.Append("SUM(Carga.CAR_VALOR_FRETE_PAGAR) ValorFrete, ");
                    }
                    break;

                case "ValorFreteInformadoPeloOperador":
                    if (!select.Contains("ValorFreteInformadoPeloOperador, "))
                        select.Append("SUM(Carga.CAR_VALOR_FRETE_OPERADOR) ValorFreteInformadoPeloOperador, ");
                    break;

                case "ValorFreteLiquido":
                    if (!select.Contains("ValorFreteLiquido, "))
                        select.Append("SUM(Carga.CAR_VALOR_FRETE) ValorFreteLiquido, ");
                    break;

                case "ValorFreteResidual":
                    if (!select.Contains("ValorFreteResidual, "))
                        select.Append("SUM(Carga.CAR_VALOR_FRETE_RESIDUAL) ValorFreteResidual, ");
                    break;

                case "ValorViagem":
                    if (!select.Contains("ValorViagem, "))
                        select.Append("SUM(Carga.CAR_VALOR_BASE_FRETE) ValorViagem, ");
                    break;

                case "ValorFreteSemImposto":
                    if (!select.Contains("ValorFrete, "))
                        select.Append("SUM(Carga.CAR_VALOR_FRETE_PAGAR) ValorFrete, ");

                    if (!select.Contains("ValorISS, "))
                        select.Append("SUM(Carga.CAR_VALOR_ISS) ValorISS, ");

                    if (!select.Contains("ValorICMS, "))
                        select.Append("SUM(Carga.CAR_VALOR_ICMS) ValorICMS, ");
                    break;

                case "ValorICMS":
                    if (!select.Contains("ValorICMS, "))
                        select.Append("SUM(Carga.CAR_VALOR_ICMS) ValorICMS, ");
                    break;

                case "ValorISS":
                    if (!select.Contains("ValorISS, "))
                        select.Append("SUM(Carga.CAR_VALOR_ISS) ValorISS, ");
                    break;

                case "CSTIBSCBS":
                    if (!select.Contains(" CSTIBSCBS, "))
                    {

                        select.Append("CargaPedido.CPE_CST_IBSCBS CSTIBSCBS, ");
                        groupBy.Append("CargaPedido.CPE_CST_IBSCBS, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "ClassificacaoTributariaIBSCBS":
                    if (!select.Contains(" ClassificacaoTributariaIBSCBS, "))
                    {
                        select.Append("CargaPedido.CPE_CLASSIFICACAO_TRIBUTARIA_IBSCBS ClassificacaoTributariaIBSCBS, ");
                        groupBy.Append("CargaPedido.CPE_CLASSIFICACAO_TRIBUTARIA_IBSCBS, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "BaseCalculoIBSCBS":
                    if (!select.Contains(" BaseCalculoIBSCBS, "))
                    {
                        select.Append("min(CargaPedido.CPE_BASE_CALCULO_IBSCBS) BaseCalculoIBSCBS, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "AliquotaIBSEstadual":
                    if (!select.Contains(" AliquotaIBSEstadual, "))
                    {
                        select.Append("min(CargaPedido.CPE_ALIQUOTA_IBS_ESTADUAL) AliquotaIBSEstadual, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "PercentualReducaoIBSEstadual":
                    if (!select.Contains(" PercentualReducaoIBSEstadual, "))
                    {
                        select.Append("min(CargaPedido.CPE_PERCENTUAL_REDUCAO_IBS_ESTADUAL) PercentualReducaoIBSEstadual, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "ValorIBSEstadual":
                    if (!select.Contains(" ValorIBSEstadual, "))
                    {
                        select.Append("min(CargaPedido.CPE_VALOR_IBS_ESTADUAL) ValorIBSEstadual, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "AliquotaIBSMunicipal":
                    if (!select.Contains(" AliquotaIBSMunicipal, "))
                    {
                        select.Append("min(CargaPedido.CPE_ALIQUOTA_IBS_MUNICIPAL) AliquotaIBSMunicipal, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "PercentualReducaoIBSMunicipal":
                    if (!select.Contains(" PercentualReducaoIBSMunicipal, "))
                    {
                        select.Append("min(CargaPedido.CPE_PERCENTUAL_REDUCAO_IBS_MUNICIPAL) PercentualReducaoIBSMunicipal, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "ValorIBSMunicipal":
                    if (!select.Contains(" ValorIBSMunicipal, "))
                    {
                        select.Append("min(CargaPedido.CPE_VALOR_IBS_MUNICIPAL) ValorIBSMunicipal, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "AliquotaCBS":
                    if (!select.Contains(" AliquotaCBS, "))
                    {
                        select.Append("min(CargaPedido.CPE_ALIQUOTA_CBS) AliquotaCBS, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "PercentualReducaoCBS":
                    if (!select.Contains(" PercentualReducaoCBS, "))
                    {
                        select.Append("min(CargaPedido.CPE_PERCENTUAL_REDUCAO_CBS) PercentualReducaoCBS, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "ValorCBS":
                    if (!select.Contains(" ValorCBS, "))
                    {
                        select.Append("min(CargaPedido.CPE_VALOR_CBS) ValorCBS, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "ValorKm":
                    SetarSelect("KmRodado", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);

                    if (!select.Contains("ValorFrete, "))
                        select.Append("SUM(Carga.CAR_VALOR_FRETE_PAGAR) ValorFrete, ");
                    break;

                case "ValorLiquidoKm":
                    SetarSelect("KmRodado", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);

                    if (!select.Contains("ValorFreteLiquido, "))
                        select.Append("SUM(Carga.CAR_VALOR_FRETE) ValorFreteLiquido, ");
                    break;

                case "ValorTabelaFrete":
                    if (!select.Contains("ValorTabelaFrete, "))
                        select.Append("SUM(Carga.CAR_VALOR_FRETE_TABELA_DE_FRETE) ValorTabelaFrete, ");
                    break;

                case "ValorTotalNotaFiscal":
                    if (!select.Contains("ValorTotalNotaFiscal, "))
                    {
                        select.Append("( ");
                        select.Append("    select sum(_xmlnotafiscal.NF_VALOR) ");
                        select.Append("      from T_CARGA _carga ");
                        select.Append("      left join T_CARGA_PEDIDO _cargapedido on _cargapedido.CAR_CODIGO = _carga.CAR_CODIGO ");
                        select.Append("      left join T_PEDIDO_XML_NOTA_FISCAL _pedidoxmlnotafiscal on _pedidoxmlnotafiscal.CPE_CODIGO = _cargapedido.CPE_CODIGO ");
                        select.Append("      left join T_XML_NOTA_FISCAL _xmlnotafiscal on _xmlnotafiscal.NFX_CODIGO = _pedidoxmlnotafiscal.NFX_CODIGO ");
                        select.Append("     where _carga.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append(") ValorTotalNotaFiscal, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "ValorValePedagio":
                    if (!select.Contains("ValorValePedagio,"))
                    {
                        select.Append(@"  COALESCE((SELECT SUM(CargaValePedagio.CVP_VALOR) FROM T_CARGA_VALE_PEDAGIO CargaValePedagio WHERE CargaValePedagio.CAR_CODIGO = Carga.CAR_CODIGO),
                                            (SELECT SUM(CargaIntegracaoValePedagio.CVP_VALOR_VALE_PEDAGIO) FROM T_CARGA_INTEGRACAO_VALE_PEDAGIO CargaIntegracaoValePedagio 
                                                WHERE CargaIntegracaoValePedagio.CAR_CODIGO = Carga.CAR_CODIGO)
                                           ) ValorValePedagio, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "Veiculos":
                    if (!select.Contains("Veiculos, "))
                    {
                        select.Append("( ");
                        select.Append("    (select _veiculo.VEI_PLACA from T_VEICULO _veiculo where _veiculo.VEI_CODIGO = Carga.CAR_VEICULO) + ");
                        select.Append("    isnull(( ");
                        select.Append("        select ', ' + _veiculo.VEI_PLACA ");
                        select.Append("          from T_CARGA_VEICULOS_VINCULADOS _veiculovinculadocarga ");
                        select.Append("          join T_VEICULO _veiculo on _veiculovinculadocarga.VEI_CODIGO = _veiculo.VEI_CODIGO ");
                        select.Append("         where _veiculovinculadocarga.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("           for xml path('') ");
                        select.Append("    ), '') ");
                        select.Append(") Veiculos, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        groupBy.Append("Carga.CAR_VEICULO, ");
                    }
                    break;

                case "GuaritaEntrada":
                    if (!select.Contains(" GuaritaEntrada, "))
                    {
                        select.Append("(");
                        select.Append("    SELECT TOP 1 CONVERT(VARCHAR(10), G.GUA_DATA_SAIDA_ENTRADA , 103) + ' '  + convert(VARCHAR(8), G.GUA_HORA_SAIDA_ENTRADA, 14)");
                        select.Append("    FROM T_GUARITA_TMS G ");
                        select.Append("    WHERE G.GUA_ENTRADA_SAIDA = 1 AND G.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("    ORDER BY G.GUA_DATA_SAIDA_ENTRADA DESC, G.GUA_HORA_SAIDA_ENTRADA DESC ");
                        select.Append(") GuaritaEntrada, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "GuaritaSaida":
                    if (!select.Contains(" GuaritaSaida, "))
                    {
                        select.Append("(");
                        select.Append("    SELECT TOP 1 CONVERT(VARCHAR(10), G.GUA_DATA_SAIDA_ENTRADA , 103) + ' '  + convert(VARCHAR(8), G.GUA_HORA_SAIDA_ENTRADA, 14)");
                        select.Append("    FROM T_GUARITA_TMS G ");
                        select.Append("    WHERE G.GUA_ENTRADA_SAIDA = 2 AND G.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("    ORDER BY G.GUA_DATA_SAIDA_ENTRADA DESC, G.GUA_HORA_SAIDA_ENTRADA DESC ");
                        select.Append(") GuaritaSaida, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
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

                case "Navio":
                    if (!select.Contains(" Navio, "))
                    {
                        select.Append("Viagem.PVN_DESCRICAO Navio, ");
                        groupBy.Append("Viagem.PVN_DESCRICAO, ");

                        SetarJoinsViagem(joins);
                    }
                    break;

                case "NumeroDeControle":
                    if (!select.Contains(" NumeroDeControle, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    SELECT DISTINCT '/ ' + cte.CON_NUMERO_CONTROLE ");
                        select.Append("      FROM T_CARGA_CTE cargaCTe ");
                        select.Append("      inner join T_CTE cte ON cte.CON_CODIGO = cargaCTe.CON_CODIGO ");
                        select.Append("     where cargaCTe.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroDeControle, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "ValorReceberCTe":
                    if (!select.Contains(" ValorReceberCTe, "))
                    {
                        select.Append("(SELECT SUM(cte.CON_VALOR_RECEBER) ");
                        select.Append("FROM T_CARGA_CTE cargaCTe ");
                        select.Append("INNER JOIN T_CTE cte ON cte.CON_CODIGO = cargaCTe.CON_CODIGO ");
                        select.Append("WHERE cargaCTe.CAR_CODIGO = Carga.CAR_CODIGO AND cargaCTe.CCC_CODIGO IS NULL) ");
                        select.Append("ValorReceberCTe, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "Containeres":
                    if (!select.Contains(" Containeres, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    SELECT DISTINCT ', ' + cote.CTR_NUMERO ");
                        select.Append("      FROM T_CARGA_PEDIDO cargaPedido ");
                        select.Append("      inner join T_PEDIDO ped ON ped.PED_CODIGO = cargaPedido.PED_CODIGO ");
                        select.Append("      inner join T_CONTAINER cote ON cote.CTR_CODIGO = ped.CTR_CODIGO ");
                        select.Append("     where cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO or cargaPedido.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) Containeres, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "NumeroOS":
                    if (!select.Contains(" NumeroOS, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    SELECT DISTINCT ', ' + ped.PED_NUMERO_OS ");
                        select.Append("      FROM T_CARGA_PEDIDO cargaPedido ");
                        select.Append("      inner join T_PEDIDO ped ON ped.PED_CODIGO = cargaPedido.PED_CODIGO ");
                        select.Append("     where cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO or cargaPedido.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroOS, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "NumeroProposta":
                    if (!select.Contains(" NumeroProposta, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    SELECT DISTINCT ', ' + ped.PED_CODIGO_PROPOSTA ");
                        select.Append("      FROM T_CARGA_PEDIDO cargaPedido ");
                        select.Append("      inner join T_PEDIDO ped ON ped.PED_CODIGO = cargaPedido.PED_CODIGO ");
                        select.Append("     where cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO or cargaPedido.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroProposta, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "TipoProposta":
                    if (!select.Contains(" TipoProposta, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    SELECT DISTINCT ', ' + ");
                        select.Append("           case ");
                        select.Append("               WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 1 THEN 'Carga Fechada' ");
                        select.Append("               WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 2 THEN 'Carga Fracionada' ");
                        select.Append("               WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 3 THEN 'Feeder' ");
                        select.Append("               WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 4 THEN 'VAS' ");
                        select.Append("               WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 5 THEN 'Embarque Certo - Feeder' ");
                        select.Append("               WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 6 THEN 'Embarque Certo - Cabotagem' ");
                        select.Append("               WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 7 THEN 'No Show - Cabotagem' ");
                        select.Append("               WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 8 THEN 'Faturamento - Contabilidade' ");
                        select.Append("               WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 9 THEN 'Demurrage - Cabotagem' ");
                        select.Append("               WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 10 THEN 'Detention - Cabotagem' ");
                        select.Append("               else '' ");
                        select.Append("           end ");
                        select.Append("      FROM T_CARGA_PEDIDO cargaPedido ");
                        select.Append("     where cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO or cargaPedido.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) TipoProposta, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "CargaIMO":
                    if (!select.Contains(" CargaIMO, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    SELECT DISTINCT ', ' + ");
                        select.Append("           case ");
                        select.Append("               WHEN ped.PED_POSSUI_CARGA_PERIGOSA = 1 THEN 'SIM' ");
                        select.Append("               ELSE 'NÃO' ");
                        select.Append("           end ");
                        select.Append("      FROM T_CARGA_PEDIDO cargaPedido ");
                        select.Append("      inner join T_PEDIDO ped ON ped.PED_CODIGO = cargaPedido.PED_CODIGO ");
                        select.Append("     where cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO or cargaPedido.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) CargaIMO, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "QuantidadeNF":
                    if (!select.Contains(" QuantidadeNF, "))
                    {
                        select.Append("( ");
                        select.Append("    SELECT COUNT(1) FROM T_PEDIDO_XML_NOTA_FISCAL PX ");
                        select.Append("      JOIN T_CARGA_PEDIDO cargaPedido ON cargaPedido.CPE_CODIGO = PX.CPE_CODIGO ");
                        select.Append("     where cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO or cargaPedido.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO ");
                        select.Append(") QuantidadeNF, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DataFaturaDocumento":
                    if (!select.Contains(" DataFaturaDocumento, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    SELECT DISTINCT ', ' + CONVERT(NVARCHAR(10), F.FAT_DATA_FATURA, 103) ");
                        select.Append("      FROM T_CARGA_CTE cargaCTe ");
                        select.Append("      inner join T_CTE cte ON cte.CON_CODIGO = cargaCTe.CON_CODIGO ");
                        select.Append("      inner join T_DOCUMENTO_FATURAMENTO DF ON DF.CON_CODIGO = cte.CON_CODIGO ");
                        select.Append("      inner join T_FATURA_DOCUMENTO FD on FD.DFA_CODIGO = DF.DFA_CODIGO ");
                        select.Append("      inner join T_FATURA F on F.FAT_CODIGO = FD.FAT_CODIGO ");
                        select.Append("     where cargaCTe.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) DataFaturaDocumento, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "NomeProprietarioVeiculo":
                    if (!select.Contains(" NomeProprietarioVeiculo, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct '/ ' + ProprietarioVeiculoCTe.PVE_NOME ");
                        select.Append("      from T_CARGA_CTE cargaCTe ");
                        select.Append("      join T_CTE_VEICULO VeiculoCTe on VeiculoCTe.CON_CODIGO = cargaCTe.CON_CODIGO ");
                        select.Append("      join T_CTE_VEICULO_PROPRIETARIO ProprietarioVeiculoCTe on ProprietarioVeiculoCTe.PVE_CODIGO = VeiculoCTe.PVE_CODIGO ");
                        select.Append("     where cargaCTe.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NomeProprietarioVeiculo, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "SerieCte":
                    if (!select.Contains(" SerieCte, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct '/ ' + convert(varchar(20), Serie.ESE_NUMERO) ");
                        select.Append("      from T_CARGA_CTE cargaCTe ");
                        select.Append("      join T_CTE CTe on CTe.CON_CODIGO = cargaCTe.CON_CODIGO ");
                        select.Append("      join T_EMPRESA_SERIE Serie on CTe.CON_SERIE = Serie.ESE_CODIGO ");
                        select.Append("     where cargaCTe.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) SerieCte, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "PortoOrigem":
                    if (!select.Contains(" PortoOrigem, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + _porto.POT_DESCRICAO ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargapedido.PED_CODIGO ");
                        select.Append("      join T_PORTO _porto ON _porto.POT_CODIGO = _pedido.POT_CODIGO_ORIGEM ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) PortoOrigem, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "PortoDestino":
                    if (!select.Contains(" PortoDestino, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + _porto.POT_DESCRICAO ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargapedido.PED_CODIGO ");
                        select.Append("      join T_PORTO _porto ON _porto.POT_CODIGO = _pedido.POT_CODIGO_DESTINO ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) PortoDestino, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "PortoTransbordo":
                    if (!select.Contains(" PortoTransbordo, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + _porto.POT_DESCRICAO ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_PEDIDO_TRANSBORDO _pedidoTransbordo ON _pedidoTransbordo.PED_CODIGO = _cargapedido.PED_CODIGO ");
                        select.Append("      join T_PORTO _porto ON _porto.POT_CODIGO = _pedidoTransbordo.POT_CODIGO ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) PortoTransbordo, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "NumeroCargaAgrupada":
                    if (!select.Contains(" NumeroCargaAgrupada "))
                    {
                        select.Append("CargaAgrupada.CAR_CODIGO_CARGA_EMBARCADOR NumeroCargaAgrupada, ");
                        groupBy.Append("CargaAgrupada.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCargaAgrupada(joins);
                    }
                    break;

                case "NumeroFatura":
                    if (!select.Contains(" NumeroFatura, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    SELECT DISTINCT ', ' + CAST((CASE WHEN F.FAT_NUMERO_FATURA_INTEGRACAO IS NULL OR F.FAT_NUMERO_FATURA_INTEGRACAO = 0 THEN F.FAT_NUMERO ELSE F.FAT_NUMERO_FATURA_INTEGRACAO END) AS NVARCHAR(20)) ");
                        select.Append("      FROM T_CARGA_CTE cargaCTe ");
                        select.Append("      inner join T_DOCUMENTO_FATURAMENTO DF ON DF.CON_CODIGO = cargaCTe.CON_CODIGO ");
                        select.Append("      inner join T_FATURA_DOCUMENTO FD on FD.DFA_CODIGO = DF.DFA_CODIGO ");
                        select.Append("      inner join T_FATURA F on F.FAT_CODIGO = FD.FAT_CODIGO ");
                        select.Append("     where cargaCTe.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroFatura, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "NumeroBoleto":
                    if (!select.Contains(" NumeroBoleto, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    SELECT DISTINCT ', ' + T.TIT_NOSSO_NUMERO ");
                        select.Append("      FROM T_CARGA_CTE cargaCTe ");
                        select.Append("      inner join T_DOCUMENTO_FATURAMENTO DF ON DF.CON_CODIGO = cargaCTe.CON_CODIGO ");
                        select.Append("      inner join T_FATURA_DOCUMENTO FD on FD.DFA_CODIGO = DF.DFA_CODIGO ");
                        select.Append("      inner join T_FATURA F on F.FAT_CODIGO = FD.FAT_CODIGO ");
                        select.Append("      inner join T_FATURA_PARCELA FP on FP.FAT_CODIGO = F.FAT_CODIGO ");
                        select.Append("      inner join T_TITULO T on T.FAP_CODIGO = FP.FAP_CODIGO ");
                        select.Append("     where cargaCTe.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroBoleto, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DataBoleto":
                    if (!select.Contains(" DataBoleto, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    SELECT DISTINCT ', ' + CONVERT(NVARCHAR(10), T.TIT_DATA_EMISSAO, 103) ");
                        select.Append("      FROM T_CARGA_CTE cargaCTe ");
                        select.Append("      inner join T_DOCUMENTO_FATURAMENTO DF ON DF.CON_CODIGO = cargaCTe.CON_CODIGO ");
                        select.Append("      inner join T_FATURA_DOCUMENTO FD on FD.DFA_CODIGO = DF.DFA_CODIGO ");
                        select.Append("      inner join T_FATURA F on F.FAT_CODIGO = FD.FAT_CODIGO ");
                        select.Append("      inner join T_FATURA_PARCELA FP on FP.FAT_CODIGO = F.FAT_CODIGO ");
                        select.Append("      inner join T_TITULO T on T.FAP_CODIGO = FP.FAP_CODIGO ");
                        select.Append("     where cargaCTe.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) DataBoleto, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "SituacaoFatura":
                    if (!select.Contains(" SituacaoFatura, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    SELECT DISTINCT ', ' + ");
                        select.Append("    CASE ");
                        select.Append("         WHEN F.FAT_SITUACAO = 1 THEN 'Em Andamento' ");
                        select.Append("         WHEN F.FAT_SITUACAO = 2 THEN 'Fechado' ");
                        select.Append("         WHEN F.FAT_SITUACAO = 3 THEN 'Cancelado' ");
                        select.Append("         WHEN F.FAT_SITUACAO = 4 THEN 'Liquidado' ");
                        select.Append("    ELSE '' END ");
                        select.Append("      FROM T_CARGA_CTE cargaCTe ");
                        select.Append("      inner join T_DOCUMENTO_FATURAMENTO DF ON DF.CON_CODIGO = cargaCTe.CON_CODIGO ");
                        select.Append("      inner join T_FATURA_DOCUMENTO FD on FD.DFA_CODIGO = DF.DFA_CODIGO ");
                        select.Append("      inner join T_FATURA F on F.FAT_CODIGO = FD.FAT_CODIGO ");
                        select.Append("     where cargaCTe.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) SituacaoFatura, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "SituacaoBoleto":
                    if (!select.Contains(" SituacaoBoleto, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    SELECT DISTINCT ', ' + ");
                        select.Append("    CASE ");
                        select.Append("         WHEN T.TIT_STATUS = 1 THEN 'Aberto' ");
                        select.Append("         WHEN T.TIT_STATUS = 3 THEN 'Quitado' ");
                        select.Append("         WHEN T.TIT_STATUS = 4 THEN 'Cancelado' ");
                        select.Append("    ELSE 'Em Negociação' END ");
                        select.Append("      FROM T_CARGA_CTE cargaCTe ");
                        select.Append("      inner join T_DOCUMENTO_FATURAMENTO DF ON DF.CON_CODIGO = cargaCTe.CON_CODIGO ");
                        select.Append("      inner join T_FATURA_DOCUMENTO FD on FD.DFA_CODIGO = DF.DFA_CODIGO ");
                        select.Append("      inner join T_FATURA F on F.FAT_CODIGO = FD.FAT_CODIGO ");
                        select.Append("      inner join T_FATURA_PARCELA FP on FP.FAT_CODIGO = F.FAT_CODIGO ");
                        select.Append("      inner join T_TITULO T on T.FAP_CODIGO = FP.FAP_CODIGO ");
                        select.Append("     where cargaCTe.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) SituacaoBoleto, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "Lacres":
                    if (!select.Contains(" Lacres, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    SELECT DISTINCT ");
                        select.Append("      CASE WHEN (ped.PED_LACRE_CONTAINER_UM IS NOT NULL and RTRIM(ped.PED_LACRE_CONTAINER_UM) <> '') THEN ', ' + ped.PED_LACRE_CONTAINER_UM ELSE '' END ");
                        select.Append("    + CASE WHEN (ped.PED_LACRE_CONTAINER_DOIS IS NOT NULL and RTRIM(ped.PED_LACRE_CONTAINER_DOIS) <> '') THEN ', ' + ped.PED_LACRE_CONTAINER_DOIS ELSE '' END ");
                        select.Append("    + CASE WHEN (ped.PED_LACRE_CONTAINER_TRES IS NOT NULL and RTRIM(ped.PED_LACRE_CONTAINER_TRES) <> '') THEN ', ' + ped.PED_LACRE_CONTAINER_TRES ELSE '' END ");
                        select.Append("      FROM T_CARGA_PEDIDO cargaPedido ");
                        select.Append("      inner join T_PEDIDO ped ON ped.PED_CODIGO = cargaPedido.PED_CODIGO ");
                        select.Append("     where cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO or cargaPedido.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) Lacres, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "Tara":
                    if (!select.Contains(" Tara, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    SELECT DISTINCT ', ' + ped.PED_TARA_CONTAINER ");
                        select.Append("      FROM T_CARGA_PEDIDO cargaPedido ");
                        select.Append("      inner join T_PEDIDO ped ON ped.PED_CODIGO = cargaPedido.PED_CODIGO ");
                        select.Append("     where cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO or cargaPedido.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) Tara, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "TipoContainers":
                    if (!select.Contains(" TipoContainers, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    SELECT DISTINCT ', ' + conteinerTipo.CTI_DESCRICAO ");
                        select.Append("      FROM T_CARGA_PEDIDO cargaPedido ");
                        select.Append("      inner join T_PEDIDO ped ON ped.PED_CODIGO = cargaPedido.PED_CODIGO ");
                        select.Append("      inner join T_CONTAINER conteiner ON conteiner.CTR_CODIGO = ped.CTR_CODIGO ");
                        select.Append("      inner join T_CONTAINER_TIPO conteinerTipo ON conteinerTipo.CTI_CODIGO = conteiner.CTI_CODIGO ");
                        select.Append("     where cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO or cargaPedido.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) TipoContainers, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "TipoSeparacao":
                    if (!select.Contains(" TipoSeparacao, "))
                    {
                        select.Append("TipoSeparacao.TSE_DESCRICAO TipoSeparacao, ");
                        groupBy.Append("TipoSeparacao.TSE_DESCRICAO, ");

                        SetarJoinsTipoSeparacao(joins);
                    }
                    break;

                case "DataInicioViagemFormatada":
                    if (!select.Contains(" DataInicioViagem, "))
                    {
                        select.Append("Carga.CAR_DATA_INICIO_VIAGEM DataInicioViagem, ");
                        groupBy.Append("Carga.CAR_DATA_INICIO_VIAGEM, ");
                    }
                    break;

                case "DataFimViagemFormatada":
                    if (!select.Contains(" DataFimViagem, "))
                    {
                        select.Append("Carga.CAR_DATA_FIM_VIAGEM DataFimViagem, ");
                        groupBy.Append("Carga.CAR_DATA_FIM_VIAGEM, ");
                    }
                    break;

                case "AliquotaPIS":
                    if (!select.Contains(" AliquotaPIS, "))
                    {
                        select.Append("SUM(ConfiguracaoTransportador.COF_ALIQUOTA_PIS) AliquotaPIS, ");

                        SetarJoinsConfiguracaoTransportador(joins);
                    }
                    break;

                case "AliquotaCOFINS":
                    if (!select.Contains(" AliquotaCOFINS, "))
                    {
                        select.Append("SUM(ConfiguracaoTransportador.COF_ALIQUOTA_COFINS) AliquotaCOFINS, ");

                        SetarJoinsConfiguracaoTransportador(joins);
                    }
                    break;

                case "ValorPIS":
                    SetarSelect("AliquotaPIS", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("ValorFreteLiquido", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("ValorICMS", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    break;

                case "ValorCOFINS":
                    SetarSelect("AliquotaCOFINS", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("ValorFreteLiquido", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("ValorICMS", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    break;

                case "ObservacaoInterna":
                    if (!select.Contains(" ObservacaoInterna, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    SELECT DISTINCT ', ' + ped.PED_OBSERVACAO_INTERNA ");
                        select.Append("      FROM T_CARGA_PEDIDO cargaPedido ");
                        select.Append("      inner join T_PEDIDO ped ON ped.PED_CODIGO = cargaPedido.PED_CODIGO ");
                        select.Append("     where cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO or cargaPedido.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) ObservacaoInterna, ");
                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;
                case "ObservacaoCarga":
                    if (!select.Contains(" ObservacaoCarga, "))
                    {
                        select.Append("MontagemCarga.CRG_OBSERVACAO ObservacaoCarga, ");
                        groupBy.Append("MontagemCarga.CRG_OBSERVACAO, ");
                        SetarJoinsMontagemCarga(joins);
                    }
                    break;
                case "ObservacaoCTe":
                    if (!select.Contains(" ObservacaoCTe, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    SELECT DISTINCT ', ' + ped.PED_OBSERVACAO_CTE ");
                        select.Append("      FROM T_CARGA_PEDIDO cargaPedido ");
                        select.Append("      inner join T_PEDIDO ped ON ped.PED_CODIGO = cargaPedido.PED_CODIGO ");
                        select.Append("     where cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO or cargaPedido.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) ObservacaoCTe, ");
                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;
                case "OperadorPedido":
                    if (!select.Contains(" OperadorPedido, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    SELECT DISTINCT ', ' + F.FUN_NOME ");
                        select.Append("      FROM T_PEDIDO P ");
                        select.Append("      join T_CARGA_PEDIDO CA ON CA.PED_CODIGO = P.PED_CODIGO ");
                        select.Append("      join T_FUNCIONARIO  F ON F.FUN_CODIGO = P.FUN_CODIGO ");
                        select.Append("     where CA.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) OperadorPedido, ");
                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "TipoServico":
                    if (!select.Contains(" TipoServico, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    SELECT DISTINCT ', ' + ");
                        select.Append("    CASE ");
                        select.Append("         WHEN CargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 1 THEN 'Normal' ");
                        select.Append("         WHEN CargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 2 THEN 'Subcontratação' ");
                        select.Append("         WHEN CargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 3 THEN 'Redespacho Intermediário' ");
                        select.Append("         WHEN CargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 4 THEN 'Vinculado Multimodal Terceiro' ");
                        select.Append("         WHEN CargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 5 THEN 'Vinculado Multimodal Próprio' ");
                        select.Append("         WHEN CargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 6 THEN 'Redespacho' ");
                        select.Append("      END ");
                        select.Append("      FROM T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO or CargaPedido.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) TipoServico, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;
                case "QuantidadeVolumesCarga":
                    if (!select.Contains(" QuantidadeVolumesCarga, "))
                    {
                        select.Append("DadosSumarizados.CDS_QUANTIDADE_VOLUMES QuantidadeVolumesCarga, ");

                        if (!groupBy.Contains("DadosSumarizados.CDS_QUANTIDADE_VOLUMES, "))
                            groupBy.Append("DadosSumarizados.CDS_QUANTIDADE_VOLUMES, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;
                case "PrevisaoEntregaPrimeiroPedido":
                    if (!select.Contains(" PrevisaoEntregaPrimeiroPedido, "))
                    {
                        select.Append(@"(SELECT CONVERT(VARCHAR(10), MIN(_pedido.PED_PREVISAO_ENTREGA), 103) + ' ' + CONVERT(VARCHAR(5), MIN(_pedido.PED_PREVISAO_ENTREGA), 108) from T_PEDIDO _pedido
                                        inner join T_CARGA_PEDIDO _cargaPedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO 
                                        inner join T_CARGA _carga ON _carga.CAR_CODIGO = _cargaPedido.CAR_CODIGO
                                        WHERE _carga.CAR_CODIGO = Carga.CAR_CODIGO) PrevisaoEntregaPrimeiroPedido, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;
                case "PrevisaoEntregaUltimoPedido":
                    if (!select.Contains(" PrevisaoEntregaUltimoPedido, "))
                    {
                        select.Append(@"(SELECT CONVERT(VARCHAR(10), MAX(_pedido.PED_PREVISAO_ENTREGA), 103) + ' ' + CONVERT(VARCHAR(5), MAX(_pedido.PED_PREVISAO_ENTREGA), 108) from T_PEDIDO _pedido
                                        inner join T_CARGA_PEDIDO _cargaPedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO 
                                        inner join T_CARGA _carga ON _carga.CAR_CODIGO = _cargaPedido.CAR_CODIGO
                                        WHERE _carga.CAR_CODIGO = Carga.CAR_CODIGO) PrevisaoEntregaUltimoPedido, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;
                case "HorasEmTransito":
                    if (!select.Contains(" HorasEmTransito, "))
                    {
                        select.Append(@"(SELECT DATEDIFF(hh, MIN(_pedido.PED_PREVISAO_ENTREGA), MAX(_pedido.PED_PREVISAO_ENTREGA)) from T_PEDIDO _pedido
                                        inner join T_CARGA_PEDIDO _cargaPedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO 
                                        inner join T_CARGA _carga ON _carga.CAR_CODIGO = _cargaPedido.CAR_CODIGO
                                        WHERE _carga.CAR_CODIGO = Carga.CAR_CODIGO) HorasEmTransito, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;
                case "DataRealizadaUltimaEntrega":
                    if (!select.Contains(" DataRealizadaUltimaEntrega, "))
                    {
                        select.Append(@"(SELECT CONVERT(VARCHAR(10), MAX(_cargaEntrega.CEN_DATA_FIM_ENTREGA), 103) + ' ' + CONVERT(VARCHAR(5), MAX(_cargaEntrega.CEN_DATA_FIM_ENTREGA), 108)
                                        FROM T_CARGA_ENTREGA _cargaEntrega 
                                        inner join T_CARGA _carga on _carga.CAR_CODIGO = _cargaEntrega.CAR_CODIGO
                                        where _carga.CAR_CODIGO = Carga.CAR_CODIGO) DataRealizadaUltimaEntrega, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;
                case "DataUltimaSaidaRaio":
                    if (!select.Contains(" DataUltimaSaidaRaio, "))
                    {
                        select.Append(@"(SELECT CONVERT(VARCHAR(10), MAX(_cargaEntrega.CEN_DATA_SAIDA_RAIO), 103) + ' ' + CONVERT(VARCHAR(5), MAX(_cargaEntrega.CEN_DATA_SAIDA_RAIO), 108)
                                        FROM T_CARGA_ENTREGA _cargaEntrega 
                                        inner join T_CARGA _carga on _carga.CAR_CODIGO = _cargaEntrega.CAR_CODIGO
                                        where _carga.CAR_CODIGO = Carga.CAR_CODIGO) DataUltimaSaidaRaio, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;
                case "TotalEntregasRealizadas":
                    if (!select.Contains(" TotalEntregasRealizadas, "))
                    {
                        select.Append(@"(SELECT COUNT(_cargaEntrega.CAR_CODIGO) FROM T_CARGA_ENTREGA _cargaEntrega
                                        WHERE _cargaEntrega.CEN_COLETA = 0
                                        AND _cargaEntrega.CEN_DATA_ENTREGA is not null
                                        AND _cargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO) TotalEntregasRealizadas, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;
                case "TotalEntregasDevolvidas":
                    if (!select.Contains(" TotalEntregasDevolvidas, "))
                    {
                        select.Append(@"(SELECT COUNT(_cargaEntrega.CAR_CODIGO) FROM T_CARGA_ENTREGA _cargaEntrega
                                        WHERE _cargaEntrega.CEN_COLETA = 0
                                        AND _cargaEntrega.CEN_DATA_REJEITADO is not null
                                        AND _cargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO) TotalEntregasDevolvidas, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;
                case "TotalEntregasPendentes":
                    if (!select.Contains(" TotalEntregasPendentes, "))
                    {
                        select.Append(@"(SELECT COUNT(_cargaEntrega.CAR_CODIGO) FROM T_CARGA_ENTREGA _cargaEntrega
                                        WHERE _cargaEntrega.CEN_COLETA = 0
                                        AND _cargaEntrega.CEN_DATA_ENTREGA is null
                                        AND _cargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO) TotalEntregasPendentes, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "TotalEntregasBaixaManual":
                    if (!select.Contains(" TotalEntregasBaixaManual, "))
                    {
                        select.Append(@"(SELECT COUNT(_cargaEntrega.CAR_CODIGO) FROM T_CARGA_ENTREGA _cargaEntrega
                                        WHERE _cargaEntrega.CEN_FINALIZADA_MANUALMENTE = 1
                                        AND _cargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO) TotalEntregasBaixaManual, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DistanciaRota":
                    if (!select.Contains(" DistanciaRota, "))
                    {
                        select.Append("DadosSumarizados.CDS_DISTANCIA DistanciaRota, ");

                        if (!groupBy.Contains("DadosSumarizados.CDS_DISTANCIA, "))
                            groupBy.Append("DadosSumarizados.CDS_DISTANCIA, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "DataInicioGeracaoCTesFormatada":
                    if (!select.Contains(" DataInicioGeracaoCTes, "))
                    {
                        select.Append("Carga.CAR_DATA_INICIO_GERACAO_CTES DataInicioGeracaoCTes, ");
                        groupBy.Append("Carga.CAR_DATA_INICIO_GERACAO_CTES, ");
                    }
                    break;

                case "DataFinalizacaoDocumentosFiscaisFormatada":
                    if (!select.Contains(" DataFinalizacaoProcessamentoDocumentosFiscais, "))
                    {
                        select.Append("Carga.CAR_DATA_FINALIZACAO_PROCESSAMENTO_DOCUMENTOS_FISCAIS DataFinalizacaoProcessamentoDocumentosFiscais, ");
                        groupBy.Append("Carga.CAR_DATA_FINALIZACAO_PROCESSAMENTO_DOCUMENTOS_FISCAIS, ");
                    }
                    break;

                case "DataPedido":
                    if (!select.Contains("DataPedido, "))
                    {
                        select.Append("substring((");
                        select.Append("    select ', ' + convert(nvarchar(MAX), FORMAT(_pedido.PED_DATA_CRIACAO,'dd/MM/yyyy HH:mm:ss'), 120) ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_PEDIDO _pedido on _pedido.PED_CODIGO = _cargapedido.PED_CODIGO ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) DataPedido, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;
                case "DataRoteirizacaoCarga":
                    if (!select.Contains("DataRoteirizacaoCarga, "))
                    {
                        select.Append("MontagemCarga.CRG_DATA_CRIACAO DataRoteirizacaoCarga, ");

                        if (!groupBy.Contains("MontagemCarga.CRG_DATA_CRIACAO,"))
                            groupBy.Append("MontagemCarga.CRG_DATA_CRIACAO, ");

                        SetarJoinsMontagemCarga(joins);
                    }
                    break;
                case "DataProgramacaoCarga":
                    if (!select.Contains("DataProgramacaoCarga, "))
                    {
                        select.Append("isnull(FORMAT(JanelaCarregamento.CJC_DATA_CARREGAMENTO_PROGRAMADA,'dd/MM/yyyy HH:mm:ss'), '') DataProgramacaoCarga, ");

                        groupBy.Append("JanelaCarregamento.CJC_DATA_CARREGAMENTO_PROGRAMADA, ");

                        SetarJoinsJanelaCarregamento(joins);
                    }
                    break;

                case "DataExpedicao":
                    if (!select.Contains(" DataExpedicao, "))
                    {
                        select.Append("isnull(FORMAT(FluxoGestaoPatio.FGP_DATA_SAIDA_GUARITA_PREVISTA,'dd/MM/yyyy HH:mm:ss'), (substring((select ', ' + convert(nvarchar(MAX), FORMAT(XNF.NF_DATA_EMISSAO,'dd/MM/yyyy HH:mm:ss'), 120)");
                        select.Append("                                 								                                     from T_CARGA_PEDIDO CAP inner join T_PEDIDO_XML_NOTA_FISCAL PNF on CAP.CPE_CODIGO = PNF.CPE_CODIGO ");
                        select.Append("                                                                                                           inner join T_XML_NOTA_FISCAL XNF on XNF.NFX_CODIGO = PNF.NFX_CODIGO ");
                        select.Append("                                                                                                     where CAP.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("                                                                                                       for xml path('') ), 3, 1000) ))  DataExpedicao, ");

                        groupBy.Append("FluxoGestaoPatio.FGP_DATA_SAIDA_GUARITA_PREVISTA, ");

                        SetarJoinsFluxoGestaoPatio(joins);
                    }
                    break;

                case "DataDocaInformadaFormatada":
                    if (!select.Contains(" DataDocaInformada, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_DOCA_INFORMADA DataDocaInformada, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_DOCA_INFORMADA, ");

                        SetarJoinsFluxoGestaoPatio(joins);
                    }
                    break;

                case "DataChegadaVeiculoFormatada":
                    if (!select.Contains(" DataChegadaVeiculo, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_CHEGADA_VEICULO DataChegadaVeiculo, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_CHEGADA_VEICULO, ");

                        SetarJoinsFluxoGestaoPatio(joins);
                    }
                    break;

                case "DataFaturamentoFormatada":
                    if (!select.Contains(" DataFaturamento, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_FATURAMENTO DataFaturamento, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_FATURAMENTO, ");

                        SetarJoinsFluxoGestaoPatio(joins);
                    }
                    break;

                case "NumeroPedidoCliente":
                    if (!select.Contains("NumeroPedidoCliente, "))
                    {
                        select.Append("DadosSumarizados.CDS_CODIGO_PEDIDO_CLIENTE NumeroPedidoCliente, ");
                        groupBy.Append("DadosSumarizados.CDS_CODIGO_PEDIDO_CLIENTE, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "QuantidadeTotalProduto":
                    if (!select.Contains(" QuantidadeTotalProduto "))
                    {
                        select.Append("( ");
                        select.Append("    select sum(_xmlProduto.XFP_QUANTIDADE) ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("      join T_PEDIDO_XML_NOTA_FISCAL _pedidoXmlNotaFiscal on _pedidoXmlNotaFiscal.CPE_CODIGO = _cargapedido.CPE_CODIGO ");
                        select.Append("      join T_XML_NOTA_FISCAL_PRODUTO _xmlProduto on _xmlProduto.NFX_CODIGO = _pedidoXmlNotaFiscal.NFX_CODIGO ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append(") QuantidadeTotalProduto, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "QtdVolumesCarga":
                    if (!select.Contains(" QtdVolumesCarga, "))
                    {
                        select.Append("DadosSumarizados.CDS_VOLUMES_TOTAL QtdVolumesCarga, ");

                        if (!groupBy.Contains("DadosSumarizados.CDS_VOLUMES_TOTAL, "))
                            groupBy.Append("DadosSumarizados.CDS_VOLUMES_TOTAL, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "VolumesCTe":
                    if (!select.Contains(" VolumesCTe, "))
                    {
                        select.Append(@" (
		                                    SELECT SUM(VolumesNF.Volumes) FROM
				                                      (
				                                       SELECT 
				                                         CASE WHEN _CTe.CON_VOLUMES > 0 
				                                         THEN CONVERT(int, _CTe.CON_VOLUMES) 
				                                         ELSE (
						                                       SELECT SUM(NotasFiscaisCTe.NFC_VOLUME) 
						                                         FROM T_CTE_DOCS NotasFiscaisCTe 
						                                        WHERE NotasFiscaisCTe.CON_CODIGO = _CTe.CON_CODIGO
						                                      ) 
				                                          END AS Volumes
				                                         FROM T_CTE _CTe
					                                     JOIN T_CARGA_CTE _CargaCTe
					                                       ON _CargaCTe.CON_CODIGO = _CTe.CON_CODIGO
					                                     WHERE _CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO
				                                      ) VolumesNF
	                                       ) VolumesCTe, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DataConfirmacaoDocumentoFormatada":
                    if (!select.Contains(" DataConfirmacaoDocumento, "))
                    {
                        select.Append("Carga.CAR_DATA_INICIO_CONFIRMACAO_DOCUMENTOS_FISCAIS DataConfirmacaoDocumento, ");
                        groupBy.Append("Carga.CAR_DATA_INICIO_CONFIRMACAO_DOCUMENTOS_FISCAIS, ");
                    }
                    break;

                case "Expedidor":
                    if (!select.Contains(" Expedidor, "))
                    {
                        select.Append("DadosSumarizados.CDS_EXPEDIDORES Expedidor, ");
                        groupBy.Append("DadosSumarizados.CDS_EXPEDIDORES, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "SerieNfe":
                    if (!select.Contains(" SerieNfe, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ' / ' + XmlNotaFiscal.NF_SERIE ");
                        select.Append("      FROM T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoXmlNotaFiscal ON PedidoXmlNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO ");
                        select.Append("      JOIN T_XML_NOTA_FISCAL XmlNotaFiscal ON XmlNotaFiscal.NFX_CODIGO = PedidoXmlNotaFiscal.NFX_CODIGO ");
                        select.Append("     WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       FOR XML PATH('') ");
                        select.Append("), 4, 1000) SerieNfe, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "Recebedor":
                    if (!select.Contains(" Recebedor, "))
                    {
                        select.Append("DadosSumarizados.CDS_RECEBEDORES Recebedor, ");
                        groupBy.Append("DadosSumarizados.CDS_RECEBEDORES, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "ValorToneladaSimulado":
                    if (!select.Contains(" ValorToneladaSimulado, "))
                    {
                        select.Append("(");
                        select.Append("  SELECT FORMAT(SUM(CASE WHEN ValorFrete.PED_VALOR_FRETE > 0 AND DadosSumarizados.CDS_PESO_TOTAL > 0 ");
                        select.Append("  THEN (ValorFrete.PED_VALOR_FRETE / DadosSumarizados.CDS_PESO_TOTAL) * 1000 ");
                        select.Append("   ELSE 0 ");
                        select.Append("    END ");
                        select.Append(" ), 'N4', 'pt-BR') ");
                        select.Append("  FROM T_CARGA_PEDIDO ValorFrete ");
                        select.Append("  JOIN T_CARGA_DADOS_SUMARIZADOS DadosSumarizados ON DadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO ");
                        select.Append("  WHERE ValorFrete.CAR_CODIGO IN (Carga.CAR_CODIGO, Carga.CAR_CODIGO_AGRUPAMENTO) ");
                        select.Append(") AS ValorToneladaSimulado, ");

                        if (!groupBy.Contains("Carga.CDS_CODIGO,"))
                            groupBy.Append("Carga.CDS_CODIGO, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO_AGRUPAMENTO,"))
                            groupBy.Append("Carga.CAR_CODIGO_AGRUPAMENTO, ");
                    }
                    break;

                case "ValorFreteSimulacao":
                    if (!select.Contains(" ValorFreteSimulacao, "))
                    {
                        select.Append("FORMAT(SimulacaoFreteCarregamento.SFC_VALOR_FRETE, 'N4', 'pt-BR') ValorFreteSimulacao, ");

                        if (!groupBy.Contains("SimulacaoFreteCarregamento.SFC_VALOR_FRETE,"))
                            groupBy.Append("SimulacaoFreteCarregamento.SFC_VALOR_FRETE, ");

                        SetarJoinsSimulacaoFreteCarregamento(joins);
                    }
                    break;

                case "LocRecebedor":
                    if (!select.Contains(" LocRecebedor, "))
                    {
                        select.Append("SUBSTRING((");
                        select.Append("    SELECT ' / ' + LocalidadeCliente.LOC_DESCRICAO + ' - ' + LocalidadeCliente.UF_SIGLA ");
                        select.Append("      FROM T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("      JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = Pedido.CLI_CODIGO_RECEBEDOR ");
                        select.Append("      JOIN T_LOCALIDADES LocalidadeCliente ON LocalidadeCliente.LOC_CODIGO = Cliente.LOC_CODIGO ");
                        select.Append("     WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       AND Pedido.CLI_CODIGO_RECEBEDOR IS NOT NULL ");
                        select.Append("       FOR XML PATH('') ");
                        select.Append("), 4, 1000) LocRecebedor, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "NumeroEXP":
                    if (!select.Contains(" NumeroEXP, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + Pedido.PED_NUMERO_EXP ");
                        select.Append("      from T_PEDIDO Pedido ");
                        select.Append("      join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO ");
                        select.Append("     where (CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO or CargaPedido.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO) ");
                        select.Append("       and isnull(Pedido.PED_NUMERO_EXP, '') <> '' ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroEXP, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "NumeroContainer":
                    if (!select.Contains(" NumeroContainer, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + Container.CTR_Numero ");
                        select.Append("      from T_CONTAINER Container ");
                        select.Append("      join T_COLETA_CONTAINER ColetaContainer on ColetaContainer.CTR_CODIGO = Container.CTR_CODIGO ");
                        select.Append("      join T_COLETA_CONTAINER_HISTORICO historico on ColetaContainer.CCR_CODIGO = historico.CCR_CODIGO ");
                        select.Append("     where (ColetaContainer.CAR_CODIGO_ATUAL = Carga.CAR_CODIGO OR ColetaContainer.CAR_CODIGO = Carga.CAR_CODIGO OR historico.CAR_CODIGO = Carga.CAR_CODIGO )");
                        select.Append("       and isnull(Container.CTR_Numero, '') <> '' ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroContainer, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DataEnvioUltimaNfe":
                    if (!select.Contains(" DataEnvioUltimaNfe, "))
                    {
                        select.Append("CONVERT(NVARCHAR(10), Carga.CAR_DATA_ENVIO_ULTIMA_NFE, 103) + ' ' + CONVERT(NVARCHAR(8), Carga.CAR_DATA_ENVIO_ULTIMA_NFE, 108) DataEnvioUltimaNfe, ");
                        groupBy.Append("Carga.CAR_DATA_ENVIO_ULTIMA_NFE, ");
                    }
                    break;

                case "Frotas":
                    if (!select.Contains(" Frotas, "))
                    {
                        select.Append("( ");
                        select.Append("    (select _veiculo.VEI_NUMERO_FROTA from T_VEICULO _veiculo where _veiculo.VEI_CODIGO = Carga.CAR_VEICULO) + ");
                        select.Append("    isnull(( ");
                        select.Append("        select ', ' + _veiculo.VEI_NUMERO_FROTA  ");
                        select.Append("        from T_CARGA_VEICULOS_VINCULADOS _veiculovinculadocarga  ");
                        select.Append("        join T_VEICULO _veiculo on _veiculovinculadocarga.VEI_CODIGO = _veiculo.VEI_CODIGO ");
                        select.Append("        where _veiculovinculadocarga.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("        for xml path('') ");
                        select.Append("    ), '') ");
                        select.Append(") Frotas, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        if (!groupBy.Contains("Carga.CAR_VEICULO,"))
                            groupBy.Append("Carga.CAR_VEICULO, ");
                    }
                    break;

                case "DataAceiteTransportador":
                case "DataAceiteTransportadorFormatada":
                    if (!select.Contains("DataAceiteTransportador, "))
                    {
                        select.Append(@" (
                                            SELECT TOP 1 _transportadorTermoAceite.TTA_DATA FROM T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR_TERMO_ACEITE _transportadorTermoAceite
                                              JOIN T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR _janelaCarregamentoTransportador
                                                ON _transportadorTermoAceite.JCT_CODIGO = _janelaCarregamentoTransportador.JCT_CODIGO
                                              JOIN T_CARGA_JANELA_CARREGAMENTO _janelaCarregamento
                                                ON _janelaCarregamento.CJC_CODIGO = _janelaCarregamentoTransportador.CJC_CODIGO
                                              WHERE Carga.CAR_CODIGO = _janelaCarregamento.CAR_CODIGO
                                                AND _janelaCarregamentoTransportador.JCT_SITUACAO = 3
                                                AND _janelaCarregamentoTransportador.EMP_CODIGO = Carga.EMP_CODIGO
                                         ) DataAceiteTransportador, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        if (!groupBy.Contains("Carga.EMP_CODIGO,"))
                            groupBy.Append("Carga.EMP_CODIGO, ");
                    }
                    break;

                case "UsuarioAceiteTransportador":
                    if (!select.Contains("UsuarioAceiteTransportador, "))
                    {
                        select.Append(@" (
                                            SELECT TOP 1 _usuarioAceite.FUN_NOME FROM T_FUNCIONARIO _usuarioAceite
                                              JOIN T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR_TERMO_ACEITE _transportadorTermoAceite
                                                ON _transportadorTermoAceite.FUN_CODIGO = _usuarioAceite.FUN_CODIGO
                                              JOIN T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR _janelaCarregamentoTransportador
                                                ON _transportadorTermoAceite.JCT_CODIGO = _janelaCarregamentoTransportador.JCT_CODIGO
                                              JOIN T_CARGA_JANELA_CARREGAMENTO _janelaCarregamento
                                                ON _janelaCarregamento.CJC_CODIGO = _janelaCarregamentoTransportador.CJC_CODIGO
                                              WHERE Carga.CAR_CODIGO = _janelaCarregamento.CAR_CODIGO
                                                AND _janelaCarregamentoTransportador.JCT_SITUACAO = 3
                                                AND _janelaCarregamentoTransportador.EMP_CODIGO = Carga.EMP_CODIGO
                                         ) UsuarioAceiteTransportador, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        if (!groupBy.Contains("Carga.EMP_CODIGO,"))
                            groupBy.Append("Carga.EMP_CODIGO, ");
                    }
                    break;

                case "NaoComparecimentoDescricao":
                    if (!select.Contains(" NaoComparecimento, "))
                    {
                        select.Append("JanelaCarregamento.CJC_NAO_COMPARECIDO NaoComparecimento, ");
                        groupBy.Append("JanelaCarregamento.CJC_NAO_COMPARECIDO, ");

                        SetarJoinsJanelaCarregamento(joins);
                    }
                    break;

                case "TipoPropriedade":
                    if (!select.Contains(" TipoPropriedade, "))
                    {
                        select.Append("( ");
                        select.Append("    (select CASE WHEN VEI_TIPO = 'P' THEN 'Proprio' WHEN VEI_TIPO = 'T' THEN 'Terceiros' ELSE '' END from T_VEICULO _veiculo where _veiculo.VEI_CODIGO = Carga.CAR_VEICULO) + ");
                        select.Append("    isnull(( ");
                        select.Append("        select ', ' + ");
                        select.Append("             CASE ");
                        select.Append("                 WHEN VEI_TIPO = 'P' THEN 'Proprio' ");
                        select.Append("                 WHEN VEI_TIPO = 'T' THEN 'Terceiros' ");
                        select.Append("                 ELSE '' ");
                        select.Append("             END ");
                        select.Append("          from T_CARGA_VEICULOS_VINCULADOS _veiculovinculadocarga ");
                        select.Append("          join T_VEICULO _veiculo on _veiculovinculadocarga.VEI_CODIGO = _veiculo.VEI_CODIGO ");
                        select.Append("         where _veiculovinculadocarga.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("           for xml path('') ");
                        select.Append("    ), '') ");
                        select.Append(") TipoPropriedade, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        groupBy.Append("Carga.CAR_VEICULO, ");
                    }
                    break;

                case "ModeloCarroceria":
                    if (!select.Contains(" ModeloCarroceria, "))
                    {
                        select.Append("(( ");
                        select.Append("     SELECT ModeloCarroceria.MCA_DESCRICAO ");
                        select.Append("     FROM T_VEICULO Veiculo ");
                        select.Append("         LEFT JOIN T_MODELO_CARROCERIA ModeloCarroceria ON ModeloCarroceria.MCA_CODIGO = Veiculo.MCA_CODIGO ");
                        select.Append("     WHERE Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ");
                        select.Append(") + ( ");
                        select.Append("    ISNULL(( ");
                        select.Append("        SELECT ', ' + ");
                        select.Append("            ModeloCarroceria.MCA_DESCRICAO ");
                        select.Append("        FROM T_CARGA_VEICULOS_VINCULADOS VeiculosVinculados ");
                        select.Append("            LEFT JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = VeiculosVinculados.VEI_CODIGO ");
                        select.Append("            LEFT JOIN T_MODELO_CARROCERIA ModeloCarroceria ON ModeloCarroceria.MCA_CODIGO = Veiculo.MCA_CODIGO ");
                        select.Append("        WHERE VeiculosVinculados.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("        FOR XML PATH('')), ");
                        select.Append("    '') ");
                        select.Append(")) ModeloCarroceria, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        if (!groupBy.Contains("Carga.CAR_VEICULO"))
                            groupBy.Append("Carga.CAR_VEICULO, ");
                    }
                    break;

                case "NumeroContainerVeiculo":
                    if (!select.Contains(" NumeroContainerVeiculo, "))
                    {
                        select.Append("SUBSTRING( ");
                        select.Append("   ISNULL(( ");
                        select.Append("       SELECT ', ' + VeiContainer.CVC_NUMERO_CONTAINER ");
                        select.Append("       FROM T_CARGA_VEICULOS_VINCULADOS Reboques");
                        select.Append("           LEFT JOIN T_CARGA_VEICULO_CONTAINER VeiContainer ");
                        select.Append("               ON VeiContainer.VEI_CODIGO = Reboques.VEI_CODIGO AND VeiContainer.CAR_CODIGO = Reboques.CAR_CODIGO ");
                        select.Append("       WHERE Reboques.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       FOR XML PATH('')), ");
                        select.Append("   ''), ");
                        select.Append("3, 1000) NumeroContainerVeiculo, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "PercentualBonificsacaoTransportador":
                    if (!select.Contains("PercentualBonificsacaoTransportador, "))
                        select.Append("SUM(Carga.CAR_PERCENTUAL_BONIFICACAO_TRANSPORTADOR) PercentualBonificsacaoTransportador, ");
                    break;

                case "ValorFreteTabelaFrete":
                    if (!select.Contains(" ValorFreteTabelaFrete, "))
                    {
                        select.Append("Carga.CAR_VALOR_FRETE_TABELA_DE_FRETE ValorFreteTabelaFrete, ");
                        groupBy.Append("Carga.CAR_VALOR_FRETE_TABELA_DE_FRETE, ");
                    }
                    break;

                case "LocalRetiradaContainer":
                    if (!select.Contains(" LocalRetiradaContainer, "))
                    {
                        select.Append("ClienteRetiradaContainer.CLI_NOME LocalRetiradaContainer, ");
                        groupBy.Append("ClienteRetiradaContainer.CLI_NOME, ");

                        SetarJoinsRetiradaContainer(joins);

                        SetarJoinsClienteRetiradaContainer(joins);
                    }
                    break;

                case "CargaPreCarga":
                case "CargaDePreCarga":
                    if (!select.Contains(" CargaDePreCarga, "))
                    {
                        select.Append("Carga.CAR_CARGA_DE_PRE_CARGA CargaDePreCarga, ");
                        groupBy.Append("Carga.CAR_CARGA_DE_PRE_CARGA, ");
                    }
                    break;

                case "Terceiro":
                case "FreteDeTerceiro":
                    if (!select.Contains(" FreteDeTerceiro, "))
                    {
                        select.Append("Carga.CAR_FRETE_TERCEIRO FreteDeTerceiro, ");
                        groupBy.Append("Carga.CAR_FRETE_TERCEIRO, ");
                    }
                    break;

                case "CodigoRedespacho":
                    if (!select.Contains(" CodigoRedespacho, "))
                    {
                        select.Append("Carga.RED_CODIGO CodigoRedespacho, ");
                        groupBy.Append("Carga.RED_CODIGO, ");
                    }
                    break;

                case "TipoContratacaoCarga":
                    if (!select.Contains(" TipoContratacaoCarga, "))
                    {
                        select.Append("Carga.CAR_CONTRATACAO_CARGA TipoContratacaoCarga, ");
                        groupBy.Append("Carga.CAR_CONTRATACAO_CARGA, ");
                    }
                    break;

                case "Redespacho":
                    SetarSelect("CodigoRedespacho", 0, select, joins, groupBy, false, filtroPesquisa);
                    SetarSelect("TipoContratacaoCarga", 0, select, joins, groupBy, false, filtroPesquisa);
                    break;

                case "HorarioEncaixadoDescricao":
                    if (!select.Contains(" HorarioEncaixado, "))
                    {
                        select.Append("JanelaCarregamento.CJC_HORARIO_ENCAIXADO HorarioEncaixado, ");
                        groupBy.Append("JanelaCarregamento.CJC_HORARIO_ENCAIXADO, ");

                        SetarJoinsJanelaCarregamento(joins);
                    }
                    break;

                case "KmRota":
                    if (!select.Contains("KmRota, "))
                    {
                        select.Append("Rota.ROF_QUILOMETROS KmRota, ");
                        groupBy.Append("Rota.ROF_QUILOMETROS, ");

                        SetarJoinsRota(joins);
                    }
                    break;

                case "ClienteFinal":
                    if (!select.Contains(" ClienteFinal, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + ClienteFinal.CLI_NOME ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("      join T_CLIENTE ClienteFinal ON ClienteFinal.CLI_CGCCPF = Pedido.CLI_CODIGO_CLIENTE_ADICIONAL ");
                        select.Append("     where (CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO or CargaPedido.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO) ");
                        select.Append("       and Pedido.CLI_CODIGO_CLIENTE_ADICIONAL IS NOT NULL ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) ClienteFinal, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "TaraContainer":
                    if (!select.Contains(" TaraContainer, "))
                    {

                        select.Append("CargaVeiculoContainer.CVC_TARA_CONTAINER TaraContainer, ");

                        if (!groupBy.Contains("CargaVeiculoContainer.CVC_TARA_CONTAINER, "))
                            groupBy.Append("CargaVeiculoContainer.CVC_TARA_CONTAINER, ");

                        SetarJoinCargaVeiculoContainer(joins);
                    }
                    break;

                case "MaxGross":
                    if (!select.Contains(" MaxGross, "))
                    {

                        select.Append("CargaVeiculoContainer.CVC_MAX_GROSS MaxGross, ");

                        if (!groupBy.Contains("CargaVeiculoContainer.CVC_MAX_GROSS, "))
                            groupBy.Append("CargaVeiculoContainer.CVC_MAX_GROSS, ");

                        SetarJoinCargaVeiculoContainer(joins);
                    }
                    break;

                case "NumeroDocumentoOriginario":
                    if (!select.Contains(" NumeroDocumentoOriginario, "))
                    {
                        select.Append(@"SUBSTRING((
                                SELECT DISTINCT ', ' + CONVERT(NVARCHAR(50), documentoOriginario.CDO_NUMERO) + '-' + documentoOriginario.CDO_SERIE
                                        FROM T_CARGA_CTE cargaCTe
                                        inner join T_CTE cte ON cte.CON_CODIGO = cargaCTe.CON_CODIGO
                                        inner join T_CTE_DOCUMENTO_ORIGINARIO documentoOriginario ON documentoOriginario.CON_CODIGO = CTe.CON_CODIGO
                                WHERE cargaCTe.CAR_CODIGO = Carga.CAR_CODIGO for xml path('')), 3, 1000) NumeroDocumentoOriginario, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DataNumeroDocumentoOriginario":
                    if (!select.Contains(" DataNumeroDocumentoOriginario, "))
                    {
                        select.Append(@"SUBSTRING((
                                SELECT DISTINCT ', ' + CONVERT(NVARCHAR(50), FORMAT(documentoOriginario.CDO_DATA_EMISSAO, 'dd/MM/yyyy HH:mm:ss'))
                                        FROM T_CARGA_CTE cargaCTe
                                        inner join T_CTE cte ON cte.CON_CODIGO = cargaCTe.CON_CODIGO
                                        inner join T_CTE_DOCUMENTO_ORIGINARIO documentoOriginario ON documentoOriginario.CON_CODIGO = CTe.CON_CODIGO
                                WHERE cargaCTe.CAR_CODIGO = Carga.CAR_CODIGO for xml path('')), 3, 1000) DataNumeroDocumentoOriginario, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                default:
                    if (!somenteContarNumeroRegistros && propriedade.Contains("ValorComponente"))
                        select.Append($"(SELECT SUM(CCF_VALOR_COMPONENTE) FROM T_CARGA_COMPONENTES_FRETE Componente WHERE Componente.CAR_CODIGO = Carga.CAR_CODIGO AND Componente.CFR_CODIGO = {codigoDinamico}) {propriedade}, "); 

                    if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                        groupBy.Append("Carga.CAR_CODIGO, ");

                    break;

                case "NaturezaOP":
                    if (!select.Contains(" NaturezaOP, "))
                    {
                        select.Append(
                                @" (SELECT TOP 1 XMLNotaFiscal.NF_NATUREZA_OP
                                      FROM VIEW_PEDIDO_XML PedidoXMl
                                      LEFT JOIN T_XML_NOTA_FISCAL XMLNotaFiscal on PedidoXML.NFX_CODIGO = XMLNotaFiscal.NFX_CODIGO
                                      LEFT JOIN T_CARGA_PEDIDO CargaPedido on PedidoXML.PED_CODIGO = CargaPedido.PED_CODIGO
		                              WHERE Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO
                                ) NaturezaOP, "
                            );

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "KMExecutado":
                    if (!select.Contains(" KMExecutado, "))
                    {
                        select.Append(
                                @" (select sum (CRP_DISTANCIA / 1000.0) Distancia from T_CARGA_ROTA_FRETE_PONTOS_PASSAGEM cargaRotaFretePontosPassagem
			                            where cargaRotaFretePontosPassagem.CRF_CODIGO =  cargaRotaFrete.CRF_CODIGO                            
                                ) KMExecutado, "
                            );
                        groupBy.Append("cargaRotaFrete.CRF_CODIGO, ");
                        SetarJoinsCargaRotaFrete(joins);
                    }
                    break;
                case "QuantidadeAnexos":
                    if (!select.Contains(" QuantidadeAnexos, "))
                    {
                        select.Append("Count(CargaVeiculoContainerAnexo.CVC_CODIGO) QuantidadeAnexos, ");

                        if (groupBy.Contains("CargaVeiculoContainerAnexo.CVC_CODIGO"))
                            groupBy.Append("CargaVeiculoContainerAnexo.CVC_CODIGO, ");

                        SetarJoinsCargaVeiculoContainerAnexo(joins);
                    }
                    break;
                case "TipoAnexo":
                    if (!select.Contains(" TipoAnexo, "))
                    {
                        select.Append("STRING_AGG(CargaVeiculoTipoAnexo.CTA_DESCRICAO,',') TipoAnexo, ");

                        if (groupBy.Contains("CargaVeiculoTipoAnexo.CTA_DESCRICAO"))
                            groupBy.Append("CargaVeiculoTipoAnexo.CTA_DESCRICAO, ");

                        SetarJoinsTipoAnexoVeiculoContainer(joins);
                    }
                    break;
                case "ExternalDT1":
                case "ExternalDT2":
                    if (!select.Contains(" DocumentoTransporte, "))
                    {
                        select.Append("DocumentoTransporteCarga.CDP_DOCUMENTO DocumentoTransporte, ");

                        if (!groupBy.Contains("DocumentoTransporteCarga.CDP_DOCUMENTO"))
                            groupBy.Append("DocumentoTransporteCarga.CDP_DOCUMENTO, ");

                        SetarJoinsDocumentoTransporte(joins);
                    }
                    break;

                case "CNPJLocalRetiradaContainerFormatado":
                    if (!select.Contains(" CNPJLocalRetiradaContainer,"))
                    {
                        select.Append("ClienteRetiradaContainer.CLI_CGCCPF CNPJLocalRetiradaContainer, ClienteRetiradaContainer.CLI_FISJUR TipoClienteLocalRetiradaContiner, ");
                        groupBy.Append("ClienteRetiradaContainer.CLI_CGCCPF, ClienteRetiradaContainer.CLI_FISJUR, ");

                        SetarJoinsRetiradaContainer(joins);

                        SetarJoinsClienteRetiradaContainer(joins);
                    }
                    break;

                case "JustificativaEncerramento":
                    if (!select.Contains(" JustificativaEncerramento, "))
                    {

                        select.Append(" EncerramentoManualViagem.EMV_DESCRICAO JustificativaEncerramento, ");

                        if (!groupBy.Contains("EncerramentoManualViagem.EMV_DESCRICAO, "))
                            groupBy.Append("EncerramentoManualViagem.EMV_DESCRICAO, ");

                        SetarJoinsEncerramentoManualViagem(joins);
                    }
                    break;

                case "ObservacaoEncerramento":
                    if (!select.Contains(" ObservacaoEncerramento, "))
                    {

                        select.Append(" Carga.CAR_OBSERVACAO_ENCERRAMENTO_MANUAL_VIAGEM ObservacaoEncerramento, ");

                        if (!groupBy.Contains("Carga.CAR_OBSERVACAO_ENCERRAMENTO_MANUAL_VIAGEM, "))
                            groupBy.Append("Carga.CAR_OBSERVACAO_ENCERRAMENTO_MANUAL_VIAGEM, ");
                    }
                    break;

                case "EncerramentoManual":
                case "PossuiEncerramentoManual":
                    if (!select.Contains(" EncerramentoManual, "))
                    {
                        select.Append("Carga.EMV_CODIGO EncerramentoManual, ");

                        if (!groupBy.Contains("Carga.EMV_CODIGO,"))
                            groupBy.Append("Carga.EMV_CODIGO, ");
                    }
                    break;

                case "DistanciaPrevista":
                case "KMPrevisto":
                    if (!select.Contains(" DistanciaPrevista, "))
                    {
                        select.Append("Monitoramento.MON_DISTANCIA_PREVISTA DistanciaPrevista, ");

                        if (!groupBy.Contains("Monitoramento.MON_CODIGO,"))
                            groupBy.Append("Monitoramento.MON_CODIGO, ");

                        if (!groupBy.Contains("Monitoramento.MON_DISTANCIA_PREVISTA,"))
                            groupBy.Append("Monitoramento.MON_DISTANCIA_PREVISTA, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "DistanciaRealizada":
                case "KMRealizado":
                    if (!select.Contains(" DistanciaRealizada, "))
                    {
                        select.Append("Monitoramento.MON_DISTANCIA_REALIZADA DistanciaRealizada, ");

                        if (!groupBy.Contains("Monitoramento.MON_CODIGO,"))
                            groupBy.Append("Monitoramento.MON_CODIGO, ");

                        if (!groupBy.Contains("Monitoramento.MON_DISTANCIA_REALIZADA,"))
                            groupBy.Append("Monitoramento.MON_DISTANCIA_REALIZADA, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "ValorNFSemPallet":
                    if (!select.Contains(" ValorNFSemPallet, "))
                    {
                        select.Append("DadosSumarizados.CDS_VALOR_TOTAL_PRODUTOS ValorNFSemPallet, ");
                        groupBy.Append("DadosSumarizados.CDS_VALOR_TOTAL_PRODUTOS, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "CargaRelacionada":
                    if (!select.Contains(" CargaRelacionada, "))
                    {
                        select.Append(@"(SELECT Carga.CAR_CODIGO_CARGA_EMBARCADOR FROM T_CARGA Carga 
                                            WHERE Carga.CAR_CODIGO = CargaRelacionada.CAR_CODIGO_RELACAO) CargaRelacionada, ");

                        groupBy.Append("CargaRelacionada.CAR_CODIGO_RELACAO, ");

                        SetarJoinsCargaRelacionada(joins);
                    }
                    break;

                case "CanalEntrega":
                    if (!select.Contains(" CanalEntrega, "))
                    {
                        select.Append(@"(SELECT Top 1 CanalEntrega.CNE_DESCRICAO
			                                FROM T_CANAL_ENTREGA CanalEntrega
			                                JOIN T_STAGE Stage ON Stage.CNE_CODIGO = CanalEntrega.CNE_CODIGO
			                                JOIN T_PEDIDO_STAGE PedidoStage ON PedidoStage.STA_CODIGO = Stage.STA_CODIGO
			                                JOIN T_CARGA_PEDIDO CargaPedido ON CargaPedido.PED_CODIGO = PedidoStage.PED_CODIGO
			                                WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO) CanalEntrega, ");
                        groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "JustificativaCargaRelacionada":
                    if (!select.Contains(" JustificativaCargaRelacionada, "))
                    {
                        select.Append("Carga.CAR_JUSTIFICATIVA_CARGA_RELACIONADA JustificativaCargaRelacionada, ");
                        groupBy.Append("Carga.CAR_JUSTIFICATIVA_CARGA_RELACIONADA, ");
                    }
                    break;

                case "DescricaoTAGPedagio":
                    if (!select.Contains(" TAGPedagio, "))
                    {
                        select.Append("Carga.CAR_TAG_PEDAGIO TAGPedagio, ");
                        groupBy.Append("Carga.CAR_TAG_PEDAGIO, ");
                    }
                    break;

                case "CustoFrete":
                    if (!select.Contains("CustoFrete, "))
                    {
                        select.Append("DadosSumarizados.CDS_CUSTO_FRETE CustoFrete, ");
                        groupBy.Append("DadosSumarizados.CDS_CUSTO_FRETE, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "CargaLacre":
                    if (!select.Contains(" CargaLacre, "))
                    {
                        select.Append(@"substring((     SELECT ', ' +  CargaLacre.CLA_NUMERO  FROM T_CARGA_LACRE CargaLacre  where CargaLacre.CAR_CODIGO = Carga.CAR_CODIGO        
                                        for xml path('') ), 3, 1000) CargaLacre, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "Cubagem":
                    if (!select.Contains("Cubagem, "))
                    {
                        select.Append("DadosSumarizados.CDS_CUBAGEM_TOTAL Cubagem, ");
                        groupBy.Append("DadosSumarizados.CDS_CUBAGEM_TOTAL, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;
                case "Aprovador":
                    if (!select.Contains("Aprovador, "))
                    {
                        select.Append("FuncioanrioAprovacao.FUN_NOME Aprovador, ");
                        groupBy.Append("FuncioanrioAprovacao.FUN_NOME, ");

                        SetarJoinsAprovador(joins);
                    }
                    break;
                case "SituacaoAprovacaoFormatada":
                    if (!select.Contains("SituacaoAprovacao, "))
                    {
                        select.Append("Autorizacao.AAL_SITUACAO SituacaoAprovacao, ");
                        groupBy.Append("Autorizacao.AAL_SITUACAO, ");

                        SetarJoinsAutorizacao(joins);
                    }
                    break;
                case "DataAutorizacaoFormatada":
                    if (!select.Contains("DataAutorizacao, "))
                    {
                        select.Append("Autorizacao.AAL_DATA DataAutorizacao, ");
                        groupBy.Append("Autorizacao.AAL_DATA, ");

                        SetarJoinsAutorizacao(joins);
                    }
                    break;
                case "ContratoTerceiro":
                    if (!select.Contains("ContratoTerceiro, "))
                    {
                        select.Append("ContratoFreteTerceiro.CFT_NUMERO_CONTRATO ContratoTerceiro, ");
                        groupBy.Append("ContratoFreteTerceiro.CFT_NUMERO_CONTRATO, ");
                        SetarJoinContratoFreteTerceiro(joins);
                    }
                    break;
                case "PedagioPagoTerceiro":
                    if (!select.Contains("PedagioPagoTerceiro, "))
                    {
                        select.Append("ContratoFreteTerceiro.CFT_VALOR_PEDAGIO PedagioPagoTerceiro, ");
                        groupBy.Append("ContratoFreteTerceiro.CFT_VALOR_PEDAGIO, ");
                        SetarJoinContratoFreteTerceiro(joins);
                    }
                    break;
                case "OutrosDescontosTerceiro":
                    if (!select.Contains("OutrosDescontosTerceiro, "))
                    {
                        select.Append("ContratoFreteTerceiro.CFT_DESCONTO OutrosDescontosTerceiro, ");
                        groupBy.Append("ContratoFreteTerceiro.CFT_DESCONTO, ");
                        SetarJoinContratoFreteTerceiro(joins);
                    }
                    break;
                case "IRPFTerceiro":
                    if (!select.Contains("IRPFTerceiro, "))
                    {
                        select.Append("ContratoFreteTerceiro.CFT_VALOR_IRRF IRPFTerceiro, ");
                        groupBy.Append("ContratoFreteTerceiro.CFT_VALOR_IRRF, ");
                        SetarJoinContratoFreteTerceiro(joins);
                    }
                    break;
                case "INSSTerceiro":
                    if (!select.Contains("INSSTerceiro, "))
                    {
                        select.Append("ContratoFreteTerceiro.CFT_VALOR_INSS INSSTerceiro, ");
                        groupBy.Append("ContratoFreteTerceiro.CFT_VALOR_INSS, ");
                        SetarJoinContratoFreteTerceiro(joins);
                    }
                    break;
                case "SESTSENATTerceiro":
                    if (!select.Contains("SESTSENATTerceiro, "))
                    {
                        select.Append("(ISNULL(ContratoFreteTerceiro.CFT_VALOR_SEST, 0) + ISNULL(ContratoFreteTerceiro.CFT_VALOR_SENAT, 0)) SESTSENATTerceiro, ");
                        groupBy.Append("ContratoFreteTerceiro.CFT_VALOR_SEST, ");
                        groupBy.Append("ContratoFreteTerceiro.CFT_VALOR_SENAT, ");
                        SetarJoinContratoFreteTerceiro(joins);
                    }
                    break;
                case "OutrasTaxasTerceiro":
                    if (!select.Contains("OutrasTaxasTerceiro, "))
                    {
                        select.Append("ISNULL((select SUM(CFV_VALOR) from T_CONTRATO_FRETE_TERCEIRO_VALOR v where CFT_CODIGO = 406 and CFV_APLICACAO_VALOR = 1 and CFV_TIPO_JUSTIFICATIVA = 1 and v.CFT_CODIGO = ContratoFreteTerceiro.CFT_CODIGO), 0) OutrasTaxasTerceiro, ");
                        groupBy.Append("ContratoFreteTerceiro.CFT_CODIGO, ");
                        SetarJoinContratoFreteTerceiro(joins);
                    }
                    break;
                case "ValorTotalTerceiro":
                    if (!select.Contains("ValorTotalTerceiro, "))
                    {
                        select.Append("ContratoFreteTerceiro.CFT_VALOR_FRETE_SUB_CONTRATACAO ValorTotalTerceiro, ");
                        groupBy.Append("ContratoFreteTerceiro.CFT_VALOR_FRETE_SUB_CONTRATACAO, ");
                        SetarJoinContratoFreteTerceiro(joins);
                    }
                    break;

                case "QuantidadeCTeAnterior":
                    if (!select.Contains(" QuantidadeCTeAnterior, "))
                    {
                        select.Append("( ");
                        select.Append("    SELECT COUNT(1) from T_PEDIDO_CTE_PARA_SUB_CONTRATACAO PedidoCTESubContratacao ");
                        select.Append("    inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoCTESubContratacao.CPE_CODIGO ");
                        select.Append("    where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append(") QuantidadeCTeAnterior, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "QuantidadePacotes":
                    if (!select.Contains(" QuantidadePacotes, "))
                    {
                        select.Append("( ");
                        select.Append("    select COUNT(1) from T_CARGA_PEDIDO_PACOTE CargaPedidoPacote ");
                        select.Append("    inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = CargaPedidoPacote.CPE_CODIGO ");
                        select.Append("    where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append(") QuantidadePacotes, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "QuantidadePacotesColetados":
                    if (!select.Contains(" QuantidadePacotesColetados, "))
                    {
                        select.Append("( ");
                        select.Append("    select Sum(CargaEntrega.CEN_QUANTIDADE_PACOTES_COLETADOS) from T_CARGA_ENTREGA CargaEntrega ");
                        select.Append("    where CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("     and CargaEntrega.CEN_COLETA = 1 ");
                        select.Append(") QuantidadePacotesColetados, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DataCarregamentoInicio":
                    if (!select.Contains(" DataCarregamentoInicio, "))
                    {
                        select.Append("Carga.CAR_DATA_INICIO_CARREGAMENTO DataCarregamentoInicio, ");
                        if (!groupBy.Contains("Carga.CAR_DATA_INICIO_CARREGAMENTO, "))
                            groupBy.Append("Carga.CAR_DATA_INICIO_CARREGAMENTO, ");
                    }
                    break;
                case "DataCarregamentoFim":
                    if (!select.Contains(" DataCarregamentoFim, "))
                    {
                        select.Append("Carga.CAR_DATA_INICIO_CARREGAMENTO DataCarregamentoFim, ");
                        if (!groupBy.Contains("Carga.CAR_DATA_INICIO_CARREGAMENTO, "))
                            groupBy.Append("Carga.CAR_DATA_INICIO_CARREGAMENTO, ");
                    }
                    break;

                case "PalletsCarregadosNestaCarga":
                    if (!select.Contains(" PalletsCarregadosNestaCarga, "))
                    {
                        select.Append(@"(
	                                        SELECT CAST(ISNULL(SUM(CarregamentoPedido.CRP_PALLET), 0) as decimal(10, 3))
	                                        FROM T_CARREGAMENTO_PEDIDO CarregamentoPedido
	                                        WHERE Carga.CRG_CODIGO = CarregamentoPedido.CRG_CODIGO
                                        ) PalletsCarregadosNestaCarga, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        if (!groupBy.Contains("Carga.CRG_CODIGO, "))
                            groupBy.Append("Carga.CRG_CODIGO, ");
                    }
                    break;

                case "ContratoFrete":
                    if (!select.Contains(" ContratoFrete, "))
                    {
                        select.Append("(case when ContratoFreteTransportador.CFT_CODIGO is null then '' else ContratoFreteTransportador.CFT_NUMERO_EMBARCADOR + ' - ' + ContratoFreteTransportador.CFT_DESCRICAO end) ContratoFrete, ");
                        groupBy.Append("ContratoFreteTransportador.CFT_CODIGO, ContratoFreteTransportador.CFT_NUMERO_EMBARCADOR, ContratoFreteTransportador.CFT_DESCRICAO, ");

                        SetarJoinsContratoFreteTransportador(joins);
                    }
                    break;

                case "NumeroContratoFrete":
                    if (!select.Contains(" NumeroContratoFrete, "))
                    {
                        select.Append(@"(select TOP(1) CFT_NUMERO_EMBARCADOR from T_CONTRATO_FRETE_TRANSPORTADOR CFT 
                                            where Carga.EMP_CODIGO = CFT.EMP_CODIGO 
                                        and Carga.CAR_DATA_CARREGAMENTO between CFT.CFT_DATA_INICIAL and CFT.CFT_DATA_FINAL) NumeroContratoFrete, ");

                        if (!groupBy.Contains("Carga.EMP_CODIGO,"))
                            groupBy.Append("Carga.EMP_CODIGO, ");

                        if (!groupBy.Contains("Carga.CAR_DATA_CARREGAMENTO,"))
                            groupBy.Append("Carga.CAR_DATA_CARREGAMENTO, ");
                    }
                    break;

                case "DescricaoContratoFrete":
                    if (!select.Contains(" DescricaoContratoFrete, "))
                    {
                        select.Append(@"(select TOP(1) CFT_DESCRICAO from T_CONTRATO_FRETE_TRANSPORTADOR CFT 
                                            where Carga.EMP_CODIGO = CFT.EMP_CODIGO 
                                        and Carga.CAR_DATA_CARREGAMENTO between CFT.CFT_DATA_INICIAL and CFT.CFT_DATA_FINAL) DescricaoContratoFrete, ");

                        if (!groupBy.Contains("Carga.EMP_CODIGO,"))
                            groupBy.Append("Carga.EMP_CODIGO, ");

                        if (!groupBy.Contains("Carga.CAR_DATA_CARREGAMENTO,"))
                            groupBy.Append("Carga.CAR_DATA_CARREGAMENTO, ");
                    }
                    break;

                case "DataInicialContratoFrete":
                case "DataInicialContratoFreteFormatada":
                    if (!select.Contains(" DataInicialContratoFrete, "))
                    {
                        select.Append(@"(select TOP(1) CFT_DATA_INICIAL from T_CONTRATO_FRETE_TRANSPORTADOR CFT 
                                            where Carga.EMP_CODIGO = CFT.EMP_CODIGO 
                                        and Carga.CAR_DATA_CARREGAMENTO between CFT.CFT_DATA_INICIAL and CFT.CFT_DATA_FINAL) DataInicialContratoFrete, ");

                        if (!groupBy.Contains("Carga.EMP_CODIGO,"))
                            groupBy.Append("Carga.EMP_CODIGO, ");

                        if (!groupBy.Contains("Carga.CAR_DATA_CARREGAMENTO,"))
                            groupBy.Append("Carga.CAR_DATA_CARREGAMENTO, ");
                    }
                    break;

                case "DataFinalContratoFrete":
                case "DataFinalContratoFreteFormatada":
                    if (!select.Contains(" DataFinalContratoFrete, "))
                    {
                        select.Append(@"(select TOP(1) CFT_DATA_FINAL from T_CONTRATO_FRETE_TRANSPORTADOR CFT 
                                            where Carga.EMP_CODIGO = CFT.EMP_CODIGO 
                                        and Carga.CAR_DATA_CARREGAMENTO between CFT.CFT_DATA_INICIAL and CFT.CFT_DATA_FINAL) DataFinalContratoFrete, ");

                        if (!groupBy.Contains("Carga.EMP_CODIGO,"))
                            groupBy.Append("Carga.EMP_CODIGO, ");

                        if (!groupBy.Contains("Carga.CAR_DATA_CARREGAMENTO,"))
                            groupBy.Append("Carga.CAR_DATA_CARREGAMENTO, ");
                    }
                    break;

                case "TipoContratoFrete":
                    if (!select.Contains(" TipoContratoFrete, "))
                    {
                        select.Append(@"(select TOP(1) TipoContratoFrete.TCF_DESCRICAO from T_CONTRATO_FRETE_TRANSPORTADOR CFT 
                                         left join T_TIPO_CONTRATO_FRETE TipoContratoFrete on TipoContratoFrete.TCF_CODIGO = CFT.TCF_CODIGO
                                            where Carga.EMP_CODIGO = CFT.EMP_CODIGO 
                                        and Carga.CAR_DATA_CARREGAMENTO between CFT.CFT_DATA_INICIAL and CFT.CFT_DATA_FINAL) TipoContratoFrete, ");

                        if (!groupBy.Contains("Carga.EMP_CODIGO,"))
                            groupBy.Append("Carga.EMP_CODIGO, ");

                        if (!groupBy.Contains("Carga.CAR_DATA_CARREGAMENTO,"))
                            groupBy.Append("Carga.CAR_DATA_CARREGAMENTO, ");
                    }
                    break;
                case "AgrupamentoCargas":
                    if (!select.Contains(" AgrupamentoCargas, "))
                    {
                        select.Append(@"SUBSTRING(
                                        (
                                            SELECT 
                                            ', ' + CargaCodigosAgrupados.CAR_CODIGO_CARGA_AGRUPADO 
                                            FROM 
                                            T_CARGA_CODIGOS_AGRUPADOS CargaCodigosAgrupados 
                                            where 
                                            Carga.CAR_CODIGO = CargaCodigosAgrupados.CAR_CODIGO for xml path('')
                                        ), 
                                        3, 
                                        1000
                                        ) AgrupamentoCargas, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "RotaRecorrente":
                case "RotaRecorrenteDescricao":
                    if (!select.Contains(" RotaRecorrente, "))
                    {
                        select.Append("Carga.CAR_ROTA_RECORRENTE RotaRecorrente, ");

                        if (!groupBy.Contains("Carga.CAR_ROTA_RECORRENTE,"))
                            groupBy.Append("Carga.CAR_ROTA_RECORRENTE, ");
                    }
                    break;

                case "CodigoIntegracaoRota":
                    if (!select.Contains("CodigoIntegracaoRota, "))
                    {
                        select.Append("Rota.ROF_CODIGO_INTEGRACAO CodigoIntegracaoRota, ");
                        groupBy.Append("Rota.ROF_CODIGO_INTEGRACAO, ");

                        SetarJoinsRota(joins);
                    }
                    break;

                case "AprovadorDocumento":
                    if (!select.Contains("AprovadorDocumento, "))
                    {
                        select.Append(@"(SELECT TOP 1 Aprovador.FUN_NOME
                                            FROM T_FUNCIONARIO Aprovador
                                            LEFT JOIN T_AUTORIZACAO_ALCADA_DOCUMENTO AutorizacaoAlcada on Aprovador.FUN_CODIGO = AutorizacaoAlcada.FUN_CODIGO
                                            LEFT JOIN T_GESTAO_DOCUMENTO GestaoDocumento on AutorizacaoAlcada.GED_CODIGO = GestaoDocumento.GED_CODIGO
                                            LEFT JOIN T_CARGA_CTE CargaCTE on GestaoDocumento.CON_CODIGO = CargaCTE.CON_CODIGO
                                            WHERE CargaCTE.CAR_CODIGO = Carga.CAR_CODIGO AND GestaoDocumento.GED_SITUACAO_GESTAO_DOCUMENTO = 3 AND AutorizacaoAlcada.AAL_SITUACAO = 1
                                            ORDER BY AutorizacaoAlcada.AAL_CODIGO desc) AprovadorDocumento, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                    }
                    break;

                case "DataAprovacaoDocumentoFormatada":
                    if (!select.Contains("DataAprovacaoDocumento, "))
                    {
                        select.Append(@"(SELECT TOP 1 AutorizaoAlcada.AAL_DATA
                                            FROM T_AUTORIZACAO_ALCADA_DOCUMENTO AutorizaoAlcada
                                            LEFT JOIN T_GESTAO_DOCUMENTO GestaoDocumento ON AutorizaoAlcada.GED_CODIGO = GestaoDocumento.GED_CODIGO
                                            LEFT JOIN T_CARGA_CTE CargaCTE ON GestaoDocumento.CON_CODIGO = CargaCTE.CON_CODIGO
                                            WHERE CargaCTE.CAR_CODIGO = Carga.CAR_CODIGO and AutorizaoAlcada.AAL_SITUACAO = 1
                                            ORDER BY AutorizaoAlcada.AAL_CODIGO DESC) DataAprovacaoDocumento, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                    }
                    break;

                case "SituacaoAprovacaoDocumento":
                    if (!select.Contains("SituacaoAprovacaoDocumento, "))
                    {
                        select.Append(@"SUBSTRING((
                                            SELECT ', ' || DescricaoSituacao FROM ( 

                                            SELECT DISTINCT 
                                            CASE GestaoDocumento.GED_SITUACAO_GESTAO_DOCUMENTO when 1 then 'Inconsistente'
                                            when 2 then 'Rejeitado'
                                            when 3 then 'Aprovado'
                                            when 4 then 'Aprovado com Desconto'
                                            when 5 then 'Aguardando Aprovação'
                                            when 6 then 'Sem Regra de Aprovação'
                                            when 7 then 'Em Tratativa'

                                            else 'Outros' 
                                            END
                                            AS DescricaoSituacao
                                             FROM T_GESTAO_DOCUMENTO GestaoDocumento 
                                            JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CCT_CODIGO = GestaoDocumento.CCT_CODIGO                                            
                                             WHERE CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO ) Dados
                                             FOR XML PATH('')
                                             ), 3, 1000) SituacaoAprovacaoDocumento, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                    }
                    break;

                case "ParametroBaseCalculoFreteDescricao":
                    if (!select.Contains("ParametroBaseCalculoFrete, "))
                    {
                        select.Append(@"(SELECT TOP 1 ComposicaoFrete.CCF_TIPO_PARAMETRO 
                                                FROM T_CARGA_COMPOSICAO_FRETE ComposicaoFrete
                                                WHERE Carga.CAR_CODIGO = ComposicaoFrete.CAR_CODIGO) ParametroBaseCalculoFrete, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "PossuiComponenteDuplicado":
                    if (!select.Contains("ComponenteDuplicado, "))
                    {
                        select.Append(@"(SELECT TOP 1 COUNT(*)
                                           FROM T_CARGA_COMPOSICAO_FRETE ComposicaoFrete
                                      LEFT JOIN T_CARGA_COMPOSICAO_FRETE_CARGA_PEDIDO CargaComposicaoFreteCargaPedido on CargaComposicaoFreteCargaPedido.CCF_CODIGO = ComposicaoFrete.CCF_CODIGO
	                                      WHERE Carga.CAR_CODIGO = ComposicaoFrete.CAR_CODIGO
                                       GROUP BY ComposicaoFrete.CAR_CODIGO
		                                      , ISNULL(CargaComposicaoFreteCargaPedido.CPE_CODIGO, 0)
	                                          , ComposicaoFrete.CCF_FORMULA
		                                      , CASE WHEN CHARINDEX(' * ', CCF_VALORES_FORMULA) > 0 
				                                          THEN SUBSTRING(CCF_VALORES_FORMULA, 1, CHARINDEX(' * ', CCF_VALORES_FORMULA))
				                                     WHEN CHARINDEX(' = ', CCF_VALORES_FORMULA) > 0 
				                                          THEN SUBSTRING(CCF_VALORES_FORMULA, 1, CHARINDEX(' = ', CCF_VALORES_FORMULA))
				                                     ELSE CCF_VALORES_FORMULA
		                                        END
		                                 HAVING COUNT(*) > 1) ComponenteDuplicado, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "NumeroPagamento":
                    if (!select.Contains("NumeroPagamento, "))
                    {
                        select.Append(@"(SELECT Pagamento.PAG_NUMERO
                                           FROM T_PAGAMENTO Pagamento
                                          WHERE Pagamento.PAG_CODIGO = (SELECT MAX(PagamentoAuxiliar.PAG_CODIGO)
                                                                          FROM T_DOCUMENTO_FATURAMENTO DocumentoFaturamento
                                                                          JOIN T_PAGAMENTO PagamentoAuxiliar on DocumentoFaturamento.PAG_CODIGO = PagamentoAuxiliar.PAG_CODIGO
                                                                     LEFT JOIN T_CARGA_CTE CargaCTe on CargaCTe.CON_CODIGO = DocumentoFaturamento.CON_CODIGO
                                                                         WHERE CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO)) NumeroPagamento, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DataEnvioPagamentoFormatada":
                    if (!select.Contains("DataEnvioPagamento, "))
                    {
                        select.Append(@"(SELECT Pagamento.PAG_DATA_CRIACAO
                                           FROM T_PAGAMENTO Pagamento
                                          WHERE Pagamento.PAG_CODIGO = (SELECT MAX(PagamentoAuxiliar.PAG_CODIGO)
                                                                          FROM T_DOCUMENTO_FATURAMENTO DocumentoFaturamento
                                                                          JOIN T_PAGAMENTO PagamentoAuxiliar on DocumentoFaturamento.PAG_CODIGO = PagamentoAuxiliar.PAG_CODIGO
                                                                     LEFT JOIN T_CARGA_CTE CargaCTe on CargaCTe.CON_CODIGO = DocumentoFaturamento.CON_CODIGO
                                                                         WHERE CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO)) DataEnvioPagamento, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "SituacaoPagamentoDescricao":
                    if (!select.Contains("SituacaoPagamento, "))
                    {
                        select.Append(@"(SELECT Pagamento.PAG_SITUACAO
                                           FROM T_PAGAMENTO Pagamento
                                          WHERE Pagamento.PAG_CODIGO = (SELECT MAX(PagamentoAuxiliar.PAG_CODIGO)
                                                                          FROM T_DOCUMENTO_FATURAMENTO DocumentoFaturamento
                                                                          JOIN T_PAGAMENTO PagamentoAuxiliar on DocumentoFaturamento.PAG_CODIGO = PagamentoAuxiliar.PAG_CODIGO
                                                                     LEFT JOIN T_CARGA_CTE CargaCTe on CargaCTe.CON_CODIGO = DocumentoFaturamento.CON_CODIGO
                                                                         WHERE CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO)) SituacaoPagamento, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DiferencaValores":
                    if (!select.Contains("DiferencaValores, "))
                    {
                        select.Append(@"SUBSTRING((
                                        SELECT ', CT-e ' || CTe.CON_NUM || ': ' || CAST((ISNULL(PreCTe.PCO_VALOR_RECEBER, 0) - CTe.CON_VALOR_RECEBER) AS DECIMAL(18,2))
		                                   FROM T_GESTAO_DOCUMENTO GestaoDocumento 
		                                   JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CCT_CODIGO = GestaoDocumento.CCT_CODIGO                                           
                                      LEFT JOIN T_CTE CTe on CTe.CON_CODIGO = GestaoDocumento.CON_CODIGO
                                      LEFT JOIN T_CARGA_CTE_COMPLEMENTO_INFO CargaCTeComplementoInfo ON CargaCTeComplementoInfo.CCC_CODIGO = GestaoDocumento.CCC_CODIGO
                                      LEFT JOIN T_PRE_CTE PreCTe ON PreCTe.PCO_CODIGO = ISNULL(CargaCTe.PCO_CODIGO, CargaCTeComplementoInfo.PCO_CODIGO)
                                          WHERE CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO
                                        FOR XML PATH('')
                                        ), 3, 1000) DiferencaValores, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "ResponsavelValePedagio":
                    if (!select.Contains("ResponsavelValePedagio, "))
                    {
                        select.Append(@"(SELECT T_CLIENTE.CLI_NOME AS ResponsavelValePedagio
                                           FROM T_CARGA_VALE_PEDAGIO
                                           LEFT JOIN T_CLIENTE
                                           ON T_CARGA_VALE_PEDAGIO.cli_codigo_responsavel = T_CLIENTE.cli_cgccpf
                                           WHERE T_CARGA_VALE_PEDAGIO.car_codigo = carga.car_codigo ) ResponsavelValePedagio, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "TipoOSConvertidoDescricao":
                    if (!select.Contains("TipoOSConvertido"))
                    {
                        // Este campo representa um Enum. Quando foi criado, não havia a opção nenhum/todos. 
                        // Como o campo é nullable, se estiver null, é convertido para 0. 
                        // No entanto, 0 não representa a opção nenhum/todos, mas sim a opção 2, 
                        // que foi criada posteriormente.
                        select.Append(@"( SELECT STRING_AGG(COALESCE(PED_TIPO_OS_CONVERTIDO, 4), ', ') 
                                      FROM 
                                        (
                                          SELECT DISTINCT Pedido.PED_TIPO_OS_CONVERTIDO
                                          FROM 
                                            T_CARGA_PEDIDO CargaPedido 
                                            LEFT JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                                          WHERE 
                                            CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO
                                        ) AS DescricoesUnicas
                                    ) AS TipoOSConvertido, "
);
                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;
                case "TipoOSDescricao":
                    if (!select.Contains("TipoOS, "))
                    {
                        // Este campo representa um Enum. Quando foi criado, não havia a opção nenhum/todos. 
                        // Como o campo é nullable, se estiver null, é convertido para 0. 
                        // No entanto, 0 não representa a opção nenhum/todos, mas sim a opção 4, 
                        // que foi criada posteriormente.
                        select.Append(@"( SELECT STRING_AGG(COALESCE(PED_TIPO_OS, 4), ', ') 
                                      FROM 
                                        (
                                          SELECT DISTINCT Pedido.PED_TIPO_OS
                                          FROM 
                                            T_CARGA_PEDIDO CargaPedido 
                                            LEFT JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                                          WHERE 
                                            CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO
                                        ) AS DescricoesUnicas
                                    ) AS TipoOS, "
                        );
                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "ProvedorOS":
                    if (!select.Contains("ProvedorOS, "))
                    {
                        select.Append(@"( SELECT STRING_AGG(CLI_NOME, ', ') 
                                      FROM 
                                        (
                                          SELECT DISTINCT ProvedorOS.CLI_NOME
                                          FROM 
                                            T_CARGA_PEDIDO CargaPedido 
                                            LEFT JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                                            LEFT JOIN T_CLIENTE ProvedorOS ON Pedido.CLI_CODIGO_PROVEDOR_OS = ProvedorOS.CLI_CGCCPF
                                          WHERE 
                                            CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO
                                        ) AS DescricoesUnicas
                                    ) AS ProvedorOS, "
                        );

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DirecionamentoCustoExtraDescricao":
                    if (!select.Contains("DirecionamentoCustoExtra, "))
                    {
                        // Este campo representa um Enum. Quando foi criado, não havia a opção nenhum/todos. 
                        // Como o campo é nullable, se estiver null, é convertido para 0. 
                        // No entanto, 0 não representa a opção nenhum/todos, mas sim a opção 3, 
                        // que foi criada posteriormente.
                        select.Append("COALESCE(ConfiguracaoTipoOperacaoCarga.CCG_DIRECIONAMENTO_CUSTO_EXTRA, 3) DirecionamentoCustoExtra,");

                        if (!groupBy.Contains("ConfiguracaoTipoOperacaoCarga.CCG_DIRECIONAMENTO_CUSTO_EXTRA,"))
                            groupBy.Append("ConfiguracaoTipoOperacaoCarga.CCG_DIRECIONAMENTO_CUSTO_EXTRA, ");

                        SetarJoinConfiguracaoTipoOperacaoCarga(joins);
                    }
                    break;

                case "StatusCustoExtraDescricao":
                    if (!select.Contains("StatusCustoExtra, "))
                    {
                        // Este campo representa um Enum. Quando foi criado, não havia a opção nenhum/todos. 
                        // Como o campo é nullable, se estiver null, é convertido para 0. 
                        // No entanto, 0 não representa a opção nenhum/todos, mas sim a opção 4, 
                        // que foi criada posteriormente.
                        select.Append("COALESCE(Carga.CAR_STATUS_CUSTO_EXTRA, 4) StatusCustoExtra,");

                        if (!groupBy.Contains("Carga.CAR_STATUS_CUSTO_EXTRA,"))
                            groupBy.Append("Carga.CAR_STATUS_CUSTO_EXTRA, ");
                    }
                    break;

                case "ValorFreteIntegracao":
                    if (!select.Contains("ValorFreteIntegracao, "))
                    {
                        select.Append("Carga.CAR_VALOR_TOTAL_PROVEDOR ValorFreteIntegracao,");

                        if (!groupBy.Contains("Carga.CAR_VALOR_TOTAL_PROVEDOR,"))
                            groupBy.Append("Carga.CAR_VALOR_TOTAL_PROVEDOR, ");
                    }
                    break;

                case "UsuarioAlteracaoFrete":
                    if (!select.Contains(" UsuarioAlteracaoFrete,"))
                    {
                        select.Append(" DadosSumarizados.CDS_USUARIO_ALTERACAO_FRETE as UsuarioAlteracaoFrete, ");

                        SetarJoinsCargaDadosSumarizados(joins);

                        if (!groupBy.Contains("DadosSumarizados.CDS_USUARIO_ALTERACAO_FRETE,"))
                            groupBy.Append("DadosSumarizados.CDS_USUARIO_ALTERACAO_FRETE, ");
                    }
                    break;

                case "JustificativaCustoExtra":
                    if (!select.Contains("JustificativaCustoExtra, "))
                    {
                        select.Append("JustificativaAutorizacaoCarga.JAC_DESCRICAO JustificativaCustoExtra,");

                        if (!groupBy.Contains("JustificativaAutorizacaoCarga.JAC_DESCRICAO,"))
                            groupBy.Append("JustificativaAutorizacaoCarga.JAC_DESCRICAO, ");

                        SetarJoinJustificativaAutorizacaoCarga(joins);
                    }
                    break;

                case "TipoTerceiro":
                    if (!select.Contains("TipoTerceiro, "))
                    {
                        select.Append("TipoTerceiro.TPT_DESCRICAO TipoTerceiro,");

                        if (!groupBy.Contains("TipoTerceiro.TPT_DESCRICAO,"))
                            groupBy.Append("TipoTerceiro.TPT_DESCRICAO, ");

                        SetarJoinsTipoTerceiro(joins);
                    }
                    break;

                case "SetorResponsavel":
                    if (!select.Contains("SetorResponsavel, "))
                    {
                        select.Append("Setor.SET_DESCRICAO SetorResponsavel,");

                        if (!groupBy.Contains("Setor.SET_DESCRICAO,"))
                            groupBy.Append("Setor.SET_DESCRICAO, ");

                        SetarJoinSetor(joins);
                    }
                    break;

                case "CentroDeCustoViagemDescricao":
                    if (!select.Contains("CentroDeCustoViagemDescricao, "))
                    {
                        select.Append(@"( SELECT STRING_AGG(CCV_DESCRICAO, ', ') 
                                      FROM 
                                        (
                                          SELECT DISTINCT CentroDeCustoViagem.CCV_DESCRICAO
                                          FROM 
                                            T_CARGA_PEDIDO CargaPedido 
                                            LEFT JOIN T_PEDIDO Pedido ON CargaPedido.PED_CODIGO = Pedido.PED_CODIGO 
                                            LEFT JOIN T_CENTRO_CUSTO_VIAGEM CentroDeCustoViagem ON CentroDeCustoViagem.CCV_CODIGO = Pedido.CCV_CODIGO 
                                          WHERE 
                                            CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO
                                        ) AS DescricoesUnicas
                                    ) AS CentroDeCustoViagemDescricao, "
                        );

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "GrupoProduto":
                    if (!select.Contains("GrupoProduto, "))
                    {
                        select.Append(@"(SELECT 
                                           STUFF((
                                           SELECT DISTINCT ', ' + GrupoProduto.GRP_DESCRICAO
                                           FROM T_CARGA_PEDIDO CargaPedido
                                           LEFT JOIN T_CARGA_PEDIDO_PRODUTO CargaPedidoProduto ON CargaPedido.CPE_CODIGO = CargaPedidoProduto.CPE_CODIGO
                                           LEFT JOIN T_PRODUTO_EMBARCADOR ProdutoEmbarcador ON CargaPedidoProduto.PRO_CODIGO = ProdutoEmbarcador.PRO_CODIGO
                                           LEFT JOIN T_GRUPO_PRODUTO GrupoProduto ON ProdutoEmbarcador.GRP_CODIGO = GrupoProduto.GPR_CODIGO
                                           WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO
                                           FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
                                           ) GrupoProduto, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;
                case "SituacaoBRKFormatada":
                    if (!select.Contains("SituacaoBRK, "))
                    {
                        select.Append(@"(SELECT
                                           Integracao.INT_SITUACAO_INTEGRACAO
                                           FROM T_CARGA_CARGA_INTEGRACAO Integracao
		                                   LEFT JOIN T_TIPO_INTEGRACAO TipoIntegracao on Integracao.TPI_CODIGO = TipoIntegracao.TPI_CODIGO
		                                   WHERE Integracao.CAR_CODIGO = Carga.CAR_CODIGO) SituacaoBRK,");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;
                case "MensagemBRK":
                    if (!select.Contains("MensagemBRK, "))
                    {
                        select.Append(@"(SELECT
                                           MensagemIntegracao.INT_PROBLEMA_INTEGRACAO
                                           FROM T_CARGA_CARGA_INTEGRACAO MensagemIntegracao
		                                   LEFT JOIN T_TIPO_INTEGRACAO TipoIntegracao on MensagemIntegracao.TPI_CODIGO = TipoIntegracao.TPI_CODIGO
		                                   WHERE MensagemIntegracao.CAR_CODIGO = Carga.CAR_CODIGO) MensagemBRK,");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;
                case "PercentualExecucao":

                    select.Append(!select.Contains("PercentualExecucao") ? "CargaMotoristaDocumento.CMD_PERCENTUAL_EXECUCAO as PercentualExecucao, " : "");

                    joins.Append(!joins.Contains("CargaMotoristaDocumento") ?
                        @" OUTER APPLY (
                                SELECT CAST(cfmo.CMD_PERCENTUAL_EXECUCAO AS VARCHAR(10)) as CMD_PERCENTUAL_EXECUCAO
                                FROM T_COMISSAO_FUNCIONARIO_MOTORISTA_DOCUMENTO cfmo
                                WHERE cfmo.CAR_CODIGO = Carga.CAR_CODIGO
                            ) AS CargaMotoristaDocumento " : "");

                    groupBy.Append(!groupBy.Contains("CargaMotoristaDocumento.CMD_PERCENTUAL_EXECUCAO") ? "CargaMotoristaDocumento.CMD_PERCENTUAL_EXECUCAO, " : "");

                    break;

                case "CargaBloqueada":
                    if (!select.Contains("CargaBloqueada, "))
                    {
                        select.Append(@"(select CASE WHEN COUNT(*) > 0 THEN 'Sim' ELSE 'Não' END from T_MENSAGEM_ALERTA 
                                            WHERE MAL_BLOQUEAR = 1 AND MAL_CONFIRMADA = 0 AND Carga.CAR_CODIGO = T_MENSAGEM_ALERTA.CAR_CODIGO
                                            ) CargaBloqueada,");
                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;
                case "ValorTotalMercadoriaDosPedidos":
                    if (!select.Contains("ValorTotalMercadoriaDosPedidos, "))
                    {
                        select.Append("DadosSumarizados.CDS_VALOR_TOTAL_MERCADORIA_PEDIDOS ValorTotalMercadoriaDosPedidos, ");
                        groupBy.Append("DadosSumarizados.CDS_VALOR_TOTAL_MERCADORIA_PEDIDOS, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "OutrosNumerosDaCarga":
                    if (!select.Contains("OutrosNumerosDaCarga, "))
                    {
                        select.Append(@"(SELECT 
                                            COALESCE(SUBSTRING(
                                                (
                                                    SELECT ', ' + CargaAgrupada.CAR_CODIGO_CARGA_AGRUPADO
                                                    FROM T_CARGA_CODIGOS_AGRUPADOS CargaAgrupada
                                                    WHERE CargaAgrupada.CAR_CODIGO = Carga.CAR_CODIGO
                                                    FOR XML PATH(''), TYPE
                                                ).value('.', 'NVARCHAR(MAX)'), 2, LEN(
                                                    (
                                                        SELECT ', ' + CargaAgrupada.CAR_CODIGO_CARGA_AGRUPADO
                                                        FROM T_CARGA_CODIGOS_AGRUPADOS CargaAgrupada
                                                        WHERE CargaAgrupada.CAR_CODIGO = Carga.CAR_CODIGO
                                                        FOR XML PATH(''), TYPE
                                                    ).value('.', 'NVARCHAR(MAX)')
                                            )), ISNULL((SELECT CAST(CARGAAGRUPAMENTO.CAR_CODIGO_CARGA_EMBARCADOR AS VARCHAR)
                                                     FROM T_CARGA CARGAAGRUPAMENTO
                                                     WHERE CARGAAGRUPAMENTO.CAR_CODIGO = CARGA.CAR_CODIGO_AGRUPAMENTO), '')) AS CargaAgrupada
                                         ) OutrosNumerosDaCarga,");
                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                        if (!groupBy.Contains("CARGA.CAR_CODIGO_AGRUPAMENTO,"))
                            groupBy.Append("CARGA.CAR_CODIGO_AGRUPAMENTO, ");

                    }
                    break;
                case "DataInclusaoDadosTransporte":
                    if (!select.Contains("DataInclusaoDadosTransporte, "))
                    {
                        select.Append(@"Carga.CAR_DATA_SALVAMENTO_DADOS_TRANSPORTE DataInclusaoDadosTransporte,");

                        if (!groupBy.Contains("Carga.CAR_DATA_SALVAMENTO_DADOS_TRANSPORTE,"))
                            groupBy.Append("Carga.CAR_DATA_SALVAMENTO_DADOS_TRANSPORTE, ");
                    }
                    break;
                case "CargaReentrega":
                    if (!select.Contains("CargaReentrega, "))
                    {
                        select.Append("CASE WHEN COALESCE(DadosSumarizados.CDS_REENTREGA, 0) = 1 THEN 'Sim' ELSE 'Não' END CargaReentrega, ");
                        groupBy.Append("DadosSumarizados.CDS_REENTREGA, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;
                case "NumeroOrdemServico":
                    if (!select.Contains("NumeroOrdemServico, "))
                    {
                        select.Append("COALESCE(DadosSumarizados.CDS_NUMERO_ORDEM, '') NumeroOrdemServico,");
                        groupBy.Append("DadosSumarizados.CDS_NUMERO_ORDEM, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;
                case "MotivoSolicitacaoFrete":
                    if (!select.Contains("MotivoSolicitacaoFrete, "))
                    {
                        select.Append("MSF.MSF_DESCRICAO MotivoSolicitacaoFrete,");

                        if (!groupBy.Contains("MSF.MSF_DESCRICAO,"))
                            groupBy.Append("MSF.MSF_DESCRICAO, ");

                        SetarJoinMotivoSolicitacaoFrete(joins);
                    }
                    break;
                case "ObservacaoSolicitacaoFrete":
                    if (!select.Contains("ObservacaoSolicitacaoFrete, "))
                    {
                        select.Append("COALESCE(CARGA.CAR_OBSERVACAO_SOLICITACAO_FRETE, '') ObservacaoSolicitacaoFrete,");
                        groupBy.Append("CARGA.CAR_OBSERVACAO_SOLICITACAO_FRETE, ");
                    }
                    break;
                case "NumeroCTes":
                    if (!select.Contains("NumeroCTes, "))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("    SELECT DISTINCT ', ' +  CAST(CTE.CON_NUM AS NVARCHAR(20)) ");
                        select.Append("    FROM T_CARGA_CTE CARGACTE ");
                        select.Append("    JOIN T_CTE CTE ON CTE.CON_CODIGO = CARGACTE.CON_CODIGO ");
                        select.Append("    WHERE CARGACTE.CAR_CODIGO = CARGA.CAR_CODIGO ");
                        select.Append("    FOR XML PATH('') ");
                        select.Append("), 3, 1000) NumeroCTes, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DataVinculoTracaoFormatada":
                    if (!somenteContarNumeroRegistros && !select.Contains(" DataVinculoTracao,"))
                    {
                        select.Append(" (select max(HistoricoVinculoTracao.THV_DATA_HORA_VINCULO) " +
                                         " from T_HISTORICO_VINCULO HistoricoVinculoTracao " +
                                        " where HistoricoVinculoTracao.CAR_CODIGO = Carga.CAR_CODIGO " +
                                          " and HistoricoVinculoTracao.VEI_CODIGO_TRACAO = Carga.CAR_VEICULO) DataVinculoTracao, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        if (!groupBy.Contains("Carga.CAR_VEICULO, "))
                            groupBy.Append("Carga.CAR_VEICULO, ");
                    }
                    break;

                case "DataVinculoReboqueFormatada":
                    if (!somenteContarNumeroRegistros && !select.Contains(" DataVinculoReboque,"))
                    {
                        select.Append(" (select max(HistoricoVinculoReboque.THV_DATA_HORA_VINCULO) " +
                                         " from T_HISTORICO_VINCULO HistoricoVinculoReboque " +
                                        " inner join T_HISTORICO_VINCULO_REBOQUES HVReboques on HVReboques.THV_CODIGO = HistoricoVinculoReboque.THV_CODIGO " +
                                        " where HistoricoVinculoReboque.CAR_CODIGO = Carga.CAR_CODIGO " +
                                          " and HVReboques.VEI_CODIGO in (select CReboques.VEI_CODIGO from T_CARGA_VEICULOS_VINCULADOS CReboques where CReboques.CAR_CODIGO = Carga.CAR_CODIGO) " +
                                       ") DataVinculoReboque, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DataVinculoMotoristaFormatada":
                    if (!somenteContarNumeroRegistros && !select.Contains(" DataVinculoMotorista,"))
                    {
                        select.Append(" (select max(HistoricoVinculoMotorista.THV_DATA_HORA_VINCULO) " +
                                         " from T_HISTORICO_VINCULO HistoricoVinculoMotorista " +
                                        " inner join T_HISTORICO_VINCULO_MOTORISTAS HVMotoristas on HVMotoristas.THV_CODIGO = HistoricoVinculoMotorista.THV_CODIGO " +
                                        " where HistoricoVinculoMotorista.CAR_CODIGO = Carga.CAR_CODIGO " +
                                          " and HVMotoristas.FUN_CODIGO in (select CMotoristas.CAR_MOTORISTA from T_CARGA_MOTORISTA CMotoristas where CMotoristas.CAR_CODIGO = Carga.CAR_CODIGO) " +
                                       " ) DataVinculoMotorista, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DescricaoLocalVinculo":
                    if (!somenteContarNumeroRegistros && !select.Contains(" LocalVinculo,"))
                    {
                        select.Append(" (select max(HistoricoVinculo.THV_LOCAL_VINCULO) " +
                                         " from T_HISTORICO_VINCULO HistoricoVinculo " +
                                        " where HistoricoVinculo.CAR_CODIGO = Carga.CAR_CODIGO " +
                                       " ) LocalVinculo, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;
                case "Observacao":
                    if (!select.Contains("Observacao, "))
                    {
                        select.Append("Carga.CAR_OBSERVACAO Observacao, ");

                        if (!groupBy.Contains("Carga.CAR_OBSERVACAO, "))
                            groupBy.Append("Carga.CAR_OBSERVACAO, ");
                    }
                    break;

                case "NumeroDoca":
                    if (!select.Contains(" NumeroDoca,"))
                    {
                        select.Append("Carga.CAR_NUMERO_DOCA NumeroDoca, ");

                        if (!groupBy.Contains("Carga.CAR_NUMERO_DOCA, "))
                            groupBy.Append("Carga.CAR_NUMERO_DOCA, ");
                    }
                    break;

                case "PesoReentrega":
                    if (!select.Contains(" PesoReentrega,"))
                    {
                        select.Append(@"DadosSumarizados.CDS_PESO_TOTAL_REENTREGA PesoReentrega, ");

                        if (!groupBy.Contains("DadosSumarizados.CDS_PESO_TOTAL_REENTREGA, "))
                            groupBy.Append("DadosSumarizados.CDS_PESO_TOTAL_REENTREGA, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "PesoTotal":
                    if (!select.Contains(" PesoTotal,"))
                    {
                        select.Append(@" SUM(DadosSumarizados.CDS_PESO_TOTAL) + SUM(ISNULL(DadosSumarizados.CDS_PESO_TOTAL_REENTREGA, 0)) AS PesoTotal, ");
                        SetarJoinsCargaDadosSumarizados(joins);
                    }

                    break;

                case "CMDID":
                    if (!select.Contains(" CMDID,"))
                    {
                        select.Append(@" (SELECT TOP 1 ISNULL(Tomador.CLI_CMDID, Expedidor.CLI_CMDID) FROM T_CARGA_PEDIDO CargaPedido " +
                            "LEFT JOIN T_CLIENTE Tomador ON Tomador.CLI_CGCCPF = CargaPedido.CLI_CODIGO_TOMADOR " +
                            "LEFT JOIN T_CLIENTE Expedidor ON Expedidor.CLI_CGCCPF = CargaPedido.CLI_CODIGO_EXPEDIDOR " +
                            "WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO " +
                            "ORDER BY CargaPedido.CAR_CODIGO) " +
                            "AS CMDID, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;


                case "CodigoNavio":
                    if (!select.Contains(" CodigoNavio,"))
                    {
                        select.Append(@" (Select N.NAV_CODIGO_NAVIO " +
                                "From T_PEDIDO_VIAGEM_NAVIO PVN " +
                                "	LEFT JOIN T_NAVIO N ON N.NAV_CODIGO = PVN.NAV_CODIGO " +
                                "Where Carga.PVN_CODIGO = PVN.PVN_CODIGO) " +
                            "as CodigoNavio, ");

                        if (!groupBy.Contains("Carga.PVN_CODIGO, "))
                            groupBy.Append("Carga.PVN_CODIGO, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.ExibirCargasAgrupadas)
                where.Append(" and Carga.CAR_CARGA_FECHADA = 1 ");
            else
                where.Append(" and ((Carga.CAR_CARGA_FECHADA = 1 and Carga.CAR_CARGA_AGRUPADA = 0 ) or (Carga.CAR_CARGA_FECHADA = 0 and Carga.CAR_CODIGO_AGRUPAMENTO is not null)) ");

            if (filtrosPesquisa.InformacoesRelatorioCargas == InformacoesRelatorioCarga.SomentePreCargas)
            {
                where.Append(" and Carga.CAR_CARGA_DE_PRE_CARGA = 1 ");
            }
            else if (filtrosPesquisa.InformacoesRelatorioCargas == InformacoesRelatorioCarga.SomenteCargas)
            {
                where.Append(" and Carga.CAR_CARGA_DE_PRE_CARGA = 0 ");
            }

            if (filtrosPesquisa.HabilitarHoraFiltroDataInicialFinalRelatorioCargas)
            {
                string patternHour = "yyyy-MM-dd HH:mm";
                if (filtrosPesquisa.DataInicial.HasValue)
                    where.Append($" and CAST(Carga.CAR_DATA_CRIACAO AS DATE) >= '{filtrosPesquisa.DataInicial.Value.ToString(patternHour)}'");

                if (filtrosPesquisa.DataFinal.HasValue)
                    where.Append($" and CAST(Carga.CAR_DATA_CRIACAO AS DATE) <= '{filtrosPesquisa.DataFinal.Value.ToString(patternHour)}'");
            }
            else
            {
                if (filtrosPesquisa.DataInicial.HasValue)
                    where.Append($" and CAST(Carga.CAR_DATA_CRIACAO AS DATE) >= '{filtrosPesquisa.DataInicial.Value.ToString(pattern)}'");

                if (filtrosPesquisa.DataFinal.HasValue)
                    where.Append($" and CAST(Carga.CAR_DATA_CRIACAO AS DATE) < '{filtrosPesquisa.DataFinal.Value.AddDays(1).ToString(pattern)}'");
            }

            if (filtrosPesquisa.DataAnulacaoInicial.HasValue || filtrosPesquisa.DataAnulacaoFinal.HasValue)
            {
                SetarJoinsCargaAnulacao(joins);

                if (filtrosPesquisa.DataAnulacaoInicial.HasValue)
                    where.Append($" and CAST(CargaAnulacao.CAC_DATA_CANCELAMENTO AS DATE) >= '{filtrosPesquisa.DataAnulacaoInicial.Value.ToString(pattern)}'");

                if (filtrosPesquisa.DataAnulacaoFinal.HasValue)
                    where.Append($" and CAST(CargaAnulacao.CAC_DATA_CANCELAMENTO AS DATE) <= '{filtrosPesquisa.DataAnulacaoFinal.Value.ToString(pattern)}'");
            }

            if (filtrosPesquisa.DataInicialInicioEmissaoDocumentos.HasValue)
                where.Append($" and CAST(Carga.CAR_DATA_INICIO_GERACAO_CTES AS DATE) >= '{filtrosPesquisa.DataInicialInicioEmissaoDocumentos.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataFinalInicioEmissaoDocumentos.HasValue)
                where.Append($" and CAST(Carga.CAR_DATA_INICIO_GERACAO_CTES AS DATE) <= '{filtrosPesquisa.DataFinalInicioEmissaoDocumentos.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataInicialFimEmissaoDocumentos.HasValue)
                where.Append($" and CAST(Carga.CAR_DATA_FINALIZACAO_EMISSAO AS DATE) >= '{filtrosPesquisa.DataInicialFimEmissaoDocumentos.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataFinalFimEmissaoDocumentos.HasValue)
                where.Append($" and CAST(Carga.CAR_DATA_FINALIZACAO_EMISSAO AS DATE) <= '{filtrosPesquisa.DataFinalFimEmissaoDocumentos.Value.ToString(pattern)}'");

            if (filtrosPesquisa.CodigosTransportador?.Count > 0)
                where.Append($" and Carga.EMP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTransportador)})");

            if (filtrosPesquisa.SomenteTerceiros.HasValue)
            {
                if (filtrosPesquisa.SomenteTerceiros.Value)
                    where.Append(" and Carga.CAR_FRETE_TERCEIRO = 1 ");
                else
                    where.Append(" and (Carga.CAR_FRETE_TERCEIRO = 0 OR Carga.CAR_FRETE_TERCEIRO is null) ");
            }

            if (filtrosPesquisa.Transbordo.HasValue)
            {
                if (filtrosPesquisa.Transbordo.Value)
                    where.Append(" and Carga.CAR_CARGA_TRANSBORDO = 1 ");
                else
                    where.Append(" and (Carga.CAR_CARGA_TRANSBORDO = 0 or Carga.CAR_CARGA_TRANSBORDO is null) ");
            }

            if (filtrosPesquisa.CodigosFilial?.Count > 0)
            {
                if (filtrosPesquisa.CodigosFilial.Any(o => o == -1))
                {
                    where.Append($@" and (Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)}) OR EXISTS (   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                        FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                        LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                        WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                        AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.CpfCnpjRecebedoresOuSemRecebedores)})))");
                }
                else
                    where.Append($" and Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)})");

            }


            if (filtrosPesquisa.CodigosCentroCarregamento.Count > 0)
            {
                SetarJoinsJanelaCarregamento(joins);

                where.Append($" and JanelaCarregamento.CEC_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosCentroCarregamento)})");
            }
            if (filtrosPesquisa.CodigosRotas.Count > 0)
                where.Append($" and Carga.ROF_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosRotas)})");

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                where.Append($" and (Carga.TCG_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoCarga)}){(filtrosPesquisa.CodigosTipoCarga.Contains(-1) ? " or Carga.TCG_CODIGO is null" : "")})");

            if (filtrosPesquisa.CodigosModeloVeicularCarga.Count > 0)
                where.Append($" and Carga.MVC_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosModeloVeicularCarga)})");

            if (filtrosPesquisa.CodigoGrupoPessoas.Count() > 0)
                where.Append($" and Carga.GRP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigoGrupoPessoas)})");

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                where.Append($" and (Carga.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)}){(filtrosPesquisa.CodigosTipoOperacao.Contains(-1) ? " or Carga.TOP_CODIGO is null" : "")})");

            if (filtrosPesquisa.CodigoOperador > 0)
                where.Append($" and Carga.CAR_OPERADOR = {filtrosPesquisa.CodigoOperador}");

            if (filtrosPesquisa.Situacoes != null && filtrosPesquisa.Situacoes.Count > 0)
            {
                string situacoes = string.Join(", ", from situacao in filtrosPesquisa.Situacoes select situacao.ToString("d"));

                where.Append($" and Carga.CAR_SITUACAO IN ({situacoes})");
            }
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

            if (filtrosPesquisa.CodigosVeiculos?.Count > 0)
            {
                where.Append(" and ( ");
                where.Append($"        Carga.CAR_VEICULO in ({string.Join(", ", filtrosPesquisa.CodigosVeiculos)}) or ");
                where.Append("         Carga.CAR_CODIGO in ( ");
                where.Append("             select _cargaveiculos.CAR_CODIGO ");
                where.Append("               from T_CARGA_VEICULOS_VINCULADOS _cargaveiculos ");
                where.Append($"             WHERE _cargaveiculos.VEI_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosVeiculos)}) ");
                where.Append("         ) ");
                where.Append("     )");
            }

            if (filtrosPesquisa.CodigosMotorista?.Count > 0)
            {
                where.Append(" and Carga.CAR_CODIGO in ( ");
                where.Append("         select _cargamotorista.CAR_CODIGO ");
                where.Append("           from T_CARGA_MOTORISTA _cargamotorista ");
                where.Append($"         where _cargamotorista.CAR_MOTORISTA in ({string.Join(", ", filtrosPesquisa.CodigosMotorista)}) ");
                where.Append("     )");
            }

            if (filtrosPesquisa.SomenteDescontoOperador)
                where.Append(" AND (Carga.CAR_VALOR_FRETE_TABELA_DE_FRETE > Carga.CAR_VALOR_FRETE_PAGAR AND Carga.CAR_TIPO_FRETE_ESCOLHIDO = 2 AND Carga.CAR_VALOR_FRETE_TABELA_DE_FRETE > 0)");

            if (filtrosPesquisa.NumeroMDFe > 0)
            {
                where.Append(" AND EXISTS ( ");
                where.Append("         SELECT 1 ");
                where.Append("           FROM T_CARGA_MDFE CargaMdfe ");
                where.Append("           JOIN T_MDFE Mdfe ");
                where.Append("             ON CargaMdfe.MDF_CODIGO = Mdfe.MDF_CODIGO ");
                where.Append("          WHERE CargaMdfe.CAR_CODIGO = Carga.CAR_CODIGO ");
                where.Append($"           AND Mdfe.MDF_NUMERO = {filtrosPesquisa.NumeroMDFe} ");
                where.Append("     ) ");
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.PreCarga))
            {
                where.Append($" and PreCarga.PCA_NUMERO_CARGA = '{filtrosPesquisa.PreCarga}'");

                SetarJoinsPreCarga(joins);
            }

            if (filtrosPesquisa.CodigoCarregamento > 0)
            {
                where.Append($" and MontagemCarga.CRG_CODIGO = {filtrosPesquisa.CodigoCarregamento}");

                SetarJoinsMontagemCarga(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
            {
                if (filtrosPesquisa.FiltrarCargasPorParteDoNumero)
                    where.Append($" and (Carga.CAR_CODIGO_CARGA_EMBARCADOR like '%{filtrosPesquisa.CodigoCargaEmbarcador}%' or CargaAgrupada.CAR_CODIGO_CARGA_EMBARCADOR like '%{filtrosPesquisa.CodigoCargaEmbarcador}%')");
                else
                    where.Append($" and (Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}' or CargaAgrupada.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}')");

                SetarJoinsCargaAgrupada(joins);
            }

            if (filtrosPesquisa.CodigosTabelasFrete != null && filtrosPesquisa.CodigosTabelasFrete.Count > 0)
                where.Append($" and Carga.TBF_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTabelasFrete)})");

            if (filtrosPesquisa.CodigoPedidoViagemNavio > 0)
            {
                where.Append($" and Viagem.PVN_CODIGO = {filtrosPesquisa.CodigoPedidoViagemNavio}");

                SetarJoinsViagem(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroControle))
            {
                where.Append(" and Carga.CAR_CODIGO in (");
                where.Append("         select _cargaCTe.CAR_CODIGO ");
                where.Append("           from T_CARGA_CTE _cargaCTe");
                where.Append("           inner join T_CTE _cte on _cte.CON_CODIGO = _cargaCTe.CON_CODIGO ");
                where.Append($"         where _cte.CON_NUMERO_CONTROLE = '{filtrosPesquisa.NumeroControle}' ");
                where.Append("     ) ");
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.SituacaoCTe))
            {
                where.Append(" and Carga.CAR_CODIGO in (");
                where.Append("         select _cargaCTe.CAR_CODIGO ");
                where.Append("           from T_CARGA_CTE _cargaCTe");
                where.Append("           inner join T_CTE _cte on _cte.CON_CODIGO = _cargaCTe.CON_CODIGO ");
                where.Append($"         where _cte.CON_STATUS = '{filtrosPesquisa.SituacaoCTe}' ");
                where.Append("     ) ");
            }

            if (filtrosPesquisa.NumeroNF > 0)
            {
                where.Append(" and Carga.CAR_CODIGO in (");
                where.Append("         select _cargapedido.CAR_CODIGO ");
                where.Append("           from T_CARGA_PEDIDO _cargapedido");
                where.Append("           inner join T_PEDIDO_XML_NOTA_FISCAL _pex on _pex.CPE_CODIGO = _cargapedido.CPE_CODIGO ");
                where.Append("           inner join T_XML_NOTA_FISCAL _nfx on _nfx.NFX_CODIGO = _pex.NFX_CODIGO ");
                where.Append($"         where _nfx.NF_NUMERO = {filtrosPesquisa.NumeroNF} ");
                where.Append("     ) ");
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoNFe))
            {
                where.Append(" and Carga.CAR_CODIGO in (");
                where.Append("     select _cargapedido.CAR_CODIGO ");
                where.Append("     from T_CARGA_PEDIDO _cargapedido");
                where.Append("     inner join T_PEDIDO_XML_NOTA_FISCAL _pex on _pex.CPE_CODIGO = _cargapedido.CPE_CODIGO ");
                where.Append("     inner join T_XML_NOTA_FISCAL _nfx on _nfx.NFX_CODIGO = _pex.NFX_CODIGO ");
                where.Append($"     where _nfx.NF_NUMERO = {filtrosPesquisa.NumeroPedidoNFe} ");
                where.Append("     and _nfx.NF_NUMERO_PEDIDO_EMBARCADOR IS NOT NULL ");
                where.Append(" ) ");
                where.Append(" and NF_NUMERO_PEDIDO_EMBARCADOR = _nfx.NF_NUMERO_PEDIDO_EMBARCADOR");
            }

            if (filtrosPesquisa.CodigoTipoSeparacao > 0)
            {
                where.Append($" and TipoSeparacao.TSE_CODIGO = {filtrosPesquisa.CodigoTipoSeparacao}");

                SetarJoinsTipoSeparacao(joins);
            }

            if (filtrosPesquisa.DataEncerramentoInicial.HasValue)
                where.Append($" and CAST(Carga.CAR_DATA_ENCERRAMENTO_CARGA AS DATE) >= '{filtrosPesquisa.DataEncerramentoInicial.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataEncerramentoFinal.HasValue)
                where.Append($" and CAST(Carga.CAR_DATA_ENCERRAMENTO_CARGA AS DATE) <= '{filtrosPesquisa.DataEncerramentoFinal.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataConfirmacaoDocumentosInicial.HasValue)
                where.Append($" and CAST(Carga.CAR_DATA_INICIO_CONFIRMACAO_DOCUMENTOS_FISCAIS AS DATE) >= '{filtrosPesquisa.DataConfirmacaoDocumentosInicial.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataConfirmacaoDocumentosFinal.HasValue)
                where.Append($" and CAST(Carga.CAR_DATA_INICIO_CONFIRMACAO_DOCUMENTOS_FISCAIS AS DATE) <= '{filtrosPesquisa.DataConfirmacaoDocumentosFinal.Value.ToString(pattern)}'");

            if (filtrosPesquisa.TipoCTe.Count > 0)
            {
                where.Append(" and Carga.CAR_CODIGO in (");
                where.Append("         select DISTINCT _cargaCTe.CAR_CODIGO ");
                where.Append("           from T_CARGA_CTE _cargaCTe");
                where.Append("           inner join T_CTE _cte on _cte.CON_CODIGO = _cargaCTe.CON_CODIGO ");
                where.Append($"         where _cte.CON_TIPO_CTE in ({string.Join(", ", filtrosPesquisa.TipoCTe.Select(o => o.ToString("D")))}) ");
                where.Append("     ) ");
            }

            if (filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                where.Append(" and (TipoOperacao.TOP_OCULTAR_CARGAS_COM_ESSE_TIPO_OPERACAO_PORTAL_TRANSPORTADOR IS NULL OR TipoOperacao.TOP_OCULTAR_CARGAS_COM_ESSE_TIPO_OPERACAO_PORTAL_TRANSPORTADOR = 0) ");

                SetarJoinsTipoOperacao(joins);
            }

            if (filtrosPesquisa.TipoPropostaMultimodal.Count > 0)
            {
                where.Append(" and Carga.CAR_CODIGO in (");
                where.Append("         select DISTINCT _cargaPedido.CAR_CODIGO ");
                where.Append("           from T_CARGA_PEDIDO _cargaPedido");
                where.Append("           inner join T_CARGA_CTE _cargaCte on _cargaCte.CAR_CODIGO = _cargaPedido.CAR_CODIGO ");
                where.Append($"         where _cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL in ({string.Join(", ", filtrosPesquisa.TipoPropostaMultimodal.Select(o => o.ToString("D")))}) ");
                where.Append("     ) ");
            }

            if (filtrosPesquisa.TipoServicoMultimodal.Count > 0)
            {
                where.Append(" and Carga.CAR_CODIGO in (");
                where.Append("         select DISTINCT _cargaPedido.CAR_CODIGO ");
                where.Append("           from T_CARGA_PEDIDO _cargaPedido");
                where.Append("           inner join T_CARGA_CTE _cargaCte on _cargaCte.CAR_CODIGO = _cargaPedido.CAR_CODIGO ");
                where.Append($"         where _cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL in ({string.Join(", ", filtrosPesquisa.TipoServicoMultimodal.Select(o => o.ToString("D")))}) ");
                where.Append("     ) ");
            }

            if (filtrosPesquisa.VeioPorImportacao == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Nao)
            {
                where.Append(" and Carga.CAR_CODIGO in (");
                where.Append("      select DISTINCT CargaCTe.CAR_CODIGO from T_CARGA_CTE CargaCTe");
                where.Append("      inner join T_CARGA Carga on Carga.CAR_CODIGO=CargaCTe.CAR_CODIGO");
                where.Append("      inner join T_CTE CTe on CTe.CON_CODIGO=CargaCTe.CON_CODIGO");
                where.Append("      where (CTe.CON_CTE_IMPORTADO_EMBARCADOR = 0 or CTe.CON_CTE_IMPORTADO_EMBARCADOR IS NULL))");
            }
            else if (filtrosPesquisa.VeioPorImportacao == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
            {
                where.Append(" and Carga.CAR_CODIGO in (");
                where.Append("      select DISTINCT CargaCTe.CAR_CODIGO from T_CARGA_CTE CargaCTe");
                where.Append("      inner join T_CARGA Carga on Carga.CAR_CODIGO=CargaCTe.CAR_CODIGO");
                where.Append("      inner join T_CTE CTe on CTe.CON_CODIGO=CargaCTe.CON_CODIGO");
                where.Append("      where CTe.CON_CTE_IMPORTADO_EMBARCADOR = 1)");
            }

            if (filtrosPesquisa.SomenteCTeSubstituido)
            {
                where.Append(" and Carga.CAR_CODIGO in (");
                where.Append("      select DISTINCT CargaCTe.CAR_CODIGO from T_CARGA_CTE CargaCTe");
                where.Append("      inner join T_CARGA Carga on Carga.CAR_CODIGO=CargaCTe.CAR_CODIGO");
                where.Append("      inner join T_CTE CTe on CTe.CON_CODIGO=CargaCTe.CON_CODIGO");
                where.Append("      where exists (select _cte.CON_CODIGO from T_CTE _cte where _cte.CON_CHAVE_CTE_SUB_COMP = CTe.CON_CHAVECTE))");
            }

            if (filtrosPesquisa.Problemas == ProblemasCarga.ProblemasAverbacao)
                where.Append(" and Carga.CAR_PROBLEMA_AVERBACAO_CTE = 1");
            else if (filtrosPesquisa.Problemas == ProblemasCarga.ProblemasCTe)
                where.Append(" and Carga.CAR_PROBLEMA_CTE = 1");
            else if (filtrosPesquisa.Problemas == ProblemasCarga.ProblemasMDFe)
                where.Append(" and Carga.CAR_PROBLEMA_MDFE = 1");
            else if (filtrosPesquisa.Problemas == ProblemasCarga.ProblemasValePedagio)
                where.Append(" and Carga.CAR_PROBLEMA_INTEGRACAO_VALE_PEDAGIO = 1");

            if (filtrosPesquisa.NaoComparecimento == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Nao)
            {
                where.Append($" and (JanelaCarregamento.CJC_NAO_COMPARECIDO = {(int)TipoNaoComparecimento.Compareceu} or JanelaCarregamento.CJC_NAO_COMPARECIDO IS NULL) ");
                SetarJoinsJanelaCarregamento(joins);
            }
            else if (filtrosPesquisa.NaoComparecimento == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
            {
                where.Append($" and JanelaCarregamento.CJC_NAO_COMPARECIDO in ({(int)TipoNaoComparecimento.NaoCompareceu}, {(int)TipoNaoComparecimento.NaoCompareceuComFalha}) ");
                SetarJoinsJanelaCarregamento(joins);
            }

            if (filtrosPesquisa.CargaTrechos == Dominio.Enumeradores.CargaTrechos.Nao)
            {
                where.Append($" and StageAgrupamento.CAR_CODIGO = Carga.CAR_CODIGO ");
                SetarJoinsCargaTrechos(joins);
            }
            else if (filtrosPesquisa.CargaTrechos == Dominio.Enumeradores.CargaTrechos.ApenasTrechos)
            {
                where.Append($" and not exists (select StageAgrupamento.STG_CODIGO from T_STAGE_AGRUPAMENTO StageAgrupamento where StageAgrupamento.CAR_CODIGO = Carga.CAR_CODIGO)");
            }

            if (filtrosPesquisa.CargaTrechoSumarizada.HasValue)
            {
                where.Append($"and DadosSumarizados.CDS_CARGA_TRECHO = {filtrosPesquisa.CargaTrechoSumarizada.Value.ToString("D")}");
                SetarJoinsCargaDadosSumarizados(joins);
            }

            if (filtrosPesquisa.DataCarregamentoInicio.HasValue)
                where.Append($" AND CAST(Carga.CAR_DATA_CARREGAMENTO AS DATE) >= '{filtrosPesquisa.DataCarregamentoInicio.Value.ToString(pattern)}'");
            if (filtrosPesquisa.DataCarregamentoFim.HasValue)
                where.Append($" AND CAST(Carga.CAR_DATA_CARREGAMENTO AS DATE) <= '{filtrosPesquisa.DataCarregamentoFim.Value.ToString(pattern)}'");

            if (filtrosPesquisa.CargasSemPacote)
            {
                where.Append($"AND ");
                where.Append("( ");
                where.Append("    select COUNT(1) from T_CARGA_PEDIDO_PACOTE CargaPedidoPacote ");
                where.Append("    inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = CargaPedidoPacote.CPE_CODIGO ");
                where.Append("    where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                where.Append(") <= 0 ");
            }

            if (filtrosPesquisa.CentroDeCustoViagemCodigo > 0)
            {
                where.Append(" AND EXISTS (");
                where.Append("    SELECT 1 from T_CARGA_PEDIDO CargaPedido");
                where.Append("    LEFT JOIN T_PEDIDO Pedido ON CargaPedido.PED_CODIGO = Pedido.PED_CODIGO");
                where.Append("    WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO");
                where.Append($"   AND Pedido.CCV_CODIGO = {filtrosPesquisa.CentroDeCustoViagemCodigo})");
            }

            if (filtrosPesquisa.FlagCargaPercentualExecucao)
            {
                SetarJoinsPercentualExecucao(joins);
            }

            if (joins.Contains(" CargaMotoristaDocumento "))
                where.Append(" and (CargaMotoristaDocumento.CMD_PERCENTUAL_EXECUCAO < 100.00 OR CargaMotoristaDocumento.CMD_PERCENTUAL_EXECUCAO IS NULL)");

            if (filtrosPesquisa.NumeroDtNatura > 0)
            {
                if (!joins.Contains(" left join T_CARGA_INTEGRACAO_NATURA IntegracaoNatura on IntegracaoNatura.CAR_CODIGO = Carga.CAR_CODIGO "))
                    SetarJoinsNumeroDtNatura(joins);
                where.Append($" and IntegracaoNatura.IDT_CODIGO = {filtrosPesquisa.NumeroDtNatura} ");
            }

            #region Filtros do Pedido
            StringBuilder wherePedido = new StringBuilder();

            if (filtrosPesquisa.CodigosOrigem?.Count > 0)
                wherePedido.Append($" and _cargapedido.LOC_CODIGO_ORIGEM in ({string.Join(", ", filtrosPesquisa.CodigosOrigem)}) ");

            if (filtrosPesquisa.CodigosDestino?.Count > 0)
                wherePedido.Append($" and _cargapedido.LOC_CODIGO_DESTINO in ({string.Join(", ", filtrosPesquisa.CodigosDestino)}) ");

            if (filtrosPesquisa.CpfCnpjDestinatarios.Count > 0)
                wherePedido.Append($" and _pedido.CLI_CODIGO in ({string.Join(", ", filtrosPesquisa.CpfCnpjDestinatarios)}) ");

            if (filtrosPesquisa.CpfCnpjRemetente > 0d)
                wherePedido.Append($" and _pedido.CLI_CODIGO_REMETENTE = {filtrosPesquisa.CpfCnpjRemetente} ");

            if (filtrosPesquisa.CpfCnpjExpedidores?.Count > 0d)
                wherePedido.Append($" and _cargapedido.CLI_CODIGO_EXPEDIDOR in ({string.Join(", ", filtrosPesquisa.CpfCnpjExpedidores)}) ");

            if (filtrosPesquisa.CpfCnpjRecebedores?.Count > 0d)
                wherePedido.Append($" and _cargapedido.CLI_CODIGO_RECEBEDOR in ({string.Join(", ", filtrosPesquisa.CpfCnpjRecebedores)}) ");

            if (filtrosPesquisa.TipoLocalPrestacao == TipoLocalPrestacao.intraMunicipal)
                wherePedido.Append(" and _pedido.LOC_CODIGO_ORIGEM = _pedido.LOC_CODIGO_DESTINO ");
            else if (filtrosPesquisa.TipoLocalPrestacao == TipoLocalPrestacao.interMunicipal)
                wherePedido.Append(" and _pedido.LOC_CODIGO_ORIGEM <> _pedido.LOC_CODIGO_DESTINO ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Pedido))
                wherePedido.Append($" and _pedido.PED_NUMERO_PEDIDO_EMBARCADOR = '{filtrosPesquisa.Pedido}' ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.DeliveryTerm))
                wherePedido.Append($" and _pedido.PED_DELIVERY_TERM = '{filtrosPesquisa.DeliveryTerm}' ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.IdAutorizacao))
                wherePedido.Append($" and _pedido.PED_ID_AUTORIZACAO = '{filtrosPesquisa.IdAutorizacao}' ");

            if (filtrosPesquisa.DataInclusaoBookingInicial.HasValue)
                wherePedido.Append($" and CAST(_pedido.PED_DATA_INCLUSAO_BOOKING AS DATE) >= '{filtrosPesquisa.DataInclusaoBookingInicial.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataInclusaoBookingLimite.HasValue)
                wherePedido.Append($" and CAST(_pedido.PED_DATA_INCLUSAO_BOOKING AS DATE) <= '{filtrosPesquisa.DataInclusaoBookingLimite.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataInclusaoPCPInicial.HasValue)
                wherePedido.Append($" and CAST(_pedido.PED_DATA_INCLUSAO_PCP AS DATE) >= '{filtrosPesquisa.DataInclusaoPCPInicial.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataInclusaoPCPLimite.HasValue)
                wherePedido.Append($" and CAST(_pedido.PED_DATA_INCLUSAO_PCP AS DATE) <= '{filtrosPesquisa.DataInclusaoPCPLimite.Value.ToString(pattern)}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
                wherePedido.Append($" and _pedido.PED_NUMERO_BOOKING = '{filtrosPesquisa.NumeroBooking}' ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOS))
                wherePedido.Append($" and _pedido.PED_NUMERO_OS = '{filtrosPesquisa.NumeroOS}' ");

            if (filtrosPesquisa.PortoOrigem > 0)
                wherePedido.Append($" and _pedido.POT_CODIGO_ORIGEM = {filtrosPesquisa.PortoOrigem} ");

            if (filtrosPesquisa.PortoDestino > 0)
                wherePedido.Append($" and _pedido.POT_CODIGO_DESTINO = {filtrosPesquisa.PortoDestino} ");

            if (filtrosPesquisa.Container > 0)
                wherePedido.Append($" and _pedido.CTR_CODIGO = {filtrosPesquisa.Container} ");

            if (filtrosPesquisa.SomenteComReserva)
                wherePedido.Append(" and isnull(_pedido.PED_RESERVA, '') <> '' ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoCliente))
                wherePedido.Append($" and _pedido.PED_CODIGO_PEDIDO_CLIENTE = '{filtrosPesquisa.NumeroPedidoCliente}' ");

            if (filtrosPesquisa.CodigosCentroResultado.Count > 0)
                wherePedido.Append($" and _pedido.CRE_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosCentroResultado)}) ");

            if (wherePedido.Length > 0)
            {
                where.Append(" and Carga.CAR_CODIGO in (");
                where.Append("         select _cargapedido.CAR_CODIGO ");
                where.Append("           from T_CARGA_PEDIDO _cargapedido");
                where.Append("           join T_PEDIDO _pedido on _pedido.PED_CODIGO = _cargapedido.PED_CODIGO ");
                where.Append($"         where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO {wherePedido.ToString().Trim()} ");
                where.Append("     ) ");
            }

            if (filtrosPesquisa.CanalEntrega > 0)
            {
                where.Append(" AND EXISTS ( ");
                where.Append("         SELECT 1 ");
                where.Append("           FROM T_CANAL_ENTREGA CanalEntrega ");
                where.Append("           JOIN T_STAGE Stage ON Stage.CNE_CODIGO = CanalEntrega.CNE_CODIGO ");
                where.Append("           JOIN T_PEDIDO_STAGE PedidoStage ON PedidoStage.STA_CODIGO = Stage.STA_CODIGO ");
                where.Append("           JOIN T_CARGA_PEDIDO CargaPedido ON CargaPedido.PED_CODIGO = PedidoStage.PED_CODIGO ");
                where.Append("          WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                where.Append($"           AND CanalEntrega.CNE_CODIGO = {filtrosPesquisa.CanalEntrega} ");
                where.Append("     ) ");
            }

            if (filtrosPesquisa.DataFaturamentoInicial.HasValue)
                where.Append($" AND CAST(Carga.CAR_DATA_CARREGAMENTO AS DATE) >= '{filtrosPesquisa.DataFaturamentoInicial.Value.ToString(pattern)}'");
            if (filtrosPesquisa.DataFaturamentoFinal.HasValue)
                where.Append($" AND CAST(Carga.CAR_DATA_CARREGAMENTO AS DATE) <= '{filtrosPesquisa.DataFaturamentoFinal.Value.ToString(pattern)}'");

            if (filtrosPesquisa.TipoOSConvertido.Count > 0)
            {
                where.Append(" AND EXISTS (");
                where.Append("    SELECT 1 from T_CARGA_PEDIDO CargaPedido");
                where.Append("    LEFT JOIN T_PEDIDO Pedido ON CargaPedido.PED_CODIGO = Pedido.PED_CODIGO");
                where.Append("    WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO");
                where.Append($"   AND Pedido.PED_TIPO_OS_CONVERTIDO in ({string.Join(", ", filtrosPesquisa.TipoOSConvertido.Select(o => o.ToString("d")))}))");
            }

            if (filtrosPesquisa.TipoOS.Count > 0)
            {
                where.Append(" AND EXISTS (");
                where.Append("    SELECT 1 from T_CARGA_PEDIDO CargaPedido");
                where.Append("    LEFT JOIN T_PEDIDO Pedido ON CargaPedido.PED_CODIGO = Pedido.PED_CODIGO");
                where.Append("    WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO");
                where.Append($"   AND Pedido.PED_TIPO_OS in ({string.Join(", ", filtrosPesquisa.TipoOS.Select(o => o.ToString("d")))}))");
            }

            if (filtrosPesquisa.CodigosProvedores.Count > 0)
            {
                where.Append(" AND EXISTS (");
                where.Append("    SELECT 1 from T_CARGA_PEDIDO CargaPedido");
                where.Append("    LEFT JOIN T_PEDIDO Pedido ON CargaPedido.PED_CODIGO = Pedido.PED_CODIGO");
                where.Append("    LEFT JOIN T_CLIENTE ProvedorOS ON Pedido.CLI_CODIGO_PROVEDOR_OS = ProvedorOS.CLI_CGCCPF");
                where.Append("    WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO");
                where.Append($"   AND ProvedorOS.CLI_CGCCPF in ({string.Join(", ", filtrosPesquisa.CodigosProvedores.Select(o => o.ToString()))}))");
            }

            if (filtrosPesquisa.DirecionamentoCustoExtra.Count > 0)
            {
                where.Append($" and ConfiguracaoTipoOperacaoCarga.CCG_DIRECIONAMENTO_CUSTO_EXTRA in ({string.Join(", ", filtrosPesquisa.DirecionamentoCustoExtra.Select(o => o.ToString("d")))})");

                SetarJoinConfiguracaoTipoOperacaoCarga(joins);
            }

            if (filtrosPesquisa.StatusCustoExtra.Count > 0)
            {
                where.Append($" and Carga.CAR_STATUS_CUSTO_EXTRA in ({string.Join(", ", filtrosPesquisa.StatusCustoExtra.Select(o => o.ToString("d")))})");
            }

            if (filtrosPesquisa.CodigosGrupoProduto?.Count() > 0)
            {
                where.Append(" AND EXISTS ( ");
                where.Append("         SELECT 1 ");
                where.Append("           FROM T_CARGA_PEDIDO CargaPedido ");
                where.Append("           JOIN T_CARGA_PEDIDO_PRODUTO CargaPedidoProduto on CargaPedido.CPE_CODIGO = CargaPedidoProduto.CPE_CODIGO");
                where.Append("           JOIN T_PRODUTO_EMBARCADOR ProdutoEmbarcador on CargaPedidoProduto.PRO_CODIGO = ProdutoEmbarcador.PRO_CODIGO ");
                where.Append("           WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                where.Append($"          AND ProdutoEmbarcador.GRP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosGrupoProduto)}) ");
                where.Append("     ) ");
            }

            #endregion Filtros do Pedido
        }

        #endregion
    }
}
