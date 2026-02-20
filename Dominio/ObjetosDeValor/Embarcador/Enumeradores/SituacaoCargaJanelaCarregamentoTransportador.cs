namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCargaJanelaCarregamentoTransportador
    {
        Disponivel = 0,
        ComInteresse = 1,
        AgConfirmacao = 2,
        Confirmada = 3,
        Rejeitada = 4,
        AgAceite = 5
    }

    public static class SituacaoCargaJanelaCarregamentoTransportadorHelper
    {
        public static string ObterDescricao(this SituacaoCargaJanelaCarregamentoTransportador situacao)
        {
            switch (situacao)
            {
                case SituacaoCargaJanelaCarregamentoTransportador.AgAceite: return "Aguardando Aceite";
                case SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao: return "Aguardando Confirmação";
                case SituacaoCargaJanelaCarregamentoTransportador.ComInteresse: return "Com Interesse";
                case SituacaoCargaJanelaCarregamentoTransportador.Confirmada: return "Confirmada";
                case SituacaoCargaJanelaCarregamentoTransportador.Disponivel: return "Disponível";
                case SituacaoCargaJanelaCarregamentoTransportador.Rejeitada: return "Rejeitada";
                default: return string.Empty;
            }
        }

        public static bool PermitirEnviarEmailCargaDisponibilizada(this SituacaoCargaJanelaCarregamentoTransportador situacao)
        {
            return (
                (situacao == SituacaoCargaJanelaCarregamentoTransportador.ComInteresse) ||
                (situacao == SituacaoCargaJanelaCarregamentoTransportador.Disponivel) ||
                (situacao == SituacaoCargaJanelaCarregamentoTransportador.Rejeitada)
            );
        }
    }
}
