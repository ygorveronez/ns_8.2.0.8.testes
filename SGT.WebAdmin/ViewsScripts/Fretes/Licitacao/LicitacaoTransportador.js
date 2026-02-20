/// <reference path="Licitacao.js" />
/// <reference path="../../Consultas/Tranportador.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridLicitacaoTransportador;
var _licitacaoTransportador;

/*
 * Declaração das Classes
 */

var LicitacaoTransportador = function () {
    this.LiberarTodosTransportadores = _licitacao.LiberarTodosTransportadores;
    this.Grid = PropertyEntity({ type: types.local });

    this.Transportador = PropertyEntity({ type: types.event, text: "Adicionar Transportadores", idBtnSearch: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadLicitacaoTransportador() {
    _licitacaoTransportador = new LicitacaoTransportador();
    KoBindings(_licitacaoTransportador, "knockoutLicitacaoTransportador");

    loadGridLicitacaoTransportador();
}

function loadGridLicitacaoTransportador() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: excluirLicitacaoTransportadorClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoExcluir] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "RazaoSocial", title: "Razão Social", width: "30%" },
        { data: "CNPJ", title: "CNPJ", width: "20%" },
        { data: "Localidade", title: "Localidade", width: "30%" }
    ];

    _gridLicitacaoTransportador = new BasicDataTable(_licitacaoTransportador.Grid.id, header, menuOpcoes);

    new BuscarTransportadores(_licitacaoTransportador.Transportador, null, null, null, _gridLicitacaoTransportador);

    _licitacaoTransportador.Transportador.basicTable = _gridLicitacaoTransportador;

    recarregarGridLicitacaoTransportador();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function excluirLicitacaoTransportadorClick(registroSelecionado) {
    var transportadores = _gridLicitacaoTransportador.BuscarRegistros();

    for (var i = 0; i < transportadores.length; i++) {
        if (registroSelecionado.Codigo == transportadores[i].Codigo) {
            transportadores.splice(i, 1);
            break;
        }
    }

    _gridLicitacaoTransportador.CarregarGrid(transportadores);
}

/*
 * Declaração das Funções
 */

function obterTransportadores() {
    return JSON.stringify(_gridLicitacaoTransportador.BuscarRegistros());
}

function recarregarGridLicitacaoTransportador() {
    _gridLicitacaoTransportador.CarregarGrid(_licitacao.Transportadores.val() || []);
}
