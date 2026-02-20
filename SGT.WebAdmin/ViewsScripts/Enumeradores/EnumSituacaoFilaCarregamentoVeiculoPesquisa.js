var EnumSituacaoFilaCarregamentoVeiculoPesquisaHelper = function () {
    this.Todas = "";
    this.AguardandoAceite = 1;
    this.AguardandoCarga = 2;
    this.AguardandoConfirmacao = 3;
    this.AguardandoConjuntos = 4;
    this.AguardandoDesatrelar = 5;
    this.CargaCancelada = 6;
    this.CargaRecusada = 7;
    this.EmChecklist = 8;
    this.EmRemocao = 9;
    this.EmReversa = 10;
    this.PerdeuSenha = 11;
    this.Vazio = 12;
    this.EmViagem = 13;
    this.AguardandoAceitePreCarga = 14;
}

EnumSituacaoFilaCarregamentoVeiculoPesquisaHelper.prototype = {
    obterOpcoesPesquisa: function () {
        return [
            { text: "Ag. Aceite Pré Carga", value: this.AguardandoAceite },
            { text: "Ag. Aceite Pré Planejamento", value: this.AguardandoAceitePreCarga },
            { text: "Ag. Conjuntos", value: this.AguardandoConjuntos },
            { text: "Ag. Pré Carga", value: this.AguardandoCarga },
            { text: "Ag. Pré Planejamento", value: this.Vazio },
            { text: "Pré Carga Vinculada", value: this.EmViagem },
            { text: "Pré Carga Cancelada", value: this.CargaCancelada }
        ];
    }
}

var EnumSituacaoFilaCarregamentoVeiculoPesquisa = Object.freeze(new EnumSituacaoFilaCarregamentoVeiculoPesquisaHelper());
