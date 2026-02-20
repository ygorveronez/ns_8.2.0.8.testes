namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum GrupoDeResultado
    {
        Nenhum = 0,
        ReceitaOperacionalBruta = 1,
        DeducaoReceitaBruta = 2,
        CustoVenda = 3,
        DespesaOperacional = 4,
        ResultadoFinanceiro = 5,
        ResultadoNaoOperacional = 6,
        IrpjCsll = 7,
        Investimento = 8
    }

    public static class GrupoDeResultadoHelper
    {
        public static string ObterDescricao(this GrupoDeResultado grupoDeResultado)
        {
            switch (grupoDeResultado)
            {
                case GrupoDeResultado.Nenhum: return Localization.Resources.Enumeradores.GrupoResultado.Nenhum;
                case GrupoDeResultado.ReceitaOperacionalBruta: return Localization.Resources.Enumeradores.GrupoResultado.ReceitaOperacionalBruta;
                case GrupoDeResultado.DeducaoReceitaBruta: return Localization.Resources.Enumeradores.GrupoResultado.DeducaoReceitaBruta;
                case GrupoDeResultado.CustoVenda: return Localization.Resources.Enumeradores.GrupoResultado.CustoVenda;
                case GrupoDeResultado.DespesaOperacional: return Localization.Resources.Enumeradores.GrupoResultado.DespesaOperacional;
                case GrupoDeResultado.ResultadoFinanceiro: return Localization.Resources.Enumeradores.GrupoResultado.ResultadoFinanceiro;
                case GrupoDeResultado.ResultadoNaoOperacional: return Localization.Resources.Enumeradores.GrupoResultado.ResultadoNaoOperacional;
                case GrupoDeResultado.IrpjCsll: return Localization.Resources.Enumeradores.GrupoResultado.IrpjCsll;
                case GrupoDeResultado.Investimento: return Localization.Resources.Enumeradores.GrupoResultado.Investimento;
                default: return string.Empty;
            }
        }
    }
}