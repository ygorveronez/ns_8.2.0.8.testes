namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoJanelaDescarregamento
    {
        AguardandoCarregamento = 1,
        EmViagem = 2,
        AgParaDescarregar = 3,
        EmDescarregamento = 4,
        Liberada = 5
    }

    public static class SituacaoJanelaDescarregamentoHelper
    {
        public static string ObterCor(this SituacaoJanelaDescarregamento situacao)
        {
            switch (situacao)
            {
                case SituacaoJanelaDescarregamento.AgParaDescarregar: return CorGrid.Danger;
                case SituacaoJanelaDescarregamento.Liberada: return CorGrid.Success;
                case SituacaoJanelaDescarregamento.EmDescarregamento: return CorGrid.Warning;
                default: return string.Empty;
            }
        }

        public static string ObterDescricao(this SituacaoJanelaDescarregamento situacao)
        {
            switch (situacao)
            {
                case SituacaoJanelaDescarregamento.AgParaDescarregar: return "Aguardando para Descarregar";
                case SituacaoJanelaDescarregamento.AguardandoCarregamento: return "Aguardando Carregamento";
                case SituacaoJanelaDescarregamento.Liberada: return "Liberada";
                case SituacaoJanelaDescarregamento.EmDescarregamento: return "Em Descarregamento";
                case SituacaoJanelaDescarregamento.EmViagem: return "Em Viagem";
                default: return string.Empty;
            }
        }
    }
}
