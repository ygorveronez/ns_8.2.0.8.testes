namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoDataFaturamentoMensal
    {
        DataEmissao = 1,
        DataVencimento = 2,
        DataQuitacao = 3
    }

    public static class TipoDataFaturamentoMensalHelper
    {
        public static string ObterDescricao(this TipoDataFaturamentoMensal tipo)
        {
            switch (tipo)
            {
                case TipoDataFaturamentoMensal.DataEmissao: return "Data Emissão";
                case TipoDataFaturamentoMensal.DataVencimento: return "Data Vencimento";
                case TipoDataFaturamentoMensal.DataQuitacao: return "Data Quitação";
                default: return string.Empty;
            }
        }
    }
}
