/// <reference path="Localidade.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _map;
var _marker;
var _searchBox;

function limparGeolocalizacaoClick(e) {
    _localidade.LatitudeEntrega.val('');
    _localidade.LongitudeEntrega.val('');
    carregarGeolocalizacao();
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGeolocalizacao() {
    carregarMapa();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function dragendEvent() {
    var latLng = _marker.getPosition();
    setLatLng(latLng.lat().toString(), latLng.lng().toString());
}

function setLatLng(lat, lng) {
    _localidade.LatitudeEntrega.val(lat);
    _localidade.LongitudeEntrega.val(lng);
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

function carregarGeolocalizacao() {
    if (_map != null) {
        var latLng = { lat: -10.861639, lng: -53.104038 };
        var zoom = 4;

        var lat = parseFloat(_localidade.LatitudeEntrega.val());
        var lng = parseFloat(_localidade.LongitudeEntrega.val());

        if (!isNaN(lat) && !isNaN(lng)) {
            latLng = { lat: lat, lng: lng };
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

            _map = new google.maps.Map(document.getElementById(_localidade.Map.id));
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

                    setLatLng(place.geometry.location.lat().toString(), place.geometry.location.lng().toString());

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