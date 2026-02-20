/*Tracking.lib.js*/

var TRACKING_IGNICAO_COR_LIGADO = "#33cc33";
var TRACKING_IGNICAO_COR_DELIGADO = "#e74c3c";

var TRACKING_ROTA_COR_PLANEJADA = "#00BFFF";
var TRACKING_ROTA_COR_REALIZADA = "#FF6961";
var TRACKING_ROTA_COR_ATE_ORIGEM = "#00EA00";
var TRACKING_ROTA_COR_ATE_DESTINO = "#00BC00";

var TRACKING_MONITORAMENTO_CRITICO_COR = "#FF0000";
var TRACKING_MONITORAMENTO_CRITICO_COR_BACKGROUND = "#FFF0F0";

var TRACKING_ENTREGA_PENDENTE_COR = "#D45B5B";
var TRACKING_ENTREGA_EN_ANDAMENTO_COR = "#DED84C";
var TRACKING_ENTREGA_REALIZADA_COR = "#5ECC71";
var TRACKING_ENTREGA_VISITADO = "#000";

var TRACKING_ENTREGAS = new Array();
var TRACKING_ENTREGA_MARKERS = new Array();

var _mapTracking;

function TrackingIconRastreador(online) {
    var typeofOnline = typeof online;
    var isOnline = ((typeofOnline === "boolean" && online) || (typeofOnline == "string" && online.toLowerCase() == "true"))
    var color = (isOnline) ? TRACKING_RASTREADOR_COR_ONLINE : TRACKING_RASTREADOR_COR_OFFLINE;
    var icon = TrackingIconWifi(color);
    return icon;
}

function TrackingIconIgnicao(on) {
    var typeofOn = typeof on;
    var isOn = ((typeofOn === "boolean" && on) || (typeofOn == "string" && on.toLowerCase() == "true"))
    var color = (isOn) ? TRACKING_IGNICAO_COR_LIGADO : TRACKING_IGNICAO_COR_DELIGADO;
    var icon = TrackingIconOnOff(color);
    return icon;
}

function TrackingIconOnOff(color) {
    var icon =
        ' <svg xmlns="http://www.w3.org/2000/svg" x="0px" y="0px" ' +
        ' width="20" height="20" ' +
        ' viewBox="0 0 172 172" ' +
        ' style=" fill:#000000;"><g fill="none" fill-rule="nonzero" stroke="none" stroke-width="1" stroke-linecap="butt" stroke-linejoin="miter" stroke-miterlimit="10" stroke-dasharray="" stroke-dashoffset="0" font-family="none" font-weight="none" font-size="none" text-anchor="none" style="mix-blend-mode: normal"><path d="M0,172v-172h172v172z" fill="none"></path>' +
        ' <g fill="' + color + '"><path d="M86,14.33333c-39.5815,0 -71.66667,32.08517 -71.66667,71.66667c0,39.5815 32.08517,71.66667 71.66667,71.66667c39.5815,0 71.66667,-32.08517 71.66667,-71.66667c0,-39.5815 -32.08517,-71.66667 -71.66667,-71.66667zM78.83333,28.66667h14.33333v57.33333h-14.33333zM86,143.33333c-31.61217,0 -57.33333,-25.72117 -57.33333,-57.33333c0,-24.00833 14.84933,-44.58383 35.83333,-53.11217v15.9315c-12.82833,7.44617 -21.5,21.3065 -21.5,37.18067c0,23.7145 19.2855,43 43,43c23.7145,0 43,-19.2855 43,-43c0,-15.87417 -8.67167,-29.7345 -21.5,-37.18067v-15.9315c20.984,8.52833 35.83333,29.10383 35.83333,53.11217c0,31.61217 -25.72117,57.33333 -57.33333,57.33333z"></path></g></g></svg > ';
    return icon;
}

