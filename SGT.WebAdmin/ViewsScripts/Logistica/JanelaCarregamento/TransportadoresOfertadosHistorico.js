/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumTipoCargaJanelaCarregamentoTransportadorHistorico.js" />
/// <reference path="TransportadoresOfertadosHistoricoOferta.js" />

// #region Objetos Globais do Arquivo

var _gridTransportadoresOfertadosHistorico;

// #endregion Objetos Globais do Arquivo

// #region Funções de Inicialização

function loadTransportadoresOfertadosHistorico() {
    loadGridTransportadoresOfertadosHistorico();
    loadTransportadoresOfertadosHistoricoOferta();
}

function loadGridTransportadoresOfertadosHistorico() {
    var quantidadePorPagina = 10;
    var opcaoDetalhes = { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: "clasEditar", evento: "onclick", metodo: visualizarTransportadoresOfertadosHistoricoOfertaClick, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDetalhes] };
    var ordenacao = { column: 0, dir: orderDir.asc };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Tipo", visible: false },
        { data: "Data", title: Localization.Resources.Cargas.Carga.Data, width: "15%", className: 'text-align-center', orderable: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "45%", orderable: false },
        { data: "Usuario", title: Localization.Resources.Cargas.Carga.Usuario, width: "25%", orderable: false }
    ];

    _gridTransportadoresOfertadosHistorico = new BasicDataTable("grid-transportadores-ofertados-historico", header, menuOpcoes, ordenacao, null, quantidadePorPagina);
    _gridTransportadoresOfertadosHistorico.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function visualizarTransportadoresOfertadosHistoricoOfertaClick(registroSelecionado) {
    if (registroSelecionado.Tipo == EnumTipoCargaJanelaCarregamentoTransportadorHistorico.OfertaCargaPorRota)
        visualizarTransportadoresOfertadosHistoricoOferta(registroSelecionado.Codigo);
    else
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Cargas.Carga.NenhumDetalheAdicionalSerExibidoParaEsteHistorico);
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function visualizarTransportadoresOfertadosHistorico(codigoJanelaCarregamentoTransportador) {
    executarReST("JanelaCarregamento/ObterTransportadoresOfertadosHistorico", { Codigo: codigoJanelaCarregamentoTransportador }, function (retorno) {
        if (retorno.Success) {
            _gridTransportadoresOfertadosHistorico.CarregarGrid(retorno.Data);
            exibirModalTransportadoresOfertadosHistorico();
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function exibirModalTransportadoresOfertadosHistorico() {
    Global.abrirModal('divModalTransportadoresOfertadosHistorico');
}

// #endregion Funções Privadas
