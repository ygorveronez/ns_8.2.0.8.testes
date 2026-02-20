/// <reference path="../../Enumeradores/EnumSituacaoIntegracao.js"/>
/// <reference path="CargaCTeManual.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridIntegracoesCTeManual;
var _cteManualIntegracao;
var _pesquisaCTeManualIntegracoes;
var _problemaCTeManualIntegracao;
var _pesquisaHistoricoIntegracao;
var _gridHistoricoIntegracao;

/*
 * Declaração das Classes
 */
var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function PesquisaCTeManualIntegracoes() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracao.obterOpcoesPesquisa(), text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), def: "", issue: 272 });

    this.Pesquisar = PropertyEntity({ eventClick: carregarIntegracoes, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true) });
    this.ReenviarTodos = PropertyEntity({ eventClick: reenviarTodasIntegracoesClick, type: types.event, text: Localization.Resources.Gerais.Geral.ReenviarTodas, visible: ko.observable(false) });
};

function ProblemaCTeManualIntegracao() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ type: types.map, val: ko.observable(""), text: Localization.Resources.Gerais.Geral.Motivo.getRequiredFieldDescription(), maxlength: 300, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarProblemaIntegracaoClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar) });
};

function CTeManualIntegracao() {
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

function loadGridCTeManualIntegracoes() {
    var linhasPorPaginas = 5;
    var opcaoIntegrar = { descricao: Localization.Resources.Gerais.Geral.Integrar, id: guid(), metodo: integrarClick, icone: "" };
    //var opcaoProblemaIntegracao = { descricao: "Problema", id: guid(), metodo: exibirModalProblemaIntegracaoClick, icone: "" };
    var historico = { descricao: Localization.Resources.Gerais.Geral.HistoricoIntegracao, id: guid(), metodo: exibirHistoricoIntegracoesClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [opcaoIntegrar, historico] }; //,opcaoProblemaIntegracao

    _gridIntegracoesCTeManual = new GridView(_pesquisaCTeManualIntegracoes.Pesquisar.idGrid, "CargaCTeManualIntegracao/PesquisaCargaCTeManualIntegracoes", _pesquisaCTeManualIntegracoes, menuOpcoes, null, linhasPorPaginas);
    _gridIntegracoesCTeManual.CarregarGrid();
}

function loadCTeManualIntegracoes() {
    $.get("Content/Static/Integracao/Integracao.html?dyn=" + guid(), function (html) {
        $("#tabIntegracoesConteudo").append(html);

        LocalizeCurrentPage();

        _cteManualIntegracao = new CTeManualIntegracao();
        KoBindings(_cteManualIntegracao, "knockoutDadosIntegracao");

        _pesquisaCTeManualIntegracoes = new PesquisaCTeManualIntegracoes();
        KoBindings(_pesquisaCTeManualIntegracoes, "knockoutPesquisaIntegracao", false, _pesquisaCTeManualIntegracoes.Pesquisar.id);

        _problemaCTeManualIntegracao = new ProblemaCTeManualIntegracao();
        KoBindings(_problemaCTeManualIntegracao, "knockoutMotivoProblemaIntegracao");

        loadGridCTeManualIntegracoes();
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
    if (ValidarCamposObrigatorios(_problemaCTeManualIntegracao)) {
        var dadosProblemaIntegracao = RetornarObjetoPesquisa(_problemaCTeManualIntegracao);

        executarReST("CargaCTeManualIntegracao/ProblemaIntegracao", dadosProblemaIntegracao, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    fecharModalProblemaIntegracao();
                    carregarIntegracoes();
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
    _problemaCTeManualIntegracao.Codigo.val(registroSelecionado.Codigo);

    Global.abrirModal('divModalMotivoProblemaIntegracao');
    $("#divModalMotivoProblemaIntegracao").one('hidden.bs.modal', function () {
        LimparCampos(_problemaCTeManualIntegracao);
    });
}

function integrarClick(registroSelecionado) {
    executarReST("CargaCTeManualIntegracao/Reenviar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                carregarIntegracoes();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function reenviarTodasIntegracoesClick() {
    executarReST("CargaCTeManualIntegracao/ReenviarTodos", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                carregarIntegracoes();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function DownloadArquivosHistoricoIntegracaoClick(historicoConsulta) {
    executarDownload("CargaCTeManualIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

/*
 * Declaração das Funções Públicas
 */

function recarregarCTeManualIntegracoes() {
    if (_pesquisaCTeManualIntegracoes != null) {
        _pesquisaCTeManualIntegracoes.Codigo.val(_carga.Codigo.val());

        carregarIntegracoes();
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

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "CargaCTeManualIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function carregarIntegracoes() {
    _gridIntegracoesCTeManual.CarregarGrid();

    carregarTotaisIntegracao();
}

function carregarTotaisIntegracao() {
    executarReST("CargaCTeManualIntegracao/ObterTotaisIntegracoes", { Codigo: _pesquisaCTeManualIntegracoes.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            _cteManualIntegracao.TotalGeral.val(retorno.Data.TotalGeral);
            _cteManualIntegracao.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
            _cteManualIntegracao.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
            _cteManualIntegracao.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
            _cteManualIntegracao.TotalIntegrado.val(retorno.Data.TotalIntegrado);

            controlarExibicaoAbaIntegracoes();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function controlarExibicaoAbaIntegracoes() {
    if (_cteManualIntegracao.TotalGeral.val() > 0)
        $("#liTabIntegracoes").show();
    else
        $("#liTabIntegracoes").hide();
}

function fecharModalProblemaIntegracao() {
    Global.fecharModal('divModalMotivoProblemaIntegracao');
}