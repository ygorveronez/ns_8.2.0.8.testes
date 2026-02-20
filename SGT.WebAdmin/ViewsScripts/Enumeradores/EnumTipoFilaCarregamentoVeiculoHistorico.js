var EnumTipoFilaCarregamentoVeiculoHistoricoHelper = function () {
    this.Todos = "";
    this.EntradaFila = 1;
    this.SaidaReversa = 2;
    this.CargaAlocada = 3;
    this.CargaAceita = 4;
    this.CargaRecusada = 5;
    this.SenhaPerdida = 6;
    this.PosicaoAlterada = 7;
    this.SaidaFila = 8;
    this.CargaCancelada = 9;
    this.MotoristaLiberado = 10;
    this.ConjuntoMotoristaAlterado = 11;
    this.ConjuntoVeiculoAlterado = 12;
    this.SolicitacaoSaidaFila = 13;
    this.SolicitacaoSaidaFilaRecusada = 14;
    this.CargaAceitaPortal = 15;
    this.CargaRecusadaPortal = 16;
    this.VeiculoLiberado = 17;
    this.PreCargaAlocada = 18;
    this.PreCargaDesalocada = 19;
    this.ChegadaVeiculoConfirmada = 20;
    this.ChecklistConcluido = 21;
    this.CargaAdicionada = 22;
    this.CargaRemovida = 23;
    this.VeiculoAtrelado = 24;
    this.VeiculoDesatrelado = 25;
    this.PreCargaAceita = 26;
    this.PreCargaRecusada = 27;
    this.DataProgramadaAlterada = 28;
}

EnumTipoFilaCarregamentoVeiculoHistoricoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Carga Aceita", value: this.CargaAceita },
            { text: "Carga Aceita pelo Portal", value: this.CargaAceitaPortal },
            { text: "Carga Alocada", value: this.CargaAlocada },
            { text: "Carga Cancelada", value: this.CargaCancelada },
            { text: "Carga Recusada", value: this.CargaRecusada },
            { text: "Carga Recusada pelo Portal", value: this.CargaRecusadaPortal },
            { text: "Checklist Concluído", value: this.ChecklistConcluido },
            { text: "Chegada de Veículo Corfirmada", value: this.ChegadaVeiculoConfirmada },
            { text: "Conjunto de Motorista Alterado", value: this.ConjuntoMotoristaAlterado },
            { text: "Conjunto de Veiculo Alterado", value: this.ConjuntoVeiculoAlterado },
            { text: "Entrada na Fila", value: this.EntradaFila },
            { text: "Motorista Liberado", value: this.MotoristaLiberado },
            { text: "Posição Alterada", value: this.PosicaoAlterada },
            { text: "Pré Planejamento Alocado", value: this.PreCargaAlocada },
            { text: "Pré Planejamento Desalocado", value: this.PreCargaDesalocada },
            { text: "Saída da Fila", value: this.SaidaFila },
            { text: "Saída da Reversa", value: this.SaidaReversa },
            { text: "Senha Perdida", value: this.SenhaPerdida },
            { text: "Solicitação de Saída da Fila", value: this.SolicitacaoSaidaFila },
            { text: "Solicitação de Saída da Fila Recusada", value: this.SolicitacaoSaidaFilaRecusada },
            { text: "Veículo Liberado", value: this.VeiculoLiberado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoFilaCarregamentoVeiculoHistorico = Object.freeze(new EnumTipoFilaCarregamentoVeiculoHistoricoHelper());