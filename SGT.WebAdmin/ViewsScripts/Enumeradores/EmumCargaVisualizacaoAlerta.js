var EnumCargaVisualizacaoAlertaHelper = function() {
    this.Todos = "";
    this.AcompanhamentoCargas = 1;
    this.TorreMonitoramento = 2;
    
};


EnumCargaVisualizacaoAlertaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.AcompanhamentoCargas, value: this.AcompanhamentoCargas },
            { text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.TorreMonitoramento, value: this.TorreMonitoramento },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },

};


var EnumCargaVisualizacaoAlerta = Object.freeze(new EnumCargaVisualizacaoAlertaHelper());