namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoLocalArmazanamentoProdutoTransferencia
    {
        AgTransferencia = 1,
        Transferido = 2,
        Cancelado = 3,
        ProblemaTransferencia = 4,

    }

    public static class SituacaoLocalArmazanamentoProdutoTransferenciaHelper
    {
        public static string ObterDescricao(this SituacaoLocalArmazanamentoProdutoTransferencia situacaoLocalArmazanagemProduto)
        {
            switch (situacaoLocalArmazanagemProduto)
            {
                case SituacaoLocalArmazanamentoProdutoTransferencia.AgTransferencia: return "Ag. Transferência";
                case SituacaoLocalArmazanamentoProdutoTransferencia.Transferido: return "Transferido";
                case SituacaoLocalArmazanamentoProdutoTransferencia.Cancelado: return "Cancelado";
                case SituacaoLocalArmazanamentoProdutoTransferencia.ProblemaTransferencia: return "Problema Transferência";
                default: return string.Empty;
            }
        }
    }
}