function TrackingDesenharInformacoesMapa(mapa, dados) {
    mapa.direction.limparPolilinhas();
    TrackingPolilinhaMapa(mapa, dados.PolilinhaPrevista, dados.DistanciaPrevista, TRACKING_ROTA_COR_PLANEJADA, "rota-planejada", true);
    TrackingPolilinhaMapa(mapa, dados.PolilinhaAteOrigem, dados.DistanciaAteOrigem, TRACKING_ROTA_COR_ATE_ORIGEM, "rota-ate-origem", false);
    TrackingPolilinhaMapa(mapa, dados.PolilinhaAteDestino, dados.DistanciaAteDestino, TRACKING_ROTA_COR_ATE_DESTINO, "rota-ate-destino", false);
    TrackingPolilinhaMapa(mapa, dados.MonitoramentoVeiculos, dados.DistanciaRealizada, TRACKING_ROTA_COR_REALIZADA, "rota-realizada", true);
    mapa.direction.adicionarMarcadorComPontosDaRotaSemDestinos(dados.PontosPrevistos, false);
    TrackingDesenharEntregasMonitoramento(mapa, dados.Entregas);
    TrackingDesenharAreasMonitoramento(mapa, dados.Areas);
    TrackingDesenharParadasMonitoramento(mapa, dados.Paradas);
    $(".legenda-rotas-container").show();
}

function TrackingPolilinhaMapa(mapa, polilinhaObject, distancia, cor, classLi, visible) {
    var classVeiculo = "rota-veiculo";
    var classOff = "off";
    var element = $("ul.legenda li." + classLi);
    $("ul.legenda li." + classLi).removeClass(classOff);
    if (polilinhaObject != null && ((typeof polilinhaObject === 'object' && polilinhaObject.length > 0) || (typeof polilinhaObject !== 'object' && polilinhaObject != ""))) {
        var indicePolilinhaInicio;
        var indicePolilinhaFim;
        $("ul.legenda li." + classVeiculo).remove();
        if (typeof polilinhaObject === 'object' && polilinhaObject.length > 1) {
            indicePolilinhaInicio = mapa.direction.getTotalPolilinhas();
            for (var i = 0; i < polilinhaObject.length; i++) {
                if (polilinhaObject[i].Polilinha != "" && polilinhaObject[i].Polilinha != null) {
                    mapa.direction.desenharPolilinha(polilinhaObject[i].Polilinha, false, cor, visible);
                }
                $("ul.legenda").append(
                    $('<li class="rota-realizada ' + classVeiculo + '"><i class="fa fa-truck"></i><span class="descricao">' + polilinhaObject[i].Placa + '</span><span class="distancia">' + polilinhaObject[i].Distancia + '</span></li>')
                );
            }
            indicePolilinhaFim = mapa.direction.getTotalPolilinhas() - 1;
        } else {
            var polilinha = (typeof polilinhaObject === 'object') ? polilinhaObject[0].Polilinha : polilinhaObject;
            mapa.direction.desenharPolilinha(polilinha, false, cor, visible);
            indicePolilinhaInicio = mapa.direction.getTotalPolilinhas() - 1;
            indicePolilinhaFim = indicePolilinhaInicio;
        }
        if (element) {
            element.show();
            if (!visible) element.addClass(classOff);
            element.find("span.distancia").html(distancia);
            element.find("span.linha").css("background-color", cor);
            element.unbind("click").click(function () {
                $(this).toggleClass(classOff);
                if (classLi == "rota-realizada") $("ul.legenda li." + classVeiculo).toggleClass(classOff);
                for (var i = indicePolilinhaInicio; i <= indicePolilinhaFim; i++) {
                    mapa.direction.togglePolilinha(i);
                }
            });
        }
    } else {
        if (element) element.hide();
    }
}

function TrackingResetMarkersIcons() {
    for (var i = 0; i < TRACKING_ENTREGAS.length; i++) {
        var icon;
        if (TRACKING_ENTREGAS[i].OrdemPrevista == 0) {
            icon = "http://maps.google.com/mapfiles/kml/pal2/icon13.png";
        } else {
            icon = _mapTracking.direction.obterIconSVGMarcador(_mapTracking.direction.obterSVGPin(TrackingCorEntrega(TRACKING_ENTREGAS[i].Situacao), TRACKING_ENTREGAS[i].OrdemPrevista));
        }
        TrackingMarkerSetIcon(i, icon);
    }
}

function TrackingMarkerSetIconVisitado(indice) {
    var icon = _mapTracking.direction.obterIconSVGMarcador(_mapTracking.direction.obterSVGPin(TRACKING_ENTREGA_VISITADO, TRACKING_ENTREGAS[indice].OrdemPrevista));
    TrackingMarkerSetIcon(indice, icon);
}

