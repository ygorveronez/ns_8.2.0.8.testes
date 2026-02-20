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
/// <reference path="../../Enumeradores/EnumTipoPropostaMultimodal.js" />
/// <reference path="Permissoes.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPerfil;
var _perfil;
var _pesquisaPerfilAcesso;

var PesquisaPerfilAcesso = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Pessoas.PerfilAcesso.Descricao.getFieldDescription() });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Pessoas.PerfilAcesso.Situacao.getFieldDescription() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPerfil.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Pessoas.PerfilAcesso.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Pessoas.PerfilAcesso.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var PerfilAcesso = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Pessoas.PerfilAcesso.Descricao.getRequiredFieldDescription(), issue: 586, required: true });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Pessoas.PerfilAcesso.CodigoIntegracao.getFieldDescription() });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: Localization.Resources.Pessoas.PerfilAcesso.Situacao.getRequiredFieldDescription(), issue: 557, required: true });
    this.TiposPropostasMultimodal = PropertyEntity({ val: ko.observable([]), options: EnumTipoPropostaMultimodal.obterOpcoesPerfil(), def: [], getType: typesKnockout.selectMultiple, text: Localization.Resources.Pessoas.PerfilAcesso.TipoProposta.getFieldDescription(), visible: ko.observable(false) });
    this.PermiteFaturamentoPermissaoExclusiva = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.PerfilAcesso.EstePerfilAcessoPossibilitaFaturamento, def: false, enable: ko.observable(true), visible: ko.observable(false) });

    this.HoraInicialAcesso = PropertyEntity({ text: Localization.Resources.Pessoas.PerfilAcesso.HorarioInicialAcesso.getFieldDescription(), getType: typesKnockout.time });
    this.HoraFinalAcesso = PropertyEntity({
        text: Localization.Resources.Pessoas.PerfilAcesso.HorarioFinalAcesso.getFieldDescription(), getType: typesKnockout.time });

    this.PerfilAdministrador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Pessoas.PerfilAcesso.EstePerfilAcessoAdministrador, visible: ko.observable(true) });
    this.PermiteSalvarNovoRelatorio = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Pessoas.PerfilAcesso.EstePerfilPermissaoParaSalvarRelatorio, visible: ko.observable(true) });
    this.PermiteTornarRelatorioPadrao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Pessoas.PerfilAcesso.EstePerfilPermissaoTornarRelatorioPadrao, visible: ko.observable(true) });
    this.PermiteSalvarConfiguracoesRelatoriosParaTodos = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Pessoas.PerfilAcesso.EstePerfilPermissoesSalvarConfiguracoes, visible: ko.observable(true) });
    this.VisualizarTitulosPagamentoSalario = PropertyEntity({ text: Localization.Resources.Pessoas.PerfilAcesso.VisualizarTitulosPagamentoSalario, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.PermitirAbrirOcorrenciaAposPrazoSolicitacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Pessoas.PerfilAcesso.PermitirAbrirOcorrenciaDataSuperior, visible: ko.observable(true) });

    this.FormulariosPerfil = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0) });
    this.ModulosPerfil = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0) });
    this.FormulariosNSelecionados = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0) });
    this.ModulosNSelecionados = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0) });

    this.Turno = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.PerfilAcesso.Turno.getFieldDescription(), idBtnSearch: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Pessoas.PerfilAcesso.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Pessoas.PerfilAcesso.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Pessoas.PerfilAcesso.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Pessoas.PerfilAcesso.Cancelar, visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadPerfilAcesso() {
    _perfil = new PerfilAcesso();
    KoBindings(_perfil, "knockoutCadastroPerfil");

    _pesquisaPerfilAcesso = new PesquisaPerfilAcesso();
    KoBindings(_pesquisaPerfilAcesso, "knockoutPesquisaPerfil");

    HeaderAuditoria("PerfilAcesso", _perfil);

    new BuscarTurno(_perfil.Turno);

    buscarPerfis();
    buscarPaginas();
    loadOcorrencia();

    configurarLayoutPerfilAcessoPorTipoSistema();
}

