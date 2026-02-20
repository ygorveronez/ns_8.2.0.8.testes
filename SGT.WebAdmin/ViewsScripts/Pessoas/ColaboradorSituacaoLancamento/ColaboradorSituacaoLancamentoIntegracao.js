/// <reference path="ColaboradorSituacaoLancamento.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />

var _gridColaboradorSituacaoLancamentoIntegracao;
var _colaboradorSituacaoLancamentoIntegracao;
var _pesquisaColaboradorSituacaoLancamentoIntegracao;
var _problemaColaboradorSituacaoLancamentoIntegracao;
var _gridHistoricoIntegracao;
var _historicoIntegracaoColaboradorSituacao;

var HistoricoIntegracaoColaboradorSituacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({});
}

function ColaboradorSituacaoLancamentoIntegracao() {
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando integração:" });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando retorno:" });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total geral:" });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Integrados:" });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Problemas na integração:" });

    this.ObterTotais = PropertyEntity({ eventClick: carregarTotaisIntegracao, type: types.event, text: ko.observable("Obter totais"), idGrid: guid(), visible: ko.observable(true) });
};

function PesquisaColaboradorSituacaoLancamentoIntegracao() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: ko.observable(Localization.Resources.Gerais.Geral.Situacao.getFieldDescription()), def: "", issue: 272 });

    this.Pesquisar = PropertyEntity({ eventClick: carregarIntegracoes, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Pesquisar), idGrid: guid(), visible: ko.observable(true) });
    this.ReenviarTodos = PropertyEntity({ eventClick: reenviarTodasIntegracoesClick, type: types.event, text: ko.observable("Reenviar todas"), visible: ko.observable(false) });
};

function ProblemaColaboradorSituacaoLancamentoIntegracao() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ type: types.map, val: ko.observable(""), text: ko.observable("Motivo"), maxlength: 300, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarProblemaIntegracaoClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar) });
};

function loadGridColaboradorSituacaoLancamentoIntegracao() {
    var linhasPorPaginas = 5;
    var opcaoIntegrar = { descricao: "Integrar", id: guid(), metodo: integrarClick, icone: "" };

    var historico = { descricao: "Histórico de integração", id: guid(), metodo: exibirHistoricoIntegracoesClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [opcaoIntegrar, historico] }; // opcaoProblemaIntegracao

    _gridColaboradorSituacaoLancamentoIntegracao = new GridView(_pesquisaColaboradorSituacaoLancamentoIntegracao.Pesquisar.idGrid, "ColaboradorSituacaoLancamentoIntegracao/PesquisaIntegracao", _pesquisaColaboradorSituacaoLancamentoIntegracao, menuOpcoes, null, linhasPorPaginas);
    _gridColaboradorSituacaoLancamentoIntegracao.CarregarGrid();
}

function loadColaboradorSituacaoLancamentoIntegracao() {
    $.get("Content/Static/Integracao/Integracao.html?dyn=" + guid(), function (html) {
        $("#tabIntegracoesConteudo").append(html);

        LocalizeCurrentPage();

        _colaboradorSituacaoLancamentoIntegracao = new ColaboradorSituacaoLancamentoIntegracao();
        KoBindings(_colaboradorSituacaoLancamentoIntegracao, "knockoutDadosIntegracao");

        _pesquisaColaboradorSituacaoLancamentoIntegracao = new PesquisaColaboradorSituacaoLancamentoIntegracao();
        KoBindings(_pesquisaColaboradorSituacaoLancamentoIntegracao, "knockoutPesquisaIntegracao", false, _pesquisaColaboradorSituacaoLancamentoIntegracao.Pesquisar.id);

        _problemaColaboradorSituacaoLancamentoIntegracao = new ProblemaColaboradorSituacaoLancamentoIntegracao();
        KoBindings(_problemaColaboradorSituacaoLancamentoIntegracao, "knockoutMotivoProblemaIntegracao");

        _historicoIntegracaoColaboradorSituacao = new HistoricoIntegracaoColaboradorSituacao();
        KoBindings(_historicoIntegracaoColaboradorSituacao, "knockoutHistoricoIntegracaoColaboradorSituacao");

        loadGridColaboradorSituacaoLancamentoIntegracao();
    });
}

