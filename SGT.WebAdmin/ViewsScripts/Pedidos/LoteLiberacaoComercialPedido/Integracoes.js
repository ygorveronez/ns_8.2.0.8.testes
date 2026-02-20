/// <reference path="LoteLiberacaoComercialPedido.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />

var _gridLoteLiberacaoComercialPedidoIntegracoes;
var _loteLiberacaoComercialPedidoIntegracao;
var _pesquisaLoteLiberacaoComercialPedidoIntegracoes;
var _pesquisaHistoricoIntegracao;
var _gridHistoricoIntegracao;

var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function LoteLiberacaoComercialPedidoIntegracao() {
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Integração:" });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Retorno:" });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Geral:" });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Integrados:" });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Problemas na Integração:" });

    this.ObterTotais = PropertyEntity({ eventClick: carregarTotaisIntegracao, type: types.event, text: ko.observable("Obter Totais"), idGrid: guid(), visible: ko.observable(true) });
};

function PesquisaLoteLiberacaoComercialPedidoIntegracoes() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: ko.observable(Localization.Resources.Gerais.Geral.Situacao.getFieldDescription()), def: "", issue: 272 });

    this.Pesquisar = PropertyEntity({ eventClick: carregarIntegracoes, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Pesquisar), idGrid: guid(), visible: ko.observable(true) });
    this.ReenviarTodos = PropertyEntity({ eventClick: reenviarTodasIntegracoesClick, type: types.event, text: ko.observable("Reenviar Todos"), visible: ko.observable(false) });
};

function loadGridLoteLiberacaoComercialPedidoIntegracoes() {
    const linhasPorPaginas = 5;
    const opcaoIntegrar = { descricao: "Integrar", id: guid(), metodo: integrarClick, icone: "" };
    const historico = { descricao: "Histórico de integração", id: guid(), metodo: exibirHistoricoIntegracoesClick, icone: "" };
    const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [opcaoIntegrar, historico] };

    _gridLoteLiberacaoComercialPedidoIntegracoes = new GridView(_pesquisaLoteLiberacaoComercialPedidoIntegracoes.Pesquisar.idGrid, "LoteLiberacaoComercialPedidoIntegracao/Pesquisa", _pesquisaLoteLiberacaoComercialPedidoIntegracoes, menuOpcoes, null, linhasPorPaginas);
    _gridLoteLiberacaoComercialPedidoIntegracoes.CarregarGrid();
}

function LoadLoteLiberacaoComercialPedidoIntegracao() {
    $.get("Content/Static/Integracao/Integracao.html?dyn=" + guid(), function (html) {
        $("#tabIntegracoesConteudo").append(html);

        LocalizeCurrentPage();

        _loteLiberacaoComercialPedidoIntegracao = new LoteLiberacaoComercialPedidoIntegracao();
        KoBindings(_loteLiberacaoComercialPedidoIntegracao, "knockoutDadosIntegracao");

        _pesquisaLoteLiberacaoComercialPedidoIntegracoes = new PesquisaLoteLiberacaoComercialPedidoIntegracoes();
        KoBindings(_pesquisaLoteLiberacaoComercialPedidoIntegracoes, "knockoutPesquisaIntegracao", false, _pesquisaLoteLiberacaoComercialPedidoIntegracoes.Pesquisar.id);

        loadGridLoteLiberacaoComercialPedidoIntegracoes();
    });
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function exibirHistoricoIntegracoesClick(integracao) {
    BuscarHistoricoIntegracao(integracao);
    Global.abrirModal("divModalHistoricoIntegracao");
}


function integrarClick(registroSelecionado) {
    executarReST("LoteLiberacaoComercialPedidoIntegracao/Integrar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
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

function LiberarcomIntegracaoFalhaClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente liberar com integração falha?", function () {
        executarReST("LoteLiberacaoComercialPedidoIntegracao/LiberarcomIntegracaoFalha", { Codigo: _pesquisaLoteLiberacaoComercialPedidoIntegracoes.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Liberação efetuada com sucesso.");

                    _gridLoteLiberacaoComercialPedido.CarregarGrid();
                    _CRUDLoteLiberacaoComercialPedido.LiberarcomIntegracaoFalha.visible(false);
                    Etapa2Aprovada();

                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}


function reenviarTodasIntegracoesClick() {

}

function DownloadArquivosHistoricoIntegracaoClick(historicoConsulta) {
    executarDownload("LoteLiberacaoComercialPedidoIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

/*
 * Declaração das Funções Públicas
 */

function recarregarLoteLiberacaoComercialPedidoIntegracoes() {
    _pesquisaLoteLiberacaoComercialPedidoIntegracoes.Codigo.val(_pesquisaCadastroLoteLiberacaoComercialPedido.Codigo.val());
    carregarIntegracoes();
}

/*
 * Declaração das Funções
 */

function BuscarHistoricoIntegracao(integracao) {
    _pesquisaHistoricoIntegracao = new PesquisaHistoricoIntegracao();
    _pesquisaHistoricoIntegracao.Codigo.val(integracao.Codigo);

    const download = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoClick, tamanho: "20", icone: "" };

    const menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "LoteLiberacaoComercialPedidoIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function carregarIntegracoes() {
    _gridLoteLiberacaoComercialPedidoIntegracoes.CarregarGrid();

    carregarTotaisIntegracao();
}

function carregarTotaisIntegracao() {
    executarReST("LoteLiberacaoComercialPedidoIntegracao/ObterTotaisIntegracoes", { Codigo: _pesquisaLoteLiberacaoComercialPedidoIntegracoes.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            _loteLiberacaoComercialPedidoIntegracao.TotalGeral.val(retorno.Data.TotalGeral);
            _loteLiberacaoComercialPedidoIntegracao.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
            _loteLiberacaoComercialPedidoIntegracao.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
            _loteLiberacaoComercialPedidoIntegracao.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
            _loteLiberacaoComercialPedidoIntegracao.TotalIntegrado.val(retorno.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}