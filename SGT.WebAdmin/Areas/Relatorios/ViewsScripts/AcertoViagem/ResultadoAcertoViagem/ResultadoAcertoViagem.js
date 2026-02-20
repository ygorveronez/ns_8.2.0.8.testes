/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/knockout-3.1.0.js" />
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
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/SegmentoVeiculo.js" />
//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioResultadoAcertoViagem, _gridResultadoAcertoViagem, _pesquisaResultadoAcertoViagem, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaResultadoAcertoViagem = function () {
    this.DataInicial = PropertyEntity({ text: "Período final do acerto: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });

    this.Motorista = PropertyEntity({ text: "Motorista:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.GrupoPessoa = PropertyEntity({ text: "Grupo de Pessoa:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.ModeloVeiculo = PropertyEntity({ text: "Modelo Veícular:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.VeiculoTracao = PropertyEntity({ text: "Veículo de Tração:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.VeiculoReboque = PropertyEntity({ text: "Veículo de Reboque:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.SegmentoVeiculo = PropertyEntity({ text: "Segmento do Veículo:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridResultadoAcertoViagem.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioResultadoAcertoViagem() {

    _pesquisaResultadoAcertoViagem = new PesquisaResultadoAcertoViagem();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridResultadoAcertoViagem = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ResultadoAcertoViagem/Pesquisa", _pesquisaResultadoAcertoViagem, null, null, 10);
    _gridResultadoAcertoViagem.SetPermitirEdicaoColunas(true);

    _relatorioResultadoAcertoViagem = new RelatorioGlobal("Relatorios/ResultadoAcertoViagem/BuscarDadosRelatorio", _gridResultadoAcertoViagem, function () {
        _relatorioResultadoAcertoViagem.loadRelatorio(function () {
            KoBindings(_pesquisaResultadoAcertoViagem, "knockoutPesquisaResultadoAcertoViagem");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaResultadoAcertoViagem");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaResultadoAcertoViagem");
            new BuscarMotoristas(_pesquisaResultadoAcertoViagem.Motorista);
            new BuscarGruposPessoas(_pesquisaResultadoAcertoViagem.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Clientes);
            new BuscarModelosVeicularesCarga(_pesquisaResultadoAcertoViagem.ModeloVeiculo);
            new BuscarVeiculos(_pesquisaResultadoAcertoViagem.VeiculoTracao); 
            new BuscarSegmentoVeiculo(_pesquisaResultadoAcertoViagem.SegmentoVeiculo);
            new BuscarReboques(_pesquisaResultadoAcertoViagem.VeiculoReboque);
            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaResultadoAcertoViagem);

}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioResultadoAcertoViagem.gerarRelatorio("Relatorios/ResultadoAcertoViagem/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioResultadoAcertoViagem.gerarRelatorio("Relatorios/ResultadoAcertoViagem/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
