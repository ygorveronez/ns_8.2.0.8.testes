var _retornoAcompanhamentoPallet, _botoesAcompanhamentoPallet;

var BotoesAcompanhamentoPallet = function () {
    this.Cancelar = PropertyEntity({ eventClick: cancelarAgendamentoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: limparAgendamentoClick, type: types.event, text: "Limpar", visible: ko.observable(true) });
}

var RetornoAcompanhamentoPallet = function () {
    this.Situacao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.SituacaoCodigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.SenhaAgendamento = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.DataProgramada = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.DataSolicitada = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.QuantidadePallet = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.NumeroCarga = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.NumeroCarregamento = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.OperadorAgendamento = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
};

function carregarDadosAcompanhamentoPallet() {
    _retornoAcompanhamentoPallet = new RetornoAcompanhamentoPallet();
    KoBindings(_retornoAcompanhamentoPallet, "knockoutRetornoAgendamentoPallet");

    _botoesAcompanhamentoPallet = new BotoesAcompanhamentoPallet();
    KoBindings(_botoesAcompanhamentoPallet, "knockoutBotoesAgendamentoPallet");
}