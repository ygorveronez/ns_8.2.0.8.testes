/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumCSTIPI.js" />
/// <reference path="../../Enumeradores/EnumOrigemMercadoria.js" />
/// <reference path="../../Enumeradores/EnumCategoriaProduto.js" />
/// <reference path="../../Enumeradores/EnumGeneroProduto.js" />
/// <reference path="../../Enumeradores/EnumIndicadorEscalaRelevante.js" />
/// <reference path="../../Enumeradores/EnumUnidadeMedida.js" />
/// <reference path="../../Enumeradores/EnumCodigoControleImportacao.js" />
/// <reference path="../../Enumeradores/EnumIndicadorImportacaoCombustivel.js" />
/// <reference path="../../Consultas/GrupoImposto.js" />
/// <reference path="../../Consultas/LocalArmazenamentoProduto.js" />
/// <reference path="../../Consultas/MarcaProduto.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="Foto.js" />
/// <reference path="Armazenamento.js" />
/// <reference path="Pneu.js" />
/// <reference path="EPI.js" />
/// <reference path="Combustivel.js" />
/// <reference path="Composicao.js" />
/// <reference path="BemProduto.js" />
/// <reference path="../ProdutoLote/ProdutoLote.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridProduto;
var _produto;
var _pesquisaProduto;
var _crudProduto;
var _casasValorProdutoNFe;

var _cstIPIVenda = [
    { text: "Selecione", value: 0 },
    { text: "50 - Saída tributada", value: EnumCSTIPI.CST50 },
    { text: "51 - Saída tributada com alíquota zero", value: EnumCSTIPI.CST51 },
    { text: "52 - Saída isenta", value: EnumCSTIPI.CST52 },
    { text: "53 - Saída não-tributada", value: EnumCSTIPI.CST53 },
    { text: "54 - Saída imune", value: EnumCSTIPI.CST54 },
    { text: "55 - Saída com suspensão", value: EnumCSTIPI.CST55 },
    { text: "99 - Outras saídas", value: EnumCSTIPI.CST99 }
];

var _indicadorEscalaRelevante = [
    { text: "Nenhum", value: EnumIndicadorEscalaRelevante.Nenhum },
    { text: "N - Produzido em Escala NÃO Relevante", value: EnumIndicadorEscalaRelevante.ProduzidoEscalaNaoRelevante },
    { text: "S - Produzido em Escala Relevante", value: EnumIndicadorEscalaRelevante.ProduzidoEscalaRelevante }
];

var _indicadorImportacaoCombustivel = [
    { text: "Nenhum", value: EnumIndicadorImportacaoCombustivel.Nenhum },
    { text: "Nacional", value: EnumIndicadorImportacaoCombustivel.Nacional },
    { text: "Importado", value: EnumIndicadorImportacaoCombustivel.Importado }
];

var _cstIPICompra = [
    { text: "Selecione", value: 0 },
    { text: "00 - Entrada com recuperação de crédito", value: EnumCSTIPI.CST00 },
    { text: "01 - Entrada tributada com alíquota zero", value: EnumCSTIPI.CST01 },
    { text: "02 - Entrada isenta", value: EnumCSTIPI.CST02 },
    { text: "03 - Entrada não-tributada", value: EnumCSTIPI.CST03 },
    { text: "04 - Entrada imune", value: EnumCSTIPI.CST04 },
    { text: "05 - Entrada com suspenção", value: EnumCSTIPI.CST05 },
    { text: "49 - Outras entradas", value: EnumCSTIPI.CST49 }
];

