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
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumStatusFaturamentoMensal.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoFaturamento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Servico.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/BoletoConfiguracao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioCobrancasMensais, _gridCobrancasMensais, _pesquisaCobrancasMensais, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _statusCobrancaMensal = [
    { text: "Todos", value: 0 },
    { text: "Documentos Autorizados", value: EnumStatusFaturamentoMensal.DocumentosAutorizados },
    { text: "Finalizado", value: EnumStatusFaturamentoMensal.Finalizado },
    { text: "Gerado Boletos", value: EnumStatusFaturamentoMensal.GeradoBoletos },
    { text: "Gerado Documentos", value: EnumStatusFaturamentoMensal.GeradoDocumentos },
    { text: "Iniciada", value: EnumStatusFaturamentoMensal.Iniciada },
    { text: "Cancelado", value: EnumStatusFaturamentoMensal.Cancelado },
    { text: "Aguardando Envio dos E-mails", value: EnumStatusFaturamentoMensal.AguardandoEnvioEmail },
    { text: "Aguardando Autorização dos Documentos", value: EnumStatusFaturamentoMensal.AguardandoAutorizacaoDocumento },
    { text: "Enviando E-mails", value: EnumStatusFaturamentoMensal.EmGeracaoEnvioEmail },
    { text: "Autorizando Documentos", value: EnumStatusFaturamentoMensal.EmGeracaoAutorizacaoDocumento }
];

var PesquisaCobrancasMensais = function () {
    this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão Inicial: ", getType: typesKnockout.date });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data Emissão Final: ", getType: typesKnockout.date });
    this.DataVencimentoInicial = PropertyEntity({ text: "Data Vencimento Inicial: ", getType: typesKnockout.date });
    this.DataVencimentoFinal = PropertyEntity({ text: "Data Vencimento Final: ", getType: typesKnockout.date });

    this.Status = PropertyEntity({ val: ko.observable(0), options: _statusCobrancaMensal, def: 0, text: "Status: " });
    this.Dia = PropertyEntity({ text: "Dia Fatura: ", getType: typesKnockout.int });

    this.GrupoFaturamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo Faturamento: ", idBtnSearch: guid(), visible: true });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa: ", idBtnSearch: guid(), visible: true });
    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Serviço: ", idBtnSearch: guid(), visible: true });
    this.ConfiguracaoBoleto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Configuração Boleto: ", idBtnSearch: guid(), visible: true });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridCobrancasMensais.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioCobrancasMensais() {

    _pesquisaCobrancasMensais = new PesquisaCobrancasMensais();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridCobrancasMensais = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/CobrancaMensal/Pesquisa", _pesquisaCobrancasMensais, null, null, 10);
    _gridCobrancasMensais.SetPermitirEdicaoColunas(true);

    _relatorioCobrancasMensais = new RelatorioGlobal("Relatorios/CobrancaMensal/BuscarDadosRelatorio", _gridCobrancasMensais, function () {
        _relatorioCobrancasMensais.loadRelatorio(function () {
            KoBindings(_pesquisaCobrancasMensais, "knockoutPesquisaCobrancasMensais");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCobrancasMensais");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCobrancasMensais");

            new BuscarClientes(_pesquisaCobrancasMensais.Pessoa);
            new BuscarGrupoFaturamento(_pesquisaCobrancasMensais.GrupoFaturamento);
            new BuscarServicoTMS(_pesquisaCobrancasMensais.Servico);
            new BuscarBoletoConfiguracao(_pesquisaCobrancasMensais.ConfiguracaoBoleto, function (data) {
                _pesquisaCobrancasMensais.ConfiguracaoBoleto.codEntity(data.Codigo);
                _pesquisaCobrancasMensais.ConfiguracaoBoleto.val(data.DescricaoBanco);
            }, null);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCobrancasMensais);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioCobrancasMensais.gerarRelatorio("Relatorios/CobrancaMensal/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioCobrancasMensais.gerarRelatorio("Relatorios/CobrancaMensal/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}