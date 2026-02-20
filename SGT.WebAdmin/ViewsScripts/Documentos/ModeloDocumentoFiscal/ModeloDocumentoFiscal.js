/// <reference path="../../Enumeradores/EnumTipoDocumentoCreditoDebito.js" />
/// <reference path="../../Enumeradores/EnumTipoDocumentoModeloDeImpressao.js" />
//*******MAPEAMENTO KNOUCKOUT*******

var _gridModeloDocumentoFiscal;
var _modeloDocumentoFiscal;
var _pesquisaModeloDocumentoFiscal;
var _crudModeloDocumentoFiscal;

var PesquisaModeloDocumentoFiscal = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridModeloDocumentoFiscal.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var ModeloDocumentoFiscal = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 150 });
    this.Abreviacao = PropertyEntity({ text: "*Abreviação: ", required: ko.observable(true), maxlength: 40 });
    this.Especie = PropertyEntity({ text: "Espécie: ", maxlength: 50, visible: (_CONFIGURACAO_TMS.ExibirEspecieDocumentoCteComplementarOcorrencia == true) });
    this.Numero = PropertyEntity({ text: "*Número do Documento: ", required: true, maxlength: 10 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.TipoDocumentoCreditoDebito = PropertyEntity({ val: ko.observable(EnumTipoDocumentoCreditoDebito.Credito), options: EnumTipoDocumentoCreditoDebito.obterOpcoes(), def: EnumTipoDocumentoCreditoDebito.Credito, text: "*Tipo Documento: " });    
    this.Editavel = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool });
    this.CalcularImpostos = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Calcular ICMS", visible: ko.observable(true) });
    this.UtilizarNumeracaoCTe = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Utilizar a mesma numeração do CT-e", visible: ko.observable(true) });
    this.UtilizarNumeracaoNFe = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Utilizar a mesma numeração da NF-e", visible: ko.observable(true) });
    this.NaoGerarFaturamento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Não gerar faturamento para este modelo de documento", visible: ko.observable(true) });
    this.GerarISSAutomaticamente = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Gerar o ISS quando for 'Outros Documentos' de forma automática para este modelo de documento", visible: ko.observable(true) });
    this.NaoGerarEscrituracao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Não gerar escrituração para este modelo de documento", visible: ko.observable(true) });
    this.DocumentoComMoedaEstrangeira = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Este documento é lançado com moeda estrangeira", visible: ko.observable(true) });
    this.MoedaCotacaoBancoCentral = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.DolarVenda), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.DolarVenda, text: "*Moeda Banco Central: ", visible: ko.observable(false) });
    this.DocumentoTipoCRT = PropertyEntity({ getType: typesKnockout.bool, text: "Documento do tipo CRT", def: false, val: ko.observable(false), visible: ko.observable(true) });
    this.DescontarValorDesseDocumentoFatura = PropertyEntity({ getType: typesKnockout.bool, text: "Acrescentar/Descontar o valor desse documento na Fatura (Baseado no tipo documento)", def: false, val: ko.observable(false), visible: ko.observable(true) });

    this.AverbarDocumento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Fazer a averbação do seguro para este modelo de documento?", idFade: guid(), visibleFade: ko.observable(false) });

    this.GerarMovimentoAutomatico = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Gerar movimento financeiro automatizado para a emissão deste documento", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoEmissao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Emissão:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoCancelamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Cancelamento:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoAnulacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Anulação:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoAnulacaoComNotaAnulacaoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Anulação (com nota de anulação do embarcador):", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });

    this.DiferenciarMovimentosParaImpostos = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Diferenciar os movimentos financeiros para os impostos do documento", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoImpostoEmissao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Emissão:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoImpostoCancelamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Cancelamento:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoImpostoAnulacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Anulação:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoImpostoAnulacaoComNotaAnulacaoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Anulação (com nota de anulação do embarcador)", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });

    this.DiferenciarMovimentosParaPIS = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Diferenciar os movimentos financeiros para o PIS do documento", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoPISEmissao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Emissão:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoPISCancelamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Cancelamento:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoPISAnulacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Anulação:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoPISAnulacaoComNotaAnulacaoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Anulação (com nota de anulação do embarcador):", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });

    this.DiferenciarMovimentosParaCOFINS = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Diferenciar os movimentos financeiros para a COFINS do documento", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoCOFINSEmissao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Emissão:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoCOFINSCancelamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Cancelamento:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoCOFINSAnulacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Anulação:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoCOFINSAnulacaoComNotaAnulacaoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Anulação (com nota de anulação do embarcador):", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });

    this.DiferenciarMovimentosParaIR = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Diferenciar os movimentos financeiros para o IR do documento", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoIREmissao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Emissão:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoIRCancelamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Cancelamento:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoIRAnulacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Anulação:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoIRAnulacaoComNotaAnulacaoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Anulação (com nota de anulação do embarcador):", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });

    this.DiferenciarMovimentosParaCSLL = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Diferenciar os movimentos financeiros para a CSLL do documento", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoCSLLEmissao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Emissão:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoCSLLCancelamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Cancelamento:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoCSLLAnulacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Anulação:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoCSLLAnulacaoComNotaAnulacaoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Anulação (com nota de anulação do embarcador):", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });

    this.DiferenciarMovimentosParaValorLiquido = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Diferenciar os movimentos financeiros para o valor líquido do documento", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoValorLiquidoEmissao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Emissão:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoValorLiquidoCancelamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Cancelamento:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoValorLiquidoAnulacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Anulação:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoValorLiquidoAnulacaoComNotaAnulacaoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Anulação (com nota de anulação do embarcador):", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });

    this.GerarMovimentoAutomaticoEntrada = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Gerar movimento financeiro automatizado para as duplicatas da entrada deste documento", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoEntrada = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso da Duplicata:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoEntrada = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão da Duplicata:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });

    this.GerarMovimentoBaseSTRetido = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Gerar movimento financeiro para a Base ST Retido deste documento", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoBaseSTRetidoEmissao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoBaseSTRetidoReversao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });

    this.GerarMovimentoValorSTRetido = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Gerar movimento financeiro para a Valor ST Retido deste documento", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoValorSTRetidoEmissao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoValorSTRetidoReversao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });

    this.Series = PropertyEntity({ type: types.dynamic, list: new Array(), idGrid: guid() });
    this.ListaSeries = PropertyEntity({ val: ko.observable(""), def: "" });

    this.DocumentoComMoedaEstrangeira.val.subscribe(function (novoValor) {
        _modeloDocumentoFiscal.MoedaCotacaoBancoCentral.visible(novoValor);
        _modeloDocumentoFiscal.MoedaCotacaoBancoCentral.val(EnumMoedaCotacaoBancoCentral.DolarVenda);
    })

    this.GerarMovimentoAutomatico.val.subscribe(function (novoValor) {
        GerarMovimentoAutomaticoChange(novoValor);
    });

    this.GerarMovimentoAutomaticoEntrada.val.subscribe(function (novoValor) {
        GerarMovimentoAutomaticoEntradaChange(novoValor);
    });

    this.GerarMovimentoBaseSTRetido.val.subscribe(function (novoValor) {
        GerarMovimentoBaseSTRetidoChange(novoValor);
    });

    this.GerarMovimentoValorSTRetido.val.subscribe(function (novoValor) {
        GerarMovimentoValorSTRetidoChange(novoValor);
    });

    this.DiferenciarMovimentosParaImpostos.val.subscribe(function (novoValor) {
        DiferenciarMovimentosParaImpostosChange(novoValor);
    });

    this.DiferenciarMovimentosParaPIS.val.subscribe(function (novoValor) {
        DiferenciarMovimentosParaPISChange(novoValor);
    });

    this.DiferenciarMovimentosParaCOFINS.val.subscribe(function (novoValor) {
        DiferenciarMovimentosParaCOFINSChange(novoValor);
    });

    this.DiferenciarMovimentosParaIR.val.subscribe(function (novoValor) {
        DiferenciarMovimentosParaIRChange(novoValor);
    });

    this.DiferenciarMovimentosParaCSLL.val.subscribe(function (novoValor) {
        DiferenciarMovimentosParaCSLLChange(novoValor);
    });

    this.DiferenciarMovimentosParaValorLiquido.val.subscribe(function (novoValor) {
        DiferenciarMovimentosParaValorLiquidoChange(novoValor);
    });

    this.Editavel.val.subscribe(function (novoValor) {
        _modeloDocumentoFiscal.UtilizarNumeracaoCTe.visible(novoValor);
        _modeloDocumentoFiscal.UtilizarNumeracaoCTe.val(false);
        _modeloDocumentoFiscal.CalcularImpostos.visible(novoValor);
    });
};

