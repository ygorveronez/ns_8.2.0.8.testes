var EnumStatusGestaoCargaHelper = function () {
    this.Todas = 0;
    this.EmCarregamento = 1;
    this.EmViagem = 2;
    this.EmDescarga = 3;
    this.Finalizado = 4;
    this.Pendente = 5;
};

EnumStatusGestaoCargaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Relatorios.Cargas.GestaoCarga.EmCarregamento, value: this.EmCarregamento },
            { text: Localization.Resources.Relatorios.Cargas.GestaoCarga.EmViagem, value: this.EmViagem },
            { text: Localization.Resources.Relatorios.Cargas.GestaoCarga.EmDescarga, value: this.EmDescarga },
            { text: Localization.Resources.Relatorios.Cargas.GestaoCarga.Finalizado, value: this.Finalizado },
            { text: Localization.Resources.Relatorios.Cargas.GestaoCarga.Pendente, value: this.Pendente }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Relatorios.Cargas.GestaoCarga.Todos, value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumStatusGestaoCarga = Object.freeze(new EnumStatusGestaoCargaHelper());