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
var _recolhimento;

var Recolhimento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.Data = PropertyEntity({ text: "*Data:", getType: typesKnockout.date, required: true, enable: ko.observable(true) });
    this.NFe = PropertyEntity({ text: "NF-e:", getType: typesKnockout.int, enable: ko.observable(true) });
    this.NFe.configInt.thousands = '';
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Quantidade = PropertyEntity({ text: "*Quantidade:", enable: ko.observable(true), required: true, getType: typesKnockout.int, enable: ko.observable(true) });
}



//*******EVENTOS*******
function LoadRecolhimento() {
    _recolhimento = new Recolhimento();
    KoBindings(_recolhimento, "knockoutRecolhimento");

    new BuscarTransportadores(_recolhimento.Transportador);

    _valePallet.Codigo.val.subscribe(function (val) {
        _recolhimento.Codigo.val(val);
    });
}


//*******MÉTODOS*******
function DadosRecolhimento(dados) {
    PreencherObjetoKnout(_recolhimento, { Data: dados.Recolhimento });

    if (dados.Situacao == EnumSituacaoValePallet.AgFinalizacao)
        ControleCamposDadosRecolhimento(true);
    else
        ControleCamposDadosRecolhimento(false);
}

function ControleCamposDadosRecolhimento(status) {
    _recolhimento.Data.enable(status);
    _recolhimento.NFe.enable(status);
    _recolhimento.Transportador.enable(status);
    _recolhimento.Quantidade.enable(status);
}

function LimparCamposDadosRecolhimento() {
    LimparCampos(_recolhimento);
    ControleCamposDadosRecolhimento(true);
}