/// <reference path="../../Enumeradores/EnumSituacaoIntegracao.js"/>
/// <reference path="CargaMDFeManualCancelamento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridIntegracoesMDFeManualCancelamento;
var _MDFeManualCancelamentoIntegracao;
var _pesquisaMDFeManualCancelamentoIntegracoes;
var _problemaMDFeManualCancelamentoIntegracao;
var _pesquisaHistoricoIntegracao;
var _gridHistoricoIntegracao;

/*
 * Declaração das Classes
 */
var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function PesquisaMDFeManualCancelamentoIntegracoes() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracao.obterOpcoesPesquisa(), text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), def: "", issue: 272 });

    this.Pesquisar = PropertyEntity({ eventClick: carregarIntegracoesMDFeManual, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true) });
    this.ReenviarTodos = PropertyEntity({ eventClick: reenviarTodasIntegracoesClick, type: types.event, text: Localization.Resources.Gerais.Geral.ReenviarTodas, visible: ko.observable(false) });
    this.FinalizarEtapa = PropertyEntity({
        eventClick: function (e) {
            FinalizarEtapaIntegracao();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FinalizarEtapa, idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true)
    });
};

function ProblemaMDFeManualCancelamentoIntegracao() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ type: types.map, val: ko.observable(""), text: Localization.Resources.Gerais.Geral.Motivo.getRequiredFieldDescription(), maxlength: 300, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarProblemaIntegracaoClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar) });
};

