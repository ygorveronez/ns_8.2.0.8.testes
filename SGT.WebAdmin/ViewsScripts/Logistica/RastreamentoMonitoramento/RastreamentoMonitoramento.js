/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/MapaDraw.js" />
/// <reference path="../../../js/Global/MapaGoogle.js" />
/// <reference path="../../../js/Global/Mapa.js"/>
/// <reference path="../../../js/Global/MapaGoogleSearch.js"/>
/// <reference path="../../../js/Global/Charts.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../Tracking/Tracking.lib.js" />
var _mapaMonitoramento;
var _CONFIGURACAO_TMS;
function CarregarDados(arg, configuracaoTMS)
{
    _CONFIGURACAO_TMS = configuracaoTMS;
    loadMapa();
    _mapaMonitoramento.clear();
    TrackingDesenharInformacoesMapa(_mapaMonitoramento, arg.Data);
    TrackingCriarMarkerVeiculo(_mapaMonitoramento, arg.Data.Veiculo, false, 0)
}

function loadMapa() {
    if (!_mapaMonitoramento) {
        var opcoesmapa = new OpcoesMapa(false, false);
        _mapaMonitoramento = new MapaGoogle("map", false, opcoesmapa);
    }
}