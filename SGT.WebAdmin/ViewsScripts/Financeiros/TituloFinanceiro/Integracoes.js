/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridTituloFinanceiroIntegracoes;
var _tituloFinanceiroIntegracao;
var _pesquisaTituloFinanceiroIntegracoes;
var _problemaTituloFinanceiroIntegracao;
var _pesquisaHistoricoIntegracao;
var _gridHistoricoIntegracao;

/*
 * Declaração das Classes
 */
var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function PesquisaTituloFinanceiroIntegracoes() {

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), def: "", issue: 272 });

    this.Pesquisar = PropertyEntity({ eventClick: carregarIntegracoes, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true) });
    this.ReenviarTodos = PropertyEntity({ eventClick: reenviarTodasIntegracoesClick, type: types.event, text: Localization.Resources.Gerais.Geral.ReenviarTodas, visible: ko.observable(false) });
};

function ProblemaTituloFinanceiroIntegracao() {

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ type: types.map, val: ko.observable(""), text: Localization.Resources.Gerais.Geral.Motivo.getRequiredFieldDescription(), maxlength: 300, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarProblemaIntegracaoClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar) });
};

function TituloFinanceiroIntegracao() {
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Integração" });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Retorno" });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Geral" });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Integrados" });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text:"Problemas Integração" });

    this.ObterTotais = PropertyEntity({ eventClick: carregarTotaisIntegracao, type: types.event, text: "ObterTotais", idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadGridTituloFinanceiroIntegracoes() {
    console.log(_pesquisaTituloFinanceiroIntegracoes);
    var linhasPorPaginas = 5;
    var opcaoIntegrar = { descricao: Localization.Resources.Gerais.Geral.Reenviar, id: guid(), metodo: reenviarClick, icone: "" };
    //var opcaoProblemaIntegracao = { descricao: "Problema", id: guid(), metodo: exibirModalProblemaIntegracaoClick, icone: "" };
    var historico = { descricao: Localization.Resources.Gerais.Geral.HistoricoIntegracao, id: guid(), metodo: exibirHistoricoIntegracoesClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [opcaoIntegrar, historico] }; //,opcaoProblemaIntegracao

    _gridTituloFinanceiroIntegracoes = new GridView(_pesquisaTituloFinanceiroIntegracoes.Pesquisar.idGrid, "TituloFinanceiroIntegracao/PesquisaTituloFinanceiroIntegracoes", _pesquisaTituloFinanceiroIntegracoes, menuOpcoes, null, linhasPorPaginas);
    _gridTituloFinanceiroIntegracoes.CarregarGrid();
}

function loadTituloFinanceiroIntegracoes() {

    $.get("Content/Static/Integracao/Integracao.html?dyn=" + guid(), function (html) {
        $("#tabIntegracoesConteudo").append(html);

        LocalizeCurrentPage();

        _tituloFinanceiroIntegracao = new TituloFinanceiroIntegracao();
        KoBindings(_tituloFinanceiroIntegracao, "knockoutDadosIntegracao");


        _pesquisaTituloFinanceiroIntegracoes = new PesquisaTituloFinanceiroIntegracoes();
        KoBindings(_pesquisaTituloFinanceiroIntegracoes, "knockoutPesquisaIntegracao", false, _pesquisaTituloFinanceiroIntegracoes.Pesquisar.id);

        loadGridTituloFinanceiroIntegracoes();
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
    if (ValidarCamposObrigatorios(_problemaTituloFinanceiroIntegracao)) {
        var dadosProblemaIntegracao = RetornarObjetoPesquisa(_problemaTituloFinanceiroIntegracao);

        executarReST("TituloFinanceiroIntegracao/ProblemaIntegracao", dadosProblemaIntegracao, function (retorno) {
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
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, "Por favor informe os campos Obrigatorios");
}

function exibirModalProblemaIntegracaoClick(registroSelecionado) {
    _problemaTituloFinanceiroIntegracao.Codigo.val(registroSelecionado.Codigo);

    Global.abrirModal("divModalMotivoProblemaIntegracao");
    $("#divModalMotivoProblemaIntegracao").one('hidden.bs.modal', function () {
        LimparCampos(_problemaTituloFinanceiroIntegracao);
    });
}

function reenviarClick(registroSelecionado) {
    executarReST("TituloFinanceiroIntegracao/Reenviar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
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

}

function DownloadArquivosHistoricoIntegracaoClick(historicoConsulta) {
    executarDownload("TituloFinanceiroIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

/*
 * Declaração das Funções Públicas
 */

function recarregarTituloFinanceiroIntegracoes() {

    if (_pesquisaTituloFinanceiroIntegracoes != null) {
        _pesquisaTituloFinanceiroIntegracoes.Codigo.val(_tituloFinanceiro.Codigo.val());

        controlarExibicaoAbaIntegracoes();
        carregarIntegracoes();
    }
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

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "TituloFinanceiroIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function carregarIntegracoes() {
    _gridTituloFinanceiroIntegracoes.CarregarGrid();

    carregarTotaisIntegracao();
}

function carregarTotaisIntegracao() {

    executarReST("TituloFinanceiroIntegracao/ObterTotaisIntegracoes", { Codigo: _pesquisaTituloFinanceiroIntegracoes.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            _tituloFinanceiroIntegracao.TotalGeral.val(retorno.Data.TotalGeral);
            _tituloFinanceiroIntegracao.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
            _tituloFinanceiroIntegracao.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
            _tituloFinanceiroIntegracao.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
            _tituloFinanceiroIntegracao.TotalIntegrado.val(retorno.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function controlarExibicaoAbaIntegracoes() {
    if (_pesquisaTituloFinanceiroIntegracoes.Codigo.val() > 0 && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiNFe) {
        $("#liTabIntegracoes").show();
    }
    else {
        $("#liTabIntegracoes").hide();
    }
}

function fecharModalProblemaIntegracao() {
    Global.fecharModal("divModalMotivoProblemaIntegracao");
}