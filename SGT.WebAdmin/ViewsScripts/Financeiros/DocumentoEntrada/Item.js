/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/Abastecimento.js" />
/// <reference path="../../Consultas/OrdemServico.js" />
/// <reference path="../../Consultas/LocalArmazenamentoProduto.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Enumeradores/EnumUnidadeMedida.js" />
/// <reference path="../../Enumeradores/EnumTipoAquisicaoPneu.js" />
/// <reference path="../../Enumeradores/EnumVidaPneu.js" />
/// <reference path="../../Enumeradores/EnumCSTICMS.js" />
/// <reference path="../../Enumeradores/EnumCSTPISCOFINS.js" />
/// <reference path="../../Enumeradores/EnumCSTIPI.js" />
/// <reference path="../../Enumeradores/EnumSituacaoOrdemServicoFrota.js" />
/// <reference path="../../Consultas/CFOP.js" />
/// <reference path="DocumentoEntrada.js" />
/// <reference path="../../Consultas/OrdemCompra.js" />
/// <reference path="../../Enumeradores/EnumSituacaoOrdemCompra.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridItem, _item, _gridItemAbastecimentos, _gridItemOrdensServico;
var _buscaCFOPItem, _buscaCFOPNFItem;
var clicouNoCRUD = false;
var executarConsultaCFOPAntesCRUD = false;

var _cstICMS = [
    { value: "", text: "Nenhum" },
    { value: "000", text: "000 - (Nacional) - Tributada integralmente" },
    { value: "010", text: "010 - (Nacional) - Tributada e com cobrança do ICMS por substituição tributária" },
    { value: "020", text: "020 - (Nacional) - Com redução de base de cálculo" },
    { value: "030", text: "030 - (Nacional) - Isenta ou não tributada e com cobrança do ICMS por substituição tributária" },
    { value: "040", text: "040 - (Nacional) - Isenta" },
    { value: "041", text: "041 - (Nacional) - Não tributada" },
    { value: "050", text: "050 - (Nacional) - Suspensão" },
    { value: "051", text: "051 - (Nacional) - Diferimento" },
    { value: "060", text: "060 - (Nacional) - ICMS cobrado anteriormente por substituição tributária" },
    { value: "061", text: "061 - (Nacional) - Tributação monofásica sobre combustíveis cobrada anteriormente" },
    { value: "070", text: "070 - (Nacional) - Com redução de base de cálculo e cobrança do ICMS por substituição tributária" },
    { value: "090", text: "090 - (Nacional) - Outras" },
    { value: "100", text: "100 - (Estrangeira - Imp. direta) - Tributada integralmente" },
    { value: "110", text: "110 - (Estrangeira - Imp. direta) - Tributada e com cobrança do ICMS por substituição tributária" },
    { value: "120", text: "120 - (Estrangeira - Imp. direta) - Com redução de base de cálculo" },
    { value: "130", text: "130 - (Estrangeira - Imp. direta) - Isenta ou não tributada e com cobrança do ICMS por substituição tributária" },
    { value: "140", text: "140 - (Estrangeira - Imp. direta) - Isenta" },
    { value: "141", text: "141 - (Estrangeira - Imp. direta) - Não tributada" },
    { value: "150", text: "150 - (Estrangeira - Imp. direta) - Suspensão" },
    { value: "151", text: "151 - (Estrangeira - Imp. direta) - Diferimento" },
    { value: "160", text: "160 - (Estrangeira - Imp. direta) - ICMS cobrado anteriormente por substituição tributária" },
    { value: "170", text: "170 - (Estrangeira - Imp. direta) - Com redução de base de cálculo e cobrança do ICMS por substituição tributária" },
    { value: "190", text: "190 - (Estrangeira - Imp. direta) - Outras" },
    { value: "200", text: "200 - (Estrangeira - Adquirida merc. interno) - Tributada integralmente" },
    { value: "210", text: "210 - (Estrangeira - Adquirida merc. interno) - Tributada e com cobrança do ICMS por substituição tributária" },
    { value: "220", text: "220 - (Estrangeira - Adquirida merc. interno) - Com redução de base de cálculo" },
    { value: "230", text: "230 - (Estrangeira - Adquirida merc. interno) - Isenta ou não tributada e com cobrança do ICMS por substituição tributária" },
    { value: "240", text: "240 - (Estrangeira - Adquirida merc. interno) - Isenta" },
    { value: "241", text: "241 - (Estrangeira - Adquirida merc. interno) - Não tributada" },
    { value: "250", text: "250 - (Estrangeira - Adquirida merc. interno) - Suspensão" },
    { value: "251", text: "251 - (Estrangeira - Adquirida merc. interno) - Diferimento" },
    { value: "260", text: "260 - (Estrangeira - Adquirida merc. interno) - ICMS cobrado anteriormente por substituição tributária" },
    { value: "270", text: "270 - (Estrangeira - Adquirida merc. interno) - Com redução de base de cálculo e cobrança do ICMS por substituição tributária" },
    { value: "290", text: "290 - (Estrangeira - Adquirida merc. interno) - Outras" },
    { value: "310", text: "310 - (Nacional - Mercadoria com conteúdo de imp. superior a 40%) - Tributada e com cobrança do ICMS por substituição tributária" },
    { value: "320", text: "320 - (Nacional - Mercadoria com conteúdo de imp. superior a 40%) - Com redução de base de cálculo" },
    { value: "330", text: "330 - (Nacional - Mercadoria com conteúdo de imp. superior a 40%) - Isenta ou não tributada e com cobrança do ICMS por substituição tributária" },
    { value: "340", text: "340 - (Nacional - Mercadoria com conteúdo de imp. superior a 40%) - Isenta" },
    { value: "341", text: "341 - (Nacional - Mercadoria com conteúdo de imp. superior a 40%) - Não tributada" },
    { value: "350", text: "350 - (Nacional - Mercadoria com conteúdo de imp. superior a 40%) - Suspensão" },
    { value: "351", text: "351 - (Nacional - Mercadoria com conteúdo de imp. superior a 40%) - Diferimento" },
    { value: "360", text: "360 - (Nacional - Mercadoria com conteúdo de imp. superior a 40%) - ICMS cobrado anteriormente por substituição tributária" },
    { value: "370", text: "370 - (Nacional - Mercadoria com conteúdo de imp. superior a 40%) - Com redução de base de cálculo e cobrança do ICMS por substituição tributária" },
    { value: "390", text: "390 - (Nacional - Mercadoria com conteúdo de imp. superior a 40%) - Outras" },
    { value: "410", text: "410 - (Nacional - Produção em conformidade com Decreto-Lei nº 288/1967, e as Leis nºs 8.248/1991, 8.387/1991, 10.176/2001 e 11.484/2007) - Tributada e com cobrança do ICMS por substituição tributária" },
    { value: "420", text: "420 - (Nacional - Produção em conformidade com Decreto-Lei nº 288/1967, e as Leis nºs 8.248/1991, 8.387/1991, 10.176/2001 e 11.484/2007) - Com redução de base de cálculo" },
    { value: "430", text: "430 - (Nacional - Produção em conformidade com Decreto-Lei nº 288/1967, e as Leis nºs 8.248/1991, 8.387/1991, 10.176/2001 e 11.484/2007) - Isenta ou não tributada e com cobrança do ICMS por substituição tributária" },
    { value: "440", text: "440 - (Nacional - Produção em conformidade com Decreto-Lei nº 288/1967, e as Leis nºs 8.248/1991, 8.387/1991, 10.176/2001 e 11.484/2007) - Isenta" },
    { value: "441", text: "441 - (Nacional - Produção em conformidade com Decreto-Lei nº 288/1967, e as Leis nºs 8.248/1991, 8.387/1991, 10.176/2001 e 11.484/2007) - Não tributada" },
    { value: "450", text: "450 - (Nacional - Produção em conformidade com Decreto-Lei nº 288/1967, e as Leis nºs 8.248/1991, 8.387/1991, 10.176/2001 e 11.484/2007) - Suspensão" },
    { value: "451", text: "451 - (Nacional - Produção em conformidade com (Decreto-Lei nº 288/1967 , e as Leis nºs 8.248/1991, 8.387/1991, 10.176/2001 e 11.484/2007) - Diferimento" },
    { value: "460", text: "460 - (Nacional - Produção em conformidade com (Decreto-Lei nº 288/1967 , e as Leis nºs 8.248/1991, 8.387/1991, 10.176/2001 e 11.484/2007) - ICMS cobrado anteriormente por substituição tributária" },
    { value: "470", text: "470 - (Nacional - Produção em conformidade com (Decreto-Lei nº 288/1967 , e as Leis nºs 8.248/1991, 8.387/1991, 10.176/2001 e 11.484/2007) - Com redução de base de cálculo e cobrança do ICMS por substituição tributária" },
    { value: "490", text: "490 - (Nacional - Produção em conformidade com (Decreto-Lei nº 288/1967 , e as Leis nºs 8.248/1991, 8.387/1991, 10.176/2001 e 11.484/2007) - Outras" },
    { value: "510", text: "510 - (Nacional - Mercadoria com conteúdo de imp. inferior a 40%) - Tributada e com cobrança do ICMS por substituição tributária" },
    { value: "520", text: "520 - (Nacional - Mercadoria com conteúdo de imp. inferior a 40%) - Com redução de base de cálculo" },
    { value: "530", text: "530 - (Nacional - Mercadoria com conteúdo de imp. inferior a 40%) - Isenta ou não tributada e com cobrança do ICMS por substituição tributária" },
    { value: "540", text: "540 - (Nacional - Mercadoria com conteúdo de imp. inferior a 40%) - Isenta" },
    { value: "541", text: "541 - (Nacional - Mercadoria com conteúdo de imp. inferior a 40%) - Não tributada" },
    { value: "550", text: "550 - (Nacional - Mercadoria com conteúdo de imp. inferior a 40%) - Suspensão" },
    { value: "551", text: "551 - (Nacional - Mercadoria com conteúdo de imp. inferior a 40%) - Diferimento" },
    { value: "560", text: "560 - (Nacional - Mercadoria com conteúdo de imp. inferior a 40%) - ICMS cobrado anteriormente por substituição tributária" },
    { value: "570", text: "570 - (Nacional - Mercadoria com conteúdo de imp. inferior a 40%) - Com redução de base de cálculo e cobrança do ICMS por substituição tributária" },
    { value: "590", text: "590 - (Nacional - Mercadoria com conteúdo de imp. inferior a 40%) - Outras" },
    { value: "600", text: "600 - (Estrangeira – Importação direta) - Tributada integralmente" },
    { value: "610", text: "610 - (Estrangeira – Importação direta) - Tributada e com cobrança do ICMS por substituição tributária" },
    { value: "620", text: "620 - (Estrangeira – Importação direta) - Com redução de base de cálculo" },
    { value: "630", text: "630 - (Estrangeira – Importação direta) - Isenta ou não tributada e com cobrança do ICMS por substituição tributária" },
    { value: "640", text: "640 - (Estrangeira – Importação direta) - Isenta" },
    { value: "641", text: "641 - (Estrangeira – Importação direta) - Não tributada" },
    { value: "650", text: "650 - (Estrangeira – Importação direta) - Suspensão" },
    { value: "651", text: "651 - (Estrangeira – Importação direta) - Diferimento" },
    { value: "660", text: "660 - (Estrangeira – Importação direta) - ICMS cobrado anteriormente por substituição tributária" },
    { value: "670", text: "670 - (Estrangeira – Importação direta) - Com redução de base de cálculo e cobrança do ICMS por substituição tributária" },
    { value: "690", text: "690 - (Estrangeira – Importação direta) - Outras" },
    { value: "700", text: "700 - (Estrangeira – Adquirida no mercado interno) - Tributada integralmente" },
    { value: "710", text: "710 - (Estrangeira – Adquirida no mercado interno) - Tributada e com cobrança do ICMS por substituição tributária" },
    { value: "720", text: "720 - (Estrangeira – Adquirida no mercado interno) - Com redução de base de cálculo" },
    { value: "730", text: "730 - (Estrangeira – Adquirida no mercado interno) - Isenta ou não tributada e com cobrança do ICMS por substituição tributária" },
    { value: "740", text: "740 - (Estrangeira – Adquirida no mercado interno) - Isenta" },
    { value: "741", text: "741 - (Estrangeira – Adquirida no mercado interno) - Não tributada" },
    { value: "750", text: "750 - (Estrangeira – Adquirida no mercado interno) - Suspensão" },
    { value: "751", text: "751 - (Estrangeira – Adquirida no mercado interno) - Diferimento" },
    { value: "760", text: "760 - (Estrangeira – Adquirida no mercado interno) - ICMS cobrado anteriormente por substituição tributária" },
    { value: "770", text: "770 - (Estrangeira – Adquirida no mercado interno) - Com redução de base de cálculo e cobrança do ICMS por substituição tributária" },
    { value: "790", text: "790 - (Estrangeira – Adquirida no mercado interno) - Outras" },
    { value: "800", text: "800 - (Nacional - Mercadoria com conteúdo de imp. superior a 70%) - Tributada integralmente" },
    { value: "810", text: "810 - (Nacional - Mercadoria com conteúdo de imp. superior a 70%) - Tributada e com cobrança do ICMS por substituição tributária" },
    { value: "820", text: "820 - (Nacional - Mercadoria com conteúdo de imp. superior a 70%) - Com redução de base de cálculo" },
    { value: "830", text: "830 - (Nacional - Mercadoria com conteúdo de imp. superior a 70%) - Isenta ou não tributada e com cobrança do ICMS por substituição tributária" },
    { value: "840", text: "840 - (Nacional - Mercadoria com conteúdo de imp. superior a 70%) - Isenta" },
    { value: "841", text: "841 - (Nacional - Mercadoria com conteúdo de imp. superior a 70%) - Não tributada" },
    { value: "850", text: "850 - (Nacional - Mercadoria com conteúdo de imp. superior a 70%) - Suspensão" },
    { value: "851", text: "851 - (Nacional - Mercadoria com conteúdo de imp. superior a 70%) - Diferimento" },
    { value: "860", text: "860 - (Nacional - Mercadoria com conteúdo de imp. superior a 70%) - ICMS cobrado anteriormente por substituição tributária" },
    { value: "870", text: "870 - (Nacional - Mercadoria com conteúdo de imp. superior a 70%) - Com redução de base de cálculo e cobrança do ICMS por substituição tributária" },
    { value: "890", text: "890 - (Nacional - Mercadoria com conteúdo de imp. superior a 70%) - Outras" },
    { value: "101", text: "101 - (Simples Nacional) - Tributada pelo Simples Nacional com permissão de crédito" },
    { value: "102", text: "102 - (Simples Nacional) - Tributada pelo Simples Nacional sem permissão de crédito" },
    { value: "103", text: "103 - (Simples Nacional) - Isenção do ICMS no Simples Nacional para faixa de receita bruta" },
    { value: "201", text: "201 - (Simples Nacional) - Tributada pelo Simples Nacional com permissão de crédito e com cobrança do ICMS por substituição tributária" },
    { value: "202", text: "202 - (Simples Nacional) - Tributada pelo Simples Nacional sem permissão de crédito e com cobrança do ICMS por substituição tributária" },
    { value: "203", text: "203 - (Simples Nacional) - Isenção do ICMS no Simples Nacional para faixa de receita bruta e com cobrança do ICMS por substituição tributaria" },
    { value: "300", text: "300 - (Simples Nacional) - Imune / 300 - (Nacional - Mercadoria com conteúdo de imp. superior a 40%) - Tributada integralmente" },
    { value: "400", text: "400 - (Simples Nacional) - Não tributada pelo Simples Nacional / 400 - (Nacional - Produção em conformidade com Decreto-Lei nº 288/1967, e as Leis nºs 8.248/1991, 8.387/1991, 10.176/2001 e 11.484/2007) - Tributada integralmente" },
    { value: "500", text: "500 - (Simples Nacional) - ICMS cobrado anteriormente por substituição tributaria (substituído) ou por antecipação / 500 - (Nacional - Mercadoria com conteúdo de imp. inferior a 40%) - Tributada integralmente" },
    { value: "900", text: "900 - (Simples Nacional) - Outros" }
];

var _cstPIS = [
    { value: "", text: "Nenhum" },
    { value: "01", text: "01 - Operação Tributável com Alíquota Básica" },
    { value: "02", text: "02 - Operação Tributável com Alíquota Diferenciada" },
    { value: "03", text: "03 - Operação Tributável com Alíquota por Unidade de Medida de Produto" },
    { value: "04", text: "04 - Operação Tributável Monofásica - Revenda a Alíquota Zero" },
    { value: "05", text: "05 - Operação Tributável por Substituição Tributária" },
    { value: "06", text: "06 - Operação Tributável a Alíquota Zero" },
    { value: "07", text: "07 - Operação Isenta da Contribuição" },
    { value: "08", text: "08 - Operação sem Incidência da Contribuição" },
    { value: "09", text: "09 - Operação com Suspensão da Contribuição" },
    { value: "49", text: "49 - Outras Operações de Saída" },
    { value: "50", text: "50 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita Tributada no Mercado Interno" },
    { value: "51", text: "51 - Operação com Direito a Crédito – Vinculada Exclusivamente a Receita Não Tributada no Mercado Interno" },
    { value: "52", text: "52 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita de Exportação" },
    { value: "53", text: "53 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno" },
    { value: "54", text: "54 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas no Mercado Interno e de Exportação" },
    { value: "55", text: "55 - Operação com Direito a Crédito - Vinculada a Receitas Não-Tributadas no Mercado Interno e de Exportação" },
    { value: "56", text: "56 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno, e de Exportação" },
    { value: "60", text: "60 - Crédito Presumido - Operação de Aquisição Vinculada Exclusivamente a Receita Tributada no Mercado Interno" },
    { value: "61", text: "61 - Crédito Presumido - Operação de Aquisição Vinculada Exclusivamente a Receita Não-Tributada no Mercado Interno" },
    { value: "62", text: "62 - Crédito Presumido - Operação de Aquisição Vinculada Exclusivamente a Receita de Exportação" },
    { value: "63", text: "63 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno" },
    { value: "64", text: "64 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Tributadas no Mercado Interno e de Exportação" },
    { value: "65", text: "65 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Não-Tributadas no Mercado Interno e de Exportação" },
    { value: "66", text: "66 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno, e de Exportação" },
    { value: "67", text: "67 - Crédito Presumido - Outras Operações" },
    { value: "70", text: "70 - Operação de Aquisição sem Direito a Crédito" },
    { value: "71", text: "71 - Operação de Aquisição com Isenção" },
    { value: "72", text: "72 - Operação de Aquisição com Suspensão" },
    { value: "73", text: "73 - Operação de Aquisição a Alíquota Zero" },
    { value: "74", text: "74 - Operação de Aquisição sem Incidência da Contribuição" },
    { value: "75", text: "75 - Operação de Aquisição por Substituição Tributária" },
    { value: "98", text: "98 - Outras Operações de Entrada" },
    { value: "99", text: "99 - Outras Operações" }
];

