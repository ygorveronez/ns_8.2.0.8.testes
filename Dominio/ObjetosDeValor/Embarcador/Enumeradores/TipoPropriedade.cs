namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPropriedade
    {
        FrotaPropria = 1,
        Terceiros = 2,
        Agregado = 3
    }

    public static class TipoPropriedadeHelper
    {
        public static string ObterDescricao(this TipoPropriedade situacao)
        {
            switch (situacao)
            {
                case TipoPropriedade.FrotaPropria: return "Frota Própria";
                case TipoPropriedade.Terceiros: return "Terceiros";
                case TipoPropriedade.Agregado: return "Agregados";
                default: return null;
            }
        }
    }
}
