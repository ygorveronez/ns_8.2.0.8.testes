/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="ValorParametroOcorrencia.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _escolta;
var _escoltaValor;
var _gridEscoltaValores;

var Escolta = function () {
    this.TipoOcorrencia = PropertyEntity({
        type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Ocorrência:", idBtnSearch: guid(), visible: ko.observable(true), required: function () {
            return _escolta.ComponenteFrete.codEntity() > 0;
        }  });
    this.ComponenteFrete = PropertyEntity({
        type: types.entity, codEntity: ko.observable(0), text: "*Componente Frete:", idBtnSearch: guid(), visible: ko.observable(true), required: function () {
            return _escolta.TipoOcorrencia.codEntity() > 0;
        }
    });

    this.HoraContratada = PropertyEntity({ text: "Hora Contratada:", getType: typesKnockout.time });
    this.ValorHoraExcedente = PropertyEntity({ text: "Valor da Hora Excedente:", getType: typesKnockout.decimal });
    this.KmContratada = PropertyEntity({ text: "KM Contratada:", getType: typesKnockout.int });
    this.ValorKmExcedente = PropertyEntity({ text: "Valor do KM Excedente:", getType: typesKnockout.decimal });
}

//*******EVENTOS*******
function LoadEscolta() {
    _escolta = new Escolta();
    KoBindings(_escolta, "knockoutEscolta");

    new BuscarComponentesDeFrete(_escolta.ComponenteFrete);
    new BuscarTipoOcorrencia(_escolta.TipoOcorrencia);

    _dados.EscoltaTipoOcorrencia = _escolta.TipoOcorrencia;
    _dados.EscoltaComponenteFrete = _escolta.ComponenteFrete;
    _dados.EscoltaHoraContratada = _escolta.HoraContratada;
    _dados.EscoltaValorHoraExcedente = _escolta.ValorHoraExcedente;
    _dados.EscoltaKmContratada = _escolta.KmContratada;
    _dados.EscoltaValorKmExcedente = _escolta.ValorKmExcedente;
}

//*******METODOS*******
function LimparCamposEscolta() {
    LimparCampos(_escolta);
}