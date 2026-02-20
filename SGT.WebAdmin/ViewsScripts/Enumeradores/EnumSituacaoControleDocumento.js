var EnumSituacaoControleDocumentoHelper = function () {
    this.AguardandoAprovacao = 1;
    this.RejeitadoPeloTransportador = 2;
    this.ParqueadoManualmente = 3;
    this.Inconsistente = 4;
    this.Desparqueado = 5;
    this.ParqueadoAutomaticamente = 6;
    this.Liberado = 7;
    this.AguardandoValidacao = 8;
    //this.InconsistenteSemTratativa = 9;
};

EnumSituacaoControleDocumentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Aprovação", value: this.AguardandoAprovacao },
            { text: "Rejeitado Pelo Transportador", value: this.RejeitadoPeloTransportador },
            { text: "Parqueado Manualmente", value: this.ParqueadoManualmente },
            { text: "Parqueado Automaticamente", value: this.ParqueadoAutomaticamente },
            { text: "Liberado", value: this.Liberado },
            { text: "Aguardando Validação", value: this.AguardandoValidacao },
            { text: "Inconsistente", value: this.Inconsistente },
            //{ text: "Inconsistente Sem Tratativa", value: this.InconsistenteSemTratativa },
        ];
    },

    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoControleDocumento = Object.freeze(new EnumSituacaoControleDocumentoHelper());