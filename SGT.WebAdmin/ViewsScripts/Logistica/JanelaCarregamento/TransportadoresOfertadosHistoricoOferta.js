/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

// #region Objetos Globais do Arquivo

var _gridTransportadoresOfertadosHistoricoOferta;

// #endregion Objetos Globais do Arquivo

// #region Funções de Inicialização

function loadTransportadoresOfertadosHistoricoOferta() {
    loadGridTransportadoresOfertadosHistoricoOferta();
}

function loadGridTransportadoresOfertadosHistoricoOferta() {
    var quantidadePorPagina = 10;
    var ordenacao = { column: 1, dir: orderDir.asc };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Ordem", title: Localization.Resources.Cargas.Carga.Ordem, width: "6%", className: 'text-align-center', orderable: false },
        { data: "Descricao", title: Localization.Resources.Cargas.Carga.Configuracao, width: "30%", orderable: false },
        { data: "Empresa", title: Localization.Resources.Cargas.Carga.Empresa, width: "30%", orderable: false },
        { data: "PercentualConfigurado", title: Localization.Resources.Cargas.Carga.PercentualConfigurado, width: "17%", className: 'text-align-right', orderable: false, visible: !_CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorPrioridade },
        { data: "PercentualCargas", title: Localization.Resources.Cargas.Carga.PercentualDeCargas, width: "17%", className: 'text-align-right', orderable: false, visible: !_CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorPrioridade },
        { data: "Prioridade", title: Localization.Resources.Cargas.Carga.Prioridade, width: "17%", className: 'text-align-right', orderable: false, visible: _CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorPrioridade }
    ];

    _gridTransportadoresOfertadosHistoricoOferta = new BasicDataTable("grid-transportadores-ofertados-historico-oferta", header, null, ordenacao, null, quantidadePorPagina);
    _gridTransportadoresOfertadosHistoricoOferta.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Públicas

function visualizarTransportadoresOfertadosHistoricoOferta(codigoJanelaCarregamentoTransportadorHistorico) {
    executarReST("JanelaCarregamento/ObterTransportadoresOfertadosHistoricoOferta", { Codigo: codigoJanelaCarregamentoTransportadorHistorico }, function (retorno) {
        if (retorno.Success) {
            _gridTransportadoresOfertadosHistoricoOferta.CarregarGrid(retorno.Data);
            exibirModalTransportadoresOfertadosHistoricoOferta();
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function exibirModalTransportadoresOfertadosHistoricoOferta() {
    Global.abrirModal('divModalTransportadoresOfertadosHistoricoOferta');
}

// #endregion Funções Privadas
