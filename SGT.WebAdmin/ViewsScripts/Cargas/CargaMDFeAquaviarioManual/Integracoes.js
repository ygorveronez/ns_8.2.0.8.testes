/// <reference path="../../Enumeradores/EnumSituacaoIntegracao.js"/>
/// <reference path="CargaMDFeAquaviarioManual.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridIntegracoesMDFeAquaviarioManual;
var _MDFeAquaviarioManualIntegracao;
var _pesquisaMDFeAquaviarioManualIntegracoes;
var _problemaMDFeAquaviarioManualIntegracao;
var _pesquisaHistoricoIntegracao;
var _gridHistoricoIntegracao;

/*
 * Declaração das Classes
 */
var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function PesquisaMDFeAquaviarioManualIntegracoes() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracao.obterOpcoesPesquisa(), text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), def: "", issue: 272 });

    this.Pesquisar = PropertyEntity({ eventClick: carregarIntegracoesMDFeManual, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true) });
    this.ReenviarTodos = PropertyEntity({ eventClick: reenviarTodasIntegracoesClick, type: types.event, text: Localization.Resources.Gerais.Geral.ReenviarTodas, visible: ko.observable(false) });
};

function ProblemaMDFeAquaviarioManualIntegracao() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ type: types.map, val: ko.observable(""), text: Localization.Resources.Gerais.Geral.Motivo.getRequiredFieldDescription(), maxlength: 300, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarProblemaIntegracaoClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar) });
};

function MDFeAquaviarioManualIntegracao() {
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

function loadGridMDFeAquaviarioManualIntegracoes() {
    var linhasPorPaginas = 5;
    var opcaoIntegrar = { descricao: Localization.Resources.Gerais.Geral.Integrar, id: guid(), metodo: integrarClick, icone: "" };
    //var opcaoProblemaIntegracao = { descricao: "Problema", id: guid(), metodo: exibirModalProblemaIntegracaoClick, icone: "" };
    var historico = { descricao: Localization.Resources.Gerais.Geral.HistoricoIntegracao, id: guid(), metodo: exibirHistoricoIntegracoesClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [opcaoIntegrar, historico] }; //,opcaoProblemaIntegracao

    _gridIntegracoesMDFeAquaviarioManual = new GridView(_pesquisaMDFeAquaviarioManualIntegracoes.Pesquisar.idGrid, "CargaMDFeAquaviarioManualIntegracao/PesquisaMDFeManualIntegracoes", _pesquisaMDFeAquaviarioManualIntegracoes, menuOpcoes, null, linhasPorPaginas);
    _gridIntegracoesMDFeAquaviarioManual.CarregarGrid();
}

function loadMDFeAquaviarioManualIntegracoes() {
    $.get("Content/Static/Integracao/Integracao.html?dyn=" + guid(), function (html) {
        $("#tabIntegracoesConteudo").append(html);

        LocalizeCurrentPage();

        _MDFeAquaviarioManualIntegracao = new MDFeAquaviarioManualIntegracao();
        KoBindings(_MDFeAquaviarioManualIntegracao, "knockoutDadosIntegracao");

        _pesquisaMDFeAquaviarioManualIntegracoes = new PesquisaMDFeAquaviarioManualIntegracoes();
        KoBindings(_pesquisaMDFeAquaviarioManualIntegracoes, "knockoutPesquisaIntegracao", false, _pesquisaMDFeAquaviarioManualIntegracoes.Pesquisar.id);

        _problemaMDFeAquaviarioManualIntegracao = new ProblemaMDFeAquaviarioManualIntegracao();
        KoBindings(_problemaMDFeAquaviarioManualIntegracao, "knockoutMotivoProblemaIntegracao");

        loadGridMDFeAquaviarioManualIntegracoes();
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
    if (ValidarCamposObrigatorios(_problemaMDFeAquaviarioManualIntegracao)) {
        var dadosProblemaIntegracao = RetornarObjetoPesquisa(_problemaMDFeAquaviarioManualIntegracao);

        executarReST("CargaMDFeAquaviarioManualIntegracao/ProblemaIntegracao", dadosProblemaIntegracao, function (retorno) {
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
    _problemaMDFeAquaviarioManualIntegracao.Codigo.val(registroSelecionado.Codigo);

    Global.abrirModal('divModalMotivoProblemaIntegracao');
    $("#divModalMotivoProblemaIntegracao").one('hidden.bs.modal', function () {
        LimparCampos(_problemaMDFeAquaviarioManualIntegracao);
    });
}

function integrarClick(registroSelecionado) {
    executarReST("CargaMDFeAquaviarioManualIntegracao/Reenviar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
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
    executarReST("CargaMDFeAquaviarioManualIntegracao/ReenviarTodos", { Codigo: registroSelecionado.Codigo }, function (retorno) {
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
    executarDownload("CargaMDFeAquaviarioManualIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

/*
 * Declaração das Funções Públicas
 */

function recarregarMDFeAquaviarioManualIntegracoes() {
    if (_pesquisaMDFeAquaviarioManualIntegracoes != null) {
        _pesquisaMDFeAquaviarioManualIntegracoes.Codigo.val(_cargaMDFeAquaviario.Codigo.val());

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

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "CargaMDFeAquaviarioManualIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function carregarIntegracoesMDFeManual() {
    _gridIntegracoesMDFeAquaviarioManual.CarregarGrid();

    carregarTotaisIntegracaoMDFeManual();
}

function carregarTotaisIntegracaoMDFeManual() {
    executarReST("CargaMDFeAquaviarioManualIntegracao/ObterTotaisIntegracoes", { Codigo: _pesquisaMDFeAquaviarioManualIntegracoes.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            _MDFeAquaviarioManualIntegracao.TotalGeral.val(retorno.Data.TotalGeral);
            _MDFeAquaviarioManualIntegracao.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
            _MDFeAquaviarioManualIntegracao.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
            _MDFeAquaviarioManualIntegracao.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
            _MDFeAquaviarioManualIntegracao.TotalIntegrado.val(retorno.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function controlarExibicaoAbaIntegracoes() {
    if (_pesquisaMDFeAquaviarioManualIntegracoes.Codigo.val() > 0 && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiNFe)
        $("#liTabIntegracoes").show();
    else
        $("#liTabIntegracoes").hide();
}

function fecharModalProblemaIntegracao() {
    Global.fecharModal("divModalMotivoProblemaIntegracao");
}