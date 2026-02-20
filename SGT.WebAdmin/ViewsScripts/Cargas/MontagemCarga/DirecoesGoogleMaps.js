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
/// <reference path="../../Enumeradores/EnumTipoEmissaoCTeDocumentos.js" />
/// <reference path="../../Enumeradores/EnumTipoLocalizacao.js" />
/// <reference path="../../Enumeradores/EnumTipoPessoa.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Atividade.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Endereco.js" />
/// <reference path="Bloco.js" />
/// <reference path="Carga.js" />
/// <reference path="Carregamento.js" />
/// <reference path="CarregamentoCarga.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="Carregamentos.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="Distancia.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="MontagemCarga.js" />
/// <reference path="OrigemDestino.js" />
/// <reference path="Pedido.js" />
/// <reference path="PedidoProduto.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="SimulacaoFrete.js" />


var _map = null;
var _directionsService = null;
var _directionsDisplay = null;
var _arrayMarker = new Array();
var _markerCluster = null;

var _EnumMarker = {
    Pin: 1,
    PinSelecionado: 2,
    Distribuidor: 3,
    DistribuidorSelecionado: 4,
    PinRestricao: 5
};

function loadDirecoesGoogleMaps() {
    var id = "divMapa";

    _map = new google.maps.Map(document.getElementById(id), {
        zoom: 4,
        scaleControl: true,
        gestureHandling: 'greedy'
    });

    _directionsService = new google.maps.DirectionsService;
    _directionsDisplay = new google.maps.DirectionsRenderer;
    _directionsDisplay.setMap(_map);

    _markerCluster = new MarkerClusterer(_map, [], { imagePath: 'https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m' });

    setarCoordenadas();
}

function setarCoordenadas() {
    var latLng = { lat: -10.861639, lng: -53.104038 };
    var zoom = 4;

    _map.setZoom(zoom);
    _map.setCenter(latLng);
}

function ValidaEnderecoPessoa(endereco, distribuidor) {
    var lat = endereco.Latitude || "";
    var lng = endereco.Longitude || "";
    //var latTransbordo = endereco.LatitudeTransbordo || "";
    //var lngTransbordo = endereco.LongitudeTransbordo || "";

    if (/*distribuidor === false && */(lat == "" || lng == ""))
        return false;

    //if (distribuidor === true && (latTransbordo == "" || lngTransbordo == ""))
    //    return false;

    return true;
}

