/// <autosync enabled="true" />
/// <reference path="../../../../wwwroot/js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../../wwwroot/js/bootstrap/bootstrap.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.blockui.js" />
/// <reference path="../../../../wwwroot/js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../wwwroot/js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.globalize.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../Global/SignalR/SignalR.js" />
/// <reference path="../../../Configuracao/EmissaoCTe/EmissaoCTe.js" />
/// <reference path="../../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../DadosEmissao/Configuracao.js" />
/// <reference path="../DadosEmissao/DadosEmissao.js" />
/// <reference path="../DadosEmissao/Geral.js" />
/// <reference path="../DadosEmissao/Lacre.js" />
/// <reference path="../DadosEmissao/LocaisPrestacao.js" />
/// <reference path="../DadosEmissao/Observacao.js" />
/// <reference path="../DadosEmissao/Passagem.js" />
/// <reference path="../DadosEmissao/Percurso.js" />
/// <reference path="../DadosEmissao/Rota.js" />
/// <reference path="../DadosEmissao/Seguro.js" />
/// <reference path="Motorista.js" />
/// <reference path="Tipo.js" />
/// <reference path="Transportador.js" />
/// <reference path="LiberacaoSemIntegracaoGR.js" />
/// <reference path="CargaDadosZonaTransporte.js" />
/// <reference path="../Documentos/CTe.js" />
/// <reference path="../Documentos/MDFe.js" />
/// <reference path="../Documentos/NFS.js" />
/// <reference path="../Documentos/PreCTe.js" />
/// <reference path="../DocumentosEmissao/CargaPedidoDocumentoCTe.js" />
/// <reference path="../DocumentosEmissao/ConsultaReceita.js" />
/// <reference path="../DocumentosEmissao/CTe.js" />
/// <reference path="../DocumentosEmissao/Documentos.js" />
/// <reference path="../DocumentosEmissao/DropZone.js" />
/// <reference path="../DocumentosEmissao/EtapaDocumentos.js" />
/// <reference path="../DocumentosEmissao/NotaFiscal.js" />
/// <reference path="../Frete/Complemento.js" />
/// <reference path="../Frete/Componente.js" />
/// <reference path="../Frete/EtapaFrete.js" />
/// <reference path="../Frete/Frete.js" />
/// <reference path="../Frete/SemTabela.js" />
/// <reference path="../Frete/TabelaCliente.js" />
/// <reference path="../Frete/TabelaComissao.js" />
/// <reference path="../Frete/TabelaRota.js" />
/// <reference path="../Frete/TabelaSubContratacao.js" />
/// <reference path="../Frete/TabelaTerceiros.js" />
/// <reference path="../Impressao/Impressao.js" />
/// <reference path="../Integracao/Integracao.js" />
/// <reference path="../Integracao/IntegracaoCarga.js" />
/// <reference path="../Integracao/IntegracaoCTe.js" />
/// <reference path="../Integracao/IntegracaoEDI.js" />
/// <reference path="../Terceiro/ContratoFrete.js" />
/// <reference path="../DadosCarga/SignalR.js" />
/// <reference path="../DadosCarga/Carga.js" />
/// <reference path="../DadosCarga/DataCarregamento.js" />
/// <reference path="../DadosCarga/Leilao.js" />
/// <reference path="../DadosCarga/Operador.js" />
/// <reference path="../../../Consultas/Tranportador.js" />
/// <reference path="../../../Consultas/Localidade.js" />
/// <reference path="../../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/Motorista.js" />
/// <reference path="../../../Consultas/Veiculo.js" />
/// <reference path="../../../Consultas/GrupoPessoa.js" />
/// <reference path="../../../Consultas/TipoOperacao.js" />
/// <reference path="../../../Consultas/Filial.js" />
/// <reference path="../../../Consultas/Cliente.js" />
/// <reference path="../../../Consultas/Usuario.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/RotaFrete.js" />
/// <reference path="../../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../Enumeradores/EnumTipoFreteEscolhido.js" />
/// <reference path="../../../Enumeradores/EnumTipoOperacaoEmissao.js" />
/// <reference path="../../../Enumeradores/EnumMotivoPendenciaFrete.js" />
/// <reference path="../../../Enumeradores/EnumTipoContratacaoCarga.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoContratoFrete.js" />
/// <reference path="../../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="../../../Enumeradores/EnumTipoEmissaoCTeParticipantes.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoDadosFrete.js" />