var _generoProduto = [
    { text: "Selecione", value: 0 },
    { value: EnumGeneroProduto.Genero0, text: "0   Serviço" },
    { value: EnumGeneroProduto.Genero1, text: "1   Animais vivos" },
    { value: EnumGeneroProduto.Genero2, text: "2   Carnes e miudezas, comestíveis" },
    { value: EnumGeneroProduto.Genero3, text: "3   Peixes e crustáceos, moluscos e os outros invertebrados aquáticos" },
    { value: EnumGeneroProduto.Genero4, text: "4   Leite e laticínios; ovos de aves; mel natural; produtos comestíveis de origem animal, não especificados nem compreendidos em outros Capítulos da TIPI" },
    { value: EnumGeneroProduto.Genero5, text: "5   Outros produtos de origem animal, não especificados nem compreendidos em outros Capítulos da TIPI" },
    { value: EnumGeneroProduto.Genero6, text: "6   Plantas vivas e produtos de floricultura" },
    { value: EnumGeneroProduto.Genero7, text: "7   Produtos hortícolas, plantas, raízes e tubérculos, comestíveis" },
    { value: EnumGeneroProduto.Genero8, text: "8   Frutas; cascas de cítricos e de melões" },
    { value: EnumGeneroProduto.Genero9, text: "9   Café, chá, mate e especiarias" },
    { value: EnumGeneroProduto.Genero10, text: "10  Cereais" },
    { value: EnumGeneroProduto.Genero11, text: "11  Produtos da indústria de moagem; malte; amidos e féculas; inulina; glúten de trigo" },
    { value: EnumGeneroProduto.Genero12, text: "12  Sementes e frutos oleaginosos; grãos, sementes e frutos diversos; plantas industriais ou medicinais; palha e forragem" },
    { value: EnumGeneroProduto.Genero13, text: "13  Gomas, resinas e outros sucos e extratos vegetais" },
    { value: EnumGeneroProduto.Genero14, text: "14  Matérias para entrançar e outros produtos de origem vegetal, não especificadas nem compreendidas em outros Capítulos da NCM" },
    { value: EnumGeneroProduto.Genero15, text: "15  Gorduras e óleos animais ou vegetais; produtos da sua dissociação; gorduras alimentares elaboradas; ceras de origem animal ou vegetal" },
    { value: EnumGeneroProduto.Genero16, text: "16  Preparações de carne, de peixes ou de crustáceos, de moluscos ou de outros invertebrados aquáticos" },
    { value: EnumGeneroProduto.Genero17, text: "17  Açúcares e produtos de confeitaria" },
    { value: EnumGeneroProduto.Genero18, text: "18  Cacau e suas preparações" },
    { value: EnumGeneroProduto.Genero19, text: "19  Preparações à base de cereais, farinhas, amidos, féculas ou de leite; produtos de pastelaria" },
    { value: EnumGeneroProduto.Genero20, text: "20  Preparações de produtos hortícolas, de frutas ou de outras partes de plantas" },
    { value: EnumGeneroProduto.Genero21, text: "21  Preparações alimentícias diversas" },
    { value: EnumGeneroProduto.Genero22, text: "22  Bebidas, líquidos alcoólicos e vinagres" },
    { value: EnumGeneroProduto.Genero23, text: "23  Resíduos e desperdícios das indústrias alimentares; alimentos preparados para animais" },
    { value: EnumGeneroProduto.Genero24, text: "24  Fumo(tabaco) e seus sucedâneos, manufaturados" },
    { value: EnumGeneroProduto.Genero25, text: "25  Sal; enxofre; terras e pedras; gesso, cal e cimento" },
    { value: EnumGeneroProduto.Genero26, text: "26  Minérios, escórias e cinzas" },
    { value: EnumGeneroProduto.Genero27, text: "27  Combustíveis minerais, óleos minerais e produtos de sua destilação; matérias betuminosas; ceras minerais" },
    { value: EnumGeneroProduto.Genero28, text: "28  Produtos químicos inorgânicos; compostos inorgânicos ou orgânicos de metais preciosos, de elementos radioativos, de metais das terras raras ou de isótopos" },
    { value: EnumGeneroProduto.Genero29, text: "29  Produtos químicos orgânicos" },
    { value: EnumGeneroProduto.Genero30, text: "30  Produtos farmacêuticos" },
    { value: EnumGeneroProduto.Genero31, text: "31  Adubos ou fertilizantes" },
    { value: EnumGeneroProduto.Genero32, text: "32  Extratos tanantes e tintoriais; taninos e seus derivados; pigmentos e outras matérias corantes, tintas e vernizes, mástiques; tintas de escrever" },
    { value: EnumGeneroProduto.Genero33, text: "33  Óleos essenciais e resinóides; produtos de perfumaria ou de toucador preparados e preparações cosméticas" },
    { value: EnumGeneroProduto.Genero34, text: "34  Sabões, agentes orgânicos de superfície, preparações para lavagem, preparações lubrificantes, ceras artificiais, ceras preparadas..." },
    { value: EnumGeneroProduto.Genero35, text: "35  Matérias albuminóides; produtos à base de amidos ou de féculas modificados; colas; enzimas" },
    { value: EnumGeneroProduto.Genero36, text: "36  Pólvoras e explosivos; artigos de pirotecnia; fósforos; ligas pirofóricas; matérias inflamáveis" },
    { value: EnumGeneroProduto.Genero37, text: "37  Produtos para fotografia e cinematografia" },
    { value: EnumGeneroProduto.Genero38, text: "38  Produtos diversos das indústrias químicas" },
    { value: EnumGeneroProduto.Genero39, text: "39  Plásticos e suas obras" },
    { value: EnumGeneroProduto.Genero40, text: "40  Borracha e suas obras" },
    { value: EnumGeneroProduto.Genero41, text: "41  Peles, exceto a peleteria(peles com pêlo *), e couros" },
    { value: EnumGeneroProduto.Genero42, text: "42  Obras de couro; artigos de correeiro ou de seleiro; artigos de viagem, bolsas e artefatos semelhantes; obras de tripa" },
    { value: EnumGeneroProduto.Genero43, text: "43  Peleteria(peles com pêlo *) e suas obras; peleteria(peles com pêlo *) artificial" },
    { value: EnumGeneroProduto.Genero44, text: "44  Madeira, carvão vegetal e obras de madeira" },
    { value: EnumGeneroProduto.Genero45, text: "45  Cortiça e suas obras" },
    { value: EnumGeneroProduto.Genero46, text: "46  Obras de espartaria ou de cestaria" },
    { value: EnumGeneroProduto.Genero47, text: "47  Pastas de madeira ou de outras matérias fibrosas celulósicas; papel ou cartão de reciclar(desperdícios e aparas)" },
    { value: EnumGeneroProduto.Genero48, text: "48  Papel e cartão; obras de pasta de celulose, de papel ou de cartão" },
    { value: EnumGeneroProduto.Genero49, text: "49  Livros, jornais, gravuras e outros produtos das indústrias gráficas; textos manuscritos ou datilografados, planos e plantas" },
    { value: EnumGeneroProduto.Genero50, text: "50  Seda" },
    { value: EnumGeneroProduto.Genero51, text: "51  Lã e pêlos finos ou grosseiros; fios e tecidos de crina" },
    { value: EnumGeneroProduto.Genero52, text: "52  Algodão" },
    { value: EnumGeneroProduto.Genero53, text: "53  Outras fibras têxteis vegetais; fios de papel e tecido de fios de papel" },
    { value: EnumGeneroProduto.Genero54, text: "54  Filamentos sintéticos ou artificiais" },
    { value: EnumGeneroProduto.Genero55, text: "55  Fibras sintéticas ou artificiais, descontínuas" },
    { value: EnumGeneroProduto.Genero56, text: "56  Pastas('ouates'), feltros e falsos tecidos; fios especiais; cordéis, cordas e cabos; artigos de cordoaria" },
    { value: EnumGeneroProduto.Genero57, text: "57  Tapetes e outros revestimentos para pavimentos, de matérias têxteis" },
    { value: EnumGeneroProduto.Genero58, text: "58  Tecidos especiais; tecidos tufados; rendas; tapeçarias; passamanarias; bordados" },
    { value: EnumGeneroProduto.Genero59, text: "59  Tecidos impregnados, revestidos, recobertos ou estratificados; artigos para usos técnicos de matérias têxteis" },
    { value: EnumGeneroProduto.Genero60, text: "60  Tecidos de malha" },
    { value: EnumGeneroProduto.Genero61, text: "61  Vestuário e seus acessórios, de malha" },
    { value: EnumGeneroProduto.Genero62, text: "62  Vestuário e seus acessórios, exceto de malha" },
    { value: EnumGeneroProduto.Genero63, text: "63  Outros artefatos têxteis confeccionados; sortidos; artefatos de matérias têxteis, calçados, chapéus e artefatos de uso semelhante, usados; trapos" },
    { value: EnumGeneroProduto.Genero64, text: "64  Calçados, polainas e artefatos semelhantes, e suas partes" },
    { value: EnumGeneroProduto.Genero65, text: "65  Chapéus e artefatos de uso semelhante, e suas partes" },
    { value: EnumGeneroProduto.Genero66, text: "66  Guarda - chuvas, sombrinhas, guarda - sóis, bengalas, bengalas - assentos, chicotes, e suas partes" },
    { value: EnumGeneroProduto.Genero67, text: "67  Penas e penugem preparadas, e suas obras; flores artificiais; obras de cabelo" },
    { value: EnumGeneroProduto.Genero68, text: "68  Obras de pedra, gesso, cimento, amianto, mica ou de matérias semelhantes" },
    { value: EnumGeneroProduto.Genero69, text: "69  Produtos cerâmicos" },
    { value: EnumGeneroProduto.Genero70, text: "70  Vidro e suas obras" },
    { value: EnumGeneroProduto.Genero71, text: "71  Pérolas naturais ou cultivadas, pedras preciosas ou semipreciosas e semelhantes, metais preciosos, metais folheados ou chapeados de metais preciosos..." },
    { value: EnumGeneroProduto.Genero72, text: "72  Ferro fundido, ferro e aço" },
    { value: EnumGeneroProduto.Genero73, text: "73  Obras de ferro fundido, ferro ou aço" },
    { value: EnumGeneroProduto.Genero74, text: "74  Cobre e suas obras" },
    { value: EnumGeneroProduto.Genero75, text: "75  Níquel e suas obras" },
    { value: EnumGeneroProduto.Genero76, text: "76  Alumínio e suas obras" },
    { value: EnumGeneroProduto.Genero77, text: "77  (Reservado para uma eventual utilização futura no SH)" },
    { value: EnumGeneroProduto.Genero78, text: "78  Chumbo e suas obras" },
    { value: EnumGeneroProduto.Genero79, text: "79  Zinco e suas obras" },
    { value: EnumGeneroProduto.Genero80, text: "80  Estanho e suas obras" },
    { value: EnumGeneroProduto.Genero81, text: "81  Outros metais comuns; ceramais('cermets'); obras dessas matérias" },
    { value: EnumGeneroProduto.Genero82, text: "82  Ferramentas, artefatos de cutelaria e talheres, e suas partes, de metais comuns" },
    { value: EnumGeneroProduto.Genero83, text: "83  Obras diversas de metais comuns" },
    { value: EnumGeneroProduto.Genero84, text: "84  Reatores nucleares, caldeiras, máquinas, aparelhos e instrumentos mecânicos, e suas partes" },
    { value: EnumGeneroProduto.Genero85, text: "85  Máquinas, aparelhos e materiais elétricos, e suas partes; aparelhos de gravação ou de reprodução de som, aparelhos de gravação ou de reprodução de imagens e de som em televisão" },
    { value: EnumGeneroProduto.Genero86, text: "86  Veículos e material para vias férreas ou semelhantes, e suas partes; aparelhos mecânicos (incluídos os eletromecânicos) de sinalização para vias de comunicação" },
    { value: EnumGeneroProduto.Genero87, text: "87  Veículos automóveis, tratores, ciclos e outros veículos terrestres, suas partes e acessórios" },
    { value: EnumGeneroProduto.Genero88, text: "88  Aeronaves e aparelhos espaciais, e suas partes" },
    { value: EnumGeneroProduto.Genero89, text: "89  Embarcações e estruturas flutuantes" },
    { value: EnumGeneroProduto.Genero90, text: "90  Instrumentos e aparelhos de óptica, fotografia ou cinematografia, medida, controle ou de precisão; instrumentos e aparelhos médico-cirúrgicos; suas partes e acessórios" },
    { value: EnumGeneroProduto.Genero91, text: "91  Aparelhos de relojoaria e suas partes" },
    { value: EnumGeneroProduto.Genero92, text: "92  Instrumentos musicais, suas partes e acessórios" },
    { value: EnumGeneroProduto.Genero93, text: "93  Armas e munições; suas partes e acessórios" },
    { value: EnumGeneroProduto.Genero94, text: "94  Móveis, mobiliário médico-cirúrgico; colchões; iluminação e construção pré-fabricadas" },
    { value: EnumGeneroProduto.Genero95, text: "95  Brinquedos, jogos, artigos para divertimento ou para esporte; suas partes e acessórios" },
    { value: EnumGeneroProduto.Genero96, text: "96  Obras diversas" },
    { value: EnumGeneroProduto.Genero97, text: "97  Objetos de arte, de coleção e antiguidades" },
    { value: EnumGeneroProduto.Genero98, text: "98  (Reservado para usos especiais pelas Partes Contratantes)" },
    { value: EnumGeneroProduto.Genero99, text: "99  Operações especiais (utilizado exclusivamente pelo Brasil para classificar operações especiais na exportação)" }
];