function ObterPontosPedidos() {
    if (_carregamento.TipoMontagemCarga.val() != EnumTipoMontagemCarga.NovaCarga) 
        return;

    RemoverPontos();

    var pedidos = PEDIDOS();
    var totalPontos = 0;
    var listaDestinatarios = {};
    var filtroRedespacho = _objPesquisaMontagem.GerarCargasDeRedespacho;
    var codigosPedidosSelecionados = ObterCodigoPedidosSelecionados();
    
    for (var cnpj = 0; cnpj < pedidos.length; cnpj++) {
        var pedido = pedidos[cnpj];
        var endereco = pedido.EnderecoDestino;
        var pessoa = pedido.Destinatario;
        var distribuidor = false;
        if (!filtroRedespacho && pedido.EnderecoRecebedor != null) {
            endereco = pedido.EnderecoRecebedor;
            pessoa = pedido.Recebedor;

            distribuidor = true;
        }

        if (!filtroRedespacho && pedido.DisponibilizarPedidoParaColeta == true) {
            endereco = pedido.EnderecoRemetente;
            pessoa = pedido.Remetente;
        }

        if (filtroRedespacho && pedido.EnderecoRecebedor == null && pedido.EnderecoRemetente == null)
            continue;

        if (endereco != null && ValidaEnderecoPessoa(endereco, distribuidor)) {
            var key = endereco.Destinatario + (distribuidor ? "D": "");
            if (!(key in listaDestinatarios)) {
                listaDestinatarios[key] = {
                    Pessoa: pessoa,
                    Endereco: endereco,
                    Distribuidor: distribuidor,
                    Pedidos: []
                };
                totalPontos++;
            }
            
            listaDestinatarios[key].Pedidos.push(pedido);
        }
    }

    for (var cnpj in listaDestinatarios) {
        var ponto = listaDestinatarios[cnpj];
        var latLng = { lat: 0, lng: 0 };

        if (ponto.Distribuidor === true && PontoTransbordoValidos(ponto.Endereco)) {
            latLng.lat = parseFloat(ponto.Endereco.LatitudeTransbordo.replace(',', '.'));
            latLng.lng = parseFloat(ponto.Endereco.LongitudeTransbordo.replace(',', '.'));
        } else {
            latLng.lat = parseFloat(ponto.Endereco.Latitude.replace(',', '.'));
            latLng.lng = parseFloat(ponto.Endereco.Longitude.replace(',', '.'));
        }

        var marker = new google.maps.Marker({
            map: _map,
            position: latLng,
        });
        var codigoPedidosDoPonto = ponto.Pedidos.map(function (ped) { return ped.Codigo; });
        var pontoSelecionado = VerificarOcorrenciaCodigosEmCodigos(codigoPedidosDoPonto, codigosPedidosSelecionados);

        var objMarker = {
            marker: marker,
            pessoa: ponto.Pessoa,
            endereco: ponto.Endereco,
            pedidos: ponto.Pedidos,
            codigos: codigoPedidosDoPonto,
            distribuidor: ponto.Distribuidor,
            selecionado: pontoSelecionado
        };
        ponto.marker = marker;
        ponto.selecionado = pontoSelecionado;

        attachEventMarker(objMarker);
        _arrayMarker.push(objMarker);
        
    }
    _pedidoMapa.TotalPedidos.val(totalPontos);
    
    setarPontosSelecionados(true);
}

function RemoverPontos() {
    for (var i in _arrayMarker)
        _arrayMarker[i].marker.setMap(null);
    _arrayMarker = [];
    _markerCluster.clearMarkers();
}

function attachEventMarker(objMarker) {
    var pesoTotal = 0;

    var contentString =
        '<div class="map-window">' +
        '    <div class="nome-cliente">' +
        '        <h4 class="h3">' + objMarker.pessoa + '</h4>' +
        '    </div>' +

        '    <div class="pedidos">' +
        '        <ul>' +
        (function () {
            return objMarker.pedidos.map(function (pedido) {
                pesoTotal += Globalize.parseFloat(pedido.Peso);
                return "<li>" + pedido.NumeroPedidoEmbarcador + " - " + pedido.Peso + "</li>";
            }).join("");
        }()) +
        '            <li><strong>Peso Total:</strong> ' + Globalize.format(pesoTotal, "n4") + '</li>' +
        '        </ul>' +
        '    </div>' +
        '</div>';

    var infowindow = new google.maps.InfoWindow({
        content: contentString
    });

    //if (objMarker.pedido.Restricao != "") {
    //    var infowindowRestricao = new google.maps.InfoWindow({
    //        content: "<div style='padding:2px; background-color: " + objMarker.pedido.DT_RowColor + ";'>" + objMarker.pedido.Restricao + "</div>"
    //    });

    //    infowindowRestricao.open(objMarker.marker.get('map'), objMarker.marker);
    //}

    if (objMarker.pedidos.length == 1) {
        objMarker.marker.addListener('rightclick', function () {
            ObterDetalhesPedido(objMarker.pedidos[0].Codigo);
        });
    }

    objMarker.marker.addListener('click', function () {
        markerClick(objMarker);
    });

    objMarker.marker.addListener('mouseover', function () {
        infowindow.open(objMarker.marker.get('map'), objMarker.marker);
    });

    objMarker.marker.addListener('mouseout', function () {
        infowindow.close();
    });
}

