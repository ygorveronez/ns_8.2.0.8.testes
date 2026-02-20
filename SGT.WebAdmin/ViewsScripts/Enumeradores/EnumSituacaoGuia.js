var EnumSituacaoGuiaHelper = function () {
    this.NaoEmitido = 0;
    this.AguardandoEnvio = 1;
    this.AguardandoRetorno = 2;
    this.Gerada = 3;
    this.Cancelada = 4;    
}

EnumSituacaoGuiaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não Emitido", value: this.NaoEmitido },
            { text: "Aguardando Envio", value: this.AguardandoEnvio },
            { text: "Aguardando Retorno", value: this.AguardandoRetorno },
            { text: "Gerada", value: this.Gerada },
            { text: "Cancelada", value: this.Cancelada }            
        ];
    }
}

var EnumSituacaoGuia = Object.freeze(new EnumSituacaoGuiaHelper());