var CRUDModeloDocumentoFiscal = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadModeloDocumentoFiscal() {

    _crudModeloDocumentoFiscal = new CRUDModeloDocumentoFiscal();
    KoBindings(_crudModeloDocumentoFiscal, "knockoutCRUDModeloDocumentoFiscal");

    _modeloDocumentoFiscal = new ModeloDocumentoFiscal();
    KoBindings(_modeloDocumentoFiscal, "knockoutCadastroModeloDocumentoFiscal");

    _pesquisaModeloDocumentoFiscal = new PesquisaModeloDocumentoFiscal();
    KoBindings(_pesquisaModeloDocumentoFiscal, "knockoutPesquisaModeloDocumentoFiscal", false, _pesquisaModeloDocumentoFiscal.Pesquisar.id);

    HeaderAuditoria("ModeloDocumentoFiscal", _modeloDocumentoFiscal);

    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoUsoEntrada);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoReversaoEntrada);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoEmissao);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoCancelamento);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoAnulacao);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoAnulacaoComNotaAnulacaoEmbarcador);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoImpostoEmissao);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoImpostoCancelamento);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoImpostoAnulacao);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoImpostoAnulacaoComNotaAnulacaoEmbarcador);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissao);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamento);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacao);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacaoComNotaAnulacaoEmbarcador);

    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoPISEmissao);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoPISCancelamento);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoPISAnulacao);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoPISAnulacaoComNotaAnulacaoEmbarcador);

    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoCOFINSEmissao);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoCOFINSCancelamento);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoCOFINSAnulacao);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoCOFINSAnulacaoComNotaAnulacaoEmbarcador);

    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoIREmissao);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoIRCancelamento);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoIRAnulacao);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoIRAnulacaoComNotaAnulacaoEmbarcador);

    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoCSLLEmissao);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoCSLLCancelamento);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoCSLLAnulacao);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoCSLLAnulacaoComNotaAnulacaoEmbarcador);

    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoBaseSTRetidoEmissao);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoBaseSTRetidoReversao);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoValorSTRetidoEmissao);
    new BuscarTipoMovimento(_modeloDocumentoFiscal.TipoMovimentoValorSTRetidoReversao);

    LoadSerieModeloDocumentoFiscal();
    buscarModeloDocumentoFiscals();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        $("#liTabMovimentoFinanceiro").removeClass("d-none");
        $("#liTabMovimentoFinanceiroEntrada").removeClass("d-none");
        $("#liTabSerie").removeClass("d-none");
        _modeloDocumentoFiscal.NaoGerarEscrituracao.visible(false);
    }

    $('.nav-tabs a').click(function (e) {
        e.preventDefault();
        $('#tabsModeloDocumento .tab-content').each(function (i, tabContent) {
            $(tabContent).children().each(function (z, el) {
                $(el).removeClass('active');
            });
        });
        $(this).tab('show');
    });
}

