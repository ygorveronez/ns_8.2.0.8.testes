var EnumFiltroPreCargaHelper = function () {
    this.Todos = 0;
    this.ComCarga = 1;
    this.ComDadosInformados = 2;
    this.EmDia = 3;
    this.EmAtraso = 4;
    this.ProblemaVincularCarga = 5;
};

EnumFiltroPreCargaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Com Carga", value: this.ComCarga },
            { text: "Com Dados Informados", value: this.ComDadosInformados },
            { text: "Em Dia", value: this.EmDia },
            { text: "Em Atraso", value: this.EmAtraso },
            { text: "Problema ao Vincular Carga", value: this.ProblemaVincularCarga }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumFiltroPreCarga = Object.freeze(new EnumFiltroPreCargaHelper());
