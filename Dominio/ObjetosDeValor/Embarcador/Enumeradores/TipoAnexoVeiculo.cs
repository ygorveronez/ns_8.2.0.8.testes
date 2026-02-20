namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAnexoVeiculo
    {
        Outros = 0,
        Crlv = 1
    }
    public static class TipoAnexoVeiculoHelper
    {
        public static string ObterDescricao(this TipoAnexoVeiculo tipo)
        {
            switch (tipo)
            {
                case TipoAnexoVeiculo.Outros: return "Outros";
                case TipoAnexoVeiculo.Crlv: return "CRLV";
                default: return string.Empty;
            }
        }
    }
}
