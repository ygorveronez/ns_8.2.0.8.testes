namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoColetaEntregaDevolucao
    {
        Total = 0,
        Parcial = 1
    }

    public static class TipoColetaEntregaDevolucaoHelper
    {
        public static string ObterDescricao(this TipoColetaEntregaDevolucao tipo)
        {
            switch (tipo)
            {
                case TipoColetaEntregaDevolucao.Parcial: return "Parcial";
                default: return "Total";
            }
        }
    }
}
