/// <reference path="DocumentoEntrada.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridDocumentoEntradaIntegracoes;
var _documentoEntradaIntegracao;
var _pesquisaDocumentoEntradaIntegracoes;
var _problemaDocumentoEntradaIntegracao;
var _pesquisaHistoricoIntegracao;
var _gridHistoricoIntegracao;

/*
 * Declaração das Classes
 */
var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function DocumentoEntradaIntegracao() {
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Transportadores.Motorista.AguardandoIntegracao.getFieldDescription() });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Transportadores.Motorista.AguardandoRetorno.getFieldDescription() });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Transportadores.Motorista.TotalGeral.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Transportadores.Motorista.Integrados.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Transportadores.Motorista.ProblemasNaIntegracao.getFieldDescription() });

    this.ObterTotais = PropertyEntity({ eventClick: carregarTotaisIntegracaoDocumentoEntrada, type: types.event, text: ko.observable(Localization.Resources.Transportadores.Motorista.ObterTotais), idGrid: guid(), visible: ko.observable(true) });
};

function PesquisaDocumentoEntradaIntegracao() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Codigos = PropertyEntity({ val: ko.observable([0]), getType: new Array() });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: ko.observable(Localization.Resources.Gerais.Geral.Situacao.getFieldDescription()), def: "", issue: 272 });

    this.Pesquisar = PropertyEntity({ eventClick: carregarIntegracoesDocumentoEntrada, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Pesquisar), idGrid: guid(), visible: ko.observable(true) });
    this.ReenviarTodos = PropertyEntity({ eventClick: reenviarTodasIntegracoesClick, type: types.event, text: ko.observable(Localization.Resources.Transportadores.Motorista.ReenviarTodas), visible: ko.observable(false) });
};

function ProblemaDocumentoEntradaIntegracao() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ type: types.map, val: ko.observable(""), text: ko.observable(Localization.Resources.Transportadores.Motorista.Motivo), maxlength: 300, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarProblemaIntegracaoClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadGridDocumentoEntradaIntegracoes() {
    var linhasPorPaginas = 5;
    var opcaoIntegrar = { descricao: Localization.Resources.Transportadores.Motorista.Integrar, id: guid(), metodo: integrarClick, icone: "" };
    var opcaoProblemaIntegracao = { descricao: "Problema", id: guid(), metodo: exibirModalProblemaIntegracaoClick, icone: "" };
    var historico = { descricao: Localization.Resources.Transportadores.Motorista.HistoricoDeIntegracao, id: guid(), metodo: exibirHistoricoIntegracoesClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [opcaoIntegrar, historico, opcaoProblemaIntegracao] };

    _gridDocumentoEntradaIntegracoes = new GridView(_pesquisaDocumentoEntradaIntegracoes.Pesquisar.idGrid, "DocumentoEntradaIntegracao/PesquisaDocumentoEntradaIntegracoes", _pesquisaDocumentoEntradaIntegracoes, menuOpcoes, null, linhasPorPaginas);
    _gridDocumentoEntradaIntegracoes.CarregarGrid();
}

function LoadDocumentoEntradaIntegracoes() {
    $.get("Content/Static/Integracao/Integracao.html?dyn=" + guid(), function (html) {
        $("#modalIntegracoesConteudo").append(html);

        LocalizeCurrentPage();

        _documentoEntradaIntegracao = new DocumentoEntradaIntegracao();
        KoBindings(_documentoEntradaIntegracao, "knockoutDadosIntegracao");

        _pesquisaDocumentoEntradaIntegracoes = new PesquisaDocumentoEntradaIntegracao();
        KoBindings(_pesquisaDocumentoEntradaIntegracoes, "knockoutPesquisaIntegracao", false, _pesquisaDocumentoEntradaIntegracoes.Pesquisar.id);

        _problemaDocumentoEntradaIntegracao = new ProblemaDocumentoEntradaIntegracao();
        KoBindings(_problemaDocumentoEntradaIntegracao, "knockoutMotivoProblemaIntegracao");

        loadGridDocumentoEntradaIntegracoes();
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
    if (ValidarCamposObrigatorios(_problemaDocumentoEntradaIntegracao)) {
        var dadosProblemaIntegracao = RetornarObjetoPesquisa(_problemaDocumentoEntradaIntegracao);

        executarReST("DocumentoEntradaIntegracao/ProblemaIntegracao", dadosProblemaIntegracao, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    fecharModalProblemaIntegracao();
                    carregarIntegracoesDocumentoEntrada();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        });
    }
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function exibirModalProblemaIntegracaoClick(registroSelecionado) {
    _problemaDocumentoEntradaIntegracao.Codigo.val(registroSelecionado.Codigo);

    Global.abrirModal('divModalMotivoProblemaIntegracao');
    $("#divModalMotivoProblemaIntegracao").one('hidden.bs.modal', function () {
        LimparCampos(_problemaDocumentoEntradaIntegracao);
    });
}

function integrarClick(registroSelecionado) {
    executarReST("DocumentoEntradaIntegracao/Integrar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                carregarIntegracoesDocumentoEntrada();
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
    executarDownload("DocumentoEntradaIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

/*
 * Declaração das Funções Públicas
 */

function recarregarDocumentoEntradaIntegracoes(_documentoEntrada) {
    _pesquisaDocumentoEntradaIntegracoes.Codigo.val(_documentoEntrada.Codigo);
    _pesquisaDocumentoEntradaIntegracoes.Situacao.val(_documentoEntrada.Situacao);

    controlarExibicaoModalDocumentoEntradaIntegracoes();
    carregarIntegracoesDocumentoEntrada();
}

/*
 * Declaração das Funções
 */

function BuscarHistoricoIntegracao(integracao) {
    _pesquisaHistoricoIntegracao = new PesquisaHistoricoIntegracao();
    _pesquisaHistoricoIntegracao.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "DocumentoEntradaIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function carregarIntegracoesDocumentoEntrada() {
    _gridDocumentoEntradaIntegracoes.CarregarGrid();
    carregarTotaisIntegracaoDocumentoEntrada();
}

function carregarTotaisIntegracaoDocumentoEntrada() {
    executarReST("DocumentoEntradaIntegracao/ObterTotaisIntegracoes", { Codigo: _pesquisaDocumentoEntradaIntegracoes.Codigo }, function (retorno) {
        if (retorno.Success) {
            _documentoEntradaIntegracao.TotalGeral.val(retorno.Data.TotalGeral);
            _documentoEntradaIntegracao.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
            _documentoEntradaIntegracao.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
            _documentoEntradaIntegracao.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
            _documentoEntradaIntegracao.TotalIntegrado.val(retorno.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function controlarExibicaoModalDocumentoEntradaIntegracoes() {
    if (_pesquisaDocumentoEntradaIntegracoes.Codigo.val() > 0)
        Global.abrirModal("divModalIntegracoes");   
}

function fecharModalProblemaIntegracao() {
    Global.fecharModal("divModalMotivoProblemaIntegracao");
}