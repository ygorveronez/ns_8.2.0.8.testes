/// <reference path="Bloco.js" />
/// <reference path="Carregamento.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="Carregamentos.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="DirecoesGoogleMaps.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="MontagemCarga.js" />
/// <reference path="Pedido.js" />
/// <reference path="PedidoProduto.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="SimulacaoFrete.js" />

function CalcRadiusDistance(lat1, lon1, lat2, lon2) {
    var RADIUSMILES = 3961,
        RADIUSKILOMETERS = 6373,
        latR1 = deg2rad(lat1),
        lonR1 = deg2rad(lon1),
        latR2 = deg2rad(lat2),
        lonR2 = deg2rad(lon2),
        latDifference = latR2 - latR1,
        lonDifference = lonR2 - lonR1,
        a = Math.pow(Math.sin(latDifference / 2), 2) + Math.cos(latR1) * Math.cos(latR2) * Math.pow(Math.sin(lonDifference / 2), 2),
        c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a)),
        dm = c * RADIUSMILES,
        dk = c * RADIUSKILOMETERS;

    return Math.round(dk * 10) / 10;

    //this.mi = this.round(dm);
    //this.km = this.round(dk);
}

function deg2rad(deg) {
    var rad = deg * Math.PI / 180;
    return rad;
};
