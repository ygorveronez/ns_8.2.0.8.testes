namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCargaJanelaDescarregamentoCadastrada
    {
        PendenteColeta = 1,
        Programado = 2,
        AguardandoCarregamento = 3,
        EmTransito = 4,
        AguardandoDescarga = 5,
        AguardandoVeiculoEncostar = 6,
        EmDescarga = 7,
        DescarregamentoFinalizado = 8,
        BloqueadaParaDescarga = 9
    }

    public static class SituacaoCargaJanelaDescarregamentoSituacaoHelper
    {
        public static string ObterDescricao(this SituacaoCargaJanelaDescarregamentoCadastrada situacao)
        {
            switch (situacao)
            {
                case SituacaoCargaJanelaDescarregamentoCadastrada.PendenteColeta: return "Pendente de Coleta";
                case SituacaoCargaJanelaDescarregamentoCadastrada.Programado: return "Programado";
                case SituacaoCargaJanelaDescarregamentoCadastrada.AguardandoCarregamento: return "Aguardando Carregamento";
                case SituacaoCargaJanelaDescarregamentoCadastrada.EmTransito: return "Em Trânsito";
                case SituacaoCargaJanelaDescarregamentoCadastrada.AguardandoDescarga: return "Aguardando Descarga";
                case SituacaoCargaJanelaDescarregamentoCadastrada.AguardandoVeiculoEncostar: return "Aguardando Veíulo Encostar";
                case SituacaoCargaJanelaDescarregamentoCadastrada.EmDescarga: return "Em Descarga";
                case SituacaoCargaJanelaDescarregamentoCadastrada.DescarregamentoFinalizado: return "Descarregamento Finalizado";
                case SituacaoCargaJanelaDescarregamentoCadastrada.BloqueadaParaDescarga: return "Bloqueada para Descarga";
                default: return string.Empty;
            }
        }
    }
}