function configurarLayoutPerfilAcessoPorTipoSistema() {

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
        _perfil.TiposPropostasMultimodal.visible(true);
        _perfil.PermiteFaturamentoPermissaoExclusiva.visible(true);
    }

    if (!_CONFIGURACAO_TMS.UsaPermissaoControladorRelatorios) {
        _perfil.PermiteSalvarNovoRelatorio.visible(false);
        _perfil.PermiteTornarRelatorioPadrao.visible(false);
        _perfil.PermiteSalvarConfiguracoesRelatoriosParaTodos.visible(false);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _perfil.VisualizarTitulosPagamentoSalario.visible(true);
    }
}

function adicionarClick(e, sender) {
    buscarPermissoesFormularios();
    preencherOcorrencia();
    Salvar(e, "PerfilAcesso/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Pessoas.PerfilAcesso.Sucesso, Localization.Resources.Pessoas.PerfilAcesso.PerfilAcessoCadastrado);
                _gridPerfil.CarregarGrid();
                limparCamposPerfil();
            } else {
                exibirMensagem("aviso", Localization.Resources.Pessoas.PerfilAcesso.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.PerfilAcesso.Falha, arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function atualizarClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Pessoas.PerfilAcesso.Confirmacao, Localization.Resources.Pessoas.PerfilAcesso.RealmenteDesejaAlterarPerfil, function () {
        buscarPermissoesFormularios();
        preencherOcorrencia();
        Salvar(e, "PerfilAcesso/Atualizar", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Pessoas.PerfilAcesso.Sucesso, Localization.Resources.Pessoas.PerfilAcesso.AtualizadoSucesso);
                _gridPerfil.CarregarGrid();
                limparCamposPerfil();
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.PerfilAcesso.Falha, arg.Msg);
            }
        }, sender, exibirCamposObrigatorio);
    });
}

function excluirClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Pessoas.PerfilAcesso.Confirmacao, Localization.Resources.Pessoas.PerfilAcesso.RealmenteDesejaExcluirPerfilAcesso.format(_perfil.Descricao.val()), function () {
        ExcluirPorCodigo(_perfil, "PerfilAcesso/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Pessoas.PerfilAcesso.Sucesso, Localization.Resources.Pessoas.PerfilAcesso.ExcluidoSucesso);
                    _gridPerfil.CarregarGrid();
                    limparCamposPerfil();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.PerfilAcesso.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.PerfilAcesso.Falha, arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    resetarTabs();
    limparCamposPerfil();
}

//*******MÉTODOS*******

function buscarPerfis() {
    var editar = { descricao: Localization.Resources.Pessoas.PerfilAcesso.Editar, id: "clasEditar", evento: "onclick", metodo: editarPerfil, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridPerfil = new GridView(_pesquisaPerfilAcesso.Pesquisar.idGrid, "PerfilAcesso/Pesquisa", _pesquisaPerfilAcesso, menuOpcoes, null);
    _gridPerfil.CarregarGrid();
}

function editarPerfil(itemGrid) {
    limparCamposPerfil();

    _perfil.Codigo.val(itemGrid.Codigo);
    BuscarPorCodigo(_perfil, "PerfilAcesso/BuscarPorCodigo", function (arg) {
        setarPermissoesModulosFormularios();
        _pesquisaPerfilAcesso.ExibirFiltros.visibleFade(false);
        if (arg.Data) {
            _ocorrencia.PermitirAbrirOcorrenciaAposPrazoSolicitacao.val(_perfil.PermitirAbrirOcorrenciaAposPrazoSolicitacao.val());
        }
        _perfil.Atualizar.visible(true);
        _perfil.Cancelar.visible(true);
        _perfil.Excluir.visible(true);
        _perfil.Adicionar.visible(false);
    }, null);
}

function limparCamposPerfil() {
    limparPermissoesModulosFormularios();
    limparCamposOcorrencia();
    _perfil.Atualizar.visible(false);
    _perfil.Cancelar.visible(false);
    _perfil.Excluir.visible(false);
    _perfil.Adicionar.visible(true);
    resetarTabs();
    LimparCampos(_perfil);
}

function exibirCamposObrigatorio() {
    resetarTabs();
    exibirMensagem("atencao", Localization.Resources.Pessoas.PerfilAcesso.CampoObrigatorio, Localization.Resources.Pessoas.PerfilAcesso.PorFavorInformeCamposObrigatorios);
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}

