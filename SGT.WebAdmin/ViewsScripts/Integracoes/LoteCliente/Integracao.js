/// <reference path="LoteCliente.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridIntegracoes;
var _integracao;
var _pesquisaIntegracoes;

/*
 * Declaração das Classes
 */

function Integracao() {
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Integração:" });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Retorno:" });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Geral:" });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Integrados:" });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Problemas na Integração:" });

    this.ObterTotais = PropertyEntity({ eventClick: carregarTotaisIntegracao, type: types.event, text: "Obter Totais", idGrid: guid(), visible: ko.observable(true) });
};

function PesquisaIntegracoes() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: "Situação:", def: "", issue: 272 });

    this.Pesquisar = PropertyEntity({ eventClick: carregarIntegracoes, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.ReenviarTodos = PropertyEntity({ eventClick: reenviarTodasIntegracoesClick, type: types.event, text: "Reenviar Todas", visible: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function VisibilidadeOpcaoReenviarIntegracaoEDI(data) {
    data.Tipo == EnumTipoIntegracao.NaoPossuiIntegracao || data.SituacaoIntegracao != EnumSituacaoIntegracao.ProblemaIntegracao
}

function loadGridIntegracoes() {
    var download = { descricao: "Download", id: guid(), metodo: DownloadIntegracaoEDI, tamanho: "20", icone: "" };
    var reenviar = { descricao: "Reenviar", id: guid(), metodo: ReenviarIntegracaoEDI, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoReenviarIntegracaoEDI };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [download, reenviar] };

    _gridIntegracoes = new GridView(_pesquisaIntegracoes.Pesquisar.idGrid, "LoteClienteIntegracaoEDI/Pesquisa", _pesquisaIntegracoes, menuOpcoes, null, 5);
    _gridIntegracoes.CarregarGrid();
}

function loadIntegracoes() {
    $.get("Content/Static/Integracao/Integracao.html?dyn=" + guid(), function (html) {
        $("#tabIntegracoesConteudo").append(html);

        LocalizeCurrentPage();

        _integracao = new Integracao();
        KoBindings(_integracao, "knockoutDadosIntegracao");

        _pesquisaIntegracoes = new PesquisaIntegracoes();
        KoBindings(_pesquisaIntegracoes, "knockoutPesquisaIntegracao", false, _pesquisaIntegracoes.Pesquisar.id);

        loadGridIntegracoes();
    });
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function DownloadIntegracaoEDI(data) {
    executarDownload("LoteClienteIntegracaoEDI/Download", { Codigo: data.Codigo });
}

function ReenviarIntegracaoEDI(data) {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar o arquivo EDI?", function () {
        executarReST("LoteClienteIntegracaoEDI/Reenviar", { Codigo: data.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
                    _gridIntegracoes.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function reenviarTodasIntegracoesClick() {

}

/*
 * Declaração das Funções Públicas
 */

function recarregarIntegracoes() {
    _pesquisaIntegracoes.Codigo.val(_loteCliente.Codigo.val());

    carregarIntegracoes();
}

/*
 * Declaração das Funções
 */

function carregarIntegracoes() {
    _gridIntegracoes.CarregarGrid();

    carregarTotaisIntegracao();
}

function carregarTotaisIntegracao() {
    executarReST("LoteClienteIntegracaoEDI/ObterTotais", { Codigo: _pesquisaIntegracoes.Codigo.val() }, function (retorno) {
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