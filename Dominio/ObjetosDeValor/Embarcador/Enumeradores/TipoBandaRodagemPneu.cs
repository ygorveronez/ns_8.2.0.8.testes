namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoBandaRodagemPneu
    {
        Borrachudo = 1,
        Liso = 2,
        Macico = 3,
        Misto = 4,
        Outros = 5
    }

    public static class TipoBandaRodagemPneuHelper
    {
        public static string ObterDescricao(this TipoBandaRodagemPneu tipo)
        {
            switch (tipo)
            {
                case TipoBandaRodagemPneu.Borrachudo: return "Borrachudo";
                case TipoBandaRodagemPneu.Liso: return "Liso";
                case TipoBandaRodagemPneu.Macico: return "Maciço";
                case TipoBandaRodagemPneu.Misto: return "Misto";
                case TipoBandaRodagemPneu.Outros: return "Outros";
                default: return string.Empty;
            }
        }
    }
}
