/// <reference path="TabelaFreteCliente.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridTabelaFreteClienteIntegracoes;
var _tabelaFreteClienteIntegracao;
var _pesquisaTabelaFreteClienteIntegracoes;
var _problemaTabelaFreteClienteIntegracao;
var _pesquisaHistoricoIntegracaoTabelaFrete;
var _gridHistoricoIntegracaoTabelaFreteIntegracao;

/*
 * Declaração das Classes
 */
var PesquisaHistoricoIntegracaoTabelaFrete = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Historico = PropertyEntity({ idGrid: guid() });
};

function PesquisaTabelaFreteClienteIntegracoes() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), def: "", issue: 272 });

    this.Pesquisar = PropertyEntity({ eventClick: carregarIntegracoes, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true) });
    this.ReenviarTodos = PropertyEntity({ eventClick: "", type: types.event, text: Localization.Resources.Gerais.Geral.ReenviarTodas, visible: ko.observable(false) });
};

function TabelaFreteClienteIntegracao() {
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoIntegracao.getFieldDescription() });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoRetorno.getFieldDescription() });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.TotalGeral.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Integrados.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.ProblemasIntegracao.getFieldDescription() });

    this.ObterTotais = PropertyEntity({ eventClick: carregarTotaisIntegracao, type: types.event, text: Localization.Resources.Gerais.Geral.ObterTotais, idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadGridTabelaFreteClienteIntegracoes() {
    var linhasPorPaginas = 5;

    var opcaoHistorico = { descricao: Localization.Resources.Gerais.Geral.HistoricoIntegracao, id: guid(), metodo: exibirHistoricoIntegracoesClick, icone: "" };
    var opcaoReenviar = { descricao: Localization.Resources.Gerais.Geral.Reenviar, id: guid(), metodo: reenviarIntegracoesClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [opcaoHistorico, opcaoReenviar] };

    _gridTabelaFreteClienteIntegracoes = new GridView(_pesquisaTabelaFreteClienteIntegracoes.Pesquisar.idGrid, "TabelaFreteClienteIntegracao/PesquisaTabelaFreteClienteIntegracoes", _pesquisaTabelaFreteClienteIntegracoes, menuOpcoes, null, linhasPorPaginas);
    _gridTabelaFreteClienteIntegracoes.CarregarGrid();
}

function loadTabelaFreteClienteIntegracoes() {
    $.get("Content/Static/Integracao/Integracao.html?dyn=" + guid(), function (html) {
        $("#tabIntegracoesConteudo").append(html);

        LocalizeCurrentPage();

        _tabelaFreteClienteIntegracao = new TabelaFreteClienteIntegracao();
        KoBindings(_tabelaFreteClienteIntegracao, "knockoutDadosIntegracao");

        _pesquisaTabelaFreteClienteIntegracoes = new PesquisaTabelaFreteClienteIntegracoes();
        KoBindings(_pesquisaTabelaFreteClienteIntegracoes, "knockoutPesquisaIntegracao", false, _pesquisaTabelaFreteClienteIntegracoes.Pesquisar.id);

        _pesquisaHistoricoIntegracaoTabelaFrete = new PesquisaHistoricoIntegracaoTabelaFrete();
        KoBindings(_pesquisaHistoricoIntegracaoTabelaFrete, "knockouHistorioIntegracaoTabelaFrete");

        loadGridTabelaFreteClienteIntegracoes();
    });
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function DownloadArquivosHistoricoIntegracaoClick(historicoConsulta) {
    executarDownload("TabelaFreteClienteIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function exibirHistoricoIntegracoesClick(integracao) {
    BuscarHistoricoIntegracaoTabelaFreteIntegracao(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoTabelaFreteCliente");
}

function reenviarIntegracoesClick(integracao) {
    executarReST("TabelaFreteClienteIntegracao/Reenviar", { Codigo: integracao.Codigo }, (arg) => {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ReenvioSolicitadoComSucesso);
            carregarIntegracoes();
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    })
}

/*
 * Declaração das Funções Públicas
 */

function recarregarTabelaFreteClienteIntegracoes() {
    if (_pesquisaTabelaFreteClienteIntegracoes != null) {
        _pesquisaTabelaFreteClienteIntegracoes.Codigo.val(_tabelaFreteCliente.Codigo.val());

        controlarExibicaoAbaIntegracoes(false);
        carregarIntegracoes();
    }
}

/*
 * Declaração das Funções
 */

function BuscarHistoricoIntegracaoTabelaFreteIntegracao(integracao) {
    _pesquisaHistoricoIntegracaoTabelaFrete.Codigo.val(integracao.Codigo);
    var download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoTabelaFreteIntegracao = new GridView(_pesquisaHistoricoIntegracaoTabelaFrete.Historico.idGrid, "TabelaFreteClienteIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoTabelaFrete, menuOpcoes);
    _gridHistoricoIntegracaoTabelaFreteIntegracao.CarregarGrid();
}

function carregarIntegracoes() {
    _gridTabelaFreteClienteIntegracoes.CarregarGrid();

    carregarTotaisIntegracao();
}

function carregarTotaisIntegracao() {
    executarReST("TabelaFreteClienteIntegracao/ObterTotaisIntegracoes", { Codigo: _pesquisaTabelaFreteClienteIntegracoes.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            _tabelaFreteClienteIntegracao.TotalGeral.val(retorno.Data.TotalGeral);
            _tabelaFreteClienteIntegracao.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
            _tabelaFreteClienteIntegracao.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
            _tabelaFreteClienteIntegracao.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
            _tabelaFreteClienteIntegracao.TotalIntegrado.val(retorno.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function controlarExibicaoAbaIntegracoes(IntegracaoOpentech) {
    if (IntegracaoOpentech)
        $("#liTabIntegracoes").show();

    if (_pesquisaTabelaFreteClienteIntegracoes.Codigo.val() > 0 && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
        $("#liTabIntegracoes").show();
    else
        $("#liTabIntegracoes").hide();
}

function fecharModalProblemaIntegracao() {
    Global.fecharModal("divModalMotivoProblemaIntegracao");
}
