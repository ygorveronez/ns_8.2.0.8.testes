//*******MAPEAMENTO*******

var _cargaDadosEmissaoObservacao;

var CargaDadosEmissaoObservacao = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Pedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idTab: guid(), enable: ko.observable(true) });

    this.Observacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ObservacaoParaCte.getFieldDescription(), required: false, maxlength: 400, visible: ko.observable(true), enable: ko.observable(true) });
    this.ObservacaoTerceiro = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ObservacaoParaCteDeTerceiros.getFieldDescription(), required: false, maxlength: 400, visible: ko.observable(false), enable: ko.observable(true) });

    this.Atualizar = PropertyEntity({ eventClick: atualizarCargaDadosEmissaoObservacao, type: types.event, text: Localization.Resources.Cargas.Carga.AtualizarObservacao, visible: ko.observable(true), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadCargaDadosEmissaoObservacao(callback) {
    _cargaDadosEmissaoObservacao = new CargaDadosEmissaoObservacao();

    var configuracaoObservacaoCTe = new ConfiguracaoObservacaoCTe();

    var parametrosConfiguracaoObservacaoCTe = {
        Knouckout: _cargaDadosEmissaoObservacao,
        KnoutObservacao: _cargaDadosEmissaoObservacao.Observacao,
        KnoutObservacaoTerceiro: _cargaDadosEmissaoObservacao.ObservacaoTerceiro,
        IdContainerObservacao: "divContainerObservacao_" + _cargaAtual.DadosEmissaoFrete.id,
        IdContainerObservacaoTerceiro: "divContainerObservacaoTerceiro_" + _cargaAtual.DadosEmissaoFrete.id,
        KnoutEnable: _cargaAtual.EtapaFreteEmbarcador.enable
    };

    configuracaoObservacaoCTe.Load(parametrosConfiguracaoObservacaoCTe).then(function () {
        KoBindings(_cargaDadosEmissaoObservacao, "tabObservacoes_" + _cargaAtual.DadosEmissaoFrete.id);

        $("#tabObservacoes_" + _cargaAtual.DadosEmissaoFrete.id + "_li").show();

        _cargaDadosEmissaoObservacao.Pedido.enable(_cargaAtual.EtapaFreteEmbarcador.enable());
        _cargaDadosEmissaoObservacao.Atualizar.enable(_cargaDadosEmissaoObservacao.Pedido.enable());
        _cargaDadosEmissaoObservacao.Observacao.enable(_cargaDadosEmissaoObservacao.Pedido.enable());
        _cargaDadosEmissaoObservacao.ObservacaoTerceiro.enable(_cargaDadosEmissaoObservacao.Pedido.enable());

        if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AlterarObservacao, _PermissoesPersonalizadasCarga) && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe)
            _cargaDadosEmissaoObservacao.Atualizar.enable(false);

        if (_cargaAtual.FreteDeTerceiro.val() === true)
            _cargaDadosEmissaoObservacao.ObservacaoTerceiro.visible(true);
    });
}

function preencherCargaDadosEmissaoObservacao(dados) {
    if (_cargaDadosEmissaoObservacao != null)
        PreencherObjetoKnout(_cargaDadosEmissaoObservacao, dados);
}

function atualizarCargaDadosEmissaoObservacao(e, sender) {
    e.Carga.val(_cargaAtual.Codigo.val());
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteAtualizarObservacaoParaOsCtesDaCarga, function () {
        Salvar(e, "DadosEmissaoObservacao/AtualizarObservacao", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ObservacaoAtualizadaComSucesso);
                    obterDadosEmissaoGeralCarga();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}