async function EtapaInicioTMSClick(e, sender) {
    _cargaAtual = e;
    ocultarTodasAbas(e);
    Global.ResetarAba(e.EtapaInicioTMS.idGrid);
    var data = { Carga: e.Codigo.val() };

    await executarReST("CargaDadosTransporte/VerificarDadosTransporteCarga", data, function (arg) {
        if (arg.Success) {
            if (arg.Data.TabelaQuePossuiOperacao) {
                e.TipoOperacao.required = true;
                e.TipoOperacao.text(Localization.Resources.Cargas.Carga.TipoDeOperacao.getRequiredFieldDescription());
            } else {
                if (arg.Data.TabelaQueNaoPossuiOperacao) {
                    e.TipoOperacao.required = false;
                    e.TipoOperacao.text(Localization.Resources.Cargas.Carga.TipoDeOperacao.getFieldDescription());
                }
            }

            if (arg.Data.PortalMultiTransportador) {
                if (arg.Data.PossuiIntegracao && arg.Data.PermitirTransportadorReenviarIntegracoesComProblemasOpenTech)
                    LoadCargaDadosTransporteIntegracao(e);
            } else {
                if (arg.Data.PossuiIntegracao)
                    LoadCargaDadosTransporteIntegracao(e);
            }

            if (arg.Data.PossuiObservacao)
                loadCargaDadosEmissaoObservacaoDadosCarga(e);

            if (arg.Data.PermitirVisualizarOrdernarZonasTransporte)
                loadCargaDadosZonaTransporte(e);

            if (arg.Data.PossuiIntegracaoMotorista)
                loadIntegracoesGRMotorista(e);

            if (arg.Data.PossuiIntegracaoVeiculo)
                loadIntegracoesGRVeiculo(e);

            if (arg.Data.PermitirVisualizarTipoCarregamento)
                e.TipoCarregamento.visible(true);
            else
                e.TipoCarregamento.visible(false);

            if (arg.Data.PermitirVisualizarCentroResultado)
                e.CentroResultado.visible(true);
            else
                e.CentroResultado.visible(false);

            e.AdicionarAjudantes.visible(arg.Data.PermitirInformarAjudantesNaCarga);
            e.ExigePlacaTracao.val(arg.Data.ExigePlacaTracao);
            e.NaoPermitirAlterarMotoristaAposAverbacaoContainer.val(arg.Data.NaoPermitirAlterarMotoristaAposAverbacaoContainer);

            if (_CONFIGURACAO_TMS.NaoPermitirAlterarDadosCargaQuandoTiverIntegracaoIntegrada && arg.Data.PossuiIntegracaoIntegrada)
                DesabilitarCamposCargaQuandoTiverIntegracaoIntegrada(e);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor)
        loadLacresCargaTransportador(e);

    if (_cargaAtual.ExigirInformarContainer.val() && _cargaAtual.TransferenciaContainer.val() && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS)
        loadCargaContainer(_cargaAtual);

    loadEtapasPreCheckin(_cargaAtual, false);
    LoadCargaDadosEmissaoCargaLacre(_cargaAtual);
    loadDadosIntegracaoCargaPreChekinIntegracao(_cargaAtual, false);
    verificarCamposHabilitados(e);
}

function verificarCamposHabilitados(e) {
    var habilitarCampo = e.EtapaInicioTMS.enable();

    e.ImprimirDadosTransporte.visible(true);
    e.SalvarDadosTransporte.enable(habilitarCampo);
    e.SalvarDadosTransporteESolicitarNFes.enable(habilitarCampo);
    e.Veiculo.enable(habilitarCampo);
    e.Reboque.enable(habilitarCampo);
    e.SegundoReboque.enable(habilitarCampo);
    e.TerceiroReboque.enable(habilitarCampo);
    e.ModeloVeicularCarga.enable(habilitarCampo);
    e.Empresa.enable(habilitarCampo);
    e.TipoCarga.enable(habilitarCampo);
    e.TipoContainer.enable(habilitarCampo);
    e.TipoOperacao.enable(habilitarCampo);
    e.PedidoViagemNavio.enable(habilitarCampo);
    e.PortoOrigem.enable(habilitarCampo);
    e.PortoDestino.enable(habilitarCampo);
    e.TerminalOrigem.enable(habilitarCampo);
    e.TerminalDestino.enable(habilitarCampo);
    e.ApoliceSeguro.enable(habilitarCampo);
    e.InicioCarregamento.enable(habilitarCampo);
    e.TerminoCarregamento.enable(habilitarCampo);
    e.NumeroPager.enable(habilitarCampo);
    e.ObservacaoTransportador.enable(habilitarCampo);
    e.DataBaseCRT.enable(habilitarCampo);
    e.QuantidadePaletes.enable(habilitarCampo);
    e.TipoCarregamento.enable(habilitarCampo);
    e.CentroResultado.enable(habilitarCampo);
    e.Navio.enable(habilitarCampo);
    e.Balsa.enable(habilitarCampo);
    e.TransportadorSubcontratado.enable(habilitarCampo);

    if (e.EstaEmParqueamento.val()) {
        e.Veiculo.enable(true);
        if (e.ExigePlacaTracao.val()) {
            e.Reboque.enable(false);
        }

        e.SalvarDadosTransporte.enable(true);
    }

    if (e.Empresa.codEntity() > 0 && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_PermiteAlterarTransportador, _PermissoesPersonalizadasCarga))
        e.Empresa.enable(false);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS)
        e.ImprimirDadosTransporte.visible(false);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        e.DisponibilizarParaTransportadorCarga.visible(false);
        e.InformarRetiradaContainer.visible(false);
        e.ModeloVeicularCarga.enable(false);
        e.TipoCarga.enable(false);
        e.TipoOperacao.enable(false);
        e.Empresa.enable(false);
    }
    else {
        e.DisponibilizarParaTransportadorCarga.visible(habilitarCampo && (e.RejeitadaPeloTransportador.val() || _CONFIGURACAO_TMS.PermitirDisponibilizarCargaParaTransportador));
        e.InformarRetiradaContainer.visible(e.ExigirInformarRetiradaContainer.val() && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador);

        if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_SalvarDadosTransporte, _PermissoesPersonalizadasCarga)) {
            e.SalvarDadosTransporte.enable(false);
            e.SalvarDadosTransporteESolicitarNFes.enable(false);
        }
    }

    if (Boolean(e.OrdemEmbarque.val()) && e.CargaDePreCarga.val())
        e.ModeloVeicularCarga.enable(false);

    if (e.DisponibilizarParaTransportadorCarga.visible())
        e.Empresa.enable(true);

    if (e.MotoristaRecusouCarga.val()) {
        e.ConfirmacaoMotorista.visible(true);
        e.AdicionarMotoristas.visible(true);
        e.ConfirmacaoMotorista.enable(false);

        e.ConfirmacaoMotorista.text("Motorista não aceitou a carga");

    } else if (e.HorarioLimiteConfirmacaoMotorista.val()) {
        e.ConfirmacaoMotorista.visible(true);
        e.AdicionarMotoristas.visible(true);
        e.HorarioLimiteConfirmacaoMotorista.visible(true);

        setTimeout(function () {
            $("#" + e.HorarioLimiteConfirmacaoMotorista.id)
                .countdown(moment(e.HorarioLimiteConfirmacaoMotorista.val(), "DD/MM/YYYY HH:mm:ss").format("YYYY/MM/DD HH:mm:ss"), { elapse: true, precision: 1000 })
                .on('update.countdown', function (event) {
                    if (event.elapsed) {
                        $(this).text(" [Tempo limite esgotado]")
                        e.ConfirmacaoMotorista.enable(false);
                    }
                    else {
                        if (event.offset.totalDays > 0)
                            $(this).text(" [" + event.strftime('%-Dd %H:%M:%S') + "]");
                        else
                            $(this).text(" [" + event.strftime('%H:%M:%S') + "]");
                    }
                });
        }, 300);
    }

    if (e.PossuiOperacaoContainer.val() && e.NaoPermitirAlterarMotoristaAposAverbacaoContainer.val()) {
        e.AdicionarMotoristas.basicTable.DesabilitarOpcoes();
        e.EtapaInicioTMS.enable(false);
    }
}

