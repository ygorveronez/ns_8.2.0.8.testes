/// <reference path="Estoque.js" />
/// <reference path="MovimentacaoPneuParaEstoque.js" />
/// <reference path="MovimentacaoPneuParaReforma.js" />
/// <reference path="MovimentacaoPneuParaSucata.js" />
/// <reference path="MovimentacaoPneuParaVeiculo.js" />
/// <reference path="MovimentacaoPneuReformaParaEstoque.js" />
/// <reference path="MovimentacaoPneuReformaParaEstoqueProduto.js" />
/// <reference path="Reforma.js" />
/// <reference path="Veiculo.js" />
/// <reference path="../../Enumeradores/EnumTipoContainerPneu.js" />

/*
 * Declaração de Evento Global
 */

$('body').click(function () {
    ocultarExibicaoTooltip();
});

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _indicePneuRemovidoEstoque;
var _indicePneuRemovidoReforma;
var _pneuAdicionar;
var _pneuEditar;
var _pneuRemover;
var _tipoContainerComPneu;
var _tipoContainerSemPneu;
var _$containerComPneu;
var _$containerSemPneu;
var _$containerPneuArrastado;
var _tipoContainerPneuArrastado;
var _$containerPneuDestino;
var _tipoContainerPneuDestino;
var _pneuArrastado;
var _pneuDestino;

/*
 * Declaração das Classes
 */

var Pneu = function () {
    this.CodigoPneu = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Almoxarifado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.BandaRodagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.NumeroOS = PropertyEntity({ getType: typesKnockout.int });
    this.ValorMaoObra = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true } });
    this.ValorProdutos = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true } });
    this.LocalManutencao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.Modelo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.KmAtualRodado = PropertyEntity({});
    this.Marca = PropertyEntity({});
    this.NumeroFogo = PropertyEntity({});
    this.Sulco = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false } });
    this.Vida = PropertyEntity({});
    this.Removido = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.OrdemServico = PropertyEntity({});
    this.DataMovimentacao = PropertyEntity({});
}

/*
 * Declaração das Funções de Inicialização
 */

