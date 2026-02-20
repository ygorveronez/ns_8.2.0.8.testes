/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ParametroOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumEtapaFluxoGestaoPatio.js" />
/// <reference path="../../Enumeradores/EnumTipoGatilhoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumTipoGatilhoOcorrenciaInicialFluxoPatio.js" />
/// <reference path="../../Enumeradores/EnumTipoParametroOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumGatilhoInicialTraking.js" />
/// <reference path="../../Enumeradores/EnumGatilhoFinalTraking.js" />
/// <reference path="../../Enumeradores/EnumTipoAplicacaoGatilhoTracking.js" />
/// <reference path="../../Enumeradores/EnumTipoDataAlteracaoGatilho.js" />
/// <reference path="TipoOcorrencia.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gatilho;
var _gatilhosFluxoPatio = ['GatilhoInicialFluxoPatio', 'GatilhoFinalFluxoPatio'];
var _gatilhosTraking = ['GatilhoInicialTraking', 'GatilhoFinalTraking'];
var _gatilhosAlteracaoData = ['TipoDataAlteracaoGatilho'];
var _gatilhos = [].concat(_gatilhosFluxoPatio, _gatilhosTraking);

/*
 * Declaração das Classes
 */

var Gatilho = function () {
    var self = this;

    this.DefinirAutomaticamenteTempoEstadiaPorTempoParadaNoLocalEntrega = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.DefinirAutomaticamenteTempoEstadiaPorTempoParadaNoLocalEntrega, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ValidarDataAgendadaEntrega = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.ValidarDataAgendadaEntrega, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.TipoOcorrencia.Filial.getFieldDescription(), idBtnSearch: guid() });
    this.GatilhoInicialFluxoPatio = PropertyEntity({ val: ko.observable(EnumTipoGatilhoOcorrenciaInicialFluxoPatio.Todos), options: EnumTipoGatilhoOcorrenciaInicialFluxoPatio.obterOpcoes(), def: EnumTipoGatilhoOcorrenciaInicialFluxoPatio.Todos, text: Localization.Resources.Ocorrencias.TipoOcorrencia.GatilhoInicial.getRequiredFieldDescription(), required: true, visible: ko.observable(true) });
    this.GatilhoFinalFluxoPatio = PropertyEntity({ val: ko.observable(""), options: EnumEtapaFluxoGestaoPatio.obterOpcoesGatilhoOcorrenciaFinal(), def: "", text: Localization.Resources.Ocorrencias.TipoOcorrencia.GatilhoFilial.getRequiredFieldDescription(), required: true, visible: ko.observable(true) });
    this.GatilhoInicialTraking = PropertyEntity({ val: ko.observable(EnumGatilhoInicialTraking.Todos), options: EnumGatilhoInicialTraking.obterOpcoes(), def: EnumGatilhoInicialTraking.Todos, text: Localization.Resources.Ocorrencias.TipoOcorrencia.GatilhoInicial.getRequiredFieldDescription(), required: false, visible: ko.observable(false) });
    this.GatilhoFinalTraking = PropertyEntity({ val: ko.observable(EnumGatilhoFinalTraking.Todos), options: EnumGatilhoFinalTraking.obterOpcoes(), def: EnumGatilhoFinalTraking.Todos, text: Localization.Resources.Ocorrencias.TipoOcorrencia.GatinhoFinal.getRequiredFieldDescription(), required: false, visible: ko.observable(false) });
    this.HorasMinimas = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.HorasMinimas.getFieldDescription(), getType: typesKnockout.int, required: function () { return !self.UtilizarTempoCarregamentoComoHoraMinima.val() }, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 3 });
    this.UtilizarTempoCarregamentoComoHoraMinima = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.UtilizarTempoCarregamentoComoHoraMinima, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.Parametro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.TipoOcorrencia.ParametroPorPeriodo.getRequiredFieldDescription(), idBtnSearch: guid(), required: function () { return self.GerarAutomaticamente.val() } });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoGatilhoOcorrencia.FluxoPatio), options: EnumTipoGatilhoOcorrencia.obterOpcoes(), def: EnumTipoGatilhoOcorrencia.FluxoPatio, text: Localization.Resources.Ocorrencias.TipoOcorrencia.Tipo.getRequiredFieldDescription(), required: true });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.TipoOcorrencia.TipoOperacao.getFieldDescription(), idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.TipoOcorrencia.Transportador.getFieldDescription(), idBtnSearch: guid() });
    this.UtilizarGatilhoGeracaoOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.UtilizarGatilhoGeracaoOcorrencia, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.GerarAutomaticamente = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.GerarAutomaticamente, val: ko.observable(true), def: true, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.NaoPermiteDuplicarOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.NaoPermiteDuplicarOcorrencia, val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.TipoAplicacaoGatilhoTracking = PropertyEntity({ val: ko.observable(EnumTipoAplicacaoGatilhoTracking.AplicarSempre), options: EnumTipoAplicacaoGatilhoTracking.obterOpcoes(), def: EnumTipoAplicacaoGatilhoTracking.AplicarSempre, text: Localization.Resources.Ocorrencias.TipoOcorrencia.Aplicacao.getFieldDescription(), visible: ko.observable(false) });
    this.TipoCobrancaMultimodal = PropertyEntity({ val: ko.observable(EnumTipoCobrancaMultimodal.Nenhum), options: EnumTipoCobrancaMultimodal.obterOpcoes(), text: Localization.Resources.Ocorrencias.TipoOcorrencia.ModalTransporte.getFieldDescription(), def: EnumTipoCobrancaMultimodal.Nenhum, enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });

    this.Tipo.val.subscribe(tipoGatilhoChange);
    this.GerarAutomaticamente.val.subscribe(controlarExibicaoAbaParametro);
    this.UtilizarGatilhoGeracaoOcorrencia.val.subscribe(controlarExibicaoAbaParametro);
    this.TipoDataAlteracaoGatilho = PropertyEntity({ val: ko.observable(EnumTipoDataAlteracaoGatilho.DataAgendamentoEntregaTransportador), options: EnumTipoDataAlteracaoGatilho.obterOpcoes(), def: EnumTipoDataAlteracaoGatilho.DataAgendamentoEntregaTransportador, text: "Tipo da data de alteração", required: ko.observable(false), visible: ko.observable(false) });
    this.AtribuirDataOcorrenciaNaDataAgendamentoTransportador = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.AtribuirDataOcorrenciaNaDataAgendamentoTransportador, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    var self = this;

    this.ValidarDataAgendadaEntregaVisivel = ko.computed(function () {
        return self.Tipo && self.Tipo.val() === EnumTipoGatilhoOcorrencia.Tracking &&
            self.TipoAplicacaoGatilhoTracking && (self.TipoAplicacaoGatilhoTracking.val() === EnumTipoAplicacaoGatilhoTracking.Entrega || self.TipoAplicacaoGatilhoTracking.val() === EnumTipoAplicacaoGatilhoTracking.Coleta)  &&
            self.GatilhoInicialTraking && self.GatilhoInicialTraking.val() === EnumGatilhoInicialTraking.EntradaCliente;
    });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadGatilho() {
    _gatilho = new Gatilho();
    KoBindings(_gatilho, "knockoutGatilho");

    BuscarParametroOcorrencia(_gatilho.Parametro, undefined, EnumTipoParametroOcorrencia.Periodo);
    BuscarTiposOperacao(_gatilho.TipoOperacao);
    BuscarFilial(_gatilho.Filial);
    BuscarTransportadores(_gatilho.Transportador);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function tipoGatilhoChange() {
    switch (_gatilho.Tipo.val()) {
        case EnumTipoGatilhoOcorrencia.Tracking:
            alterarGatilhosVisiveis(_gatilhosTraking);
            _gatilho.TipoAplicacaoGatilhoTracking.visible(true);
            _gatilho.TipoDataAlteracaoGatilho.visible(false);
            break;

        case EnumTipoGatilhoOcorrencia.FluxoPatio:
            alterarGatilhosVisiveis(_gatilhosFluxoPatio);
            _gatilho.TipoAplicacaoGatilhoTracking.visible(false);
            _gatilho.TipoDataAlteracaoGatilho.visible(false);
            break;

        case EnumTipoGatilhoOcorrencia.AlteracaoData:
            alterarGatilhosVisiveis(_gatilhosAlteracaoData);
            _gatilho.TipoAplicacaoGatilhoTracking.visible(false);
            _gatilho.HorasMinimas.required(false);
            break;

        case EnumTipoGatilhoOcorrencia.AtingirData:
            alterarGatilhosVisiveis(_gatilhosAlteracaoData);
            _gatilho.TipoAplicacaoGatilhoTracking.visible(false);
            _gatilho.TipoDataAlteracaoGatilho.visible(false);
            _gatilho.HorasMinimas.required(false);
            break;
    }
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposGatilho() {
    LimparCampos(_gatilho);
}

function preencherGatilho(dadosGatilho) {
    PreencherObjetoKnout(_gatilho, { Data: dadosGatilho });
}

function preencherGatilhoSalvar(tipoOcorrencia) {
    const gatilho = {
        DefinirAutomaticamenteTempoEstadiaPorTempoParadaNoLocalEntrega: _gatilho.DefinirAutomaticamenteTempoEstadiaPorTempoParadaNoLocalEntrega.val(),
        GatilhoInicialFluxoPatio: _gatilho.GatilhoInicialFluxoPatio.val(),
        GatilhoFinalFluxoPatio: _gatilho.GatilhoFinalFluxoPatio.val(),
        GatilhoInicialTraking: _gatilho.GatilhoInicialTraking.val(),
        GatilhoFinalTraking: _gatilho.GatilhoFinalTraking.val(),
        HorasMinimas: _gatilho.HorasMinimas.val(),
        Parametro: _gatilho.Parametro.codEntity(),
        Tipo: _gatilho.Tipo.val(),
        UtilizarGatilhoGeracaoOcorrencia: _gatilho.UtilizarGatilhoGeracaoOcorrencia.val(),
        GerarAutomaticamente: _gatilho.GerarAutomaticamente.val(),
        UtilizarTempoCarregamentoComoHoraMinima: _gatilho.UtilizarTempoCarregamentoComoHoraMinima.val(),
        TipoAplicacaoGatilhoTracking: _gatilho.TipoAplicacaoGatilhoTracking.val(),
        TipoCobrancaMultimodal: _gatilho.TipoCobrancaMultimodal.val(),
        ValidarDataAgendadaEntrega: _gatilho.ValidarDataAgendadaEntrega.val(),
        TipoDataAlteracaoGatilho: _gatilho.TipoDataAlteracaoGatilho.val(),
        NaoPermiteDuplicarOcorrencia: _gatilho.NaoPermiteDuplicarOcorrencia.val(),
        AtribuirDataOcorrenciaNaDataAgendamentoTransportador: _gatilho.AtribuirDataOcorrenciaNaDataAgendamentoTransportador.val()
    };

    tipoOcorrencia["Gatilho"] = JSON.stringify(gatilho);
    tipoOcorrencia["GatilhoTipoOperacao"] = JSON.stringify(recursiveMultiplesEntities(_gatilho.TipoOperacao));
    tipoOcorrencia["GatilhoFilial"] = JSON.stringify(recursiveMultiplesEntities(_gatilho.Filial));
    tipoOcorrencia["GatilhoTransportador"] = JSON.stringify(recursiveMultiplesEntities(_gatilho.Transportador));
}

/*
 * Declaração das Funções Privadas
 */

function alterarGatilhosVisiveis(gatilhosHabilitar) {
    for (var gatilho of _gatilhos) {
        _gatilho[gatilho].visible(false);
        _gatilho[gatilho].required = false;
    }

    for (var gatilhoHabilitar of gatilhosHabilitar) {
        _gatilho[gatilhoHabilitar].visible(true);
        _gatilho[gatilhoHabilitar].required = true;
    }
}
