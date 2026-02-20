////// <autosync enabled="true" />
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
/// <reference path="../../Roteirizacao/Enumerador.js" />
/// <reference path="../../Roteirizacao/Marker.js" />

var map;
var directionsDisplay;
var limitePorVez = 23;
var _directionsService;
var _directions = [];
var _makersOrdem = [];

function renderizarGoogleMaps() {
    if (!_mapaRenderizado) {
        setTimeout(function () {
            directionsDisplay = new google.maps.DirectionsRenderer();
            _directionsService = new google.maps.DirectionsService();
            var latlng = new google.maps.LatLng(-10.861639, -53.104038);
            var options = {
                zoom: 4,
                center: latlng,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };
            map = new google.maps.Map(document.getElementById(_roteirizador.Map.id), options);
            directionsDisplay.setMap(map);
            roterizarGoogleMaps();
            _mapaRenderizado = true;
            //reordenarPosicoesEntregas()
        }, 200);
    } else {
        //if (verificarMudouPosicao()) {
            //locations = reordenarPosicoesEntregas();
            roterizarGoogleMaps();
        //}
    }
}

function roterizarGoogleMaps() {
    var reprocessar = true;
    var quantidade = locations.length;
    var ultimoIndex = quantidade - 1;
    var ultimoPonto = 0;
    var indexDestino;

    if (_roteirizador.TipoUltimoPontoRoteirizacao.val() == EnumTipoUltimoPontoRoteirizacao.AteOrigem || _roteirizador.TipoUltimoPontoRoteirizacao.val() == EnumTipoUltimoPontoRoteirizacao.Retornando)
        indexDestino = 0;
    else {
        quantidade--;
        indexDestino = ultimoIndex;
        ultimoPonto = ultimoIndex;
    }
    var latLngOrigem = { lat: parseFloat(locations[0].latLng.lat), lng: parseFloat(locations[0].latLng.lng) };
    var latLngDestino = { lat: parseFloat(locations[indexDestino].latLng.lat), lng: parseFloat(locations[indexDestino].latLng.lng) };

    var waypts = []
    for (var i = 1; i < quantidade; i++) {
        waypts.push({
            location: {
                lat: parseFloat(locations[i].latLng.lat),
                lng: parseFloat(locations[i].latLng.lng)
            },
            stopover: true
        });
    }

    for (var i = 0; i < _directions.length; i++) {
        _directions[i].setPanel(null);
        _directions[i].setMap(null);
    }
    for (var i = 0; i < _makersOrdem.length; i++) {
        _makersOrdem[i].setMap(null);
    }
    _makersOrdem = [];
    _directions = [];

    iniciarControleManualRequisicao();
    setarDirections(latLngOrigem, latLngDestino, waypts, ultimoPonto, reprocessar, 0);
}


function setarDirections(latLngOrigem, latLngDestino, waypts, indexUltimoPonto, preecherRetorno, limite, responseArray) {
    if (responseArray == null)
        responseArray = [];

    var limitWaypts = [];
    var destino = latLngDestino;
    var origem = latLngOrigem;

    var indexInicio = limite;
    var limite = limite + limitePorVez;

    if (limite >= waypts.length) {
        limite = waypts.length;
    }
    else {
        if (indexInicio > 0)
            limite++;

        if (limite < waypts.length)
            destino = waypts[limite].location;
    }

    if (indexInicio > 0) {
        origem = waypts[indexInicio].location;
        indexInicio++;
    }

    for (var i = indexInicio; i < limite; i++) {
        limitWaypts.push(waypts[i]);
    }

    var _routeOptions = {
        origin: origem,
        destination: destino,
        waypoints: limitWaypts,
        optimizeWaypoints: false,
        travelMode: google.maps.TravelMode.DRIVING
    };

    _directionsService.route(_routeOptions, function (response, status) {
        if (status === google.maps.DirectionsStatus.OK) {
            responseArray.push(response)

            if (limite < waypts.length) {
                setarDirections(latLngOrigem, latLngDestino, waypts, indexUltimoPonto, preecherRetorno, limite, responseArray);
            } else {
                var seq = 0;
                var bounds = new google.maps.LatLngBounds();
                var localidadeOrigem = new google.maps.LatLng(latLngOrigem.lat, latLngOrigem.lng);
                var localidadeDestino = new google.maps.LatLng(latLngDestino.lat, latLngDestino.lng);
                var ateOrigemOuRetornando = _roteirizador.TipoUltimoPontoRoteirizacao.val() != EnumTipoUltimoPontoRoteirizacao.AteOrigem || _roteirizador.TipoUltimoPontoRoteirizacao.val() != EnumTipoUltimoPontoRoteirizacao.Retornando;

                for (var j = 0; j < responseArray.length; j++) {
                    var directionPoint = responseArray[j];
                    var directionsDisplayRoteirizador = new google.maps.DirectionsRenderer({
                        map: map,
                        preserveViewport: true,
                        suppressMarkers: true,
                        draggable: false
                    });

                    directionsDisplayRoteirizador.setDirections(directionPoint);
                    _directions.push(directionsDisplayRoteirizador);

                    for (var i = 0; i < directionPoint.routes[0].legs.length; i++) {
                        var leg = directionPoint.routes[0].legs[i];
                        if (seq == 0) {
                            makeMarker(leg.start_location, seq, "img/map_partida.png");
                        } else {
                            makeMarker(leg.start_location, seq);
                        }

                        if (responseArray.length - 1 == j && directionPoint.routes[0].legs.length - 1 == i && ateOrigemOuRetornando) {
                            makeMarker(leg.end_location, (seq + 1));
                        }
                        seq++;
                    }
                    directionsDisplayRoteirizador.setPanel(document.getElementById(_roteirizador.Narativa.id));
                }
                //directionsDisplayRoteirizador.setPanel(document.getElementById(_roteirizador.Narativa.id));

                bounds.extend(localidadeOrigem);
                for (var i = 0; i < waypts.length; i++) {
                    var localidadeWayPoint = new google.maps.LatLng(waypts[i].location.lat, waypts[i].location.lng);
                    bounds.extend(localidadeWayPoint);
                }
                bounds.extend(localidadeDestino);
                map.fitBounds(bounds);

                if (preecherRetorno)
                    preencherRetornoRoteirizacao(responseArray, indexUltimoPonto);

                finalizarControleManualRequisicao();
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.Carga.ErroAoRoterizar, Localization.Resources.Cargas.Carga.ServidorRetornou.format(status));
            finalizarControleManualRequisicao();
        }
    });
}

function makeMarker(position, index, icon) {
    var possuiRestricao = locations[index] != null && locations[index].coordenadas != null && locations[index].coordenadas.possuiPrimeiraEntrega;

    if (!icon) {
        var pinColor = CorMarkerPorTipo(possuiRestricao ? EnumTipoMarker.PinRestricao : EnumTipoMarker.Pin);
        var svg = ObterSVGPin(pinColor, index);
        icon = IconSVGMarker(svg);
    }

    var marker = new google.maps.Marker({
        position: position,
        map: map,
        icon: icon
    });

    _makersOrdem.push(marker);

    if (possuiRestricao) {
        var arrayDescricoes = locations[index].coordenadas.RestricoesEntregas.map(function (r) { return r.Descricao });
        var restricoes = "<strong>Restrição de entrega:</strong><br/> " + arrayDescricoes.join("<br/>");
        var infowindowRestricao = new google.maps.InfoWindow({
            content: "<div style='padding:2px;'>" + restricoes + "</div>"
        });

        marker.addListener('click', function () {
            infowindowRestricao.open(marker.get('map'), marker);
        });
    }
}