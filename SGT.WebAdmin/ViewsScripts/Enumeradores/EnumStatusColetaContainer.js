var EnumStatusColetaContainerHelper = function () {
    this.Todas = "";
    this.AguardandoColeta = 0;
    this.EmDeslocamentoVazio = 1;
    this.EmAreaEsperaVazio = 2;
    this.EmDeslocamentoCarregamento = 3;
    this.EmDeslocamentoCarregado = 4;
    this.EmAreaEsperaCarregado = 5;
    this.Porto = 6;
    this.Cancelado = 7;
    this.EmCarregamento = 8;
    this.EmbarcadoNavio = 9;
};

EnumStatusColetaContainerHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Coleta", value: this.AguardandoColeta },
            { text: "Em Deslocamento Vazio", value: this.EmDeslocamentoVazio },
            { text: "Em Área de espera Vazio", value: this.EmAreaEsperaVazio },
            { text: "Em Deslocamento para Carregamento", value: this.EmDeslocamentoCarregamento },
            { text: "Em Carregamento", value: this.EmCarregamento },
            { text: "Em Deslocamento Carregado", value: this.EmDeslocamentoCarregado },
            { text: "Em Área de espera Carregado", value: this.EmAreaEsperaCarregado },
            { text: "Porto", value: this.Porto },
            { text: "Cancelado", value: this.Cancelado },
            { text: "Embarcado Navio", value: this.EmbarcadoNavio }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    },
    obterDescricao: function (valor) {
        switch (valor) {
            case this.AguardandoColeta: return "Aguardando Coleta";
            case this.EmDeslocamentoVazio: return "Em Deslocamento Vazio";
            case this.EmAreaEsperaVazio: return "Em Área de espera Vazio";
            case this.EmDeslocamentoCarregamento: return "Em Deslocamento para Carregamento";
            case this.EmCarregamento: return "Em Carregamento";
            case this.EmDeslocamentoCarregado: return "Em Deslocamento Carregado";
            case this.EmAreaEsperaCarregado: return "Em Área de espera Carregado";
            case this.Porto: return "Porto";
            case this.Cancelado: return "Cancelado";
            case this.EmbarcadoNavio: return "Embarcado Navio";
            default: return "";
        }
    }
}

var EnumStatusColetaContainer = Object.freeze(new EnumStatusColetaContainerHelper());