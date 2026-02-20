namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum GrupoTipoIntegracao
    {
        ValePedagio = 1,
        GerenciadoraDeRisco = 2,
        Rastreadora = 3,
        OcorrenciaEntrega = 4,
        DadosCarga = 5
    }

    public static class GrupoTipoIntegracaoHelper
    {
        public static string ObterDescricao(this GrupoTipoIntegracao grupoTipoIntegracao)
        {
            switch (grupoTipoIntegracao)
            {
                case GrupoTipoIntegracao.ValePedagio: return "Vale pedágio";
                case GrupoTipoIntegracao.GerenciadoraDeRisco: return "Gerenciadora de risco";
                case GrupoTipoIntegracao.Rastreadora: return "Rastreadora";
                case GrupoTipoIntegracao.OcorrenciaEntrega: return "Ocorrência de entrega";
                case GrupoTipoIntegracao.DadosCarga: return "Dados da Carga";
                default: return string.Empty;
            }
        }
    }
}
