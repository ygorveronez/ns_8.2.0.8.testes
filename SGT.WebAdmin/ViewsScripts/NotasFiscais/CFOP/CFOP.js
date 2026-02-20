/// <reference path="../../Enumeradores/EnumCSTPISCOFINS.js" />
/// <reference path="../../Enumeradores/EnumCSTIPI.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumTipoCFOP.js" />
/// <reference path="../../Enumeradores/EnumCSTICMS.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridCFOP;
var _cfop;
var _pesquisaCFOP;

var _statusCFOP = [
    { text: "Ativo", value: "A" },
    { text: "Inativo", value: "I" }
];

var _statusPesquisaCFOP = [
    { text: "Todos", value: "" },
    { text: "Ativo", value: "A" },
    { text: "Inativo", value: "I" }
];

var _diaMes = [
    { text: "1", value: 1 },
    { text: "2", value: 2 },
    { text: "3", value: 3 },
    { text: "4", value: 4 },
    { text: "5", value: 5 },
    { text: "6", value: 6 },
    { text: "7", value: 7 },
    { text: "8", value: 8 },
    { text: "9", value: 9 },
    { text: "10", value: 10 },
    { text: "11", value: 11 },
    { text: "12", value: 12 },
    { text: "13", value: 13 },
    { text: "14", value: 14 },
    { text: "15", value: 15 },
    { text: "16", value: 16 },
    { text: "17", value: 17 },
    { text: "18", value: 18 },
    { text: "19", value: 19 },
    { text: "20", value: 20 },
    { text: "21", value: 21 },
    { text: "22", value: 22 },
    { text: "23", value: 23 },
    { text: "24", value: 24 },
    { text: "25", value: 25 },
    { text: "26", value: 26 },
    { text: "27", value: 27 },
    { text: "28", value: 28 },
    { text: "29", value: 29 },
    { text: "30", value: 30 },
    { text: "31", value: 31 }
];

var _tipoCFOP = [
    { text: "Entrada", value: EnumTipoCFOP.Entrada },
    { text: "Saída", value: EnumTipoCFOP.Saida }
];

var _cstICMS = [
    { text: "Selecione", value: 0 },
    { text: "00 - Tributada integralmente", value: EnumCSTICMS.CST00 },
    { text: "10 - Tributada e com cobrança do ICMS por substituição tributária", value: EnumCSTICMS.CST10 },
    { text: "20 - Com redução de base de cálculo", value: EnumCSTICMS.CST20 },
    { text: "30 - Isenta ou não tributada e com cobrança do ICMS por substituição", value: EnumCSTICMS.CST30 },
    { text: "40 - Isenta", value: EnumCSTICMS.CST40 },
    { text: "41 - Não Tributada", value: EnumCSTICMS.CST41 },
    { text: "50 - Suspensão", value: EnumCSTICMS.CST50 },
    { text: "51 - Diferimento", value: EnumCSTICMS.CST51 },
    { text: "60 - ICMS cobrado anteriormente por substituição tributária", value: EnumCSTICMS.CST60 },
    { text: "61 - Tributação monofásica sobre combustíveis cobrada anteriormente.", value: EnumCSTICMS.CST61 },
    { text: "70 - Com redução de base de cálculo e cobrança do ICMS por substituição tributária", value: EnumCSTICMS.CST70 },
    { text: "90 - Outras", value: EnumCSTICMS.CST90 },
    { text: "101 - Tributada pelo Simples Nacional com permissão de crédito", value: EnumCSTICMS.CSOSN101 },
    { text: "102 - Tributada pelo Simples Nacional sem permissão de crédito", value: EnumCSTICMS.CSOSN102 },
    { text: "103 - Isenção do ICMS no Simples Nacional para faixa de receita bruta", value: EnumCSTICMS.CSOSN103 },
    { text: "201 - Tributada pelo Simples Nacional com permissão de crédito e com cobrança do ICMS por substituição tributária", value: EnumCSTICMS.CSOSN201 },
    { text: "202 - Tributada pelo Simples Nacional sem permissão de crédito e com cobrança do ICMS por substituição tributária", value: EnumCSTICMS.CSOSN202 },
    { text: "203 - Isenção do ICMS no Simples Nacional para faixa de receita bruta e com cobrança do ICMS por substituicao tributaria", value: EnumCSTICMS.CSOSN203 },
    { text: "300 - Imune", value: EnumCSTICMS.CSOSN300 },
    { text: "400 - Nao tributada pelo Simples Nacional", value: EnumCSTICMS.CSOSN400 },
    { text: "500 - ICMS cobrado anteriormente por substituicao tributaria (substituido) ou por antecipacao", value: EnumCSTICMS.CSOSN500 },
    { text: "900 - Outros", value: EnumCSTICMS.CSOSN900 }
];

