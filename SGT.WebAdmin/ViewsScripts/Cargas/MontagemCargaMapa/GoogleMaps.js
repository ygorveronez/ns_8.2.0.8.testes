/// <reference path="Bloco.js" />
/// <reference path="Carregamento.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="Carregamentos.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="DirecoesGoogleMaps.js" />
/// <reference path="Distancia.js" />
/// <reference path="MontagemCarga.js" />
/// <reference path="Pedido.js" />
/// <reference path="PedidoProduto.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="SimulacaoFrete.js" />
/// <reference path="../Roteirizacao/Enumerador.js" />
/// <reference path="../Roteirizacao/Marker.js" />


function ValidarSeMudouOrdemManualmente() {
    return reordenarPosicoesEntregasCarregamento();
}

var limitePorVez = 23;
var _directions = new Array();
var _makersOrdem = new Array();

function renderizarGoogleMapsCarregamento(reprocessar) {
    $("#" + _roteirizadorCarregamento.Narativa.id).html("");

    var latLngOrigem = { lat: parseFloat(_Pontos[0].coordenadas.latitude), lng: parseFloat(_Pontos[0].coordenadas.longitude) };

    var latLngDestino = null;
    var len = _Pontos.length;
    var ultimoPonto = 0;
    if (_roteirizadorCarregamento.TipoUltimoPontoRoteirizacao.val() == EnumTipoUltimoPontoRoteirizacao.AteOrigem || _roteirizadorCarregamento.TipoUltimoPontoRoteirizacao.val() == EnumTipoUltimoPontoRoteirizacao.Retornando)
        latLngDestino = { lat: parseFloat(_Pontos[0].coordenadas.latitude), lng: parseFloat(_Pontos[0].coordenadas.longitude) };
    else {
        latLngDestino = { lat: parseFloat(_Pontos[_Pontos.length - 1].coordenadas.latitude), lng: parseFloat(_Pontos[_Pontos.length - 1].coordenadas.longitude) };
        len--;
        ultimoPonto = _Pontos.length - 1;
    }

    var waypts = new Array();
    for (var i = 1; i < len; i++) {
        var lt = { lat: parseFloat(_Pontos[i].coordenadas.latitude), lng: parseFloat(_Pontos[i].coordenadas.longitude) };
        waypts.push({
            location: lt,
            stopover: true
        });
    }
    for (var i = 0; i < _directions.length; i++) {
        _directions[i].setMap(null);
    }
    _directions = new Array();

    for (var i = 0; i < _makersOrdem.length; i++) {
        _makersOrdem[i].setMap(null);
    }
    _makersOrdem = new Array();

    iniciarControleManualRequisicao();
    setarDirections(latLngOrigem, latLngDestino, waypts, ultimoPonto, false, reprocessar, 0);
}


var _mapRoteirizador;
var _directionsServiceRoteirizador;
var _directionsDisplayRoteirizador;

function loadGoogleMapa() {
    var latlng = new google.maps.LatLng(-10.861639, -53.104038);
    _mapRoteirizador = new google.maps.Map(document.getElementById(_roteirizadorCarregamento.Map.id), {
        zoom: 4,
        center: latlng,
        scaleControl: true,
        gestureHandling: 'greedy'
    });

    _directionsServiceRoteirizador = new google.maps.DirectionsService;
    _directionsDisplayRoteirizador = new google.maps.DirectionsRenderer({
        map: _mapRoteirizador,
        suppressMarkers: true
    });
}

function PossuiRestricaPrimeiraEntrega(roteirizacoes) {
    for (var i in roteirizacoes) {
        if (roteirizacoes[i].PrimeiraEntrega)
            return true;
    }

    return false;
}


function compare(a, b) {
    if (a.distancia < b.distancia)
        return -1;
    if (a.distancia > b.distancia)
        return 1;
    return 0;
}

function comparePrimeiraEntrega(a, b) {
    var entregaB = b.coordenadas.possuiPrimeiraEntrega;
    var entregaA = a.coordenadas.possuiPrimeiraEntrega;

    if (entregaA < entregaB)
        return 1;
    if (entregaA > entregaB)
        return -1;
    return 0;
}

var _ordenados;

function ObterOrdem(pontos, latOrigem, lngOrigem) {
    var distancias = new Array();
    for (var i = 1; i < pontos.length; i++) {
        if (pontos[i].coordenadas.possuiPrimeiraEntrega) {
            distancias.push({ index: i - 1, ponto: pontos[i], distancia: distancia });
            break;
        }
        var lt = { lat: parseFloat(pontos[i].coordenadas.latitude), lng: parseFloat(pontos[i].coordenadas.longitude) };
        var distancia = CalcRadiusDistance(latOrigem, lngOrigem, lt.lat, lt.lng);
        distancias.push({ index: i - 1, ponto: pontos[i], distancia: distancia });
    }

    distancias.sort(compare);
    _ordenados.push(distancias[0]);
    latOrigem = parseFloat(pontos[distancias[0].index + 1].coordenadas.latitude);
    lngOrigem = parseFloat(pontos[distancias[0].index + 1].coordenadas.longitude);
    pontos.splice((distancias[0].index) + 1, 1);

    if (pontos.length > 1) {
        ObterOrdem(pontos, latOrigem, lngOrigem);
    }
}

