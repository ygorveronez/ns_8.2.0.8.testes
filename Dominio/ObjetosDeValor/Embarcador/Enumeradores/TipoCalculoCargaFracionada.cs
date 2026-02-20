namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCalculoCargaFracionada
    {
        Nenhum = 0,
        Tonelada = 1,
        MetroCubito = 2
    }

    public static class TipoCalculoCargaFracionadaHelper
    {
        public static string ObterDescricao(this TipoCalculoCargaFracionada tipo)
        {
            switch (tipo)
            {
                case TipoCalculoCargaFracionada.Nenhum: return "Nenhum";
                case TipoCalculoCargaFracionada.Tonelada: return "Tonelada";
                case TipoCalculoCargaFracionada.MetroCubito: return "Metro Cúbico";
                default: return string.Empty;
            }
        }
    }
}
