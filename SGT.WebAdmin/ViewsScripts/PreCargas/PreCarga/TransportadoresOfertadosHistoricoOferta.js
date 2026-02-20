/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

// #region Objetos Globais do Arquivo

var _gridPreCargaTransportadoresOfertadosHistoricoOferta;

// #endregion Objetos Globais do Arquivo

// #region Funções de Inicialização

function loadPreCargaTransportadoresOfertadosHistoricoOferta() {
    loadGridPreCargaTransportadoresOfertadosHistoricoOferta();
}

function loadGridPreCargaTransportadoresOfertadosHistoricoOferta() {
    var quantidadePorPagina = 10;
    var ordenacao = { column: 1, dir: orderDir.asc };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Ordem", title: Localization.Resources.Gerais.Geral.Ordem, width: "6%", className: 'text-align-center', orderable: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Configuracao, width: "30%", orderable: false },
        { data: "Empresa", title: Localization.Resources.Gerais.Geral.Empresa, width: "30%", orderable: false },
        { data: "PercentualConfigurado", title: Localization.Resources.Gerais.Geral.PercentualConfigurado, width: "17%", className: 'text-align-right', orderable: false, visible: !_configuracaoPreCarga.DisponibilizarCargaParaTransportadoresPorPrioridade },
        { data: "PercentualCargas", title: Localization.Resources.Gerais.Geral.PercentualCarga, width: "17%", className: 'text-align-right', orderable: false, visible: !_configuracaoPreCarga.DisponibilizarCargaParaTransportadoresPorPrioridade },
        { data: "Prioridade", title: Localization.Resources.Gerais.Geral.Prioridade, width: "17%", className: 'text-align-right', orderable: false, visible: _configuracaoPreCarga.DisponibilizarCargaParaTransportadoresPorPrioridade }
    ];

    _gridPreCargaTransportadoresOfertadosHistoricoOferta = new BasicDataTable("grid-pre-carga-transportadores-ofertados-historico-oferta", header, null, ordenacao, null, quantidadePorPagina);
    _gridPreCargaTransportadoresOfertadosHistoricoOferta.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Públicas

function visualizarPreCargaTransportadoresOfertadosHistoricoOferta(codigoOfertaTransportadorHistorico) {
    executarReST("PreCarga/ObterTransportadoresOfertadosHistoricoOferta", { Codigo: codigoOfertaTransportadorHistorico }, function (retorno) {
        if (retorno.Success) {
            _gridPreCargaTransportadoresOfertadosHistoricoOferta.CarregarGrid(retorno.Data);
            exibirModalPreCargaTransportadoresOfertadosHistoricoOferta();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function exibirModalPreCargaTransportadoresOfertadosHistoricoOferta() {
    Global.abrirModal('divModalPreCargaTransportadoresOfertadosHistoricoOferta');
}

// #endregion Funções Privadas
