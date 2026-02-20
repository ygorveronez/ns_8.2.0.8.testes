var EnumTipoParametroBaseTabelaFreteHelper = function () {
    this.Nenhum = "";
    this.TipoCarga = 1;
    this.ModeloReboque = 2;
    this.ModeloTracao = 3;
    this.ComponenteFrete = 4;
    this.NumeroEntrega = 5;
    this.Peso = 6;
    this.Distancia = 7;
    this.Rota = 8;
    this.ParametrosOcorrencia = 9;
    this.Pallets = 10;
    this.Tempo = 11;
    this.Ajudante = 12;
    this.ValorFreteLiquido = 13;
    //this.ValorBase = 14;
    //this.ExcedenteEntrega = 15;
    this.Hora = 16;
    this.TipoEmbalagem = 17;
    this.Pacote = 18;
};

EnumTipoParametroBaseTabelaFreteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoParametroBaseTabelaFrete.TipoCarga, value: this.TipoCarga },
            { text: Localization.Resources.Enumeradores.TipoParametroBaseTabelaFrete.ModeloReboque, value: this.ModeloReboque },
            { text: Localization.Resources.Enumeradores.TipoParametroBaseTabelaFrete.ModeloTracao, value: this.ModeloTracao },
            //{ text: Localization.Resources.Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete, value: this.ComponenteFrete },
            { text: Localization.Resources.Enumeradores.TipoParametroBaseTabelaFrete.NumeroEntrega, value: this.NumeroEntrega },
            { text: Localization.Resources.Enumeradores.TipoParametroBaseTabelaFrete.Peso, value: this.Peso },
            { text: Localization.Resources.Enumeradores.TipoParametroBaseTabelaFrete.Distancia, value: this.Distancia },
            { text: Localization.Resources.Enumeradores.TipoParametroBaseTabelaFrete.Rota, value: this.Rota },
            //{ text: Localization.Resources.Enumeradores.TipoParametroBaseTabelaFrete.ParametrosOcorrencia, value: this.ParametrosOcorrencia },
            { text: Localization.Resources.Enumeradores.TipoParametroBaseTabelaFrete.Pallets, value: this.Pallets },
            { text: Localization.Resources.Enumeradores.TipoParametroBaseTabelaFrete.Tempo, value: this.Tempo },
            //{ text: Localization.Resources.Enumeradores.TipoParametroBaseTabelaFrete.Ajudante, value: this.Ajudante },
            //{ text: Localization.Resources.Enumeradores.TipoParametroBaseTabelaFrete.ValorFreteLiquido, value: this.ValorFreteLiquido },
            //{ text: Localization.Resources.Enumeradores.TipoParametroBaseTabelaFrete.ValorBase, value: this.ValorBase },
            //{ text: Localization.Resources.Enumeradores.TipoParametroBaseTabelaFrete.ExcedenteEntrega, value: this.ExcedenteEntrega },
            { text: Localization.Resources.Enumeradores.TipoParametroBaseTabelaFrete.Hora, value: this.Hora },
            { text: Localization.Resources.Enumeradores.TipoParametroBaseTabelaFrete.TipoEmbalagem, value: this.TipoEmbalagem },
            { text: Localization.Resources.Enumeradores.TipoParametroBaseTabelaFrete.Pacote, value: this.Pacote },
        ];
    },
    obterOpcoesComNenhum: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoParametroBaseTabelaFrete.Nenhum, value: this.Nenhum }].concat(this.obterOpcoes());
    }
};

var EnumTipoParametroBaseTabelaFrete = Object.freeze(new EnumTipoParametroBaseTabelaFreteHelper());