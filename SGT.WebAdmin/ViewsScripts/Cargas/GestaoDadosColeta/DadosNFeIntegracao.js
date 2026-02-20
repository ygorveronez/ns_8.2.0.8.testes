/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumSituacaoGestaoDadosColeta.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />
/// <reference path="GestaoDadosColeta.js" />

// #region Objetos Globais do Arquivo

var _gridGestaoDadosColetaDadosNFeIntegracao;
var _gridGestaoDadosColetaDadosNFeIntegracaoHistorico;
var _pesquisaGestaoDadosColetaDadosNFeIntegracao;
var _pesquisaGestaoDadosColetaDadosNFeIntegracaoHistorico;
var _totalizacaoGestaoDadosColetaDadosNFeIntegracao;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaGestaoDadosColetaDadosNFeIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: "Situação:", def: "", issue: 272 });

    this.Pesquisar = PropertyEntity({ eventClick: carregarGestaoDadosColetaDadosNFeIntegracao, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true) });
    this.ReenviarTodos = PropertyEntity({ type: types.event, text: "Reeviar Todos", visible: false });
};

var PesquisaGestaoDadosColetaDadosNFeIntegracaoHistorico = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

var TotalizacaoGestaoDadosColetaDadosNFeIntegracao = function () {
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Integração:" });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Retorno" });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Geral:" });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Integrados:" });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Problemas na Integração:" });
    this.ObterTotais = PropertyEntity({ eventClick: carregarGestaoDadosColetaDadosNFeIntegracaoTotais, type: types.event, text: "Obter Totais:", idGrid: guid(), visible: ko.observable(true) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadGridGestaoDadosColetaDadosNFeIntegracao() {
    var linhasPorPaginas = 5;
    var opcaoIntegrar = { descricao: "Integrar", id: guid(), metodo: integrarGestaoDadosColetaDadosNFeClick, icone: "" };
    var historico = { descricao: "Histórico Integracação", id: guid(), metodo: exibirModalGestaoDadosColetaDadosNFeIntegracaoHistoricoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [opcaoIntegrar, historico] };

    _gridGestaoDadosColetaDadosNFeIntegracao = new GridView(_pesquisaGestaoDadosColetaDadosNFeIntegracao.Pesquisar.idGrid, "GestaoDadosColetaIntegracao/PesquisaGestaoDadosColetaIntegracoes", _pesquisaGestaoDadosColetaDadosNFeIntegracao, menuOpcoes, null, linhasPorPaginas);
    _gridGestaoDadosColetaDadosNFeIntegracao.CarregarGrid();
}

function loadGridGestaoDadosColetaDadosNFeIntegracaoHistorico(integracao) {
    var download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: downloadGestaoDadosColetaDadosNFeIntegracaoHistoricoArquivosClick, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [download] };

    _gridGestaoDadosColetaDadosNFeIntegracaoHistorico = new GridView("gridHistoricoIntegracaoGestaoDadosColetaDadosNfe", "GestaoDadosColetaIntegracao/ConsultarHistoricoIntegracao", _pesquisaGestaoDadosColetaDadosNFeIntegracaoHistorico, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridGestaoDadosColetaDadosNFeIntegracaoHistorico.CarregarGrid();
}

function loadGestaoDadosColetaDadosNFeIntegracao() {
    $.get("Content/Static/Integracao/Integracao.html?dyn=" + guid(), function (html) {
        html = html
            .replace("knockoutDadosIntegracao", "knockoutIntegracaoGestaoDadosColetaDadosNFe")
            .replace("knockoutPesquisaIntegracao", "knockoutPesquisaIntegracaoGestaoDadosColetaDadosNFe");

        $("#containerIntegracaoGestaoDadosColetaDadosNFe").append(html);

        LocalizeCurrentPage();

        _totalizacaoGestaoDadosColetaDadosNFeIntegracao = new TotalizacaoGestaoDadosColetaDadosNFeIntegracao();
        KoBindings(_totalizacaoGestaoDadosColetaDadosNFeIntegracao, "knockoutIntegracaoGestaoDadosColetaDadosNFe");

        _pesquisaGestaoDadosColetaDadosNFeIntegracao = new PesquisaGestaoDadosColetaDadosNFeIntegracao();
        KoBindings(_pesquisaGestaoDadosColetaDadosNFeIntegracao, "knockoutPesquisaIntegracaoGestaoDadosColetaDadosNFe", false, _pesquisaGestaoDadosColetaDadosNFeIntegracao.Pesquisar.id);

        _pesquisaGestaoDadosColetaDadosNFeIntegracaoHistorico = new PesquisaGestaoDadosColetaDadosNFeIntegracaoHistorico();

        loadGridGestaoDadosColetaDadosNFeIntegracao();
        loadGridGestaoDadosColetaDadosNFeIntegracaoHistorico();
    });
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function downloadGestaoDadosColetaDadosNFeIntegracaoHistoricoArquivosClick(registroSelecionado) {
    executarDownload("GestaoDadosColetaIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: registroSelecionado.Codigo });
}

function exibirModalGestaoDadosColetaDadosNFeIntegracaoHistoricoClick(registroSelecionado) {
    _pesquisaGestaoDadosColetaDadosNFeIntegracaoHistorico.Codigo.val(registroSelecionado.Codigo);
    _gridGestaoDadosColetaDadosNFeIntegracaoHistorico.CarregarGrid();
    Global.abrirModal("divModalHistoricoIntegracaoGestaoDadosColetaDadosNfe");
}

function integrarGestaoDadosColetaDadosNFeClick(registroSelecionado) {
    executarReST("GestaoDadosColetaIntegracao/Integrar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                carregarGestaoDadosColetaDadosNFeIntegracao();
                carregarGestaoDadosColetaDadosNFeIntegracaoTotais();
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

function limparGestaoDadosColetaDadosNFeIntegracao() {
    LimparCampos(_pesquisaGestaoDadosColetaDadosNFeIntegracao);

    $("#liTabIntegracaoGestaoDadosColetaDadosNFe").hide();
}

function preencherGestaoDadosColetaDadosNFeIntegracao(codigo, situacao) {
    if (situacao != EnumSituacaoGestaoDadosColeta.Aprovado)
        return;

    _pesquisaGestaoDadosColetaDadosNFeIntegracao.Codigo.val(codigo);

    carregarGestaoDadosColetaDadosNFeIntegracao();
    carregarGestaoDadosColetaDadosNFeIntegracaoTotais();
    $("#liTabIntegracaoGestaoDadosColetaDadosNFe").show();
}

// #endregion Funções Públicas

// #region Funções Privadas

function carregarGestaoDadosColetaDadosNFeIntegracao() {
    _gridGestaoDadosColetaDadosNFeIntegracao.CarregarGrid();
}

function carregarGestaoDadosColetaDadosNFeIntegracaoTotais() {
    executarReST("GestaoDadosColetaIntegracao/ObterTotaisIntegracoes", { Codigo: _pesquisaGestaoDadosColetaDadosNFeIntegracao.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            _totalizacaoGestaoDadosColetaDadosNFeIntegracao.TotalGeral.val(retorno.Data.TotalGeral);
            _totalizacaoGestaoDadosColetaDadosNFeIntegracao.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
            _totalizacaoGestaoDadosColetaDadosNFeIntegracao.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
            _totalizacaoGestaoDadosColetaDadosNFeIntegracao.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
            _totalizacaoGestaoDadosColetaDadosNFeIntegracao.TotalIntegrado.val(retorno.Data.TotalIntegrado);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Privadas
