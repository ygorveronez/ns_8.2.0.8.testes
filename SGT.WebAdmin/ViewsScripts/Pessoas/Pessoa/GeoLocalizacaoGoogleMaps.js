/// <reference path="../../Enumeradores/EnumTipoPessoa.js" />
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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Atividade.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Endereco.js" />
/// <reference path="ListaEmail.js" />
/// <reference path="ListaEndereco.js" />
/// <reference path="DadoBancario.js" />
/// <reference path="Emissao.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoCTeDocumentos.js" />
/// <reference path="../../Enumeradores/EnumTipoLocalizacao.js" />
/// <reference path="../../consultas/locais.js" />

var _pesquisaGeolocalizacao = null;
var _map = null;
var _mapDraw = null;
//var _geocoder = null;
var _marker = null;
var _markerTransbordo = null;
var _searchBox = null;
var latLngDefault = { lat: -10.861639, lng: -53.104038 };
var centralizarMapa = null;
var $liGeoLocalizacao = null;

var PesquisaGeolocalizacao = function () {
    this.Map = PropertyEntity();
    this.BuscarCoordenadas = PropertyEntity({ eventClick: BuscarCoordenadasClick, type: types.event, text: Localization.Resources.Pessoas.Pessoa.BuscarCoordenadasDoEndereco, visible: ko.observable(true) });
    this.PontoTransbordo = PropertyEntity({ eventClick: PontoTransbordoClick, type: types.event, val: ko.observable(false) });
    this.PrecisaoCoordenadas = PropertyEntity({ visible: ko.observable(false) });
    this.RaioEmMetros = _pessoa.RaioEmMetros;
    this.AtualizarPontoApoioMaisProximoAutomaticamente = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.AtualizarPontoApoioAutomaticamente.getFieldDescription(), val: ko.observable(false), options: Global.ObterOpcoesBooleano(Localization.Resources.Enumeradores.SimNao.Sim, Localization.Resources.Enumeradores.SimNao.Nao), def: false, visible: ko.observable(true) });
    this.PontoDeApoio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pessoas.Pessoa.PontoDeApoio.getFieldDescription()), idBtnSearch: guid() });
    this.TipoArea = _pessoa.TipoArea;
    this.AlvoEstrategico = _pessoa.AlvoEstrategico;
    this.Latitude = _pessoa.Latitude;
    this.Longitude = _pessoa.Longitude;
    this.BuscarLatitudeLongitude = _pessoa.BuscarLatitudeLongitude;
    this.GeoLocalizacaoRaioLocalidade = _pessoa.GeoLocalizacaoRaioLocalidade;
    this.RaioGeoLocalizacaoLocalidade = _pessoa.RaioGeoLocalizacaoLocalidade;
}

function loadGeolocalizacao() {

    _pesquisaGeolocalizacao = new PesquisaGeolocalizacao();
    KoBindings(_pesquisaGeolocalizacao, "knockoutGeolocalizacao");

    new BuscarLocais(_pesquisaGeolocalizacao.PontoDeApoio);

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

        _map = new google.maps.Map(document.getElementById(_pesquisaGeolocalizacao.Map.id), opcoesmapa);

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
    _pessoa.Latitude.val(latLng.lat().toString());
    _pessoa.Longitude.val(latLng.lng().toString());
}

function dragendTransbordoEvent(event) {
    var latLng = _markerTransbordo.getPosition();
    _pessoa.LatitudeTransbordo.val(latLng.lat().toString());
    _pessoa.LongitudeTransbordo.val(latLng.lng().toString());
}

function setarRaioEmMetros() {
    if (_mapDraw)
        _mapDraw.deleteAll();

    if ((_mapDraw) && (_pessoa.RaioEmMetros.val() !== "" && _pessoa.Latitude.val() !== "" && _pessoa.Longitude.val() !== "")) {
        var latLngNormal = { lat: parseFloat(_pessoa.Latitude.val()), lng: parseFloat(_pessoa.Longitude.val()) };
        var shapeCircle = new ShapeCircle();
        shapeCircle.type = google.maps.drawing.OverlayType.CIRCLE;
        shapeCircle.fillColor = "#FF0000";
        shapeCircle.radius = parseInt(_pessoa.RaioEmMetros.val());
        shapeCircle.center = latLngNormal
        _mapDraw.addShape(shapeCircle);
    }
}

