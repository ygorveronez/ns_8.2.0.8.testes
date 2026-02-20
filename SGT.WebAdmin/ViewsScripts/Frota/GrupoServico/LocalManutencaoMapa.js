/// <reference path="../../Enumeradores/EnumTipoPessoa.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoCTeDocumentos.js" />
/// <reference path="../../Enumeradores/EnumTipoLocalizacao.js" />

var _map = null;
var _mapDraw = null;
var _marker = null;
var _searchBox = null;
var latLngDefault = { lat: -10.861639, lng: -53.104038 };
var $liGeoLocalizacao = null;
var _markersArray = [];

function loadGeolocalizacaoMotorista() {
    CarregarMapa();
}

function CarregarMapa() {
    if (_map == null) {
        var opcoesmapa = {
            zoom: 5,
            scaleControl: true,
            gestureHandling: 'greedy'
        };

        _map = new google.maps.Map(document.getElementById(_localManutencao.Map.id), opcoesmapa);

        setTimeout(function () {

            var input = document.getElementById('pac-input');
            _searchBox = new google.maps.places.SearchBox(input);
            _map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);

            _map.addListener('bounds_changed', function () {
                _searchBox.setBounds(_map.getBounds());
            });

            _mapDraw = new MapaDraw(_map);

            _marker = new google.maps.Marker({
                map: _map,
                draggable: true,
                icon: GerarPinMapa('#d45b5b')
            });

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

            setarCoordenadas();
        }, 200);
    };
}

function GerarPinMapa(cor) {
    return {
        path: fontawesome.markers.MAP_MARKER,
        scale: 0.8,
        strokeWeight: 0.2,
        strokeColor: 'black',
        strokeOpacity: 1,
        fillColor: cor,
        fillOpacity: 1,
        anchor: new google.maps.Point(19, 0),
    }
}

function setarCoordenadas() {
    if (_map != null) {
        //var latLng = { lat: -10.861639, lng: -53.104038 }; //Ponto central no brasil, removido pois clientes estavam ficando neste ponto
        var latLng = { lat: 0, lng: 0 };
        var bounds = new google.maps.LatLngBounds();
        var zoom = 4;

        var localManutencaoGrid = _gridLocalManutencao.BuscarRegistros();

        for (var i = 0; i < localManutencaoGrid.length; i++) {
            var localManutencao = localManutencaoGrid[i];

            if (localManutencao.Latitude != null && localManutencao.Longitude != null && localManutencao.Latitude != "" && localManutencao.Longitude != "") {
                var latLngNormal = { lat: parseFloat(localManutencao.Latitude.replace(',', '.')), lng: parseFloat(localManutencao.Longitude.replace(',', '.')) };
                latLng = latLngNormal;
                zoom = 5;

                var loc = new google.maps.LatLng(latLngNormal.lat, latLngNormal.lng);
                bounds.extend(loc);
            }
            _marker = new google.maps.Marker({
                map: _map,
                draggable: false,
                icon: GerarPinMapa('#d45b5b')
            });
            _markersArray.push(_marker);
            _marker.setPosition(latLng);
        }
        _map.setZoom(zoom);
        _map.setCenter(bounds.getCenter());

    }
}

function limparCamposMapaRequest() {
    for (var i = 0; i < this._markersArray.length; i++) {
        this._markersArray[i].setMap(null);
    }
}