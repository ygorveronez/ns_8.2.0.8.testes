/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />


//#region Variaveis Globais

var _dadosTransporte, _aceiteTransporte, _mensagemEtapaAgendamento, _CRUDDadosTransporte;

//#endregion

//#region Mapeamento Knockout

var DadosTransporte = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, visible: false, val: ko.observable(0) });

    this.Motorista = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.entity, text: "Motorista:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Veiculo = PropertyEntity({ codEntity: ko.observable(0), required: true, type: types.entity, text: "Veículo:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.ApenasGerarPedido = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, type: types.map });

    this.Atualizar = PropertyEntity({ eventClick: AtualizarDadosTransporteClick, type: types.event, text: "Atualizar", visible: ko.observable(true) });
}

var AceiteTransporte = function () {
    this.Situacao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.Veiculo = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.Modelo = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.Motorista = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.Reboque = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.ApenasGerarPedido = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, type: types.map });
}

var MensagemEtapaAgendamento = function () {
    this.ApenasGerarPedido = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, type: types.map });
}

var CRUDDadosTransporte = function () {
    this.Limpar = PropertyEntity({ eventClick: LimparClick, type: types.event, text: "Limpar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Imprimir = PropertyEntity({ eventClick: ImprimirClick, type: types.event, text: "Imprimir", visible: ko.observable(false) });
    this.InformarNotasFiscais = PropertyEntity({ eventClick: InformarNotasFiscaisClick, type: types.event, text: "Informar Notas Fiscais", visible: ko.observable(true) });
}

function LoadDadosTransporte() {
    _dadosTransporte = new DadosTransporte();
    KoBindings(_dadosTransporte, "knockoutDadosTransporte");

    _aceiteTransporte = new AceiteTransporte();
    KoBindings(_aceiteTransporte, "knockoutAceiteTransportador");

    _mensagemEtapaAgendamento = new MensagemEtapaAgendamento();
    KoBindings(_mensagemEtapaAgendamento, "knockoutMensagemEtapaAgendamento");

    _CRUDDadosTransporte = new CRUDDadosTransporte();
    KoBindings(_CRUDDadosTransporte, "knockoutCRUDDadosTransporte");

    new BuscarVeiculos(_dadosTransporte.Veiculo, null, _etapaCarga.Transportador);
    new BuscarMotoristas(_dadosTransporte.Motorista, null, _etapaCarga.Transportador);
}

//#endregion

//#region Métodos Globais

function AtualizarDadosTransporteClick(e, sender) {
    if (_dadosTransporte.Veiculo.codEntity() == 0 && _dadosTransporte.Motorista.codEntity() == 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário selecionar ao menos o motorista ou o veículo para atualizar.");
        return;
    }

    Salvar(_dadosTransporte, "AgendamentoColeta/AtualizarDadosTransporte", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Dados de transporte atualizados.");

                LimparCampos(_dadosTransporte);
                PreencherObjetoKnout(_aceiteTransporte, { Data: retorno.Data.AceiteTransporte });
                _dadosTransporte.Codigo.val(_agendamentoColeta.CodigoCarga.val());

                if (retorno.Data.AvancarEtapa)
                    InformarNotasFiscaisClick();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function InformarNotasFiscaisClick() {
    if (_etapaCarga.TipoOperacao.InformarDadosNotaCte()) {

        executarReST("AgendamentoColeta/SalvarDocumentoParaTransporte", {
            Codigo: _agendamentoColeta.CodigoAgendamento.val(),
            DocumentoParaTransporte: "[]"
        }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    BuscarAgendamento({ Codigo: _agendamentoColeta.CodigoAgendamento.val() });
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
        return;
    }


    executarReST("AgendamentoColeta/InformarNotasFiscais", {
        Codigo: _agendamentoColeta.CodigoAgendamento.val()
    }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                BuscarAgendamento({ Codigo: _agendamentoColeta.CodigoAgendamento.val() });
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

//#endregion