function gerarRoteirizacaoGoogleMaps() {
    iniciarControleManualRequisicao();

    var latLngOrigem = { lat: parseFloat(_Pontos[0].coordenadas.latitude), lng: parseFloat(_Pontos[0].coordenadas.longitude) };

    var latLngDestino = null;

    if (_roteirizadorCarregamento.TipoUltimoPontoRoteirizacao.val() == EnumTipoUltimoPontoRoteirizacao.AteOrigem) 
        latLngDestino = { lat: parseFloat(_Pontos[0].coordenadas.latitude), lng: parseFloat(_Pontos[0].coordenadas.longitude) };

    var waypts = new Array();
    _ordenados = new Array();
    var pontos = _Pontos.slice();
    pontos.splice(0, 1);
    pontos.sort(comparePrimeiraEntrega);
    pontos.unshift(_Pontos[0]);

    ObterOrdem(pontos, latLngOrigem.lat, latLngOrigem.lng);
    var orderPontos = new Array();
    orderPontos.push(_Pontos[0]);
    for (var i = 0; i < _ordenados.length; i++) {
        orderPontos.push(_ordenados[i].ponto);
    }

    _Pontos = orderPontos;
    for (var i = 1; i < _Pontos.length; i++) {
        var _ponto = _Pontos[i].coordenadas;
        if (_ponto.possuiPrimeiraEntrega) {
            latLngOrigem.lat = parseFloat(_ponto.latitude);
            latLngOrigem.lng = parseFloat(_ponto.longitude);
        } else {
            var lt = { lat: parseFloat(_ponto.latitude), lng: parseFloat(_ponto.longitude) };
            waypts.push({
                location: lt,
                stopover: true
            });
        }
    }

    if (latLngDestino != null) {
        setarDirections(latLngOrigem, latLngDestino, waypts, 0, true, true, 0);
    } else {
        indexMaiordistancia = waypts.length - 1;
        latLngDestino = { lat: parseFloat(_Pontos[indexMaiordistancia + 1].coordenadas.latitude), lng: parseFloat(_Pontos[indexMaiordistancia + 1].coordenadas.longitude) };
        waypts.splice(indexMaiordistancia, 1);
        setarDirections(latLngOrigem, latLngDestino, waypts, indexMaiordistancia + 1, true, true, 0);
    }
}

function setarDirections(latLngOrigem, latLngDestino, waypts, indexUltimoPonto, otimizar, preecherRetorno, limite, responseArray) {
    if (responseArray == null)
        responseArray = new Array();

    var limitWaypts = new Array();
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

    _directionsServiceRoteirizador.route({
        origin: origem,
        destination: destino,
        waypoints: limitWaypts,
        optimizeWaypoints: otimizar,
        travelMode: 'DRIVING'
    }, function (response, status) {
        if (status === google.maps.DirectionsStatus.OK) {
            responseArray.push(response);
            if (limite < waypts.length) {
                setarDirections(latLngOrigem, latLngDestino, waypts, indexUltimoPonto, otimizar, preecherRetorno, limite, responseArray);
            } else {
                if (!otimizar) {
                    var seq = 0;
                    for (var j = 0; j < responseArray.length; j++) {
                        response = responseArray[j];
                        var directionsDisplayRoteirizador = new google.maps.DirectionsRenderer({
                            map: _mapRoteirizador,
                            preserveViewport: true,
                            suppressMarkers: true,
                            draggable: true
                        });

                        directionsDisplayRoteirizador.addListener('directions_changed', function () {
                            var res = directionsDisplayRoteirizador.getDirections();
                            computeTotalDistance(responseArray, res);
                        });

                        directionsDisplayRoteirizador.setDirections(response);
                        directionsDisplayRoteirizador.setPanel(document.getElementById(_roteirizadorCarregamento.Narativa.id));
                        _directions.push(directionsDisplayRoteirizador);

                        for (var i = 0; i < response.routes[0].legs.length; i++) {

                            var leg = response.routes[0].legs[i];
                            if (seq == 0) {
                                makeMarker(leg.start_location, seq, "img/map_partida.png");
                            } else {
                                makeMarker(leg.start_location, seq);
                            }

                            if (responseArray.length - 1 == j) {
                                if (response.routes[0].legs.length - 1 == i) {
                                    if (_roteirizadorCarregamento.TipoUltimoPontoRoteirizacao.val() != EnumTipoUltimoPontoRoteirizacao.AteOrigem || _roteirizadorCarregamento.TipoUltimoPontoRoteirizacao.val() != EnumTipoUltimoPontoRoteirizacao.Retornando)
                                        makeMarker(leg.end_location, (seq + 1));
                                }
                            }
                            seq++;

                        }
                    }

                    var bounds = new google.maps.LatLngBounds();

                    var locor = new google.maps.LatLng(latLngOrigem.lat, latLngOrigem.lng);
                    bounds.extend(locor);
                    for (var i = 0; i < waypts.length; i++) {
                        var loc = new google.maps.LatLng(waypts[i].location.lat, waypts[i].location.lng);
                        bounds.extend(loc);
                    }

                    var locdes = new google.maps.LatLng(latLngDestino.lat, latLngDestino.lng);
                    bounds.extend(locdes);
                    _mapRoteirizador.fitBounds(bounds);
                }

                if (preecherRetorno) {
                    preencherRetornoRoteirizacao(responseArray, indexUltimoPonto, otimizar);
                }
                finalizarControleManualRequisicao();
            }
        } else {
            var msg = MensagemDirectionsStatus(status);
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.MontagemCargaMapa.ErroAoRoteirizar, msg);
            finalizarControleManualRequisicao();
        }
    });
}

