namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum CSTServico
    {
        Todos = -1,
        TributadaIntegralmente = 0,
        TributadaIntegralmenteISSRF = 1,
        TributadaIntegralmenteST = 2,
        TributadaReducaoBC = 3,
        TributadaReducaoBCISSRF = 4,
        TributadaReducaoBCST = 5,
        Isenta = 6,
        Imune = 7,
        NaoTributadaISSRegimeFixo = 8,
        NaoTributadaISSRegimeEstimativa = 9,
        NaoTributadaISSConstrucaoCivil = 10,
        NaoTributadaISSRecolhidoNotaAvulsa = 11,
        NaoTributadaPrestadorEstabelecidoMunicipio = 12,
        NaoTributadaRecolhimentoForaMunicipio = 13,
        NaoTributada = 14,
        NaoTributadaAtoCooperado = 15,
        ProdutosDocumentoFiscalConjugado = 99
    }

    public static class CSTServicoHelper
    {
        public static string ObterDescricao(this CSTServico cstServico)
        {
            switch (cstServico)
            {
                case CSTServico.TributadaIntegralmente: return "00 - Tributada Integralmente";
                case CSTServico.TributadaIntegralmenteISSRF: return "01 - Tributada Integralmente com ISSRF";
                case CSTServico.TributadaIntegralmenteST: return "02 - Tributada Integralmente e sujeita à Substituição Tributária";
                case CSTServico.TributadaReducaoBC: return "03 - Tributada com redução da base de cálculo";
                case CSTServico.TributadaReducaoBCISSRF: return "04 - Tributada com redução da base de cálculo com ISSRF";
                case CSTServico.TributadaReducaoBCST: return "05 - Tributada com redução da base de cálculo e sujeita à Substituição Tributária";
                case CSTServico.Isenta: return "06 - Isenta";
                case CSTServico.Imune: return "07 - Imune";
                case CSTServico.NaoTributadaISSRegimeFixo: return "08 - Não Tributada - ISS regime Fixo";
                case CSTServico.NaoTributadaISSRegimeEstimativa: return "09 - Não Tributada - ISS regime Estimativa";
                case CSTServico.NaoTributadaISSConstrucaoCivil: return "10 - Não Tributada - ISS Construção Civil recolhido antecipadamente";
                case CSTServico.NaoTributadaISSRecolhidoNotaAvulsa: return "11 - Não Tributada - ISS recolhido por Nota Avulsa";
                case CSTServico.NaoTributadaPrestadorEstabelecidoMunicipio: return "12 - Não Tributada - Prestador estabelecido no Município";
                case CSTServico.NaoTributadaRecolhimentoForaMunicipio: return "13 - Não Tributada - Recolhimento efetuado pelo prestador de fora do Município";
                case CSTServico.NaoTributada: return "14 - Não Tributada";
                case CSTServico.NaoTributadaAtoCooperado: return "15 - Não Tributada - Ato Cooperado";
                case CSTServico.ProdutosDocumentoFiscalConjugado: return "99 - Produtos Documento Fiscal Conjugado";
                default: return string.Empty;
            }
        }
    }
}
