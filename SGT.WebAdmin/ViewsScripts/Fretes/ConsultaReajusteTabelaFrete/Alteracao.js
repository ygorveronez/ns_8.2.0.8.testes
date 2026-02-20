/// <reference path="Autorizacao.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/TabelaFrete.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAjusteTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumEtapaAutorizacaoTabelaFrete.js" />
/// <reference path="AutorizarRegras.js" />
/// <reference path="Anexos.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _alteracoes;
var _gridPreviewAjusteTabelaFrete;
var _relatorioConsultaTabelaFrete;
var _gridValoresAjustesAlteradosSelecionados;
var _modalValoresSelecionados;

function ModalValoresSelecionados() {
    this.Codigos = PropertyEntity({ val: ko.observable("") });
    this.GerarPlanilhaExcel = PropertyEntity({ text: "Gerar Planilha Excel", idGrid: guid(), eventClick: gerarPlanilhaExcelValoresSelecionadosClick, visible: ko.observable(true) });
    this.Preview = PropertyEntity({ text: "Preview", idGrid: guid(), eventClick: function () { _gridValoresAjustesAlteradosSelecionados.CarregarGrid() }, visible: ko.observable(true) });

}

var Alteracoes = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TabelaFrete = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoRegistro = PropertyEntity({ val: ko.observable(EnumTipoRegistroAjusteTabelaFrete.Alterados), def: EnumTipoRegistroAjusteTabelaFrete.Alterados });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPreviewAjusteTabelaFrete.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: guid(), visible: ko.observable(true)
    });
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF", icon: "fa fa-file-pdf-o" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", icon: "fa fa-file-excel-o" });

}
//*******EVENTOS*******


function loadAlteracoes() {
    _alteracoes = new Alteracoes();
    KoBindings(_alteracoes, "knockoutAlteracoes");
}

function loadGridValoresSelecionados() {
    _modalValoresSelecionados = new ModalValoresSelecionados();
    KoBindings(_modalValoresSelecionados, "knockoutTabelaFreteAlteracoesSelecionadas");

    _gridValoresAjustesAlteradosSelecionados = new GridView("grid-valores-selecionados-alterados-tabelas", "ConsultaReajusteTabelaFrete/PesquisaValoresAlteradosTabelaFreteSelecionadas", _modalValoresSelecionados);
    _gridValoresAjustesAlteradosSelecionados.SetPermitirEdicaoColunas(true);
    _gridValoresAjustesAlteradosSelecionados.SetQuantidadeLinhasPorPagina(25);
}

function carregarValoresSelecionados() {
    buscarCodigosAjustesSelecionados(function (codigosTabelaFreteSelecionadas) {
        _modalValoresSelecionados.Codigos.val(JSON.stringify(codigosTabelaFreteSelecionadas));
        _relatorioConsultaTabelaFreteCliente = new RelatorioGlobal("AjusteTabelaFrete/BuscarDadosRelatorioAlteracoesTabelaFrete", _gridValoresAjustesAlteradosSelecionados, function () {
            _relatorioConsultaTabelaFreteCliente.loadRelatorio(function () {
                _gridValoresAjustesAlteradosSelecionados.CarregarGrid();

                _relatorioConsultaTabelaFreteCliente.obterKnoutRelatorio().Report.visibleFade(true);
                _relatorioConsultaTabelaFreteCliente.obterKnoutRelatorio().Report.visible(false);
                _relatorioConsultaTabelaFreteCliente.obterKnoutRelatorio().ExibirSumarios.visible(false);
                Global.abrirModal('divModalExportarDetalhes');

            });
        }, { Codigos: _modalValoresSelecionados.Codigos.val() }, null, _modalValoresSelecionados, false);
    });

}


function GerarRelatorioPDFClick(e, sender) {
    _relatorioConsultaTabelaFrete.gerarRelatorio("AjusteTabelaFrete/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioConsultaTabelaFrete.gerarRelatorio("AjusteTabelaFrete/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function gerarPlanilhaExcelValoresSelecionadosClick() {
    buscarCodigosAjustesSelecionados(function (codigosTabelaFreteSelecionadas) {
        executarDownloadPost("AjusteTabelaFrete/GerarPlanilhaExcelAjustesSelecionados", { Codigos: JSON.stringify(codigosTabelaFreteSelecionadas), Colunas: JSON.stringify(_gridValoresAjustesAlteradosSelecionados.ObterColunasVisiveis()) });
    });
}

//*******MÉTODOS*******
function AtualizarRelatorioAlteracoes(data) {
    if (data.EnumSituacao == EnumSituacaoAjusteTabelaFrete.AgAprovacao || data.EnumSituacao == EnumSituacaoAjusteTabelaFrete.Finalizado || data.EnumSituacao == EnumSituacaoAjusteTabelaFrete.AgIntegracao) {
        $("#liAlteracoes").show();

        _alteracoes.Codigo.val(data.Codigo);
        _alteracoes.TabelaFrete.val(data.TabelaFrete);

        _gridPreviewAjusteTabelaFrete = new GridView(_alteracoes.Preview.idGrid, "AjusteTabelaFrete/Pesquisa", _alteracoes);
        _gridPreviewAjusteTabelaFrete.SetQuantidadeLinhasPorPagina(15);
        _gridPreviewAjusteTabelaFrete.SetPermitirEdicaoColunas(true);

        _relatorioConsultaTabelaFrete = new RelatorioGlobal("AjusteTabelaFrete/BuscarDadosRelatorio", _gridPreviewAjusteTabelaFrete, function () {
            _relatorioConsultaTabelaFrete.loadRelatorio(function () {
                //if (_ajusteTabelaFrete.DataVigencia.codEntity() > 0)
                _gridPreviewAjusteTabelaFrete.CarregarGrid();

                _relatorioConsultaTabelaFrete.obterKnoutRelatorio().Report.visibleFade(true);
                _relatorioConsultaTabelaFrete.obterKnoutRelatorio().Report.visible(false);
                _relatorioConsultaTabelaFrete.obterKnoutRelatorio().ExibirSumarios.visible(false);
            });

        }, { TabelaFrete: _alteracoes.TabelaFrete.val() }, null, _alteracoes, false);
    } else {
        $("#liAlteracoes").hide();
    }
}

function buscarCodigosAjustesSelecionados(callback) {
    if (!_gridValoresAjustesAlteradosSelecionados)
        callback([]);

    var dados = RetornarObjetoPesquisa(_pesquisaReajustes);

    dados.SelecionarTodos = _pesquisaReajustes.SelecionarTodos.val();
    dados.ItensSelecionados = JSON.stringify(_gridAvaria.ObterMultiplosSelecionados());
    dados.ItensNaoSelecionados = JSON.stringify(_gridAvaria.ObterMultiplosNaoSelecionados());

    executarReST("ConsultaReajusteTabelaFrete/BuscarCodigosTabelaFreteAjustesSelecionados", dados, function (retorno) {
        if (retorno.Success)
            callback(retorno.Data);
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}
