/*GridMonitoramento.js*/
/// <reference path="../../Logistica/Tracking/Tracking.lib.js" />

var _gridControleEntregMonitoramento;
var _mapaControleEntrega;
var _mapaModalControleEntrega;
var _indexStartReproduzirRota;
var _ultimoIndicePolilinhaReproduzirRota;

var MapaModalControleEntrega = function () {
    this.ReproduzirRota = PropertyEntity({ visible: ko.observable(true), enable: ko.observable(true), eventClick: reproduzirRotaClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.ControleEntrega.ReproduzirRota) });
    this.FinalizarReproducao = PropertyEntity({ visible: ko.observable(false), eventClick: resetarRotaClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.ControleEntrega.Finalizar) });
    this.PausarReproduzirRotaCadaPonto = PropertyEntity({ visible: ko.observable(true), enable: ko.observable(true), type: types.event, text: Localization.Resources.Cargas.ControleEntrega.PausarCadaDestino, val: ko.observable(false) });
    //this.CentralizarMapaReproduzirRota = PropertyEntity({ visible: ko.observable(false), enable: ko.observable(true), type: types.event, text: "Centralizar ao reproduzir ?", val: ko.observable(true) });
    this.SpeedReproduzirRota = PropertyEntity({ visible: ko.observable(true), enable: ko.observable(true), type: types.event, text: Localization.Resources.Cargas.ControleEntrega.VelocidadeAoReproduzir.getFieldDescription(), val: ko.observable(2.5), enable: ko.observable(true) });
    this.LegendaOrigem = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Origem, src: "http://maps.google.com/mapfiles/kml/pal2/icon13.png", width: 22 });
    this.LegendaDestino = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DestinoColetaOuEntrega, src: MapaObterIconSVGMarcador(MapaObterSVGPin("#D45B5B", "")), width: 14 });
    this.Reproduzindo = PropertyEntity({ val: ko.observable(false) });
}

function loadMapaModalControleEntrega() {
    _mapaModalControleEntrega = new MapaModalControleEntrega();
    KoBindings(_mapaModalControleEntrega, "knockouMapaModalControleEntrega");
}

function loadMapaControleEntrega() {
    if (!_mapaControleEntrega) {
        var opcoesmapa = new OpcoesMapa(false, false);
        _mapaControleEntrega = new MapaGoogle("map", false, opcoesmapa);
    }
}

function exibirModalMapaControleEntrega() {
    Global.abrirModal("divModalMapaControleEntrega");
    $("#divModalMapaControleEntrega").one('hidden.bs.modal', function () {
        _mapaControleEntrega.direction.limparMapa();
        _ultimoIndicePolilinhaReproduzirRota = 0;
        resetItensMapa();
    });
}

function criarMakerVeiculoControleEntrega(info) {
    if ((typeof info.Veiculo.Latitude) == "string")
        info.Veiculo.Latitude = Globalize.parseFloat(info.Veiculo.Latitude);

    if ((typeof info.Veiculo.Longitude) == "string")
        info.Veiculo.Longitude = Globalize.parseFloat(info.Veiculo.Longitude);

    if (info.Latitude == 0 || info.Longitude == 0)
        return;

    if (info.Veiculo.Latitude == 0 && info.Veiculo.Longitude == 0) {
        info.Veiculo.Latitude = info.Latitude;
        info.Veiculo.Longitude = info.Longitude;
        info.Veiculo.Data = info.Data;
    }

    var icone = TrackingIconRastreador(info.online);

    var marker = new ShapeMarker();
    marker.setPosition(info.Latitude, info.Longitude);
    marker.icon = _mapaControleEntrega.draw.icons.truck();
    marker.title = info.Veiculo.PlacaVeiculo;
    marker.content =
        '<div style="width: 170px; float: left;"><strong>' + Localization.Resources.Cargas.ControleEntrega.Veiculo.getFieldDescription() + ' ' + info.Veiculo.PlacaVeiculo + '</strong></div>' + '<div class="tracking-indicador" style="float: right; width: 50px; margin-top:5px" title="' + info.Data + '">' + icone + '</div>' +
        '<div>' + Localization.Resources.Cargas.ControleEntrega.Data.getFieldDescription() + ' ' + info.Veiculo.Data + '</div>' +
        '<div>' + Localization.Resources.Cargas.ControleEntrega.Coordenadas.getFieldDescription() + ' ' + info.Veiculo.Latitude + '; ' + info.Veiculo.Longitude + '<div>';
        '<div> Rastreador: ' + info.Rastreador + '<div>';
    var maker = _mapaControleEntrega.draw.addShape(marker, false, "Click");
    maker.AbrirInfo();
}

