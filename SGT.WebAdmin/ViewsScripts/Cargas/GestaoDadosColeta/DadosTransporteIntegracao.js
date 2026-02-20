/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumSituacaoGestaoDadosColeta.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />
/// <reference path="GestaoDadosColeta.js" />

// #region Objetos Globais do Arquivo

var _gridGestaoDadosColetaDadosTransporteIntegracao;
var _gridGestaoDadosColetaDadosTransporteIntegracaoHistorico;
var _pesquisaGestaoDadosColetaDadosTransporteIntegracao;
var _pesquisaGestaoDadosColetaDadosTransporteIntegracaoHistorico;
var _totalizacaoGestaoDadosColetaDadosTransporteIntegracao;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaGestaoDadosColetaDadosTransporteIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: "Situação:", def: "", issue: 272 });

    this.Pesquisar = PropertyEntity({ eventClick: carregarGestaoDadosColetaDadosTransporteIntegracao, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true) });
    this.ReenviarTodos = PropertyEntity({ type: types.event, text: "Reeviar Todos", visible: false });
};

var PesquisaGestaoDadosColetaDadosTransporteIntegracaoHistorico = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

var TotalizacaoGestaoDadosColetaDadosTransporteIntegracao = function () {
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Integração:" });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Retorno" });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Geral:" });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Integrados:" });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Problemas na Integração:" });
    this.ObterTotais = PropertyEntity({ eventClick: carregarGestaoDadosColetaDadosTransporteIntegracaoTotais, type: types.event, text: "Obter Totais:", idGrid: guid(), visible: ko.observable(true) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadGridGestaoDadosColetaDadosTransporteIntegracao() {
    var linhasPorPaginas = 5;
    var opcaoIntegrar = { descricao: "Integrar", id: guid(), metodo: integrarGestaoDadosColetaDadosTransporteClick, icone: "" };
    var historico = { descricao: "Histórico Integracação", id: guid(), metodo: exibirModalGestaoDadosColetaDadosTransporteIntegracaoHistoricoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [opcaoIntegrar, historico] };

    _gridGestaoDadosColetaDadosTransporteIntegracao = new GridView(_pesquisaGestaoDadosColetaDadosTransporteIntegracao.Pesquisar.idGrid, "GestaoDadosColetaIntegracao/PesquisaGestaoDadosColetaIntegracoes", _pesquisaGestaoDadosColetaDadosTransporteIntegracao, menuOpcoes, null, linhasPorPaginas);
    _gridGestaoDadosColetaDadosTransporteIntegracao.CarregarGrid();
}

function loadGridGestaoDadosColetaDadosTransporteIntegracaoHistorico(integracao) {
    var download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: downloadGestaoDadosColetaDadosTransporteIntegracaoHistoricoArquivosClick, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [download] };

    _gridGestaoDadosColetaDadosTransporteIntegracaoHistorico = new GridView("gridHistoricoIntegracaoGestaoDadosColetaDadosTransporte", "GestaoDadosColetaIntegracao/ConsultarHistoricoIntegracao", _pesquisaGestaoDadosColetaDadosTransporteIntegracaoHistorico, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridGestaoDadosColetaDadosTransporteIntegracaoHistorico.CarregarGrid();
}

function loadGestaoDadosColetaDadosTransporteIntegracao() {
    $.get("Content/Static/Integracao/Integracao.html?dyn=" + guid(), function (html) {
        html = html
            .replace("knockoutDadosIntegracao", "knockoutIntegracaoGestaoDadosColetaDadosTransporte")
            .replace("knockoutPesquisaIntegracao", "knockoutPesquisaIntegracaoGestaoDadosColetaDadosTransporte");

        $("#containerIntegracaoGestaoDadosColetaDadosTransporte").append(html);

        LocalizeCurrentPage();

        _totalizacaoGestaoDadosColetaDadosTransporteIntegracao = new TotalizacaoGestaoDadosColetaDadosTransporteIntegracao();
        KoBindings(_totalizacaoGestaoDadosColetaDadosTransporteIntegracao, "knockoutIntegracaoGestaoDadosColetaDadosTransporte");

        _pesquisaGestaoDadosColetaDadosTransporteIntegracao = new PesquisaGestaoDadosColetaDadosTransporteIntegracao();
        KoBindings(_pesquisaGestaoDadosColetaDadosTransporteIntegracao, "knockoutPesquisaIntegracaoGestaoDadosColetaDadosTransporte", false, _pesquisaGestaoDadosColetaDadosTransporteIntegracao.Pesquisar.id);

        _pesquisaGestaoDadosColetaDadosTransporteIntegracaoHistorico = new PesquisaGestaoDadosColetaDadosTransporteIntegracaoHistorico();

        loadGridGestaoDadosColetaDadosTransporteIntegracao();
        loadGridGestaoDadosColetaDadosTransporteIntegracaoHistorico();
    });
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function downloadGestaoDadosColetaDadosTransporteIntegracaoHistoricoArquivosClick(registroSelecionado) {
    executarDownload("GestaoDadosColetaIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: registroSelecionado.Codigo });
}

function exibirModalGestaoDadosColetaDadosTransporteIntegracaoHistoricoClick(registroSelecionado) {
    _pesquisaGestaoDadosColetaDadosTransporteIntegracaoHistorico.Codigo.val(registroSelecionado.Codigo);
    _gridGestaoDadosColetaDadosTransporteIntegracaoHistorico.CarregarGrid();
    Global.abrirModal("divModalHistoricoIntegracaoGestaoDadosColetaDadosTransporte");
}

function integrarGestaoDadosColetaDadosTransporteClick(registroSelecionado) {
    executarReST("GestaoDadosColetaIntegracao/Integrar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                carregarGestaoDadosColetaDadosTransporteIntegracao();
                carregarGestaoDadosColetaDadosTransporteIntegracaoTotais();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function limparGestaoDadosColetaDadosTransporteIntegracao() {
    LimparCampos(_pesquisaGestaoDadosColetaDadosTransporteIntegracao);

    $("#liTabIntegracaoGestaoDadosColetaDadosTransporte").hide();
}

function preencherGestaoDadosColetaDadosTransporteIntegracao(codigo, situacao) {
    if (situacao != EnumSituacaoGestaoDadosColeta.Aprovado)
        return;

    _pesquisaGestaoDadosColetaDadosTransporteIntegracao.Codigo.val(codigo);

    carregarGestaoDadosColetaDadosTransporteIntegracao();
    carregarGestaoDadosColetaDadosTransporteIntegracaoTotais();
    $("#liTabIntegracaoGestaoDadosColetaDadosTransporte").show();
}

// #endregion Funções Públicas

// #region Funções Privadas

function carregarGestaoDadosColetaDadosTransporteIntegracao() {
    _gridGestaoDadosColetaDadosTransporteIntegracao.CarregarGrid();
}

function carregarGestaoDadosColetaDadosTransporteIntegracaoTotais() {
    executarReST("GestaoDadosColetaIntegracao/ObterTotaisIntegracoes", { Codigo: _pesquisaGestaoDadosColetaDadosTransporteIntegracao.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            _totalizacaoGestaoDadosColetaDadosTransporteIntegracao.TotalGeral.val(retorno.Data.TotalGeral);
            _totalizacaoGestaoDadosColetaDadosTransporteIntegracao.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
            _totalizacaoGestaoDadosColetaDadosTransporteIntegracao.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
            _totalizacaoGestaoDadosColetaDadosTransporteIntegracao.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
            _totalizacaoGestaoDadosColetaDadosTransporteIntegracao.TotalIntegrado.val(retorno.Data.TotalIntegrado);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Privadas
