namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoNotaFiscal
    {
        Nenhum = 0,
        Entregue = 1,
        EntregueParcial = 2, // Não utilizar
        Devolvida = 3,
        DevolvidaParcial = 4,
        AgReentrega = 5,
        AgEntrega = 6,
        NaoEntregue = 7
    }

    public static class SituacaoNotaFiscalHelper
    {
        public static string ObterDescricao(this SituacaoNotaFiscal tipo)
        {
            switch (tipo)
            {
                case SituacaoNotaFiscal.Nenhum: return "Nenhum";
                case SituacaoNotaFiscal.Entregue: return "Entregue";
                case SituacaoNotaFiscal.EntregueParcial: return "Entregue Parcial";
                case SituacaoNotaFiscal.Devolvida: return "Devolvida";
                case SituacaoNotaFiscal.DevolvidaParcial: return "Devolvida Parcial";
                case SituacaoNotaFiscal.AgReentrega: return "Aguardando Reentrega";
                case SituacaoNotaFiscal.AgEntrega: return "Aguardando Entrega";
                case SituacaoNotaFiscal.NaoEntregue: return "Não Entregue";
                default: return string.Empty;
            }
        }
    }
}
