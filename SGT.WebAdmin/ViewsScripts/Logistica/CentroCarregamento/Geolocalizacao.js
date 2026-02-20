/// <reference path="CentroCarregamento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _pesquisaGeolocalizacao;
var _map;
var _marker;
var _searchBox;

/*
 * Declaração das Classes
 */

var PesquisaGeolocalizacao = function () {
    this.Map = PropertyEntity();
    this.PrecisaoCoordenadas = PropertyEntity({ visible: ko.observable(false) });
    this.DistanciaMinimaEntrarFilaCarregamento = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, maxlength: 5, text: Localization.Resources.Logistica.CentroCarregamento.DistanciaMinimaParaEntrarNafilaDeCarregamentoKM.getFieldDescription() });

    this.DistanciaMinimaEntrarFilaCarregamento.val.subscribe(function (valor) { _centroCarregamento.DistanciaMinimaEntrarFilaCarregamento.val(valor); });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGeolocalizacao() {
    _pesquisaGeolocalizacao = new PesquisaGeolocalizacao();
    KoBindings(_pesquisaGeolocalizacao, "knockoutGeolocalizacao");

    carregarMapa();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function dragendEvent() {
    var latLng = _marker.getPosition();

    _centroCarregamento.Latitude.val(latLng.lat().toString());
    _centroCarregamento.Longitude.val(latLng.lng().toString());
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposGeolocalizacao() {
    carregarGeolocalizacao();
}

/*
 * Declaração das Funções
 */

function carregarGeolocalizacao(distanciaMinimaEntrarFilaCarregamento) {
    _pesquisaGeolocalizacao.DistanciaMinimaEntrarFilaCarregamento.val(distanciaMinimaEntrarFilaCarregamento > 0 ? distanciaMinimaEntrarFilaCarregamento : "");

    if (_map != null) {
        var latLng = { lat: -10.861639, lng: -53.104038 };
        var zoom = 4;

        if (_centroCarregamento.Latitude.val() != "" && _centroCarregamento.Longitude.val() != "") {
            latLng = { lat: parseFloat(_centroCarregamento.Latitude.val()), lng: parseFloat(_centroCarregamento.Longitude.val()) };
            zoom = 16;
        }

        _map.setZoom(zoom);
        _map.setCenter(latLng);
        _marker.setPosition(latLng);
    }
}

function carregarMapa() {
    if (_map == null) {
        setTimeout(function () {

            _map = new google.maps.Map(document.getElementById(_pesquisaGeolocalizacao.Map.id));
            var input = document.getElementById('pac-input');
            _searchBox = new google.maps.places.SearchBox(input);
            _map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);

            _map.addListener('bounds_changed', function () {
                _searchBox.setBounds(_map.getBounds());
            });

            _marker = new google.maps.Marker({
                map: _map,
                draggable: true
            });

            _marker.addListener("dragend", dragendEvent)


            var markers = [];

            _searchBox.addListener('places_changed', function () {
                var places = _searchBox.getPlaces();

                if (places.length == 0) {
                    return;
                }

                markers.forEach(function (marker) {
                    marker.setMap(null);
                });
                markers = [];

                var bounds = new google.maps.LatLngBounds();
                places.forEach(function (place) {
                    if (!place.geometry) {
                        console.log("Returned place contains no geometry");
                        return;
                    }
                    _marker.setPosition(place.geometry.location);
                    if (place.geometry.viewport) {
                        bounds.union(place.geometry.viewport);
                    } else {
                        bounds.extend(place.geometry.location);
                    }
                });
                _map.fitBounds(bounds);
            });

            carregarGeolocalizacao();
        }, 200);
    };
}