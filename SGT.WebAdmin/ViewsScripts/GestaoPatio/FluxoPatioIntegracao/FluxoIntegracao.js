/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumEtapaFluxoGestaoPatio.js" />
//#region Variaveis Globais
var _gridFluxoPatioIntegracoes;
var _pesquisaFluxoPatioIntegracao;
var _fluxoPatioIntegracoes;

var _situacaoIntegracaoFluxoIntegracao = [{ value: "", text: "Todas" },
{ value: EnumSituacaoIntegracaoCarga.AgIntegracao, text: "Aguardando Integração" },
{ value: EnumSituacaoIntegracaoCarga.AgRetorno, text: "Aguardando Retorno" },
{ value: EnumSituacaoIntegracaoCarga.Integrado, text: "Integrado" },
{ value: EnumSituacaoIntegracaoCarga.ProblemaIntegracao, text: "Falha na Integração" }];

//#endregion

//#region Funções Constructoras
function PesquisaFluxoPatioIntegracao() {

    this.NumeroCarga = PropertyEntity({ val: ko.observable(""), text: "Numero Carga:" });
    this.Integradora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Integradora:", idBtnSearch: guid(), val: ko.observable("") });
    this.SituacaoIntegracao = PropertyEntity({ text: "Situação Integração:", val: ko.observable(_situacaoIntegracaoFluxoIntegracao[0]), options: _situacaoIntegracaoFluxoIntegracao, def: _situacaoIntegracaoFluxoIntegracao[0] });
    this.EtapaPatio = PropertyEntity({ text: "Etapa Fluxo:", val: ko.observable(""), options: EnumEtapaFluxoGestaoPatio.obterOpcoesGatilhoOcorrenciaFinal(), def: EnumEtapaFluxoGestaoPatio.Todas });
    this.Data = PropertyEntity({ getType: typesKnockout.dateTime, text: "Data:", required: false, enable: ko.observable(true) });

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

function LoadFluxoPatioIntegracao() {
    _pesquisaFluxoPatioIntegracao = new PesquisaFluxoPatioIntegracao();
    KoBindings(_pesquisaFluxoPatioIntegracao, "knockoutPesquisaFluxoPatioIntegracao");

    new BuscarIntegradora(_pesquisaFluxoPatioIntegracao.Integradora);

    BuscarIntegracoes();
}

//#endregion

//#region Funções Auxiliares

function BuscarIntegracoes() {
    const auditar = { descricao: "Auditar", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("FluxoPatioIntegracao"), tamanho: "15", icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    const reenviar = { descricao: "Reenviar", id: guid(), evento: "onclick", metodo: ReenviarIntegracao, tamanho: "15", icone: "" };
    const baixar = { descricao: "Arquivos Integração", id: guid(), evento: "onclick", metodo: ExibirHistoricoIntegracao, tamanho: "15", icone: "" };

    const menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(auditar);
    menuOpcoes.opcoes.push(reenviar);
    menuOpcoes.opcoes.push(baixar);

    _gridFluxoPatioIntegracoes = new GridView(_pesquisaFluxoPatioIntegracao.Pesquisar.idGrid, "FluxoPatioIntegracao/Pesquisar", _pesquisaFluxoPatioIntegracao, menuOpcoes, null);
    _gridFluxoPatioIntegracoes.CarregarGrid();
}

function ReenviarIntegracao(data) {
    executarReST("FluxoPatioIntegracao/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
            _gridFluxoPatioIntegracoes.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}
function DownloadArquivosHistoricoIntegracao(data) {
    executarDownload("FluxoPatioIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: data.Codigo });
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

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "FluxoPatioIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function ExibirHistoricoIntegracao(integracao) {
    BuscarHistoricoIntegracao(integracao);
    Global.abrirModal("divModalHistoricoIntegracao");
}

//#endregion