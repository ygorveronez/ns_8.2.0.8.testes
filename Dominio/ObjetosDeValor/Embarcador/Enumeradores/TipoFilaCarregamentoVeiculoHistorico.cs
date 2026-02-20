namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoFilaCarregamentoVeiculoHistorico
    {
        EntradaFila = 1,
        SaidaReversa = 2,
        CargaAlocada = 3,
        CargaAceita = 4,
        CargaRecusada = 5,
        SenhaPerdida = 6,
        PosicaoAlterada = 7,
        SaidaFila = 8,
        CargaCancelada = 9,
        MotoristaLiberado = 10,
        ConjuntoMotoristaAlterado = 11,
        ConjuntoVeiculoAlterado = 12,
        SolicitacaoSaidaFila = 13,
        SolicitacaoSaidaFilaRecusada = 14,
        CargaAceitaPortal = 15,
        CargaRecusadaPortal = 16,
        VeiculoLiberado = 17,
        PreCargaAlocada = 18,
        PreCargaDesalocada = 19,
        ChegadaVeiculoConfirmada = 20,
        ChecklistConcluido = 21,
        CargaAdicionada = 22,
        CargaRemovida = 23,
        VeiculoAtrelado = 24,
        VeiculoDesatrelado = 25,
        PreCargaAceita = 26,
        PreCargaRecusada = 27,
        DataProgramadaAlterada = 28
    }

    public static class TipoFilaCarregamentoVeiculoHistoricoHelper
    {
        public static string ObterDescricao(this TipoFilaCarregamentoVeiculoHistorico tipo)
        {
            switch (tipo)
            {
                case TipoFilaCarregamentoVeiculoHistorico.CargaAceita: return "Carga Aceita";
                case TipoFilaCarregamentoVeiculoHistorico.CargaAceitaPortal: return "Carga Aceita pelo Portal";
                case TipoFilaCarregamentoVeiculoHistorico.CargaAlocada: return "Carga Alocada";
                case TipoFilaCarregamentoVeiculoHistorico.CargaCancelada: return "Carga Cancelada";
                case TipoFilaCarregamentoVeiculoHistorico.CargaAdicionada: return "Carga Adicionada";
                case TipoFilaCarregamentoVeiculoHistorico.CargaRecusada: return "Carga Recusada";
                case TipoFilaCarregamentoVeiculoHistorico.CargaRecusadaPortal: return "Carga Recusada pelo Portal";
                case TipoFilaCarregamentoVeiculoHistorico.CargaRemovida: return "Carga Removida";
                case TipoFilaCarregamentoVeiculoHistorico.ChecklistConcluido: return "Checklist Concluído";
                case TipoFilaCarregamentoVeiculoHistorico.ChegadaVeiculoConfirmada: return "Chegada de Veículo Confirmada";
                case TipoFilaCarregamentoVeiculoHistorico.ConjuntoMotoristaAlterado: return "Conjunto de Motorista Alterado";
                case TipoFilaCarregamentoVeiculoHistorico.ConjuntoVeiculoAlterado: return "Conjunto de Veiculo Alterado";
                case TipoFilaCarregamentoVeiculoHistorico.DataProgramadaAlterada: return "Data de Previsão de Chegada Alterada";
                case TipoFilaCarregamentoVeiculoHistorico.EntradaFila: return "Entrada na Fila";
                case TipoFilaCarregamentoVeiculoHistorico.MotoristaLiberado: return "Motorista Liberado";
                case TipoFilaCarregamentoVeiculoHistorico.PosicaoAlterada: return "Posição Alterada";
                case TipoFilaCarregamentoVeiculoHistorico.PreCargaAceita: return "Pré Planejamento Aceito";
                case TipoFilaCarregamentoVeiculoHistorico.PreCargaAlocada: return "Pré Planejamento Alocado";
                case TipoFilaCarregamentoVeiculoHistorico.PreCargaDesalocada: return "Pré Planejamento Desalocado";
                case TipoFilaCarregamentoVeiculoHistorico.PreCargaRecusada: return "Pré Planejamento Recusado";
                case TipoFilaCarregamentoVeiculoHistorico.SaidaFila: return "Saída da Fila";
                case TipoFilaCarregamentoVeiculoHistorico.SaidaReversa: return "Saída da Reversa";
                case TipoFilaCarregamentoVeiculoHistorico.SenhaPerdida: return "Senha Perdida";
                case TipoFilaCarregamentoVeiculoHistorico.SolicitacaoSaidaFila: return "Solicitação de Saída da Fila";
                case TipoFilaCarregamentoVeiculoHistorico.SolicitacaoSaidaFilaRecusada: return "Solicitação de Saída da Fila Recusada";
                case TipoFilaCarregamentoVeiculoHistorico.VeiculoAtrelado: return "Veículo Atrelado";
                case TipoFilaCarregamentoVeiculoHistorico.VeiculoDesatrelado: return "Veículo Desatrelado";
                case TipoFilaCarregamentoVeiculoHistorico.VeiculoLiberado: return "Veículo Liberado";
                default: return string.Empty;
            }
        }
    }
}
