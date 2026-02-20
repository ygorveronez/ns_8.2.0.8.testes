var EnumTipoTomadorHelper = function () {
    this.Todos = "",
    this.NaoInformado = -1;
    this.Remetente = 0;
    this.Expedidor = 1;
    this.Recebedor = 2;
    this.Destinatario = 3;
    this.Outros = 4;
    this.Intermediario = 5;
};

EnumTipoTomadorHelper.prototype = {
    obterDescricao: function (tipoTomador) {
        switch (tipoTomador) {
            case this.NaoInformado: return Localization.Resources.Enumeradores.TipoTomador.NaoInformado;
            case this.Remetente: return Localization.Resources.Enumeradores.TipoTomador.Remetente;
            case this.Expedidor: return Localization.Resources.Enumeradores.TipoTomador.Expedidor;
            case this.Recebedor: return Localization.Resources.Enumeradores.TipoTomador.Recebedor;
            case this.Destinatario: return Localization.Resources.Enumeradores.TipoTomador.Destinatario;
            case this.Outros: return Localization.Resources.Enumeradores.TipoTomador.Outros;
            case this.Intermediario: return Localization.Resources.Enumeradores.TipoTomador.Intermediario;
            default: return undefined;
        }
    },
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoTomador.NaoInformado, value: this.NaoInformado },
            { text: Localization.Resources.Enumeradores.TipoTomador.Remetente, value: this.Remetente },
            { text: Localization.Resources.Enumeradores.TipoTomador.Expedidor, value: this.Expedidor },
            { text: Localization.Resources.Enumeradores.TipoTomador.Recebedor, value: this.Recebedor },
            { text: Localization.Resources.Enumeradores.TipoTomador.Destinatario, value: this.Destinatario },
            { text: Localization.Resources.Enumeradores.TipoTomador.Outros, value: this.Outros },
            { text: Localization.Resources.Enumeradores.TipoTomador.Intermediario, value: this.Intermediario }
        ];
    },
    obterOpcoesPadrao: function () {
        return [            
            { text: Localization.Resources.Enumeradores.TipoTomador.Remetente, value: this.Remetente },
            { text: Localization.Resources.Enumeradores.TipoTomador.Expedidor, value: this.Expedidor },
            { text: Localization.Resources.Enumeradores.TipoTomador.Recebedor, value: this.Recebedor },
            { text: Localization.Resources.Enumeradores.TipoTomador.Destinatario, value: this.Destinatario },
            { text: Localization.Resources.Enumeradores.TipoTomador.Outros, value: this.Outros }
            //{ text: Localization.Resources.Enumeradores.TipoTomador.Intermediario, value: this.Intermediario }
        ];
    },
    obterOpcoesRestricaoFilaCarregamento: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoTomador.NaoInformado, value: this.NaoInformado },
            { text: Localization.Resources.Enumeradores.TipoTomador.Remetente, value: this.Remetente },
            { text: Localization.Resources.Enumeradores.TipoTomador.Destinatario, value: this.Destinatario }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoTomador.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesTipoCliente: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoTomador.Remetente, value: this.Remetente },
            { text: Localization.Resources.Enumeradores.TipoTomador.Destinatario, value: this.Destinatario }
        ];
    },
}

var EnumTipoTomador = Object.freeze(new EnumTipoTomadorHelper());