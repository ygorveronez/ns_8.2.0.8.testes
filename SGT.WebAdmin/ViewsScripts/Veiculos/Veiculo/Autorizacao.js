//*******MAPEAMENTO KNOUCKOUT*******

var _cadastroVeiculoAutorizacao;
var _gridAutorizacoes;

var CadastroVeiculoAutorizacao = function () {
    this.Codigo = PropertyEntity({ visible: ko.observable(false) });
    this.Situacao = PropertyEntity({ visible: ko.observable(false) });
    this.DataCadastro = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.DataCadastro.getFieldDescription(), visible: ko.observable(true) });
    this.Tipo = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Tipo.getFieldDescription(), visible: ko.observable(true) });
    this.SituacaoDescricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), visible: ko.observable(true) });
    this.Solicitate = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.Solicitante.getFieldDescription(), visible: ko.observable(true) });
    this.MotivoRejeicao = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.Motivo.getFieldDescription(), visible: ko.observable(true) });

    this.Autorizacao = PropertyEntity({ visible: ko.observable(false) });
    this.Rejeitado = PropertyEntity({ visible: ko.observable(false) });

    this.UsuariosAutorizadores = PropertyEntity({ idGrid: guid(), visible: ko.observable(true) });

    this.MensagemEtapaSemRegra = PropertyEntity({ type: types.local, visible: ko.observable(false), title: Localization.Resources.Veiculos.Veiculo.AutorizacaoPendente, text: '<b class="margin-bottom-10" style="display:block">{Localize::Veiculos.Veiculo.NenhumaRegraCadastrada}</b><b>{Localize::Veiculos.Veiculo.CadastrosPermanecemAguardandoAutorizacaoCadastros}</b>' });
}

//*******EVENTOS*******

function loadCadastroVeiculoAutorizacao() {
    _cadastroVeiculoAutorizacao = new CadastroVeiculoAutorizacao();
    KoBindings(_cadastroVeiculoAutorizacao, "knockoutAprovacaoEtapa");

    _gridAutorizacoes = new GridView(_cadastroVeiculoAutorizacao.UsuariosAutorizadores.idGrid, "AutorizacaoCadastroVeiculo/ConsultarAutorizacoes", { Codigo: _cadastroVeiculoAutorizacao.Codigo });
}

//*******MÉTODOS*******

function preencherDadosAprovacao(data) {
    setarEtapasCadastroVeiculo();
    var dadosAutorizacao = data.DadosAutorizacao;

    _cadastroVeiculoAutorizacao.UsuariosAutorizadores.visible(false);
    _cadastroVeiculoAutorizacao.Autorizacao.visible(false);
    _cadastroVeiculoAutorizacao.MensagemEtapaSemRegra.visible(false);
    _cadastroVeiculoAutorizacao.Rejeitado.visible(false);
    _cadastroVeiculoAutorizacao.Situacao.visible(false);

    var data = { Data: dadosAutorizacao || {} };
    PreencherObjetoKnout(_cadastroVeiculoAutorizacao, data);

    if (dadosAutorizacao == null || dadosAutorizacao.Situacao == EnumSituacaoCadastroVeiculo.Aprovado) {
        _cadastroVeiculoAutorizacao.Autorizacao.visible(true);
    } else if (dadosAutorizacao.Situacao == EnumSituacaoCadastroVeiculo.Pendente) {
        _cadastroVeiculoAutorizacao.Situacao.visible(true);
        _cadastroVeiculoAutorizacao.UsuariosAutorizadores.visible(true);
    } else if (dadosAutorizacao.Situacao == EnumSituacaoCadastroVeiculo.Rejeitada) {
        _cadastroVeiculoAutorizacao.Rejeitado.visible(true);
    } else if (dadosAutorizacao.Situacao == EnumSituacaoCadastroVeiculo.SemRegraAprovacao) {
        _cadastroVeiculoAutorizacao.MensagemEtapaSemRegra.visible(true);
    } else if (dadosAutorizacao.Situacao == EnumSituacaoCadastroVeiculo.Todos) {
        _cadastroVeiculoAutorizacao.SituacaoDescricao.visible(false)
    }

    _gridAutorizacoes.CarregarGrid(function () {
        if (dadosAutorizacao != null && dadosAutorizacao.Situacao == EnumSituacaoCadastroVeiculo.Pendente && _gridAutorizacoes.NumeroRegistros() > 0) {
            _cadastroVeiculoAutorizacao.UsuariosAutorizadores.visible(true);
        }
    });
}

function IsAprovacaoCadastroAtiva() {
    return _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador && _CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoVeiculo;
}

function BloquearCamposCadastroVeiculo() {
    if (!IsAprovacaoCadastroAtiva())
        return;
    SetarEnableCamposKnockout(_veiculoVinculado, false);
    SetarEnableCamposKnockout(_veiculo, false);
    _veiculo.Atualizar.visible(false);
    _anexos.Adicionar.visible(false);
}

function DesbloquearCamposCadastroVeiculo() {
    SetarEnableCamposKnockout(_veiculoVinculado, true);
    SetarEnableCamposKnockout(_veiculo, true);
    _anexos.Adicionar.visible(true);
}