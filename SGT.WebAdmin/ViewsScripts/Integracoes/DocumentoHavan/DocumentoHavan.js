/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _documentoHavan;
var _CRUDDocumentoHavan;

var DocumentoHavan = function () {
    this.DataVigencia = PropertyEntity({ text: "*Data Vigência:", val: ko.observable(Global.DataAtual()), getType: typesKnockout.date, def: Global.DataAtual(), required: true });
};

var CRUDDocumentoHavan = function () {
    this.Rota = PropertyEntity({ eventClick: gerarArquivoRotaClick, type: types.event, text: "Gerar Arquivo Rota", visible: ko.observable(true), enable: ko.observable(true) });
    this.Pedagio = PropertyEntity({ eventClick: gerarArquivoPedagioClick, type: types.event, text: "Gerar Arquivo Pedágio", visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorFreteCargaModeloVeiculo = PropertyEntity({ eventClick: gerarValorFreteCargaModeloVeiculoClick, type: types.event, text: "Gerar Excel Valor Frete Carga por Modelo Veicular" });
    this.TabelaPagamentoTerceiros = PropertyEntity({ eventClick: gerarExcelTabelaPagamentoTerceirosClick, type: types.event, text: "Gerar Excel Tabela Pagamento de Terceiros" });
};

//*******EVENTOS*******

function loadDocumentoHavan() {
    _documentoHavan = new DocumentoHavan();
    KoBindings(_documentoHavan, "knockoutDocumentoHavan");

    _CRUDDocumentoHavan = new CRUDDocumentoHavan();
    KoBindings(_CRUDDocumentoHavan, "knockoutCRUDDocumentoHavan");
}

function gerarArquivoRotaClick() {
    if (!validarCamposObrigatorios())
        return camposObrigatorios();

    executarDownload("DocumentoHavan/GerarArquivoRota", obterFiltros());
}

function gerarArquivoPedagioClick() {
    if (!validarCamposObrigatorios())
        return camposObrigatorios();

    executarDownload("DocumentoHavan/GerarArquivoPedagio", obterFiltros());
}

function gerarValorFreteCargaModeloVeiculoClick() {
    if (!validarCamposObrigatorios())
        return camposObrigatorios();

    executarDownload("DocumentoHavan/GerarExcelValorFreteCargaModeloVeicular", obterFiltros());
}

function gerarExcelTabelaPagamentoTerceirosClick() {
    if (!validarCamposObrigatorios())
        return camposObrigatorios();

    executarDownload("DocumentoHavan/GerarExcelTabelaPagamentoTerceiros", obterFiltros());
}

//*******MÉTODOS*******

function obterFiltros() {
    return { DataVigencia: _documentoHavan.DataVigencia.val() };
}

function validarCamposObrigatorios() {
    return ValidarCamposObrigatorios(_documentoHavan);
}

function camposObrigatorios() {
    return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios");
}