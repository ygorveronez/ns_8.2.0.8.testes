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
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoQuantidadeCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridQuantidade, _pesquisaQuantidade, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioQuantidade;

var PesquisaQuantidade = function () {
    var dataAtual = moment().format("DD/MM/YYYY");

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.DataInicial = PropertyEntity({ text: "Data Inicial:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ text: "Transportador:", issue: 69, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), issue: 63, visible: ko.observable(true) });
    this.CentroDescarregamento = PropertyEntity({ text: "Centro Descarregamento:", issue: 320, type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Operador = PropertyEntity({ text: "Operador:", issue: 210, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ text: "Tipo de Carga:", issue: 53, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.ModeloVeiculo = PropertyEntity({ text: "Modelo do Veículo:", issue: 44, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Rota = PropertyEntity({ text: "Rota:", issue: 320, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCargaJanelaDescarregamento.AguardandoConfirmacaoAgendamento), options: EnumSituacaoCargaJanelaDescarregamento.obterOpcoesRelatorioQuantidadeDescarga(), def: EnumSituacaoCargaJanelaDescarregamento.AguardandoConfirmacaoAgendamento, text: "Situação:", visible: ko.observable(true), getType: typesKnockout.selectMultiple });
    this.Filial = PropertyEntity({ text: "Filial:", issue: 70, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ text: "Veículo:", issue: 143, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridQuantidade.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaQuantidade.Visible.visibleFade() == true) {
                _pesquisaQuantidade.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaQuantidade.Visible.visibleFade(true);
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

function LoadQuantidadeCarga() {
    _pesquisaQuantidade = new PesquisaQuantidade();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

     _gridQuantidade = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/QuantidadeDescarga/Pesquisa", _pesquisaQuantidade, undefined, undefined,
        undefined, null, false, false, undefined,
        undefined, undefined, undefined, undefined, undefined, callbackRowVeiculoMonitoramento);

    _gridQuantidade.SetPermitirEdicaoColunas(true);
    _gridQuantidade.SetQuantidadeLinhasPorPagina(10);

    _relatorioQuantidade = new RelatorioGlobal("Relatorios/QuantidadeDescarga/BuscarDadosRelatorio", _gridQuantidade, function () {
        _relatorioQuantidade.loadRelatorio(function () {
            KoBindings(_pesquisaQuantidade, "knockoutPesquisaQuantidade", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaQuantidade", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaQuantidade", false);

            new BuscarClientes(_pesquisaQuantidade.Destinatario);
            new BuscarTransportadores(_pesquisaQuantidade.Transportador);
            new BuscarOperador(_pesquisaQuantidade.Operador);
            new BuscarCentrosDescarregamento(_pesquisaQuantidade.CentroDescarregamento);
            new BuscarTiposdeCarga(_pesquisaQuantidade.TipoCarga);
            new BuscarModelosVeicularesCarga(_pesquisaQuantidade.ModeloVeiculo);
            new BuscarRotasFrete(_pesquisaQuantidade.Rota);
            new BuscarFilial(_pesquisaQuantidade.Filial);
            new BuscarVeiculos(_pesquisaQuantidade.Veiculo);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaQuantidade);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioQuantidade.gerarRelatorio("Relatorios/QuantidadeDescarga/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioQuantidade.gerarRelatorio("Relatorios/QuantidadeDescarga/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function gerarColunaRastreador(nRow, aData) {
    var indice = _gridQuantidade.GetColumnIndex('Rastreador');
    if (indice == undefined) return;
    var colunaRastreador = $(nRow).find('td').eq(indice);
    if (colunaRastreador) {
        color = "#e74c3c";
        if (aData.Rastreador)
            var color = "#33cc33";

        var icone =
            ' <svg xmlns="http://www.w3.org/2000/svg" x="0px" y="0px" width="20" height="20" viewBox="0 0 172 172" ' +
            ' style=" fill:#000000;"><g fill="none" fill-rule="nonzero" stroke="none" stroke-width="1" stroke-linecap="butt" stroke-linejoin="miter" stroke-miterlimit="10" stroke-dasharray="" stroke-dashoffset="0" font-family="none" font-weight="none" font-size="none" text-anchor="none" style="mix-blend-mode: normal"><path d="M0,172v-172h172v172z" fill="none"></path>' +
            '<g fill="' + color + '"><path d="M86,14.33333c-39.5815,0 -71.66667,32.08517 -71.66667,71.66667c0,39.5815 32.08517,71.66667 71.66667,71.66667c39.5815,0 71.66667,-32.08517 71.66667,-71.66667c0,-39.5815 -32.08517,-71.66667 -71.66667,-71.66667zM78.83333,28.66667h14.33333v57.33333h-14.33333zM86,143.33333c-31.61217,0 -57.33333,-25.72117 -57.33333,-57.33333c0,-24.00833 14.84933,-44.58383 35.83333,-53.11217v15.9315c-12.82833,7.44617 -21.5,21.3065 -21.5,37.18067c0,23.7145 19.2855,43 43,43c23.7145,0 43,-19.2855 43,-43c0,-15.87417 -8.67167,-29.7345 -21.5,-37.18067v-15.9315c20.984,8.52833 35.83333,29.10383 35.83333,53.11217c0,31.61217 -25.72117,57.33333 -57.33333,57.33333z"></path></g></g></svg >';

        var html = '<div>' + icone + '</div>';
        $(colunaRastreador).addClass('rastreador');
        $(colunaRastreador).html(html);

    }
}

function callbackRowVeiculoMonitoramento(nRow, aData) {
    gerarColunaRastreador(nRow, aData);
}