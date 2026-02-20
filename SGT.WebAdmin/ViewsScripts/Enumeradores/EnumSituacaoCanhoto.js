var EnumSituacaoCanhotoHelper = function () {
    this.Todas = 0;
    this.Pendente = 1;
    this.Justificado = 2;
    this.RecebidoFisicamente = 3;
    this.Extraviado = 4;
    this.EntregueMotorista = 5;
    this.EnviadoCliente = 6;
    this.RecebidoCliente = 7;
    this.Cancelado = 8;
    this.NFe;
    this.Avulso;
};

EnumSituacaoCanhotoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.Pendente, value: this.Pendente },
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.Justificado, value: this.Justificado },
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.RecebidoFisicamente, value: this.RecebidoFisicamente },
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.Extraviado, value: this.Extraviado },
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.EntreguePeloMotorista, value: this.EntregueMotorista },
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.EnviadoAoCliente, value: this.EnviadoCliente },
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.RecebidoPeloCliente, value: this.RecebidoCliente },
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.Cancelado, value: this.Cancelado }
        ];
    },
    obterOpcoesPesquisa: function (defaultText, defaultValue) {
        return [{ text: defaultText || Localization.Resources.Gerais.Geral.NaoSelecionado , value: defaultValue || "" }].concat(this.obterOpcoes());
    },
    obterOpcoesPesquisaComPlaceHolder: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoCanhoto.TodasAsSituacoes, value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesMultiEmbarcador: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.Pendente, value: this.Pendente },
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.Justificado, value: this.Justificado },
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.RecebidoFisicamente, value: this.RecebidoFisicamente },
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.Extraviado, value: this.Extraviado },
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.Cancelado, value: this.Cancelado }
        ];
    },
    obterOpcoesMultiTMS: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.Pendente, value: this.Pendente },
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.EntreguePeloMotorista, value: this.EntregueMotorista },
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.RecebidoFisicamente, value: this.RecebidoFisicamente },
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.EnviadoAoCliente, value: this.EnviadoCliente },
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.RecebidoPeloCliente, value: this.RecebidoCliente },
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.Justificado, value: this.Justificado },
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.Extraviado, value: this.Extraviado }
        ];
    },

    obterOpcoesTMS: function (defaultText, defaultValue) {
        return [{ text: defaultText || Localization.Resources.Gerais.Geral.Todos, value: defaultValue || "" }].concat(this.obterOpcoesMultiTMS());
    },

    obterOpcoesSituacao: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.Pendente, value: this.Pendente },
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.Justificado, value: this.Justificado },
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.RecebidoFisicamente, value: this.RecebidoFisicamente },
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.Extraviado, value: this.Extraviado }
        ];
    },
    obterOpcoesSituacaoPesquisa: function (defaultText, defaultValue) {
        return [{ text: defaultText || Localization.Resources.Enumeradores.SituacaoCanhoto.Todos, value: defaultValue || "" }].concat(this.obterOpcoesSituacao());
    },

    obterOpcoesTipoCanhoto: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.NFe, value: this.NFe },
            { text: Localization.Resources.Enumeradores.SituacaoCanhoto.Avulso, value: this.Avulso }
        ];
    },
    obterOpcoesTipoCanhotoPesquisa: function (defaultText, defaultValue) {
        return [{ text: defaultText || Localization.Resources.Enumeradores.SituacaoCanhoto.Todos, value: defaultValue || "" }].concat(this.obterOpcoesTipoCanhoto());
    },

    obterOpcoesMultiTMSPesquisa: function (defaultText, defaultValue) {
        return [{ text: defaultText || Localization.Resources.Enumeradores.SituacaoCanhoto.Todos, value: defaultValue || "" }].concat(this.obterOpcoesMultiTMS());
    },
    
    
}

var EnumSituacaoCanhoto = Object.freeze(new EnumSituacaoCanhotoHelper());