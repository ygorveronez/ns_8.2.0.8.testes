/// <reference path="PreAgrupamentoCargaDetalhe.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPreAgrupamentoCarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _pesquisaPreAgrupamentoCarga;
var _gridPreAgrupamentoCarga;

/*
 * Declaração das Classes
 */

var PesquisaPreAgrupamentoCarga = function () {
    var dataAtual = moment().add(-1, 'days').format("DD/MM/YYYY");
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });

    this.Agrupamento = PropertyEntity({ text: "Código Agrupamento:", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 10 });
    this.NumeroNota = PropertyEntity({ text: "Número Nota:", maxlength: 20 });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoPreAgrupamentoCarga.Todas), options: EnumSituacaoPreAgrupamentoCarga.obterOpcoesPesquisa(), def: EnumSituacaoPreAgrupamentoCarga.Todas, text: "Situação: " });
    this.Viagem = PropertyEntity({ text: "Código Viagem:", maxlength: 20 });
    
    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridPreAgrupamentoCarga, type: types.event, text: "Pesquisar", idGrid: "grid-pesquisa-pre-agrupamento-carga", visible: ko.observable(true) });

    this.Emitente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Emitente:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Expedidor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Recebedor:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataPrevisaoEntregaInicial = PropertyEntity({ text: "Data Prevista de Entrega Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataPrevisaoEntregaFinal = PropertyEntity({ text: "Data Prevista de Entrega Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });

    this.DataPrevisaoEntregaInicial.dateRangeLimit = this.DataPrevisaoEntregaFinal;
    this.DataPrevisaoEntregaFinal.dateRangeInit = this.DataPrevisaoEntregaInicial;

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridPreAgrupamentoCarga() {
    var opcaoDetalhes = { descricao: "Detalhes", id: guid(), metodo: detalhesClick, icone: "" };
    var opcaoDownload = { descricao: "Download XML Integração", id: guid(), metodo: downloadArquivoIntegracaoClick, icone: "" };
    var excluir = { descricao: "Excluir", id: guid(), metodo: ExcluirIntegracaoAgrupadoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [opcaoDetalhes, opcaoDownload, excluir] };
    var configuracoesExportacao = { url: "AcompanhamentoPreAgrupamentoCarga/ExportarPesquisa", titulo: "Acompanhamento de Pré Agrupamento de Carga" };
    var quantidadePorPagina = 20;

    _gridPreAgrupamentoCarga = new GridViewExportacao(_pesquisaPreAgrupamentoCarga.Pesquisar.idGrid, "AcompanhamentoPreAgrupamentoCarga/Pesquisa", _pesquisaPreAgrupamentoCarga, menuOpcoes, configuracoesExportacao, undefined, quantidadePorPagina);

    _gridPreAgrupamentoCarga.SetPermitirEdicaoColunas(true);
    _gridPreAgrupamentoCarga.SetSalvarPreferenciasGrid(true);
    _gridPreAgrupamentoCarga.CarregarGrid();
}

function loadPreAgrupamentoCarga() {
    _pesquisaPreAgrupamentoCarga = new PesquisaPreAgrupamentoCarga();
    KoBindings(_pesquisaPreAgrupamentoCarga, "knockoutPesquisaPreAgrupamentoCarga", false, _pesquisaPreAgrupamentoCarga.Pesquisar.id);

    loadGridPreAgrupamentoCarga();
    loadPreAgrupamentoCargaDetalhe();

    HeaderAuditoria("PreAgrupamentoCargaAgrupador", _pesquisaPreAgrupamentoCarga);

    new BuscarClientes(_pesquisaPreAgrupamentoCarga.Emitente);
    new BuscarClientes(_pesquisaPreAgrupamentoCarga.Recebedor);
    new BuscarClientes(_pesquisaPreAgrupamentoCarga.Expedidor);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function detalhesClick(registroSelecionado) {
    exibirDetalhes(registroSelecionado);
}

function ExcluirIntegracaoAgrupadoClick(registroSelecionado) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse agrupamento?", function () {
        executarReST("AcompanhamentoPreAgrupamentoCarga/ExcluirPreAgrupamento", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Registro excluído com sucesso.");
                    recarregarGridPreAgrupamentoCarga();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });

}

function downloadArquivoIntegracaoClick(registroSelecionado) {
    executarDownload("AcompanhamentoPreAgrupamentoCarga/DownloadArquivoIntegracao", { Codigo: registroSelecionado.Codigo });
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */

function recarregarGridPreAgrupamentoCarga() {
    _gridPreAgrupamentoCarga.CarregarGrid();
}