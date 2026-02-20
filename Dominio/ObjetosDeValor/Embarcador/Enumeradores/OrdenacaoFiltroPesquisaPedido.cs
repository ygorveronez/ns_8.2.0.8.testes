namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum OrdenacaoFiltroPesquisaPedido
    {
        Padrao = 0,
        Remetente = 1,
        Destinatario = 2
    }

    public static class OrdenacaoFiltroPesquisaPedidoHelper
    {
        public static string ObterDescricao(this OrdenacaoFiltroPesquisaPedido o)
        {
            switch (o)
            {
                case OrdenacaoFiltroPesquisaPedido.Padrao: return "Padrão";
                case OrdenacaoFiltroPesquisaPedido.Remetente: return "Remetente";
                case OrdenacaoFiltroPesquisaPedido.Destinatario: return "Destinatário";
                default: return string.Empty;
            }
        }
    }
}
