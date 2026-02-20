/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Validacao.js" />
/// <reference path="../../Enumeradores/EnumEtapaFluxoGestaoPatio.js" />
/// <reference path="FluxoPatio.js" />

// #region Objetos Globais do Arquivo

var _gridObservacoesEtapasFluxoPatio;
var _observacoesEtapaFluxoPatio;
var _CRUDobservacoesEtapaFluxoPatio;

// #endregion Objetos Globais do Arquivo

// #region Classes

var ObservacoesEtapaFluxoPatio = function () {
    this.Etapa = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.ObservacoesEtapa = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), maxlength: 300, required: true, text: Localization.Resources.GestaoPatio.FluxoPatio.Observacoes.getFieldDescription(), enable: ko.observable(true) });
};

var CRUDObservacoesEtapaFluxoPatio = function () {
    this.Salvar = PropertyEntity({ eventClick: salvarObservacaoEtapaClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.Salvar, visible: ko.observable(true) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadGridObservacoesEtapas() {
    var quantidadePorPagina = 50;
    var exibirPaginacao = false;
    var header = [
        { data: "Codigo", visible: false },
        { data: "Observacao", title: Localization.Resources.GestaoPatio.FluxoPatio.Observacao, width: "70%" },
        { data: "Etapa", title: Localization.Resources.GestaoPatio.FluxoPatio.Etapa, width: "30%" }
    ];

    _gridObservacoesEtapasFluxoPatio = new BasicDataTable("grid-observacoes-etapas-fluxo-patio", header, null, null, null, quantidadePorPagina, null, exibirPaginacao);
    _gridObservacoesEtapasFluxoPatio.CarregarGrid([]);
}

function loadObservacoesEtapas() {
    _observacoesEtapaFluxoPatio = new ObservacoesEtapaFluxoPatio();
    _CRUDobservacoesEtapaFluxoPatio = new CRUDObservacoesEtapaFluxoPatio();

    KoBindings(_observacoesEtapaFluxoPatio, "knockoutObservacoesEtapaFluxoPatio");
    KoBindings(_CRUDobservacoesEtapaFluxoPatio, "knockoutCRUDObservacoesEtapaFluxoPatio");

    loadGridObservacoesEtapas();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function salvarObservacaoEtapaClick() {
    if (!ValidarCamposObrigatorios(_observacoesEtapaFluxoPatio)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
        return;
    }

    var data = {
        FluxoGestaoPatio: _fluxoAtual.Codigo.val(),
        Etapa: _observacoesEtapaFluxoPatio.Etapa.val(),
        ObservacoesEtapa: _observacoesEtapaFluxoPatio.ObservacoesEtapa.val()
    };

    executarReST("FluxoPatio/SalvarObservacoesEtapa", data, function (retorno) {
        if (retorno.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg);
            fecharModalObservacoesEtapaFluxoPatio();
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function buscarObservacoesEtapa(etapaFluxoGestaoPatio) {
    _observacoesEtapaFluxoPatio.Etapa.val(etapaFluxoGestaoPatio);

    executarReST("FluxoPatio/BuscarObservacoesEtapa", { FluxoGestaoPatio: _fluxoAtual.Codigo.val(), Etapa: _observacoesEtapaFluxoPatio.Etapa.val() }, function (retorno) {
        if (retorno.Success) {
            var habilitarEdicaoObservacao = _fluxoAtual.EtapaFluxoGestaoPatioAtual.val() == _observacoesEtapaFluxoPatio.Etapa.val() && !edicaoEtapaFluxoPatioBloqueada();

            PreencherObjetoKnout(_observacoesEtapaFluxoPatio, retorno);

            _observacoesEtapaFluxoPatio.ObservacoesEtapa.enable(habilitarEdicaoObservacao);
            _CRUDobservacoesEtapaFluxoPatio.Salvar.visible(habilitarEdicaoObservacao);
                
            exibirModalObservacoesEtapaFluxoPatio();
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function exibirObservacoesEtapasFluxoPatio(codigoFluxoGestaoPatio) {
    executarReST("FluxoPatio/BuscarObservacoesEtapas", { FluxoGestaoPatio: codigoFluxoGestaoPatio }, function (retorno) {
        if (retorno.Success) {
            _gridObservacoesEtapasFluxoPatio.CarregarGrid(retorno.Data);
            exibirModalObservacoesEtapasFluxoPatio();
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function exibirModalObservacoesEtapaFluxoPatio() {
    $("#divModalObservacoesEtapaFluxoPatio").modal("show").on("hidden.bs.modal", function () {
        LimparCampos(_observacoesEtapaFluxoPatio);
    });
}

function exibirModalObservacoesEtapasFluxoPatio() {
    Global.abrirModal('divModalObservacoesEtapasFluxoPatio');
}

function fecharModalObservacoesEtapaFluxoPatio() {
    Global.fecharModal('divModalObservacoesEtapaFluxoPatio');
}

// #endregion Funções Privadas