var _statusProduto = [
    { text: "Ativo", value: "A" },
    { text: "Inativo", value: "I" }
];

var PesquisaProduto = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.CodigoProduto = PropertyEntity({ text: "Cód. Produto: " });
    this.CodigoBarrasEAN = PropertyEntity({ text: "Cód. EAN: " });
    this.Codigo = PropertyEntity({ text: "Código: ", getType: typesKnockout.int });
    this.CodigoNCM = PropertyEntity({ text: "Cód. NCM: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Status: " });
    this.LocalArmazenamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local de Armazenamento:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridProduto.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var Produto = function () {
    this.Codigo = PropertyEntity({ text: "Código: ", val: ko.observable(0), def: 0, getType: typesKnockout.int, enabled: ko.observable(false), visible: true, enable: false });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 200 });
    this.CodigoAnvisa = PropertyEntity({ text: "Código ANVISA: ", required: false, maxlength: 13 });
    this.CodigoProduto = PropertyEntity({ text: "Cód. Produto: ", required: false, maxlength: 50 });
    this.CodigoBarrasEAN = PropertyEntity({ text: "Cód. Barras EAN: ", required: false, maxlength: 14 });
    this.CodigoNCM = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("*NCM:"), idBtnSearch: guid(), enable: ko.observable(true), issue: 139 });
    this.UnidadeMedida = PropertyEntity({ val: ko.observable(EnumUnidadeMedida.Quilograma), options: EnumUnidadeMedida.obterOpcoes(), text: "*Unidade de Medida: ", def: EnumUnidadeMedida.Quilograma, issue: 88 });
    this.Status = PropertyEntity({ val: ko.observable("A"), options: _statusProduto, def: "A", text: "*Status: " });

    this.DescricaoNotaFiscal = PropertyEntity({ text: "*Descrição na Nota Fiscal: ", required: true, maxlength: 120, val: ko.observable("") });
    this.OrigemMercadoria = PropertyEntity({ text: "*Origem Mercadoria: ", val: ko.observable(EnumOrigemMercadoria.Origem0), options: EnumOrigemMercadoria.obterOpcoesCadastro(), def: EnumOrigemMercadoria.Origem0 });
    this.UltimoCusto = PropertyEntity({ text: "Último Custo: ", required: false, getType: typesKnockout.decimal, maxlength: 22, configDecimal: { precision: 4, allowZero: true }, val: ko.observable("") });
    this.CustoMedio = PropertyEntity({ text: "Custo Médio: ", required: false, getType: typesKnockout.decimal, maxlength: 22, configDecimal: { precision: 4, allowZero: true }, val: ko.observable("") });
    this.MargemLucro = PropertyEntity({ val: ko.observable("100,00"), text: "*Margem Lucro: ", def: "100,00", required: true, getType: typesKnockout.decimal, maxlength: 18 });
    this.ValorVenda = PropertyEntity({ def: "0,00000", val: ko.observable("0,00000"), text: "Valor Venda: ", required: false, getType: typesKnockout.decimal, maxlength: 22, configDecimal: { precision: _casasValorProdutoNFe, allowZero: true } });
    this.PesoBruto = PropertyEntity({ text: "Peso Bruto: ", getType: typesKnockout.decimal, maxlength: 18 });
    this.PesoLiquido = PropertyEntity({ text: "Peso Líquido: ", getType: typesKnockout.decimal, maxlength: 18 });
    this.CodigoEAN = PropertyEntity({ text: "Código de Barras: ", maxlength: 14 });
    this.CodigoEnquadramentoIPI = PropertyEntity({ text: "Cód. Enquadramento IPI: ", maxlength: 5 });
    this.CSTIPIVenda = PropertyEntity({ val: ko.observable(0), options: _cstIPIVenda, def: 0, text: "CST IPI Venda: ", required: false });
    this.AliquotaIPIVenda = PropertyEntity({ text: "Alíquota IPI Venda: ", getType: typesKnockout.decimal, maxlength: 18 });
    this.CSTIPICompra = PropertyEntity({ val: ko.observable(0), options: _cstIPICompra, def: 0, text: "CST IPI Compra: ", required: false });
    this.AliquotaIPICompra = PropertyEntity({ text: "Alíquota IPI Compra: ", getType: typesKnockout.decimal, maxlength: 18 });
    this.CategoriaProduto = PropertyEntity({ val: ko.observable(EnumCategoriaProduto.MercadoriaRevenda), options: EnumCategoriaProduto.obterOpcoesComNumero(), def: EnumCategoriaProduto.MercadoriaRevenda, text: "*Tipo Item: " });
    this.GeneroProduto = PropertyEntity({ val: ko.observable(0), options: _generoProduto, def: 0, text: "Gênero Produto: " });
    this.CodigoCEST = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("CEST:"), idBtnSearch: guid(), enable: ko.observable(true) });

    this.ValorMinimoVenda = PropertyEntity({ text: "Valor Mínimo Venda:", getType: typesKnockout.decimal, maxlength: 18, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false } });   
    this.GrupoProdutoTMS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Produto:", idBtnSearch: guid(), required: false, visible: true });
    this.GrupoImposto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo Imposto:", idBtnSearch: guid(), required: false, visible: true });
    this.LocalArmazenamentoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Local de Armazenamento:"), idBtnSearch: guid(), required: ko.observable(false), visible: true });
    this.MarcaProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Marca:", idBtnSearch: guid(), required: false, visible: true });
    this.FinalidadeProdutoOrdemServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Finalidade do Produto na OS:", idBtnSearch: guid(), required: false, visible: true });

    this.IndicadorEscalaRelevante = PropertyEntity({ val: ko.observable(EnumIndicadorEscalaRelevante.Nenhum), options: _indicadorEscalaRelevante, def: EnumIndicadorEscalaRelevante.Nenhum, text: "Indicador Escala Relevante: ", required: false });    
    this.CNPJFabricante = PropertyEntity({ text: "CNPJ Fabricante: ", required: false, maxlength: 20, getType: typesKnockout.cnpj });
    this.CodigoBeneficioFiscal = PropertyEntity({ text: "Código Beneficio Fiscal: ", required: false, maxlength: 50 });
    this.CalculoCustoProduto = PropertyEntity({ text: "Custo: ", required: false, maxlength: 5000, visible: false, val: ko.observable(""), def: ko.observable("") });

    //this.CalculoCustoProduto = PropertyEntity({ text: "Fórmula para o custo do produto: *Informe os complementos para o cálculo: ((Valor Total do Item) + ......... )", required: false, maxlength: 300, visible: ko.observable(true), val: ko.observable("#+ #ValorDiferencial #+ #ValorICMSST #+ #ValorIPI #+ #ValorFrete #+ #ValorSeguro #+ #ValorOutras #- #ValorDesconto #- #ValorDescontoFora #+ #ValorImpostoFora #+ #ValorOutrasFora #+ #ValorFreteFora #- #ValorICMSFreteFora #+ #ValorDiferencialFreteFora"), def: "#+ #ValorDiferencial #+ #ValorICMSST #+ #ValorIPI #+ #ValorFrete #+ #ValorSeguro #+ #ValorOutras #- #ValorDesconto #- #ValorDescontoFora #+ #ValorImpostoFora #+ #ValorOutrasFora #+ #ValorFreteFora #- #ValorICMSFreteFora #+ #ValorDiferencialFreteFora", enable: ko.observable(true) });

    this.Fornecedores = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Foto = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.Pneu = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.Armazenamento = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.EPI = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.Combustivel = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.Bem = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });

    this.ProdutoKIT = PropertyEntity({ getType: typesKnockout.dynamic });
    this.Composicoes = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    //this.CalculoCustoProduto.val.subscribe(function (novoValor) {
    //    _custoProduto.CalculoCustoProduto.val(_produto.CalculoCustoProduto.val());
    //});
};

