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

var _motivoReajuste;
var _pesquisaMotivoReajuste;
var _gridMotivoReajuste;

var MotivoReajuste = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:",issue: 586, required: true, maxlength: 250 });
    this.Observacao = PropertyEntity({ text: "Observação:", issue: 593, maxlength: 2000 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "Situação: ", issue: 557 });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaMotivoReajuste = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMotivoReajuste.CarregarGrid();
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
function loadMotivoReajuste() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaMotivoReajuste = new PesquisaMotivoReajuste();
    KoBindings(_pesquisaMotivoReajuste, "knockoutPesquisaMotivoReajuste", false, _pesquisaMotivoReajuste.Pesquisar.id);

    // Instancia objeto principal
    _motivoReajuste = new MotivoReajuste();
    KoBindings(_motivoReajuste, "knockoutMotivoReajuste");

    // Inicia busca
    buscarMotivoReajuste();
}

function adicionarClick(e, sender) {
    Salvar(_motivoReajuste, "MotivoReajuste/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridMotivoReajuste.CarregarGrid();
                limparCamposMotivoReajuste();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoReajuste, "MotivoReajuste/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMotivoReajuste.CarregarGrid();
                limparCamposMotivoReajuste();
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
        ExcluirPorCodigo(_motivoReajuste, "MotivoReajuste/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMotivoReajuste.CarregarGrid();
                    limparCamposMotivoReajuste();
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
    limparCamposMotivoReajuste();
}

function editarMotivoReajusteClick(itemGrid) {
    // Limpa os campos
    limparCamposMotivoReajuste();

    // Seta o codigo do objeto
    _motivoReajuste.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_motivoReajuste, "MotivoReajuste/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaMotivoReajuste.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _motivoReajuste.Atualizar.visible(true);
                _motivoReajuste.Excluir.visible(true);
                _motivoReajuste.Cancelar.visible(true);
                _motivoReajuste.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarMotivoReajuste() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarMotivoReajusteClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridMotivoReajuste = new GridView(_pesquisaMotivoReajuste.Pesquisar.idGrid, "MotivoReajuste/Pesquisa", _pesquisaMotivoReajuste, menuOpcoes, null);
    _gridMotivoReajuste.CarregarGrid();
}

function limparCamposMotivoReajuste() {
    _motivoReajuste.Atualizar.visible(false);
    _motivoReajuste.Cancelar.visible(false);
    _motivoReajuste.Excluir.visible(false);
    _motivoReajuste.Adicionar.visible(true);
    LimparCampos(_motivoReajuste);
}