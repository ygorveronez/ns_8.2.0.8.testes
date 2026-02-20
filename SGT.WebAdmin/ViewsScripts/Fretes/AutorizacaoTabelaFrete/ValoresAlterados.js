/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="AutorizacaoTabelaFrete.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridValoresAlterados;
var _gridValoresAlteradosTabelasSeleciona;
var _modalValoresAlteradosSelecionados;
var _relatorioConsultaTabelaFreteCliente;

/*
 * Declaração das Funções de Inicialização
 */

function ModalValoresAlteradosSelecionados() {
    this.Codigos = PropertyEntity({ val: ko.observable("") });
    this.GerarPlanilhaExcel = PropertyEntity({ text: "Gerar Planilha Detalhes", idGrid: guid(), eventClick: gerarPlanilhaExcelSelecionadosClick, visible: ko.observable(true) });
    this.Preview = PropertyEntity({ text: "Preview", idGrid: guid(), eventClick: function () { _gridValoresAlteradosTabelasSeleciona.CarregarGrid() }, visible: ko.observable(true) });
}

function loadGridValoresAlterados() {
    var knoutTabelaFrete = {
        Codigo: _tabelaFrete.Codigo
    };

    _gridValoresAlterados = new GridView("grid-valores-alterados", "AutorizacaoTabelaFrete/PesquisaValoresAlteradosTabelaFrete", knoutTabelaFrete);

    _gridValoresAlterados.SetPermitirEdicaoColunas(true);
    _gridValoresAlterados.SetCallbackEdicaoColunas();
}

function loadGridValoresAlteradosSelecionados() {
    _modalValoresAlteradosSelecionados = new ModalValoresAlteradosSelecionados();
    KoBindings(_modalValoresAlteradosSelecionados, "knockoutTabelaFreteSelecionadasUnicas");

    _gridValoresAlteradosTabelasSeleciona = new GridView("grid-valores-alterados-tabelas", "AutorizacaoTabelaFrete/PesquisaValoresAlteradosTabelaFreteSelecionadas", _modalValoresAlteradosSelecionados);
    _gridValoresAlteradosTabelasSeleciona.SetPermitirEdicaoColunas(true);
    _gridValoresAlteradosTabelasSeleciona.SetCallbackEdicaoColunas();
}

function loadValoresAlterados() {
    loadGridValoresAlterados();
}

/*
 * Declaração das Funções Públicas
 */

function atualizarValoresAlterados() {
    _gridValoresAlterados.CarregarGrid();
}

function carregarValoresAlterados() {
    _relatorioConsultaTabelaFreteCliente = new RelatorioGlobal("AutorizacaoTabelaFrete/BuscarDadosRelatorio", _gridValoresAlterados, function () {
        _relatorioConsultaTabelaFreteCliente.loadRelatorio(function () {
            _gridValoresAlterados.CarregarGrid();
        });
    }, { Codigo: _tabelaFrete.Codigo.val() }, null, null, false);
}

function carregarValoresAlteradosSelecionados() {
    buscarCodigosTabelaFreteSelecionadas(function (codigosTabelaFreteSelecionadas) {
        _modalValoresAlteradosSelecionados.Codigos.val(JSON.stringify(codigosTabelaFreteSelecionadas));

        _relatorioConsultaTabelaFreteCliente = new RelatorioGlobal("AutorizacaoTabelaFrete/BuscarDadosRelatorio", _gridValoresAlteradosTabelasSeleciona, function () {
            _relatorioConsultaTabelaFreteCliente.loadRelatorio(function () {
                _gridValoresAlteradosTabelasSeleciona.CarregarGrid();
                Global.abrirModal("divModalDetalhesTabelasSelecionadaUnica");
            });
        }, { Codigos: _modalValoresAlteradosSelecionados.Codigos.val() }, null, null, false);
    });
}

function controlarExibicaoAbaValoresAlterados(exibirDelegar) {
    if (exibirDelegar)
        $("#liValoresAlterados").show();
    else
        $("#liValoresAlterados").hide();
}

function isSituacaoPermiteExibirAbaValoresAlterados(situacao) {
    return (situacao === EnumSituacaoAlteracaoTabelaFrete.AguardandoAprovacao) || (situacao === EnumSituacaoAlteracaoTabelaFrete.Reprovada);
}

function limparValoresAlterados() {
    controlarExibicaoAbaValoresAlterados(false);

    _gridValoresAlterados.CarregarGrid();
}

function gerarPlanilhaExcelClick() {
    executarDownload("AutorizacaoTabelaFrete/GerarPlanilhaExcel", { Codigo: _tabelaFrete.Codigo.val(), Colunas: JSON.stringify(_gridValoresAlterados.ObterColunasVisiveis()) });
}

function gerarPlanilhaExcelSelecionadosClick() {
    buscarCodigosTabelaFreteSelecionadas(function (codigosTabelaFreteSelecionadas) {
        executarDownloadPost("AutorizacaoTabelaFrete/GerarPlanilhaExcelTodasTabelas", { Codigos: JSON.stringify(codigosTabelaFreteSelecionadas), Colunas: JSON.stringify(_gridValoresAlteradosTabelasSeleciona.ObterColunasVisiveis()) });
    });
}

function buscarCodigosTabelaFreteSelecionadas(callback) {
    if (!_gridValoresAlteradosTabelasSeleciona)
        callback([]);

    var dados = RetornarObjetoPesquisa(_pesquisaTabelaFrete);

    dados.SelecionarTodos = _pesquisaTabelaFrete.SelecionarTodos.val();
    dados.ItensSelecionados = JSON.stringify(_gridTabelaFrete.ObterMultiplosSelecionados());
    dados.ItensNaoSelecionados = JSON.stringify(_gridTabelaFrete.ObterMultiplosNaoSelecionados());

    executarReST("AutorizacaoTabelaFrete/BuscarCodigosTabelaFreteAlteracaoSelecionados", dados, function (retorno) {
        if (retorno.Success)
            callback(retorno.Data);
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}
