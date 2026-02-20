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

var _tipoContratoFrete;
var _pesquisaTipoContratoFrete;
var _gridTipoContratoFrete;

var TipoContratoFrete = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    
    // Propriedades
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });
    this.TipoAditivo = PropertyEntity({ text: "Esse tipo de contrato é do tipo aditivo?", getType: typesKnockout.bool, val: ko.observable(false), visible: _PermiteUsarContratoFreteAditivo });
    this.ContratoFreteAditivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Aditivo desse tipo de contrato:", idBtnSearch: guid(), visible: _PermiteUsarContratoFreteAditivo });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable("") });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaTipoContratoFrete = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Ativo = PropertyEntity({ text: "Status: ", issue: 557, val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoContratoFrete.CarregarGrid();
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
function loadTipoContratoFrete() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaTipoContratoFrete = new PesquisaTipoContratoFrete();
    KoBindings(_pesquisaTipoContratoFrete, "knockoutPesquisaTipoContratoFrete", false, _pesquisaTipoContratoFrete.Pesquisar.id);

    // Instancia ProdutoAvaria
    _tipoContratoFrete = new TipoContratoFrete();
    KoBindings(_tipoContratoFrete, "knockoutTipoContratoFrete");

    HeaderAuditoria("TipoContratoFrete", _tipoContratoFrete);

    // Inicia busca
    new BuscarTipoContratoFrete(_tipoContratoFrete.ContratoFreteAditivo, true);

    buscarTipoContratoFrete();
}

function adicionarClick(e, sender) {
    Salvar(_tipoContratoFrete, "TipoContratoFrete/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridTipoContratoFrete.CarregarGrid();
                limparCamposTipoContratoFrete();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_tipoContratoFrete, "TipoContratoFrete/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTipoContratoFrete.CarregarGrid();
                limparCamposTipoContratoFrete();
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
        ExcluirPorCodigo(_tipoContratoFrete, "TipoContratoFrete/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTipoContratoFrete.CarregarGrid();
                    limparCamposTipoContratoFrete();
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
    limparCamposTipoContratoFrete();
}

function editarTipoContratoFreteClick(itemGrid) {
    // Limpa os campos
    limparCamposTipoContratoFrete();

    // Seta o codigo do ProdutoAvaria
    _tipoContratoFrete.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_tipoContratoFrete, "TipoContratoFrete/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaTipoContratoFrete.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _tipoContratoFrete.Atualizar.visible(true);
                _tipoContratoFrete.Excluir.visible(true);
                _tipoContratoFrete.Cancelar.visible(true);
                _tipoContratoFrete.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarTipoContratoFrete() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTipoContratoFreteClick, tamanho: "10", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };


    var configExportacao = {
        url: "TipoContratoFrete/ExportarPesquisa",
        titulo: "Motivo Avaria"
    };


    // Inicia Grid de busca
    _gridTipoContratoFrete = new GridView(_pesquisaTipoContratoFrete.Pesquisar.idGrid, "TipoContratoFrete/Pesquisa", _pesquisaTipoContratoFrete, menuOpcoes);
    _gridTipoContratoFrete.CarregarGrid();
}

function limparCamposTipoContratoFrete() {
    _tipoContratoFrete.Atualizar.visible(false);
    _tipoContratoFrete.Cancelar.visible(false);
    _tipoContratoFrete.Excluir.visible(false);
    _tipoContratoFrete.Adicionar.visible(true);
    LimparCampos(_tipoContratoFrete);
}