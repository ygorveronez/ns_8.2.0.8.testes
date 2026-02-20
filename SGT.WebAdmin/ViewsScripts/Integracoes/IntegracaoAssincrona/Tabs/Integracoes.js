var _integracaoIntegradora;
var _gridIntegracoes;
var _pesquisaHistoricoIntegracaoAssincrona;

var IntegracaoIntegradora = function () {
    this.Codigo = PropertyEntity({ visible: ko.observable(false) });
    this.Situacao = PropertyEntity({ text: "Status", val: ko.observable(EnumSituacaoIntegracao.Todos), options: EnumSituacaoIntegracao.obterOpcoesPesquisa(), def: EnumSituacaoIntegracao.Todos });
    this.Pesquisar = PropertyEntity({
        type: types.event, eventClick: function (e) {
            recarregarGridDetalhesIntegracoes();
        }, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.Grid = PropertyEntity({ id: guid() });
}

var PesquisaHistoricoIntegracaoAssincrona = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "" });
};

function carregarIngradoraIntegracao() {
    _integracaoIntegradora = new IntegracaoIntegradora();

    KoBindings(_integracaoIntegradora, "knoutIntegracaoIntegradora");

    var historicoIntegracao = {
        descricao: "Histórico de Integração",
        id: guid(),
        evento: "onclick",
        metodo: historicoIntegracaoClick,
        tamanho: "10",
        icone: ""
    };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [historicoIntegracao] };

    _gridIntegracoes = new GridView(_integracaoIntegradora.Grid.id, "IntegracaoAssincrona/BuscarIntegracoesIntegradora", _integracaoIntegradora, menuOpcoes);
}

function recarregarGridDetalhesIntegracoes() {
    _gridIntegracoes.CarregarGrid();
}

function historicoIntegracaoClick(integracao) {
    buscarHistoricoIntegracao(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoAssincrona");
}

function buscarHistoricoIntegracao(integracao) {
    _pesquisaHistoricoIntegracaoAssincrona = new PesquisaHistoricoIntegracaoAssincrona();
    _pesquisaHistoricoIntegracaoAssincrona.Codigo.val(integracao.Codigo);

    var download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: downloadArquivosHistoricoIntegracao, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracao = new GridView("grid-gestao-carga-integracao-assincrona-historico-integracao", "IntegracaoAssincrona/BuscarHistoricoIntegracao", _pesquisaHistoricoIntegracaoAssincrona, menuOpcoes);
    _gridHistoricoIntegracao.CarregarGrid();
}

function downloadArquivosHistoricoIntegracao(historicoSelecionado) {
    var dados = { Codigo: historicoSelecionado.Codigo };

    executarDownload("IntegracaoAssincrona/DownloadArquivosHistoricoIntegracao", dados);
}