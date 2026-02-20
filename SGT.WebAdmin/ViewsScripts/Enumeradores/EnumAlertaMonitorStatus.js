var EnumAlertaMonitorStatusHelper = function () {
    this.Todos = -1;
    this.EmAberto = 0;
    this.Finalizado = 1;
    this.EmTratativa= 2;
};

EnumAlertaMonitorStatusHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Em aberto", value: this.EmAberto },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Em tratativa", value: this.EmTratativa }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
    obterDescricao: function (status) {
        switch (status) {
            case this.Todos: return "Todos";
            case this.EmAberto: return "Em aberto";
            case this.Finalizado: return "Finalizado";
            case this.EmTratativa: return "Em tratativa";
        }
    }
}

var EnumAlertaMonitorStatus = Object.freeze(new EnumAlertaMonitorStatusHelper());