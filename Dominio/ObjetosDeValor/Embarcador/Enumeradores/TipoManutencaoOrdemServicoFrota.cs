namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoManutencaoOrdemServicoFrota
    {
        Preventiva = 0,
        Corretiva = 1,
        PreventivaECorretiva = 2
    }

    public static class TipoManutencaoOrdemServicoFrotaHelper
    {
        public static string ObterDescricao(this TipoManutencaoOrdemServicoFrota tipo)
        {
            switch (tipo)
            {
                case TipoManutencaoOrdemServicoFrota.Preventiva: return "Preventiva";
                case TipoManutencaoOrdemServicoFrota.Corretiva: return "Corretiva";
                case TipoManutencaoOrdemServicoFrota.PreventivaECorretiva: return "Corretiva e Preventiva";
                default: return "";
            }
        }
    }
}