function TrackingMarkerSetIcon(indice, icon) {
    if (TRACKING_ENTREGA_MARKERS[indice]) {
        TRACKING_ENTREGA_MARKERS[indice].setIcon(icon);
    }
}

function TrackingMarkerClick(indice) {
    google.maps.event.trigger(TRACKING_ENTREGA_MARKERS[indice], 'click');
}

function TrackingDesenharEntregasMonitoramento(map, entregas) {

    TRACKING_ENTREGAS = entregas;
    TRACKING_ENTREGA_MARKERS = new Array();
    _mapTracking = map;

    var total = entregas.length;
    if (total) {
        var bounds = new google.maps.LatLngBounds();
        for (var i = 0; i < total; i++) {

            if ((typeof entregas[i].Latitude) == "string")
                entregas[i].Latitude = Globalize.parseFloat(entregas[i].Latitude);

            if ((typeof entregas[i].Longitude) == "string")
                entregas[i].Longitude = Globalize.parseFloat(entregas[i].Longitude);

            var latlng = new google.maps.LatLng(entregas[i].Latitude, entregas[i].Longitude);
            bounds.extend(latlng);

            var icon;
            if (entregas[i].OrdemPrevista == 0) {
                icon = "http://maps.google.com/mapfiles/kml/pal2/icon13.png";
            } else {
                icon = map.direction.obterIconSVGMarcador(map.direction.obterSVGPin(TrackingCorEntrega(entregas[i].Situacao), entregas[i].OrdemPrevista));
            }

            var marker = new ShapeMarker();
            marker.setPosition(entregas[i].Latitude, entregas[i].Longitude);
            marker.icon = icon;
            marker.title = entregas[i].Descricao;
            marker.content = '<strong>' + entregas[i].Descricao + '</strong><br/><br>';
            var tipoDestino = (entregas[i].Coleta) ? 'coleta' : 'entrega';
            if (entregas[i].ValorTotalNF != undefined && entregas[i].ValorTotalNF != '') marker.content += 'Valor: ' + entregas[i].ValorTotalNF + '<br/>';
            if (entregas[i].Status != undefined && entregas[i].Status != '') marker.content += 'Situação da ' + tipoDestino + ': ' + entregas[i].Status + '<br/>';
            if (entregas[i].OrdemPrevista != undefined && entregas[i].OrdemPrevista > 0) marker.content += 'Sequência ' + tipoDestino + ' prevista : ' + entregas[i].OrdemPrevista + '<br/>';
            if (entregas[i].OrdemRealizada != undefined && entregas[i].OrdemRealizada > 0) marker.content += 'Sequência ' + tipoDestino + ' realizada : ' + entregas[i].OrdemRealizada + '<br/>';
            if (entregas[i].Chegada != undefined && entregas[i].Chegada != '') marker.content += 'Chegada: ' + entregas[i].Chegada + '<br/>';
            if (entregas[i].Saida != undefined && entregas[i].Saida != '') marker.content += 'Saída: ' + entregas[i].Saida + '<br/>';
            if (entregas[i].TempoAtendimento != undefined && entregas[i].TempoAtendimento != '') marker.content += 'Tempo de atendimento: ' + entregas[i].TempoAtendimento + '<br/>';
            if (entregas[i].TempoDirigindo != undefined && entregas[i].TempoDirigindo != '') marker.content += 'Tempo dirigindo: ' + entregas[i].TempoDirigindo + '<br/>';
            if (entregas[i].DistanciaPercorrida != undefined && entregas[i].DistanciaPercorrida != '') marker.content += 'Distância percorrida: ' + entregas[i].DistanciaPercorrida;

            const newShape = map.draw.addShape(marker, false, "click");
            TRACKING_ENTREGA_MARKERS.push(newShape);
        }
        map.draw.centerShapes();
    }
}

