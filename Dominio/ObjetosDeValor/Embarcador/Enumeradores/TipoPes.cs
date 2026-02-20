namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPes
    {
        NaoInformado = 0,
        vintepes = 1,
        quarentapes = 2
    }

    public static class TipoPesHelper
    {
        public static string ObterDescricao(this TipoPes tipoPes)
        {
            switch (tipoPes)
            {
                case TipoPes.NaoInformado: return "Não informado";
                case TipoPes.vintepes: return "20 Pés";
                case TipoPes.quarentapes: return "40 Pés";

                default: return string.Empty;
            }
        }
    }

}
