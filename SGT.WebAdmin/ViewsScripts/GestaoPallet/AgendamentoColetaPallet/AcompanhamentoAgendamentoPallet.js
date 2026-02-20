var _etapaAcompanhamentoAgendamentoPallet, _retornoAcompanhamentoAgendamentoPallet, _botoesAcompanhamentoAgendamentoPallet;

var BotoesAcompanhamentoAgendamentoPallet = function () {
    this.Limpar = PropertyEntity({ eventClick: limparAgendamentoColetaClick, type: types.event, text: "Limpar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarAgendamentoColetaClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.Imprimir = PropertyEntity({ eventClick: imprimirAgendamentoColetaClick, type: types.event, text: "Imprimir", visible: ko.observable(true) });
}

var RetornoAcompanhamentoAgendamentoPallet = function () {

    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Carga:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.QuantidadePallets = PropertyEntity({ text: "Quantidade de Paletes", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Filial = PropertyEntity({ text: "Filial", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.StatusAgendamento = PropertyEntity({ text: "Status Agendamento", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Solicitante = PropertyEntity({ text: "Solicitante", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ text: "Transportador", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Cliente = PropertyEntity({ text: "Cliente", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ text: "Veiculo", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DataOrdem = PropertyEntity({ text: "DataOrdem", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DataCarregamento = PropertyEntity({ text: "DataCarregamento", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroOrdem = PropertyEntity({ text: "NumeroOrdem", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ text: "Motorista", val: ko.observable(""), def: "", visible: ko.observable(true) });

};

function carregarDadosAcompanhamentoAgendamentoPallet() {

    _retornoAcompanhamentoAgendamentoPallet = new RetornoAcompanhamentoAgendamentoPallet();
    KoBindings(_retornoAcompanhamentoAgendamentoPallet, "knockoutRetornoAgendamentoColetaPallet");

    _botoesAcompanhamentoAgendamentoPallet = new BotoesAcompanhamentoAgendamentoPallet();
    KoBindings(_botoesAcompanhamentoAgendamentoPallet, "knockoutBotoesAgendamentoColetaPallet");
}