function setarTipoArea() {

    if (_mapDraw) {
        _mapDraw.deleteAll();
        var raioVisible = _pessoa.TipoArea.val() === 1;
        _pessoa.RaioEmMetros.visible(raioVisible)

        if (!raioVisible) {
            var divMap = document.getElementById(_pesquisaGeolocalizacao.Map.id)
            _mapDraw.ShowDrawPalette(divMap);
        }

        if (raioVisible) {
            _mapDraw.HideDrawPalette();

        }
    }
}

function SetarAreaGeoLocalizacao() {
    if ((_mapDraw) && (_pessoa.TipoArea.val() === 2)) {
        _mapDraw.clear();
        _mapDraw.setJson(_pessoa.Area.val());
    }

}

function setarPontoDeApoio(pontoDeApoio, pontoDeApoioAuto) {
    if (pontoDeApoio) {
        _pesquisaGeolocalizacao.PontoDeApoio.codEntity(pontoDeApoio.Codigo);
        _pesquisaGeolocalizacao.PontoDeApoio.val(pontoDeApoio.Descricao);
    }
    _pesquisaGeolocalizacao.AtualizarPontoApoioMaisProximoAutomaticamente.val(pontoDeApoioAuto);
}

function obterJsonPoligonoGeoLocalizacao() {
    if ((_mapDraw) && (_pessoa.TipoArea.val() === 2))
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

        if (_pessoa.Latitude.val() != null && _pessoa.Longitude.val() != null && _pessoa.Latitude.val() != "" && _pessoa.Longitude.val() != "") {
            var latLngNormal = { lat: parseFloat(_pessoa.Latitude.val().replace(',', '.')), lng: parseFloat(_pessoa.Longitude.val().replace(',', '.')) };
            latLng = latLngNormal;
            zoom = 18;

            var loc = new google.maps.LatLng(latLngNormal.lat, latLngNormal.lng);
            bounds.extend(loc);
        }

        if (_pessoa.LatitudeTransbordo.val() != null && _pessoa.LongitudeTransbordo.val() != null && _pessoa.LatitudeTransbordo.val() != "" && _pessoa.LongitudeTransbordo.val() != "") {
            var latLngTransbordo = { lat: parseFloat(_pessoa.LatitudeTransbordo.val().replace(',', '.')), lng: parseFloat(_pessoa.LongitudeTransbordo.val().replace(',', '.')) };

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
    _pesquisaGeolocalizacao.PontoTransbordo.val(false);
    _pesquisaGeolocalizacao.AtualizarPontoApoioMaisProximoAutomaticamente.val(false);
    _pesquisaGeolocalizacao.PontoDeApoio.codEntity(0);
    _pesquisaGeolocalizacao.PontoDeApoio.val('');
}

function PontoTransbordoClick() {
    //if (_marker.getMap() == null)
    //    return exibirMensagem(tipoMensagem.aviso, "Mapa", "É necessário informar a posição principal antes de informar o ponto do transbordo");

    var possuiPonto = !_pesquisaGeolocalizacao.PontoTransbordo.val();

    _pesquisaGeolocalizacao.PontoTransbordo.val(possuiPonto);

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
    if (_pessoa.Endereco.val() != "") {
        address += _pessoa.Endereco.val();
    }

    if (_pessoa.Numero.val() != "" && _pessoa.SN_Numero.val() == true) {
        // Separador de milhar.. não rolou.. deu problema nas RUAS
        //#31217 - "ROD BR 101, 9000, NATAL - RN, CEP 59115900"
        // Problema ao geocodificar rodovia, aonde não possui KM no número ou um "." no número do endereço
        if ((address.toUpperCase().indexOf('ROD ') >= 0 || address.toUpperCase().indexOf('RODOVIA ') >= 0 ||
            address.toUpperCase().indexOf('ROD. ') >= 0) && _pessoa.Numero.val().indexOf(".") < 0 && _pessoa.Numero.val().toUpperCase().indexOf('KM') < 0)
            address += ", KM ";
    }

    dadosEndereco = new DadosEndereco(address, _pessoa.Numero.val(), _pessoa.Localidade.val(), "", _pessoa.CEP.val(), _pessoa.Bairro.val(), "", _pessoa.TipoLogradouro.val());

    new MapaGoogleGeoLocalizacao().buscarCoordenadas(dadosEndereco, function (resposta) {

        if (resposta.status === 'OK') {
            _pessoa.Latitude.val(resposta.latitude.toString());
            _pessoa.Longitude.val(resposta.longitude.toString());
            setarCoordenadas();
        } else if (resposta.status === "NotFound") {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.Pessoa.Atencao, Localization.Resources.Pessoas.Pessoa.GeolocalizacaoNotFound);
        } else if (resposta.status === "ErroNominatim") {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.Pessoa.Atencao, Localization.Resources.Pessoas.Pessoa.GeolocalizacaoErroNominatim);
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
    //if (_pessoa.Endereco.val() != "") {
    //    address += _pessoa.Endereco.val() + ", ";
    //}

    //if (_pessoa.Numero.val() != "" && _pessoa.SN_Numero.val() == true) {
    //    // Separador de milhar.. não rolou.. deu problema nas RUAS
    //    //#31217 - "ROD BR 101, 9000, NATAL - RN, CEP 59115900"
    //    // Problema ao geocodificar rodovia, aonde não possui KM no número ou um "." no número do endereço
    //    if ((address.toUpperCase().indexOf('ROD ') >= 0 || address.toUpperCase().indexOf('RODOVIA ') >= 0 ||
    //        address.toUpperCase().indexOf('ROD. ') >= 0) && _pessoa.Numero.val().indexOf(".") < 0 && _pessoa.Numero.val().toUpperCase().indexOf('KM') < 0)
    //        address += "KM ";
    //    address += _pessoa.Numero.val() + ", ";
    //}

    //if (_pessoa.Localidade.val() != "") {
    //    address += _pessoa.Localidade.val() + ", ";
    //}

    //if (_pessoa.CEP.val() != "") {
    //    address += _pessoa.CEP.val();
    //}

    //_geocoder.geocode({ 'address': address }, function (results, status) {
    //    if (status === 'OK') {
    //        _pessoa.Latitude.val(results[0].geometry.location.lat().toString());
    //        _pessoa.Longitude.val(results[0].geometry.location.lng().toString());
    //        setarCoordenadas();
    //    } else {
    //        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS)
    //            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Pessoas.Pessoa.NaoFoiLocalizadaUmaPosicaoParaEnderecoInformado);
    //    }

    //    if (callback != null)
    //        callback();
    //});
}

function ClienteSemCoordenadas() {
    if (_pessoa.Latitude.val() == "" || _pessoa.Longitude.val() == "" || _pessoa.Latitude.val() == null || _pessoa.Longitude.val() == null)
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
            Latitude: _pessoa.Latitude.val(),
            Longitude: _pessoa.Longitude.val(),
            RaioMaximo: _CONFIGURACAO_TMS.RaioMaximoGeoLocalidadeGeoCliente
        };

        executarReST("Pessoa/ValidarRaioMaximoLatLngClienteXLocalidade", data, function (arg) {
            if (arg.Success) {

                if (arg.Msg == null || arg.Msg == '') {
                    callback();
                } else {
                    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, arg.Msg + Localization.Resources.Pessoas.Pessoa.DesejaContinuarContinuarMesmoAssim, function () {
                        callback();
                    });
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });

    }
}