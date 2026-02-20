var EnumControlarExibicaoMapaGridHelper = function () {
    this.MapaEGrid = 0;
    this.Mapa = 1;
    this.Grid = 2;
}

EnumControlarExibicaoMapaGridHelper.prototype = {
    obterTodos: function () {
        return [
            this.MapaEGrid,
            this.Mapa,
            this.Grid
        ];
    }
}

var EnumControlarExibicaoMapaGrid = Object.freeze(new EnumControlarExibicaoMapaGridHelper());





