/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="HorarioAcesso.js" />

var _turno;
var _pesquisaTurno;
var _gridTurno;

var Turno = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Filiais.Turno.Descricao.getRequiredFieldDescription(), issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Filiais.Turno.Observacao.getFieldDescription(), getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: Localization.Resources.Filiais.Turno.Status.getFieldDescription(), val: ko.observable(true), options: _status, def: true });

    this.HorariosAcessos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
};

var CRUDTurno = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Filiais.Turno.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Filiais.Turno.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Filiais.Turno.Cancelar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Filiais.Turno.Excluir, visible: ko.observable(false) });
};

var PesquisaTurno = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Filiais.Turno.Descricao.getRequiredFieldDescription(), issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: Localization.Resources.Filiais.Turno.Status.getFieldDescription(), issue: 557, val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: Localization.Resources.Filiais.Turno.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
    this.Pesquisar = PropertyEntity({ eventClick: function () { _gridTurno.CarregarGrid(); }, type: types.event, text: Localization.Resources.Filiais.Turno.Pesquisar, idGrid: guid(), visible: ko.observable(true) });
};

function loadTurno() {
    _pesquisaTurno = new PesquisaTurno();
    KoBindings(_pesquisaTurno, "knockoutPesquisaTurno", false, _pesquisaTurno.Pesquisar.id);

    _turno = new Turno();
    KoBindings(_turno, "knockoutTurno");

    _crudTurno = new CRUDTurno();
    KoBindings(_crudTurno, "knockoutCRUDTurno");

    HeaderAuditoria("Turno", _turno);

    loadHorarioAcesso();

    buscarTurno();
}

function adicionarClick(e, sender) {
    Salvar(_turno, "Turno/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Filiais.Turno.Sucesso, Localization.Resources.Filiais.Turno.CadastradoSucesso);
                _gridTurno.CarregarGrid();
                limparCamposTurno();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Filiais.Turno.Aviso, arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Filiais.Turno.Falha, arg.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_turno, "Turno/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Filiais.Turno.Sucesso, Localization.Resources.Filiais.Turno.AtualizadoSucesso);
                _gridTurno.CarregarGrid();
                limparCamposTurno();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Filiais.Turno.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Filiais.Turno.Falha, arg.Msg);
        }
    }, sender);
}

function buscarTurno() {
    var editar = { descricao: Localization.Resources.Filiais.Turno.Editar, id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    var configuracoesExportacao = {
        url: "Turno/ExportarPesquisa",
        titulo: "Turno"
    };

    _gridTurno = new GridViewExportacao(_pesquisaTurno.Pesquisar.idGrid, "Turno/Pesquisa", _pesquisaTurno, menuOpcoes, configuracoesExportacao);

    _gridTurno.CarregarGrid();
}

function cancelarClick(e) {
    limparCamposTurno();
}

function controlarBotoesHabilitados(isEdicao) {
    _crudTurno.Atualizar.visible(isEdicao);
    _crudTurno.Excluir.visible(isEdicao);
    _crudTurno.Cancelar.visible(isEdicao);
    _crudTurno.Adicionar.visible(!isEdicao);
}

function editarClick(registroSelecionado) {
    limparCamposTurno();

    _turno.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_turno, "Turno/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaTurno.ExibirFiltros.visibleFade(false);

                var isEdicao = true;

                controlarBotoesHabilitados(isEdicao);
                RecarregarGridHorarioAcesso();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Filiais.Turno.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Filiais.Turno.Falha, arg.Msg);
        }
    }, null);
}

function excluirClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Filiais.Turno.Confirmacao, Localization.Resources.Filiais.Turno.RealmenteDesejaExcluirCadastro, function () {
        ExcluirPorCodigo(_turno, "Turno/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Filiais.Turno.Sucesso, Localization.Resources.Filiais.Turno.ExcluidoSucesso);
                    _gridTurno.CarregarGrid();
                    limparCamposTurno();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Filiais.Turno.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Filiais.Turno.Falha, arg.Msg);
            }

        }, null);
    });
}

function limparCamposTurno() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);

    $("#tabTurno a:eq(0)").tab("show");

    LimparCampos(_turno);
    _turno.HorariosAcessos.list = new Array();

    LimparCamposHorarioAcesso();
    RecarregarGridHorarioAcesso();
}