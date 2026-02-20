namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPropriedadeVeiculo
    {
        Proprio = 1,
        Terceiros = 2,
        Ambos = 3
    }

    public static class TipoPropriedadeVeiculoHelper
    {
        public static string ObterDescricao(this TipoPropriedadeVeiculo tipo)
        {
            switch (tipo)
            {
                case TipoPropriedadeVeiculo.Proprio: return "Próprio";
                case TipoPropriedadeVeiculo.Terceiros: return "Terceiro";
                case TipoPropriedadeVeiculo.Ambos: return "Ambos";
                default: return string.Empty;
            }
        }
    }
}