function GerarMovimentoAutomaticoEntradaChange(novoValor) {
    _modeloDocumentoFiscal.GerarMovimentoAutomaticoEntrada.visibleFade(novoValor);
    _modeloDocumentoFiscal.TipoMovimentoUsoEntrada.required(novoValor);
    _modeloDocumentoFiscal.TipoMovimentoReversaoEntrada.required(novoValor);
}

function GerarMovimentoBaseSTRetidoChange(novoValor) {
    _modeloDocumentoFiscal.GerarMovimentoBaseSTRetido.visibleFade(novoValor);
    _modeloDocumentoFiscal.TipoMovimentoBaseSTRetidoEmissao.required(novoValor);
    _modeloDocumentoFiscal.TipoMovimentoBaseSTRetidoReversao.required(novoValor);
}

function GerarMovimentoValorSTRetidoChange(novoValor) {
    _modeloDocumentoFiscal.GerarMovimentoValorSTRetido.visibleFade(novoValor);
    _modeloDocumentoFiscal.TipoMovimentoValorSTRetidoEmissao.required(novoValor);
    _modeloDocumentoFiscal.TipoMovimentoValorSTRetidoReversao.required(novoValor);
}

function GerarMovimentoAutomaticoChange(novoValor) {
    if (novoValor) {
        _modeloDocumentoFiscal.GerarMovimentoAutomatico.visibleFade(true);

        if (_modeloDocumentoFiscal.Numero.val() == 57) {
            _modeloDocumentoFiscal.TipoMovimentoAnulacaoComNotaAnulacaoEmbarcador.visible(true);
            _modeloDocumentoFiscal.TipoMovimentoAnulacaoComNotaAnulacaoEmbarcador.required(true);
        } else {
            _modeloDocumentoFiscal.TipoMovimentoAnulacaoComNotaAnulacaoEmbarcador.visible(false);
            _modeloDocumentoFiscal.TipoMovimentoAnulacaoComNotaAnulacaoEmbarcador.required(false);
        }

        _modeloDocumentoFiscal.TipoMovimentoEmissao.required(true);
        _modeloDocumentoFiscal.TipoMovimentoCancelamento.required(true);
        _modeloDocumentoFiscal.TipoMovimentoAnulacao.required(true);
    } else {
        _modeloDocumentoFiscal.GerarMovimentoAutomatico.visibleFade(false);

        if (_modeloDocumentoFiscal.Numero.val() == 57) {
            _modeloDocumentoFiscal.TipoMovimentoAnulacaoComNotaAnulacaoEmbarcador.visible(true);
            _modeloDocumentoFiscal.TipoMovimentoAnulacaoComNotaAnulacaoEmbarcador.required(false);
        } else {
            _modeloDocumentoFiscal.TipoMovimentoAnulacaoComNotaAnulacaoEmbarcador.visible(false);
            _modeloDocumentoFiscal.TipoMovimentoAnulacaoComNotaAnulacaoEmbarcador.required(false);
        }

        _modeloDocumentoFiscal.TipoMovimentoEmissao.required(false);
        _modeloDocumentoFiscal.TipoMovimentoCancelamento.required(false);
        _modeloDocumentoFiscal.TipoMovimentoAnulacao.required(false);
    }

    DiferenciarMovimentosParaImpostosChange();
    DiferenciarMovimentosParaValorLiquidoChange();
}