function loadMovimentacaoPneu() {
    loadVeiculo();
    loadEstoque();
    loadReforma();
    adicionarEventosDroppableEnvio();
    loadMovimentacaoPneuParaVeiculo();
    loadMovimentacaoPneuParaEstoque();
    loadMovimentacaoPneuParaSucata();
    loadMovimentacaoPneuParaReforma();
    loadMovimentacaoPneuReformaParaEstoque();
    loadMovimentacaoPneuParaRodizio();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function pneuSoltado(event, ui) {
    _$containerComPneu = $("#" + ui.draggable[0].id);
    _$containerSemPneu = $("#" + event.target.id);
    _tipoContainerComPneu = _$containerComPneu.data("tipo-container-pneu");
    _tipoContainerSemPneu = _$containerSemPneu.data("tipo-container-pneu");

    if (isPermitirSoltarPneu()) {
        _pneuRemover = obterPneu(_$containerComPneu, _tipoContainerComPneu);
        _pneuAdicionar = obterPneu(_$containerSemPneu, _tipoContainerSemPneu);

        adicionarPneuContainerSemPneu();
        removerPneuContainerComPneu();
        exibirModalMovimentacaoPneu();
    }
}

function pneuSoltadoRodizio(event, ui) {
    _$containerPneuArrastado = $("#" + ui.draggable[0].id);
    _tipoContainerPneuArrastado = _$containerPneuArrastado.data("tipo-container-pneu");

    _$containerPneuDestino = $("#" + event.target.id);
    _tipoContainerPneuDestino = _$containerPneuDestino.data("tipo-container-pneu");

    _pneuAdicionar = obterPneu(_$containerPneuArrastado, _tipoContainerPneuArrastado);
    _pneuRemover = obterPneu(_$containerPneuDestino, _tipoContainerPneuDestino);

    exibirModalMovimentacaoPneuParaRodizio();
}

/*
 * Declaração das Funções Públicas
 */

function adicionarEventosDraggableEClickPneu(idContainerPneu, adicionarEventoDuploClick) {
    var idTimeout;

    $("#" + idContainerPneu).draggable({
        cursor: "move",
        revert: true,
        start: function (event, ui) {
            if (idTimeout) {
                clearTimeout(idTimeout);
                idTimeout = undefined;
            }

            ocultarExibicaoTooltip();

            if ($(event.target).data("tipo-container-pneu") == EnumTipoContainerPneu.Reforma)
                $("#container-movimentacao-pneu").addClass("container-movimentacao-pneu-reforma");

            ui.helper.addClass("conteudo-pneu-hover-draggable");
        },
        stop: function (event, ui) {
            $("#container-movimentacao-pneu").removeClass("container-movimentacao-pneu-reforma");
            ui.helper.removeClass("conteudo-pneu-hover-draggable");
        }
    });

    $("#" + idContainerPneu).droppable(obterObjetoDroppablePneuRodizio());

    $("#" + idContainerPneu).on("click", function (event) {
        event.stopPropagation();

        if (!idTimeout) {
            idTimeout = setTimeout(function () {
                idTimeout = undefined;
                controlarExibicaoTooltip(idContainerPneu);
            }, 300);
        }
    });

    if (adicionarEventoDuploClick) {
        $("#" + idContainerPneu).on("dblclick", function () {
            event.stopPropagation();

            if (idTimeout) {
                clearTimeout(idTimeout);
                idTimeout = undefined;

                efetuarMovimentacaoViradaPneuVeiculo(idContainerPneu);
            }
        });
    }
}

function adicionarEventoDroppablePneu(idContainerPneu) {
    $("#" + idContainerPneu).droppable(obterObjetoDroppablePneu());
}

function copiarPneu(pneuAdicionar, pneuRemover) {
    pneuAdicionar.CodigoPneu.val(pneuRemover.CodigoPneu.val());
    pneuAdicionar.Almoxarifado.codEntity(pneuRemover.Almoxarifado.codEntity());
    pneuAdicionar.Almoxarifado.val(pneuRemover.Almoxarifado.val());
    pneuAdicionar.BandaRodagem.codEntity(pneuRemover.BandaRodagem.codEntity());
    pneuAdicionar.BandaRodagem.val(pneuRemover.BandaRodagem.val());
    pneuAdicionar.KmAtualRodado.val(pneuRemover.KmAtualRodado.val());
    pneuAdicionar.Marca.val(pneuRemover.Marca.val());
    pneuAdicionar.NumeroFogo.val(pneuRemover.NumeroFogo.val());
    pneuAdicionar.Sulco.val(pneuRemover.Sulco.val());
    pneuAdicionar.Vida.val(pneuRemover.Vida.val());
    pneuAdicionar.Modelo.codEntity(pneuRemover.Modelo.codEntity());
    pneuAdicionar.Modelo.val(pneuRemover.Modelo.val());
    pneuAdicionar.DataMovimentacao.val(pneuRemover.DataMovimentacao.val());
}

function trocarPneu(pneuAdicionar, pneuRemover) {
    var pneuA = pneuRemover;
    var pneuR = pneuAdicionar;

    pneuAdicionar.CodigoPneu.val(pneuA.CodigoPneu.val());
    pneuAdicionar.Almoxarifado.codEntity(pneuA.Almoxarifado.codEntity());
    pneuAdicionar.Almoxarifado.val(pneuA.Almoxarifado.val());
    pneuAdicionar.BandaRodagem.codEntity(pneuA.BandaRodagem.codEntity());
    pneuAdicionar.BandaRodagem.val(pneuA.BandaRodagem.val());
    pneuAdicionar.KmAtualRodado.val(pneuA.KmAtualRodado.val());
    pneuAdicionar.Marca.val(pneuA.Marca.val());
    pneuAdicionar.NumeroFogo.val(pneuA.NumeroFogo.val());
    pneuAdicionar.Sulco.val(pneuA.Sulco.val());
    pneuAdicionar.Vida.val(pneuA.Vida.val());
    pneuAdicionar.Modelo.codEntity(pneuA.Modelo.codEntity());
    pneuAdicionar.Modelo.val(pneuA.Modelo.val());
    pneuAdicionar.DataMovimentacao.val(pneuA.DataMovimentacao.val());

    limparPneu(pneuRemover);

    pneuRemover.CodigoPneu.val(pneuR.CodigoPneu.val());
    pneuRemover.Almoxarifado.codEntity(pneuR.Almoxarifado.codEntity());
    pneuRemover.Almoxarifado.val(pneuR.Almoxarifado.val());
    pneuRemover.BandaRodagem.codEntity(pneuR.BandaRodagem.codEntity());
    pneuRemover.BandaRodagem.val(pneuR.BandaRodagem.val());
    pneuRemover.KmAtualRodado.val(pneuR.KmAtualRodado.val());
    pneuRemover.Marca.val(pneuR.Marca.val());
    pneuRemover.NumeroFogo.val(pneuR.NumeroFogo.val());
    pneuRemover.Sulco.val(pneuR.Sulco.val());
    pneuRemover.Vida.val(pneuR.Vida.val());
    pneuRemover.Modelo.codEntity(pneuR.Modelo.codEntity());
    pneuRemover.Modelo.val(pneuR.Modelo.val());
    pneuRemover.DataMovimentacao.val(pneuR.DataMovimentacao.val());
}

function efetivarMovimentacaoPneu() {
    if (_indicePneuRemovidoEstoque >= 0)
        atualizarEstoque();
    else if (_indicePneuRemovidoReforma >= 0)
        atualizarReforma();
}

function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
}

