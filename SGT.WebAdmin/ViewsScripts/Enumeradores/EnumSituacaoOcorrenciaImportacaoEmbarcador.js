var EnumSituacaoOcorrenciaIntegracaoEmbarcadorHelper = function () {
    this.AgConsultaCTes = 1;
    this.Pendente = 2;
    this.Problemas = 3;
    this.AgGeracaoOcorrencia = 4;
    this.Finalizado = 5;
    this.AgConsultaCTesCancelados = 6;
    this.Cancelado = 7;
    this.AgGeracaoCancelamento = 8;
};

EnumSituacaoOcorrenciaIntegracaoEmbarcadorHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { value: this.AgConsultaCTes, text: "Ag. Consulta dos CT-es" },
            { value: this.Pendente, text: "Pendente" },
            { value: this.Problemas, text: "Falha" },
            { value: this.AgGeracaoOcorrencia, text: "Ag. Geração Ocorrência" },
            { value: this.Finalizado, text: "Finalizado" },
            { value: this.AgConsultaCTesCancelados, text: "Ag. Consulta CT-es Cancelados" },
            { value: this.AgGeracaoCancelamento, text: "Ag. Geração Cancelamento" },
            { value: this.Cancelado, text: "Cancelamento Gerado" }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ value: "", text: "Todos" }].concat(this.ObterOpcoes());
    }
};

var EnumSituacaoOcorrenciaIntegracaoEmbarcador = Object.freeze(new EnumSituacaoOcorrenciaIntegracaoEmbarcadorHelper());