function DiferenciarMovimentosParaImpostosChange(novoValor) {
    if (novoValor == null)
        novoValor = _modeloDocumentoFiscal.DiferenciarMovimentosParaImpostos.val();

    if (novoValor)
        _modeloDocumentoFiscal.DiferenciarMovimentosParaImpostos.visibleFade(true);
    else
        _modeloDocumentoFiscal.DiferenciarMovimentosParaImpostos.visibleFade(false);

    if (novoValor && _modeloDocumentoFiscal.GerarMovimentoAutomatico.val()) {
        if (_modeloDocumentoFiscal.Numero.val() == 57) {
            _modeloDocumentoFiscal.TipoMovimentoImpostoAnulacaoComNotaAnulacaoEmbarcador.visible(true);
            _modeloDocumentoFiscal.TipoMovimentoImpostoAnulacaoComNotaAnulacaoEmbarcador.required(true);
        } else {
            _modeloDocumentoFiscal.TipoMovimentoImpostoAnulacaoComNotaAnulacaoEmbarcador.visible(false);
            _modeloDocumentoFiscal.TipoMovimentoImpostoAnulacaoComNotaAnulacaoEmbarcador.required(false);
        }

        _modeloDocumentoFiscal.TipoMovimentoImpostoEmissao.required(true);
        _modeloDocumentoFiscal.TipoMovimentoImpostoCancelamento.required(true);
        _modeloDocumentoFiscal.TipoMovimentoImpostoAnulacao.required(true);
    } else {
        if (_modeloDocumentoFiscal.Numero.val() == 57) {
            _modeloDocumentoFiscal.TipoMovimentoImpostoAnulacaoComNotaAnulacaoEmbarcador.visible(true);
            _modeloDocumentoFiscal.TipoMovimentoImpostoAnulacaoComNotaAnulacaoEmbarcador.required(false);
        } else {
            _modeloDocumentoFiscal.TipoMovimentoImpostoAnulacaoComNotaAnulacaoEmbarcador.visible(false);
            _modeloDocumentoFiscal.TipoMovimentoImpostoAnulacaoComNotaAnulacaoEmbarcador.required(false);
        }

        _modeloDocumentoFiscal.TipoMovimentoImpostoEmissao.required(false);
        _modeloDocumentoFiscal.TipoMovimentoImpostoCancelamento.required(false);
        _modeloDocumentoFiscal.TipoMovimentoImpostoAnulacao.required(false);
    }
}

