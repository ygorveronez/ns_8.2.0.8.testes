var EnumOrigemSituacaoEntregaHelper = function () {
    this.Todas = "";
    this.UsuarioMultiEmbarcador = 1;
    this.UsuarioPortalTransportador = 2;
    this.App = 3;
    this.ArquivoEDI = 4;
    this.MonitoramentoAutomaticamente = 5;
    this.WebService = 6;
    this.LiberacaoCanhoto = 7;
    this.Correios = 8;
    this.AlertaMonitoramento = 9;
}

EnumOrigemSituacaoEntregaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Usuário do ME", value: this.UsuarioMultiEmbarcador },
            { text: "Usuário do portal do transportador", value: this.UsuarioPortalTransportador },
            { text: "App", value: this.App },
            { text: "Arquivo EDI", value: this.ArquivoEDI },
            { text: "Monitoramento Automaticamente", value: this.MonitoramentoAutomaticamente },
            { text: "WebService", value: this.WebService },
            { text: "Liberação de Canhoto", value: this.LiberacaoCanhoto },
            { text: "Correios", value: this.Correios },
            { text: "Alerta Monitoramento", value: this.Correios },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumOrigemSituacaoEntrega = Object.freeze(new EnumOrigemSituacaoEntregaHelper());
