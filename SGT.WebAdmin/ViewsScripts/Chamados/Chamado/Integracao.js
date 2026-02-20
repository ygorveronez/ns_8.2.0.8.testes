/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />
/// <reference path="Chamado.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridIntegracoes;
var _integracao;
var _pesquisaIntegracoes;
var _pesquisaHistoricoIntegracao;
var _gridHistoricoIntegracao;
var _CRUDIntegracao;
var _chamadoOcorrenciaModalHistoricoIntegracao;

/*
 * Declaração das Classes
 */

var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function Integracao() {
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Integração:" });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Retorno:" });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Geral:" });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Integrados:" });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Problemas na Integração:" });

    this.ObterTotais = PropertyEntity({ eventClick: carregarTotaisIntegracaoChamado, type: types.event, text: "Obter Totais", idGrid: guid(), visible: ko.observable(true) });
};

function PesquisaIntegracoes() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: "Situação:", def: "", issue: 272 });

    this.Pesquisar = PropertyEntity({ eventClick: carregarIntegracoesChamado, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.ReenviarTodos = PropertyEntity({ eventClick: reenviarTodasIntegracoesChamadoClick, type: types.event, text: "Reenviar Todas", visible: ko.observable(false) });
};

var CRUDIntegracao = function () {
    this.FinalizarComIntegracaoRejeitada = PropertyEntity({ eventClick: finalizarComIntegracaoRejeitadaClick, type: types.event, text: "Finalizar mesmo com Integração Rejeitada", visible: ko.observable(false) });
    this.ReenviarIntegracaoInformacoesFechamento = PropertyEntity({ eventClick: reenviarIntegracaoInformacoesFechamentoClick, type: types.event, text: "Reenviar Integrações", visible: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadGridIntegracoes() {
    var linhasPorPaginas = 5;
    var opcaoIntegrar = { descricao: "Integrar", id: guid(), metodo: integrarChamadoClick, icone: "" };
    var historico = { descricao: "Histórico de Integração", id: guid(), metodo: exibirHistoricoIntegracoesChamadoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [opcaoIntegrar, historico] };

    _gridIntegracoes = new GridView(_pesquisaIntegracoes.Pesquisar.idGrid, "ChamadoIntegracao/PesquisaIntegracoes", _pesquisaIntegracoes, menuOpcoes, null, linhasPorPaginas);
}

function loadIntegracoesChamado() {
    $.get("Content/Static/Integracao/Integracao.html?dyn=" + guid(), function (html) {
        $("#divConteudoIntegracao").append(html);

        LocalizeCurrentPage();

        _integracao = new Integracao();
        KoBindings(_integracao, "knockoutDadosIntegracao");

        _pesquisaIntegracoes = new PesquisaIntegracoes();
        KoBindings(_pesquisaIntegracoes, "knockoutPesquisaIntegracao", false, _pesquisaIntegracoes.Pesquisar.id);

        _CRUDIntegracao = new CRUDIntegracao();
        KoBindings(_CRUDIntegracao, "knockoutCRUDIntegracao");

        loadGridIntegracoes();

        _chamadoOcorrenciaModalHistoricoIntegracao = new bootstrap.Modal(document.getElementById("divModalHistoricoIntegracao"), { backdrop: 'static' });
    });
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function exibirHistoricoIntegracoesChamadoClick(integracao) {
    BuscarHistoricoIntegracaoChamado(integracao);
    _chamadoOcorrenciaModalHistoricoIntegracao.show();
}

function integrarChamadoClick(registroSelecionado) {
    executarReST("ChamadoIntegracao/Integrar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                carregarIntegracoesChamado();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
        }
    });
}

function reenviarTodasIntegracoesChamadoClick() {

}

function DownloadArquivosHistoricoIntegracaoChamadoClick(historicoConsulta) {
    executarDownload("ChamadoIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function finalizarComIntegracaoRejeitadaClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente finalizar mesmo com a integração rejeitada?", function () {
        executarReST("ChamadoIntegracao/FinalizarComIntegracaoRejeitada", { Codigo: _chamado.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Finalizado com sucesso.");
                    limparCamposChamado();
                    recarregarGridChamados();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
            }
        });
    });
}

function reenviarIntegracaoInformacoesFechamentoClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reenviar todas as integrações?", function () {
        executarReST("ChamadoIntegracao/ReenviarMultiplasIntegracoes", { Codigo: _chamado.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
                    _gridIntegracoes.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            } else
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        });
        _gridIntegracoes.CarregarGrid();
    });
}

/*
 * Declaração das Funções Públicas
 */

function recarregarIntegracoesChamado() {
    if (!_chamado.PossuiIntegracao.val())
        return;

    _pesquisaIntegracoes.Codigo.val(_chamado.Codigo.val());

    carregarIntegracoesChamado();
}

function RenderizarEtapaIntegracaoChamado() {
    _CRUDIntegracao.FinalizarComIntegracaoRejeitada.visible(false);

    if (_chamado.PossuiIntegracao.val()) {
        $("#divConteudoIntegracao").show();
        $("#divNaoPossuiIntegracao").hide();

        if (_chamado.Situacao.val() === EnumSituacaoChamado.FalhaIntegracao)
            _CRUDIntegracao.FinalizarComIntegracaoRejeitada.visible(true);
    }
    else {
        $("#divConteudoIntegracao").hide();
        $("#divNaoPossuiIntegracao").show();
    }
}

/*
 * Declaração das Funções
 */

function BuscarHistoricoIntegracaoChamado(integracao) {
    _pesquisaHistoricoIntegracao = new PesquisaHistoricoIntegracao();
    _pesquisaHistoricoIntegracao.Codigo.val(integracao.Codigo);

    var download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoChamadoClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "ChamadoIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function carregarIntegracoesChamado() {
    _gridIntegracoes.CarregarGrid();

    carregarTotaisIntegracaoChamado();
}

function carregarTotaisIntegracaoChamado() {
    executarReST("ChamadoIntegracao/ObterTotaisIntegracoes", { Codigo: _pesquisaIntegracoes.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            _integracao.TotalGeral.val(retorno.Data.TotalGeral);
            _integracao.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
            _integracao.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
            _integracao.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
            _integracao.TotalIntegrado.val(retorno.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
        }
    });
}