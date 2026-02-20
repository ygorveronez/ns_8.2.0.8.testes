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
/// <reference path="../../../js/plugin/bootstrap-menu/bootstrapmenu.min.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoCTeDocumentos.js" />
/// <reference path="../../Enumeradores/EnumTipoLocalizacao.js" />
/// <reference path="../../Enumeradores/EnumTipoPessoa.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Atividade.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Endereco.js" />
/// <reference path="Bloco.js" />
/// <reference path="Carregamento.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="Carregamentos.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="Distancia.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="MontagemCarga.js" />
/// <reference path="Pedido.js" />
/// <reference path="PedidoProduto.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="SimulacaoFrete.js" />

var _map = null;
var _directionsService = null;
var _directionsDisplay = null;
var _arrayMarker = new Array();
var _markerCD = null;
var _drawingManager = null;
var _overlay;
var _ctrl_pressed = false;
var _polylineCargas;
var _info_window_timer;
var _info_window = new google.maps.InfoWindow();

var _raioGeograficoLocalizacao;
var _radiusDrawMaps;

var _colours = {
    "0": "#000000", "1": "#00FFFF", "2": "#7FFFD4", "3": "#708090", "4": "#808000", "5": "#0000FF", "6": "#8A2BE2", "7": "#A52A2A", "8": "#FF0000", "9": "#DEB887", "10": "#5F9EA0",
    "11": "#7FFF00", "12": "#D2691E", "13": "#FF7F50", "14": "#6495ED", "15": "#DC143C", "16": "#00008B", "17": "#008B8B", "18": "#FFFF00", "19": "#B8860B", "20": "#A9A9A9",
    "21": "#006400", "22": "#BDB76B", "23": "#8B008B", "24": "#FF8C00", "25": "#9932CC", "26": "#1E90FF", "27": "#8B0000", "28": "#E9967A", "29": "#8FBC8F", "30": "#483D8B",
    "31": "#2F4F4F", "32": "#FF1493", "33": "#00BFFF", "34": "#228B22", "35": "#FF00FF", "36": "#FFD700", "37": "#DAA520", "38": "#ADFF2F", "39": "#FF69B4", "40": "#CD5C5C",
    "41": "#4B0082", "42": "#20B2AA", "43": "#800000", "44": "#66CDAA", "45": "#0000CD", "46": "#BA55D3", "47": "#3CB371", "48": "#7B68EE", "49": "#00FA9A", "50": "#C71585",
    "51": "#191970", "52": "#808000", "53": "#6B8E23", "54": "#FFA500", "55": "#FF4500", "56": "#FFFFFF"
};

var _labelObservacao = {
    text: '*',
    fontWeight: 'bold',
    fontSize: '24px',
    fontFamily: '"Courier New", Courier,Monospace',
    color: 'red'
};

var _labelFreteCombinado = {
    text: '$',
    fontWeight: 'bold',
    fontSize: '24px',
    fontFamily: '"Courier New", Courier,Monospace',
    color: 'red'
};

function colourHex(nro_carga) {
    if (nro_carga < 0) return '#000';
    if (nro_carga > 55) {
        nro_carga = nro_carga % 55;
        //if (nro_carga == 0) nro_carga = 1;
    }
    if (typeof _colours[nro_carga] != 'undefined')
        return _colours[nro_carga];
    else
        return '#000';
    return false;
}

function imgColorMarker(nro_carga, distribuidor, urlOnly) {
    //if (nro_carga < 0) return "../img/montagem-carga-mapa/markers/colors/black.png";
    //if (nro_carga > 55) {
    //    nro_carga = nro_carga % 55;
    //    //if (nro_carga == 0) nro_carga = 1;
    //}
    //return "../img/montagem-carga-mapa/markers/colors/" + nro_carga + (distribuidor ? '_S' : '') + ".png";

    if (nro_carga < 0) {
        return {
            url: "../img/montagem-carga-mapa/markers/colors/black.png",
            labelOrigin: new google.maps.Point(20, 0)
        };
    }

    if (nro_carga > 55) {
        nro_carga = nro_carga % 55;
    }
    var url = "../img/montagem-carga-mapa/markers/colors/" + nro_carga + (distribuidor ? '_S' : '') + ".png";

    if (urlOnly === true)
        return url;

    return {
        url: url,
        labelOrigin: new google.maps.Point(20, 0)
    };
}

var _EnumMarker = {
    Pin: 1,
    PinSelecionado: 2,
    Distribuidor: 3,
    DistribuidorSelecionado: 4,
    PinRestricao: 5,
    PinColeta: 6,
    PinReentrega: 7
};

var RaioGeograficoLocalizacao = function () {

    var latitudeInicial = "-27.096800";
    var longitudeInicial = "-52.618600";
    this.LatitudeRaio = PropertyEntity({ val: ko.observable(latitudeInicial), def: latitudeInicial, getType: typesKnockout.decimal, text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.Latitude), required: true, maxlength: 10, configDecimal: { precision: 6, allowZero: true, allowNegative: true, thousands: "" } });
    this.LongitudeRaio = PropertyEntity({ val: ko.observable(longitudeInicial), def: longitudeInicial, getType: typesKnockout.decimal, text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.Longitude), required: true, maxlength: 10, configDecimal: { precision: 6, allowZero: true, allowNegative: true, thousands: "" } });

    this.Raio1 = PropertyEntity({ val: ko.observable(25), def: 0, getType: typesKnockout.int, text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.Raio1), maxlength: 10 });
    this.Raio2 = PropertyEntity({ val: ko.observable(50), def: 0, getType: typesKnockout.int, text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.Raio2), maxlength: 10 });
    this.Raio3 = PropertyEntity({ val: ko.observable(100), def: 0, getType: typesKnockout.int, text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.Raio3), maxlength: 10 });

    this.Cancelar = PropertyEntity({ eventClick: cancelarRaioGeograficoLocalizacaoClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Cancelar) });
    this.Confirmar = PropertyEntity({ eventClick: confirmarRaioGeograficoLocalizacaoClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Confirmar) });
}

function loadDirecoesGoogleMaps() {

    var id = "divMapa";
    if (_map === null) {
        _map = new google.maps.Map(document.getElementById(id), {
            zoom: 9,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            zoomControl: true,
            zoomControlOptions: {
                position: google.maps.ControlPosition.LEFT_TOP
            },
            gestureHandling: 'greedy', // Desabilita selecionar o CTRL par aaplicar o zoom..
            scaleControl: true,
            streetViewControl: false, //true,
            fullscreenControl: false,
            //streetViewControlOptions: {
            //    position: google.maps.ControlPosition.LEFT_TOP
            //},
            logoControlOptions: {
                position: google.maps.ControlPosition.LEFT_TOP
            },
            // Desabilita a opção satélite...
            disableDefaultUI: true
        });

        //Adicionando o eventro click no mapa..
        google.maps.event.addListener(_map, 'click', function (event) {
            closeInfoWindowGoogleMaps();
        });

        google.maps.event.addListener(_map, 'rightclick', function (event) {
            setLatLngRaioGeograficoLocalizacao(event.latLng);
            closeInfoWindowGoogleMaps();
            iniciarNovoCarregamentoClick();
        });

        _directionsService = new google.maps.DirectionsService;
        _directionsDisplay = new google.maps.DirectionsRenderer;
        _directionsDisplay.setMap(_map);

        setarCoordenadas();

        createDrawingManager();

    }

    _raioGeograficoLocalizacao = new RaioGeograficoLocalizacao();
    KoBindings(_raioGeograficoLocalizacao, "knockoutRaioGeograficoLocalizacao");

    //if (_markerCluster === null) {
    //    _markerCluster = new MarkerClusterer(_map, [], { imagePath: 'https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m' });
    //}
}

function closeInfoWindowGoogleMaps() {
    if (_info_window) {
        if (_info_window.map != null) {
            _info_window.close();
        }
    }
}

function setarCoordenadas() {
    var latLng = { lat: -10.861639, lng: -53.104038 };
    var zoom = 4;
    _map.setZoom(zoom);
    _map.setCenter(latLng);
}

function createDrawingManager() {

    //Habilitando opção para desenhar/selecionar no mapa
    var options = {
        fillColor: 'red',
        fillOpacity: 0.1,
        strokeColor: 'red',
        strokeWeight: 0.5,
        clickable: false,
        editable: false
    };

    // Inicianlizando google maps drawing
    _drawingManager = new google.maps.drawing.DrawingManager({
        //drawingMode: google.maps.drawing.OverlayType.POLYGON,
        drawingControl: true,
        drawingControlOptions: {
            position: google.maps.ControlPosition.TOP_CENTER,
            drawingModes: ['circle', 'polygon', 'rectangle']//['marker', 'circle', 'polygon', 'polyline', 'rectangle']
        },
        markerOptions: { icon: 'content/markers/colors/red.png' },
        circleOptions: options,
        rectangleOptions: options,
        polygonOptions: options
    });

    google.maps.event.addListener(_drawingManager, 'overlaycomplete', function (event) {
        if (_overlay) _overlay.setMap(null);
        //Limpando os markadores selecionados
        //resetMarkersSelecionado();
        _overlay = event.overlay;
        filtroPedidosMapa(event);
        //Ao completar o desenho, desabilitado a forma selecionada para que o usuário arraste o mapa.
        _drawingManager.setOptions({ drawingMode: null });

        //Em 5 segundos vamos limpar o mapa.
        setTimeout(function () {
            if (_overlay) _overlay.setMap(null);
        }, 5000);

    });
    _drawingManager.setMap(_map);

    // Centralizar tudo...
    var div = document.createElement('div');
    div.className = 'g-opt-mapa';
    cc = new CustomControl(div, Localization.Resources.Cargas.MontagemCargaMapa.CentralizarTudo, "fal fa-fw fa-lg fa-search-plus", zoomIn, 'divMapaCentralizarTudo');
    _map.controls[google.maps.ControlPosition.TOP_CENTER].push(div);

    div = document.createElement('div');
    div.className = 'g-opt-mapa';
    cc = new CustomControl(div, Localization.Resources.Cargas.MontagemCargaMapa.RaioGeograficoLocalizacao, "fal fa-fw fa-lg fa-podcast", modalRaioGeograficoLocalizacao);
    _map.controls[google.maps.ControlPosition.TOP_CENTER].push(div);

    div = document.createElement('div');
    div.className = 'g-opt-mapa';
    cc = new CustomControl(div, Localization.Resources.Cargas.MontagemCargaMapa.AbrirFiltroDeRotaLocalidade, "fal fa-fw fa-lg fa-filter", modalFiltroRotaLocalidade, 'divFiltroRotaLocalidade');
    _map.controls[google.maps.ControlPosition.TOP_CENTER].push(div);

    div = document.createElement('div');
    div.className = 'g-opt-mapa';
    cc = new CustomControl(div, Localization.Resources.Cargas.MontagemCargaMapa.LimparRaioGeograficoLocalizacao, "fal fa-fw fa-lg fa-eraser", limparRaioGeograficoLocalizacao);
    _map.controls[google.maps.ControlPosition.TOP_CENTER].push(div);
}

