namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoFilial
    {
        Filial = 1,
        Matriz = 2,
        Agencia = 3
    }

    public static class TipoFilialHelper
    {
        public static string ObterDescricao(this TipoFilial tipoFilial)
        {
            switch (tipoFilial)
            {
                case TipoFilial.Filial: return Localization.Resources.Enumeradores.TipoFilial.Filial;
                case TipoFilial.Matriz: return Localization.Resources.Enumeradores.TipoFilial.Matriz;
                case TipoFilial.Agencia: return Localization.Resources.Enumeradores.TipoFilial.Agencia;
                default: return "";
            }
        }
    }

}
