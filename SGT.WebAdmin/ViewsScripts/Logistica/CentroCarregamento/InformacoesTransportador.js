/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumCamposOpcionaisJanelaCarregamentoTransportador.js" />

var _informacoesTransportador;

var InformacoesTransportador = function () {
    this.Propriedades = PropertyEntity({ type: types.local, val: ko.observableArray(obterPropriedades()) });
}

function loadInformacoesTransportador() {
    registrarComponenteInformacaoTransportador();

    _informacoesTransportador = new InformacoesTransportador();
    KoBindings(_informacoesTransportador, "knockoutInformacoesTransportador");
}

function registrarComponenteInformacaoTransportador() {
    if (ko.components.isRegistered('propriedade-informacao-transportador'))
        return;

    ko.components.register('propriedade-informacao-transportador', {
        template: {
            element: "propriedade-informacao-transportador"
        }
    });
}

function obterPropriedades() {
    const descricoes = EnumCamposOpcionaisJanelaCarregamentoTransportador.obterDescricoes();
    const propriedades = [];

    for (let i = 0; i < descricoes.length; i++)
        propriedades.push(PropertyEntity({ getType: typesKnockout.bool, def: false, val: ko.observable(false), numeroEnum: descricoes[i].value, text: descricoes[i].text, visible: ko.observable(true) }));

    return propriedades;
}

function preencherCamposVisiveisTransportador(centroCarregamento) {
    const camposVisiveis = _informacoesTransportador.Propriedades.val()
        .filter(function (e) {
            return e.val() === true;
        }).map(function (e) {
            return e.numeroEnum;
        });

    const stringCamposVisiveis = camposVisiveis.join(";");
    
    centroCarregamento["CamposVisiveisTransportador"] = stringCamposVisiveis;
}

function limparCamposInformacoesTransportador() {
    LimparCampos(_informacoesTransportador.Propriedades.val());
}

function preencherCentroCarregamentoCamposVisiveisTransportador(camposVisiveis) {
    if (string.IsNullOrWhiteSpace(camposVisiveis))
        camposVisiveis = "";

    const camposVisiveisArr = camposVisiveis.split(";");

    for (let i = 0; i < _informacoesTransportador.Propriedades.val().length; i++) {
        const infoTrans = _informacoesTransportador.Propriedades.val()[i];
        _informacoesTransportador.Propriedades.val()[i].val(camposVisiveisArr.includes(infoTrans.numeroEnum.toString()));
    }
}