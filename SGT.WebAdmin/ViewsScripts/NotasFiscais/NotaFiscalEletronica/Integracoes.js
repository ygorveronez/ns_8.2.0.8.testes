/// <reference path="NotaFiscalEletronica.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridNotaFiscalEletronicaIntegracoes;
var _notaFiscalEletronicaIntegracao;
var _pesquisaNotaFiscalEletronicaIntegracoes;
var _problemaNotaFiscalEletronicaIntegracao;
var _pesquisaHistoricoIntegracao;
var _gridHistoricoIntegracao;

/*
 * Declaração das Classes
 */
var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function NotaFiscalEletronicaIntegracao() {
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Transportadores.Motorista.AguardandoIntegracao.getFieldDescription() });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Transportadores.Motorista.AguardandoRetorno.getFieldDescription() });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Transportadores.Motorista.TotalGeral.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Transportadores.Motorista.Integrados.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Transportadores.Motorista.ProblemasNaIntegracao.getFieldDescription() });

    this.ObterTotais = PropertyEntity({ eventClick: carregarTotaisIntegracaoNotaFiscalEletronica, type: types.event, text: ko.observable(Localization.Resources.Transportadores.Motorista.ObterTotais), idGrid: guid(), visible: ko.observable(true) });
};

function PesquisaNotaFiscalEletronicaIntegracao() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Codigos = PropertyEntity({ val: ko.observable([0]), getType: new Array() });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: ko.observable(Localization.Resources.Gerais.Geral.Situacao.getFieldDescription()), def: "", issue: 272 });

    this.Pesquisar = PropertyEntity({ eventClick: carregarIntegracoesNotaFiscalEletronica, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Pesquisar), idGrid: guid(), visible: ko.observable(true) });
    this.ReenviarTodos = PropertyEntity({ eventClick: reenviarTodasIntegracoesClick, type: types.event, text: ko.observable(Localization.Resources.Transportadores.Motorista.ReenviarTodas), visible: ko.observable(false) });
};

function ProblemaNotaFiscalEletronicaIntegracao() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ type: types.map, val: ko.observable(""), text: ko.observable(Localization.Resources.Transportadores.Motorista.Motivo), maxlength: 300, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarProblemaIntegracaoClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadGridNotaFiscalEletronicaIntegracoes() {
    var linhasPorPaginas = 5;
    var opcaoIntegrar = { descricao: Localization.Resources.Transportadores.Motorista.Integrar, id: guid(), metodo: integrarClick, icone: "" };
    var opcaoProblemaIntegracao = { descricao: "Problema", id: guid(), metodo: exibirModalProblemaIntegracaoClick, icone: "" };
    var historico = { descricao: Localization.Resources.Transportadores.Motorista.HistoricoDeIntegracao, id: guid(), metodo: exibirHistoricoIntegracoesClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [opcaoIntegrar, historico, opcaoProblemaIntegracao] };

    _gridNotaFiscalEletronicaIntegracoes = new GridView(_pesquisaNotaFiscalEletronicaIntegracoes.Pesquisar.idGrid, "NotaFiscalEletronicaIntegracao/PesquisaNotaFiscalEletronicaIntegracoes", _pesquisaNotaFiscalEletronicaIntegracoes, menuOpcoes, null, linhasPorPaginas);
    _gridNotaFiscalEletronicaIntegracoes.CarregarGrid();
}

function LoadNotaFiscalEletronicaIntegracoes() {
    $.get("Content/Static/Integracao/Integracao.html?dyn=" + guid(), function (html) {
        $("#modalIntegracoesConteudo").append(html);

        LocalizeCurrentPage();

        _notaFiscalEletronicaIntegracao = new NotaFiscalEletronicaIntegracao();
        KoBindings(_notaFiscalEletronicaIntegracao, "knockoutDadosIntegracao");

        _pesquisaNotaFiscalEletronicaIntegracoes = new PesquisaNotaFiscalEletronicaIntegracao();
        KoBindings(_pesquisaNotaFiscalEletronicaIntegracoes, "knockoutPesquisaIntegracao", false, _pesquisaNotaFiscalEletronicaIntegracoes.Pesquisar.id);

        _problemaNotaFiscalEletronicaIntegracao = new ProblemaNotaFiscalEletronicaIntegracao();
        KoBindings(_problemaNotaFiscalEletronicaIntegracao, "knockoutMotivoProblemaIntegracao");

        loadGridNotaFiscalEletronicaIntegracoes();
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
    if (ValidarCamposObrigatorios(_problemaNotaFiscalEletronicaIntegracao)) {
        var dadosProblemaIntegracao = RetornarObjetoPesquisa(_problemaNotaFiscalEletronicaIntegracao);

        executarReST("NotaFiscalEletronicaIntegracao/ProblemaIntegracao", dadosProblemaIntegracao, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    fecharModalProblemaIntegracao();
                    carregarIntegracoesNotaFiscalEletronica();
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
    _problemaNotaFiscalEletronicaIntegracao.Codigo.val(registroSelecionado.Codigo);

    Global.abrirModal('divModalMotivoProblemaIntegracao');
    $("#divModalMotivoProblemaIntegracao").one('hidden.bs.modal', function () {
        LimparCampos(_problemaNotaFiscalEletronicaIntegracao);
    });
}

function integrarClick(registroSelecionado) {
    executarReST("NotaFiscalEletronicaIntegracao/Integrar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                carregarIntegracoesNotaFiscalEletronica();
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
    executarDownload("NotaFiscalEletronicaIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

/*
 * Declaração das Funções Públicas
 */

function recarregarNotaFiscalEletronicaIntegracoes(notaFiscalEletronicaGrid) {
    _pesquisaNotaFiscalEletronicaIntegracoes.Codigo.val(notaFiscalEletronicaGrid.Codigo);
    _pesquisaNotaFiscalEletronicaIntegracoes.Situacao.val(notaFiscalEletronicaGrid.Situacao);

    controlarExibicaoModalNotaFiscalEletronicaIntegracoes();
    carregarIntegracoesNotaFiscalEletronica();
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

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "NotaFiscalEletronicaIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function carregarIntegracoesNotaFiscalEletronica() {
    _gridNotaFiscalEletronicaIntegracoes.CarregarGrid();
    carregarTotaisIntegracaoNotaFiscalEletronica();
}

function carregarTotaisIntegracaoNotaFiscalEletronica() {
    executarReST("NotaFiscalEletronicaIntegracao/ObterTotaisIntegracoes", { Codigo: _pesquisaNotaFiscalEletronicaIntegracoes.Codigo }, function (retorno) {
        if (retorno.Success) {
            _notaFiscalEletronicaIntegracao.TotalGeral.val(retorno.Data.TotalGeral);
            _notaFiscalEletronicaIntegracao.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
            _notaFiscalEletronicaIntegracao.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
            _notaFiscalEletronicaIntegracao.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
            _notaFiscalEletronicaIntegracao.TotalIntegrado.val(retorno.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function controlarExibicaoModalNotaFiscalEletronicaIntegracoes() {
    if (_pesquisaNotaFiscalEletronicaIntegracoes.Codigo.val() > 0)
        Global.abrirModal("divModalIntegracoes");   
}

function fecharModalProblemaIntegracao() {
    Global.fecharModal("divModalMotivoProblemaIntegracao");
}