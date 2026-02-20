/// <reference path="SeparacaoPedido.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridSeparacaoPedidoIntegracoes;
var _separacaoPedidoIntegracao;
var _pesquisaSeparacaoPedidoIntegracoes;
var _problemaSeparacaoPedidoIntegracao;
var _pesquisaHistoricoIntegracao;
var _gridHistoricoIntegracao;


/*
 * Declaração das Classes
 */
var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function SeparacaoPedidoIntegracao() {
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Integração:" });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Retorno:" });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Geral:" });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Integrados:" });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Problemas na Integração:" });

    this.ObterTotais = PropertyEntity({ eventClick: carregarTotaisIntegracao, type: types.event, text: "Obter Totais", idGrid: guid(), visible: ko.observable(true) });
}

function PesquisaSeparacaoPedidoIntegracoes() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: "Situação:", def: "", issue: 272 });

    this.Pesquisar = PropertyEntity({ eventClick: carregarIntegracoes, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.ReenviarTodos = PropertyEntity({ eventClick: reenviarTodasIntegracoesClick, type: types.event, text: "Reenviar Todas", visible: ko.observable(false) });
}

function ProblemaSeparacaoPedidoIntegracao() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ type: types.map, val: ko.observable(""), text: "*Motivo:", maxlength: 300, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarProblemaIntegracaoClick, type: types.event, text: ko.observable("Adicionar") });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridSeparacaoPedidoIntegracoes() {
    var linhasPorPaginas = 5;
    var opcaoIntegrar = { descricao: "Integrar", id: guid(), metodo: integrarClick, icone: "" };
    //var opcaoProblemaIntegracao = { descricao: "Problema", id: guid(), metodo: exibirModalProblemaIntegracaoClick, icone: "" };
    var historico = { descricao: "Histórico de Integração", id: guid(), metodo: exibirHistoricoIntegracoesClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [opcaoIntegrar, historico] }; //, opcaoProblemaIntegracao

    _gridSeparacaoPedidoIntegracoes = new GridView(_pesquisaSeparacaoPedidoIntegracoes.Pesquisar.idGrid, "SeparacaoPedido/PesquisaSeparacaoPedidoIntegracoes", _pesquisaSeparacaoPedidoIntegracoes, menuOpcoes, null, linhasPorPaginas);
    _gridSeparacaoPedidoIntegracoes.CarregarGrid();
}

function loadSeparacaoPedidoIntegracoes() {
    $.get("Content/Static/Integracao/Integracao.html?dyn=" + guid(), function (html) {
        $("#tabIntegracoesConteudo").append(html);

        LocalizeCurrentPage();

        _separacaoPedidoIntegracao = new SeparacaoPedidoIntegracao();
        KoBindings(_separacaoPedidoIntegracao, "knockoutDadosIntegracao");

        _pesquisaSeparacaoPedidoIntegracoes = new PesquisaSeparacaoPedidoIntegracoes();
        KoBindings(_pesquisaSeparacaoPedidoIntegracoes, "knockoutPesquisaIntegracao", false, _pesquisaSeparacaoPedidoIntegracoes.Pesquisar.id);

        _problemaSeparacaoPedidoIntegracao = new ProblemaSeparacaoPedidoIntegracao();
        KoBindings(_problemaSeparacaoPedidoIntegracao, "knockoutMotivoProblemaIntegracao");

        loadGridSeparacaoPedidoIntegracoes();
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
    if (ValidarCamposObrigatorios(_problemaSeparacaoPedidoIntegracao)) {
        var dadosProblemaIntegracao = RetornarObjetoPesquisa(_problemaSeparacaoPedidoIntegracao);

        executarReST("SeparacaoPedido/ProblemaIntegracao", dadosProblemaIntegracao, function (retorno) {
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
    _problemaSeparacaoPedidoIntegracao.Codigo.val(registroSelecionado.Codigo);
        
    Global.abrirModal("divModalMotivoProblemaIntegracao");
    $("#divModalMotivoProblemaIntegracao").one('hidden.bs.modal', function () {
        LimparCampos(_problemaSeparacaoPedidoIntegracao);
    });
}

function integrarClick(registroSelecionado) {
    executarReST("SeparacaoPedido/Integrar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            carregarIntegracoes();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
        }
    });
}

function reenviarTodasIntegracoesClick() {

}

function DownloadArquivosHistoricoIntegracaoClick(historicoConsulta) {
    executarDownload("SeparacaoPedido/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

/*
 * Declaração das Funções Públicas
 */

function recarregarSeparacaoPedidoIntegracoes() {
    _pesquisaSeparacaoPedidoIntegracoes.Codigo.val(_separacaoPedido.Codigo.val());

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

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "SeparacaoPedido/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function carregarIntegracoes() {
    _gridSeparacaoPedidoIntegracoes.CarregarGrid();

    carregarTotaisIntegracao();
}

function carregarTotaisIntegracao() {
    executarReST("SeparacaoPedido/ObterTotaisIntegracoes", { Codigo: _pesquisaSeparacaoPedidoIntegracoes.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            _separacaoPedidoIntegracao.TotalGeral.val(retorno.Data.TotalGeral);
            _separacaoPedidoIntegracao.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
            _separacaoPedidoIntegracao.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
            _separacaoPedidoIntegracao.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
            _separacaoPedidoIntegracao.TotalIntegrado.val(retorno.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
        }
    });
}

function controlarExibicaoAbaIntegracoes() {
    if (_pesquisaSeparacaoPedidoIntegracoes.Codigo.val() > 0)
        $("#liTabIntegracoes").show();
    else
        $("#liTabIntegracoes").hide();
}

function fecharModalProblemaIntegracao() {
    Global.fecharModal("divModalMotivoProblemaIntegracao");
}