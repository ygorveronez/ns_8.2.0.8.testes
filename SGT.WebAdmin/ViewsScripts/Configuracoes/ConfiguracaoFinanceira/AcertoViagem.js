//*******MAPEAMENTO KNOUCKOUT*******

var _configuracaoAcertoViagem;

var ConfiguracaoAcertoViagem = function () {
    this.GerarMovimentoAutomaticoNoAcertoViagem = PropertyEntity({ getType: typesKnockout.bool, text: "Gerar movimento financeiro automático na geração do acerto de viagem:", val: ko.observable(false), def: false, visible: ko.observable(true) });


    this.TipoMovimentoAbastecimentoPagoPeloMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Valor Abastecimento Pago Pelo Motorista:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoAbastecimentoPagoPeloMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão Valor Abastecimento Pago Pelo Motorista:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoAbastecimentoPagoPelaEmpresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Valor Abastecimento Pago Pela Empresa:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoAbastecimentoPagoPelaEmpresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão Valor Abastecimento Pago Pela Empresa:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoPedagioRecebidoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Pedágio Recebido do Embarcador (Cartão):", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoPedagioRecebidoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão Pedágio Recebido do Embarcador (Cartão):", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoPedagioPagoPelaEmpresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Valor Pedágio Pago Pela Empresa:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoRevesaoPedagioPagoPelaEmpresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão Valor Pedágio Pago Pela Empres:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoPedagioPagoPeloMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Valor Pedágio Pago Pelo Motorista:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoPedagioPagoPeloMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão Valor Pedágio Pago Pelo Motorista:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoComissaoDoMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Valor Comissão do motorista:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoComissaoDoMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão Valor Comissão do motorista:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.ContaEntradaAdiantamentoMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Conta para Pagamento de Comissão do Motorista:", idBtnSearch: guid(), issue: 359, visible: ko.observable(true), required: ko.observable(false) });
    this.ContaEntradaComissaoMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Conta para Pagamento de Adiantamento ao Motorista:", idBtnSearch: guid(), issue: 359, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoPedagioRecebidoEmbarcadorCredito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Pedágio Recebido do Embarcador por Crédito:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoPedagioRecebidoEmbarcadorCredito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão Pedágio Recebido do Embarcador por Crédito:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoOutrasDespesas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Outras Despesas:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoOutrasDespesas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão Outras Despesas:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });

    this.Visible = this.ExibirFiltros = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Salvar = PropertyEntity({ eventClick: SalvarConfiguracaoAcertoViagemClick, type: types.event, text: "Salvar", icon: "fa fa-save", visible: ko.observable(true) });

    this.GerarMovimentoAutomaticoNoAcertoViagem.val.subscribe(function (novoValor) {
        GerarMovimentoAutomaticoNoAcertoViagemChange(novoValor);
    });
};

//*******EVENTOS*******

