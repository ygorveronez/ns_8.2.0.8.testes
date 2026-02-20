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
/// <reference path="../../../js/Global/Auditoria.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Credito.js" />
/// <reference path="../ControleSaldo/ControleSaldo.js" />
/// <reference path="CreditoExtra.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _gridCreditoDisponivel;
var _creditoDisponivel;
var _CRUDcreditoDisponivel;
var _pesquisaCreditoDisponivel;

var PesquisaCreditoDisponivel = function () {
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid() });
    this.SomenteAtivos = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: "Buscar somente ativos? " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCreditoDisponivel.CarregarGrid();
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

var CreditoDisponivel = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required : true, text: "Operador:", idBtnSearch: guid() });
    this.DataInicioCredito = PropertyEntity({ text: "*Data inicio utilização Crédito: ", required: true, getType: typesKnockout.date });
    this.DataFimCredito = PropertyEntity({ text: "*Data limite para utilização do Crédito: ", dateRangeInit: this.DataInicioCredito, required: true, getType: typesKnockout.date });
    this.DataInicioCredito.dateRangeLimit = this.DataFimCredito;
    this.DataFimCredito.dateRangeInit = this.DataInicioCredito;
    this.CreditosUtilizados = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array() });
    this.ValorCredito = PropertyEntity({ text: "Valor do crédito: ", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });

}

var CRUDCreditoDisponivel = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadCreditoDisponivel() {

    _creditoDisponivel = new CreditoDisponivel();
    KoBindings(_creditoDisponivel, "knockoutCadastroCreditoDisponivel");

    _pesquisaCreditoDisponivel = new PesquisaCreditoDisponivel();
    KoBindings(_pesquisaCreditoDisponivel, "knockoutPesquisaCreditoDisponivel");

    _CRUDcreditoDisponivel = new CRUDCreditoDisponivel();
    KoBindings(_CRUDcreditoDisponivel, "knockoutCRUDCadastroCreditoDisponivel");

    HeaderAuditoria("CreditoDisponivel", _creditoDisponivel);

    new BuscarOperadoresAbaixoHierarquia(_creditoDisponivel.Recebedor);
    new BuscarOperadoresAbaixoHierarquia(_pesquisaCreditoDisponivel.Recebedor);
    buscarCreditoDisponivels();

    loadControleSaldo();
}

function adicionarClick(e, sender) {

    if (ValidarCamposObrigatorios(_creditoDisponivel)) {

        var creditoDisponibilizado = {
            Codigo: _creditoDisponivel.Codigo.val(),
            Recebedor: _creditoDisponivel.Recebedor.codEntity(),
            DataInicioCredito: _creditoDisponivel.DataInicioCredito.val(),
            DataFimCredito: _creditoDisponivel.DataFimCredito.val(),
            ValorCredito: _creditoDisponivel.ValorCredito.val(),
            CreditosUtilizados : new Array()
        };
        ValidarUtilizacaoSaldo(creditoDisponibilizado.ValorCredito, function (creditosUtilizados) {
            creditoDisponibilizado.CreditosUtilizados =  JSON.stringify(creditosUtilizados);
                executarReST("CreditoDisponivel/Adicionar", creditoDisponibilizado, function (arg) {
                    if (arg.Success) {
                        if (arg.Data) {
                            exibirMensagem(tipoMensagem.ok, "sucesso", "cadastrado");
                            _gridCreditoDisponivel.CarregarGrid();
                            limparCamposCreditoDisponivel();
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

function atualizarClick(e, sender) {

    if (ValidarCamposObrigatorios(_creditoDisponivel)) {
        var creditoDisponibilizado = {
            Codigo: _creditoDisponivel.Codigo.val(),
            Recebedor: _creditoDisponivel.Recebedor.codEntity(),
            DataInicioCredito: _creditoDisponivel.DataInicioCredito.val(),
            DataFimCredito: _creditoDisponivel.DataFimCredito.val(),
            ValorCredito: _creditoDisponivel.ValorCredito.val(),
            CreditosUtilizados: new Array()
        };
        ValidarUtilizacaoSaldo(creditoDisponibilizado.ValorCredito, function (creditosUtilizados) {
            creditoDisponibilizado.CreditosUtilizados = JSON.stringify(creditosUtilizados);
            executarReST("CreditoDisponivel/Atualizar", creditoDisponibilizado, function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        exibirMensagem(tipoMensagem.ok, "sucesso", "cadastrado");
                        _gridCreditoDisponivel.CarregarGrid();
                        limparCamposCreditoDisponivel();
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


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir?", function () {
        ExcluirPorCodigo(_creditoDisponivel, "CreditoDisponivel/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridCreditoDisponivel.CarregarGrid();
                    limparCamposCreditoDisponivel();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposCreditoDisponivel();
}

//*******MÉTODOS*******


function buscarCreditoDisponivels() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarCreditoDisponivel, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridCreditoDisponivel = new GridView(_pesquisaCreditoDisponivel.Pesquisar.idGrid, "CreditoDisponivel/Pesquisa", _pesquisaCreditoDisponivel, menuOpcoes, null);
    _gridCreditoDisponivel.CarregarGrid();
}

function editarCreditoDisponivel(creditoDisponivelGrid) {
    limparCamposCreditoDisponivel(function () {
        _creditoDisponivel.CreditosUtilizados.val(new Array());
        _creditoDisponivel.Codigo.val(creditoDisponivelGrid.Codigo);
        BuscarPorCodigo(_creditoDisponivel, "CreditoDisponivel/BuscarPorCodigo", function (arg) {
            _pesquisaCreditoDisponivel.ExibirFiltros.visibleFade(false);
            _CRUDcreditoDisponivel.Atualizar.visible(true);
            _CRUDcreditoDisponivel.Cancelar.visible(true);
            _CRUDcreditoDisponivel.Excluir.visible(true);
            _CRUDcreditoDisponivel.Adicionar.visible(false);

            var creditosUtilizados = new Array();
            $.each(_creditoDisponivel.CreditosUtilizados.val(), function (i, creditoUtilizado) {
                creditosUtilizados.push({ Codigo: creditoUtilizado.Codigo, Creditor: { Codigo: creditoUtilizado.Creditor.Codigo, Descricao: creditoUtilizado.Creditor.Nome }, ValorUtilizado: Globalize.parseFloat(creditoUtilizado.ValorUtilizado) })
            });
            SetarCreditosUtilizadosParaAtualizacao(creditosUtilizados);
        }, null);
    });
}

function limparCamposCreditoDisponivel(callback) {
    _CRUDcreditoDisponivel.Atualizar.visible(false);
    _CRUDcreditoDisponivel.Cancelar.visible(false);
    _CRUDcreditoDisponivel.Excluir.visible(false);
    _CRUDcreditoDisponivel.Adicionar.visible(true);
    _creditoDisponivel.CreditosUtilizados.val(new Array());
    LimparCampos(_creditoDisponivel);
    AtualizarDadosControleSaldo(callback);
}
