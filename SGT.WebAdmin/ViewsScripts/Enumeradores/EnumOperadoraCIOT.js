var EnumOperadoraCIOTHelper = function () {
    this.eFrete = 1;
    this.Repom = 2;
    this.Pamcard = 3;
    this.Pagbem = 4;
    this.Target = 5;
    this.Extratta = 6;
    this.BBC = 7;
    this.Ambipar = 8;
    this.Rodocred = 9;
    this.RepomFrete = 10;
    this.TruckPad = 11;
};

EnumOperadoraCIOTHelper.prototype.ObterOpcoes = function () {
    return [
        { text: Localization.Resources.Enumeradores.OperadorCIOT.eFrete, value: this.eFrete },
        { text: Localization.Resources.Enumeradores.OperadorCIOT.Repom, value: this.Repom },
        { text: Localization.Resources.Enumeradores.OperadorCIOT.Pamcard, value: this.Pamcard },
        { text: Localization.Resources.Enumeradores.OperadorCIOT.Pagbem, value: this.Pagbem },
        { text: Localization.Resources.Enumeradores.OperadorCIOT.Target, value: this.Target },
        { text: Localization.Resources.Enumeradores.OperadorCIOT.Extratta, value: this.Extratta },
        { text: Localization.Resources.Enumeradores.OperadorCIOT.BBC, value: this.BBC },
        { text: Localization.Resources.Enumeradores.OperadorCIOT.Ambipar, value: this.Ambipar },
        { text: Localization.Resources.Enumeradores.OperadorCIOT.Rodocred, value: this.Rodocred },
        { text: Localization.Resources.Enumeradores.OperadorCIOT.RepomFrete, value: this.RepomFrete },
        { text: Localization.Resources.Enumeradores.OperadorCIOT.TruckPad, value: this.TruckPad }
    ];
};

EnumOperadoraCIOTHelper.prototype.obterDescricao =  function (valor) {
    switch (valor) {
        case this.eFrete: return Localization.Resources.Enumeradores.OperadorCIOT.eFrete;
        case this.Repom: return Localization.Resources.Enumeradores.OperadorCIOT.Repom;
        case this.Pamcard: return Localization.Resources.Enumeradores.OperadorCIOT.Pamcard;
        case this.Pagbem: return Localization.Resources.Enumeradores.OperadorCIOT.Pagbem;
        case this.Target: return Localization.Resources.Enumeradores.OperadorCIOT.Target;
        case this.Extratta: return Localization.Resources.Enumeradores.OperadorCIOT.Extratta;
        case this.BBC: return Localization.Resources.Enumeradores.OperadorCIOT.BBC;
        case this.Ambipar: return Localization.Resources.Enumeradores.OperadorCIOT.Ambipar;
        case this.Rodocred: return Localization.Resources.Enumeradores.OperadorCIOT.Rodocred;
        case this.RepomFrete: return Localization.Resources.Enumeradores.OperadorCIOT.RepomFrete;
        case this.TruckPad: return Localization.Resources.Enumeradores.OperadorCIOT.TruckPad;

        default: return "";
    }
   
};

EnumOperadoraCIOTHelper.prototype.ObterOpcoesPesquisa = function () {
    return [{ text: Localization.Resources.Enumeradores.OperadorCIOT.Todos, value: "" }].concat( this.ObterOpcoes());
};

var EnumOperadoraCIOT = Object.freeze(new EnumOperadoraCIOTHelper());