var EnumEscalationListHelper = function () {
    this.Todos = "";
    this.Nenhum = 0;
    this.Nivel1 = 1;
    this.Nivel2 = 2;
    this.Nivel3 = 3;
    this.Nivel4 = 4;
    this.Nivel5 = 5;
    this.Nivel6 = 6;
    this.Nivel7 = 7;
    this.Nivel8 = 8;
    this.Nivel9 = 9;
    this.Nivel10 = 10;
};

EnumEscalationListHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.EscalationList.Nivelum , value: this.Nivel1 },
            { text: Localization.Resources.Enumeradores.EscalationList.Niveldois , value: this.Nivel2 },
            { text: Localization.Resources.Enumeradores.EscalationList.Niveltres , value: this.Nivel3 },
            { text: Localization.Resources.Enumeradores.EscalationList.Nivelquatro , value: this.Nivel4 },
            { text: Localization.Resources.Enumeradores.EscalationList.Nivelcinco , value: this.Nivel5 },
            { text: Localization.Resources.Enumeradores.EscalationList.Nivelseis , value: this.Nivel6 },
            { text: Localization.Resources.Enumeradores.EscalationList.Nivelsete , value: this.Nivel7 },
            { text: Localization.Resources.Enumeradores.EscalationList.Niveloito , value: this.Nivel8 },
            { text: Localization.Resources.Enumeradores.EscalationList.Nivelnove , value: this.Nivel9 },
            { text: Localization.Resources.Enumeradores.EscalationList.Niveldez , value: this.Nivel10 },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.EscalationList.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesCadastro: function () {
        return [{ text: Localization.Resources.Enumeradores.EscalationList.Nenhum, value: this.Nenhum }].concat(this.obterOpcoes());
    },
    obterDescricao: function (codigEscalation) {
        switch (codigEscalation) {
            case this.Nivel1:
                return Localization.Resources.Enumeradores.EscalationList.Nivelum 

            case this.Nivel2:
                return Localization.Resources.Enumeradores.EscalationList.Niveldois

            case this.Nivel3:
                return Localization.Resources.Enumeradores.EscalationList.Niveltres

            case this.Nivel4:
                return Localization.Resources.Enumeradores.EscalationList.Nivelquatro

            case this.Nivel5:
                return Localization.Resources.Enumeradores.EscalationList.Nivelcinco

            case this.Nivel6:
                return Localization.Resources.Enumeradores.EscalationList.Nivelseis

            case this.Nivel7:
                return Localization.Resources.Enumeradores.EscalationList.Nivelsete

            case this.Nivel8:
                return Localization.Resources.Enumeradores.EscalationList.Niveloito

            case this.Nivel9:
                return Localization.Resources.Enumeradores.EscalationList.Nivelnove

            case this.Nivel10:
                return Localization.Resources.Enumeradores.EscalationList.Niveldez

            default:
                return Localization.Resources.Enumeradores.EscalationList.SemNivel

        }
    }
};

var EnumEscalationList = Object.freeze(new EnumEscalationListHelper());