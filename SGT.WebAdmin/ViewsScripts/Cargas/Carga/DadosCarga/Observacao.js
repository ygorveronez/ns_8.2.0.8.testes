//*******MAPEAMENTO*******

var _cargaDadosEmissaoObservacaoDadosCarga, _HTMLCargaDadosTransporteObservacao, _infoObservacao;

var CargaDadosEmissaoObservacaoEtapaUm = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Pedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idTab: guid(), enable: ko.observable(true) });

    this.Observacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ObservacaoParaCte.getFieldDescription(), required: false, maxlength: 400, visible: ko.observable(true), enable: ko.observable(true) });
    this.ObservacaoTerceiro = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ObservacaoParaCteDeTerceiros.getFieldDescription(), required: false, maxlength: 400, visible: ko.observable(false), enable: ko.observable(true) });

    this.AtualizarObservacaoCargaEtapaUm = PropertyEntity({ eventClick: atualizarCargaDadosEmissaoObservacaoDadosCarga, type: types.event, text: Localization.Resources.Cargas.Carga.AtualizarObservacao, visible: ko.observable(true), enable: ko.observable(true) });
};

var ObservacaoCarregamento = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Observacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ObservacaoParaCte.getFieldDescription(), required: false, maxlength: 2000, visible: ko.observable(true), enable: ko.observable(true), val: ko.observable("") });

    this.Cancelar = PropertyEntity({ eventClick: cancelarAlterarObservacaoClick, type: types.event, visible: ko.observable(true), text: Localization.Resources.Gerais.Geral.Cancelar });
    this.Salvar = PropertyEntity({ eventClick: salvarAlteracaoObservacaoClick, type: types.event, visible: ko.observable(true), text: Localization.Resources.Gerais.Geral.Salvar });
};

//*******EVENTOS*******

function loadCargaDadosEmissaoObservacaoDadosCarga(knoutCarga) {

    CarregarHTMLCargaDadosTransporteObservacao().then(function () {

        SetarHTMLCargaDadosTransporteObservacao("tabCargaDadosTransporteObservacao_" + knoutCarga.EtapaInicioTMS.idGrid, knoutCarga.DivCarga.id);

        _cargaDadosEmissaoObservacaoDadosCarga = new CargaDadosEmissaoObservacaoEtapaUm();

        let configuracaoObservacaoCTe = new ConfiguracaoObservacaoCTe();

        let parametrosConfiguracaoObservacaoCTe = {
            Knouckout: _cargaDadosEmissaoObservacaoDadosCarga,
            KnoutObservacao: _cargaDadosEmissaoObservacaoDadosCarga.Observacao,
            KnoutObservacaoTerceiro: _cargaDadosEmissaoObservacaoDadosCarga.ObservacaoTerceiro,
            IdContainerObservacao: "divContainerObservacaoCargaEtapaUmDadosTransporteAlteracao_" + knoutCarga.DivCarga.id,
            IdContainerObservacaoTerceiro: "divContainerObservacaoTerceiroCargaEtapaUmDadosTransporte_" + knoutCarga.DivCarga.id,
            KnoutEnable: knoutCarga.EtapaFreteEmbarcador.enable
        };

        configuracaoObservacaoCTe.Load(parametrosConfiguracaoObservacaoCTe).then(function () {
            KoBindings(_cargaDadosEmissaoObservacaoDadosCarga, "divContainerObservacaoCargaEtapaUmDadosTransporte_" + knoutCarga.DivCarga.id);
            LocalizeCurrentPage();

            obterCargaDadosEmissaoObservacaoDadosCarga();

            $("#litabCargaDadosTransporteObservacao_" + knoutCarga.EtapaInicioTMS.idGrid).removeClass("d-none");

            _cargaDadosEmissaoObservacaoDadosCarga.Pedido.enable(_cargaAtual.EtapaFreteEmbarcador.enable());
            _cargaDadosEmissaoObservacaoDadosCarga.AtualizarObservacaoCargaEtapaUm.enable(_cargaDadosEmissaoObservacaoDadosCarga.Pedido.enable());
            _cargaDadosEmissaoObservacaoDadosCarga.Observacao.enable(_cargaDadosEmissaoObservacaoDadosCarga.Pedido.enable());
            _cargaDadosEmissaoObservacaoDadosCarga.ObservacaoTerceiro.enable(_cargaDadosEmissaoObservacaoDadosCarga.Pedido.enable());

            if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AlterarObservacao, _PermissoesPersonalizadasCarga) && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe)
                _cargaDadosEmissaoObservacaoDadosCarga.Atualizar.enable(false);

            if (knoutCarga.FreteDeTerceiro.val() === true)
                _cargaDadosEmissaoObservacaoDadosCarga.ObservacaoTerceiro.visible(true);
        });
    });
}