function exibirModalMovimentacaoPneu() {
    switch (_tipoContainerSemPneu) {
        case EnumTipoContainerPneu.Estepe:
        case EnumTipoContainerPneu.Veiculo:
            exibirModalMovimentacaoPneuParaVeiculo();
            break;

        case EnumTipoContainerPneu.EnvioEstoque:
            if (_tipoContainerComPneu == EnumTipoContainerPneu.Reforma)
                exibirModalMovimentacaoPneuReformaParaEstoque();
            else
                exibirModalMovimentacaoPneuParaEstoque();

            break;

        case EnumTipoContainerPneu.EnvioSucata:
            exibirModalMovimentacaoPneuParaSucata();
            break;

        case EnumTipoContainerPneu.EnvioReforma:
            exibirModalMovimentacaoPneuParaReforma();
            break;
    }
}

function limparCamposMovimentacaoPneu() {
    _indicePneuRemovidoEstoque = undefined;
    _indicePneuRemovidoReforma = undefined;
    _pneuAdicionar = undefined;
    _pneuEditar = undefined;
    _pneuRemover = undefined;
    _tipoContainerComPneu = undefined;
    _tipoContainerSemPneu = undefined;
    _$containerSemPneu = undefined;
    _$containerComPneu = undefined;
    _$containerPneuArrastado = undefined;
    _tipoContainerPneuArrastado = undefined;
    _$containerPneuDestino = undefined;
    _tipoContainerPneuDestino = undefined;
    _pneuArrastado = undefined;
    _pneuDestino = undefined;
}

function reverterMovimentacaoPneu() {
    if ((_indicePneuRemovidoEstoque >= 0) || (_indicePneuRemovidoReforma >= 0)) {
        var pneu = new Pneu();

        copiarPneu(pneu, _pneuAdicionar);

        if (_indicePneuRemovidoEstoque >= 0)
            adicionarPneuEstoquePorIndice(_indicePneuRemovidoEstoque);
        else
            adicionarPneuReformaPorIndice(_indicePneuRemovidoReforma);
    }
    else {
        copiarPneu(_pneuRemover, _pneuAdicionar);
        adicionarEventosDraggableEClickPneu(_$containerComPneu[0].id, (_tipoContainerComPneu == EnumTipoContainerPneu.Veiculo));
    }

    limparPneu(_pneuAdicionar);

    if (EnumTipoContainerPneu.IsMovimentacaoParaVeiculo(_tipoContainerSemPneu))
        adicionarEventoDroppablePneu(_$containerSemPneu[0].id);
}

/*
 * Declaração das Funções Privadas
 */

function adicionarEventosDroppableEnvio() {
    $("#envio-pneu-estoque").droppable(obterObjetoDroppableEnvioEstoque());
    $("#envio-pneu-sucata").droppable(obterObjetoDroppableEnvioSucata());
    $("#envio-pneu-reforma").droppable(obterObjetoDroppableEnvioReforma());
}

function adicionarPneuContainerSemPneu() {
    if (_pneuAdicionar) {
        copiarPneu(_pneuAdicionar, _pneuRemover);
        adicionarEventosDraggableEClickPneu(_$containerSemPneu[0].id, (_tipoContainerSemPneu == EnumTipoContainerPneu.Veiculo));
    }
    else {
        _pneuAdicionar = new Pneu();
        copiarPneu(_pneuAdicionar, _pneuRemover);
    }
}

function controlarExibicaoTooltip(idContainerPneu) {
    var $tooltip = $("#" + idContainerPneu + " .pneu-tooltip");

    if ($tooltip) {
        var hasClass = $tooltip.hasClass("pneu-tooltip-hover");
        var posicaoPneu = $tooltip.data("posicao-pneu");
        var posicaoPneuColuna = ((posicaoPneu - 1) % 8) + 1;

        ocultarExibicaoTooltip();

        $tooltip.addClass("pneu-tooltip-posicao-" + posicaoPneuColuna);
        $tooltip.toggleClass("pneu-tooltip-hover", !hasClass);
    }
}

