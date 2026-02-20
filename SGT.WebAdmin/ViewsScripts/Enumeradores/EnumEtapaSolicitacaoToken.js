var EnumEtapaSolicitacaoTokenHelper = function () {
    this.Todos = "";
    this.SemRegraAprovacao = 1;
    this.Cancelado = 2;
    this.AgAprovacao = 3;
    this.SolicitacaoAprovada = 4;
    this.SolicitacaoReprovada = 5;
    this.Finalizada = 6;
    this.EmLiberacaoSistematica = 7;
    this.LiberacaoSistematicaProblema = 8;
};

EnumEtapaSolicitacaoTokenHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Sem Regra de Aprovação", value: this.SemRegraAprovacao },
            { text: "Cancelado", value: this.Cancelado },
            { text: "Aguardando Aprovação", value: this.AgAprovacao },
            { text: "Solicitação Aprovada", value: this.SolicitacaoAprovada },
            { text: "Solicitação Reprovada", value: this.SolicitacaoReprovada },
            { text: "Finalizada", value: this.Finalizada },
            { text: "Em Liberacao Sistematica", value: this.EmLiberacaoSistematica },
            { text: "Liberação Sistematica Com Problema", value: this.LiberacaoSistematicaProblema },
 
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumEtapaSolicitacaoToken = Object.freeze(new EnumEtapaSolicitacaoTokenHelper());