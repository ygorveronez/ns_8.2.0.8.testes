/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../DadosParaTransporte/DadosParaTransporte.js" />
/// <reference path="Container.js" />

// #region Funções de Inicialização

function loadDadosParaTransporte() {
    loadPreCargaDadosParaTransporte();
}

// #endregion Funções de Inicialização

// #region Funções Públicas

function exibirModalDadosParaTransporte(codigoPreCarga) {
    exibirPreCargaDadosParaTransporte(codigoPreCarga, function () {
        recarregarPreCargas();
    });
}

// #endregion Funções Públicas
