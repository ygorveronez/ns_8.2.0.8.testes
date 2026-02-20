var EnumModalPropostaMultimodalHelper = function () {
    this.Todos = -1;
    this.Nenhum = 0;
    this.PortoPorta = 1;
    this.PortaPorto = 2;
    this.PortaPorta = 3;
    this.PortoPorto = 4;
}

EnumModalPropostaMultimodalHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.Nenhum: return Localization.Resources.Enumeradores.ModalPropostaMultimodal.Nenhum;
            case this.PortoPorta: return Localization.Resources.Enumeradores.ModalPropostaMultimodal.UmPortoPorta;
            case this.PortaPorto: return Localization.Resources.Enumeradores.ModalPropostaMultimodal.DoisPortaPorto;
            case this.PortaPorta: return Localization.Resources.Enumeradores.ModalPropostaMultimodal.TresPortaPorta;
            case this.PortoPorto: return Localization.Resources.Enumeradores.ModalPropostaMultimodal.QuatroPortoPorto;
            case this.Todos: return Localization.Resources.Enumeradores.ModalPropostaMultimodal.Todos;
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.ModalPropostaMultimodal.Nenhum, value: this.Nenhum },
            { text: Localization.Resources.Enumeradores.ModalPropostaMultimodal.UmPortoPorta, value: this.PortoPorta },
            { text: Localization.Resources.Enumeradores.ModalPropostaMultimodal.DoisPortaPorto, value: this.PortaPorto },
            { text: Localization.Resources.Enumeradores.ModalPropostaMultimodal.TresPortaPorta, value: this.PortaPorta },
            { text: Localization.Resources.Enumeradores.ModalPropostaMultimodal.QuatroPortoPorto, value: this.PortoPorto }                 
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.ModalPropostaMultimodal.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumModalPropostaMultimodal = Object.freeze(new EnumModalPropostaMultimodalHelper());