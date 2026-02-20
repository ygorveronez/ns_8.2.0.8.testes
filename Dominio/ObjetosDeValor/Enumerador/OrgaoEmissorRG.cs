namespace Dominio.ObjetosDeValor.Enumerador
{
    public enum OrgaoEmissorRG
    {
        Nenhum = 0,
        SSP = 1,
        CNH = 2,
        MMA = 3,
        DIC = 4,
        POF = 5,
        IFP = 6,
        POM = 7,
        IPF = 8,
        SES = 9,
        MAE = 10,
        MEX = 11,
        SJS = 12,
        SJ = 13,
        SPTC = 14,
        SECC = 15,
        SEJUSP = 16
    }

    public static class OrgaoEmissorRGHelper
    {
        public static string ObterDescricao(this OrgaoEmissorRG orgao)
        {
            switch (orgao)
            {
                case OrgaoEmissorRG.SSP: return "SSP";
                case OrgaoEmissorRG.CNH: return "CNH";
                case OrgaoEmissorRG.MMA: return "MMA";
                case OrgaoEmissorRG.DIC: return "DIC";
                case OrgaoEmissorRG.POF: return "POF";
                case OrgaoEmissorRG.IFP: return "IFP";
                case OrgaoEmissorRG.POM: return "POM";
                case OrgaoEmissorRG.IPF: return "IPF";
                case OrgaoEmissorRG.SES: return "SES";
                case OrgaoEmissorRG.MAE: return "MAE";
                case OrgaoEmissorRG.MEX: return "MEX";
                case OrgaoEmissorRG.SJS: return "SJS";
                case OrgaoEmissorRG.SJ: return "SJ";
                case OrgaoEmissorRG.SPTC: return "SPTC";
                case OrgaoEmissorRG.SECC: return "SECC";
                case OrgaoEmissorRG.SEJUSP: return "SEJUSP";
                default: return string.Empty;
            }
        }

        public static string ObterSigla(this OrgaoEmissorRG orgao)
        {
            switch (orgao)
            {
                case OrgaoEmissorRG.SSP: return "Secretaria de Segurança Pública";
                case OrgaoEmissorRG.CNH: return "Carteira Nacional de Habilitação";
                case OrgaoEmissorRG.MMA: return "Ministério da Marinha";
                case OrgaoEmissorRG.DIC: return "Diretoria de Identificação Civil";
                case OrgaoEmissorRG.POF: return "Polícia Federal";
                case OrgaoEmissorRG.IFP: return "Instituto Félix Pacheco";
                case OrgaoEmissorRG.POM: return "Polícia Militar";
                case OrgaoEmissorRG.IPF: return "Instituto Pereira Faustino";
                case OrgaoEmissorRG.SES: return "Carteira de Estrangeiro";
                case OrgaoEmissorRG.MAE: return "Ministério da Aeronáutica";
                case OrgaoEmissorRG.MEX: return "Ministério do Exército";
                case OrgaoEmissorRG.SJS: return "Secretaria da Justiça e Segurança";
                case OrgaoEmissorRG.SJ: return "Secretaria da Justiça";
                case OrgaoEmissorRG.SPTC: return "Secretaria de Polícia Técnico-Científica";
                case OrgaoEmissorRG.SECC: return "Secretaria de Estado da Casa Civil";
                case OrgaoEmissorRG.SEJUSP: return "Secretaria de Estado de Justiça e Segurança Pública";
                default: return string.Empty;
            }
        }
    }
}
