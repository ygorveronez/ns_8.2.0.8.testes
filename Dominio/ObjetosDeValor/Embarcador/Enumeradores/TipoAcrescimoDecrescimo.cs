namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAcrescimoDecrescimo
    {
        Acrescimo = 1,
        Decrescimo = 2,
    }

    public static class TipoAcrescimoDecrescimoHelper
    {
        public static string ObterDescricao(this TipoAcrescimoDecrescimo status)
        {
            switch (status)
            {
                case TipoAcrescimoDecrescimo.Acrescimo: return "Acréscimo";
                case TipoAcrescimoDecrescimo.Decrescimo: return "Decréscimo";
                default: return string.Empty;
            }
        }
    }
}
