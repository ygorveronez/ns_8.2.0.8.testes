/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracao.js" />

// #region Objetos Globais do Arquivo

var _gridCargaExportacaoIntegracao;
var _gridCargaExportacaoIntegracaoHistorico;
var _pesquisaCargaExportacaoIntegracao;
var _pesquisaCargaExportacaoIntegracaoHistorico;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaCargaExportacaoIntegracao = function () {
    var dataPadrao = Global.DataAtual();

    this.DataInicialAgendamento = PropertyEntity({ text: "Data Inicial do Agendamento:", val: ko.observable(dataPadrao), def: dataPadrao, getType: typesKnockout.date });
    this.DataLimiteAgendamento = PropertyEntity({ text: "Data Limite do Agendamento:", val: ko.observable(dataPadrao), def: dataPadrao, getType: typesKnockout.date });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable(EnumSituacaoIntegracao.Todas), def: EnumSituacaoIntegracao.Todas, options: EnumSituacaoIntegracao.obterOpcoesPesquisa() });
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga:", val: ko.observable(""), getType: typesKnockout.string });
    this.NumeroEXP = PropertyEntity({ text: "Número EXP:", val: ko.observable(""), getType: typesKnockout.string });

    this.DataInicialAgendamento.dateRangeLimit = this.DataLimiteAgendamento;
    this.DataLimiteAgendamento.dateRangeInit = this.DataInicialAgendamento;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _pesquisaCargaExportacaoIntegracao.ExibirFiltros.visibleFade(!_pesquisaCargaExportacaoIntegracao.ExibirFiltros.visibleFade());
            recarregarGridCargaExportacaoIntegracao();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true), visibleFade: ko.observable(false)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

var PesquisaCargaExportacaoIntegracaoHistorico = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGridCargaExportacaoIntegracao() {
    var reenviar = { descricao: "Reenviar", id: guid(), evento: "onclick", metodo: reenviarCargaExportacaoIntegracaoClick, icone: "" };
    var historico = { descricao: "Histórico de Integração", id: guid(), evento: "onclick", metodo: exibirHistoricoCargaExportacaoIntegracaoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, tamanho: 10, opcoes: [historico, reenviar] };
    var configuracoesExportacao = { url: "CargaExportacaoIntegracao/ExportarPesquisa", titulo: "Integrações de Exportação" };

    _gridCargaExportacaoIntegracao = new GridViewExportacao("grid-carga-exportacao-integracao", "CargaExportacaoIntegracao/Pesquisa", _pesquisaCargaExportacaoIntegracao, menuOpcoes, configuracoesExportacao, null, 20);
    _gridCargaExportacaoIntegracao.CarregarGrid();
}

function loadGridCargaExportacaoIntegracaoHistorico() {
    var opcaoDownload = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: downloadArquivosCargaExportacaoIntegracaoHistoricoClick, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDownload] };

    _gridCargaExportacaoIntegracaoHistorico = new GridView("grid-carga-exportacao-integracao-historico", "CargaExportacaoIntegracao/PesquisaHistoricoIntegracao", _pesquisaCargaExportacaoIntegracaoHistorico, menuOpcoes, { column: 1, dir: orderDir.desc });
}

function loadCargaExportacaoIntegracao() {
    _pesquisaCargaExportacaoIntegracao = new PesquisaCargaExportacaoIntegracao();
    KoBindings(_pesquisaCargaExportacaoIntegracao, "knockoutPesquisaCargaExportacaoIntegracao", _pesquisaCargaExportacaoIntegracao.Pesquisar.id);

    _pesquisaCargaExportacaoIntegracaoHistorico = new PesquisaCargaExportacaoIntegracaoHistorico();

    new BuscarFilial(_pesquisaCargaExportacaoIntegracao.Filial);
    new BuscarVeiculos(_pesquisaCargaExportacaoIntegracao.Veiculo);
    new BuscarTransportadores(_pesquisaCargaExportacaoIntegracao.Transportador);

    loadGridCargaExportacaoIntegracao();
    loadGridCargaExportacaoIntegracaoHistorico();
}

// #endregion Funções de Inicialização

//#region Funções Associadas a Eventos

function downloadArquivosCargaExportacaoIntegracaoHistoricoClick(registroSelecionado) {
    executarDownload("CargaExportacaoIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: registroSelecionado.Codigo });
}

function exibirHistoricoCargaExportacaoIntegracaoClick(registroSelecionado) {
    _pesquisaCargaExportacaoIntegracaoHistorico.Codigo.val(registroSelecionado.Codigo);
    recarregarGridCargaExportacaoIntegracaoHistorico();

    Global.abrirModal('divModalCargaExportacaoIntegracaoHistorico');
}

function reenviarCargaExportacaoIntegracaoClick(registroSelecionado) {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar a integração?", function () {
        executarReST("CargaExportacaoIntegracao/Reenviar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
                    recarregarGridCargaExportacaoIntegracao();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso!", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
        });
    });
}

//#endregion Funções Associadas a Eventos

//#region Funções

function recarregarGridCargaExportacaoIntegracao() {
    _gridCargaExportacaoIntegracao.CarregarGrid();
}

function recarregarGridCargaExportacaoIntegracaoHistorico() {
    _gridCargaExportacaoIntegracaoHistorico.CarregarGrid();
}

//#endregion Funções