function efetuarMovimentacaoViradaPneuVeiculo(idContainerPneu) {
    ocultarExibicaoTooltip();

    _$containerComPneu = $("#" + idContainerPneu);
    _tipoContainerComPneu = _$containerComPneu.data("tipo-container-pneu");
    _pneuEditar = obterPneu(_$containerComPneu, _tipoContainerComPneu);

    exibirModalMovimentacaoViradaPneuVeiculo();
}

function isPermitirSoltarPneu() {
    if (_tipoContainerComPneu === EnumTipoContainerPneu.Reforma) {
        return (_tipoContainerSemPneu == EnumTipoContainerPneu.EnvioEstoque) || (_tipoContainerSemPneu == EnumTipoContainerPneu.EnvioSucata);
    }

    return true;
}

function limparPneu(pneuRemover) {
    pneuRemover.CodigoPneu.val(pneuRemover.CodigoPneu.def);
    pneuRemover.Almoxarifado.codEntity(pneuRemover.Almoxarifado.defCodEntity);
    pneuRemover.Almoxarifado.val(pneuRemover.Almoxarifado.def);
    pneuRemover.BandaRodagem.codEntity(pneuRemover.BandaRodagem.defCodEntity);
    pneuRemover.BandaRodagem.val(pneuRemover.BandaRodagem.def);
    pneuRemover.KmAtualRodado.val(pneuRemover.KmAtualRodado.def);
    pneuRemover.Marca.val(pneuRemover.Marca.def);
    pneuRemover.NumeroFogo.val(pneuRemover.NumeroFogo.def);
    pneuRemover.Sulco.val(pneuRemover.Sulco.def);
    pneuRemover.Vida.val(pneuRemover.Vida.def);
    pneuRemover.Modelo.codEntity(pneuRemover.Modelo.codEntity());
    pneuRemover.Modelo.val(pneuRemover.Modelo.val());
    pneuRemover.DataMovimentacao.val(pneuRemover.DataMovimentacao.val());
}

function ocultarExibicaoTooltip() {
    $(".pneu-tooltip").removeClass("pneu-tooltip-hover");
}

function obterObjetoDroppableEnvioEstoque() {
    return {
        drop: pneuSoltado,
        hoverClass: "envio-pneu-estoque-hover"
    };
}

function obterObjetoDroppableEnvioReforma() {
    return {
        drop: pneuSoltado,
        hoverClass: "envio-pneu-reforma-hover"
    };
}

function obterObjetoDroppableEnvioSucata() {
    return {
        drop: pneuSoltado,
        hoverClass: "envio-pneu-sucata-hover"
    };
}

function obterObjetoDroppablePneu() {
    return {
        drop: pneuSoltado,
        hoverClass: "conteudo-pneu-hover"
    };
}

function obterObjetoDroppablePneuRodizio() {
    return {
        drop: pneuSoltadoRodizio,
        hoverClass: "conteudo-pneu-hover"
    };
}

function obterPneu($containerPneu, tipoContainer) {
    switch (tipoContainer) {
        case EnumTipoContainerPneu.Veiculo:
            return obterPneuVeiculo($containerPneu.data("codigo-eixo"), $containerPneu.data("codigo-eixo-pneu"));

        case EnumTipoContainerPneu.Estepe:
            return obterPneuEstepe($containerPneu.data("codigo-estepe"));

        case EnumTipoContainerPneu.Estoque:
            return obterPneuEstoque($containerPneu.data("codigo-pneu"));

        case EnumTipoContainerPneu.Reforma:
            return obterPneuReforma($containerPneu.data("codigo-pneu"));

        default:
            return undefined;
    }
}

function removerPneuContainerComPneu() {
    switch (_tipoContainerComPneu) {
        case EnumTipoContainerPneu.Estoque:
            _indicePneuRemovidoEstoque = removerPneuEstoque(_pneuRemover.CodigoPneu.val());
            break;

        case EnumTipoContainerPneu.Reforma:
            _indicePneuRemovidoReforma = removerPneuReforma(_pneuRemover.CodigoPneu.val());
            break;

        default:
            limparPneu(_pneuRemover);
            adicionarEventoDroppablePneu(_$containerComPneu[0].id);
    }
}