function MDFeManualCancelamentoIntegracao() {
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoIntegracao.getFieldDescription() });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoRetorno.getFieldDescription() });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.TotalGeral.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Integrados.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.ProblemasIntegracao.getFieldDescription() });

    this.ObterTotais = PropertyEntity({ eventClick: carregarTotaisIntegracaoMDFeManual, type: types.event, text: Localization.Resources.Gerais.Geral.ObterTotais, idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadGridMDFeManualCancelamentoIntegracoes() {
    var linhasPorPaginas = 5;
    var opcaoIntegrar = { descricao: Localization.Resources.Gerais.Geral.Reenviar, id: guid(), metodo: integrarClick, icone: "" };
    //var opcaoProblemaIntegracao = { descricao: "Problema", id: guid(), metodo: exibirModalProblemaIntegracaoClick, icone: "" };
    var historico = { descricao: Localization.Resources.Gerais.Geral.HistoricoIntegracao, id: guid(), metodo: exibirHistoricoIntegracoesClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [opcaoIntegrar, historico] }; //,opcaoProblemaIntegracao

    _gridIntegracoesMDFeManualCancelamento = new GridView(_pesquisaMDFeManualCancelamentoIntegracoes.Pesquisar.idGrid, "CargaMDFeManualCancelamentoIntegracao/PesquisaMDFeManualIntegracoes", _pesquisaMDFeManualCancelamentoIntegracoes, menuOpcoes, null, linhasPorPaginas);
    _gridIntegracoesMDFeManualCancelamento.CarregarGrid();
}

function loadMDFeManualCancelamentoIntegracoes() {
    $.get("Content/Static/Carga/CargaMDFeManualCancelamentoIntegracoes.html?dyn=" + guid(), function (html) {
        $("#tabIntegracoesConteudo").append(html);

        LocalizeCurrentPage();

        _MDFeManualCancelamentoIntegracao = new MDFeManualCancelamentoIntegracao();
        KoBindings(_MDFeManualCancelamentoIntegracao, "knockoutDadosIntegracao");

        _pesquisaMDFeManualCancelamentoIntegracoes = new PesquisaMDFeManualCancelamentoIntegracoes();
        KoBindings(_pesquisaMDFeManualCancelamentoIntegracoes, "knockoutPesquisaIntegracao", false, _pesquisaMDFeManualCancelamentoIntegracoes.Pesquisar.id);

        _problemaMDFeManualCancelamentoIntegracao = new ProblemaMDFeManualCancelamentoIntegracao();
        KoBindings(_problemaMDFeManualCancelamentoIntegracao, "knockoutMotivoProblemaIntegracao");

        loadGridMDFeManualCancelamentoIntegracoes();

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
    if (ValidarCamposObrigatorios(_problemaMDFeManualCancelamentoIntegracao)) {
        var dadosProblemaIntegracao = RetornarObjetoPesquisa(_problemaMDFeManualCancelamentoIntegracao);

        executarReST("CargaMDFeManualCancelamentoIntegracao/ProblemaIntegracao", dadosProblemaIntegracao, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    fecharModalProblemaIntegracao();
                    carregarIntegracoesMDFeManual();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        });
    }
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);
}

function exibirModalProblemaIntegracaoClick(registroSelecionado) {
    _problemaMDFeManualCancelamentoIntegracao.Codigo.val(registroSelecionado.Codigo);

    Global.abrirModal('divModalMotivoProblemaIntegracao');
    $("#divModalMotivoProblemaIntegracao").one('hidden.bs.modal', function () {
        LimparCampos(_problemaMDFeManualCancelamentoIntegracao);
    });
}

function integrarClick(registroSelecionado) {
    executarReST("CargaMDFeManualCancelamentoIntegracao/Reenviar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                carregarIntegracoesMDFeManual();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function reenviarTodasIntegracoesClick() {
    executarReST("CargaMDFeManualCancelamentoIntegracao/ReenviarTodos", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                carregarIntegracoesMDFeManual();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function DownloadArquivosHistoricoIntegracaoClick(historicoConsulta) {
    executarDownload("CargaMDFeManualCancelamentoIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

/*
 * Declaração das Funções Públicas
 */

function recarregarMDFeManualCancelamentoIntegracoes() {
    if (_pesquisaMDFeManualCancelamentoIntegracoes != null) {
        _pesquisaMDFeManualCancelamentoIntegracoes.Codigo.val(_cancelamento.Codigo.val());

        controlarExibicaoAbaIntegracoes();
        carregarIntegracoesMDFeManual();
    }
}

/*
 * Declaração das Funções
 */

function BuscarHistoricoIntegracao(integracao) {
    _pesquisaHistoricoIntegracao = new PesquisaHistoricoIntegracao();
    _pesquisaHistoricoIntegracao.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "CargaMDFeManualCancelamentoIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function carregarIntegracoesMDFeManual() {
    if (_pesquisaMDFeManualCancelamentoIntegracoes != null)
        _pesquisaMDFeManualCancelamentoIntegracoes.Codigo.val(_cancelamento.Codigo.val());

    _gridIntegracoesMDFeManualCancelamento.CarregarGrid();

    carregarTotaisIntegracaoMDFeManual();

    if (_cancelamento.Situacao.val() == EnumSituacaoMDFeManualCancelamento.FalhaIntegracao)
        _pesquisaMDFeManualCancelamentoIntegracoes.FinalizarEtapa.visible(true);
}

function carregarTotaisIntegracaoMDFeManual() {
    if (_pesquisaMDFeManualCancelamentoIntegracoes != null)
        _pesquisaMDFeManualCancelamentoIntegracoes.Codigo.val(_cancelamento.Codigo.val());

    executarReST("CargaMDFeManualCancelamentoIntegracao/ObterTotaisIntegracoes", { Codigo: _pesquisaMDFeManualCancelamentoIntegracoes.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            _MDFeManualCancelamentoIntegracao.TotalGeral.val(retorno.Data.TotalGeral);
            _MDFeManualCancelamentoIntegracao.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
            _MDFeManualCancelamentoIntegracao.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
            _MDFeManualCancelamentoIntegracao.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
            _MDFeManualCancelamentoIntegracao.TotalIntegrado.val(retorno.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function controlarExibicaoAbaIntegracoes() {
    if (_pesquisaMDFeManualCancelamentoIntegracoes.Codigo.val() > 0 && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiNFe)
        $("#liTabIntegracoes").show();
    else
        $("#liTabIntegracoes").hide();
}

function fecharModalProblemaIntegracao() {
    Global.fecharModal("divModalMotivoProblemaIntegracao");
}


function FinalizarEtapaIntegracao() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, "Finalizar sem concluir integrações", function () {
        executarReST("CargaMDFeManualCancelamentoIntegracao/Finalizar", { CargaMDFeManualCancelamento: _cancelamento.Codigo.val() }, function (r) {
            if (r.Data != null) {
                if (r.Success) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SolicitacaoRealizadaComSucesso);
                    carregarIntegracoesMDFeManual();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}