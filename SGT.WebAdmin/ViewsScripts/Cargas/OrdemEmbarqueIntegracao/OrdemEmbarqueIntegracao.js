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

var _gridOrdemEmbarqueIntegracao;
var _gridOrdemEmbarqueIntegracaoHistorico;
var _pesquisaOrdemEmbarqueIntegracao;
var _pesquisaOrdemEmbarqueIntegracaoHistorico;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaOrdemEmbarqueIntegracao = function () {
    let dataPadrao = Global.DataAtual();

    this.DataInicialAgendamento = PropertyEntity({ text: "Data Inicial do Agendamento:", val: ko.observable(dataPadrao), def: dataPadrao, getType: typesKnockout.date });
    this.DataLimiteAgendamento = PropertyEntity({ text: "Data Limite do Agendamento:", val: ko.observable(dataPadrao), def: dataPadrao, getType: typesKnockout.date });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text:"Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable(EnumSituacaoIntegracao.Todas), def: EnumSituacaoIntegracao.Todas, options: EnumSituacaoIntegracao.obterOpcoesPesquisa(true) });
    this.NumeroOrdemEmbarque = PropertyEntity({ text: "Número da OE:", val: ko.observable(""),  getType: typesKnockout.string });
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Numero da Carga:", val: ko.observable(""), getType: typesKnockout.string });
    this.SomenteComLicencaInvalida = PropertyEntity({ text: "Somente registros não integrados com licença inválida", type: types.bool, val: ko.observable(false), def: false });

    this.DataInicialAgendamento.dateRangeLimit = this.DataLimiteAgendamento;
    this.DataLimiteAgendamento.dateRangeInit = this.DataInicialAgendamento;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _pesquisaOrdemEmbarqueIntegracao.ExibirFiltros.visibleFade(!_pesquisaOrdemEmbarqueIntegracao.ExibirFiltros.visibleFade());
            recarregarGridOrdemEmbarqueIntegracao();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true), visibleFade: ko.observable(false)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

var PesquisaOrdemEmbarqueIntegracaoHistorico = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGridOrdemEmbarqueIntegracao() {
    const reenviar = { descricao: "Reenviar", id: guid(), evento: "onclick", metodo: reenviarOrdemEmbarqueIntegracaoClick, icone: "" };
    const historico = { descricao: "Histórico de Integração", id: guid(), evento: "onclick", metodo: exibirHistoricoOrdemEmbarqueIntegracaoClick, icone: "" };
    const menuOpcoes = { tipo: TypeOptionMenu.list, tamanho: 10, opcoes: [historico, reenviar] };

    const configExportacao = {
        url: "OrdemEmbarqueIntegracao/ExportarPesquisa",
        titulo: "Integração de Ordens de Embarque"
    };

    _gridOrdemEmbarqueIntegracao = new GridViewExportacao("grid-ordem-embarque-integracao", "OrdemEmbarqueIntegracao/Pesquisa", _pesquisaOrdemEmbarqueIntegracao, menuOpcoes, configExportacao, null, 20);
    _gridOrdemEmbarqueIntegracao.CarregarGrid();
}

function loadGridOrdemEmbarqueIntegracaoHistorico() {
    const opcaoDownload = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: downloadArquivosOrdemEmbarqueIntegracaoHistoricoClick, tamanho: "20", icone: "" };
    const menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDownload] };

    _gridOrdemEmbarqueIntegracaoHistorico = new GridView("grid-ordem-embarque-integracao-historico", "OrdemEmbarqueIntegracao/PesquisaHistoricoIntegracao", _pesquisaOrdemEmbarqueIntegracaoHistorico, menuOpcoes, { column: 1, dir: orderDir.desc });
}

function loadOrdemEmbarqueIntegracao() {
    _pesquisaOrdemEmbarqueIntegracao = new PesquisaOrdemEmbarqueIntegracao();
    KoBindings(_pesquisaOrdemEmbarqueIntegracao, "knockoutPesquisaOrdemEmbarqueIntegracao", _pesquisaOrdemEmbarqueIntegracao.Pesquisar.id);

    _pesquisaOrdemEmbarqueIntegracaoHistorico = new PesquisaOrdemEmbarqueIntegracaoHistorico();

    BuscarFilial(_pesquisaOrdemEmbarqueIntegracao.Filial);
    BuscarVeiculos(_pesquisaOrdemEmbarqueIntegracao.Veiculo);
    BuscarTransportadores(_pesquisaOrdemEmbarqueIntegracao.Transportador);

    loadGridOrdemEmbarqueIntegracao();
    loadGridOrdemEmbarqueIntegracaoHistorico();
}

// #endregion Funções de Inicialização

//#region Funções Associadas a Eventos

function downloadArquivosOrdemEmbarqueIntegracaoHistoricoClick(registroSelecionado) {
    executarDownload("OrdemEmbarqueIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: registroSelecionado.Codigo });
}

function exibirHistoricoOrdemEmbarqueIntegracaoClick(registroSelecionado) {
    _pesquisaOrdemEmbarqueIntegracaoHistorico.Codigo.val(registroSelecionado.Codigo);
    recarregarGridOrdemEmbarqueIntegracaoHistorico();

    Global.abrirModal('divModalOrdemEmbarqueIntegracaoHistorico');
}

function reenviarOrdemEmbarqueIntegracaoClick(registroSelecionado) {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar a integração?", function () {
        executarReST("OrdemEmbarqueIntegracao/Reenviar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
                    recarregarGridOrdemEmbarqueIntegracao();
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

function recarregarGridOrdemEmbarqueIntegracao() {
    _gridOrdemEmbarqueIntegracao.CarregarGrid();
}

function recarregarGridOrdemEmbarqueIntegracaoHistorico() {
    _gridOrdemEmbarqueIntegracaoHistorico.CarregarGrid();
}

//#endregion Funções
