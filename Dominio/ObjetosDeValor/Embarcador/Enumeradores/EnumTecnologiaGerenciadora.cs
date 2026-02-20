namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    //ATENÇÃO Este Enumerador SEGUE O MESMO ENUM DA NSTECH (nao adicione novos sem analisar o Notion)
    //Notion: https://plataformanstech.notion.site/RAST-Rastreamento-Veicular-180e264261d84a088e86793f1e16072d

    public enum EnumTecnologiaGerenciadora
    {
        NaoDefinido = 0,
        BRK = 1,
        Buonny = 2,
        Trafegus = 4,
        LogRisk = 21,
        AngelLira = 108,
        Opentech = 5
    }

    public static class EnumTecnologiaGerenciadoraHelper
    {
        public static string ObterDescricao(this EnumTecnologiaGerenciadora GR)
        {
            switch (GR)
            {
                case EnumTecnologiaGerenciadora.BRK: return "BRK";
                case EnumTecnologiaGerenciadora.Buonny: return "Buonny";
                case EnumTecnologiaGerenciadora.Trafegus: return "Trafegus";
                case EnumTecnologiaGerenciadora.LogRisk: return "LogRisk";
                case EnumTecnologiaGerenciadora.AngelLira: return "AngelLira";
                case EnumTecnologiaGerenciadora.Opentech: return "Opentech";
                case EnumTecnologiaGerenciadora.NaoDefinido: return "";
                default: return string.Empty;
            }
        }

        public static EnumTecnologiaGerenciadora ObterEnumPorDescricao(string Descricao)
        {
            if (string.IsNullOrEmpty(Descricao))
                return EnumTecnologiaGerenciadora.NaoDefinido;


            if (Descricao.ToLower().Contains("brk"))
                return EnumTecnologiaGerenciadora.BRK;
            else if (Descricao.ToLower().Contains("bounny"))
                return EnumTecnologiaGerenciadora.Buonny;
            else if (Descricao.ToLower().Contains("trafegus"))
                return EnumTecnologiaGerenciadora.Trafegus;
            else if (Descricao.ToLower().Contains("logrisk"))
                return EnumTecnologiaGerenciadora.LogRisk;
            else if (Descricao.ToLower().Contains("angellira"))
                return EnumTecnologiaGerenciadora.AngelLira;
            else if (Descricao.ToLower().Contains("opentech"))
                return EnumTecnologiaGerenciadora.Opentech;
            else
                return EnumTecnologiaGerenciadora.NaoDefinido;
        }
    }
}

