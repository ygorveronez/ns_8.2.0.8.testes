/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/PagamentoEletronico.js" />
/// <reference path="../../Consultas/BoletoConfiguracao.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/LayoutEDI.js" />
/// <reference path="../../Consultas/Banco.js" />
/// <reference path="../../Enumeradores/EnumTipoDocumentoPesquisaTitulo.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPagamentoEletronico.js" />
/// <reference path="../../Enumeradores/EnumSituacaoBoletoTitulo.js" />
/// <reference path="../../Enumeradores/EnumTipoContaPagamentoEletronico.js" />
/// <reference path="../../Enumeradores/EnumFinalidadePagamentoEletronico.js" />
/// <reference path="../../Enumeradores/EnumDescricaoUsoEmpresaPagamentoEletronico.js" />
/// <reference path="../../Enumeradores/EnumModalidadePagamentoEletronico.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAutorizacaoPagamentoEletronico.js" />
/// <reference path="../../Enumeradores/EnumTipoServicoPagamentoEletronico.js" />
/// <reference path="../../Enumeradores/EnumFormaLancamentoPagamentoEletronico.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _mensalidade, _gridTitulos;
var _pesquisaNotaFiscalEletronica;
var _gridDANFERelatorio;

var PesquisaNotaFiscalEletronica = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoTitulo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var Mensalidade = function () {
    this.Titulos = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.ListaTitulos = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(""), def: "" });

    //this.GerarPDF = PropertyEntity({ eventClick: GerarPDFClick, type: types.event, text: "Gerar PDF", visible: ko.observable(true) });
    //this.GerarExcel = PropertyEntity({ eventClick: GerarExcelClick, type: types.event, text: "Gerar Excel", visible: ko.observable(true) });

    this.PesquisarTitulos = PropertyEntity({ eventClick: PesquisarTitulosClick, type: types.event, text: "Atualizar", visible: ko.observable(true), enable: ko.observable(true) });
};


//*******EVENTOS*******

function loadMensalidade() {

    _pesquisaNotaFiscalEletronica = new PesquisaNotaFiscalEletronica();
    _pesquisaNotaFiscalEletronica.Codigo.val(0);
    _pesquisaNotaFiscalEletronica.CodigoTitulo.val(0);

    _mensalidade = new Mensalidade();
    KoBindings(_mensalidade, "knockoutMensalidade", false);

    buscarTitulos();
}

function BoletoClick(e, sender) {
    var dados = { Codigo: e.Codigo };
    executarDownload("TituloFinanceiro/DownloadBoleto", dados);
}

function NFeClick(e, sender) {
    _gridDANFERelatorio = new GridView("qualquercoisa", "Relatorios/DANFE/Pesquisa", _pesquisaNotaFiscalEletronica);
    _pesquisaNotaFiscalEletronica.Codigo.val(e.Codigo);
    _pesquisaNotaFiscalEletronica.CodigoTitulo.val(e.Codigo);

    var _relatorioDANFE = new RelatorioGlobal("Relatorios/DANFE/BuscarDadosRelatorio", _gridDANFERelatorio, function () {
        _relatorioDANFE.loadRelatorio(function () {
            _relatorioDANFE.gerarRelatorio("Relatorios/DANFE/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
        });
    }, null, null, _pesquisaNotaFiscalEletronica);

}

function PesquisarTitulosClick(e, sender) {
    _gridTitulos.CarregarGrid();
}

//*******MÉTODOS*******

function buscarTitulos() {

    var boleto = { descricao: "Boleto", id: "clasBoleto", evento: "onclick", metodo: BoletoClick, tamanho: "15", icone: "" };
    var nfe = { descricao: "DANFE NF-e", id: "clasNFe", evento: "onclick", metodo: NFeClick, tamanho: "15", icone: "" };

    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(boleto);
    menuOpcoes.opcoes.push(nfe);

    var configExportacao = {
        url: "Mensalidade/ExportarPesquisa",
        titulo: "Mensalidade"
    };

    _gridTitulos = new GridViewExportacao(_mensalidade.Titulos.idGrid, "Mensalidade/Pesquisa", _mensalidade, menuOpcoes, configExportacao, { column: 0, dir: orderDir.desc });
    _gridTitulos.CarregarGrid();
}