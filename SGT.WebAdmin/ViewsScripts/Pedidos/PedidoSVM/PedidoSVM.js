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

var _pedidoSVM;
var _pesquisaPedidoSVM;
var _gridPedidoSVM;

var PedidoSVM = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NumeroPedido = PropertyEntity({ val: ko.observable(""), def: "", text: "Número do Pedido:", configInt: { precision: 0, allowZero: true }, getType: typesKnockout.int, visible: ko.observable(false), enable: ko.observable(false) });
    this.DataColeta = PropertyEntity({ text: "Data ", getType: typesKnockout.date, required: false, issue: 2, enable: ko.observable(false), val: ko.observable(""), def: ko.observable(Global.DataAtual()) });
    this.PedidoViagemNavio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: ko.observable("*Navio/Viagem/Direção:"), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TerminalOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: ko.observable("*Terminal de Origem:"), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TerminalDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: ko.observable("*Terminal de Destino:"), idBtnSearch: guid(), visible: ko.observable(true) });
    this.SomenteCargaPerigosa = PropertyEntity({ getType: typesKnockout.bool, text: "Gerar pedido somente com as cargas perigosas?", val: ko.observable(false), def: false, visible: true });

    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable("") });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaPedidoSVM = function () {
    this.NumeroPedido = PropertyEntity({ val: ko.observable(""), def: "", text: "Número do Pedido:", configInt: { precision: 0, allowZero: true }, getType: typesKnockout.int, visible: ko.observable(true) });
    this.PedidoViagemNavio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Navio/Viagem/Direção:"), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TerminalOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Terminal de Origem:"), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TerminalDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Terminal de Destino:"), idBtnSearch: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPedidoSVM.CarregarGrid();
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
function loadPedidoSVM() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaPedidoSVM = new PesquisaPedidoSVM();
    KoBindings(_pesquisaPedidoSVM, "knockoutPesquisaPedidoSVM", false, _pesquisaPedidoSVM.Pesquisar.id);

    new BuscarTipoTerminalImportacao(_pesquisaPedidoSVM.TerminalOrigem);
    new BuscarTipoTerminalImportacao(_pesquisaPedidoSVM.TerminalDestino);
    new BuscarPedidoViagemNavio(_pesquisaPedidoSVM.PedidoViagemNavio);


    // Instancia objeto principal
    _pedidoSVM = new PedidoSVM();
    KoBindings(_pedidoSVM, "knockoutPedidoSVM");

    new BuscarTipoTerminalImportacao(_pedidoSVM.TerminalOrigem);
    new BuscarTipoTerminalImportacao(_pedidoSVM.TerminalDestino);
    new BuscarPedidoViagemNavio(_pedidoSVM.PedidoViagemNavio);

    HeaderAuditoria("PedidoSVM", _pedidoSVM);

    // Inicia busca
    buscarPedidoSVM();
}

function adicionarClick(e, sender) {
    if ($("#alerta-erros").length > 0) {
        $("#alerta-erros").remove();
    }

    Salvar(_pedidoSVM, "PedidoSVM/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridPedidoSVM.CarregarGrid();
                limparCamposPedidoSVM();
            } else {
                $("#knockoutPedidoSVM").before('<p id="alerta-erros" class="alert alert-info alert-dismissible fade show"><button class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button><i class="fal fa-info-circle me-2"></i><strong>Atenção!</strong> Retorno da geração do SVM:<br/>' + arg.Msg.replace(/\n/g, "<br />") + '</p>');
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                _gridPedidoSVM.CarregarGrid();
                limparCamposPedidoSVM();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_pedidoSVM, "PedidoSVM/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridPedidoSVM.CarregarGrid();
                limparCamposPedidoSVM();
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
        ExcluirPorCodigo(_pedidoSVM, "PedidoSVM/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridPedidoSVM.CarregarGrid();
                    limparCamposPedidoSVM();
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
    limparCamposPedidoSVM();
}

function editarPedidoSVMClick(itemGrid) {
    // Limpa os campos
    limparCamposPedidoSVM();

    // Seta o codigo do objeto
    _pedidoSVM.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_pedidoSVM, "PedidoSVM/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaPedidoSVM.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _pedidoSVM.Atualizar.visible(true);
                _pedidoSVM.Excluir.visible(true);
                _pedidoSVM.Cancelar.visible(true);
                _pedidoSVM.Adicionar.visible(false);
                _pedidoSVM.NumeroPedido.visible(true);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

//*******MÉTODOS*******
function buscarPedidoSVM() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPedidoSVMClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridPedidoSVM = new GridView(_pesquisaPedidoSVM.Pesquisar.idGrid, "PedidoSVM/Pesquisa", _pesquisaPedidoSVM, menuOpcoes, null);
    _gridPedidoSVM.CarregarGrid();
}

function limparCamposPedidoSVM() {
    _pedidoSVM.Atualizar.visible(false);
    _pedidoSVM.Cancelar.visible(false);
    _pedidoSVM.Excluir.visible(false);
    _pedidoSVM.Adicionar.visible(true);
    _pedidoSVM.NumeroPedido.visible(false);
    LimparCampos(_pedidoSVM);
    _pedidoSVM.DataColeta.val(Global.DataAtual());
}