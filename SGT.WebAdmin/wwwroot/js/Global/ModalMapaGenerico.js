// Abre um moda com um mapa. Pode-se configurar:
// - Título
// - Zoom
// - Altura do modal
// - Uma lista de markers
// A posição do mapa será o centro de todos os markers
function abrirModalMapaGenerico(options) {
    const { containerId, titulo, zoom, height, markers } = options;

    const container = $(containerId);

    let _mapa = null;

    const finalHeight = height != null ? height : 600;

    container.html(`
        <div class= "modal modal-wide fade" id="divModalMapaGenerico" role = "dialog" aria-hidden="true" tabindex="-1" >
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-hidden="true"> &times;</button>
                        <h4 class="modal-title" id="myModalLabel">${titulo}</h4>
                    </div>
                    <div class="modal-body" style="padding:0px;">
                        <div>
                            <div class="smart-form">
                                <fieldset>
                                    <div class="map-container">
                                        <div id="mapaGenerico" style="width:100%; height:${finalHeight}px"></div>
                                    </div>
                                </fieldset>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div >    
   `);

    function carregarMapa() {
        _mapa = null;
        if (_mapa == null) {
            opcoesMapa = new OpcoesMapa(false, false);
            _mapa = new MapaGoogle("mapaGenerico", false, opcoesMapa);
        }

        _mapa.clear();
    }

    function criarMarkers() {
        for (let marker of markers) {
            _mapa.draw.addShape(marker);
        }
    }

    carregarMapa();
    criarMarkers();

    _mapa.direction.setZoom(zoom != null ? zoom : 13);

    setTimeout(function () { _mapa.draw.centerShapes(); }, 500);

    $("#divModalMapaGenerico").modal('show');
}