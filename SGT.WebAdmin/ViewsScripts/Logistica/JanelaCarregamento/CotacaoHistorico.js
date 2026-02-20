/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

// #region Objetos Globais do Arquivo

var _gridCotacaoHistorico;

// #endregion Objetos Globais do Arquivo

// #region Funções de Inicialização

function loadCotacaoHistorico() {
    loadGridCotacaoHistorico();
}

function loadGridCotacaoHistorico() {
    var quantidadePorPagina = 10;
    var menuOpcoes = null;
    var ordenacao = { column: 0, dir: orderDir.asc };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Data", title: Localization.Resources.Cargas.Carga.Data, width: "20%", className: 'text-align-center', orderable: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%", orderable: false },
    ];

    _gridCotacaoHistorico = new BasicDataTable("grid-cotacao-historico", header, menuOpcoes, ordenacao, null, quantidadePorPagina);
    _gridCotacaoHistorico.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Públicas

function visualizarHistoricoCotacao(codigoJanelaCarregamento) {
    executarReST("JanelaCarregamento/ObterHistoricoCotacao", { Codigo: codigoJanelaCarregamento }, function (retorno) {
        if (retorno.Success) {
            _gridCotacaoHistorico.CarregarGrid(retorno.Data);
            exibirModalCotacaoHistorico();
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function exibirModalCotacaoHistorico() {
    Global.abrirModal('divModalCotacaoHistorico');
}

// #endregion Funções Privadas
