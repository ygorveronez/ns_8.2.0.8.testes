/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _observacaoGuarita;

/*
 * Declaração das Classes
 */

var ObservacaoGuarita = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao.getFieldDescription(), maxlength: 300 });

    this.Salvar = PropertyEntity({ eventClick: salvarObservacaoGuaritaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadObservacaoGuarita() {
    inserirHtmlModalObservacaoGuarita(function () {
        _observacaoGuarita = new ObservacaoGuarita();
        KoBindings(_observacaoGuarita, "knockoutObservacaoGuarita");
    });
}

function inserirHtmlModalObservacaoGuarita(callback) {
    $.get("Content/Static/GestaoPatio/ObservacaoGuarita.html?dyn=" + guid(), function (data) {

        var $containerModalObservacaoGuarita = $("#containerModalObservacaoGuarita");

        if ($containerModalObservacaoGuarita.length == 0) {
            $("#js-page-content").append("<div id='containerModalObservacaoGuarita'></div>");
            $containerModalObservacaoGuarita = $("#containerModalObservacaoGuarita");
        }

        $("#containerModalObservacaoGuarita").html(data);

        callback();
    });
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function salvarObservacaoGuaritaClick() {
    executarReST("JanelaCarregamento/SalvarObservacaoGuarita", RetornarObjetoPesquisa(_observacaoGuarita), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ObservacaoDaGuaritaAtualizadaComSucesso);
                fecharModalObservacaoGuarita();
                
                if (typeof _gridGuarita !== 'undefined')
                    _gridGuarita.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

/*
 * Declaração das Funções Públicas
 */

function exibirObservacaoGuarita(cargaJanelaCarregamento) {
    if (typeof cargaJanelaCarregamento.Carga !== 'undefined')
        _observacaoGuarita.Codigo.val(cargaJanelaCarregamento.Carga.Codigo);
    else
        _observacaoGuarita.Codigo.val(cargaJanelaCarregamento.CodigoCarga);

    _observacaoGuarita.Observacao.val(cargaJanelaCarregamento.ObservacaoGuarita);

    exibirModalObservacaoGuarita();
}

/*
 * Declaração das Funções Privadas
 */

function exibirModalObservacaoGuarita() {
    Global.abrirModal('divModalObservacaoGuarita');
    $("#divModalObservacaoGuarita").one('hidden.bs.modal', function () {
        LimparCampos(_observacaoGuarita);
    });
}

function fecharModalObservacaoGuarita() {
    Global.fecharModal('divModalObservacaoGuarita');
}
