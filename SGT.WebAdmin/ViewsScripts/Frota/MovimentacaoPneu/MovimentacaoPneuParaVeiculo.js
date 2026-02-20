/// <reference path="MovimentacaoPneu.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _isMovimentacaoPneuParaVeiculoSalva;
var _movimentacaoPneuParaVeiculo;

/*
 * Declaração das Classes
 */

var MovimentacaoPneuParaVeiculo = function () {
    MovimentacaoPneuDadosAdicionais.call(this);

    this.CodigoEixoPneu = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoEixoPneuDestino = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoEixoPneuOrigem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoEstepeDestino = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoEstepeOrigem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoPneu = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoVeiculo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });    
    this.Hodometro = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataHora = PropertyEntity({ text: "*Data/Hora: ", getType: typesKnockout.dateTime, required: true });
    this.NumeroFogo = PropertyEntity({ text: "*Número de Fogo:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 500, required: true, enable: false });

    this.Adicionar = PropertyEntity({ eventClick: adicionarMovimentacaoPneuParaVeiculoClick, type: types.event, text: ko.observable("Adicionar"), visible: true });
}

MovimentacaoPneuParaVeiculo.prototype = Object.create(MovimentacaoPneuDadosAdicionais.prototype);
MovimentacaoPneuParaVeiculo.prototype.constructor = MovimentacaoPneuParaVeiculo;

/*
 * Declaração das Funções de Inicialização
 */

function loadMovimentacaoPneuParaVeiculo() {
    _movimentacaoPneuParaVeiculo = new MovimentacaoPneuParaVeiculo();
    KoBindings(_movimentacaoPneuParaVeiculo, "knockoutMovimentacaoPneuParaVeiculo");
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarMovimentacaoPneuParaVeiculoClick() {
    _movimentacaoPneuParaVeiculo.Hodometro.val(_dadosVeiculo.Hodometro.val());

    Salvar(_movimentacaoPneuParaVeiculo, "MovimentacaoPneu/MovimentarPneuParaVeiculo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _isMovimentacaoPneuParaVeiculoSalva = true;

                fecharModalMovimentacaoPneuParaVeiculo();

                if (_pneuAdicionar != undefined && _pneuAdicionar != null)
                    _pneuAdicionar.DataMovimentacao.val(_movimentacaoPneuParaVeiculo.DataHora.val());
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

function exibirModalMovimentacaoPneuParaVeiculo() {
    preencherMovimentacaoPneuParaVeiculo();

    _isMovimentacaoPneuParaVeiculoSalva = false;

    Global.abrirModal('divModalMovimentacaoPneuParaVeiculo');
    $("#divModalMovimentacaoPneuParaVeiculo").one('hidden.bs.modal', function () {
        if (_isMovimentacaoPneuParaVeiculoSalva) {
            efetivarMovimentacaoPneu();
            atualizarPneuMovidoParaVeiculo(_pneuAdicionar);
        }
        else
            reverterMovimentacaoPneu();

        LimparCampos(_movimentacaoPneuParaVeiculo);
        limparCamposMovimentacaoPneu();
    });
}

function exibirModalMovimentacaoViradaPneuVeiculo() {
    preencherMovimentacaoViradaPneuVeiculo();

    _isMovimentacaoPneuParaVeiculoSalva = false;

    Global.abrirModal('divModalMovimentacaoPneuParaVeiculo');
    $("#divModalMovimentacaoPneuParaVeiculo").one('hidden.bs.modal', function () {
        if (_isMovimentacaoPneuParaVeiculoSalva)
            atualizarPneuMovidoParaVeiculo(_pneuEditar);

        LimparCampos(_movimentacaoPneuParaVeiculo);
        limparCamposMovimentacaoPneu();
    });
}

/*
 * Declaração das Funções Privadas
 */

function atualizarPneuMovidoParaVeiculo(pneu) {
    pneu.Sulco.val(_movimentacaoPneuParaVeiculo.SulcoAtual.val());
}

function fecharModalMovimentacaoPneuParaVeiculo() {
    Global.fecharModal('divModalMovimentacaoPneuParaVeiculo');
}

function preencherMovimentacaoPneuParaVeiculo() {
    _movimentacaoPneuParaVeiculo.CodigoPneu.val(_pneuAdicionar.CodigoPneu.val());
    _movimentacaoPneuParaVeiculo.CodigoVeiculo.val(_veiculo.Codigo.val());
    _movimentacaoPneuParaVeiculo.DataHora.val(Global.DataHoraAtual());
    _movimentacaoPneuParaVeiculo.NumeroFogo.val(_pneuAdicionar.NumeroFogo.val());
    _movimentacaoPneuParaVeiculo.SulcoAnterior.val(_pneuAdicionar.Sulco.val());

    if (_tipoContainerComPneu == EnumTipoContainerPneu.Estepe)
        _movimentacaoPneuParaVeiculo.CodigoEstepeOrigem.val(_pneuRemover.Codigo.val());
    else if (_tipoContainerComPneu == EnumTipoContainerPneu.Veiculo)
        _movimentacaoPneuParaVeiculo.CodigoEixoPneuOrigem.val(_pneuRemover.Codigo.val());

    if (_tipoContainerSemPneu == EnumTipoContainerPneu.Estepe)
        _movimentacaoPneuParaVeiculo.CodigoEstepeDestino.val(_pneuAdicionar.Codigo.val());
    else if (_tipoContainerSemPneu == EnumTipoContainerPneu.Veiculo)
        _movimentacaoPneuParaVeiculo.CodigoEixoPneuDestino.val(_pneuAdicionar.Codigo.val());
}

function preencherMovimentacaoViradaPneuVeiculo() {
    _movimentacaoPneuParaVeiculo.CodigoPneu.val(_pneuEditar.CodigoPneu.val());
    _movimentacaoPneuParaVeiculo.CodigoVeiculo.val(_veiculo.Codigo.val());
    _movimentacaoPneuParaVeiculo.DataHora.val(Global.DataHoraAtual());
    _movimentacaoPneuParaVeiculo.NumeroFogo.val(_pneuEditar.NumeroFogo.val());
    _movimentacaoPneuParaVeiculo.SulcoAnterior.val(_pneuEditar.Sulco.val());
    _movimentacaoPneuParaVeiculo.CodigoEixoPneu.val(_pneuEditar.Codigo.val());
}