/****** EVENTOS ******/

function exibirHistoricoIntegracoesClick(integracao) {
    BuscarHistoricoIntegracao(integracao);
    Global.abrirModal("divModalHistoricoIntegracao");
}

function adicionarProblemaIntegracaoClick() {
    if (ValidarCamposObrigatorios(_problemaColaboradorSituacaoLancamentoIntegracao)) {
        var dadosProblemaIntegracao = RetornarObjetoPesquisa(_problemaColaboradorSituacaoLancamentoIntegracao);

        executarReST("ColaboradorSituacaoLancamentoIntegracao/ProblemaIntegracao", dadosProblemaIntegracao, function (retorno) {
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
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function exibirModalProblemaIntegracaoClick(registroSelecionado) {
    _problemaColaboradorSituacaoLancamentoIntegracao.Codigo.val(registroSelecionado.Codigo);

    Global.abrirModal('divModalMotivoProblemaIntegracao');
    $("#divModalMotivoProblemaIntegracao").one('hidden.bs.modal', function () {
        LimparCampos(_problemaColaboradorSituacaoLancamentoIntegracao);
    });
}

function integrarClick(registroSelecionado) {
    executarReST("ColaboradorSituacaoLancamentoIntegracao/Integrar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
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
    executarDownload("ColaboradorSituacaoLancamentoIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

/******* MÉTODOS ********/

function recarregarColaboradorSituacaoLancamentoIntegracao() {
    _pesquisaColaboradorSituacaoLancamentoIntegracao.Codigo.val(_colaboradorSituacaoLancamento.Codigo.val());

    controlarExibicaoAbaIntegracoes();
    carregarIntegracoes();
    _gridColaboradorSituacaoLancamentoIntegracao.CarregarGrid();
}

function BuscarHistoricoIntegracao(integracao) {
    _historicoIntegracaoColaboradorSituacao.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracao = new GridView(_historicoIntegracaoColaboradorSituacao.Grid.id, "ColaboradorSituacaoLancamentoIntegracao/ConsultarHistoricoIntegracao", _historicoIntegracaoColaboradorSituacao, menuOpcoes);
    _gridHistoricoIntegracao.CarregarGrid();
}

function carregarIntegracoes() {
    _gridColaboradorSituacaoLancamentoIntegracao.CarregarGrid();

    carregarTotaisIntegracao();
}

function carregarTotaisIntegracao() {
    executarReST("ColaboradorSituacaoLancamentoIntegracao/ObterTotaisIntegracoes", { Codigo: _pesquisaColaboradorSituacaoLancamentoIntegracao.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            _colaboradorSituacaoLancamentoIntegracao.TotalGeral.val(retorno.Data.TotalGeral);
            _colaboradorSituacaoLancamentoIntegracao.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
            _colaboradorSituacaoLancamentoIntegracao.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
            _colaboradorSituacaoLancamentoIntegracao.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
            _colaboradorSituacaoLancamentoIntegracao.TotalIntegrado.val(retorno.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function controlarExibicaoAbaIntegracoes() {
    if (_pesquisaColaboradorSituacaoLancamentoIntegracao.Codigo.val() > 0) {
        $("#liIntegracoes").show();
        //_gridColaboradorSituacaoLancamentoIntegracao.CarregarGrid();
    }
    else
        $("#liIntegracoes").hide();
}

function fecharModalProblemaIntegracao() {
    Global.fecharModal('divModalMotivoProblemaIntegracao');    
}

function limparCamposColaboradorSituacaoLancamentoIntegracao() {
    LimparCampos(_pesquisaColaboradorSituacaoLancamentoIntegracao);
    LimparCampos(_colaboradorSituacaoLancamentoIntegracao);
    LimparCampos(_problemaColaboradorSituacaoLancamentoIntegracao);
    recarregarColaboradorSituacaoLancamentoIntegracao();
}