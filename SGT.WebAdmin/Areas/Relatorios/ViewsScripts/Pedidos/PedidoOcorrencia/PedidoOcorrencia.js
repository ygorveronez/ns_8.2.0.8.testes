/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoEntrega.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOcorrencia.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />

//********MAPEAMENTO KNOCKOUT********

var _relatorioPedidoOcorrencia, _gridPedidoOcorrencia, _pesquisaPedidoOcorrencia, _CRUDRelatorio, _CRUDFiltrosRelatorio;


var PesquisaPedidoOcorrencia = function () {
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.TipoRelatorio.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.NumeroPedido = PropertyEntity({ text: Localization.Resources.Pedidos.PedidoOcorrencia.NumeroPedido.getFieldDescription(), val: ko.observable(""), def: "" });
    this.NumeroCarga = PropertyEntity({ text: Localization.Resources.Pedidos.PedidoOcorrencia.NumeroCarga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.DataOcorrenciaInicial = PropertyEntity({ text: Localization.Resources.Pedidos.PedidoOcorrencia.DataOcorrenciaInicial.getFieldDescription(), getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataOcorrenciaFinal = PropertyEntity({ text: Localization.Resources.Pedidos.PedidoOcorrencia.DataOcorrenciaFinal.getFieldDescription(), getType: typesKnockout.date, val: ko.observable(""), def: "" });

    this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.PedidoOcorrencia.TipoOcorrencia.getFieldDescription(), idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Transportador.getFieldDescription(), idBtnSearch: guid() });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Remetente.getFieldDescription(), idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Filial.getFieldDescription(), idBtnSearch: guid() });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Origem.getFieldDescription(), idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Destino.getFieldDescription(), idBtnSearch: guid() });
    this.NumeroNotaFiscal = PropertyEntity({ text: Localization.Resources.Pedidos.PedidoOcorrencia.NumeroNF.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.SituacaoEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.PedidoOcorrencia.SituacaoEntrega.getFieldDescription(), options: EnumSituacaoEntrega.obterOpcoesPesquisa(), val: ko.observable(""), def: "" });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPedidoOcorrencia.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaPedidoOcorrencia.Visible.visibleFade() === true) {
                _pesquisaPedidoOcorrencia.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaPedidoOcorrencia.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: Localization.Resources.Gerais.Geral.GerarPDF });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: Localization.Resources.Gerais.Geral.GerarPlanilhaExcel });
}

//*********EVENTOS**********

function LoadPedidoOcorrencia() {
    _pesquisaPedidoOcorrencia = new PesquisaPedidoOcorrencia();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPedidoOcorrencia = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PedidoOcorrencia/Pesquisa", _pesquisaPedidoOcorrencia, null, null, 10);
    _gridPedidoOcorrencia.SetPermitirEdicaoColunas(true);

    _relatorioPedidoOcorrencia = new RelatorioGlobal("Relatorios/PedidoOcorrencia/BuscarDadosRelatorio", _gridPedidoOcorrencia, function () {
        _relatorioPedidoOcorrencia.loadRelatorio(function () {
            KoBindings(_pesquisaPedidoOcorrencia, "knockoutPesquisaPedidoOcorrencia", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPedidoOcorrencia", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPedidoOcorrencia", false);

            new BuscarTipoOcorrencia(_pesquisaPedidoOcorrencia.TipoOcorrencia, null, null, null, null, null, null, true, null, null, null, null, true);
            new BuscarTransportadores(_pesquisaPedidoOcorrencia.Transportador);
            new BuscarClientes(_pesquisaPedidoOcorrencia.Remetente);
            new BuscarFilial(_pesquisaPedidoOcorrencia.Filial);
            new BuscarLocalidades(_pesquisaPedidoOcorrencia.Origem);
            new BuscarLocalidades(_pesquisaPedidoOcorrencia.Destino);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
                _pesquisaPedidoOcorrencia.Transportador.text("Empresa/Filial:");

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPedidoOcorrencia);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPedidoOcorrencia.gerarRelatorio("Relatorios/PedidoOcorrencia/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPedidoOcorrencia.gerarRelatorio("Relatorios/PedidoOcorrencia/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}