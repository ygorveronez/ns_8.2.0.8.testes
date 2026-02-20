/// <reference path="AutorizarRegras.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/MotivoAvariaPallet.js" />
/// <reference path="../../Consultas/SetorFuncionario.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAvariaPallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridDiariasAutomaticas;
var _pesquisaDiariaAutomatica;

var _tiposFreeTime = [
    { text: "Todos tipos FreeTime", value: 0 },
    { text: "Coleta", value: 1 },
    { text: "Fronteira", value: 2 },
    { text: "Entrega", value: 3 },
    { text: "Todos", value: 99 },
];

var _statusDiariaAutomatica = [
    { text: "Todos", value: 0 },
    { text: "Iniciada", value: 1 },
    { text: "Em deslocamento", value: 2 },
    { text: "Finalizado", value: 3 },
    { text: "Cancelado", value: 4 },
];

/*
 * Declaração das Classes
 */

var PesquisaDiariaAutomatica = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", issue: 69, idBtnSearch: guid(), enable: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.LocalFreeTime = PropertyEntity({ val: ko.observable(0), options: _tiposFreeTime, def: 0, text: "Tipo Free Time: ", visible: ko.observable(true) });
    this.Status = PropertyEntity({ val: ko.observable(0), options: _statusDiariaAutomatica, def: 0, text: "Situação: ", visible: ko.observable(true) });

    this.DataInicial.dateRangeLimit = this.DataLimite;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridDiariaAutomatica, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

}


/*
 * Declaração das Funções de Inicialização
 */

function loadDiariaAutomatica() {
    _pesquisaDiariaAutomatica = new PesquisaDiariaAutomatica();
    KoBindings(_pesquisaDiariaAutomatica, "knockoutPesquisaDiariaAutomatica");

    new BuscarTransportadores(_pesquisaDiariaAutomatica.Transportador);
    new BuscarFilial(_pesquisaDiariaAutomatica.Filial);
    new BuscarCargas(_pesquisaDiariaAutomatica.Carga);

    loadGridDiariaAutomatica();
}


function loadGridDiariaAutomatica() {
    var opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharDiariaAutomatica,
        tamanho: "10",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [opcaoDetalhes]
    };


    var configuracaoExportacao = {
        url: "DiariaAutomatica/ExportarPesquisa",
        titulo: "Diárias Automáticas"
    };

    _gridDiariasAutomaticas = new GridView(_pesquisaDiariaAutomatica.Pesquisar.idGrid, "DiariaAutomatica/Pesquisa", _pesquisaDiariaAutomatica, menuOpcoes, null, 25, null, null, null, null, null, null, configuracaoExportacao);
    atualizarGridDiariaAutomatica();
}


/*
 * Declaração das Funções
 */

function atualizarGridDiariaAutomatica() {
    _gridDiariasAutomaticas.CarregarGrid();
}


function detalharDiariaAutomatica(registroSelecionado) {
    openModalDetalhes(registroSelecionado.Codigo, registroSelecionado.CodigoCarga);
}