var _cstIPI = [
    { text: "Selecione", value: 0 },
    { text: "00 - Entrada com recuperação de crédito", value: EnumCSTIPI.CST00 },
    { text: "01 - Entrada tributada com alíquota zero", value: EnumCSTIPI.CST01 },
    { text: "02 - Entrada isenta", value: EnumCSTIPI.CST02 },
    { text: "03 - Entrada não-tributada", value: EnumCSTIPI.CST03 },
    { text: "04 - Entrada imune", value: EnumCSTIPI.CST04 },
    { text: "05 - Entrada com suspenção", value: EnumCSTIPI.CST05 },
    { text: "49 - Outras entradas", value: EnumCSTIPI.CST49 },
    { text: "50 - Saída tributada", value: EnumCSTIPI.CST50 },
    { text: "51 - Saída tributada com alíquota zero", value: EnumCSTIPI.CST51 },
    { text: "52 - Saída isent", value: EnumCSTIPI.CST52 },
    { text: "53 - Saída não-tributada", value: EnumCSTIPI.CST53 },
    { text: "54 - Saída imune", value: EnumCSTIPI.CST54 },
    { text: "55 - Saída com suspensão", value: EnumCSTIPI.CST55 },
    { text: "99 - Outras saídas", value: EnumCSTIPI.CST99 }
];

var _cstPISCOFINS = [
    { text: "Selecione", value: 0 },
    { text: "01 - Operação Tributável com Alíquota Básica", value: EnumCSTPISCOFINS.CST01 },
    { text: "02 - Operação Tributável com Alíquota Diferenciada", value: EnumCSTPISCOFINS.CST02 },
    { text: "03 - Operação Tributável com Alíquota por Unidade de Medida de Produto", value: EnumCSTPISCOFINS.CST03 },
    { text: "04 - Operação Tributável Monofásica - Revenda a Alíquota Zero", value: EnumCSTPISCOFINS.CST04 },
    { text: "05 - Operação Tributável por Substituição Tributária", value: EnumCSTPISCOFINS.CST05 },
    { text: "06 - Operação Tributável a Alíquota Zero", value: EnumCSTPISCOFINS.CST06 },
    { text: "07 - Operação Isenta da Contribuição", value: EnumCSTPISCOFINS.CST07 },
    { text: "08 - Operação sem Incidência da Contribuição", value: EnumCSTPISCOFINS.CST08 },
    { text: "09 - Operação com Suspensão da Contribuição", value: EnumCSTPISCOFINS.CST09 },
    { text: "49 - Outras Operações de Saída", value: EnumCSTPISCOFINS.CST49 },
    { text: "50 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita Tributada no Mercado Interno", value: EnumCSTPISCOFINS.CST50 },
    { text: "51 - Operação com Direito a Crédito – Vinculada Exclusivamente a Receita Não Tributada no Mercado Interno", value: EnumCSTPISCOFINS.CST51 },
    { text: "52 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita de Exportação", value: EnumCSTPISCOFINS.CST52 },
    { text: "53 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno", value: EnumCSTPISCOFINS.CST53 },
    { text: "54 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas no Mercado Interno e de Exportação", value: EnumCSTPISCOFINS.CST54 },
    { text: "55 - Operação com Direito a Crédito - Vinculada a Receitas Não-Tributadas no Mercado Interno e de Exportação", value: EnumCSTPISCOFINS.CST55 },
    { text: "56 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno, e de Exportação", value: EnumCSTPISCOFINS.CST56 },
    { text: "60 - Crédito Presumido - Operação de Aquisição Vinculada Exclusivamente a Receita Tributada no Mercado Interno", value: EnumCSTPISCOFINS.CST60 },
    { text: "61 - Crédito Presumido - Operação de Aquisição Vinculada Exclusivamente a Receita Não-Tributada no Mercado Interno", value: EnumCSTPISCOFINS.CST61 },
    { text: "62 - Crédito Presumido - Operação de Aquisição Vinculada Exclusivamente a Receita de Exportação", value: EnumCSTPISCOFINS.CST62 },
    { text: "63 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno", value: EnumCSTPISCOFINS.CST63 },
    { text: "64 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Tributadas no Mercado Interno e de Exportação", value: EnumCSTPISCOFINS.CST64 },
    { text: "65 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Não-Tributadas no Mercado Interno e de Exportação", value: EnumCSTPISCOFINS.CST65 },
    { text: "66 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno, e de Exportação", value: EnumCSTPISCOFINS.CST66 },
    { text: "67 - Crédito Presumido - Outras Operações", value: EnumCSTPISCOFINS.CST67 },
    { text: "70 - Operação de Aquisição sem Direito a Crédito", value: EnumCSTPISCOFINS.CST70 },
    { text: "71 - Operação de Aquisição com Isenção", value: EnumCSTPISCOFINS.CST71 },
    { text: "72 - Operação de Aquisição com Suspensão", value: EnumCSTPISCOFINS.CST72 },
    { text: "73 - Operação de Aquisição a Alíquota Zero", value: EnumCSTPISCOFINS.CST73 },
    { text: "74 - Operação de Aquisição sem Incidência da Contribuição", value: EnumCSTPISCOFINS.CST74 },
    { text: "75 - Operação de Aquisição por Substituição Tributária", value: EnumCSTPISCOFINS.CST75 },
    { text: "98 - Outras Operações de Entrada", value: EnumCSTPISCOFINS.CST98 },
    { text: "99 - Outras Operações", value: EnumCSTPISCOFINS.CST99 }
];

