/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroCarregamento.js" />
/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../js/app.config.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Estado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/RotaFrete.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridDirecionamentoOperador, _pesquisaDirecionamentoOperador, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioDirecionamentoOperador;

var PesquisaDirecionamentoOperador = function () {
    var dataAtual = moment().format("DD/MM/YYYY");

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.DataInicial = PropertyEntity({ text: "Data Inicial:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.NumeroCarga = PropertyEntity({ text: "Número da Carga:", visible: ko.observable(true) });

    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ text: "Transportador:", issue: 69, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), issue: 63, visible: ko.observable(true) });
    this.CentroCarregamento = PropertyEntity({ text: "Centro de Carregamento:", issue: 320, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Operador = PropertyEntity({ text: "Operador:", issue: 210, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ text: "Tipo de Carga:", issue: 53, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.ModeloVeiculo = PropertyEntity({ text: "Modelo do Veículo:", issue: 44, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Rota = PropertyEntity({ text: "Rota:", issue: 830, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ text: "Filial:", issue: 70, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ text: "Veículo:", issue: 143, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridDirecionamentoOperador.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaDirecionamentoOperador.Visible.visibleFade()) {
                _pesquisaDirecionamentoOperador.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaDirecionamentoOperador.Visible.visibleFade(true);
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

function loadRelatorioDirecionamentoOperador() {
    _pesquisaDirecionamentoOperador = new PesquisaDirecionamentoOperador();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridDirecionamentoOperador = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/DirecionamentoOperador/Pesquisa", _pesquisaDirecionamentoOperador);

    _gridDirecionamentoOperador.SetPermitirEdicaoColunas(true);
    _gridDirecionamentoOperador.SetQuantidadeLinhasPorPagina(10);

    _relatorioDirecionamentoOperador = new RelatorioGlobal("Relatorios/DirecionamentoOperador/BuscarDadosRelatorio", _gridDirecionamentoOperador, function () {
        _relatorioDirecionamentoOperador.loadRelatorio(function () {
            KoBindings(_pesquisaDirecionamentoOperador, "knockoutPesquisaDirecionamentoOperador", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaDirecionamentoOperador", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCTe", false);

            new BuscarClientes(_pesquisaDirecionamentoOperador.Destinatario);
            new BuscarTransportadores(_pesquisaDirecionamentoOperador.Transportador);
            new BuscarOperador(_pesquisaDirecionamentoOperador.Operador);
            new BuscarCentrosCarregamento(_pesquisaDirecionamentoOperador.CentroCarregamento);
            new BuscarTiposdeCarga(_pesquisaDirecionamentoOperador.TipoCarga);
            new BuscarModelosVeicularesCarga(_pesquisaDirecionamentoOperador.ModeloVeiculo);
            new BuscarRotasFrete(_pesquisaDirecionamentoOperador.Rota);
            new BuscarFilial(_pesquisaDirecionamentoOperador.Filial);
            new BuscarVeiculos(_pesquisaDirecionamentoOperador.Veiculo);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaDirecionamentoOperador);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioDirecionamentoOperador.gerarRelatorio("Relatorios/DirecionamentoOperador/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioDirecionamentoOperador.gerarRelatorio("Relatorios/DirecionamentoOperador/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}