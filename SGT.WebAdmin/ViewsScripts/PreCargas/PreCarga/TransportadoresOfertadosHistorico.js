/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumTipoPreCargaOfertaTransportadorHistorico.js" />
/// <reference path="TransportadoresOfertadosHistoricoOferta.js" />

// #region Objetos Globais do Arquivo

var _gridPreCargaTransportadoresOfertadosHistorico;

// #endregion Objetos Globais do Arquivo

// #region Funções de Inicialização

function loadPreCargaTransportadoresOfertadosHistorico() {
    loadGridPreCargaTransportadoresOfertadosHistorico();
    loadPreCargaTransportadoresOfertadosHistoricoOferta();
}

function loadGridPreCargaTransportadoresOfertadosHistorico() {
    var quantidadePorPagina = 10;
    var opcaoDetalhes = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: visualizarPreCargaTransportadoresOfertadosHistoricoOfertaClick, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDetalhes] };
    var ordenacao = { column: 0, dir: orderDir.asc };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Tipo", visible: false },
        { data: "Data", title: Localization.Resources.Gerais.Geral.Data, width: "15%", className: 'text-align-center', orderable: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "45%", orderable: false },
        { data: "Usuario", title: Localization.Resources.Gerais.Geral.Usuario, width: "25%", orderable: false }
    ];

    _gridPreCargaTransportadoresOfertadosHistorico = new BasicDataTable("grid-pre-carga-transportadores-ofertados-historico", header, menuOpcoes, ordenacao, null, quantidadePorPagina);
    _gridPreCargaTransportadoresOfertadosHistorico.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function visualizarPreCargaTransportadoresOfertadosHistoricoOfertaClick(registroSelecionado) {
    if (registroSelecionado.Tipo == EnumTipoPreCargaOfertaTransportadorHistorico.OfertaPorRota)
        visualizarPreCargaTransportadoresOfertadosHistoricoOferta(registroSelecionado.Codigo);
    else
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Gerais.Geral.NenhumDetalheAdicionalExibidoHistorico);
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function visualizarPreCargaTransportadoresOfertadosHistorico(codigoOfertaTransportador) {
    executarReST("PreCarga/ObterTransportadoresOfertadosHistorico", { Codigo: codigoOfertaTransportador }, function (retorno) {
        if (retorno.Success) {
            _gridPreCargaTransportadoresOfertadosHistorico.CarregarGrid(retorno.Data);
            exibirModalPreCargaTransportadoresOfertadosHistorico();
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function exibirModalPreCargaTransportadoresOfertadosHistorico() {
    Global.abrirModal('divModalPreCargaTransportadoresOfertadosHistorico');
}

// #endregion Funções Privadas
