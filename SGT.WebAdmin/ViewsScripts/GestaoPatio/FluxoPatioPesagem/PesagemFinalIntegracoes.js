/// <reference path="Pesagem.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridPesagemFinalIntegracoes;
var _pesagemFinalIntegracao;
var _pesquisaPesagemFinalIntegracoes;
var _problemaPesagemFinalIntegracao;
var _pesquisaHistoricoPesagemFinalIntegracao;
var _gridHistoricoPesagemFinalIntegracao;


/*
 * Declaração das Classes
 */
var PesquisaHistoricoPesagemFinalIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function PesagemFinalIntegracao() {
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Integração:" });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Retorno:" });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Geral:" });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Integrados:" });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Problemas na Integração:" });

    this.ObterTotais = PropertyEntity({ eventClick: carregarTotaisPesagemFinalIntegracao, type: types.event, text: "Obter Totais", idGrid: guid(), visible: ko.observable(true) });
}

function PesquisaPesagemFinalIntegracoes() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: "Situação:", def: "", issue: 272 });

    this.Pesquisar = PropertyEntity({ eventClick: carregarPesagemFinalIntegracoes, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.ReenviarTodos = PropertyEntity({ eventClick: reenviarTodasPesagemFinalIntegracoesClick, type: types.event, text: "Reenviar Todas", visible: ko.observable(false) });
}

function ProblemaPesagemFinalIntegracao() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ type: types.map, val: ko.observable(""), text: "*Motivo:", maxlength: 300, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarProblemaPesagemFinalIntegracaoClick, type: types.event, text: ko.observable("Adicionar") });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridPesagemFinalIntegracoes() {
    var linhasPorPaginas = 5;
    var opcaoIntegrar = { descricao: "Reenviar", id: guid(), metodo: integrarPesagemFinalClick, icone: "" };
    //var opcaoProblemaIntegracao = { descricao: "Problema", id: guid(), metodo: exibirModalProblemaPesagemFinalIntegracaoClick, icone: "" };
    var historico = { descricao: "Histórico de Integração", id: guid(), metodo: exibirHistoricoPesagemFinalIntegracoesClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [opcaoIntegrar, historico] }; //, opcaoProblemaIntegracao

    _gridPesagemFinalIntegracoes = new GridView(_pesquisaPesagemFinalIntegracoes.Pesquisar.idGrid, "Pesagem/PesquisaPesagemIntegracoes", _pesquisaPesagemFinalIntegracoes, menuOpcoes, null, linhasPorPaginas);
}

function loadPesagemFinalIntegracoes() {
    $.get("Content/Static/Integracao/Integracao.html?dyn=" + guid(), function (html) {

        html = html.replace("knockoutPesquisaIntegracao", "knockoutPesquisaIntegracao_PesagemFinal");
        html = html.replace("knockoutDadosIntegracao", "knockoutDadosIntegracao_PesagemFinal");

        $("#tabIntegracoesPesagemFinalConteudo").append(html);

        LocalizeCurrentPage();

        _pesagemFinalIntegracao = new PesagemFinalIntegracao();
        KoBindings(_pesagemFinalIntegracao, "knockoutDadosIntegracao_PesagemFinal");

        _pesquisaPesagemFinalIntegracoes = new PesquisaPesagemFinalIntegracoes();
        KoBindings(_pesquisaPesagemFinalIntegracoes, "knockoutPesquisaIntegracao_PesagemFinal", false, _pesquisaPesagemFinalIntegracoes.Pesquisar.id);

        //_problemaPesagemFinalIntegracao = new ProblemaPesagemFinalIntegracao();
        //KoBindings(_problemaPesagemFinalIntegracao, "knockoutMotivoProblemaIntegracao");

        loadGridPesagemFinalIntegracoes();

        $("#liTabPesagemFinalIntegracao").on("click", function () {
            recarregarPesagemFinalIntegracoes();
        });
    });
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function exibirHistoricoPesagemFinalIntegracoesClick(integracao) {
    BuscarHistoricoPesagemFinalIntegracao(integracao);
    Global.abrirModal("divModalHistoricoIntegracao");
}

function adicionarProblemaPesagemFinalIntegracaoClick() {
    if (ValidarCamposObrigatorios(_problemaPesagemFinalIntegracao)) {
        var dadosProblemaIntegracao = RetornarObjetoPesquisa(_problemaPesagemFinalIntegracao);

        executarReST("Pesagem/ProblemaIntegracao", dadosProblemaIntegracao, function (retorno) {
            if (retorno.Success) {
                fecharModalProblemaPesagemFinalIntegracao();
                carregarPesagemFinalIntegracoes();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
            }
        });
    }
    else
        exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
}

function exibirModalProblemaPesagemFinalIntegracaoClick(registroSelecionado) {
    _problemaPesagemFinalIntegracao.Codigo.val(registroSelecionado.Codigo);

    Global.abrirModal('divModalMotivoProblemaIntegracao');
    $("#divModalMotivoProblemaIntegracao").one('hidden.bs.modal', function () {
        LimparCampos(_problemaPesagemFinalIntegracao);
    });
}

function integrarPesagemFinalClick(registroSelecionado) {
    executarReST("Pesagem/Integrar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                carregarPesagemFinalIntegracoes();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
        }
    });
}

