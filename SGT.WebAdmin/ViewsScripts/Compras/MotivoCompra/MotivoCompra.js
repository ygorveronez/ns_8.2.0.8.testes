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
/// <reference path="../../Enumeradores/EnumFormaRequisicaoMercadoria.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _motivoCompra;
var _CRUDMotivoCompra;
var _pesquisaMotivoCompra;
var _gridMotivoCompra;

var MotivoCompra = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração: ", val: ko.observable(""), def: "", visible: ko.observable(true), maxlength: 100 });
    this.ExigeInformarVeiculoObrigatoriamente = PropertyEntity({ text: "Exige informar o veículo obrigatoriamente?", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable("") });
    
    this.Forma = PropertyEntity({ val: ko.observable(EnumFormaRequisicaoMercadoria.GerarPeloEstoque), options: EnumFormaRequisicaoMercadoria.obterOpcoes(), text: "Forma da Requisição: ", def: EnumFormaRequisicaoMercadoria.GerarPeloEstoque });

    this.GerarImpressaoOC = PropertyEntity({ text: "Gerar Impressão na O.C para requisição de exames", val: ko.observable(false), def: false, getType: typesKnockout.bool });
};

var CRUDMotivoCompra = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

var PesquisaMotivoCompra = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMotivoCompra.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

//*******EVENTOS*******

function loadMotivoCompra() {
    _pesquisaMotivoCompra = new PesquisaMotivoCompra();
    KoBindings(_pesquisaMotivoCompra, "knockoutPesquisaMotivoCompra", false, _pesquisaMotivoCompra.Pesquisar.id);

    _motivoCompra = new MotivoCompra();
    KoBindings(_motivoCompra, "knockoutMotivoCompra");

    HeaderAuditoria("MotivoCompra", _motivoCompra);

    _CRUDMotivoCompra = new CRUDMotivoCompra();
    KoBindings(_CRUDMotivoCompra, "knockoutCRUDMotivoCompra");

    buscarMotivoCompra();
}

function adicionarClick(e, sender) {
    Salvar(_motivoCompra, "MotivoCompra/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridMotivoCompra.CarregarGrid();
                limparCamposMotivoCompra();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoCompra, "MotivoCompra/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMotivoCompra.CarregarGrid();
                limparCamposMotivoCompra();
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
        ExcluirPorCodigo(_motivoCompra, "MotivoCompra/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMotivoCompra.CarregarGrid();
                    limparCamposMotivoCompra();
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
    limparCamposMotivoCompra();
}

function editarMotivoCompraClick(itemGrid) {
    // Limpa os campos
    limparCamposMotivoCompra();

    // Seta o codigo do objeto
    _motivoCompra.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_motivoCompra, "MotivoCompra/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaMotivoCompra.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _CRUDMotivoCompra.Atualizar.visible(true);
                _CRUDMotivoCompra.Excluir.visible(true);
                _CRUDMotivoCompra.Cancelar.visible(true);
                _CRUDMotivoCompra.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

//*******MÉTODOS*******

function buscarMotivoCompra() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarMotivoCompraClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridMotivoCompra = new GridView(_pesquisaMotivoCompra.Pesquisar.idGrid, "MotivoCompra/Pesquisa", _pesquisaMotivoCompra, menuOpcoes, null);
    _gridMotivoCompra.CarregarGrid();
}

function limparCamposMotivoCompra() {
    _CRUDMotivoCompra.Atualizar.visible(false);
    _CRUDMotivoCompra.Cancelar.visible(false);
    _CRUDMotivoCompra.Excluir.visible(false);
    _CRUDMotivoCompra.Adicionar.visible(true);
    LimparCampos(_motivoCompra);
}