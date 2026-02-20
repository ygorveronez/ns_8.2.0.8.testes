namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ProblemasCarga
    {
        Nenhum = 0,
        ProblemasCTe = 1,
        ProblemasAverbacao = 2,
        ProblemasValePedagio = 3,
        ProblemasMDFe = 4
    }

    public static class ProblemasCargaHelper
    {
        public static string ObterDescricao(this ProblemasCarga problema)
        {
            switch (problema)
            {
                case ProblemasCarga.Nenhum: return "";
                case ProblemasCarga.ProblemasCTe: return "CT-e";
                case ProblemasCarga.ProblemasAverbacao: return "Averbação";
                case ProblemasCarga.ProblemasValePedagio: return "Vale Pedágio";
                case ProblemasCarga.ProblemasMDFe: return "MDF-e";
                default: return string.Empty;
            }
        }

    }
}
