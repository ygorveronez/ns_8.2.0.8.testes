namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoBuscarPracasNaGeracaoDaCarga
    {
        OrigemDestino = 1,
        Polilinhas = 2        
    }

    public static class TipoBuscarPracasNaGeracaoDaCargaHelper
    {
        public static string ObterDescricao(this TipoBuscarPracasNaGeracaoDaCarga TipoBuscarPracasNaGeracaoDaCarga)
        {
            switch (TipoBuscarPracasNaGeracaoDaCarga)
            {
                case TipoBuscarPracasNaGeracaoDaCarga.OrigemDestino: return "Origem e Destino";
                case TipoBuscarPracasNaGeracaoDaCarga.Polilinhas: return "Polilinhas";
                default: return string.Empty;
            }
        }
    }
}