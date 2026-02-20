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
/// <reference path="Load.js" />
/// <reference path="Navegacao.js" />
/// <reference path="Rua.js" />


//*******MAPEAMENTO KNOUCKOUT*******
var _deposito;
var _gridDeposito;
var _depositoSelecionado;

var Deposito = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 200 });
    this.CodigoIntegracao = PropertyEntity({ text: "*Código Integração: ", required: true, maxlength: 50 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Status: " });
    this.NumeroUnidadeImpressao = PropertyEntity({ text: "Número Unidade Impressão:", issue: 734, required: false, getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: '' }, val: ko.observable(0.00), maxlength: 10 });

    this.Depositos = PropertyEntity({ idGrid: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarDepositoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarDepositoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirDepositoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarDepositoClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });

    this.Etiqueta = PropertyEntity({ eventClick: gerarEtiquetaClick("ETIQUETA_DEPOSITO"), type: types.event, text: "Etiqueta", visible: ko.observable(false) });

};


//*******EVENTOS*******
function loadDeposito() {
    _deposito = new Deposito();
    KoBindings(_deposito, "knockoutDeposito");

    GridDepositos();
}

function gerarEtiquetaClick(tipoEtiqueta) {
    return (e, sender) => {
        const dataEnvio = { TipoEtiqueta: tipoEtiqueta, Codigo: e.Codigo.val() }

        executarReST("Deposito/ImprimirEtiqueta", dataEnvio, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde a geração do arquivo da impressão de etiqueta.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });
    }
}

function adicionarDepositoClick(e, sender) {
    Salvar(e, "Deposito/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                limparCamposDeposito();
                _gridDeposito.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarDepositoClick(e, sender) {
    Salvar(e, "Deposito/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                limparCamposDeposito();
                _gridDeposito.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirDepositoClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o depósito?", function () {
        ExcluirPorCodigo(e, "Deposito/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Msg != undefined && arg.Msg != null && arg.Msg != "") {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                } else {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    limparCamposDeposito();
                    _gridDeposito.CarregarGrid();
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarDepositoClick(e) {
    limparCamposDeposito();
}

//*******MÉTODOS*******
function GridDepositos() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarDeposito, tamanho: "15", icone: "" };
    var selecionar = { descricao: "Selecionar", id: guid(), evento: "onclick", metodo: selecionarDeposito, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        tamanho: 10,
        descricao: "Opções",
        opcoes: [editar, selecionar]
    };

    _gridDeposito = new GridView(_deposito.Depositos.idGrid, "Deposito/Pesquisa", _deposito, menuOpcoes);
    _gridDeposito.CarregarGrid();
}

function editarDeposito(objeto) {
    _deposito.Codigo.val(objeto.Codigo);
    _deposito.Descricao.val(objeto.Descricao);
    _deposito.CodigoIntegracao.val(objeto.CodigoIntegracao);
    _deposito.NumeroUnidadeImpressao.val(objeto.NumeroUnidadeImpressao);
    _deposito.Ativo.val(objeto.Status);

    _deposito.Atualizar.visible(true);
    _deposito.Excluir.visible(true);
    _deposito.Adicionar.visible(false);
    _deposito.Etiqueta.visible(true);
}

function selecionarDeposito(data) {
    SetDeposito(data);

    _rua.Deposito.text(data.Descricao);
    _rua.Deposito.val(data.Codigo);

    _bloco.Deposito.text(data.Descricao);
    _bloco.Deposito.val(data.Codigo);

    _posicao.Deposito.text(data.Descricao);
    _posicao.Deposito.val(data.Codigo);

    _gridRua.CarregarGrid(function () {
        AvancarEtapa();
    });
}

function limparCamposDeposito() {
    _deposito.Atualizar.visible(false);
    _deposito.Excluir.visible(false);
    _deposito.Adicionar.visible(true);
    _deposito.Etiqueta.visible(false);

    LimparCampos(_deposito);
}

function GetDeposito() {
    return _depositoSelecionado;
}

function SetDeposito(deposito) {
    _depositoSelecionado = deposito;

    if (deposito != null)
        Etapa2Liberada();
    else
        Etapa2Desativada();
}