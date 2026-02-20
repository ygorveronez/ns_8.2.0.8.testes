var EnumTipoPreenchimentoChecklistHelper = function () {
    this.PreenchimentoObrigatorio = 0;
    this.PreenchimentoDesabilitado = 2;
};

EnumTipoPreenchimentoChecklistHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Preenchimento do Checklist Obrigatório", value: this.PreenchimentoObrigatorio },
            { text: "Desabilitar a Etapa de Checklist", value: this.PreenchimentoDesabilitado },
        ];
    }
}

var EnumTipoPreenchimentoChecklist = Object.freeze(new EnumTipoPreenchimentoChecklistHelper());