namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoManutencaoServicoVeiculo
    {
        Outros = 0,
        Preventiva = 1,
        Corretiva = 2,
        Preditiva = 3,
        Detectiva = 4,
        PreventivaECorretiva = 5
    }

    public static class TipoManutencaoServicoVeiculoHelper
    {
        public static string ObterDescricao(this TipoManutencaoServicoVeiculo tipo)
        {
            switch (tipo)
            {
                case TipoManutencaoServicoVeiculo.Preventiva: return "Preventiva";
                case TipoManutencaoServicoVeiculo.Corretiva: return "Corretiva";
                case TipoManutencaoServicoVeiculo.Preditiva: return "Preditiva";
                case TipoManutencaoServicoVeiculo.Detectiva: return "Detectiva";
                case TipoManutencaoServicoVeiculo.PreventivaECorretiva: return "Preventiva e Corretiva";
                default: return "Outros";
            }
        }
    }
}
