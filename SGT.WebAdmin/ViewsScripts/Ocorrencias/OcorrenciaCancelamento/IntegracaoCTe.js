/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridHistoricoIntegracaoCTeOcorrenciaCancelamento;
var _gridIntegracaoCTeOcorrenciaCancelamento;
var _integracaoCTeOcorrenciaCancelamento;
var _pesquisaHistoricoIntegracaoCTeOcorrenciaCancelamento;

/*
 * Declaração das Classes
 */

var IntegracaoCTeOcorrenciaCancelamento = function () {
    this.OcorrenciaCancelamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.PossuiIntegracao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoIntegracaoCarga.Todas), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisaSemAguardandoRetorno(), text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Situacao.getFieldDescription(), def: EnumSituacaoIntegracaoCarga.Todas });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.TotalGeral.getFieldDescription() });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.AguardandoIntegracao.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.ProblemasIntegracao.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Integrados.getFieldDescription() });

    this.ObterTotais = PropertyEntity({ eventClick: obterTotaisIntegracaoCTeOcorrenciaCancelamentoClick, type: types.event, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.ObterTotais, idGrid: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            recarregarGridIntegracaoCTeOcorrenciaCancelamento();
            obterTotaisIntegracaoCTeOcorrenciaCancelamento();
        }, type: types.event, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ReenviarTodos = PropertyEntity({ eventClick: reenviarTodasIntegracoesOcorrenciaCancelamentoClick, type: types.event, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.ReenviarTodos, idGrid: guid(), visible: ko.observable(true) });
}

var PesquisaHistoricoIntegracaoCTeOcorrenciaCancelamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridHistoricoIntegracaoCTeOcorrenciaCancelamento() {
    var opcaoDownload = { descricao: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.DownloadArquivos, id: guid(), evento: "onclick", metodo: downloadArquivosHistoricoIntegracaoCTeOcorrenciaCancelamentoClick, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDownload] };

    _gridHistoricoIntegracaoCTeOcorrenciaCancelamento = new GridView("grid-historico-integracao-ocorrencia-cancelamento", "OcorrenciaCancelamentoIntegracaoCTe/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoCTeOcorrenciaCancelamento, menuOpcoes, { column: 1, dir: orderDir.desc });
}

function loadGridIntegracaoCTeOcorrenciaCancelamento() {
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Opcoes, tamanho: 7, opcoes: [] };

    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Reenviar, id: guid(), metodo: reenviarIntegracaoCTeOcorrenciaCancelamentoClick, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.HistoricoIntegracao, id: guid(), metodo: exibirHistoricoIntegracaoCTeOcorrenciaCancelamentoClick, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Auditoria, id: guid(), metodo: OpcaoAuditoria("OcorrenciaCancelamentoIntegracaoCTe"), tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoAuditoria });

    _gridIntegracaoCTeOcorrenciaCancelamento = new GridView(_integracaoCTeOcorrenciaCancelamento.Pesquisar.idGrid, "OcorrenciaCancelamentoIntegracaoCTe/Pesquisa", _integracaoCTeOcorrenciaCancelamento, menuOpcoes);
    _gridIntegracaoCTeOcorrenciaCancelamento.CarregarGrid();
}

function loadIntegracaoCTe() {
    _integracaoCTeOcorrenciaCancelamento = new IntegracaoCTeOcorrenciaCancelamento();
    KoBindings(_integracaoCTeOcorrenciaCancelamento, "knockoutIntegracaoCTe");

    _pesquisaHistoricoIntegracaoCTeOcorrenciaCancelamento = new PesquisaHistoricoIntegracaoCTeOcorrenciaCancelamento();

    loadGridIntegracaoCTeOcorrenciaCancelamento();
    loadGridHistoricoIntegracaoCTeOcorrenciaCancelamento();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function downloadArquivosHistoricoIntegracaoCTeOcorrenciaCancelamentoClick(registroSelecionado) {
    executarDownload("OcorrenciaCancelamentoIntegracaoCTe/DownloadArquivosHistoricoIntegracao", { Codigo: registroSelecionado.Codigo });
}

function exibirHistoricoIntegracaoCTeOcorrenciaCancelamentoClick(registroSelecionado) {
    _pesquisaHistoricoIntegracaoCTeOcorrenciaCancelamento.Codigo.val(registroSelecionado.Codigo);
    recarregarGridHistoricoIntegracaoCTeOcorrenciaCancelamento();

    Global.abrirModal("divModalHistoricoIntegracaoCTeOcorrenciaCancelamento");
}

function obterTotaisIntegracaoCTeOcorrenciaCancelamentoClick() {
    obterTotaisIntegracaoCTeOcorrenciaCancelamento();
}

function reenviarIntegracaoCTeOcorrenciaCancelamentoClick(registroSelecionado) {
    executarReST("OcorrenciaCancelamentoIntegracaoCTe/Reenviar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Sucesso, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.ReenvioSolicitadoComSucesso);
            recarregarGridIntegracaoCTeOcorrenciaCancelamento();
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Falha, retorno.Msg);
    });
}

function reenviarTodasIntegracoesOcorrenciaCancelamentoClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar todas as integrações?", function () {
        executarReST("OcorrenciaCancelamentoIntegracaoCTe/ReenviarTodos", RetornarObjetoPesquisa(_integracaoCTeOcorrenciaCancelamento), function (retorno) {
            if (retorno.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Sucesso, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.ReenvioSolicitadoComSucesso);
                recarregarGridIntegracaoCTeOcorrenciaCancelamento();
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Falha, retorno.Msg);
        });
    });
}

/*
 * Declaração das Funções Públicas
 */

function limparIntegracaoCTeOcorrenciaCancelamento() {
    LimparCampos(_integracaoCTeOcorrenciaCancelamento);

    recarregarGridIntegracaoCTeOcorrenciaCancelamento();
}

function preencherIntegracaoCTeOcorrenciaCancelamento(dadosOcorrenciaCancelamento) {
    _integracaoCTeOcorrenciaCancelamento.OcorrenciaCancelamento.val(dadosOcorrenciaCancelamento.Codigo);
    _integracaoCTeOcorrenciaCancelamento.PossuiIntegracao.val(dadosOcorrenciaCancelamento.PossuiIntegracaoCTe);

    recarregarGridIntegracaoCTeOcorrenciaCancelamento();
    obterTotaisIntegracaoCTeOcorrenciaCancelamento();
}

/*
 * Declaração das Funções Privadas
 */

function obterTotaisIntegracaoCTeOcorrenciaCancelamento() {
    executarReST("OcorrenciaCancelamentoIntegracaoCTe/ObterTotais", { OcorrenciaCancelamento: _integracaoCTeOcorrenciaCancelamento.OcorrenciaCancelamento.val() }, function (retorno) {
        if (retorno.Success)
            PreencherObjetoKnout(_integracaoCTeOcorrenciaCancelamento, retorno);
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Falha, retorno.Msg);
    });
}

function recarregarGridHistoricoIntegracaoCTeOcorrenciaCancelamento() {
    _gridHistoricoIntegracaoCTeOcorrenciaCancelamento.CarregarGrid();
}

function recarregarGridIntegracaoCTeOcorrenciaCancelamento() {
    _gridIntegracaoCTeOcorrenciaCancelamento.CarregarGrid();
}
