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
/// <reference path="../../Enumeradores/EnumAprovacaoRejeicao.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _motivoRejeicaoAuditoria;
var _pesquisaMotivoRejeicaoAuditoria;
var _CRUDMotivoRejeicaoAuditoria;
var _gridMotivoRejeicaoAuditoria;

var MotivoRejeicaoAuditoria = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "*Status: ", issue: 557, val: ko.observable(true), options: _status, def: true });
    this.Observacao = PropertyEntity({ text: "Observação:", issue: 593, getType: typesKnockout.string, val: ko.observable("") });
};

var CRUDMotivoRejeicaoAuditoria = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

var PesquisaMotivoRejeicaoAuditoria = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _statusPesquisa, def: true });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMotivoRejeicaoAuditoria.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function loadMotivoRejeicaoAuditoria() {

    _pesquisaMotivoRejeicaoAuditoria = new PesquisaMotivoRejeicaoAuditoria();
    KoBindings(_pesquisaMotivoRejeicaoAuditoria, "knockoutPesquisaMotivoRejeicaoAuditoria", false, _pesquisaMotivoRejeicaoAuditoria.Pesquisar.id);

    _motivoRejeicaoAuditoria = new MotivoRejeicaoAuditoria();
    KoBindings(_motivoRejeicaoAuditoria, "knockoutMotivoRejeicaoAuditoria");

    //HeaderAuditoria("MotivoRejeicaoAuditoria", _motivoRejeicaoAuditoria);

    _CRUDMotivoRejeicaoAuditoria = new CRUDMotivoRejeicaoAuditoria();
    KoBindings(_CRUDMotivoRejeicaoAuditoria, "knockoutCRUDMotivoRejeicaoAuditoria");

    BuscarMotivoRejeicaoAuditoria();
}

function adicionarClick(e, sender) {
    Salvar(_motivoRejeicaoAuditoria, "MotivoRejeicaoAuditoria/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridMotivoRejeicaoAuditoria.CarregarGrid();
                limparCamposMotivoRejeicaoAuditoria();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoRejeicaoAuditoria, "MotivoRejeicaoAuditoria/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMotivoRejeicaoAuditoria.CarregarGrid();
                limparCamposMotivoRejeicaoAuditoria();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_motivoRejeicaoAuditoria, "MotivoRejeicaoAuditoria/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMotivoRejeicaoAuditoria.CarregarGrid();
                    limparCamposMotivoRejeicaoAuditoria();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposMotivoRejeicaoAuditoria();
}

function editarMotivoRejeicaoAuditoriaClick(itemGrid) {
    limparCamposMotivoRejeicaoAuditoria();

    _motivoRejeicaoAuditoria.Codigo.val(itemGrid.Codigo);

    BuscarPorCodigo(_motivoRejeicaoAuditoria, "MotivoRejeicaoAuditoria/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaMotivoRejeicaoAuditoria.ExibirFiltros.visibleFade(false);

                _CRUDMotivoRejeicaoAuditoria.Atualizar.visible(true);
                _CRUDMotivoRejeicaoAuditoria.Excluir.visible(true);
                _CRUDMotivoRejeicaoAuditoria.Cancelar.visible(true);
                _CRUDMotivoRejeicaoAuditoria.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

//*******MÉTODOS*******

function BuscarMotivoRejeicaoAuditoria() {

    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarMotivoRejeicaoAuditoriaClick, tamanho: "10", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    var configExportacao = {
        url: "MotivoRejeicaoAuditoria/ExportarPesquisa",
        titulo: "Motivos de Rejeição de Ocorrência"
    };

    _gridMotivoRejeicaoAuditoria = new GridViewExportacao(_pesquisaMotivoRejeicaoAuditoria.Pesquisar.idGrid, "MotivoRejeicaoAuditoria/Pesquisa", _pesquisaMotivoRejeicaoAuditoria, menuOpcoes, configExportacao);
    _gridMotivoRejeicaoAuditoria.CarregarGrid();
}

function limparCamposMotivoRejeicaoAuditoria() {
    _CRUDMotivoRejeicaoAuditoria.Atualizar.visible(false);
    _CRUDMotivoRejeicaoAuditoria.Cancelar.visible(false);
    _CRUDMotivoRejeicaoAuditoria.Excluir.visible(false);
    _CRUDMotivoRejeicaoAuditoria.Adicionar.visible(true);
    LimparCampos(_motivoRejeicaoAuditoria);
}