var _cstCOFINS = [
    { value: "", text: "Nenhum" },
    { value: "01", text: "01 - Operação Tributável com Alíquota Básica" },
    { value: "02", text: "02 - Operação Tributável com Alíquota Diferenciada" },
    { value: "03", text: "03 - Operação Tributável com Alíquota por Unidade de Medida de Produto" },
    { value: "04", text: "04 - Operação Tributável Monofásica - Revenda a Alíquota Zero" },
    { value: "05", text: "05 - Operação Tributável por Substituição Tributária" },
    { value: "06", text: "06 - Operação Tributável a Alíquota Zero" },
    { value: "07", text: "07 - Operação Isenta da Contribuição" },
    { value: "08", text: "08 - Operação sem Incidência da Contribuição" },
    { value: "09", text: "09 - Operação com Suspensão da Contribuição" },
    { value: "49", text: "49 - Outras Operações de Saída" },
    { value: "50", text: "50 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita Tributada no Mercado Interno" },
    { value: "51", text: "51 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita Não-Tributada no Mercado Interno" },
    { value: "52", text: "52 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita de Exportação" },
    { value: "53", text: "53 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno" },
    { value: "54", text: "54 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas no Mercado Interno e de Exportação" },
    { value: "55", text: "55 - Operação com Direito a Crédito - Vinculada a Receitas Não Tributadas no Mercado Interno e de Exportação" },
    { value: "56", text: "56 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno e de Exportação" },
    { value: "60", text: "60 - Crédito Presumido - Operação de Aquisição Vinculada Exclusivamente a Receita Tributada no Mercado Interno" },
    { value: "61", text: "61 - Crédito Presumido - Operação de Aquisição Vinculada Exclusivamente a Receita Não-Tributada no Mercado Interno" },
    { value: "62", text: "62 - Crédito Presumido - Operação de Aquisição Vinculada Exclusivamente a Receita de Exportação" },
    { value: "63", text: "63 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno" },
    { value: "64", text: "64 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Tributadas no Mercado Interno e de Exportação" },
    { value: "65", text: "65 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Não-Tributadas no Mercado Interno e de Exportação" },
    { value: "66", text: "66 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno e de Exportação" },
    { value: "67", text: "67 - Crédito Presumido - Outras Operações" },
    { value: "70", text: "70 - Operação de Aquisição sem Direito a Crédito" },
    { value: "71", text: "71 - Operação de Aquisição com Isenção" },
    { value: "72", text: "72 - Operação de Aquisição com Suspensão" },
    { value: "73", text: "73 - Operação de Aquisição a Alíquota Zero" },
    { value: "74", text: "74 - Operação de Aquisição sem Incidência da Contribuição" },
    { value: "75", text: "75 - Operação de Aquisição por Substituição Tributária" },
    { value: "98", text: "98 - Outras Operações de Entrada" },
    { value: "99", text: "99 - Outras Operações" }
];

var _cstIPI = [
    { value: "", text: "Nenhum" },
    { value: "00", text: "00 - Entrada com Recuperação de Crédito" },
    { value: "01", text: "01 - Entrada Tributável com Alíquota Zero" },
    { value: "02", text: "02 - Entrada Isenta" },
    { value: "03", text: "03 - Entrada Não-Tributada" },
    { value: "04", text: "04 - Entrada Imune" },
    { value: "05", text: "05 - Entrada com Suspensão" },
    { value: "49", text: "49 - Outras Entradas" },
    { value: "50", text: "50 - Saída Tributada" },
    { value: "51", text: "51 - Saída Tributável com Alíquota Zero" },
    { value: "52", text: "52 - Saida Isenta" },
    { value: "53", text: "53 - Saída Não-Tributada" },
    { value: "54", text: "54 - Saída Imune" },
    { value: "55", text: "55 - Saída com Suspensão" },
    { value: "99", text: "99 - Outras Saídas" }
];

