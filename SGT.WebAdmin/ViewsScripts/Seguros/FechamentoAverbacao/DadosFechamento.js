/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFechamentoAverbacoes.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _dadosFechamento;

var DadosFechamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Numero = PropertyEntity({ type: types.map, configInt: { precision: 0, allowZero: false, thousands: '' }, getType: typesKnockout.int, val: ko.observable("0"), def: "0", text: "Número:", enable: false });

    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:",issue: 69, idBtnSearch: guid(), enable: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:",issue: 121, idBtnSearch: guid(), enable: ko.observable(true) });

    this.DataInicio = PropertyEntity({ text: "Data Início: ", getType: typesKnockout.date, val: ko.observable(""), def: "", enable: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "Data Fim: ", getType: typesKnockout.date, val: ko.observable(""), def: "", enable: ko.observable(true) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
}

//*******EVENTOS*******
function loadDadosFechamento() {
    _dadosFechamento = new DadosFechamento();
    KoBindings(_dadosFechamento, "knockoutDadosFechamento");

    new BuscarTransportadores(_dadosFechamento.Transportador);
    new BuscarTiposOperacao(_dadosFechamento.TipoOperacao);
}

//*******MÉTODOS*******
function EditarDadosFechamento(data) {
    _dadosFechamento.Codigo.val(data.Codigo);
    if (data.DadosFechamento != null) {
        PreencherObjetoKnout(_dadosFechamento, { Data: data.DadosFechamento });
    }
    ControleCamposDadosFechamento(false); 
}

function ControleCamposDadosFechamento(status) {
    _dadosFechamento.DataInicio.enable(status);
    _dadosFechamento.DataFim.enable(status);
    _dadosFechamento.Transportador.enable(status);
    _dadosFechamento.TipoOperacao.enable(status);
}

function LimparCamposDadosFechamento() {
    LimparCampos(_dadosFechamento);
    ControleCamposDadosFechamento(true);
}
