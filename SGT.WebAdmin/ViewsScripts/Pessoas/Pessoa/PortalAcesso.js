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
/// <reference path="Pessoa.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Enumeradores/EnumTipoOficina.js" />
/// <reference path="../../Enumeradores/EnumFormaTitulo.js" />

var _portalAcesso;

//*******MAPEAMENTO KNOUCKOUT*******

var PortalAcesso = function () {
    this.AtivarAcessoPortal = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.AtivarAcessoAoPortal, issue: 0, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.DesabilitarCancelamentoAgendamentoColeta = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.DesabilitarCancelamentoAgendamentoColeta, issue: 0, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.CompartilharAcessoEntreGrupoPessoas = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.CompartilharAcessoEntrePessoasDoGrupoDePessoas, def: false, visible: ko.observable(false) });
    this.VisualizarApenasParaPedidoDesteTomador = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.VisualizarApenasParaPedidoDesteTomador, def: false, visible: ko.observable(false) });
    this.VisualizarPedidosApenasAlgunsDeterminadosGruposDePessoas = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.VisualizarPedidosApenasAlgunsDeterminadosGruposDePessoas, def: false, visible: ko.observable(false) });
    this.VisualizarPedidosApenasAlgunsDeterminadosGruposDePessoas.val.subscribe((novoValor) => {
        if (_grupoPessoas.CadastrarGrupoPessoas == null)
            return;
        _grupoPessoas.CadastrarGrupoPessoas.visible(novoValor);
    });

    this.Usuario = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.UsuarioParaAcesso.getFieldDescription(), required: false, getType: typesKnockout.string, maxlength: 20, enable: ko.observable(false), visible: ko.observable(true), cssClass: ko.observable("col col-4"), cssGroupClass: ko.observable("") });
    this.Senha = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.SenhaParaAcesso.getFieldDescription(), required: false, getType: typesKnockout.string, maxlength: 15, visible: ko.observable(true) });
    this.ConfirmaSenha = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ConfirmaSenhaParaAcesso.getFieldDescription(), required: false, getType: typesKnockout.string, maxlength: 15, visible: ko.observable(true) });
    this.CodigoUsuario = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(true) });
    this.RedefinirSenha = PropertyEntity({ eventClick: redefinirSenhaClick, type: types.event, text: Localization.Resources.Pessoas.Pessoa.RedefinirSenha, visible: ko.observable(false) });

    this.EnviarInformacaoAcessoPorEmail = PropertyEntity({ eventClick: enviarInformacaoAcessoPorEmailClick, type: types.event, text: Localization.Resources.Pessoas.Pessoa.EnviarInformacaoPorEmail, visible: ko.observable(false) });

    this.HabilitarFornecedorParaLancamentoOrdemServico = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.HabilitarFornecedorParaLancamentoOrdemServico, val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.Vendedor = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Vendedor.getFieldDescription(), required: false, getType: typesKnockout.string, maxlength: 15, visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadPortalAcesso() {
    _portalAcesso = new PortalAcesso();
    KoBindings(_portalAcesso, "knockoutAcessoPortal");

    _pessoa.GrupoPessoas.codEntity.subscribe(function (val) {
        _portalAcesso.CompartilharAcessoEntreGrupoPessoas.visible(val > 0);
    });

    ObterPoliticaSenha();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _portalAcesso.VisualizarApenasParaPedidoDesteTomador.visible(true);
    }
}

function ObterPoliticaSenha() {
    executarReST("PoliticaSenha/BuscarPoliticaSenhaPorServicoMultiSoftware", { TipoServicoPoliticaSenha: EnumTipoServicoMultisoftware.Fornecedor }, function (arg) {
        if (arg.Success) {
            _politicaSenha = arg.Data;

            if (_politicaSenha.ExigirTrocaSenhaPrimeiroAcesso) {
                _portalAcesso.Senha.visible(false);
                _portalAcesso.Senha.required = false;
                _portalAcesso.ConfirmaSenha.visible(false);

                _portalAcesso.Usuario.cssClass("col col-6");
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function preencherUsuarioPortalAcessoAutomatico() {
    if (_pessoa.TipoPessoa.val() == EnumTipoPessoa.Exterior)
        _portalAcesso.Usuario.enable(true);
    else
        _portalAcesso.Usuario.enable(false);

    if (_pessoa.UsuarioPortal.val() != "") {
        _portalAcesso.Usuario.val(_pessoa.UsuarioPortal.val());
        _portalAcesso.Senha.val("");
        _portalAcesso.ConfirmaSenha.val("");

        if (_politicaSenha.ExigirTrocaSenhaPrimeiroAcesso) {
            _portalAcesso.Usuario.cssGroupClass("input-group");
            _portalAcesso.RedefinirSenha.visible(true);
        }
    }
    else if (_pessoa.CNPJ.val() != "") {
        var login = _pessoa.CNPJ.val().replace(/[^0-9]+/g, '');
        var senha = login.substring(0, 5);
        _portalAcesso.Usuario.val(login);

        if (!_politicaSenha.HabilitarPoliticaSenha) {
            if (_portalAcesso.Senha.val() == "" && _pessoa.TipoPessoa.val() != EnumTipoPessoa.Exterior) {
                _portalAcesso.Senha.val(senha);
                _portalAcesso.ConfirmaSenha.val(senha);
            }
        }

        if (_politicaSenha.ExigirTrocaSenhaPrimeiroAcesso) {
            _portalAcesso.Usuario.cssGroupClass("input-group");
            _portalAcesso.RedefinirSenha.visible(true);
        }

    }
}

//*******MÉTODOS*******

function validarCamposPessoaPortalAcesso() {
    var tudoCerto = _portalAcesso.AtivarAcessoPortal.val() ? ValidarCamposObrigatorios(_portalAcesso) : true;
    if (!tudoCerto) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        $("#liPortalAcesso a").tab("show");
    }

    return tudoCerto;
}

function enviarInformacaoAcessoPorEmailClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pessoas.Pessoa.TemCertezaQueDesjaEnviarUmEmailComOsDadosDeAcesso, function () {
        executarReST("PessoaFornecedor/EnviarDadosAcesso", { Codigo: _pessoa.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Pessoas.Pessoa.EmailEnviadoComSucesso);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

function redefinirSenhaClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pessoas.Pessoa.RealmenteDesejaRedefinirSenhaDesteUsuarioLembrandoQueAposRedefinicaoDeSenhaUsuarioIraPerderSenhaAtualSeraObrigadoDefinarUmaNovaSenhaEmSeuProximoAcesso, function () {
        executarReST("Usuario/RedefinirSenha", { Codigo: _portalAcesso.CodigoUsuario.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Pessoas.Pessoa.SenhaRedefinidaComSucesso);
                }
                else
                    exibirMensagem("aviso", Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }, sender, exibirCamposObrigatorio);
    });
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function PreencherDadosAcessoPortal(dados) {
    PreencherObjetoKnout(_portalAcesso, { Data: dados });
    _portalAcesso.EnviarInformacaoAcessoPorEmail.visible(true);
}

function LimparCamposAcessoPortal() {
    LimparCampos(_portalAcesso);
    _portalAcesso.Usuario.enable(false);
    _portalAcesso.EnviarInformacaoAcessoPorEmail.visible(false);
}
