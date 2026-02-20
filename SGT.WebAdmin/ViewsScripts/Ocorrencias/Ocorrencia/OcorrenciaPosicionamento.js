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
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="Ocorrencia.js" />

var _pesquisaGeolocalizacao;

var _map;
//var _geocoder;
var _marker;

var latLngDefault = { lat: -10.861639, lng: -53.104038 };

var PesquisaGeolocalizacao = function () {

    this.Map = PropertyEntity();
    this.BuscarCoordenadas = PropertyEntity({ eventClick: BuscarCoordenadasClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.BuscarCoordenadasOcorrencia , visible: ko.observable(true) });
    this.PrecisaoCoordenadas = PropertyEntity({ visible: ko.observable(false) });
}

function loadGeolocalizacao() {
    _pesquisaGeolocalizacao = new PesquisaGeolocalizacao();
    KoBindings(_pesquisaGeolocalizacao, "knockoutPosicionamento");
    CarregarMapa();
}

function CarregarMapa() {
    if (_map == null) {
        setTimeout(function () {
            //_geocoder = new google.maps.Geocoder();

            _map = new google.maps.Map(document.getElementById(_pesquisaGeolocalizacao.Map.id));
            _marker = new google.maps.Marker({
                map: _map,
                draggable: true
            });
            _marker.addListener("dragend", dragendEvent)
            //setarCoordenadas();
        }, 200);
    };
}

function dragendEvent(event) {
    var latLng = _marker.getPosition();
    _ocorrencia.Latitude.val(latLng.lat().toString());
    _ocorrencia.Longitude.val(latLng.lng().toString());
}

function setarCoordenadasOcorrenciaPosicionamento() {
    if (_ocorrencia.Codigo.val() > 0 && (_ocorrencia.Latitude.val() == "" || _ocorrencia.Longitude.val() == "" || _ocorrencia.Latitude.val() == null || _ocorrencia.Longitude.val() == null)) {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Ocorrencias.Ocorrencia.OcorrenciaSemPosicionamentoCadastrado);
        return;
    } else {
        var latLng = { lat: -10.861639, lng: -53.104038 };
        var zoom = 4;
        if (_ocorrencia.Latitude.val() != "" && _ocorrencia.Longitude.val() != "") {
            latLng = { lat: parseFloat(_ocorrencia.Latitude.val()), lng: parseFloat(_ocorrencia.Longitude.val()) };
            zoom = 16;
        }
        _map.setZoom(zoom);
        _map.setCenter(latLng);
        _marker.setPosition(latLng);
    }
}

function limparCamposMapaRequest() {
    setarCoordenadasOcorrenciaPosicionamento();
}

function BuscarCoordenadasClick(e, sender) {
    BuscarCoordenadas();
}

function BuscarCoordenadas(callback) {
    setarCoordenadasOcorrenciaPosicionamento();
}