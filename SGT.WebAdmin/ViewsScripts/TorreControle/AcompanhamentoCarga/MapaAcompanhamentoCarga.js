/// <reference path="acompanhamentocarga.js" />
/// <reference path="tratamentoalerta.js" />
/// <reference path="signalr.js" />
/// <reference path="../../../js/Global/MapaGoogle.js" />
/// <reference path="../../../js/Global/Mapa.js"/>
/// <reference path="../../../js/Global/MapaDraw.js"/>
/// <reference path="../../../js/Global/MapaGoogleSearch.js"/>

var _mapaAcompanhamento = null;
var _markers;
var _markersEntregas;
var _markerArray;
var _polilinhaRealizada;
var _polilinhaPlanejada;
var _mapaGoogle;
var _markerVeiculo;
var _cargaSelecionada;
var _corAnteriorSelecionada;

var _carregouMapa;
function loadMapa() {

    if (_mapaAcompanhamento != null && _markers != null) {
        _markers.clearLayers();

        if (_mapaGoogle != null)
            _mapaGoogle.clear();
    }

    if (!_mapaAcompanhamento) {
        _carregouMapa = true;

        _mapaGoogle = new Mapa("tmp", null, null, null);

        let layers = [
            L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
                minZoom: 0,
                maxZoom: 20,
                attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            })
        ];

        _mapaAcompanhamento = L.map('divMapa', { zoomControl: false, wheelPxPerZoomLevel: 90, layers: layers }).setView([-12.30521, -51.17696], 4);

        L.control.layers({ "Ruas": layers[0] }, null, { position: 'topleft' }).addTo(_mapaAcompanhamento);

        _mapaAcompanhamento.on('click', function (e) {
            fecharDetalhes();
        });
    }
}

function loadCargasNoMapa(signalR) {
    _markerArrayAlert = new Array();

    if (_cargaSelecionada == null)
        _markersEntregas = new Array();

    if (signalR == undefined)
        signalR = false;

    if (_mapaAcompanhamento == null)
        return;

    if (_mapaAcompanhamento != null && _markers != null) {
        _markers.clearLayers();
    }

    _markers = L.markerClusterGroup();

    for (var i = 0; i < _cardAcompanhamentoCarga.Cargas().length; i++) {

        let v = _cardAcompanhamentoCarga.Cargas()[i].Data;

        if (v.Latitude != 0) {
            var conteudo = CarregarContent(v);

            let marker = L.marker([v.Latitude, v.Longitude], { icon: new iconeMarkerCarga(false, v.Cor) });
            marker.bindPopup(conteudo, { offset: [12, 0] });

            marker.on('mouseover', function (ev) {
                ev.target.openPopup(marker.getLatLng());
            });

            marker.on('click', function (infoWindow, maker) {
                clickMarkerVeiculo(v, marker);
            });

            if (v.IconeUltimoAlertaExibirTela != undefined && v.IconeUltimoAlertaExibirTela != "" && v.CodigoUltimoAlerta > 0) {
                var popupContent = ' <img onclick="alertaCardMapClick(' + v.CodigoCarga + ')" title="Carga: ' + v.CargaEmbarcador + ' - ' + v.DescricaoTipoUltimoAlertaExibirTela + '" src="' + v.IconeUltimoAlertaExibirTela + '" style="width:30px; cursor: pointer;" >'
                marker.bindPopup(popupContent, { maxWidth: 30, minWidth: 30, maxHeight: 30, autoPan: false, closeButton: false, closeOnClick: false, autoClose: false });

                _markerArrayAlert.push(marker);
            }
            if (_cargaSelecionada != null && _cargaSelecionada.CodigoCarga.val() == _cardAcompanhamentoCarga.Cargas()[i].Data.CodigoCarga) {  //se o marker esta selecionado vamos substitui-lo
                var icon = _markerVeiculo.getIcon();
                marker.setIcon(icon);

                _markerVeiculo = marker;
            }

            _markers.addLayer(marker);
        }
    }

    _mapaAcompanhamento.addLayer(_markers);

    if (_markers.length > 0 && !signalR) {
        _mapaAcompanhamento.fitBounds(_markers.getBounds());
    }

    _mapaAcompanhamento.on('zoomend', function () {
        openAllPopups();
    });

    actionLegendas();
    openAllPopups();
}


