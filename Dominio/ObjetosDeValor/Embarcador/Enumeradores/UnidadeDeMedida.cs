namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum UnidadeDeMedida
    {
        Nenhum = 0,
        Quilograma = 1,
        MetroCubico = 2,
        Tonelada = 3,
        Unidade = 4,
        Litros = 5,
        MMBTU = 6,
        Servico = 7,
        Caixa = 8,
        Ampola = 9,
        Balde = 10,
        Bandeja = 11,
        Barra = 12,
        Bisnaga = 13,
        Bloco = 14,
        Bobina = 15,
        Bombona = 16,
        Capsula = 17,
        Cartela = 18,
        Cento = 19,
        Conjunto = 20,
        Centimetro = 21,
        CentimetroQuadrado = 22,
        CaixaCom2Unidades = 23,
        CaixaCom3Unidades = 24,
        CaixaCom5Unidades = 25,
        CaixaCom10Unidades = 26,
        CaixaCom15Unidades = 27,
        CaixaCom20Unidades = 28,
        CaixaCom25Unidades = 29,
        CaixaCom50Unidades = 30,
        CaixaCom100Unidades = 31,
        Display = 32,
        Duzia = 33,
        Embalagem = 34,
        Fardo = 35,
        Folha = 36,
        Frasco = 37,
        Galao = 38,
        Garrafa = 39,
        Gramas = 40,
        Jogo = 41,
        Kit = 42,
        Lata = 43,
        Metro = 44,
        MetroQuadrado = 45,
        Milheiro = 46,
        Mililitro = 47,
        MegawattHora = 48,
        Pacote = 49,
        Palete = 50,
        Pares = 51,
        Peca = 52,
        Pote = 53,
        Quilate = 54,
        Resma = 55,
        Rolo = 56,
        Saco = 57,
        Sacola = 58,
        Tambor = 59,
        Tanque = 60,
        Tubo = 61,
        Vasilhame = 62,
        Vidro = 63,
        UnidadeUN = 64,
        Cone = 65,
        Bolsa = 66,
        Dose = 67
    }

    public static class UnidadeDeMedidaHelper
    {
        public static string ObterSigla(this UnidadeDeMedida unidade)
        {
            return ObterSigla((UnidadeDeMedida?)unidade);
        }

        public static string ObterSigla(UnidadeDeMedida? unidade)
        {
            switch (unidade)
            {
                case UnidadeDeMedida.Quilograma: return "KG";
                case UnidadeDeMedida.Tonelada: return "TON";
                case UnidadeDeMedida.Unidade: return "UNID";
                case UnidadeDeMedida.UnidadeUN: return "UN";
                case UnidadeDeMedida.Litros: return "L";
                case UnidadeDeMedida.MetroCubico: return "M3";
                case UnidadeDeMedida.MMBTU: return "MMBTU";
                case UnidadeDeMedida.Servico: return "SERV";
                case UnidadeDeMedida.Caixa: return "CX";
                case UnidadeDeMedida.Ampola: return "AMPOLA";
                case UnidadeDeMedida.Balde: return "BALDE";
                case UnidadeDeMedida.Bandeja: return "BANDEJ";
                case UnidadeDeMedida.Barra: return "BARRA";
                case UnidadeDeMedida.Bisnaga: return "BISNAG";
                case UnidadeDeMedida.Bloco: return "BLOCO";
                case UnidadeDeMedida.Bobina: return "BOBINA";
                case UnidadeDeMedida.Bombona: return "BOMB";
                case UnidadeDeMedida.Capsula: return "CAPS";
                case UnidadeDeMedida.Cartela: return "CART";
                case UnidadeDeMedida.Cento: return "CENTO";
                case UnidadeDeMedida.Conjunto: return "CJ";
                case UnidadeDeMedida.Centimetro: return "CM";
                case UnidadeDeMedida.CentimetroQuadrado: return "CM2";
                case UnidadeDeMedida.CaixaCom2Unidades: return "CX2";
                case UnidadeDeMedida.CaixaCom3Unidades: return "CX3";
                case UnidadeDeMedida.CaixaCom5Unidades: return "CX5";
                case UnidadeDeMedida.CaixaCom10Unidades: return "CX10";
                case UnidadeDeMedida.CaixaCom15Unidades: return "CX15";
                case UnidadeDeMedida.CaixaCom20Unidades: return "CX20";
                case UnidadeDeMedida.CaixaCom25Unidades: return "CX25";
                case UnidadeDeMedida.CaixaCom50Unidades: return "CX50";
                case UnidadeDeMedida.CaixaCom100Unidades: return "CX100";
                case UnidadeDeMedida.Display: return "DISP";
                case UnidadeDeMedida.Duzia: return "DUZIA";
                case UnidadeDeMedida.Embalagem: return "EMBAL";
                case UnidadeDeMedida.Fardo: return "FARDO";
                case UnidadeDeMedida.Folha: return "FOLHA";
                case UnidadeDeMedida.Frasco: return "FRASCO";
                case UnidadeDeMedida.Galao: return "GALAO";
                case UnidadeDeMedida.Garrafa: return "GF";
                case UnidadeDeMedida.Gramas: return "GRAMAS";
                case UnidadeDeMedida.Jogo: return "JOGO";
                case UnidadeDeMedida.Kit: return "KIT";
                case UnidadeDeMedida.Lata: return "LATA";
                case UnidadeDeMedida.Metro: return "M";
                case UnidadeDeMedida.MetroQuadrado: return "M2";
                case UnidadeDeMedida.Milheiro: return "MM";
                case UnidadeDeMedida.Mililitro: return "MILHEI";
                case UnidadeDeMedida.MegawattHora: return "MWH";
                case UnidadeDeMedida.Pacote: return "PACOTE";
                case UnidadeDeMedida.Palete: return "PALETE";
                case UnidadeDeMedida.Pares: return "PARES";
                case UnidadeDeMedida.Peca: return "PC";
                case UnidadeDeMedida.Pote: return "POTE";
                case UnidadeDeMedida.Quilate: return "K";
                case UnidadeDeMedida.Resma: return "RESMA";
                case UnidadeDeMedida.Rolo: return "ROLO";
                case UnidadeDeMedida.Saco: return "SACO";
                case UnidadeDeMedida.Sacola: return "SACOLA";
                case UnidadeDeMedida.Tambor: return "TAMBOR";
                case UnidadeDeMedida.Tanque: return "TANQUE";
                case UnidadeDeMedida.Tubo: return "TUBO";
                case UnidadeDeMedida.Vasilhame: return "VASIL";
                case UnidadeDeMedida.Vidro: return "VIDRO";
                case UnidadeDeMedida.Cone: return "CN";
                case UnidadeDeMedida.Bolsa: return "BO";
                case UnidadeDeMedida.Dose: return "DS";
                default: return string.Empty;
            }
        }

        public static string ObterDescricao(this UnidadeDeMedida unidade)
        {
            return ObterDescricao((UnidadeDeMedida?)unidade);
        }

        public static string ObterDescricao(UnidadeDeMedida? unidade)
        {
            switch (unidade)
            {
                case UnidadeDeMedida.Quilograma: return "KG - Quilograma";
                case UnidadeDeMedida.Tonelada: return "TON - Tonelada";
                case UnidadeDeMedida.Unidade: return "UNID - Unidade";
                case UnidadeDeMedida.UnidadeUN: return "UN - Unidade";
                case UnidadeDeMedida.Litros: return "L - Litros";
                case UnidadeDeMedida.MetroCubico: return "M3 - Metro Cúbico";
                case UnidadeDeMedida.MMBTU: return "MMBTU";
                case UnidadeDeMedida.Servico: return "SERV - Serviço";
                case UnidadeDeMedida.Caixa: return "CX - Caixa";
                case UnidadeDeMedida.Ampola: return "AMPOLA - Ampola";
                case UnidadeDeMedida.Balde: return "BALDE - Balde";
                case UnidadeDeMedida.Bandeja: return "BANDEJ - Bandeja";
                case UnidadeDeMedida.Barra: return "BARRA - Barra";
                case UnidadeDeMedida.Bisnaga: return "BISNAG - Bisnaga";
                case UnidadeDeMedida.Bloco: return "BLOCO - Bloco";
                case UnidadeDeMedida.Bobina: return "BOBINA - Bobina";
                case UnidadeDeMedida.Bombona: return "BOMB - Bombona";
                case UnidadeDeMedida.Capsula: return "CAPS - Capsula";
                case UnidadeDeMedida.Cartela: return "CART - Cartela";
                case UnidadeDeMedida.Cento: return "CENTO - Cento";
                case UnidadeDeMedida.Conjunto: return "CJ - Conjunto";
                case UnidadeDeMedida.Centimetro: return "CM - Centimetro";
                case UnidadeDeMedida.CentimetroQuadrado: return "CM2 - Centimetro Quadrado";
                case UnidadeDeMedida.CaixaCom2Unidades: return "CX2 - Caixa Com 2 Unidades";
                case UnidadeDeMedida.CaixaCom3Unidades: return "CX3 - Caixa Com 3 Unidades";
                case UnidadeDeMedida.CaixaCom5Unidades: return "CX5 - Caixa Com 5 Unidades";
                case UnidadeDeMedida.CaixaCom10Unidades: return "CX10 - Caixa Com 10 Unidades";
                case UnidadeDeMedida.CaixaCom15Unidades: return "CX15 - Caixa Com 15 Unidades";
                case UnidadeDeMedida.CaixaCom20Unidades: return "CX20 - Caixa Com 20 Unidades";
                case UnidadeDeMedida.CaixaCom25Unidades: return "CX25 - Caixa Com 25 Unidades";
                case UnidadeDeMedida.CaixaCom50Unidades: return "CX50 - Caixa Com 50 Unidades";
                case UnidadeDeMedida.CaixaCom100Unidades: return "CX100 - Caixa Com 100 Unidades";
                case UnidadeDeMedida.Display: return "DISP - Display";
                case UnidadeDeMedida.Duzia: return "DUZIA - Duzia";
                case UnidadeDeMedida.Embalagem: return "EMBAL - Embalagem";
                case UnidadeDeMedida.Fardo: return "FARDO - Fardo";
                case UnidadeDeMedida.Folha: return "FOLHA - Folha";
                case UnidadeDeMedida.Frasco: return "FRASCO - Frasco";
                case UnidadeDeMedida.Galao: return "GALAO - Galão";
                case UnidadeDeMedida.Garrafa: return "GF - Garrafa";
                case UnidadeDeMedida.Gramas: return "GRAMAS - Gramas";
                case UnidadeDeMedida.Jogo: return "JOGO - Jogo";
                case UnidadeDeMedida.Kit: return "KIT - Kit";
                case UnidadeDeMedida.Lata: return "LATA - Lata";
                case UnidadeDeMedida.Metro: return "M - Metro";
                case UnidadeDeMedida.MetroQuadrado: return "M2 - Metro Quadrado";
                case UnidadeDeMedida.Milheiro: return "MILHEI - Milheiro";
                case UnidadeDeMedida.Mililitro: return "MM - Mililitro";
                case UnidadeDeMedida.MegawattHora: return "MWH - Megawatt Hora";
                case UnidadeDeMedida.Pacote: return "PACOTE - Pacote";
                case UnidadeDeMedida.Palete: return "PALETE - Palete";
                case UnidadeDeMedida.Pares: return "PARES - Pares";
                case UnidadeDeMedida.Peca: return "PC - Peça";
                case UnidadeDeMedida.Pote: return "POTE - Pote";
                case UnidadeDeMedida.Quilate: return "K - Quilate";
                case UnidadeDeMedida.Resma: return "RESMA - Resma";
                case UnidadeDeMedida.Rolo: return "ROLO - Rolo";
                case UnidadeDeMedida.Saco: return "SACO - Saco";
                case UnidadeDeMedida.Sacola: return "SACOLA - Sacola";
                case UnidadeDeMedida.Tambor: return "TAMBOR - Tambor";
                case UnidadeDeMedida.Tanque: return "TANQUE - Tanque";
                case UnidadeDeMedida.Tubo: return "TUBO - Tubo";
                case UnidadeDeMedida.Vasilhame: return "VASIL - Vasilhame";
                case UnidadeDeMedida.Vidro: return "VIDRO - Vidro";
                case UnidadeDeMedida.Cone: return "CN - Cone";
                case UnidadeDeMedida.Bolsa: return "BO - Bolsa";
                case UnidadeDeMedida.Dose: return "DS - Dose";
                default: return string.Empty;
            }
        }

        public static UnidadeDeMedida? ObterUnidade(string unidade)
        {
            switch (unidade)
            {
                case "KG": return UnidadeDeMedida.Quilograma;
                case "TON": return UnidadeDeMedida.Tonelada;
                case "UNID": return UnidadeDeMedida.Unidade;
                case "UN": return UnidadeDeMedida.UnidadeUN;
                case "LITRO": return UnidadeDeMedida.Litros;
                case "L": return UnidadeDeMedida.Litros;
                case "M3": return UnidadeDeMedida.MetroCubico;
                case "MMBTU": return UnidadeDeMedida.MMBTU;
                case "SERV": return UnidadeDeMedida.Servico;
                case "CX": return UnidadeDeMedida.Caixa;
                case "AMPOLA": return UnidadeDeMedida.Ampola;
                case "BALDE": return UnidadeDeMedida.Balde;
                case "BANDEJ": return UnidadeDeMedida.Bandeja;
                case "BARRA": return UnidadeDeMedida.Barra;
                case "BISNAG": return UnidadeDeMedida.Bisnaga;
                case "BLOCO": return UnidadeDeMedida.Bloco;
                case "BOBINA": return UnidadeDeMedida.Bobina;
                case "BOMB": return UnidadeDeMedida.Bombona;
                case "CAPS": return UnidadeDeMedida.Capsula;
                case "CART": return UnidadeDeMedida.Cartela;
                case "CENTO": return UnidadeDeMedida.Cento;
                case "CJ": return UnidadeDeMedida.Conjunto;
                case "CM": return UnidadeDeMedida.Centimetro;
                case "CM2": return UnidadeDeMedida.CentimetroQuadrado;
                case "CX2": return UnidadeDeMedida.CaixaCom2Unidades;
                case "CX3": return UnidadeDeMedida.CaixaCom3Unidades;
                case "CX5": return UnidadeDeMedida.CaixaCom5Unidades;
                case "CX10": return UnidadeDeMedida.CaixaCom10Unidades;
                case "CX15": return UnidadeDeMedida.CaixaCom15Unidades;
                case "CX20": return UnidadeDeMedida.CaixaCom20Unidades;
                case "CX25": return UnidadeDeMedida.CaixaCom25Unidades;
                case "CX50": return UnidadeDeMedida.CaixaCom50Unidades;
                case "CX100": return UnidadeDeMedida.CaixaCom100Unidades;
                case "DIPS": return UnidadeDeMedida.Display;
                case "DUZIA": return UnidadeDeMedida.Duzia;
                case "EMBAL": return UnidadeDeMedida.Embalagem;
                case "FARDO": return UnidadeDeMedida.Fardo;
                case "FOLHA": return UnidadeDeMedida.Folha;
                case "FRASCO": return UnidadeDeMedida.Frasco;
                case "GALAO": return UnidadeDeMedida.Galao;
                case "GF": return UnidadeDeMedida.Garrafa;
                case "GRAMAS": return UnidadeDeMedida.Gramas;
                case "JOGO": return UnidadeDeMedida.Jogo;
                case "KIT": return UnidadeDeMedida.Kit;
                case "LATA": return UnidadeDeMedida.Lata;
                case "M": return UnidadeDeMedida.Metro;
                case "M2": return UnidadeDeMedida.MetroQuadrado;
                case "MM": return UnidadeDeMedida.Milheiro;
                case "MILHEI": return UnidadeDeMedida.Mililitro;
                case "MWH": return UnidadeDeMedida.MegawattHora;
                case "PACOTE": return UnidadeDeMedida.Pacote;
                case "PALETE": return UnidadeDeMedida.Palete;
                case "PARES": return UnidadeDeMedida.Pares;
                case "PC": return UnidadeDeMedida.Peca;
                case "POTE": return UnidadeDeMedida.Pote;
                case "K": return UnidadeDeMedida.Quilate;
                case "RESMA": return UnidadeDeMedida.Resma;
                case "ROLO": return UnidadeDeMedida.Rolo;
                case "SACO": return UnidadeDeMedida.Saco;
                case "SACOLA": return UnidadeDeMedida.Sacola;
                case "TAMBOR": return UnidadeDeMedida.Tambor;
                case "TANQUE": return UnidadeDeMedida.Tanque;
                case "TUBO": return UnidadeDeMedida.Tubo;
                case "VASIL": return UnidadeDeMedida.Vasilhame;
                case "VIDRO": return UnidadeDeMedida.Vidro;
                case "CN": return UnidadeDeMedida.Cone;
                case "BO": return UnidadeDeMedida.Bolsa;
                case "DS": return UnidadeDeMedida.Dose;
                default: return UnidadeDeMedida.Unidade;
            }
        }

    }
}