function DesabilitarCamposCargaQuandoTiverIntegracaoIntegrada(e) {
    if (e.SituacaoCarga != EnumSituacoesCarga.Cancelada &&
        e.SituacaoCarga != EnumSituacoesCarga.EmCancelamento &&
        e.SituacaoCarga != EnumSituacoesCarga.RejeicaoCancelamento &&
        e.SituacaoCarga != EnumSituacoesCarga.Anulada &&
        e.SituacaoCarga != EnumSituacoesCarga.Nova) {

        e.SalvarDadosTransporte.enable(false);
        e.SalvarDadosTransporteESolicitarNFes.enable(false);
        e.Veiculo.enable(false);
        e.Reboque.enable(false);
        e.SegundoReboque.enable(false);
        e.TerceiroReboque.enable(false);
        e.ModeloVeicularCarga.enable(false);
        e.Empresa.enable(false);
        e.TipoCarga.enable(false);
        e.TipoContainer.enable(false);
        e.TipoOperacao.enable(false);
        e.PedidoViagemNavio.enable(false);
        e.PortoOrigem.enable(false);
        e.PortoDestino.enable(false);
        e.TerminalOrigem.enable(false);
        e.TerminalDestino.enable(false);
        e.ApoliceSeguro.enable(false);
        e.InicioCarregamento.enable(false);
        e.TerminoCarregamento.enable(false);
        e.NumeroPager.enable(false);
        e.ObservacaoTransportador.enable(false);
        e.DataBaseCRT.enable(false);
        e.QuantidadePaletes.enable(false);
        e.TipoCarregamento.enable(false);
        e.CentroResultado.enable(false);
        e.Navio.enable(false);
        e.Balsa.enable(false);
        e.TransportadorSubcontratado.enable(false);

        e.ConfirmacaoMotorista.enable(false);
        e.EtapaInicioTMS.enable(false)
        e.AdicionarMotoristas.basicTable.DesabilitarOpcoes();
    }
}