function openAllPopups() {
    _markerArrayAlert.forEach(function (marker) {
        var popup = marker.getPopup();
        marker.bindPopup(popup.getContent()).openPopup();
    });
}

function clickMarkerVeiculo(data, marker) {
    _cargaSelecionada = data;

    if (_corAnteriorSelecionada == null)
        _corAnteriorSelecionada = data.Cor;

    var card;

    for (var i = 0; i < _cardAcompanhamentoCarga.Cargas().length; i++) {
        if (_cardAcompanhamentoCarga.Cargas()[i].CodigoCarga.val() == data.CodigoCarga) {
            card = _cardAcompanhamentoCarga.Cargas()[i].Data;

            viewDetalhesCargaClick(_cardAcompanhamentoCarga.Cargas()[i]);
            break;
        }
    }

    if (_polilinhaPlanejada != null || _polilinhaRealizada != null) {
        if (_polilinhaPlanejada != null || _polilinhaRealizada != null) {
            try {
                _mapaAcompanhamento.removeLayer(_polilinhaPlanejada);
                _mapaAcompanhamento.removeLayer(_polilinhaRealizada);
            }
            catch (e) { }
        }
    }

    if (_markersEntregas.length > 0) {
        try {
            for (var i = 0; i < _markersEntregas.length; i++) {
                if (_markersEntregas[i] != null) {
                    _mapaAcompanhamento.removeLayer(_markersEntregas[i]);
                }
            }

            _markersEntregas = new Array();
        }
        catch (e) { }
    }

    _polilinhaPlanejada = null;
    _polilinhaRealizada = null;

    if (_markerVeiculo != null)//volta o anterior caso clicou em outro
        _markerVeiculo.setIcon(new iconeMarkerCarga(false, _corAnteriorSelecionada));

    _corAnteriorSelecionada = data.Cor;
    _markerVeiculo = marker;

    //AQUI VAMOS BUSAR A POLILINHA DA CARGA.. PREVISTA E REALISADA. E MARCAR OS PONTOS DOS CLIENTES =D BUSCAR A POLILINHA DO MONITORAMENTO
    var arrayLatLongPrevista = new Array();
    var arrayLatLongRealizada = new Array();
    executarReST("Monitoramento/ObterDadosMapaHistoricoPosicaoTelaAcompanhamentoCarga", {
        Codigo: card.CodigoMonitoramento
    }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                setTimeout(function () {

                    for (var i = 0; i < arg.Data.wayPointsPrevistos.length; i++) {
                        var latlng = L.latLng(arg.Data.wayPointsPrevistos[i].Latitude, arg.Data.wayPointsPrevistos[i].Longitude);
                        arrayLatLongPrevista.push(latlng);
                    }

                    if (arg.Data.wayPointsRealizados != null) {
                        for (var i = 0; i < arg.Data.wayPointsRealizados.length; i++) {
                            var latlng = L.latLng(arg.Data.wayPointsRealizados[i].Latitude, arg.Data.wayPointsRealizados[i].Longitude);
                            arrayLatLongRealizada.push(latlng);
                        }
                    }

                    desenharEntregasMapa(arg.Data.Entregas);

                    var iconVeiculoDestaque = L.icon({
                        iconUrl: '../../../content/torrecontrole/acompanhamentocarga/assets/icons/icon_truck.png',
                        iconSize: [38, 55],
                        iconAnchor: [22, 64],
                        popupAnchor: [-3, -76]
                    });

                    _markerVeiculo.setIcon(iconVeiculoDestaque);
                    _markerVeiculo.openPopup(_markerVeiculo.getLatLng());

                    if (arrayLatLongRealizada.length > 0) {
                        var vermelho = '#9400d3';
                        _polilinhaRealizada = L.polyline(arrayLatLongRealizada, { color: vermelho, smoothFactor: 3.0, weight: 5, }).addTo(_mapaAcompanhamento);
                        LegendasMapaCards(arg.Data.DistanciaRealizada, vermelho, "rota-realizada", true);
                    }

                    if (arrayLatLongPrevista.length > 0) {
                        var azul = '#016f65'
                        _polilinhaPlanejada = L.polyline(arrayLatLongPrevista, { color: azul, smoothFactor: 3.0, weight: 5, opacity: 0.6 }).addTo(_mapaAcompanhamento);

                        // zoom the map to the polyline
                        //_mapaAcompanhamento.fitBounds(_polilinhaPlanejada.getBounds());
                        LegendasMapaCards(_cargaSelecionada.DistanciaPrevista, azul, "rota-planejada", true);
                    }

                    $(".legenda-mapaCards").show();
                }, 500);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function clickMarkerVeiculoCard(data, marker) {
    _cargaSelecionada = data;

    if (_corAnteriorSelecionada == null)
        _corAnteriorSelecionada = data.Cor.val();

    viewDetalhesCargaClick(data);

    if (_polilinhaPlanejada != null || _polilinhaRealizada != null) {
        if (_polilinhaPlanejada != null || _polilinhaRealizada != null) {
            try {
                _mapaAcompanhamento.removeLayer(_polilinhaPlanejada);
                _mapaAcompanhamento.removeLayer(_polilinhaRealizada);
            }
            catch (e) { }
        }
    }

    if (_markersEntregas.length > 0) {
        try {
            for (var i = 0; i < _markersEntregas.length; i++) {
                if (_markersEntregas[i] != null) {
                    _mapaAcompanhamento.removeLayer(_markersEntregas[i]);
                }
            }

            _markersEntregas = new Array();
        }
        catch (e) { }
    }

    _polilinhaPlanejada = null;
    _polilinhaRealizada = null;

    if (_markerVeiculo != null)//volta o anterior caso clicou em outro
        _markerVeiculo.setIcon(new iconeMarkerCarga(false, _corAnteriorSelecionada));
    _corAnteriorSelecionada = data.Cor.val();
    _markerVeiculo = marker;

    //AQUI VAMOS BUSAR A POLILINHA DA CARGA.. PREVISTA E REALISADA. E MARCAR OS PONTOS DOS CLIENTES =D BUSCAR A POLILINHA DO MONITORAMENTO
    var arrayLatLongPrevista = new Array();
    var arrayLatLongRealizada = new Array();
    executarReST("Monitoramento/ObterDadosMapaHistoricoPosicaoTelaAcompanhamentoCarga", {
        Codigo: data.CodigoMonitoramento.val()
    }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                setTimeout(function () {

                    for (var i = 0; i < arg.Data.wayPointsPrevistos.length; i++) {
                        var latlng = L.latLng(arg.Data.wayPointsPrevistos[i].Latitude, arg.Data.wayPointsPrevistos[i].Longitude);
                        arrayLatLongPrevista.push(latlng);
                    }

                    if (arg.Data.wayPointsRealizados != null) {
                        for (var i = 0; i < arg.Data.wayPointsRealizados.length; i++) {
                            var latlng = L.latLng(arg.Data.wayPointsRealizados[i].Latitude, arg.Data.wayPointsRealizados[i].Longitude);
                            arrayLatLongRealizada.push(latlng);
                        }
                    }

                    desenharEntregasMapa(arg.Data.Entregas);

                    var iconVeiculoDestaque = L.icon({
                        iconUrl: '../../../content/torrecontrole/acompanhamentocarga/assets/icons/icon_truck.png',
                        iconSize: [38, 55],
                        iconAnchor: [22, 64],
                        popupAnchor: [-3, -76]
                    });

                    _markerVeiculo.setIcon(iconVeiculoDestaque);
                    _markerVeiculo.openPopup(_markerVeiculo.getLatLng());

                    if (arrayLatLongRealizada.length > 0) {
                        var vermelho = '#9400d3';
                        _polilinhaRealizada = L.polyline(arrayLatLongRealizada, { color: vermelho, smoothFactor: 3.0, weight: 5, }).addTo(_mapaAcompanhamento);
                        LegendasMapaCards(arg.Data.DistanciaRealizada, vermelho, "rota-realizada", true);
                    }

                    if (arrayLatLongPrevista.length > 0) {
                        var azul = '#016f65'
                        _polilinhaPlanejada = L.polyline(arrayLatLongPrevista, { color: azul, smoothFactor: 3.0, weight: 5, opacity: 0.6 }).addTo(_mapaAcompanhamento);

                        // zoom the map to the polyline
                        //_mapaAcompanhamento.fitBounds(_polilinhaPlanejada.getBounds());
                        LegendasMapaCards(_cargaSelecionada.DistanciaPrevista.val(), azul, "rota-planejada", true);
                    }

                    $(".legenda-mapaCards").show();
                }, 500);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Gerais.Geral.Falha, arg.Msg);
        }
    });
}


