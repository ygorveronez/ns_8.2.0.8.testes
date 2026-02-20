$(document).ready(function () {
    var _map;
    var _mapMarker;
    var _geocoder;

    if (typeof localidade != "undefined") {

        CarregarMapaRastreamento();

        if (localidade.ExibirMapa) {
            var markerPosition = {
                lat: localidade.Latitude,
                lng: localidade.Longitude,
            };

            SetarEntregaRota(localidade.PolilinhaPlanejada, "#00BFFF", 2);
            SetarEntregaRota(localidade.PolilinhaRealizada, "#FF6961", 4);
            SetarEntregas(localidade.Entregas);
            SetarEntregaVeiculo(markerPosition);
        }
    }

    function CarregarMapaRastreamento() {
        _map = new google.maps.Map(document.getElementById("map-position"));
        _geocoder = new google.maps.Geocoder;

        _mapMarker = new google.maps.Marker({
            map: _map,
            icon: "../img/van.png"
        });
        window.markerPosition = _mapMarker;
    }

    function SetarEntregaVeiculo(markerPosition, positionVeiculo) {
        if (_map != null) {
            _mapMarker.setPosition(markerPosition);
            _mapMarker.setMap(_map);
            _geocoder.geocode({ 'location': positionVeiculo }, function (results, status) {
                if (status === 'OK' && results.length >= 0) {
                    $("#map-address").html(results[0].formatted_address);
                }
            });
            _map.setCenter(_mapMarker.getPosition());
            _map.setZoom(14);
        }
    }

    function SetarEntregaRota(polyline, color, weight) {
        if (_map != null && polyline != "") {
            var path = google.maps.geometry.encoding.decodePath(polyline);
            new google.maps.Polyline({
                path: path,
                strokeColor: color,
                strokeOpacity: 1.0,
                strokeWeight: weight,
                map: _map,
                zIndex: 0,
                visible: true
            });
        }
    }

    function SetarEntregas(data) {
        var total = data.length;
        if (_map != null && total) {
            var bounds = new google.maps.LatLngBounds();
            for (var i = 0; i < total; i++) {
                var latlng = new google.maps.LatLng(data[i].Latitude, data[i].Longitude);
                //bounds.extend(latlng);

                var icon;
                if (data[i].Ordem == 0) {
                    icon = "http://maps.google.com/mapfiles/kml/pal2/icon13.png"
                } else {
                    icon = 'data:image/svg+xml,' + encodeURIComponent('<svg xmlns="http://www.w3.org/2000/svg" width="20" height="30" viewBox="0 0 1026 1539"><g><path stroke="0" fill="#d45b5b" d="m1024,512q0,109 -33,179l-364,774q-16,33 -47.5,52t-67.5,19t-67.5,-19t-46.5,-52l-365,-774q-33,-70 -33,-179q0,-212 150,-362t362,-150t362,150t150,362z" id="svg_1"/><text stroke="#000" fill="#FFFFFF" stroke-width="0" x="636" y="560" id="svg_2" font-size="220" font-family="Arial, sans-serif" text-anchor="middle" xml:space="preserve" transform="matrix(2.812777609818733,0,0,2.8326989413850754,-1278.5555187961586,-781.1031876516241)" font-weight="bold">' + data[i].Ordem + '</text> </g></svg>');
                }

                new google.maps.Marker({
                    type: google.maps.drawing.OverlayType.MARKER,
                    map: _map,
                    position: latlng,
                    icon: icon,
                    title: data[i].Descricao
                });
            }
        }
    }
}); 