function reenviarTodasPesagemFinalIntegracoesClick() {

}

function DownloadArquivosHistoricoPesagemFinalIntegracaoClick(historicoConsulta) {
    executarDownload("Pesagem/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

/*
 * Declaração das Funções Públicas
 */

function recarregarPesagemFinalIntegracoes() {
    _pesquisaPesagemFinalIntegracoes.Codigo.val(_pesagemFinal.CodigoPesagem.val());

    controlarExibicaoAbaPesagemFinalIntegracoes();
    carregarPesagemFinalIntegracoes();
}

/*
 * Declaração das Funções
 */

function BuscarHistoricoPesagemFinalIntegracao(integracao) {
    _pesquisaHistoricoPesagemFinalIntegracao = new PesquisaHistoricoPesagemFinalIntegracao();
    _pesquisaHistoricoPesagemFinalIntegracao.Codigo.val(integracao.Codigo);

    var download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoPesagemFinalIntegracaoClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoPesagemFinalIntegracao = new GridView("tblHistoricoIntegracao", "Pesagem/ConsultarHistoricoIntegracao", _pesquisaHistoricoPesagemFinalIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoPesagemFinalIntegracao.CarregarGrid();
}

function carregarPesagemFinalIntegracoes() {
    _gridPesagemFinalIntegracoes.CarregarGrid();
    atualizarCampoPesagemFinal();
    carregarTotaisPesagemFinalIntegracao();
    
}

function carregarTotaisPesagemFinalIntegracao() {
    executarReST("Pesagem/ObterTotaisIntegracoes", { Codigo: _pesquisaPesagemFinalIntegracoes.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            _pesagemFinalIntegracao.TotalGeral.val(retorno.Data.TotalGeral);
            _pesagemFinalIntegracao.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
            _pesagemFinalIntegracao.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
            _pesagemFinalIntegracao.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
            _pesagemFinalIntegracao.TotalIntegrado.val(retorno.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
        }
    });
}

function controlarExibicaoAbaPesagemFinalIntegracoes() {
    if (_pesquisaPesagemFinalIntegracoes.Codigo.val() > 0)
        $("#liTabIntegracoes").show();
    else
        $("#liTabIntegracoes").hide();
}

function fecharModalProblemaPesagemFinalIntegracao() {
    Global.fecharModal("divModalMotivoProblemaIntegracao");
}

function atualizarCampoPesagemFinal() {
    var codigo = _pesagemFinal.CodigoGuarita.val()
    if (!codigo) return;

    executarReST("Guarita/BuscarInformacoesPesagemFinal", { Codigo: codigo }, function (retorno) {
        if (retorno.Success && retorno.Data) {
            _pesagemFinal.PesagemFinal.val(retorno.Data.PesagemFinal);
        }
    });
}