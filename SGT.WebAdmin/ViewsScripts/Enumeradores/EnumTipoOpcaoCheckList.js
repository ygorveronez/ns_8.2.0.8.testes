var EnumTipoOpcaoCheckListHelper = function () {
    this.Aprovacao = 0;
    this.SimNao = 1;
    this.Opcoes = 2;
    this.Informativo = 3;
    this.Selecoes = 4;
    this.Escala = 5;
};

EnumTipoOpcaoCheckListHelper.prototype = {
    obterOpcoes: function () {
        var opcoes = [];

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS)
            opcoes.push({ text: "Aprovação", value: this.Aprovacao });

        opcoes.push({ text: "Informativo", value: this.Informativo });
        opcoes.push({ text: "Opções (Múltiplo)", value: this.Opcoes });
        opcoes.push({ text: "Seleções (Uníco)", value: this.Selecoes });
        opcoes.push({ text: "Sim e Não", value: this.SimNao });
        opcoes.push({ text: "Escala", value: this.Escala });

        return opcoes;
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: null }].concat(this.obterOpcoes());
    }
};

var EnumTipoOpcaoCheckList = Object.freeze(new EnumTipoOpcaoCheckListHelper());