function MensagemDirectionsStatus(status) {
    switch (status) {
        case google.maps.DirectionsStatus.NOT_FOUND: return Localization.Resources.Cargas.MontagemCargaMapa.ServidorAPINaoConseguiuResolverAlgumDosPontosInformadosNaRota;
        case google.maps.DirectionsStatus.ZERO_RESULTS: return Localization.Resources.Cargas.MontagemCargaMapa.NaoFoiEncontradaNenhumaRotaEntreOrigemDestinoVerifiqueOsPontosQueCompoeCarregamento;
        case google.maps.DirectionsStatus.MAX_WAYPOINTS_EXCEEDED: return Localization.Resources.Cargas.MontagemCargaMapa.QuantidadeDePontosParaRoterizacaoFoiExcedida;
        case google.maps.DirectionsStatus.MAX_ROUTE_LENGTH_EXCEEDED: return Localization.Resources.Cargas.MontagemCargaMapa.RotaMuitoLongaOuCOmplexaNaoPodeSerProcessadaPeloServidorAPI;
        case google.maps.DirectionsStatus.INVALID_REQUEST: return Localization.Resources.Cargas.MontagemCargaMapa.RotaInvalidaParaProcessamento;
        case google.maps.DirectionsStatus.OVER_QUERY_LIMIT: return Localization.Resources.Cargas.MontagemCargaMapa.PaginaUltrapassouLimiteDeRequisicoesDentroDoPeriodoPermitido;
        case google.maps.DirectionsStatus.REQUEST_DENIED: return Localization.Resources.Cargas.MontagemCargaMapa.PaginaEstaSemPermissaoParaUsarServico;
        default: return Localization.Resources.Cargas.MontagemCargaMapa.ServidorRetornouUmaSituacaoInesperada;
    }
}

function computeTotalDistance(responseArray, res) {
    var total = 0;
    var myroute = res.routes[0];
    for (var i = 0; i < myroute.legs.length; i++) {
        total += myroute.legs[i].distance.value;
    }
    _roteirizadorCarregamento.Distancia.val(Globalize.format(total / 1000, "n2") + " km");
}

function makeMarker(position, index, icon) {
    var possuiRestricao = _Pontos[index] != null && _Pontos[index].coordenadas != null && _Pontos[index].coordenadas.possuiPrimeiraEntrega;
    
    if (!icon) {
        var pinColor = CorMarkerPorTipo(possuiRestricao ? _EnumMarker.PinRestricao : _EnumMarker.Pin);
        var svg = ObterSVGPin(pinColor, index);
        icon = IconSVGMarker(svg);
    }

    var marker = new google.maps.Marker({
        position: position,
        map: _mapRoteirizador,
        icon: icon
    });

    _makersOrdem.push(marker);

    if (possuiRestricao) {
        var arrayDescricoes = _Pontos[index].coordenadas.RestricoesEntregas.map(function (r) { return r.Descricao });
        var restricoes = "<strong>" + Localization.Resources.Cargas.MontagemCargaMapa.RestricaoDeEntrega + ":</strong><br/> " + arrayDescricoes.join("<br/>");
        var infowindowRestricao = new google.maps.InfoWindow({
            content: "<div style='padding:2px;'>" + restricoes + "</div>"
        });

        marker.addListener('click', function () {
            infowindowRestricao.open(marker.get('map'), marker);
        });
    }
}