function DiferenciarMovimentosParaPISChange(novoValor) {
    if (novoValor == null)
        novoValor = _modeloDocumentoFiscal.DiferenciarMovimentosParaPIS.val();

    if (novoValor)
        _modeloDocumentoFiscal.DiferenciarMovimentosParaPIS.visibleFade(true);
    else
        _modeloDocumentoFiscal.DiferenciarMovimentosParaPIS.visibleFade(false);

    if (novoValor && _modeloDocumentoFiscal.GerarMovimentoAutomatico.val()) {
        if (_modeloDocumentoFiscal.Numero.val() == 57) {
            _modeloDocumentoFiscal.TipoMovimentoPISAnulacaoComNotaAnulacaoEmbarcador.visible(true);
            _modeloDocumentoFiscal.TipoMovimentoPISAnulacaoComNotaAnulacaoEmbarcador.required(true);
        } else {
            _modeloDocumentoFiscal.TipoMovimentoPISAnulacaoComNotaAnulacaoEmbarcador.visible(false);
            _modeloDocumentoFiscal.TipoMovimentoPISAnulacaoComNotaAnulacaoEmbarcador.required(false);
        }

        _modeloDocumentoFiscal.TipoMovimentoPISEmissao.required(true);
        _modeloDocumentoFiscal.TipoMovimentoPISCancelamento.required(true);
        _modeloDocumentoFiscal.TipoMovimentoPISAnulacao.required(true);
    } else {
        if (_modeloDocumentoFiscal.Numero.val() == 57) {
            _modeloDocumentoFiscal.TipoMovimentoPISAnulacaoComNotaAnulacaoEmbarcador.visible(true);
            _modeloDocumentoFiscal.TipoMovimentoPISAnulacaoComNotaAnulacaoEmbarcador.required(false);
        } else {
            _modeloDocumentoFiscal.TipoMovimentoPISAnulacaoComNotaAnulacaoEmbarcador.visible(false);
            _modeloDocumentoFiscal.TipoMovimentoPISAnulacaoComNotaAnulacaoEmbarcador.required(false);
        }

        _modeloDocumentoFiscal.TipoMovimentoPISEmissao.required(false);
        _modeloDocumentoFiscal.TipoMovimentoPISCancelamento.required(false);
        _modeloDocumentoFiscal.TipoMovimentoPISAnulacao.required(false);
    }
}

