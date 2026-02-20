//#region Variaveis Globais
var _gridIntegracaoNFe;
var _pesquisaIntegracaoNFe;
var _pesquisaHistoricoIntegracao;
var _gridHistoricoIntegracao;

var _situacaoIntegracaoFluxoIntegracao = [
    { value: EnumSituacaoIntegracao.AgIntegracao, text: "Aguardando Integração" },
    { value: EnumSituacaoIntegracao.AgRetorno, text: "Aguardando Retorno" },
    { value: EnumSituacaoIntegracao.Integrado, text: "Integrado" },
    { value: EnumSituacaoIntegracao.ProblemaIntegracao, text: "Falha na Integração" }];

var _situacaoProcessamentoRegistro = [
    { value: EnumSituacaoProcessamentoRegistro.Pendente, text: "Pendente" },
    { value: EnumSituacaoProcessamentoRegistro.Sucesso, text: "Sucesso" },
    { value: EnumSituacaoProcessamentoRegistro.Falha, text: "Falha" },
    { value: EnumSituacaoProcessamentoRegistro.Liberado, text: "Liberado" },
    { value: EnumSituacaoProcessamentoRegistro.FalhaLiberacao, text: "Falha na liberação" }];

//#endregion

//#region Funções Constructoras

function PesquisaIntegracaoNFe() {
    this.NumeroCarga = PropertyEntity({ val: ko.observable(""), text: "Número Carga:" });
    this.NumeroPedido = PropertyEntity({ val: ko.observable(""), text: "Número Pedido:" });
    this.Chave = PropertyEntity({ val: ko.observable(""), text: "Chave:" });
    this.SituacaoIntegracao = PropertyEntity({ text: "Situação Integração:", getType: typesKnockout.selectMultiple, val: ko.observable(_situacaoIntegracaoFluxoIntegracao[0]), options: _situacaoIntegracaoFluxoIntegracao, def: _situacaoIntegracaoFluxoIntegracao[0], visible: ko.observable(true) });
    this.SituacaoProcessamentoRegistro = PropertyEntity({ text: "Situação Processamento:", getType: typesKnockout.selectMultiple, val: ko.observable(_situacaoProcessamentoRegistro[0]), options: _situacaoProcessamentoRegistro, def: _situacaoProcessamentoRegistro[0], visible: ko.observable(false) });

    this.Situacoes = PropertyEntity({ text: "Situação da Carga: ", val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: ko.observable(_situacaoIntegracaoFluxoIntegracao), });


    this.DataIntegracaoInicial = PropertyEntity({ text: ko.observable("Data Integração Inicial: "), val: ko.observable(''), def: '', getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataIntegracaoFinal = PropertyEntity({ text: ko.observable("Data Integração Final: "), val: ko.observable(''), def: '', getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataIntegracaoInicial.dateRangeLimit = this.DataIntegracaoFinal;
    this.DataIntegracaoFinal.dateRangeInit = this.DataIntegracaoInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridIntegracaoNFe.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            let visible = e.ExibirFiltros.visibleFade() == true;
            e.ExibirFiltros.visibleFade(!visible);
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

function LoadIntegracaoNFe() {
    _pesquisaIntegracaoNFe = new PesquisaIntegracaoNFe();
    KoBindings(_pesquisaIntegracaoNFe, "knockoutPesquisaIntegracaoNFe");

    BuscarIntegracoes();
    if (!_ConfiguracaoTMS.ProcessarXMLNotasFiscaisAssincrono) {
        _pesquisaIntegracaoNFe.SituacaoIntegracao.visible(false);
        _pesquisaIntegracaoNFe.SituacaoProcessamentoRegistro.visible(true);
    };
}

//#endregion

//#region Funções Auxiliares

function BuscarIntegracoes() {
    const reenviar = { descricao: "Reenviar", id: guid(), evento: "onclick", metodo: ReenviarIntegracao, tamanho: "15", icone: "" };
    const baixar = { descricao: "Arquivos Integração Retorno", id: guid(), evento: "onclick", metodo: ExibirHistoricoIntegracao, tamanho: "15", icone: "" };
    const download = { descricao: "Download", id: guid(), evento: "onclick", metodo: DownloadArquivosIntegracao, tamanho: "15", icone: "" };

    const menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(reenviar);
    //menuOpcoes.opcoes.push(baixar);
    if (_ConfiguracaoTMS.ProcessarXMLNotasFiscaisAssincrono)
        menuOpcoes.opcoes.push(download);

    _gridIntegracaoNFe = new GridView(_pesquisaIntegracaoNFe.Pesquisar.idGrid, "IntegracaoNFe/Pesquisar", _pesquisaIntegracaoNFe, menuOpcoes, null);
    _gridIntegracaoNFe.CarregarGrid();
}

function ReenviarIntegracao(data) {
    executarReST("IntegracaoNFe/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
            _gridIntegracaoNFe.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}
function DownloadArquivosHistoricoIntegracao(data) {
    executarDownload("IntegracaoNFe/DownloadArquivosHistoricoIntegracao", { Codigo: data.Codigo });
}
function DownloadArquivosIntegracao(data) {
    executarDownload("IntegracaoNFe/DownloadArquivosIntegracao", { Codigo: data.Codigo });
}
var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function BuscarHistoricoIntegracao(integracao) {
    _pesquisaHistoricoIntegracao = new PesquisaHistoricoIntegracao();
    _pesquisaHistoricoIntegracao.Codigo.val(integracao.Codigo);

    let download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracao, tamanho: "20", icone: "" };

    let menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "IntegracaoNFe/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function ExibirHistoricoIntegracao(integracao) {
    BuscarHistoricoIntegracao(integracao);
    Global.abrirModal("divModalHistoricoIntegracao");
}

//#endregion