function carregarDadosMapaControleEntrega(filaselecionada) {
    _mapaControleEntrega.clear();
    var data = { Carga: filaselecionada.Carga, Veiculo: filaselecionada.Veiculo, IDEquipamento: filaselecionada.IDEquipamento };
    executarReST("Monitoramento/ObterDadosMapa", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                setTimeout(function () {
                    _mapaControleEntrega.direction.desenharPolilinha(arg.Data.PolilinhaPrevista);
                    _mapaControleEntrega.direction.desenharPolilinha(arg.Data.PolilinhaRealizada, false, "#FF6961");
                    _mapaControleEntrega.PolilinhaPrevista = arg.Data.PolilinhaPrevista;
                    _mapaControleEntrega.PolilinhaRealizada = arg.Data.PolilinhaRealizada;
                    TrackingDesenharEntregasMonitoramento(_mapaControleEntrega, arg.Data.Entregas);
                    desenharAreasControleEntregaMonitoramento(_mapaControleEntrega, arg.Data.Areas);
                    criarMakerVeiculoControleEntrega(arg.Data);
                }, 500);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function visualizarDadosMapaControleEntregaClick(dadosMapa) {
    exibirModalMapaControleEntrega()
    loadMapaControleEntrega();
    loadMapaModalControleEntrega();

    carregarDadosMapaControleEntrega(dadosMapa);
}

function visualizarHistoricoContoleEntregaPosicaoMapaClick(filaSelecionada) {
    var dataInicial = Global.DataHora(EnumTipoOperacaoDate.Subtract, 2, EnumTipoOperacaoObjetoDate.Hours);
    _pesquisaHistoricoPosicao.DataInicial.val(dataInicial),
        _pesquisaHistoricoPosicao.DataFinal.val(Global.DataHoraAtual())

    ExibirModalMapaHistoricoPosicao()
    loadMapaHistoricoPosicao();

    carregarDadosMapaHistoricoPosicao();
}

function criarPoligon(mapaAreas, obj) {
    var shapePolygon = new ShapePolygon();
    shapePolygon.fillColor = obj.fillColor;
    shapePolygon.paths = obj.paths;
    shapePolygon.zIndex = obj.zIndex;
    mapaAreas.draw.addShape(shapePolygon);
}

function criarRectangle(mapaAreas, obj) {
    var shapeRectangle = new ShapeRectangle();
    shapeRectangle.fillColor = obj.fillColor;
    shapeRectangle.bounds = obj.bounds;
    shapeRectangle.zIndex = obj.zIndex;
    mapaAreas.draw.addShape(shapeRectangle);
}

function criarCircle(mapaAreas, area) {
    var latLngNormal = { lat: parseFloat(area.Latitude), lng: parseFloat(area.Longitude) };
    var shapeCircle = new ShapeCircle();
    shapeCircle.type = google.maps.drawing.OverlayType.CIRCLE;
    shapeCircle.fillColor = "#1E90FF";
    shapeCircle.radius = parseInt(area.Raio);
    shapeCircle.center = latLngNormal

    mapaAreas.draw.addShape(shapeCircle);
}

function desenharAreasControleEntregaMonitoramento(mapaAreas, Areas) {
    if ((Areas === null) || (Areas === undefined))
        return

    for (var i = 0; i < Areas.length; i++) {

        area = Areas[i];

        if (area.TipoArea == 2 && area.Area != "") {

            var objArea = JSON.parse(area.Area);

            for (var j = 0; j < objArea.length; j++) {
                obj = objArea[j];

                if (obj.type == google.maps.drawing.OverlayType.POLYGON)
                    criarPoligon(mapaAreas, obj);

                else if (obj.type == google.maps.drawing.OverlayType.RECTANGLE)
                    criarRectangle(mapaAreas, obj);
            }

        }
        else {
            criarCircle(mapaAreas, area);
        }
    }
}

function obterDistancia(lat1, lon1, lat2, lon2, unit) {
    if ((lat1 == lat2) && (lon1 == lon2)) {
        return 0;
    }
    else {
        var radlat1 = Math.PI * lat1 / 180;
        var radlat2 = Math.PI * lat2 / 180;
        var theta = lon1 - lon2;
        var radtheta = Math.PI * theta / 180;
        var dist = Math.sin(radlat1) * Math.sin(radlat2) + Math.cos(radlat1) * Math.cos(radlat2) * Math.cos(radtheta);
        if (dist > 1) {
            dist = 1;
        }
        dist = Math.acos(dist);
        dist = dist * 180 / Math.PI;
        dist = dist * 60 * 1.1515;
        if (unit == "K") { dist = dist * 1.609344 }
        if (unit == "N") { dist = dist * 0.8684 }
        return dist;
    }
}

function obterSleep(pontos) {
    if (pontos < 100)
        return 200;
    if (pontos < 1000)
        return 150;
    if (pontos < 5000)
        return 120;
    if (pontos < 10000)
        return 80;
    if (pontos < 15000)
        return 50;
    return 20;
}

function obterDistanciaMinima(pontos) {
    if (pontos < 100)
        return 0.005;
    if (pontos < 500)
        return 0.009;
    if (pontos < 1000)
        return 0.015;
    if (pontos < 10000)
        return 0.07;
    if (pontos < 20000)
        return 0.1;
    if (pontos < 30000)
        return 0.20;
    return 0.3;
}

function opcoesReproduzirRota(reproduzir) {
    if (reproduzir)
        _mapaModalControleEntrega.ReproduzirRota.text(Localization.Resources.Cargas.ControleEntrega.Pausar);
    else {
        if (_ultimoIndicePolilinhaReproduzirRota == 0)
            _mapaModalControleEntrega.ReproduzirRota.text(Localization.Resources.Cargas.ControleEntrega.ReproduzirRota);
        else
            _mapaModalControleEntrega.ReproduzirRota.text(Localization.Resources.Cargas.ControleEntrega.Continuar);
    }
    _mapaModalControleEntrega.Reproduzindo.val(reproduzir);
    _mapaModalControleEntrega.FinalizarReproducao.visible(reproduzir);
    _mapaModalControleEntrega.PausarReproduzirRotaCadaPonto.enable(!reproduzir);
    //_mapaModalControleEntrega.CentralizarMapaReproduzirRota.enable(!reproduzir);
    _mapaModalControleEntrega.SpeedReproduzirRota.enable(!reproduzir);
}

function obterIndicesMaisProximoCadaEntrega(realizada) {
    var indicesPolilinhaEntregas = new Array();
    for (var i = 0; i < TRACKING_ENTREGAS.length; i++) {
        var indice = -1;
        var menor = 999999999;
        if ((realizada == false || TRACKING_ENTREGAS[i].Situacao == 2) && TRACKING_ENTREGAS[i].OrdemPrevista > 0) {
            for (var j = 0; j < _pathCoords.length; j++) {
                var dist = obterDistancia(_pathCoords[j].lat(), _pathCoords[j].lng(), TRACKING_ENTREGAS[i].Latitude, TRACKING_ENTREGAS[i].Longitude, "K");
                if (dist < menor) {
                    menor = dist;
                    indice = j;
                }
            }
        }
        if (indice >= 0) {
            indicesPolilinhaEntregas.push({ index: indice, index_marker: i });
        }
    }
    // Ordenando de acordo com a polilinha...
    // return indicesPolilinhaEntregas;
    return indicesPolilinhaEntregas.sort(function (a, b) {
        return a.index - b.index;
    });
}

function resetarRotaClick() {
    _timer && _timer.cancel();
    _timer = null;
    _lineCar.setPath(_pathCoords)
    _markerCar.setMap(null);
    finalizouSimulacaoRota(true);
}

function reproduzirRotaClick() {

    if (_mapaModalControleEntrega.Reproduzindo.val()) {
        _timer && _timer.pause();
        opcoesReproduzirRota(false);
        return;
    }

    var realizada = true;
    var polilinha = _mapaControleEntrega.PolilinhaRealizada;

    if (polilinha == "" || polilinha == undefined) {
        polilinha = _mapaControleEntrega.PolilinhaPrevista;
        realizada = false;
    }

    _pathCoords = google.maps.geometry.encoding.decodePath(polilinha);

    if (_pathCoords.length == 0)
        return;

    if (_pathCoords.length > 0) {

        var indicesPolilinhaEntregas = obterIndicesMaisProximoCadaEntrega(realizada);

        opcoesReproduzirRota(true);

        finalizouSimulacaoRota(false, false);

        if (_indexStartReproduzirRota == 0 || _indexStartReproduzirRota == undefined) {
            _indexStartReproduzirRota = 1;
            _ultimoIndicePolilinhaReproduzirRota = 0;
        }

        var latIni = _pathCoords[0].lat();
        var lngIni = _pathCoords[0].lng();

        if (_ultimoIndicePolilinhaReproduzirRota == 0) {

            resetItensMapa();

            var configMaker = new ShapeMarker();
            configMaker.setPosition(latIni, lngIni);
            configMaker.icon = _mapaControleEntrega.draw.icons.truck();

            _markerCar = _mapaControleEntrega.draw.addShape(configMaker);

            _lineCar = new google.maps.Polyline({
                path: [],
                strokeColor: "#FF0000",
                strokeOpacity: 1.0,
                strokeWeight: 5,// 2,
                geodesic: true, //set to false if you want straight line instead of arc
                map: _mapaControleEntrega.direction.getMap(),
                zIndex: 1000
            });
        }

        var minDist = obterDistanciaMinima(_pathCoords.length);
        var minSleep = obterSleep(_pathCoords.length);

        recursiveAnimate(_ultimoIndicePolilinhaReproduzirRota, minDist, minSleep, indicesPolilinhaEntregas);
    }
}

function renderizarGridMonitoramento(element, data) {
    var dataGrid = data.GridMonitoramento.val();

    if (dataGrid) {
        var gridMonitoramento = new BasicDataTable(data.GridMonitoramento.id, dataGrid.header, null, null, null, 100, false, false, null, null, false, false, null, gridMonitoramentoCallbackRow, gridMonitoramentoCallbackColumnDefault);

        gridMonitoramento.setTamanhoPadraoPorColuna(200);
        gridMonitoramento.CarregarGrid(dataGrid.data);
    }
}


/* REPRODUZIR NOVO */

var _timer;
var _pathCoords;
var _lineCar;

function resetItensMapa() {
    if (_markerCar)
        _markerCar.setMap(null);

    if (_lineCar) {
        _lineCar.getPath().clear();
        _lineCar.setMap(null);
    }
}

function finalizouSimulacaoRota(resetIndiceEntrega, resetarItensMapa) {

    TrackingResetMarkersIcons();

    if (resetarItensMapa)
        resetItensMapa();

    if (resetIndiceEntrega) {
        _indexStartReproduzirRota = 0;
        _ultimoIndicePolilinhaReproduzirRota = 0;
    }

    if (resetIndiceEntrega) {
        opcoesReproduzirRota(false);
    }
}

var _markerCar;
var timePerStep = 1;

function recursiveAnimate(index, minDist, minSleep, indicesPolilinhaEntregas) {

    if (!_mapaModalControleEntrega.Reproduzindo.val())
        return;

    _timer && _timer.cancel()
    var coordsDeparture = _pathCoords[index];
    var coordsArrival = _pathCoords[index + 1];

    _ultimoIndicePolilinhaReproduzirRota = index;

    var dist = obterDistancia(coordsDeparture.lat(), coordsDeparture.lng(), coordsArrival.lat(), coordsArrival.lng(), "K");

    if (dist >= minDist) {
        //validar a distancia do próximo destino
        if (_indexStartReproduzirRota <= indicesPolilinhaEntregas.length) {
            if (_ultimoIndicePolilinhaReproduzirRota >= indicesPolilinhaEntregas[_indexStartReproduzirRota - 1].index) {
                //Dar o click do botãozinho
                _indexStartReproduzirRota++;
                if (_mapaModalControleEntrega.PausarReproduzirRotaCadaPonto.val()) {
                    var index_marker = indicesPolilinhaEntregas[_indexStartReproduzirRota - 2].index_marker;
                    TrackingMarkerSetIconVisitado(index_marker);
                    TrackingMarkerClick(index_marker);
                    opcoesReproduzirRota(false);
                    _timer && _timer.pause();
                    return;
                }
            }
        }
        var departure = new google.maps.LatLng(coordsDeparture.lat(), coordsDeparture.lng()); //Set to whatever lat/lng you need for your departure location
        var arrival = new google.maps.LatLng(coordsArrival.lat(), coordsArrival.lng()); //Set to whatever lat/lng you need for your arrival location

        var step = 0;
        var numSteps = 20; //Change this to set animation resolution
        var timePerStep = 5.1 - parseFloat(_mapaModalControleEntrega.SpeedReproduzirRota.val()); // 3; //Change this to alter animation speed

        _timer = InvervalTimer(function (arg) {
            step += 1;
            if (step > numSteps) {
                //clearInterval(interval);
                step = 0
                _timer.cancel()
                if (index < _pathCoords.length - 2) {
                    recursiveAnimate(index + 1, minDist, minSleep, indicesPolilinhaEntregas);
                } else {
                    finalizouSimulacaoRota(true, true);
                }
            } else {
                var are_we_there_yet = google.maps.geometry.spherical.interpolate(departure, arrival, step / numSteps);
                _lineCar.getPath().push(are_we_there_yet);
                moveMarker(_mapaControleEntrega.direction.getMap(), _markerCar, departure, are_we_there_yet)
            }
        }, timePerStep);

    } else {
        if (index < _pathCoords.length - 2) {
            recursiveAnimate(index + 1, minDist, minSleep, indicesPolilinhaEntregas);
        } else {
            finalizouSimulacaoRota(true, true);
        }
    }
}

function moveMarker(map, marker, departure, currentMarkerPos) {
    marker.setPosition(currentMarkerPos);
    map.panTo(currentMarkerPos);
    var heading = google.maps.geometry.spherical.computeHeading(departure, currentMarkerPos);
    //_iconCar.rotation = heading;
    //marker.setIcon(_iconCar);
}

function InvervalTimer(callback, interval, arg) {

    var timerId, startTime, remaining = 0;
    var state = 0; //  0 = idle, 1 = running, 2 = paused, 3= resumed
    var timeoutId
    this.pause = function () {
        if (state != 1) return;

        remaining = interval - (new Date() - startTime);
        window.clearInterval(timerId);
        state = 2;
    };

    this.resume = function () {
        if (state != 2) return;

        state = 3;
        timeoutId = window.setTimeout(this.timeoutCallback, remaining, arg);
    };

    this.timeoutCallback = function (_timer) {
        if (state != 3) return;
        clearTimeout(timeoutId);
        startTime = new Date();
        timerId = window.setInterval(function () {
            callback(arg)
        }, interval);
        state = 1;
    };

    this.cancel = function () {
        clearInterval(timerId)
    }

    startTime = new Date();
    timerId = window.setInterval(function () {
        callback(arg)
    }, interval);

    state = 1;

    return {
        cancel: cancel,
        pause: pause,
        resume: resume,
        timeoutCallback: timeoutCallback
    };
}