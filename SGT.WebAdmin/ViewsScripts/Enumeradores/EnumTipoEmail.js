var EnumTipoEmailHelper = function () {
    this.Principal = 1;
    this.Administrativo = 2;
    this.Cobranca = 3;
    this.Financeiro = 4;
    this.Pessoal = 5;
    this.SAC = 6;
    this.Coleta = 7;
    this.Agendamento = 8;
    this.Outros = 99;
};

EnumTipoEmailHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoEmail.Principal, value: this.Principal },
            { text: Localization.Resources.Enumeradores.TipoEmail.Administrativo, value: this.Administrativo },
            { text: Localization.Resources.Enumeradores.TipoEmail.Cobranca, value: this.Cobranca },
            { text: Localization.Resources.Enumeradores.TipoEmail.Financeiro, value: this.Financeiro },
            { text: Localization.Resources.Enumeradores.TipoEmail.Pessoal, value: this.Pessoal },
            { text: Localization.Resources.Enumeradores.TipoEmail.SAC, value: this.SAC },
            { text: Localization.Resources.Enumeradores.TipoEmail.Coleta, value: this.Coleta },
            { text: Localization.Resources.Enumeradores.TipoEmail.Agendamento, value: this.Agendamento },
            { text: Localization.Resources.Enumeradores.TipoEmail.Outros, value: this.Outros }
        ];
    },
    obterDescricao: function (tipoEmail) {
        switch (tipoEmail) {
            case EnumTipoEmail.Principal: return Localization.Resources.Enumeradores.TipoEmail.Principal;
            case EnumTipoEmail.Administrativo: return Localization.Resources.Enumeradores.TipoEmail.Administrativo;
            case EnumTipoEmail.Cobranca: return Localization.Resources.Enumeradores.TipoEmail.Cobranca;
            case EnumTipoEmail.Financeiro: return Localization.Resources.Enumeradores.TipoEmail.Financeiro;
            case EnumTipoEmail.Pessoal: return Localization.Resources.Enumeradores.TipoEmail.Pessoal;
            case EnumTipoEmail.SAC: return Localization.Resources.Enumeradores.TipoEmail.SAC;
            case EnumTipoEmail.Coleta: return Localization.Resources.Enumeradores.TipoEmail.Coleta;
            case EnumTipoEmail.Agendamento: return Localization.Resources.Enumeradores.TipoEmail.Agendamento;
            case EnumTipoEmail.Outros: return Localization.Resources.Enumeradores.TipoEmail.Outros;
            default: return "";
        }
    }
};

var EnumTipoEmail = Object.freeze(new EnumTipoEmailHelper());