function CarregarHTMLCargaDadosTransporteObservacao() {
    var p = new promise.Promise();

    if (_HTMLCargaDadosTransporteObservacao == null) {
        $.get("Content/Static/Carga/CargaDadosEmissaoObservacao.html?dyn=" + guid(), function (data) {
            _HTMLCargaDadosTransporteObservacao = data;
            p.done();
        });
    } else {
        p.done();
    }

    return p;
}

function SetarHTMLCargaDadosTransporteObservacao(idContent, idReplace) {
    $("#" + idContent).html(_HTMLCargaDadosTransporteObservacao.replace(/#idDivCarga/g, idReplace));
}

function preencherCargaDadosEmissaoObservacaoDadosCarga(dados) {
    if (_cargaDadosEmissaoObservacaoDadosCarga != null)
        PreencherObjetoKnout(_cargaDadosEmissaoObservacaoDadosCarga, dados);
}

function atualizarCargaDadosEmissaoObservacaoDadosCarga(e, sender) {
    e.Carga.val(_cargaAtual.Codigo.val());
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteAtualizarObservacaoParaOsCtesDaCarga, function () {
        Salvar(e, "DadosEmissaoObservacao/AtualizarObservacao", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ObservacaoAtualizadaComSucesso);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function obterCargaDadosEmissaoObservacaoDadosCarga(callback) {
    executarReST("DadosEmissaoObservacao/ObterInformacoesCargaDadosEmissaoObservacaoDadosCarga", { Carga: _cargaAtual.Codigo.val() }, function (r) {
        if (r.Success && r.Data) {
            preencherCargaDadosEmissaoObservacaoDadosCarga(r);
            if (callback)
                callback();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function alterarObservacaoCarregamentoClick(e, sender) {
    _infoObservacao = new ObservacaoCarregamento();

    _infoObservacao.Observacao.val(e.ObservacaoCarregamento.val());
    _infoObservacao.Carga.val(e.Codigo.val());

    KoBindings(_infoObservacao, "knockoutAlterarObservacao");
    Global.abrirModal("divModalAlterarObservacaoCarregamento");
}

function salvarAlteracaoObservacaoClick() {
    executarReST("Carga/AlterarObservacaoCarregamento", { Carga: _infoObservacao.Carga.val(), Observacao: _infoObservacao.Observacao.val() }, function (r) {
        if (r.Success) {
            _listaKnoutsCarga.forEach(carga => {
                if (carga.Codigo.val() == _infoObservacao.Carga.val())
                    carga.ObservacaoCarregamento.val(_infoObservacao.Observacao.val());
            })

            limparCamposAlterarObservacaoCarregamento();
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ObservacaoAtualizadaComSucesso);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function cancelarAlterarObservacaoClick() {
    limparCamposAlterarObservacaoCarregamento();
}

function limparCamposAlterarObservacaoCarregamento() {
    LimparCampos(_infoObservacao);
    Global.fecharModal("divModalAlterarObservacaoCarregamento");
}