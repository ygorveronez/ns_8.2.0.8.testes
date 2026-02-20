namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusIntegracaoEMP
    {
        NotPersisted = 0,
        PossiblyPersisted = 1,
        Persisted = 2
    }

    public static class StatusIntegracaoEMPHelper
    {
        public static string ObterDescricao(this StatusIntegracaoEMP status)
        {
            switch (status)
            {
                case StatusIntegracaoEMP.NotPersisted: return "Not Persisted";
                case StatusIntegracaoEMP.PossiblyPersisted: return "Possibly Persisted";
                case StatusIntegracaoEMP.Persisted: return "Persisted";
                default: return string.Empty;
            }
        }
    }
}
