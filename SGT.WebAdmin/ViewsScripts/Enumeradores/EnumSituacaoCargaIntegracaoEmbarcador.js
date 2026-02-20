var EnumSituacaoCargaIntegracaoEmbarcadorHelper = function () {
    this.AgConsultaCTes = 1;
    this.AgConsultaMDFes = 2;
    this.AgGeracaoCarga = 3;
    this.Pendente = 4;
    this.Problemas = 5;
    this.Finalizado = 6;
    this.AgConsultaCTesCancelados = 7;
    this.AgConsultaMDFesCancelados = 8;
    this.AgGeracaoCancelamento = 9;
    this.Cancelado = 10;
    this.AjustadoManualmente = 11;
};

EnumSituacaoCargaIntegracaoEmbarcadorHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { value: this.AgConsultaCTes, text: "Ag. Consulta dos CT-es" },
            { value: this.AgConsultaMDFes, text: "Ag. Consulta dos MDF-es" },
            { value: this.AgGeracaoCarga, text: "Ag. Geração da Carga" },
            { value: this.Pendente, text: "Pendente" },
            { value: this.Problemas, text: "Falha" },
            { value: this.Finalizado, text: "Carga Gerada" },
            { value: this.AgConsultaCTesCancelados, text: "Ag. Consulta CT-es Cancelados" },
            //{ value: this.AgConsultaMDFesCancelados, text: "Ag. Consulta MDF-es Cancelados" },
            { value: this.AgGeracaoCancelamento, text: "Ag. Geração do Cancelamento" },
            { value: this.Cancelado, text: "Cancelamento Gerado" },
            { value: this.AjustadoManualmente, text: "Ajuste manual"}
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ value: "", text: "Todos" }].concat(this.ObterOpcoes());
    }
};

var EnumSituacaoCargaIntegracaoEmbarcador = Object.freeze(new EnumSituacaoCargaIntegracaoEmbarcadorHelper());