function MapaGoogleSearch(map, input) {
  var searchBox = new google.maps.places.SearchBox(input);
  map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);

  map.addListener('bounds_changed', function () {
    searchBox.setBounds(map.getBounds());
  });

    var markers = [];
    
    
    this.getMaker = function () {

        if ((markers) && (markers.length > 0)) {
            return markers[0];
        }

        return null;

    }

    searchBox.addListener('places_changed', function () {
        
        var places = searchBox.getPlaces();

        if (places.length == 0) {
            return;
        }

        markers.forEach(function (marker) {
            marker.setMap(null);
        });

        markers = [];

        var bounds = new google.maps.LatLngBounds();

        if (!places || places.length == 0)
            return;

        var place = places[0];


        if (!place.geometry) {
            return;
        }

        markers.push(new google.maps.Marker({
            map: map,
            //icon: icon,
            title: place.name,
            draggable: true,
            position: place.geometry.location
        }));

        if (place.geometry.viewport) {
            bounds.union(place.geometry.viewport);
        }
        else {
            bounds.extend(place.geometry.location);
        }


        map.fitBounds(bounds);
    });
};