function SalvarDadosTransporteClick(e) {
    const isMultiEmbarcador = _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador;
    const validoTipoCarga = ValidarCampoObrigatorioEntity(e.TipoCarga);
    const validoEmpresa = (_cargaAtual.CargaTipoConsolidacao.val() || ValidarCampoObrigatorioEntity(e.Empresa));
    const validoVeiculo = (e.EstaEmParqueamento.val() || _cargaAtual.CargaTipoConsolidacao.val() || ValidarCampoObrigatorioEntity(e.Veiculo));
    const validoReboque = (_cargaAtual.CargaTipoConsolidacao.val()) || (e.ExigeConfirmacaoTracao.val() ? ValidarCampoObrigatorioEntity(e.Reboque) : true);
    const validoTipoOperacao = ValidarCampoObrigatorioEntity(e.TipoOperacao);

    const validoModeloVeicular = (_cargaAtual.CargaTipoConsolidacao.val() || (e.LiberadaComCargaSemPlanejamento.val() ? true : ValidarCampoObrigatorioEntity(e.ModeloVeicularCarga)));
    const validoJustificativaAutorizacaoCarga = (_cargaAtual.JustificativaAutorizacaoCarga.val() || ValidarCampoObrigatorioEntity(e.JustificativaAutorizacaoCarga));
    const validoInicioCarregamento = ValidarCampoObrigatorioMap(e.InicioCarregamento);
    const validoTerminoCarregamento = ValidarCampoObrigatorioMap(e.TerminoCarregamento);
    const validoMotoristas = (_cargaAtual.CargaTipoConsolidacao.val() || e.AdicionarMotoristas.basicTable.BuscarRegistros().length > 0);
    const validoNavioViagemDirecao = (!_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal || ValidarCampoObrigatorioEntity(e.PedidoViagemNavio));
    const dadosValidos = validoVeiculo && validoReboque && validoModeloVeicular && validoTipoCarga && validoJustificativaAutorizacaoCarga;
    const liberadaComCargaSemPlanejamento = e.LiberadaComCargaSemPlanejamento.val() && validoReboque && validoTipoCarga && validoJustificativaAutorizacaoCarga;
    const dadosMinimosAvancoEtapa = isMultiEmbarcador || dadosValidos || liberadaComCargaSemPlanejamento;
    const avancarEtapa = (dadosValidos && validoMotoristas) || liberadaComCargaSemPlanejamento;
    const validoBalsa = ValidarCampoObrigatorioEntity(e.Balsa);
    const validoNavio = ValidarCampoObrigatorioEntity(e.Balsa);
    const validoTransportadorSubcontratado = ValidarCampoObrigatorioEntity(e.TransportadorSubcontratado);
    const validoLocalCarregamento = ValidarCampoObrigatorioEntity(e.LocalCarregamento);

    if (e.Setor.required && (e.Setor.codEntity() == 0 || e.Setor.val() == "")) {
        e.Setor.requiredClass("form-control is-invalid");
        e.Setor.codEntity(0);
        e.Setor.val("");
        tudoCerto = false;
    } else {
        e.Setor.requiredClass("form-control");
    }


    $("#" + e.VeiculosInvalidosCarga.idContainer).hide();

    if (dadosMinimosAvancoEtapa && validoEmpresa && validoTipoOperacao && validoNavioViagemDirecao && validoInicioCarregamento && validoTerminoCarregamento && validoBalsa && validoNavio && validoTransportadorSubcontratado && validoLocalCarregamento) {
        if (isMultiEmbarcador || validoMotoristas || e.NaoExigeVeiculoParaEmissao.val() || _CONFIGURACAO_TMS.UtilizaEmissaoMultimodal || liberadaComCargaSemPlanejamento) {
            if (isMultiEmbarcador) {
                LimparStatusObrigatorioEntity(e.TipoCarga);
                LimparStatusObrigatorioEntity(e.Veiculo);
                LimparStatusObrigatorioEntity(e.Reboque);
                LimparStatusObrigatorioEntity(e.ModeloVeicularCarga);
            }            
            var data = {
                codigo: e.Codigo.val(),
                TipoCarga: e.TipoCarga.codEntity(),
                Empresa: e.Empresa.codEntity(),
                Veiculo: e.Veiculo.codEntity(),
                Reboque: e.Reboque.codEntity(),
                ReboqueDescricao: e.Reboque.val(),
                SegundoReboque: e.SegundoReboque.codEntity(),
                TerceiroReboque: e.TerceiroReboque.codEntity(),
                TipoOperacao: (e.TipoOperacao.val() == "" ? 0 : e.TipoOperacao.codEntity()),
                Motoristas: JSON.stringify(e.AdicionarMotoristas.basicTable.BuscarRegistros()),
                Ajudantes: JSON.stringify(e.AdicionarAjudantes.basicTable.BuscarCodigosRegistros()),
                ModeloVeicular: e.ModeloVeicularCarga.codEntity(),
                JustificativaAutorizacaoCarga: e.JustificativaAutorizacaoCarga.codEntity(),
                LiberadaComCargaSemPlanejamento: e.LiberadaComCargaSemPlanejamento.val(),
                PedidoViagemNavio: e.PedidoViagemNavio.codEntity(),
                PortoOrigem: e.PortoOrigem.codEntity(),
                PortoDestino: e.PortoDestino.codEntity(),
                TerminalOrigem: e.TerminalOrigem.codEntity(),
                TerminalDestino: e.TerminalDestino.codEntity(),
                ApoliceSeguro: JSON.stringify(recursiveMultiplesEntities(e.ApoliceSeguro)),
                InicioCarregamento: e.InicioCarregamento.val(),
                TerminoCarregamento: e.TerminoCarregamento.val(),
                LiberarComProblemaIntegracaoGrMotoristaVeiculo: e.LiberadoComProblemaIntegracaoGrMotoristaVeiculo.val(),
                SalvarDadosTransporteSemSolicitarNFes: e.SalvarDadosTransporteSemSolicitarNFes.val(),
                ProtocoloIntegracaoGR: e.ProtocoloIntegracaoGR.val(),
                NumeroPager: e.NumeroPager.val(),
                LiberadaComLicencaInvalida: e.LiberadaComLicencaInvalida.val(),
                ObservacaoTransportador: e.ObservacaoTransportador.val(),
                TipoContainer: e.TipoContainer.codEntity(),
                SalvarDadosTransporteCargaSemVinculoPreCarga: false,
                DataBaseCRT: e.DataBaseCRT.val(),
                QuantidadePaletes: e.QuantidadePaletes.val(),
                CargaEstaEmParqueamento: e.EstaEmParqueamento.val,
                TipoCarregamento: e.TipoCarregamento.codEntity(),
                CentroResultado: e.CentroResultado.codEntity(),
                Navio: e.Navio.codEntity(),
                Balsa: e.Balsa.codEntity(),
                TransportadorSubcontratado: e.TransportadorSubcontratado.codEntity(),
                PlacasCarregamento: _gridPlacaCarregamento != null ? JSON.stringify(_gridPlacaCarregamento.ObterMultiplosSelecionados()) : null,
                LocalCarregamento: e.LocalCarregamento.codEntity(),
                Setor: e.Setor.codEntity(),                
                ObservacaoCarga: e.ObservacaoCarga.val(),
                NumeroContainerVeiculo: e.NumeroContainerVeiculo.val(),
                Container: e.Container.codEntity()
            };
            if (_cargaAtual.TipoOperacao.validarSeCargaPossuiVinculoComPreCarga) {
                if (_cargaAtual.TipoOperacao.validarSeCargaPossuiVinculoComPreCarga) {
                    executarReST("Carga/ValidarCargaVinculadaPreCarga", { NumeroCarga: _cargaAtual.CodigoCargaEmbarcador.val() }, function (retorno) {
                        if (retorno.Success) {
                            if (retorno.Data) {
                                if (!retorno.Data.PossuiPreCargaComNumeroCarga)
                                    exibirConfirmacao("Confirmação", "Não existe uma pré carga vinculada a essa carga. Deseja prosseguir mesmo assim?",
                                        function () {
                                            data.SalvarDadosTransporteCargaSemVinculoPreCarga = true;
                                            salvarDadosTransporteCarga(e, data, avancarEtapa);
                                        });
                            } else {
                                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
                            }
                        }
                        else {
                            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
                        }
                    });
                }
            }
            else
                salvarDadosTransporteCarga(e, data, avancarEtapa);
        }
        else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.Carga.InformeMotorista, Localization.Resources.Cargas.Carga.ObrigatorioInformarAoMenosUmMotorista);
            _cargaAtual.LiberadaComCargaSemPlanejamento.val(false);
        }
    }
    else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Cargas.Carga.PorFavorInformeOsCamposObrigatoriosAntesDeContinuar);
        _cargaAtual.LiberadaComCargaSemPlanejamento.val(false);
    }
}

