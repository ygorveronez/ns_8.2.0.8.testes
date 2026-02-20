/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../js/Global/Globais.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridConfiguracaoCentroResultado, _pesquisaConfiguracaoCentroResultado, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioConfiguracaoCentroResultado;

var PesquisaConfiguracaoCentroResultado = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid() });
    this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), text: "Tipo da Ocorrência:", idBtnSearch: guid(), issue: 410 });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Produto:", idBtnSearch: guid() });
    this.RotaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Rota de Frete:", idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.CentroResultadoEscrituracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de resultado Escrituração:", idBtnSearch: guid()});
    this.CentroResultadoContabilizacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de resultado Contabilização:", idBtnSearch: guid() });
    this.CentroResultadoICMS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de resultado ICMS:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.CentroResultadoPedidoObrigatorio) });
    this.CentroResultadoPIS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de resultado PIS:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.CentroResultadoPedidoObrigatorio) });
    this.CentroResultadoCOFINS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de resultado COFINS:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.CentroResultadoPedidoObrigatorio) });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridConfiguracaoCentroResultado.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioConfiguracaoCentroResultado = PropertyEntity({ eventClick: GerarRelatorioConfiguracaoCentroResultadoClick, type: types.event, text: "Gerar Relatório Configuracação Centro Resultado" });
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadConfiguracaoCentroResultado() {
    _pesquisaConfiguracaoCentroResultado = new PesquisaConfiguracaoCentroResultado();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridConfiguracaoCentroResultado = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ConfiguracaoCentroResultado/Pesquisa", _pesquisaConfiguracaoCentroResultado);

    _gridConfiguracaoCentroResultado.SetPermitirEdicaoColunas(true);
    _gridConfiguracaoCentroResultado.SetQuantidadeLinhasPorPagina(10);

    _relatorioConfiguracaoCentroResultado = new RelatorioGlobal("Relatorios/ConfiguracaoCentroResultado/BuscarDadosRelatorio", _gridConfiguracaoCentroResultado, function () {
        _relatorioConfiguracaoCentroResultado.loadRelatorio(function () {
            KoBindings(_pesquisaConfiguracaoCentroResultado, "knockoutPesquisaConfiguracaoCentroResultado", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaConfiguracaoCentroResultado", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaConfiguracaoCentroResultado", false);

            new BuscarClientes(_pesquisaConfiguracaoCentroResultado.Remetente);
            new BuscarClientes(_pesquisaConfiguracaoCentroResultado.Destinatario);
            new BuscarClientes(_pesquisaConfiguracaoCentroResultado.Tomador);
            new BuscarTransportadores(_pesquisaConfiguracaoCentroResultado.Transportador);
            new BuscarTiposOperacao(_pesquisaConfiguracaoCentroResultado.TipoOperacao);
            new BuscarTipoOcorrencia(_pesquisaConfiguracaoCentroResultado.TipoOcorrencia);
            new BuscarRotasFrete(_pesquisaConfiguracaoCentroResultado.RotaFrete);
            new BuscarGruposProdutos(_pesquisaConfiguracaoCentroResultado.GrupoProduto);
            new BuscarCentroResultado(_pesquisaConfiguracaoCentroResultado.CentroResultadoContabilizacao);
            new BuscarCentroResultado(_pesquisaConfiguracaoCentroResultado.CentroResultadoICMS);
            new BuscarCentroResultado(_pesquisaConfiguracaoCentroResultado.CentroResultadoPIS);
            new BuscarCentroResultado(_pesquisaConfiguracaoCentroResultado.CentroResultadoCOFINS);
            new BuscarCentroResultado(_pesquisaConfiguracaoCentroResultado.CentroResultadoEscrituracao);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaConfiguracaoCentroResultado);

}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioConfiguracaoCentroResultado.gerarRelatorio("Relatorios/ConfiguracaoCentroResultado/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioConfiguracaoCentroResultado.gerarRelatorio("Relatorios/ConfiguracaoCentroResultado/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function GerarRelatorioConfiguracaoCentroResultadoClick(e, sender) {
    executarDownload("Relatorios/ConfiguracaoCentroResultado/GerarRelatorioConfiguracaoCentroResultado", RetornarObjetoPesquisa(_pesquisaConfiguracaoCentroResultado));
}

//*******MÉTODOS*******