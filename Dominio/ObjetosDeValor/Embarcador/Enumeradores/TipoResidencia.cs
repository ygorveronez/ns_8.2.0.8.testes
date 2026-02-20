namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoResidencia
    {
        Nenhum = 0,
        Proprio = 1,
        ProprioFinanciado = 2,
        Alugado = 3,
        Familiar = 4,
        Cedido = 5
    }

    public static class TipoResidenciaHelper
    {
        public static string ObterDescricao(this TipoResidencia TipoResidencia)
        {
            switch (TipoResidencia)
            {
                case TipoResidencia.Nenhum: return Localization.Resources.Enumeradores.TipoResidencia.Nenhum;
                case TipoResidencia.Proprio: return Localization.Resources.Enumeradores.TipoResidencia.Proprio;
                case TipoResidencia.ProprioFinanciado: return Localization.Resources.Enumeradores.TipoResidencia.ProprioFinanciado;
                case TipoResidencia.Alugado: return Localization.Resources.Enumeradores.TipoResidencia.Alugado;
                case TipoResidencia.Familiar: return Localization.Resources.Enumeradores.TipoResidencia.Familiar;
                case TipoResidencia.Cedido: return Localization.Resources.Enumeradores.TipoResidencia.Cedido;

                default: return string.Empty;
            }
        }
    }
}
