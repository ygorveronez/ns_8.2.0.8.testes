namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusPrazoEntrega
    {
        NoPrazo = 0,
        Antecipado = 1,
        Atrasado = 2
    }

    public static class StatusPrazoEntregaHelper
    {
        public static string ObterDescricao(this StatusPrazoEntrega status)
        {
            switch (status)
            {
                case StatusPrazoEntrega.NoPrazo: return "No Prazo";
                case StatusPrazoEntrega.Antecipado: return "Antecipado";
                case StatusPrazoEntrega.Atrasado: return "Atrasado";


                default: return string.Empty;
            }
        }
    }
}
