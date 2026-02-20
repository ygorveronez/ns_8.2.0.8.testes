namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoModalidade
    {
        Cliente = 1,
        Fornecedor = 2,
        TransportadorTerceiro = 3
    }

    public static class TipoModalidadeHelper
    {
        public static string ObterDescricao(this TipoModalidade tipoModalidade)
        {
            switch (tipoModalidade)
            {
                case TipoModalidade.Cliente: return "Cliente";
                case TipoModalidade.Fornecedor: return "Fornecedor";
                case TipoModalidade.TransportadorTerceiro: return "Transportador Terceiro";
                default: return string.Empty;
            }
        }
    }
}
