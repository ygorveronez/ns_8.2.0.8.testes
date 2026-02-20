/// <reference path="../../Enumeradores/EnumSituacaoIntegracao.js"/>
/// <reference path="ContratoFreteTransportador.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridIntegracoesContratoFreteTranportador;
var _contratoFreteTransportadorIntegracao;
var _pesquisaFreteTransportadorIntegracao;
var _problemaContratoFreteTransportadorIntegracao;
var _pesquisaHistoricoIntegracao;
var _gridHistoricoIntegracao;

/*
 * Declaração das Classes
 */
var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function PesquisaContratoFreteTransportadorIntegracao() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracao.obterOpcoesPesquisa(), text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), def: "", issue: 272 });

    this.Pesquisar = PropertyEntity({ eventClick: carregarIntegracoesContratoFreteTransportador, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true) });
    this.ReenviarTodos = PropertyEntity({ eventClick: reenviarTodasIntegracoesClick, type: types.event, text: Localization.Resources.Gerais.Geral.ReenviarTodas, visible: ko.observable(false) });
};

function ProblemaContratoFreteTransportadorIntegracao() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ type: types.map, val: ko.observable(""), text: Localization.Resources.Gerais.Geral.Motivo.getRequiredFieldDescription(), maxlength: 300, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarProblemaIntegracaoClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar) });
};

function ContratoFreteTransportadorIntegracao() {
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoIntegracao.getFieldDescription() });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoRetorno.getFieldDescription() });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.TotalGeral.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Integrados.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.ProblemasIntegracao.getFieldDescription() });

    this.ObterTotais = PropertyEntity({ eventClick: carregarTotaisIntegracaoContratoFreteTransportador, type: types.event, text: Localization.Resources.Gerais.Geral.ObterTotais, idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function BuscarIntegracoesContratoFreteTransportador() {
    var linhasPorPaginas = 5;
    var opcaoIntegrar = { descricao: Localization.Resources.Gerais.Geral.Integrar, id: guid(), metodo: integrarClick, icone: "" };
    //var opcaoProblemaIntegracao = { descricao: "Problema", id: guid(), metodo: exibirModalProblemaIntegracaoClick, icone: "" };
    var historico = { descricao: Localization.Resources.Gerais.Geral.HistoricoIntegracao, id: guid(), metodo: exibirHistoricoIntegracoesClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [opcaoIntegrar, historico] }; //,opcaoProblemaIntegracao

    _gridIntegracoesContratoFreteTranportador = new GridView(_pesquisaFreteTransportadorIntegracao.Pesquisar.idGrid, "ContratoFreteTransportadorIntegracao/PesquisaContratoFreteTransportadorIntegracoes", _pesquisaFreteTransportadorIntegracao, menuOpcoes, null, linhasPorPaginas);
    _gridIntegracoesContratoFreteTranportador.CarregarGrid();
}

function loadContratoFreteTransportadorIntegracoes() {
    $.get("Content/Static/Integracao/Integracao.html?dyn=" + guid(), function (html) {
        $("#tabIntegracoesConteudo").append(html);

        LocalizeCurrentPage();

        _contratoFreteTransportadorIntegracao = new ContratoFreteTransportadorIntegracao();
        KoBindings(_contratoFreteTransportadorIntegracao, "knockoutDadosIntegracao");

        _pesquisaFreteTransportadorIntegracao = new PesquisaContratoFreteTransportadorIntegracao();
        KoBindings(_pesquisaFreteTransportadorIntegracao, "knockoutPesquisaIntegracao", false, _pesquisaFreteTransportadorIntegracao.Pesquisar.id);

        _problemaContratoFreteTransportadorIntegracao = new ProblemaContratoFreteTransportadorIntegracao();
        KoBindings(_problemaContratoFreteTransportadorIntegracao, "knockoutMotivoProblemaIntegracao");

        BuscarIntegracoesContratoFreteTransportador();
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
    if (ValidarCamposObrigatorios(_problemaContratoFreteTransportadorIntegracao)) {
        var dadosProblemaIntegracao = RetornarObjetoPesquisa(_problemaContratoFreteTransportadorIntegracao);

        executarReST("ContratoFreteTransportadorIntegracao/ProblemaIntegracao", dadosProblemaIntegracao, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    fecharModalProblemaIntegracao();
                    carregarIntegracoesContratoFreteTransportador();
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
    _problemaContratoFreteTransportadorIntegracao.Codigo.val(registroSelecionado.Codigo);

    Global.abrirModal('divModalMotivoProblemaIntegracao');
    $("#divModalMotivoProblemaIntegracao").one('hidden.bs.modal', function () {
        LimparCampos(_problemaContratoFreteTransportadorIntegracao);
    });
}

function integrarClick(registroSelecionado) {
    executarReST("ContratoFreteTransportadorIntegracao/Reenviar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                carregarIntegracoesContratoFreteTransportador();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function reenviarTodasIntegracoesClick() {
    executarReST("ContratoFreteTransportadorIntegracao/ReenviarTodos", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                carregarIntegracoesContratoFreteTransportador();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function DownloadArquivosHistoricoIntegracaoClick(historicoConsulta) {
    executarDownload("ContratoFreteTransportadorIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

/*
 * Declaração das Funções Públicas
 */

function recarregarContratoFreteTransportadorIntegracoes() {
    if (_pesquisaFreteTransportadorIntegracao != null) {
        _pesquisaFreteTransportadorIntegracao.Codigo.val(_cargaMDFeAquaviario.Codigo.val());

        controlarExibicaoAbaIntegracoes();
        carregarIntegracoesContratoFreteTransportador();
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

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "ContratoFreteTransportadorIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function carregarIntegracoesContratoFreteTransportador() {
    _gridIntegracoesContratoFreteTranportador.CarregarGrid();

    carregarTotaisIntegracaoContratoFreteTransportador();
}

function carregarTotaisIntegracaoContratoFreteTransportador() {
    executarReST("ContratoFreteTransportadorIntegracao/ObterTotaisIntegracoes", { Codigo: _pesquisaFreteTransportadorIntegracao.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            _contratoFreteTransportadorIntegracao.TotalGeral.val(retorno.Data.TotalGeral);
            _contratoFreteTransportadorIntegracao.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
            _contratoFreteTransportadorIntegracao.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
            _contratoFreteTransportadorIntegracao.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
            _contratoFreteTransportadorIntegracao.TotalIntegrado.val(retorno.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function controlarExibicaoAbaIntegracoes() {
    if (_pesquisaFreteTransportadorIntegracao.Codigo.val() > 0)
        $("#liTabIntegracoes").show();
    else
        $("#liTabIntegracoes").hide();
}

function fecharModalProblemaIntegracao() {
    Global.fecharModal("divModalMotivoProblemaIntegracao");
}