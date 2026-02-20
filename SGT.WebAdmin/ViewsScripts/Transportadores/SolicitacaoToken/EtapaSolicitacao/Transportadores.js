/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/Global/Rest.js" />


var _gridTransportadoresSolicitacaoToken;
var _solicitacaoTokenTransportadores;

var SolicitacaoTokenTransportadores = function () {
    this.Transportadores = PropertyEntity({ text: 'Adicionar Transportador', idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true), visible: ko.observable(true) });
}

var loadTransportadoresSolicitacaoToken = function () {
    _solicitacaoTokenTransportadores = new SolicitacaoTokenTransportadores();
    KoBindings(_solicitacaoTokenTransportadores, 'knockoutSolicitacaoTokenTransportadores');

    loadGridTransportadoresSolicitacaoToken();
}

function loadGridTransportadoresSolicitacaoToken() {
    var linhasPorPaginas = 10;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerTransportadorSolicitacaoTokenClick, icone: "", visibilidade: _solicitacaoTokenTransportadores.Transportadores.enable };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "45%", className: "text-align-left" },
        { data: "CNPJ", title: "CNPJ", width: "45%", className: "text-align-center" },
    ];

    _gridTransportadoresSolicitacaoToken = new BasicDataTable(_solicitacaoTokenTransportadores.Transportadores.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    BuscarTransportadores(_solicitacaoTokenTransportadores.Transportadores, null, null, null, _gridTransportadoresSolicitacaoToken);

    _gridTransportadoresSolicitacaoToken.CarregarGrid([]);
}

function obterListaTransportadoresSolicitacaoToken() {
    return _gridTransportadoresSolicitacaoToken.BuscarRegistros();
}

function removerTransportadorSolicitacaoTokenClick(registroSelecionado) {
    let trs = obterListaTransportadoresSolicitacaoToken();

    for (var i = 0; i < trs.length; i++) {
        if (registroSelecionado.Codigo == trs[i].Codigo) {
            trs.splice(i, 1);
            break;
        }
    }

    _gridTransportadoresSolicitacaoToken.CarregarGrid(trs);
}

function renderizarGridTransportadoresSolicitacaoToken() {

    let trs = obterListaTransportadoresSolicitacaoToken();

    _gridTransportadoresSolicitacaoToken.CarregarGrid(trs);
}

function recarregarGridTransportadoresSolicitacaoToken() {
    _gridTransportadoresSolicitacaoToken.CarregarGrid([]);
}

