//*******MAPEAMENTO KNOUCKOUT*******

var _configuracaoFatura;

var ConfiguracaoFatura = function () {
    this.GerarMovimentoAutomatico = PropertyEntity({ getType: typesKnockout.bool, text: "Gerar movimento financeiro automático na geração da fatura:", val: ko.observable(false), def: false, visible: ko.observable(false), visibleFade: ko.observable(false) });
    this.TipoMovimentoUso = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso da Fatura:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão da Fatura:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });

    this.HabilitarGeracaoMovimentoFinanceiroPorModeloDocumento = PropertyEntity({ getType: typesKnockout.bool, text: "Habilitar geração de Movimento Financeiro por Modelo de Documento:", val: ko.observable(false), def: false, visible: ko.observable(false), visibleFade: ko.observable(false) });
    this.TipoMovimentoModeloDocumento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo Movimento:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.TipoModeloDocumentoFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Modelo:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.Adicionar = PropertyEntity({ eventClick: AdicionarHabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });

    this.HabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoReversao = PropertyEntity({ getType: typesKnockout.bool, text: "Habilitar geração de Movimento Financeiro de Reversão por Modelo de Documento:", val: ko.observable(false), def: false, visible: ko.observable(false), visibleFade: ko.observable(false) });
    this.TipoMovimentoModeloDocumentoReversao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo Movimento:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.TipoModeloDocumentoFiscalReversao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Modelo:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.AdicionarReversao = PropertyEntity({ eventClick: AdicionarHabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoReversaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });

    this.GridHabilitarGeracaoMovimentoFinanceiroPorModeloDocumento = PropertyEntity({ type: types.local });
    this.GridHabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoReversao = PropertyEntity({ type: types.local });

    this.GeracaoMovimentosFinanceirosPorModeloDocumento = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.GeracaoMovimentosFinanceirosPorModeloDocumentoReversao = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Salvar = PropertyEntity({ eventClick: SalvarConfiguracaoFaturaClick, type: types.event, text: "Salvar", icon: "fa fa-save", visible: ko.observable(true) });

    this.GerarMovimentoAutomatico.val.subscribe(function (novoValor) {
        GerarMovimentoAutomaticoFaturaChange(novoValor);
    });

    this.HabilitarGeracaoMovimentoFinanceiroPorModeloDocumento.val.subscribe(function (novoValor) {
        HabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoChange(novoValor);
    });

    this.HabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoReversao.val.subscribe(function (novoValor) {
        HabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoReversaoChange(novoValor);
    });
}

//*******EVENTOS*******

function LoadConfiguracaoFatura() {

    _configuracaoFatura = new ConfiguracaoFatura();
    KoBindings(_configuracaoFatura, "divConfiguracaoFinanceiraFatura");

    new BuscarTipoMovimento(_configuracaoFatura.TipoMovimentoUso);
    new BuscarTipoMovimento(_configuracaoFatura.TipoMovimentoReversao);

    LoadGridsModeloTipoMovimento();
}

function SalvarConfiguracaoFaturaClick(e, sender) {
    Salvar(_configuracaoFatura, "ConfiguracaoFinanceira/SalvarConfiguracaoFatura", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Dados salvos com sucesso!");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function GerarMovimentoAutomaticoFaturaChange(novoValor) {
    _configuracaoFatura.GerarMovimentoAutomatico.visibleFade(novoValor);
    _configuracaoFatura.TipoMovimentoUso.required(novoValor);
    _configuracaoFatura.TipoMovimentoReversao.required(novoValor);
}

function HabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoChange(novoValor) {
    _configuracaoFatura.HabilitarGeracaoMovimentoFinanceiroPorModeloDocumento.visibleFade(novoValor);
}

function HabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoReversaoChange(novoValor) {
    _configuracaoFatura.HabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoReversao.visibleFade(novoValor);
}

//*******MÉTODOS*******

