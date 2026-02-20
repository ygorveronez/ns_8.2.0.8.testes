/// <reference path="../../Consultas/PlanoConta.js" />
/// <reference path="../../Consultas/Filial.js" />
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


//*******MAPEAMENTO KNOUCKOUT*******

var _canalVenda;
var _pesquisaCanalVenda;
var _gridCanalVenda;


var CanalVenda = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: Localization.Resources.Pedidos.CanalVenda.Descricao.getFieldDescription(), issue: 586, required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 200 });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.CanalVenda.Filial.getFieldDescription(), idBtnSearch: guid() });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Pedidos.CanalVenda.CodigoIntegracao.getFieldDescription(), issue: 15, getType: typesKnockout.string, val: ko.observable(""), maxlength: 50 });
    this.NivelPrioridade = PropertyEntity({ text: Localization.Resources.Pedidos.CanalVenda.NivelPrioridade.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(""), maxlength: 50 });
    this.Ativo = PropertyEntity({ text: Localization.Resources.Pedidos.CanalVenda.Ativo.getFieldDescription(), val: ko.observable(true), issue: 557, options: _status, def: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Pedidos.CanalVenda.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Pedidos.CanalVenda.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Pedidos.CanalVenda.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Pedidos.CanalVenda.Cancelar, visible: ko.observable(false) });
}

var PesquisaCanalVenda = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Pedidos.CanalVenda.Descricao.getFieldDescription(), issue: 586, required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 200 });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Pedidos.CanalVenda.CodigoIntegracao.getFieldDescription(), issue: 15, getType: typesKnockout.string, val: ko.observable(""), maxlength: 50 });
    this.Ativo = PropertyEntity({ text: Localization.Resources.Pedidos.CanalVenda.Ativo.getFieldDescription(), issue: 557, val: ko.observable(1), options: _statusPesquisa, def: 1 });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.CanalVenda.Filial.getFieldDescription(), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCanalVenda.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Pedidos.CanalVenda.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Pedidos.CanalVenda.ExibirFiltros, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}


//*******EVENTOS*******
function loadCanalVenda() {
    _pesquisaCanalVenda = new PesquisaCanalVenda();
    KoBindings(_pesquisaCanalVenda, "knockoutPesquisaCanalVenda", false, _pesquisaCanalVenda.Pesquisar.id);

    new BuscarFilial(_pesquisaCanalVenda.Filial);

    _canalVenda = new CanalVenda();
    KoBindings(_canalVenda, "knockoutCanalVenda");

    HeaderAuditoria("CanalVenda", _canalVenda);

    new BuscarFilial(_canalVenda.Filial);

    buscarCanalVenda();
}
function adicionarClick(e, sender) {
    Salvar(_canalVenda, "CanalVenda/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridCanalVenda.CarregarGrid();
                limparCamposCanalVenda();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_canalVenda, "CanalVenda/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridCanalVenda.CarregarGrid();
                limparCamposCanalVenda();
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
        ExcluirPorCodigo(_canalVenda, "CanalVenda/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridCanalVenda.CarregarGrid();
                    limparCamposCanalVenda();
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
    limparCamposCanalVenda();
}

function editarCanalVendaClick(itemGrid) {
    limparCamposCanalVenda();

    _canalVenda.Codigo.val(itemGrid.Codigo);

    BuscarPorCodigo(_canalVenda, "CanalVenda/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaCanalVenda.ExibirFiltros.visibleFade(false);

                _canalVenda.Atualizar.visible(true);
                _canalVenda.Excluir.visible(true);
                _canalVenda.Cancelar.visible(true);
                _canalVenda.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarCanalVenda() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarCanalVendaClick, tamanho: "10", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    var configExportacao = {
        url: "CanalVenda/ExportarPesquisa",
        titulo: "Motivo Avaria"
    };

    _gridCanalVenda = new GridViewExportacao(_pesquisaCanalVenda.Pesquisar.idGrid, "CanalVenda/Pesquisa", _pesquisaCanalVenda, menuOpcoes, configExportacao);
    _gridCanalVenda.CarregarGrid();
}

function limparCamposCanalVenda() {
    _canalVenda.Atualizar.visible(false);
    _canalVenda.Cancelar.visible(false);
    _canalVenda.Excluir.visible(false);
    _canalVenda.Adicionar.visible(true);
    LimparCampos(_canalVenda);
}