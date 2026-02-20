namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{

    public enum OrigemDoAceite
    {
        Fila = 1,
        OfertaCarga = 2,
        OfertaPrePlano = 3
    }


    public static class OrigemDoAceiteHelper
    {

        public static string ObterDescricao(this OrigemDoAceite origemDoAceite)
        {
            switch (origemDoAceite)
            {
                case OrigemDoAceite.Fila: return Localization.Resources.Cargas.Carga.Fila;
                case OrigemDoAceite.OfertaCarga: return Localization.Resources.Cargas.Carga.OfertaCarga;
                case OrigemDoAceite.OfertaPrePlano: return Localization.Resources.Cargas.Carga.OfertaPrePlano;
                default: return "";
            }
        }

    }
}