function TrackingDesenharAreasMonitoramento(mapaAreas, Areas) {
    if ((Areas === null) || (Areas === undefined)) return;

    for (var i = 0; i < Areas.length; i++) {

        var area = Areas[i];
        if (area.TipoArea == 2 && area.Area != "") {
            var objArea = JSON.parse(area.Area);
            for (var j = 0; j < objArea.length; j++) {
                var obj = objArea[j];
                obj['content'] = "Área principal";
                if (obj.type == google.maps.drawing.OverlayType.POLYGON)
                    TrackingCriarPoligonMonitoramento(mapaAreas, obj);
                else if (obj.type == google.maps.drawing.OverlayType.RECTANGLE)
                    TrackingCriarRectangleMonitoramento(mapaAreas, obj);
                else if (obj.type == google.maps.drawing.OverlayType.CIRCLE)
                    TrackingCriarCircleMonitoramento(mapaAreas, obj);
            }
        } else {
            TrackingCriarCircleRaio(mapaAreas, area);
        }

        // Subáreas
        if (area.Subareas != null && area.Subareas != undefined) {
            for (var j = 0; j < area.Subareas.length; j++) {
                var objSubarea = JSON.parse(area.Subareas[j].Area);
                for (var k = 0; k < objSubarea.length; k++) {
                    var obj = objSubarea[k];
                    obj['content'] = '<strong>' + area.Subareas[j].Descricao + '</strong><br/><i>' + area.Subareas[j].Tipo + '</i>';
                    obj['fillOpacity'] = 0.2;
                    if (obj.type == google.maps.drawing.OverlayType.POLYGON)
                        TrackingCriarPoligonMonitoramento(mapaAreas, obj);
                    else if (obj.type == google.maps.drawing.OverlayType.RECTANGLE)
                        TrackingCriarRectangleMonitoramento(mapaAreas, obj);
                    else if (obj.type == google.maps.drawing.OverlayType.CIRCLE)
                        TrackingCriarCircleMonitoramento(mapaAreas, obj);
                }
            }
        }
    }
}

function TrackingDesenharParadasMonitoramento(mapa, paradas) {
    if ((paradas === null) || (paradas === undefined)) return;
    for (var i = 0; i < paradas.length; i++) {
        var marker = new ShapeMarker();
        marker.setPosition(paradas[i].Latitude, paradas[i].Longitude);
        var cor = (paradas[i].Alerta) ? '#FF0000' : '#dbab1a';
        marker.icon = mapa.draw.icons.stop(48, 48, cor);
        marker.title = paradas[i].Tipo;
        marker.content =
            '<div><strong>' + paradas[i].Tipo + '</strong></div>' +
            '<div>' + paradas[i].Placa + '</div>' +
            '<div>' + paradas[i].Descricao.replace(': ', '<br/>Duração: ') + '</div>'
        mapa.draw.addShape(marker, false, "click");
    }
}

function TrackingCriarCircleRaio(mapaAreas, area) {
    var shapeCircle = new ShapeCircle();
    shapeCircle.type = google.maps.drawing.OverlayType.CIRCLE;
    shapeCircle.fillColor = "#1E90FF";
    shapeCircle.radius = parseInt(area.Raio);
    shapeCircle.content = "Área principal";
    shapeCircle.center = { lat: parseFloat(area.Latitude), lng: parseFloat(area.Longitude) };
    mapaAreas.draw.addShape(shapeCircle);
}

function TrackingCriarRectangleMonitoramento(mapaAreas, obj) {
    var shapeRectangle = new ShapeRectangle();
    shapeRectangle.fillColor = obj.fillColor;
    shapeRectangle.bounds = obj.bounds;
    shapeRectangle.zIndex = obj.zIndex;
    if (obj.content != undefined) shapeRectangle.content = obj.content;
    if (obj.fillOpacity != undefined) shapeRectangle.fillOpacity = obj.fillOpacity;
    mapaAreas.draw.addShape(shapeRectangle, null, "click");
}

function TrackingCriarCircleMonitoramento(mapaAreas, obj) {
    var shapeCircle = new ShapeCircle();
    shapeCircle.type = google.maps.drawing.OverlayType.CIRCLE;
    shapeCircle.fillColor = obj.fillColor;
    shapeCircle.radius = parseInt(obj.radius);
    shapeCircle.center = obj.center;
    if (obj.content != undefined) shapeCircle.content = obj.content;
    if (obj.fillOpacity != undefined) shapeCircle.fillOpacity = obj.fillOpacity;
    mapaAreas.draw.addShape(shapeCircle, null, "click");
}

