var EnumMarker = {
    Pin: 1,
    PinSelecionado: 2,
    Distribuidor: 3,
    DistribuidorSelecionado: 4,
    PinRestricao: 5,
    Coleta: 6
};

function MapaOpcoes() {
    this.moverPrimeiroPonto = false;
    this.moverUltimoPonto = true;
    this.plotarPontoPassagem = false;
}

function PontosRota(descricao, lat, lng, pedagio, pontopassagem, distancia, tempo, codigo, sequencia, tipoponto, informacao) {
    this.descricao = descricao;
    this.lat = lat;
    this.lng = lng;
    this.pedagio = pedagio;
    this.pontopassagem = pontopassagem;
    this.tempo = tempo;
    this.distancia = distancia;
    this.codigo = codigo;
    this.sequencia = sequencia;
    this.tipoponto = tipoponto;
    this.informacao = informacao;
    this.tempoEstimadoPermanencia = 0;
    this.primeiraEntrega = false;
    this.codigoOutroEndereco = 0;

    if (!pedagio)
        this.pedagio = false;
    if (!pontopassagem)
        this.pontopassagem = false;
    if (distancia === undefined)
        this.distancia = 0;
    if (tempo === undefined)
        this.tempo = 0;
    if (codigo === undefined)
        this.codigo = 0;
    if (sequencia === undefined)
        this.sequencia = 0;
    if (informacao === undefined)
        this.informacao = "";
};

