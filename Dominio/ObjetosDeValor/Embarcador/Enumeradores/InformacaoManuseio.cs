namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum InformacaoManuseio
    {
        Im01 = 1,
        Im02 = 2,
        Im03 = 3,
        Im04 = 4,
        Im05 = 5,
        Im06 = 6,
        Im07 = 7,
        Im08 = 8,
        Im09 = 9,
        Im10 = 10,
        Im11 = 11,
        Im12 = 12,
        Im13 = 13,
        Im14 = 14,
        Im15 = 15,
        Im99 = 99
    }

    public static class InformacaoManuseioHelper
    {
        public static string ObterDescricao(this InformacaoManuseio informacaoManuseio)
        {
            switch (informacaoManuseio)
            {
                case InformacaoManuseio.Im01: return "01 - certificado do expedidor para embarque de animal vivo";
                case InformacaoManuseio.Im02: return "02 - artigo perigoso conforme Declaração do Expedidor anexa";
                case InformacaoManuseio.Im03: return "03 - somente em aeronave cargueira";
                case InformacaoManuseio.Im04: return "04 - artigo perigoso - declaração do expedidor não requerida";
                case InformacaoManuseio.Im05: return "05 - artigo perigoso em quantidade isenta";
                case InformacaoManuseio.Im06: return "06 - gelo seco para refrigeração (especificar no campo observações a quantidade)";
                case InformacaoManuseio.Im07: return "07 - não restrito (especificar a Disposição Especial no campo observações)";
                case InformacaoManuseio.Im08: return "08 - artigo perigoso em carga consolidada (especificar a quantidade no campo observações)";
                case InformacaoManuseio.Im09: return "09 - autorização da autoridade governamental anexa (especificar no campo observações)";
                case InformacaoManuseio.Im10: return "10 - baterias de íons de lítio em conformidade com a Seção II da PI965 - CAO";
                case InformacaoManuseio.Im11: return "11 - baterias de íons de lítio em conformidade com a Seção II da PI966";
                case InformacaoManuseio.Im12: return "12 - baterias de íons de lítio em conformidade com a Seção II da PI967";
                case InformacaoManuseio.Im13: return "13 - baterias de metal lítio em conformidade com a Seção II da PI968 - CAO";
                case InformacaoManuseio.Im14: return "14 - baterias de metal lítio em conformidade com a Seção II da PI969";
                case InformacaoManuseio.Im15: return "15 - baterias de metal lítio em conformidade com a Seção II da PI970";
                case InformacaoManuseio.Im99: return "99 - outro (especificar no campo observações)";
                default: return string.Empty;
            }
        }
    }
}
