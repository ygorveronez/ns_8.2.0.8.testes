namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EstadoPneu
    {
        PneuNovo = 1,
        PneuUsado = 2,
        PneuRecauchutadoNovo = 3,
        PneuRecauchitadoUsado = 4
    }

    public static class EstadoPneuHelper
    {
        public static string ObterDescricao(this EstadoPneu estadoPneu)
        {
            switch (estadoPneu)
            {
                case EstadoPneu.PneuNovo: return "Pneu Novo";
                case EstadoPneu.PneuUsado: return "Pneu Usado";
                case EstadoPneu.PneuRecauchutadoNovo: return "Pneu Recauchutado Novo";
                case EstadoPneu.PneuRecauchitadoUsado: return "Pneu Recauchutado Usado";
                default: return string.Empty;
            }
        }
    }
}