function desenharEntregasMapa(entregas) {
    if (_markersEntregas == null)
        _markersEntregas = new Array();

    var total = entregas.length;
    if (total) {

        for (var i = 0; i < total; i++) {
            var iconeVeiculoOptions = null;

            if ((typeof entregas[i].Latitude) == "string")
                entregas[i].Latitude = Globalize.parseFloat(entregas[i].Latitude);

            if ((typeof entregas[i].Longitude) == "string")
                entregas[i].Longitude = Globalize.parseFloat(entregas[i].Longitude);

            iconeVeiculoOptions = {
                iconUrl: entregas[i].OrdemPrevista == 0 ? "'../../../content/torrecontrole/acompanhamentocarga/assets/icons/icon_start.png" : _mapaGoogle.obterIconSVGMarcador(_mapaGoogle.obterSVGPin(CorEntrega(entregas[i].Situacao), entregas[i].OrdemPrevista))
            };

            let iconeEntrega = L.Icon.extend({
                options: iconeVeiculoOptions
            });

            var conteudo = carregarContentEntrega(entregas[i]);

            let markerEntrega = L.marker([entregas[i].Latitude, entregas[i].Longitude], { icon: new iconeEntrega() });
            markerEntrega.bindPopup(conteudo, { offset: [12, 0] });
            markerEntrega.addTo(_mapaAcompanhamento);

            _markersEntregas.push(markerEntrega);
        }
    }
}


