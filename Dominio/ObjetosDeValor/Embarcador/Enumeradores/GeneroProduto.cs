namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum GeneroProduto
    {
        Nenhum = 0,
        Genero0 = 1,
        Genero1 = 2,
        Genero2 = 3,
        Genero3 = 4,
        Genero4 = 5,
        Genero5 = 6,
        Genero6 = 7,
        Genero7 = 8,
        Genero8 = 9,
        Genero9 = 10,
        Genero10 = 11,
        Genero11 = 12,
        Genero12 = 13,
        Genero13 = 14,
        Genero14 = 15,
        Genero15 = 16,
        Genero16 = 17,
        Genero17 = 18,
        Genero18 = 19,
        Genero19 = 20,
        Genero20 = 21,
        Genero21 = 22,
        Genero22 = 23,
        Genero23 = 24,
        Genero24 = 25,
        Genero25 = 26,
        Genero26 = 27,
        Genero27 = 28,
        Genero28 = 29,
        Genero29 = 30,
        Genero30 = 31,
        Genero31 = 32,
        Genero32 = 33,
        Genero33 = 34,
        Genero34 = 35,
        Genero35 = 36,
        Genero36 = 37,
        Genero37 = 38,
        Genero38 = 39,
        Genero39 = 40,
        Genero40 = 41,
        Genero41 = 42,
        Genero42 = 43,
        Genero43 = 44,
        Genero44 = 45,
        Genero45 = 46,
        Genero46 = 47,
        Genero47 = 48,
        Genero48 = 49,
        Genero49 = 50,
        Genero50 = 51,
        Genero51 = 52,
        Genero52 = 53,
        Genero53 = 54,
        Genero54 = 55,
        Genero55 = 56,
        Genero56 = 57,
        Genero57 = 58,
        Genero58 = 59,
        Genero59 = 60,
        Genero60 = 61,
        Genero61 = 62,
        Genero62 = 63,
        Genero63 = 64,
        Genero64 = 65,
        Genero65 = 66,
        Genero66 = 67,
        Genero67 = 68,
        Genero68 = 69,
        Genero69 = 70,
        Genero70 = 71,
        Genero71 = 72,
        Genero72 = 73,
        Genero73 = 74,
        Genero74 = 75,
        Genero75 = 76,
        Genero76 = 77,
        Genero77 = 78,
        Genero78 = 79,
        Genero79 = 80,
        Genero80 = 81,
        Genero81 = 82,
        Genero82 = 83,
        Genero83 = 84,
        Genero84 = 85,
        Genero85 = 86,
        Genero86 = 87,
        Genero87 = 88,
        Genero88 = 89,
        Genero89 = 90,
        Genero90 = 91,
        Genero91 = 92,
        Genero92 = 93,
        Genero93 = 94,
        Genero94 = 95,
        Genero95 = 96,
        Genero96 = 97,
        Genero97 = 98,
        Genero98 = 99,
        Genero99 = 100
    }

    public static class GeneroProdutoHelper
    {
        public static string ObterDescricao(this GeneroProduto generoProduto)
        {
            switch (generoProduto)
            {
                case GeneroProduto.Genero0: return "0   Serviço";
                case GeneroProduto.Genero1: return "1   Animais vivos";
                case GeneroProduto.Genero2: return "2   Carnes e miudezas, comestíveis";
                case GeneroProduto.Genero3: return "3   Peixes e crustáceos, moluscos e os outros invertebrados aquáticos";
                case GeneroProduto.Genero4: return "4   Leite e laticínios; ovos de aves; mel natural; produtos comestíveis de origem animal, não especificados nem compreendidos em outros Capítulos da TIPI";
                case GeneroProduto.Genero5: return "5   Outros produtos de origem animal, não especificados nem compreendidos em outros Capítulos da TIPI";
                case GeneroProduto.Genero6: return "6   Plantas vivas e produtos de floricultura";
                case GeneroProduto.Genero7: return "7   Produtos hortícolas, plantas, raízes e tubérculos, comestíveis";
                case GeneroProduto.Genero8: return "8   Frutas; cascas de cítricos e de melões";
                case GeneroProduto.Genero9: return "9   Café, chá, mate e especiarias";
                case GeneroProduto.Genero10: return "10  Cereais";
                case GeneroProduto.Genero11: return "11  Produtos da indústria de moagem; malte; amidos e féculas; inulina; glúten de trigo";
                case GeneroProduto.Genero12: return "12  Sementes e frutos oleaginosos; grãos, sementes e frutos diversos; plantas industriais ou medicinais; palha e forragem";
                case GeneroProduto.Genero13: return "13  Gomas, resinas e outros sucos e extratos vegetais";
                case GeneroProduto.Genero14: return "14  Matérias para entrançar e outros produtos de origem vegetal, não especificadas nem compreendidas em outros Capítulos da NCM";
                case GeneroProduto.Genero15: return "15  Gorduras e óleos animais ou vegetais; produtos da sua dissociação; gorduras alimentares elaboradas; ceras de origem animal ou vegetal";
                case GeneroProduto.Genero16: return "16  Preparações de carne, de peixes ou de crustáceos, de moluscos ou de outros invertebrados aquáticos";
                case GeneroProduto.Genero17: return "17  Açúcares e produtos de confeitaria";
                case GeneroProduto.Genero18: return "18  Cacau e suas preparações";
                case GeneroProduto.Genero19: return "19  Preparações à base de cereais, farinhas, amidos, féculas ou de leite; produtos de pastelaria";
                case GeneroProduto.Genero20: return "20  Preparações de produtos hortícolas, de frutas ou de outras partes de plantas";
                case GeneroProduto.Genero21: return "21  Preparações alimentícias diversas";
                case GeneroProduto.Genero22: return "22  Bebidas, líquidos alcoólicos e vinagres";
                case GeneroProduto.Genero23: return "23  Resíduos e desperdícios das indústrias alimentares; alimentos preparados para animais";
                case GeneroProduto.Genero24: return "24  Fumo(tabaco) e seus sucedâneos, manufaturados";
                case GeneroProduto.Genero25: return "25  Sal; enxofre; terras e pedras; gesso, cal e cimento";
                case GeneroProduto.Genero26: return "26  Minérios, escórias e cinzas";
                case GeneroProduto.Genero27: return "27  Combustíveis minerais, óleos minerais e produtos de sua destilação; matérias betuminosas; ceras minerais";
                case GeneroProduto.Genero28: return "28  Produtos químicos inorgânicos; compostos inorgânicos ou orgânicos de metais preciosos, de elementos radioativos, de metais das terras raras ou de isótopos";
                case GeneroProduto.Genero29: return "29  Produtos químicos orgânicos";
                case GeneroProduto.Genero30: return "30  Produtos farmacêuticos";
                case GeneroProduto.Genero31: return "31  Adubos ou fertilizantes";
                case GeneroProduto.Genero32: return "32  Extratos tanantes e tintoriais; taninos e seus derivados; pigmentos e outras matérias corantes, tintas e vernizes, mástiques; tintas de escrever";
                case GeneroProduto.Genero33: return "33  Óleos essenciais e resinóides; produtos de perfumaria ou de toucador preparados e preparações cosméticas";
                case GeneroProduto.Genero34: return "34  Sabões, agentes orgânicos de superfície, preparações para lavagem, preparações lubrificantes, ceras artificiais, ceras preparadas...";
                case GeneroProduto.Genero35: return "35  Matérias albuminóides; produtos à base de amidos ou de féculas modificados; colas; enzimas";
                case GeneroProduto.Genero36: return "36  Pólvoras e explosivos; artigos de pirotecnia; fósforos; ligas pirofóricas; matérias inflamáveis";
                case GeneroProduto.Genero37: return "37  Produtos para fotografia e cinematografia";
                case GeneroProduto.Genero38: return "38  Produtos diversos das indústrias químicas";
                case GeneroProduto.Genero39: return "39  Plásticos e suas obras";
                case GeneroProduto.Genero40: return "40  Borracha e suas obras";
                case GeneroProduto.Genero41: return "41  Peles, exceto a peleteria(peles com pêlo *), e couros";
                case GeneroProduto.Genero42: return "42  Obras de couro; artigos de correeiro ou de seleiro; artigos de viagem, bolsas e artefatos semelhantes; obras de tripa";
                case GeneroProduto.Genero43: return "43  Peleteria(peles com pêlo *) e suas obras; peleteria(peles com pêlo *) artificial";
                case GeneroProduto.Genero44: return "44  Madeira, carvão vegetal e obras de madeira";
                case GeneroProduto.Genero45: return "45  Cortiça e suas obras";
                case GeneroProduto.Genero46: return "46  Obras de espartaria ou de cestaria";
                case GeneroProduto.Genero47: return "47  Pastas de madeira ou de outras matérias fibrosas celulósicas; papel ou cartão de reciclar(desperdícios e aparas)";
                case GeneroProduto.Genero48: return "48  Papel e cartão; obras de pasta de celulose, de papel ou de cartão";
                case GeneroProduto.Genero49: return "49  Livros, jornais, gravuras e outros produtos das indústrias gráficas; textos manuscritos ou datilografados, planos e plantas";
                case GeneroProduto.Genero50: return "50  Seda";
                case GeneroProduto.Genero51: return "51  Lã e pêlos finos ou grosseiros; fios e tecidos de crina";
                case GeneroProduto.Genero52: return "52  Algodão";
                case GeneroProduto.Genero53: return "53  Outras fibras têxteis vegetais; fios de papel e tecido de fios de papel";
                case GeneroProduto.Genero54: return "54  Filamentos sintéticos ou artificiais";
                case GeneroProduto.Genero55: return "55  Fibras sintéticas ou artificiais, descontínuas";
                case GeneroProduto.Genero56: return "56  Pastas('ouates'), feltros e falsos tecidos; fios especiais; cordéis, cordas e cabos; artigos de cordoaria";
                case GeneroProduto.Genero57: return "57  Tapetes e outros revestimentos para pavimentos, de matérias têxteis";
                case GeneroProduto.Genero58: return "58  Tecidos especiais; tecidos tufados; rendas; tapeçarias; passamanarias; bordados";
                case GeneroProduto.Genero59: return "59  Tecidos impregnados, revestidos, recobertos ou estratificados; artigos para usos técnicos de matérias têxteis";
                case GeneroProduto.Genero60: return "60  Tecidos de malha";
                case GeneroProduto.Genero61: return "61  Vestuário e seus acessórios, de malha";
                case GeneroProduto.Genero62: return "62  Vestuário e seus acessórios, exceto de malha";
                case GeneroProduto.Genero63: return "63  Outros artefatos têxteis confeccionados; sortidos; artefatos de matérias têxteis, calçados, chapéus e artefatos de uso semelhante, usados; trapos";
                case GeneroProduto.Genero64: return "64  Calçados, polainas e artefatos semelhantes, e suas partes";
                case GeneroProduto.Genero65: return "65  Chapéus e artefatos de uso semelhante, e suas partes";
                case GeneroProduto.Genero66: return "66  Guarda - chuvas, sombrinhas, guarda - sóis, bengalas, bengalas - assentos, chicotes, e suas partes";
                case GeneroProduto.Genero67: return "67  Penas e penugem preparadas, e suas obras; flores artificiais; obras de cabelo";
                case GeneroProduto.Genero68: return "68  Obras de pedra, gesso, cimento, amianto, mica ou de matérias semelhantes";
                case GeneroProduto.Genero69: return "69  Produtos cerâmicos";
                case GeneroProduto.Genero70: return "70  Vidro e suas obras";
                case GeneroProduto.Genero71: return "71  Pérolas naturais ou cultivadas, pedras preciosas ou semipreciosas e semelhantes, metais preciosos, metais folheados ou chapeados de metais preciosos...";
                case GeneroProduto.Genero72: return "72  Ferro fundido, ferro e aço";
                case GeneroProduto.Genero73: return "73  Obras de ferro fundido, ferro ou aço";
                case GeneroProduto.Genero74: return "74  Cobre e suas obras";
                case GeneroProduto.Genero75: return "75  Níquel e suas obras";
                case GeneroProduto.Genero76: return "76  Alumínio e suas obras";
                case GeneroProduto.Genero77: return "77  (Reservado para uma eventual utilização futura no SH)";
                case GeneroProduto.Genero78: return "78  Chumbo e suas obras";
                case GeneroProduto.Genero79: return "79  Zinco e suas obras";
                case GeneroProduto.Genero80: return "80  Estanho e suas obras";
                case GeneroProduto.Genero81: return "81  Outros metais comuns; ceramais('cermets'); obras dessas matérias";
                case GeneroProduto.Genero82: return "82  Ferramentas, artefatos de cutelaria e talheres, e suas partes, de metais comuns";
                case GeneroProduto.Genero83: return "83  Obras diversas de metais comuns";
                case GeneroProduto.Genero84: return "84  Reatores nucleares, caldeiras, máquinas, aparelhos e instrumentos mecânicos, e suas partes";
                case GeneroProduto.Genero85: return "85  Máquinas, aparelhos e materiais elétricos, e suas partes; aparelhos de gravação ou de reprodução de som, aparelhos de gravação ou de reprodução de imagens e de som em televisão";
                case GeneroProduto.Genero86: return "86  Veículos e material para vias férreas ou semelhantes, e suas partes; aparelhos mecânicos (incluídos os eletromecânicos) de sinalização para vias de comunicação";
                case GeneroProduto.Genero87: return "87  Veículos automóveis, tratores, ciclos e outros veículos terrestres, suas partes e acessórios";
                case GeneroProduto.Genero88: return "88  Aeronaves e aparelhos espaciais, e suas partes";
                case GeneroProduto.Genero89: return "89  Embarcações e estruturas flutuantes";
                case GeneroProduto.Genero90: return "90  Instrumentos e aparelhos de óptica, fotografia ou cinematografia, medida, controle ou de precisão; instrumentos e aparelhos médico-cirúrgicos; suas partes e acessórios";
                case GeneroProduto.Genero91: return "91  Aparelhos de relojoaria e suas partes";
                case GeneroProduto.Genero92: return "92  Instrumentos musicais, suas partes e acessórios";
                case GeneroProduto.Genero93: return "93  Armas e munições; suas partes e acessórios";
                case GeneroProduto.Genero94: return "94  Móveis, mobiliário médico-cirúrgico; colchões; iluminação e construção pré-fabricadas";
                case GeneroProduto.Genero95: return "95  Brinquedos, jogos, artigos para divertimento ou para esporte; suas partes e acessórios";
                case GeneroProduto.Genero96: return "96  Obras diversas";
                case GeneroProduto.Genero97: return "97  Objetos de arte, de coleção e antiguidades";
                case GeneroProduto.Genero98: return "98  (Reservado para usos especiais pelas Partes Contratantes)";
                case GeneroProduto.Genero99: return "99  Operações especiais (utilizado exclusivamente pelo Brasil para classificar operações especiais na exportação)";
                default: return string.Empty;
            }
        }

        public static string ObterSigla(this GeneroProduto generoProduto)
        {
            switch (generoProduto)
            {
                case GeneroProduto.Genero0: return "0";
                case GeneroProduto.Genero1: return "1";
                case GeneroProduto.Genero2: return "2";
                case GeneroProduto.Genero3: return "3";
                case GeneroProduto.Genero4: return "4";
                case GeneroProduto.Genero5: return "5";
                case GeneroProduto.Genero6: return "6";
                case GeneroProduto.Genero7: return "7";
                case GeneroProduto.Genero8: return "8";
                case GeneroProduto.Genero9: return "9";
                case GeneroProduto.Genero10: return "10";
                case GeneroProduto.Genero11: return "11";
                case GeneroProduto.Genero12: return "12";
                case GeneroProduto.Genero13: return "13";
                case GeneroProduto.Genero14: return "14";
                case GeneroProduto.Genero15: return "15";
                case GeneroProduto.Genero16: return "16";
                case GeneroProduto.Genero17: return "17";
                case GeneroProduto.Genero18: return "18";
                case GeneroProduto.Genero19: return "19";
                case GeneroProduto.Genero20: return "20";
                case GeneroProduto.Genero21: return "21";
                case GeneroProduto.Genero22: return "22";
                case GeneroProduto.Genero23: return "23";
                case GeneroProduto.Genero24: return "24";
                case GeneroProduto.Genero25: return "25";
                case GeneroProduto.Genero26: return "26";
                case GeneroProduto.Genero27: return "27";
                case GeneroProduto.Genero28: return "28";
                case GeneroProduto.Genero29: return "29";
                case GeneroProduto.Genero30: return "30";
                case GeneroProduto.Genero31: return "31";
                case GeneroProduto.Genero32: return "32";
                case GeneroProduto.Genero33: return "33";
                case GeneroProduto.Genero34: return "34";
                case GeneroProduto.Genero35: return "35";
                case GeneroProduto.Genero36: return "36";
                case GeneroProduto.Genero37: return "37";
                case GeneroProduto.Genero38: return "38";
                case GeneroProduto.Genero39: return "39";
                case GeneroProduto.Genero40: return "40";
                case GeneroProduto.Genero41: return "41";
                case GeneroProduto.Genero42: return "42";
                case GeneroProduto.Genero43: return "43";
                case GeneroProduto.Genero44: return "44";
                case GeneroProduto.Genero45: return "45";
                case GeneroProduto.Genero46: return "46";
                case GeneroProduto.Genero47: return "47";
                case GeneroProduto.Genero48: return "48";
                case GeneroProduto.Genero49: return "49";
                case GeneroProduto.Genero50: return "50";
                case GeneroProduto.Genero51: return "51";
                case GeneroProduto.Genero52: return "52";
                case GeneroProduto.Genero53: return "53";
                case GeneroProduto.Genero54: return "54";
                case GeneroProduto.Genero55: return "55";
                case GeneroProduto.Genero56: return "56";
                case GeneroProduto.Genero57: return "57";
                case GeneroProduto.Genero58: return "58";
                case GeneroProduto.Genero59: return "59";
                case GeneroProduto.Genero60: return "60";
                case GeneroProduto.Genero61: return "61";
                case GeneroProduto.Genero62: return "62";
                case GeneroProduto.Genero63: return "63";
                case GeneroProduto.Genero64: return "64";
                case GeneroProduto.Genero65: return "65";
                case GeneroProduto.Genero66: return "66";
                case GeneroProduto.Genero67: return "67";
                case GeneroProduto.Genero68: return "68";
                case GeneroProduto.Genero69: return "69";
                case GeneroProduto.Genero70: return "70";
                case GeneroProduto.Genero71: return "71";
                case GeneroProduto.Genero72: return "72";
                case GeneroProduto.Genero73: return "73";
                case GeneroProduto.Genero74: return "74";
                case GeneroProduto.Genero75: return "75";
                case GeneroProduto.Genero76: return "76";
                case GeneroProduto.Genero77: return "77";
                case GeneroProduto.Genero78: return "78";
                case GeneroProduto.Genero79: return "79";
                case GeneroProduto.Genero80: return "80";
                case GeneroProduto.Genero81: return "81";
                case GeneroProduto.Genero82: return "82";
                case GeneroProduto.Genero83: return "83";
                case GeneroProduto.Genero84: return "84";
                case GeneroProduto.Genero85: return "85";
                case GeneroProduto.Genero86: return "86";
                case GeneroProduto.Genero87: return "87";
                case GeneroProduto.Genero88: return "88";
                case GeneroProduto.Genero89: return "89";
                case GeneroProduto.Genero90: return "90";
                case GeneroProduto.Genero91: return "91";
                case GeneroProduto.Genero92: return "92";
                case GeneroProduto.Genero93: return "93";
                case GeneroProduto.Genero94: return "94";
                case GeneroProduto.Genero95: return "95";
                case GeneroProduto.Genero96: return "96";
                case GeneroProduto.Genero97: return "97";
                case GeneroProduto.Genero98: return "98";
                case GeneroProduto.Genero99: return "99";
                default: return string.Empty;
            }
        }
    }
}
