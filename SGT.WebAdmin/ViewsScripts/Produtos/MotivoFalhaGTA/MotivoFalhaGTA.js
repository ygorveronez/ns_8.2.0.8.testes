/// <reference path="../../Consultas/PlanoConta.js" />
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

var _status = [
    { text: "Ativo", value: true },
    { text: "Inativo", value: false }
];

var _statusPesquisa = [
    { text: "Ativo", value: 1 },
    { text: "Inativo", value: 2 },
    { text: "Todos", value: 0 }
];

//*******MAPEAMENTO KNOUCKOUT*******

var _motivoFalhaGTA;
var _pesquisaMotivoFalhaGTA;
var _gridMotivoFalhaGTA;

var MotivoFalhaGTA = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Propriedades
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });
    this.ExigirFotoGTA = PropertyEntity({ text: "Exigir foto do GTA", val: ko.observable(false), getType: typesKnockout.bool, def: false });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaMotivoFalhaGTA = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMotivoFalhaGTA.CarregarGrid();
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


//*******EVENTOS*******
function loadMotivoFalhaGTA() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaMotivoFalhaGTA = new PesquisaMotivoFalhaGTA();
    KoBindings(_pesquisaMotivoFalhaGTA, "knockoutPesquisaMotivoFalhaGTA", false, _pesquisaMotivoFalhaGTA.Pesquisar.id);

    // Instancia MotivoFalhaGTA
    _motivoFalhaGTA = new MotivoFalhaGTA();
    KoBindings(_motivoFalhaGTA, "knockoutMotivoFalhaGTA");

    HeaderAuditoria("MotivoFalhaGTA", _motivoFalhaGTA);

    // Inicia busca
    buscarMotivoFalhaGTA();
}

function adicionarClick(e, sender) {
    Salvar(_motivoFalhaGTA, "MotivoFalhaGTA/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridMotivoFalhaGTA.CarregarGrid();
                limparCamposMotivoFalhaGTA();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoFalhaGTA, "MotivoFalhaGTA/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMotivoFalhaGTA.CarregarGrid();
                limparCamposMotivoFalhaGTA();
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
        ExcluirPorCodigo(_motivoFalhaGTA, "MotivoFalhaGTA/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMotivoFalhaGTA.CarregarGrid();
                    limparCamposMotivoFalhaGTA();
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
    limparCamposMotivoFalhaGTA();
}

function editarMotivoFalhaGTAClick(itemGrid) {
    // Limpa os campos
    limparCamposMotivoFalhaGTA();

    // Seta o codigo
    _motivoFalhaGTA.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_motivoFalhaGTA, "MotivoFalhaGTA/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaMotivoFalhaGTA.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _motivoFalhaGTA.Atualizar.visible(true);
                _motivoFalhaGTA.Excluir.visible(true);
                _motivoFalhaGTA.Cancelar.visible(true);
                _motivoFalhaGTA.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

//*******MÉTODOS*******
function buscarMotivoFalhaGTA() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarMotivoFalhaGTAClick, tamanho: "10", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    var configExportacao = {
        url: "MotivoFalhaGTA/ExportarPesquisa",
        titulo: "Motivo Falha GTA"
    };

    // Inicia Grid de busca
    _gridMotivoFalhaGTA = new GridViewExportacao(_pesquisaMotivoFalhaGTA.Pesquisar.idGrid, "MotivoFalhaGTA/Pesquisa", _pesquisaMotivoFalhaGTA, menuOpcoes, configExportacao);
    _gridMotivoFalhaGTA.CarregarGrid();
}

function limparCamposMotivoFalhaGTA() {
    _motivoFalhaGTA.Atualizar.visible(false);
    _motivoFalhaGTA.Cancelar.visible(false);
    _motivoFalhaGTA.Excluir.visible(false);
    _motivoFalhaGTA.Adicionar.visible(true);
    LimparCampos(_motivoFalhaGTA);
}