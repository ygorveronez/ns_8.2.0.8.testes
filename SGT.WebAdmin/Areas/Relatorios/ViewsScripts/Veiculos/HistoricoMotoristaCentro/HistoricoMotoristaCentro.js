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
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridMotorista, _pesquisaMotorista, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioMotorista;

var PesquisaMotorista = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.DataHoraVinculoInicialHistoricoMotorista = PropertyEntity({ text: "*Data Inicial: ", val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual(), getType: typesKnockout.dateTime, required: ko.observable(true) });
    this.DataHoraVinculoFinalHistoricoMotorista = PropertyEntity({ text: "*Data Final: ", val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual(), getType: typesKnockout.dateTime, required: ko.observable(true) });
    this.DataInicialVinculoCentroResultado = PropertyEntity({ text: "Data Inicial Vínculo Centro Resultado: ", val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual(), getType: typesKnockout.dateTime });
    this.DataFinalVinculoCentroResultado = PropertyEntity({ text: "Data Final Vínculo Centro Resultado: ", val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual(), getType: typesKnockout.dateTime });

    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: "Motorista:", idBtnSearch: guid(), visible: ko.observable(true) });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            if (ValidarFiltrosObrigatorios())
                _gridMotorista.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaMotorista.Visible.visibleFade()) {
                _pesquisaMotorista.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaMotorista.Visible.visibleFade(true);
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

function LoadMotorista() {
    _pesquisaMotorista = new PesquisaMotorista();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridMotorista = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/HistoricoMotoristaCentro/Pesquisa", _pesquisaMotorista);

    _gridMotorista.SetPermitirEdicaoColunas(true);
    _gridMotorista.SetQuantidadeLinhasPorPagina(10);

    _relatorioMotorista = new RelatorioGlobal("Relatorios/HistoricoMotoristaCentro/BuscarDadosRelatorio", _gridMotorista, function () {
        _relatorioMotorista.loadRelatorio(function () {
            KoBindings(_pesquisaMotorista, "knockoutPesquisaMotorista", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaMotorista", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaMotorista", false);

            new BuscarMotorista(_pesquisaMotorista.Motorista);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaMotorista);

}

function retornoProprietario(row) {
    _pesquisaMotorista.Proprietario.codEntity(row.Codigo);
    _pesquisaMotorista.Proprietario.val(row.Nome);
}

function GerarRelatorioPDFClick(e, sender) {
    if (ValidarFiltrosObrigatorios()) {
        _relatorioMotorista.gerarRelatorio("Relatorios/HistoricoMotoristaCentro/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
    }
}

function GerarRelatorioExcelClick(e, sender) {
    if (ValidarFiltrosObrigatorios()) {
        _relatorioMotorista.gerarRelatorio("Relatorios/HistoricoMotoristaCentro/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
    }
}

function ValidarFiltrosObrigatorios() {
    var tudoCerto = true;
    var valido = ValidarCamposObrigatorios(_pesquisaMotorista);

    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, "Filtros Obrigatórios", "Informe os filtros obrigatórios!");
        tudoCerto = false;
    }

    var totalDias = Global.ObterDiasEntreDatas(_pesquisaMotorista.DataHoraVinculoInicialHistoricoMotorista.val(), _pesquisaMotorista.DataHoraVinculoFinalHistoricoMotorista.val());
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