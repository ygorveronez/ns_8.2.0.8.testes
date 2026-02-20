function ObterSVGPin(color, label) {
    return (
        '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="30" viewBox="0 0 1026 1539">' +
        ' <g>' +
        '  <path stroke="0" fill="' + color + '" d="m1024,512q0,109 -33,179l-364,774q-16,33 -47.5,52t-67.5,19t-67.5,-19t-46.5,-52l-365,-774q-33,-70 -33,-179q0,-212 150,-362t362,-150t362,150t150,362z" id="svg_1"/>' +
        '  <text stroke="#000" fill="#FFFFFF" stroke-width="0" x="636" y="560" id="svg_2" font-size="220" font-family="Arial, sans-serif" text-anchor="middle" xml:space="preserve" transform="matrix(2.812777609818733,0,0,2.8326989413850754,-1278.5555187961586,-781.1031876516241) " font-weight="bold">' +
        label +
        '</text>' +
        ' </g>' +
        '</svg>'
    );
}

function IconSVGMarker(svg) {
    return 'data:image/svg+xml,' + encodeURIComponent(svg);
}

function CorMarkerPorTipo(tipoMarker) {
    var cor = "";

    if (tipoMarker == EnumTipoMarker.Pin)
        cor = '#d45b5b';
    else if (tipoMarker == EnumTipoMarker.Distribuidor)
        cor = '#2386dc';
    if (tipoMarker == EnumTipoMarker.PinRestricao)
        cor = '#bfb104';
    else if (tipoMarker == EnumTipoMarker.DistribuidorSelecionado || tipoMarker == EnumTipoMarker.PinSelecionado)
        cor = '#2dab10';

    return cor;
}