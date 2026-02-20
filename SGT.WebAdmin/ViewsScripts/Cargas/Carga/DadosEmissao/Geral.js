/// <autosync enabled="true" />
/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../Global/SignalR/SignalR.js" />
/// <reference path="../../../Configuracao/EmissaoCTe/EmissaoCTe.js" />
/// <reference path="../../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="Configuracao.js" />
/// <reference path="DadosEmissao.js" />
/// <reference path="Lacre.js" />
/// <reference path="LocaisPrestacao.js" />
/// <reference path="Observacao.js" />
/// <reference path="Passagem.js" />
/// <reference path="Percurso.js" />
/// <reference path="Rota.js" />
/// <reference path="Seguro.js" />
/// <reference path="../DadosTransporte/DadosTransporte.js" />
/// <reference path="../DadosTransporte/Motorista.js" />
/// <reference path="../DadosTransporte/Tipo.js" />
/// <reference path="../DadosTransporte/Transportador.js" />
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
/// <reference path="../../../Consultas/CentroResultado.js" />
/// <reference path="../../../Consultas/PlanoConta.js" />
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
/// <reference path="../../../Enumeradores/EnumTipoTomador.js" />

//*******MAPEAMENTO*******

var _cargaDadosEmissaoGeral;

var CargaDadosEmissaoGeral = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Pedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idTab: guid(), enable: ko.observable(true) });

    this.UsarTipoPagamentoNF = PropertyEntity({ text: Localization.Resources.Cargas.Carga.UtilizarToipDoPagamentoDasNotas, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.IncluirICMSBC = PropertyEntity({ text: Localization.Resources.Cargas.Carga.IncluirValorDoICMSNaBaseDeCalculo, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.ViagemJaOcorreu = PropertyEntity({ text: Localization.Resources.Cargas.Carga.EssaViagemJaOcorreuNaoVaiEmitirMdfe, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.NaoComprarValePedagio = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NaoComprarValePedagio, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.GerarMDFeManualmente = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.EmitirMdfeManualmente), val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.TipoPagamento = PropertyEntity({ val: ko.observable(EnumTipoPagamento.Pago), eventChange: tipoPagamentoChange, options: EnumTipoPagamento.obterOpcoes(), def: EnumTipoPagamento.Pago, text: Localization.Resources.Cargas.Carga.TipoDePagamento.getFieldDescription(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, visibleFade: ko.observable(false), text: Localization.Resources.Cargas.Carga.Tomador.getRequiredFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.NumeroPedido = PropertyEntity({ type: types.map, text: Localization.Resources.Cargas.Carga.NumeroPedidoNoEmbarcador.getFieldDescription(), visible: ko.observable(true) });
    this.AplicarGeralEmTodosPedidos = PropertyEntity({ val: ko.observable(true), text: Localization.Resources.Cargas.Carga.AoAtualizarEsssaConfiguracaoAplicarTodosOsPedidosDaCarga, def: true, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.CobrarOutroDocumento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.CobrarValorEmOutroDocumentoFiscal.getFieldDescription(), enable: ko.observable(true), required: false, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Fronteira = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, visibleFade: ko.observable(false), text: Localization.Resources.Cargas.Carga.Fronteira.getFieldDescription(), enable: ko.observable(true), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Fronteiras = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.Carga.Fronteira.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true), required: false });
    this.NumeroCargaVincularPreCarga = PropertyEntity({ type: types.map, text: Localization.Resources.Cargas.Carga.NumeroCargaVinculadaPreCarga.getFieldDescription(), visible: ko.observable(false), maxlength: 50 });
    this.NumeroOrdem = PropertyEntity({ type: types.map, text: Localization.Resources.Cargas.Carga.Ordem.getFieldDescription(), visible: ko.observable(false), maxlength: 30 });
    this.ElementoPEP = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Cargas.Carga.ElementoPEP.getFieldDescription(), visible: ko.observable(false), enable: ko.observable(true), val: ko.observable(""), def: "", maxlength: 24 });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.CentroDeResultado.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.ContaContabil = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.Carga.ContaContabil.getRequiredFieldDescription()), idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true), required: false });

    this.TipoPropostaMultimodal = PropertyEntity({ val: ko.observable(EnumTipoPropostaMultimodal.Nenhum), options: EnumTipoPropostaMultimodal.obterOpcoes(), text: Localization.Resources.Cargas.Carga.TipoDaProposta.getFieldDescription(), def: EnumTipoPropostaMultimodal.Nenhum, enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(false) });
    this.TipoServicoMultimodal = PropertyEntity({ val: ko.observable(EnumTipoServicoMultimodal.Nenhum), options: (!_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal ? EnumTipoServicoMultimodal.obterOpcoesSVMTerceiro() : EnumTipoServicoMultimodal.obterOpcoes()), text: Localization.Resources.Cargas.Carga.TipoDoServico.getFieldDescription(), def: EnumTipoServicoMultimodal.Nenhum, enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(false) });
    this.ModalPropostaMultimodal = PropertyEntity({ val: ko.observable(EnumModalPropostaMultimodal.Nenhum), options: EnumModalPropostaMultimodal.obterOpcoes(), text: Localization.Resources.Cargas.Carga.ModalDaProposta.getFieldDescription(), def: EnumModalPropostaMultimodal.Nenhum, enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(false) });
    this.TipoCobrancaMultimodal = PropertyEntity({ val: ko.observable(EnumTipoCobrancaMultimodal.Nenhum), options: (!_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal ? EnumTipoCobrancaMultimodal.obterOpcoesMultimodal() : EnumTipoCobrancaMultimodal.obterOpcoes()), text: Localization.Resources.Cargas.Carga.TipoDaCobranca.getFieldDescription(), def: EnumTipoCobrancaMultimodal.Nenhum, enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(false) });

    this.AutorizarModalidadePagamento = PropertyEntity({ eventClick: autorizarModalidadePagamentoClick, type: types.event, text: Localization.Resources.Cargas.Carga.AutorizarModalidadeDePagamento, visible: ko.observable(false), enable: ko.observable(true) });
    this.MensagemAutorizarModalidadePagamento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ExistemDiferentesFormasDePagamentosInformadosParaAsNotasDestaFormaSistemaIraSepararEmissaoConformeModalidadeDePagamentoDaNotaParaIssoNecessarioAutorizarAntesDeEmitirOsDocumentos, visible: ko.observable(false) });

    this.AutorizarPeso = PropertyEntity({ eventClick: AutorizarPesoCargaClick, type: types.event, text: Localization.Resources.Cargas.Carga.AutorizarQuantidade, visible: ko.observable(false), enable: ko.observable(true) });
    this.MensagemAutorizarPeso = PropertyEntity({ text: ko.observable(""), visible: ko.observable(false) });

    this.AutorizarManutencao = PropertyEntity({ eventClick: AutorizarManutencoesPendentesVeiculosCargaClick, type: types.event, text: Localization.Resources.Cargas.Carga.AutorizarAsManutencoes, visible: ko.observable(false), enable: ko.observable(true) });
    this.MensagemAutorizarManutencao = PropertyEntity({ text: ko.observable(""), visible: ko.observable(false) });

    this.AutorizarValorMaximoPendentePagamento = PropertyEntity({ eventClick: AutorizarValorMaximoPendentePagamentoCargaClick, type: types.event, text: Localization.Resources.Cargas.Carga.AutorizarValorPendente, visible: ko.observable(false), enable: ko.observable(true) });

    this.MensagemAutorizarValorMaximoPendentePagamento = PropertyEntity({ text: ko.observable(""), visible: ko.observable(false) });

    this.Atualizar = PropertyEntity({ eventClick: alterarDadosEmissaoGeralClick, type: types.event, text: Localization.Resources.Cargas.Carga.AtualizarDadosGerais, visible: ko.observable(true), enable: ko.observable(true) });

    this.RelatorioBoletimViagem = PropertyEntity({ eventClick: RelatorioBoletimViagemEmissaoGeral, type: types.event, text: Localization.Resources.Cargas.Carga.BoletimDeViagem, visible: ko.observable(false) });
    this.RelatorioPedidoPacote = PropertyEntity({ eventClick: RelatorioPedidoPacoteClick, type: types.event, text: Localization.Resources.Cargas.Carga.RelatorioPedidoPacote, visible: ko.observable(false) });
    this.FilialCargaAgrupada = PropertyEntity({ val: ko.observable(0), options: ko.observable(retornarListaFiliais()), text: "Filial da Carga Agrupada Para Vale Pedágio:", enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(false) });

    this.ElementoPEP.val.subscribe(VerificarCamposElementoPEPCentroResultado);
    this.CentroResultado.val.subscribe(VerificarCamposElementoPEPCentroResultado);
    this.AplicarGeralEmTodosPedidos.val.subscribe(aplicarGeralEmTodosPedidosClick);

    this.FilialCargaAgrupada.options.subscribe((opt) => {
        if (!opt.some(o => o.value == this.FilialCargaAgrupada.val())) {
            this.FilialCargaAgrupada.val(0);
        }
    })
};

