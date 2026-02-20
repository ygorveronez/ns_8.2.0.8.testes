/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Enumeradores/EnumTipoDocumentoAcompanhamento.js" />

var _pesquisaMovimentacaoContaPagarTransportador;
var _gridMovimentacaoContaPagaTransportador;

var PesquisaMovimentacaoContaPagar = function () {
    this.DataDocumentoInicial = PropertyEntity({ text: "Data Documento Inicial:", enable: false, getType: typesKnockout.dateTime, val: ko.observable(""), enable: false });
    this.DataDocumentoFinal = PropertyEntity({ text: "Data Documento Final:", enable: false, getType: typesKnockout.dateTime, val: ko.observable(""), enable: false });
    this.DataCompensacaoInicial = PropertyEntity({ text: "Data Compensação Inicial:", enable: false, getType: typesKnockout.dateTime, val: ko.observable(""), enable: false });
    this.DataCompensacaoFinal = PropertyEntity({ text: "Data Compensação Inicial:", enable: false, getType: typesKnockout.dateTime, val: ko.observable(""), enable: false });
    this.DocumentoCompensacao = PropertyEntity({ text: "Documento de Baixa:", getType: typesKnockout.string, visible: ko.observable(true) });
    this.NumeroDocumento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Número Documento:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoArquivo = PropertyEntity({ text: "Tipo Arquivo: ", val: ko.observable(EnumTipoDocumentoAcompanhamento.SemTipo), getType: typesKnockout.select, options: EnumTipoDocumentoAcompanhamento.obterOpcoes(), def: EnumTipoDocumentoAcompanhamento.SemTipo });
    this.TodasFiliaisTransportador = PropertyEntity({ text: "Todas as filiais do Transportador", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMovimentacaoContaPagaTransportador.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: "grid-movimentacao-contas-pagar-transportador", visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

function loadMovimentacaoContaPagarTransportador() {
    _pesquisaMovimentacaoContaPagarTransportador = new PesquisaMovimentacaoContaPagar();
    KoBindings(_pesquisaMovimentacaoContaPagarTransportador, "knockoutPesquisaMovimentacaoContaPagarTransportador");

    new BuscarCTes(_pesquisaMovimentacaoContaPagarTransportador.NumeroDocumento);

    loadGridMovimentacaoContaPagarTransportador();
}

function loadGridMovimentacaoContaPagarTransportador() {
    var configExportacao = {
        url: "MovimentacaoContasPagarTransportador/ExportarPesquisa",
        titulo: "Movimentação Conta a Pagar Transportador"
    };

    _gridMovimentacaoContaPagaTransportador = new GridView(_pesquisaMovimentacaoContaPagarTransportador.Pesquisar.idGrid, "MovimentacaoContasPagarTransportador/Pesquisa", _pesquisaMovimentacaoContaPagarTransportador, null, null, 25, null, null, null, null, null, null, configExportacao);
    _gridMovimentacaoContaPagaTransportador.SetPermitirEdicaoColunas(true);
    _gridMovimentacaoContaPagaTransportador.SetSalvarPreferenciasGrid(true);

    _gridMovimentacaoContaPagaTransportador.CarregarGrid();
}