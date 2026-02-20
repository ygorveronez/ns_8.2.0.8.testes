namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EtapaLote
    {
        Todas = 0,
        CriacaoLote = 1,
        AutorizacaoLote = 2,
        IntegracaoLote = 3,
        Integrado = 4,
        Finalizado = 5
    }

    public static class EtapaLoteHelper
    {
        public static string ObterDescricao(this EtapaLote etapaLote)
        {
            switch (etapaLote)
            {
                case EtapaLote.AutorizacaoLote: return "Autorização Lote";
                case EtapaLote.CriacaoLote: return "Criação Lote";
                case EtapaLote.IntegracaoLote: return "Integração Lote";
                case EtapaLote.Integrado: return "Integrado";
                case EtapaLote.Finalizado: return "Finalizado";
                default: return string.Empty;
            }
        }
    }
}
