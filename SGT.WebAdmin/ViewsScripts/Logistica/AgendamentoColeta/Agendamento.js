/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Veiculo.js" />

//#region Variaveis Globais

var _dadosAgendamento, _retornoAgendamento;

//#endregion

//#region Mapeamento Knockout

var DadosAgendamento = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, visible: false, val: ko.observable(0) });

    this.Transportador = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.entity, text: "Transportador:", idBtnSearch: guid(), enable: ko.observable(true), enable: ko.observable(true) });
    this.Motorista = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.entity, text: "Motorista:", idBtnSearch: guid(), enable: ko.observable(true), enable: ko.observable(true) });
    this.Veiculo = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.entity, text: "Veículo:", idBtnSearch: guid(), enable: ko.observable(true), enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarDadosAgendamentoClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
};

var RetornoAgendamento = function () {
    this.Situacao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.SituacaoCodigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.SenhaAgendamento = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.DataProgramada = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.DataCancelamento = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.DataSolicitada = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.VolumesAgendados = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.NumeroCarga = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.OperadorAgendamento = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
};

function LoadDadosAgendamento() {
    _dadosAgendamento = new DadosAgendamento();
    KoBindings(_dadosAgendamento, "knockoutDadosAgendamento");

    _retornoAgendamento = new RetornoAgendamento();
    KoBindings(_retornoAgendamento, "knockoutRetornoAgendamento");

    new BuscarTransportadores(_dadosAgendamento.Transportador, RetornoTransportadorDadosAgendamento, null, null, null, null, null, null, null, null, null, null, null, _configuracaoAgendamentoColeta.ConsultarSomenteTransportadoresPermitidosCadastro);
    new BuscarMotoristas(_dadosAgendamento.Motorista, null, _dadosAgendamento.Transportador);
    new BuscarVeiculos(_dadosAgendamento.Veiculo, null, _dadosAgendamento.Transportador);

}

//#endregion

//#region Métodos Globais

function RetornoTransportadorDadosAgendamento(data) {
    _dadosAgendamento.Transportador.codEntity(data.Codigo);
    _dadosAgendamento.Transportador.val(data.Descricao);

    LimparCampo(_dadosAgendamento.Veiculo);
    LimparCampo(_dadosAgendamento.Motorista);
}

function AdicionarDadosAgendamentoClick(e, sender) {
    if (_dadosAgendamento.Veiculo.codEntity() == 0 && _dadosAgendamento.Motorista.codEntity() == 0 && _dadosAgendamento.Transportador.codEntity() == 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário um dos campos.");
        return;
    }

    Salvar(_dadosAgendamento, "AgendamentoColeta/AdicionarDadosAgendamento", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Dados de Agendamento atualizados.");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

//#endregion
