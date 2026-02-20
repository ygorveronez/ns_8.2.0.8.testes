/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumEtapaFluxoGestaoPatio.js" />
//#region Variaveis Globais
var _gridFluxoPatioIntegracoes;
var _pesquisaCargaIntegracaoEvento;
var _fluxoPatioIntegracoes;

var _situacaoIntegracaoFluxoIntegracao = [{ value: null, text: "Todas" },
    { value: EnumSituacaoIntegracao.AgIntegracao, text: "Aguardando Integração" },
    { value: EnumSituacaoIntegracao.AgRetorno, text: "Aguardando Retorno" },
    { value: EnumSituacaoIntegracao.Integrado, text: "Integrado" },
    { value: EnumSituacaoIntegracao.ProblemaIntegracao, text: "Falha na Integração" }];

//#endregion

//#region Funções Constructoras
function PesquisaCargaIntegracaoEvento() {

    this.NumeroCarga = PropertyEntity({ val: ko.observable(""), text: "Número Carga:" });
    this.SituacaoIntegracao = PropertyEntity({ text: "Situação Integração:", val: ko.observable(_situacaoIntegracaoFluxoIntegracao[0]), options: _situacaoIntegracaoFluxoIntegracao, def: _situacaoIntegracaoFluxoIntegracao[0] });
    this.DataIntegracaoInicial = PropertyEntity({ text: ko.observable("Data Integração Inicial: "), val: ko.observable(''), def: '', getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataIntegracaoFinal = PropertyEntity({ text: ko.observable("Data Integração Final: "), val: ko.observable(''), def: '', getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataIntegracaoInicial.dateRangeLimit = this.DataIntegracaoFinal;
    this.DataIntegracaoFinal.dateRangeInit = this.DataIntegracaoInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridFluxoPatioIntegracoes.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            let visible = e.ExibirFiltros.visibleFade() == true;
            e.ExibirFiltros.visibleFade(!visible);
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

function LoadCargaIntegracaoEvento() {
    _pesquisaCargaIntegracaoEvento = new PesquisaCargaIntegracaoEvento();
    KoBindings(_pesquisaCargaIntegracaoEvento, "knockoutPesquisaCargaIntegracaoEvento");


    BuscarIntegracoes();
}

//#endregion

//#region Funções Auxiliares

function BuscarIntegracoes() {
    const auditar = { descricao: "Auditar", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("CargaIntegracaoEvento"), tamanho: "15", icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    const reenviar = { descricao: "Reenviar", id: guid(), evento: "onclick", metodo: ReenviarIntegracao, tamanho: "15", icone: "" };
    const baixar = { descricao: "Arquivos Integração", id: guid(), evento: "onclick", metodo: ExibirHistoricoIntegracao, tamanho: "15", icone: "" };

    const menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(auditar);
    menuOpcoes.opcoes.push(reenviar);
    menuOpcoes.opcoes.push(baixar);

    _gridFluxoPatioIntegracoes = new GridView(_pesquisaCargaIntegracaoEvento.Pesquisar.idGrid, "CargaIntegracaoEvento/Pesquisar", _pesquisaCargaIntegracaoEvento, menuOpcoes, null);
    _gridFluxoPatioIntegracoes.CarregarGrid();
}

function ReenviarIntegracao(data) {
    executarReST("CargaIntegracaoEvento/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
            _gridFluxoPatioIntegracoes.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}
function DownloadArquivosHistoricoIntegracao(data) {
    executarDownload("CargaIntegracaoEvento/DownloadArquivosHistoricoIntegracao", { Codigo: data.Codigo });
}
var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function BuscarHistoricoIntegracao(integracao) {
    _pesquisaHistoricoIntegracao = new PesquisaHistoricoIntegracao();
    _pesquisaHistoricoIntegracao.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracao, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "CargaIntegracaoEvento/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function ExibirHistoricoIntegracao(integracao) {
    BuscarHistoricoIntegracao(integracao);
    Global.abrirModal("divModalHistoricoIntegracao");
}

//#endregion