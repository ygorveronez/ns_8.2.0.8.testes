/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _openTech;

var OpenTech = function () {
    this.CodigoTipoSensorOpentech = PropertyEntity({ val: ko.observable(""), options: ko.observable([]), def: "", text: Localization.Resources.Cargas.TipoCarga.TipoSensorOpentech.getFieldDescription() });
    this.QuantidadeSensores = PropertyEntity({ text: Localization.Resources.Cargas.TipoCarga.QuantidadeSensores.getFieldDescription(), visible: ko.observable(true), required: false, getType: typesKnockout.int });
    this.ToleranciaTemperaturaSuperior = PropertyEntity({ text: Localization.Resources.Cargas.TipoCarga.ToleranciaTemperaturaSuperior.getFieldDescription(), visible: ko.observable(true), required: false, getType: typesKnockout.decimal, configDecimal: { precision: 0, allowZero: false, allowNegative: true } });
    this.ToleranciaTemperaturaInferior = PropertyEntity({ text: Localization.Resources.Cargas.TipoCarga.ToleranciaTemperaturaInferior.getFieldDescription(), visible: ko.observable(true), required: false, getType: typesKnockout.decimal, configDecimal: { precision: 0, allowZero: false, allowNegative: true } });
    this.TemperaturaIdealSuperior = PropertyEntity({ text: Localization.Resources.Cargas.TipoCarga.TemperaturaIdealSuperior.getFieldDescription(), visible: ko.observable(true), required: false, getType: typesKnockout.decimal, configDecimal: { precision: 0, allowZero: false, allowNegative: true } });
    this.TemperaturaIdealInferior = PropertyEntity({ text: Localization.Resources.Cargas.TipoCarga.TemperaturaIdealInferior.getFieldDescription(), visible: ko.observable(true), required: false, getType: typesKnockout.decimal, configDecimal: { precision: 0, allowZero: false, allowNegative: true } });
};

//*******EVENTOS*******

function loadOpenTech() {
    _openTech = new OpenTech();
    KoBindings(_openTech, "knockoutOpenTech");

    _tipoCarga.CodigoTipoSensorOpentech = _openTech.CodigoTipoSensorOpentech;
    _tipoCarga.QuantidadeSensores = _openTech.QuantidadeSensores;
    _tipoCarga.ToleranciaTemperaturaSuperior = _openTech.ToleranciaTemperaturaSuperior;
    _tipoCarga.ToleranciaTemperaturaInferior = _openTech.ToleranciaTemperaturaInferior;
    _tipoCarga.TemperaturaIdealSuperior = _openTech.TemperaturaIdealSuperior;
    _tipoCarga.TemperaturaIdealInferior = _openTech.TemperaturaIdealInferior;
}

//*******MÉTODOS*******

function limparCamposOpenTech() {
    LimparCampos(_openTech);

    _openTech.QuantidadeSensores.val("1");
    _openTech.ToleranciaTemperaturaInferior.val("-999");
    _openTech.ToleranciaTemperaturaSuperior.val("-999");
    _openTech.TemperaturaIdealInferior.val("-999");
    _openTech.TemperaturaIdealSuperior.val("-999");
}

function obterTipoSensorOpentech() {
    executarReST("TipoCarga/BuscarTipoSensorOpenTech", {}, function (arg) {
        if (arg.Success) {
            loadOpenTech();
            var tipoSensorOpentech = [{ value: "", text: "Selecione" }].concat(arg.Data);

            _openTech.CodigoTipoSensorOpentech.options(tipoSensorOpentech);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}