var PesquisaCFOP = function () {
    this.NumeroCFOP = PropertyEntity({ text: "Número: ", getType: typesKnockout.int, maxlength: 4 });
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Extensao = PropertyEntity({ text: "Extensão: ", required: false, maxlength: 4 });
    this.Status = PropertyEntity({ val: ko.observable("A"), options: _statusPesquisaCFOP, def: true, text: "Status: " });
    this.TipoEntradaSaida = PropertyEntity({ val: ko.observable(EnumTipoCFOPPesquisa.Todos), options: EnumTipoCFOPPesquisa.obterOpcoesPesquisa(), def: EnumTipoCFOPPesquisa.Todos, text: "Tipo: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCFOP.CarregarGrid();
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

var CFOP = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoCFOP = PropertyEntity({ text: "*Número: ", required: true, maxlength: 4, enable: ko.observable(true) });
    this.Extensao = PropertyEntity({ text: "Extensão: ", required: false, maxlength: 4, visible: ko.observable(true) });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 1000 });
    this.Status = PropertyEntity({ val: ko.observable("A"), options: _statusCFOP, def: "A", text: "*Status: " });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoCFOP.Entrada), options: _tipoCFOP, def: EnumTipoCFOP.Entrada, text: "*Tipo: ", enable: ko.observable(true) });
    this.CSTICMS = PropertyEntity({ options: _cstICMS, val: ko.observable(0), def: 0, text: "CST/CSOSN ICMS: ", visible: ko.observable(true) });
    this.AliquotaInterna = PropertyEntity({ text: "% Alíquota Interna: ", required: false, getType: typesKnockout.decimal, maxlength: 5, visible: ko.observable(true) });
    this.AliquotaInterestadual = PropertyEntity({ text: "% Alíquota Interestadual: ", required: false, getType: typesKnockout.decimal, maxlength: 5, visible: ko.observable(true) });
    this.MVA = PropertyEntity({ text: "% MVA: ", required: false, getType: typesKnockout.decimal, maxlength: 6, visible: ko.observable(true) });
    this.ReducaoMVA = PropertyEntity({ text: "% Redução MVA: ", required: false, getType: typesKnockout.decimal, maxlength: 5, visible: ko.observable(true)});
    this.CSTIPI = PropertyEntity({ options: _cstIPI, val: ko.observable(0), def: 0, text: "CST IPI: ", visible: ko.observable(true) });
    this.ReducaoBCIPI = PropertyEntity({ text: "% Redução BC IPI: ", required: false, getType: typesKnockout.decimal, maxlength: 5, visible: ko.observable(true)});
    this.AliquotaIPI = PropertyEntity({ text: "% Alíquota IPI: ", required: false, getType: typesKnockout.decimal, maxlength: 5, visible: ko.observable(true)});
    this.CSTPIS = PropertyEntity({ options: _cstPISCOFINS, val: ko.observable(0), def: 0, text: "CST PIS: ", visible: ko.observable(true) });
    this.ReducaoBCPIS = PropertyEntity({ text: "% Redução BC PIS: ", required: false, getType: typesKnockout.decimal, maxlength: 5, visible: ko.observable(true)});
    this.AliquotaPIS = PropertyEntity({ text: "% Alíquota PIS: ", required: false, getType: typesKnockout.decimal, maxlength: 5, visible: ko.observable(true) });
    this.CSTCOFINS = PropertyEntity({ options: _cstPISCOFINS, val: ko.observable(0), def: 0, text: "CST COFINS: ", visible: ko.observable(true) });
    this.ReducaoBCCOFINS = PropertyEntity({ text: "% Redução BC COFINS: ", required: false, getType: typesKnockout.decimal, maxlength: 5, visible: ko.observable(true)});
    this.AliquotaCOFINS = PropertyEntity({ text: "% Alíquota COFINS: ", required: false, getType: typesKnockout.decimal, maxlength: 5, visible: ko.observable(true) });
    this.AliquotaDiferencial = PropertyEntity({ text: "Alíquota do Diferencial: ", required: false, getType: typesKnockout.decimal, maxlength: 5, visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), required: false, visible: ko.observable(false) });
    this.GeraEstoque = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Gera Estoque", def: false, visible: ko.observable(true) });
    this.RealizarRateioDespesaVeiculo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Realizar o rateio para a despesa do veículo", def: false });
    this.RealizarRateioSomenteQuandoTiverOS = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Realizar o rateio somente quando não tiver OS", def: false });
    this.ObrigarVincularAbastecimentoAoItemDocumentoEntrada = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Obrigar vincular abastecimento ao item do documento de entrada", def: false });
    this.ObrigarInformarLocalArmazenamento = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Obrigar informar local de armazenamento", def: false });
    this.BloqueioDocumentoEntrada = PropertyEntity({ text: "Bloqueio no Documento de Entrada: ", val: ko.observable(EnumBloqueioDocumentoEntrada.SemBloqueio), options: EnumBloqueioDocumentoEntrada.obterOpcoes(), def: EnumBloqueioDocumentoEntrada.SemBloqueio, visible: ko.observable(true) });
    this.CreditoSobreTotalParaItensSujeitosICMSST = PropertyEntity({ text: "Crédito sobre o Total para itens sujeitos a ICMS-ST", val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.CreditoSobreTotalParaProdutosUsoConsumo = PropertyEntity({ text: "Crédito sobre o Total para produtos de uso e consumo", val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.NaturezaOperacaoCTe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Natureza de Operação para Emissão de CT-e:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPrioridade = PropertyEntity({ text: "Grupo Prioridade:", required: false, getType: typesKnockout.int, maxlength: 15, enable: ko.observable(true), configInt: { precision: 0, allowZero: true }, def: "0", val: ko.observable("0"), visible: ko.observable(true) });
    this.IrrelevanteParaNaoConformidade = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Irrelevante Para Não Conformidade", def: false, visible: ko.observable(true)});

    this.AliquotaRetencaoPIS = PropertyEntity({ text: "Alíquota Retenção PIS: ", getType: typesKnockout.decimal, maxlength: 5, visible: ko.observable(true) });
    this.AliquotaParaCredito = PropertyEntity({ text: "Alíquota para crédito: ", getType: typesKnockout.decimal, maxlength: 5, visible: ko.observable(false) });
    this.AliquotaRetencaoCOFINS = PropertyEntity({ text: "Alíquota Retenção COFINS: ", getType: typesKnockout.decimal, maxlength: 5, visible: ko.observable(true)});
    this.AliquotaRetencaoINSS = PropertyEntity({ text: "Alíquota Retenção INSS: ", getType: typesKnockout.decimal, maxlength: 5, visible: ko.observable(true) });
    this.AliquotaRetencaoIPI = PropertyEntity({ text: "Alíquota Retenção IPI: ", getType: typesKnockout.decimal, maxlength: 5, visible: ko.observable(true)});
    this.AliquotaRetencaoCSLL = PropertyEntity({ text: "Alíquota Retenção CSLL: ", getType: typesKnockout.decimal, maxlength: 5, visible: ko.observable(true)});
    this.AliquotaRetencaoOutras = PropertyEntity({ text: "Alíquota Outras Retenções: ", getType: typesKnockout.decimal, maxlength: 5, visible: ko.observable(true)});
    this.AliquotaRetencaoIR = PropertyEntity({ text: "Alíquota Retenção IR: ", getType: typesKnockout.decimal, maxlength: 5, visible: ko.observable(true)});
    this.AliquotaRetencaoISS = PropertyEntity({ text: "Alíquota Retenção ISS: ", getType: typesKnockout.decimal, maxlength: 5, visible: ko.observable(true) });

    this.GerarMovimentoAutomatico = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Gerar movimento automatizado para esta CFOP: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUso = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Reversão:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomatico.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, ""); });

    this.GerarMovimentoAutomaticoDesconto = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Valor do Desconto - gerar movimento automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoDesconto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso do Desconto:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoDesconto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão do Desconto:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomaticoDesconto.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, "Desconto"); });

    this.GerarMovimentoAutomaticoOutrasDespesas = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Valor de Outras Despesas - gerar movimento automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoOutrasDespesas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso de Outras Despesas:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoOutrasDespesas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão de Outras Despesas:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomaticoOutrasDespesas.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, "OutrasDespesas"); });

    this.GerarMovimentoAutomaticoFrete = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Valor do Frete - gerar movimento automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso do Frete:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão do Frete:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomaticoFrete.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, "Frete"); });

    this.GerarMovimentoAutomaticoICMS = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Valor do ICMS - gerar movimento automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoICMS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso do ICMS:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoICMS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão do ICMS:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomaticoICMS.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, "ICMS"); });

    this.GerarMovimentoAutomaticoPIS = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Valor do PIS - gerar movimento automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoPIS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso do PIS:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoPIS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão do PIS:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomaticoPIS.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, "PIS"); });

    this.GerarMovimentoAutomaticoCOFINS = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Valor da COFINS - gerar movimento automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoCOFINS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso da COFINS:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoCOFINS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão da COFINS:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomaticoCOFINS.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, "COFINS"); });

    this.GerarMovimentoAutomaticoIPI = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Valor do IPI - gerar movimento automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoIPI = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso do IPI:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoIPI = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão do IPI:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomaticoIPI.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, "IPI"); });

    this.GerarMovimentoAutomaticoICMSST = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Valor do ICMS ST - gerar movimento automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoICMSST = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso do ICMS ST:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoICMSST = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão do ICMS ST:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomaticoICMSST.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, "ICMSST"); });

    this.GerarMovimentoAutomaticoDiferencial = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Valor do Diferencial - gerar movimento automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoDiferencial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso do Diferencial:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoDiferencial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão do Diferencial:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomaticoDiferencial.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, "Diferencial"); });

    this.GerarMovimentoAutomaticoSeguro = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Valor do Seguro - gerar movimento automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoSeguro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso do Seguro:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoSeguro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão do Seguro:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomaticoSeguro.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, "Seguro"); });

    this.GerarMovimentoAutomaticoFreteFora = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Valor do Frete Fora - gerar movimento automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoFreteFora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso do Frete Fora:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoFreteFora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão do Frete Fora:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomaticoFreteFora.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, "FreteFora"); });

    this.GerarMovimentoAutomaticoOutrasFora = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Valor do Outras Fora - gerar movimento automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoOutrasFora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso do Outras Fora:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoOutrasFora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão do Outras Fora:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomaticoOutrasFora.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, "OutrasFora"); });

    this.GerarMovimentoAutomaticoDescontoFora = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Valor do Desconto Fora - gerar movimento automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoDescontoFora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso do Desconto Fora:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoDescontoFora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão do Desconto Fora:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomaticoDescontoFora.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, "DescontoFora"); });

    this.GerarMovimentoAutomaticoImpostoFora = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Valor do Imposto Fora - gerar movimento automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoImpostoFora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso do Imposto Fora:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoImpostoFora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão do Imposto Fora:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomaticoImpostoFora.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, "ImpostoFora"); });

    this.GerarMovimentoAutomaticoDiferencialFreteFora = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Valor do Diferencial do Frete Fora - gerar movimento automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoDiferencialFreteFora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso do Diferencial do Frete Fora:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoDiferencialFreteFora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão do Diferencial do Frete Fora:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomaticoDiferencialFreteFora.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, "DiferencialFreteFora"); });

    this.GerarMovimentoAutomaticoICMSFreteFora = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Valor do ICMS do Frete Fora - gerar movimento automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoICMSFreteFora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso do ICMS do Frete Fora:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoICMSFreteFora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão do ICMS do Frete Fora:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomaticoICMSFreteFora.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, "ICMSFreteFora"); });

    this.GerarMovimentoAutomaticoCusto = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Valor do Custo - gerar movimento automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoCusto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso do Custo:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoCusto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão do Custo:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomaticoCusto.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, "Custo"); });

    this.GerarMovimentoAutomaticoRetencaoPIS = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Retenção do PIS - gerar movimento automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoRetencaoPIS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso da Retenção do PIS:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoRetencaoPIS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão da Retenção do PIS:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomaticoRetencaoPIS.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, "RetencaoPIS"); });

    this.ReduzValorLiquidoRetencaoPIS = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Reduz o valor líquido a pagar ao fornecedor da nota de entrada?", def: false });
    this.GerarGuiaPagarRetencaoPIS = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Guia do PIS - gerar título automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoTituloRetencaoPIS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso da GUIA do PIS:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoTituloRetencaoPIS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão da GUIA do PIS:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.FornecedorRetencaoPIS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Fornecedor:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.DiaGerencaoRetencaoPIS = PropertyEntity({ val: ko.observable(25), options: _diaMes, def: 25, text: "*Dia Vencimento GUIA do PIS: ", visible: ko.observable(true), required: ko.observable(false) });
    this.CalcularVenvimentoAPartirDataVencimentoTituloNotaRetencaoPIS = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Calcular o vencimento da Guia a partir da data de vencimento do Título da Nota?", def: false });

    this.GerarGuiaPagarRetencaoPIS.val.subscribe(function (novoValor) { GerarTituloAutomaticoChange(novoValor, "RetencaoPIS"); });

    this.GerarMovimentoAutomaticoRetencaoCOFINS = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Retenção da COFINS - gerar movimento automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoRetencaoCOFINS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso da Retenção da COFINS:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoRetencaoCOFINS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão da Retenção da COFINS:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomaticoRetencaoCOFINS.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, "RetencaoCOFINS"); });

    this.ReduzValorLiquidoRetencaoCOFINS = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Reduz o valor líquido a pagar ao fornecedor da nota de entrada?", def: false });
    this.GerarGuiaPagarRetencaoCOFINS = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Guia do COFINS - gerar título automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoTituloRetencaoCOFINS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso da GUIA do COFINS:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoTituloRetencaoCOFINS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão da GUIA do COFINS:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.FornecedorRetencaoCOFINS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Fornecedor:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.DiaGerencaoRetencaoCOFINS = PropertyEntity({ val: ko.observable(25), options: _diaMes, def: 25, text: "*Dia Vencimento GUIA do COFINS: ", visible: ko.observable(true), required: ko.observable(false) });
    this.GerarGuiaPagarRetencaoCOFINS.val.subscribe(function (novoValor) { GerarTituloAutomaticoChange(novoValor, "RetencaoCOFINS"); });

    this.GerarMovimentoAutomaticoRetencaoINSS = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Retenção do INSS - gerar movimento automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoRetencaoINSS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso da Retenção do INSS:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoRetencaoINSS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão da Retenção do INSS:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomaticoRetencaoINSS.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, "RetencaoINSS"); });

    this.ReduzValorLiquidoRetencaoINSS = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Reduz o valor líquido a pagar ao fornecedor da nota de entrada?", def: false });
    this.GerarGuiaPagarRetencaoINSS = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Guia do INSS - gerar título automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoTituloRetencaoINSS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso da GUIA do INSS:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoTituloRetencaoINSS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão da GUIA do INSS:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.FornecedorRetencaoINSS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Fornecedor:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.DiaGerencaoRetencaoINSS = PropertyEntity({ val: ko.observable(25), options: _diaMes, def: 25, text: "*Dia Vencimento GUIA do INSS: ", visible: ko.observable(true), required: ko.observable(false) });
    this.GerarGuiaPagarRetencaoINSS.val.subscribe(function (novoValor) { GerarTituloAutomaticoChange(novoValor, "RetencaoINSS"); });

    this.GerarMovimentoAutomaticoRetencaoCSLL = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Retenção da CSLL - gerar movimento automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoRetencaoCSLL = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso da Retenção da CSLL:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoRetencaoCSLL = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão da Retenção da CSLL:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomaticoRetencaoCSLL.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, "RetencaoCSLL"); });

    this.ReduzValorLiquidoRetencaoCSLL = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Reduz o valor líquido a pagar ao fornecedor da nota de entrada?", def: false });
    this.GerarGuiaPagarRetencaoCSLL = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Guia do CSLL - gerar título automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoTituloRetencaoCSLL = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso da GUIA do CSLL:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoTituloRetencaoCSLL = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão da GUIA do CSLL:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.FornecedorRetencaoCSLL = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Fornecedor:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.DiaGerencaoRetencaoCSLL = PropertyEntity({ val: ko.observable(25), options: _diaMes, def: 25, text: "*Dia Vencimento GUIA do CSLL: ", visible: ko.observable(true), required: ko.observable(false) });
    this.GerarGuiaPagarRetencaoCSLL.val.subscribe(function (novoValor) { GerarTituloAutomaticoChange(novoValor, "RetencaoCSLL"); });

    this.GerarMovimentoAutomaticoRetencaoIPI = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Retenção do IPI - gerar movimento automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoRetencaoIPI = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso da Retenção do IPI:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoRetencaoIPI = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão da Retenção do IPI:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomaticoRetencaoIPI.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, "RetencaoIPI"); });

    this.ReduzValorLiquidoRetencaoIPI = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Reduz o valor líquido a pagar ao fornecedor da nota de entrada?", def: false });
    this.GerarGuiaPagarRetencaoIPI = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Guia do IPI - gerar título automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoTituloRetencaoIPI = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso da GUIA do IPI:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoTituloRetencaoIPI = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão da GUIA do IPI:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.FornecedorRetencaoIPI = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Fornecedor:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.DiaGerencaoRetencaoIPI = PropertyEntity({ val: ko.observable(25), options: _diaMes, def: 25, text: "*Dia Vencimento GUIA do IPI: ", visible: ko.observable(true), required: ko.observable(false) });
    this.GerarGuiaPagarRetencaoIPI.val.subscribe(function (novoValor) { GerarTituloAutomaticoChange(novoValor, "RetencaoIPI"); });

    this.GerarMovimentoAutomaticoRetencaoOutras = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Retenção do Outras - gerar movimento automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoRetencaoOutras = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso da Retenção do Outras:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoRetencaoOutras = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão da Retenção do Outras:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomaticoRetencaoOutras.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, "RetencaoOutras"); });

    this.ReduzValorLiquidoRetencaoOutras = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Reduz o valor líquido a pagar ao fornecedor da nota de entrada?", def: false });
    this.GerarGuiaPagarRetencaoOutras = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Guia de Outras Retençõe - gerar título automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoTituloRetencaoOutras = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso da GUIA de Outras Retençõe:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoTituloRetencaoOutras = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão da GUIA de Outras Retençõe:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.FornecedorRetencaoOutras = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Fornecedor:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.DiaGerencaoRetencaoOutras = PropertyEntity({ val: ko.observable(25), options: _diaMes, def: 25, text: "*Dia Vencimento GUIA de Outras Retenções: ", visible: ko.observable(true), required: ko.observable(false) });
    this.GerarGuiaPagarRetencaoOutras.val.subscribe(function (novoValor) { GerarTituloAutomaticoChange(novoValor, "RetencaoOutras"); });

    this.GerarMovimentoAutomaticoRetencaoISS = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Retenção do ISS - gerar movimento automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoRetencaoISS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso da Retenção do ISS:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoRetencaoISS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão da Retenção do ISS:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomaticoRetencaoISS.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, "RetencaoISS"); });

    this.ReduzValorLiquidoRetencaoISS = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Reduz o valor líquido a pagar ao fornecedor da nota de entrada?", def: false });
    this.GerarGuiaPagarRetencaoISS = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Guia do ISS - gerar título automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoTituloRetencaoISS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso da GUIA do ISS:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoTituloRetencaoISS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão da GUIA do ISS:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.FornecedorRetencaoISS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Fornecedor:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.DiaGerencaoRetencaoISS = PropertyEntity({ val: ko.observable(25), options: _diaMes, def: 25, text: "*Dia Vencimento GUIA do ISS: ", visible: ko.observable(true), required: ko.observable(false) });
    this.GerarGuiaPagarRetencaoISS.val.subscribe(function (novoValor) { GerarTituloAutomaticoChange(novoValor, "RetencaoISS"); });

    this.GerarMovimentoAutomaticoRetencaoIR = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Retenção do IR - gerar movimento automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoRetencaoIR = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso da Retenção do IR:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoRetencaoIR = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão da Retenção do IR:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomaticoRetencaoIR.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor, "RetencaoIR"); });

    this.ReduzValorLiquidoRetencaoIR = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Reduz o valor líquido a pagar ao fornecedor da nota de entrada?", def: false });
    this.GerarGuiaPagarRetencaoIR = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Guia do IR - gerar título automatizado: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoTituloRetencaoIR = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso da GUIA do IR:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoTituloRetencaoIR = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão da GUIA do IR:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.FornecedorRetencaoIR = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Fornecedor:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.DiaGerencaoRetencaoIR = PropertyEntity({ val: ko.observable(25), options: _diaMes, def: 25, text: "*Dia Vencimento GUIA do IR: ", visible: ko.observable(true), required: ko.observable(false) });
    this.GerarGuiaPagarRetencaoIR.val.subscribe(function (novoValor) { GerarTituloAutomaticoChange(novoValor, "RetencaoIR"); });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.Tipo.val.subscribe(function (novoValor) {
        var visivelSaida = (novoValor == EnumTipoCFOP.Saida);

        _cfop.NaturezaOperacaoCTe.visible(visivelSaida);
    });

    this.CreditoSobreTotalParaProdutosUsoConsumo.val.subscribe(function (novoValor) {
        _cfop.AliquotaParaCredito.visible(novoValor);
    });
};