function Mapa(idelemento, mapaSatelite, mapInicializado, callbackOpcoesMapa, draw) {
    var RespostaRotaDetalhe = function () {
        this.distancia = 0;
        this.tempo = 0;
        this.polilinha = "";
        this.status = google.maps.DirectionsStatus.OK;
        this.pontos = [];
        this.pontosroteirizacao = "";
    };

    var RespostaRota = function () {
        this.distancia = 0;
        this.tempo = 0;
        this.codigo = 0;

        this.polilinha = "";
        this.status = google.maps.DirectionsStatus.OK;
        this.pontos = [];
        this.pontosroteirizacao = "";

        this.ida = new RespostaRotaDetalhe();
        this.volta = new RespostaRotaDetalhe();
    };

    var tipoultimoponto = null;
    var directionsDisplay = [];
    var listamarkers = [];
    var listapolilinha = [];
    var map = null;
    var pontosultimarota = null;
    var bounds = new google.maps.LatLngBounds();
    var self = this;
    var callbackAlteracaoRota = null;
    var callbackAlteracaoEntrega = null;
    var listapracas = [];

    if (!mapInicializado) {

        var inicializarMapa = function () {

            var latlng = new google.maps.LatLng(-15.7941, -47.8825);
            var maptype = google.maps.MapTypeId.ROADMAP;
            if (mapaSatelite)
                var maptype = google.maps.MapTypeId.SATELLITE;

            var opcoesmapa = {
                zoom: 5,
                center: latlng,
                mapTypeId: maptype,
                scaleControl: true,
                optimizeWaypoints: false,
                gestureHandling: 'greedy'
            };

            if (callbackOpcoesMapa instanceof Function) {
                callbackOpcoesMapa(opcoesmapa);
            }

            var elemento = document.getElementById(idelemento);
            map = new google.maps.Map(elemento, opcoesmapa);

        }();

        if (draw === true) {
            var divColorPalette;
            this.draw = new MapaDraw(map, divColorPalette);
        }
    }
    else
        map = mapInicializado;


    directionsService = new google.maps.DirectionsService();

    var limparPolilinha = function () {

        for (var i = 0; i < listapolilinha.length; i++)
            listapolilinha[i].setMap(null);

        listapolilinha = [];
    };

    this.getTotalPolilinhas = function () {
        return listapolilinha.length;
    };

    this.limparPolilinhas = function () {
        limparPolilinha();
    };

    this.ocultarPolilinha = function (indice) {
        if (typeof listapolilinha[indice] !== undefined)
            listapolilinha[indice].setVisible(false);
    };

    this.mostrarPolilinha = function (indice) {
        if (typeof listapolilinha[indice] !== undefined)
            listapolilinha[indice].setVisible(true);
    };

    this.togglePolilinha = function (indice) {
        if (typeof listapolilinha[indice] != 'undefined') {
            listapolilinha[indice].setVisible(!listapolilinha[indice].getVisible());
        }
    };

    this.ordenarRotaOpenStreetMap = function (pontos, tipoultimopontorota, callback, manterPontoDestino, codigoCarregamento) {
        tipoultimoponto = tipoultimopontorota;

        var data = { pontos: JSON.stringify(pontos), tipoUltimoPontoRoteirizacao: tipoultimoponto, manterPontoDestino: manterPontoDestino, codigoCarregamento: codigoCarregamento };

        return $.ajax({
            type: "POST",
            url: "RotaFrete/OrdenarRota",
            data: data,
            dataType: 'json',
            success: function (retorno) {
                if (retorno.Data !== false) {
                    /*
                    RotaOrdenada = rotaordenada,
                    RespostaOSM = new
                    {
                        Distancia = respostaRoteirizacao?.Distancia ?? 0,
                        Status = respostaRoteirizacao?.Status ?? "Erro",
                        Polilinha = respostaRoteirizacao?.Polilinha ?? "",
                        TempoMinutos = respostaRoteirizacao?.TempoMinutos ?? 0
                    }
                    */
                    var resposta = retorno.Data.RotaOrdenada;

                    var listaordenada = [];

                    for (var i = 0; i < resposta.length; i++) {
                        listaordenada.push({
                            descricao: resposta[i].Descricao,
                            pedagio: resposta[i].Pedagio,
                            fronteira: resposta[i].Fronteira,
                            lat: parseFloat(resposta[i].Lat),
                            lng: parseFloat(resposta[i].Lng),
                            codigo: resposta[i].Codigo,
                            codigoCliente: resposta[i].CodigoCliente,
                            usarOutroEndereco: resposta[i].UsarOutroEndereco,
                            sequencia: resposta[i].Sequencia,
                            tipoponto: resposta[i].TipoPonto,
                            informacao: resposta[i].Informacao,
                            localDeParqueamento: resposta[i].LocalDeParqueamento,
                            utilizaLocalidade: resposta[i].UtilizaLocalidade,
                            tempoEstimadoPermanencia: resposta[i].tempoEstimadoPermanencia,
                            distancia: resposta[i].Distancia,
                            codigoOutroEndereco: resposta[i].CodigoOutroEndereco
                        });
                    }

                    callback(listaordenada, "OK", retorno.Data.RespostaOSM);
                }
                else
                    callback(null, retorno.Msg);

            },
            error: function (retorno) {
                callback(null, "Falha na requisição.");
            }
        });

    }

    var desenharRota = function (rotaresposta) {

        var display = new google.maps.DirectionsRenderer(
            {
                suppressMarkers: true,
                draggable: true,
                map: map,
                preserveViewport: true
            });



        var string = JSON.stringify(rotaresposta);

        display.setDirections(rotaresposta);


        listenerAlterouRota(this, display);
        directionsDisplay.push(display);
    };

    var limparRoteirizacao = function () {
        if (directionsDisplay == null)
            return;

        for (var i = 0; i < directionsDisplay.length; i++) {
            directionsDisplay[i].setMap(null);
        }

        directionsDisplay = [];
    };

    var validarRetornoRota = function (listarespostas) {

        for (var i = 0; i < listarespostas.length; i++) {
            if (listarespostas[i].status != google.maps.DirectionsStatus.OK) {
                return listarespostas[i].status;
            }
        };

        return google.maps.DirectionsStatus.OK;
    }

    var obterRespostaRoteirizacao = function (listarespostas) {

        var resposta = new RespostaRota();

        var listawaypoint = [];
        var listawaypointIda = [];
        var listawaypointVolta = [];

        resposta.status = validarRetornoRota(listarespostas);

        respotaOK = resposta.status == google.maps.DirectionsStatus.OK;

        if (!respotaOK)
            return resposta;

        var idxultimarota = 0;
        var pontoultimarota = pontosultimarota[idxultimarota];
        
        var waypoint = {
            descricao: pontoultimarota.descricao,
            pedagio: pontoultimarota.pedagio,
            fronteira: pontoultimarota.fronteira,
            lat: parseFloat(pontoultimarota.lat),
            lng: parseFloat(pontoultimarota.lng),
            pontopassagem: pontoultimarota.pontopassagem,
            distanciaDireta: 0,
            distancia: 0,
            tempo: 0,
            codigo: pontoultimarota.codigo,
            sequencia: pontoultimarota.sequencia,
            tipoponto: pontoultimarota.tipoponto,
            localDeParqueamento: pontoultimarota.localDeParqueamento,
            utilizaLocalidade: pontoultimarota.utilizaLocalidade,
            informacao: pontoultimarota.informacao,
            codigoOutroEndereco: pontoultimarota.codigoOutroEndereco
        };

        listawaypoint.push(waypoint);
        listawaypointIda.push(waypoint);

        for (var i = 0; i < listarespostas.length; i++) {
            var routes = listarespostas[i].routes;
            if (routes && routes[0]) {
                var tempoEstimadoPermanencia = 0;
                for (var j = 0; j < routes[0].legs.length; j++) {
                    var leg = routes[0].legs[j];

                    idxultimarota++;
                    var pontoultimarota = pontosultimarota[idxultimarota];

                    var ehvolta = (i == listarespostas.length - 1) && (j >= routes[0].legs.length - 2) && (tipoultimoponto == 2 || tipoultimoponto == 1);


                    for (var s = 0; s < leg.via_waypoints.length; s++) {
                        waypoint = {
                            descricao: "ponto de passagem", pedagio: false, lat: parseFloat(leg.via_waypoints[s].lat()), lng: parseFloat(leg.via_waypoints[s].lng()), pontopassagem: true,
                            distancia: 0, tempo: 0, codigo: 0, sequencia: 0, tipoponto: EnumTipoPontoPassagem.Passagem, informacao: ""
                        };
                        listawaypoint.push(waypoint);

                        if (ehvolta)
                            listawaypointVolta.push(waypoint);
                        else
                            listawaypointIda.push(waypoint);
                    }

                    var waypoint = {
                        descricao: pontoultimarota.descricao,
                        pedagio: pontoultimarota.pedagio,
                        lat: parseFloat(pontoultimarota.lat),
                        lng: parseFloat(pontoultimarota.lng),
                        fronteira: pontoultimarota.fronteira,
                        pontopassagem: pontoultimarota.pontopassagem,
                        distancia: leg.distance.value,
                        distanciaDireta: pontoultimarota.distanciaDireta,
                        tempo: leg.duration.value + tempoEstimadoPermanencia,
                        tempoEstimadoPermanencia: pontoultimarota.tempoEstimadoPermanencia,
                        codigo: pontoultimarota.codigo,
                        sequencia: pontoultimarota.sequencia,
                        tipoponto: pontoultimarota.tipoponto,
                        localDeParqueamento: pontoultimarota.localDeParqueamento,
                        utilizaLocalidade: pontoultimarota.utilizaLocalidade,
                        informacao: pontoultimarota.informacao,
                        codigoOutroEndereco: pontoultimarota.codigoOutroEndereco
                    };

                    listawaypoint.push(waypoint);

                    if (ehvolta) {
                        resposta.volta.distancia = resposta.volta.distancia + leg.distance.value;
                        resposta.volta.tempo = resposta.volta.tempo + leg.duration.value;
                        listawaypointVolta.push(waypoint);

                    }
                    else {
                        resposta.ida.distancia = resposta.ida.distancia + leg.distance.value;
                        resposta.ida.tempo = resposta.ida.tempo + leg.duration.value;
                        listawaypointIda.push(waypoint);
                    }

                    resposta.distancia = resposta.distancia + leg.distance.value;
                    resposta.tempo = resposta.tempo + leg.duration.value + tempoEstimadoPermanencia;
                    tempoEstimadoPermanencia += pontoultimarota.tempoEstimadoPermanencia;


                    for (w = 0; w < leg.steps.length; w++) {
                        var steps = leg.steps[w];
                        var polilinepoint = steps.polyline.points;
                        var points = google.maps.geometry.encoding.decodePath(polilinepoint);

                        for (r = 0; r < points.length; r++) {
                            resposta.pontos.push(points[r]);

                            if (ehvolta)
                                resposta.volta.pontos.push(points[r]);
                            else
                                resposta.ida.pontos.push(points[r]);
                        }
                    }
                }
            }
        }

        resposta.pontosroteirizacao = JSON.stringify(listawaypoint);
        resposta.polilinha = google.maps.geometry.encoding.encodePath(resposta.pontos);
        resposta.ida.pontosroteirizacao = JSON.stringify(listawaypointIda);
        resposta.ida.polilinha = google.maps.geometry.encoding.encodePath(resposta.ida.pontos);
        resposta.volta.polilinha = google.maps.geometry.encoding.encodePath(resposta.volta.pontos);
        resposta.volta.pontosroteirizacao = JSON.stringify(listawaypointVolta);

        resposta.status = statusRota(resposta.status);

        return resposta
    };

    var RespostaId = function (id, resposta) {
        this.id = id;
        this.resposta = resposta;
    };

    var roteirizar = function (lista, callback) {
        if (typeof (lista) == "string")
            lista = self_converterListaToArray(lista);

        self.limparMapa();
        pontosultimarota = lista.slice();
        var novalista = separarLista(lista);

        if (novalista.length == 0) {
            var resposta = new RespostaRota();
            resposta.status = 'Rota sem pontos';
            callback(resposta);
            return;
        }

        limparRoteirizacao();

        var resposta = function () {
            resposta.resposta = null;
            resposta.status = null;
        }

        var totalrespoestas = novalista.length;
        var respostaatual = 0;

        var listarespostas = [];
        var listarespostasid = []

        for (i = 0; i < novalista.length; i++) {

            roteirizarPontos(novalista[i], 0, i, function (resposta, id) {

                respostaatual++;

                listarespostasid.push(new RespostaId(id, resposta));

                if (respostaatual == totalrespoestas) {

                    listarespostasid.sort(function (a, b) { return a.id - b.id });

                    for (var j = 0; j < listarespostasid.length; j++)
                        listarespostas.push(listarespostasid[j].resposta);


                    var respostaroteiriacao = obterRespostaRoteirizacao(listarespostas);


                    if (respostaroteiriacao.status == google.maps.DirectionsStatus.OK) {
                        for (i = 0; i < novalista.length; i++)
                            desenharRota(listarespostas[i]);
                    }

                    if (respostaroteiriacao.status == google.maps.GeocoderStatus.OK) {
                        bounds = new google.maps.LatLngBounds();
                        self.adicionarMarcador(pontosultimarota, false, true)
                        self.centralizarBounds();
                    }

                    callback(respostaroteiriacao);
                }

            });
        }
    };

    var roteirizarPontos = function (lista, tempoquerylimit, id, callback) {
        var request = {
            origin: null,
            destination: null,
            travelMode: google.maps.DirectionsTravelMode.DRIVING,
            waypoints: [],
            unitSystem: google.maps.DirectionsUnitSystem.METRIC
        };

        var lat = lista[0].lat;
        var log = lista[0].lng;
        request.origin = new google.maps.LatLng(lat, log);

        var latDest = lista[lista.length - 1].lat.toString();
        var logDest = lista[lista.length - 1].lng.toString();

        request.destination = new google.maps.LatLng(latDest.replace(",", "."), logDest.replace(",", "."));

        for (var i = 1; i < lista.length - 1; i++) {
            request.waypoints.push({
                location: new google.maps.LatLng(lista[i].lat, lista[i].lng),
                stopover: true
            });
        }


        directionsService.route(request, function (resposta, status) {

            switch (status) {
                case google.maps.DirectionsStatus.OVER_QUERY_LIMIT:
                    if ((typeof tempoquerylimit === "undefined") || (tempoquerylimit == 0))
                        tempoquerylimit = 100;

                    tempoquerylimit = tempoquerylimit + 100;

                    if (tempoquerylimit > 5000) {
                        callback(resposta, id);
                        return;
                    }

                    setTimeout(function () {
                        roteirizarPontos(lista, tempoquerylimit, function (resposta, status) {
                            callback(resposta, id);
                        })
                    }, tempoquerylimit);

                    break;
                default:
                    callback(resposta, id);
            }

        });
    };

    var listenerExibirInfo = function (marker, eventShowInfoWindowEvent) {

        if (eventShowInfoWindowEvent == "click") {
            marker.addListener('click', function (event) {
                self.closeInfoWindowOpened();
                self.openInfoWindow(infowindow, this);
            });
        } else {
            marker.addListener('mouseover', function () {
                if (this.info.content = ! "") {
                    self.closeInfoWindowOpened();
                    this.info.open(map, this);
                    infoWindowOpened = this.info;
                }
            });
            marker.addListener('mouseout', function () {
                this.info.close();
                infoWindowOpened = null;
            });
        }
    }

    this.closeInfoWindowOpened = function () {
        if (infoWindowOpened) {
            infoWindowOpened.close();
            infoWindowOpened = null;
        }
    }

    var listenerAlterouRota = function (owner, directions) {

        directions.addListener('directions_changed', function () {

            var listarotas = [];

            for (var i = 0; i < directionsDisplay.length; i++) {

                var route = directionsDisplay[i].getDirections();

                listarotas.push(route);

            }

            var respostaroteiriacao = obterRespostaRoteirizacao(listarotas);

            if (callbackAlteracaoRota)
                callbackAlteracaoRota(respostaroteiriacao);

        });
    };

    var listenerAlterouOrdemPontos = function (marker) {
        marker.addListener('dragend', function () {
            var lat = this.getPosition().lat();
            var lng = this.getPosition().lng();

            var idNovo = obterPontoMaisProximo(lat, lng, pontosultimarota);
            var idAntigo = this.id;

            if ((idNovo != idAntigo) && (idNovo >= 0)) {
                iniciarRequisicao();

                limparMarkers();

                var lista = pontosultimarota.slice();

                var pontoorigem = pontosultimarota[idAntigo];
                var pontodestino = pontosultimarota[idNovo];

                lista[idAntigo] = pontodestino;
                lista[idNovo] = pontoorigem;

                roteirizar(lista, function (respostaroteiriacao) {
                    self.adicionarMarcador(pontosultimarota, false, true);

                    if (callbackAlteracaoRota)
                        callbackAlteracaoRota(respostaroteiriacao);

                    if (callbackAlteracaoEntrega) {
                        callbackAlteracaoEntrega(respostaroteiriacao, pontoorigem, pontodestino);
                    }

                    //Centralizar
                    bounds = new google.maps.LatLngBounds();

                    var latlngdestino = new google.maps.LatLng(pontodestino.lat, pontodestino.lng);
                    bounds.extend(latlngdestino);

                    var latlngorigem = new google.maps.LatLng(pontoorigem.lat, pontoorigem.lng);
                    bounds.extend(latlngorigem);

                    self.centralizarBounds();

                    finalizarRequisicao();
                });


            }
            else {
                var latlng = new google.maps.LatLng(pontosultimarota[this.id].lat, pontosultimarota[this.id].lng);
                this.setPosition(latlng);
            }

        });

    };

    this.centralizarBounds = function () {
        map.fitBounds(bounds);
        map.panToBounds(bounds);
    };

    var alterarIcone = function (marker, tipo) {
        var pinColor = self.corTipoMarcador(tipo);
        var svg = self.obterSVGPin(pinColor, marker.id);
        var icon = self.obterIconSVGMarcador(svg);
        marker.setIcon(icon);
    }

    this.corTipoMarcador = function (tipoMarker) {
        var cor = "";

        if (tipoMarker == EnumMarker.Pin)
            cor = '#d45b5b';
        else if (tipoMarker == EnumMarker.Distribuidor)
            cor = '#2386dc';
        if (tipoMarker == EnumMarker.PinRestricao)
            cor = '#bfb104';
        else if (tipoMarker == EnumMarker.DistribuidorSelecionado || tipoMarker == EnumMarker.PinSelecionado)
            cor = '#2dab10';
        else if (tipoMarker == EnumMarker.Coleta)
            cor = '#766ec5';


        return cor;
    }

    var statusRota = function (status) {
        switch (status) {
            case google.maps.DirectionsStatus.INVALID_REQUEST:
                return 'Requisição inválida'
            case google.maps.DirectionsStatus.MAX_WAYPOINTS_EXCEEDED:
                return 'Exedeu o numero de pontos da rota'
            case google.maps.DirectionsStatus.NOT_FOUND:
                return 'Pontos da rota inválidos'
            case google.maps.DirectionsStatus.OVER_QUERY_LIMIT:
                return 'Limite diário atingido'
            case google.maps.DirectionsStatus.REQUEST_DENIED:
                return 'Permissão de acesso negada'
            case google.maps.DirectionsStatus.UNKNOWN_ERROR:
                return 'Erro ao carregar mapa. Tente novamente.'
            case google.maps.DirectionsStatus.ZERO_RESULTS:
                return 'Não foi possível encontrar uma rota entre origem e o destino';
            case google.maps.GeocoderStatus.OK:
                return google.maps.GeocoderStatus.OK;
            case "MAX_ROUTE_LENGTH_EXCEEDED":
                return 'Máximo de pontos excedido';
            default:
                return status;

        }
    }

    var obterPontoMaisProximo = function (lat, lng, listapontos) {
        var menordist = 999999999;
        var menoridx = -1;

        for (var i = 1; i < listapontos.length; i++) {

            var ehpedagio = listapontos[i].pedagio;
            var ehpontopassagem = listapontos[i].pontopassagem;

            if ((!ehpedagio) && (!ehpontopassagem)) {

                var latlista = listapontos[i].lat;
                var lgnlista = listapontos[i].lng;

                var dist = calculaDistancia(lat, lng, latlista, lgnlista);

                if ((dist < menordist) && (dist < 50)) {
                    menordist = dist
                    menoridx = i;
                }
            }
        }
        return menoridx;
    };

    var calculaDistancia = function (lat1, lon1, lat2, lon2) {

        var deg2rad = function (deg) {
            var rad = deg * Math.PI / 180;
            return rad;
        };

        var round = function (x) {
            return Math.round(x * 10) / 10;
        };

        var RADIUSMILES = 3961,
            RADIUSKILOMETERS = 6373,
            latR1 = deg2rad(lat1),
            lonR1 = deg2rad(lon1),
            latR2 = deg2rad(lat2),
            lonR2 = deg2rad(lon2),
            latDifference = latR2 - latR1,
            lonDifference = lonR2 - lonR1,
            a = Math.pow(Math.sin(latDifference / 2), 2) + Math.cos(latR1) * Math.cos(latR2) * Math.pow(Math.sin(lonDifference / 2), 2),
            c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a)),
            dm = c * RADIUSMILES,
            dk = c * RADIUSKILOMETERS;

        return Math.round(dk * 10) / 10;
    };

    this.obterSVGPin = function (color, label) {
        return MapaObterSVGPin(color, label);
    }

    var obterSVGPraca = function (color) {
        return MapaObterSVGPraca(color);
    }

    var obterSVGFronteira = function (color) {
        return MapaObterSVGFronteira(color);
    }

    this.obterIconSVGMarcador = function (svg) {
        return MapaObterIconSVGMarcador(svg);

    }

    var converterListaToArray = function (lista) {
        var arraypontos = lista.split(";");
        var pontos = []
        for (var i = 0; i < arraypontos.length; i++) {

            p = arraypontos[i].split(",");

            pontos.push([p[0], p[1]]);
        }

        return pontos;
    };

    var separarLista = function (lista) {
        var tamanhomaximo = 23;


        var arrlista = [];

        var ultimo = tamanhomaximo;

        while (lista.length > 1) {

            arrlista.push(lista.slice(0, ultimo));
            lista.splice(0, ultimo - 1);

        }
        return arrlista;
    };

    //Metodos publicos

    this.limparMarcadorPracas = function () {

        for (var i = 0; i < listamarkers.length; i++) {
            if (listamarkers[i].pedagio)
                listamarkers[i].setMap(null);
        };

    }

    this.centralizar = function (lat, lng) {
        setTimeout(function () {
            map.panTo(new google.maps.LatLng(lat, lng));
        }, 200);

    };

    this.setZoom = function (zoom) {
        if (!zoom)
            zoom = 5;

        map.setZoom(zoom);
    };

    map.setZoom(12);

    this.opcoes = new MapaOpcoes();

    this.desenharPolilinha = function (polilinha, limpar, cor, visivel) {
        if (polilinha == "" || polilinha == null)
            return

        if (limpar)
            limparPolilinha();

        if (visivel == undefined)
            visivel = true;

        var path = google.maps.geometry.encoding.decodePath(polilinha);

        var corLinha = "#00BFFF";
        if ((cor != undefined) && (cor != null))
            corLinha = cor;

        var polilinha = new google.maps.Polyline({
            path: path,
            strokeColor: corLinha,
            strokeOpacity: 1.0,
            strokeWeight: 4,
            map: map,
            zIndex: listapolilinha.length,
            visible: visivel
        });

        listapolilinha.push(polilinha);

    };

    this.limparMapa = function () {
        limparRoteirizacao();
        limparMarkers();
        limparPolilinha();
        bounds = new google.maps.LatLngBounds();
    };

    var limparMarkers = function () {
        if (listamarkers == null)
            return;

        for (var i = 0; i < listamarkers.length; i++) {
            listamarkers[i].setMap(null);
        };

        listamarkers = [];
    };

    this.adicionarMarcador = function (lista, centralizar, movermarcador) {
        if (!lista)
            return;

        seqentrega = 1;

        for (var i = 0; i < lista.length; i++) {

            var pontopassagem = false;
            var localParqueamento = lista[i].localDeParqueamento;

            if (lista[i].pontopassagem)
                var pontopassagem = lista[i].pontopassagem;

            if (lista[i].tipoponto == EnumTipoPontoPassagem.Passagem)
                var pontopassagem = true;


            if (pontopassagem) {
                var latlngpassagem = new google.maps.LatLng(lista[i].lat, lista[i].lng);
                bounds.extend(latlngpassagem);
            }

            var plotarPontoPassagem = this.opcoes.plotarPontoPassagem;

            if (lista[i].localDeParqueamento) {
                var latLng = new google.maps.LatLng(lista[i].lat, lista[i].lng);

                var pam = new google.maps.Marker({
                    position: latLng,
                    icon: "../img/montagem-carga-mapa/markers/PontoPassagemPlaca.png",
                    editable: false,
                    title: lista[i].descricao,
                    map: map
                });

                listamarkers.push(pam);
            }


            if (pontopassagem && plotarPontoPassagem) {

                var latLng = new google.maps.LatLng(lista[i].lat, lista[i].lng);

                var pam = new google.maps.Marker({
                    position: latLng,
                    icon: "../img/montagem-carga-mapa/markers/ponto-apoio.png",
                    editable: false,
                    title: lista[i].descricao,
                    map: map
                });

                listamarkers.push(pam);

            } else if (!pontopassagem) {

                var latlng = new google.maps.LatLng(lista[i].lat, lista[i].lng);

                var icon = "";

                var ehpedagio = lista[i].pedagio;
                var ehFronteira = lista[i].fronteira;
                var primeiroitem = ((i == 0) && (!ehpedagio && !ehFronteira));
                var ultimoitem = i == (lista.length - 1);
                var retornoorigem = (tipoultimoponto == 2 || tipoultimoponto == 1) && ultimoitem && !ehpedagio && !ehFronteira;
                var ehEntrega = false;


                if (primeiroitem)
                    icon = "http://maps.google.com/mapfiles/kml/pal2/icon13.png";
                else {

                    if (ehpedagio) {
                        var svgprava = obterSVGPraca('#06609E');
                        icon = self.obterIconSVGMarcador(svgprava);
                    }
                    else if (ehFronteira) {
                        var svgprava = obterSVGFronteira('#000000');
                        icon = self.obterIconSVGMarcador(svgprava);
                    }
                    else {
                        var tipoMarcador = EnumMarker.Pin;
                        if (lista[i].tipoponto == EnumTipoPontoPassagem.Coleta)
                            tipoMarcador = EnumMarker.Coleta;

                        var pinColor = self.corTipoMarcador(tipoMarcador);

                        var svg = self.obterSVGPin(pinColor, seqentrega);
                        icon = self.obterIconSVGMarcador(svg);

                        seqentrega++;
                    }
                }

                var dragable = !(primeiroitem) && !(retornoorigem) && !(ehpedagio) && !ehFronteira && movermarcador;

                if (primeiroitem)
                    dragable = this.opcoes.moverPrimeiroPonto;

                if (ultimoitem)
                    dragable = this.opcoes.moverUltimoPonto;

                if (!retornoorigem) {
                    var marker = new google.maps.Marker({
                        draggable: dragable,
                        position: latlng,
                        icon: icon,
                        map: map,
                    });

                    var titulo = lista[i].descricao;
                    if (lista[i].codigo !== null && lista[i].codigo !== undefined && lista[i].codigo !== 0) {
                        titulo = titulo + ' ' + lista[i].codigo;
                        if ((lista[i].informacao) && (lista[i].informacao !== ""))
                            titulo = lista[i].informacao;
                    }

                    var infowindow = new google.maps.InfoWindow({
                        content: titulo
                    });

                    marker.set("id", i);
                    marker.set("info", infowindow);
                    marker.set("pedagio", ehpedagio);
                    marker.set("fronteira", ehFronteira);

                    listenerExibirInfo(marker);

                    if (dragable)
                        listenerAlterouOrdemPontos(marker);

                    bounds.extend(marker.position);

                    listamarkers.push(marker);
                }
            }

        }

        if (centralizar) {
            self.centralizarBounds();
        }
    }

    this.adicionarMarcadorComPontosDaRota = function (pontosdarota, centralizar, movermarcador) {
        if ((pontosdarota) && (pontosdarota != "")) {
            var pontosrota = JSON.parse(pontosdarota);
            self.adicionarMarcadorComPontosDaRotaNoMapa(pontosrota, centralizar, movermarcador);
        } else {
            var resposta = new RespostaRota()
            resposta.status = 'Rota sem pontos';
        }
    }

    this.adicionarMarcadorComPontosDaRotaNoMapa = function (pontosrota, centralizar, movermarcador) {

        movermarcador = (movermarcador ? movermarcador : false);

        bounds = new google.maps.LatLngBounds();
        self.adicionarMarcador(pontosrota, false, movermarcador);

        if (movermarcador)
            pontosultimarota = pontosrota;

        if (centralizar == null || centralizar == undefined)
            centralizar = true;
        if (centralizar)
            self.centralizarBounds(bounds);
    }

    this.adicionarMarcadorComPontosDaRotaSemDestinos = function (pontosdarota, centralizar) {
        if ((pontosdarota) && (pontosdarota != "")) {
            var lista = JSON.parse(pontosdarota);
            var pontosrota = [];
            for (var i = 0; i < lista.length; i++) {
                if (lista[i].pontopassagem || lista[i].pedagio || lista[i].fronteira) {
                    pontosrota.push(lista[i]);
                }
            }
            self.adicionarMarcadorComPontosDaRotaNoMapa(pontosrota, centralizar);
        } else {
            var resposta = new RespostaRota()
            resposta.status = 'Rota sem pontos';
        }
    }

    this.roteirizarComPontosDaRota = function (pontosdarota, callback) {

        if (pontosdarota) {
            var pontos = JSON.parse(pontosdarota);

            roteirizar(pontos, function (resposta) {
                callback(resposta);

            });
        }
        else {
            var resposta = new RespostaRota()
            resposta.status = 'Rota sem pontos';
            callback(resposta)
        }


    }

    this.adicionarPracasPedagio = function (pracas) {

        self.limparMarcadorPracas();

        listapracas = pracas;
        self.adicionarMarcador(pracas, false, false);
    }

    this.setarCallbackAlteracaoRota = function (callback) {
        callbackAlteracaoRota = callback;
    };

    this.setarCallbackAlteracaoEntrega = function (callback) {
        callbackAlteracaoEntrega = callback;
    };

    this.roteirizarSemOrdem = function (lista, tipoultimopontorota, callback) {
        tipoultimoponto = tipoultimopontorota;
        // 1 = Retorno Vazio, 2 = Até a Origem, 3 = Ponto Mais Distante
        if (tipoultimoponto === 2 || tipoultimoponto === 1) {
            var pontoOrigem = lista[0];
            //Vamos chegar se o último item da lista de pontos não é a origem.. se for.. não precisamos adicionar novamente.
            var ultimoPonto = lista[lista.length - 1];
            if (pontoOrigem.codigo != ultimoPonto.codigo)
                lista.push(pontoOrigem);
        }

        roteirizar(lista, function (respostaroteirizar) {
            callback(respostaroteirizar);
        });
    };

    this.roteirizar = function (lista, tipoultimopontorota, callback) {
        tipoultimoponto = tipoultimopontorota;

        this.ordenarRotaOpenStreetMap(lista, tipoultimoponto, function (respostaOrdenada, status, respostaOSM) {
            if (status == "OK") {
                //console.log(respostaOSM);
                //var osrmValida = false;
                //if (respostaOSM != null && respostaOSM != undefined) {
                //    if (respostaOSM.Distancia > 0 && respostaOSM.Polilinha != '') {
                //        osrmValida = true;
                //    }
                //}
                //if (osrmValida) {

                //} else {
                roteirizar(respostaOrdenada, function (respostaroteirizar) { callback(respostaroteirizar) });
                //}
            }
            else {
                var resposta = new RespostaRota();

                resposta.status = status;

                callback(resposta);
            }

        });
    };

    this.getMap = function () {
        return map;
    }

}

