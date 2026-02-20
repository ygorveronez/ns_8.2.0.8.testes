namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoQuitacaoCIOT
    {
        Padrao = 0,
        QualquerLugar = 1,
        ETF = 2,
        Filial = 3,
        Transportadora = 4
    }

    public static class TipoQuitacaoCIOTHelper
    {
        public static string ObterDescricao(this TipoQuitacaoCIOT status)
        {
            switch (status)
            {
                case TipoQuitacaoCIOT.Padrao: return "Padrão";
                case TipoQuitacaoCIOT.QualquerLugar: return "Qualquer Lugar";
                case TipoQuitacaoCIOT.ETF: return "ETF";
                case TipoQuitacaoCIOT.Filial: return "Filial";
                case TipoQuitacaoCIOT.Transportadora: return "Transportadora";
                default: return string.Empty;
            }
        }

        public static string ObterEnumeradorPagbem(this TipoQuitacaoCIOT status)
        {
            switch (status)
            {
                case TipoQuitacaoCIOT.Padrao: return "QualquerLugar";
                case TipoQuitacaoCIOT.QualquerLugar: return "QualquerLugar";
                case TipoQuitacaoCIOT.ETF: return "ETF";
                case TipoQuitacaoCIOT.Filial: return "Filial";
                case TipoQuitacaoCIOT.Transportadora: return "Transportadora";
                default: return string.Empty;
            }
        }
    }
}