function ImprimirPlanoViagemTransporteClick(e, sender) {
    var data = { Codigo: e.Codigo.val(), Carregamento: false, Carga: true };
    executarDownload("Pedido/GerarRelatorioPlanoViagem", data);
}

function ImprimirDadosTransporteClick(e, sender) {
    var data = { Codigo: e.Codigo.val(), Carregamento: false, Carga: true };

    executarReST("Pedido/GerarRelatorio", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                BuscarProcessamentosPendentes();
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.AguardeQueSeuRelatorioEstaSendoGerado);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
        }
    });
}

function ImprimirFichaMotoristaClick(e, sender) {
    var data = { Codigo: e.Codigo.val() };
    executarDownload("Carga/GerarFichaMotorista", data);
}

function ImprimirOrdemColetaClick(e, sender) {
    var data = { Codigo: e.Codigo.val() };
    executarDownload("Carga/GerarOrdemColeta", data);
}

//*******MÉTODOS*******

function EtapaInicioTMSProblema(e) {
    $("#" + e.EtapaInicioTMS.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaInicioTMS.idTab + " .step").attr("class", "step red");
    e.EtapaInicioTMS.enable(true);
}

function EtapaInicioTMSLiberada(e) {
    $("#" + e.EtapaInicioTMS.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaInicioTMS.idTab + " .step").attr("class", "step yellow");

    e.EtapaInicioTMS.enable(true);
}