function LegendasMapaCards(distancia, cor, classLi, visible) {
    var classVeiculo = "rota-veiculo";
    var classOff = "off";
    let veiculos = typeof _cargaSelecionada.Veiculos == 'object' ? _cargaSelecionada.Veiculos.val() : _cargaSelecionada.Veiculos;
    let distanciaPrevista = typeof _cargaSelecionada.DistanciaPrevista == 'object' ? _cargaSelecionada.DistanciaPrevista.val() : _cargaSelecionada.DistanciaPrevista;
    var element = $("ul.legendaMapaCard li." + classLi);
    $("ul.legendaMapaCard li." + classLi).removeClass(classOff);
    $("ul.legendaMapaCard li." + classVeiculo).remove();
    $("ul.legendaMapaCard li.icone-legenda").remove();

    $("ul.legendaMapaCard").append(
        $('<li class="icone-legenda"><img style="widht:30px; height:30px" src="../../../content/torrecontrole/acompanhamentocarga/assets/icons/icon_start.png" ></img><span class="descricao"> Coleta/Inicio</span></li>'),
        $('<li class="icone-legenda"><img style="widht:30px; height:30px" src="../../../content/torrecontrole/acompanhamentocarga/assets/icons/icon_truck.png" ></img><span class="descricao"> Posição atual do veículo </span></li>'),
        $('<li class="rota-realizada rota-veiculo"><i class="fa fa-truck"></i><span class="descricao"> ' + veiculos + '</span><span class="distancia">' + distanciaPrevista + '</span></li>'),
    );

    if (element) {
        element.show();
        if (!visible) element.addClass(classOff);
        element.find("span.distancia").html(distancia);
        element.find("span.linha").css("background-color", cor);
    }

}

