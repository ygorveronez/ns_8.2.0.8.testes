namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum PadraoEixosVeiculo
    {
        Simples = 1,
        Duplo = 2,
    }

    public static class PadraoEixosVeiculoHelper
    {
        public static string ObterDescricao(this PadraoEixosVeiculo tipo)
        {
            switch (tipo)
            {
                case PadraoEixosVeiculo.Simples: return "Simples";
                case PadraoEixosVeiculo.Duplo: return "Duplo";
                default: return string.Empty;
            }
        }
    }
}
