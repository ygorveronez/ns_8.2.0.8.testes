/// <reference path="./Mapa.js"/>
/// <reference path="./MapaDraw.js"/>
/// <reference path="./MapaGoogleSearch.js"/>


function OpcoesMapa(exibirDesenho, exibirBusca) {
    this.exibirDesenho = exibirDesenho;
    this.exibirBusca = exibirBusca;

    if (this.exibirDesenho == undefined)
        this.exibirDesenho = true;

    if (this.exibirBusca == undefined)
        this.exibirBusca = true;

}


function MapaGoogle(element, mapaSatelite, opcoesmapa, callbackOpcoesMapa) {

    if (!opcoesmapa) {
        opcoesmapa = new OpcoesMapa(true, true, true, true, true);
    }
    var map;
    var divMapa = document.getElementById(element);

    var divColorPalette;
    var searchInput;

    if (opcoesmapa.exibirBusca) {

        var createSearchInput = function () {
            searchInput = document.createElement('input');
            searchInput.id = "_searchInput_";
            divMapa.appendChild(searchInput);
        }();


        var setStyleSearch = function () {
            searchInput.style.background = "#fff";
            searchInput.style.margin = "6px";
            searchInput.style.padding = "0 11px 0 13px";
            searchInput.style.width = "380px";
            searchInput.style.height = "18px"
            searchInput.style.font.fontsize = "15px";
        }();

    }


    var initializeMap = function () {

        var latlng = new google.maps.LatLng(-15.7941, -47.8825);
        var maptype = google.maps.MapTypeId.ROADMAP;
        if (mapaSatelite)
            maptype = google.maps.MapTypeId.SATELLITE;

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

        map = new google.maps.Map(divMapa, opcoesmapa);

    }();


    this.draw = new MapaDraw(map, divColorPalette);
    if (opcoesmapa.exibirDesenho)
        this.draw.ShowDrawPalette(divMapa);


    this.direction = new Mapa(null, null, map, callbackOpcoesMapa);

    this.geo = new MapaGoogleGeoLocalizacao();

    this.clear = function () {
        this.draw.clear();
        this.direction.limparMapa();
    };

    if (opcoesmapa.exibirBusca) {

        this.search = new MapaGoogleSearch(map, searchInput);

    };

    this.panTo = function (lat, lng) {
        map.panTo(new google.maps.LatLng(lat, lng));
    }

    this.getZoom = function () {
        return map.getZoom();
    }

}