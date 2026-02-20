/// <reference path="MovimentacaoPneu.js" />
/// <reference path="../../Consultas/Almoxarifado.js" />
/// <reference path="../../Enumeradores/EnumTipoContainerPneu.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _isMovimentacaoPneuParaEstoqueSalva;
var _movimentacaoPneuParaEstoque;

/*
 * Declaração das Classes
 */

var MovimentacaoPneuParaEstoque = function () {
    MovimentacaoPneuDadosAdicionais.call(this);

    this.CodigoAlmoxarifadoOrigem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoEixoPneuOrigem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoEstepeOrigem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoPneu = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoVeiculo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Hodometro = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Almoxarifado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Almoxarifado:", idBtnSearch: guid(), required: true });
    this.DataHora = PropertyEntity({ text: "*Data/Hora: ", getType: typesKnockout.dateTime, required: true });
    this.NumeroFogo = PropertyEntity({ text: "*Número de Fogo:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 500, required: true, enable: false });

    this.Adicionar = PropertyEntity({ eventClick: adicionarMovimentacaoPneuParaEstoqueClick, type: types.event, text: ko.observable("Adicionar"), visible: true });
}

MovimentacaoPneuParaEstoque.prototype = Object.create(MovimentacaoPneuDadosAdicionais.prototype);
MovimentacaoPneuParaEstoque.prototype.constructor = MovimentacaoPneuParaEstoque;

/*
 * Declaração das Funções de Inicialização
 */

function loadMovimentacaoPneuParaEstoque() {
    _movimentacaoPneuParaEstoque = new MovimentacaoPneuParaEstoque();
    KoBindings(_movimentacaoPneuParaEstoque, "knockoutMovimentacaoPneuParaEstoque");

    new BuscarAlmoxarifado(_movimentacaoPneuParaEstoque.Almoxarifado);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarMovimentacaoPneuParaEstoqueClick() {
    if (ValidarCamposObrigatorios(_movimentacaoPneuParaEstoque)) {
        if (validarAlmoxarifado()) {
            _movimentacaoPneuParaEstoque.CodigoVeiculo.val(_veiculo.Codigo.val());
            _movimentacaoPneuParaEstoque.Hodometro.val(_dadosVeiculo.Hodometro.val());
            executarReST("MovimentacaoPneu/EnviarParaEstoque", RetornarObjetoPesquisa(_movimentacaoPneuParaEstoque), function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        _isMovimentacaoPneuParaEstoqueSalva = true;

                        fecharModalMovimentacaoPneuParaEstoque();
                    }
                    else
                        exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
        }
        else
            exibirMensagem(tipoMensagem.atencao, "Atenção", "Pro favor, informe um almoxarifado diferente do atual.");
    }
    else
        exibirMensagemCamposObrigatorio();
}

/*
 * Declaração das Funções Públicas
 */

function exibirModalMovimentacaoPneuParaEstoque() {
    preencherMovimentacaoPneuParaEstoque();

    _isMovimentacaoPneuParaEstoqueSalva = false;

    Global.abrirModal('divModalMovimentacaoPneuParaEstoque');
    $("#divModalMovimentacaoPneuParaEstoque").one('hidden.bs.modal', function () {
        if (_isMovimentacaoPneuParaEstoqueSalva) {
            atualizardadosPneuEstoque();
            adicionarPneuEstoque(_pneuAdicionar);
            efetivarMovimentacaoPneu();
        }
        else
            reverterMovimentacaoPneu();

        LimparCampos(_movimentacaoPneuParaEstoque);
        limparCamposMovimentacaoPneu();
    });
}

/*
 * Declaração das Funções Privadas
 */

function atualizardadosPneuEstoque() {
    _pneuAdicionar.Almoxarifado.codEntity(_movimentacaoPneuParaEstoque.Almoxarifado.codEntity());
    _pneuAdicionar.Almoxarifado.val(_movimentacaoPneuParaEstoque.Almoxarifado.val());
    _pneuAdicionar.Sulco.val(_movimentacaoPneuParaEstoque.SulcoAtual.val());
}

function fecharModalMovimentacaoPneuParaEstoque() {
    Global.fecharModal('divModalMovimentacaoPneuParaEstoque');
}

function preencherMovimentacaoPneuParaEstoque() {
    _movimentacaoPneuParaEstoque.CodigoAlmoxarifadoOrigem.val(_pneuAdicionar.Almoxarifado.codEntity());
    _movimentacaoPneuParaEstoque.CodigoPneu.val(_pneuAdicionar.CodigoPneu.val());
    _movimentacaoPneuParaEstoque.DataHora.val(Global.DataHoraAtual());
    _movimentacaoPneuParaEstoque.NumeroFogo.val(_pneuAdicionar.NumeroFogo.val());
    _movimentacaoPneuParaEstoque.SulcoAnterior.val(_pneuAdicionar.Sulco.val());

    if (_tipoContainerComPneu != EnumTipoContainerPneu.Estoque) {
        _movimentacaoPneuParaEstoque.Almoxarifado.codEntity(_pneuAdicionar.Almoxarifado.codEntity());
        _movimentacaoPneuParaEstoque.Almoxarifado.val(_pneuAdicionar.Almoxarifado.val());
    }

    if (_tipoContainerComPneu == EnumTipoContainerPneu.Estepe)
        _movimentacaoPneuParaEstoque.CodigoEstepeOrigem.val(_pneuRemover.Codigo.val());
    else if (_tipoContainerComPneu == EnumTipoContainerPneu.Veiculo)
        _movimentacaoPneuParaEstoque.CodigoEixoPneuOrigem.val(_pneuRemover.Codigo.val());

    _movimentacaoPneuParaEstoque.UtilizarDadosAdicionais.val(
        (_tipoContainerComPneu == EnumTipoContainerPneu.Estepe) ||
        (_tipoContainerComPneu == EnumTipoContainerPneu.Veiculo)
    );
}

function validarAlmoxarifado() {
    return (
        (_tipoContainerComPneu != EnumTipoContainerPneu.Estoque) ||
        (_movimentacaoPneuParaEstoque.Almoxarifado.codEntity() != _movimentacaoPneuParaEstoque.CodigoAlmoxarifadoOrigem.val())
    );
}