function DiferenciarMovimentosParaCOFINSChange(novoValor) {
    if (novoValor == null)
        novoValor = _modeloDocumentoFiscal.DiferenciarMovimentosParaCOFINS.val();

    if (novoValor)
        _modeloDocumentoFiscal.DiferenciarMovimentosParaCOFINS.visibleFade(true);
    else
        _modeloDocumentoFiscal.DiferenciarMovimentosParaCOFINS.visibleFade(false);

    if (novoValor && _modeloDocumentoFiscal.GerarMovimentoAutomatico.val()) {
        if (_modeloDocumentoFiscal.Numero.val() == 57) {
            _modeloDocumentoFiscal.TipoMovimentoCOFINSAnulacaoComNotaAnulacaoEmbarcador.visible(true);
            _modeloDocumentoFiscal.TipoMovimentoCOFINSAnulacaoComNotaAnulacaoEmbarcador.required(true);
        } else {
            _modeloDocumentoFiscal.TipoMovimentoCOFINSAnulacaoComNotaAnulacaoEmbarcador.visible(false);
            _modeloDocumentoFiscal.TipoMovimentoCOFINSAnulacaoComNotaAnulacaoEmbarcador.required(false);
        }

        _modeloDocumentoFiscal.TipoMovimentoCOFINSEmissao.required(true);
        _modeloDocumentoFiscal.TipoMovimentoCOFINSCancelamento.required(true);
        _modeloDocumentoFiscal.TipoMovimentoCOFINSAnulacao.required(true);
    } else {
        if (_modeloDocumentoFiscal.Numero.val() == 57) {
            _modeloDocumentoFiscal.TipoMovimentoCOFINSAnulacaoComNotaAnulacaoEmbarcador.visible(true);
            _modeloDocumentoFiscal.TipoMovimentoCOFINSAnulacaoComNotaAnulacaoEmbarcador.required(false);
        } else {
            _modeloDocumentoFiscal.TipoMovimentoCOFINSAnulacaoComNotaAnulacaoEmbarcador.visible(false);
            _modeloDocumentoFiscal.TipoMovimentoCOFINSAnulacaoComNotaAnulacaoEmbarcador.required(false);
        }

        _modeloDocumentoFiscal.TipoMovimentoCOFINSEmissao.required(false);
        _modeloDocumentoFiscal.TipoMovimentoCOFINSCancelamento.required(false);
        _modeloDocumentoFiscal.TipoMovimentoCOFINSAnulacao.required(false);
    }
}

function DiferenciarMovimentosParaIRChange(novoValor) {
    if (novoValor == null)
        novoValor = _modeloDocumentoFiscal.DiferenciarMovimentosParaIR.val();

    if (novoValor)
        _modeloDocumentoFiscal.DiferenciarMovimentosParaIR.visibleFade(true);
    else
        _modeloDocumentoFiscal.DiferenciarMovimentosParaIR.visibleFade(false);

    if (novoValor && _modeloDocumentoFiscal.GerarMovimentoAutomatico.val()) {
        if (_modeloDocumentoFiscal.Numero.val() == 57) {
            _modeloDocumentoFiscal.TipoMovimentoIRAnulacaoComNotaAnulacaoEmbarcador.visible(true);
            _modeloDocumentoFiscal.TipoMovimentoIRAnulacaoComNotaAnulacaoEmbarcador.required(true);
        } else {
            _modeloDocumentoFiscal.TipoMovimentoIRAnulacaoComNotaAnulacaoEmbarcador.visible(false);
            _modeloDocumentoFiscal.TipoMovimentoIRAnulacaoComNotaAnulacaoEmbarcador.required(false);
        }

        _modeloDocumentoFiscal.TipoMovimentoIREmissao.required(true);
        _modeloDocumentoFiscal.TipoMovimentoIRCancelamento.required(true);
        _modeloDocumentoFiscal.TipoMovimentoIRAnulacao.required(true);
    } else {
        if (_modeloDocumentoFiscal.Numero.val() == 57) {
            _modeloDocumentoFiscal.TipoMovimentoIRAnulacaoComNotaAnulacaoEmbarcador.visible(true);
            _modeloDocumentoFiscal.TipoMovimentoIRAnulacaoComNotaAnulacaoEmbarcador.required(false);
        } else {
            _modeloDocumentoFiscal.TipoMovimentoIRAnulacaoComNotaAnulacaoEmbarcador.visible(false);
            _modeloDocumentoFiscal.TipoMovimentoIRAnulacaoComNotaAnulacaoEmbarcador.required(false);
        }

        _modeloDocumentoFiscal.TipoMovimentoIREmissao.required(false);
        _modeloDocumentoFiscal.TipoMovimentoIRCancelamento.required(false);
        _modeloDocumentoFiscal.TipoMovimentoIRAnulacao.required(false);
    }
}

