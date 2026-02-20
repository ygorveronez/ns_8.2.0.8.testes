/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFechamentoPontuacao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _fechamentoPontuacaoTransportador;
var _fechamentoPontuacaoTransportadorDetalhes;
var _gridFechamentoPontuacaoTransportador;
var _gridFechamentoPontuacaoTransportadorDetalhes;

/*
 * Declaração das Classes
 */

var FechamentoPontuacaoTransportador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.AguardandoFinalizacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(true), def: true });
}

var FechamentoPontuacaoTransportadorDetalhes = function () {
    this.Pontuacao = PropertyEntity({ text: "Pontuação Total: " });
    this.Regras = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.Regras.val.subscribe(function (regras) {
        _gridFechamentoPontuacaoTransportadorDetalhes.CarregarGrid(regras);
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridFechamentoPontuacaoTransportador() {
    var opcaoDetalhes = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: exibirDetalhesPontuacaoClick, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDetalhes] }

    _gridFechamentoPontuacaoTransportador = new GridViewExportacao("grid-fechamento-pontuacao-transportador", "FechamentoPontuacao/PesquisaPontuacao", _fechamentoPontuacaoTransportador, menuOpcoes);
    _gridFechamentoPontuacaoTransportador.CarregarGrid();
}

function loadGridFechamentoPontuacaoTransportadorDetalhes() {
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "70%", orderable: false },
        { data: "Pontuacao", title: "Pontuacao", width: "30%", orderable: false, className: 'text-align-center' }
    ];
    var menuOpcoes = null;

    _gridFechamentoPontuacaoTransportadorDetalhes = new BasicDataTable("grid-fechamento-pontuacao-transportador-detalhes", header, menuOpcoes);
}

function loadFechamentoPontuacaoTransportador() {
    _fechamentoPontuacaoTransportador = new FechamentoPontuacaoTransportador();
    KoBindings(_fechamentoPontuacaoTransportador, "knockoutFechamentoPontuacaoTransportador");

    _fechamentoPontuacaoTransportadorDetalhes = new FechamentoPontuacaoTransportadorDetalhes();
    KoBindings(_fechamentoPontuacaoTransportadorDetalhes, "knockoutFechamentoPontuacaoTransportadorDetalhes");

    loadGridFechamentoPontuacaoTransportador();
    loadGridFechamentoPontuacaoTransportadorDetalhes();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function exibirDetalhesPontuacaoClick(registroSelecionado) {
    executarReST("FechamentoPontuacao/BuscarDetalhesPontuacaoPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_fechamentoPontuacaoTransportadorDetalhes, retorno);
                exibirFechamentoPontuacaoTransportadorModalDetalhes();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções Públicas
 */

function preencherFechamentoPontuacaoTransportador(dadosFechamentoPontuacao) {
    _fechamentoPontuacaoTransportador.Codigo.val(dadosFechamentoPontuacao.Codigo);
    _fechamentoPontuacaoTransportador.AguardandoFinalizacao.val(dadosFechamentoPontuacao.Situacao == EnumSituacaoFechamentoPontuacao.AguardandoFinalizacao);

    if (!_fechamentoPontuacaoTransportador.AguardandoFinalizacao.val())
        recarregarGridFechamentoPontuacaoTransportador();
}

function limparCamposFechamentoPontuacaoTransportador() {
    LimparCampos(_fechamentoPontuacaoTransportador);
    recarregarGridFechamentoPontuacaoTransportador();
}

/*
 * Declaração das Funções Privadas
 */

function exibirFechamentoPontuacaoTransportadorModalDetalhes() {
    Global.abrirModal('divModalDetalhesPontuacao');
    $("#divModalDetalhesPontuacao").one('hidden.bs.modal', function () {
        limparCamposFechamentoPontuacaoTransportadorDetalhes();
    });
}

function limparCamposFechamentoPontuacaoTransportadorDetalhes() {
    LimparCampos(_gridFechamentoPontuacaoTransportadorDetalhes);

    _gridFechamentoPontuacaoTransportadorDetalhes.Regras.val(new Array());
}

function recarregarGridFechamentoPontuacaoTransportador() {
    _gridFechamentoPontuacaoTransportador.CarregarGrid();
}