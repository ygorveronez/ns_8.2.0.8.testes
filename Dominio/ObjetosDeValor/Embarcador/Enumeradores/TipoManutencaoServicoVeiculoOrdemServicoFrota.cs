namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoManutencaoServicoVeiculoOrdemServicoFrota
    {
        Preventiva = 0,
        Corretiva = 1
    }

    public static class TipoManutencaoServicoVeiculoOrdemServicoFrotaHelper
    {
        public static string ObterDescricao(this TipoManutencaoServicoVeiculoOrdemServicoFrota tipo)
        {
            switch (tipo)
            {
                case TipoManutencaoServicoVeiculoOrdemServicoFrota.Preventiva: return "Preventiva";
                case TipoManutencaoServicoVeiculoOrdemServicoFrota.Corretiva: return "Corretiva";
                default: return string.Empty;
            }
        }
    }
}
