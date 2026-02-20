/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

// #region Objetos Globais do Arquivo

var _gridRetornoMultiplaSelecao;

// #endregion Objetos Globais do Arquivo

// #region Funções de Inicialização

function loadGridRetornoMultiplaSelecao() {
    var quantidadePorPagina = 10;
    var ordenacao = { column: 0, dir: orderDir.asc };
    var header = [
        { data: "NumeroCarga", title: Localization.Resources.Gerais.Geral.Carga, width: "20%", className: 'text-align-center', orderable: true },
        { data: "MensagemRetorno", title: Localization.Resources.Gerais.Geral.Mensagem, width: "80%", orderable: false }
    ];

    _gridRetornoMultiplaSelecao = new BasicDataTable("grid-retorno-multipla-selecao-janela-carregamento", header, null, ordenacao, null, quantidadePorPagina);
    _gridRetornoMultiplaSelecao.CarregarGrid([]);
}

function loadRetornoMultiplaSelecao() {
    loadGridRetornoMultiplaSelecao();
}

// #endregion Funções de Inicialização

// #region Funções Públicas

function exibirRetornoMultiplaSelecao(registrosRetornados) {
    if (registrosRetornados.length == 1) {
        if (registrosRetornados[0].Sucesso)
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, registrosRetornados[0].MensagemRetorno);
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, registrosRetornados[0].MensagemRetorno);
    }
    else {
        _gridRetornoMultiplaSelecao.CarregarGrid(registrosRetornados);
        Global.abrirModal('divModalRetornoMultiplaSelecaoJanelaCarregamento');
    }
}

// #endregion Funções Públicas
