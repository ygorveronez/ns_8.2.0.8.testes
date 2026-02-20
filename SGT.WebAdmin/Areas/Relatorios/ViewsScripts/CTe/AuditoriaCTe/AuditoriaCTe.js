/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />

//********MAPEAMENTO KNOCKOUT********

var _relatorioAuditoriaCTe, _gridAuditoriaCTe, _pesquisaAuditoriaCTe, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _situacaoCTe = [
    { text: "Pendente", value: "P" },
    { text: "Enviado", value: "E" },
    { text: "Rejeitado", value: "R" },
    { text: "Autorizado", value: "A" },
    { text: "Cancelado", value: "C" },
    { text: "Inutilizado", value: "I" },
    { text: "Denegado", value: "D" },
    { text: "Em Digitação", value: "S" },
    { text: "Em Cancelamento", value: "K" },
    { text: "Em Inutilização", value: "L" },
    { text: "Anulado Gerencialmente", value: "Z" },
    { text: "Aguardando Assinatura", value: "X" },
    { text: "Aguardando Assinatura Cancelamento", value: "V" },
    { text: "Aguardando Assinatura Inutilização", value: "B" },
    { text: "Aguardando Emissão e-mail", value: "M" },
    { text: "Contingência FSDA", value: "F" },
    { text: "Contingência EPEC", value: "Q" },
    { text: "Aguardando Finalizar Carga Integração", value: "Y" },
];

var PesquisaAuditoriaCTe = function () {
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report })

    this.DataEmissaoInicial = PropertyEntity({ text: "Data de emissão:", getType: typesKnockout.date });
    this.DataEmissaoFinal = PropertyEntity({ text: "Até:", getType: typesKnockout.date });
    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;
    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;

    this.NumeroDocumentoInicial = PropertyEntity({ text: "Documento inicial:", getType: typesKnockout.int });
    this.NumeroDocumentoFinal = PropertyEntity({ text: "Até:", getType: typesKnockout.int });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), issue: 195});
    this.Tomador = PropertyEntity({ text: "Cliente Tomador:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.ModeloDocumento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Mod. Documento:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas do Tomador:", issue: 58, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: _situacaoCTe, text: "Situação do CT-e:", issue: 120, visible: ko.observable(true) });
    this.Filial = PropertyEntity({ text: "Filial:", issue: 70, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(false) });
    this.Empresa = PropertyEntity({ text: "Empresa:", issue: 69, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(false) });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridAuditoriaCTe.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel" });
}

//*********EVENTOS**********

function LoadAuditoriaCTe() {
    _pesquisaAuditoriaCTe = new PesquisaAuditoriaCTe();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridAuditoriaCTe = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/AuditoriaCTe/Pesquisa", _pesquisaAuditoriaCTe, null, null, 10);
    _gridAuditoriaCTe.SetPermitirEdicaoColunas(true);

    _relatorioAuditoriaCTe = new RelatorioGlobal("Relatorios/AuditoriaCTe/BuscarDadosRelatorio", _gridAuditoriaCTe, function () {
        _relatorioAuditoriaCTe.loadRelatorio(function () {
            KoBindings(_pesquisaAuditoriaCTe, "knockoutPesquisaAuditoriaCTe", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaAuditoriaCTe", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaAuditoriaCTe", false);

            new BuscarClientes(_pesquisaAuditoriaCTe.Tomador);
            new BuscarCargas(_pesquisaAuditoriaCTe.Carga);
            new BuscarGruposPessoas(_pesquisaAuditoriaCTe.GrupoPessoas);
            new BuscarFilial(_pesquisaAuditoriaCTe.Filial);
            new BuscarModeloDocumentoFiscal(_pesquisaAuditoriaCTe.ModeloDocumento, null, null, null, null, true);
            new BuscarTransportadores(_pesquisaAuditoriaCTe.Empresa, null, null, null, null, null, null, true);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaAuditoriaCTe);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _pesquisaAuditoriaCTe.Empresa.visible(true);
        _pesquisaAuditoriaCTe.Filial.visible(false);
    }
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador)
    {
        _pesquisaAuditoriaCTe.Filial.visible(true);
        _pesquisaAuditoriaCTe.Empresa.visible(false);
    }
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioAuditoriaCTe.gerarRelatorio("Relatorios/AuditoriaCTe/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioAuditoriaCTe.gerarRelatorio("Relatorios/AuditoriaCTe/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}