function EtapaInicioTMSAprovada(e) {
    $("#" + e.EtapaInicioTMS.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaInicioTMS.idTab + " .step").attr("class", "step green");

    e.EtapaInicioTMS.enable(true);
}

function EtapaInicioTMSAguardando(e) {
    $("#" + e.EtapaInicioTMS.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaInicioTMS.idTab + " .step").attr("class", "step yellow");

    e.EtapaInicioTMS.enable(true);
}

function EtapaInicioTMSAlerta(e) {
    $("#" + e.EtapaInicioTMS.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaInicioTMS.idTab + " .step").attr("class", "step orange");

    e.EtapaInicioTMS.enable(true);
}

function EtapaInicioTMSEdicaoDesabilitada(e) {
    e.EtapaInicioTMS.enable(false);
}

function confirmarMotoristaCarga(e) {
    exibirConfirmacao("Confirmação", "Realmente deseja confirmar o motorista para a carga?", function () {
        var codigo = e.Codigo.val();
        if (codigo > 0) {
            executarReST("Carga/ConfirmarMotoristaCarga", { Carga: codigo }, function (arg) {
                if (arg.Success) {
                    if (arg.Data !== false) {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                        e.ConfirmacaoMotorista.visible(false);
                        e.AdicionarMotoristas.visible(true);
                        e.HorarioLimiteConfirmacaoMotorista.visible(false);

                        SalvarDadosTransporteClick(e);
                    } else {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
                }
            });
        }
    });
}

function confirmarAjudanteCarga(e) {
    exibirConfirmacao("Confirmação", "Realmente deseja confirmar o ajudante para a carga?", function () {
        var codigo = e.Codigo.val();
        if (codigo > 0) {
            executarReST("Carga/ConfirmarMotoristaCarga", { Carga: codigo }, function (arg) {
                if (arg.Success) {
                    if (arg.Data !== false) {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                        e.ConfirmacaoAjudante.visible(false);
                        e.AdicionarAjudantes.visible(true);
                        e.HorarioLimiteConfirmacaoAJudante.visible(false);

                        SalvarDadosTransporteClick(e);
                    } else {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
                }
            });
        }
    });
}

function salvarDadosTransporteCarga(e, data, avancarEtapa) {
    var etapaAnterior = e.SituacaoCarga.val();
    executarReST("CargaDadosTransporte/IntegracaoDigitamcomComProblema", data, (arg) => {
        if (!arg.Data.PossuiIntegracaoComProblema) {
            executarReST("CargaDadosTransporte/SalvarDadosTransporteCarga", data, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data != false) {

                        if (retorno.Data.AlterouVeiculoEmParqueamento) {
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.SalvoComSucesso);
                            return;
                        }
                        var dadosFrete = retorno.Data.DadosFrete;
                        var dadosContainer = retorno.Data.DadosContainer;
                        var infoMotoristas = "";

                        $.each(e.AdicionarMotoristas.basicTable.BuscarRegistros(), function (i, motorista) {
                            if (i > 0) {
                                infoMotoristas += ", ";
                            }
                            infoMotoristas += motorista.Nome;
                        });
                        e.Motorista.motoristas(infoMotoristas);

                        let permite = retorno.Data.PermiteImportarDocumentosManualmente;
                        e.PermiteImportarDocumentosManualmente.val(permite);

                        e.CodModeloVeicularCargaOriginal.val = e.ModeloVeicularCarga.codEntity();
                        e.CodTipoCargaOriginal.val = e.TipoCarga.codEntity();

                        $("#" + e.DivCarga.id + "_ribbonCargaNova").hide();

                        if (string.IsNullOrWhiteSpace(dadosFrete.MensagemProblemaIntegracaoGrMotoristaVeiculo) && string.IsNullOrWhiteSpace(dadosFrete.MensagemLicencaInvalida))
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);

                        if (avancarEtapa)
                            e.SituacaoCarga.val(dadosFrete.situacaoCarga);

                        if (dadosFrete.situacao == EnumSituacaoRetornoDadosFrete.FreteValido) {
                            e.ValorFrete.val(Globalize.format(dadosFrete.valorFreteAPagar, "n2"));
                            e.InfoTipoFreteEscolhido.visible(false);
                        }

                        e.Veiculo.placas(dadosFrete.Placas);
                        e.Reboque.val(data.ReboqueDescricao);

                        if (!string.IsNullOrWhiteSpace(dadosFrete.MensagemProblemaIntegracaoGrMotoristaVeiculo)) {
                            $("#" + e.LiberarComProblemaIntegracaoGrMotoristaVeiculo.idText).text(dadosFrete.MensagemProblemaIntegracaoGrMotoristaVeiculo);
                            $("#" + e.LiberarComProblemaIntegracaoGrMotoristaVeiculo.idContainer).show();
                        }
                        else
                            $("#" + e.LiberarComProblemaIntegracaoGrMotoristaVeiculo.idContainer).hide();

                        if (!string.IsNullOrWhiteSpace(dadosFrete.MensagemLicencaInvalida)) {
                            $("#" + e.ProblemaLicencaInvalida.idText).text(dadosFrete.MensagemLicencaInvalida);
                            $("#" + e.ProblemaLicencaInvalida.idContainer).show();

                            if ($("#" + _cargaAtual.LiberarComProblemaIntegracaoGrMotoristaVeiculo.idContainer).css("display") != "none")
                                $("#" + e.ProblemaLicencaInvalida.idContainer).css("margin-top", "20px");
                            else
                                $("#" + e.ProblemaLicencaInvalida.idContainer).css("margin-top", "60px");
                        }
                        else
                            $("#" + e.ProblemaLicencaInvalida.idContainer).hide();

                        if (dadosFrete.ProblemaIntegracaoGrMotoristaVeiculo) {
                            e.SalvarDadosTransporteESolicitarNFes.visible(false);
                            e.LiberarComProblemaIntegracaoGrMotoristaVeiculo.visible(dadosFrete.PermitirLiberarComProblemaIntegracaoGrMotoristaVeiculo);
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, dadosFrete.MensagemProblemaIntegracaoGrMotoristaVeiculo);
                        }
                        else if (dadosFrete.LicencaInvalida) {
                            e.SalvarDadosTransporteESolicitarNFes.visible(dadosFrete.PermitirAvancarEtapaComLicencaInvalida);
                            if (!dadosFrete.PermitirAvancarEtapaComLicencaInvalida) {
                                e.LiberarComLicencaInvalida.visible(dadosFrete.LiberarComLicencaInvalida);
                            }
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, dadosFrete.MensagemLicencaInvalida);
                            if (etapaAnterior === EnumSituacoesCarga.AgNFe)
                                retornarEtapaNFe(e);
                        }
                        else if (e.SalvarDadosTransporteSemSolicitarNFes.val()) {
                            e.LiberarComProblemaIntegracaoGrMotoristaVeiculo.visible(false);
                        }
                        else if (avancarEtapa) {
                            e.SalvarDadosTransporteESolicitarNFes.visible(false);
                            e.SituacaoCarga.val(EnumSituacoesCarga.AgNFe);
                            EtapaNotaFiscalAguardando(e);
                        }

                        if (dadosFrete.PossuiIntegracao === true)
                            LoadCargaDadosTransporteIntegracao(e);

                        if (isNaN(e.CodigoContainerReboque.val()) && (dadosContainer.CodigoContainerReboque > 0))
                            e.ContainerReboqueAnexo.enviarArquivosAnexados(dadosContainer.CodigoContainerReboque);

                        if (isNaN(e.CodigoContainerSegundoReboque.val()) && (dadosContainer.CodigoContainerSegundoReboque > 0))
                            e.ContainerSegundoReboqueAnexo.enviarArquivosAnexados(dadosContainer.CodigoContainerSegundoReboque);

                        if (isNaN(e.CodigoContainerVeiculo.val()) && (dadosContainer.CodigoContainerVeiculo > 0))
                            e.ContainerVeiculoAnexo.enviarArquivosAnexados(dadosContainer.CodigoContainerVeiculo);

                        enviarArquivosAnexadosLiberacaoSemIntegracaoGRAnexo(e.Codigo.val());
                        fecharModalLiberacaoSemIntegracaoGR();
                    }
                    else
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
                }
                else {
                    if (retorno.Data != null && retorno.Data.VeiculosInvalidosCarga) {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
                        $("#" + e.VeiculosInvalidosCarga.idText).text(retorno.Data.Mensagem);
                        $("#" + e.VeiculosInvalidosCarga.idContainer).show();
                    }
                    else
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
                }
            });
            return;
        }

        exibirConfirmacao("Advertencia", "Realmente deseja seguir com a coleta sem o vale pedágio confirmado?", () => {
            data.ProblemaValePedagio = true;
            executarReST("CargaDadosTransporte/SalvarDadosTransporteCarga", data, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data != false) {

                        if (retorno.Data.AlterouVeiculoEmParqueamento) {
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.SalvoComSucesso);
                            return;
                        }
                        var dadosFrete = retorno.Data.DadosFrete;
                        var dadosContainer = retorno.Data.DadosContainer;
                        var infoMotoristas = "";

                        $.each(e.AdicionarMotoristas.basicTable.BuscarRegistros(), function (i, motorista) {
                            if (i > 0) {
                                infoMotoristas += ", ";
                            }
                            infoMotoristas += motorista.Nome;
                        });
                        e.Motorista.motoristas(infoMotoristas);

                        let permite = retorno.Data.PermiteImportarDocumentosManualmente;
                        e.PermiteImportarDocumentosManualmente.val(permite);

                        e.CodModeloVeicularCargaOriginal.val = e.ModeloVeicularCarga.codEntity();
                        e.CodTipoCargaOriginal.val = e.TipoCarga.codEntity();

                        $("#" + e.DivCarga.id + "_ribbonCargaNova").hide();

                        if (string.IsNullOrWhiteSpace(dadosFrete.MensagemProblemaIntegracaoGrMotoristaVeiculo) && string.IsNullOrWhiteSpace(dadosFrete.MensagemLicencaInvalida))
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);

                        if (avancarEtapa)
                            e.SituacaoCarga.val(dadosFrete.situacaoCarga);

                        if (dadosFrete.situacao == EnumSituacaoRetornoDadosFrete.FreteValido) {
                            e.ValorFrete.val(Globalize.format(dadosFrete.valorFreteAPagar, "n2"));
                            e.InfoTipoFreteEscolhido.visible(false);
                        }

                        e.Veiculo.placas(dadosFrete.Placas);
                        e.Reboque.val(data.ReboqueDescricao);

                        if (!string.IsNullOrWhiteSpace(dadosFrete.MensagemProblemaIntegracaoGrMotoristaVeiculo)) {
                            $("#" + e.LiberarComProblemaIntegracaoGrMotoristaVeiculo.idText).text(dadosFrete.MensagemProblemaIntegracaoGrMotoristaVeiculo);
                            $("#" + e.LiberarComProblemaIntegracaoGrMotoristaVeiculo.idContainer).show();
                        }
                        else
                            $("#" + e.LiberarComProblemaIntegracaoGrMotoristaVeiculo.idContainer).hide();

                        if (!string.IsNullOrWhiteSpace(dadosFrete.MensagemLicencaInvalida)) {
                            $("#" + e.ProblemaLicencaInvalida.idText).text(dadosFrete.MensagemLicencaInvalida);
                            $("#" + e.ProblemaLicencaInvalida.idContainer).show();

                            if ($("#" + _cargaAtual.LiberarComProblemaIntegracaoGrMotoristaVeiculo.idContainer).css("display") != "none")
                                $("#" + e.ProblemaLicencaInvalida.idContainer).css("margin-top", "20px");
                            else
                                $("#" + e.ProblemaLicencaInvalida.idContainer).css("margin-top", "60px");
                        }
                        else
                            $("#" + e.ProblemaLicencaInvalida.idContainer).hide();

                        if (dadosFrete.ProblemaIntegracaoGrMotoristaVeiculo) {
                            e.SalvarDadosTransporteESolicitarNFes.visible(false);
                            e.LiberarComProblemaIntegracaoGrMotoristaVeiculo.visible(dadosFrete.PermitirLiberarComProblemaIntegracaoGrMotoristaVeiculo);
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, dadosFrete.MensagemProblemaIntegracaoGrMotoristaVeiculo);
                        }
                        else if (dadosFrete.LicencaInvalida) {
                            e.SalvarDadosTransporteESolicitarNFes.visible(dadosFrete.PermitirAvancarEtapaComLicencaInvalida);
                            if (!dadosFrete.PermitirAvancarEtapaComLicencaInvalida) {
                                e.LiberarComLicencaInvalida.visible(dadosFrete.LiberarComLicencaInvalida);
                            }
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, dadosFrete.MensagemLicencaInvalida);
                            if (etapaAnterior === EnumSituacoesCarga.AgNFe)
                                retornarEtapaNFe(e);
                        }
                        else if (e.SalvarDadosTransporteSemSolicitarNFes.val()) {
                            e.LiberarComProblemaIntegracaoGrMotoristaVeiculo.visible(false);
                        }
                        else if (avancarEtapa) {
                            e.SalvarDadosTransporteESolicitarNFes.visible(false);
                            e.SituacaoCarga.val(EnumSituacoesCarga.AgNFe);
                            EtapaNotaFiscalAguardando(e);
                        }

                        if (dadosFrete.PossuiIntegracao === true)
                            LoadCargaDadosTransporteIntegracao(e);

                        if (isNaN(e.CodigoContainerReboque.val()) && (dadosContainer.CodigoContainerReboque > 0))
                            e.ContainerReboqueAnexo.enviarArquivosAnexados(dadosContainer.CodigoContainerReboque);

                        if (isNaN(e.CodigoContainerSegundoReboque.val()) && (dadosContainer.CodigoContainerSegundoReboque > 0))
                            e.ContainerSegundoReboqueAnexo.enviarArquivosAnexados(dadosContainer.CodigoContainerSegundoReboque);

                        if (isNaN(e.CodigoContainerVeiculo.val()) && (dadosContainer.CodigoContainerVeiculo > 0))
                            e.ContainerVeiculoAnexo.enviarArquivosAnexados(dadosContainer.CodigoContainerVeiculo);

                        enviarArquivosAnexadosLiberacaoSemIntegracaoGRAnexo(e.Codigo.val());
                        fecharModalLiberacaoSemIntegracaoGR();
                    }
                    else
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
                }
                else {
                    if (retorno.Data != null && retorno.Data.VeiculosInvalidosCarga) {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
                        $("#" + e.VeiculosInvalidosCarga.idText).text(retorno.Data.Mensagem);
                        $("#" + e.VeiculosInvalidosCarga.idContainer).show();
                    }
                    else
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
                }
            });
        })
    });


}