var CRUDProduto = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.ImportarLote = PropertyEntity({ eventClick: importarProdutoLoteClick, type: types.event, text: "Importar Lote", visible: ko.observable(false) });
    this.GerarEtiqueta = PropertyEntity({ eventClick: GerarEtiquetaClik, type: types.event, text: "Gerar Etiqueta", visible: ko.observable(false) });



    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",

        UrlImportacao: "Produto/Importar",
        UrlConfiguracao: "Produto/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O015_Produto,
        CallbackImportacao: function () {
            _gridProduto.CarregarGrid();
        }
    });
};

//*******EVENTOS*******

function loadProduto() {
    executarReST("NotaFiscalEletronica/BuscarDadosEmpresa", null, function (r) {
        if (r.Success) {
            _casasValorProdutoNFe = r.Data.CasasValorProdutoNFe;

            _produto = new Produto();
            KoBindings(_produto, "knockoutCadastroProduto");

            HeaderAuditoria("Produto", _produto);

            _crudProduto = new CRUDProduto();
            KoBindings(_crudProduto, "knockoutCRUDProduto");

            _pesquisaProduto = new PesquisaProduto();
            KoBindings(_pesquisaProduto, "knockoutPesquisaProduto", false, _pesquisaProduto.Pesquisar.id);

            $("#" + _produto.CodigoNCM.id).mask("00000000", { selectOnFocus: true, clearIfNotMatch: true });

            new BuscarNCMS(_produto.CodigoNCM, retornoSelecaoNCM);
            new BuscarGrupoImposto(_produto.GrupoImposto, null, _produto.CodigoNCM);
            new BuscarGruposProdutosTMS(_produto.GrupoProdutoTMS, null);
            new BuscarCESTS(_produto.CodigoCEST, retornoSelecaoCEST, _produto.CodigoNCM);
            new BuscarLocalArmazenamentoProduto(_produto.LocalArmazenamentoProduto);
            new BuscarMarcaProduto(_produto.MarcaProduto);
            new BuscarLocalArmazenamentoProduto(_pesquisaProduto.LocalArmazenamento);
            new BuscarFinalidadeProdutoOrdemServico(_produto.FinalidadeProdutoOrdemServico);

            buscarProdutos();

            LoadCustoProduto();
            LoadFornecedorProduto();
            LoadArmazenamento();
            loadProdutoFoto();
            loadProdutoPneu();
            loadProdutoEPI();
            loadProdutoCombustivel();
            loadProdutoComposicao();
            loadProdutoBem();

            if (r.Data.CasasValorProdutoNFe === 0) {
                _produto.ValorVenda.def = "0";
                _produto.ValorVenda.val("0");
                _produto.ValorVenda.configDecimal = { precision: 0, allowZero: true };
            } else if (r.Data.CasasValorProdutoNFe === 1) {
                _produto.ValorVenda.def = "0,0";
                _produto.ValorVenda.val("0,0");
                _produto.ValorVenda.configDecimal = { precision: 1, allowZero: true };
            } else if (r.Data.CasasValorProdutoNFe === 2) {
                _produto.ValorVenda.def = "0,00";
                _produto.ValorVenda.val("0,00");
                _produto.ValorVenda.configDecimal = { precision: 2, allowZero: true };
            } else if (r.Data.CasasValorProdutoNFe === 3) {
                _produto.ValorVenda.def = "0,000";
                _produto.ValorVenda.val("0,000");
                _produto.ValorVenda.configDecimal = { precision: 3, allowZero: true };
            } else if (r.Data.CasasValorProdutoNFe === 4) {
                _produto.ValorVenda.def = "0,0000";
                _produto.ValorVenda.val("0,0000");
                _produto.ValorVenda.configDecimal = { precision: 4, allowZero: true };
            } else if (r.Data.CasasValorProdutoNFe === 5) {
                _produto.ValorVenda.def = "0,00000";
                _produto.ValorVenda.val("0,00000");
                _produto.ValorVenda.configDecimal = { precision: 5, allowZero: true };
            } else if (r.Data.CasasValorProdutoNFe === 6) {
                _produto.ValorVenda.def = "0,000000";
                _produto.ValorVenda.val("0,000000");
                _produto.ValorVenda.configDecimal = { precision: 6, allowZero: true };
            } else if (r.Data.CasasValorProdutoNFe === 7) {
                _produto.ValorVenda.def = "0,0000000";
                _produto.ValorVenda.val("0,0000000");
                _produto.ValorVenda.configDecimal = { precision: 7, allowZero: true };
            } else if (r.Data.CasasValorProdutoNFe === 8) {
                _produto.ValorVenda.def = "0,00000000";
                _produto.ValorVenda.val("0,00000000");
                _produto.ValorVenda.configDecimal = { precision: 8, allowZero: true };
            } else if (r.Data.CasasValorProdutoNFe === 9) {
                _produto.ValorVenda.def = "0,000000000";
                _produto.ValorVenda.val("0,000000000");
                _produto.ValorVenda.configDecimal = { precision: 9, allowZero: true };
            } else if (r.Data.CasasValorProdutoNFe === 10) {
                _produto.ValorVenda.def = "0,0000000000";
                _produto.ValorVenda.val("0,0000000000");
                _produto.ValorVenda.configDecimal = { precision: 10, allowZero: true };
            } else {
                _produto.ValorVenda.def = "0,00000";
                _produto.ValorVenda.val("0,00000");
                _produto.ValorVenda.configDecimal = { precision: 5, allowZero: true };
            }

            if (_CONFIGURACAO_TMS.UtilizaMultiplosLocaisArmazenamento) {
                _produto.LocalArmazenamentoProduto.required(true);
                _produto.LocalArmazenamentoProduto.text("*Local de Armazenamento:");
            }

            BuscarConfiguracoesEmpresa();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });

}

function AdicionarCalculoPadraoClick(e, sender) {
    _custoProduto.CalculoCustoProduto.val(_CONFIGURACAO_TMS.FormulaCustoPadrao);
    //_custoProduto.CalculoCustoProduto.val("#+ #ValorDiferencial #+ #ValorICMSST #+ #ValorIPI #+ #ValorFrete #+ #ValorSeguro #+ #ValorOutras #- #ValorDesconto #- #ValorDescontoFora #+ #ValorImpostoFora #+ #ValorOutrasFora #+ #ValorFreteFora #- #ValorICMSFreteFora #+ #ValorDiferencialFreteFora");
}

function retornoSelecaoCEST(data) {
    _produto.CodigoCEST.val(data.CEST);
    _produto.CodigoCEST.codEntity(data.CEST);
}

function codigoDescricaoExit() {
    if ($("#" + _produto.Descricao.id).val() != "" && _produto.DescricaoNotaFiscal.val() == "") {
        _produto.DescricaoNotaFiscal.val($("#" + _produto.Descricao.id).val().substring(0, 120));
    }
}

function codigoNCMExit() {
    if ($("#" + _produto.CodigoNCM.id).val() != "") {
        _produto.CodigoNCM.codEntity($("#" + _produto.CodigoNCM.id).val());
        //_produto.CodigoNCM.val($("#" + _produto.CodigoNCM.id).val());
    }
}

function retornoSelecaoNCM(e, sender) {
    _produto.CodigoNCM.val(e.Descricao);
    _produto.CodigoNCM.codEntity(e.Descricao);
    var generoProduto = parseInt(e.Descricao.substring(0, 2));
    _produto.GeneroProduto.val(generoProduto + 1);
}

function adicionarClick(e, sender) {
    resetarTabs();
    var valido = true;

    _produto.CodigoNCM.requiredClass("form-control ");
    if (_produto.CodigoNCM.val() == "") {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe o NCM do produto!");
        _produto.CodigoNCM.requiredClass("form-control  is-invalid");
        return;
    } else if (_produto.CodigoAnvisa.val() != "" && _produto.CodigoAnvisa.val() != null && _produto.CodigoAnvisa.val().length < 13) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informar os 13 dígitos do Código ANVISA do produto!");
        _produto.CodigoAnvisa.requiredClass("form-control  is-invalid");
        return;
    } else if (_produtoCombustivel.CodigoANP.val() != "" && _produtoCombustivel.CodigoANP.val() != null && _produtoCombustivel.CodigoANP.val().length < 9) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informar os 9 dígitos do Código ANP do produto na aba Combustível!");
        _produtoCombustivel.CodigoANP.requiredClass("form-control  is-invalid");
        return;
    } else if (!validaCamposObrigatoriosFoto()) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe todos os campos da foto");
        return;
    } else if (!validaCamposObrigatoriosPneu()) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe todos os campos do Pneu");
        return;
    } else if (!validaCamposObrigatoriosEPI()) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe todos os campos do EPI");
        return;
    } else if (!validaCamposObrigatoriosComposicao()) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe pelo menos um Insumo na Composição");
        return;
    } else if (!validaCamposObrigatoriosBem()) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe todos os campos do Bem");
        return;
    }

    if (valido && ValidarEnumeradoresObrigatorios()) {
        _produto.Foto.val(JSON.stringify(RetornarObjetoPesquisa(_produtoFoto)));
        _produto.Pneu.val(JSON.stringify(RetornarObjetoPesquisa(_produtoPneu)));
        _produto.EPI.val(JSON.stringify(RetornarObjetoPesquisa(_produtoEPI)));
        _produto.Combustivel.val(JSON.stringify(RetornarObjetoPesquisa(_produtoCombustivel)));
        _produto.ProdutoKIT.val(_produtoComposicao.ProdutoKIT.val());
        _produto.Bem.val(JSON.stringify(RetornarObjetoPesquisa(_produtoBem)));

        _produto.CalculoCustoProduto.val(_custoProduto.CalculoCustoProduto.val());
        Salvar(_produto, "Produto/Adicionar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    if (adicionarFoto()) {
                        EnviarFoto(arg.Data.Codigo);
                    } else {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                        _gridProduto.CarregarGrid();
                        limparCamposProduto();
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    }
}