function DiferenciarMovimentosParaCSLLChange(novoValor) {
    if (novoValor == null)
        novoValor = _modeloDocumentoFiscal.DiferenciarMovimentosParaCSLL.val();

    if (novoValor)
        _modeloDocumentoFiscal.DiferenciarMovimentosParaCSLL.visibleFade(true);
    else
        _modeloDocumentoFiscal.DiferenciarMovimentosParaCSLL.visibleFade(false);

    if (novoValor && _modeloDocumentoFiscal.GerarMovimentoAutomatico.val()) {
        if (_modeloDocumentoFiscal.Numero.val() == 57) {
            _modeloDocumentoFiscal.TipoMovimentoCSLLAnulacaoComNotaAnulacaoEmbarcador.visible(true);
            _modeloDocumentoFiscal.TipoMovimentoCSLLAnulacaoComNotaAnulacaoEmbarcador.required(true);
        } else {
            _modeloDocumentoFiscal.TipoMovimentoCSLLAnulacaoComNotaAnulacaoEmbarcador.visible(false);
            _modeloDocumentoFiscal.TipoMovimentoCSLLAnulacaoComNotaAnulacaoEmbarcador.required(false);
        }

        _modeloDocumentoFiscal.TipoMovimentoCSLLEmissao.required(true);
        _modeloDocumentoFiscal.TipoMovimentoCSLLCancelamento.required(true);
        _modeloDocumentoFiscal.TipoMovimentoCSLLAnulacao.required(true);
    } else {
        if (_modeloDocumentoFiscal.Numero.val() == 57) {
            _modeloDocumentoFiscal.TipoMovimentoCSLLAnulacaoComNotaAnulacaoEmbarcador.visible(true);
            _modeloDocumentoFiscal.TipoMovimentoCSLLAnulacaoComNotaAnulacaoEmbarcador.required(false);
        } else {
            _modeloDocumentoFiscal.TipoMovimentoCSLLAnulacaoComNotaAnulacaoEmbarcador.visible(false);
            _modeloDocumentoFiscal.TipoMovimentoCSLLAnulacaoComNotaAnulacaoEmbarcador.required(false);
        }

        _modeloDocumentoFiscal.TipoMovimentoCSLLEmissao.required(false);
        _modeloDocumentoFiscal.TipoMovimentoCSLLCancelamento.required(false);
        _modeloDocumentoFiscal.TipoMovimentoCSLLAnulacao.required(false);
    }
}

function DiferenciarMovimentosParaValorLiquidoChange(novoValor) {
    if (novoValor == null)
        novoValor = _modeloDocumentoFiscal.DiferenciarMovimentosParaValorLiquido.val();

    if (novoValor)
        _modeloDocumentoFiscal.DiferenciarMovimentosParaValorLiquido.visibleFade(true);
    else
        _modeloDocumentoFiscal.DiferenciarMovimentosParaValorLiquido.visibleFade(false);

    if (novoValor && _modeloDocumentoFiscal.GerarMovimentoAutomatico.val()) {
        if (_modeloDocumentoFiscal.Numero.val() == 57) {
            _modeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacaoComNotaAnulacaoEmbarcador.visible(true);
            _modeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacaoComNotaAnulacaoEmbarcador.required(true);
        } else {
            _modeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacaoComNotaAnulacaoEmbarcador.visible(false);
            _modeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacaoComNotaAnulacaoEmbarcador.required(false);
        }

        _modeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissao.required(true);
        _modeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamento.required(true);
        _modeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacao.required(true);
    } else {
        if (_modeloDocumentoFiscal.Numero.val() == 57) {
            _modeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacaoComNotaAnulacaoEmbarcador.visible(true);
            _modeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacaoComNotaAnulacaoEmbarcador.required(false);
        } else {
            _modeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacaoComNotaAnulacaoEmbarcador.visible(false);
            _modeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacaoComNotaAnulacaoEmbarcador.required(false);
        }

        _modeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissao.required(false);
        _modeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamento.required(false);
        _modeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacao.required(false);
    }
}

