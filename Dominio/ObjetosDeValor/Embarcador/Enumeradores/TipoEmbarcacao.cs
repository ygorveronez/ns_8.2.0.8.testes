namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoEmbarcacao
    {
        Nenhum = 0,
        Alvarenga = 1,
        Barcaca = 2,
        BarcacaPropulsada = 3,
        Cargueiro = 4,
        CatamaraMisto = 5,
        CatamaraPassageiros = 6,
        ChataCargueira = 7,
        ChataGraneleira = 8,
        ChataMista = 9,
        ChataTanque = 10,
        Empurrador = 11,
        Estimulacao = 12,
        FerryBoat = 13,
        Flotel = 14,
        Flutuante = 15,
        CabreaGuindaste = 16,
        LanchaMista = 17,
        LanchaPassageiros = 18,
        LanchaPratico = 19,
        ManuseioDeEspias = 20,
        Pesquisa = 21,
        Rebocador = 22,
        Suprimento = 23,
        Transbordador = 24,
        CargaPesada = 25,
        Frigorifico = 26,
        Glp = 27,
        Graneleiro = 28,
        MineroPetroleiro = 29,
        MultiProposito = 30,
        NavioCisterna = 31,
        Passageiros = 32,
        Petroleiro = 33,
        PortaConteiner = 34,
        TanqueQuimico = 35,
        RoRo = 36,
        Draga = 37,
        Outros = 38,
        Balsa = 39,
        Dsv = 40,
        Swath = 41,
        Rov = 42,
        Ahts = 43,
        Ut4000 = 44,
        Ut750 = 45,
        Rsv = 46,
        TanqueGlp = 47,
        PatrolVessel = 48,
        TanqueGnl = 50,
        LançamentoDeLinhas = 51,
        BarcoAMotor = 52,
        BalsaMotorizada = 53,
        Mpsv = 54,
        Pesqueiro = 55,
        Psv = 56
    }

    public static class TipoEmbarcacaoHelper
    {
        public static string ObterDescricao(this TipoEmbarcacao tipoEmbarcacao)
        {
            switch (tipoEmbarcacao)
            {
                case TipoEmbarcacao.Alvarenga: return "01 Alvarenga";
                case TipoEmbarcacao.Barcaca: return "02 Barcaca";
                case TipoEmbarcacao.BarcacaPropulsada: return "03 Barcaca Propulsada";
                case TipoEmbarcacao.Cargueiro: return "04 Cargueiro";
                case TipoEmbarcacao.CatamaraMisto: return "05 Catamara Misto";
                case TipoEmbarcacao.CatamaraPassageiros: return "06 Catamara Passageiros";
                case TipoEmbarcacao.ChataCargueira: return "07 Chata Cargueira";
                case TipoEmbarcacao.ChataGraneleira: return "08 Chata Graneleira";
                case TipoEmbarcacao.ChataMista: return "09 Chata Mista";
                case TipoEmbarcacao.ChataTanque: return "10 Chata Tanque";
                case TipoEmbarcacao.Empurrador: return "11 Empurrador";
                case TipoEmbarcacao.Estimulacao: return "12 Estimulacao";
                case TipoEmbarcacao.FerryBoat: return "13 Ferry Boat";
                case TipoEmbarcacao.Flotel: return "14 Flotel";
                case TipoEmbarcacao.Flutuante: return "15 Flutuante";
                case TipoEmbarcacao.CabreaGuindaste: return "16 Cabrea Guindaste";
                case TipoEmbarcacao.LanchaMista: return "17 Lancha Mista";
                case TipoEmbarcacao.LanchaPassageiros: return "18 Lancha Passageiros";
                case TipoEmbarcacao.LanchaPratico: return "19 Lancha Pratico";
                case TipoEmbarcacao.ManuseioDeEspias: return "20 Manuseio De Espias";
                case TipoEmbarcacao.Pesquisa: return "21 Pesquisa";
                case TipoEmbarcacao.Rebocador: return "22 Rebocador";
                case TipoEmbarcacao.Suprimento: return "23 Suprimento";
                case TipoEmbarcacao.Transbordador: return "24 Transbordador";
                case TipoEmbarcacao.CargaPesada: return "25 Carga Pesada";
                case TipoEmbarcacao.Frigorifico: return "26 Frigorifico";
                case TipoEmbarcacao.Glp: return "27 Glp";
                case TipoEmbarcacao.Graneleiro: return "28 Graneleiro";
                case TipoEmbarcacao.MineroPetroleiro: return "29 Minero Petroleiro";
                case TipoEmbarcacao.MultiProposito: return "30 Multi Proposito";
                case TipoEmbarcacao.NavioCisterna: return "31 Navio Cisterna";
                case TipoEmbarcacao.Passageiros: return "32 Passageiros";
                case TipoEmbarcacao.Petroleiro: return "33 Petroleiro";
                case TipoEmbarcacao.PortaConteiner: return "34 Porta Conteiner";
                case TipoEmbarcacao.TanqueQuimico: return "35 Tanque Quimico";
                case TipoEmbarcacao.RoRo: return "36 RoRo";
                case TipoEmbarcacao.Draga: return "37 Draga";
                case TipoEmbarcacao.Outros: return "38 Outros";
                case TipoEmbarcacao.Balsa: return "39 Balsa";
                case TipoEmbarcacao.Dsv: return "40 Dsv";
                case TipoEmbarcacao.Swath: return "41 Swath";
                case TipoEmbarcacao.Rov: return "42 Rov";
                case TipoEmbarcacao.Ahts: return "43 Ahts";
                case TipoEmbarcacao.Ut4000: return "44 Ut4000";
                case TipoEmbarcacao.Ut750: return "45 Ut750";
                case TipoEmbarcacao.Rsv: return "46 Rsv";
                case TipoEmbarcacao.TanqueGlp: return "47 Tanque Glp";
                case TipoEmbarcacao.PatrolVessel: return "48 Patrol Vessel";
                case TipoEmbarcacao.TanqueGnl: return "50 Tanque Gnl";
                case TipoEmbarcacao.LançamentoDeLinhas: return "51 Lancamento De Linhas";
                case TipoEmbarcacao.BarcoAMotor: return "52 Barco A Motor";
                case TipoEmbarcacao.BalsaMotorizada: return "53 Balsa Motorizada";
                case TipoEmbarcacao.Mpsv: return "54 Mpsv";
                case TipoEmbarcacao.Pesqueiro: return "55 Pesqueiro";
                case TipoEmbarcacao.Psv: return "56 Psv";
                default: return string.Empty;
            }
        }
    }
}