function TrackingCriarPoligonMonitoramento(mapaAreas, obj) {
    var shapePolygon = new ShapePolygon();
    shapePolygon.fillColor = obj.fillColor;
    shapePolygon.paths = obj.paths;
    shapePolygon.zIndex = obj.zIndex;
    if (obj.content != undefined) shapePolygon.content = obj.content;
    if (obj.fillOpacity != undefined) shapePolygon.fillOpacity = obj.fillOpacity;
    mapaAreas.draw.addShape(shapePolygon, null, "click");
}

function TrackingCriarMarkerVeiculo(map, info, center, template = 0) {
    if ((typeof info.Latitude) == "string")
        info.Latitude = Globalize.parseFloat(info.Latitude);

    if ((typeof info.Longitude) == "string")
        info.Longitude = Globalize.parseFloat(info.Longitude);

    if (info.Latitude == 0 || info.Longitude == 0)
        return;
    var marker = new ShapeMarker();
    marker.setPosition(info.Latitude, info.Longitude);
    marker.icon = map.draw.icons.truck();
    marker.title = info.PlacaVeiculo;
    var html = `<div id="InfoWindowVeiculo"><div class="placa">${info.PlacaVeiculo}</div>`;
    if (template == 0) {
        if (info.Transportador != undefined) html += `<div class="transportadora"><label>Transportadora</label><span>${info.Transportador}</span></div>`;
        if (info.Status != undefined) html += `<div class="statu"><label>Status</label><span>${info.Status}</span></div>`;
        if (info.Carga != undefined) html += `<div class="plano"><label>Nº plano</label><span>${info.Carga}</span></div>`;
        if (info.Data != undefined) html += `<div class="data-ultima-posicao"><label>Data última posição</label><span>${info.Data}</span></div>`;
        if (info.Descricao != undefined) html += `<div class="desc-ultima-posicao"><label>Desc última posição</label><span>${info.Descricao}</span></div>`;
        if (info.Latitude != undefined && info.Latitude != undefined) html += `<div class="lat-long"><label>Latitude,Longitude</label><span>${info.Latitude}, ${info.Longitude}</span></div>`;
        if (info.Embarcador != undefined) html += `<div class="embarcador"><label>Embarcador</label><span>${info.Embarcador}</span></div>`;
        if (info.Rastreador != undefined) html += `<div class="embarcador"><label>Rastreador</label><span>${info.Rastreador}</span></div>`;
        if (info.Destinos != undefined) {
            for (var i = 0; i < info.Destinos.length; i++) {
                html += `<div class="destinos"><label>Destino ${info.Destinos[i].Ordem}</label><span>${info.Destinos[i].Descricao}</span></div>`;
            }
        }
    } else if (template == 1) {
        if (info.Descricao != undefined) html += `<div class="desc-ultima-posicao">${info.Descricao}</div>`;
        if (info.Latitude != undefined && info.Latitude != undefined) html += `<div class="lat-long">${info.Latitude}, ${info.Longitude}</div>`;
    }
    html += '</div>'
    marker.content = html;
    marker = map.draw.addShape(marker, false, "click");

    //Servira para afastar por um camião por um periudo de tempo #71548 
    marker.addListener('mouseover', function () {
        let moverLat = 0.03
        let moverLong = 0.03
        let zoom = map.getZoom()
        if (zoom > 6 && zoom < 16) {
            moverLat = 0.001
            moverLong = 0.001
        } else if (zoom >= 16 && zoom <= 30) {
            moverLat = 0.00001
            moverLong = 0.00001
        } else if (zoom > 30) {
            moverLat = 0.000001
            moverLong = 0.000001
        }

        marker.setPosition({ lat: info.Latitude + moverLat, lng: info.Longitude + moverLong })

        setTimeout(() => {
            marker.setPosition({ lat: info.Latitude, lng: info.Longitude })
        }, 2500)
    });


    if (center == undefined) center = true;
    if (center) {
        map.direction.setZoom(10);
        map.direction.centralizar(info.Latitude, info.Longitude);
    }

}

function TrackingCorEntrega(situacao) {
    if (situacao == EnumSituacaoEntrega.Entregue) {
        return TRACKING_ENTREGA_REALIZADA_COR;
    } else if (situacao == EnumSituacaoEntrega.EmCliente) {
        return TRACKING_ENTREGA_EN_ANDAMENTO_COR;
    } else {
        return TRACKING_ENTREGA_PENDENTE_COR;
    }
}
