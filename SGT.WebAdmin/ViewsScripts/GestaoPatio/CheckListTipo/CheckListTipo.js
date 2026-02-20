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
/// <reference path="PerfilAcesso.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _checkListTipo;
var _pesquisaCheckListTipo;
var _gridCheckListTipo;

var CheckListTipo = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });
    this.PerfisAcesso = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0) });
    this.Clientes = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0) });
    this.EnviarEmailParaCliente = PropertyEntity({ text: "Enviar e-mail para o cliente ao responder", val: ko.observable(false) });
    this.EnviarEmailParaMotorista = PropertyEntity({ text: "Enviar e-mail para o motorista ao responder", val: ko.observable(false) });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaCheckListTipo = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCheckListTipo.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}


//*******EVENTOS*******
function loadCheckListTipo() {
    loadPerfilAcesso();
    loadCliente()
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaCheckListTipo = new PesquisaCheckListTipo();
    KoBindings(_pesquisaCheckListTipo, "knockoutPesquisaCheckListTipo", false, _pesquisaCheckListTipo.Pesquisar.id);

    // Instancia objeto principal
    _checkListTipo = new CheckListTipo();
    KoBindings(_checkListTipo, "knockoutCheckListTipo");

    HeaderAuditoria("CheckListTipo", _checkListTipo);



    // Inicia busca
    buscarCheckListTipo();
}

function adicionarClick(e, sender) {
    Salvar(_checkListTipo, "CheckListTipo/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridCheckListTipo.CarregarGrid();
                limparCamposCheckListTipo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_checkListTipo, "CheckListTipo/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridCheckListTipo.CarregarGrid();
                limparCamposCheckListTipo();
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
        ExcluirPorCodigo(_checkListTipo, "CheckListTipo/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridCheckListTipo.CarregarGrid();
                    limparCamposCheckListTipo();
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
    limparCamposCheckListTipo();
}

function editarCheckListTipoClick(itemGrid) {
    // Limpa os campos
    limparCamposCheckListTipo();

    // Seta o codigo do objeto
    _checkListTipo.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_checkListTipo, "CheckListTipo/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaCheckListTipo.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _checkListTipo.Atualizar.visible(true);
                _checkListTipo.Excluir.visible(true);
                _checkListTipo.Cancelar.visible(true);
                _checkListTipo.Adicionar.visible(false);

                // Carrega grid de perfis de acesso
                let gridArray = arg.Data.PerfisAcesso;
                _perfilAcesso.PerfilAcesso.basicTable.SetarRegistros(gridArray);
                _perfilAcesso.PerfilAcesso.basicTable.CarregarGrid(gridArray);

                let gridArrayCliete = arg.Data.Clientes;
                _cliente.Cliente.basicTable.SetarRegistros(gridArrayCliete);
                _cliente.Cliente.basicTable.CarregarGrid(gridArrayCliete);

            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

//*******MÉTODOS*******
function buscarCheckListTipo() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarCheckListTipoClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridCheckListTipo = new GridView(_pesquisaCheckListTipo.Pesquisar.idGrid, "CheckListTipo/Pesquisa", _pesquisaCheckListTipo, menuOpcoes, null);
    _gridCheckListTipo.CarregarGrid();
}

function limparCamposCheckListTipo() {
    _checkListTipo.Atualizar.visible(false);
    _checkListTipo.Cancelar.visible(false);
    _checkListTipo.Excluir.visible(false);
    _checkListTipo.Adicionar.visible(true);
    _perfilAcesso.PerfilAcesso.basicTable.SetarRegistros([]);
    _perfilAcesso.PerfilAcesso.basicTable.CarregarGrid([]);

    _cliente.Cliente.basicTable.SetarRegistros([]);
    _cliente.Cliente.basicTable.CarregarGrid([]);
    LimparCampos(_checkListTipo);
}