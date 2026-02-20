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
/// <reference path="Load.js" />


//*******MAPEAMENTO KNOUCKOUT*******
var _posicao;
var _gridPosicao;

var Posicao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 200 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Status: " });
    this.QuantidadePaletsMaximo = PropertyEntity({ getType: typesKnockout.decimal, text: "Qtd. Palets Máx.:", required: false, visible: ko.observable(true) });
    this.MetroCubicoMaximo = PropertyEntity({ getType: typesKnockout.decimal, text: "M³ Máx.:", required: false, visible: ko.observable(true) });
    this.PesoMaximo = PropertyEntity({ getType: typesKnockout.decimal, text: "Peso Máx. (Kg):", required: false, visible: ko.observable(true) });
    this.ProdutoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto do Embarcador:", idBtnSearch: guid(), required: false, visible: ko.observable(true) });

    this.Deposito = PropertyEntity({ text: ko.observable(""), val: function () { return (GetDeposito() || {}).Codigo }, def: 0, getType: typesKnockout.int });
    this.Rua = PropertyEntity({ text: ko.observable(""), val: function () { return (GetRua() || {}).Codigo }, def: 0, getType: typesKnockout.int });
    this.Bloco = PropertyEntity({ text: ko.observable(""), val: function () { return (GetBloco() || {}).Codigo }, def: 0, getType: typesKnockout.int });

    this.Posicoes = PropertyEntity({ idGrid: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarPosicaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarPosicaoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirPosicaoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarPosicaoClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });

    this.Etiqueta = PropertyEntity({ eventClick: gerarEtiquetaPosicaoClick, type: types.event, text: "Etiqueta", visible: ko.observable(false) });
}


//*******EVENTOS*******
function loadPosicao() {
    _posicao = new Posicao();
    KoBindings(_posicao, "knockoutPosicao");

    new BuscarProdutos(_posicao.ProdutoEmbarcador);

    GridPosicoes();
}

function gerarEtiquetaPosicaoClick(posicao, sender) {
    var codigo = 0;
    if (posicao != null && posicao.Codigo != null)
        codigo = posicao.Codigo;
    else
        codigo = posicao.Codigo.val();

    const dataEnvio = { TipoEtiqueta: "ETIQUETA_POSICAO", Codigo: posicao.Codigo > 0 ? posicao.Codigo : posicao.Codigo.val() }
    executarReST("Deposito/ImprimirEtiqueta", dataEnvio, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde a geração do arquivo da impressão de etiqueta.");
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    });
}

function adicionarPosicaoClick(e, sender) {
    Salvar(e, "Posicao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                limparCamposPosicao();
                _gridPosicao.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarPosicaoClick(e, sender) {
    Salvar(e, "Posicao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                limparCamposPosicao();
                _gridPosicao.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirPosicaoClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o bloco?", function () {
        ExcluirPorCodigo(e, "Posicao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Msg != undefined && arg.Msg != null && arg.Msg != "") {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                } else {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    limparCamposPosicao();
                    _gridPosicao.CarregarGrid();
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarPosicaoClick(e) {
    limparCamposPosicao();
}

//*******MÉTODOS*******
function GridPosicoes() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPosicao, tamanho: "15", icone: "" };
    var etiqueta = { descricao: "Etiqueta", id: "clasEtiqueta", evento: "onclick", metodo: gerarEtiquetaPosicaoClick, tamanho: "15", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        tamanho: 5,
        descricao: "Opções",
        opcoes: [editar, etiqueta]
    };

    _gridPosicao = new GridView(_posicao.Posicoes.idGrid, "Posicao/Pesquisa", _posicao, menuOpcoes);
}

function editarPosicao(objeto) {
    limparCamposPosicao();
    _posicao.Codigo.val(objeto.Codigo);

    BuscarPorCodigo(_posicao, "Posicao/BuscarPorCodigo", function (arg) {
        _posicao.Atualizar.visible(true);
        _posicao.Excluir.visible(true);
        _posicao.Adicionar.visible(false);
        _posicao.Etiqueta.visible(true);

    }, null);
}

function limparCamposPosicao() {
    _posicao.Atualizar.visible(false);
    _posicao.Excluir.visible(false);
    _posicao.Adicionar.visible(true);
    _posicao.Etiqueta.visible(false);

    LimparCampos(_posicao);
}