var Item = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.TotalDescontosItens = PropertyEntity({ text: "Descontos: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.TotalValorItens = PropertyEntity({ text: "Total: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.SubTotalItens = PropertyEntity({ text: "Subtotal: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.Sequencial = PropertyEntity({ text: "Item:", val: ko.observable("Automático"), def: "Automático", enable: ko.observable(false) });
    this.UnidadeMedida = PropertyEntity({ val: ko.observable(EnumUnidadeMedida.Unidade), options: EnumUnidadeMedida.obterOpcoes(), def: EnumUnidadeMedida.Unidade, text: "*Unidade de Medida:", enable: ko.observable(true) });
    this.Quantidade = PropertyEntity({ text: "*Quantidade:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,0000"), def: "0,0000", configDecimal: { precision: 4, allowZero: true, allowNegative: false }, required: true, enable: ko.observable(true) });
    this.ValorUnitario = PropertyEntity({ text: "*Valor Unitário:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,0000"), def: "0,0000", configDecimal: { precision: 4, allowZero: true, allowNegative: false }, required: true, enable: ko.observable(true) });
    this.Desconto = PropertyEntity({ text: "Desconto:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotal = PropertyEntity({ text: "Valor Total:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", enable: ko.observable(true) });
    this.ValorCustoUnitario = PropertyEntity({ text: "Custo Unitário:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,0000"), def: "0,0000", enable: ko.observable(false), configDecimal: { precision: 4, allowZero: true, allowNegative: false } });
    this.ValorCustoTotal = PropertyEntity({ text: "Custo Total:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,0000"), def: "0,0000", enable: ko.observable(false), configDecimal: { precision: 4, allowZero: true, allowNegative: false } });
    this.CalculoCustoProduto = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string, maxlength: 500, visible: ko.observable(false) });
    this.ValorOutrasDespesas = PropertyEntity({ text: "Valor de Outras Despesas:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorFrete = PropertyEntity({ text: "Valor do Frete:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorSeguro = PropertyEntity({ text: "Valor do Seguro:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Produto:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.CFOP = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*CFOP:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.NaturezaOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Natureza da Operação:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Movimento:", idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(true), visible: ko.observable(true) });

    //Campos de manipulação ocultos
    this.SiglaUnidadeMedida = PropertyEntity({ val: ko.observable(""), def: "", text: "" });
    this.CodigoProdutoFornecedor = PropertyEntity({ val: ko.observable(""), def: "", text: "", visible: ko.observable(false) });
    this.DescricaoProdutoFornecedor = PropertyEntity({ val: ko.observable(""), def: "", text: "" });
    this.CodigoBarrasEAN = PropertyEntity({ val: ko.observable(""), def: "", text: "" });
    this.NCMProdutoFornecedor = PropertyEntity({ val: ko.observable(""), def: "", text: "" });
    this.CESTProdutoFornecedor = PropertyEntity({ val: ko.observable(""), def: "", text: "" });
    this.CFOPNF = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*CFOP:", idBtnSearch: guid(), required: false, enable: ko.observable(false), visible: ko.observable(false) });
    this.NaturezaOperacaoNF = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Natureza da Operação:", idBtnSearch: guid(), required: false, enable: ko.observable(false), visible: ko.observable(false) });
    this.TipoMovimentoNF = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Movimento:", idBtnSearch: guid(), enable: ko.observable(false), required: false, visible: ko.observable(false) });
    this.NCM = PropertyEntity({ val: ko.observable(""), def: "", text: "" });

    //Aba Adicionais
    this.OrdemServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Ordem de Serviço:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.OrdemCompraMercadoria = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "O.C Item:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.RegraEntradaDocumento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Regra Automática:", idBtnSearch: guid(), enable: ko.observable(false) });
    this.Equipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Equipamento:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Horimetro = PropertyEntity({ text: "Horímetro:", enable: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false }, maxlength: 11 });
    this.KMAbastecimento = PropertyEntity({ text: "KM do Abastec.:", enable: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false }, maxlength: 11 });
    this.DataAbastecimento = PropertyEntity({ text: "Data Abastec.:", getType: typesKnockout.dateTime, val: ko.observable(""), def: "", required: false, enable: ko.observable(true) });
    this.ObservacaoItem = PropertyEntity({ text: "Observação:", val: ko.observable(""), def: "", maxlength: 3000, required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.OrigemMercadoria = PropertyEntity({ text: "Origem Mercadoria:", val: ko.observable(-1), def: -1, options: ko.observable(EnumOrigemMercadoria.obterOpcoesCadastro()), required: false, enable: ko.observable(true) });
    this.CstIcmsFornecedor = PropertyEntity({ val: ko.observable(""), options: _cstICMS, def: "", text: "CST ICMS do Fornecedor:", enable: ko.observable(true) });
    this.CfopFornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CFOP do Fornecedor:", idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.BaseCalculoICMSFornecedor = PropertyEntity({ text: "Base de Cálculo ICMS do Fornecedor:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,0000"), def: "0,0000", configDecimal: { precision: 4, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.AliquotaICMSFornecedor = PropertyEntity({ text: "Alíquota ICMS Fornecedor:", getType: typesKnockout.decimal, maxlength: 5, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 4, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorICMSFornecedor = PropertyEntity({ text: "Valor do ICMS do Fornecedor:", getType: typesKnockout.decimal, maxlength: 10, val: ko.observable("0,00"), def: "0,00", configDecimal: { allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.UnidadeMedidaFornecedor = PropertyEntity({ text: "Unidade Medida Fornecedor:", val: ko.observable(""), def: "", maxlength: 6, required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.QuantidadeFornecedor = PropertyEntity({ text: "Quantidade Fornecedor:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,0000"), def: "0,0000", configDecimal: { precision: 4, allowZero: true, allowNegative: false }, required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.ValorUnitarioFornecedor = PropertyEntity({ text: "Valor Unit. Fornecedor:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,0000"), def: "0,0000", configDecimal: { precision: 4, allowZero: true, allowNegative: false }, required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.EncerrarOrdemServico = PropertyEntity({ text: "Deseja encerrar a O.S. após finalizar esta nota?", val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false), enable: ko.observable(true) });
    this.OrdemCompra = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Ordem de Compra:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    //Aba ICMS
    this.CSTICMS = PropertyEntity({ val: ko.observable(""), options: _cstICMS, def: "", text: "*CST do ICMS:", enable: ko.observable(true) });
    this.BaseCalculoICMS = PropertyEntity({ text: "Base de Cálculo:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.AliquotaICMS = PropertyEntity({ text: "Alíquota:", getType: typesKnockout.decimal, maxlength: 7, val: ko.observable("0,0000"), def: "0,0000", configDecimal: { precision: 4, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorICMS = PropertyEntity({ text: "Valor:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });

    //Aba PIS
    this.CSTPIS = PropertyEntity({ val: ko.observable(""), options: _cstPIS, def: "", text: "*CST do PIS:", enable: ko.observable(true) });
    this.PercentualReducaoBaseCalculoPIS = PropertyEntity({ text: "% Redução da BC:", getType: typesKnockout.decimal, maxlength: 5, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.BaseCalculoPIS = PropertyEntity({ text: "Base de Cálculo:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.AliquotaPIS = PropertyEntity({ text: "Alíquota:", getType: typesKnockout.decimal, maxlength: 5, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorPIS = PropertyEntity({ text: "Valor:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });

    //Aba COFINS
    this.CSTCOFINS = PropertyEntity({ val: ko.observable(""), options: _cstCOFINS, def: "", text: "*CST da COFINS:", enable: ko.observable(true) });
    this.PercentualReducaoBaseCalculoCOFINS = PropertyEntity({ text: "% Redução da BC:", getType: typesKnockout.decimal, maxlength: 5, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.BaseCalculoCOFINS = PropertyEntity({ text: "Base de Cálculo:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.AliquotaCOFINS = PropertyEntity({ text: "Alíquota:", getType: typesKnockout.decimal, maxlength: 5, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorCOFINS = PropertyEntity({ text: "Valor:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });

    //Aba IPI
    this.CSTIPI = PropertyEntity({ val: ko.observable(""), options: _cstIPI, def: "", text: "*CST do IPI:", enable: ko.observable(true) });
    this.PercentualReducaoBaseCalculoIPI = PropertyEntity({ text: "% Redução da BC:", getType: typesKnockout.decimal, maxlength: 5, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.BaseCalculoIPI = PropertyEntity({ text: "Base de Cálculo:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.AliquotaIPI = PropertyEntity({ text: "Alíquota:", getType: typesKnockout.decimal, maxlength: 5, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorIPI = PropertyEntity({ text: "Valor:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });

    //Aba ICMS ST
    this.BaseCalculoICMSST = PropertyEntity({ text: "Base de Cálculo:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.AliquotaICMSST = PropertyEntity({ text: "Alíquota:", getType: typesKnockout.decimal, maxlength: 5, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorICMSST = PropertyEntity({ text: "Valor:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.BaseSTRetido = PropertyEntity({ text: "Base ST Retido:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorSTRetido = PropertyEntity({ text: "Valor ST Retido:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });

    //Aba Crédito Presumido
    this.BaseCalculoCreditoPresumido = PropertyEntity({ text: "Base de Cálculo:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.AliquotaCreditoPresumido = PropertyEntity({ text: "Alíquota:", getType: typesKnockout.decimal, maxlength: 5, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorCreditoPresumido = PropertyEntity({ text: "Valor:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });

    //Aba Diferencial
    this.BaseCalculoDiferencial = PropertyEntity({ text: "Base de Cálculo:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.AliquotaDiferencial = PropertyEntity({ text: "Alíquota:", getType: typesKnockout.decimal, maxlength: 5, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorDiferencial = PropertyEntity({ text: "Valor:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });

    //Aba Valores Fora
    this.ValorFreteFora = PropertyEntity({ text: "Valor Frete Fora:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorOutrasDespesasFora = PropertyEntity({ text: "Valor Outras Despesas Fora:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorDescontoFora = PropertyEntity({ text: "Valor Desconto Fora:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorImpostosFora = PropertyEntity({ text: "Valor Impostos Fora:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorDiferencialFreteFora = PropertyEntity({ text: "Valor Diferencial do Frete Fora:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorICMSFreteFora = PropertyEntity({ text: "Valor ICMS do Frete Fora:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });

    //Aba Retenções
    this.ValorRetencaoPIS = PropertyEntity({ text: "Retenção PIS:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorRetencaoCOFINS = PropertyEntity({ text: "Retenção COFINS:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorRetencaoINSS = PropertyEntity({ text: "Retenção INSS:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorRetencaoIPI = PropertyEntity({ text: "Retenção IPI:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorRetencaoCSLL = PropertyEntity({ text: "Retenção CSLL:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorRetencaoOutras = PropertyEntity({ text: "Outras Retenções:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorRetencaoIR = PropertyEntity({ text: "Retenção IR:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorRetencaoISS = PropertyEntity({ text: "Retenção ISS:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });

    //Aba Abastecimentos
    this.GridAbastecimento = PropertyEntity({ type: types.local });
    this.Abastecimento = PropertyEntity({ type: types.event, text: "Adicionar Abastecimento(s)", idBtnSearch: guid(), enable: ko.observable(true) });

    //Aba Pneu
    this.NumeroFogoInicial = PropertyEntity({ text: "Número de Fogo Inicial:", getType: typesKnockout.int, enable: ko.observable(true), required: ko.observable(false) });
    this.TipoAquisicao = PropertyEntity({ text: "Tipo Aquisição: ", val: ko.observable(EnumTipoAquisicaoPneu.Todos), options: EnumTipoAquisicaoPneu.obterOpcoesCadastro(), def: EnumTipoAquisicaoPneu.Todos, enable: ko.observable(true), required: ko.observable(false) });
    this.VidaAtual = PropertyEntity({ text: "Vida Atual: ", val: ko.observable(EnumVidaPneu.Todas), options: EnumVidaPneu.obterOpcoesCadastro(), def: EnumVidaPneu.Todas, enable: ko.observable(true), required: ko.observable(false) });
    this.Almoxarifado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Almoxarifado:", idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(false) });

    //Aba Outros
    this.ProdutoVinculado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto Vinculado:", idBtnSearch: guid(), enable: ko.observable(true), required: false, visible: ko.observable(true) });
    this.QuantidadeProdutoVinculado = PropertyEntity({ text: "Quantidade Rateio Custo (Produto Vinculado):", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,0000"), def: "0,0000", configDecimal: { precision: 4, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.LocalArmazenamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local de Armazenamento:", idBtnSearch: guid(), enable: ko.observable(true), required: false, visible: ko.observable(_CONFIGURACAO_TMS.UtilizaMultiplosLocaisArmazenamento) });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid(), enable: ko.observable(true), required: false, visible: ko.observable(true) });

    //Aba Ordens de Serviço
    this.GridOrdemServico = PropertyEntity({ type: types.local });
    this.OrdemServicoMultiplaSelecao = PropertyEntity({ type: types.event, text: "Adicionar Ordens de Serviço", idBtnSearch: guid(), enable: ko.observable(true) });

    //CRUD
    this.Adicionar = PropertyEntity({ type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ type: types.event, text: "Atualizar", visible: ko.observable(false), enable: ko.observable(true) });
    this.Excluir = PropertyEntity({ type: types.event, text: "Excluir", visible: ko.observable(false), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, text: "Cancelar", visible: ko.observable(false), enable: ko.observable(true) });

    this.OrdemServico.codEntity.subscribe(function () {
        ControleVisibilidadeOrdemServicoItem();
    });
};

//*******EVENTOS*******

function LoadItem() {

    _item = new Item();
    KoBindings(_item, "knockoutItens");

    new BuscarTipoMovimento(_item.TipoMovimento);
    new BuscarProdutoTMS(_item.Produto, RetornoProdutoTMS, null, null, _item.OrdemCompra, null, true);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        _buscaCFOPItem = new BuscarCFOPNotaFiscal(_item.CFOP, function (r) { RetornoConsultaCFOPItem(r.Codigo); }, EnumTipoCFOP.Entrada, null, _documentoEntrada.Fornecedor, null, _documentoEntrada.Destinatario, null, null, null);
      }
    else { 
        _buscaCFOPItem = new BuscarCFOPNotaFiscal(_item.CFOP, function (r) { RetornoConsultaCFOPItem(r.Codigo); }, EnumTipoCFOP.Entrada, null, _documentoEntrada.Fornecedor, null, _documentoEntrada.Destinatario, null, null, _item.NaturezaOperacao);
     }
    //Buscas não visíveis
    new BuscarNaturezasOperacoesNotaFiscal(_item.NaturezaOperacaoNF, null, null, RetornoConsultaNaturezaOperacao);
    new BuscarTipoMovimento(_item.TipoMovimentoNF, null, null, RetornoConsultaTipoMovimento);
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFe) {
        _buscaCFOPNFItem = new BuscarCFOPNotaFiscal(_item.CFOPNF, function (r) { RetornoConsultaCFOP(r.Codigo); }, EnumTipoCFOP.Entrada, null, _documentoEntrada.Fornecedor, null, _documentoEntrada.Destinatario, null, null, _documentoEntrada.NaturezaOperacao);
       }
    else {
        _buscaCFOPNFItem = new BuscarCFOPNotaFiscal(_item.CFOPNF, function (r) { RetornoConsultaCFOP(r.Codigo); }, EnumTipoCFOP.Entrada, null, _documentoEntrada.Fornecedor, null, _documentoEntrada.Destinatario, null, null, null);
       }

    //

    new BuscarOrdemServico(_item.OrdemServico, RetornoConsultaOrdemServicoItem, null, [EnumSituacaoOrdemServicoFrota.EmManutencao, EnumSituacaoOrdemServicoFrota.AgNotaFiscal, _CONFIGURACAO_TMS.PermitirSelecionarOSFinalizadaDocumentoEntrada ? EnumSituacaoOrdemServicoFrota.Finalizada : null]);

    new BuscarOrdemCompra(_item.OrdemCompra, RetornoConsultaOrdemCompra, null, _item.Produto, _item.OrdemCompra, EnumSituacaoOrdemCompra.Aprovada);

    if (!_CONFIGURACAO_TMS.VisualizarTodosItensOrdemCompraDocumentoEntrada)
        new BuscarOrdemCompraMercadoria(_item.OrdemCompraMercadoria, RetornoConsultaOrdemCompraMercadoria, null, _item.Produto, _documentoEntrada.OrdemCompra, EnumSituacaoOrdemCompra.Aprovada);
    else
        new BuscarOrdemCompraMercadoria(_item.OrdemCompraMercadoria, RetornoConsultaOrdemCompraMercadoria, null, _item.Produto, null, EnumSituacaoOrdemCompra.Aprovada);

    new BuscarVeiculos(_item.Veiculo, RetornoBuscarVeiculosItem);
    new BuscarEquipamentos(_item.Equipamento, RetornoEquipamentoItem);
    new BuscarNaturezasOperacoesNotaFiscal(_item.NaturezaOperacao, null, null, RetornoConsultaNaturezaOperacaoItem);
    new BuscarAlmoxarifado(_item.Almoxarifado);
    new BuscarProdutoTMS(_item.ProdutoVinculado);
    new BuscarLocalArmazenamentoProduto(_item.LocalArmazenamento);
    new BuscarCentroResultado(_item.CentroResultado);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        _item.TipoMovimento.visible(false);
        _item.TipoMovimento.required(false);
    }
    new BuscarCFOPs(_item.CfopFornecedor, EnumTipoCFOP.Saida, function (cfop) {
        _item.CfopFornecedor.val(cfop.CodigoCFOP);
        _item.CfopFornecedor.codEntity(cfop.Codigo);
        Global.setarFocoProximoCampo(_item.CfopFornecedor.id);
    });
    LoadItemGrids();
    RecarregarGridItem();

    $("#" + _item.Quantidade.id).focusout(function () {
        AlterarValorTotalItem();
    });

    $("#" + _item.ValorUnitario.id).focusout(function (e) {
        e.preventDefault();
        FocusoutCampoValorUnitarioTotal(false);
    });

    $("#" + _item.ValorTotal.id).focusout(function (e) {
        e.preventDefault();
        FocusoutCampoValorUnitarioTotal(true);
    });

    $("#" + _item.AliquotaICMS.id + ", #" + _item.BaseCalculoICMS.id).focusout(function () {
        AtualizarValorICMS();
    });

    $("#" + _item.AliquotaICMSST.id + ", #" + _item.BaseCalculoICMSST.id).focusout(function () {
        AtualizarValorICMSST();
    });

    $("#" + _item.AliquotaCreditoPresumido.id + ", #" + _item.BaseCalculoCreditoPresumido.id).focusout(function () {
        AtualizarValorCreditoPresumido();
    });

    $("#" + _item.AliquotaDiferencial.id + ", #" + _item.BaseCalculoDiferencial.id).focusout(function () {
        AtualizarValorDiferencial();
    });

    $("#" + _item.AliquotaPIS.id + ", #" + _item.BaseCalculoPIS.id + ", #" + _item.PercentualReducaoBaseCalculoPIS.id).focusout(function () {
        AtualizarValorPIS();
    });

    $("#" + _item.AliquotaCOFINS.id + ", #" + _item.BaseCalculoCOFINS.id + ", #" + _item.PercentualReducaoBaseCalculoCOFINS.id).focusout(function () {
        AtualizarValorCOFINS();
    });

    $("#" + _item.AliquotaIPI.id + ", #" + _item.BaseCalculoIPI.id + ", #" + _item.PercentualReducaoBaseCalculoIPI.id).focusout(function () {
        AtualizarValorIPI();
    });

    $("#" + _item.Adicionar.id).mousedown(function (e) {
        e.preventDefault();
        MousedownCRUD(_item.Adicionar.id, AdicionarItemClick);
    });

    $("#" + _item.Atualizar.id).mousedown(function (e) {
        e.preventDefault();
        MousedownCRUD(_item.Atualizar.id, AtualizarItemClick);
    });

    $("#" + _item.Excluir.id).mousedown(function (e) {
        e.preventDefault();
        MousedownCRUD(_item.Excluir.id, ExcluirItemClick);
    });

    $("#" + _item.Cancelar.id).mousedown(function (e) {
        e.preventDefault();
        MousedownCRUD(_item.Cancelar.id, CancelarItemClick);
    });
}

function FocusoutCampoValorUnitarioTotal(ehValorTotal) {
    if (ehValorTotal)
        AlterarQuantidadeValorUnitario();
    else
        AlterarValorTotalItem();

    if (clicouNoCRUD)
        executarConsultaCFOPAntesCRUD = true;
    else
        RetornoConsultaCFOPItem(_item.CFOP.codEntity());
}

function MousedownCRUD(idButton, functionClick) {
    clicouNoCRUD = true;
    $("#" + idButton).focus();
    if (executarConsultaCFOPAntesCRUD)
        RetornoConsultaCFOPItem(_item.CFOP.codEntity(), functionClick);
    else
        functionClick();
    clicouNoCRUD = false;
    executarConsultaCFOPAntesCRUD = false;
}

function LoadItemGrids() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: EditarItemClick, tamanho: 10 }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoProduto", title: "Cód. Produto", width: "12%" },
        { data: "Item", title: "Item", width: "6%" },
        { data: "NCM", title: "NCM", width: "5%" },
        { data: "Produto", title: "Produto", width: "30%" },
        { data: "SiglaUnidadeMedida", title: "UN", width: "6%" },
        { data: "Quantidade", title: "Quantidade", width: "7%" },
        { data: "ValorUnitario", title: "Valor Unitário", width: "10%" },
        { data: "Desconto", title: "Desconto", width: "10%" },
        { data: "ValorTotal", title: "Valor Total", width: "12%" },
        { data: "CFOP", title: "CFOP", width: "8%" },
        { data: "Veiculo", title: "Placa", width: "8%" }
    ];
    _gridItem = new BasicDataTable(_item.Grid.id, header, menuOpcoes, { column: 2, dir: orderDir.asc });

    //Aba Abastecimentos
    var menuOpcoesAbastecimento = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: function (data) { ExcluirAbastecimentoClick(_item.Abastecimento, data); } }] };
    var headerAbastecimento = [
        { data: "CodigoInterno", visible: false },
        { data: "Codigo", visible: false },
        { data: "CodigoItem", visible: false },
        { data: "Horimetro", visible: false },
        { data: "CodigoEquipamento", visible: false },

        { data: "Placa", title: "Veículo", width: "10%" },
        { data: "Posto", title: "Posto", width: "30%" },
        { data: "Data", title: "Data", width: "15%" },
        { data: "KM", title: "KM", width: "15%" },
        { data: "Litros", title: "Litros", width: "15%" }
    ];
    _gridItemAbastecimentos = new BasicDataTable(_item.GridAbastecimento.id, headerAbastecimento, menuOpcoesAbastecimento, { column: 0, dir: orderDir.asc });

    new BuscarAbastecimentos(_item.Abastecimento, function (r) {
        if (r != null) {
            var abastecimentos = _gridItemAbastecimentos.BuscarRegistros();
            for (var i = 0; i < r.length; i++)
                abastecimentos.push({
                    CodigoInterno: 0,
                    Codigo: r[i].Codigo,
                    Horimetro: r[i].Horimetro,
                    CodigoEquipamento: r[i].CodigoEquipamento,
                    CodigoItem: _item.Codigo.val(),
                    Placa: r[i].Placa,
                    Posto: r[i].Posto,
                    Data: r[i].Data,
                    KM: r[i].KM,
                    Litros: r[i].Litros
                });

            _gridItemAbastecimentos.CarregarGrid(abastecimentos);
        }
    }, _gridItemAbastecimentos, _item.Veiculo, _item.Produto, null, null, _documentoEntrada.Fornecedor, _documentoEntrada.DataEmissao);
    _item.Abastecimento.basicTable = _gridItemAbastecimentos;

    //Aba Ordens de Serviço
    var menuOpcoesOrdemServico = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: function (data) { ExcluirOrdemServicoClick(_item.OrdemServicoMultiplaSelecao, data); } }] };
    var headerOrdemServico = [
        { data: "CodigoInterno", visible: false },
        { data: "Codigo", visible: false },
        { data: "CodigoItem", visible: false },

        { data: "Numero", title: "Número", width: "10%" },
        { data: "DataProgramada", title: "Data", width: "10%" },
        { data: "Veiculo", title: "Veículo", width: "10%" },
        { data: "Equipamento", title: "Equipamento", width: "10%" },
        { data: "NumeroFrota", title: "Número Frota", width: "10%" },
        { data: "Motorista", title: "Motorista", width: "10%" },
        { data: "LocalManutencao", title: "Local Manutenção", width: "10%" },
        { data: "Operador", title: "Operador", width: "10%" },
        { data: "TipoManutencao", title: "Tipo Manutenção", width: "10%" },
        { data: "Situacao", title: "Situação", width: "10%" }
    ];
    _gridItemOrdensServico = new BasicDataTable(_item.GridOrdemServico.id, headerOrdemServico, menuOpcoesOrdemServico, { column: 0, dir: orderDir.asc });

    new BuscarOrdemServico(_item.OrdemServicoMultiplaSelecao, function (r) {
        if (r != null) {
            var ordens = _gridItemOrdensServico.BuscarRegistros();
            for (var i = 0; i < r.length; i++) {
                var ordem = r[i];
                ordens.push({
                    CodigoInterno: 0,
                    Codigo: ordem.Codigo,
                    CodigoItem: _item.Codigo.val(),

                    Numero: ordem.Numero,
                    DataProgramada: ordem.DataProgramada,
                    Veiculo: ordem.Veiculo,
                    Equipamento: ordem.Equipamento,
                    NumeroFrota: ordem.NumeroFrota,
                    Motorista: ordem.Motorista,
                    LocalManutencao: ordem.LocalManutencao,
                    Operador: ordem.Operador,
                    TipoManutencao: ordem.TipoManutencao,
                    Situacao: ordem.Situacao
                });
            }

            _gridItemOrdensServico.CarregarGrid(ordens);
        }
    }, _gridItemOrdensServico, [EnumSituacaoOrdemServicoFrota.EmManutencao, EnumSituacaoOrdemServicoFrota.AgNotaFiscal]);
    _item.OrdemServicoMultiplaSelecao.basicTable = _gridItemOrdensServico;
}

function RetornoConsultaNaturezaOperacaoItem(dados) {
    _item.NaturezaOperacao.codEntity(dados.Codigo);
    _item.NaturezaOperacao.val(dados.Descricao);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFe)
        LimparCampoEntity(_item.CFOP);

    $("#" + _item.CFOP.id).focus();

    ObterDetalhesNaturezaOperacaoItem(dados);
}

function ObterDetalhesNaturezaOperacaoItem(dados) {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFe)
        return;

    executarReST("NaturezaDaOperacao/ObterDetalhes", { Codigo: dados.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {
                var data = r.Data;
                if (data.CodigoPrimeiraCFOP > 0)
                    RetornoConsultaCFOP(data.CodigoPrimeiraCFOP);

                TrocaComponenteModalBuscaCFOPItemDocumentoEntrada(data.QuantidadeCFOPs);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function RetornoBuscarVeiculosItem(data) {
    _item.Veiculo.codEntity(data.Codigo);
    _item.Veiculo.val(data.DescricaoComMarcaModelo);

    _item.CentroResultado.codEntity(data.CodigoCentroResultado);
    _item.CentroResultado.val(data.CentroResultado);
}

function RetornoEquipamentoItem(data) {
    _item.Equipamento.codEntity(data.Codigo);
    _item.Equipamento.val(data.DescricaoComMarcaModelo);
}

function RetornoProdutoTMS(data) {
    _item.Produto.val(data.Descricao);
    _item.Produto.codEntity(data.Codigo);

    _item.UnidadeMedida.val(data.CodigoUnidadeMedida);
    _item.NCM.val(data.CodigoNCM);

    RetornarSemCalculo();
    BuscarFormulaCustoProduto(data.Codigo, 0, "");
}

function BuscarFormulaCustoProduto(codigo, codigoItem, formula) {
    executarReST("Produto/BuscarFormulaCusto", { Codigo: codigo, CodigoItem: codigoItem, Formula: formula }, function (r) {
        if (r.Success) {
            if (r.Data) {
                if (r.Data.ValorDesconto != "") {
                    if (r.Data.ValorDesconto == "+")
                        SomarDescontoClick();
                    else if (r.Data.ValorDesconto == "-")
                        SubtrairDescontoClick();
                }
                if (r.Data.ValorICMS != "") {
                    if (r.Data.ValorICMS == "+")
                        SomarICMSClick();
                    else if (r.Data.ValorICMS == "-")
                        SubtrairICMSClick();
                }
                if (r.Data.ValorCreditoPresumido != "") {
                    if (r.Data.ValorCreditoPresumido == "+")
                        SomarCreditoPresumidoClick();
                    else if (r.Data.ValorCreditoPresumido == "-")
                        SubtrairCreditoPresumidoClick();
                }
                if (r.Data.ValorDiferencial != "") {
                    if (r.Data.ValorDiferencial == "+")
                        SomarDiferencialClick();
                    else if (r.Data.ValorDiferencial == "-")
                        SubtrairDiferencialClick();
                }
                if (r.Data.ValorICMSST != "") {
                    if (r.Data.ValorICMSST == "+")
                        SomarICMSSTClick();
                    else if (r.Data.ValorICMSST == "-")
                        SubtrairICMSSTClick();
                }
                if (r.Data.ValorIPI != "") {
                    if (r.Data.ValorIPI == "+")
                        SomarIPIClick();
                    else if (r.Data.ValorIPI == "-")
                        SubtrairIPIClick();
                }
                if (r.Data.ValorFrete != "") {
                    if (r.Data.ValorFrete == "+")
                        SomarFreteClick();
                    else if (r.Data.ValorFrete == "-")
                        SubtrairFreteClick();
                }
                if (r.Data.ValorSeguro != "") {
                    if (r.Data.ValorSeguro == "+")
                        SomarSeguroClick();
                    else if (r.Data.ValorSeguro == "-")
                        SubtrairSeguroClick();
                }
                if (r.Data.ValorOutras != "") {
                    if (r.Data.ValorOutras == "+")
                        SomarOutrasDespesasClick();
                    else if (r.Data.ValorOutras == "-")
                        SubtrairOutrasDespesasClick();
                }
                if (r.Data.ValorDescontoFora != "") {
                    if (r.Data.ValorDescontoFora == "+")
                        SomarDescontoForaClick();
                    else if (r.Data.ValorDescontoFora == "-")
                        SubtrairDescontoForaClick();
                }
                if (r.Data.ValorImpostoFora != "") {
                    if (r.Data.ValorImpostoFora == "+")
                        SomarImpostosForaClick();
                    else if (r.Data.ValorImpostoFora == "-")
                        SubtrairImpostosForaClick();
                }
                if (r.Data.ValorOutrasFora != "") {
                    if (r.Data.ValorOutrasFora == "+")
                        SomarOutrasDespesasForaClick();
                    else if (r.Data.ValorOutrasFora == "-")
                        SubtrairOutrasDespesasForaClick();
                }
                if (r.Data.ValorFreteFora != "") {
                    if (r.Data.ValorFreteFora == "+")
                        SomarFreteForaClick();
                    else if (r.Data.ValorFreteFora == "-")
                        SubtrairFreteForaClick();
                }
                if (r.Data.ValorICMSFreteFora != "") {
                    if (r.Data.ValorICMSFreteFora == "+")
                        SomarICMSFreteForaClick();
                    else if (r.Data.ValorICMSFreteFora == "-")
                        SubtrairICMSFreteForaClick();
                }
                if (r.Data.ValorDiferencialFreteFora != "") {
                    if (r.Data.ValorDiferencialFreteFora == "+")
                        SomarDiferencialFreteForaClick();
                    else if (r.Data.ValorDiferencialFreteFora == "-")
                        SubtrairDiferencialFreteForaClick();
                }
                if (r.Data.ValorPIS != "") {
                    if (r.Data.ValorPIS == "+")
                        SomarValorPISClick();
                    else if (r.Data.ValorPIS == "-")
                        SubtrairValorPISClick();
                }
                if (r.Data.ValorCOFINS != "") {
                    if (r.Data.ValorCOFINS == "+")
                        SomarValorCOFINSClick();
                    else if (r.Data.ValorCOFINS == "-")
                        SubtrairValorCOFINSClick();
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function RetornoConsultaCFOPItem(codigo, callback) {
    if (codigo == 0) return;
    executarReST("CFOPNotaFiscal/BuscarPorCodigo", { Codigo: codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {
                executarReST("DocumentoEntrada/ObterDetalhesParticipantes", { Fornecedor: _documentoEntrada.Fornecedor.codEntity(), Destinatario: _documentoEntrada.Destinatario.codEntity() }, function (rp) {
                    if (rp.Success) {
                        if (rp.Data) {
                            var data = r.Data;
                            _item.CFOP.codEntity(data.Codigo);
                            var baseCalculoImposto = 0;
                            var valorTotal = Globalize.parseFloat(_item.ValorTotal.val());
                            var desconto = Globalize.parseFloat(_item.Desconto.val());
                            var outrasDespesas = Globalize.parseFloat(_item.ValorOutrasDespesas.val());
                            var valorFrete = Globalize.parseFloat(_item.ValorFrete.val());
                            var valorSeguro = Globalize.parseFloat(_item.ValorSeguro.val());

                            if (isNaN(valorTotal))
                                valorTotal = 0;
                            if (isNaN(desconto))
                                desconto = 0;
                            if (isNaN(outrasDespesas))
                                outrasDespesas = 0;
                            if (isNaN(valorFrete))
                                valorFrete = 0;
                            if (isNaN(valorSeguro))
                                valorSeguro = 0;

                            baseCalculoImposto = valorTotal - desconto + outrasDespesas + valorFrete + valorSeguro;

                            if (!string.IsNullOrWhiteSpace(r.Data.Extensao))
                                _item.CFOP.val(r.Data.CodigoCFOP + "." + r.Data.Extensao + " - " + (r.Data.Descricao || ''));
                            else
                                _item.CFOP.val(r.Data.CodigoCFOP + " - " + r.Data.Descricao);
                            _item.CFOP.entityDescription(_item.CFOP.val());

                            if (r.Data.TipoMovimentoUso.Codigo > 0) {
                                _item.TipoMovimento.codEntity(r.Data.TipoMovimentoUso.Codigo);
                                _item.TipoMovimento.val(r.Data.TipoMovimentoUso.Descricao);
                            }

                            //Aba PIS
                            _item.CSTPIS.val(ObterCSTPISCOFINS(r.Data.CSTPIS));
                            _item.PercentualReducaoBaseCalculoPIS.val(Globalize.format(r.Data.ReducaoBCPIS, "n2"));
                            _item.AliquotaPIS.val(Globalize.format(r.Data.AliquotaPIS, "n2"));

                            if (r.Data.AliquotaPIS > 0)
                                _item.BaseCalculoPIS.val(Globalize.format(baseCalculoImposto, "n2"));
                            else
                                _item.BaseCalculoPIS.val("0,00");

                            _item.ValorPIS.val(Globalize.format(CalcularImposto(_item.BaseCalculoPIS.val(), _item.AliquotaPIS.val(), _item.PercentualReducaoBaseCalculoPIS.val()), "n2"));

                            //Aba COFINS
                            _item.CSTCOFINS.val(ObterCSTPISCOFINS(r.Data.CSTCOFINS));
                            _item.PercentualReducaoBaseCalculoCOFINS.val(Globalize.format(r.Data.ReducaoBCCOFINS, "n2"));
                            _item.AliquotaCOFINS.val(Globalize.format(r.Data.AliquotaCOFINS, "n2"));

                            if (r.Data.AliquotaCOFINS > 0)
                                _item.BaseCalculoCOFINS.val(Globalize.format(baseCalculoImposto, "n2"));
                            else
                                _item.BaseCalculoCOFINS.val("0,00");

                            _item.ValorCOFINS.val(Globalize.format(CalcularImposto(_item.BaseCalculoCOFINS.val(), _item.AliquotaCOFINS.val(), _item.PercentualReducaoBaseCalculoCOFINS.val()), "n2"));

                            //Aba IPI
                            _item.CSTIPI.val(ObterCSTIPI(r.Data.CSTIPI));

                            if (r.Data.AliquotaIPI > 0)
                                _item.BaseCalculoIPI.val(Globalize.format(baseCalculoImposto, "n2"));
                            else
                                _item.BaseCalculoIPI.val("0,00");

                            _item.PercentualReducaoBaseCalculoIPI.val(Globalize.format(r.Data.ReducaoBCIPI, "n2"));
                            _item.AliquotaIPI.val(Globalize.format(r.Data.AliquotaIPI, "n2"));
                            _item.ValorIPI.val(Globalize.format(CalcularImposto(_item.BaseCalculoIPI.val(), _item.AliquotaIPI.val(), _item.PercentualReducaoBaseCalculoIPI.val()), "n2"));

                            //Aba ICMS
                            _item.CSTICMS.val(ObterCSTICMS(r.Data.CSTICMS));

                            var aliquotaICMS = 0;
                            if (rp.Data.Interestadual)
                                aliquotaICMS = r.Data.AliquotaInterestadual;
                            else
                                aliquotaICMS = r.Data.AliquotaInterna;

                            _item.AliquotaICMS.val(Globalize.format(aliquotaICMS, "n4"));

                            if (aliquotaICMS > 0)
                                _item.BaseCalculoICMS.val(Globalize.format(baseCalculoImposto, "n2"));
                            else
                                _item.BaseCalculoICMS.val("0,00");

                            _item.ValorICMS.val(Globalize.format(CalcularImposto(_item.BaseCalculoICMS.val(), _item.AliquotaICMS.val(), 0), "n2"));

                            //Aba Crédito Presumido
                            _item.AliquotaCreditoPresumido.val(Globalize.format(r.Data.AliquotaParaCredito, "n2"));

                            if (r.Data.AliquotaParaCredito > 0)
                                _item.BaseCalculoCreditoPresumido.val(Globalize.format(baseCalculoImposto, "n2"));
                            else
                                _item.BaseCalculoCreditoPresumido.val("0,00");

                            _item.ValorCreditoPresumido.val(Globalize.format(CalcularImposto(_item.BaseCalculoCreditoPresumido.val(), _item.AliquotaCreditoPresumido.val(), 0), "n2"));

                            //Aba Diferencial
                            _item.AliquotaDiferencial.val(Globalize.format(r.Data.AliquotaDiferencial, "n2"));

                            if (r.Data.AliquotaDiferencial > 0)
                                _item.BaseCalculoDiferencial.val(Globalize.format(baseCalculoImposto, "n2"));
                            else
                                _item.BaseCalculoDiferencial.val("0,00");

                            _item.ValorDiferencial.val(Globalize.format(CalcularImposto(_item.BaseCalculoDiferencial.val(), _item.AliquotaDiferencial.val(), 0), "n2"));


                            //Cálculos de Retenções
                            if (data.AliquotaRetencaoPIS > 0)
                                _item.ValorRetencaoPIS.val(Globalize.format(CalcularImposto(baseCalculoImposto, Globalize.format(data.AliquotaRetencaoPIS, "n2"), 0), "n2"));
                            else
                                _item.ValorRetencaoPIS.val("0,00");

                            if (data.AliquotaRetencaoCOFINS > 0)
                                _item.ValorRetencaoCOFINS.val(Globalize.format(CalcularImposto(baseCalculoImposto, Globalize.format(data.AliquotaRetencaoCOFINS, "n2"), 0), "n2"));
                            else
                                _item.ValorRetencaoCOFINS.val("0,00");

                            if (data.AliquotaRetencaoINSS > 0)
                                _item.ValorRetencaoINSS.val(Globalize.format(CalcularImposto(baseCalculoImposto, Globalize.format(data.AliquotaRetencaoINSS, "n2"), 0), "n2"));
                            else
                                _item.ValorRetencaoINSS.val("0,00");

                            if (data.AliquotaRetencaoIPI > 0)
                                _item.ValorRetencaoIPI.val(Globalize.format(CalcularImposto(baseCalculoImposto, Globalize.format(data.AliquotaRetencaoIPI, "n2"), 0), "n2"));
                            else
                                _item.ValorRetencaoIPI.val("0,00");

                            if (data.AliquotaRetencaoCSLL > 0)
                                _item.ValorRetencaoCSLL.val(Globalize.format(CalcularImposto(baseCalculoImposto, Globalize.format(data.AliquotaRetencaoCSLL, "n2"), 0), "n2"));
                            else
                                _item.ValorRetencaoCSLL.val("0,00");

                            if (data.AliquotaRetencaoOutras > 0)
                                _item.ValorRetencaoOutras.val(Globalize.format(CalcularImposto(baseCalculoImposto, Globalize.format(data.AliquotaRetencaoOutras, "n2"), 0), "n2"));
                            else
                                _item.ValorRetencaoOutras.val("0,00");

                            if (data.AliquotaRetencaoIR > 0)
                                _item.ValorRetencaoIR.val(Globalize.format(CalcularImposto(baseCalculoImposto, Globalize.format(data.AliquotaRetencaoIR, "n2"), 0), "n2"));
                            else
                                _item.ValorRetencaoIR.val("0,00");

                            if (data.AliquotaRetencaoISS > 0)
                                _item.ValorRetencaoISS.val(Globalize.format(CalcularImposto(baseCalculoImposto, Globalize.format(data.AliquotaRetencaoISS, "n2"), 0), "n2"));
                            else
                                _item.ValorRetencaoISS.val("0,00");


                            //Regras iguais as da importação de xml
                            if (data.CreditoSobreTotalParaItensSujeitosICMSST) {

                                _item.BaseCalculoDiferencial.val(_item.BaseCalculoICMS.val());
                                _item.AliquotaDiferencial.val(_item.AliquotaICMS.val());
                                _item.ValorDiferencial.val(_item.ValorICMS.val());

                                _item.BaseCalculoICMS.val("0,00");
                                _item.AliquotaICMS.val("0,0000");
                                _item.ValorICMS.val("0,00");

                                _item.BaseSTRetido.val("0,00");
                                _item.ValorSTRetido.val("0,00");
                            }

                            if (callback instanceof Function)
                                callback();
                        } else {
                            exibirMensagem(tipoMensagem.atencao, "Atenção", rp.Msg);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, "Falha", rp.Msg);
                    }
                });
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function SemCalculoDescontoClick(e, sender) {
    $("#btnValores_Desconto")[0].title = "Sem cálculo ";
    $("#btnValores_Desconto")[0].innerHTML = '<i class="fal fa-stop"></i>&nbsp;&nbsp;Sem cálculo';
    CalcularCustoProduto();
}
function SomarDescontoClick(e, sender) {
    $("#btnValores_Desconto")[0].title = "Somar ";
    $("#btnValores_Desconto")[0].innerHTML = '<i class="fal fa-plus"></i>&nbsp;&nbsp;Somar';
    CalcularCustoProduto();
}
function SubtrairDescontoClick(e, sender) {
    $("#btnValores_Desconto")[0].title = "Subtrair ";
    $("#btnValores_Desconto")[0].innerHTML = '<i class="fal fa-minus"></i>&nbsp;&nbsp;Subtrair';
    CalcularCustoProduto();
}
function SemCalculoOutrasDespesasClick(e, sender) {
    $("#btnValores_OutrasDespesas")[0].title = "Sem cálculo ";
    $("#btnValores_OutrasDespesas")[0].innerHTML = '<i class="fal fa-stop"></i>&nbsp;&nbsp;Sem cálculo';
    CalcularCustoProduto();
}
function SomarOutrasDespesasClick(e, sender) {
    $("#btnValores_OutrasDespesas")[0].title = "Somar ";
    $("#btnValores_OutrasDespesas")[0].innerHTML = '<i class="fal fa-plus"></i>&nbsp;&nbsp;Somar';
    CalcularCustoProduto();
}
function SubtrairOutrasDespesasClick(e, sender) {
    $("#btnValores_OutrasDespesas")[0].title = "Subtrair ";
    $("#btnValores_OutrasDespesas")[0].innerHTML = '<i class="fal fa-minus"></i>&nbsp;&nbsp;Subtrair';
    CalcularCustoProduto();
}
function SemCalculoFreteClick(e, sender) {
    $("#btnValores_Frete")[0].title = "Sem cálculo ";
    $("#btnValores_Frete")[0].innerHTML = '<i class="fal fa-stop"></i>&nbsp;&nbsp;Sem cálculo';
    CalcularCustoProduto();
}
function SomarFreteClick(e, sender) {
    $("#btnValores_Frete")[0].title = "Somar ";
    $("#btnValores_Frete")[0].innerHTML = '<i class="fal fa-plus"></i>&nbsp;&nbsp;Somar';
    CalcularCustoProduto();
}
function SubtrairFreteClick(e, sender) {
    $("#btnValores_Frete")[0].title = "Subtrair ";
    $("#btnValores_Frete")[0].innerHTML = '<i class="fal fa-minus"></i>&nbsp;&nbsp;Subtrair';
    CalcularCustoProduto();
}
function SemCalculoSeguroClick(e, sender) {
    $("#btnValores_Seguro")[0].title = "Sem cálculo ";
    $("#btnValores_Seguro")[0].innerHTML = '<i class="fal fa-stop"></i>&nbsp;&nbsp;Sem cálculo';
    CalcularCustoProduto();
}
function SomarSeguroClick(e, sender) {
    $("#btnValores_Seguro")[0].title = "Somar ";
    $("#btnValores_Seguro")[0].innerHTML = '<i class="fal fa-plus"></i>&nbsp;&nbsp;Somar';
    CalcularCustoProduto();
}
function SubtrairSeguroClick(e, sender) {
    $("#btnValores_Seguro")[0].title = "Subtrair ";
    $("#btnValores_Seguro")[0].innerHTML = '<i class="fal fa-minus"></i>&nbsp;&nbsp;Subtrair';
    CalcularCustoProduto();
}
function SemCalculoICMSClick(e, sender) {
    $("#btnValores_ICMS")[0].title = "Sem cálculo ";
    $("#btnValores_ICMS")[0].innerHTML = '<i class="fal fa-stop"></i>&nbsp;&nbsp;Sem cálculo';
    CalcularCustoProduto();
}
function SomarICMSClick(e, sender) {
    $("#btnValores_ICMS")[0].title = "Somar ";
    $("#btnValores_ICMS")[0].innerHTML = '<i class="fal fa-plus"></i>&nbsp;&nbsp;Somar';
    CalcularCustoProduto();
}
function SubtrairICMSClick(e, sender) {
    $("#btnValores_ICMS")[0].title = "Subtrair ";
    $("#btnValores_ICMS")[0].innerHTML = '<i class="fal fa-minus"></i>&nbsp;&nbsp;Subtrair';
    CalcularCustoProduto();
}
function SemCalculoIPIClick(e, sender) {
    $("#btnValores_IPI")[0].title = "Sem cálculo ";
    $("#btnValores_IPI")[0].innerHTML = '<i class="fal fa-stop"></i>&nbsp;&nbsp;Sem cálculo';
    CalcularCustoProduto();
}
function SomarIPIClick(e, sender) {
    $("#btnValores_IPI")[0].title = "Somar ";
    $("#btnValores_IPI")[0].innerHTML = '<i class="fal fa-plus"></i>&nbsp;&nbsp;Somar';
    CalcularCustoProduto();
}
function SubtrairIPIClick(e, sender) {
    $("#btnValores_IPI")[0].title = "Subtrair ";
    $("#btnValores_IPI")[0].innerHTML = '<i class="fal fa-minus"></i>&nbsp;&nbsp;Subtrair';
    CalcularCustoProduto();
}
function SemCalculoICMSSTClick(e, sender) {
    $("#btnValores_ICMSST")[0].title = "Sem cálculo ";
    $("#btnValores_ICMSST")[0].innerHTML = '<i class="fal fa-stop"></i>&nbsp;&nbsp;Sem cálculo';
    CalcularCustoProduto();
}
function SomarICMSSTClick(e, sender) {
    $("#btnValores_ICMSST")[0].title = "Somar ";
    $("#btnValores_ICMSST")[0].innerHTML = '<i class="fal fa-plus"></i>&nbsp;&nbsp;Somar';
    CalcularCustoProduto();
}
function SubtrairICMSSTClick(e, sender) {
    $("#btnValores_ICMSST")[0].title = "Subtrair ";
    $("#btnValores_ICMSST")[0].innerHTML = '<i class="fal fa-minus"></i>&nbsp;&nbsp;Subtrair';
    CalcularCustoProduto();
}
function SemCalculoCreditoPresumidoClick(e, sender) {
    $("#btnValores_CreditoPresumido")[0].title = "Sem cálculo ";
    $("#btnValores_CreditoPresumido")[0].innerHTML = '<i class="fal fa-stop"></i>&nbsp;&nbsp;Sem cálculo';
    CalcularCustoProduto();
}
function SomarCreditoPresumidoClick(e, sender) {
    $("#btnValores_CreditoPresumido")[0].title = "Somar ";
    $("#btnValores_CreditoPresumido")[0].innerHTML = '<i class="fal fa-plus"></i>&nbsp;&nbsp;Somar';
    CalcularCustoProduto();
}
function SubtrairCreditoPresumidoClick(e, sender) {
    $("#btnValores_CreditoPresumido")[0].title = "Subtrair ";
    $("#btnValores_CreditoPresumido")[0].innerHTML = '<i class="fal fa-minus"></i>&nbsp;&nbsp;Subtrair';
    CalcularCustoProduto();
}
function SemCalculoDiferencialClick(e, sender) {
    $("#btnValores_Diferencial")[0].title = "Sem cálculo ";
    $("#btnValores_Diferencial")[0].innerHTML = '<i class="fal fa-stop"></i>&nbsp;&nbsp;Sem cálculo';
    CalcularCustoProduto();
}
function SomarDiferencialClick(e, sender) {
    $("#btnValores_Diferencial")[0].title = "Somar ";
    $("#btnValores_Diferencial")[0].innerHTML = '<i class="fal fa-plus"></i>&nbsp;&nbsp;Somar';
    CalcularCustoProduto();
}
function SubtrairDiferencialClick(e, sender) {
    $("#btnValores_Diferencial")[0].title = "Subtrair ";
    $("#btnValores_Diferencial")[0].innerHTML = '<i class="fal fa-minus"></i>&nbsp;&nbsp;Subtrair';
    CalcularCustoProduto();
}
function SemCalculoFreteForaClick(e, sender) {
    $("#btnValores_FreteFora")[0].title = "Sem cálculo ";
    $("#btnValores_FreteFora")[0].innerHTML = '<i class="fal fa-stop"></i>&nbsp;&nbsp;Sem cálculo';
    CalcularCustoProduto();
}
function SomarFreteForaClick(e, sender) {
    $("#btnValores_FreteFora")[0].title = "Somar ";
    $("#btnValores_FreteFora")[0].innerHTML = '<i class="fal fa-plus"></i>&nbsp;&nbsp;Somar';
    CalcularCustoProduto();
}
function SubtrairFreteForaClick(e, sender) {
    $("#btnValores_FreteFora")[0].title = "Subtrair ";
    $("#btnValores_FreteFora")[0].innerHTML = '<i class="fal fa-minus"></i>&nbsp;&nbsp;Subtrair';
    CalcularCustoProduto();
}
function SemCalculoOutrasDespesasForaClick(e, sender) {
    $("#btnValores_OutrasDespesasFora")[0].title = "Sem cálculo ";
    $("#btnValores_OutrasDespesasFora")[0].innerHTML = '<i class="fal fa-stop"></i>&nbsp;&nbsp;Sem cálculo';
    CalcularCustoProduto();
}
function SomarOutrasDespesasForaClick(e, sender) {
    $("#btnValores_OutrasDespesasFora")[0].title = "Somar ";
    $("#btnValores_OutrasDespesasFora")[0].innerHTML = '<i class="fal fa-plus"></i>&nbsp;&nbsp;Somar';
    CalcularCustoProduto();
}
function SubtrairOutrasDespesasForaClick(e, sender) {
    $("#btnValores_OutrasDespesasFora")[0].title = "Subtrair ";
    $("#btnValores_OutrasDespesasFora")[0].innerHTML = '<i class="fal fa-minus"></i>&nbsp;&nbsp;Subtrair';
    CalcularCustoProduto();
}
function SemCalculoDescontoForaClick(e, sender) {
    $("#btnValores_OutrasDespesasFora")[0].title = "Sem cálculo ";
    $("#btnValores_OutrasDespesasFora")[0].innerHTML = '<i class="fal fa-stop"></i>&nbsp;&nbsp;Sem cálculo';
    CalcularCustoProduto();
}
function SomarDescontoForaClick(e, sender) {
    $("#btnValores_DescontoFora")[0].title = "Somar ";
    $("#btnValores_DescontoFora")[0].innerHTML = '<i class="fal fa-plus"></i>&nbsp;&nbsp;Somar';
    CalcularCustoProduto();
}
function SubtrairDescontoForaClick(e, sender) {
    $("#btnValores_DescontoFora")[0].title = "Subtrair ";
    $("#btnValores_DescontoFora")[0].innerHTML = '<i class="fal fa-minus"></i>&nbsp;&nbsp;Subtrair';
    CalcularCustoProduto();
}
function SemCalculoImpostosForaClick(e, sender) {
    $("#btnValores_ImpostosFora")[0].title = "Sem cálculo ";
    $("#btnValores_ImpostosFora")[0].innerHTML = '<i class="fal fa-stop"></i>&nbsp;&nbsp;Sem cálculo';
    CalcularCustoProduto();
}
function SomarImpostosForaClick(e, sender) {
    $("#btnValores_ImpostosFora")[0].title = "Somar ";
    $("#btnValores_ImpostosFora")[0].innerHTML = '<i class="fal fa-plus"></i>&nbsp;&nbsp;Somar';
    CalcularCustoProduto();
}
function SubtrairImpostosForaClick(e, sender) {
    $("#btnValores_ImpostosFora")[0].title = "Subtrair ";
    $("#btnValores_ImpostosFora")[0].innerHTML = '<i class="fal fa-minus"></i>&nbsp;&nbsp;Subtrair';
    CalcularCustoProduto();
}
function SemCalculoDiferencialFreteForaClick(e, sender) {
    $("#btnValores_DiferencialFreteFora")[0].title = "Sem cálculo ";
    $("#btnValores_DiferencialFreteFora")[0].innerHTML = '<i class="fal fa-stop"></i>&nbsp;&nbsp;Sem cálculo';
    CalcularCustoProduto();
}
function SomarDiferencialFreteForaClick(e, sender) {
    $("#btnValores_DiferencialFreteFora")[0].title = "Somar ";
    $("#btnValores_DiferencialFreteFora")[0].innerHTML = '<i class="fal fa-plus"></i>&nbsp;&nbsp;Somar';
    CalcularCustoProduto();
}
function SubtrairDiferencialFreteForaClick(e, sender) {
    $("#btnValores_DiferencialFreteFora")[0].title = "Subtrair ";
    $("#btnValores_DiferencialFreteFora")[0].innerHTML = '<i class="fal fa-minus"></i>&nbsp;&nbsp;Subtrair';
    CalcularCustoProduto();
}
function SemCalculoValorCOFINSClick(e, sender) {
    $("#btnValores_ValorCOFINS")[0].title = "Sem cálculo ";
    $("#btnValores_ValorCOFINS")[0].innerHTML = '<i class="fal fa-stop"></i>&nbsp;&nbsp;Sem cálculo';
    CalcularCustoProduto();
}
function SomarValorCOFINSClick(e, sender) {
    $("#btnValores_ValorCOFINS")[0].title = "Somar ";
    $("#btnValores_ValorCOFINS")[0].innerHTML = '<i class="fal fa-plus"></i>&nbsp;&nbsp;Somar';
    CalcularCustoProduto();
}
function SubtrairValorCOFINSClick(e, sender) {
    $("#btnValores_ValorCOFINS")[0].title = "Subtrair ";
    $("#btnValores_ValorCOFINS")[0].innerHTML = '<i class="fal fa-minus"></i>&nbsp;&nbsp;Subtrair';
    CalcularCustoProduto();
}
function SemCalculoValorPISClick(e, sender) {
    $("#btnValores_ValorPIS")[0].title = "Sem cálculo ";
    $("#btnValores_ValorPIS")[0].innerHTML = '<i class="fal fa-stop"></i>&nbsp;&nbsp;Sem cálculo';
    CalcularCustoProduto();
}
function SomarValorPISClick(e, sender) {
    $("#btnValores_ValorPIS")[0].title = "Somar ";
    $("#btnValores_ValorPIS")[0].innerHTML = '<i class="fal fa-plus"></i>&nbsp;&nbsp;Somar';
    CalcularCustoProduto();
}
function SubtrairValorPISClick(e, sender) {
    $("#btnValores_ValorPIS")[0].title = "Subtrair ";
    $("#btnValores_ValorPIS")[0].innerHTML = '<i class="fal fa-minus"></i>&nbsp;&nbsp;Subtrair';
    CalcularCustoProduto();
}
function SemCalculoICMSFreteForaClick(e, sender) {
    $("#btnValores_ICMSFreteFora")[0].title = "Sem cálculo ";
    $("#btnValores_ICMSFreteFora")[0].innerHTML = '<i class="fal fa-stop"></i>&nbsp;&nbsp;Sem cálculo';
    CalcularCustoProduto();
}
function SomarICMSFreteForaClick(e, sender) {
    $("#btnValores_ICMSFreteFora")[0].title = "Somar ";
    $("#btnValores_ICMSFreteFora")[0].innerHTML = '<i class="fal fa-plus"></i>&nbsp;&nbsp;Somar';
    CalcularCustoProduto();
}
function SubtrairICMSFreteForaClick(e, sender) {
    $("#btnValores_ICMSFreteFora")[0].title = "Subtrair ";
    $("#btnValores_ICMSFreteFora")[0].innerHTML = '<i class="fal fa-minus"></i>&nbsp;&nbsp;Subtrair';
    CalcularCustoProduto();
}

function AtualizarValorICMS() {
    var baseCalculo = Globalize.parseFloat(_item.BaseCalculoICMS.val());
    var aliquota = Globalize.parseFloat(_item.AliquotaICMS.val());

    _item.ValorICMS.val(Globalize.format(CalcularImposto(baseCalculo, aliquota, 0), "n2"));
}

function AtualizarValorPIS() {
    var baseCalculo = Globalize.parseFloat(_item.BaseCalculoPIS.val());
    var aliquota = Globalize.parseFloat(_item.AliquotaPIS.val());
    var percentualReducaoBC = Globalize.parseFloat(_item.PercentualReducaoBaseCalculoPIS.val());

    _item.ValorPIS.val(Globalize.format(CalcularImposto(baseCalculo, aliquota, percentualReducaoBC), "n2"));
}

function AtualizarValorCOFINS() {
    var baseCalculo = Globalize.parseFloat(_item.BaseCalculoCOFINS.val());
    var aliquota = Globalize.parseFloat(_item.AliquotaCOFINS.val());
    var percentualReducaoBC = Globalize.parseFloat(_item.PercentualReducaoBaseCalculoCOFINS.val());

    _item.ValorCOFINS.val(Globalize.format(CalcularImposto(baseCalculo, aliquota, percentualReducaoBC), "n2"));
}

function AtualizarValorIPI() {
    var baseCalculo = Globalize.parseFloat(_item.BaseCalculoIPI.val());
    var aliquota = Globalize.parseFloat(_item.AliquotaIPI.val());
    var percentualReducaoBC = Globalize.parseFloat(_item.PercentualReducaoBaseCalculoIPI.val());

    _item.ValorIPI.val(Globalize.format(CalcularImposto(baseCalculo, aliquota, percentualReducaoBC), "n2"));
}

function AtualizarValorICMSST() {
    var baseCalculo = Globalize.parseFloat(_item.BaseCalculoICMSST.val());
    var aliquota = Globalize.parseFloat(_item.AliquotaICMSST.val());

    _item.ValorICMSST.val(Globalize.format(CalcularImposto(baseCalculo, aliquota, 0), "n2"));
}

function AtualizarValorCreditoPresumido() {
    var baseCalculo = Globalize.parseFloat(_item.BaseCalculoCreditoPresumido.val());
    var aliquota = Globalize.parseFloat(_item.AliquotaCreditoPresumido.val());

    _item.ValorCreditoPresumido.val(Globalize.format(CalcularImposto(baseCalculo, aliquota, 0), "n2"));
}

function AtualizarValorDiferencial() {
    var baseCalculo = Globalize.parseFloat(_item.BaseCalculoDiferencial.val());
    var aliquota = Globalize.parseFloat(_item.AliquotaDiferencial.val());

    _item.ValorDiferencial.val(Globalize.format(CalcularImposto(baseCalculo, aliquota, 0), "n2"));
}

function CalcularImposto(baseCalculo, aliquota, percentualReducaoBC) {
    if (typeof baseCalculo === "string")
        baseCalculo = Globalize.parseFloat(baseCalculo);
    if (typeof aliquota === "string")
        aliquota = Globalize.parseFloat(aliquota);
    if (typeof percentualReducaoBC === "string")
        percentualReducaoBC = Globalize.parseFloat(percentualReducaoBC);

    if (isNaN(baseCalculo))
        baseCalculo = 0;
    if (isNaN(aliquota))
        aliquota = 0;
    if (isNaN(percentualReducaoBC))
        percentualReducaoBC = 0;

    var imposto = baseCalculo * (1 - (percentualReducaoBC / 100)) * (aliquota / 100);

    return imposto;
}

function AlterarValorTotalItem() {
    var quantidade = Globalize.parseFloat(_item.Quantidade.val());
    if (isNaN(quantidade))
        quantidade = 0;

    var valorUnitario = Globalize.parseFloat(_item.ValorUnitario.val());
    if (isNaN(valorUnitario))
        valorUnitario = 0;

    var valorTotal = Globalize.format((quantidade * valorUnitario), "n2");

    _item.ValorTotal.val(valorTotal);
}

function AlterarQuantidadeValorUnitario() {
    var valorTotal = Globalize.parseFloat(_item.ValorTotal.val());
    if (isNaN(valorTotal))
        valorTotal = 0;

    var quantidade = Globalize.parseFloat(_item.Quantidade.val());
    if (isNaN(quantidade))
        quantidade = 0;

    if (quantidade > 0) {
        var valorUnitario = Globalize.format((valorTotal / quantidade), "n4");
        _item.ValorUnitario.val(valorUnitario);
    }
}

function AtualizarTotaisImpostos() {
    var valorTotalICMS = 0,
        valorTotalICMSST = 0,
        totalValorSTRetido = 0,
        totalBaseSTRetido = 0,
        valorTotalPIS = 0,
        valorTotalCOFINS = 0,
        valorTotalIPI = 0,
        valorTotalCreditoPresumido = 0,
        valorTotalDiferencial = 0,
        valorTotalDesconto = 0,
        valorTotalOutrasDespesas = 0,
        valorTotalFrete = 0,
        valorTotalSeguro = 0,
        valorTotalFreteFota = 0,
        valorTotalOutrasDespesasFora = 0,
        valorTotalDescontoFora = 0,
        valorTotalImpostosFora = 0,
        valorTotalDiferencialFreteFota = 0,
        valorTotalCusto = 0,
        totalBaseCalculoICMS = 0,
        totalBaseCalculoICMSST = 0,
        totalBaseCalculoPIS = 0,
        totalBaseCalculoCOFINS = 0,
        totalBaseCalculoIPI = 0,
        totalBaseCalculoCreditoPresumido = 0,
        totalBaseCalculoDiferencial = 0,
        valorTotalRetencaoPIS = 0,
        valorTotalRetencaoCOFINS = 0,
        valorTotalRetencaoINSS = 0,
        valorTotalRetencaoIPI = 0,
        valorTotalRetencaoCSLL = 0,
        valorTotalRetencaoOutras = 0,
        valorTotalRetencaoIR = 0,
        valorTotalRetencaoISS = 0,
        valorTotalBruto = 0,
        valorTotalProdutos = 0,
        valorTotalTotal = 0;

    $.each(_documentoEntrada.Itens.list, function (i, item) {
        var valorICMS = Globalize.parseFloat(item.ValorICMS.val);
        var valorPIS = Globalize.parseFloat(item.ValorPIS.val);
        var valorCOFINS = Globalize.parseFloat(item.ValorCOFINS.val);
        var valorIPI = Globalize.parseFloat(item.ValorIPI.val);
        var valorICMSST = Globalize.parseFloat(item.ValorICMSST.val);
        var valorCreditoPresumido = Globalize.parseFloat(item.ValorCreditoPresumido.val);
        var valorDiferencial = Globalize.parseFloat(item.ValorDiferencial.val);

        var baseSTRetido = Globalize.parseFloat(item.BaseSTRetido.val);
        var valorSTRetido = Globalize.parseFloat(item.ValorSTRetido.val);

        var valorDesconto = Globalize.parseFloat(item.Desconto.val);
        var valorOutrasDespesas = Globalize.parseFloat(item.ValorOutrasDespesas.val);
        var valorFrete = Globalize.parseFloat(item.ValorFrete.val);
        var valorSeguro = Globalize.parseFloat(item.ValorSeguro.val);
        var valorFreteFota = Globalize.parseFloat(item.ValorFreteFora.val);
        var valorOutrasDespesasFora = Globalize.parseFloat(item.ValorOutrasDespesasFora.val);
        var valorDescontoFora = Globalize.parseFloat(item.ValorDescontoFora.val);
        var valorImpostosFora = Globalize.parseFloat(item.ValorImpostosFora.val);
        var valorDiferencialFreteFota = Globalize.parseFloat(item.ValorDiferencialFreteFora.val);
        var valorCusto = Globalize.parseFloat(item.ValorCustoTotal.val);

        var valorRetencaoPIS = Globalize.parseFloat(item.ValorRetencaoPIS.val);
        var valorRetencaoCOFINS = Globalize.parseFloat(item.ValorRetencaoCOFINS.val);
        var valorRetencaoINSS = Globalize.parseFloat(item.ValorRetencaoINSS.val);
        var valorRetencaoIPI = Globalize.parseFloat(item.ValorRetencaoIPI.val);
        var valorRetencaoCSLL = Globalize.parseFloat(item.ValorRetencaoCSLL.val);
        var valorRetencaoOutras = Globalize.parseFloat(item.ValorRetencaoOutras.val);
        var valorRetencaoIR = Globalize.parseFloat(item.ValorRetencaoIR.val);
        var valorRetencaoISS = Globalize.parseFloat(item.ValorRetencaoISS.val);

        var baseCalculoICMS = Globalize.parseFloat(item.BaseCalculoICMS.val);
        var baseCalculoPIS = Globalize.parseFloat(item.BaseCalculoPIS.val);
        var baseCalculoCOFINS = Globalize.parseFloat(item.BaseCalculoCOFINS.val);
        var baseCalculoIPI = Globalize.parseFloat(item.BaseCalculoIPI.val);
        var baseCalculoICMSST = Globalize.parseFloat(item.BaseCalculoICMSST.val);
        var baseCalculoCreditoPresumido = Globalize.parseFloat(item.BaseCalculoCreditoPresumido.val);
        var baseCalculoDiferencial = Globalize.parseFloat(item.BaseCalculoDiferencial.val);

        var valorBruto = Globalize.parseFloat(item.ValorTotal.val);
        var valorProdutos = Globalize.parseFloat(item.ValorTotal.val);
        var valorTotal = Globalize.parseFloat(item.ValorTotal.val);

        if (isNaN(valorICMS))
            valorICMS = 0;
        if (isNaN(valorPIS))
            valorPIS = 0;
        if (isNaN(valorCOFINS))
            valorCOFINS = 0;
        if (isNaN(valorIPI))
            valorIPI = 0;
        if (isNaN(valorICMSST))
            valorICMSST = 0;
        if (isNaN(baseSTRetido))
            baseSTRetido = 0;
        if (isNaN(valorSTRetido))
            valorSTRetido = 0;
        if (isNaN(valorCreditoPresumido))
            valorCreditoPresumido = 0;
        if (isNaN(valorDiferencial))
            valorDiferencial = 0;
        if (isNaN(baseCalculoICMS))
            baseCalculoICMS = 0;
        if (isNaN(baseCalculoPIS))
            baseCalculoPIS = 0;
        if (isNaN(baseCalculoCOFINS))
            baseCalculoCOFINS = 0;
        if (isNaN(baseCalculoIPI))
            baseCalculoIPI = 0;
        if (isNaN(baseCalculoICMSST))
            baseCalculoICMSST = 0;
        if (isNaN(baseCalculoCreditoPresumido))
            base = 0;
        if (isNaN(baseCalculoDiferencial))
            baseCalculoDiferencial = 0;

        if (isNaN(valorDesconto))
            valorDesconto = 0;
        if (isNaN(valorOutrasDespesas))
            valorOutrasDespesas = 0;
        if (isNaN(valorFrete))
            valorFrete = 0;
        if (isNaN(valorSeguro))
            valorSeguro = 0;
        if (isNaN(valorFreteFota))
            valorFreteFota = 0;
        if (isNaN(valorOutrasDespesasFora))
            valorOutrasDespesasFora = 0;
        if (isNaN(valorDescontoFora))
            valorDescontoFora = 0;
        if (isNaN(valorImpostosFora))
            valorImpostosFora = 0;
        if (isNaN(valorDiferencialFreteFota))
            valorDiferencialFreteFota = 0;
        if (isNaN(valorCusto))
            valorCusto = 0;
        if (isNaN(valorRetencaoPIS))
            valorRetencaoPIS = 0;
        if (isNaN(valorRetencaoCOFINS))
            valorRetencaoCOFINS = 0;
        if (isNaN(valorRetencaoINSS))
            valorRetencaoINSS = 0;
        if (isNaN(valorRetencaoIPI))
            valorRetencaoIPI = 0;
        if (isNaN(valorRetencaoCSLL))
            valorRetencaoCSLL = 0;
        if (isNaN(valorRetencaoOutras))
            valorRetencaoOutras = 0;
        if (isNaN(valorRetencaoIR))
            valorRetencaoIR = 0;
        if (isNaN(valorRetencaoISS))
            valorRetencaoISS = 0;

        if (isNaN(valorBruto))
            valorBruto = 0;
        if (isNaN(valorProdutos))
            valorProdutos = 0;
        if (isNaN(valorTotal))
            valorTotal = 0;
        valorTotal = valorTotal + valorSeguro + valorICMSST + valorFrete + valorOutrasDespesas + valorIPI - valorDesconto;

        valorTotalICMS += valorICMS;
        valorTotalCOFINS += valorCOFINS;
        valorTotalICMSST += valorICMSST;
        totalBaseSTRetido += baseSTRetido;
        totalValorSTRetido += valorSTRetido;
        valorTotalIPI += valorIPI;
        valorTotalPIS += valorPIS;
        valorTotalCreditoPresumido += valorTotalCreditoPresumido;
        valorTotalDiferencial += valorTotalDiferencial;
        totalBaseCalculoCOFINS += baseCalculoCOFINS;
        totalBaseCalculoCreditoPresumido += baseCalculoCreditoPresumido;
        totalBaseCalculoDiferencial += baseCalculoDiferencial;
        totalBaseCalculoICMS += baseCalculoICMS;
        totalBaseCalculoICMSST += baseCalculoICMSST;
        totalBaseCalculoIPI += baseCalculoIPI;
        totalBaseCalculoPIS += baseCalculoPIS;
        valorTotalDesconto += valorDesconto;
        valorTotalOutrasDespesas += valorOutrasDespesas;
        valorTotalFrete += valorFrete;
        valorTotalSeguro += valorSeguro;
        valorTotalFreteFota += valorFreteFota;
        valorTotalOutrasDespesasFora += valorOutrasDespesasFora;
        valorTotalDescontoFora += valorDescontoFora;
        valorTotalImpostosFora += valorImpostosFora;
        valorTotalDiferencialFreteFota += valorDiferencialFreteFota;
        valorTotalCusto += valorCusto;
        valorTotalRetencaoCOFINS += valorRetencaoCOFINS;
        valorTotalRetencaoCSLL += valorRetencaoCSLL;
        valorTotalRetencaoINSS += valorRetencaoINSS;
        valorTotalRetencaoIPI += valorRetencaoIPI;
        valorTotalRetencaoOutras += valorRetencaoOutras;
        valorTotalRetencaoIR += valorRetencaoIR;
        valorTotalRetencaoISS += valorRetencaoISS;
        valorTotalRetencaoPIS += valorRetencaoPIS;

        valorTotalBruto += valorBruto;
        valorTotalProdutos += valorProdutos;
        valorTotalTotal += valorTotal;
    });

    _documentoEntrada.BaseCalculoICMS.val(Globalize.format(totalBaseCalculoICMS, "n2"));
    _documentoEntrada.ValorTotalICMS.val(Globalize.format(valorTotalICMS, "n2"));
    _documentoEntrada.BaseCalculoICMSST.val(Globalize.format(totalBaseCalculoICMSST, "n2"));
    _documentoEntrada.ValorTotalICMSST.val(Globalize.format(valorTotalICMSST, "n2"));
    //talvez precise criar uma regra pois varias notas os valores de icms retiro não vem nos itens e só vem na observação
    _documentoEntrada.BaseSTRetido.val(Globalize.format(totalBaseSTRetido, "n2"));
    _documentoEntrada.ValorSTRetido.val(Globalize.format(totalValorSTRetido, "n2"));
    //
    _documentoEntrada.ValorTotalIPI.val(Globalize.format(valorTotalIPI, "n2"));
    _documentoEntrada.ValorTotalPIS.val(Globalize.format(valorTotalPIS, "n2"));
    _documentoEntrada.ValorTotalCOFINS.val(Globalize.format(valorTotalCOFINS, "n2"));
    _documentoEntrada.ValorTotalCreditoPresumido.val(Globalize.format(valorTotalCreditoPresumido, "n2"));
    _documentoEntrada.ValorTotalDiferencial.val(Globalize.format(valorTotalDiferencial, "n2"));

    _documentoEntrada.ValorTotalDesconto.val(Globalize.format(valorTotalDesconto, "n2"));
    _documentoEntrada.ValorTotalOutrasDespesas.val(Globalize.format(valorTotalOutrasDespesas, "n2"));
    _documentoEntrada.ValorTotalFrete.val(Globalize.format(valorTotalFrete, "n2"));
    _documentoEntrada.ValorTotalSeguro.val(Globalize.format(valorTotalSeguro, "n2"));
    _documentoEntrada.ValorTotalFreteFora.val(Globalize.format(valorTotalFreteFota, "n2"));
    _documentoEntrada.ValorTotalOutrasDespesasFora.val(Globalize.format(valorTotalOutrasDespesasFora, "n2"));
    _documentoEntrada.ValorTotalDescontoFora.val(Globalize.format(valorTotalDescontoFora, "n2"));
    _documentoEntrada.ValorTotalImpostosFora.val(Globalize.format(valorTotalImpostosFora, "n2"));
    _documentoEntrada.ValorTotalDiferencialFreteFora.val(Globalize.format(valorTotalDiferencialFreteFota, "n2"));
    _documentoEntrada.ValorTotalCusto.val(Globalize.format(valorTotalCusto, "n2"));
    _documentoEntrada.ValorTotalRetencaoPIS.val(Globalize.format(valorTotalRetencaoPIS, "n2"));
    _documentoEntrada.ValorTotalRetencaoCOFINS.val(Globalize.format(valorTotalRetencaoCOFINS, "n2"));
    _documentoEntrada.ValorTotalRetencaoINSS.val(Globalize.format(valorTotalRetencaoINSS, "n2"));
    _documentoEntrada.ValorTotalRetencaoIPI.val(Globalize.format(valorTotalRetencaoIPI, "n2"));
    _documentoEntrada.ValorTotalRetencaoCSLL.val(Globalize.format(valorTotalRetencaoCSLL, "n2"));
    _documentoEntrada.ValorTotalRetencaoOutras.val(Globalize.format(valorTotalRetencaoOutras, "n2"));
    _documentoEntrada.ValorTotalRetencaoIR.val(Globalize.format(valorTotalRetencaoIR, "n2"));
    _documentoEntrada.ValorTotalRetencaoISS.val(Globalize.format(valorTotalRetencaoISS, "n2"));

    _documentoEntrada.ValorBruto.val(Globalize.format(valorTotalBruto, "n2"));
    _documentoEntrada.ValorProdutos.val(Globalize.format(valorTotalProdutos, "n2"));
    _documentoEntrada.ValorTotal.val(Globalize.format(valorTotalTotal, "n2"));
}

function ObterCSTPISCOFINS(cst) {
    switch (cst) {
        case EnumCSTPISCOFINS.CST01:
            return "01";
        case EnumCSTPISCOFINS.CST02:
            return "02";
        case EnumCSTPISCOFINS.CST03:
            return "03";
        case EnumCSTPISCOFINS.CST04:
            return "04";
        case EnumCSTPISCOFINS.CST05:
            return "05";
        case EnumCSTPISCOFINS.CST06:
            return "06";
        case EnumCSTPISCOFINS.CST07:
            return "07";
        case EnumCSTPISCOFINS.CST08:
            return "08";
        case EnumCSTPISCOFINS.CST09:
            return "09";
        case EnumCSTPISCOFINS.CST49:
            return "49";
        case EnumCSTPISCOFINS.CST50:
            return "50";
        case EnumCSTPISCOFINS.CST51:
            return "51";
        case EnumCSTPISCOFINS.CST52:
            return "52";
        case EnumCSTPISCOFINS.CST53:
            return "53";
        case EnumCSTPISCOFINS.CST54:
            return "54";
        case EnumCSTPISCOFINS.CST55:
            return "55";
        case EnumCSTPISCOFINS.CST56:
            return "56";
        case EnumCSTPISCOFINS.CST60:
            return "60";
        case EnumCSTPISCOFINS.CST61:
            return "61";
        case EnumCSTPISCOFINS.CST62:
            return "62";
        case EnumCSTPISCOFINS.CST63:
            return "63";
        case EnumCSTPISCOFINS.CST64:
            return "64";
        case EnumCSTPISCOFINS.CST65:
            return "65";
        case EnumCSTPISCOFINS.CST66:
            return "66";
        case EnumCSTPISCOFINS.CST67:
            return "67";
        case EnumCSTPISCOFINS.CST70:
            return "70";
        case EnumCSTPISCOFINS.CST71:
            return "71";
        case EnumCSTPISCOFINS.CST72:
            return "72";
        case EnumCSTPISCOFINS.CST73:
            return "73";
        case EnumCSTPISCOFINS.CST74:
            return "74";
        case EnumCSTPISCOFINS.CST75:
            return "75";
        case EnumCSTPISCOFINS.CST98:
            return "98";
        case EnumCSTPISCOFINS.CST99:
            return "99";
        default:
            return "";
    }
}

function ObterCSTICMS(cst) {
    switch (cst) {
        case EnumCSTICMS.CST00:
            return "000";
        case EnumCSTICMS.CST10:
            return "010";
        case EnumCSTICMS.CST20:
            return "020";
        case EnumCSTICMS.CST30:
            return "030";
        case EnumCSTICMS.CST40:
            return "040";
        case EnumCSTICMS.CST41:
            return "041";
        case EnumCSTICMS.CST50:
            return "050";
        case EnumCSTICMS.CST51:
            return "051";
        case EnumCSTICMS.CST60:
            return "060";
        case EnumCSTICMS.CST70:
            return "070";
        case EnumCSTICMS.CST90:
            return "090";
        default:
            return "";
    }
}

function ObterCSTIPI(cst) {
    switch (cst) {
        case EnumCSTIPI.CST00:
            return "00";
        case EnumCSTIPI.CST01:
            return "01";
        case EnumCSTIPI.CST02:
            return "02";
        case EnumCSTIPI.CST03:
            return "03";
        case EnumCSTIPI.CST04:
            return "04";
        case EnumCSTIPI.CST05:
            return "05";
        case EnumCSTIPI.CST49:
            return "49";
        case EnumCSTIPI.CST50:
            return "50";
        case EnumCSTIPI.CST51:
            return "51";
        case EnumCSTIPI.CST52:
            return "52";
        case EnumCSTIPI.CST53:
            return "53";
        case EnumCSTIPI.CST54:
            return "54";
        case EnumCSTIPI.CST55:
            return "55";
        case EnumCSTIPI.CST99:
            return "99";
        default:
            return "";
    }
}

function RecarregarGridItem(manterPaginacao) {

    var data = new Array();
    var totalDescontos = 0;
    var totalItem = 0;
    var subtotal = 0;

    $.each(_documentoEntrada.Itens.list, function (i, item) {
        var itemGrid = new Object();

        itemGrid.Codigo = item.Codigo.val;
        if (item.Produto.codEntity <= 0)
            itemGrid.CodigoProduto = 0;
        else
            itemGrid.CodigoProduto = item.Produto.codEntity;
        itemGrid.Item = item.Sequencial.val;

        if (item.Produto.codEntity <= 0)
            itemGrid.Produto = "(NÃO SELECIONADO) " + item.CodigoProdutoFornecedor.val + " - " + item.DescricaoProdutoFornecedor.val;
        else
            itemGrid.Produto = item.Produto.val;

        itemGrid.SiglaUnidadeMedida = item.SiglaUnidadeMedida.val;
        itemGrid.Quantidade = item.Quantidade.val;
        itemGrid.ValorUnitario = item.ValorUnitario.val;
        itemGrid.Desconto = item.Desconto.val;
        itemGrid.ValorTotal = item.ValorTotal.val;
        if (item.NCM != undefined)
            itemGrid.NCM = item.NCM.val;
        else if (item.NCMProdutoFornecedor != undefined)
            itemGrid.NCM = item.NCMProdutoFornecedor.val;

        totalDescontos += Globalize.parseFloat(item.Desconto.val);
        totalItem += Globalize.parseFloat(item.ValorTotal.val);

        itemGrid.CFOP = item.CFOP.codEntity <= 0 ? "NÃO SELECIONADO" : item.CFOP.val;
        itemGrid.Veiculo = item.Veiculo.val;

        itemGrid.CstIcmsFornecedor = item.CstIcmsFornecedor.val;
        itemGrid.CSTICMS = item.CSTICMS.val;

        data.push(itemGrid);
    });

    subtotal = totalItem - totalDescontos;
    _item.TotalDescontosItens.val(Globalize.format(totalDescontos, "n2"));
    _item.TotalValorItens.val(Globalize.format(totalItem, "n2"));
    _item.SubTotalItens.val(Globalize.format(subtotal, "n2"));

    _gridItem.CarregarGrid(data, null, manterPaginacao);
    RecarregarGridAbastecimento();
    RecarregarGridOrdemServico();
}

function MontarGridItemAbastecimento() {
    $.each(_documentoEntrada.Itens.list, function (i, item) {
        $.each(item.ItensAbastecimentos.list, function (j, abastecimento) {
            var abastecimentoGrid = new Object();

            abastecimentoGrid.CodigoInterno = abastecimento.CodigoInterno.val;
            abastecimentoGrid.Codigo = abastecimento.Codigo.val;
            abastecimentoGrid.CodigoItem = abastecimento.CodigoItem.val;
            abastecimentoGrid.CodigoEquipamento = abastecimento.CodigoEquipamento.val;
            abastecimentoGrid.Horimetro = abastecimento.Horimetro.val;

            abastecimentoGrid.Placa = abastecimento.Placa.val;
            abastecimentoGrid.Posto = abastecimento.Posto.val;
            abastecimentoGrid.Data = abastecimento.Data.val;
            abastecimentoGrid.KM = abastecimento.KM.val;
            abastecimentoGrid.Litros = abastecimento.Litros.val;

            _documentoEntrada.ItensAbastecimentos.list.push(abastecimentoGrid);
        });
    });
}

function MontarGridItemOrdensServico() {
    $.each(_documentoEntrada.Itens.list, function (i, item) {
        $.each(item.ItensOrdensServico.list, function (j, ordem) {
            var ordemGrid = new Object();

            ordemGrid.CodigoInterno = ordem.CodigoInterno.val;
            ordemGrid.Codigo = ordem.Codigo.val;
            ordemGrid.CodigoItem = ordem.CodigoItem.val;

            ordemGrid.Numero = ordem.Numero.val;
            ordemGrid.DataProgramada = ordem.DataProgramada.val;
            ordemGrid.Veiculo = ordem.Veiculo.val;
            ordemGrid.Equipamento = ordem.Equipamento.val;
            ordemGrid.NumeroFrota = ordem.NumeroFrota.val;
            ordemGrid.Motorista = ordem.Motorista.val;
            ordemGrid.LocalManutencao = ordem.LocalManutencao.val;
            ordemGrid.Operador = ordem.Operador.val;
            ordemGrid.TipoManutencao = ordem.TipoManutencao.val;
            ordemGrid.Situacao = ordem.Situacao.val;

            _documentoEntrada.ItensOrdensServico.list.push(ordemGrid);
        });
    });
}

function RecarregarGridAbastecimento() {
    _gridItemAbastecimentos.CarregarGrid(_documentoEntrada.ItensAbastecimentos.list.filter(function (obj) { return obj.CodigoItem == _item.Codigo.val(); }));
}

function RecarregarGridOrdemServico() {
    _gridItemOrdensServico.CarregarGrid(_documentoEntrada.ItensOrdensServico.list.filter(function (obj) { return obj.CodigoItem == _item.Codigo.val(); }));
}

function ExcluirAbastecimentoClick(knoutAbastecimento, data) {
    var abastecimentoGrid = knoutAbastecimento.basicTable.BuscarRegistros();

    for (var i = 0; i < abastecimentoGrid.length; i++) {
        if (data.Codigo == abastecimentoGrid[i].Codigo) {
            abastecimentoGrid.splice(i, 1);
            break;
        }
    }

    knoutAbastecimento.basicTable.CarregarGrid(abastecimentoGrid);
}

function ExcluirOrdemServicoClick(knoutOrdemServico, data) {
    var ordemGrid = knoutOrdemServico.basicTable.BuscarRegistros();

    for (var i = 0; i < ordemGrid.length; i++) {
        if (data.Codigo == ordemGrid[i].Codigo) {
            ordemGrid.splice(i, 1);
            break;
        }
    }

    knoutOrdemServico.basicTable.CarregarGrid(ordemGrid);
}

function EditarItemClick(data) {
    for (var i = 0; i < _documentoEntrada.Itens.list.length; i++) {
        if (data.Codigo == _documentoEntrada.Itens.list[i].Codigo.val) {

            var item = _documentoEntrada.Itens.list[i];

            BuscarFormulaCustoProduto(item.Produto.codEntity, item.Codigo.val, item.CalculoCustoProduto.val);

            _item.Codigo.val(item.Codigo.val);
            _item.Sequencial.val(item.Sequencial.val);
            _item.UnidadeMedida.val(item.UnidadeMedida.val);
            _item.Quantidade.val(item.Quantidade.val);
            _item.ValorUnitario.val(item.ValorUnitario.val);
            _item.Desconto.val(item.Desconto.val);
            _item.ValorTotal.val(item.ValorTotal.val);
            _item.Produto.val(item.Produto.val);
            _item.Produto.codEntity(item.Produto.codEntity);
            _item.NCM.val(item.NCM.val);

            if (item.CodigoProdutoFornecedor.val != "" || item.DescricaoProdutoFornecedor.val != "") {
                _item.CodigoProdutoFornecedor.val(item.CodigoProdutoFornecedor.val);
                _item.DescricaoProdutoFornecedor.val(item.DescricaoProdutoFornecedor.val);
                _item.NCMProdutoFornecedor.val(item.NCMProdutoFornecedor.val);
                _item.CESTProdutoFornecedor.val(item.CESTProdutoFornecedor.val);
                _item.CodigoProdutoFornecedor.visible(true);
            } else {
                _item.CodigoProdutoFornecedor.visible(false);
            }

            _item.CodigoBarrasEAN.val(item.CodigoBarrasEAN.val);
            _item.OrdemServico.val(item.OrdemServico.val);
            _item.OrdemServico.codEntity(item.OrdemServico.codEntity);
            _item.OrdemCompraMercadoria.val(item.OrdemCompraMercadoria.val);
            _item.OrdemCompraMercadoria.codEntity(item.OrdemCompraMercadoria.codEntity);
            _item.OrdemCompra.val(_documentoEntrada.OrdemCompra.val);
            _item.OrdemCompra.codEntity(_documentoEntrada.OrdemCompra.codEntity);
            _item.RegraEntradaDocumento.val(item.RegraEntradaDocumento.val);
            _item.RegraEntradaDocumento.codEntity(item.RegraEntradaDocumento.codEntity);
            _item.Veiculo.val(item.Veiculo.val);
            _item.Veiculo.codEntity(item.Veiculo.codEntity);
            _item.Equipamento.val(item.Equipamento.val);
            _item.Equipamento.codEntity(item.Equipamento.codEntity);
            _item.KMAbastecimento.val(item.KMAbastecimento.val);
            _item.Horimetro.val(item.Horimetro.val);
            _item.DataAbastecimento.val(item.DataAbastecimento.val);
            _item.NaturezaOperacao.val(item.NaturezaOperacao.val);
            _item.NaturezaOperacao.codEntity(item.NaturezaOperacao.codEntity);
            _item.CFOP.val(item.CFOP.val);
            _item.CFOP.codEntity(item.CFOP.codEntity);
            _item.TipoMovimento.val(item.TipoMovimento.val);
            _item.TipoMovimento.codEntity(item.TipoMovimento.codEntity);
            _item.ObservacaoItem.val(item.ObservacaoItem.val);
            _item.CSTICMS.val(item.CSTICMS.val);
            _item.BaseCalculoICMS.val(item.BaseCalculoICMS.val);
            _item.AliquotaICMS.val(item.AliquotaICMS.val);
            _item.ValorICMS.val(item.ValorICMS.val);

            _item.CSTPIS.val(item.CSTPIS.val);
            _item.PercentualReducaoBaseCalculoPIS.val(item.PercentualReducaoBaseCalculoPIS.val);
            _item.BaseCalculoPIS.val(item.BaseCalculoPIS.val);
            _item.AliquotaPIS.val(item.AliquotaPIS.val);
            _item.ValorPIS.val(item.ValorPIS.val);

            _item.CSTCOFINS.val(item.CSTCOFINS.val);
            _item.PercentualReducaoBaseCalculoCOFINS.val(item.PercentualReducaoBaseCalculoCOFINS.val);
            _item.BaseCalculoCOFINS.val(item.BaseCalculoCOFINS.val);
            _item.AliquotaCOFINS.val(item.AliquotaCOFINS.val);
            _item.ValorCOFINS.val(item.ValorCOFINS.val);

            _item.CSTIPI.val(item.CSTIPI.val);
            _item.PercentualReducaoBaseCalculoIPI.val(item.PercentualReducaoBaseCalculoIPI.val);
            _item.BaseCalculoIPI.val(item.BaseCalculoIPI.val);
            _item.AliquotaIPI.val(item.AliquotaIPI.val);
            _item.ValorIPI.val(item.ValorIPI.val);

            _item.BaseCalculoICMSST.val(item.BaseCalculoICMSST.val);
            _item.AliquotaICMSST.val(item.AliquotaICMSST.val);
            _item.ValorICMSST.val(item.ValorICMSST.val);
            _item.BaseSTRetido.val(item.BaseSTRetido.val);
            _item.ValorSTRetido.val(item.ValorSTRetido.val);

            _item.BaseCalculoCreditoPresumido.val(item.BaseCalculoCreditoPresumido.val);
            _item.AliquotaCreditoPresumido.val(item.AliquotaCreditoPresumido.val);
            _item.ValorCreditoPresumido.val(item.ValorCreditoPresumido.val);

            _item.BaseCalculoDiferencial.val(item.BaseCalculoDiferencial.val);
            _item.AliquotaDiferencial.val(item.AliquotaDiferencial.val);
            _item.ValorDiferencial.val(item.ValorDiferencial.val);

            _item.ValorOutrasDespesas.val(item.ValorOutrasDespesas.val);
            _item.ValorFrete.val(item.ValorFrete.val);

            _item.ValorCustoUnitario.val(item.ValorCustoUnitario.val);
            _item.ValorCustoTotal.val(item.ValorCustoTotal.val);
            _item.CalculoCustoProduto.val(item.CalculoCustoProduto.val);

            _item.ValorFreteFora.val(item.ValorFreteFora.val);
            _item.ValorOutrasDespesasFora.val(item.ValorOutrasDespesasFora.val);
            _item.ValorDescontoFora.val(item.ValorDescontoFora.val);
            _item.ValorImpostosFora.val(item.ValorImpostosFora.val);
            _item.ValorDiferencialFreteFora.val(item.ValorDiferencialFreteFora.val);
            _item.ValorICMSFreteFora.val(item.ValorICMSFreteFora.val);

            _item.ValorSeguro.val(item.ValorSeguro.val);

            _item.ValorRetencaoCOFINS.val(item.ValorRetencaoCOFINS.val);
            _item.ValorRetencaoCSLL.val(item.ValorRetencaoCSLL.val);
            _item.ValorRetencaoINSS.val(item.ValorRetencaoINSS.val);
            _item.ValorRetencaoIPI.val(item.ValorRetencaoIPI.val);
            _item.ValorRetencaoOutras.val(item.ValorRetencaoOutras.val);
            _item.ValorRetencaoIR.val(item.ValorRetencaoIR.val);
            _item.ValorRetencaoISS.val(item.ValorRetencaoISS.val);
            _item.ValorRetencaoPIS.val(item.ValorRetencaoPIS.val);

            _item.NumeroFogoInicial.val(item.NumeroFogoInicial.val);
            _item.TipoAquisicao.val(item.TipoAquisicao.val);
            _item.VidaAtual.val(item.VidaAtual.val);
            _item.Almoxarifado.val(item.Almoxarifado.val);
            _item.Almoxarifado.codEntity(item.Almoxarifado.codEntity);

            _item.ProdutoVinculado.val(item.ProdutoVinculado.val);
            _item.ProdutoVinculado.codEntity(item.ProdutoVinculado.codEntity);
            _item.QuantidadeProdutoVinculado.val(item.QuantidadeProdutoVinculado.val);
            _item.LocalArmazenamento.val(item.LocalArmazenamento.val);
            _item.LocalArmazenamento.codEntity(item.LocalArmazenamento.codEntity);
            _item.CentroResultado.val(item.CentroResultado.val);
            _item.CentroResultado.codEntity(item.CentroResultado.codEntity);

            _item.UnidadeMedidaFornecedor.val(item.UnidadeMedidaFornecedor.val);
            _item.QuantidadeFornecedor.val(item.QuantidadeFornecedor.val);
            _item.ValorUnitarioFornecedor.val(item.ValorUnitarioFornecedor.val);
            _item.OrigemMercadoria.val(item.OrigemMercadoria.val);
            _item.EncerrarOrdemServico.val(item.EncerrarOrdemServico.val);
            _item.BaseCalculoICMSFornecedor.val(item.BaseCalculoICMSFornecedor.val);
            _item.AliquotaICMSFornecedor.val(item.AliquotaICMS.val);
            _item.ValorICMSFornecedor.val(item.ValorICMSFornecedor.val);
            _item.CfopFornecedor.val(item.CfopFornecedor.val);
            _item.Adicionar.visible(false);
            _item.Atualizar.visible(true);
            _item.Excluir.visible(true);
            _item.Cancelar.visible(true);

            _item.CstIcmsFornecedor.val(item.CstIcmsFornecedor.val);

            CalcularCustoProduto();
            RecarregarGridAbastecimento();
            RecarregarGridOrdemServico();

            break;
        }
    }
}

function ExcluirItemClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir o item?", function () {
        for (var i = 0; i < _documentoEntrada.Itens.list.length; i++) {
            if (_item.Codigo.val() == _documentoEntrada.Itens.list[i].Codigo.val) {

                _documentoEntrada.Itens.list.splice(i, 1);

                _documentoEntrada.Itens.list.sort(function (a, b) {
                    return a.Sequencial.val - b.Sequencial.val;
                });

                for (var j = 0; j < _documentoEntrada.Itens.list.length; j++)
                    _documentoEntrada.Itens.list[j].Sequencial.val = (j + 1);

                for (var k = 0; k < _documentoEntrada.ItensAbastecimentos.list.length; k++) {
                    if (_documentoEntrada.ItensAbastecimentos.list[k].CodigoItem == "" || _documentoEntrada.ItensAbastecimentos.list[k].CodigoItem == _item.Codigo.val()) {
                        _documentoEntrada.ItensAbastecimentos.list.splice(k, 1);
                        k--;
                    }
                }

                for (var k = 0; k < _documentoEntrada.ItensOrdensServico.list.length; k++) {
                    if (_documentoEntrada.ItensOrdensServico.list[k].CodigoItem == "" || _documentoEntrada.ItensOrdensServico.list[k].CodigoItem == _item.Codigo.val()) {
                        _documentoEntrada.ItensOrdensServico.list.splice(k, 1);
                        k--;
                    }
                }

                break;
            }
        }

        LimparCamposItem();
        RecarregarGridItem();
        AtualizarTotaisImpostos();
    });
}

function AtualizarItemClick(e, sender) {
    if (!ValidarRegrasItem())
        return;

    CalcularCustoProduto();
    SalvarFormulaCustoProduto();

    if (_item.OrdemServico.val() == "")
        _item.OrdemServico.codEntity(0);

    if (_item.OrdemCompraMercadoria.val() == "")
        _item.OrdemCompraMercadoria.codEntity(0);

    if (_item.OrdemCompra.val() == "")
        _item.OrdemCompra.codEntity(0);

    if (_item.RegraEntradaDocumento.val() == "")
        _item.RegraEntradaDocumento.codEntity(0);
    
    if (!(_item.Veiculo.codEntity() >= 0)) {
        _item.Veiculo.val("");
        _item.Veiculo.codEntity(0);
    }

    var siglaUN = $("#" + _item.UnidadeMedida.id + "  option:selected").text();
    if (siglaUN.includes("-"))
        siglaUN = siglaUN.substring(0, siglaUN.indexOf("-")).trim();
    _item.SiglaUnidadeMedida.val(siglaUN);

    for (var i = 0; i < _documentoEntrada.Itens.list.length; i++) {
        if (_item.Codigo.val() == _documentoEntrada.Itens.list[i].Codigo.val) {
            _documentoEntrada.Itens.list[i] = SalvarListEntity(_item);
            break;
        }
    }

    //Abastecimentos Itens - Remove antes de adicionar novamente
    for (var i = 0; i < _documentoEntrada.ItensAbastecimentos.list.length; i++) {
        if (_item.Codigo.val() == _documentoEntrada.ItensAbastecimentos.list[i].CodigoItem) {
            _documentoEntrada.ItensAbastecimentos.list.splice(i, 1);
            i--;
        }
    }

    var abastecimentosItem = _gridItemAbastecimentos.BuscarRegistros();
    for (var i = 0; i < abastecimentosItem.length; i++) {
        if (abastecimentosItem[i].CodigoItem == "" || abastecimentosItem[i].CodigoItem == _item.Codigo.val())
            abastecimentosItem[i].CodigoItem = _item.Codigo.val();
    }

    for (var i = 0; i < abastecimentosItem.length; i++) {
        _documentoEntrada.ItensAbastecimentos.list.push(abastecimentosItem[i]);
    }

    //Ordens de Serviço Itens - Remove antes de adicionar novamente
    for (var i = 0; i < _documentoEntrada.ItensOrdensServico.list.length; i++) {
        if (_item.Codigo.val() == _documentoEntrada.ItensOrdensServico.list[i].CodigoItem) {
            _documentoEntrada.ItensOrdensServico.list.splice(i, 1);
            i--;
        }
    }
    var ordensServicoItem = _gridItemOrdensServico.BuscarRegistros();
    for (var i = 0; i < ordensServicoItem.length; i++) {
        if (ordensServicoItem[i].CodigoItem == "" || ordensServicoItem[i].CodigoItem == _item.Codigo.val())
            ordensServicoItem[i].CodigoItem = _item.Codigo.val();
    }
    for (var i = 0; i < ordensServicoItem.length; i++) {
        _documentoEntrada.ItensOrdensServico.list.push(ordensServicoItem[i]);
    }

    LimparCamposItem();
    RecarregarGridItem(true);
    AtualizarTotaisImpostos();
}

function CancelarItemClick(e, sender) {
    LimparCamposItem();
}

function AdicionarItemClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_item);
    if (valido) {
        if (!ValidarRegrasItem())
            return;

        CalcularCustoProduto();
        SalvarFormulaCustoProduto();
        _item.Sequencial.val(_documentoEntrada.Itens.list.length + 1);

        var codigoItem = guid();
        var abastecimentosItem = _gridItemAbastecimentos.BuscarRegistros();
        for (var i = 0; i < abastecimentosItem.length; i++) {
            if (abastecimentosItem[i].CodigoItem == "" || abastecimentosItem[i].CodigoItem == _item.Codigo.val())
                abastecimentosItem[i].CodigoItem = codigoItem;
        }

        var ordensServicoItem = _gridItemOrdensServico.BuscarRegistros();
        for (var i = 0; i < ordensServicoItem.length; i++) {
            if (ordensServicoItem[i].CodigoItem == "" || ordensServicoItem[i].CodigoItem == _item.Codigo.val())
                ordensServicoItem[i].CodigoItem = codigoItem;
        }

        _item.Codigo.val(codigoItem);

        if (_item.OrdemServico.val() == "")
            _item.OrdemServico.codEntity(0);

        if (_item.OrdemCompraMercadoria.val() == "")
            _item.OrdemCompraMercadoria.codEntity(0);

        if (_item.OrdemCompra.val() == "")
            _item.OrdemCompra.codEntity(0);

        if (_item.RegraEntradaDocumento.val() == "")
            _item.RegraEntradaDocumento.codEntity(0);

        if (!(_item.Veiculo.codEntity() >= 0)) {
            _item.Veiculo.val("");
            _item.Veiculo.codEntity(0);
        }

        var siglaUN = $("#" + _item.UnidadeMedida.id + "  option:selected").text();
        if (siglaUN.includes("-"))
            siglaUN = siglaUN.substring(0, siglaUN.indexOf("-")).trim();
        _item.SiglaUnidadeMedida.val(siglaUN);

        _documentoEntrada.Itens.list.push(SalvarListEntity(_item));
        for (var i = 0; i < abastecimentosItem.length; i++) {
            _documentoEntrada.ItensAbastecimentos.list.push(abastecimentosItem[i]);
        }
        for (var i = 0; i < ordensServicoItem.length; i++) {
            _documentoEntrada.ItensOrdensServico.list.push(ordensServicoItem[i]);
        }

        RecarregarGridItem();
        LimparCamposItem();
        AtualizarTotaisImpostos();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function ValidarRegrasItem() {
    var valido = true;

    //Regras aba Pneu
    if (!string.IsNullOrWhiteSpace(_item.NumeroFogoInicial.val()) || _item.Almoxarifado.codEntity() > 0 || _item.TipoAquisicao.val() != "" || _item.VidaAtual.val() != "") {
        _item.NumeroFogoInicial.required(true);
        _item.Almoxarifado.required(true);
        _item.TipoAquisicao.required(true);
        _item.VidaAtual.required(true);

        valido = ValidarCamposObrigatorios(_item);
        if (!valido)
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios Pneu", "Favor informar todos os campos da Aba pneu!");
    }
    _item.NumeroFogoInicial.required(false);
    _item.Almoxarifado.required(false);
    _item.TipoAquisicao.required(false);
    _item.VidaAtual.required(false);

    return valido;
}

function LimparCamposItem() {
    var totalDescontos = Globalize.parseFloat(_item.TotalDescontosItens.val());
    var totalItem = Globalize.parseFloat(_item.TotalValorItens.val());
    var subtotal = Globalize.parseFloat(_item.SubTotalItens.val());

    _item.Adicionar.visible(true);
    _item.Atualizar.visible(false);
    _item.Excluir.visible(false);
    _item.Cancelar.visible(false);
    _item.CodigoProdutoFornecedor.visible(false);
    _item.EncerrarOrdemServico.visible(false);

    LimparCampos(_item);
    RetornarSemCalculo();

    _item.TotalDescontosItens.val(Globalize.format(totalDescontos, "n2"));
    _item.TotalValorItens.val(Globalize.format(totalItem, "n2"));
    _item.SubTotalItens.val(Globalize.format(subtotal, "n2"));

    if (_documentoEntrada.Veiculo.codEntity() > 0) {
        _item.Veiculo.codEntity(_documentoEntrada.Veiculo.codEntity());
        _item.Veiculo.val(_documentoEntrada.Veiculo.val());
    }
    if (_documentoEntrada.Equipamento.codEntity() > 0) {
        _item.Equipamento.codEntity(_documentoEntrada.Equipamento.codEntity());
        _item.Equipamento.val(_documentoEntrada.Equipamento.val());
    }
    if (_documentoEntrada.DataAbastecimento.val() != "")
        _item.DataAbastecimento.val(_documentoEntrada.DataAbastecimento.val());

    if (Boolean(_documentoEntrada.Horimetro.val()) && Globalize.parseInt(_documentoEntrada.Horimetro.val()) > 0)
        _item.Horimetro.val(_documentoEntrada.Horimetro.val());

    if (Boolean(_documentoEntrada.KMAbastecimento.val()) && Globalize.parseInt(_documentoEntrada.KMAbastecimento.val()) > 0)
        _item.KMAbastecimento.val(_documentoEntrada.KMAbastecimento.val());

    if (_documentoEntrada.CFOP.codEntity() > 0) {
        _item.CFOP.codEntity(_documentoEntrada.CFOP.codEntity());
        _item.CFOP.val(_documentoEntrada.CFOP.val());
    }

    if (_documentoEntrada.TipoMovimento.codEntity() > 0) {
        _item.TipoMovimento.codEntity(_documentoEntrada.TipoMovimento.codEntity());
        _item.TipoMovimento.val(_documentoEntrada.TipoMovimento.val());
    }

    if (_documentoEntrada.NaturezaOperacao.codEntity() > 0) {
        _item.NaturezaOperacao.codEntity(_documentoEntrada.NaturezaOperacao.codEntity());
        _item.NaturezaOperacao.val(_documentoEntrada.NaturezaOperacao.val());
    }

    if (_documentoEntrada.OrdemServico.codEntity() > 0) {
        _item.OrdemServico.codEntity(_documentoEntrada.OrdemServico.codEntity());
        _item.OrdemServico.val(_documentoEntrada.OrdemServico.val());
    }

    RecarregarGridAbastecimento();
    RecarregarGridOrdemServico();
}

function ControleVisibilidadeOrdemServicoItem() {
    if (_item.OrdemServico.codEntity() > 0)
        _item.EncerrarOrdemServico.visible(true);
    else {
        _item.EncerrarOrdemServico.visible(false);
        _item.EncerrarOrdemServico.val(false);
    }
}

function RetornoConsultaOrdemServicoItem(dados) {
    _item.OrdemServico.val(dados.Numero + " (" + dados.VeiculoEquipamento + ")");
    _item.OrdemServico.codEntity(dados.Codigo);

    if (dados.CodigoVeiculo > 0) {
        _item.Veiculo.codEntity(dados.CodigoVeiculo);
        _item.Veiculo.val(dados.Veiculo);
    }
}

function RetornoConsultaOrdemCompraMercadoria(dados) {
    _item.OrdemCompraMercadoria.val(dados.Numero + " (" + dados.Produto + ")");
    _item.OrdemCompraMercadoria.codEntity(dados.Codigo);
}

function RetornoConsultaOrdemCompra(dados) {
    _item.OrdemCompra.val(dados.Numero);
    _item.OrdemCompra.codEntity(dados.Codigo);
    _documentoEntrada.OrdemCompra.val(dados.Numero);
    _documentoEntrada.OrdemCompra.codEntity(dados.Codigo);
}

function TrocaComponenteModalBuscaCFOPItemDocumentoEntrada(quantidadeCFOPs) {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFe)
        return;

    _buscaCFOPItem.Destroy();
    _buscaCFOPNFItem.Destroy();
    _buscaCFOPNFFornecedor.Destroy();

    if (quantidadeCFOPs > 0) {
        _buscaCFOPItem = new BuscarCFOPNotaFiscal(_item.CFOP, function (r) { RetornoConsultaCFOPItem(r.Codigo); }, EnumTipoCFOP.Entrada, null, _documentoEntrada.Fornecedor, null, _documentoEntrada.Destinatario, null, null, _item.NaturezaOperacao);
        _buscaCFOPNFItem = new BuscarCFOPNotaFiscal(_item.CFOPNF, function (r) { RetornoConsultaCFOP(r.Codigo); }, EnumTipoCFOP.Entrada, null, _documentoEntrada.Fornecedor, null, _documentoEntrada.Destinatario, null, null, _documentoEntrada.NaturezaOperacao);
       }
    else {
        _buscaCFOPItem = new BuscarCFOPNotaFiscal(_item.CFOP, function (r) { RetornoConsultaCFOPItem(r.Codigo); }, EnumTipoCFOP.Entrada, null, _documentoEntrada.Fornecedor, null, _documentoEntrada.Destinatario, null, null, null);
        _buscaCFOPNFItem = new BuscarCFOPNotaFiscal(_item.CFOPNF, function (r) { RetornoConsultaCFOP(r.Codigo); }, EnumTipoCFOP.Entrada, null, _documentoEntrada.Fornecedor, null, _documentoEntrada.Destinatario, null, null, null);
     }
}

function RetornarSemCalculo() {
    $("#btnValores_Desconto")[0].title = "Sem cálculo ";
    $("#btnValores_Desconto")[0].innerHTML = '<i class="fa fa-stop"></i>&nbsp;&nbsp;Sem cálculo';

    $("#btnValores_OutrasDespesas")[0].title = "Sem cálculo ";
    $("#btnValores_OutrasDespesas")[0].innerHTML = '<i class="fa fa-stop"></i>&nbsp;&nbsp;Sem cálculo';

    $("#btnValores_Frete")[0].title = "Sem cálculo ";
    $("#btnValores_Frete")[0].innerHTML = '<i class="fa fa-stop"></i>&nbsp;&nbsp;Sem cálculo';

    $("#btnValores_Seguro")[0].title = "Sem cálculo ";
    $("#btnValores_Seguro")[0].innerHTML = '<i class="fa fa-stop"></i>&nbsp;&nbsp;Sem cálculo';

    $("#btnValores_ICMS")[0].title = "Sem cálculo ";
    $("#btnValores_ICMS")[0].innerHTML = '<i class="fa fa-stop"></i>&nbsp;&nbsp;Sem cálculo';

    $("#btnValores_IPI")[0].title = "Sem cálculo ";
    $("#btnValores_IPI")[0].innerHTML = '<i class="fa fa-stop"></i>&nbsp;&nbsp;Sem cálculo';

    $("#btnValores_ICMSST")[0].title = "Sem cálculo ";
    $("#btnValores_ICMSST")[0].innerHTML = '<i class="fa fa-stop"></i>&nbsp;&nbsp;Sem cálculo';

    $("#btnValores_CreditoPresumido")[0].title = "Sem cálculo ";
    $("#btnValores_CreditoPresumido")[0].innerHTML = '<i class="fa fa-stop"></i>&nbsp;&nbsp;Sem cálculo';

    $("#btnValores_Diferencial")[0].title = "Sem cálculo ";
    $("#btnValores_Diferencial")[0].innerHTML = '<i class="fa fa-stop"></i>&nbsp;&nbsp;Sem cálculo';

    $("#btnValores_FreteFora")[0].title = "Sem cálculo ";
    $("#btnValores_FreteFora")[0].innerHTML = '<i class="fa fa-stop"></i>&nbsp;&nbsp;Sem cálculo';

    $("#btnValores_OutrasDespesasFora")[0].title = "Sem cálculo ";
    $("#btnValores_OutrasDespesasFora")[0].innerHTML = '<i class="fa fa-stop"></i>&nbsp;&nbsp;Sem cálculo';

    $("#btnValores_DescontoFora")[0].title = "Sem cálculo ";
    $("#btnValores_DescontoFora")[0].innerHTML = '<i class="fa fa-stop"></i>&nbsp;&nbsp;Sem cálculo';

    $("#btnValores_ImpostosFora")[0].title = "Sem cálculo ";
    $("#btnValores_ImpostosFora")[0].innerHTML = '<i class="fa fa-stop"></i>&nbsp;&nbsp;Sem cálculo';

    $("#btnValores_DiferencialFreteFora")[0].title = "Sem cálculo ";
    $("#btnValores_DiferencialFreteFora")[0].innerHTML = '<i class="fa fa-stop"></i>&nbsp;&nbsp;Sem cálculo';

    $("#btnValores_ValorPIS")[0].title = "Sem cálculo ";
    $("#btnValores_ValorPIS")[0].innerHTML = '<i class="fa fa-stop"></i>&nbsp;&nbsp;Sem cálculo';

    $("#btnValores_ValorCOFINS")[0].title = "Sem cálculo ";
    $("#btnValores_ValorCOFINS")[0].innerHTML = '<i class="fa fa-stop"></i>&nbsp;&nbsp;Sem cálculo';

    $("#btnValores_ICMSFreteFora")[0].title = "Sem cálculo ";
    $("#btnValores_ICMSFreteFora")[0].innerHTML = '<i class="fa fa-stop"></i>&nbsp;&nbsp;Sem cálculo';

    CalcularCustoProduto();
}

function CalcularCustoProduto() {
    var quantidade = Globalize.parseFloat(_item.Quantidade.val());
    if (isNaN(quantidade))
        quantidade = 0;

    var valorUnitario = Globalize.parseFloat(_item.ValorUnitario.val());
    if (isNaN(valorUnitario))
        valorUnitario = 0;

    var valorTotal = Globalize.format((quantidade * valorUnitario), "n4");
    valorTotal = Globalize.parseFloat(valorTotal);
    if (isNaN(valorTotal))
        valorTotal = 0;

    if ((valorTotal) > 0) {
        var valorICMS = Globalize.parseFloat(_item.ValorICMS.val());
        if (isNaN(valorICMS))
            valorICMS = 0;

        var valorCreditoPresumido = Globalize.parseFloat(_item.ValorCreditoPresumido.val());
        if (isNaN(valorCreditoPresumido))
            valorCreditoPresumido = 0;

        var valorDiferencial = Globalize.parseFloat(_item.ValorDiferencial.val());
        if (isNaN(valorDiferencial))
            valorDiferencial = 0;

        var valorICMSST = Globalize.parseFloat(_item.ValorICMSST.val());
        if (isNaN(valorICMSST))
            valorICMSST = 0;

        var valorIPI = Globalize.parseFloat(_item.ValorIPI.val());
        if (isNaN(valorIPI))
            valorIPI = 0;

        var valorFrete = Globalize.parseFloat(_item.ValorFrete.val());
        if (isNaN(valorFrete))
            valorFrete = 0;

        var valorOutras = Globalize.parseFloat(_item.ValorOutrasDespesas.val());
        if (isNaN(valorOutras))
            valorOutras = 0;

        var valorSeguro = Globalize.parseFloat(_item.ValorSeguro.val());
        if (isNaN(valorSeguro))
            valorSeguro = 0;

        var valorDesconto = Globalize.parseFloat(_item.Desconto.val());
        if (isNaN(valorDesconto))
            valorDesconto = 0;

        var valorDescontoFora = Globalize.parseFloat(_item.ValorDescontoFora.val());
        if (isNaN(valorDescontoFora))
            valorDescontoFora = 0;

        var valorImpostoFora = Globalize.parseFloat(_item.ValorImpostosFora.val());
        if (isNaN(valorImpostoFora))
            valorImpostoFora = 0;

        var valorOutrasFora = Globalize.parseFloat(_item.ValorOutrasDespesasFora.val());
        if (isNaN(valorOutrasFora))
            valorOutrasFora = 0;

        var valorFreteFora = Globalize.parseFloat(_item.ValorFreteFora.val());
        if (isNaN(valorFreteFora))
            valorFreteFora = 0;

        var valorICMSFreteFora = Globalize.parseFloat(_item.ValorICMSFreteFora.val());
        if (isNaN(valorICMSFreteFora))
            valorICMSFreteFora = 0;

        var valorDiferencialFreteFora = Globalize.parseFloat(_item.ValorDiferencialFreteFora.val());
        if (isNaN(valorDiferencialFreteFora))
            valorDiferencialFreteFora = 0;

        var valorPIS = Globalize.parseFloat(_item.ValorPIS.val());
        if (isNaN(valorPIS))
            valorPIS = 0;

        var valorCOFINS = Globalize.parseFloat(_item.ValorCOFINS.val());
        if (isNaN(valorCOFINS))
            valorCOFINS = 0;

        var custoUnitario = 0.000;
        var custoTotal = 0.000;

        custoUnitario = (valorTotal);

        if (valorDesconto > 0) {
            if ($("#btnValores_Desconto")[0].title.trim().toUpperCase() == "SOMAR")
                custoUnitario = custoUnitario + valorDesconto;
            else if ($("#btnValores_Desconto")[0].title.trim().toUpperCase() == "SUBTRAIR")
                custoUnitario = custoUnitario - valorDesconto;
        }

        if (valorOutras > 0) {
            if ($("#btnValores_OutrasDespesas")[0].title.trim().toUpperCase() == "SOMAR")
                custoUnitario = custoUnitario + valorOutras;
            else if ($("#btnValores_OutrasDespesas")[0].title.trim().toUpperCase() == "SUBTRAIR")
                custoUnitario = custoUnitario - valorOutras;
        }

        if (valorFrete > 0) {
            if ($("#btnValores_Frete")[0].title.trim().toUpperCase() == "SOMAR")
                custoUnitario = custoUnitario + valorFrete;
            else if ($("#btnValores_Frete")[0].title.trim().toUpperCase() == "SUBTRAIR")
                custoUnitario = custoUnitario - valorFrete;
        }

        if (valorSeguro > 0) {
            if ($("#btnValores_Seguro")[0].title.trim().toUpperCase() == "SOMAR")
                custoUnitario = custoUnitario + valorSeguro;
            else if ($("#btnValores_Seguro")[0].title.trim().toUpperCase() == "SUBTRAIR")
                custoUnitario = custoUnitario - valorSeguro;
        }

        if (valorICMS > 0) {
            if ($("#btnValores_ICMS")[0].title.trim().toUpperCase() == "SOMAR")
                custoUnitario = custoUnitario + valorICMS;
            else if ($("#btnValores_ICMS")[0].title.trim().toUpperCase() == "SUBTRAIR")
                custoUnitario = custoUnitario - valorICMS;
        }

        if (valorIPI > 0) {
            if ($("#btnValores_IPI")[0].title.trim().toUpperCase() == "SOMAR")
                custoUnitario = custoUnitario + valorIPI;
            else if ($("#btnValores_IPI")[0].title.trim().toUpperCase() == "SUBTRAIR")
                custoUnitario = custoUnitario - valorIPI;
        }

        if (valorICMSST > 0) {
            if ($("#btnValores_ICMSST")[0].title.trim().toUpperCase() == "SOMAR")
                custoUnitario = custoUnitario + valorICMSST;
            else if ($("#btnValores_ICMSST")[0].title.trim().toUpperCase() == "SUBTRAIR")
                custoUnitario = custoUnitario - valorICMSST;
        }

        if (valorCreditoPresumido > 0) {
            if ($("#btnValores_CreditoPresumido")[0].title.trim().toUpperCase() == "SOMAR")
                custoUnitario = custoUnitario + valorCreditoPresumido;
            else if ($("#btnValores_CreditoPresumido")[0].title.trim().toUpperCase() == "SUBTRAIR")
                custoUnitario = custoUnitario - valorCreditoPresumido;
        }

        if (valorDiferencial > 0) {
            if ($("#btnValores_Diferencial")[0].title.trim().toUpperCase() == "SOMAR")
                custoUnitario = custoUnitario + valorDiferencial;
            else if ($("#btnValores_Diferencial")[0].title.trim().toUpperCase() == "SUBTRAIR")
                custoUnitario = custoUnitario - valorDiferencial;
        }

        if (valorFreteFora > 0) {
            if ($("#btnValores_FreteFora")[0].title.trim().toUpperCase() == "SOMAR")
                custoUnitario = custoUnitario + valorFreteFora;
            else if ($("#btnValores_FreteFora")[0].title.trim().toUpperCase() == "SUBTRAIR")
                custoUnitario = custoUnitario - valorFreteFora;
        }

        if (valorOutrasFora > 0) {
            if ($("#btnValores_OutrasDespesasFora")[0].title.trim().toUpperCase() == "SOMAR")
                custoUnitario = custoUnitario + valorOutrasFora;
            else if ($("#btnValores_OutrasDespesasFora")[0].title.trim().toUpperCase() == "SUBTRAIR")
                custoUnitario = custoUnitario - valorOutrasFora;
        }

        if (valorImpostoFora > 0) {
            if ($("#btnValores_ImpostosFora")[0].title.trim().toUpperCase() == "SOMAR")
                custoUnitario = custoUnitario + valorImpostoFora;
            else if ($("#btnValores_ImpostosFora")[0].title.trim().toUpperCase() == "SUBTRAIR")
                custoUnitario = custoUnitario - valorImpostoFora;
        }

        if (valorDiferencialFreteFora > 0) {
            if ($("#btnValores_DiferencialFreteFora")[0].title.trim().toUpperCase() == "SOMAR")
                custoUnitario = custoUnitario + valorDiferencialFreteFora;
            else if ($("#btnValores_DiferencialFreteFora")[0].title.trim().toUpperCase() == "SUBTRAIR")
                custoUnitario = custoUnitario - valorDiferencialFreteFora;
        }

        if (valorCOFINS > 0) {
            if ($("#btnValores_ValorCOFINS")[0].title.trim().toUpperCase() == "SOMAR")
                custoUnitario = custoUnitario + valorCOFINS;
            else if ($("#btnValores_ValorCOFINS")[0].title.trim().toUpperCase() == "SUBTRAIR")
                custoUnitario = custoUnitario - valorCOFINS;
        }

        if (valorPIS > 0) {
            if ($("#btnValores_ValorPIS")[0].title.trim().toUpperCase() == "SOMAR")
                custoUnitario = custoUnitario + valorPIS;
            else if ($("#btnValores_ValorPIS")[0].title.trim().toUpperCase() == "SUBTRAIR")
                custoUnitario = custoUnitario - valorPIS;
        }

        if (valorICMSFreteFora > 0) {
            if ($("#btnValores_ICMSFreteFora")[0].title.trim().toUpperCase() == "SOMAR")
                custoUnitario = custoUnitario + valorICMSFreteFora;
            else if ($("#btnValores_ICMSFreteFora")[0].title.trim().toUpperCase() == "SUBTRAIR")
                custoUnitario = custoUnitario - valorICMSFreteFora;
        }

        if (valorDescontoFora > 0) {
            if ($("#btnValores_DescontoFora")[0].title.trim().toUpperCase() == "SOMAR")
                custoUnitario = custoUnitario + valorDescontoFora;
            else if ($("#btnValores_DescontoFora")[0].title.trim().toUpperCase() == "SUBTRAIR")
                custoUnitario = custoUnitario - valorDescontoFora;
        }

        custoTotal = custoUnitario;
        custoUnitario = custoUnitario / quantidade;

        if (custoUnitario > 0 && custoTotal > 0) {
            _item.ValorCustoUnitario.val(Globalize.format(custoUnitario, "n4"));
            _item.ValorCustoTotal.val(Globalize.format(custoTotal, "n4"));
        } else {
            _item.ValorCustoUnitario.val("0,0000");
            _item.ValorCustoTotal.val("0,0000");
        }
    } else {
        _item.ValorCustoUnitario.val("0,0000");
        _item.ValorCustoTotal.val("0,0000");
    }
}

function SalvarFormulaCustoProduto() {
    var formulaCusto = "";

    if ($("#btnValores_Desconto")[0].title.trim().toUpperCase() == "SOMAR")
        formulaCusto += "#+ #ValorDesconto ";
    else if ($("#btnValores_Desconto")[0].title.trim().toUpperCase() == "SUBTRAIR")
        formulaCusto += "#- #ValorDesconto ";

    if ($("#btnValores_OutrasDespesas")[0].title.trim().toUpperCase() == "SOMAR")
        formulaCusto += "#+ #ValorOutras ";
    else if ($("#btnValores_OutrasDespesas")[0].title.trim().toUpperCase() == "SUBTRAIR")
        formulaCusto += "#- #ValorOutras ";

    if ($("#btnValores_Frete")[0].title.trim().toUpperCase() == "SOMAR")
        formulaCusto += "#+ #ValorFrete ";
    else if ($("#btnValores_Frete")[0].title.trim().toUpperCase() == "SUBTRAIR")
        formulaCusto += "#- #ValorFrete ";

    if ($("#btnValores_Seguro")[0].title.trim().toUpperCase() == "SOMAR")
        formulaCusto += "#+ #ValorSeguro ";
    else if ($("#btnValores_Seguro")[0].title.trim().toUpperCase() == "SUBTRAIR")
        formulaCusto += "#- #ValorSeguro ";

    if ($("#btnValores_ICMS")[0].title.trim().toUpperCase() == "SOMAR")
        formulaCusto += "#+ #ValorICMS ";
    else if ($("#btnValores_ICMS")[0].title.trim().toUpperCase() == "SUBTRAIR")
        formulaCusto += "#- #ValorICMS ";

    if ($("#btnValores_IPI")[0].title.trim().toUpperCase() == "SOMAR")
        formulaCusto += "#+ #ValorIPI ";
    else if ($("#btnValores_IPI")[0].title.trim().toUpperCase() == "SUBTRAIR")
        formulaCusto += "#- #ValorIPI ";

    if ($("#btnValores_ICMSST")[0].title.trim().toUpperCase() == "SOMAR")
        formulaCusto += "#+ #ValorICMSST ";
    else if ($("#btnValores_ICMSST")[0].title.trim().toUpperCase() == "SUBTRAIR")
        formulaCusto += "#- #ValorICMSST ";

    if ($("#btnValores_CreditoPresumido")[0].title.trim().toUpperCase() == "SOMAR")
        formulaCusto += "#+ #ValorCreditoPresumido ";
    else if ($("#btnValores_CreditoPresumido")[0].title.trim().toUpperCase() == "SUBTRAIR")
        formulaCusto += "#- #ValorCreditoPresumido ";

    if ($("#btnValores_Diferencial")[0].title.trim().toUpperCase() == "SOMAR")
        formulaCusto += "#+ #ValorDiferencial ";
    else if ($("#btnValores_Diferencial")[0].title.trim().toUpperCase() == "SUBTRAIR")
        formulaCusto += "#- #ValorDiferencial ";

    if ($("#btnValores_FreteFora")[0].title.trim().toUpperCase() == "SOMAR")
        formulaCusto += "#+ #ValorFreteFora ";
    else if ($("#btnValores_FreteFora")[0].title.trim().toUpperCase() == "SUBTRAIR")
        formulaCusto += "#- #ValorFreteFora ";

    if ($("#btnValores_OutrasDespesasFora")[0].title.trim().toUpperCase() == "SOMAR")
        formulaCusto += "#+ #ValorOutrasFora ";
    else if ($("#btnValores_OutrasDespesasFora")[0].title.trim().toUpperCase() == "SUBTRAIR")
        formulaCusto += "#- #ValorOutrasFora ";

    if ($("#btnValores_ImpostosFora")[0].title.trim().toUpperCase() == "SOMAR")
        formulaCusto += "#+ #ValorImpostoFora ";
    else if ($("#btnValores_ImpostosFora")[0].title.trim().toUpperCase() == "SUBTRAIR")
        formulaCusto += "#- #ValorImpostoFora ";

    if ($("#btnValores_DiferencialFreteFora")[0].title.trim().toUpperCase() == "SOMAR")
        formulaCusto += "#+ #ValorDiferencialFreteFora ";
    else if ($("#btnValores_DiferencialFreteFora")[0].title.trim().toUpperCase() == "SUBTRAIR")
        formulaCusto += "#- #ValorDiferencialFreteFora ";

    if ($("#btnValores_ValorPIS")[0].title.trim().toUpperCase() == "SOMAR")
        formulaCusto += "#+ #ValorPIS ";
    else if ($("#btnValores_ValorPIS")[0].title.trim().toUpperCase() == "SUBTRAIR")
        formulaCusto += "#- #ValorPIS ";

    if ($("#btnValores_ValorCOFINS")[0].title.trim().toUpperCase() == "SOMAR")
        formulaCusto += "#+ #ValorCOFINS ";
    else if ($("#btnValores_ValorCOFINS")[0].title.trim().toUpperCase() == "SUBTRAIR")
        formulaCusto += "#- #ValorCOFINS ";

    if ($("#btnValores_ICMSFreteFora")[0].title.trim().toUpperCase() == "SOMAR")
        formulaCusto += "#+ #ValorICMSFreteFora ";
    else if ($("#btnValores_ICMSFreteFora")[0].title.trim().toUpperCase() == "SUBTRAIR")
        formulaCusto += "#- #ValorICMSFreteFora ";

    if ($("#btnValores_DescontoFora")[0].title.trim().toUpperCase() == "SOMAR")
        formulaCusto += "#+ #ValorDescontoFora ";
    else if ($("#btnValores_DescontoFora")[0].title.trim().toUpperCase() == "SUBTRAIR")
        formulaCusto += "#- #ValorDescontoFora ";

    _item.CalculoCustoProduto.val(formulaCusto);
}