//*******EVENTOS*******


function loadCFOP() {

    _pesquisaCFOP = new PesquisaCFOP();
    KoBindings(_pesquisaCFOP, "knockoutPesquisaCFOP", false, _pesquisaCFOP.Pesquisar.id);

    _cfop = new CFOP();
    KoBindings(_cfop, "knockoutCadastroCFOP");

    HeaderAuditoria("CFOP", _cfop);

    new BuscarTipoMovimento(_cfop.TipoMovimentoUso);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversao);
    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoDesconto);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoDesconto);
    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoOutrasDespesas);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoOutrasDespesas);
    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoFrete);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoFrete);
    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoICMS);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoICMS);
    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoPIS);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoPIS);
    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoCOFINS);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoCOFINS);
    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoIPI);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoIPI);
    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoICMSST);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoICMSST);
    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoDiferencial);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoDiferencial);

    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoSeguro);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoSeguro);
    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoFreteFora);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoFreteFora);
    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoOutrasFora);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoOutrasFora);
    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoDescontoFora);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoDescontoFora);
    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoImpostoFora);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoImpostoFora);
    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoDiferencialFreteFora);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoDiferencialFreteFora);
    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoICMSFreteFora);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoICMSFreteFora);
    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoCusto);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoCusto);

    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoRetencaoCOFINS);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoRetencaoCSLL);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoRetencaoINSS);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoRetencaoIPI);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoRetencaoOutras);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoRetencaoPIS);
    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoRetencaoCOFINS);
    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoRetencaoCSLL);
    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoRetencaoINSS);
    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoRetencaoIPI);
    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoRetencaoOutras);
    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoRetencaoPIS);

    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoRetencaoISS);
    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoRetencaoIR);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoRetencaoISS);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoRetencaoIR);

    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoTituloRetencaoPIS);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoTituloRetencaoPIS);
    new BuscarClientes(_cfop.FornecedorRetencaoPIS);

    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoTituloRetencaoCOFINS);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoTituloRetencaoCOFINS);
    new BuscarClientes(_cfop.FornecedorRetencaoCOFINS);

    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoTituloRetencaoINSS);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoTituloRetencaoINSS);
    new BuscarClientes(_cfop.FornecedorRetencaoINSS);

    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoTituloRetencaoCSLL);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoTituloRetencaoCSLL);
    new BuscarClientes(_cfop.FornecedorRetencaoCSLL);

    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoTituloRetencaoIPI);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoTituloRetencaoIPI);
    new BuscarClientes(_cfop.FornecedorRetencaoIPI);

    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoTituloRetencaoOutras);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoTituloRetencaoOutras);
    new BuscarClientes(_cfop.FornecedorRetencaoOutras);

    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoTituloRetencaoISS);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoTituloRetencaoISS);
    new BuscarClientes(_cfop.FornecedorRetencaoISS);

    new BuscarTipoMovimento(_cfop.TipoMovimentoUsoTituloRetencaoIR);
    new BuscarTipoMovimento(_cfop.TipoMovimentoReversaoTituloRetencaoIR);
    new BuscarClientes(_cfop.FornecedorRetencaoIR);

    new BuscarNaturezasOperacoes(_cfop.NaturezaOperacaoCTe);

    buscarCFOPs();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _cfop.Extensao.visible(false);
        _cfop.GeraEstoque.visible(true);
        _cfop.CSTICMS.visible(false);
        _cfop.AliquotaInterna.visible(false);
        _cfop.AliquotaInterestadual.visible(false);
        _cfop.MVA.visible(false);
        _cfop.ReducaoMVA.visible(false);
        _cfop.CSTIPI.visible(false);
        _cfop.ReducaoBCIPI.visible(false);
        _cfop.AliquotaIPI.visible(false);
        _cfop.CSTPIS.visible(false);
        _cfop.ReducaoBCPIS.visible(false);
        _cfop.AliquotaPIS.visible(false);
        _cfop.CSTCOFINS.visible(false);
        _cfop.ReducaoBCCOFINS.visible(false);
        _cfop.AliquotaCOFINS.visible(false);
        _cfop.AliquotaDiferencial.visible(false);
        _cfop.BloqueioDocumentoEntrada.visible(false);
        _cfop.AliquotaRetencaoPIS.visible(false);
        _cfop.AliquotaRetencaoCOFINS.visible(false);
        _cfop.AliquotaRetencaoINSS.visible(false);
        _cfop.AliquotaRetencaoIPI.visible(false);
        _cfop.AliquotaRetencaoOutras.visible(false);
        _cfop.AliquotaRetencaoIR.visible(false);
        _cfop.AliquotaRetencaoISS.visible(false);
        _cfop.AliquotaRetencaoCSLL.visible(false);
        $("#liTabMovimentoFinanceiroEntrada").hide();
    } 
}