function LegendasCoresVeiculos(Descr, cor, classLi, visible) {
    var classVeiculo = "rota-veiculo";
    var classOff = "off";
    var element = $("ul.legendaMapaCard li." + classLi);
    $("ul.legendaMapaCores li." + classLi).removeClass(classOff);
    $("ul.legendaMapaCores li." + classVeiculo).remove();
    $("ul.legendaMapaCores li.icone-legenda").remove();

    $("ul.legendaMapaCores").append(
        $('<li class="icone-legenda"><img style="widht:30px; height:30px" src="../../../content/torrecontrole/acompanhamentocarga/assets/icons/icon_start.png" ></img><span class="descricao"> Coleta/Inicio</span></li>'),
        $('<li class="icone-legenda"><img style="widht:30px; height:30px" src="../../../content/torrecontrole/acompanhamentocarga/assets/icons/icon_truck.png" ></img><span class="descricao"> Posição atual do veículo </span></li>'),
        $('<li class="rota-realizada rota-veiculo"><i class="fa fa-truck"></i><span class="descricao"> ' + _cargaSelecionada.Veiculos + '</span><span class="distancia">' + _cargaSelecionada.DistanciaPrevista + '</span></li>'),
    );

    if (element) {
        element.show();
        if (!visible) element.addClass(classOff);
        element.find("span.distancia").html(distancia);
        element.find("span.linha").css("background-color", cor);
    }

}


function iconeMarkerCarga(destacado, corStatus) {
    if (destacado) {
        var iconVeiculoDestaque = L.icon({
            iconUrl: '../../../content/torrecontrole/acompanhamentocarga/assets/icons/icon_truck.png',
            iconSize: [38, 55],
            popupAnchor: [5, -30]
        });

        return iconVeiculoDestaque;
    } else {

        //var iconVeiculo = L.icon({
        //    iconUrl: '../../../content/torrecontrole/acompanhamentocarga/assets/icons/icon_truck_new.png',
        //    iconSize: [38, 40],
        //    popupAnchor: [5, -30]
        //});

        //return iconVeiculo;
        var iconeVeiculoOptions = null;
        var mapdraw = new MapaDraw();

        iconeVeiculoOptions = {
            iconUrl: mapdraw.icons.trucknewCircule(45, 47, corStatus),
            popupAnchor: [5, -30]
        }

        var iconeVeiculo = L.Icon.extend({
            options: iconeVeiculoOptions
        });

        return new iconeVeiculo();
    }
}

function CorEntrega(situacao) {
    var TRACKING_ENTREGA_PENDENTE_COR = "#D45B5B";
    var TRACKING_ENTREGA_EN_ANDAMENTO_COR = "#DED84C";
    var TRACKING_ENTREGA_REALIZADA_COR = "#5ECC71";

    if (situacao == EnumSituacaoEntrega.Entregue) {
        return TRACKING_ENTREGA_REALIZADA_COR;
    } else if (situacao == EnumSituacaoEntrega.EmCliente) {
        return TRACKING_ENTREGA_EN_ANDAMENTO_COR;
    } else {
        return TRACKING_ENTREGA_PENDENTE_COR;
    }
}

function carregarContentEntrega(entrega) {

    var html = '<div> <strong>' + entrega.Descricao + '</strong><br/><br>';
    var tipoDestino = (entrega.Coleta) ? 'coleta' : 'entrega';
    if (entrega.ValorTotalNF != undefined && entrega.ValorTotalNF != '') html += 'Valor: ' + entrega.ValorTotalNF + '<br/>';
    if (entrega.Status != undefined && entrega.Status != '') html += 'Situação da ' + tipoDestino + ': ' + entrega.Status + '<br/>';
    if (entrega.OrdemPrevista != undefined && entrega.OrdemPrevista > 0) html += 'Sequência ' + tipoDestino + ' prevista : ' + entrega.OrdemPrevista + '<br/>';
    if (entrega.OrdemRealizada != undefined && entrega.OrdemRealizada > 0) html += 'Sequência ' + tipoDestino + ' realizada : ' + entrega.OrdemRealizada + '<br/>';
    if (entrega.Chegada != undefined && entrega.Chegada != '') html += 'Chegada: ' + entrega.Chegada + '<br/>';
    if (entrega.Saida != undefined && entrega.Saida != '') html += 'Saída: ' + entrega.Saida + '<br/>';
    if (entrega.TempoAtendimento != undefined && entrega.TempoAtendimento != '') html += 'Tempo de atendimento: ' + entrega.TempoAtendimento + '<br/>';
    if (entrega.TempoDirigindo != undefined && entrega.TempoDirigindo != '') html += 'Tempo dirigindo: ' + entrega.TempoDirigindo + '<br/>';
    if (entrega.DistanciaPercorrida != undefined && entrega.DistanciaPercorrida != '') html += 'Distância percorrida: ' + entrega.DistanciaPercorrida;

    html += '</div>';

    return html;
}

