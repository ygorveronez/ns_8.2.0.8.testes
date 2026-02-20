namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoManobra
    {
        AguardandoManobra = 1,
        EmManobra = 2,
        Finalizada = 3,
        Reservada = 4,
        Cancelada = 5
    }

    public static class SituacaoManobraHelper
    {
        public static string ObterCorFonte(this SituacaoManobra situacao)
        {
            switch (situacao)
            {
                case SituacaoManobra.Finalizada: return "#ffffff";
                case SituacaoManobra.Cancelada: return "#333";
                default: return "#666";
            }
        }

        public static string ObterCorLinha(this SituacaoManobra situacao)
        {
            switch (situacao)
            {
                case SituacaoManobra.AguardandoManobra: return "#85de7b";
                case SituacaoManobra.Cancelada: return "#ff8080";
                case SituacaoManobra.EmManobra: return "#c8e8ff";
                case SituacaoManobra.Finalizada: return "#008e83";
                case SituacaoManobra.Reservada: return "#ffcc99";
                default: return string.Empty;
            }
        }

        public static string ObterDescricao(this SituacaoManobra situacao)
        {
            switch (situacao)
            {
                case SituacaoManobra.AguardandoManobra: return "Aguardando";
                case SituacaoManobra.Cancelada: return "Cancelada";
                case SituacaoManobra.EmManobra: return "Em Manobra";
                case SituacaoManobra.Finalizada: return "Finalizada";
                case SituacaoManobra.Reservada: return "Reservada";
                default: return string.Empty;
            }
        }
    }
}
