var EnumSituacaoAutorizacaoClienteChamadoHelper = function () {
    this.Todos = 0;
    this.Aberto = 1;
    this.AguardandoAutorizacao = 2;
    this.Aprovado = 3;
    this.AprovadoParcial = 4;
    this.Rejeitado = 5;
};

EnumSituacaoAutorizacaoClienteChamadoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aberto", value: this.Aberto },
            { text: "Aguardando Autorização", value: this.AguardandoAutorizacao },
            { text: "Aprovado", value: this.Aprovado },
            { text: "Aprovado Parcial", value: this.AprovadoParcial },
            { text: "Rejeitado", value: this.Rejeitado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoAutorizacaoClienteChamado = Object.freeze(new EnumSituacaoAutorizacaoClienteChamadoHelper());