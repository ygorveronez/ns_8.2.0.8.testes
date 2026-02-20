namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum BloqueioDocumentoEntrada
    {
        SemBloqueio = 0,
        SemOrdemServico = 1,
        SemOrdemCompra = 2,
        SemOrdemServicoESemOrdemCompra = 3,
        SemOrdemServicoOuSemOrdemCompra = 4
    }

    public static class BloqueioDocumentoEntradaHelper
    {
        public static string ObterDescricao(this BloqueioDocumentoEntrada bloqueioDocumentoEntrada)
        {
            switch (bloqueioDocumentoEntrada)
            {
                case BloqueioDocumentoEntrada.SemBloqueio: return "Sem Bloqueio";
                case BloqueioDocumentoEntrada.SemOrdemServico: return "Sem Ordem Serviço";
                case BloqueioDocumentoEntrada.SemOrdemCompra: return "Sem Ordem Compra";
                case BloqueioDocumentoEntrada.SemOrdemServicoESemOrdemCompra: return "Sem Ordem Serviço e sem Ordem Compra";
                case BloqueioDocumentoEntrada.SemOrdemServicoOuSemOrdemCompra: return "Sem Ordem Serviço ou sem Ordem Compra";
                default: return string.Empty;
            }

        }
    }
}