function GerarMovimentoAutomaticoChange(novoValor, propriedade) {
    var propriedadeGeraMovimento = "GerarMovimentoAutomatico" + propriedade;
    var propriedadeTipoMovimentoUso = "TipoMovimentoUso" + propriedade;

    _cfop[propriedadeGeraMovimento].visibleFade(novoValor);
    _cfop[propriedadeTipoMovimentoUso].required(novoValor);
}

function GerarTituloAutomaticoChange(novoValor, propriedade) {
    var propriedadeGeraMovimento = "GerarGuiaPagar" + propriedade;
    var propriedadeTipoMovimentoUso = "TipoMovimentoUsoTitulo" + propriedade;
    var propriedadeTipoMovimentoReversao = "TipoMovimentoReversaoTitulo" + propriedade;
    var propriedadeFornecedor = "Fornecedor" + propriedade;
    var propriedadeDiaGerencao = "DiaGerencao" + propriedade;

    _cfop[propriedadeGeraMovimento].visibleFade(novoValor);
    _cfop[propriedadeTipoMovimentoUso].required(novoValor);
    _cfop[propriedadeTipoMovimentoReversao].required(novoValor);
    _cfop[propriedadeFornecedor].required(novoValor);
    _cfop[propriedadeDiaGerencao].required(novoValor);
}