function CustomControl(controlDiv, title, className, callback, id) {
    // Set CSS for the control border
    var div = document.createElement('div');
    div.className = 'opt-mapa';
    div.title = title;
    controlDiv.appendChild(div);

    // Set CSS for the control interior
    var controlText = document.createElement('div');
    if (id != undefined && id != null) {
        controlText.id = id;
    }
    controlText.style.width = '24px';
    controlText.style.height = '24px';
    controlText.style.overflow = 'hidden';
    controlText.style.position = 'relative';
    controlText.style.color = "#999";

    var elem = document.createElement("i");
    elem.className = className;
    elem.style.marginTop = "6px";
    controlText.appendChild(elem);

    div.appendChild(controlText);

    // Setup the click event listeners
    google.maps.event.addDomListener(div, 'click', function () {
        callback();
    });
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

function ObterPontoRemetente() {
    var latLng = { lat: 0, lng: 0 };
    if (_markerCD != null) {
        latLng.lat = _markerCD.position.lat();
        latLng.lng = _markerCD.position.lng();
        return latLng;
    }

    var pedidos = PEDIDOS();
    if (pedidos) {
        for (var cnpj = 0; cnpj < pedidos.length; cnpj++) {
            var pedido = pedidos[cnpj];
            var endereco = pedido.EnderecoExpedidor;
            var pessoa = pedido.Expedidor;
            if (endereco == undefined || endereco == null) {
                endereco = pedido.EnderecoRemetente;
                pessoa = pedido.Remetente;
            }
            if (_sessaoRoteirizador.TipoRoteirizacaoColetaEntrega.val() != EnumTipoRoteirizacaoColetaEntrega.Entrega) {
                endereco = pedido.EnderecoDestino;
                pessoa = pedido.Destinatario;
            }
            if (endereco != null && endereco != undefined) {
                latLng.lat = parseFloat(endereco.Latitude.replace(',', '.'));
                latLng.lng = parseFloat(endereco.Longitude.replace(',', '.'));
                if (!isNaN(latLng.lat) && !isNaN(latLng.lng)) {
                    _markerCD = new google.maps.Marker({
                        map: _map,
                        position: latLng,
                        icon: "../img/montagem-carga-mapa/markers/colors/numbers/_white.png",
                        title: pessoa
                    });
                    return latLng;
                }
            }
        }
    }
    return latLng;
}

function ObterPontosPedidos() {
    if (_carregamento.TipoMontagemCarga.val() != EnumTipoMontagemCarga.NovaCarga)
        return;
    RemoverPontos();

    var pedidos = PEDIDOS();
    var totalPontos = 0;
    var listaDestinatarios = {};
    var filtroRedespacho = _objPesquisaMontagem.GerarCargasDeRedespacho;
    var filtroPedidosOrigemRecebedor = _objPesquisaMontagem.PedidosOrigemRecebedor;
    var codigosPedidosSelecionados = ObterCodigoPedidosSelecionados();
    var agrupaPedidosEmMarker = false;
    var coletaEntrega = false;
    if (_sessaoRoteirizador)
        coletaEntrega = (_sessaoRoteirizador.TipoRoteirizacaoColetaEntrega.val() == EnumTipoRoteirizacaoColetaEntrega.ColetaEntrega);
    //coletaEntrega = _sessaoRoteirizador.MontagemCarregamentoColetaEntrega.val();

    for (var cnpj = 0; cnpj < pedidos.length; cnpj++) {
        var pedido = pedidos[cnpj];
        var endereco = pedido.EnderecoDestino;
        var pessoa = pedido.Destinatario;
        var reentrega = pedido.Reentrega;
        var distribuidor = false;
        let pessoaNomeFantasia = pedido.DestinatarioNomeFantasia

        if (_sessaoRoteirizador.TipoRoteirizacaoColetaEntrega.val() == EnumTipoRoteirizacaoColetaEntrega.Coleta) {
            endereco = pedido.EnderecoRemetente;
            pessoa = pedido.Remetente;
        } else {

            if (!filtroRedespacho && pedido.EnderecoRecebedor != null && !filtroPedidosOrigemRecebedor) {
                endereco = pedido.EnderecoRecebedor;
                pessoa = pedido.Recebedor;
                pessoaNomeFantasia = pedido.RecebedorNomeFantasia;
                distribuidor = true;
            }

            if (!filtroRedespacho && pedido.DisponibilizarPedidoParaColeta == true) {
                endereco = pedido.EnderecoRemetente;
                pessoa = pedido.Remetente;
            }

            if (filtroRedespacho && pedido.EnderecoRecebedor == null && pedido.EnderecoRemetente == null)
                continue;
        }

        if (endereco != null && ValidaEnderecoPessoa(endereco, distribuidor)) {
            if (agrupaPedidosEmMarker) {
                var key = endereco.Destinatario + (distribuidor ? "D" : "");
                if (!(key in listaDestinatarios)) {
                    listaDestinatarios[key] = {
                        Pessoa: pessoa,
                        PessoaNomeFantasia: pessoaNomeFantasia,
                        Endereco: endereco,
                        Distribuidor: distribuidor,
                        Reentrega: reentrega,
                        Coleta: false,
                        Pedidos: []
                    };
                    totalPontos++;
                }
                listaDestinatarios[key].Pedidos.push(pedido);
            } else {
                var key = pedido.Codigo;
                var ident_entrega = endereco.Destinatario + (distribuidor ? "D" : "");
                listaDestinatarios[key] = {
                    Pessoa: pessoa,
                    PessoaNomeFantasia: pessoaNomeFantasia,
                    Endereco: endereco,
                    Distribuidor: distribuidor,
                    Reentrega: reentrega,
                    Identificacao: ident_entrega,
                    Coleta: false,
                    Pedidos: []
                };
                totalPontos++;
                listaDestinatarios[key].Pedidos.push(pedido);
            }
        }

        ////Se for montagem por várias coletas e entregas.
        if (coletaEntrega && pedido.EnderecoRemetente != null) {
            if (ValidaEnderecoPessoa(pedido.EnderecoRemetente, false)) {

                var key = "COLETA_" + pedido.EnderecoRemetente.Destinatario;
                var ident_entrega = pedido.Remetente;
                if (!(key in listaDestinatarios)) {
                    listaDestinatarios[key] = {
                        Pessoa: pessoa,
                        PessoaNomeFantasia: pessoaNomeFantasia,
                        Endereco: pedido.EnderecoRemetente,
                        Distribuidor: false,
                        Reentrega: reentrega,
                        Coleta: true,
                        Identificacao: ident_entrega,
                        Pedidos: []
                    };
                    totalPontos++;
                }
                listaDestinatarios[key].Pedidos.push(pedido);
            }
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
            pessoaNomeFantasia: ponto.PessoaNomeFantasia,
            endereco: ponto.Endereco,
            pedidos: ponto.Pedidos,
            codigos: codigoPedidosDoPonto,
            codigo_carregamento: 0,
            distribuidor: ponto.Distribuidor,
            selecionado: pontoSelecionado,
            identificador: ponto.Identificacao,
            coleta: ponto.Coleta,
            reentrega: ponto.Reentrega
        };
        ponto.marker = marker;
        ponto.selecionado = pontoSelecionado;

        if (!_sessaoRoteirizadorParametros.OcultarDetalhesDoPontoNoMapa.val())
            attachEventMarker(objMarker, latLng);

        if (ponto.Pedidos.length > 0) {
            if (ponto.Pedidos[0].ObservacaoDestinatario != null && ponto.Pedidos[0].ObservacaoDestinatario != undefined && ponto.Pedidos[0].ObservacaoDestinatario != '') {
                objMarker.marker.setLabel(_labelObservacao);
            }
            if (ponto.Pedidos[0].ValorCobrancaFreteCombinado > 0)
                objMarker.marker.setLabel(_labelFreteCombinado);
        }

        _arrayMarker.push(objMarker);

    }
    _pedidoMapa.TotalPedidos.val(totalPontos);
    setarPontosSelecionados(true);
}

function RemoverPontos() {
    for (var i in _arrayMarker) {
        clearListenerMarker(_arrayMarker[i].marker);
        _arrayMarker[i].marker.setMap(null);
    }
    _arrayMarker = [];
    //_markerCluster.clearMarkers();
}

function clearListenerMarker(marker) {
    google.maps.event.clearListeners(marker);
    google.maps.event.clearInstanceListeners(marker);
}

var _markerDraggable = null;

function getMarkersFromIdentificador(identificador) {
    var markers = [];
    for (var i in _arrayMarker) {
        if (_arrayMarker[i].identificador == identificador) {
            markers.push(_arrayMarker[i]);
        }
    }
    return markers;
}

/* Obtem a lista de markers porem com a mesma coordenada */
function getMarkersFromMarker(objMarker) {
    var markers = [];
    for (var i in _arrayMarker) {
        if (_arrayMarker[i].identificador == objMarker.identificador && _arrayMarker[i].endereco.Latitude == objMarker.endereco.Latitude && _arrayMarker[i].endereco.Longitude == objMarker.endereco.Longitude) {
            markers.push(_arrayMarker[i]);
        }
    }
    return markers;
}

function getMarkerFromPedido(codigoPedido) {
    for (var i in _arrayMarker) {
        for (var p in _arrayMarker[i].pedidos) {
            var key = _arrayMarker[i].pedidos[p].Codigo;
            if (key == codigoPedido) {
                return _arrayMarker[i];
            }
        }
    }
    return null;
}

//function getCodigosPedidosMarkersFromIdentificador(identificador) {
//    //var codigoPedidosDoPonto = ponto.Pedidos.map(function (ped) { return ped.Codigo; });
//    var codigos = [];
//    var markers = getMarkersFromIdentificador(identificador);
//    for (var i in markers) {
//        for (var p in markers[i].pedidos) {
//            var key = markers[i].pedidos[p].Codigo;
//            if (!(key in codigos)) {
//                codigos.push(key);
//            }
//        }
//    }
//    return codigos;
//}

function getPedidosPorCodigo(codigo) {
    var pedido = PEDIDOS.where(function (ped) { return codigo == ped.Codigo; });
    return pedido;
}

/* Criado nova função getPedidosMarkersLocalizacao abaixo para obter apenas os pedidos de entrega no mesmo ponto.
 * pois clientes com 2 endereços de entrega .. apresentando comportamento diferente.
 */
//function getPedidosMarkersFromIdentificador(identificador, somenteCodigo) {
//    //var codigoPedidosDoPonto = ponto.Pedidos.map(function (ped) { return ped.Codigo; });
//    var codigos = [];
//    var pedidos = []
//    var markers = getMarkersFromIdentificador(identificador);
//    for (var i in markers) {
//        for (var p in markers[i].pedidos) {
//            var key = markers[i].pedidos[p].Codigo;
//            if (!(key in codigos)) {
//                codigos.push(key);
//                pedidos.push(markers[i].pedidos[p]);
//            }
//        }
//    }
//    if (somenteCodigo) {
//        return codigos;
//    } else {
//        return pedidos;
//    }
//}

function getPedidosMarkersLocalizacao(objMarker, somenteCodigo) {
    var codigos = [];
    var pedidos = []
    //var markers = getMarkersFromIdentificador(objMarker.identificador);
    var markers = getMarkersFromMarker(objMarker);
    for (var i in markers) {
        //if (markers[i].endereco.Latitude == objMarker.endereco.Latitude && markers[i].endereco.Longitude == objMarker.endereco.Longitude) {
        for (var p in markers[i].pedidos) {
            var key = markers[i].pedidos[p].Codigo;
            if (!(key in codigos)) {
                codigos.push(key);
                pedidos.push(markers[i].pedidos[p]);
            }
        }
        //}
    }
    if (somenteCodigo) {
        return codigos;
    } else {
        return pedidos;
    }
}

function attachEventMarker(objMarker, point) {

    ////Limpando todos os eventos do marker...
    clearListenerMarker(objMarker.marker);

    if (!objMarker.coleta) {

        objMarker.marker.addListener('rightclick', function () {
            //Validar o carregamento selecionado
            var pesquisar = true;
            if (_carregamento && _carregamento.Carregamento.codEntity() === parseInt(objMarker.codigo_carregamento)) {
                pesquisar = false;
            } else if (objMarker.codigo_carregamento == 0) {
                pesquisar = false;
            }
            if (pesquisar) {
                resetColorPolylines();
                detalharCarregamentoClickPolyline(parseInt(objMarker.codigo_carregamento));
            }
            _menu_carga = null;
            menuContextCarregamento(objMarker.codigo_carregamento, objMarker.pedidos[0].Codigo, objMarker.pedidos.length, true, objMarker);
            if (_menu_carga) {
                var tmp = getCanvasXY(objMarker.marker.position);
                _menu_carga.$menu[0].style.top = (tmp.y + 75) + 'px';
                _menu_carga.$menu[0].style.left = tmp.x + 'px';
                _menu_carga.open();
            }
        });

        objMarker.marker.addListener('click', function () {
            markerClick(objMarker);
        });
    }

    objMarker.marker.addListener('mouseover', function (event) {
        if (_markerDraggable != null) {
            if (_markerDraggable != objMarker.marker) {
                _markerDraggable.setOptions({ draggable: false });
            }
        }
        objMarker.marker.setOptions({ draggable: true });
        _markerDraggable = objMarker.marker;

        //Vamos criar um timeout de 1 segundo para abrir as informações.. pois quando tem muitos.. atrapalha abrir a todo instante...
        clearTimeout(_info_window_timer);

        _info_window_timer = setTimeout(function () {
            var pesoTotal = 0;
            var valorTotal = 0;
            var pedidos = getPedidosMarkersLocalizacao(objMarker, false);
            var pedido = pedidos[0];

            var nro_carregamento = '';
            if (objMarker.codigo_carregamento > 0) {
                var index = obterIndiceKnoutCarregamento(objMarker.codigo_carregamento);
                if (index >= 0) {
                    nro_carregamento = '    <div style="padding-top: 10px;"><b>' + Localization.Resources.Cargas.MontagemCargaMapa.Carregamento + ': </b>' + _knoutsCarregamentos[index].NumeroCarregamento.val() + '</div>';
                }
            }

            var position = objMarker.marker.position;

            var contentString =
                '<div class="map-window">' +
                '    <div class="nome-cliente">' +
                '        <h6 class="h6">' + objMarker.pessoa + '</h6>' +
                '         <div style ="padding-top: 2px;"> <b>' + Localization.Resources.Cargas.MontagemCargaMapa.NomeFantasia + ': </b>' + objMarker.pessoaNomeFantasia + '</div >' +
                '    </div>' +

                (nro_carregamento != '' ? nro_carregamento : '') +

                '    <div style="padding-top: 10px;"><b>' + Localization.Resources.Cargas.MontagemCargaMapa.PrevisaoEntrega + ': </b>' + pedido.DataPrevisaoEntrega + '</div>' +
                '    <div style="padding-top: 10px;"><td><strong>' + Localization.Resources.Cargas.MontagemCargaMapa.CEP + ':</strong> ' + pedido.CEPDestinatario + '</td></div>' +
                (pedido.ValorCobrancaFreteCombinado == 0 ? '' : '    <div style="padding-top: 10px;"><td><strong>' + Localization.Resources.Cargas.MontagemCargaMapa.ValorCobrancaFreteCombinado + '</strong> ' + formatterCurrency.format(pedido.ValorCobrancaFreteCombinado) + '</td></div>') +
                '    <div style="padding: 10px 0px 15px;"><td><div class="col-lg-6"><strong>' + Localization.Resources.Cargas.MontagemCargaMapa.Latitude + ':</strong> ' + position.lat() + '</div><div class="col-lg-6"><strong>' + Localization.Resources.Cargas.MontagemCargaMapa.Longitude + ':</strong> ' + position.lng() + '</div></td></div>' +
                '    <div class="pedidos" style="max-height: 100px; padding-top: 10px;">' +
                '        <table style="width: 100%;">' +
                '          <thead><th>' + Localization.Resources.Cargas.MontagemCargaMapa.Data + '</th>' +
                '                 <th>' + Localization.Resources.Cargas.MontagemCargaMapa.Pedidos + '</th>' +
                '                 <th>' + Localization.Resources.Cargas.MontagemCargaMapa.TipoCarga  + '</th>' +
                '                 <th>' + Localization.Resources.Cargas.MontagemCargaMapa.Peso + '</th>' +
                '                 <th>' + Localization.Resources.Cargas.MontagemCargaMapa.TipoDePagamento + '</th>' +
                '                 <th>' + Localization.Resources.Cargas.MontagemCargaMapa.ValorMercadoria + '</th></thead><tbody>' +
                (function () {
                    return pedidos.map(function (pedido) {
                        pesoTotal += Globalize.parseFloat(pedido.Peso);
                        valorTotal += Globalize.parseFloat(pedido.ValorTotalNotasFiscais);
                        return "<tr><td>" + pedido.DataCarregamentoPedido + "</td><td>" + pedido.NumeroPedidoEmbarcador + "</td><td>" + pedido.TipoCarga + "</td><td>" + pedido.Peso + "</td><td>" + pedido.DescricaoTipoCondicaoPagamento + "</td><td>" + pedido.Valor + "</td></tr>";
                    }).join("");
                }()) +
                '       <tr><td colspan="3"><strong>' + Localization.Resources.Cargas.MontagemCargaMapa.PesoTotal + ':</strong></td><td><strong>' + Globalize.format(pesoTotal, "n4") + '</strong></td></tr>' +
                '       <tr><td colspan="5"><strong>' + Localization.Resources.Cargas.MontagemCargaMapa.ValorTotalDaMercadoria + ':</strong></td><td><strong>R$ ' + Globalize.format(valorTotal, "n2") + '</strong></td></tr>' +
                '       </tbody></table>' +
                '    </div>' +
                (pedido.ObservacaoDestinatario != '' ? '<div style="padding-top: 10px;"><td><strong>' + Localization.Resources.Cargas.MontagemCargaMapa.Observacao + ':</strong>' + pedido.ObservacaoDestinatario + '</td></div>' : '') +
                '</div>';

            if (_info_window.map != null) {
                _info_window.close();
            }

            _info_window.setContent(contentString);
            _info_window.setPosition(event.latLng);
            _info_window.open(objMarker.marker.get('map'), objMarker.marker);

        }, 700);
    });

    //Adicionando o evento para o dragend na linha 
    objMarker.marker.addListener('dragend', function () {
        var newPoint = this.getPosition();
        objMarker.marker.setPosition(point);

        if (_polylineCargas && _Carregamentos) {
            var nro_carga_new = 0;
            for (i in _Carregamentos) {
                if (_polylineCargas[_Carregamentos[i].Codigo] != undefined) {
                    if (_polylineCargas[_Carregamentos[i].Codigo].getVisible()) {
                        var contain = google.maps.geometry.poly.isLocationOnEdge(newPoint, _polylineCargas[_Carregamentos[i].Codigo], .002);
                        if (contain) {
                            nro_carga_new = _Carregamentos[i].Codigo;
                            break;
                        }
                    }
                }
            };
            //Achou o novo carregamento ao arrastar sobre a linha.
            if (nro_carga_new > 0) {
                //var codigos = getPedidosMarkersFromIdentificador(objMarker.identificador, true);
                var codigos = getPedidosMarkersLocalizacao(objMarker, true);
                if (objMarker.codigo_carregamento > 0) {
                    if (exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.MontagemCargaMapa.DesejaAlterarCarregamentoDoPedido, function () {
                        removerPedidosCarregamento(objMarker.codigo_carregamento, codigos, function () {
                            adicionarPedidosCarregamento(nro_carga_new, codigos, function () {
                                BuscarDadosMontagemCarga(2);
                            });
                        });
                    }));
                } else {
                    adicionarPedidosCarregamento(nro_carga_new, codigos, function () {
                        BuscarDadosMontagemCarga(2);
                    });
                }
            }
        }
    });
}