function MapaObterIconSVGMarcador(svg) {
    return 'data:image/svg+xml,' + encodeURIComponent(svg);
}

function MapaObterSVGPin(color, label) {
    var sgv = '';

    sgv += '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="30" viewBox="0 0 1026 1539">';
    sgv += '  <g>';
    sgv += '    <path stroke="0" fill="' + color + '" d="m1024,512q0,109 -33,179l-364,774q-16,33 -47.5,52t-67.5,19t-67.5,-19t-46.5,-52l-365,-774q-33,-70 -33,-179q0,-212 150,-362t362,-150t362,150t150,362z" id="svg_1"/>';

    if (label)
        sgv += '<text stroke="#000" fill="#FFFFFF" stroke-width="0" x="636" y="560" id="svg_2" font-size="220" font-family="Arial, sans-serif" text-anchor="middle" xml:space="preserve" transform="matrix(2.812777609818733,0,0,2.8326989413850754,-1278.5555187961586,-781.1031876516241) " font-weight="bold">' + label + '</text>';

    sgv += '  </g>';
    sgv += '</svg>';

    return sgv;
}

function MapaObterSVGPraca(color) {
    return (
        '<svg xmlns="http://www.w3.org/2000/svg" x="0px" y="0px" width="20" height="30" viewBox="0 0 192 192" style=" fill:#000000;">' +
        '<g fill="none" fill-rule="nonzero" stroke="none" stroke-width="1" stroke-linecap="butt" stroke-linejoin="miter" stroke-miterlimit="10" stroke-dasharray="" stroke-dashoffset="0" font-family="none" font-weight="none" font-size="none" text-anchor="none" style="mix-blend-mode: normal">' +
        '<path d="M0,192v-192h192v192z" fill="none"></path>' +
        '<g fill="' + color + '">' +
        '<g id="surface1">' +
        '<path d="M49.92,0c-0.855,0 -1.695,0.285 -2.4,0.84l-37.32,29.88h79.44l-37.32,-29.88c-0.705,-0.555 -1.545,-0.84 -2.4,-0.84zM7.68,38.4v122.88h84.48v-122.88zM179.04,46.08c-0.48,0.09 -1.005,0.315 -1.44,0.6l-77.76,50.04v9.12l7.68,-4.92l-2.4,10.8l-5.28,3.36v9.12l57.36,-36.84l33,-21.6c0.855,-0.555 1.47,-1.395 1.68,-2.4c0.21,-1.005 0.09,-2.025 -0.48,-2.88l-8.52,-12.72c-0.87,-1.305 -2.385,-1.935 -3.84,-1.68zM30.72,53.76h38.4v34.56h-38.4zM178.56,55.2l4.2,6.24l-9.6,6.36l2.28,-10.56zM165.6,63.6l-2.16,10.44l-9.84,6.48l2.28,-10.68zM146.28,75.96l-2.28,10.8l-9.72,6.12l2.4,-10.68zM127.08,88.32l-2.28,10.68l-10.08,6.48l2.4,-10.8zM3.84,168.96c-2.115,0 -3.84,1.71 -3.84,3.84v15.36c0,2.13 1.725,3.84 3.84,3.84h92.16c2.115,0 3.84,-1.71 3.84,-3.84v-15.36c0,-2.13 -1.725,-3.84 -3.84,-3.84z"></path>' +
        '</g>' +
        '</g>' +
        '</g>' +
        '</svg>'
    );
}

function MapaObterSVGFronteira(color) {
    return (
        '<svg height="30px" width="30px"  ' +
        '    fill="' + color + '" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" x="0px" y="0px" viewBox="0 0 100 100" enable-background="new 0 0 100 100" xml:space="preserve">' +
        '    <g>' +
        '        <path d="M11.244,67.535l0.622,5.547c24.583,10.702,52.489,10.702,77.052,0l0.62-5.547C64.622,56.47,36.172,56.47,11.244,67.535z"></path>' +
        '    </g>' +
        '    <g>' +
        '        <path d="M97.062,48.758l-8.884,13.446c-24.146-10.275-51.425-10.275-75.57,0L3.722,48.758c-0.128-0.177-0.237-0.374-0.305-0.581   ' +
        '        c-0.702-1.728,0.128-3.683,1.846-4.383c11.293-4.581,21.628-11.244,30.436-19.667c8.222-7.856,21.164-7.856,29.388,0   ' +
        '        c8.817,8.423,19.143,15.086,30.436,19.667c0.217,0.077,0.414,0.188,0.603,0.314C97.674,45.136,98.089,47.219,97.062,48.758z">' +
        '        </path>' +
        '    </g>' +
        '' +
        '</svg>'
    );
}