function atualizarClick(e, sender) {
    resetarTabs();
    var valido = true;

    _produto.CodigoNCM.requiredClass("form-control ");
    if (_produto.CodigoNCM.val() == "") {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe o NCM do produto!");
        _produto.CodigoNCM.requiredClass("form-control  is-invalid");
        return;
    } else if (_produto.CodigoAnvisa.val() != "" && _produto.CodigoAnvisa.val() != null && _produto.CodigoAnvisa.val().length < 13) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informar os 13 dígitos do Código ANVISA do produto!");
        _produto.CodigoAnvisa.requiredClass("form-control  is-invalid");
        return;
    } else if (_produtoCombustivel.CodigoANP.val() != "" && _produtoCombustivel.CodigoANP.val() != null && _produtoCombustivel.CodigoANP.val().length < 9) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informar os 9 dígitos do Código ANP do produto na aba Combustível!");
        _produtoCombustivel.CodigoANP.requiredClass("form-control  is-invalid");
        return;
    } else if (!validaCamposObrigatoriosFoto()) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por Favor, Informe todos os campos da foto");
        return;
    } else if (!validaCamposObrigatoriosPneu()) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por Favor, Informe todos os campos do Pneu");
        return;
    } else if (!validaCamposObrigatoriosEPI()) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por Favor, Informe todos os campos do EPI");
        return;
    } else if (!validaCamposObrigatoriosComposicao()) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por Favor, Informe pelo menos um Insumo na Composição");
        return;
    } else if (!validaCamposObrigatoriosBem()) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por Favor, Informe todos os campos do Bem");
        return;
    }

    if (valido && ValidarEnumeradoresObrigatorios()) {
        _produto.CalculoCustoProduto.val(_custoProduto.CalculoCustoProduto.val());
        _produto.Pneu.val(JSON.stringify(RetornarObjetoPesquisa(_produtoPneu)));
        _produto.EPI.val(JSON.stringify(RetornarObjetoPesquisa(_produtoEPI)));
        _produto.Combustivel.val(JSON.stringify(RetornarObjetoPesquisa(_produtoCombustivel)));
        _produto.ProdutoKIT.val(_produtoComposicao.ProdutoKIT.val());
        _produto.Bem.val(JSON.stringify(RetornarObjetoPesquisa(_produtoBem)));

        Salvar(_produto, "Produto/Atualizar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    if ((_produtoFoto.Codigo.val() > 0) && (!temFoto())) {
                        InativarFoto();
                    }
                    if (adicionarFoto()) {
                        EnviarFoto(_produto.Codigo.val());
                    } else {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                        _gridProduto.CarregarGrid();
                        limparCamposProduto();
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    }
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o produto " + _produto.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_produto, "Produto/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridProduto.CarregarGrid();
                    limparCamposProduto();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposProduto();
}