//*******EVENTOS*******

function loadCargaDadosEmissaoGeral() {
    _cargaDadosEmissaoGeral = new CargaDadosEmissaoGeral();
    KoBindings(_cargaDadosEmissaoGeral, "tabGeral_" + _cargaAtual.DadosEmissaoFrete.id);
    $("#tabGeral_" + _cargaAtual.DadosEmissaoFrete.id + "_li").show();

    _cargaDadosEmissaoGeral.Pedido.enable(_cargaAtual.EtapaFreteEmbarcador.enable());
    _cargaDadosEmissaoGeral.TipoPagamento.enable(_cargaAtual.EtapaFreteEmbarcador.enable());
    _cargaDadosEmissaoGeral.Atualizar.enable(_cargaAtual.EtapaFreteEmbarcador.enable());
    _cargaDadosEmissaoGeral.AutorizarModalidadePagamento.enable(_cargaAtual.EtapaFreteEmbarcador.enable());
    _cargaDadosEmissaoGeral.FilialCargaAgrupada.enable(_cargaAtual.EtapaFreteEmbarcador.enable());

    new BuscarCentroResultado(_cargaDadosEmissaoGeral.CentroResultado);
    new BuscarPlanoConta(_cargaDadosEmissaoGeral.ContaContabil);
    new BuscarClientes(_cargaDadosEmissaoGeral.Tomador, retornoTomador);
    new BuscarClientes(_cargaDadosEmissaoGeral.Fronteiras, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _cargaDadosEmissaoGeral.GerarMDFeManualmente.visible(true);
        _cargaDadosEmissaoGeral.GerarMDFeManualmente.val(_cargaAtual.NaoGerarMDFe.val());

        _cargaDadosEmissaoGeral.UsarTipoPagamentoNF.visible(false);
        _cargaDadosEmissaoGeral.IncluirICMSBC.visible(false);
        _cargaDadosEmissaoGeral.ViagemJaOcorreu.visible(false);
        _cargaDadosEmissaoGeral.TipoPagamento.visible(false);
        _cargaDadosEmissaoGeral.Tomador.visible(false);
        _cargaDadosEmissaoGeral.NumeroPedido.visible(false);
        _cargaDadosEmissaoGeral.AplicarGeralEmTodosPedidos.visible(false);
        _cargaDadosEmissaoGeral.CobrarOutroDocumento.visible(false);
        _cargaDadosEmissaoGeral.ModeloDocumentoFiscal.visible(false);
        _cargaDadosEmissaoGeral.AutorizarModalidadePagamento.visible(false);
        _cargaDadosEmissaoGeral.MensagemAutorizarModalidadePagamento.visible(false);
        _cargaDadosEmissaoGeral.AutorizarPeso.visible(false);
        _cargaDadosEmissaoGeral.MensagemAutorizarPeso.visible(false);
        _cargaDadosEmissaoGeral.AutorizarManutencao.visible(false);
        _cargaDadosEmissaoGeral.MensagemAutorizarManutencao.visible(false);
        _cargaDadosEmissaoGeral.AutorizarValorMaximoPendentePagamento.visible(false);
        _cargaDadosEmissaoGeral.MensagemAutorizarValorMaximoPendentePagamento.visible(false);
        _cargaDadosEmissaoGeral.Fronteiras.visible(true);

        _cargaDadosEmissaoGeral.RelatorioBoletimViagem.visible(_CONFIGURACAO_TMS.HabilitarRelatorioBoletimViagem);
        _cargaDadosEmissaoGeral.RelatorioPedidoPacote.visible(_CONFIGURACAO_TMS.ExisteIntegracaoLoggi && _cargaAtual.ExigeNotaFiscalParaCalcularFrete.val());

        new BuscarFronteiras(_cargaDadosEmissaoGeral.Fronteira);

        //Quando está com Pendencia Emissão e na etapa do Emissão deixa habilitado as fronteiras
        if ((_cargaAtual.EtapaMDFe.enable() && _cargaAtual.PossuiPendencia) || (_cargaAtual.EtapaFreteEmbarcador.enable())) //_cargaAtual.EtapaFreteEmbarcador.enable()
        {
            _cargaDadosEmissaoGeral.Fronteiras.enable(true);
            _cargaDadosEmissaoGeral.Atualizar.enable(true);
        }
        else {
            _cargaDadosEmissaoGeral.Fronteiras.enable(false);
        }

        _cargaDadosEmissaoGeral.TipoCobrancaMultimodal.visible(true);
    } else {
        _cargaDadosEmissaoGeral.ViagemJaOcorreu.visible(false);
        _cargaDadosEmissaoGeral.GerarMDFeManualmente.text(Localization.Resources.Cargas.Carga.NaoGerarMdfeGerarMdfeManualmente);
        _cargaDadosEmissaoGeral.Fronteiras.visible(false);

        $("#" + _cargaDadosEmissaoGeral.UsarTipoPagamentoNF.id).click(usarTipoPagamentoNFClick);
        $("#" + _cargaDadosEmissaoGeral.CobrarOutroDocumento.id).click(CobrerOutroDocumentoClick);

        //$("#" + _cargaDadosEmissaoGeral.AplicarGeralEmTodosPedidos.id).click(aplicarGeralEmTodosPedidosClick);

        if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_PermiteAlterarInclusaoICMS, _PermissoesPersonalizadasCarga))
            _cargaDadosEmissaoGeral.IncluirICMSBC.visible(false);

        if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AlterarDadosPedido, _PermissoesPersonalizadasCarga))
            _cargaDadosEmissaoGeral.Atualizar.enable(false);

        if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AutorizarModalidadePagamentoNota, _PermissoesPersonalizadasCarga))
            _cargaDadosEmissaoGeral.AutorizarModalidadePagamento.enable(false);

        if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AutorizarPesoCarga, _PermissoesPersonalizadasCarga))
            _cargaDadosEmissaoGeral.AutorizarPeso.enable(false);

        if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AutorizarManutencaoPendenteVeiculo, _PermissoesPersonalizadasCarga))
            _cargaDadosEmissaoGeral.AutorizarManutencao.enable(false);

        new BuscarModeloDocumentoFiscal(_cargaDadosEmissaoGeral.ModeloDocumentoFiscal, null, null, true);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
            _cargaDadosEmissaoGeral.AplicarGeralEmTodosPedidos.val(false);
            _cargaDadosEmissaoGeral.AplicarGeralEmTodosPedidos.def = false;
            _cargaDadosEmissaoGeral.TipoServicoMultimodal.visible(true);
            _cargaDadosEmissaoGeral.TipoCobrancaMultimodal.visible(true);
        }
    }
    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
        _cargaDadosEmissaoGeral.TipoCobrancaMultimodal.visible(true);
        _cargaDadosEmissaoGeral.ModalPropostaMultimodal.visible(true);
        _cargaDadosEmissaoGeral.TipoPropostaMultimodal.visible(true);
    }

    if (_cargaAtual.CargaDestinadaCTeComplementar.val())
        _cargaDadosEmissaoGeral.Tomador.visible(false);
    else
        _cargaDadosEmissaoGeral.Tomador.visible(true);
}

function autorizarModalidadePagamentoClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealementeAutorizarSeguroDaCarga, function () {
        Salvar(e, "DadosEmissao/AutorizarModalidadePagamentoNota", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ModalidadeDePagamentoAutorizadaComSucesso);
                    _cargaDadosEmissaoGeral.AutorizarModalidadePagamento.visible(false);
                    _cargaDadosEmissaoGeral.MensagemAutorizarModalidadePagamento.visible(false);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function AutorizarPesoCargaClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteAutorizarQuantidadeDaCarga, function () {
        Salvar(e, "DadosEmissao/AutorizarPesoCarga", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.QuantidadeAutorizadaComSucesso);
                    _cargaDadosEmissaoGeral.AutorizarPeso.visible(false);
                    _cargaDadosEmissaoGeral.MensagemAutorizarPeso.visible(false);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function AutorizarManutencoesPendentesVeiculosCargaClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmentePermitirViagemComManutencoesPendentesParaOsVeiculos, function () {
        Salvar(e, "DadosEmissao/AutorizarManutencaoPendenteVeiculoCarga", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.AutorizacaoRealizadaComSucesso);
                    _cargaDadosEmissaoGeral.AutorizarManutencao.visible(false);
                    _cargaDadosEmissaoGeral.MensagemAutorizarManutencao.visible(false);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function AutorizarValorMaximoPendentePagamentoCargaClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmentePermitirViagemMesmoComValorMaximoPendenteParaPagamentoDesteTomadorExcedido, function () {
        Salvar(e, "DadosEmissao/AutorizarValorMaximoPendentePagamentoCarga", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.AutorizacaoRealizadaComSucesso);
                    _cargaDadosEmissaoGeral.AutorizarValorMaximoPendentePagamento.visible(false);
                    _cargaDadosEmissaoGeral.MensagemAutorizarValorMaximoPendentePagamento.visible(false);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function aplicarGeralEmTodosPedidosClick() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        if (!_cargaDadosEmissaoGeral.AplicarGeralEmTodosPedidos.val()) {
            carregarDadosPedido(0, () => {
                $("#" + _cargaDadosEmissaoGeral.Pedido.idTab).slideDown();
                _cargaDadosEmissaoGeral.NumeroPedido.visible(true);
            });
        } else {
            $("#" + _cargaDadosEmissaoGeral.Pedido.idTab).slideUp();
            _cargaDadosEmissaoGeral.NumeroPedido.visible(false);
        }
    }

    VerificarCamposAplicarGeralTodosPedidos();
}

function visualizarAtualizacaoNumeroPedido() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        if (!_cargaDadosEmissaoGeral.AplicarGeralEmTodosPedidos.val()) {
            _cargaDadosEmissaoGeral.NumeroPedido.visible(true);
        } else {
            _cargaDadosEmissaoGeral.NumeroPedido.visible(false);
        }
    }
}

