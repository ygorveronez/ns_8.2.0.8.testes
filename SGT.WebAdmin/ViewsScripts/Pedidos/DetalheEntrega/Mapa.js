var MapaDetalheEntrega = function (idContainer) {
    this._idContainer = idContainer;
    this._mapa;
    this._dadosPolilinhasPlanejadas = [];
    this._dadosPolilinhasRealizadas = [];

    this._inicializar();
}

MapaDetalheEntrega.prototype = {
    carregarPolilinhasPlanejadas: function (polilinhas) {
        this._limparDadosPolilinhas(this._dadosPolilinhasPlanejadas);
        this._dadosPolilinhasPlanejadas = this._obterDadosPolilinhas(polilinhas);
    },
    carregarPolilinhasRealizadas: function (polilinhas) {
        this._limparDadosPolilinhas(this._dadosPolilinhasRealizadas);
        this._dadosPolilinhasRealizadas = this._obterDadosPolilinhas(polilinhas);
    },
    centralizarMapa: function () {
        var bounds = new google.maps.LatLngBounds();

        this._adicionarFronteirasPorPolilinhas(this._dadosPolilinhasPlanejadas, bounds);
        this._adicionarFronteirasPorPolilinhas(this._dadosPolilinhasRealizadas, bounds);

        this._mapa.fitBounds(bounds);
    },
    _adicionarFronteirasPorPolilinhas: function (dadosPolilinhas, bounds) {
        for (var i in dadosPolilinhas) {
            dadosPolilinhas[i].polilinha.getPath().forEach(function (item) {
                bounds.extend(new google.maps.LatLng(item.lat(), item.lng()));
            });
        }
    },
    _adicionarMarcador: function (ponto, icone) {
        return new google.maps.Marker({
            position: new google.maps.LatLng(ponto.lat(), ponto.lng()),
            info: null,
            icon: icone,
            type: google.maps.drawing.OverlayType.MARKER,
            editable: false,
            draggable: false,
            backgroundOnly: false,
            map: this._mapa,
            title: '',
            content: null
        });
    },
    _adicionarMarcadorFinal: function (path) {
        var icone = MapaObterIconSVGMarcador(MapaObterSVGPin('#d45b5b'));

        return this._adicionarMarcador(path[path.length - 1], icone);
    },
    _adicionarMarcadorInicial: function (path) {
        var icone = "http://maps.google.com/mapfiles/kml/pal2/icon13.png";

        return this._adicionarMarcador(path[0], icone);
    },
    _adicionarMarcadorPorCoordenadas: function (latitude, longitude, endereco, titulo, icone) {
        posicao = new google.maps.LatLng(latitude, longitude);

        const marker = new google.maps.Marker({
            position: posicao,
            map: this._mapa,
            title: titulo,
            icon: icone
        });
        
        if (!string.IsNullOrWhiteSpace(endereco)) {
            var conteudoInfoWindow =
                '<div>' +
                '<div id="siteNotice">' +
                "</div>" +
                '<h6 id="firstHeading" class="firstHeading">' + titulo + '</h6>' +
                '<div id="bodyContent">' +
                "<p><b>" + endereco + "</b></p>" +
                "</div>" +
                "</div>";

            const infoWindow = new google.maps.InfoWindow({
                content: conteudoInfoWindow
            });

            marker.addListener("click", function () {
                infoWindow.open({
                    anchor: marker,
                    map: this._mapa,
                    shouldFocus: false
                });
            });
        }

        marker.setMap(this._mapa);

        this._focarPosicao(latitude, longitude);
    },
    _adicionarPolilinha: function (path) {
        return new google.maps.Polyline({
            path: path,
            strokeColor: "#00BFFF",
            strokeOpacity: 1.0,
            strokeWeight: 2,
            map: this._mapa,
            zIndex: 0,
            visible: true
        });
    },
    _focarPosicao(latitude, longitude) {
        var position = {
            lat: latitude,
            lng: longitude
        };

        this._mapa.setZoom(4);
        this._mapa.setCenter(position);
    },
    _inicializar: function (idContainer) {
        var container = document.getElementById(this._idContainer);

        var opcoes = {
            zoom: 4,
            center: this._obterLocalizacaoInicial(),
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };

        this._mapa = new google.maps.Map(container, opcoes);
    },
    _limparDadosPolilinhas: function (dadosPolilinhas) {
        for (var i in dadosPolilinhas) {
            var dadosPolilinha = dadosPolilinhas[i];

            dadosPolilinha.polilinha.setMap(null);
            dadosPolilinha.marcadorFinal.setMap(null);
            dadosPolilinha.marcadorInicial.setMap(null);
        }
    },
    _obterDadosPolilinhas: function (polilinhas) {
        var dadosPolilinhas = [];

        for (var i in polilinhas) {
            var polilinha = polilinhas[i];
            var path = google.maps.geometry.encoding.decodePath(polilinha);

            dadosPolilinhas.push({
                polilinha: this._adicionarPolilinha(path),
                marcadorFinal: this._adicionarMarcadorFinal(path),
                marcadorInicial: this._adicionarMarcadorInicial(path)
            });
        }

        return dadosPolilinhas;
    },
    _obterLocalizacaoInicial: function () {
        return new google.maps.LatLng(-10.861639, -53.104038);
    }
};