function LoadConfiguracaoAcertoViagem() {

    _configuracaoAcertoViagem = new ConfiguracaoAcertoViagem();
    KoBindings(_configuracaoAcertoViagem, "divConfiguracaoFinanceiraAcertoViagem");

    new BuscarTipoMovimento(_configuracaoAcertoViagem.TipoMovimentoAbastecimentoPagoPeloMotorista);
    new BuscarTipoMovimento(_configuracaoAcertoViagem.TipoMovimentoReversaoAbastecimentoPagoPeloMotorista);
    new BuscarTipoMovimento(_configuracaoAcertoViagem.TipoMovimentoAbastecimentoPagoPelaEmpresa);
    new BuscarTipoMovimento(_configuracaoAcertoViagem.TipoMovimentoReversaoAbastecimentoPagoPelaEmpresa);
    new BuscarTipoMovimento(_configuracaoAcertoViagem.TipoMovimentoPedagioRecebidoEmbarcador);
    new BuscarTipoMovimento(_configuracaoAcertoViagem.TipoMovimentoReversaoPedagioRecebidoEmbarcador);
    new BuscarTipoMovimento(_configuracaoAcertoViagem.TipoMovimentoPedagioPagoPelaEmpresa);
    new BuscarTipoMovimento(_configuracaoAcertoViagem.TipoMovimentoRevesaoPedagioPagoPelaEmpresa);
    new BuscarTipoMovimento(_configuracaoAcertoViagem.TipoMovimentoPedagioPagoPeloMotorista);
    new BuscarTipoMovimento(_configuracaoAcertoViagem.TipoMovimentoReversaoPedagioPagoPeloMotorista);
    new BuscarTipoMovimento(_configuracaoAcertoViagem.TipoMovimentoComissaoDoMotorista);
    new BuscarTipoMovimento(_configuracaoAcertoViagem.TipoMovimentoReversaoComissaoDoMotorista);
    new BuscarPlanoConta(_configuracaoAcertoViagem.ContaEntradaAdiantamentoMotorista, "Selecione a Conta Analítica", "Contas Analíticas", null, EnumAnaliticoSintetico.Analitico);
    new BuscarPlanoConta(_configuracaoAcertoViagem.ContaEntradaComissaoMotorista, "Selecione a Conta Analítica", "Contas Analíticas", null, EnumAnaliticoSintetico.Analitico);
    new BuscarTipoMovimento(_configuracaoAcertoViagem.TipoMovimentoPedagioRecebidoEmbarcadorCredito);
    new BuscarTipoMovimento(_configuracaoAcertoViagem.TipoMovimentoReversaoPedagioRecebidoEmbarcadorCredito);
    new BuscarTipoMovimento(_configuracaoAcertoViagem.TipoMovimentoOutrasDespesas);
    new BuscarTipoMovimento(_configuracaoAcertoViagem.TipoMovimentoReversaoOutrasDespesas);
}

function SalvarConfiguracaoAcertoViagemClick(e, sender) {
    Salvar(_configuracaoAcertoViagem, "ConfiguracaoFinanceira/SalvarConfiguracaoAcertoViagem", function (r) {
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

function GerarMovimentoAutomaticoNoAcertoViagemChange(novoValor) {
    if (novoValor)
        setarRequiredCampos(true);
    else
        setarRequiredCampos(false);
}

//*******MÉTODOS*******

function setarRequiredCampos(novoStatus) {
    _configuracaoAcertoViagem.Visible.visibleFade(novoStatus);
    _configuracaoAcertoViagem.TipoMovimentoAbastecimentoPagoPeloMotorista.required(novoStatus);
    _configuracaoAcertoViagem.TipoMovimentoReversaoAbastecimentoPagoPeloMotorista.required(novoStatus);
    _configuracaoAcertoViagem.TipoMovimentoAbastecimentoPagoPelaEmpresa.required(novoStatus);
    _configuracaoAcertoViagem.TipoMovimentoReversaoAbastecimentoPagoPelaEmpresa.required(novoStatus);
    _configuracaoAcertoViagem.TipoMovimentoPedagioRecebidoEmbarcador.required(novoStatus);
    _configuracaoAcertoViagem.TipoMovimentoReversaoPedagioRecebidoEmbarcador.required(novoStatus);
    _configuracaoAcertoViagem.TipoMovimentoPedagioPagoPelaEmpresa.required(novoStatus);
    _configuracaoAcertoViagem.TipoMovimentoRevesaoPedagioPagoPelaEmpresa.required(novoStatus);
    _configuracaoAcertoViagem.TipoMovimentoPedagioPagoPeloMotorista.required(novoStatus);
    _configuracaoAcertoViagem.TipoMovimentoReversaoPedagioPagoPeloMotorista.required(novoStatus);
    _configuracaoAcertoViagem.TipoMovimentoComissaoDoMotorista.required(novoStatus);
    _configuracaoAcertoViagem.TipoMovimentoReversaoComissaoDoMotorista.required(novoStatus);
    _configuracaoAcertoViagem.ContaEntradaAdiantamentoMotorista.required(novoStatus);
    _configuracaoAcertoViagem.ContaEntradaComissaoMotorista.required(novoStatus);
    _configuracaoAcertoViagem.TipoMovimentoPedagioRecebidoEmbarcadorCredito.required(novoStatus);
    _configuracaoAcertoViagem.TipoMovimentoReversaoPedagioRecebidoEmbarcadorCredito.required(novoStatus);
    _configuracaoAcertoViagem.TipoMovimentoOutrasDespesas.required(novoStatus);
    _configuracaoAcertoViagem.TipoMovimentoReversaoOutrasDespesas.required(novoStatus);
}