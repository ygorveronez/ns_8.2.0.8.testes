/// <reference path="NegociacaoBaixaTituloReceber.js" />
/// <reference path="..\..\Consultas\Cheque.js" />
/// <reference path="..\..\Enumeradores\EnumBaixaTituloReceber.js" />
/// <reference path="../../Enumeradores/EnumTipoCheque.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridChequeBaixa;
var _chequeBaixa;
var _cheques = new Array();

/*
 * Declaração das Classes
 */

var ChequeBaixa = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.ChequeBaixa = PropertyEntity({ type: types.event, text: "Adicionar Cheques", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.AdicionarCheque = PropertyEntity({ eventClick: AdicionarChequeClick, type: types.event, text: "Novo Cheque", visible: ko.observable(true), enable: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridChequeBaixa() {
    if (_gridChequeBaixa)
        _gridChequeBaixa.Destroy();

    var menuOpcoes = null;

    if (isPermitirGerenciarCheques()) {
        var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: excluirChequeClick };

        menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoExcluir] };
    }

    var header = [
        { data: "Codigo", visible: false },
        { data: "NumeroCheque", title: "Número", width: "10%", className: "text-align-center" },
        { data: "Banco", title: "Banco", width: "20%" },
        { data: "Pessoa", title: "Pessoa", width: "20%" },
        { data: "Status", title: "Status", width: "12%" },
        { data: "Tipo", title: "Tipo", width: "12%" },
        { data: "Valor", title: "Valor", width: "12%", className: "text-align-right" }
    ];

    _gridChequeBaixa = new BasicDataTable(_chequeBaixa.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _chequeBaixa.ChequeBaixa.basicTable = _gridChequeBaixa;
}

function AdicionarChequeClick(e, sender) {
    limparCamposCheque();

    Global.abrirModal('divModalCheque');
}

function LoadChequeBaixa() {
    _chequeBaixa = new ChequeBaixa();
    KoBindings(_chequeBaixa, "knockoutChequesNegociacaoBaixaTituloReceber");

    loadGridChequeBaixa();

    new BuscarCheque(_chequeBaixa.ChequeBaixa, adicionarCheques, _gridChequeBaixa);//, EnumTipoCheque.obterListaTiposBaixaTituloReceber());

    recarregarGridChequeBaixa(new Array());
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function excluirChequeClick(chequeSelecionado) {
    executarReST("BaixaTituloReceberNovo/ExcluirChequePorCodigo", { Codigo: chequeSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                excluirChequeLocal(chequeSelecionado.Codigo);
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

/*W
 * Declaração das Funções
 */

function adicionarCheques(chequesSelecionadosPesquisa) {
    var listaCodigoCheque = new Array();

    for (var i = 0; i < chequesSelecionadosPesquisa.length; i++)
        listaCodigoCheque.push(chequesSelecionadosPesquisa[i].Codigo);

    var dados = {
        Codigo: _baixaTituloReceber.Codigo.val(),
        Cheques: JSON.stringify(listaCodigoCheque)
    };

    executarReST("BaixaTituloReceberNovo/AdicionarDadosCheque", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                carregarDadosChequeBaixa();
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function carregarDadosChequeBaixa() {
    executarReST("BaixaTituloReceberNovo/CarregarDadosCheque", { Codigo: _baixaTituloReceber.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                recarregarChequeBaixa(retorno.Data);
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function excluirChequeLocal(codigo) {
    var cheques = _chequeBaixa.ChequeBaixa.basicTable.BuscarRegistros();

    for (var i = 0; i < cheques.length; i++) {
        if (codigo == cheques[i].Codigo) {
            cheques.splice(i, 1);
            break;
        }
    }

    _chequeBaixa.ChequeBaixa.basicTable.CarregarGrid(cheques);
}

function isPermitirGerenciarCheques() {
    return (_baixaTituloReceber.Etapa.val() == EnumEtapasBaixaTituloReceber.Iniciada) || (_baixaTituloReceber.Etapa.val() == EnumEtapasBaixaTituloReceber.EmNegociacao);
}

function LimparCamposChequeBaixa() {
    LimparCampos(_chequeBaixa);

    recarregarChequeBaixa(new Array());
}

function recarregarChequeBaixa(cheques) {
    loadGridChequeBaixa();
    recarregarGridChequeBaixa(cheques);

    _chequeBaixa.ChequeBaixa.visible(isPermitirGerenciarCheques());
}

function recarregarGridChequeBaixa(cheques) {
    _gridChequeBaixa.CarregarGrid(cheques);
    _cheques = new Array();

    for (var i = 0; i < cheques.length; i++) {
        _cheques.push(cheques[i]);
    }
}