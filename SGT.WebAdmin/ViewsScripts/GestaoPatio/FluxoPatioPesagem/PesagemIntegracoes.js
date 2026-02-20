/// <reference path="Pesagem.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridPesagemIntegracoes;
var _pesagemIntegracao;
var _pesquisaPesagemIntegracoes;
var _problemaPesagemIntegracao;
var _pesquisaHistoricoIntegracao;
var _gridHistoricoIntegracao;


/*
 * Declaração das Classes
 */
var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function PesagemIntegracao() {
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Integração:" });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Retorno:" });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Geral:" });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Integrados:" });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Problemas na Integração:" });

    this.ObterTotais = PropertyEntity({ eventClick: carregarTotaisIntegracao, type: types.event, text: "Obter Totais", idGrid: guid(), visible: ko.observable(true) });
}

function PesquisaPesagemIntegracoes() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: "Situação:", def: "", issue: 272 });

    this.Pesquisar = PropertyEntity({ eventClick: carregarIntegracoes, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.ReenviarTodos = PropertyEntity({ eventClick: reenviarTodasIntegracoesClick, type: types.event, text: "Reenviar Todas", visible: ko.observable(false) });
}

function ProblemaPesagemIntegracao() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ type: types.map, val: ko.observable(""), text: "*Motivo:", maxlength: 300, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarProblemaIntegracaoClick, type: types.event, text: ko.observable("Adicionar") });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridPesagemIntegracoes() {
    var linhasPorPaginas = 5;
    var opcaoIntegrar = { descricao: "Reenviar", id: guid(), metodo: integrarClick, icone: "" };
    //var opcaoProblemaIntegracao = { descricao: "Problema", id: guid(), metodo: exibirModalProblemaIntegracaoClick, icone: "" };
    var historico = { descricao: "Histórico de Integração", id: guid(), metodo: exibirHistoricoIntegracoesClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [opcaoIntegrar, historico] }; //, opcaoProblemaIntegracao

    _gridPesagemIntegracoes = new GridView(_pesquisaPesagemIntegracoes.Pesquisar.idGrid, "Pesagem/PesquisaPesagemIntegracoes", _pesquisaPesagemIntegracoes, menuOpcoes, null, linhasPorPaginas);
}

function loadPesagemIntegracoes() {
    $.get("Content/Static/Integracao/Integracao.html?dyn=" + guid(), function (html) {
        $("#tabIntegracoesPesagemInicialConteudo").append(html);

        LocalizeCurrentPage();

        _pesagemIntegracao = new PesagemIntegracao();
        KoBindings(_pesagemIntegracao, "knockoutDadosIntegracao");

        _pesquisaPesagemIntegracoes = new PesquisaPesagemIntegracoes();
        KoBindings(_pesquisaPesagemIntegracoes, "knockoutPesquisaIntegracao", false, _pesquisaPesagemIntegracoes.Pesquisar.id);

        _problemaPesagemIntegracao = new ProblemaPesagemIntegracao();
        KoBindings(_problemaPesagemIntegracao, "knockoutMotivoProblemaIntegracao");

        loadGridPesagemIntegracoes();

        $("#liTabPesagemInicialIntegracao").on("click", function () {
            recarregarPesagemIntegracoes();
        });
    });
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function exibirHistoricoIntegracoesClick(integracao) {
    BuscarHistoricoIntegracao(integracao);
    Global.abrirModal("divModalHistoricoIntegracao");
}

function adicionarProblemaIntegracaoClick() {
    if (ValidarCamposObrigatorios(_problemaPesagemIntegracao)) {
        var dadosProblemaIntegracao = RetornarObjetoPesquisa(_problemaPesagemIntegracao);

        executarReST("Pesagem/ProblemaIntegracao", dadosProblemaIntegracao, function (retorno) {
            if (retorno.Success) {
                fecharModalProblemaIntegracao();
                carregarIntegracoes();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
            }
        });
    }
    else
        exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
}

function exibirModalProblemaIntegracaoClick(registroSelecionado) {
    _problemaPesagemIntegracao.Codigo.val(registroSelecionado.Codigo);

    Global.abrirModal('divModalMotivoProblemaIntegracao');
    $("#divModalMotivoProblemaIntegracao").one('hidden.bs.modal', function () {
        LimparCampos(_problemaPesagemIntegracao);
    });
}

function integrarClick(registroSelecionado) {
    executarReST("Pesagem/Integrar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                carregarIntegracoes();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
        }
    });
}

function reenviarTodasIntegracoesClick() {

}

function DownloadArquivosHistoricoIntegracaoClick(historicoConsulta) {
    executarDownload("Pesagem/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

/*
 * Declaração das Funções Públicas
 */

function recarregarPesagemIntegracoes() {
    _pesquisaPesagemIntegracoes.Codigo.val(_pesagem.CodigoPesagem.val());

    controlarExibicaoAbaIntegracoes();
    carregarIntegracoes();
}

/*
 * Declaração das Funções
 */

function BuscarHistoricoIntegracao(integracao) {
    _pesquisaHistoricoIntegracao = new PesquisaHistoricoIntegracao();
    _pesquisaHistoricoIntegracao.Codigo.val(integracao.Codigo);

    var download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "Pesagem/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function carregarIntegracoes() {
    _gridPesagemIntegracoes.CarregarGrid();

    carregarTotaisIntegracao();
}

function carregarTotaisIntegracao() {
    executarReST("Pesagem/ObterTotaisIntegracoes", { Codigo: _pesquisaPesagemIntegracoes.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            _pesagemIntegracao.TotalGeral.val(retorno.Data.TotalGeral);
            _pesagemIntegracao.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
            _pesagemIntegracao.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
            _pesagemIntegracao.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
            _pesagemIntegracao.TotalIntegrado.val(retorno.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
        }
    });
}

function controlarExibicaoAbaIntegracoes() {
    if (_pesquisaPesagemIntegracoes.Codigo.val() > 0)
        $("#liTabIntegracoes").show();
    else
        $("#liTabIntegracoes").hide();
}

function fecharModalProblemaIntegracao() {
    Global.fecharModal("divModalMotivoProblemaIntegracao");
}