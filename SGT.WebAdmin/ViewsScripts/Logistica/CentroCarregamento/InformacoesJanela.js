/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumCamposVisiveisNaJanela.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _informacoesJanela;

/*
 * Declaração das Classes
 */

var InformacoesJanela = function () {
    this.Propriedades = PropertyEntity({ type: types.local, val: ko.observableArray(obterPropriedadesJanela()) });
}

function loadInformacoesJanela() {
    registrarComponenteInformacaoJanela();

    _informacoesJanela = new InformacoesJanela();
    KoBindings(_informacoesJanela, "knockoutInformacoesJanela");
}

function registrarComponenteInformacaoJanela() {
    if (ko.components.isRegistered('propriedade-informacao-janela'))
        return;

    ko.components.register('propriedade-informacao-janela', {
        template: {
            element: "propriedade-informacao-janela"
        }
    });
}

function obterPropriedadesJanela() {
    const descricoes = EnumCamposVisiveisNaJanela.obterDescricoes();
    const propriedades = [];

    for (let i = 0; i < descricoes.length; i++)
        propriedades.push(PropertyEntity({ getType: typesKnockout.bool, def: false, val: ko.observable(false), numeroEnum: descricoes[i].value, text: descricoes[i].text, visible: ko.observable(true) }));

    return propriedades;
}

function preencherCamposVisiveisJanela(centroCarregamento) {
    const camposVisiveis = _informacoesJanela.Propriedades.val()
        .filter(function (e) {
            return e.val() === true;
        }).map(function (e) {
            return e.numeroEnum;
        });

    const stringCamposVisiveis = camposVisiveis.join(";");

    centroCarregamento["CamposVisiveisJanela"] = stringCamposVisiveis;
}

function limparCamposInformacoesJanela() {
    LimparCampos(_informacoesJanela.Propriedades.val());
}

function preencherCentroCarregamentoCamposVisiveisJanela(camposVisiveis) {
    if (string.IsNullOrWhiteSpace(camposVisiveis))
        camposVisiveis = "";

    const camposVisiveisArr = camposVisiveis.split(";");

    for (let i = 0; i < _informacoesJanela.Propriedades.val().length; i++) {
        const infoJanela = _informacoesJanela.Propriedades.val()[i];
        _informacoesJanela.Propriedades.val()[i].val(camposVisiveisArr.includes(infoJanela.numeroEnum.toString()));
    }
}