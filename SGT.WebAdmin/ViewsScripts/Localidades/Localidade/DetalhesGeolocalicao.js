/// <reference path="Localidade.js" />

var _mapDetalhes;
var _markerDetalhes;
const latLngPadrao = { lat: -20.0968, lng: -42.6186 };


function limparGeolocalizacaoClick(e) {
    _localidade.Latitude.val('');
    _localidade.Longitude.val('');
}


function loadDetalhesGeolocalizacao() {
    carregarMapaDetalhes();
}


function dragendEventMap() {
    const latLng = _markerDetalhes.getPosition();
    setLatLngMap(latLng.lat().toString(), latLng.lng().toString());
}


function setLatLngMap(lat, lng) {
    console.log('passou');
    _localidade.Latitude.val(lat);
    _localidade.Longitude.val(lng);
    if (_markerDetalhes != null && lat != '') {
        var position = new google.maps.LatLng(lat, lng);
        _markerDetalhes.setPosition(position);
        _mapDetalhes.panTo(position);
    }
}

function carregarMapaDetalhes() {

    if (_mapDetalhes == null) {
        setTimeout(function () {
            const mapOptions = {
                center: latLngPadrao,
                zoom: 14
            };

            _mapDetalhes = new google.maps.Map(document.getElementById(_localidade.MapDetalhes.id), mapOptions);

            _markerDetalhes = new google.maps.Marker({
                map: _mapDetalhes,
                draggable: true
            });

            _markerDetalhes.addListener("dragend", dragendEventMap)

            _markerDetalhes.setPosition(latLngPadrao);

        }, 200);
    };
}


function obterGeolocalizacaoAtual() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(
            (position) => {
                const latitude = position.coords.latitude;
                const longitude = position.coords.longitude;
                setLatLngMap(latitude, longitude);
            },
            (error) => exibirMensagem(tipoMensagem.error, "Error", error));
    } else {
        exibirMensagem(tipoMensagem.error, "Error", Localization.Resources.Localidades.Localidade.NavegadorNaoSuportaGeolocalizacao);
    }

}