function CobrerOutroDocumentoClick() {
    if (_cargaDadosEmissaoGeral.CobrarOutroDocumento.val()) {
        _cargaDadosEmissaoGeral.ModeloDocumentoFiscal.required = true;
        _cargaDadosEmissaoGeral.ModeloDocumentoFiscal.enable(true);
    } else {
        _cargaDadosEmissaoGeral.ModeloDocumentoFiscal.enable(false);
        _cargaDadosEmissaoGeral.ModeloDocumentoFiscal.required = false;
        _cargaDadosEmissaoGeral.ModeloDocumentoFiscal.val("");
        _cargaDadosEmissaoGeral.ModeloDocumentoFiscal.codEntity(0);
    }
}

function usarTipoPagamentoNFClick() {
    if (_cargaDadosEmissaoGeral.UsarTipoPagamentoNF.val())
        _cargaDadosEmissaoGeral.TipoPagamento.enable(false);
    else
        _cargaDadosEmissaoGeral.TipoPagamento.enable(true);
}

function tipoPagamentoChange(e, sender) {
    if (e.TipoPagamento.val() == EnumTipoPagamento.Outros) {
        e.Tomador.visibleFade(true);
        e.Tomador.required = true;
    } else {
        e.Tomador.visibleFade(false);
        e.Tomador.required = false;
    }

    mudarKnoutPessoaParaRota();
}

