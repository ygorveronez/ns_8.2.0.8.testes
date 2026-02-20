namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAreaVeiculo
    {
        Doca = 1,
        Patio = 2
    }

    public static class TipoAreaVeiculoHelper
    {
        public static string Obterdescricao(this TipoAreaVeiculo tipo)
        {
            switch (tipo)
            {
                case TipoAreaVeiculo.Doca: return "Doca";
                case TipoAreaVeiculo.Patio: return "Pátio";
                default: return string.Empty;
            }
        }
    }
}
