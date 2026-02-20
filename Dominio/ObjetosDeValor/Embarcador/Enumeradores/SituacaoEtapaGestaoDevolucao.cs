namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoEtapaGestaoDevolucao
    {
        NaoIniciada = 0,
        Finalizada = 1,
        Rejeitada = 2,
        EmAndamento = 3
    }

    public static class SituacaoEtapaGestaoDevolucaoHelper
    {
        public static string ObterDescricao(this SituacaoEtapaGestaoDevolucao situacaoEtapaGestaoDevolucao)
        {
            switch (situacaoEtapaGestaoDevolucao)
            {
                case SituacaoEtapaGestaoDevolucao.Finalizada: return "Finalizada";
                case SituacaoEtapaGestaoDevolucao.Rejeitada: return "Rejeitada";
                case SituacaoEtapaGestaoDevolucao.EmAndamento: return "Em Andamento";
                default: return string.Empty;
            }
        }
    }
}