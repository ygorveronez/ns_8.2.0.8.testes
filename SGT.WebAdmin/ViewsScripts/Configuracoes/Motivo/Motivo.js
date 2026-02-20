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


//*******MAPEAMENTO KNOUCKOUT*******

var _motivo;
var _pesquisaMotivo;
var _gridMotivo;

var Motivo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoMotivo.RejeicaoDadosNFeColeta), def: "", options: EnumTipoMotivo.obterOpcoesPesquisa(), text: ko.observable("Tipo Motivo: ") });

    this.Ativo = PropertyEntity({ text: "Situação: ", val: ko.observable(true), options: _status, def: true });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaMotivo = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Ativo = PropertyEntity({ text: "Status: ", issue: 557, val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMotivo.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

function loadMotivo() {
    _pesquisaMotivo = new PesquisaMotivo();
    KoBindings(_pesquisaMotivo, "knockoutPesquisaMotivo", false, _pesquisaMotivo.Pesquisar.id);

    _motivo = new Motivo();
    KoBindings(_motivo, "knockoutMotivo");


    buscarMotivo();
}

function adicionarClick(e, sender) {
    Salvar(_motivo, "Motivo/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridMotivo.CarregarGrid();
                limparCamposMotivo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivo, "Motivo/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMotivo.CarregarGrid();
                limparCamposMotivo();
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
        ExcluirPorCodigo(_motivo, "Motivo/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMotivo.CarregarGrid();
                    limparCamposMotivo();
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
    limparCamposMotivo();
}

function editarMotivoClick(itemGrid) {
    limparCamposMotivo();

    _motivo.Codigo.val(itemGrid.Codigo);

    BuscarPorCodigo(_motivo, "Motivo/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaMotivo.ExibirFiltros.visibleFade(false);

                _motivo.Atualizar.visible(true);
                _motivo.Excluir.visible(true);
                _motivo.Cancelar.visible(true);
                _motivo.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function buscarMotivo() {
    const editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarMotivoClick, tamanho: "10", icone: "" };

    const menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridMotivo = new GridView(_pesquisaMotivo.Pesquisar.idGrid, "Motivo/Pesquisa", _pesquisaMotivo, menuOpcoes);
    _gridMotivo.CarregarGrid();
}

function limparCamposMotivo() {
    _motivo.Atualizar.visible(false);
    _motivo.Cancelar.visible(false);
    _motivo.Excluir.visible(false);
    _motivo.Adicionar.visible(true);
    LimparCampos(_motivo);
}