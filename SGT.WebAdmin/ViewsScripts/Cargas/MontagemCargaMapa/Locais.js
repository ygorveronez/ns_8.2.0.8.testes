
var EnumLocalTipoLocal = {
    AREA_RISCO: 1,
    PERNOITE: 2,
    MICRO_REGIAO_ROTEIRIZACAO: 3,
    PONTO_DE_APOIO: 4,
    BALANCA: 5,
    PRACA_PEDAGIO: 99
};

var _MicroRegioes = [];
var _PontosDeApoio = [];
var _Balancas = [];
var _PracasPedagio = [];

function loadLogisticaLocais() {
    var filiais = CodigoFiliaisDistintasPedidos();
    var dados = { TiposLocal: JSON.stringify([EnumLocalTipoLocal.MICRO_REGIAO_ROTEIRIZACAO, EnumLocalTipoLocal.PONTO_DE_APOIO, EnumLocalTipoLocal.BALANCA]), CodigosFiliais: JSON.stringify(filiais) };
    executarReST("Locais/BuscarPorTiposEFiliais", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                for (var i = 0; i < arg.Data.length; i++) {
                    if (arg.Data[i].Tipo == EnumLocalTipoLocal.MICRO_REGIAO_ROTEIRIZACAO)
                        drawMicrosRegiaoRoteirizacao(arg.Data[i].Data);
                    else
                        drawMarkerPontosLocais(arg.Data[i].Data, arg.Data[i].Tipo);
                }
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

//function loadMicroRegiaoRoteirizacao() {
//    var filiais = CodigoFiliaisDistintasPedidos();
//    var dados = { TipoLocal: EnumLocalTipoLocal.MICRO_REGIAO_ROTEIRIZACAO, CodigosFiliais: JSON.stringify(filiais) };
//    executarReST("Locais/BuscarPorTipoEFiliais", dados, function (arg) {
//        if (arg.Success) {
//            drawMicrosRegiaoRoteirizacao(arg.Data);
//        } else {
//            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
//        }
//    });
//}

//function loadPontosDeApoio() {
//    loadPontosLocais(EnumLocalTipoLocal.PONTO_DE_APOIO);
//}

//function loadBalancas() {
//    loadPontosLocais(EnumLocalTipoLocal.BALANCA);
//}

//function loadPontosLocais(tipoLocal) {
//    var filiais = CodigoFiliaisDistintasPedidos();
//    var dados = { TipoLocal: tipoLocal, CodigosFiliais: JSON.stringify(filiais) };
//    executarReST("Locais/BuscarPorTipoEFiliais", dados, function (arg) {
//        if (arg.Success) {
//            drawMarkerPontosLocais(arg.Data, tipoLocal);
//        } else {
//            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
//        }
//    });
//}

function loadPracasPedagio() {
    executarReST("PracaPedagio/ListaPracasPedagio", {}, function (arg) {
        if (arg.Success) {
            drawMarkerPontosLocais(arg.Data, EnumLocalTipoLocal.PRACA_PEDAGIO);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function clearMicroRegioes() {
    if (_MicroRegioes) {
        for (var i = 0; i < _MicroRegioes.length; i++) {
            _MicroRegioes[i].setMap(null);
        }
        _MicroRegioes = [];
    }
}

function clearMarkerPontosDeApoio() {
    if (_PontosDeApoio) {
        for (var i = 0; i < _PontosDeApoio.length; i++) {
            _PontosDeApoio[i].setMap(null);
        }
        _PontosDeApoio = [];
    }
}

function clearMarkerBalancas() {
    if (_Balancas) {
        for (var i = 0; i < _Balancas.length; i++) {
            _Balancas[i].setMap(null);
        }
        _Balancas = [];
    }
}

function clearMarkerPracasPedagio() {
    if (_PracasPedagio) {
        for (var i = 0; i < _PracasPedagio.length; i++) {
            _PracasPedagio[i].setMap(null);
        }
        _PracasPedagio = [];
    }
}

function drawMicrosRegiaoRoteirizacao(areas) {
    clearMicroRegioes();
    for (var i = 0; i < areas.length; i++) {
        var lista = JSON.parse(areas[i].Area);
        for (var j = 0; j < lista.length; j++) {
            var microRegiao = lista[j];
            draw(microRegiao, false, null, null, false);
        }
    }
}

function drawMarkerPontosLocais(pontos, tipoLocal) {
    var icon = '';
    var pontoApoio = false;
    var balanca = false;

    if (tipoLocal == EnumLocalTipoLocal.PONTO_DE_APOIO) {
        clearMarkerPontosDeApoio();
        icon = "../img/montagem-carga-mapa/markers/ponto-apoio.png";
        pontoApoio = true;
    } else if (tipoLocal == EnumLocalTipoLocal.BALANCA) {
        clearMarkerBalancas();
        icon = "../img/montagem-carga-mapa/markers/balanca-16.png";
        balanca = true;
    }
    if (tipoLocal == EnumLocalTipoLocal.PRACA_PEDAGIO) {
        clearMarkerPracasPedagio();
        icon = "../img/montagem-carga-mapa/markers/pedagio-16.png";

        for (var i = 0; i < pontos.length; i++) {

            var info = '<h6 class="h6">' + pontos[i].Descricao + '</h6><br/>' +
                '<table style="width:100%;"><thead><tr><th>Modelo</th><th>Tarifa</th></thead><tbody>' +
                (function () {
                    return pontos[i].Tarifas.map(function (tarifa) {
                        return "<tr><td>" + tarifa.ModeloVeicularCarga + "</td><td>" + tarifa.Tarifa + "</td></tr>";
                    }).join("");
                }()) + '</tbody></table>';

            var marker = new google.maps.Marker({
                position: new google.maps.LatLng(pontos[i].Latitude, pontos[i].Longitude),
                info: _info_window,
                icon: icon,
                type: google.maps.drawing.OverlayType.MARKER,
                editable: false,
                title: pontos[i].Descricao,
                info: info,
                map: _map
            });

            marker.addListener('mouseover', function () {
                _info_window.setContent(this.info);
                _info_window.setPosition(this.position);
                _info_window.open(_map, this);
            });

            marker.addListener('mouseout', function () {
                _info_window.close();
            });

            _PracasPedagio.push(marker);
        }

    } else if (pontos) {
        for (var i = 0; i < pontos.length; i++) {
            var lista = JSON.parse(pontos[i].Area);
            for (var j = 0; j < lista.length; j++) {
                var ponto = lista[j];
                draw(ponto, pontoApoio, pontos[i].Descricao, icon, balanca);
            }
        }
    }
}

function draw(microRegiao, pontoDeApoio, title, icon, balanca) {
    var newMicroRegiao = null;
    if (microRegiao.type === google.maps.drawing.OverlayType.CIRCLE) {
        newMicroRegiao = new google.maps.Circle({
            strokeWeight: 0,
            fillColor: microRegiao.fillColor,
            fillOpacity: 0.35,
            center: microRegiao.center,
            radius: microRegiao.radius,
            zIndex: microRegiao.zIndex,
            type: microRegiao.type,
            editable: false,
            map: _map
        });

    } else if (microRegiao.type === google.maps.drawing.OverlayType.RECTANGLE) {
        newMicroRegiao = new google.maps.Rectangle({
            strokeWeight: 0,
            fillColor: microRegiao.fillColor,
            fillOpacity: 0.35,
            zIndex: microRegiao.zIndex,
            bounds: microRegiao.bounds,
            type: microRegiao.type,
            editable: false,
            map: _map
        });
    } else if (microRegiao.type === google.maps.drawing.OverlayType.POLYLINE) {
        newMicroRegiao = new google.maps.Polyline({
            strokeColor: microRegiao.strokeColor,
            strokeWeight: 3,
            strokeOpacity: 1,
            zIndex: microRegiao.zIndex,
            path: microRegiao.path,
            type: microRegiao.type,
            editable: false,
            map: _map
        });
    } else if (microRegiao.type === google.maps.drawing.OverlayType.POLYGON) {

        newMicroRegiao = new google.maps.Polygon({
            strokeWeight: 0,
            fillColor: microRegiao.fillColor,
            fillOpacity: 0.35,
            zIndex: microRegiao.zIndex,
            paths: microRegiao.paths,
            type: microRegiao.type,
            editable: false,
            map: _map
        });
    } else if (microRegiao.type === google.maps.drawing.OverlayType.MARKER) {
        newMicroRegiao = new google.maps.Marker({
            position: microRegiao.position,
            info: infowindow,
            icon: icon, //microRegiao.icon,
            type: microRegiao.type,
            editable: false,
            title: microRegiao.Descricao,
            map: _map
        });

        var titleMarker = null;

        if (microRegiao.title != null && microRegiao.title != "") {
            titleMarker = microRegiao.title;
        } else if (title != null && title != "") {
            titleMarker = title;
        }

        if (titleMarker != null) {
            var infowindow = new google.maps.InfoWindow({
                content: titleMarker
            });

            newMicroRegiao.addListener('mouseover', function () {
                infowindow.open(_map, this);
            });

            newMicroRegiao.addListener('mouseout', function () {
                infowindow.close();
            });
        }
    }

    if (pontoDeApoio || balanca) {

        if (pontoDeApoio)
            _PontosDeApoio.push(newMicroRegiao);
        else
            _Balancas.push(newMicroRegiao);

    } else {

        google.maps.event.addListener(newMicroRegiao, 'click', function (event) {
            closeInfoWindowGoogleMaps();
        });

        //Limpando carregamento ao clicar com o direito...
        google.maps.event.addListener(newMicroRegiao, 'rightclick', function (e) {
            closeInfoWindowGoogleMaps();
            iniciarNovoCarregamentoClick();
        });

        _MicroRegioes.push(newMicroRegiao);
    }
    return newMicroRegiao;
}