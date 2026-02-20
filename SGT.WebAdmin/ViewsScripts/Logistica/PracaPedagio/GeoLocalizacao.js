/// <reference path="../../Enumeradores/EnumTipoPracaPedagio.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="TransportadorTerceiro.js" />
/// <reference path="Fornecedor.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/PracaPedagio.js" />
/// <reference path="../../Consultas/Atividade.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Endereco.js" />
/// <reference path="ListaEmail.js" />
/// <reference path="ListaEndereco.js" />
/// <reference path="DadoBancario.js" />
/// <reference path="Emissao.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoCTeDocumentos.js" />
/// <reference path="../../Enumeradores/EnumTipoLocalizacao.js" />

var _pesquisaGeolocalizacao;

var _map;
//var _geocoder;
var _marker;
var _searchBox;
var latLngDefault = { lat: -10.861639, lng: -53.104038 };

var PesquisaGeolocalizacao = function () {

    this.Map = PropertyEntity();
    this.BuscarCoordenadas = PropertyEntity({ eventClick: BuscarCoordenadasClick, type: types.event, text: Localization.Resources.Logistica.PracaPedagio.BuscarCoordenadasDoEndereco, visible: ko.observable(true) });
    this.PrecisaoCoordenadas = PropertyEntity({ visible: ko.observable(false) });
}

function loadGeolocalizacao() {
    _pesquisaGeolocalizacao = new PesquisaGeolocalizacao();
    KoBindings(_pesquisaGeolocalizacao, "knockoutGeolocalizacao");
    CarregarMapa();
}

function CarregarMapa() {
    if (_map == null) {
        setTimeout(function () {
            //_geocoder = new google.maps.Geocoder();

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

            setarCoordenadas();
        }, 200);
    };
}

function dragendEvent(event) {
    var latLng = _marker.getPosition();
    _pracaPedagio.Latitude.val(latLng.lat().toString());
    _pracaPedagio.Longitude.val(latLng.lng().toString());
}

function setarCoordenadas() {
    if (_map != null) {
        var latLng = { lat: -10.861639, lng: -53.104038 };
        var zoom = 4;
        if (_pracaPedagio.Latitude.val() != "" && _pracaPedagio.Longitude.val() != "") {
            latLng = { lat: parseFloat(_pracaPedagio.Latitude.val()), lng: parseFloat(_pracaPedagio.Longitude.val()) };
            zoom = 16;
        }
        _map.setZoom(zoom);
        _map.setCenter(latLng);
        _marker.setPosition(latLng);
    }
}

function limparCamposMapa() {
    setarCoordenadas();
}

function BuscarCoordenadasClick(e, sender) {
    BuscarCoordenadas();
}

function BuscarCoordenadas(callback) {

    var address = _pracaPedagio.Descricao.val() + ", " + _pracaPedagio.Rodovia.val() + " - KM " + _pracaPedagio.KM.val();

    dadosEndereco = new DadosEndereco(address, '', '', '', '', '', '', '', true);

    new MapaGoogleGeoLocalizacao().buscarCoordenadas(dadosEndereco, function (resposta) {

        if (resposta.status === 'OK') {
            _pracaPedagio.Latitude.val(resposta.latitude.toString());
            _pracaPedagio.Longitude.val(resposta.longitude.toString());
            setarCoordenadas();
        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.PracaPedagio.NaoFoiLocalizadaPosicaoParaEnderecoInformado);
        }

        if (callback != null)
            callback();
    });

    //if (_geocoder == null)
    //    callback();

    //var address = _pracaPedagio.Descricao.val() + ", " + _pracaPedagio.Rodovia.val() + " - KM " + _pracaPedagio.KM.val();

    //_geocoder.geocode({ 'address': address }, function (results, status) {
    //    if (status === 'OK') {
    //        _pracaPedagio.Latitude.val(results[0].geometry.location.lat().toString());
    //        _pracaPedagio.Longitude.val(results[0].geometry.location.lng().toString());
    //        setarCoordenadas();
    //    } else {
    //        exibirMensagem(tipoMensagem.atencao, "Atenção", "Não foi localizada uma posição para o endereço informado.");
    //    }

    //    if (callback != null)
    //        callback();
    //});
}

function PracaPedagioSemCoordenadas() {
    if (_pracaPedagio.Latitude.val() == "" || _pracaPedagio.Latitude.val() == "" || _pracaPedagio.Latitude.val() == null || _pracaPedagio.Latitude.val() == null)
        return true;
    else
        return false;
}