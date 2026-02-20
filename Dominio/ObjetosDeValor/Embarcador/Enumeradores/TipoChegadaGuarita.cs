namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoChegadaGuarita
    {
        Carregamento = 1,
        Descarregamento = 2
    }

    public static class TipoChegadaGuaritaHelper
    {
        public static string ObterDescricao(this TipoChegadaGuarita tipo)
        {
            switch (tipo)
            {
                case TipoChegadaGuarita.Carregamento: return "Carregamento";
                case TipoChegadaGuarita.Descarregamento: return "Descarregamento";
                default: return string.Empty;
            }
        }
    }
}
