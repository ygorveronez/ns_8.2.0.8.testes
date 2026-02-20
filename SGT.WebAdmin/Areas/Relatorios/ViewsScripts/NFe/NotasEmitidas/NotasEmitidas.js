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
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Atividade.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/NaturezaOperacao.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CFOP.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumStatusNFe.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoEmissaoNFe.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoNota.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioNotasEmitidas, _gridNotasEmitidas, _pesquisaNotasEmitidas, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _statusNFe = [
    { text: "Todos", value: 0 },
    { text: "Em Digitação", value: EnumStatusNFe.Emitido },
    { text: "Inutilizado", value: EnumStatusNFe.Inutilizado },
    { text: "Cancelado", value: EnumStatusNFe.Cancelado },
    { text: "Autorizado", value: EnumStatusNFe.Autorizado },
    { text: "Denegado", value: EnumStatusNFe.Denegado },
    { text: "Rejeitado", value: EnumStatusNFe.Rejeitado }
];

var _tipoEmissao = [
    { text: "Todos", value: -1 },
    { text: "Entrada", value: EnumTipoEmissaoNFe.Entrada },
    { text: "Saída", value: EnumTipoEmissaoNFe.Saida }
];

var _tipoNotaImportada = [
    { text: "Pelo Sistema", value: 1 },
    { text: "Todas", value: 2 }
];

var _tipoNota = [
    { text: "Todos", value: EnumTipoNota.Todos },
    { text: "NF-e", value: EnumTipoNota.NFe },
    { text: "NFS-e", value: EnumTipoNota.NFSe }
];

var PesquisaNotasEmitidas = function () {
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final: ", getType: typesKnockout.int });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa: ", idBtnSearch: guid(), visible: true });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Serie = PropertyEntity({ text: "Série: ", getType: typesKnockout.int });
    this.NaturezaOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Natureza da Operação: ", idBtnSearch: guid(), visible: true });
    this.NotaImportada = PropertyEntity({ val: ko.observable(1), options: _tipoNotaImportada, def: 1, text: "Forma Emissão: " });
    this.Status = PropertyEntity({ val: ko.observable(0), options: _statusNFe, def: 0, text: "Status: " });
    this.Chave = PropertyEntity({ text: "Chave: ", getType: typesKnockout.string, maxlength: 44 });

    this.Atividade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Atividade: ", idBtnSearch: guid(), visible: true });
    this.DataProcessamento = PropertyEntity({ text: "Data Processamento: ", getType: typesKnockout.date });
    this.DataSaida = PropertyEntity({ text: "Data Saída: ", getType: typesKnockout.date });
    this.TipoEmissao = PropertyEntity({ val: ko.observable(-1), options: _tipoEmissao, def: -1, text: "Tipo Emissão: " });
    this.TipoDocumento = PropertyEntity({ val: ko.observable(EnumTipoNota.Todos), options: _tipoNota, def: EnumTipoNota.Todos, text: "Tipo Documento: " });

    this.Usuario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });
    this.CFOP = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "CFOP:", idBtnSearch: guid() });

    this.ExibirItens = PropertyEntity({ text: "Exibir os Itens das Notas na impressão?", val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridNotasEmitidas.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaNotasEmitidas.Visible.visibleFade()) {
                _pesquisaNotasEmitidas.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaNotasEmitidas.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });

    this.DownloadLoteXML = PropertyEntity({
        eventClick: function (e) {
            DownloadLoteXMLClick();
        }, type: types.event, text: "Baixar Lote de XML", idFade: guid(), icon: "fa fa-download"
    });
};

//*******EVENTOS*******


function loadRelatorioNotasEmitidas() {

    _pesquisaNotasEmitidas = new PesquisaNotasEmitidas();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridNotasEmitidas = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/NotasEmitidas/Pesquisa", _pesquisaNotasEmitidas, null, null, 10);
    _gridNotasEmitidas.SetPermitirEdicaoColunas(true);

    _relatorioNotasEmitidas = new RelatorioGlobal("Relatorios/NotasEmitidas/BuscarDadosRelatorio", _gridNotasEmitidas, function () {
        _relatorioNotasEmitidas.loadRelatorio(function () {
            KoBindings(_pesquisaNotasEmitidas, "knockoutPesquisaNotasEmitidas");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaNotasEmitidas");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaNotasEmitidas");

            new BuscarAtividades(_pesquisaNotasEmitidas.Atividade);
            new BuscarNaturezasOperacoesNotaFiscal(_pesquisaNotasEmitidas.NaturezaOperacao);
            new BuscarClientes(_pesquisaNotasEmitidas.Pessoa);
            new BuscarFuncionario(_pesquisaNotasEmitidas.Usuario);
            new BuscarCFOPNotaFiscal(_pesquisaNotasEmitidas.CFOP);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaNotasEmitidas);

}

function DownloadLoteXMLClick() {
    var data = RetornarObjetoPesquisa(_pesquisaNotasEmitidas);
    executarDownload("Relatorios/NotasEmitidas/DownloadLoteXML", data);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioNotasEmitidas.gerarRelatorio("Relatorios/NotasEmitidas/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioNotasEmitidas.gerarRelatorio("Relatorios/NotasEmitidas/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