function calculaValorVenda() {
    var valorVenda = 0;
    var ultimoCusto = parseFloat(formatarStrFloat(Globalize.format(_produto.UltimoCusto.val(), "n4")));
    var margemLucro = parseFloat(formatarStrFloat(Globalize.format($("#" + _produto.MargemLucro.id).val())));

    if (ultimoCusto > 0 && margemLucro > 0) {
        valorVenda = (ultimoCusto + (ultimoCusto * (margemLucro / 100)));
        _produto.ValorVenda.val(Globalize.format(valorVenda, "n" + _casasValorProdutoNFe));
    }
}

function calculaMargemLucro() {
    var margemLucro = 0;
    var ultimoCusto = parseFloat(formatarStrFloat(Globalize.format(_produto.UltimoCusto.val(), "n4")));
    var valorVenda = parseFloat(formatarStrFloat(Globalize.format($("#" + _produto.ValorVenda.id).val())));

    if (ultimoCusto > 0 && valorVenda > 0) {
        if (valorVenda >= ultimoCusto) {
            margemLucro = (((valorVenda - ultimoCusto) * 100) / ultimoCusto);
            _produto.MargemLucro.val(Globalize.format(margemLucro, "n2"));
        }
    }
}

function importarProdutoLoteClick() {
    new LancarProdutoLote();
}