var _indiceGlobalPedidoDadosEmissao;
function alterarDadosEmissaoGeralClick(e, sender) {
    if (!ValidarCamposObrigatorios(_cargaDadosEmissaoGeral)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
        return;
    }

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteAlterarOsDadosParaEmissao, function () {
        Salvar(e, "DadosEmissao/AtualizarDadosEmissao", function (arg) {
            if (arg.Success) {
                if (arg.Data != false) {
                    var cargaPedido = arg.Data.CargaPedido;
                    preecherDadosEmissao(cargaPedido);
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.DadosParaEmissaoAlteradosComSucesso);
                    if (arg.Data.RetornoFrete != null)
                        preecherRetornoFrete(_cargaAtual, arg.Data.RetornoFrete);

                    BuscarPercursoCargaDadosEmissaoPassagem();

                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

//*******MÉTODOS*******

function retornoTomador(data) {
    _cargaDadosEmissaoGeral.Tomador.codEntity(data.Codigo);
    _cargaDadosEmissaoGeral.Tomador.val("(" + data.CPF_CNPJ + ") " + data.Nome);
    mudarKnoutPessoaParaRota();
}

function mudarKnoutPessoaParaRota() {
    _cargaDadosEmissaoRota.RotasFrete.val(_cargaDadosEmissaoRota.RotasFrete.def);

    retornarDadosCargaPedido(_indiceGlobalPedidoDadosEmissao, function (pedido) {
        if (_cargaDadosEmissaoGeral.TipoPagamento.val() == EnumTipoPagamento.Pago) {
            _knoutPessoaParaRota.val(pedido.Remetente.Descricao);
            _knoutPessoaParaRota.codEntity(pedido.Remetente.Codigo);
        } else if (_cargaDadosEmissaoGeral.TipoPagamento.val() == EnumTipoPagamento.A_Pagar) {
            _knoutPessoaParaRota.val(pedido.Cliente.Descricao);
            _knoutPessoaParaRota.codEntity(pedido.Cliente.Codigo);
        } else if (_cargaDadosEmissaoGeral.TipoPagamento.val() == EnumTipoPagamento.Outros) {
            _knoutPessoaParaRota.val(_cargaDadosEmissaoGeral.Tomador.val());
            _knoutPessoaParaRota.codEntity(_cargaDadosEmissaoGeral.Tomador.codEntity());
        }
    });
}

function preecherDadosEmissao(pedido) {
    _cargaDadosEmissaoGeral.Carga.val(_cargaAtual.Codigo.val());

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _cargaDadosEmissaoGeral.Pedido.val(pedido.Codigo);
        _cargaDadosEmissaoGeral.TipoPagamento.val(pedido.TipoPagamento);
        _cargaDadosEmissaoGeral.UsarTipoPagamentoNF.val(pedido.UsarTipoPagamentoNF);
        _cargaDadosEmissaoGeral.IncluirICMSBC.val(pedido.IncluirICMSBC);
        _cargaDadosEmissaoGeral.ViagemJaOcorreu.val(pedido.ViagemJaOcorreu);
        if (pedido.UsarTipoPagamentoNF)
            _cargaDadosEmissaoGeral.TipoPagamento.enable(!pedido.UsarTipoPagamentoNF);

        if (pedido.TipoPagamento == EnumTipoPagamento.Outros) {
            _cargaDadosEmissaoGeral.Tomador.visibleFade(true);
            _cargaDadosEmissaoGeral.Tomador.required = true;
            _cargaDadosEmissaoGeral.Tomador.codEntity(pedido.Tomador.Codigo);

            if (pedido.Tomador.Codigo > 0)
                _cargaDadosEmissaoGeral.Tomador.val("(" + pedido.Tomador.CPF_CNPJ + ") " + pedido.Tomador.Nome);
            else
                _cargaDadosEmissaoGeral.Tomador.val("");

        } else {
            _cargaDadosEmissaoGeral.Tomador.codEntity(0);
            _cargaDadosEmissaoGeral.Tomador.val("");
            _cargaDadosEmissaoGeral.Tomador.visibleFade(false);
            _cargaDadosEmissaoGeral.Tomador.required = false;
        }

        _cargaDadosEmissaoGeral.AutorizarModalidadePagamento.visible(pedido.ModalidadePagamentoNFAgAprovacao);
        _cargaDadosEmissaoGeral.MensagemAutorizarModalidadePagamento.visible(pedido.ModalidadePagamentoNFAgAprovacao);

        _cargaDadosEmissaoGeral.NumeroPedido.val(pedido.NumeroPedidoEmbarcador);

        _cargaDadosEmissaoGeral.TipoCobrancaMultimodal.val(pedido.TipoCobrancaMultimodal);
        _cargaDadosEmissaoGeral.ModalPropostaMultimodal.val(pedido.ModalPropostaMultimodal);
        _cargaDadosEmissaoGeral.TipoServicoMultimodal.val(pedido.TipoServicoMultimodal);
        _cargaDadosEmissaoGeral.TipoPropostaMultimodal.val(pedido.TipoPropostaMultimodal);

        _cargaDadosEmissaoGeral.CobrarOutroDocumento.visible(true);
        _cargaDadosEmissaoGeral.ModeloDocumentoFiscal.visible(true);

        if (_cargaAtual.EtapaFreteEmbarcador.enable()) {
            _cargaDadosEmissaoGeral.ModeloDocumentoFiscal.enable(_cargaDadosEmissaoGeral.CobrarOutroDocumento.val());
        }

        if (pedido.PedidoSubContratado) {
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) {
                _cargaDadosEmissaoGeral.UsarTipoPagamentoNF.visible(false);

                $("#tabSeguro_" + _cargaAtual.DadosEmissaoFrete.id).hide();
                $('[href="#tabSeguro_' + _cargaAtual.DadosEmissaoFrete.id + '"]').closest('li').hide();
                _configuracaoEmissaoCTe.Configuracao.ApoliceSeguro.visible(false);
            }
        } else {
            _configuracaoEmissaoCTe.Configuracao.ApoliceSeguro.visible(true);
        }

        if (pedido.Empresa != null) {
            _cargaAtual.Empresa.val(pedido.Empresa.Descricao);
            _cargaAtual.Empresa.codEntity(pedido.Empresa.Codigo);
        }
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {

        if (pedido != null && pedido.Fronteiras != null && pedido.Fronteiras.length > 0) {
            _cargaDadosEmissaoGeral.Fronteiras.multiplesEntities(pedido.Fronteiras);
        }
        else {
            _cargaDadosEmissaoGeral.Fronteiras.codEntity(0);
            _cargaDadosEmissaoGeral.Fronteiras.val("");
        }

        _cargaDadosEmissaoGeral.TipoCobrancaMultimodal.val(pedido.TipoCobrancaMultimodal);
    }
}

function preecherrCargaDadosEmissaoGeral(data) {
    var pedido = data.Data;

    _cargaDadosEmissaoGeral.Carga.val(_cargaAtual.Codigo.val());

    _cargaDadosEmissaoGeral.GerarMDFeManualmente.val(pedido.NaoGerarMDFe);
    _cargaDadosEmissaoGeral.NaoComprarValePedagio.val(pedido.NaoComprarValePedagio);
    _cargaDadosEmissaoGeral.NaoComprarValePedagio.visible(pedido.ExibirOpcaoNaoComprarValePedagio);

    _cargaDadosEmissaoGeral.MensagemAutorizarPeso.text(pedido.MensagemAutorizarPeso);
    _cargaDadosEmissaoGeral.MensagemAutorizarPeso.visible(pedido.NecessarioAutorizacaoPeso);
    _cargaDadosEmissaoGeral.AutorizarPeso.visible(pedido.NecessarioAutorizacaoPeso);

    _cargaDadosEmissaoGeral.MensagemAutorizarManutencao.text(pedido.MensagemAutorizarManutencao);
    _cargaDadosEmissaoGeral.MensagemAutorizarManutencao.visible(pedido.NecessarioAutorizacaoManutencao);
    _cargaDadosEmissaoGeral.AutorizarManutencao.visible(pedido.NecessarioAutorizacaoManutencao);

    _cargaDadosEmissaoGeral.MensagemAutorizarValorMaximoPendentePagamento.text(pedido.MensagemAutorizacaoValorMaximoPendentePagamento);
    _cargaDadosEmissaoGeral.MensagemAutorizarValorMaximoPendentePagamento.visible(pedido.NecessarioAutorizacaoValorMaximoPendentePagamento);
    _cargaDadosEmissaoGeral.AutorizarValorMaximoPendentePagamento.visible(pedido.NecessarioAutorizacaoValorMaximoPendentePagamento);

    if (pedido.CobrarOutroDocumento) {
        _cargaDadosEmissaoGeral.CobrarOutroDocumento.val(pedido.CobrarOutroDocumento);
        _cargaDadosEmissaoGeral.ModeloDocumentoFiscal.val(pedido.ModeloDocumentoFiscal.Descricao);
        _cargaDadosEmissaoGeral.ModeloDocumentoFiscal.codEntity(pedido.ModeloDocumentoFiscal.Codigo);

        CobrerOutroDocumentoClick();
    }

    _cargaDadosEmissaoGeral.TipoPagamento.val(pedido.TipoPagamento);
    _cargaDadosEmissaoGeral.UsarTipoPagamentoNF.val(pedido.UsarTipoPagamentoNF);
    _cargaDadosEmissaoGeral.TipoPagamento.enable(!pedido.UsarTipoPagamentoNF);
   
    if (pedido.TipoPagamento == EnumTipoPagamento.Outros) {        
        _cargaDadosEmissaoGeral.Tomador.visibleFade(true);
        _cargaDadosEmissaoGeral.Tomador.required = true;       
    } else {      
        _cargaDadosEmissaoGeral.Tomador.visibleFade(false);
        _cargaDadosEmissaoGeral.Tomador.required = false;
    }

    if (pedido.TipoTomador == EnumTipoTomador.Outros) {
        _cargaDadosEmissaoGeral.Tomador.val(pedido.Tomador.Descricao);
        _cargaDadosEmissaoGeral.Tomador.codEntity(pedido.Tomador.Codigo);
    }
    else {
        _cargaDadosEmissaoGeral.Tomador.codEntity(0);
        _cargaDadosEmissaoGeral.Tomador.val("");
    }

    _cargaDadosEmissaoGeral.TipoCobrancaMultimodal.val(pedido.TipoCobrancaMultimodal);
    _cargaDadosEmissaoGeral.ModalPropostaMultimodal.val(pedido.ModalPropostaMultimodal);
    _cargaDadosEmissaoGeral.TipoServicoMultimodal.val(pedido.TipoServicoMultimodal);
    _cargaDadosEmissaoGeral.TipoPropostaMultimodal.val(pedido.TipoPropostaMultimodal);

    _cargaDadosEmissaoGeral.IncluirICMSBC.val(pedido.IncluirICMSBC);

    if (pedido.Fronteiras != null && pedido.Fronteiras.length > 0) {
        _cargaDadosEmissaoGeral.Fronteiras.multiplesEntities(pedido.Fronteiras);
    } else {
        _cargaDadosEmissaoGeral.Fronteiras.codEntity(0);
        _cargaDadosEmissaoGeral.Fronteiras.val("");
    }

    _cargaDadosEmissaoGeral.FilialCargaAgrupada.visible(_cargaAtual.NumeroCargaOriginais.val().trim().length > 0);
}

function carregarDadosEmissaoGeral(indice) {
    if (indice != _indiceGlobalPedidoDadosEmissao) {
        BuscarDadosEmissao(indice, function (arg) {
            $("#" + _cargaDadosEmissaoGeral.Pedido.idTab + "_" + _indiceGlobalPedidoDadosEmissao).removeClass("active");
            $("#" + _cargaDadosEmissaoGeral.Pedido.idTab + "_" + indice).addClass("active");
            _indiceGlobalPedidoDadosEmissao = indice;
            preecherDadosEmissao(arg);
            mudarKnoutPessoaParaRota();
        });
    }
}

function RelatorioBoletimViagemEmissaoGeral(e, sender) {
    executarDownload("CargaImpressaoDocumentos/RelatorioBoletimViagem", { Carga: e.Carga.val(), Recolhimento: false });
}

function VerificarCamposElementoPEPCentroResultado() {
    _cargaDadosEmissaoGeral.ElementoPEP.enable(true);
    _cargaDadosEmissaoGeral.CentroResultado.enable(true);

    if (_cargaDadosEmissaoGeral.CentroResultado.val() != "" && _cargaDadosEmissaoGeral.CentroResultado.codEntity() > 0) {
        _cargaDadosEmissaoGeral.ElementoPEP.enable(false);
        _cargaDadosEmissaoGeral.ElementoPEP.val("");
    }
    if (_cargaDadosEmissaoGeral.ElementoPEP.val() != "") {
        _cargaDadosEmissaoGeral.CentroResultado.enable(false);
        _cargaDadosEmissaoGeral.CentroResultado.val("");
    }
}

function VerificarCamposAplicarGeralTodosPedidos() {
    if (_cargaDadosEmissaoGeral.AplicarGeralEmTodosPedidos.val()) {
        _cargaDadosEmissaoGeral.ContaContabil.text(Localization.Resources.Cargas.Carga.ContaContabil.getRequiredFieldDescription());
        _cargaDadosEmissaoGeral.ContaContabil.required = true;
    }
    else {
        _cargaDadosEmissaoGeral.ContaContabil.text(Localization.Resources.Cargas.Carga.ContaContabil);
        _cargaDadosEmissaoGeral.ContaContabil.required = false;
    }
}

var retornarListaFiliais = (cargas) => {
    const opcoes = [{ text: "Nenhuma especificada ", value: 0 }];
    if (!cargas)
        return opcoes;

    for (let i = 0; i < cargas.length; i++)
        if (cargas[i].FilialCodigo != 0 && !opcoes.some(o => o.value === parseInt(cargas[i].FilialCodigo)))
            opcoes.push({ text: cargas[i].Filial, value: parseInt(cargas[i].FilialCodigo) })

    return opcoes;
}