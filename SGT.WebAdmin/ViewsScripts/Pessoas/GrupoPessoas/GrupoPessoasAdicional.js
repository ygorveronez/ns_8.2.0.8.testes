/// <reference path="../../Consultas/PedidoTipoPagamento.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFinanceira.js" />
/// <reference path="../../Enumeradores/EnumTipoImpressaoOrdemColetaExclusiva.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />
/// <reference path="../../../js/Global/PermissoesPersonalizadas.js" />
/// <reference path="GrupoPessoas.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _grupoPessoasAdicional;

var GrupoPessoasAdicional = function () {
    this.PedidoTipoPagamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.GrupoPessoas.CondicaoPedido.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    this.SituacaoFinanceira = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.SituacaoFinanceira.getFieldDescription(), val: ko.observable(EnumSituacaoFinanceira.Liberada), options: EnumSituacaoFinanceira.obterOpcoes(), def: EnumSituacaoFinanceira.Liberada, enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoImpressaoOrdemColetaExclusiva = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.TipoImpressaoOrdemColeta.getFieldDescription(), val: ko.observable(EnumTipoImpressaoOrdemColetaExclusiva.Retrato), options: EnumTipoImpressaoOrdemColetaExclusiva.obterOpcoes(), def: EnumTipoImpressaoOrdemColetaExclusiva.Retrato, enable: ko.observable(true) });
    this.ClassificacaoEmpresa = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.ClassificacaoEmpresa.getFieldDescription(), visible: ko.observable(false), enable: ko.observable(true) });

    this.ControlaPallets = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.ControlaPallets, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.CobrancaDiaria = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.CobrancaDiaria, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.CobrancaDescarga = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.CobrancaDescarga, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.CobrancaCarregamento = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.CobrancaCarregamento, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.EnviarAutomaticamenteDocumentacaoCarga = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.EnviarAutomaticamenteDocumentacaoCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.NaoGerarArquivoVgm = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.NaoGerarArquivoVGM, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal), enable: ko.observable(true) });
    this.NaoEnviarXMLCteSubcontratacaoOuRedespachoPorEmail = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.NaoEnviarXMLCteSubcontratacaoOuRedespachoPorEmail, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.GerarImpressaoOrdemColetaExclusiva = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.GerarImpressaoOrdemColetaExclusiva, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.NaoEnviarParaDocsys = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.NaoEnviarParaDocsys, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false), enable: ko.observable(true) });
    this.AdicionarDespachanteComoConsignatario = PropertyEntity({
        text: Localization.Resources.Pessoas.GrupoPessoas.AdicionarDespachanteConsignatario, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false), enable: ko.observable(true)
    });

    this.Despachante = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.GrupoPessoas.Despachante.getRequiredFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false), required: false });
    this.EmailDespachante = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.EmailDespachante.getRequiredFieldDescription(), required: false, visible: false, getType: typesKnockout.email });

    this.AdicionarDespachanteComoConsignatario.val.subscribe((novoValor) => {
        if (novoValor) {
            _grupoPessoasAdicional.Despachante.required = true;
            _grupoPessoasAdicional.EmailDespachante.required = true;
        } else {
            _grupoPessoasAdicional.Despachante.required = false;
            _grupoPessoasAdicional.EmailDespachante.required = false;
        }
    });

    this.CobrancaDiariaObservacao = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.Observacao.getFieldDescription(), maxlength: 300, enable: ko.observable(true) });
    this.CobrancaDescargaObservacao = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.Observacao.getFieldDescription(), maxlength: 300, enable: ko.observable(true) });
    this.CobrancaCarregamentoObservacao = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.Observacao.getFieldDescription(), maxlength: 300, enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadGrupoPessoasAdicional() {
    _grupoPessoasAdicional = new GrupoPessoasAdicional();
    KoBindings(_grupoPessoasAdicional, "knockoutAdicionais");

    new BuscarPedidoTipoPagamento(_grupoPessoasAdicional.PedidoTipoPagamento);
    new BuscarClientes(_grupoPessoasAdicional.Despachante);

    configurarLayoutAdicionalPorTipoSistema();

    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.GrupoPessoas_AdicionarDespachanteComoConsignatario, _PermissoesPersonalizadas)) {
        _grupoPessoasAdicional.AdicionarDespachanteComoConsignatario.visible(true);
    }
}

function configurarLayoutAdicionalPorTipoSistema() {
    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal)
        _grupoPessoasAdicional.NaoEnviarParaDocsys.visible(true);

    _grupoPessoasAdicional.SituacaoFinanceira.enable(true);
    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.GrupoPessoas_PermiteAlterarSituacaoFinanceira, _PermissoesPersonalizadas))
        _grupoPessoasAdicional.SituacaoFinanceira.enable(false);

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal)
        _grupoPessoasAdicional.ClassificacaoEmpresa.visible(true);
}

function limparCamposGrupoPessoasAdicional() {
    LimparCampos(_grupoPessoasAdicional);
}

function validarCamposObrigatoriosGrupoPessoasAdicional() {
    if (!ValidarCamposObrigatorios(_grupoPessoasAdicional)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.GrupoPessoas.Aviso, Localization.Resources.Pessoas.GrupoPessoas.NecessarioInformarCamposObrigatorios);
        return false;
    }
    return true;
}