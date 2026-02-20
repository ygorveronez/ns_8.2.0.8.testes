/// <reference path="FilaCarregamento.js" />
/// <reference path="../../../js/Global/Charts.js" />

// #region  Objetos Globais do Arquivo

var _filaCarregamentoResumo;
var _graficoFilaCarregamentoResumo;

// #endregion  Objetos Globais do Arquivo

// #region Classes

var FilaCarregamentoResumo = function () {
    this.ExibirDados = PropertyEntity({
        eventClick: function (e) {
            if (_filaCarregamentoResumo.ExibirDados.visibleFade())
                _filaCarregamentoResumo.ExibirDados.visibleFade(false);
            else {
                _filaCarregamentoResumo.ExibirDados.visibleFade(true);
                _graficoFilaCarregamentoResumo.render();
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

// #endregion Classes

// #region Funções de Inicialização

function loadFilaCarregamentoResumo() {
    _filaCarregamentoResumo = new FilaCarregamentoResumo();
    KoBindings(_filaCarregamentoResumo, "knockoutFilaCarregamentoResumo");

    loadGraficoFilaCarregamentoResumo();
}

function loadGraficoFilaCarregamentoResumo() {
    const options = {
        type: ChartType.Bar,
        idContainer: "fila-carregamento-resumo-container",
        properties: {
            y: 'Valor',
            yType: ChartPropertyType.int,
            yDecimalPlaces: 0,
            x: 'Descricao',
            xType: ChartPropertyType.string,
            color: 'Cor',
        },
        margin: {
            top: 70,
            right: 50,
            left: 80,
            bottom: 50
        },
        title: "Resumo da Fila de Carregamento",
        yTitle: "Total Na Fila",
        xTitle: "Modelo Veicular",
        fileName: "Resumo da Fila de Carregamento",
        url: "FilaCarregamento/ObterDadosResumo",
        knockoutParams: _pesquisaFilaCarregamentoAuxiliar,
        width: 0,
        height: 200
    };

    _graficoFilaCarregamentoResumo = new Chart(options);
    _graficoFilaCarregamentoResumo.init();
}

// #endregion Funções de Inicialização

// #region Funções Públicas

function recarregarGraficoFilaCarregamentoResumo() {
    if (_graficoFilaCarregamentoResumo) {
        const options = {
            data: null,
            xTitle: (_pesquisaFilaCarregamentoAuxiliar.GrupoModeloVeicular.codEntity() > 0 || _CONFIGURACAO_TMS.ExibirResumoFilaCarregamentoSomentePorModeloVeicularCarga) ? "Modelo Veicular" : "Grupo Modelo Veicular"
        };

        _graficoFilaCarregamentoResumo.updateOptions(options);
        _graficoFilaCarregamentoResumo.search();
    }
}

// #endregion Funções Públicas
