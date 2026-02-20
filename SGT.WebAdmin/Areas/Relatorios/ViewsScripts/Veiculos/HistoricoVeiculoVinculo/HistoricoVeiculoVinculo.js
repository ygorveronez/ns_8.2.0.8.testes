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
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/SegmentoVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Equipamento.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridVeiculo, _pesquisaVeiculo, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioVeiculo;

var PesquisaVeiculo = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.DataHoraVinculoInicialHistoricoVeiculo = PropertyEntity({ text: "*Data Inicial: ", val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual(), getType: typesKnockout.dateTime, required: ko.observable(true) });
    this.DataHoraVinculoFinalHistoricoVeiculo = PropertyEntity({ text: "*Data Final: ", val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual(), getType: typesKnockout.dateTime, required: ko.observable(true) });
    this.DataInicialVinculoCentroResultado = PropertyEntity({ text: "Data Inicial Vínculo Centro Resultado: ", val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual(), getType: typesKnockout.dateTime });
    this.DataFinalVinculoCentroResultado = PropertyEntity({ text: "Data Final Vínculo Centro Resultado: ", val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual(), getType: typesKnockout.dateTime });

    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Reboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: "Reboque:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Equipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: "Equipamento:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: "Motorista:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.VisualizarVinculosSubRelatorio = PropertyEntity({ text: "Visualizar vínculos em sub-relatório?", getType: typesKnockout.bool, val: ko.observable(false) });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            if (ValidarFiltrosObrigatorios())
                _gridVeiculo.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaVeiculo.Visible.visibleFade()) {
                _pesquisaVeiculo.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaVeiculo.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadVeiculo() {
    _pesquisaVeiculo = new PesquisaVeiculo();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridVeiculo = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/HistoricoVeiculoVinculo/Pesquisa", _pesquisaVeiculo);

    _gridVeiculo.SetPermitirEdicaoColunas(true);
    _gridVeiculo.SetQuantidadeLinhasPorPagina(10);

    _relatorioVeiculo = new RelatorioGlobal("Relatorios/HistoricoVeiculoVinculo/BuscarDadosRelatorio", _gridVeiculo, function () {
        _relatorioVeiculo.loadRelatorio(function () {
            KoBindings(_pesquisaVeiculo, "knockoutPesquisaVeiculo", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaVeiculo", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaVeiculo", false);

            new BuscarVeiculos(_pesquisaVeiculo.Veiculo);
            new BuscarReboques(_pesquisaVeiculo.Reboque);
            new BuscarMotorista(_pesquisaVeiculo.Motorista);
            new BuscarEquipamentos(_pesquisaVeiculo.Equipamento);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaVeiculo);

}

function retornoProprietario(row) {
    _pesquisaVeiculo.Proprietario.codEntity(row.Codigo);
    _pesquisaVeiculo.Proprietario.val(row.Nome);
}

function GerarRelatorioPDFClick(e, sender) {
    if (ValidarFiltrosObrigatorios()) {
        _relatorioVeiculo.gerarRelatorio("Relatorios/HistoricoVeiculoVinculo/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
    }
}

function GerarRelatorioExcelClick(e, sender) {
    if (ValidarFiltrosObrigatorios()) {
        _relatorioVeiculo.gerarRelatorio("Relatorios/HistoricoVeiculoVinculo/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
    }
}

function ValidarFiltrosObrigatorios() {
    var tudoCerto = true;
    var valido = ValidarCamposObrigatorios(_pesquisaVeiculo);

    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, "Filtros Obrigatórios", "Informe os filtros obrigatórios!");
        tudoCerto = false;
    }

    var totalDias = Global.ObterDiasEntreDatas(_pesquisaVeiculo.DataHoraVinculoInicialHistoricoVeiculo.val(), _pesquisaVeiculo.DataHoraVinculoFinalHistoricoVeiculo.val());
    if (_CONFIGURACAO_TMS.QuantidadeMaximaDiasRelatorios > 0 && totalDias > _CONFIGURACAO_TMS.QuantidadeMaximaDiasRelatorios) {
        exibirMensagem(tipoMensagem.atencao, "Datas Inválidas", "A diferença das datas não pode ser maior que " + _CONFIGURACAO_TMS.QuantidadeMaximaDiasRelatorios + " dias.");
        tudoCerto = false;
    }
    else if (totalDias > 60) {
        exibirMensagem(tipoMensagem.atencao, "Datas Inválidas", "A diferença das datas não pode ser maior que 60 dias.");
        tudoCerto = false;
    }

    return tudoCerto;
}