function adicionarClick(e, sender) {
    PreecherLista();
    Salvar(_modeloDocumentoFiscal, "ModeloDocumentoFiscal/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso!");
                _gridModeloDocumentoFiscal.CarregarGrid();
                limparCamposModeloDocumentoFiscal();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    PreecherLista();
    Salvar(_modeloDocumentoFiscal, "ModeloDocumentoFiscal/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso!");
                _gridModeloDocumentoFiscal.CarregarGrid();
                limparCamposModeloDocumentoFiscal();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir o modelo de documento fiscal " + _modeloDocumentoFiscal.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_modeloDocumentoFiscal, "ModeloDocumentoFiscal/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso!");
                _gridModeloDocumentoFiscal.CarregarGrid();
                limparCamposModeloDocumentoFiscal();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposModeloDocumentoFiscal();
}

//*******MÉTODOS*******

function PreecherLista() {
    _modeloDocumentoFiscal.ListaSeries.val(JSON.stringify(_modeloDocumentoFiscal.Series.list));
}


function buscarModeloDocumentoFiscals() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarModeloDocumentoFiscal, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridModeloDocumentoFiscal = new GridView(_pesquisaModeloDocumentoFiscal.Pesquisar.idGrid, "ModeloDocumentoFiscal/Pesquisa", _pesquisaModeloDocumentoFiscal, menuOpcoes, null);
    _gridModeloDocumentoFiscal.CarregarGrid();
}

function editarModeloDocumentoFiscal(modeloDocumentoFiscalGrid) {
    limparCamposModeloDocumentoFiscal();
    _modeloDocumentoFiscal.Codigo.val(modeloDocumentoFiscalGrid.Codigo);
    BuscarPorCodigo(_modeloDocumentoFiscal, "ModeloDocumentoFiscal/BuscarPorCodigo", function (arg) {

        if (!_modeloDocumentoFiscal.Editavel.val()) {
            _modeloDocumentoFiscal.Abreviacao.required(false);
            $("#liTabSerie").addClass("d-none");
            //$("#liTabSerie").show();
        } else {
            $("#liTabSerie").removeClass("d-none");
            //$("#liTabSerie").hide();
        }

        _modeloDocumentoFiscal.Series.list = new Array();
        if (arg.Data.Series != null)
            _modeloDocumentoFiscal.Series.list = arg.Data.Series;
        RecarregarGridSerie();

        _pesquisaModeloDocumentoFiscal.ExibirFiltros.visibleFade(false);
        _crudModeloDocumentoFiscal.Atualizar.visible(true);
        _crudModeloDocumentoFiscal.Cancelar.visible(true);
        _crudModeloDocumentoFiscal.Excluir.visible(true);
        _crudModeloDocumentoFiscal.Adicionar.visible(false);
    }, null);
}

function limparCamposModeloDocumentoFiscal() {
    _crudModeloDocumentoFiscal.Atualizar.visible(false);
    _crudModeloDocumentoFiscal.Cancelar.visible(false);
    _crudModeloDocumentoFiscal.Excluir.visible(false);
    _crudModeloDocumentoFiscal.Adicionar.visible(true);
    _modeloDocumentoFiscal.Abreviacao.required(true);

    LimparCampos(_serieModeloDocumentoFiscal);
    _modeloDocumentoFiscal.Series.list = new Array();
    RecarregarGridSerie();

    LimparCampos(_modeloDocumentoFiscal);
    $("#liTabSerie").removeClass("d-none");
    //$("#liTabSerie").hide();
    //resetarAbas();
    //$(".nav-tabs a:first").tab("show");
}


function resetarAbas() {
    $(".nav-tabs").each(function () {
        $(this).find("a:first").tab("show");
    });
}