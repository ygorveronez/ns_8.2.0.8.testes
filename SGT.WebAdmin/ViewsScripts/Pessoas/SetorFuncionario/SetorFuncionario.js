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
/// <reference path="../../Enumeradores/EnumTipoCargoFuncionario.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _setorFuncionario;
var _pesquisaSetorFuncionario;
var _gridSetorFuncionario;
var _CRUDSetorFuncionario;

var SetorFuncionario = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: Localization.Resources.Pessoas.SetorFuncionario.Descricao.getRequiredFieldDescription(), required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: Localization.Resources.Pessoas.SetorFuncionario.Status.getFieldDescription(), val: ko.observable(true), options: _status, def: true });
    this.TipoCargoFuncionario = PropertyEntity({ val: ko.observable([]), options: EnumTipoCargoFuncionario.obterOpcoes(), def: [], getType: typesKnockout.selectMultiple, text: Localization.Resources.Pessoas.SetorFuncionario.CargoFuncionario.getFieldDescription(), required: false, enable: ko.observable(true) });
    this.TipoGrot = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Pessoas.SetorFuncionario.TipoGROT, idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    this.PermitirAssumirChamadosDoMesmoSetor = PropertyEntity({ text: Localization.Resources.Pessoas.SetorFuncionario.PermitirUsuarioSetorAssumam, val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.PermitirCancelarAtendimento = PropertyEntity({ text: Localization.Resources.Pessoas.SetorFuncionario.PermitrQualquerUsuarioPossaCancelar, val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.TipoSetorFuncionario = PropertyEntity({ val: ko.observable(EnumTipoSetorFuncionario.NaoInformado), options: EnumTipoSetorFuncionario.obterOpcoes(), def: EnumTipoSetorFuncionario.NaoInformado, text: Localization.Resources.Pessoas.SetorFuncionario.TipoSetor.getFieldDescription() });
    this.NotificarCenarioPosEntregaImprocedenteGestaoDevolucao = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.NotificarCenarioPosEntregaImprocedenteGestaoDevolucao, val: ko.observable(false), getType: typesKnockout.bool, def: false });
};

var PesquisaSetorFuncionario = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Pessoas.SetorFuncionario.Descricao.getFieldDescription(), required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: Localization.Resources.Pessoas.SetorFuncionario.Status.getFieldDescription(), val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridSetorFuncionario.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Pessoas.SetorFuncionario.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Pessoas.SetorFuncionario.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var CRUDSetorFuncionario = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Pessoas.SetorFuncionario.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Pessoas.SetorFuncionario.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Pessoas.SetorFuncionario.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Pessoas.SetorFuncionario.Cancelar, visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadSetorFuncionario() {
    _pesquisaSetorFuncionario = new PesquisaSetorFuncionario();
    KoBindings(_pesquisaSetorFuncionario, "knockoutPesquisaSetorFuncionario", false, _pesquisaSetorFuncionario.Pesquisar.id);

    _setorFuncionario = new SetorFuncionario();
    KoBindings(_setorFuncionario, "knockoutSetorFuncionario");

    HeaderAuditoria("Setor", _setorFuncionario);

    _CRUDSetorFuncionario = new CRUDSetorFuncionario();
    KoBindings(_CRUDSetorFuncionario, "knockoutCRUDSetorFuncionario");

    new BuscarTipoGrot(_setorFuncionario.TipoGrot);
    buscarSetorFuncionario();
   
}

function adicionarClick(e, sender) {
    Salvar(_setorFuncionario, "SetorFuncionario/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Pessoas.SetorFuncionario.Sucesso, Localization.Resources.Pessoas.SetorFuncionario.CadastradoSucesso);
                _gridSetorFuncionario.CarregarGrid();
                limparCamposSetorFuncionario();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.SetorFuncionario.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.SetorFuncionario.Falha, arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_setorFuncionario, "SetorFuncionario/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Pessoas.SetorFuncionario.Sucesso, Localization.Resources.Pessoas.SetorFuncionario.AtualizadoSucesso);
                _gridSetorFuncionario.CarregarGrid();
                limparCamposSetorFuncionario();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.SetorFuncionario.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.SetorFuncionario.Falha, arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Pessoas.SetorFuncionario.Confirmacao, Localization.Resources.Pessoas.SetorFuncionario.RealmenteDesejaExcluirEsseCadastro, function () {
        ExcluirPorCodigo(_setorFuncionario, "SetorFuncionario/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Pessoas.SetorFuncionario.Sucesso, Localization.Resources.Pessoas.SetorFuncionario.ExcluidoSucesso);
                    _gridSetorFuncionario.CarregarGrid();
                    limparCamposSetorFuncionario();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.SetorFuncionario.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.SetorFuncionario.Falha, arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e) {
    limparCamposSetorFuncionario();
}

function editarSetorFuncionarioClick(itemGrid) {
    limparCamposSetorFuncionario();

    _setorFuncionario.Codigo.val(itemGrid.Codigo);

    BuscarPorCodigo(_setorFuncionario, "SetorFuncionario/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaSetorFuncionario.ExibirFiltros.visibleFade(false);

                _CRUDSetorFuncionario.Atualizar.visible(true);
                _CRUDSetorFuncionario.Excluir.visible(true);
                _CRUDSetorFuncionario.Cancelar.visible(true);
                _CRUDSetorFuncionario.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.SetorFuncionario.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.SetorFuncionario.Falha, arg.Msg);
        }
    }, null);
}

//*******MÉTODOS*******

function buscarSetorFuncionario() {
    var editar = { descricao: Localization.Resources.Pessoas.SetorFuncionario.Editar, id: "clasEditar", evento: "onclick", metodo: editarSetorFuncionarioClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridSetorFuncionario = new GridView(_pesquisaSetorFuncionario.Pesquisar.idGrid, "SetorFuncionario/Pesquisa", _pesquisaSetorFuncionario, menuOpcoes, null);
    _gridSetorFuncionario.CarregarGrid();
}

function limparCamposSetorFuncionario() {
    _CRUDSetorFuncionario.Atualizar.visible(false);
    _CRUDSetorFuncionario.Cancelar.visible(false);
    _CRUDSetorFuncionario.Excluir.visible(false);
    _CRUDSetorFuncionario.Adicionar.visible(true);
    LimparCampos(_setorFuncionario);
}