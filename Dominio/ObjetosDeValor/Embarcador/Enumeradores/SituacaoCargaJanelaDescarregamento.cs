namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCargaJanelaDescarregamento
    {
        AguardandoConfirmacaoAgendamento = 1,
        AguardandoDescarregamento = 2,
        DescarregamentoFinalizado = 3,
        NaoComparecimento = 4,
        CargaDevolvida = 5,
        NaoComparecimentoConfirmadoPeloFornecedor = 6,
        AguardandoGeracaoSenha = 7,
        CargaDevolvidaParcialmente = 8,
        ChegadaConfirmada = 9,
        SaidaVeiculoConfirmada = 10,
        EntregaParcialmente = 11,
        Cancelado = 12,
        ValidacaoFiscal = 13,
        Nucleo = 14
    }

    public static class SituacaCargaJanelaDescarregamentoHelper
    {
        public static string ObterCorLinha(this SituacaoCargaJanelaDescarregamento situacao)
        {
            switch (situacao)
            {
                case SituacaoCargaJanelaDescarregamento.AguardandoConfirmacaoAgendamento: return "#e29e23";
                case SituacaoCargaJanelaDescarregamento.AguardandoDescarregamento: return "#c8e8ff";
                case SituacaoCargaJanelaDescarregamento.DescarregamentoFinalizado: return "#85de7b";
                case SituacaoCargaJanelaDescarregamento.NaoComparecimento: return "#e8d8ed";
                case SituacaoCargaJanelaDescarregamento.NaoComparecimentoConfirmadoPeloFornecedor: return "#e8d8ed";
                case SituacaoCargaJanelaDescarregamento.CargaDevolvida: return "#ffcccc";
                case SituacaoCargaJanelaDescarregamento.AguardandoGeracaoSenha: return "#7cbdb7";
                case SituacaoCargaJanelaDescarregamento.ChegadaConfirmada: return "#6ff199";
                case SituacaoCargaJanelaDescarregamento.SaidaVeiculoConfirmada: return "#00ffff";
                case SituacaoCargaJanelaDescarregamento.Cancelado: return "#e7c1c1";
                case SituacaoCargaJanelaDescarregamento.ValidacaoFiscal: return "#e6c972";
                case SituacaoCargaJanelaDescarregamento.Nucleo: return "#d3c0f9";
                default: return string.Empty;
            }
        }

        public static string ObterDescricao(this SituacaoCargaJanelaDescarregamento situacao)
        {
            switch (situacao)
            {
                case SituacaoCargaJanelaDescarregamento.AguardandoConfirmacaoAgendamento: return "Ag. Confirmação de Agendamento";
                case SituacaoCargaJanelaDescarregamento.AguardandoDescarregamento: return "Ag. Descarregamento";
                case SituacaoCargaJanelaDescarregamento.DescarregamentoFinalizado: return "Descarregamento Finalizado";
                case SituacaoCargaJanelaDescarregamento.NaoComparecimento: return "Não Comparecimento";
                case SituacaoCargaJanelaDescarregamento.CargaDevolvida: return "Carga Devolvida";
                case SituacaoCargaJanelaDescarregamento.NaoComparecimentoConfirmadoPeloFornecedor: return "Não Comparecimento (Confirmado pelo Fornecedor)";
                case SituacaoCargaJanelaDescarregamento.AguardandoGeracaoSenha: return "Aguardando Geração Senha";
                case SituacaoCargaJanelaDescarregamento.CargaDevolvidaParcialmente: return "Carga devolvida parcialmente";
                case SituacaoCargaJanelaDescarregamento.ChegadaConfirmada: return "Chegada Confirmada";
                case SituacaoCargaJanelaDescarregamento.SaidaVeiculoConfirmada: return "Saída do Veículo Confirmada";
                case SituacaoCargaJanelaDescarregamento.EntregaParcialmente: return "Entrega Parcialmente";
                case SituacaoCargaJanelaDescarregamento.Cancelado: return "Cancelado";
                case SituacaoCargaJanelaDescarregamento.ValidacaoFiscal: return "Validação Fiscal";
                case SituacaoCargaJanelaDescarregamento.Nucleo: return "Validação Núcleo";
                default: return string.Empty;
            }
        }
    }
}