function setarPontosSelecionados(ajustarPosicaoMapa) {
    var bounds = new google.maps.LatLngBounds();
    var boundsNaoEntrados = new google.maps.LatLngBounds();

    if (_arrayMarker.length == 0)
        return setarCoordenadas();

    var encontrou = false;
    for (var i = 0; i < _arrayMarker.length; i++) {
        var marker = _arrayMarker[i];
        var objIconSet = null;
        var loc = marker.marker.getPosition();

        if (marker.coleta) {
            objIconSet = GetIconMarker(_EnumMarker.PinColeta);
        }
        else if (marker.reentrega) {
            objIconSet = GetIconMarker(_EnumMarker.PinReentrega);
        }
        else {
            if (marker.selecionado) {
                if (marker.distribuidor) {
                    objIconSet = GetIconMarker(_EnumMarker.DistribuidorSelecionado); //  GerarIconMarker(_EnumMarker.DistribuidorSelecionado);
                } else {
                    objIconSet = GetIconMarker(_EnumMarker.PinSelecionado);         // GerarIconMarker(_EnumMarker.PinSelecionado);
                }
                encontrou = true;
                bounds.extend(loc);
            } else {
                if (marker.distribuidor) {
                    objIconSet = GetIconMarker(_EnumMarker.Distribuidor);           // GerarIconMarker(_EnumMarker.Distribuidor);
                } else {
                    objIconSet = GetIconMarker(_EnumMarker.Pin);                    // GerarIconMarker(_EnumMarker.Pin);
                }
            }
        }
        boundsNaoEntrados.extend(loc);
        marker.marker.setIcon(objIconSet);
        var zIndex = marker.codigo_carregamento;
        if (marker.codigos && marker.codigos.length > 0)
            zIndex += marker.codigos[0];
        marker.marker.setZIndex(zIndex);
    }

    if (encontrou) {
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
    drawPolylineCarregamentos();
}

function drawPolylineCarregamentos(codigoCarregamento) {
    //Agora vamos pegar os demais pontos dos demais carregamentos
    var addPolylineFromOrig = true;
    if (_AreaCarregamento != null) {
        addPolylineFromOrig = _AreaCarregamento.DesenharPolilinhaRotaApartirOrigem.val();
    }
    if (isNaN(parseInt(codigoCarregamento))) {
        codigoCarregamento = 0;
    }

    clearPolylines(codigoCarregamento);
    clearMarkerMultiplosCarregamentoPedido();

    if (_Carregamentos) {
        var carregamentosPedido = [];
        var carregamentosDestinatario = {};
        for (var i in _Carregamentos) {

            i = parseInt(i);
            if (_Carregamentos[i]) {

                if (codigoCarregamento == 0 || codigoCarregamento == _Carregamentos[i].Codigo) {

                    var coordinates = [];
                    var distintos = [];
                    var cod_pedidos = _Carregamentos[i].Roteirizacao.Pedidos;
                    var selecionado = false;
                    var cont = 0;
                    if (addPolylineFromOrig) {
                        var ponto = ObterPontoRemetente();
                        if (ponto != null && ponto.lat != 0) {
                            coordinates[cont] = { lat: ponto.lat, lng: ponto.lng, destinatario: 'C.D' };
                            cont = 1;
                        }
                    }

                    var carregamentoVisivelMapa = true;
                    //Aki vamos checar se o carregamento está para ser exibido no mapa...
                    var idxKnoutCarregamento = obterIndiceKnoutCarregamento(_Carregamentos[i].Codigo);
                    if (idxKnoutCarregamento >= 0) {
                        carregamentoVisivelMapa = _knoutsCarregamentos[idxKnoutCarregamento].ExibirCarregamentoMapa.val();
                    }

                    for (var j in cod_pedidos) {
                        var index = obterIndiceMakerPedidoCodigo(parseInt(cod_pedidos[j]));
                        if (index >= 0 && !distintos[index]) {
                            coordinates[cont] = { lat: _arrayMarker[index].marker.position.lat(), lng: _arrayMarker[index].marker.position.lng(), destinatario: _arrayMarker[index].endereco.Destinatario };
                            distintos[index] = index;
                            cont++;
                        }
                        if (index >= 0) {

                            var cnpj = _arrayMarker[index].endereco.Destinatario;

                            //Pedidos compartilhados...
                            if (carregamentosPedido[parseInt(cod_pedidos[j])] == undefined) {
                                carregamentosPedido[parseInt(cod_pedidos[j])] = {
                                    qtde: 1,
                                    cod_pedido: parseInt(cod_pedidos[j]),
                                    lat: _arrayMarker[index].marker.position.lat(),
                                    lng: _arrayMarker[index].marker.position.lng(),
                                    destinatario: cnpj,
                                    descr_destinatario: _arrayMarker[index].pessoa
                                };
                            } else if (carregamentoVisivelMapa) {
                                carregamentosPedido[parseInt(cod_pedidos[j])].qtde += 1;
                            }

                            //Destinatarios carregamentos
                            if (!(cnpj in carregamentosDestinatario)) {
                                carregamentosDestinatario[cnpj] = {
                                    qtde: 1,                    // Contem a quantidade total de carregamentos do destinatario
                                    lat: _arrayMarker[index].marker.position.lat(),
                                    lng: _arrayMarker[index].marker.position.lng(),
                                    destinatario: cnpj,
                                    descr_destinatario: _arrayMarker[index].pessoa,
                                    qtde_total: 0,              // Contem a quantidade total de pedidos compartilhados do destino
                                    pedido_carregamentos: [],   // Mantem o pedido compartilhado e a quantidade de carregamento
                                    carregamentos: []
                                };
                            } else if (carregamentoVisivelMapa) {
                                carregamentosDestinatario[cnpj].qtde += 1;
                            }

                            if ($.inArray(_Carregamentos[i].Codigo, carregamentosDestinatario[cnpj].carregamentos) < 0 && carregamentoVisivelMapa) {
                                carregamentosDestinatario[cnpj].carregamentos.push(_Carregamentos[i].Codigo);
                            }

                            _arrayMarker[index].codigo_carregamento = _Carregamentos[i].Codigo;
                            _arrayMarker[index].marker.setIcon(GerarIconMarkerCarregamento(_Carregamentos[i].Codigo, _arrayMarker[index].distribuidor, _arrayMarker[index].reentrega));
                            if (_arrayMarker[index].selecionado === true)
                                selecionado = true;
                        }
                    }

                    drawPolyline(coordinates, false, _Carregamentos[i].Codigo);
                }
            }
        }

        //Agora vamos pegar os destinatarios distintos
        var destinatarios = {};
        for (var i in carregamentosPedido) {
            if (carregamentosPedido[i] != undefined) {
                var qtde = parseInt(carregamentosPedido[i].qtde);
                var cnpj = carregamentosPedido[i].destinatario;
                if (qtde > 1) {
                    if (!(cnpj in destinatarios)) {
                        destinatarios[cnpj] = {
                            lat: carregamentosPedido[i].lat,
                            lng: carregamentosPedido[i].lng,
                            destinatario: cnpj,
                            descr_destinatario: carregamentosPedido[i].descr_destinatario,
                            qtde_total: 0,              // Contem a quantidade total de pedidos compartilhados do destino
                            pedido_carregamentos: [],   // Mantem o pedido compartilhado e a quantidade de carregamento
                            carregamentos: []
                        };
                    }
                    destinatarios[cnpj].qtde_total += qtde;
                    destinatarios[cnpj].pedido_carregamentos.push({
                        cod_pedido: carregamentosPedido[i].cod_pedido,
                        qtde: qtde
                    });
                }
            }
        }
        //Agora vamos gerar um novo marker para os pedidos com mais de um carregamento
        for (var cnpj in destinatarios) {
            var ponto = destinatarios[cnpj];
            createMarkerMultiplosCarregamentoPedido(ponto, ponto.qtde_total, false);
        }
        //Agora vamos gerar um marker para os destinatarios que possuem carregamentos compartilhados e não pedidos compartilhados.
        for (var i in carregamentosDestinatario) {
            if (carregamentosDestinatario[i].carregamentos.length > 1) {
                //Agora vamos ver se não está na lista de destinatarios com pedidos compartilhados
                if (!(i in destinatarios)) {
                    var ponto = carregamentosDestinatario[i];
                    createMarkerMultiplosCarregamentoPedido(ponto, ponto.carregamentos.length, true);
                }
            }
        }
    }
}

var _arrayMarkerMultiplosCarregamentoPedido = [];

function clearMarkerMultiplosCarregamentoPedido() {
    for (var i in _arrayMarkerMultiplosCarregamentoPedido) {
        clearListenerMarker(_arrayMarkerMultiplosCarregamentoPedido[i]);
        _arrayMarkerMultiplosCarregamentoPedido[i].setMap(null);
    }
    _arrayMarkerMultiplosCarregamentoPedido = [];
}

function imgMarkerDefault() {
    var url = '';
    url = "../img/montagem-carga-mapa/markers/colors/numbers/_red.png";
    return url;
}

function createMarkerMultiplosCarregamentoPedido(item, qtde, carregamento) {
    return false;
    var latLng = { lat: item.lat, lng: item.lng };
    var marker = new google.maps.Marker({
        map: _map,
        position: latLng,
        zIndex: 999999999,
        destinatario: item.destinatario,
        descr_destinatario: item.descr_destinatario,
        pedido_carregamentos: item.pedido_carregamentos,    // Mantem o pedido compartilhado e a quantidade de carregamento
        carregamento: carregamento,                         // bolean se é markador com qtde de carregamentos
        carregamentos: item.carregamentos

    });

    //var color = '';
    //if (carregamento) {
    //    color = '&color=_black';
    //}

    //executarReST("MontagemCarga/GetMarkerColorNumber?number=" + qtde + color, null, function (arg) {
    //    if (arg.Success) {
    //        if (arg.Data !== false) {
    //            var retorno = arg.Data;
    //            marker.setIcon(retorno);
    //        } else {
    //            marker.setIcon(imgMarkerDefault());
    //        }
    //    } else {
    //        marker.setIcon(imgMarkerDefault());
    //    }
    //}, null);

    if (carregamento) {
        marker.setIcon('../img/montagem-carga-mapa/markers/colors/numbers/' + qtde + '_black.png');
    } else {
        marker.setIcon('../img/montagem-carga-mapa/markers/colors/numbers/' + qtde + '_red.png');
    }

    marker.addListener('click', function () {
        filtrarCarregamentosDestinatario(marker.destinatario);
    });

    marker.addListener('mouseover', function (event) {
        //Vamos criar um timeout de 1 segundo para abrir as informações.. pois quando tem muitos.. atrapalha abrir a todo instante...
        clearTimeout(_info_window_timer);

        _info_window_timer = setTimeout(function () {
            var contentString =
                '<div class="map-window">' +
                '    <div class="nome-cliente">' +
                '        <h6 class="h6">' + marker.descr_destinatario + '</h6>' +
                '        <h6 class="h6"></h6>' +
                '        <h6 class="h6">' + (carregamento == false ? Localization.Resources.Cargas.MontagemCargaMapa.NumeroPedido + ' - ' : '') + Localization.Resources.Cargas.MontagemCargaMapa.Carregamentos + '</h6>' +
                '    </div>' +
                '    <div class="pedidos" style="max-height: 120px;">' +
                '        <ul>' +
                (carregamento == false ?
                    (function () {
                        return marker.pedido_carregamentos.map(function (pedido) {
                            var nro = getPedidosPorCodigo(pedido.cod_pedido).NumeroPedidoEmbarcador;
                            return "<li class='clicklifunction li-info-window-click' nro-ped-embarcador='" + nro + "'>" + nro + " - " + pedido.qtde + "</li>";
                            //return "<li style='cursor: pointer; color: blue; font-style: oblique;' onClick='filtrarCarregamentosPedido(" + nro + ");'>" + nro + " - " + pedido.qtde + "</li>";
                        }).join("");
                    }())
                    :
                    (function () {
                        return marker.carregamentos.map(function (carregamento) {
                            var index = obterIndiceKnoutCarregamento(carregamento);
                            if (index >= 0) {
                                var nro_carregamento = _knoutsCarregamentos[index].NumeroCarregamento.val();
                                return "<li>" + nro_carregamento + "</li>";
                            } else {
                                return "<li>" + carregamento + "</li>";
                            }
                            //return "<li style='cursor: pointer; color: blue; font-style: oblique;' onClick='filtrarCarregamentosPedido(" + nro + ");'>" + nro + " - " + pedido.qtde + "</li>";
                        }).join("");
                    }())
                ) +
                '        </ul>' +
                '    </div>' +
                '</div>';

            $(".clicklifunction").off('click');

            if (_info_window.map != null) {
                _info_window.close();
            }

            _info_window.setContent(contentString);
                        
            _info_window.setPosition(event.latLng);
            _info_window.open(marker.get('map'), marker);
            setTimeout(function () {
                $(".clicklifunction").on('click', function () {
                    var nro = $(this).attr("nro-ped-embarcador");
                    filtrarCarregamentosPedido(nro);
                });
            }, 1000);
            
        }, 700);
    });

    _arrayMarkerMultiplosCarregamentoPedido.push(marker);
}

function filtrarCarregamentosPedido(nro) {
    _pesquisaMontegemCargaCarregamentos.CodigoPedidoEmbarcador.val(nro);
    filtrarCarregamentos();
}

function filtrarCarregamentosDestinatario(cnpj) {
    _pesquisaMontegemCargaCarregamentos.Destinatario.codEntity(cnpj);
    _pesquisaMontegemCargaCarregamentos.Destinatario.val(cnpj);
    filtrarCarregamentos();
}
function disposeCarregamentoMapa(codigo) {
    if (_polylineCargas) {
        if (_polylineCargas[codigo]) {
            disposePolyline(_polylineCargas[codigo]);
        }
    }
    disposeDirection();
    //Limpar os markers...
    if (_arrayMarker) {
        $(_arrayMarker).each(function (i) {
            if (_arrayMarker[i].codigo_carregamento == codigo) {
                _arrayMarker[i].codigo_carregamento = 0;
                if (_arrayMarker[i].distribuidor) {
                    //_arrayMarker[i].marker.setIcon(GerarIconMarker(_EnumMarker.Distribuidor));
                    _arrayMarker[i].marker.setIcon(GetIconMarker(_EnumMarker.Distribuidor));
                } else {
                    //_arrayMarker[i].marker.setIcon(GerarIconMarker(_EnumMarker.Pin));
                    _arrayMarker[i].marker.setIcon(GetIconMarker(_EnumMarker.Pin));
                }
            }
        });
    }
}

function clearPolylines(codigoCarregamento) {
    if (_polylineCargas) {
        if (isNaN(parseInt(codigoCarregamento))) {
            codigoCarregamento = 0;
        }
        for (var i in _polylineCargas) {
            if (_polylineCargas[i] && (codigoCarregamento == 0 || codigoCarregamento == i)) {
                disposePolyline(_polylineCargas[i]);
            }
        }
    }
}

function disposePolyline(polyline) {
    google.maps.event.clearListeners(polyline);
    polyline.setMap(null);
    //Dispose direction
    //disposeDirection();
}

function disposeDirection() {
    if (_polylineDirection) {
        _polylineDirection.setMap(null);
        _polylineDirection = null;
    }
}

function drawPolyline(coordinates, selecionada, codigo_carregamento) {
    var strokeColor = colourHex(codigo_carregamento);
    //if (selecionada) strokeColor = colourHex(-1);
    coordinates = coordinates.filter(function (n) { return n != undefined });
    if (coordinates.length > 1) {
        if (!_polylineCargas) _polylineCargas = [];
        if (_polylineCargas[codigo_carregamento]) {
            disposePolyline(_polylineCargas[codigo_carregamento]);
        }
        _polylineCargas[codigo_carregamento] = new google.maps.Polyline({
            path: coordinates,
            geodesic: true,
            strokeColor: strokeColor,
            strokeOpacity: 1.0,
            strokeWeight: (selecionada == true ? 6 : 3.5),
            type: 'polyline_carregamento',
            codigo_carregamento: codigo_carregamento,
            zIndex: 500
        });
        _polylineCargas[codigo_carregamento].setMap(_map);
        attachEventPolyline(_polylineCargas[codigo_carregamento]);
        //Vamos verificar se o carregamento não está oclto no mapa.
        var index = obterIndiceKnoutCarregamento(codigo_carregamento);
        if (index >= 0) {
            var value = _knoutsCarregamentos[index].ExibirCarregamentoMapa.val();
            if (!value) {
                _polylineCargas[codigo_carregamento].setVisible(false);
            }
        }
    }
}

var _polylineDirection;

function drawPolylineDirection(polylineGeometry) {
    disposeDirection();
    if (polylineGeometry) {
        var decode = google.maps.geometry.encoding.decodePath(polylineGeometry);
        _polylineDirection = new google.maps.Polyline({
            path: decode,
            strokeColor: 'blue',
            strokeOpacity: 0.7,
            strokeWeight: 6,
            type: 'polyline_direction',
            zIndex: 500
        });
        _polylineDirection.setMap(_map);
    }
}

function drawPolylineSelecionados() {
    /// Primeiro pegar os pedidos do carregamento...
    var addPolylineFromOrig = true;
    if (_AreaCarregamento != null) {
        addPolylineFromOrig = _AreaCarregamento.DesenharPolilinhaRotaApartirOrigem.val();
    }

    var cod_pedidos = [];
    var codigo = parseInt(_carregamento.Carregamento.codEntity());
    var index = obterIndiceCarregamentoCodigo(codigo);
    if (index >= 0) {
        cod_pedidos = _Carregamentos[index].Roteirizacao.Pedidos;
    }
    var pedidos = PEDIDOS_SELECIONADOS();
    for (var i in pedidos) {
        var pedido = pedidos[i];
        if ($.inArray(parseInt(pedido.Codigo), cod_pedidos) < 0) {
            cod_pedidos.push(parseInt(pedido.Codigo));
        }
    }
    var coordinates = [];
    var cont = 0;
    if (addPolylineFromOrig) {
        var ponto = ObterPontoRemetente();
        if (ponto != null && ponto.lat != 0) {
            coordinates[cont] = { lat: ponto.lat, lng: ponto.lng, destinatario: 'C.D' };
            cont = 1;
        }
    }
    for (var i in cod_pedidos) {
        var marker = otberMarkerPedidoCodigo(cod_pedidos[i]);
        if (marker) {
            coordinates[cont] = { lat: marker.marker.position.lat(), lng: marker.marker.position.lng(), destinatario: marker.endereco.Destinatario };
            cont++;
        }
    }
    drawPolyline(coordinates, true, codigo);
}

function resetColorPolylines() {
    for (var i in _polylineCargas) {
        if (_polylineCargas[i]) {
            var cod_carregamento = _polylineCargas[i].codigo_carregamento;
            var strokeColor = colourHex(cod_carregamento);
            _polylineCargas[i].setOptions({ strokeColor: strokeColor, strokeWeight: 3.5 });
        }
    }
}

function attachEventPolyline(path) {
    //Removendo os eventos da linha
    google.maps.event.clearListeners(path);

    if (parseInt(path.codigo_carregamento) > 0) {
        google.maps.event.addListener(path, 'click', function (e) {
            var pesquisar = true;
            if (_carregamento && _carregamento.Carregamento.codEntity() === parseInt(this.codigo_carregamento)) {
                pesquisar = false;
            } else if (this.codigo_carregamento == 0) {
                pesquisar = false;
            }
            if (pesquisar) {
                detalharCarregamentoClickPolyline(parseInt(this.codigo_carregamento));
            }
        });

        if (!_sessaoRoteirizadorParametros.OcultarDetalhesDoPontoNoMapa.val())
            attachMouseOverEventPolyLine(path);

        google.maps.event.addListener(path, 'rightclick', function (e) {
            var pesquisar = true;
            if (_carregamento && _carregamento.Carregamento.codEntity() === parseInt(this.codigo_carregamento)) {
                pesquisar = false;
            } else if (this.codigo_carregamento == 0) {
                pesquisar = false;
            }
            if (pesquisar) {
                resetColorPolylines();
                detalharCarregamentoClickPolyline(parseInt(this.codigo_carregamento));
            }
            _menu_carga = null;
            menuContextCarregamento(path.codigo_carregamento, 0, 0, false, null);
            if (_menu_carga && !sessaoRoteirizadorFinalizada()) {
                var tmp = getCanvasXY(e.latLng);
                _menu_carga.$menu[0].style.top = (tmp.y + 75) + 'px';
                _menu_carga.$menu[0].style.left = tmp.x + 'px';
                _menu_carga.open();
            }
            _info_window.close();
        });
    }
}

function attachMouseOverEventPolyLine(path) {
    google.maps.event.addListener(path, 'mouseover', function (event) {
        //Vamos criar um timeout de 1 segundo para abrir as informações.. pois quando tem muitos.. atrapalha abrir a todo instante...
        clearTimeout(_info_window_timer);

        _info_window_timer = setTimeout(function () {
            var codigo_carregamento = path.codigo_carregamento;
            var index = obterIndiceKnoutCarregamento(codigo_carregamento);
            if (index >= 0) {
                var nro_carregamento = _knoutsCarregamentos[index].NumeroCarregamento.val();
                var info_pallets = '';
                if (_knoutsCarregamentos[index].Pallets.visible() == true) {
                    info_pallets = '<br/><b>' + Localization.Resources.Cargas.MontagemCargaMapa.Pallets + '</b> .: ' + _knoutsCarregamentos[index].Pallets.val() + '<b style="margin-left: 15%">' + Localization.Resources.Cargas.MontagemCargaMapa.Ocupacao + '</b> .: ' + formatPercentOcupacao(_knoutsCarregamentos[index].OcupacaoPallets.val());
                }
                var info_cubagem = '';
                if (_knoutsCarregamentos[index].Cubagem.visible() == true) {
                    info_cubagem = '<br/><b>' + Localization.Resources.Cargas.MontagemCargaMapa.Cubagem + '</b> .: ' + _knoutsCarregamentos[index].Cubagem.val() + '<b style="margin-left: 15%">' + Localization.Resources.Cargas.MontagemCargaMapa.Ocupacao + '</b> .: ' + formatPercentOcupacao(_knoutsCarregamentos[index].OcupacaoCubagem.val());
                }
                var info = '<b>' + Localization.Resources.Cargas.MontagemCargaMapa.Carregamento + '</b> .: ' + nro_carregamento + '<br/>' +
                    '<b>' + Localization.Resources.Cargas.MontagemCargaMapa.Filial + '</b> .: ' + _knoutsCarregamentos[index].Filial.val() + '<br/>' +
                    '<b>' + Localization.Resources.Cargas.MontagemCargaMapa.Transportador + '</b> .: ' + _knoutsCarregamentos[index].Transportador.val() + '<br/>' +
                    '<b>' + Localization.Resources.Cargas.MontagemCargaMapa.DataProgramada + '</b> .: ' + _knoutsCarregamentos[index].DataProgramada.val() + '<br/>' +
                    '<b>' + Localization.Resources.Cargas.MontagemCargaMapa.EntregasBarraPedidos + '</b> .: ' + _knoutsCarregamentos[index].QtdeEntregasPedidos.val() + '<br/>' +
                    '<b>' + Localization.Resources.Cargas.MontagemCargaMapa.PesoTotal + '</b> .: ' + _knoutsCarregamentos[index].Peso.val() + '<b style="margin-left: 15%">' + Localization.Resources.Cargas.MontagemCargaMapa.Ocupacao + '</b> .: ' + formatPercentOcupacao(_knoutsCarregamentos[index].OcupacaoPeso.val()) +
                    info_pallets +
                    info_cubagem
                    ;

                if (_info_window.map != null) {
                    _info_window.close();
                }

                _info_window.setContent(info);
                
                _info_window.setPosition(event.latLng);
                if (_menu_carga == null || _menu_carga == undefined)
                    _info_window.open(path.get('map'));
                else if (_menu_carga.$menu[0].style.display == 'none') {
                    _info_window.open(path.get('map'));
                }
                
            }
        }, 1000);
    });
}

function getCanvasXY(caurrentLatLng) {
    var scale = Math.pow(2, _map.getZoom());
    var nw = new google.maps.LatLng(_map.getBounds().getNorthEast().lat(), _map.getBounds().getSouthWest().lng());
    var worldCoordinateNW = _map.getProjection().fromLatLngToPoint(nw);
    var worldCoordinate = _map.getProjection().fromLatLngToPoint(caurrentLatLng);
    var caurrentLatLngOffset = new google.maps.Point(Math.floor((worldCoordinate.x - worldCoordinateNW.x) * scale), Math.floor((worldCoordinate.y - worldCoordinateNW.y) * scale));
    return caurrentLatLngOffset;
}

var _menu_carga;

function menuContextCarregamento(cod_carreg, cod_ped, qtde_ped, marker, objMarker) {
    
    var codigosPedidos = [];
    if (objMarker !== null) {
        //var pedidosMarker = getPedidosMarkersFromIdentificador(objMarker.identificador).map((item) => { return { text: item.NumeroPedidoEmbarcador, value: item.Codigo } });
        var pedidosMarker = getPedidosMarkersLocalizacao(objMarker).map((item) => { return { text: item.NumeroPedidoEmbarcador, value: item.Codigo } });
        codigosPedidos = pedidosMarker;
    } else {
        codigosPedidos.push({ text: cod_ped, value: cod_ped });
    }
    if (_menu_carga == null || _menu_carga == undefined) {
        _menu_carga = new BootstrapMenu('#menu-carga', {
            fetchElementData: function ($rowElem) {
                return null;
            },
            actionsGroups: [['otimizar', 'roteirizar', 'exibirRota', 'limparRota'],
            ['exibirCarregamentoMapa', 'ocultarCarregamentoMapa'],
            ['info_pedido', 'remover_pedido_sessao', 'raio_pedido'],
            ['editarCarregamento', 'cancelarCarregamento', 'salvar', 'simularFrete', 'gerarCarga'],
            ['cancelar']
            ],
            menuPosition: "aboveRight",
            actions: {
                otimizar: {
                    name: Localization.Resources.Cargas.MontagemCargaMapa.OtimizarPercurso,
                    iconClass: 'fa-map-marker',
                    onClick: function () {
                        otimizarRotaMenuClick(cod_carreg);
                    },
                    isEnabled: function () {
                        return menuCargaOpoesCarregamento(marker, cod_carreg);
                    }
                },
                roteirizar: {
                    name: Localization.Resources.Cargas.MontagemCargaMapa.VisualizarRoteirizadorCarga,
                    iconClass: 'fa-route',
                    onClick: function () {
                        roteirizarCargaClick();
                    },
                    isEnabled: function () {
                        return menuCargaOpoesCarregamento(marker, cod_carreg);
                    }
                },
                exibirRota: {
                    name: Localization.Resources.Cargas.MontagemCargaMapa.ExibirRota,
                    iconClass: 'fa-flag',
                    onClick: function () {
                        if (_roteirizadorCarregamento) {
                            drawPolylineDirection(_roteirizadorCarregamento.PolilinhaRota.val());
                        }
                    },
                    isEnabled: function () {
                        return false;
                        if (menuCargaOpoesCarregamento(marker, cod_carreg)) {
                            if (_roteirizadorCarregamento) {
                                if (cod_carreg == _roteirizadorCarregamento.Carregamento.val()) {
                                    if (_roteirizadorCarregamento.PolilinhaRota.val()) {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                },
                limparRota: {
                    name: Localization.Resources.Cargas.MontagemCargaMapa.LimparRota,
                    iconClass: 'fa-eraser',
                    onClick: function () {
                        disposeDirection();
                    },
                    isEnabled: function () {
                        return _polylineDirection != null;
                    }
                },
                exibirCarregamentoMapa: {
                    name: Localization.Resources.Cargas.MontagemCargaMapa.ExibirCarregamentoMapa,
                    iconClass: 'fa-map-marker',
                    onClick: function () {
                        showHideCarregamento(cod_carreg, true);
                    },
                    isEnabled: function () {
                        return menuCargaOpoesCarregamento(marker, cod_carreg);
                    }
                },
                ocultarCarregamentoMapa: {
                    name: Localization.Resources.Cargas.MontagemCargaMapa.OcultarCarregamentoMapa,
                    iconClass: 'fa-eraser',
                    onClick: function () {
                        showHideCarregamento(cod_carreg, false);
                    },
                    isEnabled: function () {
                        return menuCargaOpoesCarregamento(marker, cod_carreg);
                    }
                },
                remover_pedido_sessao: {
                    name: Localization.Resources.Cargas.MontagemCargaMapa.RemoverPedidoRoteirizador,
                    iconClass: 'fa-times',
                    onClick: function () {
                        removerPedidoSessao(objMarker.pedidos);
                    },
                    isEnabled: function () {
                        return (marker == true);
                    }
                },
                info_pedido: {
                    name: Localization.Resources.Cargas.MontagemCargaMapa.InformacoesDoPedido,
                    iconClass: 'fa-info',
                    onClick: function () {
                        _filtroDetalhePedido.Pedidos.options([]);
                        _filtroDetalhePedido.Pedidos.visible(false);
                        if (codigosPedidos.length == 1) {
                            ObterDetalhesPedido(cod_ped);
                        } else if (_filtroDetalhePedido) {                            
                            _filtroDetalhePedido.Pedidos.visible(true);
                            _filtroDetalhePedido.Pedidos.options(codigosPedidos);
                        }
                    },
                    isEnabled: function () {
                        return (marker == true && qtde_ped == 1);
                    }
                },
                raio_pedido: {
                    name: Localization.Resources.Cargas.MontagemCargaMapa.RaioGeograficoLocalizacao,
                    iconClass: 'fa-podcast',
                    onClick: function () {
                        modalRaioGeograficoLocalizacao(objMarker);
                    },
                    isEnabled: function () {
                        return (marker == true);
                    }
                },
                editarCarregamento: {
                    name: Localization.Resources.Cargas.MontagemCargaMapa.EditarCarregamento,
                    iconClass: 'fa-edit',
                    onClick: function () {
                        abrirJanelaPedidos();
                    },
                    isEnabled: function () {
                        return cod_carreg > 0;
                    }
                },
                cancelarCarregamento: {
                    name: Localization.Resources.Cargas.MontagemCargaMapa.CancelarCarregamento,
                    iconClass: 'fa-times',
                    onClick: function () {
                        cancelarCarregamentoClick(cod_carreg);
                    },
                    isEnabled: function () {
                        return cod_carreg > 0;
                    }
                },
                salvar: {
                    name: Localization.Resources.Cargas.MontagemCargaMapa.SalvarCarregamento,
                    iconClass: 'fa-save',
                    onClick: function () {
                        atualizarCarregamentoClick();
                    },
                    isEnabled: function () {
                        return cod_carreg > 0;
                    }
                },
                simularFrete: {
                    name: Localization.Resources.Cargas.MontagemCargaMapa.SimularFrete,
                    iconClass: 'fa-search-dollar',
                    onClick: function () {
                        simularFreteClick(true);
                    },
                    isEnabled: function () {
                        return cod_carreg > 0;
                    }
                },
                gerarCarga: {
                    name: Localization.Resources.Cargas.MontagemCargaMapa.GerarCarga,
                    iconClass: 'fa-eraser',
                    onClick: function () {
                        gerarCargaClick();
                    },
                    isEnabled: function () {
                        return cod_carreg > 0;
                    }
                },
                cancelar: {
                    name: Localization.Resources.Cargas.MontagemCargaMapa.Retornar,
                    iconClass: 'fa-reply',
                    onClick: function () {
                        _menu_carga.close();
                    }
                }
            }
        });
    }
}

function menuCargaOpoesCarregamento(marker, cod_carreg) {
    if (!marker)
        return true;
    else if (cod_carreg > 0)
        return true;
    return false;
}

function GetIconMarker(tipoMarker) {
    var marker = "Pin.png";             // Vermelho com circulo central.
    if (tipoMarker == _EnumMarker.Distribuidor || tipoMarker == _EnumMarker.PinColeta)
        marker = 'Distribuidor.png';    //Azul com circulo central.
    if (tipoMarker == _EnumMarker.PinRestricao)
        marker = 'PinRestricao.png';    //Amarelo com círculo central.
    else if (tipoMarker == _EnumMarker.DistribuidorSelecionado || tipoMarker == _EnumMarker.PinSelecionado)
        marker = 'Selecionado.png';     //Verde com círculo central.
    else if (tipoMarker == _EnumMarker.PinReentrega)
        marker = 'PinReentrega.png';     //Amarelo com icone de reentrega.

    return {
        url: "../img/montagem-carga-mapa/markers/" + marker,
        labelOrigin: new google.maps.Point(20, 0)
    };
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

function GerarIconMarkerCarregamento(codigo, distribuidor, reentrega) {
    if (reentrega) {
        return {
            url: "../img/montagem-carga-mapa/markers/PinReentrega.png",
            labelOrigin: new google.maps.Point(20, 0)
        };
    }
    else {
        return imgColorMarker(codigo, distribuidor);
    }
}

function CorMarkerCarregamento(codigo) {
    return colourHex(codigo);
}

function markerClick(objMarker) {
    /**
     * Quando o ponto não possui nenhum pedido selecionado
     * Adiciona todos pedidos ao ponto
     * 
     * Caso contrário, remove todos pedidos desse ponto
     * da lista dos pedidos selecionados
     */
    var cod_carregamento = objMarker.codigo_carregamento;
    //var codigos = getPedidosMarkersFromIdentificador(objMarker.identificador, true);
    var codigos = getPedidosMarkersLocalizacao(objMarker, true);
    if (cod_carregamento > 0 && _carregamento.Carregamento.codEntity() != cod_carregamento) {
        if (exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.MontagemCargaMapa.DesejaAlterarCarregamentoDoPedido, function () {
            removerPedidosCarregamento(cod_carregamento, codigos, function () {
                objMarker.codigo_carregamento = 0;
                confirmMarkerClick(objMarker, cod_carregamento);
            });
        }));
    }
    else if (cod_carregamento > 0 && _carregamento.Carregamento.codEntity() == cod_carregamento) {
        if (exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.MontagemCargaMapa.DesejaRemoverPedidoDoCarregamento, function () {
            removerPedidosCarregamento(cod_carregamento, codigos, function () {
                objMarker.codigo_carregamento = 0;
                confirmMarkerClick(objMarker, cod_carregamento);
            });
        }));
    } else {
        _carregamento.AlteracoesPendentes.val(true);
        confirmMarkerClick(objMarker, cod_carregamento);
    }
}

function confirmMarkerClick(objMarker, cod_carregamento) {
    var new_nro_carregamento = _carregamento.Carregamento.codEntity();

    if (objMarker.selecionado === false) {
        objMarker.selecionado = true;
    } else {
        objMarker.selecionado = false;
    }
    //var markers = getMarkersFromIdentificador(objMarker.identificador);
    var markers = getMarkersFromMarker(objMarker);
    for (var i in markers) {
        markers[i].selecionado = objMarker.selecionado;
        //if (markers[i].selecionado === false) {
        //    markers[i].selecionado = true;
        //} else {
        //    markers[i].selecionado = false;
        //}
    }

    var index_new_carregamento = -1;
    if (new_nro_carregamento > 0 && cod_carregamento != new_nro_carregamento) {
        index_new_carregamento = obterIndiceCarregamentoCodigo(new_nro_carregamento);
    }
    //var pedidos = getPedidosMarkersFromIdentificador(objMarker.identificador, false);
    var pedidos = getPedidosMarkersLocalizacao(objMarker, false);
    EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS = true;

    for (var i in pedidos) {
        SelecionarPedido(pedidos[i], (objMarker.selecionado === false));
        //Adicionando o nro do pedido no array carregamento.
        if (index_new_carregamento >= 0 && objMarker.selecionado) {
            if ($.inArray(pedidos[i], _Carregamentos[index_new_carregamento].Roteirizacao.Pedidos) < 0) {
                _Carregamentos[index_new_carregamento].Roteirizacao.Pedidos.push(pedidos[i]);
            }
        }
    }
    // Se está adicionando em novo carregamento e o nro for diferente do carregametno anterior...
    if (new_nro_carregamento > 0 && cod_carregamento != new_nro_carregamento) {
        objMarker.codigo_carregamento = new_nro_carregamento;
    } else {
        objMarker.codigo_carregamento = 0;
    }

    for (var i in markers) {
        if (new_nro_carregamento > 0 && cod_carregamento != new_nro_carregamento) {
            markers[i].codigo_carregamento = new_nro_carregamento;
        } else {
            markers[i].codigo_carregamento = 0;
        }
    }

    EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS = false;
    _precisarSetarPedidosSelecionadosTabelaPedidosSessao = true;
    PedidosSelecionadosChange();

    if (index_new_carregamento >= 0) {
        ajustarPesosCapacidades(_Carregamentos[index_new_carregamento].Carregamento, true);
    } else {
        var idx = obterIndiceCarregamentoCodigo(cod_carregamento);
        if (idx >= 0) {
            ajustarPesosCapacidades(_Carregamentos[idx].Carregamento, true);
        }
    }
    drawPolylineCarregamentos();
    drawPolylineSelecionados();
}

function obterIndiceMakerPedido(pedido) {
    return obterIndiceMakerPedidoCodigo(pedido.Codigo);
}

function obterIndiceMakerPedidoCodigo(codigo) {
    if (!NavegadorIEInferiorVersao12()) {
        return _arrayMarker.findIndex(function (item) { return item.codigos.includes(codigo) });
    } else {
        for (var i = 0; i < _arrayMarker.length; i++) {
            for (var j = 0; j < _arrayMarker[i].codigos.length; j++) {
                if (codigo == _arrayMarker[i].codigos[j])
                    return i;
            }
        }
        return -1;
    }
}

function obterIndiceMakerDestinatario(cpf_cnpj, codigo_carregamento) {
    if (isNaN(codigo_carregamento)) {
        if (!NavegadorIEInferiorVersao12()) {
            return _arrayMarker.findIndex(function (item) { return item.endereco.Destinatario == cpf_cnpj });
        } else {
            for (var i = 0; i < _arrayMarker.length; i++) {
                if (cpf_cnpj == _arrayMarker[i].endereco.Destinatario)
                    return i;
            }
            return -1;
        }
    } else {
        if (!NavegadorIEInferiorVersao12()) {
            return _arrayMarker.findIndex(function (item) { return item.endereco.Destinatario == cpf_cnpj && item.codigo_carregamento == codigo_carregamento });
        } else {
            for (var i = 0; i < _arrayMarker.length; i++) {
                if (cpf_cnpj == _arrayMarker[i].endereco.Destinatario && codigo_carregamento == _arrayMarker[i].codigo_carregamento)
                    return i;
            }
            return -1;
        }
    }
}

function otberMarkerPedidoCodigo(codigo) {
    if (!NavegadorIEInferiorVersao12()) {
        return _arrayMarker.find(function (item) { return item.codigos.includes(codigo) });
    } else {
        for (var i = 0; i < _arrayMarker.length; i++) {
            for (var j = 0; j < _arrayMarker[i].codigos.length; j++) {
                if (codigo == _arrayMarker[i].codigos[j])
                    return _arrayMarker[i];
            }
        }
        return null;
    }
}

function removerMarkers() {
    for (var i = 0; i < _arrayMarker.length; i++) {
        var marker = _arrayMarker[i];
        marker.marker.setMap(null);
    }
    _arrayMarker = new Array();
    //ObterPontosPedidos();
}

function filtroPedidosMapa(geometria) {
    if (_arrayMarker) {
        var into = [];
        if (geometria.type == google.maps.drawing.OverlayType.CIRCLE) {
            var raio = geometria.overlay.getRadius();
            var center = geometria.overlay.getCenter();
            $(_arrayMarker).each(function (i) {
                if (!_arrayMarker[i].coleta) {
                    var position = _arrayMarker[i].marker.position;
                    var dist = CalcRadiusDistance(center.lat(), center.lng(), position.lat(), position.lng());
                    if ((dist * 1000) <= raio) {
                        into.push(_arrayMarker[i]);
                    }
                }
            });
        } else if (geometria.type == google.maps.drawing.OverlayType.POLYGON) {
            var path = geometria.overlay.getPath();
            if (path != undefined) {
                var polygon = path.getArray();
                $(_arrayMarker).each(function (m) {
                    if (!_arrayMarker[m].coleta) {
                        var result = false;
                        var j = polygon.length - 1;
                        $(polygon).each(function (i) {
                            var position = _arrayMarker[m].marker.position;
                            if (polygon[i].lat() < position.lat() && polygon[j].lat() >= position.lat() || polygon[j].lat() < position.lat() && polygon[i].lat() >= position.lat()) {
                                if (polygon[i].lng() + (position.lat() - polygon[i].lat()) / (polygon[j].lat() - polygon[i].lat()) * (polygon[j].lng() - polygon[i].lng()) < position.lng()) {
                                    result = !result;
                                }
                            }
                            j = i;
                        });
                        if (result) {
                            into.push(_arrayMarker[m]);
                        }
                    }
                });
            }
        } else if (geometria.type == google.maps.drawing.OverlayType.RECTANGLE) {
            var b = geometria.overlay.getBounds();
            var lat_s = b.getSouthWest().lat();
            var lng_s = b.getSouthWest().lng();
            var lat_n = b.getNorthEast().lat();
            var lng_n = b.getNorthEast().lng();
            $(_arrayMarker).each(function (m) {
                if (!_arrayMarker[m].coleta) {
                    var position = _arrayMarker[m].marker.position;
                    if (lat_s <= position.lat() && lat_n >= position.lat() && lng_s <= position.lng() && lng_n >= position.lng()) {
                        into.push(_arrayMarker[m]);
                    }
                }
            });
        }

        //Agora vamos filtrar apenas os markers visibles...

        EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS = true;
        for (var m in into) {
            if (into[m].marker.getVisible()) {
                for (var i in into[m].pedidos) {
                    //(objMarker.selecionado === false)

                    //Vamos selecionar somente se o pedido não está em nenhum carregamento.
                    if (parseInt(into[m].codigo_carregamento) <= 0) {
                        SelecionarPedido(into[m].pedidos[i], false);
                        var pedido = into[m].pedidos[i];
                        var index = obterIndiceMakerPedido(pedido);
                        if (index >= 0) {
                            var new_nro_carregamento = _carregamento.Carregamento.codEntity();
                            // Se está adicionando em novo carregamento e o nro for diferente do carregametno anterior...
                            if (new_nro_carregamento > 0) {
                                _arrayMarker[index].codigo_carregamento = new_nro_carregamento;
                            } else {
                                _arrayMarker[index].codigo_carregamento = 0;
                            }

                        }
                    }
                }
            }
        }
        EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS = false;
        _precisarSetarPedidosSelecionadosTabelaPedidosSessao = true;
        PedidosSelecionadosChange();
    }
}

function modalFiltroRotaLocalidade() {
    Global.abrirModal("divModalFiltroRotaLocalidade");
}

function setLatLngRaioGeograficoLocalizacao(position) {
    _raioGeograficoLocalizacao.LatitudeRaio.val(position.lat().toFixed(6));
    _raioGeograficoLocalizacao.LongitudeRaio.val(position.lng().toFixed(6));
}

// Recebemos o objMarker quando clicado no menu com o botão direito no marker do mapa...opção Raio.
function modalRaioGeograficoLocalizacao(objMarker) {
    if (objMarker) {
        var position = objMarker.marker.position;
        setLatLngRaioGeograficoLocalizacao(position);
    }
    Global.abrirModal('divRaioGeograficoLocalizacao');
    $("#divRaioGeograficoLocalizacao").one('hidden.bs.modal', function () {
        // LimparCampos(_carregamentoAutorizacaoDetalhe);
    });
}

function limparRaioGeograficoLocalizacao() {
    if (_radiusDrawMaps) {
        for (var i in _radiusDrawMaps) {
            _radiusDrawMaps[i].setMap(null);
        }
    }
    _radiusDrawMaps = [];
}

function confirmarRaioGeograficoLocalizacaoClick() {
    fecharModalRaioGeograficoLocalizacao(true);
}

function cancelarRaioGeograficoLocalizacaoClick() {
    fecharModalRaioGeograficoLocalizacao(false);
}

function fecharModalRaioGeograficoLocalizacao(confirmou) {
    if (confirmou) {
        limparRaioGeograficoLocalizacao();
        var lat = parseFloat(_raioGeograficoLocalizacao.LatitudeRaio.val().replace(',', '.'));
        var lng = parseFloat(_raioGeograficoLocalizacao.LongitudeRaio.val().replace(',', '.'));
        if (!isNaN(lat) && !isNaN(lng)) {
            var raio = parseInt(_raioGeograficoLocalizacao.Raio1.val());
            if (!isNaN(raio) && raio > 0) {
                drawRadius(lat, lng, raio * 1000);
            }
            raio = parseInt(_raioGeograficoLocalizacao.Raio2.val());
            if (!isNaN(raio) && raio > 0) {
                drawRadius(lat, lng, raio * 1000);
            }
            raio = parseInt(_raioGeograficoLocalizacao.Raio3.val());
            if (!isNaN(raio) && raio > 0) {
                drawRadius(lat, lng, raio * 1000);
            }
            _map.setCenter({ lat: lat, lng: lng });
        }
    }
    Global.fecharModal("divRaioGeograficoLocalizacao");
}

function drawRadius(lat, lng, radius) {
    var raio = new google.maps.Circle({
        strokeColor: "#FF0000",
        strokeWeight: 2,
        fillColor: '#FF0000',
        fillOpacity: 0.1,
        center: { lat: lat, lng: lng },
        radius: radius,
        editable: false,
        map: _map
    });

    if (_radiusDrawMaps == null)
        _radiusDrawMaps = [];
    _radiusDrawMaps.push(raio);
}

function zoomIn() {
    var zoom = false;
    var minlat = 180;
    var maxlat = -180
    var minlng = 180;
    var maxlng = -180

    if (_arrayMarker != null && _arrayMarker != undefined && _arrayMarker.length > 0) {
        zoom = true;
        $(_arrayMarker).each(function (i) {
            if (_arrayMarker[i].marker.position.lat() < minlat) minlat = _arrayMarker[i].marker.position.lat();
            if (_arrayMarker[i].marker.position.lat() > maxlat) maxlat = _arrayMarker[i].marker.position.lat();
            if (_arrayMarker[i].marker.position.lng() < minlng) minlng = _arrayMarker[i].marker.position.lng();
            if (_arrayMarker[i].marker.position.lng() > maxlng) maxlng = _arrayMarker[i].marker.position.lng();
        });
    }

    if (_MicroRegioes != null && _MicroRegioes != undefined) {
        $(_MicroRegioes).each(function (i) {
            if (_MicroRegioes[i].type == "polygon") {
                var pa = _MicroRegioes[i].getPath();
                if (pa != undefined) {
                    var path = pa.getArray();
                    $(path).each(function (j) {
                        if (path[j].lat() < minlat) minlat = path[j].lat();
                        if (path[j].lat() > maxlat) maxlat = path[j].lat();
                        if (path[j].lng() < minlng) minlng = path[j].lng();
                        if (path[j].lng() > maxlng) maxlng = path[j].lng();
                    });
                }
            } else if (_MicroRegioes[i].type == "rectangle") {
                if (_MicroRegioes[i].getBounds().getNorthEast().lat() < minlat) minlat = _MicroRegioes[i].getBounds().getNorthEast().lat();
                if (_MicroRegioes[i].getBounds().getNorthEast().lat() > maxlat) maxlat = _MicroRegioes[i].getBounds().getNorthEast().lat();
                if (_MicroRegioes[i].getBounds().getNorthEast().lng() < minlng) minlng = _MicroRegioes[i].getBounds().getNorthEast().lng();
                if (_MicroRegioes[i].getBounds().getNorthEast().lng() > maxlng) maxlng = _MicroRegioes[i].getBounds().getNorthEast().lng();

                if (_MicroRegioes[i].getBounds().getSouthWest().lat() < minlat) minlat = _MicroRegioes[i].getBounds().getSouthWest().lat();
                if (_MicroRegioes[i].getBounds().getSouthWest().lat() > maxlat) maxlat = _MicroRegioes[i].getBounds().getSouthWest().lat();
                if (_MicroRegioes[i].getBounds().getSouthWest().lng() < minlng) minlng = _MicroRegioes[i].getBounds().getSouthWest().lng();
                if (_MicroRegioes[i].getBounds().getSouthWest().lng() > maxlng) maxlng = _MicroRegioes[i].getBounds().getSouthWest().lng();
            }
        });
    }

    if (zoom) {
        var bounds = new google.maps.LatLngBounds();
        bounds.extend(new google.maps.LatLng(minlat, minlng));
        bounds.extend(new google.maps.LatLng(maxlat, maxlng));
        _map.fitBounds(bounds);
    }
}