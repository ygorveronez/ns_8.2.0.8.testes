namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum MoedaCotacaoBancoCentral
    {
        Todas = -1,
        Real = 0,
        DolarVenda = 1,
        DolarCompra = 10813,
        PesoArgentino = 3,
        PesoUruguaio = 4,
        PesoChileno = 5,
        Guarani = 6,
        NovoSolPeruano = 7
    }

    public static class MoedaCotacaoBancoCentralHelper
    {
        public static string ObterDescricao(this MoedaCotacaoBancoCentral moeda)
        {
            switch (moeda)
            {
                case MoedaCotacaoBancoCentral.Real: return "Real";
                case MoedaCotacaoBancoCentral.DolarVenda: return "Dólar (Venda)";
                case MoedaCotacaoBancoCentral.DolarCompra: return "Dólar (Compra)";
                case MoedaCotacaoBancoCentral.PesoArgentino: return "Peso Argentino";
                case MoedaCotacaoBancoCentral.PesoUruguaio: return "Peso Uruguaio";
                case MoedaCotacaoBancoCentral.PesoChileno: return "Peso Chileno";
                case MoedaCotacaoBancoCentral.Guarani: return "Guarani";
                case MoedaCotacaoBancoCentral.NovoSolPeruano: return "Novo Sol Peruano";
                default: return string.Empty;
            }
        }

        public static string ObterDescricaoSimplificada(this MoedaCotacaoBancoCentral moeda)
        {
            switch (moeda)
            {
                case MoedaCotacaoBancoCentral.Real: return "Real";
                case MoedaCotacaoBancoCentral.DolarCompra:
                case MoedaCotacaoBancoCentral.DolarVenda:
                    return "Dólar";
                case MoedaCotacaoBancoCentral.PesoArgentino: return "Peso Argentino";
                case MoedaCotacaoBancoCentral.PesoUruguaio: return "Peso Uruguaio";
                case MoedaCotacaoBancoCentral.PesoChileno: return "Peso Chileno";
                case MoedaCotacaoBancoCentral.Guarani: return "Guarani";
                case MoedaCotacaoBancoCentral.NovoSolPeruano: return "Novo Sol Peruano";
                default: return string.Empty;
            }
        }

        public static string ObterSigla(this MoedaCotacaoBancoCentral moeda)
        {
            switch (moeda)
            {
                case MoedaCotacaoBancoCentral.DolarVenda:
                case MoedaCotacaoBancoCentral.DolarCompra:
                    return "US$";
                case MoedaCotacaoBancoCentral.Guarani:
                    return "Gs";
                case MoedaCotacaoBancoCentral.PesoArgentino:
                case MoedaCotacaoBancoCentral.PesoChileno:
                case MoedaCotacaoBancoCentral.PesoUruguaio:
                    return "$";
                case MoedaCotacaoBancoCentral.Real:
                    return "R$";
                case MoedaCotacaoBancoCentral.NovoSolPeruano:
                    return "S/";
                default: return string.Empty;
            }
        }

        public static string ObterSiglaEstrangeira(this MoedaCotacaoBancoCentral moeda)
        {
            switch (moeda)
            {
                case MoedaCotacaoBancoCentral.DolarVenda:
                case MoedaCotacaoBancoCentral.DolarCompra:
                    return "USD";
                case MoedaCotacaoBancoCentral.Guarani: return "PYG";
                case MoedaCotacaoBancoCentral.PesoArgentino: return "ARS";
                case MoedaCotacaoBancoCentral.PesoChileno: return "CLP";
                case MoedaCotacaoBancoCentral.PesoUruguaio: return "UYU";
                case MoedaCotacaoBancoCentral.Real: return "BRL";
                case MoedaCotacaoBancoCentral.NovoSolPeruano: return "PEN";
                default: return string.Empty;
            }
        }
    }
}