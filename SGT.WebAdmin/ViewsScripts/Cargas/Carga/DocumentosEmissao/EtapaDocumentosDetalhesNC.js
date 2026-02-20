/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Globais.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Rest.js" />

// #region Objetos Globais do Arquivo

var _gridDetalhesNaoConformidadePorNotaFiscal;

// #endregion Objetos Globais do Arquivo

// #region Funções de Inicialização

function loadGridDetalhesNaoConformidadePorNotaFiscal() {
    header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "65%", className: "text-align-left" },
        { data: "Situacao", title: "Situação", width: "35%", className: "text-align-center" }
    ];

    _gridDetalhesNaoConformidadePorNotaFiscal = new BasicDataTable("grid-detalhes-nao-conformidade-nota-fiscal-carga", header);
}

// #endregion Funções de Inicialização

// #region Funções Públicas

function exibirDetalhesNaoConformidadePorNotaFiscal(codigoNotaFiscal) {
    if (!_gridDetalhesNaoConformidadePorNotaFiscal)
        loadGridDetalhesNaoConformidadePorNotaFiscal();

    executarReST("CargaNotasFiscais/BuscarDetalhesNaoConformidades", { Codigo: codigoNotaFiscal }, function (retorno) {
        if (retorno.Success) {
            _gridDetalhesNaoConformidadePorNotaFiscal.CarregarGrid(retorno.Data);
            Global.abrirModal("divModalDetalhesNaoConformidadeNotaFiscalCarga");
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Públicas
