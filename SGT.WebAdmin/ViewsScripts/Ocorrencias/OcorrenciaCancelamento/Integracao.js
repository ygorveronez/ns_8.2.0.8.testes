/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridHistoricoIntegracaoOcorrenciaCancelamento;
var _gridIntegracaoOcorrenciaCancelamento;
var _integracaoOcorrenciaCancelamento;
var _pesquisaHistoricoIntegracaoOcorrenciaCancelamento;

/*
 * Declaração das Classes
 */

var IntegracaoOcorrenciaCancelamento = function () {
    this.OcorrenciaCancelamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.PossuiIntegracao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoIntegracaoCarga.Todas), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisaSemAguardandoRetorno(), text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Situacao.getFieldDescription(), def: EnumSituacaoIntegracaoCarga.Todas });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.TotalGeral.getFieldDescription() });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.AguardandoIntegracao.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.ProblemasIntegracao.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Integrados.getFieldDescription() });

    this.ObterTotais = PropertyEntity({ eventClick: obterTotaisIntegracaoOcorrenciaCancelamentoClick, type: types.event, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.ObterTotais, idGrid: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            recarregarGridIntegracaoOcorrenciaCancelamento();
            obterTotaisIntegracaoOcorrenciaCancelamento();
        }, type: types.event, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ReenviarTodos = PropertyEntity({ eventClick: reenviarTodasIntegracoesOcorrenciaCancelamentoClick, type: types.event, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.ReenviarTodos, idGrid: guid(), visible: ko.observable(true) });
}

var PesquisaHistoricoIntegracaoOcorrenciaCancelamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridHistoricoIntegracaoOcorrenciaCancelamento() {
    var opcaoDownload = { descricao: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.DownloadArquivos, id: guid(), evento: "onclick", metodo: downloadArquivosHistoricoIntegracaoOcorrenciaCancelamentoClick, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDownload] };

    _gridHistoricoIntegracaoOcorrenciaCancelamento = new GridView("grid-historico-integracao-ocorrencia-cancelamento", "OcorrenciaCancelamentoIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoOcorrenciaCancelamento, menuOpcoes, { column: 1, dir: orderDir.desc });
}

function loadGridIntegracaoOcorrenciaCancelamento() {
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Opcoes, tamanho: 7, opcoes: [] };

    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Reenviar, id: guid(), metodo: reenviarIntegracaoOcorrenciaCancelamentoClick, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.HistoricoIntegracao, id: guid(), metodo: exibirHistoricoIntegracaoOcorrenciaCancelamentoClick, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Auditoria, id: guid(), metodo: OpcaoAuditoria("OcorrenciaCancelamentoIntegracao"), tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoAuditoria });

    _gridIntegracaoOcorrenciaCancelamento = new GridView(_integracaoOcorrenciaCancelamento.Pesquisar.idGrid, "OcorrenciaCancelamentoIntegracao/Pesquisa", _integracaoOcorrenciaCancelamento, menuOpcoes);
    _gridIntegracaoOcorrenciaCancelamento.CarregarGrid();
}

function loadIntegracao() {
    _integracaoOcorrenciaCancelamento = new IntegracaoOcorrenciaCancelamento();
    KoBindings(_integracaoOcorrenciaCancelamento, "knockoutIntegracao");

    _pesquisaHistoricoIntegracaoOcorrenciaCancelamento = new PesquisaHistoricoIntegracaoOcorrenciaCancelamento();

    loadGridIntegracaoOcorrenciaCancelamento();
    loadGridHistoricoIntegracaoOcorrenciaCancelamento();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function downloadArquivosHistoricoIntegracaoOcorrenciaCancelamentoClick(registroSelecionado) {
    executarDownload("OcorrenciaCancelamentoIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: registroSelecionado.Codigo });
}

function exibirHistoricoIntegracaoOcorrenciaCancelamentoClick(registroSelecionado) {
    _pesquisaHistoricoIntegracaoOcorrenciaCancelamento.Codigo.val(registroSelecionado.Codigo);
    recarregarGridHistoricoIntegracaoOcorrenciaCancelamento();

    Global.abrirModal("divModalHistoricoIntegracaoOcorrenciaCancelamento");
}

function obterTotaisIntegracaoOcorrenciaCancelamentoClick() {
    obterTotaisIntegracaoOcorrenciaCancelamento();
}

function reenviarIntegracaoOcorrenciaCancelamentoClick(registroSelecionado) {
    executarReST("OcorrenciaCancelamentoIntegracao/Reenviar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Sucesso, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.ReenvioSolicitadoComSucesso);
            recarregarGridIntegracaoOcorrenciaCancelamento();
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Falha, retorno.Msg);
    });
}

function reenviarTodasIntegracoesOcorrenciaCancelamentoClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar todas as integrações?", function () {
        executarReST("OcorrenciaCancelamentoIntegracao/ReenviarTodos", RetornarObjetoPesquisa(_integracaoOcorrenciaCancelamento), function (retorno) {
            if (retorno.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Sucesso, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.ReenvioSolicitadoComSucesso);
                recarregarGridIntegracaoOcorrenciaCancelamento();
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Falha, retorno.Msg);
        });
    });
}

/*
 * Declaração das Funções Públicas
 */

function limparIntegracaoOcorrenciaCancelamento() {
    LimparCampos(_integracaoOcorrenciaCancelamento);

    recarregarGridIntegracaoOcorrenciaCancelamento();
}

function preencherIntegracaoOcorrenciaCancelamento(dadosOcorrenciaCancelamento) {
    _integracaoOcorrenciaCancelamento.OcorrenciaCancelamento.val(dadosOcorrenciaCancelamento.Codigo);
    _integracaoOcorrenciaCancelamento.PossuiIntegracao.val(dadosOcorrenciaCancelamento.PossuiIntegracao);

    recarregarGridIntegracaoOcorrenciaCancelamento();
    obterTotaisIntegracaoOcorrenciaCancelamento();
}

/*
 * Declaração das Funções Privadas
 */

function obterTotaisIntegracaoOcorrenciaCancelamento() {
    executarReST("OcorrenciaCancelamentoIntegracao/ObterTotais", { OcorrenciaCancelamento: _integracaoOcorrenciaCancelamento.OcorrenciaCancelamento.val() }, function (retorno) {
        if (retorno.Success)
            PreencherObjetoKnout(_integracaoOcorrenciaCancelamento, retorno);
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Falha, retorno.Msg);
    });
}

function recarregarGridHistoricoIntegracaoOcorrenciaCancelamento() {
    _gridHistoricoIntegracaoOcorrenciaCancelamento.CarregarGrid();
}

function recarregarGridIntegracaoOcorrenciaCancelamento() {
    _gridIntegracaoOcorrenciaCancelamento.CarregarGrid();
}
