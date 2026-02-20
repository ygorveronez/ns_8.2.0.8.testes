namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum MotivoServicoVeiculo
    {
        Outros = 0,
        Conserto = 1,
        Reforma = 2,
        ConsertoEReforma = 3
    }

    public static class MotivoServicoVeiculoHelper
    {
        public static string ObterDescricao(this MotivoServicoVeiculo motivo)
        {
            switch (motivo)
            {
                case MotivoServicoVeiculo.Outros: return "Outros";
                case MotivoServicoVeiculo.Conserto: return "Conserto";
                case MotivoServicoVeiculo.Reforma: return "Reforma";
                case MotivoServicoVeiculo.ConsertoEReforma: return "Conserto + Reforma";
                default: return string.Empty;
            }
        }
    }
}
