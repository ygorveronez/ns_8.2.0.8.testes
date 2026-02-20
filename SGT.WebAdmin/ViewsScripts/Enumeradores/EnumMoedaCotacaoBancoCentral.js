var EnumMoedaCotacaoBancoCentralHelper = function () {
    this.Todas = -1;
    this.Real = 0;
    this.DolarVenda = 1;
    this.DolarCompra = 10813;
    this.PesoArgentino = 3;
    this.PesoUruguaio = 4;
    this.PesoChileno = 5;
    this.Guarani = 6;
    this.NovoSolPeruano = 7;
};

EnumMoedaCotacaoBancoCentralHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.Real, value: this.Real },
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.DolarVenda, value: this.DolarVenda },
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.DolarCompra, value: this.DolarCompra },
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.PesoArgentino, value: this.PesoArgentino },
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.PesoUruguaio, value: this.PesoUruguaio },
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.PesoChileno, value: this.PesoChileno },
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.Guarani, value: this.Guarani },
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.NovoSolPeruano, value: this.NovoSolPeruano }
        ];
    },
    obterOpcoesExtrangeiro: function () {
        return [
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.Dolar, value: this.DolarVenda },
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.PesoArgentino, value: this.PesoArgentino },
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.PesoUruguaio, value: this.PesoUruguaio },
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.PesoChileno, value: this.PesoChileno },
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.Guarani, value: this.Guarani },
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.NovoSolPeruano, value: this.NovoSolPeruano }
        ];
    },
    obterOpcoesComReais: function () {
        return [
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.Dolar, value: this.DolarVenda },
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.PesoArgentino, value: this.PesoArgentino },
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.PesoUruguaio, value: this.PesoUruguaio },
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.PesoChileno, value: this.PesoChileno },
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.Guarani, value: this.Guarani },
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.Real, value: this.Real },
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.NovoSolPeruano, value: this.NovoSolPeruano }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.Todas, value: this.Todas }].concat(this.obterOpcoes());
    },
    obterDescricao: function (e) {
        switch (e) {
            case this.DolarCompra: return Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.DolarCompra;
            case this.DolarVenda: return Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.DolarVenda;
            case this.Guarani: return Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.Guarani;
            case this.PesoArgentino: return Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.PesoArgentino;
            case this.PesoChileno: return Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.PesoChileno;
            case this.PesoUruguaio: return Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.PesoUruguaio;
            case this.Real: return Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.Real;
            case this.NovoSolPeruano: return Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.NovoSolPeruano;
            default: return "";
        }
    },
    obterOpcoesMoedasEstrangeiras: function () {
        return [
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.DolarVenda, value: this.DolarVenda },
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.DolarCompra, value: this.DolarCompra },
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.PesoArgentino, value: this.PesoArgentino },
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.PesoUruguaio, value: this.PesoUruguaio },
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.PesoChileno, value: this.PesoChileno },
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.Guarani, value: this.Guarani },
            { text: Localization.Resources.Enumeradores.MoedaCotacaoBancoCentral.NovoSolPeruano, value: this.NovoSolPeruano }
        ];
    },
};

var EnumMoedaCotacaoBancoCentral = Object.freeze(new EnumMoedaCotacaoBancoCentralHelper());