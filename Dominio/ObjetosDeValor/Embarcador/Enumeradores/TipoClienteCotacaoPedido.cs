namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoClienteCotacaoPedido
    {
        Todos = 0,
        ClienteNovo = 1,
        ClienteProspect = 2,
        ClienteAtivo = 3,
        ClienteInativo = 4,
        GrupoPessoa = 5
    }

    public static class TipoClienteCotacaoPedidoHelper
    {
        public static string Descricao(this TipoClienteCotacaoPedido tipoClienteCotacaoPedido)
        {
            switch (tipoClienteCotacaoPedido)
            {
                case TipoClienteCotacaoPedido.Todos:
                    return "Todos";
                case TipoClienteCotacaoPedido.ClienteNovo:
                    return "Cliente novo";
                case TipoClienteCotacaoPedido.ClienteProspect:
                    return "Cliente prospect";
                case TipoClienteCotacaoPedido.ClienteAtivo:
                    return "Cliente ativo";
                case TipoClienteCotacaoPedido.ClienteInativo:
                    return "Cliente inativo";
                case TipoClienteCotacaoPedido.GrupoPessoa:
                    return "Grupo de pessoa";
                default:
                    return string.Empty;
            }
        }
    }
}
