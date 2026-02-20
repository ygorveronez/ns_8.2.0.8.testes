var _mapaPosicaoDaFrota;
var _pesquisaPosicaoDaFrota;

/*
 * Declaração das Classes
 */
var PesquisaPosicaoDaFrota = function () {

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _pesquisaPosicaoDaFrota.ExibirFiltros.visibleFade(false);
            carregarPosicaoDaFrotaMapa()
           
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    
    this.PlacaVeiculo = PropertyEntity({ text: "Placa: ", col: 12 });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}


/*
 * Declaração das Funções de Inicialização
 */

function loadPesquisaPosicaoDaFrota() {
    _pesquisaPosicaoDaFrota = new PesquisaPosicaoDaFrota();
    KoBindings(_pesquisaPosicaoDaFrota, "knockoutPesquisaPosicaoDaFrotaMapa", false, _pesquisaPosicaoDaFrota.Pesquisar.id);
}

function loadMapa() {
   // if (!_mapaPosicaoDaFrota) {
        var opcoesmapa = new OpcoesMapa(false, false);
        _mapaPosicaoDaFrota = new MapaGoogle("map", false, opcoesmapa);
    //}
    carregarPosicaoDaFrotaMapa();
}

function loadPosicaoDaFrotaMapa() {
    loadPesquisaPosicaoDaFrota();
    loadMapa();
}

/*
 * Declaração das Funções
 */
function criarMakersVeiculo(data) {
    for (var i = 0; i < data.length; i++) {

        var infoVeiculo = data[i];

        var marker = new ShapeMarker();
        marker.setPosition(infoVeiculo.Latitude, infoVeiculo.Longitude);
        marker.icon = _mapaPosicaoDaFrota.draw.icons.truck(infoVeiculo.StatusCor);
        marker.title =
            '<div>Veículo: ' + infoVeiculo.PlacaVeiculo + '</div>' +
            '<div>Data Posição: ' + infoVeiculo.DataPosicao + '<div>' +
            '<div>Posição: ' + infoVeiculo.Descricao + ' (' + infoVeiculo.Latitude + ',' + infoVeiculo.Longitude + ')' + '<div>' +
            '<div>Status: ' + infoVeiculo.Status + '</div>';
        _mapaPosicaoDaFrota.draw.addShape(marker);

    }

    _mapaPosicaoDaFrota.draw.addMarkerCluster();
    _mapaPosicaoDaFrota.draw.centerShapes();
    _mapaPosicaoDaFrota.direction.setZoom(5);
}

function carregarPosicaoDaFrotaMapa() {
    var data = { PlacaVeiculo: _pesquisaPosicaoDaFrota.PlacaVeiculo.val() };

    executarReST("PosicaoDaFrotaMapa/ObterDadosMapa", data , function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {

                setTimeout(function () {
                    _mapaPosicaoDaFrota.clear();
                    criarMakersVeiculo(arg.Data);

                }, 350);

            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}