//*******MÉTODOS*******

function EnviarFoto(codigoProduto) {
    var file;
    var documentos = new Array();

    file = document.getElementById(_produtoFoto.Arquivo.id);
    file = file.files[0];
    var formData = new FormData();
    formData.append("upload", file);
    var data = {
        DescricaoFoto: _produtoFoto.DescricaoFoto.val(),
        CodigoProduto: codigoProduto
    };
    enviarArquivo("Produto/EnviarFoto?callback=?", data, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Foto também inserida com sucesso");
                _gridProduto.CarregarGrid();
                limparCamposProduto();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function InativarFoto(e, sender) {
    Salvar(_produtoFoto, "Produto/InativarFoto", function (arg) {
        if (arg.Success) {
            if (arg.Data) {

            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function BuscarConfiguracoesEmpresa() {
    executarReST("Usuario/DadosUsuarioLogado", null, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _crudProduto.ImportarLote.visible(arg.Data.Empresa.HabilitaLancamentoProdutoLote);
            }
        }
    });
}

function buscarProdutos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarProduto, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridProduto = new GridView(_pesquisaProduto.Pesquisar.idGrid, "Produto/Pesquisa", _pesquisaProduto, menuOpcoes, null);
    _gridProduto.CarregarGrid();
}

function editarProduto(produtoGrid) {
    limparCamposProduto();
    _produto.Codigo.val(produtoGrid.Codigo);
    BuscarPorCodigo(_produto, "Produto/BuscarPorCodigo", function (arg) {
        _pesquisaProduto.ExibirFiltros.visibleFade(false);
        _crudProduto.Atualizar.visible(true);
        _crudProduto.Cancelar.visible(true);
        _crudProduto.Excluir.visible(true);
        _crudProduto.GerarEtiqueta.visible(true);
        _crudProduto.Adicionar.visible(false);
        _produto.CodigoNCM.val(arg.Data.CodigoNCM);
        _produto.CodigoNCM.codEntity(arg.Data.CodigoNCM);
        _produto.CodigoCEST.val(arg.Data.CodigoCEST);
        _produto.CodigoCEST.codEntity(arg.Data.CodigoCEST);
        _custoProduto.CalculoCustoProduto.val(arg.Data.CalculoCustoProduto);

        $("#liTabArmazenamento").show();
        EditarArmazenamento();

        RecarregarGridFornecedorProduto();
        RecarregarGridComposicaoProduto();

        if (arg.Data.Foto.length > 0) {
            PreencherObjetoKnout(_produtoFoto, { Data: arg.Data.Foto[0] });

            if (_produtoFoto.FotoProduto.val() != "")
                _produtoFoto.DescricaoFoto.enable(true);
            else
                _produtoFoto.DescricaoFoto.enable(false);
        }

        if (arg.Data.Pneu !== null && arg.Data.Pneu !== undefined) {
            PreencherObjetoKnout(_produtoPneu, { Data: arg.Data.Pneu });
        }
        if (arg.Data.EPI !== null && arg.Data.EPI !== undefined) {
            PreencherObjetoKnout(_produtoEPI, { Data: arg.Data.EPI });
        }
        if (arg.Data.Combustivel !== null && arg.Data.Combustivel !== undefined) {
            PreencherObjetoKnout(_produtoCombustivel, { Data: arg.Data.Combustivel });
        }
        if (arg.Data.Composicao !== null && arg.Data.Composicao !== undefined) {
            PreencherObjetoKnout(_produtoComposicao, { Data: arg.Data.Composicao });
        }
        if (arg.Data.Bem !== null && arg.Data.Bem !== undefined) {
            PreencherObjetoKnout(_produtoBem, { Data: arg.Data.Bem });
        }
    }, null);
}

function limparCamposProduto() {
    _crudProduto.Atualizar.visible(false);
    _crudProduto.Cancelar.visible(false);
    _crudProduto.Excluir.visible(false);
    _crudProduto.Adicionar.visible(true);
    LimparCampos(_produto);
    LimparCamposFornecedorProduto();
    LimparCamposPesquisaFornecedorProduto();
    limparCamposProdutoFoto();
    limparCamposProdutoPneu();
    limparCamposProdutoEPI();
    limparCamposProdutoCombustivel();
    LimparCamposComposicaoProduto();
    limparCamposProdutoBem();
    _custoProduto.CalculoCustoProduto.val("");

    $("#liTabArmazenamento").hide();
    $("#liTabProduto a").click();

    RecarregarGridFornecedorProduto();
    resetarTabs();
}

function ValidarEnumeradoresObrigatorios() {
    var valido = true;
    if (_produto.OrigemMercadoria.val() != -1) {

    } else {
        exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe a Origem da Mercadoria");
        valido = false;
    }

    return valido;
}

function formatarStrFloat(valor) {
    valor = valor.replace(".", "");
    return valor.replace(",", ".");
}

function resetarTabs() {
    $(".nav-tabs").each(function () {
        $(this).find("a:first").tab("show");
    });
}


function GerarEtiquetaClik(e) {
    executarDownload("Produto/GerarEtiqueta", { Codigo: _produto.Codigo.val() });
}
