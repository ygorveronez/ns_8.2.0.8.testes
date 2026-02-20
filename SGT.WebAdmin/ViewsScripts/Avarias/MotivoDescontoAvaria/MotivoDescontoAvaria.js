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

var _motivoDescontoAvaria;
var _pesquisaMotivoDescontoAvaria;
var _gridMotivoDescontoAvaria;

var MotivoDescontoAvaria = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Propriedades
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ",issue: 556, val: ko.observable(true), options: _status, def: true });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable("") });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaMotivoDescontoAvaria = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", issue: 557, val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMotivoDescontoAvaria.CarregarGrid();
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
function loadMotivoDescontoAvaria() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaMotivoDescontoAvaria = new PesquisaMotivoDescontoAvaria();
    KoBindings(_pesquisaMotivoDescontoAvaria, "knockoutPesquisaMotivoDescontoAvaria", false, _pesquisaMotivoDescontoAvaria.Pesquisar.id);

    // Instancia ProdutoAvaria
    _motivoDescontoAvaria = new MotivoDescontoAvaria();
    KoBindings(_motivoDescontoAvaria, "knockoutMotivoDescontoAvaria");

    HeaderAuditoria("MotivoDescontoAvaria", _motivoDescontoAvaria);

    // Inicia busca
    buscarMotivoDescontoAvaria();
}

function adicionarClick(e, sender) {
    Salvar(_motivoDescontoAvaria, "MotivoDescontoAvaria/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridMotivoDescontoAvaria.CarregarGrid();
                limparCamposMotivoDescontoAvaria();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoDescontoAvaria, "MotivoDescontoAvaria/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMotivoDescontoAvaria.CarregarGrid();
                limparCamposMotivoDescontoAvaria();
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
        ExcluirPorCodigo(_motivoDescontoAvaria, "MotivoDescontoAvaria/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMotivoDescontoAvaria.CarregarGrid();
                    limparCamposMotivoDescontoAvaria();
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
    limparCamposMotivoDescontoAvaria();
}

function editarMotivoDescontoAvariaClick(itemGrid) {
    // Limpa os campos
    limparCamposMotivoDescontoAvaria();

    // Seta o codigo do ProdutoAvaria
    _motivoDescontoAvaria.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_motivoDescontoAvaria, "MotivoDescontoAvaria/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaMotivoDescontoAvaria.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _motivoDescontoAvaria.Atualizar.visible(true);
                _motivoDescontoAvaria.Excluir.visible(true);
                _motivoDescontoAvaria.Cancelar.visible(true);
                _motivoDescontoAvaria.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarMotivoDescontoAvaria() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarMotivoDescontoAvariaClick, tamanho: "10", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };


    var configExportacao = {
        url: "MotivoDescontoAvaria/ExportarPesquisa",
        titulo: "Motivo Avaria"
    };


    // Inicia Grid de busca
    _gridMotivoDescontoAvaria = new GridViewExportacao(_pesquisaMotivoDescontoAvaria.Pesquisar.idGrid, "MotivoDescontoAvaria/Pesquisa", _pesquisaMotivoDescontoAvaria, menuOpcoes, configExportacao);
    _gridMotivoDescontoAvaria.CarregarGrid();
}

function limparCamposMotivoDescontoAvaria() {
    _motivoDescontoAvaria.Atualizar.visible(false);
    _motivoDescontoAvaria.Cancelar.visible(false);
    _motivoDescontoAvaria.Excluir.visible(false);
    _motivoDescontoAvaria.Adicionar.visible(true);
    LimparCampos(_motivoDescontoAvaria);
}