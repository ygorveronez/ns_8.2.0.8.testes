namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoFilaCarregamentoVeiculo
    {
        Reversa = 1,
        Vazio = 2
    }

    public static class TipoFilaCarregamentoVeiculoHelper
    {
        public static string ObterDescricao(this TipoFilaCarregamentoVeiculo tipoFilaCarregamento)
        {
            switch (tipoFilaCarregamento)
            {
                case TipoFilaCarregamentoVeiculo.Reversa: return "Reversa";
                case TipoFilaCarregamentoVeiculo.Vazio: return "Vazio";
                default: return string.Empty;
            }
        }
    }
}
