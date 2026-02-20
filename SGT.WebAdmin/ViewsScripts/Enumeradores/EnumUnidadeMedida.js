var EnumUnidadeMedidaHelper = function () {
    this.Todos = 0;
    this.Quilograma = 1;
    this.MetroCubico = 2;
    this.Tonelada = 3;
    this.Unidade = 4;
    this.Litros = 5;
    this.MMBTU = 6;
    this.Servico = 7;
    this.Caixa = 8;
    this.Ampola = 9;
    this.Balde = 10;
    this.Bandeja = 11;
    this.Barra = 12;
    this.Bisnaga = 13;
    this.Bloco = 14;
    this.Bobina = 15;
    this.Bombona = 16;
    this.Capsula = 17;
    this.Cartela = 18;
    this.Cento = 19;
    this.Conjunto = 20;
    this.Centimetro = 21;
    this.CentimetroQuadrado = 22;
    this.CaixaCom2Unidades = 23;
    this.CaixaCom3Unidades = 24;
    this.CaixaCom5Unidades = 25;
    this.CaixaCom10Unidades = 26;
    this.CaixaCom15Unidades = 27;
    this.CaixaCom20Unidades = 28;
    this.CaixaCom25Unidades = 29;
    this.CaixaCom50Unidades = 30;
    this.CaixaCom100Unidades = 31;
    this.Display = 32;
    this.Duzia = 33;
    this.Embalagem = 34;
    this.Fardo = 35;
    this.Folha = 36;
    this.Frasco = 37;
    this.Galao = 38;
    this.Garrafa = 39;
    this.Gramas = 40;
    this.Jogo = 41;
    this.Kit = 42;
    this.Lata = 43;
    this.Metro = 44;
    this.MetroQuadrado = 45;
    this.Milheiro = 46;
    this.Mililitro = 47;
    this.MegawattHora = 48;
    this.Pacote = 49;
    this.Palete = 50;
    this.Pares = 51;
    this.Peca = 52;
    this.Pote = 53;
    this.Quilate = 54;
    this.Resma = 55;
    this.Rolo = 56;
    this.Saco = 57;
    this.Sacola = 58;
    this.Tambor = 59;
    this.Tanque = 60;
    this.Tubo = 61;
    this.Vasilhame = 62;
    this.Vidro = 63;
    this.UnidadeUN = 64;
    this.Cone = 65;
    this.Bolsa = 66;
    this.Dose = 67;
};

EnumUnidadeMedidaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { value: this.Quilograma, text: "KG - Quilograma" },
            { value: this.Tonelada, text: "TON - Tonelada" },
            { value: this.Unidade, text: "UNID - Unidade" },
            { value: this.Litros, text: "LITRO - Litros" },
            { value: this.MetroCubico, text: "M3 - Metro Cúbico" },
            { value: this.MMBTU, text: "MMBTU" },
            { value: this.Servico, text: "SERV - Serviço" },
            { value: this.Caixa, text: "CX - Caixa" },
            { value: this.Ampola, text: "AMPOLA - Ampola" },
            { value: this.Balde, text: "BALDE - Balde" },
            { value: this.Bandeja, text: "BANDEJ - Bandeja" },
            { value: this.Barra, text: "BARRA - Barra" },
            { value: this.Bisnaga, text: "BISNAG - Bisnaga" },
            { value: this.Bloco, text: "BLOCO - Bloco" },
            { value: this.Bobina, text: "BOBINA - Bobina" },
            { value: this.Bombona, text: "BOMB - Bombona" },
            { value: this.Capsula, text: "CAPS - Capsula" },
            { value: this.Cartela, text: "CART - Cartela" },
            { value: this.Cento, text: "CENTO - Cento" },
            { value: this.Conjunto, text: "CJ - Conjunto" },
            { value: this.Centimetro, text: "CM - Centimetro" },
            { value: this.CentimetroQuadrado, text: "CM2 - Centimetro Quadrado" },
            { value: this.CaixaCom2Unidades, text: "CX2 - Caixa Com 2 Unidades" },
            { value: this.CaixaCom3Unidades, text: "CX3 - Caixa Com 3 Unidades" },
            { value: this.CaixaCom5Unidades, text: "CX5 - Caixa Com 5 Unidades" },
            { value: this.CaixaCom10Unidades, text: "CX10 - Caixa Com 10 Unidades" },
            { value: this.CaixaCom15Unidades, text: "CX15 - Caixa Com 15 Unidades" },
            { value: this.CaixaCom20Unidades, text: "CX20 - Caixa Com 20 Unidades" },
            { value: this.CaixaCom25Unidades, text: "CX25 - Caixa Com 25 Unidades" },
            { value: this.CaixaCom50Unidades, text: "CX50 - Caixa Com 50 Unidades" },
            { value: this.CaixaCom100Unidades, text: "CX100 - Caixa Com 100 Unidades" },
            { value: this.Display, text: "DISP - Display" },
            { value: this.Duzia, text: "DUZIA - Duzia" },
            { value: this.Embalagem, text: "EMBAL - Embalagem" },
            { value: this.Fardo, text: "FARDO - Fardo" },
            { value: this.Folha, text: "FOLHA - Folha" },
            { value: this.Frasco, text: "FRASCO - Frasco" },
            { value: this.Galao, text: "GALAO - Galão" },
            { value: this.Garrafa, text: "GF - Garrafa" },
            { value: this.Gramas, text: "GRAMAS - Gramas" },
            { value: this.Jogo, text: "JOGO - Jogo" },
            { value: this.Kit, text: "KIT - Kit" },
            { value: this.Lata, text: "LATA - Lata" },
            { value: this.Metro, text: "M - Metro" },
            { value: this.MetroQuadrado, text: "M2 - Metro Quadrado" },
            { value: this.Milheiro, text: "MILHEI - Milheiro" },
            { value: this.Mililitro, text: "MILI - Mililitro" },
            { value: this.MegawattHora, text: "MWH - Megawatt Hora" },
            { value: this.Pacote, text: "PACOTE - Pacote" },
            { value: this.Palete, text: "PALETE - Palete" },
            { value: this.Pares, text: "PARES - Pares" },
            { value: this.Peca, text: "PC - Peça" },
            { value: this.Pote, text: "POTE - Pote" },
            { value: this.Quilate, text: "K - Quilate" },
            { value: this.Resma, text: "RESMA - Resma" },
            { value: this.Rolo, text: "ROLO - Rolo" },
            { value: this.Saco, text: "SACO - Saco" },
            { value: this.Sacola, text: "SACOLA - Sacola" },
            { value: this.Tambor, text: "TAMBOR - Tambor" },
            { value: this.Tanque, text: "TANQUE - Tanque" },
            { value: this.Tubo, text: "TUBO - Tubo" },
            { value: this.Vasilhame, text: "VASIL - Vasilhame" },
            { value: this.Vidro, text: "VIDRO - Vidro" },
            { value: this.UnidadeUN, text: "UN - Unidade" },
            { value: this.Cone, text: "CN - Cone" },
            { value: this.Bolsa, text: "BO - Bolsa" },
            { value: this.Dose, text: "DS - Dose" },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumUnidadeMedida = Object.freeze(new EnumUnidadeMedidaHelper());