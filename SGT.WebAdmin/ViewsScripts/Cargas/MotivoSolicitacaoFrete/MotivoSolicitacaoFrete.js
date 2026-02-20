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

var _motivoSolicitacaoFrete;
var _pesquisaMotivoSolicitacaoFrete;
var _gridMotivoSolicitacaoFrete;

var MotivoSolicitacaoFrete = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Propriedades
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", issue: 556, val: ko.observable(true), options: _status, def: true });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable("") });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaMotivoSolicitacaoFrete = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", issue: 557, val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMotivoSolicitacaoFrete.CarregarGrid();
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
function loadMotivoSolicitacaoFrete() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaMotivoSolicitacaoFrete = new PesquisaMotivoSolicitacaoFrete();
    KoBindings(_pesquisaMotivoSolicitacaoFrete, "knockoutPesquisaMotivoSolicitacaoFrete", false, _pesquisaMotivoSolicitacaoFrete.Pesquisar.id);

    // Instancia ProdutoAvaria
    _motivoSolicitacaoFrete = new MotivoSolicitacaoFrete();
    KoBindings(_motivoSolicitacaoFrete, "knockoutMotivoSolicitacaoFrete");

    HeaderAuditoria("MotivoSolicitacaoFrete", _motivoSolicitacaoFrete);

    // Inicia busca
    buscarMotivoSolicitacaoFrete();
}

function adicionarClick(e, sender) {
    Salvar(_motivoSolicitacaoFrete, "MotivoSolicitacaoFrete/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridMotivoSolicitacaoFrete.CarregarGrid();
                limparCamposMotivoSolicitacaoFrete();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoSolicitacaoFrete, "MotivoSolicitacaoFrete/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMotivoSolicitacaoFrete.CarregarGrid();
                limparCamposMotivoSolicitacaoFrete();
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
        ExcluirPorCodigo(_motivoSolicitacaoFrete, "MotivoSolicitacaoFrete/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMotivoSolicitacaoFrete.CarregarGrid();
                    limparCamposMotivoSolicitacaoFrete();
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
    limparCamposMotivoSolicitacaoFrete();
}

function editarMotivoSolicitacaoFreteClick(itemGrid) {
    // Limpa os campos
    limparCamposMotivoSolicitacaoFrete();

    // Seta o codigo do ProdutoAvaria
    _motivoSolicitacaoFrete.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_motivoSolicitacaoFrete, "MotivoSolicitacaoFrete/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaMotivoSolicitacaoFrete.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _motivoSolicitacaoFrete.Atualizar.visible(true);
                _motivoSolicitacaoFrete.Excluir.visible(true);
                _motivoSolicitacaoFrete.Cancelar.visible(true);
                _motivoSolicitacaoFrete.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarMotivoSolicitacaoFrete() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarMotivoSolicitacaoFreteClick, tamanho: "10", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };


    var configExportacao = {
        url: "MotivoSolicitacaoFrete/ExportarPesquisa",
        titulo: "Motivo Solicitação de Frete"
    };


    // Inicia Grid de busca
    _gridMotivoSolicitacaoFrete = new GridViewExportacao(_pesquisaMotivoSolicitacaoFrete.Pesquisar.idGrid, "MotivoSolicitacaoFrete/Pesquisa", _pesquisaMotivoSolicitacaoFrete, menuOpcoes, configExportacao);
    _gridMotivoSolicitacaoFrete.CarregarGrid();
}

function limparCamposMotivoSolicitacaoFrete() {
    _motivoSolicitacaoFrete.Atualizar.visible(false);
    _motivoSolicitacaoFrete.Cancelar.visible(false);
    _motivoSolicitacaoFrete.Excluir.visible(false);
    _motivoSolicitacaoFrete.Adicionar.visible(true);
    LimparCampos(_motivoSolicitacaoFrete);
}