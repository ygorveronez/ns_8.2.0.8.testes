namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoLoteContabilizacao
    {
        EmCriacao = 1,
        AgIntegracao = 2,
        FalhaIntegracao = 3,
        Finalizado = 4
    }

    public static class SituacaoLoteContabilizacaoHelper
    {
        public static string ObterDescricao(this SituacaoLoteContabilizacao situacao)
        {
            switch (situacao)
            {
                case SituacaoLoteContabilizacao.EmCriacao: return "Em Criação";
                case SituacaoLoteContabilizacao.AgIntegracao: return "Ag. Integração";
                case SituacaoLoteContabilizacao.FalhaIntegracao: return "Falha na Integração";
                case SituacaoLoteContabilizacao.Finalizado: return "Finalizado";
                default: return "";
            }
        }
    }
}
