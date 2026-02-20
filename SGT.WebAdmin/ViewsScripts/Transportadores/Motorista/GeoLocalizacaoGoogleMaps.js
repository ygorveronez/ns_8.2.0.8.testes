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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Atividade.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Endereco.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoCTeDocumentos.js" />
/// <reference path="../../Enumeradores/EnumTipoLocalizacao.js" />
/// <reference path="../../Consultas/Locais.js" />

var _map = null;
var _mapDraw = null;
//var _geocoder = null;
var _marker = null;
var _markerTransbordo = null;
var _searchBox = null;
var latLngDefault = { lat: -10.861639, lng: -53.104038 };
var centralizarMapa = null;
var $liGeoLocalizacao = null;

function loadGeolocalizacaoMotorista() {
    CarregarMapa();

    $liGeoLocalizacao = $("#liGeoLocalizacao");

    $liGeoLocalizacao.on('shown.bs.tab', 'a', function () {
        if (centralizarMapa != null) {
            _map.fitBounds(centralizarMapa);
            _map.panToBounds(centralizarMapa);
            centralizarMapa = null;
        }
    });

    $(window).one('hashchange', function (e) {
        $liGeoLocalizacao.off('shown.bs.tab', 'a');
    });
}

function CarregarMapa() {
    if (_map == null) {
        var opcoesmapa = {
            zoom: 5,
            scaleControl: true,
            gestureHandling: 'greedy'
        };

        _map = new google.maps.Map(document.getElementById(_motorista.Map.id), opcoesmapa);

        setTimeout(function () {
            //_geocoder = new google.maps.Geocoder();

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

            _markerTransbordo = new google.maps.Marker({
                draggable: true,
                icon: GerarPinMapa('#2386dc')
            });

            _marker.addListener("dragend", dragendEvent)
            _markerTransbordo.addListener("dragend", dragendTransbordoEvent)


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

function dragendEvent(event) {
    var latLng = _marker.getPosition();
    _motorista.Latitude.val(latLng.lat().toString());
    _motorista.Longitude.val(latLng.lng().toString());
}

function dragendTransbordoEvent(event) {
    var latLng = _markerTransbordo.getPosition();
    _motorista.LatitudeTransbordo.val(latLng.lat().toString());
    _motorista.LongitudeTransbordo.val(latLng.lng().toString());
}

function setarRaioEmMetros() {
    if (_mapDraw)
        _mapDraw.deleteAll();

    if ((_mapDraw) && (_motorista.RaioEmMetros.val() !== "" && _motorista.Latitude.val() !== "" && _motorista.Longitude.val() !== "")) {
        var latLngNormal = { lat: parseFloat(_motorista.Latitude.val()), lng: parseFloat(_motorista.Longitude.val()) };
        var shapeCircle = new ShapeCircle();
        shapeCircle.type = google.maps.drawing.OverlayType.CIRCLE;
        shapeCircle.fillColor = "#FF0000";
        shapeCircle.radius = parseInt(_motorista.RaioEmMetros.val());
        shapeCircle.center = latLngNormal
        _mapDraw.addShape(shapeCircle);
    }
}

function setarTipoArea() {

    if (_mapDraw) {
        _mapDraw.deleteAll();
        var raioVisible = _motorista.TipoArea.val() === 1;
        _motorista.RaioEmMetros.visible(raioVisible)

        if (!raioVisible) {
            var divMap = document.getElementById(_motorista.Map.id)
            _mapDraw.ShowDrawPalette(divMap);
            _motorista.RaioEmMetros.val("");
        }


        if (raioVisible) {
            _mapDraw.HideDrawPalette();

        }
    }
}

function SetarAreaGeoLocalizacao() {
    if ((_mapDraw) && (_motorista.TipoArea.val() === 2)) {
        _mapDraw.clear();
        _mapDraw.setJson(_motorista.Area.val());
    }

}

function setarPontoDeApoio(pontoDeApoio, pontoDeApoioAuto) {
    if (pontoDeApoio) {
        _motorista.PontoDeApoio.codEntity(pontoDeApoio.Codigo);
        _motorista.PontoDeApoio.val(pontoDeApoio.Descricao);
    }
    _motorista.AtualizarPontoApoioMaisProximoAutomaticamente.val(pontoDeApoioAuto);
}

function obterJsonPoligonoGeoLocalizacao() {
    if ((_mapDraw) && (_motorista.TipoArea.val() === 2))
        return _mapDraw.getJson();

    return "";
}

function setarCoordenadas() {
    if (_map != null) {
        //var latLng = { lat: -10.861639, lng: -53.104038 }; //Ponto central no brasil, removido pois clientes estavam ficando neste ponto
        var latLng = { lat: 0, lng: 0 };
        var bounds = new google.maps.LatLngBounds();
        var zoom = 4;
        var centralizarPontoETransbordo = false;

        if (_motorista.Latitude.val() != null && _motorista.Longitude.val() != null && _motorista.Latitude.val() != "" && _motorista.Longitude.val() != "") {
            var latLngNormal = { lat: parseFloat(_motorista.Latitude.val().replace(',', '.')), lng: parseFloat(_motorista.Longitude.val().replace(',', '.')) };
            latLng = latLngNormal;
            zoom = 18;

            var loc = new google.maps.LatLng(latLngNormal.lat, latLngNormal.lng);
            bounds.extend(loc);
        }

        if (_motorista.LatitudeTransbordo.val() != null && _motorista.LongitudeTransbordo.val() != null && _motorista.LatitudeTransbordo.val() != "" && _motorista.LongitudeTransbordo.val() != "") {
            var latLngTransbordo = { lat: parseFloat(_motorista.LatitudeTransbordo.val().replace(',', '.')), lng: parseFloat(_motorista.LongitudeTransbordo.val().replace(',', '.')) };

            _markerTransbordo.setPosition(latLngTransbordo);
            _markerTransbordo.setMap(_map);

            var loc = new google.maps.LatLng(latLngTransbordo.lat, latLngTransbordo.lng);
            bounds.extend(loc);
            centralizarPontoETransbordo = true;
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

function limparCamposMapaRequest() {
    setarCoordenadas();
    _markerTransbordo.setMap(null);
    _motorista.PontoTransbordo.val(false);
    _motorista.AtualizarPontoApoioMaisProximoAutomaticamente.val(false);
    _motorista.PontoDeApoio.codEntity(0);
    _motorista.PontoDeApoio.val('');
}

function PontoTransbordoClick() {
    //if (_marker.getMap() == null)
    //    return exibirMensagem(tipoMensagem.aviso, "Mapa", "É necessário informar a posição principal antes de informar o ponto do transbordo");

    var possuiPonto = !_motorista.PontoTransbordo.val();

    _motorista.PontoTransbordo.val(possuiPonto);

    if (possuiPonto) {
        var center = _map.getCenter();
        _markerTransbordo.setPosition({ lat: center.lat(), lng: center.lng() });
        _markerTransbordo.setMap(_map);
    } else {
        _markerTransbordo.setMap(null);
    }
}

function BuscarCoordenadasClick(e, sender) {
    BuscarCoordenadas();
}

function BuscarCoordenadas(callback) {

    var address = "";
    if (_motorista.Endereco.val() != "") {
        address += _motorista.Endereco.val();
    }

    if (_motorista.NumeroEndereco.val() != "" && _motorista.SN_Numero.val() == true) {
        // Separador de milhar.. não rolou.. deu problema nas RUAS
        //#31217 - "ROD BR 101, 9000, NATAL - RN, CEP 59115900"
        // Problema ao geocodificar rodovia, aonde não possui KM no número ou um "." no número do endereço
        if ((address.toUpperCase().indexOf('ROD ') >= 0 || address.toUpperCase().indexOf('RODOVIA ') >= 0 ||
            address.toUpperCase().indexOf('ROD. ') >= 0) && _motorista.NumeroEndereco.val().indexOf(".") < 0 && _motorista.NumeroEndereco.val().toUpperCase().indexOf('KM') < 0)
            address += ", KM ";
    }

    dadosEndereco = new DadosEndereco(address, _motorista.NumeroEndereco.val(), _motorista.Localidade.val(), "", _motorista.CEP.val(), _motorista.Bairro.val(), "", _motorista.TipoLogradouro.val());

    new MapaGoogleGeoLocalizacao().buscarCoordenadas(dadosEndereco, function (resposta) {

        if (resposta.status === 'OK') {
            _motorista.Latitude.val(resposta.latitude.toString());
            _motorista.Longitude.val(resposta.longitude.toString());
            setarCoordenadas();
        } else if (resposta.status === "NotFound") {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Pessoas.Pessoa.GeolocalizacaoNotFound);
        } else if (resposta.status === "ErroNominatim") {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Pessoas.Pessoa.GeolocalizacaoErroNominatim);
        } else {
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS)
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Pessoas.Pessoa.NaoFoiLocalizadaUmaPosicaoParaEnderecoInformado);
        }

        if (callback != null)
            callback();
    });

    //if (_geocoder == null)
    //    callback();

    //var address = "";
    //if (_motorista.Endereco.val() != "") {
    //    address += _motorista.Endereco.val() + ", ";
    //}

    //if (_motorista.Numero.val() != "" && _motorista.SN_Numero.val() == true) {
    //    // Separador de milhar.. não rolou.. deu problema nas RUAS
    //    //#31217 - "ROD BR 101, 9000, NATAL - RN, CEP 59115900"
    //    // Problema ao geocodificar rodovia, aonde não possui KM no número ou um "." no número do endereço
    //    if ((address.toUpperCase().indexOf('ROD ') >= 0 || address.toUpperCase().indexOf('RODOVIA ') >= 0 ||
    //        address.toUpperCase().indexOf('ROD. ') >= 0) && _motorista.Numero.val().indexOf(".") < 0 && _motorista.Numero.val().toUpperCase().indexOf('KM') < 0)
    //        address += "KM ";
    //    address += _motorista.Numero.val() + ", ";
    //}

    //if (_motorista.Localidade.val() != "") {
    //    address += _motorista.Localidade.val() + ", ";
    //}

    //if (_motorista.CEP.val() != "") {
    //    address += _motorista.CEP.val();
    //}

    //_geocoder.geocode({ 'address': address }, function (results, status) {
    //    if (status === 'OK') {
    //        _motorista.Latitude.val(results[0].geometry.location.lat().toString());
    //        _motorista.Longitude.val(results[0].geometry.location.lng().toString());
    //        setarCoordenadas();
    //    } else {
    //        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS)
    //            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Ñão foi localizada uma posição para o Endereço informado");
    //    }

    //    if (callback != null)
    //        callback();
    //});
}

function ClienteSemCoordenadas() {
    if (_motorista.Latitude.val() == "" || _motorista.Longitude.val() == "" || _motorista.Latitude.val() == null || _motorista.Longitude.val() == null)
        return true;
    else
        return false;
}

function ValidaCoordenadaRaioLocalidade(codigoLocalidade, callback) {

    //Se não tem configuração para validar o raio maximo ou se a pessoa está sem a localidade..
    if (_CONFIGURACAO_TMS.RaioMaximoGeoLocalidadeGeoCliente == 0 || codigoLocalidade == 0) {

        callback();

    } else {

        var data = {
            CodigoLocalidade: codigoLocalidade,
            Latitude: _motorista.Latitude.val(),
            Longitude: _motorista.Longitude.val(),
            RaioMaximo: _CONFIGURACAO_TMS.RaioMaximoGeoLocalidadeGeoCliente
        };

        executarReST("Pessoa/ValidarRaioMaximoLatLngClienteXLocalidade", data, function (arg) {
            if (arg.Success) {

                if (arg.Msg == null || arg.Msg == '') {
                    callback();
                } else {
                    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, arg.Msg + "Deseja continuar mesmo assim?", function () {
                        callback();
                    });
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });

    }
}