function CarregarContent(data) {
    var icone = TrackingIconRastreador(data.onLine);

    var html = `<div id="InfoWindowVeiculo">`;
    html += `<div class="carga" style="padding: 8px !important"><label>Nº Carga</label><span>${data.CargaEmbarcador}</span></div>`;
    if (data.DataCarregamentoCargaFormatada != undefined && data.DataCarregamentoCargaFormatada != '') html += `<div class="desc-ultima-posicao"><label>Data Carregamento</label><span>${data.DataCarregamentoCargaFormatada}</span></div>`;
    html += `<div class="carga" style="padding: 8px !important"><label>Veiculo</label><span>${data.Veiculos}</span></div>`;
    if (data.Destinos != undefined && data.Destinos != '') html += `<div class="carga" style="padding: 8px !important"><label>Destinos</label><span>${data.Destinos}</span></div>`;
    if (data.ProximoDestino != undefined && data.ProximoDestino != '') html += `<div class="carga" style="padding: 8px !important"><label>Proximo Destino</label><span>${data.ProximoDestino}</span></div>`;
    if (data.ProximoDestino != undefined && data.ProximoDestino != '') html += `<div class="carga" style="padding: 8px !important"><label>Entrega Prevista</label><span>${data.DataPrevistaProximaEntrega}</span></div>`;
    if (data.NomeTransportador != undefined && data.NomeTransportador != '') html += `<div class="carga" style="padding: 8px !important"><label>Transportadora</label><span>${data.NomeTransportador}</span></div>`;
    if (data.Motoristas != undefined && data.Motoristas != '') html += `<div class="carga" style="padding: 8px !important"><label>Motorista</label><span>${data.Motoristas}</span></div>`;
    if (data.DataUltimaPosicao != undefined && data.DataUltimaPosicao != '') html += `<div class="carga" style="padding: 8px !important"><label>Data última posição</label><span>${data.DataUltimaPosicao}</span></div>` + `<div class="tracking-indicador" style="float: right; width: 50px; margin-top:-29px" title ="${data.DataUltimaPosicao}">` + icone + `</div>`;;
    if (data.DescricaoRastreador != undefined && data.DescricaoRastreador != '') html += `<div class="carga" style="padding: 8px !important"><label>Rastreador</label><span>${data.DescricaoRastreador}</span></div>`;
    html += '</div>'

    return html;
}

function actionLegendas() {
    executarReST("AcompanhamentoCarga/Legendas", null, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                if (arg.Data.Grupos != undefined) {
                    $(".legendaMapaVeiculos").html("");

                    for (var i = 0; i < arg.Data.Grupos.length; i++) {
                        InserirLegendaMapa(".legendaMapaVeiculos", arg.Data.Grupos[i].Descricao, arg.Data.Grupos[i].Cor);
                        if (arg.Data.Grupos[i].StatusViagem != undefined) {
                            for (var j = 0; j < arg.Data.Grupos[i].StatusViagem.length; j++) {
                                InserirLegendaMapa(".legendaMapaVeiculos", arg.Data.Grupos[i].StatusViagem[j].Descricao, arg.Data.Grupos[i].StatusViagem[j].Cor, null, 'sub');
                            }
                        }
                    }
                }
                //if (arg.Data.Categorias != undefined) {
                //    for (var i = 0; i < arg.Data.Categorias.length; i++) {
                //        InserirLegendaMapa(".legenda-categorias ul", arg.Data.Categorias[i].Descricao, arg.Data.Categorias[i].Cor, arg.Data.Categorias[i].Cor);
                //    }
                //}
                if (_mostrandoCards)
                    $(".legenda-mapaVeiculos").hide();
                else
                    $(".legenda-mapaVeiculos").show();

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }

        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null, true);
}

function InserirLegendaMapa(selector, text, backgroundColor, borderColor, className) {
    if (backgroundColor) {
        if (!borderColor) borderColor == 'none';
        if (!className) className = '';
        $(selector).append('<li class="' + className + '"><span style="background-color:' + backgroundColor + ';border-color:' + borderColor + 'px;"></span>' + text + '</li>');
    } else {
        $(selector).append('<li>' + text + '</li>');
    }
}