function setarPontosSelecionados(ajustarPosicaoMapa) {
    var bounds = new google.maps.LatLngBounds();
    var boundsNaoEntrados = new google.maps.LatLngBounds();
    var _markersMap = [];
    
    if (_arrayMarker.length == 0) 
        return setarCoordenadas();

    _markerCluster.clearMarkers();

    var encontrou = false;
    for (var i = 0; i < _arrayMarker.length; i++) {
        var marker = _arrayMarker[i];
        var objIconSet = null;
        var loc = new google.maps.LatLng(marker.marker.position.lat(), marker.marker.position.lng());

        _markersMap.push(marker.marker);

        if (marker.selecionado) {
            //marker.marker.setIcon('img/marker-transbordo-selecionado.png');
            //marker.marker.setIcon('http://maps.google.com/mapfiles/ms/micons/green-dot.png');
            if (marker.distribuidor)
                objIconSet = GerarIconMarker(_EnumMarker.DistribuidorSelecionado);
            else
                objIconSet = GerarIconMarker(_EnumMarker.PinSelecionado);

            bounds.extend(loc);
            encontrou = true;
        } else {
            if (marker.distribuidor)
                objIconSet = GerarIconMarker(_EnumMarker.Distribuidor);
                //marker.marker.setIcon('img/marker-transbordo.png');
            else
                objIconSet = GerarIconMarker(_EnumMarker.Pin);
            boundsNaoEntrados.extend(loc);
        }

        marker.marker.setIcon(objIconSet);
    }

    _markerCluster.addMarkers(_markersMap);

    if (ajustarPosicaoMapa) {
        var _bounds = encontrou ? bounds : boundsNaoEntrados;

        if (!$liMap.hasClass('active')) {
            _centralizarMapaNosPontos = _bounds;
        } else {
            _map.fitBounds(_bounds);
            _map.panToBounds(_bounds);
        }
    }
}

function GerarIconMarker(tipoMarker) {
    var dataIcon = {
        path: fontawesome.markers.MAP_MARKER,
        scale: 0.4,
        strokeWeight: 0.2,
        strokeColor: 'black',
        strokeOpacity: 1,
        fillColor: CorMarkerPorTipo(tipoMarker),
        fillOpacity: 1
    }

    return dataIcon;
}

function CorMarkerPorTipo(tipoMarker) {
    var cor = "";

    if (tipoMarker == _EnumMarker.Pin)
        cor = '#d45b5b';
    else if (tipoMarker == _EnumMarker.Distribuidor)
        cor = '#2386dc';
    if (tipoMarker == _EnumMarker.PinRestricao)
        cor = '#bfb104';
    else if (tipoMarker == _EnumMarker.DistribuidorSelecionado || tipoMarker == _EnumMarker.PinSelecionado)
        cor = '#2dab10';

    return cor;
}


function markerClick(objMarker) {
    /**
     * Quando o ponto não possui nenhum pedido selecionado
     * Adiciona todos pedidos ao ponto
     * 
     * Caso contrário, remove todos pedidos desse ponto
     * da lista dos pedidos selecionados
     */
    if (objMarker.selecionado === false) {
        objMarker.selecionado = true;
    } else {
        objMarker.selecionado = false;
    }

    EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS = true;
    for (var i in objMarker.pedidos) {
        SelecionarPedido(objMarker.pedidos[i], (objMarker.selecionado === false));
    }
    EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS = false;
    PedidosSelecionadosChange();
}

function obterIndiceMakerPedido(pedido) {
    if (!NavegadorIEInferiorVersao12()) {
        return _arrayMarker.findIndex(function (item) { return item.pedido.Codigo == pedido.Codigo });
    } else {
        for (var i = 0; i < _arrayMarker.length; i++) {
            if (pedido.Codigo == _arrayMarker[i].pedido.Codigo)
                return i;
        }
        return -1;
    }
}

function removerMarkers() {

    for (var i = 0; i < _arrayMarker.length; i++) {
        var marker = _arrayMarker[i];
       // _markerCluster.removeMarker(marker.marker);
        marker.marker.setMap(null);
    }

    _arrayMarker = new Array();

    //ObterPontosPedidos();
}