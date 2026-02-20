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

var _gridMonitoramentoLogisticoIntegracao;
var _gridMonitoramentoLogisticoIntegracaoHistorico;
var _pesquisaMonitoramentoLogisticoIntegracao;
var _pesquisaMonitoramentoLogisticoIntegracaoHistorico;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaMonitoramentoLogisticoIntegracao = function () {    

    this.DataInicialAgendamento = PropertyEntity({ text: "Data Inicial do Agendamento:", val: ko.observable(""), def: "", getType: typesKnockout.date });
    this.DataLimiteAgendamento = PropertyEntity({ text: "Data Limite do Agendamento:", val: ko.observable(""), def: "", getType: typesKnockout.date });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.CodigoVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text:"Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable(EnumSituacaoIntegracao.Todas), def: EnumSituacaoIntegracao.Todas, options: EnumSituacaoIntegracao.obterOpcoesPesquisa(false) });
    this.Protocolo = PropertyEntity({ text: "Protocolo:", val: ko.observable(""),  getType: typesKnockout.string });
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Numero da Carga:", val: ko.observable(""), getType: typesKnockout.string });    
    this.CodigoMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });

    this.DataInicialAgendamento.dateRangeLimit = this.DataLimiteAgendamento;
    this.DataLimiteAgendamento.dateRangeInit = this.DataInicialAgendamento;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _pesquisaMonitoramentoLogisticoIntegracao.ExibirFiltros.visibleFade(!_pesquisaMonitoramentoLogisticoIntegracao.ExibirFiltros.visibleFade());
            recarregarGridMonitoramentoLogisticoIntegracao();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true), visibleFade: ko.observable(false)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

var PesquisaMonitoramentoLogisticoIntegracaoHistorico = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGridMonitoramentoLogisticoIntegracao() {
    var reenviar = { descricao: "Reenviar", id: guid(), evento: "onclick", metodo: reenviarMonitoramentoLogisticoIntegracaoClick, icone: "" };
    var historico = { descricao: "Histórico de Integração", id: guid(), evento: "onclick", metodo: exibirHistoricoMonitoramentoLogisticoIntegracaoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, tamanho: 10, opcoes: [historico, reenviar] };

    _gridMonitoramentoLogisticoIntegracao = new GridView("grid-ordem-embarque-integracao", "MonitoramentoLogisticoIntegracao/Pesquisa", _pesquisaMonitoramentoLogisticoIntegracao, menuOpcoes, null, 20);
    _gridMonitoramentoLogisticoIntegracao.CarregarGrid();
}

function loadGridMonitoramentoLogisticoIntegracaoHistorico() {
    var opcaoDownload = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: downloadArquivosMonitoramentoLogisticoIntegracaoHistoricoClick, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDownload] };

    _gridMonitoramentoLogisticoIntegracaoHistorico = new GridView("grid-ordem-embarque-integracao-historico", "MonitoramentoLogisticoIntegracao/PesquisaHistoricoIntegracao", _pesquisaMonitoramentoLogisticoIntegracaoHistorico, menuOpcoes, { column: 1, dir: orderDir.desc });
}

function loadMonitoramentoLogisticoIntegracao() {
    _pesquisaMonitoramentoLogisticoIntegracao = new PesquisaMonitoramentoLogisticoIntegracao();
    KoBindings(_pesquisaMonitoramentoLogisticoIntegracao, "knockoutPesquisaMonitoramentoLogisticoIntegracao", _pesquisaMonitoramentoLogisticoIntegracao.Pesquisar.id);

    _pesquisaMonitoramentoLogisticoIntegracaoHistorico = new PesquisaMonitoramentoLogisticoIntegracaoHistorico();

    new BuscarFilial(_pesquisaMonitoramentoLogisticoIntegracao.Filial);
    new BuscarVeiculos(_pesquisaMonitoramentoLogisticoIntegracao.CodigoVeiculo);
    new BuscarTransportadores(_pesquisaMonitoramentoLogisticoIntegracao.Transportador);
    new BuscarMotorista(_pesquisaMonitoramentoLogisticoIntegracao.CodigoMotorista);
    loadGridMonitoramentoLogisticoIntegracao();
    loadGridMonitoramentoLogisticoIntegracaoHistorico();
}

// #endregion Funções de Inicialização

//#region Funções Associadas a Eventos

function downloadArquivosMonitoramentoLogisticoIntegracaoHistoricoClick(registroSelecionado) {
    executarDownload("MonitoramentoLogisticoIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: registroSelecionado.Codigo });
}

function exibirHistoricoMonitoramentoLogisticoIntegracaoClick(registroSelecionado) {
    _pesquisaMonitoramentoLogisticoIntegracaoHistorico.Codigo.val(registroSelecionado.Codigo);
    recarregarGridMonitoramentoLogisticoIntegracaoHistorico();

    Global.abrirModal('divModalMonitoramentoLogisticoIntegracaoHistorico');
}

function reenviarMonitoramentoLogisticoIntegracaoClick(registroSelecionado) {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar a integração?", function () {
        executarReST("MonitoramentoLogisticoIntegracao/Reenviar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
                    recarregarGridMonitoramentoLogisticoIntegracao();
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

function recarregarGridMonitoramentoLogisticoIntegracao() {
    _gridMonitoramentoLogisticoIntegracao.CarregarGrid();
}

function recarregarGridMonitoramentoLogisticoIntegracaoHistorico() {
    _gridMonitoramentoLogisticoIntegracaoHistorico.CarregarGrid();
}

//#endregion Funções
