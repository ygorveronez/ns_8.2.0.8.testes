namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoTrajeto
    {
        Origem = 1,
        TransitoOrigem = 2,
        TransitoDestino = 3,
        Destino = 4
    }

    public static class SituacaoTrajetoHelper
    {
        public static string ObterDescricao(this SituacaoTrajeto situacao)
        {
            switch (situacao)
            {
                case SituacaoTrajeto.Origem: return "Na Origem";
                case SituacaoTrajeto.TransitoOrigem: return "Em Trânsito da Origem";
                case SituacaoTrajeto.TransitoDestino: return "Em Trânsito Destino";
                case SituacaoTrajeto.Destino: return "No Destino";
                default: return string.Empty;
            }
        }
    }
}
