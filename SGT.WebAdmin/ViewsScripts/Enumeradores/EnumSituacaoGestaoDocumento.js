var EnumSituacaoGestaoDocumentoHelper = function () {
    this.Todos = "";
    this.Inconsistente = 1;
    this.Rejeitado = 2;
    this.Aprovado = 3;
    this.AprovadoComDesconto = 4;
    this.AguardandoAprovacao = 5;
    this.SemRegraAprovacao = 6;
    this.EmTratativa = 7;
};

EnumSituacaoGestaoDocumentoHelper.prototype = {
    isAprovado: function (situacao) {
        return (situacao == this.Aprovado) || (situacao == this.AprovadoComDesconto);
    },
    obterOpcoes: function () {
        var opcoes = [];

        if (_CONFIGURACAO_TMS.UsarAlcadaAprovacaoGestaoDocumentos)
            opcoes.push({ text: "Aguardando Aprovação", value: this.AguardandoAprovacao });

        opcoes.push({ text: "Aprovado", value: this.Aprovado });

        if (_CONFIGURACAO_TMS.HabilitarDescontoGestaoDocumento)
            opcoes.push({ text: "Aprovado com Desconto", value: this.AprovadoComDesconto });

        opcoes.push({ text: "Inconsistente", value: this.Inconsistente });
        opcoes.push({ text: "Rejeitado", value: this.Rejeitado });

        if (_CONFIGURACAO_TMS.UsarAlcadaAprovacaoGestaoDocumentos)
            opcoes.push({ text: "Sem Regra de Aprovação", value: this.SemRegraAprovacao });

        if (_CONFIGURACAO_TMS.PermitirDeixarDocumentoEmTratativa)
            opcoes.push({ text: "Em Tratativa", value: this.EmTratativa });
        
        return opcoes;
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoGestaoDocumento = Object.freeze(new EnumSituacaoGestaoDocumentoHelper());
