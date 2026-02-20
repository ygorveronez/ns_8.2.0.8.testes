/// <reference path="Motorista.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridMotoristaIntegracoes;
var _motoristaIntegracao;
var _pesquisaMotoristaIntegracoes;
var _problemaMotoristaIntegracao;
var _pesquisaHistoricoIntegracao;
var _gridHistoricoIntegracao;

/*
 * Declaração das Classes
 */
var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function MotoristaIntegracao() {
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Transportadores.Motorista.AguardandoIntegracao.getFieldDescription() });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Transportadores.Motorista.AguardandoRetorno.getFieldDescription() });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Transportadores.Motorista.TotalGeral.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Transportadores.Motorista.Integrados.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Transportadores.Motorista.ProblemasNaIntegracao.getFieldDescription() });

    this.ObterTotais = PropertyEntity({ eventClick: carregarTotaisIntegracao, type: types.event, text: ko.observable(Localization.Resources.Transportadores.Motorista.ObterTotais), idGrid: guid(), visible: ko.observable(true) });
};

function PesquisaMotoristaIntegracoes() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Codigos = PropertyEntity({ val: ko.observable([0]), getType: new Array() });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: ko.observable(Localization.Resources.Gerais.Geral.Situacao.getFieldDescription()), def: "", issue: 272 });

    this.Pesquisar = PropertyEntity({ eventClick: carregarIntegracoes, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Pesquisar), idGrid: guid(), visible: ko.observable(true) });
    this.ReenviarTodos = PropertyEntity({ eventClick: reenviarTodasIntegracoesClick, type: types.event, text: ko.observable(Localization.Resources.Transportadores.Motorista.ReenviarTodas), visible: ko.observable(false) });
};

function ProblemaMotoristaIntegracao() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ type: types.map, val: ko.observable(""), text: ko.observable(Localization.Resources.Transportadores.Motorista.Motivo), maxlength: 300, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarProblemaIntegracaoClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadGridMotoristaIntegracoes() {
    var linhasPorPaginas = 5;
    var opcaoIntegrar = { descricao: Localization.Resources.Transportadores.Motorista.Integrar, id: guid(), metodo: integrarClick, icone: "" };
    //var opcaoProblemaIntegracao = { descricao: "Problema", id: guid(), metodo: exibirModalProblemaIntegracaoClick, icone: "" };
    var historico = { descricao: Localization.Resources.Transportadores.Motorista.HistoricoDeIntegracao, id: guid(), metodo: exibirHistoricoIntegracoesClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [opcaoIntegrar, historico] }; //, opcaoProblemaIntegracao

    _gridMotoristaIntegracoes = new GridView(_pesquisaMotoristaIntegracoes.Pesquisar.idGrid, "MotoristaIntegracao/PesquisaMotoristaIntegracoes", _pesquisaMotoristaIntegracoes, menuOpcoes, null, linhasPorPaginas);
    _gridMotoristaIntegracoes.CarregarGrid();
}

function loadMotoristaIntegracoes() {
    $.get("Content/Static/Integracao/Integracao.html?dyn=" + guid(), function (html) {
        $("#tabIntegracoesConteudo").append(html);

        LocalizeCurrentPage();

        _motoristaIntegracao = new MotoristaIntegracao();
        KoBindings(_motoristaIntegracao, "knockoutDadosIntegracao");

        _pesquisaMotoristaIntegracoes = new PesquisaMotoristaIntegracoes();
        KoBindings(_pesquisaMotoristaIntegracoes, "knockoutPesquisaIntegracao", false, _pesquisaMotoristaIntegracoes.Pesquisar.id);

        _problemaMotoristaIntegracao = new ProblemaMotoristaIntegracao();
        KoBindings(_problemaMotoristaIntegracao, "knockoutMotivoProblemaIntegracao");

        loadGridMotoristaIntegracoes();
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
    if (ValidarCamposObrigatorios(_problemaMotoristaIntegracao)) {
        var dadosProblemaIntegracao = RetornarObjetoPesquisa(_problemaMotoristaIntegracao);

        executarReST("MotoristaIntegracao/ProblemaIntegracao", dadosProblemaIntegracao, function (retorno) {
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
    _problemaMotoristaIntegracao.Codigo.val(registroSelecionado.Codigo);

    Global.abrirModal('divModalMotivoProblemaIntegracao');
    $("#divModalMotivoProblemaIntegracao").one('hidden.bs.modal', function () {
        LimparCampos(_problemaMotoristaIntegracao);
    });
}

function integrarClick(registroSelecionado) {
    executarReST("MotoristaIntegracao/Integrar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
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
    executarDownload("MotoristaIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

/*
 * Declaração das Funções Públicas
 */

function recarregarMotoristaIntegracoes() {
    _pesquisaMotoristaIntegracoes.Codigo.val(_motorista.Codigo.val());

    controlarExibicaoAbaIntegracoes();
    carregarIntegracoes();
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

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "MotoristaIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function carregarIntegracoes() {
    _gridMotoristaIntegracoes.CarregarGrid();

    carregarTotaisIntegracao();
}

function carregarTotaisIntegracao() {
    executarReST("MotoristaIntegracao/ObterTotaisIntegracoes", { Codigo: _pesquisaMotoristaIntegracoes.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            _motoristaIntegracao.TotalGeral.val(retorno.Data.TotalGeral);
            _motoristaIntegracao.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
            _motoristaIntegracao.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
            _motoristaIntegracao.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
            _motoristaIntegracao.TotalIntegrado.val(retorno.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function controlarExibicaoAbaIntegracoes() {
    if (_pesquisaMotoristaIntegracoes.Codigo.val() > 0)
        $("#liTabIntegracoes").show();
    else
        $("#liTabIntegracoes").hide();
}

function fecharModalProblemaIntegracao() {
    Global.fecharModal("divModalMotivoProblemaIntegracao");
}