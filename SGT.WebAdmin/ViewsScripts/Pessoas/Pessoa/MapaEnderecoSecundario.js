var _mapDrawSecundario = null;
var markerSecundario;


function setarTipoAreaEnderecoSecundario() {
    if (!_mapDrawSecundario)
        return;

    _mapDrawSecundario.deleteAll();
    let raioVisible = _listaEndereco.TipoAreaEnderecoSecundario.val() === 1;
    _listaEndereco.RaioEmMetrosSecundario.visible(raioVisible)

    if (!raioVisible) {
        const divMapa = document.getElementById("mapaListaEndereco");
        _mapDrawSecundario.ShowDrawPalette(divMapa);
    }

    if (raioVisible) 
        _mapDrawSecundario.HideDrawPalette();


}


function setarRaioEmMetrosSecundario() {
    if (_mapDrawSecundario)
        _mapDrawSecundario.deleteAll();

    if ((_mapDrawSecundario) && (_listaEndereco.RaioEmMetrosSecundario.val() !== "" && _listaEndereco.Latitude.val() !== "" && _listaEndereco.Longitude.val() !== "")) {
        const latLngNormal = { lat: parseFloat(_listaEndereco.Latitude.val()), lng: parseFloat(_listaEndereco.Longitude.val()) };
        const shapeCircle = new ShapeCircle();
        shapeCircle.type = google.maps.drawing.OverlayType.CIRCLE;
        shapeCircle.fillColor = "#FF0000";
        shapeCircle.radius = parseInt(_listaEndereco.RaioEmMetrosSecundario.val());
        shapeCircle.center = latLngNormal
        _mapDrawSecundario.addShape(shapeCircle);
    }
}


function SetarAreaGeoLocalizacaoEnderecoSecundario() {
    if ((_mapDrawSecundario) && (_listaEndereco.TipoAreaEnderecoSecundario.val() === 2)) {
        _mapDrawSecundario.clear();
        _mapDrawSecundario.setJson(_listaEndereco.AreaSecundario.val());
    }
}


function obterJsonPoligonoGeoLocalizacaoSecondario() {
    if ((_mapDrawSecundario) && (_listaEndereco.TipoAreaEnderecoSecundario.val() === 2))
        return _mapDrawSecundario.getJson();

    return "";
}
