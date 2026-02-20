var EnumSituacaoPreAgrupamentoCargaHelper = function () {
    this.Todas = "";
    this.EmEscrita = 1;
    this.AguardandoProcessamento = 2;
    this.AguardandoCarregamento = 3;
    this.Carregado = 4;
    this.ProblemaCarregamento = 5;
    this.SemCarga = 6;
    this.AguardandoRedespacho = 7;
    this.AguardandoCargasPararEncaixe = 8;
};

EnumSituacaoPreAgrupamentoCargaHelper.prototype = {

    obterOpcoes: function () {
        return [
            { text: "Aguardando Carregamento", value: this.AguardandoCarregamento },
            { text: "Aguardando Processamento", value: this.AguardandoProcessamento },
            { text: "Ag. Carga de Redespacho", value: this.AguardandoRedespacho },
            { text: "Carregado", value: this.Carregado },
            { text: "Problema no Carregamento", value: this.ProblemaCarregamento },
            { text: "Aguardando cargas para fazer o encaixe", value: this.AguardandoCargasPararEncaixe },
            { text: "Sem todas as Cargas", value: this.SemCarga }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoPreAgrupamentoCarga = Object.freeze(new EnumSituacaoPreAgrupamentoCargaHelper());