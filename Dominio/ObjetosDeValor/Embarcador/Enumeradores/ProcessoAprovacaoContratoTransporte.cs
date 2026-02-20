namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ProcessoAprovacaoContratoTransporte
    {
        AprovacaoDiretaVisivelFornecedor = 0,
        AprovacaoDiretaNaoVisivelFornecedor = 1,
    }

    public static class ProcessoAprovaçãoHelper
    {
        public static string ObterDescricao(this ProcessoAprovacaoContratoTransporte processoAprovação)
        {
            switch (processoAprovação)
            {
                case ProcessoAprovacaoContratoTransporte.AprovacaoDiretaVisivelFornecedor: return "Aprovação Direta - Visível ao fornecedor";
                case ProcessoAprovacaoContratoTransporte.AprovacaoDiretaNaoVisivelFornecedor: return "Aprovação Direta - Não visível ao fornecedor";
                default: return string.Empty;
            }
        }
    }
}