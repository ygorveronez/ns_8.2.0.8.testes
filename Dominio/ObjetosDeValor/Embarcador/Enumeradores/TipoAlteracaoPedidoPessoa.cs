namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAlteracaoPedidoPessoa
    {
        Destinatario = 1,
        Recebedor = 2,
        Remetente = 3
    }

    public static class TipoAlteracaoPedidoPessoaHelper
    {
        public static string ObterDescricao(this TipoAlteracaoPedidoPessoa tipoPessoa)
        {
            switch (tipoPessoa)
            {
                case TipoAlteracaoPedidoPessoa.Destinatario: return "Destinatário";
                case TipoAlteracaoPedidoPessoa.Recebedor: return "Recebedor";
                case TipoAlteracaoPedidoPessoa.Remetente: return "Remetente";
                default: return string.Empty;
            }
        }
    }
}
