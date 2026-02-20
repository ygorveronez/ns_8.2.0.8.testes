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
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPosicaoCTe, _pesquisaPosicaoCTe, _CRUDRelatorio, _CRUDFiltrosRelatorio, _relatorioPosicaoCTe;

var _statusCTe = [{ value: '0', text: 'Todos' },
                            { value: 'A', text: 'Autorizados' },
                            { value: 'C', text: 'Cancelados' },
                            { value: 'Z', text: 'Anulados' }]

var PesquisaPosicaoCTe = function () {
    this.DataPosicao = PropertyEntity({ text: "*Data Posição: ", getType: typesKnockout.date, required: true });
    this.Transportadora = PropertyEntity({ text: "Transportador:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.StatusCTe = PropertyEntity({ val: ko.observable(""), options: _statusCTe, def: "", text: "Status dos CT-es: " });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.SomenteCTesFaturados = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Somente CT-es faturados?" })
    this.SomenteAvon = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Somente totalizador dos CT-es da Avon?" })
    this.SomenteDiaInformado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Somente os CT-es do Dia Informado?" })

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            if (ValidarCamposObrigatorios(_pesquisaPosicaoCTe)) {
                _gridPosicaoCTe.CarregarGrid();
            } else {
                exibirMensagem("atencao", "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
            }
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioPosicaoCTe() {

    _pesquisaPosicaoCTe = new PesquisaPosicaoCTe();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPosicaoCTe = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PosicaoCTe/Pesquisa", _pesquisaPosicaoCTe);

    _gridPosicaoCTe.SetPermitirEdicaoColunas(true);
    _gridPosicaoCTe.SetQuantidadeLinhasPorPagina(10);

    _relatorioPosicaoCTe = new RelatorioGlobal("Relatorios/PosicaoCTe/BuscarDadosRelatorio", _gridPosicaoCTe, function () {
        _relatorioPosicaoCTe.loadRelatorio(function () {
            KoBindings(_pesquisaPosicaoCTe, "knockoutPesquisaPosicaoCTe", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPosicaoCTe", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaFaturamento", false);

            new BuscarTransportadores(_pesquisaPosicaoCTe.Transportadora);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPosicaoCTe);
}

function GerarRelatorioPDFClick(e, sender) {
    if (ValidarCamposObrigatorios(_pesquisaPosicaoCTe)) {
        _relatorioPosicaoCTe.gerarRelatorio("Relatorios/PosicaoCTe/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
    } else {
        exibirMensagem("atencao", "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
    }
}

function GerarRelatorioExcelClick(e, sender) {
    if (ValidarCamposObrigatorios(_pesquisaPosicaoCTe)) {
        _relatorioPosicaoCTe.gerarRelatorio("Relatorios/PosicaoCTe/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
    } else {
        exibirMensagem("atencao", "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
    }
}