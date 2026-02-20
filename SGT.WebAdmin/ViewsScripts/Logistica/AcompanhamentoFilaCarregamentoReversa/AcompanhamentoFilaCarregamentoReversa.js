/// <reference path="../../Consultas/CentroCarregamento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _graficoProximidade;
var _graficoReversa;
var _gridFilaCarregamentoReversa;
var _pesquisaFilaCarregamentoReversa;
var _pesquisaFilaCarregamentoReversaAnterior;

var _enumProximidade = [
    { text: "Todas", value: "" },
    { text: "Não", value: false },
    { text: "Sim", value: true }
];

/*
 * Declaração das Classes
 */

var PesquisaFilaCarregamentoReversa = function () {
    this.CentroCarregamento = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "*CD:", idBtnSearch: guid(), required: true });
    this.Proximidade = PropertyEntity({ options: _enumProximidade, text: "Proximidade: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            if (ValidarCamposObrigatorios(_pesquisaFilaCarregamentoReversa)) {
                $("#fila-carregamento-reversa-container").removeClass("d-none");
                _pesquisaFilaCarregamentoReversa.ExibirFiltros.visibleFade(false);
                $('#wid-id-1').removeClass('jarviswidget-collapsed').children('div').slideDown('fast');
                $('#wid-id-2').removeClass('jarviswidget-collapsed').children('div').slideDown('fast');

                PreencherObjetoKnout(_pesquisaFilaCarregamentoReversaAnterior, { Data: RetornarObjetoPesquisa(_pesquisaFilaCarregamentoReversa) });
                recarregarGridFilaCarregamentoReversa();
                recarregarGraficoProximidade();
                recarregarGraficoReversa();
            }
            else
                exibirMensagemCamposObrigatorio();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAcompanhamentoFilaCarregamentoReversa() {
    _pesquisaFilaCarregamentoReversa = new PesquisaFilaCarregamentoReversa();
    KoBindings(_pesquisaFilaCarregamentoReversa, "knockoutPesquisaFilaCarregamentoReversa", false, _pesquisaFilaCarregamentoReversa.Pesquisar.id);

    _pesquisaFilaCarregamentoReversaAnterior = new PesquisaFilaCarregamentoReversa();

    new BuscarCentrosCarregamento(_pesquisaFilaCarregamentoReversa.CentroCarregamento);

    loadGridFilaCarregamentoReversa();
}

function loadGridFilaCarregamentoReversa() {
    var indiceColunaDataEntradaFila = 7;
    var ordenacaoPadrao = { column: indiceColunaDataEntradaFila, dir: orderDir.asc };
    var totalRegistrosPorPagina = 20;
    var menuOpcoes = null;

    _gridFilaCarregamentoReversa = new GridView("grid-fila-carregamento-reversa", "AcompanhamentoFilaCarregamentoReversa/Pesquisa", _pesquisaFilaCarregamentoReversa, menuOpcoes, ordenacaoPadrao, totalRegistrosPorPagina);
}

/*
 * Declaração das Funções
 */

function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
}

function obterHtmlGraficoSemRegistro() {
    return '<div class="grafico-sem-registro"><span>Nenhum registro encontrado</span></div>';
}

function obterOpcoesGrafico(idGrafico, data) {
    return {
        type: ChartType.Pie,
        idContainer: idGrafico,
        margin: {
            top: 50,
            right: 0,
            left: 0,
            bottom: 50
        },
        title: "",
        data: data,
        width: 0,
        height: 350,
        pieLabels: {
            mainLabel: {
                fontSize: 14
            }
        }
    };
}

function recarregarGraficoProximidade() {
    $("#grafico-proximidade-sem-registro").empty();

    if (_graficoProximidade) {
        _graficoProximidade.destroy();
        $("#grafico-proximidade").hide();
    }

    executarReST("AcompanhamentoFilaCarregamentoReversa/PesquisaGraficoProximidade", RetornarObjetoPesquisa(_pesquisaFilaCarregamentoReversa), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data.length > 0) {
                $("#grafico-proximidade").show();
                _graficoProximidade = new Chart(obterOpcoesGrafico("grafico-proximidade", retorno.Data));
                _graficoProximidade.init();
            }
            else
                $("#grafico-proximidade-sem-registro").append(obterHtmlGraficoSemRegistro());
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function recarregarGraficoReversa() {
    $("#grafico-reversa-sem-registro").empty();

    if (_graficoReversa) {
        _graficoReversa.destroy();
        $("#grafico-reversa").hide();
    }

    executarReST("AcompanhamentoFilaCarregamentoReversa/PesquisaGraficoReversa", RetornarObjetoPesquisa(_pesquisaFilaCarregamentoReversa), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data.length > 0) {
                $("#grafico-reversa").show();
                _graficoReversa = new Chart(obterOpcoesGrafico("grafico-reversa", retorno.Data));
                _graficoReversa.init();
            }
            else
                $("#grafico-reversa-sem-registro").append(obterHtmlGraficoSemRegistro());
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function recarregarGridFilaCarregamentoReversa() {
    _gridFilaCarregamentoReversa.CarregarGrid();
}