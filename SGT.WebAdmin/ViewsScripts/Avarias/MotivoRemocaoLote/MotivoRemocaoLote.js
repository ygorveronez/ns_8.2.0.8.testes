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

var _motivoRemocaoLote;
var _pesquisaMotivoRemocaoLote;
var _gridMotivoRemocaoLote;

var MotivoRemocaoLote = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Propriedades
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable("") });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaMotivoRemocaoLote = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMotivoRemocaoLote.CarregarGrid();
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
function loadMotivoRemocaoLote() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaMotivoRemocaoLote = new PesquisaMotivoRemocaoLote();
    KoBindings(_pesquisaMotivoRemocaoLote, "knockoutPesquisaMotivoRemocaoLote", false, _pesquisaMotivoRemocaoLote.Pesquisar.id);

    // Instancia ProdutoAvaria
    _motivoRemocaoLote = new MotivoRemocaoLote();
    KoBindings(_motivoRemocaoLote, "knockoutMotivoRemocaoLote");

    HeaderAuditoria("MotivoRemocaoLote", _motivoRemocaoLote);

    // Inicia busca
    buscarMotivoRemocaoLote();
}

function adicionarClick(e, sender) {
    Salvar(_motivoRemocaoLote, "MotivoRemocaoLote/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridMotivoRemocaoLote.CarregarGrid();
                limparCamposMotivoRemocaoLote();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoRemocaoLote, "MotivoRemocaoLote/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMotivoRemocaoLote.CarregarGrid();
                limparCamposMotivoRemocaoLote();
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
        ExcluirPorCodigo(_motivoRemocaoLote, "MotivoRemocaoLote/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMotivoRemocaoLote.CarregarGrid();
                    limparCamposMotivoRemocaoLote();
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
    limparCamposMotivoRemocaoLote();
}

function editarMotivoRemocaoLoteClick(itemGrid) {
    // Limpa os campos
    limparCamposMotivoRemocaoLote();

    // Seta o codigo do ProdutoAvaria
    _motivoRemocaoLote.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_motivoRemocaoLote, "MotivoRemocaoLote/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaMotivoRemocaoLote.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _motivoRemocaoLote.Atualizar.visible(true);
                _motivoRemocaoLote.Excluir.visible(true);
                _motivoRemocaoLote.Cancelar.visible(true);
                _motivoRemocaoLote.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarMotivoRemocaoLote() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarMotivoRemocaoLoteClick, tamanho: "10", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };


    var configExportacao = {
        url: "MotivoRemocaoLote/ExportarPesquisa",
        titulo: "Motivo Avaria"
    };


    // Inicia Grid de busca
    _gridMotivoRemocaoLote = new GridViewExportacao(_pesquisaMotivoRemocaoLote.Pesquisar.idGrid, "MotivoRemocaoLote/Pesquisa", _pesquisaMotivoRemocaoLote, menuOpcoes, configExportacao);
    _gridMotivoRemocaoLote.CarregarGrid();
}

function limparCamposMotivoRemocaoLote() {
    _motivoRemocaoLote.Atualizar.visible(false);
    _motivoRemocaoLote.Cancelar.visible(false);
    _motivoRemocaoLote.Excluir.visible(false);
    _motivoRemocaoLote.Adicionar.visible(true);
    LimparCampos(_motivoRemocaoLote);
}