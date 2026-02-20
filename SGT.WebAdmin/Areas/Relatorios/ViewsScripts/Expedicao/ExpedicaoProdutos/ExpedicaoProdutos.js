
//*******MAPEAMENTO KNOUCKOUT*******

var _gridExpedicaoProdutos, _pesquisaExpedicaoProdutos, _CRUDRelatorio, _relatorioExpedicaoProdutos, _CRUDFiltrosRelatorio;

var PesquisaExpedicaoProdutos = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:",issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });

    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador:"), issue: 69, idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:",issue: 70, idBtnSearch: guid(), visible: ko.observable(true) });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Carregamento:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", issue: 52, idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", issue: 52, idBtnSearch: guid() });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:",issue: 16,  idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:",issue: 16,  idBtnSearch: guid() });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Carga:",issue: 53,  idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", issue: 121,  idBtnSearch: guid(), visible: ko.observable(true) });
    this.ModeloVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo de Veículo:", idBtnSearch: guid() });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo do Produto:",issue: 60, idBtnSearch: guid() });
    this.UnidadeDeMedida = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Unidade de Medida:",issue: 88, idBtnSearch: guid() });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto:",issue: 59, idBtnSearch: guid() });
    this.Rota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Rota:", issue: 253, idBtnSearch: guid() });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;


}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridExpedicaoProdutos.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaExpedicaoProdutos.Visible.visibleFade() == true) {
                _pesquisaExpedicaoProdutos.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaExpedicaoProdutos.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function LoadRelatorioExpedicaoProdutos() {

    _pesquisaExpedicaoProdutos = new PesquisaExpedicaoProdutos();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridExpedicaoProdutos = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/ExpedicaoProdutos/Pesquisa", _pesquisaExpedicaoProdutos, null, null, 10, null, null, null, null, 20);
    _gridExpedicaoProdutos.SetPermitirEdicaoColunas(true);

    _relatorioExpedicaoProdutos = new RelatorioGlobal("Relatorios/ExpedicaoProdutos/BuscarDadosRelatorio", _gridExpedicaoProdutos, function () {
        _relatorioExpedicaoProdutos.loadRelatorio(function () {
            KoBindings(_pesquisaExpedicaoProdutos, "knockoutPesquisaExpedicaoProdutos");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaExpedicaoProdutos");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaExpedicaoProdutos");

            new BuscarTransportadores(_pesquisaExpedicaoProdutos.Transportador, null, null, true);
            new BuscarUnidadesMedida(_pesquisaExpedicaoProdutos.UnidadeDeMedida);
            new BuscarTiposdeCarga(_pesquisaExpedicaoProdutos.TipoCarga);
            new BuscarGruposProdutos(_pesquisaExpedicaoProdutos.GrupoProduto);
            new BuscarFilial(_pesquisaExpedicaoProdutos.Filial);
            new BuscarClientes(_pesquisaExpedicaoProdutos.Remetente);
            new BuscarClientes(_pesquisaExpedicaoProdutos.Destinatario);
            new BuscarProdutos(_pesquisaExpedicaoProdutos.Produto);
            new BuscarLocalidades(_pesquisaExpedicaoProdutos.Origem);
            new BuscarLocalidades(_pesquisaExpedicaoProdutos.Destino);
            new BuscarTiposOperacao(_pesquisaExpedicaoProdutos.TipoOperacao);
            new BuscarRotasFrete(_pesquisaExpedicaoProdutos.Rota);

        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaExpedicaoProdutos);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioExpedicaoProdutos.gerarRelatorio("Relatorios/ExpedicaoProdutos/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioExpedicaoProdutos.gerarRelatorio("Relatorios/ExpedicaoProdutos/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
