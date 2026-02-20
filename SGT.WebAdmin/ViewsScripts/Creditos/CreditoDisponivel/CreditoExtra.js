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
/// <reference path="../../Consultas/Credito.js" />
/// <reference path="../ControleSaldo/ControleSaldo.js" />
/// <reference path="CreditoExtra.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _gridCreditoExtra;
var _creditoExtra;
var _CRUDcreditoExtra;
var _pesquisaCreditoExtra;

var PesquisaCreditoExtra = function () {
    this.CreditoDisponivel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Credito Disponivel:", idBtnSearch: guid() });
}

var CreditoExtra = function () {
    this.CreditoDisponivel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Credito Disponivel:", idBtnSearch: guid() });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ValorCredito = PropertyEntity({ text: "Valor do crédito: ", required: true, getType: typesKnockout.decimal });

    this.CreditosUtilizados = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array() });
    this.ValorCredito = PropertyEntity({ text: "Valor do crédito: ", required: true, getType: typesKnockout.decimal });

    this.Adicionar = PropertyEntity({ eventClick: adicionarCreditoExtraClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarCreditoExtraClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirCreditoExtraClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarCreditoExtraClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadCreditoExtra() {

    _creditoExtra = new CreditoExtra();
    KoBindings(_creditoExtra, "knockoutCadastroCreditoExtra");

    _pesquisaCreditoExtra = new PesquisaCreditoExtra();
    KoBindings(_pesquisaCreditoExtra, "knockoutPesquisaCreditoExtra");

    _CRUDcreditoExtra = new CRUDCreditoExtra();
    KoBindings(_CRUDcreditoExtra, "knockoutCRUDCadastroCreditoExtra");

    new BuscarOperadoresAbaixoHierarquia(_creditoExtra.Recebedor);
    new BuscarOperadoresAbaixoHierarquia(_pesquisaCreditoExtra.Recebedor);
    buscarCreditoExtras();

    loadControleSaldo();
}

function adicionarCreditoExtraClick(e, sender) {

    if (ValidarCamposObrigatorios(e)) {

        var creditoDisponibilizado = {
            Codigo: e.Codigo.val(),
            Recebedor: e.Recebedor.codEntity(),
            DataInicioCredito: e.DataInicioCredito.val(),
            DataFimCredito: e.DataFimCredito.val(),
            ValorCredito: e.ValorCredito.val(),
            CreditosUtilizados: new Array()
        };
        ValidarUtilizacaoSaldo(creditoDisponibilizado.ValorCredito, function (creditosUtilizados) {
            creditoDisponibilizado.CreditosUtilizados = JSON.stringify(creditosUtilizados);
            executarReST("CreditoExtra/Adicionar", creditoDisponibilizado, function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        exibirMensagem(tipoMensagem.ok, "sucesso", "cadastrado");
                        _gridCreditoExtra.CarregarGrid();
                        limparCamposCreditoExtra();
                    } else {
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            }, sender);
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios");
    }
}

function atualizarCreditoExtraClick(e, sender) {

    if (ValidarCamposObrigatorios(e)) {
        var creditoDisponibilizado = {
            Codigo: e.Codigo.val(),
            Recebedor: e.Recebedor.codEntity(),
            DataInicioCredito: e.DataInicioCredito.val(),
            DataFimCredito: e.DataFimCredito.val(),
            ValorCredito: e.ValorCredito.val(),
            CreditosUtilizados: new Array()
        };
        ValidarUtilizacaoSaldo(creditoDisponibilizado.ValorCredito, function (creditosUtilizados) {
            creditoDisponibilizado.CreditosUtilizados = JSON.stringify(creditosUtilizados);
            executarReST("CreditoExtra/Atualizar", creditoDisponibilizado, function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        exibirMensagem(tipoMensagem.ok, "sucesso", "cadastrado");
                        _gridCreditoExtra.CarregarGrid();
                        limparCamposCreditoExtra();
                    } else {
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            }, sender);
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios");
    }
}


function excluirCreditoExtraClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir?", function () {
        ExcluirPorCodigo(_creditoExtra, "CreditoExtra/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridCreditoExtra.CarregarGrid();
                    limparCamposCreditoExtra();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarCreditoExtraClick(e) {
    limparCamposCreditoExtra();
}

//*******MÉTODOS*******


function buscarCreditoExtras() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarCreditoExtra, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridCreditoExtra = new GridView(_pesquisaCreditoExtra.Pesquisar.idGrid, "CreditoExtra/Pesquisa", _pesquisaCreditoExtra, menuOpcoes, null);
    _gridCreditoExtra.CarregarGrid();
}

function editarCreditoExtra(creditoExtraGrid) {
    limparCamposCreditoExtra(function () {
        _creditoExtra.CreditosUtilizados.list = new Array();
        _creditoExtra.Codigo.val(creditoExtraGrid.Codigo);
        BuscarPorCodigo(_creditoExtra, "CreditoExtra/BuscarPorCodigo", function (arg) {
            _pesquisaCreditoExtra.ExibirFiltros.visibleFade(false);
            _CRUDcreditoExtra.Atualizar.visible(true);
            _CRUDcreditoExtra.Cancelar.visible(true);
            _CRUDcreditoExtra.Excluir.visible(true);
            _CRUDcreditoExtra.Adicionar.visible(false);

            var creditosUtilizados = new Array();
            $.each(_creditoExtra.CreditosUtilizados.list, function (i, creditoUtilizado) {
                creditosUtilizados.push({ Codigo: creditoUtilizado.Codigo, Creditor: { Codigo: creditoUtilizado.Creditor.codEntity, Descricao: creditoUtilizado.Creditor.val }, ValorUtilizado: Globalize.parseFloat(creditoUtilizado.ValorUtilizado.val) })
            });
            SetarCreditosUtilizadosParaAtualizacao(creditosUtilizados);
        }, null);
    });
}

function limparCamposCreditoExtra(callback) {
    _CRUDcreditoExtra.Atualizar.visible(false);
    _CRUDcreditoExtra.Cancelar.visible(false);
    _CRUDcreditoExtra.Excluir.visible(false);
    _CRUDcreditoExtra.Adicionar.visible(true);
    _creditoExtra.CreditosUtilizados.val(new Array());
    LimparCampos(_creditoExtra);
    AtualizarDadosControleSaldo(callback);
}
