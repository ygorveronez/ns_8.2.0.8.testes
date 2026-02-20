namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoGrupoCarga
    {
        Nenhum = 0,
        Inbound = 1,
        Outbound = 2
    }

    public static class TipoGrupoCargaHelper
    {
        public static string ObterDescricao(this TipoGrupoCarga tipo)
        {
            switch (tipo)
            {
                case TipoGrupoCarga.Nenhum: return "Nenhum";
                case TipoGrupoCarga.Inbound: return "Inbound";
                case TipoGrupoCarga.Outbound: return "Outbound";
                default: return string.Empty;
            }
        }
    }
}
