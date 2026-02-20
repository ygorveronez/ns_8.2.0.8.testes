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
/// <reference path="../../Consultas/PlanoConta.js" />
/// <reference path="PlanoOrcamentario.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _contaPlanoOrcamentario;
var _gridContaPlanoOrcamentario;

var ContaPlanoOrcamentario = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoPlanoConta = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Percentual = PropertyEntity({ text: "*Percentual: ", required: true, getType: typesKnockout.decimal, enable: ko.observable(true), eventChange: PercentualCalcular });
    this.Valor = PropertyEntity({ text: "*Valor: ", required: true, getType: typesKnockout.decimal, enable: ko.observable(true), eventChange: ValorCalcular });
    this.PlanoConta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Plano Conta:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });

    this.Conta = PropertyEntity({ type: types.local });
    this.AdicionarConta = PropertyEntity({ eventClick: SalvarContaClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
    this.AtualizarConta = PropertyEntity({ eventClick: SalvarContaClick, type: types.event, text: "Atualizar", visible: ko.observable(false), enable: ko.observable(true) });
    this.ExcluirConta = PropertyEntity({ eventClick: ExcluirContaClick, type: types.event, text: "Excluir", visible: ko.observable(false), enable: ko.observable(true) });
    this.CancelarConta = PropertyEntity({ eventClick: limparCamposContaPlanoOrcamentario, type: types.event, text: "Cancelar", visible: ko.observable(false), enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadContaPlanoOrcamentario() {
    _contaPlanoOrcamentario = new ContaPlanoOrcamentario();
    KoBindings(_contaPlanoOrcamentario, "knockoutContaPlanoOrcamentario");

    new BuscarPlanoConta(_contaPlanoOrcamentario.PlanoConta, "Selecione a Conta Analítica", "Contas Analíticas", function (data) {
        _contaPlanoOrcamentario.PlanoConta.codEntity(data.Codigo);
        _contaPlanoOrcamentario.PlanoConta.val(data.Plano + " - " + data.Descricao);
        _contaPlanoOrcamentario.CodigoPlanoConta.val(data.Codigo);
    }, EnumAnaliticoSintetico.Analitico, null, "S");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: editarListaContaPlanoOrcamentarioClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoPlanoConta", visible: false },
        { data: "PlanoConta", title: "Plano de Contas", width: "70%" },
        { data: "Percentual", title: "Percentual", width: "10%" },
        { data: "Valor", title: "Valor", width: "10%" }
    ];

    _gridContaPlanoOrcamentario = new BasicDataTable(_contaPlanoOrcamentario.Conta.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    recarregarGridContaPlanoOrcamentario();
}

function recarregarGridContaPlanoOrcamentario() {
    var data = new Array();

    $.each(_planoOrcamentario.ListaConta.list, function (i, listaConta) {
        var listaContaGrid = new Object();

        listaContaGrid.Codigo = listaConta.Codigo.val;
        listaContaGrid.CodigoPlanoConta = listaConta.CodigoPlanoConta.val;
        listaContaGrid.PlanoConta = listaConta.PlanoConta.val;
        listaContaGrid.Percentual = listaConta.Percentual.val;
        listaContaGrid.Valor = listaConta.Valor.val;

        data.push(listaContaGrid);
    });

    _gridContaPlanoOrcamentario.CarregarGrid(data);
}

function SalvarContaClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_contaPlanoOrcamentario);

    if (Globalize.parseFloat(_contaPlanoOrcamentario.Percentual.val()) <= 0) {
        valido = false;
        _contaPlanoOrcamentario.Percentual.requiredClass("form-control is-invalid");
    }
    if (Globalize.parseFloat(_contaPlanoOrcamentario.Valor.val()) <= 0) {
        valido = false;
        _contaPlanoOrcamentario.Valor.requiredClass("form-control is-invalid");
    }

    if (valido) {
        if (_contaPlanoOrcamentario.Codigo.val() > "0" && _contaPlanoOrcamentario.Codigo.val() != undefined) {
            for (var i = 0; i < _planoOrcamentario.ListaConta.list.length; i++) {
                if (_contaPlanoOrcamentario.Codigo.val() == _planoOrcamentario.ListaConta.list[i].Codigo.val) {
                    _planoOrcamentario.ListaConta.list.splice(i, 1);
                    break;
                }
            }
        }

        var existe = false;
        $.each(_planoOrcamentario.ListaConta.list, function (i, listaConta) {
            if (listaConta.CodigoPlanoConta.val == _contaPlanoOrcamentario.CodigoPlanoConta.val()) {
                existe = true;
                return;
            }
        });

        if (existe) {
            exibirMensagem(tipoMensagem.aviso, "Plano de Contas já existente", "O plano " + _contaPlanoOrcamentario.PlanoConta.val() + " já está cadastrado.");
            return;
        }

        _contaPlanoOrcamentario.Codigo.val(guid());
        _planoOrcamentario.ListaConta.list.push(SalvarListEntity(_contaPlanoOrcamentario));
        recarregarGridContaPlanoOrcamentario();
        $("#" + _contaPlanoOrcamentario.PlanoConta.id).focus();
        limparCamposContaPlanoOrcamentario();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function ExcluirContaClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a conta " + data.PlanoConta.val() + "?", function () {
        $.each(_planoOrcamentario.ListaConta.list, function (i, listaConta) {
            if (data.Codigo.val() == listaConta.Codigo.val) {
                _planoOrcamentario.ListaConta.list.splice(i, 1);
                return false;
            }
        });

        recarregarGridContaPlanoOrcamentario();
        limparCamposContaPlanoOrcamentario();
    });
}

//*******MÉTODOS*******

function PercentualCalcular(e, sender) {
    if (Globalize.parseFloat(_contaPlanoOrcamentario.Percentual.val()) > 0 && Globalize.parseFloat(_planoOrcamentario.Valor.val()) > 0) {
        var valor = 0;
        valor = Globalize.parseFloat(_planoOrcamentario.Valor.val()) * (Globalize.parseFloat(_contaPlanoOrcamentario.Percentual.val()) / 100);
        _contaPlanoOrcamentario.Valor.val(Globalize.format(valor, "n2"));
    }
}

function ValorCalcular(e, sender) {
    if (Globalize.parseFloat(_contaPlanoOrcamentario.Valor.val()) > 0 && Globalize.parseFloat(_planoOrcamentario.Valor.val()) > 0) {
        var percentual = 0;
        percentual = (Globalize.parseFloat(_contaPlanoOrcamentario.Valor.val()) / Globalize.parseFloat(_planoOrcamentario.Valor.val())) * 100;
        _contaPlanoOrcamentario.Percentual.val(Globalize.format(percentual, "n2"));
    }
}

function editarListaContaPlanoOrcamentarioClick(data) {
    limparCamposContaPlanoOrcamentario();

    _contaPlanoOrcamentario.Codigo.val(data.Codigo);
    _contaPlanoOrcamentario.CodigoPlanoConta.val(data.CodigoPlanoConta);
    _contaPlanoOrcamentario.PlanoConta.codEntity(data.CodigoPlanoConta);
    _contaPlanoOrcamentario.PlanoConta.val(data.PlanoConta);
    _contaPlanoOrcamentario.Percentual.val(data.Percentual);
    _contaPlanoOrcamentario.Valor.val(data.Valor);

    _contaPlanoOrcamentario.AdicionarConta.visible(false);
    _contaPlanoOrcamentario.AtualizarConta.visible(true);
    _contaPlanoOrcamentario.ExcluirConta.visible(true);
    _contaPlanoOrcamentario.CancelarConta.visible(true);
}

function limparCamposContaPlanoOrcamentario() {
    //LimparCampos(_contaPlanoOrcamentario);
    LimparCampoEntity(_contaPlanoOrcamentario.PlanoConta);
    _contaPlanoOrcamentario.Codigo.val("0");
    _contaPlanoOrcamentario.CodigoPlanoConta.val("0");
    _contaPlanoOrcamentario.Percentual.val("0,00");
    _contaPlanoOrcamentario.Valor.val("0,00");

    _contaPlanoOrcamentario.AdicionarConta.visible(true);
    _contaPlanoOrcamentario.AtualizarConta.visible(false);
    _contaPlanoOrcamentario.ExcluirConta.visible(false);
    _contaPlanoOrcamentario.CancelarConta.visible(false);
}