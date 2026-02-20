/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Empresa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Pedido.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/NotaFiscal.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/knockout-3.1.0.js" />
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
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoInteracaoEntrega.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumStatusViagemControleEntrega.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridConsolidadoEntregas, _pesquisaConsolidadoEntregas, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioConsolidadoEntregas;

var PesquisaConsolidadoEntregas = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.Carga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Transporte:", idBtnSearch: guid() });
    this.DataInicioViagemPrevistaInicial = PropertyEntity({ getType: typesKnockout.date, text: "Data de Faturamento de:" });
    this.DataInicioViagemPrevistaFinal = PropertyEntity({ getType: typesKnockout.date, text: "Data de Faturamento até:" });
    this.DataInicioViagemRealizadaInicial = PropertyEntity({ getType: typesKnockout.date, text: "Data Início Viagem inicial:" });
    this.DataInicioViagemRealizadaFinal = PropertyEntity({ getType: typesKnockout.date, text: "Data Início Viagem final:" });
    this.DataConfirmacaoInicial = PropertyEntity({ getType: typesKnockout.date, text: "Data confirmada Entrega inicial:" });
    this.DataConfirmacaoFinal = PropertyEntity({ getType: typesKnockout.date, text: "Data confirmada Entrega final:" });
    this.Pedido = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pedido:", idBtnSearch: guid() });
    this.NotaFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Nota Fiscal:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Transportador:", issue: 69, idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Placa:", idBtnSearch: guid() });
    this.ClienteOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid() });
    this.CidadeOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cidade Origem:", issue: 766, idBtnSearch: guid() });
    this.ClienteDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid() });
    this.CidadeDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cidade Cliente:", issue: 766, idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Operação:", idBtnSearch: guid() });
    this.TipoInteracaoInicioViagem = PropertyEntity({ options: EnumTipoInteracaoEntrega.obterOpcoesPesquisaConsolidadoEntregas(), text: "Tipo de Interação Início Viagem:"});
    this.TipoInteracaoChegadaViagem = PropertyEntity({ options: EnumTipoInteracaoEntrega.obterOpcoesPesquisaConsolidadoEntregas(), text: "Tipo de Interação Chegada:"});
    this.StatusViagem = PropertyEntity({ options: EnumStatusViagemControleEntrega.obterOpcoesAcompanhamentoCargaPesquisa(), text: Localization.Resources.Relatorios.Relatorio.SituacaoControleEntregas.getFieldDescription() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            if (ValidarFiltrosObrigatoriosNFSe())
                _gridConsolidadoEntregas.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaConsolidadoEntregas.Visible.visibleFade() == true) {
                _pesquisaConsolidadoEntregas.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaConsolidadoEntregas.Visible.visibleFade(true);
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

function LoadConsolidadoEntregas() {
    _pesquisaConsolidadoEntregas = new PesquisaConsolidadoEntregas();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridConsolidadoEntregas = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ConsolidadoEntregas/Pesquisa", _pesquisaConsolidadoEntregas);

    _gridConsolidadoEntregas.SetPermitirEdicaoColunas(true);
    _gridConsolidadoEntregas.SetQuantidadeLinhasPorPagina(20);

    _relatorioConsolidadoEntregas = new RelatorioGlobal("Relatorios/ConsolidadoEntregas/BuscarDadosRelatorio", _gridConsolidadoEntregas, function () {
        _relatorioConsolidadoEntregas.loadRelatorio(function () {
            KoBindings(_pesquisaConsolidadoEntregas, "knockoutPesquisaConsolidadoEntregas", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaConsolidadoEntregas", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaConsolidadoEntregas", false);

            new BuscarCargas(_pesquisaConsolidadoEntregas.Carga);
            new BuscarPedidos(_pesquisaConsolidadoEntregas.Pedido);
            new BuscarXMLNotaFiscal(_pesquisaConsolidadoEntregas.NotaFiscal);
            new BuscarTransportadores(_pesquisaConsolidadoEntregas.Transportador, null, null, true);
            new BuscarMotoristas(_pesquisaConsolidadoEntregas.Motorista);
            new BuscarVeiculos(_pesquisaConsolidadoEntregas.Veiculo);
            new BuscarClientes(_pesquisaConsolidadoEntregas.ClienteDestino);
            new BuscarClientes(_pesquisaConsolidadoEntregas.ClienteOrigem);
            new BuscarLocalidades(_pesquisaConsolidadoEntregas.CidadeOrigem);
            new BuscarLocalidades(_pesquisaConsolidadoEntregas.CidadeDestino);
            new BuscarTiposOperacao(_pesquisaConsolidadoEntregas.TipoOperacao);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaConsolidadoEntregas);
}

function GerarRelatorioPDFClick(e, sender) {
    if (ValidarFiltrosObrigatoriosNFSe())
        _relatorioConsolidadoEntregas.gerarRelatorio("Relatorios/ConsolidadoEntregas/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    if (ValidarFiltrosObrigatoriosNFSe())
        _relatorioConsolidadoEntregas.gerarRelatorio("Relatorios/ConsolidadoEntregas/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function ValidarFiltrosObrigatoriosNFSe() {
    if (!ValidarCamposObrigatorios(_pesquisaConsolidadoEntregas)) {
        exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Preencha os filtros obrigatórios");
        return false;
    }
    else
        return true;
}
