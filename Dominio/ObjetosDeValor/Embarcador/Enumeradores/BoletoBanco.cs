namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum BoletoBanco
    {
        Nenhum = 0,
        BancoDoBrasil = 1,
        Santander = 2,
        CaixaEconomica = 3,
        CaixaSicob = 4,
        Bradesco = 5,
        Itau = 6,
        BancoMercantil = 7,
        Sicred = 8,
        Bancoob = 9,
        Banrisul = 10,
        Banestes = 11,
        HSBC = 12,
        BancoDoNordeste = 13,
        BRB = 14,
        BicBanco = 15,
        BradescoSICOOB = 16,
        BancoSafra = 17,
        SafraBradesco = 18,
        BancoCECRED = 19,
        BancoDaAmazonia = 20,
        BancoDoBrasilSICOOB = 21,
        Uniprime = 22,
        UnicredRS = 23,
        Banese = 24,
        CrediSIS = 25,
        UnicredES = 26,
        BancoCresolSCRS = 27,
        CitiBank = 28,
        BancoABCBrasil = 29,
        Daycoval = 30,
        UniprimeNortePR = 31,
        BancoPine = 32,
        BancoPineBradesco = 33,
        UnicredSC = 34,
        BancoAlfa = 35,
        BancoDoBrasilAPI = 36,
        BancoDoBrasilWS = 37,
        BancoCresol = 38,
        MoneyPlus = 39,
        BancoC6 = 40,
        BancoRendimento = 41,
        BancoInter = 42,
        BancoSofisaSantander = 43,
        BS2 = 44,
        JPMorgan = 61
    }

    public static class BoletoBancoHelper
    {
        public static string ObterDescricao(this BoletoBanco BoletoBanco)
        {
            switch (BoletoBanco)
            {
                case BoletoBanco.BancoDoBrasil: return "Banco Do Brasil";
                case BoletoBanco.Santander: return "Santander";
                case BoletoBanco.CaixaEconomica: return "Caixa Economica";
                case BoletoBanco.CaixaSicob: return "Caixa Sicob";
                case BoletoBanco.Bradesco: return "Bradesco";
                case BoletoBanco.Itau: return "Itau";
                case BoletoBanco.BancoMercantil: return "BancoMercantil";
                case BoletoBanco.Sicred: return "Sicred";
                case BoletoBanco.Bancoob: return "Bancoob";
                case BoletoBanco.Banrisul: return "Banrisul";
                case BoletoBanco.Banestes: return "Banestes";
                case BoletoBanco.HSBC: return "HSBC";
                case BoletoBanco.BancoDoNordeste: return "Banco Do Nordeste";
                case BoletoBanco.BRB: return "BRB";
                case BoletoBanco.BicBanco: return "Bic Banco";
                case BoletoBanco.BradescoSICOOB: return "Bradesco SICOOB";
                case BoletoBanco.BancoSafra: return "Banco Safra";
                case BoletoBanco.SafraBradesco: return "Safra Bradesco";
                case BoletoBanco.BancoCECRED: return "Banco CECRED";
                case BoletoBanco.BancoDaAmazonia: return "Banco Da Amazonia";
                case BoletoBanco.BancoDoBrasilSICOOB: return "Banco Do Brasil SICOOB";
                case BoletoBanco.Uniprime: return "Uniprime";
                case BoletoBanco.UnicredRS: return "Unicred RS";
                case BoletoBanco.Banese: return "Banese";
                case BoletoBanco.CrediSIS: return "Credi SIS";
                case BoletoBanco.UnicredES: return "Unicred ES";
                case BoletoBanco.BancoCresolSCRS: return "Cresol SC/RS";
                case BoletoBanco.CitiBank: return "Citi Bank";
                case BoletoBanco.BancoABCBrasil: return "ABC Brasil";
                case BoletoBanco.Daycoval: return "Daycoval";
                case BoletoBanco.UniprimeNortePR: return "Uniprime Norte PR";
                case BoletoBanco.BancoPine: return "Pine";
                case BoletoBanco.BancoPineBradesco: return "Pine Bradesco";
                case BoletoBanco.UnicredSC: return "Unicred SC";
                case BoletoBanco.BancoAlfa: return "Banco Alfa";
                case BoletoBanco.BancoDoBrasilAPI: return "Banco do Brasil API";
                case BoletoBanco.BancoDoBrasilWS: return "Banco do Brasil WS";
                case BoletoBanco.BancoCresol: return "Cresol";
                case BoletoBanco.MoneyPlus: return "Money Plus";
                case BoletoBanco.BancoC6: return "C6";
                case BoletoBanco.BancoRendimento: return "Rendimento";
                case BoletoBanco.BancoInter: return "Banco Inter";
                case BoletoBanco.BancoSofisaSantander: return "Sofisa Santander";
                case BoletoBanco.BS2: return "BS2";
                case BoletoBanco.JPMorgan: return "J.P. Morgan";
                default: return string.Empty;
            }
        }
    }
}