function adicionarClick(e, sender) {
    Salvar(e, "CFOPNotaFiscal/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridCFOP.CarregarGrid();
                limparCamposCFOP();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "CFOPNotaFiscal/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridCFOP.CarregarGrid();
                limparCamposCFOP();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a CFOP " + _cfop.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_cfop, "CFOPNotaFiscal/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridCFOP.CarregarGrid();
                    limparCamposCFOP();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposCFOP();
}

//*******MÉTODOS*******


function buscarCFOPs() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarCFOP, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridCFOP = new GridView(_pesquisaCFOP.Pesquisar.idGrid, "CFOPNotaFiscal/Pesquisa", _pesquisaCFOP, menuOpcoes, null);
    _gridCFOP.CarregarGrid();
}

function editarCFOP(cfopGrid) {
    limparCamposCFOP();
    _cfop.Codigo.val(cfopGrid.Codigo);
    BuscarPorCodigo(_cfop, "CFOPNotaFiscal/BuscarPorCodigo", function (arg) {
        _pesquisaCFOP.ExibirFiltros.visibleFade(false);
        _cfop.Atualizar.visible(true);
        _cfop.Cancelar.visible(true);
        _cfop.Excluir.visible(true);
        _cfop.Adicionar.visible(false);
        _cfop.CodigoCFOP.enable(false);
        _cfop.Tipo.enable(false);
    }, null);
}

function limparCamposCFOP() {
    _cfop.Atualizar.visible(false);
    _cfop.Cancelar.visible(false);
    _cfop.Excluir.visible(false);
    _cfop.Adicionar.visible(true);
    _cfop.CodigoCFOP.enable(true);
    _cfop.Tipo.enable(true);
    LimparCampos(_cfop);
    $(".nav-tabs a:first").trigger("click");
}
