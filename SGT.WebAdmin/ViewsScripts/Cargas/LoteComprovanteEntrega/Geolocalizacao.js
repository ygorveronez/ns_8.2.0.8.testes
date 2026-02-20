/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />

/**
 *  Esse arquivo cuida de tudo que é necessário para gerenciar o modal de geolocalização das cargaEntrega
 * */

var _geolocalizacaoCargaEntrega;
var _cargaEntregaAtual;

var _map = null;
var _marker = null;

// region: ENTIDADES KNOCKOUT

var GeolocalizacaoCargaEntrega = function() {
    this.Map = PropertyEntity();
    this.Latitude = PropertyEntity({ val: ko.observable(_cargaEntregaAtual.GeoLocalizacao.Latitude) });
    this.Longitude = PropertyEntity({ val: ko.observable(_cargaEntregaAtual.GeoLocalizacao.Longitude) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: ko.observable("Atualizar geolocalização"), visible: ko.observable(true), enable: ko.observable(true) });
}

// region: ACTIONS

function AtualizarClick() {
    _cargaEntregaAtual.GeoLocalizacao.Latitude = _geolocalizacaoCargaEntrega.Latitude.val();
    _cargaEntregaAtual.GeoLocalizacao.Longitude = _geolocalizacaoCargaEntrega.Longitude.val();
    Global.fecharModal("divModalGeolocalizacaoCargaEntrega");
}

function loadModalGeolocalizacaoCargaEntrega() {
    _geolocalizacaoCargaEntrega = new GeolocalizacaoCargaEntrega();
    KoBindings(_geolocalizacaoCargaEntrega, "knockoutGeolocalizacaoCargaEntrega");
    CarregarMapa();
}

function exibirModalGeolocalizacaoCargaEntrega(cargaEntrega) {
    _cargaEntregaAtual = cargaEntrega;
    Global.abrirModal('divModalGeolocalizacaoCargaEntrega');

    $("#divModalGeolocalizacaoCargaEntrega").on("hidden.bs.modal", () => {
        _map = null;
        _cargaEntregaAtual = null;
        _geolocalizacaoCargaEntrega = null;
        recarregarGridCargaEntrega();
    });

    loadModalGeolocalizacaoCargaEntrega();

    // Desabilita botões se for modo de visualização
    _geolocalizacaoCargaEntrega.Atualizar.enable(podeEditarLote());
}

function CarregarMapa() {
    if (_map == null) {
        var opcoesmapa = {
            zoom: 5,
            scaleControl: true,
            gestureHandling: 'greedy'
        };

        _map = new google.maps.Map(document.getElementById(_geolocalizacaoCargaEntrega.Map.id), opcoesmapa);

        setTimeout(function() {
            var input = document.getElementById('pac-input');
            _map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);

            _marker = new google.maps.Marker({
                map: _map,
                draggable: true,
                icon: GerarPinMapa('#d45b5b')
            });

            _marker.addListener("dragend", dragendEvent)

            setarCoordenadasComprovanteEntrega();
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

function dragendEvent(event) {
    var latLng = _marker.getPosition();
    _geolocalizacaoCargaEntrega.Latitude.val(latLng.lat().toString());
    _geolocalizacaoCargaEntrega.Longitude.val(latLng.lng().toString());
}

function setarCoordenadasComprovanteEntrega() {
    if (_map != null) {
        var latLng = { lat: 0, lng: 0 };
        var bounds = new google.maps.LatLngBounds();
        var zoom = 4;
        var centralizarPontoETransbordo = false;

        if (_cargaEntregaAtual.GeoLocalizacao.Latitude != null && _cargaEntregaAtual.GeoLocalizacao.Longitude != null && _cargaEntregaAtual.GeoLocalizacao.Latitude != "" && _cargaEntregaAtual.GeoLocalizacao.Longitude != "") {
            var latLngNormal = { lat: parseFloat(_cargaEntregaAtual.GeoLocalizacao.Latitude.replace(',', '.')), lng: parseFloat(_cargaEntregaAtual.GeoLocalizacao.Longitude.replace(',', '.')) };
            latLng = latLngNormal;
            zoom = 18;

            var loc = new google.maps.LatLng(latLngNormal.lat, latLngNormal.lng);
            bounds.extend(loc);
        }

        _marker.setPosition(latLng);

        if (centralizarPontoETransbordo) {
            if (!$liGeoLocalizacao.hasClass('active')) {
                centralizarMapa = bounds;
            } else {
                _map.fitBounds(bounds);
                _map.panToBounds(bounds);
            }
        } else {
            _map.setZoom(zoom);
            _map.setCenter(latLng);
        }
    }
}