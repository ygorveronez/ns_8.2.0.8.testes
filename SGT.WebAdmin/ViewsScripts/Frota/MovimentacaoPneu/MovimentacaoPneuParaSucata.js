/// <reference path="MovimentacaoPneu.js" />
/// <reference path="../../Consultas/MotivoSucateamentoPneu.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _isMovimentacaoPneuParaSucataSalva;
var _movimentacaoPneuParaSucata;

/*
 * Declaração das Classes
 */

var MovimentacaoPneuParaSucata = function () {
    this.CodigoEixoPneuOrigem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoEstepeOrigem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoPneu = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoVeiculo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Hodometro = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataHora = PropertyEntity({ text: "*Data/Hora: ", getType: typesKnockout.dateTime, required: true });
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motivo:", idBtnSearch: guid(), required: true });
    this.NumeroFogo = PropertyEntity({ text: "*Número de Fogo:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 500, required: true, enable: false });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.ValorCarcaca = PropertyEntity({ text: "*Valor da Carcaça:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarMovimentacaoPneuParaSucataClick, type: types.event, text: ko.observable("Adicionar"), visible: true });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadMovimentacaoPneuParaSucata() {
    _movimentacaoPneuParaSucata = new MovimentacaoPneuParaSucata();
    KoBindings(_movimentacaoPneuParaSucata, "knockoutMovimentacaoPneuParaSucata");

    new BuscarMotivoSucateamentoPneu(_movimentacaoPneuParaSucata.Motivo);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarMovimentacaoPneuParaSucataClick() {
    _movimentacaoPneuParaSucata.CodigoVeiculo.val(_veiculo.Codigo.val());
    _movimentacaoPneuParaSucata.Hodometro.val(_dadosVeiculo.Hodometro.val());
    Salvar(_movimentacaoPneuParaSucata, "MovimentacaoPneu/EnviarParaSucata", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _isMovimentacaoPneuParaSucataSalva = true;

                fecharModalMovimentacaoPneuParaSucata();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções Públicas
 */

function exibirModalMovimentacaoPneuParaSucata() {
    preencherMovimentacaoPneuParaSucata();

    _isMovimentacaoPneuParaSucataSalva = false;

    Global.abrirModal('divModalMovimentacaoPneuParaSucata');
    $("#divModalMovimentacaoPneuParaSucata").one('hidden.bs.modal', function () {
        if (_isMovimentacaoPneuParaSucataSalva)
            efetivarMovimentacaoPneu();
        else
            reverterMovimentacaoPneu();

        LimparCampos(_movimentacaoPneuParaSucata);
        limparCamposMovimentacaoPneu();
    });
}

/*
 * Declaração das Funções Privadas
 */

function fecharModalMovimentacaoPneuParaSucata() {
    Global.fecharModal('divModalMovimentacaoPneuParaSucata');
}

function preencherMovimentacaoPneuParaSucata() {
    _movimentacaoPneuParaSucata.CodigoPneu.val(_pneuAdicionar.CodigoPneu.val());
    _movimentacaoPneuParaSucata.DataHora.val(Global.DataHoraAtual());
    _movimentacaoPneuParaSucata.NumeroFogo.val(_pneuAdicionar.NumeroFogo.val());

    if (_tipoContainerComPneu == EnumTipoContainerPneu.Estepe)
        _movimentacaoPneuParaSucata.CodigoEstepeOrigem.val(_pneuRemover.Codigo.val());
    else if (_tipoContainerComPneu == EnumTipoContainerPneu.Veiculo)
        _movimentacaoPneuParaSucata.CodigoEixoPneuOrigem.val(_pneuRemover.Codigo.val());
}